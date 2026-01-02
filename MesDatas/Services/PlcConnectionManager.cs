using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using MesDatas.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MesDatas.Services
{
    public class PlcConnectionManager
    {
        // PLC连接对象
        public IReadWriteNet ReadWriteNet { get; private set; }

        // PLC连接状态
        public bool IsConnected { get; private set; }

        // 事件：当连接状态改变时触发 (true: 连接成功, false: 断开)
        public event Action<bool> OnConnectionStatusChanged;

        private dynamic _plcConnectObject; // 保持引用的具体对象用于关闭

        private readonly PlcAddressInfo _addressInfo;

        public PlcConnectionManager(PlcAddressInfo addressInfo)
        {
            _addressInfo = addressInfo;
        }

        /// <summary>
        /// 启动连接和心跳管理的后台任务
        /// <summary>
        /// <para>1. 如果未连接，循环尝试连接。</para>
        /// <para>2. 如果已连接，执行双向心跳检查。</para>
        /// 3. 任何读/写失败、异常或“卡住”都会认为断线，并返回步骤1。
        /// </summary>
        /// </summary>
        public async Task StartConnectionTaskAsync(string ip, int port, string connectType, CancellationToken token)
        {
            var failCount = 0;              // PLC心跳“卡住”的计数器
            var lastReadValue = -1;         // PLC心跳上一次的值
            var lastWriteValue = 0;         // PC心跳上一次的值
            const int failCountMax = 100;   // 最大失败次数阈值

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // --- 步骤 1: 如果未连接 ---
                    if (!IsConnected)
                    {
                        if (await TryConnectPlcAsync(ip, port, connectType))
                        {
                            UpdateConnectionStatus(true);
                            failCount = 0;
                            lastReadValue = -1;
                            lastWriteValue = 0;
                        }
                        else
                        {
                            UpdateConnectionStatus(false);
                            await Task.Delay(1000, token);
                            continue;
                        }
                    }

                    // --- 步骤 2: 如果已连接，执行双向心跳检查 ---
                    if (ReadWriteNet == null)
                    {
                        UpdateConnectionStatus(false);
                        continue;
                    }

                    // 2a. 写入PC心跳（带超时500ms）
                    var valueToWrite = (short)(lastWriteValue == 1 ? 0 : 1);
                    var writeTask = ReadWriteNet.WriteAsync(_addressInfo.PcHeartBeat, valueToWrite);
                    var completedTask = await Task.WhenAny(writeTask, Task.Delay(500, token));

                    if (writeTask != completedTask) // 超时
                    {
                        UpdateConnectionStatus(false);
                        continue;
                    }

                    var writeResult = await writeTask;
                    if (!writeResult.IsSuccess)
                    {
                        UpdateConnectionStatus(false);
                        continue;
                    }

                    lastWriteValue = valueToWrite;

                    // 2b. 读取PLC心跳
                    // 注意：这里简单实现读取，如果需要原来的重试机制，可以封装一个私有方法
                    var readRes = await ReadWriteNet.ReadInt16Async(_addressInfo.PlcHeartBeat);
                    if (!readRes.IsSuccess)
                    {
                        UpdateConnectionStatus(false);
                        continue;
                    }
                    var plcHeartValue = readRes.Content;

                    // 2c. 检查PLC心跳是否"卡住"
                    if (plcHeartValue == lastReadValue && lastReadValue != -1)
                    {
                        failCount++;
                    }
                    else
                    {
                        failCount = 0;
                        lastReadValue = plcHeartValue;
                    }

                    if (failCount >= failCountMax)
                    {
                        UpdateConnectionStatus(false);
                        continue;
                    }
                }
                catch
                {
                    UpdateConnectionStatus(false);
                }

                await Task.Delay(1000, token);
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
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
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
        private void UpdateConnectionStatus(bool status)
        {
            if (IsConnected != status)
            {
                IsConnected = status;
                OnConnectionStatusChanged?.Invoke(status);
            }
        }
    }
}