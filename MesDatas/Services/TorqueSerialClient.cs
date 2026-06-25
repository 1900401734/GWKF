using MesDatas.Utility;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesDatas.Services
{
    public class TorqueMeterData
    {
        public string RawData { get; set; }

        public string Torque { get; set; }
    }

    public class TorqueSerialClient
    {
        private SerialPort _serialPort;
        private CancellationTokenSource _cts;

        public event Action<bool, string> OnConnectionStatusChanged;
        public event Action<string> OnTorqueDataReceived;
        public event Action<string, bool> OnLog;

        public bool IsConnected { get; private set; }

        private string _portName;
        private readonly int _baudRate;
        private bool _isConnected;
        private bool _isStarted;

        // --- 新增：峰值提取与批次控制变量 ---
        private System.Timers.Timer _batchTimer;
        private float _currentMaxTorque = float.MinValue;
        private bool _isCollecting = false;
        private readonly object _batchLock = new object(); // 线程锁，保证并发安全
        StringBuilder dataBuffer = new StringBuilder();

        public TorqueSerialClient(string portName, int baudRate = 19200)
        {
            _portName = portName;
            _baudRate = baudRate;

            // 初始化批次定时器 (例如 500 毫秒没有新数据，视为当前批次结束)
            _batchTimer = new System.Timers.Timer(3000);
            _batchTimer.AutoReset = false; // 只触发一次
            _batchTimer.Elapsed += BatchTimer_Elapsed;
        }

        /// <summary>
        /// 当数据流中断(超过500ms)，触发批次结束逻辑，上传峰值
        /// </summary>
        private void BatchTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_batchLock)
            {
                if (_isCollecting)
                {
                    // 触发事件，只回传当前批次的最大值
                    OnTorqueDataReceived?.Invoke(_currentMaxTorque.ToString("F3"));
                    dataBuffer.Clear();
                    // 重置批次状态，等待下一次拧紧动作
                    _isCollecting = false;
                    _currentMaxTorque = float.MinValue;
                }
            }
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
            _batchTimer?.Stop(); // 停止批次定时器
            CleanupConnection();
        }

        /// <summary>
        /// 连接守护循环：负责断线重连、异常捕获
        /// </summary>
        private async Task ConnectionManagerLoop(CancellationToken token)
        {
            int retryCount = 0;
            const int maxRetries = 5;

            while (!token.IsCancellationRequested && _isStarted && retryCount < maxRetries)
            {
                try
                {
                    await ConnectAndRunAsync(token);
                }
                catch (Exception ex)
                {
                    retryCount++;
                    SetConnectionStatus(false, ex.Message);
                    OnLog?.Invoke($"串口通讯异常: {ex.Message}，3秒后尝试重连...", true);

                    // 等待 3 秒后重试，避免死循环导致 CPU 占用过高
                    await Task.Delay(3000, token);
                }
            }
        }

        /// <summary>
        /// 核心工作流：寻址 -> 打开端口 -> 异步监听
        /// </summary>
        private async Task ConnectAndRunAsync(CancellationToken mainToken)
        {
            CleanupConnection();

            // --- 阶段 1：自动搜索与确定端口 ---
            string targetPort = _portName;
            if (string.IsNullOrEmpty(targetPort) || targetPort.Equals("AUTO", StringComparison.OrdinalIgnoreCase))
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length == 0)
                    throw new Exception("本机无可用物理串口");

                targetPort = ports[0]; // 默认取第一个可用端口
                OnLog?.Invoke($"启动自动寻址，选中端口: {targetPort}", false);
            }

            // --- 阶段 2：配置并打开串口 ---
            _serialPort = new SerialPort
            {
                PortName = targetPort,
                BaudRate = _baudRate,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 500,
                WriteTimeout = 500,
                Encoding = Encoding.ASCII
            };

            OnLog?.Invoke($"正在尝试打开串口 {targetPort}...", false);
            _serialPort.Open(); // 若端口被占用，此处会抛出异常被外层捕获

            SetConnectionStatus(true, $"已连接到 {targetPort}");
            OnLog?.Invoke($"串口 {targetPort} 打开成功，开始监听数据", false);

            // --- 阶段 3：启动异步接收循环 ---
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(mainToken))
            {
                // 使用 await 阻塞当前方法，直到 ReceiveLoop 抛出断线异常或被取消
                await ReceiveLoop(linkedCts.Token);
            }

            throw new Exception("串口监听被意外中断");
        }

        /// <summary>
        /// 异步数据接收与解析循环 (适配 Sdddd.dd\r 协议)
        /// </summary>
        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[1024];

            while (!token.IsCancellationRequested && _serialPort.IsOpen)
            {
                /*try
                {
                    await Task.Delay(500, token);

                    if (_serialPort.BytesToRead > 0)
                    {
                        int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            throw new Exception("流被关闭");

                        string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        dataBuffer.Append(receivedData);
                    }
                }
                catch (TimeoutException) { *//* 忽略超时异常 *//* }
                catch (Exception ex)
                {
                    OnLog?.Invoke($"读取数据异常: {ex.Message}", true);
                    break;
                }*/

                // 【关键修复 1】不再使用 Task.Delay(500) 轮询。
                // 注意：必须使用 BaseStream.ReadAsync，这样才不会在没数据时阻塞线程
                // 直接使用 ReadAsync 实时等待数据，数据一到马上处理，绝不堆积。
                try
                {
                    int bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (bytesRead == 0) throw new Exception("流被关闭");

                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    dataBuffer.Append(receivedData);
                }
                catch (OperationCanceledException) { break; } // 捕获取消信号，正常退出
                catch (TimeoutException) { continue; }        // 忽略超时，继续监听
                catch (Exception ex)
                {
                    OnLog?.Invoke($"读取数据异常: {ex.Message}", true);
                    break;
                }

                // --- 协议解析逻辑 ---
                while (dataBuffer.Length > 0)
                {
                    string currentStr = dataBuffer.ToString();

                    // 1. 寻找帧头 'S'
                    int startIndex = currentStr.IndexOf('S');
                    if (startIndex < 0)
                    {
                        dataBuffer.Clear(); // 没找到帧头，全是垃圾数据，清空
                        break;
                    }

                    // 如果 'S' 前面有脏数据，剔除掉
                    if (startIndex > 0)
                    {
                        dataBuffer.Remove(0, startIndex);
                        currentStr = dataBuffer.ToString();
                    }

                    // 2. 寻找帧尾 '\r'
                    int endIndex = currentStr.IndexOf('\r');
                    if (endIndex < 0)
                        break; // 帧尾还没收到，跳出循环等待下一次 ReadAsync

                    // 3. 提取完整一帧数据 (例如: "S  0.17")
                    string frame = currentStr.Substring(0, endIndex);
                    dataBuffer.Remove(0, endIndex + 1); // 从缓冲区移除已处理的帧（包括 '\r'）

                    // 4. 解析数据：剔除首字母 'S' 并去除空格
                    string dataPart = frame.Substring(1).Trim();

                    if (float.TryParse(dataPart, out float value))
                    {
                        // 触发事件回传数据
                        //OnTorqueDataReceived?.Invoke(value.ToString());
                        //OnTorqueDataReceived?.Invoke(value.ToString("F2"));

                        // 【修改核心】进入批次数据收集模式
                        lock (_batchLock)
                        {
                            if (!_isCollecting)
                            {
                                // 新的批次开始了
                                _isCollecting = true;
                                _currentMaxTorque = value;
                            }
                            else
                            {
                                // 批次进行中，持续更新峰值 (最大值)
                                if (value > _currentMaxTorque)
                                {
                                    _currentMaxTorque = value;
                                }
                            }

                            // 只要还有数据进来，就重置/续命定时器
                            _batchTimer.Stop();
                            _batchTimer.Start();
                        }
                    }
                    else
                    {
                        OnLog?.Invoke($"无效的数据格式: {frame}", true);
                    }
                }
            }
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
            SetConnectionStatus(false, "连接已断开");

            if (_serialPort != null)
            {
                try
                {
                    if (_serialPort.IsOpen)
                        _serialPort.Close();
                }
                catch { /* 忽略关闭时的异常 */ }

                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        /// <summary>
        /// 获取当前系统可用串口列表
        /// </summary>
        public static string[] GetAvailablePorts()
        {
            try
            {
                return SerialPort.GetPortNames();
            }
            catch
            {
                // 防止 VSPD 卸载驱动瞬间读取注册表报错
                return new string[0];
            }
        }

        /// <summary>
        /// 自动刷新指定的串口下拉框 (支持同时传入多个 ComboBox)
        /// </summary>
        /// <param name="comboBoxes">需要刷新的 ComboBox 控件数组</param>
        public static void AutoRefreshComboBoxes(params ComboBox[] comboBoxes)
        {
            // 1. 安全获取最新端口
            string[] availablePorts = GetAvailablePorts();

            // 2. 遍历传入的所有下拉框（如端口1、端口2）
            foreach (var cbo in comboBoxes)
            {
                if (cbo == null || cbo.IsDisposed) continue;

                // 3. 跨线程安全处理：使用 BeginInvoke 防止死锁
                if (cbo.InvokeRequired)
                {
                    cbo.BeginInvoke(new Action(() => UpdateSingleComboBox(cbo, availablePorts)));
                }
                else
                {
                    UpdateSingleComboBox(cbo, availablePorts);
                }
            }
        }

        /// <summary>
        /// 内部方法：执行单个下拉框的更新逻辑
        /// </summary>
        private static void UpdateSingleComboBox(ComboBox cbo, string[] ports)
        {
            // 记住当前选择
            string currentSelection = cbo.Text;

            // 清空并加载最新数据
            cbo.Items.Clear();
            if (ports.Length > 0)
            {
                cbo.Items.AddRange(ports);

                // 尝试恢复选择
                if (cbo.Items.Contains(currentSelection))
                {
                    cbo.Text = currentSelection;
                }
                else
                {
                    cbo.SelectedIndex = 0;
                }
            }
            else
            {
                cbo.Text = string.Empty;
            }
        }
    }
}
