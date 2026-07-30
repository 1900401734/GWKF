using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using MesDatas.DataAcess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MesDatas.Services
{
    /// <summary>
    /// 负责 PLC 连接建立、断线重连和 PLC 心跳存活判断。
    /// </summary>
    public class PlcConnectionManager
    {
        private const int MonitorIntervalMs = 500;           // PLC连接管理循环间隔，单位：毫秒
        private const int PlcHeartbeatTimeoutMs = 10000;     // PLC心跳超过该时间未变化后判定为异常
        private const int HeartbeatReadTimeoutMs = 1000;     // 单次读取PLC心跳的等待时间

        private dynamic _plcConnectObject;

        private readonly PlcAddressInfo _addressInfo;

        public bool IsConnected { get; private set; }

        public IReadWriteNet ReadWriteNet { get; private set; }

        public event Action<bool, string> OnConnectionStatusChanged;

        public event Action<bool, string> OnHeartbeatStatusChanged;

        public PlcConnectionManager(PlcAddressInfo addressInfo)
        {
            _addressInfo = addressInfo;
        }

        /// <summary>
        /// 启动连接和心跳管理的后台任务
        /// <para>1. 如果未连接，循环尝试连接。</para>
        /// <para>2. 如果已连接，只读取PLC心跳业务信号。</para>
        /// <para>3. PLC心跳超过10秒未变化时，只触发心跳告警，不切断PLC通讯状态。</para>
        /// </summary>
        public async Task StartConnectionTaskAsync(string ip, int port, string connectType, CancellationToken token)
        {
            short? lastHeartbeatValue = null;                       // 最近一次读到的PLC心跳值
            var lastHeartbeatChangeTime = DateTime.UtcNow;          // 最近一次确认PLC心跳变化的时间
            var consecutiveReadFailCount = 0;                       // 连续读取PLC心跳失败的次数
            var isHeartbeatAlive = true;                            // 心跳告警状态，避免异常日志持续刷屏

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(MonitorIntervalMs, token);

                    // 未连接时只负责重连，连接成功后从当前时间重新开始观察心跳。
                    if (!IsConnected)
                    {
                        if (await TryConnectPlcAsync(ip, port, connectType))
                        {
                            ResetHeartbeatWatchdog(ref lastHeartbeatValue, ref lastHeartbeatChangeTime);
                            consecutiveReadFailCount = 0;
                            isHeartbeatAlive = true;
                        }

                        continue;
                    }

                    // 安全检查，避免读写PLC时抛出NullReferenceException
                    if (ReadWriteNet == null)
                    {
                        UpdateConnectionStatus(false, "[PLC连接管理器] 连接对象为null");
                        ResetHeartbeatWatchdog(ref lastHeartbeatValue, ref lastHeartbeatChangeTime);
                        consecutiveReadFailCount = 0;
                        isHeartbeatAlive = true;
                        continue;
                    }

                    // 连接管理器不写PC心跳，只读取PLC心跳业务信号判断PLC是否存活。
                    var plcHeartbeatValue = await TryReadPlcHeartbeatAsync();
                    if (plcHeartbeatValue.HasValue)
                    {
                        consecutiveReadFailCount = 0;

                        var hasHeartbeatChanged = RefreshHeartbeatWatchdog(plcHeartbeatValue.Value, ref lastHeartbeatValue, ref lastHeartbeatChangeTime);
                        if (hasHeartbeatChanged && !isHeartbeatAlive)
                        {
                            var recoveryMessage = BuildHeartbeatStatusMessage(true, lastHeartbeatValue, lastHeartbeatChangeTime, consecutiveReadFailCount);
                            UpdateHeartbeatStatus(true, recoveryMessage);
                            isHeartbeatAlive = true;
                        }
                    }
                    else
                    {
                        consecutiveReadFailCount++;
                    }

                    // 心跳超时只代表业务心跳异常，不再直接切断PLC通讯连接状态。
                    if (IsPlcHeartbeatTimeout(lastHeartbeatChangeTime) && isHeartbeatAlive)
                    {
                        var timeoutMessage = BuildHeartbeatStatusMessage(false, lastHeartbeatValue, lastHeartbeatChangeTime, consecutiveReadFailCount);
                        UpdateHeartbeatStatus(false, timeoutMessage);
                        isHeartbeatAlive = false;
                    }
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch
                {
                    UpdateConnectionStatus(false, "[PLC连接管理器] PLC连接监控异常，已切换为未连接");
                    ResetHeartbeatWatchdog(ref lastHeartbeatValue, ref lastHeartbeatChangeTime);
                    consecutiveReadFailCount = 0;
                    isHeartbeatAlive = true;
                }
            }
        }

        /// <summary>
        /// 尝试建立连接
        /// </summary>
        public async Task<bool> TryConnectPlcAsync(string ipAddress, int port, string connectionMethod)
        {
            Close(); // 先清理旧连接

            try
            {
                NetworkDeviceBase networkDeviceBase;

                switch (connectionMethod)
                {
                    case "TCP":
                        var omronFinsNet = new OmronFinsNet(ipAddress, port);
                        omronFinsNet.ConnectTimeOut = 1000;
                        omronFinsNet.DA2 = 0;
                        omronFinsNet.ByteTransform.DataFormat = DataFormat.CDAB; // 原代码是 (DataFormat)2
                        networkDeviceBase = omronFinsNet;
                        break;
                    case "UDP":
                        var omronFinsUdp = new OmronFinsUdp(ipAddress, port)
                        {
                            SA1 = 192,
                            ReceiveTimeout = 1000,
                            ByteTransform = { DataFormat = DataFormat.CDAB }
                        };
                        ReadWriteNet = omronFinsUdp;
                        _plcConnectObject = omronFinsUdp;
                        UpdateConnectionStatus(true);
                        return true; // UDP 不需要 ConnectServerAsync
                    case "MC":
                        networkDeviceBase = new MelsecMcNet(ipAddress, port);
                        break;
                    case "Modbus":
                        networkDeviceBase = new ModbusTcpNet(ipAddress, port);
                        break;
                    default:
                        return false;
                }

                networkDeviceBase.ReceiveTimeOut = 3000;
                networkDeviceBase.ConnectTimeOut = 3000;

                var connect = await networkDeviceBase.ConnectServerAsync();
                if (connect.IsSuccess)
                {
                    ReadWriteNet = networkDeviceBase;
                    _plcConnectObject = networkDeviceBase;
                    UpdateConnectionStatus(true);
                    return true;
                }
                UpdateConnectionStatus(false);
                return false;
            }
            catch
            {
                UpdateConnectionStatus(false);
                return false;
            }
        }

        /// <summary>
        /// 读取PLC心跳信号；读取失败或超时都返回null，避免一次网络波动就误判断线。
        /// </summary>
        private async Task<short?> TryReadPlcHeartbeatAsync()
        {
            try
            {
                var readTask = ReadWriteNet.ReadInt16Async(_addressInfo.PlcHeartBeat);
                var completedTask = await Task.WhenAny(readTask, Task.Delay(HeartbeatReadTimeoutMs));

                if (completedTask != readTask)
                {
                    return null;
                }

                var readResult = await readTask;
                if (!readResult.IsSuccess)
                {
                    return null;
                }

                return readResult.Content;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 重置PLC心跳看门狗，从当前时间重新开始等待PLC心跳变化。
        /// </summary>
        private static void ResetHeartbeatWatchdog(ref short? lastHeartbeatValue, ref DateTime lastHeartbeatChangeTime)
        {
            lastHeartbeatValue = null;
            lastHeartbeatChangeTime = DateTime.UtcNow;
        }

        /// <summary>
        /// PLC心跳值发生变化时刷新看门狗时间，值不变时保持原时间用于超时判断。
        /// </summary>
        private static bool RefreshHeartbeatWatchdog(short plcHeartbeatValue, ref short? lastHeartbeatValue, ref DateTime lastHeartbeatChangeTime)
        {
            if (!lastHeartbeatValue.HasValue || plcHeartbeatValue != lastHeartbeatValue.Value)
            {
                lastHeartbeatValue = plcHeartbeatValue;
                lastHeartbeatChangeTime = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断PLC心跳是否已经超过允许时间未发生变化。
        /// </summary>
        private static bool IsPlcHeartbeatTimeout(DateTime lastHeartbeatChangeTime)
        {
            return (DateTime.UtcNow - lastHeartbeatChangeTime).TotalMilliseconds >= PlcHeartbeatTimeoutMs;
        }

        /// <summary>
        /// 构建心跳异常或恢复日志，便于现场确认地址、最近值和失败次数。
        /// </summary>
        private string BuildHeartbeatStatusMessage(bool isAlive, short? lastHeartbeatValue, DateTime lastHeartbeatChangeTime, int consecutiveReadFailCount)
        {
            var statusText = isAlive ? "PLC心跳恢复" : "PLC心跳超过10秒未变化";
            var lastValueText = lastHeartbeatValue.HasValue ? lastHeartbeatValue.Value.ToString() : "无";
            var unchangedSeconds = (int)(DateTime.UtcNow - lastHeartbeatChangeTime).TotalSeconds;

            return $"[PLC连接管理器] {statusText}，地址: {_addressInfo.PlcHeartBeat}，最近值: {lastValueText}，未变化时长: {unchangedSeconds}s，连续读取失败: {consecutiveReadFailCount}次";
        }

        /// <summary>
        /// 更新PLC业务心跳状态，只负责提示，不改变PLC通讯连接状态。
        /// </summary>
        private void UpdateHeartbeatStatus(bool isAlive, string msg)
        {
            OnHeartbeatStatusChanged?.Invoke(isAlive, msg);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                _plcConnectObject?.ConnectClose();
            }
            catch {/* ignored */ }
            _plcConnectObject = null;
            // ReadWriteNet = null; // 可选：视情况是否置空，置空可能导致其他线程报错
        }

        /// <summary>
        /// 更新连接状态并触发事件
        /// </summary>
        /// <param name="status">PLC连接状态</param>
        private void UpdateConnectionStatus(bool status, string msg = "")
        {
            if (IsConnected != status)
            {
                IsConnected = status;
                OnConnectionStatusChanged?.Invoke(status, msg);
            }
        }
    }
}
