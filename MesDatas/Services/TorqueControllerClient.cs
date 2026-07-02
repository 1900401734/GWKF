using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MesDatas.Services
{
    public class TorqueData
    {
        public string RawData { get; set; }         // 原始报文
        public bool TighteningStatus { get; set; }  // 拧紧状态 (1=OK；0=NG)
        public string TorqueMin { get; set; }       // 扭力下限
        public string TorqueMax { get; set; }       // 扭力上限
        public string Torque { get; set; }          // 实际扭力
        public string TimeStamp { get; set; }       // 时间戳
    }

    public class TorqueControllerClient
    {
        private const int InitialReconnectDelayMs = 1000;       // 首次连接失败后等待1秒再重连
        private const int MaxReconnectDelayMs = 10000;          // 指数退避最大间隔10秒，避免现场长时间无人恢复
        private const int TorqueNoDataSummaryIntervalSeconds = 30; // 已连接但没有收到0061扭力数据时，周期性输出诊断摘要
        private const int PacketSummaryMaxLength = 180;            // 原始报文摘要最大长度，避免现场日志被长报文刷爆
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        public event Action<bool, string> OnConnectionStatusChanged;
        public event Action<TorqueData> OnTorqueDataReceived;
        public event Action<string, bool> OnLog;

        public bool IsConnected { get; private set; }    

        private readonly string _ip;
        private readonly int _port;
        private bool _isConnected;
        private bool _isStarted;

        // 同步等待锁
        private TaskCompletionSource<bool> _mid0002Tcs; // 通讯成功
        private TaskCompletionSource<bool> _mid0005Tcs; // 订阅成功

        // 看门狗时间戳
        private DateTime _lastReceiveTime;
        private DateTime _lastTorqueDataReceiveTime;
        private DateTime _lastNoTorqueDataSummaryTime;

        public TorqueControllerClient(string ip, int port = 4545)
        {
            _ip = ip;
            _port = port;
        }

        public void Start()
        {
            if (_isStarted) return;
            _isStarted = true;
            _cts = new CancellationTokenSource();
            _ = Task.Run(() => ConnectionManagerLoop(_cts.Token));
        }

        public void Stop()
        {
            _isStarted = false;
            _cts?.Cancel();
            CleanupConnection();
        }

        private async Task ConnectionManagerLoop(CancellationToken token)
        {
            int failCount = 1;
            const int FailCountMax = 3;

            while (!token.IsCancellationRequested && _isStarted && failCount <= FailCountMax)
            {
                try
                {
                    await ConnectAndRunAsync(token);
                }
                catch (Exception ex)
                {
                    // 触发外部的互锁与HandleError
                    SetConnectionStatus(false, ex.Message);
                    OnLog?.Invoke($"[重试次数{failCount++}] 通讯异常{ex.Message}", true);
                    await Task.Delay(3000, token);
                }
            }
        }

        /// <summary>
        /// 建立Socket -> 发0001 -> 等0002（3s超时） -> 发0060 -> 等0005 (3s超时，若收到0004报错)
        /// </summary>
        /// <param name="mainToken"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        private async Task ConnectAndRunAsync(CancellationToken mainToken)
        {
            CleanupConnection();

            // --- 阶段 1：建立 Socket 物理连接 ---

            _client = new TcpClient();
            OnLog?.Invoke($"正在建立Socket连接 {_ip}:{_port}……", false);

            var connectTask = _client.ConnectAsync(_ip, _port);
            if (await Task.WhenAny(connectTask, Task.Delay(3000, mainToken)) != connectTask)
                throw new TimeoutException("Socket连接超时");

            _stream = _client.GetStream();
            _lastReceiveTime = DateTime.Now;
            OnLog?.Invoke("Socket连接已建立", false);

            _mid0002Tcs = new TaskCompletionSource<bool>();
            _mid0005Tcs = new TaskCompletionSource<bool>();

            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(mainToken))
            {
                // 先启动接收循环和看门狗
                var receiveTask = ReceiveLoop(linkedCts.Token);
                var watchdogTask = WatchdogLoop(linkedCts.Token);

                // --- 阶段 2：Open Protocol 通讯握手 (0001 -> 0002) ---
                OnLog?.Invoke(">> 发送通讯请求 (MID 0001)", false);
                await SendAsync("00200001001000000000", linkedCts.Token);

                if (await Task.WhenAny(_mid0002Tcs.Task, Task.Delay(3000, linkedCts.Token)) != _mid0002Tcs.Task)
                    throw new TimeoutException("未收到通讯确认 (MID 0002)");

                // --- 阶段 3：订阅扭力数据 (0060 -> 0005) ---
                OnLog?.Invoke(">> 发送数据订阅请求 (MID 0060)", false);
                await SendAsync("00200060001000000000", linkedCts.Token);

                var subscribeWaitTask = await Task.WhenAny(_mid0005Tcs.Task, Task.Delay(3000, linkedCts.Token));
                if (subscribeWaitTask != _mid0005Tcs.Task)
                    throw new TimeoutException("数据订阅超时，未收到确认 (MID 0005)");

                // 检查是否是被 MID 0004 异常中断
                await _mid0005Tcs.Task;

                // --- 阶段 4：完全就绪 ---
                ResetTorqueReceiveDiagnostics();
                SetConnectionStatus(true, "通讯建立且订阅成功");
                //OnLog?.Invoke($"已连接电批 {_ip} 并完成数据订阅");

                // 等待循环意外结束 (断线、看门狗超时等)
                await Task.WhenAny(receiveTask, watchdogTask);
                linkedCts.Cancel();
            }

            throw new Exception("通讯被意外中断");
        }

        /// <summary>
        /// 带有超时控制的发送方法
        /// </summary>
        private async Task SendAsync(string message, CancellationToken token)
        {
            if (_stream == null) throw new InvalidOperationException("网络流已断开");

            try
            {
                // Note：
                // 1. All information sent over the communication links is ASCII format.
                // 2. message = [header(20 bytes)] + <data field> + [message end]
                // 3.All the messages are NUL terminated.
                // The NUL termination is not included in the message length.In this manual this is illustrated with NUL, ASCII 0x00.
                byte[] data = Encoding.ASCII.GetBytes(message + "\0");

                // 限制发送行为最多耗时 3 秒
                using (var writeCts = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    writeCts.CancelAfter(3000);
                    await _stream.WriteAsync(data, 0, data.Length, writeCts.Token);
                    await _stream.FlushAsync(writeCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("发送指令到控制器超时");
            }
            catch (Exception ex)
            {
                throw new Exception($"发送指令异常: {ex.Message}");
            }
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[4096];

            while (!token.IsCancellationRequested)
            {
                int len = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

                // 优雅断开捕捉 (正常收到了 FIN 包)
                if (len == 0) throw new SocketException((int)SocketError.ConnectionReset);

                // 刷新看门狗时间戳
                _lastReceiveTime = DateTime.Now;

                string rawMsg = Encoding.ASCII.GetString(buffer, 0, len);
                string[] packets = rawMsg.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var packet in packets)
                {
                    ProcessMessage(packet);
                }
            }
        }

        private async Task WatchdogLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(5000, token); // 每5秒发一次心跳

                // 如果超过15秒没收到任何回复，判断为物理静默断线
                if ((DateTime.Now - _lastReceiveTime).TotalSeconds > 15)
                {
                    throw new TimeoutException("心跳回应超时，设备静默断线");
                }

                // 定时发送心跳保活。如果发送超时或报错，SendAsync 会抛出异常从而中断看门狗并重连
                await SendAsync("00209999001000000000", token);

                ReportNoTorqueDataIfNeeded();
            }
        }

        private void ProcessMessage(string msg)
        {
            if (msg.Length < 8)
            {
                OnLog?.Invoke($"收到短报文，Length={msg.Length}，Raw={BuildPacketSummary(msg)}", false);
                return;
            }

            string mid = msg.Substring(4, 4);

            switch (mid)
            {
                case "0002":
                    LogNonHeartbeatPacket(mid, msg);
                    OnLog?.Invoke("<< 收到通讯确认 (MID 0002)", false);
                    _mid0002Tcs?.TrySetResult(true);
                    break;
                case "0004":
                    LogNonHeartbeatPacket(mid, msg);
                    OnLog?.Invoke($"<< 命令错误被拒绝 (MID 0004): {msg}", false);
                    // 如果在订阅期间收到0004，向任务抛出异常，直接中断连接重来
                    _mid0005Tcs?.TrySetException(new Exception("控制器拒绝了订阅请求 (返回 MID 0004)"));
                    break;
                case "0005":
                    LogNonHeartbeatPacket(mid, msg);
                    OnLog?.Invoke("<< 订阅成功 (MID 0005)", false);
                    _mid0005Tcs?.TrySetResult(true);
                    break;
                case "0061":
                    _lastTorqueDataReceiveTime = DateTime.Now;
                    OnLog?.Invoke($"收到扭力原始报文 MID 0061，Length={msg.Length}，Raw={BuildPacketSummary(msg)}", false);
                    // 收到数据后立即异步回复ACK，不阻塞接收循环
                    _ = SendAsync("00200062001000000000", _cts.Token);
                    ParseTorqueData(msg);
                    break;
                case "9999":
                    // 收到心跳回复，仅用于刷新时间戳 (在 ReceiveLoop 已刷新)
                    break;
                default:
                    OnLog?.Invoke($"收到未处理 MID {mid}，Length={msg.Length}，Raw={BuildPacketSummary(msg)}", false);
                    break;
            }
        }

        private void ParseTorqueData(string rawData)
        {
            // ID 09 Tightening Status: Offset 108, Len 1
            // ID 10 Torque status: Offset 111, Len 1（0=Low, 1=OK, 2=High）
            // ID 12 Torque Min Limit: Offset 117, Len 6
            // ID 13 Torque Max Limit: Offset 125, Len 6
            // ID 14 Torque final target: Offset 133, Len 6
            // ID 15 Torque: Offset 141, Len 6
            // ID 20 Time stamp: Offset 177, Len 19
            try
            {
                if (rawData.Length < 20) return;
                var data = new TorqueData { RawData = rawData };
                data.TighteningStatus = rawData.Substring(107, 1) == "1";
                data.TorqueMin = rawData.Substring(116, 6);
                data.TorqueMax = rawData.Substring(124, 6);
                data.Torque = rawData.Substring(140, 6);
                data.TimeStamp = rawData.Substring(176, 19);

                OnLog?.Invoke($"扭力报文解析成功，Torque={data.Torque}，Min={data.TorqueMin}，Max={data.TorqueMax}，Result={(data.TighteningStatus ? "OK" : "NG")}，Time={data.TimeStamp}", false);
                OnTorqueDataReceived?.Invoke(data);
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"解析错误: {ex.Message}", true);
            }
        }

        /// <summary>
        /// 连接订阅成功后重置扭力接收诊断时间。
        /// </summary>
        private void ResetTorqueReceiveDiagnostics()
        {
            _lastTorqueDataReceiveTime = DateTime.Now;
            _lastNoTorqueDataSummaryTime = DateTime.Now;
        }

        /// <summary>
        /// 周期性提示控制器已连接但没有收到扭力数据，方便判断控制器是否未发送0061。
        /// </summary>
        private void ReportNoTorqueDataIfNeeded()
        {
            if (!IsConnected) return;

            DateTime now = DateTime.Now;
            if ((now - _lastTorqueDataReceiveTime).TotalSeconds < TorqueNoDataSummaryIntervalSeconds) return;
            if ((now - _lastNoTorqueDataSummaryTime).TotalSeconds < TorqueNoDataSummaryIntervalSeconds) return;

            _lastNoTorqueDataSummaryTime = now;
            OnLog?.Invoke($"已连接但未收到 MID 0061 扭力数据，距上次0061={Math.Round((now - _lastTorqueDataReceiveTime).TotalSeconds)}秒", false);
        }

        /// <summary>
        /// 记录非心跳控制器报文摘要。
        /// </summary>
        private void LogNonHeartbeatPacket(string mid, string msg)
        {
            OnLog?.Invoke($"收到控制器报文 MID {mid}，Length={msg.Length}，Raw={BuildPacketSummary(msg)}", false);
        }

        /// <summary>
        /// 构造有长度限制的原始报文摘要，避免日志过长。
        /// </summary>
        private static string BuildPacketSummary(string packet)
        {
            if (string.IsNullOrEmpty(packet)) return string.Empty;

            string normalized = packet.Replace("\0", "\\0").Replace("\r", "\\r").Replace("\n", "\\n");
            if (normalized.Length <= PacketSummaryMaxLength) return normalized;

            return normalized.Substring(0, PacketSummaryMaxLength) + "...";
        }

        private void SetConnectionStatus(bool status, string msg)
        {
            if (_isConnected != status)
            {
                _isConnected = status;
                IsConnected = status;
                OnConnectionStatusChanged?.Invoke(status, msg);
            }
        }

        private void CleanupConnection()
        {
            SetConnectionStatus(false, "已断开");
            _stream?.Close();
            _client?.Close();
            _stream = null;
            _client = null;
        }
    }

}
