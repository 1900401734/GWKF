using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MesDatas.Services
{
    /// <summary>
    /// 扭力数据实体
    /// </summary>
    public class TorqueData
    {
        public bool IsOk { get; set; }           // 拧紧结果 (OK/NG)
        public string TorqueActual { get; set; } // 实际扭力
        public string TorqueMin { get; set; }    // 扭力下限
        public string TorqueMax { get; set; }    // 扭力上限
        public string AngleActual { get; set; }  // 实际角度
        public int BatchCounter { get; set; }    // 当前是第几颗螺丝 (从1开始)
        public DateTime TimeStamp { get; set; }
        public string RawData { get; set; }      // 原始报文
    }

    public class TorqueControllerClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected = false;
        private CancellationTokenSource _cts;

        // --- 事件定义 ---
        // 事件：连接状态改变
        public event Action<bool, string> OnConnectionStatusChanged;
        // 事件：收到扭力数据
        public event Action<TorqueData> OnTorqueDataReceived;
        // 日志事件（方便调试）
        public event Action<string> OnLog;

        // 配置参数
        private string _ip;
        private int _port;

        public TorqueControllerClient(string ip, int port = 4545)
        {
            _ip = ip;
            _port = port;
        }

        // --- 1. 建立连接 (Connect) ---
        public async Task ConnectAsync()
        {
            if (_isConnected) return;

            try
            {
                _client = new TcpClient();
                // 3秒超时
                OnLog?.Invoke($"正在连接电批控制器 {_ip}:{_port}...");

                var connectTask = _client.ConnectAsync(_ip, _port);
                if (await Task.WhenAny(connectTask, Task.Delay(3000)) != connectTask)
                {
                    throw new TimeoutException("连接超时");
                }

                _stream = _client.GetStream();
                _isConnected = true;
                _cts = new CancellationTokenSource();

                OnConnectionStatusChanged?.Invoke(true, "已连接");
                OnLog?.Invoke($"已连接电批 {_ip}");

                // --- 2. 握手与订阅流程 (Handshake & Subscribe) ---
                // 启动后台接收循环
                _ = Task.Run(() => ReceiveLoop(_cts.Token));

                // 启动心跳循环
                _ = Task.Run(() => KeepAliveLoop(_cts.Token));

                // 发送通信开始 MID 0001
                Send("00200001001000000000");
                OnLog?.Invoke("已发送握手请求");
                await Task.Delay(200); // 稍微延时等待回应

                // 发送订阅 MID 0060
                Send("00200060001000000000");
                OnLog?.Invoke("已发送订阅请求");
            }
            catch (Exception ex)
            {
                Disconnect();
                OnLog?.Invoke($"连接异常: {ex.Message}");
                OnConnectionStatusChanged?.Invoke(false, ex.Message);
            }
        }

        // --- 3. 发送指令辅助方法 ---
        private void Send(string message)
        {
            if (!_isConnected || _stream == null) return;

            try
            {
                // Open Protocol 规定消息以 NUL (0x00) 结尾
                byte[] data = Encoding.ASCII.GetBytes(message + "\0");
                _stream.Write(data, 0, data.Length);
                // OnLog?.Invoke($">> 发送: {message}"); // 调试时可开启
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"发送失败: {ex.Message}");
                Disconnect();
            }
        }

        // --- 4. 接收循环 (Loop) ---
        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[4096];
            while (!token.IsCancellationRequested && _isConnected)
            {
                try
                {
                    if (_stream.DataAvailable)
                    {
                        int len = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                        if (len > 0)
                        {
                            string rawMsg = Encoding.ASCII.GetString(buffer, 0, len);
                            // 处理粘包（简单处理：按NUL分割）
                            string[] packets = rawMsg.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var packet in packets)
                            {
                                ProcessMessage(packet);
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(50);
                    }
                }
                catch (Exception ex)
                {
                    if (_isConnected) OnLog?.Invoke($"接收循环异常: {ex.Message}");
                    break;
                }
            }
        }

        // --- 5. 消息处理与解析 (Parse & Ack) ---
        private void ProcessMessage(string msg)
        {
            if (msg.Length < 8) return;

            string mid = msg.Substring(4, 4); // 获取 MID 编号

            switch (mid)
            {
                case "0002": // 握手确认
                    OnLog?.Invoke("<< 收到通信确认 (MID 0002)");
                    break;
                case "0005": // 命令接受
                    OnLog?.Invoke("<< 命令被接受 (MID 0005)");
                    break;
                case "0004": // 命令错误
                    OnLog?.Invoke($"<< 命令错误 (MID 0004): {msg}");
                    break;
                case "0061": // *** 核心：拧紧结果 ***
                    OnLog?.Invoke("<< 收到拧紧数据 (MID 0061)");

                    // A. 立即回复 ACK (MID 0062)
                    Send("00200062001000000000");
                    OnLog?.Invoke(">> 已回复 ACK (MID 0062)");

                    // B. 解析数据 (复用您提供的逻辑)
                    ParseTorqueData(msg);
                    break;
                case "9999": // 心跳回应
                     //OnLog?.Invoke("<< 收到心跳回应");
                    break;
                default:
                     OnLog?.Invoke($"<< 未处理消息: {msg}");
                    break;
            }
        }

        // --- 6. 数据解析逻辑 (复用API代码) ---
        private void ParseTorqueData(string str)
        {
            try
            {
                // Header = 20 bytes. 
                // ID 01 Cell ID: Offset 23, Len 4
                // ID 08 Batch Counter: Offset 102, Len 4
                // ID 09 Status: Offset 108, Len 1
                // ID 12 Min: Offset 117, Len 6
                // ID 13 Max: Offset 125, Len 6
                // ID 15 Torque: Offset 141, Len 6
                // ID 19 Angle: Offset 170, Len 5

                /*StringBuilder sbIdx = new StringBuilder();
                StringBuilder sbTen = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    sbIdx.Append(i % 10);
                    sbTen.Append((i / 10) % 10);
                }
                OnLog?.Invoke($"\n[标尺个位] {sbIdx.ToString()}");
                OnLog?.Invoke($"[标尺十位] {sbTen.ToString()}");
                OnLog?.Invoke($"[原始报文] {str}\n");*/

                // 简单校验
                if (str.Length < 100) return;   // 长度保护

                TorqueData data = new TorqueData();
                data.RawData = str;
                data.TimeStamp = DateTime.Now;

                // 解析结果
                string status = GetSubString(str, 20 + 87, 1);
                data.IsOk = (status == "1");

                // 解析批次计数 (第几颗螺丝)
                string batchCnt = GetSubString(str, 20 + 82, 4);
                int.TryParse(batchCnt, out int bc);
                data.BatchCounter = bc;

                // 解析扭力 (除以100)
                data.TorqueActual = GetSubString(str, 20 + 120, 6).Trim();
                data.TorqueMin = GetSubString(str, 20 + 96, 6).Trim();
                data.TorqueMax = GetSubString(str, 20 + 104, 6).Trim();

                // 解析角度 (不除)
                data.AngleActual = GetSubString(str, 20 + 150, 5).TrimStart('0');
                if (string.IsNullOrEmpty(data.AngleActual)) data.AngleActual = "0";

                OnTorqueDataReceived?.Invoke(data);
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"解析错误: {ex.Message}");
            }
        }

        /*private void ParseTorqueData(string str)
        {
            try
            {
                // Header = 20 bytes. 
                // ID 01 Cell ID: Offset 23, Len 4
                // ID 08 Batch Counter: Offset 102, Len 4
                // ID 09 Status: Offset 108, Len 1
                // ID 12 Min: Offset 117, Len 6
                // ID 13 Max: Offset 125, Len 6
                // ID 15 Torque: Offset 141, Len 6
                // ID 19 Angle: Offset 170, Len 5

                // 简单校验
                if (!str.Contains("0061")) return;

                // 确保数据长度足够（OpenProtocol 标准长度较长，这里做个基础保护）
                // 您的API文档中提到 Header=20字节，数据从第21字节开始
                int headerLength = 20;
                if (str.Length < 100) return;

                TorqueData data = new TorqueData();
                data.RawData = str;

                // 截取逻辑参考您的 API.txt 文件
                // 注意：C# Substring(startIndex, length)
                // 您的文档注释：
                // ID 01 Cell ID: start 23 (index 22), len 4
                // ID 09 Tightening Status: start 109 (index 108), len 1. (0=NG, 1=OK)
                // ID 15 Torque: start 142 (index 141), len 6. (需除以100)
                // ID 19 Angle: start 171 (index 170), len 5.

                // --- 根据 API.txt 修正的索引位置 (Header=20) ---
                // 文档中的代码示例使用的是 PackDataStartPos = 20
                // 下面的索引是基于 API.txt 中的代码逻辑推算的相对位置

                // 拧紧状态 (ID 09)
                // API.txt: str.Substring(PackDataStartPos + 88, 1); => Index 108
                string statusStr = str.Substring(20 + 88, 1);
                data.IsOk = (statusStr == "1");

                // 扭力值 (ID 15)
                // API.txt: str.Substring(PackDataStartPos + 121, 6); => Index 141
                string torqueRaw = str.Substring(20 + 121, 6);
                if (double.TryParse(torqueRaw, out double tVal))
                {
                    data.Torque = (tVal / 100.0).ToString("0.00");
                }

                // 角度值 (ID 19)
                // API.txt: str.Substring(PackDataStartPos + 150, 5); => Index 170
                data.Angle = str.Substring(20 + 150, 5).TrimStart('0');

                // 站点ID (ID 01)
                data.CellId = str.Substring(20 + 3, 4);

                data.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 通知界面
                OnTorqueDataReceived?.Invoke(data);
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"解析错误: {ex.Message}");
            }
        }*/

        // --- 7. 心跳保活 (Keep Alive) ---
        private async Task KeepAliveLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _isConnected)
            {
                await Task.Delay(10000, token); // 每10秒发一次
                if (_isConnected)
                {
                    Send("00209999001000000000");
                }
            }
        }

        public void Disconnect()
        {
            _isConnected = false;
            _cts?.Cancel();
            _stream?.Close();
            _client?.Close();
            OnConnectionStatusChanged?.Invoke(false, "已断开");
        }

        private string GetSubString(string src, int idx, int len)
        {
            if (idx + len > src.Length) return "0";
            return src.Substring(idx, len);
        }

        private string FormatVal(string raw, double div)
        {
            if (double.TryParse(raw, out double val))
                return (val / div).ToString("0.00");
            return "0.00";
        }
    }
}
