using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using LabelManager2;
using MesDatas.Models;
using MesDatas.DataAcess;
using MesDatas.Services;
using MesDatas.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Res = MesDatas.Properties.Resources;

namespace MesDatas.Views
{
    public partial class Form1 : Form
    {
        private static RequestMes _request;         // mes访问类
        private static JObject _getTokenJson;       // 获取token的json，用于初始化HttpClientUtil对象
        private static HttpClientUtil _httpClient;
        private Assembly assembly;
        private ResourceManager resources;
        private PlcAddressInfo plcAddressInfo;
        private PLCAdress plcAddress;

        public static readonly Dictionary<string, object> GlobalData = new Dictionary<string, object>(); // 接口配置信息 动态全局变量

        private string[] id = { };                     // 测试项数量
        private string[] stationIdArray = { };         // 目标工位号
        private string[] realValuePointArray = { };    // 实际值
        private string[] testNameArray = { };          // 测试项名称
        private string[] maxValuePointArray = { };     // 上限地址
        private string[] minValuePointArray = { };     // 下限地址
        private string[] testResultPointArray = { };   // 测试结果地址
        private string[] unitNameArray = { };          // 单位名称

        private AccessHelper curMdb;        // 当前数据对象
        private AccessHelper sourceMdb;     // 原始数据库对象
        private WriteLog writeLog;          // 写日志到RichTextBox
        private string programLogString;
        public static string LocalFile;

        private DataGridViewData userInfoDataGridObject;
        private DataGridViewData errorPreserveDataGridObject;
        private DataGridViewData keyArgsDataGridObject;
        private DataGridViewData gatherDataGridObject;
        private DataGridViewData defectDataGridObject;
        private DataGridViewData printDirectoryObject;
        private DataGridViewData changeTypeDataGridObject;

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;

            InitializeComponent();

            InitializeVariables();
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 读取系统设置
            Load_SystemSettingsConfig();

            // 从数据库加载“生产配置”页面的参数
            Load_ProductConfig();

            // 从数据库加载并缓存检测项
            InitializeTestItemCache();

            // 读取生产信息
            // GetProduction_Info();

            //LocalFile = LocalFilePath.Text;
        }

        /// <summary>
        /// 窗体加载后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            // 检查用户权限
            CheckUserPrivilege();

            // 加载相应表格
            LoadDgvByDeviceName();

            // 获取token的json对象
            _getTokenJson = GetTokenJson();

            // 创建httpClient对象，必须在窗口加载完成后创建，否则Url_Token没有数据
            _httpClient = new HttpClientUtil(_getTokenJson, Url_Token.Text);

            // 写设备名称
            SetDeviceName();

            // 启动永久任务
            StartPermanentTask();

            // 启动动态任务
            SetDynamicTaskStart();

            // 设置工单信息
            SetOrderMessage();

            // 初始化所有需要维护的表格
            InitDataGirdView();

            // 初始化装配机界面
            Load_PrinterSet();

            // 初始化PLC地址维护界面
            LoadPlcAddress();

            // 初始状态为待机
            ProductResult.Text = Res.standby;
            ProductResult.ForeColor = Color.Black;

            System.Windows.Forms.Timer statusTimer = new System.Windows.Forms.Timer();
            statusTimer.Interval = 500;
            statusTimer.Tick += UiUpdateTimer_Tick;
            statusTimer.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (plcConnectObject != null)
            //    plcConnectObject.ConnectClose();
            _plcManager?.Close();

            // 取消任务
            permanentTaskCts.Cancel();

            // 关闭数据库连接
            curMdb.CloseConnection();
            sourceMdb.CloseConnection();

            Environment.Exit(0);
        }

        /// <summary>
        /// 根据设备名加载相应表格
        /// </summary>
        private void LoadDgvByDeviceName()
        {
            if (Global.Instance.CurDataBaseName == "装配机")
            {
                tabControl_UploadData.SizeMode = TabSizeMode.Normal;
                tabControl_UploadData.ItemSize = new Size(100, 30);

                tabPageResult1.Text = "Scan - ASSY";
                tabPageResult2.Text = "Weight";
                tabPageResult3.Text = "Screw-BA";

                CreateHeaderText(dgvResult1, "1", true);
                CreateHeaderText(dgvResult2, "2", true);
                CreateHeaderText(dgvResult3, "3", true);
            }
            else
            {
                tabControl_UploadData.SizeMode = TabSizeMode.Fixed;
                tabControl_UploadData.ItemSize = new Size(0, 1);

                tabPageResult2.Parent = null;
                tabPageResult3.Parent = null;

                CreateHeaderText(dgvResult1);
            }
        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        private void InitializeVariables()
        {
            _request = new RequestMes();
            sourceMdb = new AccessHelper(Global.Instance.SourceDataBase);
            assembly = Assembly.GetExecutingAssembly();
            resources = new ResourceManager("MesDatas.Language_Resources.language_Chinese", assembly);
            PlcAddressServer.InitTable();
            plcAddressInfo = PlcAddressServer.GetPlcAddressInfo(1);
            _plcManager = new PlcConnectionManager(plcAddressInfo);

            // 订阅事件更新 UI (注意线程安全)
            _plcManager.OnConnectionStatusChanged += (isConnected) => 
            {
                this.Invoke((Action)(() => 
                {
                    isPlcConnected = isConnected; // 如果你还保留这个字段用于兼容旧代码
                    PlcSignalLight.ForeColor = isConnected ? Color.Green : Color.Red;
            
                    // 重要：同步更新 _readWriteNet 引用，以便 Form1 其他地方的代码能继续工作
                    _readWriteNet = _plcManager.ReadWriteNet; 
                }));
            };

            DataTable database = sourceMdb.Find("SELECT database_name FROM SystemDataBase where id=1");
            Global.Instance.DataBase = database.Rows[0]["database_name"].ToString();

            curMdb = new AccessHelper(Global.Instance.DataBase);

            userInfoDataGridObject = new DataGridViewData(dataGridView1, "userinfo", sourceMdb);
            errorPreserveDataGridObject = new DataGridViewData(errorPreserveDataGridView, "ErrorReferenceTable", curMdb);
            keyArgsDataGridObject = new DataGridViewData(keyArgsDataGridView, "KeyArgsPreserve", curMdb);
            gatherDataGridObject = new DataGridViewData(dataGatherDataGridView, "Board", curMdb);
            defectDataGridObject = new DataGridViewData(deviceDefectsDataGridView, "Defect", curMdb);
            printDirectoryObject = new DataGridViewData(dgvPrintDirectory, "PrinterDirectory", curMdb);
            changeTypeDataGridObject = new DataGridViewData(changeTypeDataGridView, "ChangeProductType", curMdb);

            plcAddress = new PLCAdress();

            //programLogString = "时间:{0} 信息:{1}";
            programLogString = "{0}";

            writeLog = new WriteLog();
        }

        /// <summary>
        /// 检查用户权限
        /// </summary>
        private void CheckUserPrivilege()
        {
            //首先移除所有的TabPage
            TabPage[] tabPages = { tabPage1, tabPage2, tabPage3, tabPage4, tabPage5, tabPage6, tabPage7, tabPage8, tabPage9, tabPage9 };
            foreach (TabPage tabPage in tabPages)
            {
                if (!TabContorl1.TabPages.Contains(tabPage))
                    TabContorl1.TabPages.Add(tabPage);

            }
            switch (Global.Instance.LoginMessage.Privilege)
            {
                case 3:  //作业员
                    //TabContorl1.TabPages.Remove(tabPage2);
                    TabContorl1.TabPages.Remove(tabPage3);
                    TabContorl1.TabPages.Remove(tabPage4);
                    TabContorl1.TabPages.Remove(tabPage5);
                    TabContorl1.TabPages.Remove(tabPage6);
                    TabContorl1.TabPages.Remove(tabPage7);
                    TabContorl1.TabPages.Remove(tabPage8);
                    TabContorl1.TabPages.Remove(tabPage9);
                    //TabContorl1.TabPages.Remove(tabPage10);
                    break;
                case 2:  //操作员
                    TabContorl1.TabPages.Remove(tabPage3);
                    break;
                case 1:  //管理员
                    break;
            }
        }

        /// <summary>
        /// 设置工单信息
        /// </summary>
        private void SetOrderMessage()
        {
            // 选取最近更新的工单信息
            string sql = $"SELECT TOP 1 * FROM ChangeOrder WHERE Operator='{Global.Instance.LoginMessage.WorkId}' ORDER BY id DESC";
            DataTable order = curMdb.Find(sql);
            txtUser.Text = Global.Instance.LoginMessage.WorkId;
            if (order.Rows.Count != 1) return;
            OrderNo.Text = order.Rows[0]["OrderNo"].ToString();
            OrderNum.Text = order.Rows[0]["OrderNum"].ToString();
        }

        /// <summary>
        /// 根据 Key 和 Security 获取凭证(Access_Token)，需要使用该 token 才可以调用其他接口。
        /// <para>Key 和 Security 与 Device 绑定，随设备变更</para>
        /// </summary>
        /// <returns></returns>
        private JObject GetTokenJson()
        {
            try
            {
                TokenInputParameter token = new TokenInputParameter
                {
                    Key = CallUiSafely.GetControlPropertyValueSafely(MesKey, c => c.Text),
                    Security = Security.Text.ToLower()
                };

                string json = JsonConvert.SerializeObject(token);
                return JObject.Parse(json);
            }
            catch { throw new Exception("转换json错误"); }
        }

        /// <summary>
        /// 初始化所有的数据表
        /// </summary>
        private void InitDataGirdView()
        {
            // 初始化用户权限
            Dictionary<string, string> dataMap = new Dictionary<string, string>
            {
                {"id","id" },
                {"工号","work_id" },
                //{"密码","pwd" },
                {"权限","privilege" },
            };
            userInfoDataGridObject.BindDataToDataGridView(dataMap);
            //userInfoDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", userInfoDataGridObject.SaveButton_Click);
            userInfoDataGridObject.AddOperatorColumnsButton("操作", "Delete", "删除", userInfoDataGridObject.DeleteButton_Click, row =>
            {
                if (row.Cells["权限"].Value.ToString() == "管理员")
                    if (sourceMdb.Find("select * from userinfo where privilege='管理员'").Rows.Count <= 1)
                    {
                        MessageBox.Show("请至少保留一个管理员账号");
                        return false;
                    }
                return true;
            });
            userInfoDataGridObject.BindEventHandlerButton(UserRefresh, userInfoDataGridObject.RefreshButton);

            // 初始化故障预警信息
            dataMap = new Dictionary<string, string>
            {
                {"id","id" },
                {"故障代码","error_code" },
                {"故障信息","error_name" },
                {"PLC点位","plc_point" },
            };
            errorPreserveDataGridObject.BindDataToDataGridView(dataMap);
            errorPreserveDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", errorPreserveDataGridObject.SaveButton_Click);
            errorPreserveDataGridObject.AddOperatorColumnsButton("操作2", "Delete", "删除", errorPreserveDataGridObject.DeleteButton_Click);
            errorPreserveDataGridObject.BindEventHandlerButton(errorPreserveRefreshButton, errorPreserveDataGridObject.RefreshButton);

            // 初始化关键参数维护信息
            dataMap = new Dictionary<string, string>
            {

                {"id","id" },
                {"参数名","name" },
                {"参数标准值","standard" },
                {"参数上限","USL" },
                {"参数下限","LSL" },
                {"参数单位","unit" },
                /*这部分注释勿删,如果关键参数是用Board表里面的数据时启用
                { "id", "id" },
                { "检查项名称", "BoardName" },
                { "标准值", "StandardCode" },
                { "检查项上限PLC点位", "MaxBoardCode" },
                { "检查项下限PLC点位", "MinBoardCode" },
                { "单位", "BoardA1" },
                */
            };
            keyArgsDataGridObject.BindDataToDataGridView(dataMap);
            keyArgsDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", keyArgsDataGridObject.SaveButton_Click);
            keyArgsDataGridObject.AddOperatorColumnsButton("操作2", "Delete", "删除", keyArgsDataGridObject.DeleteButton_Click);
            keyArgsDataGridObject.BindEventHandlerButton(keyArgsRefreshButton, keyArgsDataGridObject.RefreshButton);

            // 初始化设备缺陷维护信息
            dataMap = new Dictionary<string, string>
            {
                { "id", "id" },
                { "不良代码", "DefectDesc" },
                { "不良位置", "Location" },
                { "是否误判", "Missing" },
            };
            defectDataGridObject.BindDataToDataGridView(dataMap);
            defectDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", defectDataGridObject.SaveButton_Click);
            defectDataGridObject.AddOperatorColumnsButton("操作2", "Delete", "删除", defectDataGridObject.DeleteButton_Click);
            defectDataGridObject.BindEventHandlerButton(deviceDefectsRefreshButton, defectDataGridObject.RefreshButton);

            // 初始化系统设置看板
            dataMap = new Dictionary<string, string>
            {
                { "id", "id" },
                { "工位号", "WorkID" },
                { "检查项名称", "BoardName" },
                { "检查项【实际值】", "BoardCode" },
                { "标准值", "StandardCode" },
                { "检查项上限PLC点位", "MaxBoardCode" },
                { "检查项下限PLC点位", "MinBoardCode" },
                { "检查项结果PLC点位", "ResultBoardCode" },
                { "单位", "BoardA1" },
            };
            gatherDataGridObject.BindDataToDataGridView(dataMap);
            gatherDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", gatherDataGridObject.SaveButton_Click);
            gatherDataGridObject.AddOperatorColumnsButton("操作2", "Delete", "删除", gatherDataGridObject.DeleteButton_Click);
            gatherDataGridObject.BindEventHandlerButton(dataGatherBoardRefreshButton, gatherDataGridObject.RefreshButton);

            dataMap = new Dictionary<string, string>
            {
                { "id","id" },
                { "产品型号", "order_num" },
                { "路径", "path" },
                { "多点位拍照存放目录", "mutiple_photo" },
                { "工单号对应的目录名", "order_directory" },
                { "图片类型", "type" },
                { "对应PLC地址集合", "plc_address" },
                { "对应图片数量", "picture_num" },
            };
            printDirectoryObject.BindDataToDataGridView(dataMap);
            printDirectoryObject.AddOperatorColumnsButton("操作1", "Save", "保存", printDirectoryObject.SaveButton_Click, row =>
            {
                string[] photos = row.Cells["多点位拍照存放目录"].Value.ToString().Split(',');
                string[] plcAddr = row.Cells["对应PLC地址集合"].Value.ToString().Split(',');
                string[] picNum = row.Cells["对应图片数量"].Value.ToString().Split(',');
                bool equal = photos.Length == plcAddr.Length && photos.Length == picNum.Length;
                bool parse = true;
                foreach (string pic in picNum)
                {
                    if (!int.TryParse(pic, out _))
                    {
                        parse = false;
                        break;
                    }
                }
                if (equal && parse && !photos.Contains("") && !plcAddr.Contains(""))
                    return true;
                return false;
            });
            printDirectoryObject.AddOperatorColumnsButton("操作2", "Delete", "删除", printDirectoryObject.DeleteButton_Click);
            printDirectoryObject.BindEventHandlerButton(printRefresh, printDirectoryObject.RefreshButton);

            dataMap = new Dictionary<string, string>
            {
                {"id","id" },
                {"机器型号", "product_type" },
                {"匹配SN码前缀", "barcode_match" }
            };
            changeTypeDataGridObject.BindDataToDataGridView(dataMap, true);
            changeTypeDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", changeTypeDataGridObject.SaveButton_Click, row =>
            {
                string productType = row.Cells["机器型号"].Value.ToString();
                string barcodeMatch = row.Cells["匹配SN码前缀"].Value.ToString();
                return !(string.IsNullOrEmpty(productType) || string.IsNullOrEmpty(barcodeMatch));
            });
            changeTypeDataGridObject.AddOperatorColumnsButton("操作2", "Delete", "删除", changeTypeDataGridObject.DeleteButton_Click);
            changeTypeDataGridObject.BindEventHandlerButton(changeTypeRefresh, changeTypeDataGridObject.RefreshButton);
        }

        /// <summary>
        /// UI 状态更新定时器（PLC连接状态、MES接口状态）
        /// </summary>
        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            // 更新 PLC 状态
            //PlcSignalLight.ForeColor = isPlcConnected ? Color.Green : Color.Red;

            // 更新MES接口状态
            InterfaceSignalLight.ForeColor = isDeviceAlive ? Color.Green : Color.Red;
        }

        #region ---------- 线程启动、退出和休眠 ----------

        /// <summary>
        /// 用于传入不同线程间的信号
        /// </summary>
        private static bool _shouldStopTask;
        private CancellationTokenSource dynamicTaskCts;
        private CancellationTokenSource permanentTaskCts = new CancellationTokenSource();
        private static List<Task> _allDynamicTaskList = new List<Task>(); // PLC重连时需要重启的任务

        /// <summary>
        /// 延迟指定时间并检查是否应该停止任务
        /// </summary>
        /// <param name="time">休眠的毫秒数</param>
        /// <returns>返回 true 表示需要停止线程，false 表示可以继续。</returns>
        private bool DelayAndCheckStop(int time)
        {
            Thread.Sleep(time);
            return _shouldStopTask;
        }

        /// <summary>
        /// 异步等待指定时间，并检查是否需要停止任务
        /// </summary>
        /// <param name="time">等待时间(ms)</param>
        /// <returns>返回 true 表示需要停止，false 表示继续</returns>
        private async Task<bool> DelayAndCheckStopAsync(int time)
        {
            // 如果 CTS 还没初始化（比如程序刚启动），直接用布尔值判断
            if (dynamicTaskCts == null) return _shouldStopTask;

            try
            {
                // 【核心】使用 Task.Delay 的 Token 版本
                // 如果在休眠期间 _dynamicTaskCts.Cancel() 被调用，
                // 这里会立刻抛出 TaskCanceledException，瞬间唤醒线程
                await Task.Delay(time, dynamicTaskCts.Token);

                // 如果正常睡醒了，再检查一下双重保险
                return _shouldStopTask || dynamicTaskCts.IsCancellationRequested;
            }
            catch (TaskCanceledException)
            {
                // 捕获到取消异常，说明外界要求停止
                return true;
            }
            catch (ObjectDisposedException)
            {
                // 防止 CTS 已经被 Dispose
                return true;
            }
        }

        /// <summary>
        /// 启动动态任务
        /// </summary>
        private void SetDynamicTaskStart()
        {
            _shouldStopTask = false;

            dynamicTaskCts?.Dispose(); // 如果旧的还在，先释放
            dynamicTaskCts = new CancellationTokenSource();

            StartDynamicTask(dynamicTaskCts.Token);     // 启动动态任务
        }

        /// <summary>
        /// 启动动态任务
        /// </summary>
        private void StartDynamicTask(CancellationToken token)
        {
            // 必须放在前面，不然等待退出会出现问题
            if (EnablekeyArgsUpload.Checked)
            {
                // 启用关键参数上传
                _allDynamicTaskList.Add(Task.Run(() => CallKeyArgsInterface(), token));
            }

            // 实时读取设备运行参数
            _allDynamicTaskList.Add(Task.Run(() => ReadDeviceArgsRealtime(), token));

            // 实时判断型号是否变更，输入工单
            _allDynamicTaskList.Add(Task.Run(() => MonitorModelSwitchFromPlc(), token));

            // 读取条码
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadBarcode(), token));

            // 读取工单号等生产数据
            _allDynamicTaskList.Add(Task.Run(() => Procesplc_ReadValue(), token));       // 非装配机
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadValue1(), token));     // 装配机工序1
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadValue2(), token));     // 装配机工序2
            _allDynamicTaskList.Add(Task.Run(() => Procesplc_ReadValue3(), token));      // 装配机工序3

            // 实时上传设备状态
            _allDynamicTaskList.Add(Task.Run(() => DeviceStatusUpload(DeviceStatusSignalLight, DeviceStatusDisplay), token));

            // 监听预警数据
            _allDynamicTaskList.Add(Task.Run(() => CallDeviceErrorUpload(), token));

            // 启用实时参数上传
            _allDynamicTaskList.Add(Task.Run(() => CallRealtimeArgsInterface(), token));

            // 初始化扭力采集
            _allDynamicTaskList.Add(Task.Run(() => InitTorqueSystem(), token));

#if USE_LABLEMANAGER
            // 启用打印时
            if (EnablePrintCode.Checked && Global.Instance.CurDataBaseName == "装配机")
            {
                // 启用打印模板标签
                _allDynamicTaskList.Add(Task.Run(() => CallPrintBarCode(), token));

                // 获取多图片
                Task taskGetPicture = Task.Run(() => MovePictureGroup(), token);
                _allDynamicTaskList.Add(taskGetPicture);
            }
#endif
        }

        /// <summary>
        /// 需要重启的线程全部退出
        /// </summary>
        private void SetDynamicTaskExit()
        {
            _shouldStopTask = true;

            if (dynamicTaskCts != null && !dynamicTaskCts.IsCancellationRequested)
            {
                dynamicTaskCts.Cancel();
            }

            isPlcConnected = false;                 // 设置全局连接状态为 Fasle

            TryExitDynamicTask();                   // 等待线程退出

            _allDynamicTaskList = new List<Task>(); // 将线程池清空

            //if (plcConnectObject != null)
            //    plcConnectObject.ConnectClose();    // 断开plc连接
            _plcManager.Close();
        }

        /// <summary>
        /// 启动永久任务
        /// </summary>
        private void StartPermanentTask()
        {
            var ip = CallUiSafely.GetControlPropertyValueSafely(PlcIP, c => c.Text);
            var port = CallUiSafely.GetControlPropertyValueSafely(PlcPort, c => int.Parse(c.Text));
            var type = CallUiSafely.GetControlPropertyValueSafely(PlcConnectType, c => c.Text);

            // 启用接口心跳
            Task.Factory.StartNew(async () => await InterfaceHeatBeat(), TaskCreationOptions.LongRunning);

            // PLC连接与心跳管理
            Task.Factory.StartNew(async () => await _plcManager.StartConnectionTaskAsync(ip, port, type, permanentTaskCts.Token), TaskCreationOptions.LongRunning);

            // 读取复位信号
            Task.Factory.StartNew(async () => await Recovery(), TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 等待需要关闭的线程退出
        /// </summary>
        /// <returns></returns>
        private bool TryExitDynamicTask()
        {
            try
            {
                // 推荐使用 Task.WaitAll，效率更高
                Task.WaitAll(_allDynamicTaskList.ToArray(), 2000); // 最多等2秒，防死锁
                return true;
            }
            catch (AggregateException)
            {
                // 忽略因为 Cancel 导致的异常，这是预期的行为
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region---------- Permanent Task ----------

        #region ---------- PLC连接、心跳检测 ----------

        private PlcConnectionManager _plcManager;
        private bool isPlcConnected;                // 全局连接标志
        private static IReadWriteNet _readWriteNet; // 当前plc连接对象
        private delegate Task ComponentExcuteFunction();

        /// <summary>
        /// 手动连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualConnect_Click(object sender, EventArgs e)
        {
            SetComponentWaitStatus(tabPage3, ManualConnect, "正在连接", async () =>
            {
                if (isPlcConnected)
                {
                    MessageBox.Show("当前PLC已经是连接状态,无法进行手动连接");
                    return;
                }

                string ip = CallUiSafely.GetControlPropertyValueSafely(PlcIP, c => c.Text);
                int port = int.Parse(CallUiSafely.GetControlPropertyValueSafely(PlcPort, c => c.Text));
                string connectMethod = CallUiSafely.GetControlPropertyValueSafely(PlcConnectType, c => c.Text);

                // 检查输入
                string checkInput = CheckIpPortClickInput(ip, PlcPort.Text, connectMethod);
                if (checkInput != null)
                {
                    MessageBox.Show(checkInput);
                    return;
                }

                // 通知所有线程退出
                SetDynamicTaskExit();

                // 尝试连接
                bool status = await _plcManager.TryConnectPlcAsync(ip, port, connectMethod);
                if (status)
                {
                    // 更新本地引用
                    _readWriteNet = _plcManager.ReadWriteNet;
                    isPlcConnected = true; // 兼容旧逻辑

                    // 通知所有线程启用
                    SetDynamicTaskStart();

                    // 自动保存
                    if (PlcInputAutoSave.Checked) SYS_Model_Write(false);
                }

                MessageBox.Show(status ? "连接成功" : "连接失败，请检查输入，或PLC设备在线状态");
            });
        }

        /// <summary>
        /// 设置可能卡住时的状态，比如更新游标为等待
        /// </summary>
        /// <param name="changeCursorControl">需要改变游标的控件</param>
        /// <param name="curControl">当前点击的控件</param>
        /// <param name="waitString">等待时当前控件显示的内容</param>
        /// <param name="func">更新和恢复控件期间需要执行的方法</param>
        private async void SetComponentWaitStatus(Control changeCursorControl, Control curControl, string waitString, ComponentExcuteFunction func)
        {
            string sourceText = curControl.Text;
            Cursor sourceCursor = changeCursorControl.Cursor;
            // 改变为等待态
            curControl.Text = waitString;
            changeCursorControl.Cursor = Cursors.WaitCursor;
            curControl.Enabled = false;
            // 执行方法
            await func();
            // 恢复为原来的状态
            curControl.Text = sourceText;
            curControl.Enabled = true;
            changeCursorControl.Cursor = sourceCursor;
        }

        /// <summary>
        /// 检查手动连接的输入是否正确
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="connectMethod"></param>
        /// <returns></returns>
        private string CheckIpPortClickInput(string ip, string port, string connectMethod)
        {
            if (ip == "" || !(int.TryParse(port, out _))) return "请输入正确的IP地址和端口号";
            else if (connectMethod == "") return "请选择类型";
            else return null;
        }

        #endregion

        /// <summary>
        /// 设置接口心跳状态
        /// </summary>
        private async Task InterfaceHeatBeat()
        {
            while (!permanentTaskCts.IsCancellationRequested)
            {
                DeviceHeartBeatReturnParam heartBeat = DeviceHeartBeatInterface("设备心跳", "访问设备心跳接口失败");

                isDeviceAlive = !(heartBeat is null);

                // 解析用户设定的休眠时间
                if (!int.TryParse(CallUiSafely.GetControlPropertyValueSafely(HeartbeatUploadRate, c => c.Text), out int time))
                {
                    DataTable dt = curMdb.Find("select heartbeat_rate from ProductConfig where id=1");
                    time = int.Parse(dt.Rows[0]["heartbeat_rate"].ToString());
                }

                //await DelayAndCheckStopAsync(time);
                await Task.Delay(time);
            }
        }

        /// <summary>
        /// 实时读取复位信号
        /// </summary>
        public async Task Recovery()
        {
            while (!permanentTaskCts.IsCancellationRequested)
            {
                // 若断线，保持空转
                if (!isPlcConnected)
                {
                    await Task.Delay(500);
                    continue;
                }

                await Task.Delay(500);

                var result = await TryReadInt16Async(plcAddressInfo.RecoverySignal);
                if (!result.isReadOk || result.value != 1)
                    continue;

                CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "正在复位中……"; c.ForeColor = Color.Blue; });

                // 开始复位。先写这个地址为0，防止重复读取。
                if (!await TryWriteInt16ValueAsync(plcAddressInfo.RecoverySignal, 0))
                {
                    CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = ""; c.ForeColor = Color.Blue; });
                    continue;
                }

                try
                {
                    if (CallUiSafely.GetControlPropertyValueSafely(btnManualClear, c => c.Visible))
                        btnManualClear.PerformClick();

                    /*// 获取目录
                    string localPath = CallUiSafely.GetControlPropertyValueSafely(LocalFilePath, c => c.Text);
                    string picturePath = Path.Combine(localPath, "PrdSNPictures");  // 里面存放SN码命名的文件夹
                    string txtPath = Path.Combine(localPath, "Txt");                // 里面存放SN码命名的文件
                    // 删除两个目录的文件
                    Resource.ForceDeleteFiles(picturePath, true);                   // 删除里面的文件和文件夹
                    Resource.ForceDeleteFiles(txtPath);*/

                    string orderNum = CallUiSafely.GetControlPropertyValueSafely(txtProductModel, c => c.Text);
                    string sql = $"SELECT * FROM PrinterDirectory WHERE order_num='{orderNum}'";
                    DataTable dt = curMdb.Find(sql);

                    if (dt.Rows.Count == 1)
                    {
                        // 确认当前这个type下的文件路径
                        string orderDir = dt.Rows[0]["order_directory"].ToString();
                        string type = dt.Rows[0]["type"].ToString();

                        Dictionary<string, string> paths = new Dictionary<string, string>();
                        string path = dt.Rows[0]["path"].ToString();
                        // mutiple_photo:标签检测,定位检测,封口检测
                        string[] folders = dt.Rows[0]["mutiple_photo"].ToString().Split(',');
                        string[] plcAddresses = dt.Rows[0]["order_directory"].ToString().Split(',');

                        foreach (string folder in folders)
                        {
                            string fullPath = Path.Combine(path, folder, orderDir, type);  // 里面是图片文件
                            Resource.ForceDeleteFiles(fullPath);
                        }
                    }

                    // 清空上一次扫码缓存
                    if (scannedBarcodeList != null)
                    {
                        scannedBarcodeList.Clear();
                        ProductresultList.Clear();
                    }

                    // 重置所有反馈信号
                    if (_readWriteNet != null)
                        await ResetFeedbackSignal();

                    CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "上位机复位完成!"; c.ForeColor = Color.Green; });
                }
                catch (Exception e)
                {
                    HandleOperationError(null, false, "上位机复位异常，请排除错误后再启动机器", $"上位机复位异常:{e}");
                }
            }
        }

        /// <summary>
        /// 重置所有反馈信号，防止下位机有缓存信号未清除（将信号置0）
        /// </summary>
        public async Task ResetFeedbackSignal()
        {
            // 清除所有写入信号
            string[] plcAddrs =
            {
                plcAddressInfo.PrintTrigger,
                plcAddressInfo.PrintFeedback,

                plcAddressInfo.TriggerUpload1,
                plcAddressInfo.ProductResult1,
                plcAddressInfo.Feedback1,

                plcAddressInfo.TriggerUpload2,
                plcAddressInfo.ProductResult2,
                plcAddressInfo.Feedback2,

                plcAddressInfo.TriggerUpload3,
                plcAddressInfo.ProductResult3,
                plcAddressInfo.Feedback3,

                plcAddress.WritePicSignalFirst,
                plcAddress.WritePicSignalSecond,
                plcAddress.WritePicSignalThird,

                plcAddressInfo.HasBarcodeTag,
                plcAddressInfo.BarcodeVerifyTag,
                plcAddressInfo.ManualInputBarcodeTip
            };

            List<Task> writeTasks = new List<Task>();
            foreach (var addr in plcAddrs)
            {
                // 将每个写入操作作为一个 Task
                writeTasks.Add(_readWriteNet.WriteAsync(addr, 0));
            }

            await Task.WhenAll(writeTasks);
        }

        #endregion

        #region ---------- Dynamic Task ----------

        #region 关键参数上传

        /// <summary>
        /// 调用设备程序关键参数接口
        /// <para>（1）更换程序时，上传最新设定参数；</para>
        ///（2）设备程序变更或参数变更后上传关键参数信息。 
        /// </summary>
        /// <returns></returns>
        public void CallKeyArgsInterface()
        {
            while (!isPlcConnected) { if (DelayAndCheckStop(500)) return; }

            string sql = "SELECT * fROM KeyArgsPreserve";
            DataTable allData = curMdb.Find(sql);

            while (true)
            {
                if (DelayAndCheckStop(500)) return;

                if (!isPlcConnected) continue;

                // 检测是否有程序名变更
                ushort length = Convert.ToUInt16(plcAddressInfo.ProgramNameLength);
                if (!TryReadStringValue(plcAddressInfo.DeviceProgramName, length, out string tempProgram))
                    continue;
                if (Program == tempProgram) continue;

                // 判断表和数据有没有发生改变，如果改变了需要重查数据库
                if (keyArgsDataGridObject.Changed)
                {
                    allData = curMdb.Find(sql);
                    keyArgsDataGridObject.Changed = false;
                }

                // 更新程序名的值
                Program = tempProgram;
                JArray jArray = _GetKeyArgsDataJArray(allData);
                // null则说明读取plc失败
                if (jArray is null) continue;

                // 调用接口上传数据
                DeviceKeyArgsInterface(ArgsGetRequestJson(jArray), "设备关键参数", "访问关键参数接口失败");
            }
        }

        /// <summary>
        /// 获取关键参数的json中的data内容
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private JArray _GetKeyArgsDataJArray(DataTable dataTable)
        {
            JArray dataJsonArray = new JArray();

            foreach (DataRow row in dataTable.Rows)
            {
                var json = new JObject
                {
                    // 参数名
                    ["Name"] = row["name"].ToString()
                };

                // 标准值
                if (TryGetProcessedValue(row["standard"].ToString(), out dynamic value))
                    json["Standard"] = value;
                else return null;

                // 下限
                if (TryGetProcessedValue(row["lsl"].ToString(), out value))
                    json["LSL"] = value;
                else return null;

                // 上限
                if (TryGetProcessedValue(row["usl"].ToString(), out value))
                    json["USL"] = value;
                else return null;

                // 单位
                json["Unit"] = row["unit"].ToString();

                dataJsonArray.Add(json);
            }

            return dataJsonArray;
        }

        #endregion

        #region 实时读取设备参数

        private static string barcodeRule = "";     // 条码规则
        public static string Program = "";          // 从PLC获取的程序名
        public static string ProductModel = "";     // 产品型号
        /// <summary>
        /// offLine=1：DOWNTIME机器异常导致机器停止
        /// <para>offLine=2：WAITPREVIOUS前板等待</para>
        /// offLine=3：WAITNEXT后板等待时间
        /// <para>offLine=4：RUN正常运行</para>
        /// offLine=5：POWEROFF断电
        /// <para>offLine=6：STOP类似急停</para>
        /// </summary>
        private static int offLineType;             // 设备状态

        /// <summary>
        /// 实时读取设备参数
        /// <para>1.生产指标：良品数、不良数、生产总数，并根据读取到的指标计算良率</para>
        /// 2.设备状态：停机、运行、空闲
        /// <para>3.产品型号、条码规则、程序名称</para>
        /// </summary>
        private async Task ReadDeviceArgsRealtime()
        {
            int okNum, ngNum, totalNum; double okRate;
            ushort productTypeLength = Convert.ToUInt16(plcAddressInfo.ProductTypeLength);
            ushort barcodeRuleLength = Convert.ToUInt16(plcAddressInfo.BarcodeRuleLength);
            ushort programNameLength = Convert.ToUInt16(plcAddressInfo.ProgramNameLength);

            while (true)
            {
                if (await DelayAndCheckStopAsync(500)) return;

                if (!isPlcConnected) continue;

                okNum = _readWriteNet.ReadInt32(plcAddressInfo.GoodsProducts).Content;                              // 良品数
                ngNum = _readWriteNet.ReadInt32(plcAddressInfo.NotGoodsProducts).Content;                                        // 不良数
                totalNum = _readWriteNet.ReadInt32(plcAddressInfo.ProduceCount).Content;                                         // 生产总数
                okRate = totalNum != 0 ? double.Parse(okNum.ToString()) / double.Parse(totalNum.ToString()) * 100 : 0;         // 良率
                offLineType = _readWriteNet.ReadInt16(plcAddressInfo.DeviceStatus).Content;                                      // 设备状态
                TryReadStringValue(plcAddressInfo.ProductType, productTypeLength, out ProductModel);                             // 产品型号
                TryReadStringValue(plcAddressInfo.BarcodeRule, barcodeRuleLength, out barcodeRule);                              // 条码规则
                TryReadStringValue(plcAddressInfo.DeviceProgramName, programNameLength, out Program);                            // PLC设备使用的程序名称

                if (ProductModel is null || barcodeRule is null || Program is null || !isPlcConnected)
                    continue;

                Invoke(new Action(() =>
                {
                    txtOkQuality.Text = okNum.ToString();                   // 良品数
                    txtNgQuanlity.Text = ngNum.ToString();                  // 不良数
                    txtTotalQuality.Text = totalNum.ToString();             // 生产总数
                    txtYieldRate.Text = $"{Math.Round(okRate, 2)}%";  // 良率
                    txtProductModel.Text = CodeNum.CleanString(ProductModel.Trim().Replace(" ", ""));  // 产品型号
                }));
            }
        }

        #endregion

        #region 工单切换

        /// <summary>
        /// 实时监测PLC型号切换信号，并弹出工单输入界面，输入完成后通知PLC继续生产
        /// </summary>
        public void MonitorModelSwitchFromPlc()
        {
            while (!isPlcConnected) { if (DelayAndCheckStop(500)) return; }

            while (true)
            {
                if (DelayAndCheckStop(500)) return;

                int ModelType = _readWriteNet.ReadInt16(plcAddressInfo.ModelSwitch).Content; //　型号切换
                if (ModelType != 1) continue;

                CallUiSafely.ExecuteControlSafely(ToolingNumber, c => { c.Text = "型号变更请先输入生产信息！"; c.ForeColor = Color.Red; });

                ManageOrderSwitch();
            }
        }

        #endregion

        #region 设备状态上传

        private bool isDeviceAlive;

        /// <summary>
        /// 上传设备状态线程
        /// <para>负责监听设备状态变更（如RUN/STOP），并定时（5min）或在变更时上传至MES</para>
        /// </summary>
        /// <param name="colorLabel">用于显示状态颜色的UI标签</param>
        /// <param name="textLabel">用于显示状态文本的UI标签</param>
        private async Task DeviceStatusUpload(Label colorLabel, Label textLabel)
        {
            // 检查配置：如果没有启用上传，直接结束线程
            if (!EnableDeviceStatusUpload.Checked) return;

            // --- 【改进点1】使用本地变量替代 JObject 记录状态，性能更高，更安全 ---
            string lastType = "POWEROFF";           // 记录上一次的状态
            System.DateTime lastUploadTime = System.DateTime.Now; // 记录上一次上传的时间

            // 用于构建上传数据的 JObject (复用对象或每次新建均可，这里为了逻辑清晰每次新建)
            // 注意：原代码的 UploadDeviceStatusJsonData 似乎依赖引用传递来修改 json 内部的时间，
            // 这里我们为了解耦，只在上传时构建数据。

            while (true)
            {
                try
                {
                    // --- 【改进点2】使用异步延时，释放线程资源 ---
                    // 每 500ms 检查一次
                    if (await DelayAndCheckStopAsync(500)) return;

                    // 获取当前状态
                    // 注意：Invoke 必须用于跨线程访问 UI 控件
                    string currentType = (string)this.Invoke(new Func<string>(() => ChangeDeviceStatusLabel(colorLabel, textLabel)));

                    // --- 校验逻辑 ---
                    // 1. 如果状态是 UNKNOWN，不上传
                    // 2. 如果用户中途取消了勾选，不上传 (continue 而不是 return，允许用户重新勾选恢复)
                    if (currentType.Equals("UNKNOWN") || !EnableDeviceStatusUpload.Checked)
                    {
                        continue;
                    }

                    // 计算距离上次上传过去了多少分钟
                    double timeSpanMinutes = (System.DateTime.Now - lastUploadTime).TotalMinutes;
                    bool statusChanged = !currentType.Equals(lastType);

                    // --- 触发上传的条件：状态发生改变 OR 超过5分钟未上传 ---
                    if (statusChanged || timeSpanMinutes >= 5.0)
                    {
                        // 构建上传所需的数据对象
                        // 此时才创建 JObject，而不是一直维护它
                        JObject uploadData = new JObject
                        {
                            { "Type", currentType },
                            { "LastType", lastType }, // 告诉接口上一个状态是什么
                            { "DateTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                            // 计算持续时间(毫秒)：当前时间 - 上次上传时间
                            { "Interval", (System.DateTime.Now - lastUploadTime).TotalMilliseconds },
                            // 兼容旧代码逻辑需要的字段（如果 UploadDeviceStatusJsonData 内部依赖这些字段）
                            { "LastTypeTime", lastUploadTime.ToString("yyyy-MM-dd HH:mm:ss.fff") }
                        };

                        // 执行上传操作
                        // 注意：这里假设 UploadDeviceStatusJsonData 是同步方法，如果它内部有IO操作，建议也改为 async
                        UploadDeviceStatusJsonData(uploadData, currentType);

                        // --- 更新本地状态 ---
                        lastType = currentType;
                        lastUploadTime = System.DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    // --- 【改进点3】增加异常捕获 ---
                    // 防止某个瞬间解析错误导致整个监控线程退出
                    Log4netHelper.Error($"设备状态上传线程异常: {ex.Message}");
                    // 发生异常时短暂休眠，避免死循环刷日志
                    await Task.Delay(1000);
                }
            }
        }

        /// <summary>
        /// 改变设备状态的颜色和label
        /// </summary>
        /// <param name="colorLabel"></param>
        /// <param name="textLabel"></param>
        /// <returns></returns>
        private string ChangeDeviceStatusLabel(Label colorLabel, Label textLabel)
        {
            switch (offLineType)
            {
                case 1:
                    CallUiSafely.ExecuteControlSafely(colorLabel, c => c.ForeColor = Color.Orange);
                    CallUiSafely.ExecuteControlSafely(textLabel, c => c.Text = "DOWNTIME");
                    break;
                case 2:
                    CallUiSafely.ExecuteControlSafely(colorLabel, c => c.ForeColor = Color.Green);
                    CallUiSafely.ExecuteControlSafely(textLabel, c => c.Text = "WAITPREVIOUS");
                    break;
                case 3:
                    CallUiSafely.ExecuteControlSafely(colorLabel, c => c.ForeColor = Color.Green);
                    CallUiSafely.ExecuteControlSafely(textLabel, c => c.Text = "WAITNEXT");
                    break;
                case 4:
                    CallUiSafely.ExecuteControlSafely(colorLabel, c => c.ForeColor = Color.Green);
                    CallUiSafely.ExecuteControlSafely(textLabel, c => c.Text = "RUN");
                    break;
                case 5:
                    CallUiSafely.ExecuteControlSafely(colorLabel, c => c.ForeColor = Color.Yellow);
                    CallUiSafely.ExecuteControlSafely(textLabel, c => c.Text = "POWEROFF");
                    break;
                case 6:
                    CallUiSafely.ExecuteControlSafely(colorLabel, c => c.ForeColor = Color.Red);
                    CallUiSafely.ExecuteControlSafely(textLabel, c => c.Text = "STOP");
                    break;
                default:
                    CallUiSafely.ExecuteControlSafely(colorLabel, c => c.ForeColor = Color.Black);
                    CallUiSafely.ExecuteControlSafely(textLabel, c => c.Text = "UNKNOWN");
                    break;
            }

            return CallUiSafely.GetControlPropertyValueSafely(textLabel, c => c.Text);
        }

        /// <summary>
        /// 上传设备状态的json数据 (1.状态变更后 2.超过五分钟无变更 -> 需要上传)
        /// </summary>
        /// <param name="json"></param>
        /// <param name="curType"></param>
        private void UploadDeviceStatusJsonData(JObject json, string curType)
        {
            string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";

            // 先进行数据赋值
            json["Type"] = curType;
            json["DateTime"] = System.DateTime.Now.ToString(timeFormat);

            // 如果上一次时间是空的，则说明程序刚启动，Interval只能是0
            if (System.DateTime.TryParse(json["LastTypeTime"].ToString(), out var lastTime))
                json["Interval"] = (System.DateTime.Parse(json["DateTime"].ToString()) - lastTime).TotalMilliseconds;

            DeviceStatusUploadInterface(json, "设备状态", "设备状态变更，数据无法上传至接口");

            // 将本次上传的值赋记录下来
            json["LastType"] = json["Type"];
            json["LastTypeTime"] = json["DateTime"];
        }

        #endregion

        #region 故障、预警信息上传

        /// <summary>
        /// 调用设备故障和预警数据上传接口
        /// </summary>
        /// <returns></returns>
        public void CallDeviceErrorUpload()
        {
            if (!EnableWarningUpload.Checked) return;

            while (!isPlcConnected) { if (DelayAndCheckStop(500)) return; }

            string findAllSql = "select * from ErrorReferenceTable";
            DataTable errorReferenceTable = curMdb.Find(findAllSql);
            // 获取所有用户设置过的plc地址用于循环
            var query = from row in errorReferenceTable.AsEnumerable() select row.Field<string>("plc_point");
            List<string> plcDatas = query.ToList();
            // 定义一个可以存储故障的队列
            List<ErrorWaringEntity> errorQueue = new List<ErrorWaringEntity>();

            while (true)
            {
                if (DelayAndCheckStop(500)) return;
                if (!isPlcConnected) continue;

                //判断用户有没有改变单元格的值，如改变了则重新获取数据库的值，不能读取用户的值，可能用户没保存和数据库不同步
                if (errorPreserveDataGridObject.Changed)
                {
                    errorReferenceTable = curMdb.Find(findAllSql);
                    query = from row in errorReferenceTable.AsEnumerable() select row.Field<string>("plc_point");
                    plcDatas = query.ToList();
                    errorPreserveDataGridObject.Changed = false;
                }

                //循环用户设定的所有plc地址
                foreach (var address in plcDatas)
                {
                    //从plc获取数据
                    if (!TryReadInt16Value(address, out int value) || value == -1)
                    {
                        continue;
                    }

                    // 判断数据有没有必要上传
                    int isUpload = _ErrorIsUpload(address, value, errorReferenceTable, errorQueue, out JObject json, out ErrorWaringEntity errorWaringEntity);

                    switch (isUpload)
                    {
                        case -1:
                            writeLog.WriteLogToComponent(rtbErrorLog, string.Format(programLogString, "上传预警信息过程中无法向本地数据库添加数据"));
                            break;
                        case 1:
                            //将数据上传接口
                            DeviceErrorReturnParam uploadResult = DeviceErrorMessageUploadInterface(json, "设备预警信息", "预警信息接口数据无法上传");
                            if (!(uploadResult is null) && uploadResult.Result != "Pass")
                                writeLog.WriteLogToComponent(rtbErrorLog, string.Format(programLogString, $"plc地址{address}接口数据无法上传"));
                            else
                            {
                                if (errorQueue.Contains(errorWaringEntity))  //说明当前对象需要从队列中移除
                                {
                                    //找到之前存储在队列中的对象索引
                                    int index = errorQueue.IndexOf(errorWaringEntity);
                                    json["errorID"] = errorQueue[index].errorId;
                                    //将队列中的预警信息移除
                                    errorQueue.RemoveAt(index);
                                }
                                else //说明当前对象不在队列
                                {
                                    //将本次预警添加进队列
                                    errorQueue.Add(errorWaringEntity);
                                }
                            }
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否需要上传预警数据
        /// </summary>
        /// <param name="plcAdress">plc地址</param>
        /// <param name="plcValue">通过plc地址获取到的值</param>
        /// <param name="errorReferenceTable">用户添加的数据到errorReferenceTable数据表中的DataTable数据</param>
        /// <param name="errorQueue">记录故障对象的消息队列</param>
        /// <param name="json"></param>
        /// <param name="errorWaringEntity"></param>
        /// <returns>1:正常上传 ，0:不用上传，-1:数据库插入数据错误</returns>
        private int _ErrorIsUpload(string plcAdress, int plcValue, DataTable errorReferenceTable, List<ErrorWaringEntity> errorQueue, out JObject json, out ErrorWaringEntity errorWaringEntity)
        {
            // dataType     数据类型：Alert：预警，Alarm:故障
            // errorType    给接口处理的标识类型：Occur：发生，Clear:清除
            // errorID      故障id（errorMessage表中的数据）
            // errorCode    故障代码（errorReferenceTable表中的数据）
            // errorMessage 故障名称（errorReferenceTable表中的数据)

            //创建json数据
            json = new JObject
            {
                {"DataType", ""},
                {"ErrorType","" },
                {"ErrorID","" },
                {"ErrorCode","" },
                {"ErrorMessage","" },
                {"ErrorClearUser","清除故障员工" },
            };
            //定义出plc读取到的值的对应信息
            Dictionary<int, string> DataType = new Dictionary<int, string>
            {
                { 1, "Alert" },  //预警
                { 2, "Alarm" },  //故障
            };
            ErrorWaringEntity exampleMessage;  //预警故障信息

            //0:正常，1:预警，2:故障, 同一个pcl地址只有01，02，10，20这样的信号，不可能出现12，21，11，22这样的信号
            if (plcValue == 0)  //从plc得到0，并且错误队列里面包含这个错误，一定是清除错误的
            {
                //循环遍历DataType，方便动态添加ErrorWaringEntity对象的dataType值
                for (int i = 1; i <= 2; i++)
                {
                    exampleMessage = new ErrorWaringEntity { plcAddress = plcAdress, dataType = DataType[i] };
                    if (!errorQueue.Contains(exampleMessage)) continue;
                    //开始创建数据
                    json["dataType"] = DataType[i];
                    DataRow[] foundRows = errorReferenceTable.Select($"plc_point = '{plcAdress}'");
                    json["errorCode"] = (string)foundRows[foundRows.Length - 1]["error_code"];
                    json["errorMessage"] = (string)foundRows[foundRows.Length - 1]["error_Name"];
                    json["errorType"] = "Clear";
                    errorWaringEntity = new ErrorWaringEntity { plcAddress = plcAdress, dataType = DataType[i] };
                    return 1;
                }
            }

            //从plc得到1或2，并且错误列表中不包含这个地址的预警，一定是新触发的
            else if (plcValue == 2 || plcValue == 1)
            {
                exampleMessage = new ErrorWaringEntity { plcAddress = plcAdress, dataType = DataType[plcValue] };
                //不是第一次触发plc值改变的情况，持续为1和2，未到故障或预警消除，不需要上传
                if (errorQueue.Contains(exampleMessage))
                {
                    errorWaringEntity = new ErrorWaringEntity();
                    return 0;
                }

                //第一次触发，需要添加数据进json，并获取添加数据对象到队列
                DataRow[] foundRows = errorReferenceTable.Select($"plc_point = '{plcAdress}'");
                json["errorCode"] = (string)foundRows[foundRows.Length - 1]["error_code"];
                json["errorMessage"] = (string)foundRows[foundRows.Length - 1]["error_Name"];
                json["dataType"] = DataType[plcValue];
                json["errorType"] = "Occur";
                json["errorID"] = _ByPlcAdressInsertToDbGetId(plcAdress, errorReferenceTable);
                if (json["errorID"]?.ToString() == "")
                {
                    errorWaringEntity = new ErrorWaringEntity();
                    return -1;
                }
                errorWaringEntity = new ErrorWaringEntity { plcAddress = plcAdress, errorId = json["errorID"]?.ToString(), dataType = DataType[plcValue] };
                return 1;
            }
            errorWaringEntity = new ErrorWaringEntity();
            return 0;
        }

        #endregion

        #region 实时参数上传

        /// <summary>
        /// 调用实时参数接口
        /// </summary>
        /// <returns></returns>
        private void CallRealtimeArgsInterface()
        {
            if (!EnableRealtimeArgsUpload.Checked) return;

            while (!isPlcConnected) { if (DelayAndCheckStop(500)) return; }
            string sql = "select * from Board";
            DataTable allData = curMdb.Find(sql);
            DataTable dt = curMdb.Find("select KA_upload_rate from ProductConfig where id=1");
            string uploadRate = dt.Rows[0]["KA_upload_rate"].ToString();
            while (true)
            {
                //按照用户的设定时间定时上传
                if (DelayAndCheckStop(int.Parse(uploadRate))) return;

                if (!isPlcConnected) continue;

                //判断系统设置板有没有发生改变，如果改变了需要重查数据库
                if (gatherDataGridObject.Changed)
                {
                    allData = curMdb.Find(sql);
                    gatherDataGridObject.Changed = false;
                }

                JArray jArray = _GetRealtimeArgsDataLst(allData);
                //null则说明读取plc失败
                if (jArray is null) continue;

                //调用接口上传数据
                DeviceRealtimeArgsInterface(ArgsGetRequestJson(jArray), "设备程序实时参数", "访问设备程序实时参数接口失败");
            }
        }

        /// <summary>
        /// 获取实时参数的json中的data内容
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private JArray _GetRealtimeArgsDataLst(DataTable dataTable)
        {
            JArray dataJsonArray = new JArray();

            foreach (DataRow row in dataTable.Rows)
            {
                var json = new JObject
                {
                    // 参数名
                    ["Name"] = row["BoardName"].ToString()
                };

                // 实际值
                if (TryGetProcessedValue(row["BoardCode"].ToString(), out dynamic value))
                    json["Value"] = value;
                else return null;

                // 标准值
                if (TryGetProcessedValue(row["StandardCode"].ToString(), out value))
                    json["Standard"] = value;
                else return null;

                // 下限
                if (TryGetProcessedValue(row["MinBoardCode"].ToString(), out value))
                    json["LSL"] = value;
                else return null;

                // 上限
                if (TryGetProcessedValue(row["MaxBoardCode"].ToString(), out value))
                    json["USL"] = value;
                else return null;

                // 单位
                json["Unit"] = row["BoardA1"].ToString();
                dataJsonArray.Add(json);
            }

            return dataJsonArray;
        }

        #endregion

        #region 打印线程

#if USE_LABLEMANAGER

        /// <summary>
        /// 条码打印线程（优化版：开机即预热）
        /// </summary>
        public void CallPrintBarCode()
        {
            // 定义 Codesoft 对象（作用域提升至整个方法）
            ApplicationClass csApp = null;
            Document doc = null;

            try
            {
                // ==================== 0. 【新增】预加载阶段 ====================
                // 在等待 PLC 连接的空隙，先行初始化打印软件，利用“垃圾时间”
                try
                {
                    writeLog.WriteLogToComponent(PrinterSignal, "正在后台预加载打印引擎...");

                    // 初始化 Codesoft
                    csApp = new LabelManager2.ApplicationClass
                    {
                        Visible = false // 防止弹窗干扰
                    };

                    // 获取路径配置
                    string filename = CallUiSafely.GetControlPropertyValueSafely(printTemplatePath, c => c.Text);
                    string printer = CallUiSafely.GetControlPropertyValueSafely(printerName, c => c.Text);

                    if (File.Exists(filename))
                    {
                        csApp.Documents.Open(filename, true);
                        doc = csApp.ActiveDocument;
                        if (!string.IsNullOrEmpty(printer))
                        {
                            doc.Printer.SwitchTo(printer);
                        }
                        writeLog.WriteLogToComponent(PrinterSignal, "打印引擎预加载完成，随时待命！");
                    }
                    else
                    {
                        writeLog.WriteLogToComponent(rtbErrorLog, "预加载跳过：模板文件未找到");
                    }
                }
                catch (Exception ex)
                {
                    // 预加载失败不应该阻断线程，后面主循环有自愈机制会重试
                    writeLog.WriteLogToComponent(rtbErrorLog, $"打印引擎预加载遇到轻微异常(将在主循环重试): {ex.Message}");
                    // 确保半途而废的对象被清理，防止干扰后续逻辑
                    csApp = null;
                    doc = null;
                }

                // ==================== 1. 等待 PLC 连接 ====================
                while (!isPlcConnected)
                {
                    // 稍微降低日志频率，避免刷屏
                    // writeLog.WriteLogToComponent(PrinterSignal, $"打印就绪，等待PLC连接..."); 
                    if (DelayAndCheckStop(500)) return; // 这里 return 会触发 finally 里的资源释放
                }

                // ==================== 2. 主业务循环 ====================
                while (true)
                {
                    // 线程停止检测
                    if (DelayAndCheckStop(100)) return;

                    try
                    {
                        // -------------------- 3. 【自愈机制】确保 Codesoft 可用 --------------------
                        // 如果预加载成功，这里会直接跳过；如果预加载失败或中途崩溃，这里会尝试重建
                        if (csApp == null || doc == null)
                        {
                            try
                            {
                                // 销毁旧对象（双重保险）
                                if (csApp != null) try { csApp.Quit(); }
                                    catch
                                    {
                                        // ignored
                                    }

                                csApp = new LabelManager2.ApplicationClass
                                {
                                    Visible = false
                                };

                                string filename = CallUiSafely.GetControlPropertyValueSafely(printTemplatePath, c => c.Text);
                                if (!File.Exists(filename))
                                {
                                    writeLog.WriteLogToComponent(PrinterSignal, "模板文件不存在，暂停5秒");
                                    Thread.Sleep(5000);
                                    continue;
                                }

                                csApp.Documents.Open(filename, true);
                                doc = csApp.ActiveDocument;
                                doc.Printer.SwitchTo(CallUiSafely.GetControlPropertyValueSafely(printerName, c => c.Text));

                                writeLog.WriteLogToComponent(PrinterSignal, "打印引擎初始化/恢复成功");
                            }
                            catch (Exception ex)
                            {
                                writeLog.WriteLogToComponent(rtbErrorLog, $"初始化打印机失败: {ex.Message}");
                                csApp = null; // 置空以触发下次重试
                                Thread.Sleep(3000);
                                continue;
                            }
                        }

                        // -------------------- 4. 读取 PLC 触发信号 --------------------
                        // (此处逻辑保持不变，为节省篇幅略去部分日志)
                        if (!TryReadInt16Value(plcAddressInfo.PrintTrigger, out int triggerValue))
                        {
                            Thread.Sleep(100); // 读取失败稍微歇一下
                            continue;
                        }

                        if (triggerValue != 1)
                        {
                            if (triggerValue == 2)
                            {
                                HandleOperationError(plcAddressInfo.PrintFeedback, false, "工位2过站异常，取消本次打印");
                            }
                            // 确保复位
                            TryWriteInt16Value(plcAddressInfo.PrintFeedback, 0);
                            continue;
                        }

                        // ==================== 开始打印业务 ====================

                        // 5. 获取条码
                        ushort length = Convert.ToUInt16(plcAddressInfo.BarcodeToPrintLenght);
                        if (!TryReadStringValue(plcAddressInfo.BarcodeToPrint, length, out string sn2UploadMes4Print))
                            continue;

                        writeLog.WriteLogToComponent(PrinterSignal, $"收到打印请求: {sn2UploadMes4Print}");
                        CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = $"正在处理: {sn2UploadMes4Print}"; c.ForeColor = Color.Blue; });

                        // 6. 调用 MES 接口
                        var json = new JObject
                        {
                            {"PlanNo", CallUiSafely.GetControlPropertyValueSafely(OrderNo, c=>c.Text) },
                            {"PrdSN", sn2UploadMes4Print },
                            {"Employee",  CallUiSafely.GetControlPropertyValueSafely(txtUser, c=>c.Text) }
                        };

                        PrintBarCodeReturnParam barCodeParam = PrintBarCodeInterface(json, "打印接口", "访问打印接口失败");

                        bool isSuccess = false;
                        string failReason;

                        if (barCodeParam != null && barCodeParam.Result == "Pass")
                        {
                            // 7. 传参
                            if (barCodeParam.PrintParameterList.Count > 0)
                            {
                                foreach (var variable in barCodeParam.PrintParameterList[0]["PrintParameter"])
                                {
                                    string key = variable["ParameterName"].ToString();
                                    string val = variable["ParameterValue"].ToString();
                                    if (doc.Variables.FreeVariables.Item(key) != null)
                                        doc.Variables.FreeVariables.Item(key).Value = val;
                                }
                            }

                            // 8. 打印
                            doc.PrintDocument();
                            // doc.FormFeed(); 
                            isSuccess = true;

                            CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = $"{sn2UploadMes4Print} 打印完成"; c.ForeColor = Color.Green; });
                            writeLog.WriteLogToComponent(PrinterSignal, "打印指令已发送");
                        }
                        else
                        {
                            failReason = barCodeParam == null ? "接口返回空" : barCodeParam.ErrorMessage;
                            CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = $"打印失败: {failReason}"; c.ForeColor = Color.Red; });
                            writeLog.WriteLogToComponent(PrinterSignal, $"打印失败: {failReason}");
                        }

                        // 9. 反馈 PLC
                        int feedbackValue = isSuccess ? 1 : 2;
                        if (!TryWriteInt16Value(plcAddressInfo.PrintFeedback, (short)feedbackValue))
                        {
                            writeLog.WriteLogToComponent(PrinterSignal, "严重错误：无法写入反馈信号给PLC");
                        }

                        // 10. 等待 PLC 复位 (防重复打印)
                        writeLog.WriteLogToComponent(PrinterSignal, "等待 PLC 复位信号...");
                        int waitTimeOut = 0;
                        while (true)
                        {
                            if (DelayAndCheckStop(200)) return;

                            if (TryReadInt16Value(plcAddressInfo.PrintTrigger, out int currentDtu))
                            {
                                if (currentDtu == 0)
                                {
                                    TryWriteInt16Value(plcAddressInfo.PrintFeedback, 0);
                                    writeLog.WriteLogToComponent(PrinterSignal, "流程闭环完成");
                                    break;
                                }
                            }

                            waitTimeOut++;
                            if (waitTimeOut > 50) // 10秒超时
                            {
                                writeLog.WriteLogToComponent(PrinterSignal, "警告：PLC 复位信号超时，强制重置");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        writeLog.WriteLogToComponent(PrinterSignal, $"打印线程异常: {ex.Message}");
                        csApp = null;
                        doc = null;
                        TryWriteInt16Value(plcAddressInfo.PrintFeedback, 2);
                        Thread.Sleep(2000);
                    }
                }
            }
            finally
            {
                // ==================== 资源释放 ====================
                // 无论线程如何退出（return/异常/停止信号），finally 都会执行
                // 确保不会残留 Codesoft 进程
                try
                {
                    if (doc != null)
                    {
                        doc.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                    }
                    if (csApp != null)
                    {
                        csApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(csApp);
                    }
                    writeLog.WriteLogToComponent(PrinterSignal, "打印线程已退出，资源已释放");
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// 读取PLC信号获取上传图片，将图片移动到{PrdSN}文件夹下
        /// </summary>
        public void MovePictureGroup()
        {
            while (!isPlcConnected) { if (DelayAndCheckStop(500)) return; }

            List<ReadPictureEntity> plcAddresses = new List<ReadPictureEntity>
            {
                new ReadPictureEntity
                {
                    ReadPlcAddress = plcAddress.ReadPicSignalFirst,
                    WritePlcAddress = plcAddress.WritePicSignalFirst,
                    ReadSNPlcAddress = plcAddress.ReadSNFirst
                },
                new ReadPictureEntity
                {
                    ReadPlcAddress = plcAddress.ReadPicSignalSecond,
                    WritePlcAddress = plcAddress.WritePicSignalSecond,
                    ReadSNPlcAddress = plcAddress.ReadSNSecond
                },
                new ReadPictureEntity
                {
                    ReadPlcAddress = plcAddress.ReadPicSignalThird,
                    WritePlcAddress = plcAddress.WritePicSignalThird,
                    ReadSNPlcAddress = plcAddress.ReadSNThird
                },
            };

            while (true)
            {
                if (DelayAndCheckStop(500)) return;

                try
                {
                    foreach (ReadPictureEntity plcAddr in plcAddresses)
                    {
                        // 当前存在错误，暂停执行
                        if (existErrorInErrorTip) continue;

                        // 勾选启动图片处理
                        // if (!EnableHandlerPicture.Checked)
                        // {
                        //     WriteInt16ValueToPlc(FeedBackAddress.feedbackPoint, 1);
                        //     continue;
                        // }

                        if (!TryReadInt16Value(plcAddr.ReadPlcAddress, out int value) || value != 1) continue;

                        if (!TryReadStringValue(plcAddr.ReadSNPlcAddress.Address, plcAddr.ReadSNPlcAddress.Length, out string prdSN))
                        {
                            writeLog.WriteLogToComponent(rtbErrorLog, "无法读取PLC条码");
                            break;
                        }

                        try
                        {
                            MoveFile(prdSN, plcAddr.ReadPlcAddress);
                            TryWriteInt16Value(plcAddr.WritePlcAddress, 1);
                        }
                        catch (Exception e)
                        {
                            Log4netHelper.Error($"移动图片失败:{e}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log4netHelper.Error($"移动图片过程中出现错误:{e}");
                }
            }
        }

#endif

        #endregion

        #endregion

        #region ---------- 读取条码 ----------

        private static readonly object _lockObject = new object();
        private static string FixtureCode = "";

        /// <summary>
        /// 读取条码
        /// </summary>
        private void ProcessPlc_ReadBarcode()
        {
            // 循环等待PLC连接
            while (!isPlcConnected)
            {
                writeLog.WriteLogToComponent(rtbReadBarCode, "等待PLC连接……");

                if (DelayAndCheckStop(500)) return;
            }

            bool isEnableManualInputBarcode = false;         // 需要手动输入条码标志：true按钮已启用，false为未启用，默认为false；

            while (true)
            {
                if (DelayAndCheckStop(100)) return;     // 检测是否应该退出当前任务
                if (existErrorInErrorTip) continue;          // 当前有错误，需要先清除错误后再次访问

                writeLog.WriteLogToComponent(rtbReadBarCode, $"持续监测'{plcAddressInfo.HasBarcodeTag}'信号中...");

                int triggerValue = _readWriteNet.ReadInt16(plcAddressInfo.HasBarcodeTag).Content;    // 条码标识（1=触发条码验证）
                var barcodeType = _readWriteNet.ReadInt16(plcAddressInfo.BarcodeType).Content;       // 条码类型 (1=产品条码, 2=工装条码)

                if (triggerValue == 1)
                {
                    // 如果检查到新的扫码成功信号，则自动屏蔽掉手动输入
                    if (isEnableManualInputBarcode)
                    {
                        isEnableManualInputBarcode = false;
                        CallUiSafely.ExecuteControlSafely(btnManualInputBarcode, c => c.Visible = false);
                    }

                    try
                    {
                        Log4netHelper.Info($"从{plcAddressInfo.HasBarcodeTag}检测到扫码读码信号=1");

                        // 首先清除触发信号
                        _readWriteNet.Write(plcAddressInfo.HasBarcodeTag, 0);
                        Log4netHelper.Info($"清除扫码读码信号:{plcAddressInfo.HasBarcodeTag}=0");

                        writeLog.WriteLogToComponent(rtbReadBarCode, $"监测到来自'{plcAddressInfo.HasBarcodeTag}'的信号:{triggerValue}");

                        HandlePlcScanRequest(barcodeType);

                        writeLog.WriteLogToComponent(rtbReadBarCode, $"来自'{plcAddressInfo.HasBarcodeTag}'的信号处理完成");
                    }
                    catch (Exception e)
                    {
                        HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, $"扫码读取异常:${e.Message}");
                        writeLog.WriteLogToComponent(rtbReadBarCode, $"来自'{plcAddressInfo.HasBarcodeTag}'的信号处理异常");
                    }
                }
                else
                {
                    // 更新当前状态，
                    isEnableManualInputBarcode = false;

                    // 手动输入条码信号=1
                    TryReadInt16Value(plcAddressInfo.ManualInputBarcodeTip, out int value);

                    if (value == 1)
                    {
                        //HandleOperationError(null, false, "扫码失败，请手动输入条码");
                        CallUiSafely.ExecuteControlSafely(btnManualInputBarcode, c => c.Visible = true);
                        isEnableManualInputBarcode = true;
                    }
                }
            }
        }

        /// <summary>
        /// 处理PLC扫码请求的核心业务逻辑
        /// </summary>
        /// <param name="barcodeType">条码类型 1=产品条码  2=工装条码</param>
        private void HandlePlcScanRequest(int barcodeType)
        {
            lock (_lockObject)
            {
                // 判断PLC是否连接,条码标识,是否启用读码功能
                //if (!plcConn || Read_data != 1 || ShieldBarcode.Checked) return;

                // --- 1. 初始化 --- 
                CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "准备读码"; c.ForeColor = Color.Green; });
                CallUiSafely.ExecuteControlSafely(ProductResult, c => { c.Text = "待机"; c.ForeColor = Color.Black; c.BackColor = Color.White; });

                // --- 2. 从PLC读取条码 ---
                ushort barcodeLength = Convert.ToUInt16(plcAddressInfo.PlcScannedBarcodeLength);
                if (!TryReadStringValue(plcAddressInfo.PlcScannedBarcode, barcodeLength, out string scannedBarcode))
                {
                    HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, "无法读取PLC条码信息，请检查连接");
                    return;
                }

                writeLog.WriteLogToComponent(rtbReadBarCode, $"读取条码{scannedBarcode}");
                Log4netHelper.Info($"{plcAddressInfo.PlcScannedBarcode}读取到条码：{scannedBarcode}");

                // --- 3. 业务分支：工装条码 (Type 2) ---
                if (barcodeType == 2)  // 560220-01621-DP-V01-002
                {
                    HandleToolingBarcode(scannedBarcode);
                    return; // 工装条码处理完毕，流程结束
                }

                // --- 4. 业务分支：产品条码 (Type 1 或其他) ---

                // 4a. 本地校验 1：产品型号校验 (如果启用)
                if (EnableTypeChangedVerify.Checked)
                {
                    if (!VerifyProductModelMatch(scannedBarcode)) return;
                }

                // 更新UI显示当前条码
                CallUiSafely.ExecuteControlSafely(barCode, c => c.Text = scannedBarcode);

                // 4b. 本地校验 2：条码规则校验 (如果启用)
                if (EnableBarcodeRuleVerify.Checked && BarcodeRule.Text != "" && scannedBarcode.IndexOf(BarcodeRule.Text, StringComparison.Ordinal) < 1)
                {
                    // 如果在条码中“找不到规则字符串”(-1)，或者“规则字符串在最开头”(0)，则视为校验失败。
                    HandleBarcodeRuleMismatch();
                    return;
                }

                #region ---------- MES交互1 - 获取拼板条码 ----------

                // 4c. MES校验1：获取拼板（默认情况下，列表里只有刚刚扫到的"板边码"）
                List<PrdSNs> PrdSNInfo = new List<PrdSNs>
                {
                    new PrdSNs { PrdSN = scannedBarcode }// 将当前扫描的条码作为列表的第一项
                };

                // 启用拼板条码
                if (EnableGetNextBoard.Checked)
                {
                    if (!TryGetPanelizationBarcodes(ref PrdSNInfo, scannedBarcode))
                    {
                        return;
                    }
                }

                #endregion

                #region ---------- MES交互2 - 流程检查 ----------

                // 从获取到的子板码集合中筛选出子板条码，不包含子板序号
                List<string> snList = PrdSNInfo.Select(item => item.PrdSN).ToList();

                PrdSNCollection snCollection = new PrdSNCollection { PrdSN = snList };

                // 4d. MES校验2：流程检查 (如果启用)
                if (EnableFluentVerify.Checked)
                {
                    if (!CheckRouteWithMes(ref snList, ref snCollection, scannedBarcode))
                    {
                        return;
                    }

                    // (如果启用了拼板，则把另一个条码发给PLC)
                    if (EnableGetNextBoard.Checked)
                    {
                        if (!TrySendAnotherBarcodeToPlc(snList, scannedBarcode))
                            return;
                    }

                    // 通知PLC继续生产
                    _readWriteNet.Write($"{plcAddressInfo.BarcodeVerifyTag}", 1);
                    Log4netHelper.Info($"{scannedBarcode}流程检查：流程检查成功，通知plc继续生产:{plcAddressInfo.BarcodeVerifyTag}=1");
                }
                else
                {
                    // 如果不启用流程检查，则直接向PLC反馈OK
                    BypassRouteCheck(scannedBarcode);
                }

                #endregion

                #region ---------- 启用上工装机程序 ----------

                // 4e. 缓存条码 (上工装机模式)
                // 勾选一并提交也需要Read_Barcodes里面的数据
                if (EnableUpperTooling.Checked && !scannedBarcodeList.Contains(scannedBarcode))
                {
                    scannedBarcodeList.AddRange(snList);
                }

                #endregion
            }
        }

        /// <summary>
        /// 处理工装条码的逻辑：缓存工装编号并向PLC反馈OK。
        /// </summary>
        /// <param name="toolingBarcode">读取到的工装条码</param>
        private void HandleToolingBarcode(string toolingBarcode)
        {
            FixtureCode = toolingBarcode; // 缓存工装编号，用于后续过站
            CallUiSafely.ExecuteControlSafely(ToolingNumber, c => c.Text = FixtureCode);

            // 反馈读码完成信号给PLC [ 工装编号  ]

            Log4netHelper.Info($"工装条码:{toolingBarcode} 向{plcAddressInfo.BarcodeVerifyTag}写入: 1");
            _readWriteNet.Write(plcAddressInfo.BarcodeVerifyTag, 1);
        }

        /// <summary>
        /// 验证扫描的条码是否与当前选择的产品型号匹配
        /// <para>原理：查询数据库中与当前产品型号相关联的前缀，检查条码是否包含该前缀。</para>
        /// </summary>
        /// <param name="scannedBarcode">扫码枪读取到的完整条码字符串</param>
        /// <returns>验证通过返回 true，失败（包括数据库无记录或不匹配）返回 false</returns>
        private bool VerifyProductModelMatch(string scannedBarcode)
        {
            // 原始验证逻辑
            /*string currentModel = CallUiSafely.GetControlPropertyValueSafely(txtProductModel, c => c.Text);
            string sqlQuery = $"select product_type,barcode_match from ChangeProductType where product_type='{currentModel}'";
            DataTable dt = curMdb.Find(sqlQuery);
            string[] barcodeMatch = dt.Select("").Select(row => row["barcode_match"].ToString()).ToArray();

            foreach (string barcode in barcodeMatch)
            {
                if (scannedBarcode.StartsWith(barcode))
                {
                    CallUiSafely.ExecuteControlSafely(OperTip, c => { c.ForeColor = Color.Green; c.Text = "产品型号验证通过"; });
                    return true;
                }
            }

            return HandleOperationError(plcAddress.Feedback1, true, $"条码{scannedBarcode}产品型号验证不通过");*/

            // 1.获取运行界面中当前生产的产品型号
            string currentModel = CallUiSafely.GetControlPropertyValueSafely(this.txtProductModel, c => c.Text);

            // 2.[参数校验]如果界面没选型号，直接报错
            if (string.IsNullOrEmpty(currentModel))
            {
                return HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, "未选择产品型号，无法进行校验");
            }

            // 3.查询该型号对应的条码匹配规则
            string sqlQuery = $"select product_type,barcode_match from ChangeProductType where product_type='{currentModel}'";

            // 4. [数据查询] 执行查询，获取结果集 DataTable
            DataTable matchTable = curMdb.Find(sqlQuery);

            // 5. [空值检查] 如果数据库没查到该型号的配置，视为校验失败
            if (matchTable == null || matchTable.Rows.Count == 0)
            {
                return HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, $"数据库中未找到型号[{currentModel}]的条码规则配置");
            }

            // 6. [规则遍历] 遍历查到的所有匹配规则（可能该型号支持多种条码格式）
            foreach (DataRow row in matchTable.Rows)
            {
                // 获取规则字符串，例如 "SN-2025"
                string matchRule = row["barcode_match"].ToString();

                // 忽略空规则，防止逻辑误判
                if (string.IsNullOrEmpty(matchRule)) continue;

                // 7. [核心比对] 检查扫描条码是否以规则开头，或包含规则字符
                // 优化逻辑：StartsWith 效率略高于 Contains，且业务上通常是前缀匹配，两者结合容错率更高
                if (scannedBarcode.StartsWith(matchRule))
                {
                    // 8. [成功反馈] 更新界面提示为绿色，并显示具体匹配信息
                    CallUiSafely.ExecuteControlSafely(lblOperTip, c =>
                    {
                        c.ForeColor = Color.Green;
                        c.Text = "产品型号验证通过";
                    });

                    // 找到匹配项即立即返回，无需继续循环
                    return true;
                }
            }

            // 9. [失败处理] 循环结束仍未匹配，调用错误处理方法
            // 通知 PLC (写入NG信号) 并记录错误日志
            return HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, $"条码{scannedBarcode}产品型号验证不通过");
        }

        /// <summary>
        /// 处理本地条码规则校验失败的逻辑。
        /// </summary>
        private void HandleBarcodeRuleMismatch()
        {
            CallUiSafely.ExecuteControlSafely(lblOperTip, c =>
            { c.Text = "验证失败、条码规则验证失败！"; c.ForeColor = Color.Red; });

            HandleOperationError(
                plcAddressInfo.BarcodeVerifyTag,
                isBlockingError: true,
                userMessage: "验证失败、条码规则验证失败！");

            //readWriteNet.Write($"{plcAddress.BarcodeVerifyTag}", 2);
            //LogMsg($"判断条码规则【{plcAddress.BarcodeVerifyTag}】 = 2");
        }

        /// <summary>
        /// (MES 交互 1) 调用MES获取拼板条码。
        /// </summary>
        /// <param name="prdSNs">传入已扫到的子板条码，传出MES返回的完整拼板列表</param>
        /// <param name="scannedBarcode">当前扫码枪读取到的条码</param>
        /// <returns>true 表示成功, false 表示失败 (内部已调用 HandleOperationError)</returns>
        private bool TryGetPanelizationBarcodes(ref List<PrdSNs> prdSNs, string scannedBarcode)
        {
            GetBarCodeInputParameter inputParam = new GetBarCodeInputParameter
            {
                TrackNo = "",  // 轨道号，可为空 
                PrdSN = scannedBarcode
            };

            GetBarCodeReturnParameter mesResponse = _request.GetResponseSerializeResult<GetBarCodeReturnParameter, GetBarCodeInputParameter>
                (UrlPanelization.Text, _httpClient, "GETPRDSNGROUP", inputParam, "获取拼版");

            // 1. 处理接口连接失败
            if (mesResponse == null)
            {
                Log4netHelper.Info($"{scannedBarcode}获取拼版：连接错误，无法获取拼版条码");

                return HandleOperationError(
                    plcAddressInfo.BarcodeVerifyTag, true, "连接错误，无法获取拼版条码");
            }

            // 2. 处理MES返回FAIL
            if (mesResponse.Result == nameof(Result.Fail))
            {
                Log4netHelper.Info($"{scannedBarcode}获取拼版：{mesResponse.ErrorMessage}");

                return HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, $"获取拼版条码错误:{mesResponse.ErrorMessage}");
            }

            // 3. 处理MES返回PASS，但数据不合规（如非拼板）
            if (mesResponse.PrdSNInfo.PrdSNs.Count <= 1)
            {
                Log4netHelper.Info($"{scannedBarcode}获取拼版：获取拼版接口验证通过但没返回拼版条码");

                return HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, "获取拼版接口验证通过但没返回拼版条码");
            }

            // 4. MES 返回 Pass 且数据合规
            CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "拼版条码获取成功!"; c.ForeColor = Color.Green; });

            // 更新拼板列表
            prdSNs = mesResponse.PrdSNInfo.PrdSNs;
            Log4netHelper.Info($"{scannedBarcode}获取拼版：拼版条码获取成功");
            return true;
        }

        /// <summary>
        /// (MES 交互 2) 调用MES进行流程检查。
        /// </summary>
        /// <param name="snList">子板条码集合，不包含子板序号</param>
        /// <param name="snCollection">子板条码集合，不包含子板序号。这是对snList的封装</param>
        /// <param name="scannedBarcode">从PLC获取的条码</param>
        /// <returns>true 表示成功, false 表示失败 (内部已调用 HandleOperationError)</returns>
        private bool CheckRouteWithMes(ref List<string> snList, ref PrdSNCollection snCollection, string scannedBarcode)
        {
            // ----------- 1.构造MES接口输入参数 -----------

            RouteCheckInputParam inputParam = new RouteCheckInputParam
            {
                Employee = txtUser.Text,
                BoardSide = "",  // OP或者BOTTOM ，非强制可为空
                PlanNo = CallUiSafely.GetControlPropertyValueSafely(OrderNo, c => c.Text), //"12025666-40-01",  // OrderNum.Text, //装配机12061441-80-01  原始12033377-40-01
                TrackNo = "",  // 轨道，非强制可为空
                BoardSideSN = snList[0],
                PrdSNCollection = snCollection
            };

            // ----------- 2.调用MES流程检查接口 -----------

            writeLog.WriteLogToComponent(rtbReadBarCode, $"开始访问MES流程检查{scannedBarcode}");
            RouteCheckReturnParam mesResponse = _request.GetResponseSerializeResult<RouteCheckReturnParam,
                                                RouteCheckInputParam>(Url_RouteCheck.Text, _httpClient, "CHECKROUTE", inputParam, "流程检查");
            writeLog.WriteLogToComponent(rtbReadBarCode, $"收到MES流程检查反馈{scannedBarcode}");

            // ----------- 3.处理MES接口返回结果 -----------

            // 3a.接口连接失败
            if (mesResponse == null)
            {
                Log4netHelper.Info($"{scannedBarcode}流程检查：访问接口错误，无法进行流程检查");

                writeLog.WriteLogToComponent(rtbErrorLog, string.Format(programLogString, "访问接口错误，无法进行流程检查"));

                return HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, "访问接口错误，无法进行流程检查");
            }

            // 3b.MES返回FAIL
            if (mesResponse.Result == nameof(Result.Fail))
            {
                Log4netHelper.Info($"{scannedBarcode}流程检查：MES返回非PASS,{mesResponse.ErrorMessage}");

                return HandleOperationError(plcAddressInfo.BarcodeVerifyTag, true, $"流程检查:{mesResponse.ErrorMessage}");
            }

            // 3c.MES返回PASS
            CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "流程检查成功!"; c.ForeColor = Color.Green; });


            return true;
        }

        /// <summary>
        /// 将拼版的另外一个条码发送给PLC
        /// </summary>
        /// <param name="snList"></param>
        /// <param name="scannedBarcode"></param>
        /// <returns></returns>
        private bool TrySendAnotherBarcodeToPlc(List<string> snList, string scannedBarcode)
        {
            // 从拼版列表中找到与当前扫描条码不同的另一个条码
            var anotherBarcode = snList.FirstOrDefault(x => x != scannedBarcode);

            if (string.IsNullOrWhiteSpace(anotherBarcode))
            {
                Log4netHelper.Info($"{scannedBarcode}流程检查：流程检查成功，但是查找的拼版为结果null，无法发送到plc");

                return HandleOperationError(null, false, "流程检查：无法将拼版条码发送给PLC");
            }

            OperateResult result = _readWriteNet.Write(plcAddressInfo.PanalizationBarcode, anotherBarcode);
            Log4netHelper.Info($"{scannedBarcode}流程检查：流程检查成功，拼版条码{anotherBarcode}发送至{plcAddressInfo.PanalizationBarcode},发送状态：{result.IsSuccess}");
            return true;
        }

        /// <summary>
        /// 在未勾选流程检查时，直接向PLC反馈OK。
        /// </summary>
        private void BypassRouteCheck(string readPlcSn)
        {
            CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "跳过流程检查成功!"; c.ForeColor = Color.Green; });

            _readWriteNet.Write($"{plcAddressInfo.BarcodeVerifyTag}", 1);

            Log4netHelper.Info($"{readPlcSn}跳过条码验证成功：{plcAddressInfo.BarcodeVerifyTag}=1");
        }

        #endregion

        #region ---------- 生产数据读取、上传（产品过站） ----------

        List<string> scannedBarcodeList = new List<string>();       // 读取的条码
        List<string> ProductresultList = new List<string>();        // 产品生产结果

        /// <summary>
        /// 非装配机产品过站
        /// </summary>
        private void Procesplc_ReadValue()
        {
            if (Global.Instance.CurDataBaseName == "装配机") return;

            while (!isPlcConnected)
            {
                writeLog.WriteLogToComponent(UploadMes, "等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity
            {
                Name = "产品过站",
                triggerPoint = plcAddressInfo.TriggerUpload1,  // 最终结果上传
                feedbackPoint = plcAddressInfo.Feedback1,
                ProductResult = plcAddressInfo.ProductResult1,
                BarcodeToUpload = plcAddressInfo.BarcodeToUpload1,
                BarcodeToUploadLength = plcAddressInfo.BarcodeToUploadLength1,
                Line = CallUiSafely.GetControlPropertyValueSafely(Line, c => c.Text),
                Process = CallUiSafely.GetControlPropertyValueSafely(Process, c => c.Text),
                Staiton = CallUiSafely.GetControlPropertyValueSafely(Station, c => c.Text),
                Key = CallUiSafely.GetControlPropertyValueSafely(MesKey, c => c.Text),
                Pwd = CallUiSafely.GetControlPropertyValueSafely(Security, c => c.Text),
                Device = CallUiSafely.GetControlPropertyValueSafely(Device, c => c.Text),
                DeleteFile = false
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                writeLog.WriteLogToComponent(UploadMes, "持续读取生产完成信号中……");
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                if (triggerValue != 1 || !isPlcConnected) continue;

                try
                {
                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]监测到'{uploadManager.triggerPoint}'的信号:{triggerValue}");

                    var prdSN = GetProductResult(uploadManager, scannedBarcodeList, ProductresultList);

                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]'{uploadManager.triggerPoint}'处理完成({prdSN})");
                }
                catch (Exception ex)
                {
                    HandleOperationError(uploadManager.feedbackPoint, true, $"生产结果读取异常:${ex.Message}");
                    writeLog.WriteLogToComponent(UploadMes, $"{uploadManager.Name}'{uploadManager.triggerPoint}'的信号处理异常:{ex}");

                    // 直接用新对象，防止值为null
                    scannedBarcodeList = new List<string>();
                    ProductresultList = new List<string>();
                }
            }
        }

        /// <summary>
        /// 装配机产品过站1（装配机工序1:Scan-ASSY）
        /// </summary>
        private void ProcessPlc_ReadValue1()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            while (!isPlcConnected)
            {
                writeLog.WriteLogToComponent(UploadMes, "等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity // 装配机第二次拍照完成上传
            {
                Name = "Scan-ASSY",
                triggerPoint = plcAddressInfo.TriggerUpload1,
                feedbackPoint = plcAddressInfo.Feedback1,
                ProductResult = plcAddressInfo.ProductResult1,
                BarcodeToUpload = plcAddressInfo.BarcodeToUpload1,
                BarcodeToUploadLength = plcAddressInfo.BarcodeToUploadLength1,
                Line = CallUiSafely.GetControlPropertyValueSafely(Line, c => c.Text),
                Process = CallUiSafely.GetControlPropertyValueSafely(Process, c => c.Text),
                Staiton = CallUiSafely.GetControlPropertyValueSafely(Station, c => c.Text),
                Key = CallUiSafely.GetControlPropertyValueSafely(MesKey, c => c.Text),
                Pwd = CallUiSafely.GetControlPropertyValueSafely(Security, c => c.Text),
                Device = CallUiSafely.GetControlPropertyValueSafely(Device, c => c.Text),
                DeleteFile = true,
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                writeLog.WriteLogToComponent(UploadMes, "持续读取生产完成信号中……");
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                if (triggerValue != 1 || isPlcConnected != true) continue;

                try
                {
                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]监测到'{uploadManager.triggerPoint}'的信号:{triggerValue}");

                    var prdSN = GetProductResult(uploadManager, new List<string>(scannedBarcodeList), new List<string>(ProductresultList));

                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]'{uploadManager.triggerPoint}'处理完成({prdSN})");
                }
                catch (Exception ex)
                {
                    HandleOperationError(uploadManager.feedbackPoint, true, $"生产结果读取异常:${ex.Message}");
                    writeLog.WriteLogToComponent(UploadMes, $"{uploadManager.Name}'{uploadManager.triggerPoint}'的信号处理异常:{ex}");
                }
            }
        }

        /// <summary>
        /// 装配机产品过站2（装配机工序2:Weight）
        /// </summary>
        private void ProcessPlc_ReadValue2()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            while (!isPlcConnected)
            {
                writeLog.WriteLogToComponent(UploadMes, "等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity // 装配机第二次拍照完成上传
            {
                Name = "Weight",
                triggerPoint = plcAddressInfo.TriggerUpload2,
                feedbackPoint = plcAddressInfo.Feedback2,
                ProductResult = plcAddressInfo.ProductResult2,
                BarcodeToUpload = plcAddressInfo.BarcodeToUpload2,
                BarcodeToUploadLength = plcAddressInfo.BarcodeToUploadLength2,
                Line = CallUiSafely.GetControlPropertyValueSafely(Line2, c => c.Text),
                Process = CallUiSafely.GetControlPropertyValueSafely(Process2, c => c.Text),
                Staiton = CallUiSafely.GetControlPropertyValueSafely(Station, c => c.Text),
                Key = CallUiSafely.GetControlPropertyValueSafely(MesKey2, c => c.Text),
                Pwd = CallUiSafely.GetControlPropertyValueSafely(Security2, c => c.Text),
                Device = CallUiSafely.GetControlPropertyValueSafely(Device2, c => c.Text),
                DeleteFile = true,
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                writeLog.WriteLogToComponent(UploadMes, "持续读取生产完成信号中……");
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                if (triggerValue != 1 || isPlcConnected != true) continue;

                try
                {
                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]监测到'{uploadManager.triggerPoint}'的信号:{triggerValue}");

                    var prdSN = GetProductResult(uploadManager, new List<string>(scannedBarcodeList), new List<string>(ProductresultList));

                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]'{uploadManager.triggerPoint}'处理完成({prdSN})");
                }
                catch (Exception ex)
                {
                    HandleOperationError(uploadManager.feedbackPoint, true, $"生产结果读取异常:${ex.Message}");
                    writeLog.WriteLogToComponent(UploadMes, $"{uploadManager.Name}'{uploadManager.triggerPoint}'的信号处理异常:{ex}");
                }
            }
        }

        /// <summary>
        /// 装配机产品过站3（装配机工序3:Screw-BA）
        /// </summary>
        private void Procesplc_ReadValue3()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            while (!isPlcConnected)
            {
                writeLog.WriteLogToComponent(UploadMes, "等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity
            {
                Name = "Screw-BA",
                triggerPoint = plcAddressInfo.TriggerUpload3,  //最终结果上传
                feedbackPoint = plcAddressInfo.Feedback3,
                ProductResult = plcAddressInfo.ProductResult3,
                BarcodeToUpload = plcAddressInfo.BarcodeToUpload3,
                BarcodeToUploadLength = plcAddressInfo.BarcodeToUploadLength3,
                Line = CallUiSafely.GetControlPropertyValueSafely(Line3, c => c.Text),  //最终结果上传
                Process = CallUiSafely.GetControlPropertyValueSafely(Process3, c => c.Text),
                Staiton = CallUiSafely.GetControlPropertyValueSafely(Station3, c => c.Text),
                Key = CallUiSafely.GetControlPropertyValueSafely(MesKey3, c => c.Text),
                Pwd = CallUiSafely.GetControlPropertyValueSafely(Security3, c => c.Text),
                Device = CallUiSafely.GetControlPropertyValueSafely(Device3, c => c.Text),
                DeleteFile = false,
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                writeLog.WriteLogToComponent(UploadMes, "持续读取生产完成信号中……");
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                if (triggerValue != 1 || isPlcConnected != true) continue;

                try
                {
                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]监测到'{uploadManager.triggerPoint}'的信号:{triggerValue}");

                    var prdSN = GetProductResult(uploadManager, new List<string>(scannedBarcodeList), new List<string>(ProductresultList));

                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadManager.Name}]'{uploadManager.triggerPoint}'处理完成({prdSN})");
                }
                catch (Exception ex)
                {
                    HandleOperationError(uploadManager.feedbackPoint, true, $"生产结果读取异常:${ex.Message}");
                    writeLog.WriteLogToComponent(UploadMes, $"{uploadManager.Name}'{uploadManager.triggerPoint}'的信号处理异常:{ex}");
                }
            }
        }

        /// <summary>
        /// 获取生产结果
        /// </summary>
        private string GetProductResult(UploadManagerEntity uploadManagerEntity, List<string> scannedBarcodeList, List<string> productResultList)
        {
            string prdSN = "null";
            CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "开始读取数据"; c.ForeColor = Color.Green; });

            #region --------- 准备当前需要上传的条码和测试结果 ----------

            // 上工装机程序没有测试项，直接上传条码和OK结果
            // 如果条码被存储就，说明流程检查通过，直接给测试结果赋值OK信号
            if (EnableUpperTooling.Checked)
            {
                writeLog.WriteLogToComponent(UploadMes, $"[{uploadManagerEntity.Name}]条码已存储,共{scannedBarcodeList.Count}个");

                for (int i = 0; i < scannedBarcodeList.Count; i++)
                {
                    productResultList.Add("3");
                }
            }
            else
            {
                // 从PLC读取产品结果，3=OK|其它=NG
                string result = _readWriteNet.ReadInt16(uploadManagerEntity.ProductResult).Content.ToString();
                productResultList.Add(result);

                // 获取过站所需的条码
                ushort length = Convert.ToUInt16(uploadManagerEntity.BarcodeToUploadLength);
                if (!TryReadStringValue(uploadManagerEntity.BarcodeToUpload, length, out prdSN))
                {
                    var log = $"[{uploadManagerEntity.Name}]读取条码失败,请检查PLC连接";
                    HandleOperationError(uploadManagerEntity.feedbackPoint, true, userMessage: log);
                    writeLog.WriteLogToComponent(UploadMes, log);
                    return prdSN;
                }
                else if (prdSN == "")
                {
                    var log = $"[{uploadManagerEntity.Name}]获取的条码数据为空";
                    writeLog.WriteLogToComponent(UploadMes, log);
                    HandleOperationError(uploadManagerEntity.feedbackPoint, true, log);
                    return prdSN;
                }

                writeLog.WriteLogToComponent(UploadMes, $"[{uploadManagerEntity.Name}]读取条码({prdSN})");
                scannedBarcodeList.Add(prdSN);

                // 将读取到的结果写在界面上
                CallUiSafely.ExecuteControlSafely(barCode, c => c.Text = prdSN);

                // 将图片文件移动到PrdSN命名的文件目录，如果已经被移动了会跳过
                MoveFile(prdSN, plcAddressInfo.TriggerUpload3);
            }

            #endregion

            #region ---------- 动态读取测试项数据 ----------

            writeLog.WriteLogToComponent(UploadMes, $"[{uploadManagerEntity.Name}]开始添加维护数据");

            // 1.获取数据库配置
            //boardTable = curMdb.Find("select * from Board");

            // 2. 初始化列表
            var valList = new List<string>();
            var maxList = new List<string>();
            var minList = new List<string>();
            var resList = new List<string>();

            // 3. 【功能新增】获取当前工序对应的工位号
            string currentProcessId = GetStationIdByProcessName(uploadManagerEntity.Name);
            if (!_ReadTestValueByStation(boardTable, currentProcessId, out dynamic value, ref valList, ref maxList, ref minList, ref resList))
            {
                var log = $"[{uploadManagerEntity.Name}]维护地址: {value}解析错误({prdSN})";
                HandleOperationError(uploadManagerEntity.feedbackPoint, true, log);
                writeLog.WriteLogToComponent(rtbErrorLog, log);
                return prdSN;
            }

            writeLog.WriteLogToComponent(UploadMes, $"[{uploadManagerEntity.Name}]结束添加维护数据");

            #endregion

            ReturnParamSendResult returnParam = null;

            #region ---------- 上传结果 ----------

            // 不需要上传直接通知PLC 读取结果 完成 信号
            if (!EnableResultUpload.Checked)
            {
                _readWriteNet.Write($"{uploadManagerEntity.feedbackPoint}", 1);
            }
            else
            {
                // ============= 开始上传结果 =============

                writeLog.WriteLogToComponent(UploadMes, $"[{uploadManagerEntity.Name}]开始执行结果上传流程<->");

                // 上传结果到MES（包含本地txt文件，图片等信息）
                returnParam = SendResultToMes(scannedBarcodeList, productResultList, valList, maxList, minList, resList, uploadManagerEntity);

                // 逻辑是：只有机器NG才显示NG，如果是接口返回Fail，这个产品并没有过站成功，就直接显示报错信息
                writeLog.WriteLogToComponent(UploadMes, $"[{uploadManagerEntity.Name}]结果上传流程执行结束<->");

                // ============= 解析返回来的参数 =============

                if (returnParam is null)
                {
                    // null情况直接不处理，在SendResultToMes里面处理过了，但是必须要有这个过程，不能删除这里的判断空
                    SendResultAfter(uploadManagerEntity, scannedBarcodeList, productResultList);
                    return prdSN;
                }

                if (returnParam.Result == nameof(Result.Pass))
                {
                    _readWriteNet.Write($"{uploadManagerEntity.feedbackPoint}", 1);
                    CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "生产结果上传成功"; c.ForeColor = Color.Green; });
                }
                else // 产品过站失败
                {
                    // 通知plc不能打印,只有装配机工序2：Weight 数据上传完成且OK后才能打印。
                    // （访问获取打印数据接口时需确保上一工序的过站结果为Pass）
                    if (EnablePrintCode.Checked && uploadManagerEntity.Name == "Weight")
                    {
                        TryWriteInt16Value(plcAddressInfo.PrintTrigger, 2);
                    }

                    // TODO:
                    // 如果接口返回FAIL，将返回的错误信息显示出来

                    // 根据界面上的设置决定NG显示和阻塞逻辑
                    string operJudge = CallUiSafely.GetControlPropertyValueSafely(cboProductMode, c => c.Text);
                    switch (operJudge)
                    {
                        case "不显示NG且阻塞":
                            HandleOperationError(uploadManagerEntity.feedbackPoint, true, $"{uploadManagerEntity.Name}:{returnParam.ErrorMessage}");
                            return prdSN;
                        case "显示NG且阻塞":
                            HandleOperationError(uploadManagerEntity.feedbackPoint, true, $"{uploadManagerEntity.Name}:{returnParam.ErrorMessage}");
                            break;
                        case "显示NG且不阻塞":
                            HandleOperationError(uploadManagerEntity.feedbackPoint, false, $"{uploadManagerEntity.Name}:{returnParam.ErrorMessage}");
                            break;
                    }

                    //通知PLC结果读取异常
                    //readWriteNet.Write($"{plcAddress.Feedback1}", 2);
                }
            }

            #endregion

            #region ---------- 显示结果 ----------

            if (Global.Instance.CurDataBaseName == "装配机")
            {
                if (uploadManagerEntity.Name == "Scan-ASSY")
                    ShowResult(dgvResult1, returnParam, uploadManagerEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
                else if (uploadManagerEntity.Name == "Weight")
                    ShowResult(dgvResult2, returnParam, uploadManagerEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
                else if (uploadManagerEntity.Name == "Screw-BA")
                    ShowResult(dgvResult3, returnParam, uploadManagerEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
            }
            else
            {
                ShowResult(dgvResult1, returnParam, uploadManagerEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
            }

            SendResultAfter(uploadManagerEntity, scannedBarcodeList, productResultList);

            CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = "操作完成!"; c.ForeColor = Color.Green; });
            return prdSN;

            #endregion
        }

        /// <summary>
        /// 读取PLC测试数据并添加到对应列表中（已增加工位过滤）
        /// </summary>
        /// <param name="boardTable">数据库Board表数据</param>
        /// <param name="currentProcessId">当前工序号，用于过滤不需要读取的测试项</param>
        /// <param name="value">输出调试用的中间值</param>
        /// <param name="valList">实际值列表</param>
        /// <param name="maxList">上限列表</param>
        /// <param name="minList">下限列表</param>
        /// <param name="resList">结果列表</param>
        /// <returns>读取成功返回true</returns>
        private bool _ReadTestValueByStation(DataTable boardTable, string currentProcessId, out dynamic value, ref List<string> valList, ref List<string> maxList, ref List<string> minList, ref List<string> resList)
        {
            value = null; // 初始化 out 参数

            foreach (DataRow row in boardTable.Rows)
            {
                // 1. 【功能新增】工位过滤逻辑

                // 当前测试项所处的工位，即当前测试项的目标工位号
                string targetStationId = row["WorkID"].ToString();

                // 根据 GetStationIdByProcessName，装配机返回相应工序的数字，非装配机返回空字符串
                // 判断是否只读取目标工位的测试项。true=装配机，false=非装配机。
                bool isReadConditionally = !string.IsNullOrEmpty(currentProcessId);

                // 如果指定了目标工位，且当前测试想 与目标工位不一致，则跳过
                if (isReadConditionally && targetStationId != currentProcessId)
                {
                    continue;
                }

                // 2. 【需求新增】
                string resAddr = row["ResultBoardCode"].ToString();
                if (string.IsNullOrEmpty(resAddr))
                {
                    // 如果结果地址为空，填充占位符，确保 List 索引与 NameList 对齐
                    valList.Add("null");
                    maxList.Add("null");
                    minList.Add("null");
                    resList.Add("OK"); // 或者 "SKIP"，根据 MES 要求

                    // 跳过后续的读取逻辑，直接进入下一次循环
                    continue;
                }

                // --- 开始读取 PLC 数据 ---

                // A. 读取实际值 (BoardCode)
                string valAddr = row["BoardCode"].ToString();
                if (string.IsNullOrEmpty(valAddr))
                {
                    // 如果没配置地址，填默认值或空
                    valList.Add("null");
                }
                else
                {
                    if (!TryGetProcessedValue(valAddr, out value)) return false;
                    valList.Add($"{value}");
                }

                // B. 读取上限 (MaxBoardCode)
                string maxAddr = row["MaxBoardCode"].ToString();
                if (string.IsNullOrEmpty(maxAddr))
                {
                    // 如果没配置地址，填默认值或空
                    maxList.Add("null");
                }
                else
                {
                    if (!TryGetProcessedValue(maxAddr, out value)) return false;
                    maxList.Add($"{value}");
                }

                // C. 读取下限 (MinBoardCode)
                string minAddr = row["MinBoardCode"].ToString();
                if (string.IsNullOrEmpty(minAddr))
                {
                    // 如果没配置地址，填默认值或空
                    minList.Add("null");
                }
                else
                {
                    if (!TryGetProcessedValue(minAddr, out value)) return false;
                    minList.Add($"{value}");
                }

                // D. 读取结果 (ResultBoardCode) - 前面已经校验过非空
                if (!TryGetProcessedValue(resAddr, out value)) return false;
                resList.Add($"{value}");
            }

            //value = null;
            return true;
        }

        /// <summary>
        /// 数据上传(包含本地txt文件，图片等信息）
        /// </summary>
        /// <param name="scannedBarcodeList">产品条码列表</param>
        /// <param name="productResultList">产品结果列表</param>
        /// <param name="valList">测试值</param>
        /// <param name="maxList">上限值</param>
        /// <param name="minList">下限值</param>
        /// <param name="resList">测试结果</param>
        /// <param name="uploadEntity">当前上传实体</param>
        /// <returns></returns>
        public ReturnParamSendResult SendResultToMes(List<string> scannedBarcodeList, List<string> productResultList, List<string> valList, List<string> maxList, List<string> minList, List<string> resList, UploadManagerEntity uploadEntity)
        {
            GetFilteredTestItems(uploadEntity.Name, out var currentTestNameList, out var currentUnitList);

            // 线程中需要捕获异常，否则会直接退出
            try
            {
                PrdSNCollection2 prdSNCollection = new PrdSNCollection2();
                List<PrdSNsItem> prdSNsItems = new List<PrdSNsItem>();

                var inputParam = new InputParamSendResult
                {
                    Line = uploadEntity.Line,
                    Process = uploadEntity.Process,
                    Station = uploadEntity.Staiton,
                    Device = uploadEntity.Device,
                    Employee = txtUser.Text,
                    Fixture = FixtureCode,
                    TrackNo = "",
                    PhotoFTPPath = "",          // _OperPicture中赋值
                    ResultFileFTPPath = "",     // OpertTxt中赋值
                    PlanNo = OrderNo.Text,      // 装配机12061441-80-01  上工装12033377-40-01  //螺钉机12033370-80-01
                    BoardSideSN = scannedBarcodeList[0],
                    PrdSNCollection = prdSNCollection
                };

                // isSubmitPass：false=按实际结果提交，true=强制提交Pass
                // 如果是Weight工序，并且已经选中了一键强制打印，则得到true（因访问打印接口时需确保上一工序的过站结果为Pass）
                //bool isForceOk = CallUiSafely.GetControlPropertyValueSafely(EnableResultPass, it => it.Checked);
                //bool isSubmitPass = uploadEntity.Name == "Weight" && isForceOk ? true : false;
                string type = CallUiSafely.GetControlPropertyValueSafely(cboEnforcePass, it => it.Text);
                bool isSubmitPass;
                switch (type)
                {
                    case "All":
                        isSubmitPass = true;
                        break;
                    case "None":
                        isSubmitPass = false;
                        break;
                    case "Scan-ASSY":
                        isSubmitPass = uploadEntity.Name == "Scan-ASSY";
                        break;
                    case "Weight":
                        isSubmitPass = uploadEntity.Name == "Weight";
                        break;
                    case "Screw-BA":
                        isSubmitPass = uploadEntity.Name == "Screw-BA";
                        break;
                    default:
                        isSubmitPass = false;
                        break;
                }

                // 若为拼版条码，则需拼接每个条码的数据
                for (int a = 0; a < scannedBarcodeList.Count; a++)
                {
                    writeLog.WriteLogToComponent(UploadMes, $"[{uploadEntity.Name}]结果流程中拼接条码{scannedBarcodeList[a]}");

                    // =============== 拼接测试项数据 ===============

                    TestDatas testDatas = new TestDatas { TestData = new List<TestDataItem>() };

                    // 当前工序没有测试项时，仍然需要上传一个空的测试项，避免接口报错
                    if (currentTestNameList.Count == 0)
                        testDatas.TestData.Add(new TestDataItem());

                    // 如果存在测试项，则拼接测试项数据
                    // 这里务必确保各个数组的长度一致，且数据对应正确
                    for (int i = 0; i < currentTestNameList.Count; i++)
                    {
                        testDatas.TestData.Add(new TestDataItem
                        {
                            Name = currentTestNameList[i],
                            Value = valList[i],
                            // 这里是单一测试项的结果
                            Result = isSubmitPass ? "OK" : resList[i],
                            USL = maxList[i],
                            LSL = minList[i],
                            Unit = currentUnitList[i]
                        });
                    }


                    // 若提交到MES的产品结果为Fail，则MES也返回Fail -> 过站成功，但不允许继续做
                    string productResult = isSubmitPass ? "Pass" : (productResultList[a] == "3" ? "Pass" : "Fail");

                    // 生成prdSNs里面的单个PrdSN，添加图片文件路径信息 如果有的情况下
                    PrdSNsItem prdSNs = new PrdSNsItem
                    {
                        PrdSN = scannedBarcodeList[a],
                        SubBoardId = (a + 1).ToString(),
                        BoardSkip = "False",
                        // 这里的结果是当前产品的总结果（非单一测试项结果）
                        Result = productResult,
                        MachineResult = productResult,
                        CycleTime = "0",
                        ResultFile = "",                // 单个文件，_OperTxt中赋值
                        PhotoFiles = new PhotoFiles(),  // 多个文件，_OperPicture中赋值
                        TestDatas = testDatas
                    };

                    // 非上工装机时，需要保存文件和图片数据。
                    if (!EnableUpperTooling.Checked)
                    {
                        // 上传图片并添加PrdSNs的图片文件名数据到json
                        //if (!_OperPicture(PrdSNInfo, inputParam))
                        //{
                        //HandleOperationError(uploadEntity.feedbackPoint, true, $"{uploadEntity.Name}:无法上传CCD图片至MES");
                        //    return null;
                        //}

                        // 再处理txt
                        if (!_OperTxt(prdSNs, inputParam))
                        {
                            HandleOperationError(uploadEntity.feedbackPoint, true, $"{uploadEntity.Name}:无法上传txt文件至MES");
                            return null;
                        }
                    }

                    // 添加数据
                    prdSNsItems.Add(prdSNs);
                }

                prdSNCollection.PrdSNs = prdSNsItems;
                writeLog.WriteLogToComponent(UploadMes, $"[{uploadEntity.Name}]结果流程中拼接条码结束，开始请求MES");

                // 结果上传
                ReturnParamSendResult returnParam = _request.GetResponseSerializeResult<ReturnParamSendResult, InputParamSendResult>(Url_DataUpload.Text, _httpClient, "SAVERESULT", inputParam, uploadEntity.Name);

                writeLog.WriteLogToComponent(UploadMes, $"[{uploadEntity.Name}]请求MES流程结束");

                if (returnParam == null)
                {
                    HandleOperationError(uploadEntity.feedbackPoint, true, $"{uploadEntity.Name}:上传结果接口返回数据异常");
                    return null;
                }

                return returnParam;
            }
            catch (Exception e)
            {
                HandleOperationError(uploadEntity.feedbackPoint, true, $"{uploadEntity.Name}:读取生产结果数据异常{e}");
                return null;
            }
        }

        /// <summary>
        /// 保存并上传txt文件
        /// </summary>
        /// <param name="prdSNs"></param>
        /// <param name="inputParam"></param>
        private bool _OperTxt(PrdSNsItem prdSNs, InputParamSendResult inputParam)
        {
            string txtFileName = $"{prdSNs.PrdSN}_{System.DateTime.Now:yyyyMMddHHmmss}.txt";
            //保存txt文件到本地，写入当前字符串
            string fileContent = JsonConvert.SerializeObject(prdSNs);

            //保存文件到本地
            string localPath = Path.Combine(CallUiSafely.GetControlPropertyValueSafely(LocalFilePath, c => c.Text), "Txt");
            string fullPath = Path.Combine(localPath, txtFileName);

            SaveTxtFileToLocal(fullPath, fileContent);

            string url = CallUiSafely.GetControlPropertyValueSafely(FTPlog, c => c.Text);
            string process = CallUiSafely.GetControlPropertyValueSafely(Process, c => c.Text);
            string line = CallUiSafely.GetControlPropertyValueSafely(Line, c => c.Text);
            string user = CallUiSafely.GetControlPropertyValueSafely(FTPID, c => c.Text);
            string pwd = CallUiSafely.GetControlPropertyValueSafely(FTPCODE, c => c.Text);

            HttpClientUtil ftpClient = new HttpClientUtil(url, process, line, user, pwd, "Log");
            //ftpClient.CheckFtpDirectory();
            string fileFTPPath = ftpClient.UploadToFtpServer(localPath, txtFileName, txtFileName);

            if (fileFTPPath is null)
            {
                writeLog.WriteLogToComponent(rtbErrorLog, $"上传txt文件:{fullPath} 至MES失败");
                return false;
            }

            inputParam.ResultFileFTPPath = fileFTPPath;
            //添加文件名到json数据
            prdSNs.ResultFile = txtFileName;
            //删除文件
            Resource.ForceDeleteFile(fullPath);
            return true;
        }

        /// <summary>
        /// 添加图片的文件名数据到对象，并上传，删除本地图片
        /// </summary>
        /// <param name="prdSNs"></param>
        /// <param name="inputParam"></param>
        /// <returns>是否操作成功</returns>
        private bool _OperPicture(PrdSNsItem prdSNs, InputParamSendResult inputParam)
        {
            try
            {
                string localPath = Path.Combine(CallUiSafely.GetControlPropertyValueSafely(LocalFilePath, c => c.Text), "PrdSNPictures");
                //获取本地文件
                string[] pictureFiles = Directory.GetFiles(Path.Combine(localPath, prdSNs.PrdSN));

                //将jpg转为jpeg
                //JpgConvertJpeg(picturePaths, 20);

                List<Task<string>> uploadTask = new List<Task<string>>();

                string url = CallUiSafely.GetControlPropertyValueSafely(FTPPIC, c => c.Text);
                string process = CallUiSafely.GetControlPropertyValueSafely(Process, c => c.Text);
                string line = CallUiSafely.GetControlPropertyValueSafely(Line, c => c.Text);
                string user = CallUiSafely.GetControlPropertyValueSafely(FTPID, c => c.Text);
                string pwd = CallUiSafely.GetControlPropertyValueSafely(FTPCODE, c => c.Text);

                //取出图片路径进行上传和添加json数据，
                foreach (string localFileName in pictureFiles)
                {
                    //上传有文件名校验，本地文件和上传文件名不能一样，上传文件名必须按规则
                    string uploadFilename = $"{prdSNs.PrdSN}_{System.DateTime.Now:yyyyMMddHHmmssfff}{Path.GetExtension(localFileName)}";
                    ////上传文件
                    string localFileFullPath = Path.Combine(localPath, localFileName);

                    //并发上传
                    uploadTask.Add(Task.Run(() =>
                    {
                        HttpClientUtil ftpClient = new HttpClientUtil(url, process, line, user, pwd, "Picture");
                        ftpClient.CheckFtpDirectory();
                        //创建对象用于发送文件和图片
                        return ftpClient.UploadToFtpServer(localPath, localFileName, uploadFilename);
                    }));
                    //添加当前的PrdSN的所有图片数据
                    prdSNs.PhotoFiles.PhotoFile.Add(uploadFilename);

                    Thread.Sleep(5);
                }

                // 等待所有任务结束
                Task.WaitAll(uploadTask.ToArray());

                string[] ftpPaths = uploadTask.Select(x => x.Result).ToArray();
                if (ftpPaths.Length == 0)
                {
                    writeLog.WriteLogToComponent(rtbErrorLog, "没有检测到任何图片");
                    return false;
                }

                // 查看所有任务的返回值，如果有null则说明有图片上传失败
                if (ftpPaths.Contains(null))
                {
                    writeLog.WriteLogToComponent(rtbErrorLog, "上传单张图片至MES失败");
                    return false;
                }

                inputParam.PhotoFTPPath = ftpPaths[0];

                //Directory.Delete(Path.Combine(localPath, PrdSNInfo.PrdSN), true);

                return true;
            }
            catch (Exception)
            {
                //writeLog.WriteLogToComponent(errorLog, $"上传图片异常：{e}");
                //return false;
                return true;
            }
        }

        /// <summary>
        /// 上传结果后无论成功或失败都需要执行
        /// </summary>
        /// <param name="uploadManagerEntity"></param>
        /// <param name="Read_Barcodes"></param>
        /// <param name="resultArray"></param>
        private void SendResultAfter(UploadManagerEntity uploadManagerEntity, List<string> Read_Barcodes, List<string> resultArray)
        {
            // 上传结果后删除临时存储图片的文件夹
            if (uploadManagerEntity.DeleteFile)
            {
                DeletePicture(Read_Barcodes);
            }

            Read_Barcodes.Clear();//条码数组
            resultArray.Clear();  //结果数组
        }

        /// <summary>
        /// 根据工序名称与工序的映射，筛选出对应的测试项名称和单位
        /// </summary>
        /// <param name="processName">当前工序名称 (如: Scan-ASSY)</param>
        /// <param name="testNameList">输出：筛选后的测试项名称列表</param>
        /// <param name="unitNameList">输出：筛选后的单位名称列表</param>
        private void GetFilteredTestItems(string processName, out List<string> testNameList, out List<string> unitNameList)
        {
            // 1. 获取测试项的目标工位号
            string targetStationId = GetStationIdByProcessName(processName);

            // 2. 判断是否需要过滤
            // 如果返回空字符串，说明是非装配机或者不需要区分工位的工序
            if (string.IsNullOrEmpty(targetStationId))
            {
                // 直接转换为 List 输出
                testNameList = testNameArray.ToList();
                unitNameList = unitNameArray.ToList();
                return;
            }

            // 3. 准备临时列表
            List<string> tempNameList = new List<string>();
            List<string> tempUnitList = new List<string>();

            // 4. 遍历全局工位数组 (stationIdArray) 进行筛选
            // 前提：stationIdArray, testNameArray, unitNameArray 的长度必须是一一对应的
            if (stationIdArray != null)
            {
                for (int i = 0; i < stationIdArray.Length; i++)
                {
                    // 安全检查：防止索引越界 (比如名字数组比工位数组短)
                    if (i >= testNameArray.Length || i >= unitNameArray.Length) break;

                    // 核心比对：如果当前测试项的工位号 == 目标工位号
                    if (stationIdArray[i] == targetStationId)
                    {
                        tempNameList.Add(testNameArray[i]);
                        tempUnitList.Add(unitNameArray[i]);
                    }
                }
            }

            // 5.  List，避免不必要的数组转换）
            testNameList = tempNameList;
            unitNameList = tempUnitList;
        }

        /// <summary>
        /// 建立工序名称与序号的映射
        /// <para>Scan-ASSY - 工序1</para>
        /// Weight - 工序2
        /// <para>Screw-BA - 工序3</para>
        /// </summary>
        /// <param name="processName">UploadManagerEntity.Name (例如: Scan-ASSY)</param>
        /// <returns>返回特定工序的序号标志, 如果非装配机则返回空字符串</returns>
        private string GetStationIdByProcessName(string processName)
        {
            // 如果不是装配机，通常不需要区分工位，或者默认就是 ""
            if (Global.Instance.CurDataBaseName != "装配机") return string.Empty;

            // 根据名称关键字映射到数据库里的 WorkID
            // 注意：这里的 "1", "2", "3" 必须和你 Access 数据库 Board 表里的 WorkID 字段内容完全一致
            if (processName.Contains("Scan-ASSY")) return "1";
            if (processName.Contains("Weight")) return "2";
            if (processName.Contains("Screw-BA")) return "3";

            return ""; // 如果没匹配到，默认不过滤
        }

        #endregion

        #region ---------- 扭力数据采集与转发 ----------

        // 定义两个扭力控制器客户端
        private TorqueControllerClient _clientScanAssy; // 工序1 (Scan-ASSY)
        private TorqueControllerClient _clientScrewBa;  // 工序3 (Screw-BA，实际在工位5动作)

        // 初始化扭力系统
        private async Task InitTorqueSystem()
        {
            // 初始化工序1 (Scan-ASSY) 控制器
            string ipOp1 = "192.168.1.31";
            _clientScanAssy = new TorqueControllerClient(ipOp1);

            // 处理连接状态
            _clientScanAssy.OnConnectionStatusChanged += (isConnected, msg) =>
            {
                this.Invoke((Action)(() => { ASSY.ForeColor = isConnected ? Color.Green : Color.Red; }));
            };

            // 处理日志
            _clientScanAssy.OnLog += (msg) => { this.Invoke((Action)(() => AppendLog("Scan-ASSY", $"[Scan-ASSY] {msg}"))); };

            // 收到数据 -> 转发
            _clientScanAssy.OnTorqueDataReceived += (data) =>
            {
                // 开启新线程处理转发，避免阻塞通信接收线程
                Task.Run(() => ForwardTorqueToPlc("Scan-ASSY", data));
            };

            await _clientScanAssy.ConnectAsync();


            // 初始化工序3 (Screw-BA) 控制器
            string ipOp3 = "192.168.1.32";
            _clientScrewBa = new TorqueControllerClient(ipOp3);

            // 订阅事件：处理连接状态
            _clientScrewBa.OnConnectionStatusChanged += (isConnected, msg) =>
            {
                Invoke((Action)(() => { BA.ForeColor = isConnected ? Color.Green : Color.Red; }));
            };
            // 订阅事件：处理日志
            _clientScrewBa.OnLog += (msg) => { this.Invoke((Action)(() => AppendLog("Screw-BA", $"[Screw-BA] {msg}"))); };
            // 订阅事件：收到扭力数据
            _clientScrewBa.OnTorqueDataReceived += (data) =>
            {
                // 收到数据 -> 写入 PLC (WorkID="3")
                Task.Run(() => ForwardTorqueToPlc("Screw-BA", data));
            };
            await _clientScrewBa.ConnectAsync();
        }

        // 辅助日志方法
        private void AppendLog(string processName, string msg)
        {
            // 防止窗体关闭后调用报错
            if (this.IsDisposed || rtbASSYLog.IsDisposed || rtbScrewLog.IsDisposed) return;

            try
            {
                if (processName == "Scan-ASSY")
                {
                    rtbASSYLog.AppendText($"{System.DateTime.Now:HH:mm:ss_fff} {msg}\r\n");
                    rtbASSYLog.ScrollToCaret();
                }
                else
                {
                    rtbScrewLog.AppendText($"{System.DateTime.Now:HH:mm:ss_fff} {msg}\r\n");
                    rtbScrewLog.ScrollToCaret();
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// 将接收到的扭力数据写入对应的 PLC 地址
        /// </summary>
        /// <param name="processName">工位号 ("Scan-ASSY" 或 "Screw-BA")</param>
        /// <param name="data">扭力数据</param>
        private void ForwardTorqueToPlc(string processName, TorqueData data)
        {
            try
            {
                // 0.基础检查
                if (_readWriteNet == null)
                {
                    this.Invoke((Action)(() => AppendLog(processName, $"[转发失败] PLC未连接，无法转发工位{processName}数据")));
                    return;
                }

                // 1. 获取工序对应的地址配置
                string addrTorque, addrMax, addrMin, addrResult, addrReq, addrAck;

                // 用于UI更新的控件引用
                Label lblVal, lblMin, lblMax, lblRes;

                switch (processName)
                {
                    case "Scan-ASSY":
                        addrTorque = plcAddressInfo.TorqueValue1;
                        addrMax = plcAddressInfo.TorqueMax1;
                        addrMin = plcAddressInfo.TorqueMin1;
                        addrResult = plcAddressInfo.TorqueResult1;
                        addrReq = plcAddressInfo.Request1;
                        addrAck = plcAddressInfo.Acknowledge1;

                        // 绑定工序1的UI控件
                        lblVal = lblAssyVal;
                        lblMin = lblAssyMin;
                        lblMax = lblAssyMax;
                        lblRes = lblAssyRes;
                        break;
                    case "Screw-BA":
                        addrTorque = plcAddressInfo.TorqueValue3;
                        addrMax = plcAddressInfo.TorqueMax3;
                        addrMin = plcAddressInfo.TorqueMin3;
                        addrResult = plcAddressInfo.TorqueResult3;
                        addrReq = plcAddressInfo.Request3;
                        addrAck = plcAddressInfo.Acknowledge3;

                        // 绑定工序3的UI控件
                        lblVal = lblBaVal;
                        lblMin = lblBaMin;
                        lblMax = lblBaMax;
                        lblRes = lblBaRes;
                        break;
                    default:
                        Invoke((Action)(() => AppendLog(processName, $"[转发失败] 未知工位号: {processName}")));
                        return;
                }

                // 2. 解析数据
                int.TryParse(data.TorqueActual, out int val);
                int.TryParse(data.TorqueMin, out int min);
                int.TryParse(data.TorqueMax, out int max);
                short result = data.IsOk ? (short)3 : (short)2; // 3=OK, 2=NG

                // 3. 【UI 实时更新 1】显示采集到的数据
                Invoke((Action)(() =>
                {
                    lblVal.Text = $"{val:0.00}";
                    lblMin.Text = $"{min:0.00}";
                    lblMax.Text = $"{max:0.00}";
                    lblRes.Text = data.IsOk ? "OK" : "NG";
                    lblRes.ForeColor = data.IsOk ? Color.Green : Color.Red;
                }));

                // 3. 写入数据
                bool w1 = _readWriteNet.Write(addrTorque, val).IsSuccess;
                bool w2 = _readWriteNet.Write(addrMin, min).IsSuccess;
                bool w3 = _readWriteNet.Write(addrMax, max).IsSuccess;
                bool w4 = _readWriteNet.Write(addrResult, result).IsSuccess;

                if (!w1 || !w2 || !w3 || !w4)
                {
                    Invoke((Action)(() => AppendLog(processName, $"[转发错误] 工位{processName} 数据写入PLC失败")));
                    return;
                }

                // 4. 发起握手：置位 Req = 1
                if (!_readWriteNet.Write(addrReq, (ushort)1).IsSuccess)
                {
                    Invoke((Action)(() => AppendLog(processName, $"[转发错误] 工位{processName} 握手请求发送失败")));
                    return;
                }

                // 5. 等待握手完成：轮询 Ack == 1
                // 设置超时时间 3秒
                var startTime = System.DateTime.Now;
                bool isSuccess = false;

                while ((System.DateTime.Now - startTime).TotalSeconds < 3)
                {
                    // 读取 Ack 信号
                    var readAck = _readWriteNet.ReadInt16(addrAck);
                    if (readAck.IsSuccess && readAck.Content == 1)
                    {
                        isSuccess = true;
                        break;
                    }
                    Thread.Sleep(50); // 短暂休眠
                }

                // 6. 握手结束：Req 置 0 (复位)
                _readWriteNet.Write(addrReq, (ushort)0);

                // 7. 输出结果
                Invoke((Action)(() =>
                {
                    AppendLog(processName,
                        isSuccess
                            ? $"[转发成功] 工位{processName} -> 扭力:{val}, 上限:{max}, 下限:{min}, 结果:{(data.IsOk ? "OK" : "NG")} -> PLC接收确认"
                            : $"[转发超时] 工位{processName} -> PLC未在2秒内响应Ack信号 (请检查PLC逻辑)");
                }));
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => AppendLog(processName, $"[扭力转发异常] {ex.Message}")));
            }
        }

        #endregion

        #region ---------- 加载页面参数配置 ----------

        private DataTable interfceAddr;

        /// <summary>
        /// 读取系统设置参数
        /// </summary>
        private void Load_SystemSettingsConfig()
        {
            #region ====== 读取系统设置 ======

            DataTable table1 = curMdb.Find("select * from SytemSet where ID = '1'");

            if (table1 != null && table1.Rows.Count > 0)
            {
                DataRow row = table1.Rows[0];
                PlcIP.Text = row["IP"].ToString();
                PlcPort.Text = row["PortCode"].ToString();
                DeviceName.Text = row["DeviceName"].ToString();
                PlcConnectType.Text = row["PlcType"].ToString();
            }

            deviceDataBase.Text = Path.GetFileNameWithoutExtension(Global.Instance.DataBase);

            #endregion

            #region ====== 读取MES配置 ======

            DataTable MESDateTable = curMdb.Find("select * from MesSetting where id = '1'");

            if (MESDateTable != null && MESDateTable.Rows.Count > 0)
            {
                DataRow row = MESDateTable.Rows[0];
                string[] columnNames = { "url", "Line", "Process", "Station", "MesKey", "Security", "Device", "PlanNo", "FTPlog", "FTPPIC", "FTPID", "FTPCODE", "SWVer", "HWVer" };

                foreach (string columnName in columnNames)
                {
                    // 1. 获取数据库值（防止空值报错，可加个空合并）
                    string dbValue = row[columnName]?.ToString() ?? "";

                    // 2. 【核心修复】无条件填充 GlobalData
                    // 无论界面上有没有这个控件，GlobalData 都必须有数据
                    GlobalData[columnName] = dbValue;

                    // 3. 尝试更新 UI（如果控件存在）
                    // 即使找不到控件，也不会影响 GlobalData 的完整性
                    Control textBox = Controls.Find(columnName, true).FirstOrDefault() as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = dbValue;
                    }
                }
            }

            #endregion

            #region ====== 获取接口地址集合 ======

            interfceAddr = curMdb.Find("select * from interface");
            Dictionary<string, TextBox> textBoxMap = new Dictionary<string, TextBox>
            {
                { "GetToken", Url_Token },
                { "GetSnCode", UrlPanelization },
                { "checkPath", Url_RouteCheck },
                { "UploadResults", Url_DataUpload },
                { "GetFtp", Url_FTPMessGet },
                { "GetProductName", Url_GetProductName },
                { "heartbeat", Url_Heartbeat },
                { "DeviceStatus", Url_DeviceStatus },
                { "fault", Url_ErrorInterface },
                { "AlterProcedure", Url_KeyArgs },
                { "ActualTimeParam", Url_RealtimeArgs },
                { "FixtureReplacement", Url_ToolingChange },
                { "PrintAddress",Url_PrintTemplate },
                { "LocalFileDir",LocalFilePath }
            };

            foreach (DataRow row in interfceAddr.Rows)
            {
                string interfaceName = row["InterfaceName"].ToString();
                if (textBoxMap.TryGetValue(interfaceName, out var textBox))
                {
                    textBox.Text = row["InterfaceUrl"].ToString();
                }
            }

            #endregion
        }

        /// <summary>
        /// 从数据库加载生产配置页面的参数
        /// </summary>
        public void Load_ProductConfig()
        {
            DataTable dt = curMdb.Find("select * from ProductConfig where ID=1");

            // -------- 保存后生效 --------
            EnableDeviceStatusUpload.Checked = (bool)dt.Rows[0]["status_upload"];   // 勾选启用设备状态上传
            EnableWarningUpload.Checked = (bool)dt.Rows[0]["warning_upload"];       // 勾选启用预警信息上传
            EnableRealtimeArgsUpload.Checked = (bool)dt.Rows[0]["realtime_upload"]; // 勾选启用实时参数上传
            EnablekeyArgsUpload.Checked = (bool)dt.Rows[0]["key_args_upload"];      // 勾选启用关键参数上传
            HeartbeatUploadRate.Text = dt.Rows[0]["heartbeat_rate"].ToString();     // 心跳上传频率
            BarcodeRule.Text = dt.Rows[0]["barcode_rule"].ToString();               // 条码规则
            RealtimeArgsUploadRate.Text = dt.Rows[0]["KA_upload_rate"].ToString();  // 实时参数上传频率

            // -------- 切换状态即生效 --------
            EnableGetNextBoard.Checked = (bool)dt.Rows[0]["get_next_board"];        // 勾选启用获取拼版
            BanReadBarcode.Checked = (bool)dt.Rows[0]["shield_barcode"];            // 勾选屏蔽条码读取
            EnablePrintCode.Checked = (bool)dt.Rows[0]["print_code"];               // 勾选启用打印模板
            EnableFluentVerify.Checked = (bool)dt.Rows[0]["fluence_verify"];        // 勾选启用流程验证
            EnableUpperTooling.Checked = (bool)dt.Rows[0]["tooling_program"];       // 勾选启用上工装机程序
            EnableTypeChangedVerify.Checked = (bool)dt.Rows[0]["change_type"];      // 勾选启用型号切换校验
            EnableResultUpload.Checked = (bool)dt.Rows[0]["result_upload"];         // 勾选启用上传结果
            EnableBarcodeRuleVerify.Checked = (bool)dt.Rows[0]["barcode_verify"];   // 勾选启用条码规则验证
            EnableResultPass.Checked = (bool)dt.Rows[0]["result_pass"];             // 装配机强制打印标签（上传OK）- 前道工序结果放行
            cboProductMode.Text = dt.Rows[0]["mes_product_mode"].ToString();        // 1.不显示NG且阻塞；2.显示NG且阻塞；3.显示NG且不阻塞
            cboEnforcePass.Text = dt.Rows[0]["EnforcePass"].ToString();             // 强制过站选项：1.All；2.None；3.Scan-ASSY；4.Weight；5.Screw-BA

            if (Global.Instance.CurDataBaseName == "螺钉机")
            {
                torque_prot.Text = dt.Rows[0]["torque_prot"].ToString();                // 扭力仪端口
                enable_torque.Checked = (bool)(dt.Rows[0]["enable_torque"]);            // 勾选启用螺钉机扭力->PLC
            }
            // -------- 暂未使用 --------
            //EnableTokenVerify.Checked = (bool)matchTable.Rows[0]["token_verify"];
            //EnableHandlerPicture.Checked = (bool)matchTable.Rows[0]["handler_picture"];
        }

        /// <summary>
        /// 加载装配机界面配置信息
        /// </summary>
        private void Load_PrinterSet()
        {
            string sql = "select * from PrinterSet where id=1";
            DataTable source = curMdb.Find(sql);
            if (source.Rows.Count > 0)
            {
                printerName.Text = source.Rows[0]["printer_name"].ToString();
                printTemplatePath.Text = source.Rows[0]["print_code_path"].ToString();
            }

            sql = "select * from BeforeProcess where id=1";
            source = curMdb.Find(sql);
            if (source.Rows.Count > 0)
            {
                Line2.Text = source.Rows[0]["line"].ToString();
                Process2.Text = source.Rows[0]["process"].ToString();
                Station2.Text = source.Rows[0]["station"].ToString();
                MesKey2.Text = source.Rows[0]["mes_key"].ToString();
                Security2.Text = source.Rows[0]["security"].ToString();
                Device2.Text = source.Rows[0]["device"].ToString();
            }

            sql = "select * from Process3 where id=1";
            source = curMdb.Find(sql);
            if (source.Rows.Count > 0)
            {
                Line3.Text = source.Rows[0]["line"].ToString();
                Process3.Text = source.Rows[0]["process"].ToString();
                Station3.Text = source.Rows[0]["station"].ToString();
                MesKey3.Text = source.Rows[0]["mes_key"].ToString();
                Security3.Text = source.Rows[0]["security"].ToString();
                Device3.Text = source.Rows[0]["device"].ToString();
            }
        }

        #endregion

        #region ---------- PLC读写操作 ----------

        /// <summary>
        /// 尝试读取PLC Int16值，并输出读取到的值，最多重试3次。
        /// <para> resultValue = -1表示读取失败 </para>
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryReadInt16Value(string address, out int value/*, int callCount = 0*/)
        {
            // Bug fixed: 递归调用时 callCount 没有正确传递，导致无限递归 -> StackOverflow
            /*OperateResult<short> result = readWriteNet.ReadInt16(address);
            if (result.IsSuccess)
            {
                resultValue = result.Content;
                return true;
            }
            else //读取失败
            {
                // ErrorCode < 0 属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                {
                    resultValue = -1;
                    return false;
                }
                else if (callCount < 3)
                {
                    return TryReadInt16Value(address, out resultValue, callCount++);
                }
                else
                {
                    resultValue = -1;
                    return false;
                }
            }*/

            for (int i = 0; i < 3; i++)
            {
                OperateResult<short> result = _readWriteNet.ReadInt16(address);

                if (result.IsSuccess)
                {
                    value = result.Content;
                    return true;
                }

                // ErrorCode < 0 属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                {
                    value = -1;
                    return false;
                }

                //DelayAndCheckStop(50);
            }

            // 循环3次后仍然失败
            value = -1;
            return false;
        }

        /// <summary>
        /// 尝试异步读取PLC Int32值，并输出读取到的值，最多重试3次。
        /// <para> resultValue = -1表示读取失败 </para>
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private async Task<(bool, int)> TryReadInt32ValueAsync(string address)
        {
            for (int i = 0; i < 3; i++)
            {
                var result = await _readWriteNet.ReadInt32Async(address);

                if (result.IsSuccess)
                    return (true, result.Content);

                // ErrorCode < 0 属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                    return (false, -1);
            }

            // 循环3次后仍然失败
            return (false, -1);
        }

        /// <summary>
        /// 尝试异步读取 Int16 值，带 500ms 超时保护和最多3次重试。
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <returns>
        /// 一个表示异步操作的任务。
        /// 任务结果是一个元组 (bool isReadOk, short resultValue):
        /// <list type="bullet">
        /// <item><description><c>isReadOk</c>: <c>true</c> 表示读取成功, <c>false</c> 表示3次尝试后依旧失败（包括超时）。</description></item>
        /// <item><description><c>resultValue</c>: 读取成功时的值。如果 <c>isReadOk</c> 为 <c>false</c>，则返回 -1。</description></item>
        /// </list>
        /// </returns>
        private async Task<(bool isReadOk, short value)> TryReadInt16Async(string address)
        {
            for (var i = 0; i < 3; i++)
            {
                var readTask = _readWriteNet.ReadInt16Async(address);
                var completedTask = await Task.WhenAny(readTask, Task.Delay(500));

                if (readTask != completedTask)
                {
                    await Task.Delay(50);
                    continue;
                }

                var result = await readTask;
                if (result.IsSuccess && result.ErrorCode >= 0) return (true, result.Content);

                await Task.Delay(50);
            }

            // 循环3次后仍然失败
            return (false, -1);
        }

        /// <summary>
        /// 尝试读取PLC String值，清理字符串中的空白后输出读取到的值，最多重试3次（内部调用CodeNum.CleanString）。
        /// <para> resultValue = null表示读取失败 </para>
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="length">读取字符串的长度</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryReadStringValue(string address, ushort length, out string value)
        {
            for (int i = 0; i < 3; i++)
            {
                OperateResult<string> result = _readWriteNet.ReadString(address, length);

                if (result.IsSuccess)
                {
                    value = CodeNum.CleanString(result.Content);
                    return true;
                }

                // ErrorCode < 0属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                {
                    value = null;
                    return false;
                }

                //DelayAndCheckStop(50);
            }

            // 循环3次后仍然失败
            value = null;
            return false;
        }

        /// <summary>
        /// 尝试写入PLC Int16值。
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryWriteInt16Value(string address, short value)
        {
            OperateResult result = _readWriteNet.Write(address, value);

            if (result.IsSuccess)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> TryWriteInt16ValueAsync(string address, short value)
        {
            var writeTask = _readWriteNet.WriteAsync(address, value);
            var completedTask = await Task.WhenAny(writeTask, Task.Delay(500));

            // 超时
            if (writeTask != completedTask)
            {
                return false;
            }

            var writeResult = await writeTask;

            if (writeResult.IsSuccess)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region ---------- 设备名称动态显示 ----------

        /// <summary>
        /// 设置设备名称
        /// </summary>
        private void SetDeviceName()
        {
            this.label54.Text = DeviceName.Text;//textBox3.Text;//写设备名称
            FontStyle fontStyle = FontStyle.Bold;//设置字体粗细
            float size = 42F;//字体大小
            ChangeLabelFont(label54, size, fontStyle);//内字体随着字数的增加而自动减小
        }

        /// <summary>
        /// Label内字体随着字数的增加而自动减小，Label大小不变
        /// </summary>
        /// <param name="label"></param>
        /// <param name="size"></param>
        /// <param name="fontStyle"></param>
        /// <returns></returns>
        public Label ChangeLabelFont(Label label, float size, FontStyle fontStyle)
        {
            FontFamily ff = new FontFamily(label.Font.Name);
            string content = CallUiSafely.GetControlPropertyValueSafely(label, c => c.Text);
            //初始化label状态
            label.Font = new Font(ff, size, fontStyle, GraphicsUnit.Point);
            while (true)
            {
                //获取当前一行能放多少个字======================================================
                //1、获取label宽度
                int labelwidth = label.Width;
                //2、获取当前字体宽度
                Graphics gh = label.CreateGraphics();
                SizeF sf = gh.MeasureString("0", label.Font);
                float fontwidth = sf.Width;
                //3、得到一行放几个字
                int OneRowFontNum = (int)(labelwidth / (double)fontwidth);


                //判断当前的Label能放多少列======================================================
                //1、获取当前字体的高度
                float fontheight = sf.Height;
                //2、获取当前label的高度
                int labelheight = label.Height;
                //3、得到当前label能放多少列
                int ColNum = (int)(labelheight / (double)fontheight);

                //获取当前字符串需要放多少列======================================================
                var NeedColNum = Math.Ceiling((double)content.Length / OneRowFontNum);

                //如果超出范围，则缩小字体，然后返回再判断一次===================================
                if (ColNum <= NeedColNum)
                {
                    size -= 0.25F;
                    label.Font = new Font(ff, size, fontStyle, GraphicsUnit.Point);
                }
                else
                {
                    break;
                }
            }

            return label;
        }

        #endregion

        #region ---------- 系统参数设置 ----------

        /// <summary>
        /// 系统设置页面的保存按钮触发事件
        /// </summary>
        private void SYS_Model_Write(bool alterSuccessTip)
        {
            if (PlcIP.Text == String.Empty || PlcPort.Text == String.Empty || DeviceName.Text == String.Empty || RealtimeArgsUploadRate.Text == String.Empty)
            {
                MessageBox.Show("当前界面内容均为必填项、请先填写完善");
                return;
            }

            DataTable table1 = curMdb.Find("select * from SytemSet where ID = '1'");
            if (table1.Rows.Count > 0)
            {
                string sql = "update SytemSet set IP='" + PlcIP.Text + "',PortCode='" + PlcPort.Text + "'" +
                              ",DeviceName='" + DeviceName.Text + "'" +
                              ",PlcType='" + PlcConnectType.Text + "'" +
                              " where id = '1'";
                var result = curMdb.Change(sql);
                if (result && alterSuccessTip)
                {
                    MessageBox.Show("保存成功");
                }
            }
            else
            {
                string sql = "insert into SytemSet(id,DeviceName,IP,PortCode,PlcType)value(1,'" + DeviceName.Text + "','" + PlcIP.Text + "','" + PlcPort.Text + "','" + PlcConnectType.Text + "')";
                var result = curMdb.Change(sql);
                if (result && alterSuccessTip)
                {
                    MessageBox.Show("保存成功");
                }
            }
        }

        /// <summary>
        /// 读取生产信息
        /// </summary>
        private void GetProduction_Info()
        {
            DataTable table1 = RepositoryFactory.BaseRepository("connectionstring").GetDataTable("select * from ProductionInfo where ID = '1'");

            if (table1.Rows.Count > 0)
            {
                DataRow row = table1.Rows[0];
                txtUser.Text = row["Operator"].ToString(); //操作员
                OrderNum.Text = row["OrderQty"].ToString(); //工单数量
                OrderNo.Text = row["WorkNo"].ToString(); //工单号
            }
        }

        #endregion

        #region ---------- 显示列表 ----------

        // 用于存储不同工序的当前序号 Key=工序名称, Value=当前序号
        private readonly Dictionary<string, int> _processCounters = new Dictionary<string, int>();
        private DataTable boardTable;

        /// <summary>
        /// 从数据库加载并缓存检测项
        /// </summary>
        private void InitializeTestItemCache()
        {
            // 查询检测项
            boardTable = curMdb.Find("select * from Board");

            if (boardTable == null || boardTable.Rows.Count <= 0) return;

            id = boardTable.AsEnumerable().Select(row => row["id"].ToString()).ToArray();
            stationIdArray = boardTable.AsEnumerable().Select(row => row["WorkID"].ToString()).ToArray();
            testNameArray = boardTable.AsEnumerable().Select(row => row["BoardName"].ToString()).ToArray();
            realValuePointArray = boardTable.AsEnumerable().Select(row => row["BoardCode"].ToString()).ToArray();
            maxValuePointArray = boardTable.AsEnumerable().Select(row => row["MaxBoardCode"].ToString()).ToArray();
            minValuePointArray = boardTable.AsEnumerable().Select(row => row["MinBoardCode"].ToString()).ToArray();
            testResultPointArray = boardTable.AsEnumerable().Select(row => row["ResultBoardCode"].ToString()).ToArray();
            unitNameArray = boardTable.AsEnumerable().Select(row => row["BoardA1"].ToString()).ToArray();
        }

        /// <summary>
        /// 创建列表表头
        /// </summary>
        /// <param name="gridView">DataGridView控件</param>
        /// <param name="stationToken">工位标识</param>
        /// <param name="isEnableMutiStation">是否启用多工位</param>
        private void CreateHeaderText(DataGridView gridView, string stationToken = null, bool isEnableMutiStation = false)
        {
            // 清除现有列
            if (gridView.Columns.Count > 0)
            {
                gridView.Columns.Clear();
            }

            // 获取列结构
            var columnStructure = GetColumnStructure(stationToken, isEnableMutiStation);

            // 创建列
            foreach (var columnInfo in columnStructure)
            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = GenerateColumnHeaderText(columnInfo);

                // 为动态列设置自动调整大小
                if (columnInfo.ColumnType != "Basic")
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                gridView.Columns.Add(column);
            }
        }

        /// <summary>
        /// 获取列结构信息（统一的列定义逻辑）
        /// </summary>
        /// <param name="stationToken">工位标识</param>
        /// <param name="isEnableMutiStation">是否启用多工位</param>
        /// <returns>列信息列表</returns>
        private List<ColumnInfo> GetColumnStructure(string stationToken = null, bool isEnableMutiStation = false)
        {
            var columns = new List<ColumnInfo>
            {
                // 基本信息列（固定7列）
                new ColumnInfo { HeaderKey = nameof(Res.d2No), ColumnType = "Basic" },        // 序号
                new ColumnInfo { HeaderKey = nameof(Res.d2Barcode), ColumnType = "Basic" },   // 产品条码
                new ColumnInfo { HeaderKey = nameof(Res.d2Status), ColumnType = "Basic" },    // 过站状态
                new ColumnInfo { HeaderKey = nameof(Res.d2Result), ColumnType = "Basic" },    // 产品结果
                new ColumnInfo { HeaderKey = nameof(Res.d2Model), ColumnType = "Basic" },     // 产品型号
                new ColumnInfo { HeaderKey = nameof(Res.d2Human), ColumnType = "Basic" },     // 操作员
                new ColumnInfo { HeaderKey = nameof(Res.d2Time), ColumnType = "Basic" }       // 时间
            };

            // 测试项相关列（动态列）
            if (!(id?.Length > 0)) return columns;

            for (var i = 0; i < testNameArray.Length; i++)
            {
                // 检查是否应该包含此测试项（多工位筛选）
                var shouldInclude = true;
                if (isEnableMutiStation && !string.IsNullOrEmpty(stationToken))
                {
                    shouldInclude = (stationIdArray[i] == stationToken);
                }

                if (!shouldInclude) continue;

                var testItemName = testNameArray[i];
                var unit = unitNameArray?[i] ?? "";

                // 实际值列
                if (realValuePointArray[i] != "")
                {
                    columns.Add(new ColumnInfo
                    {
                        TestItemName = testItemName,
                        ColumnType = "Value",
                        Unit = unit,
                        TestItemIndex = i
                    });
                }

                // 上限值列
                if (maxValuePointArray[i] != "")
                {
                    columns.Add(new ColumnInfo
                    {
                        TestItemName = testItemName,
                        ColumnType = "UpperLimit",
                        Unit = unit,
                        TestItemIndex = i,
                        HeaderKey = "UpperLimit"
                    });
                }

                // 下限值列
                if (minValuePointArray[i] != "")
                {
                    columns.Add(new ColumnInfo
                    {
                        TestItemName = testItemName,
                        ColumnType = "LowerLimit",
                        Unit = unit,
                        TestItemIndex = i,
                        HeaderKey = "LowerLimit"
                    });
                }

                // 测试结果列
                if (testResultPointArray[i] != "")
                {
                    columns.Add(new ColumnInfo
                    {
                        TestItemName = testItemName,
                        ColumnType = "Result",
                        Unit = "",
                        TestItemIndex = i,
                        HeaderKey = "TestResult"
                    });
                }
            }

            return columns;
        }

        /// <summary>
        /// 生成列标题文本
        /// </summary>
        /// <param name="columnInfo">列信息</param>
        /// <returns>列标题文本</returns>
        private string GenerateColumnHeaderText(ColumnInfo columnInfo)
        {
            if (columnInfo.ColumnType == "Basic")
            {
                return resources.GetString(columnInfo.HeaderKey) ?? columnInfo.HeaderKey;
            }

            string testItemName = columnInfo.TestItemName ?? "";
            string unit = columnInfo.Unit ?? "";

            switch (columnInfo.ColumnType)
            {
                case "Value":
                    return testItemName + unit;

                case "UpperLimit":
                    string upperLimit = Res.UpperLimit;
                    return $"{testItemName} {upperLimit}{unit}";

                case "LowerLimit":
                    string lowerLimit = Res.LowerLimit;
                    return $"{testItemName} {lowerLimit}{unit}";

                case "Result":
                    string testResult = Res.TestResult;
                    return $"{testItemName} {testResult}";

                default:
                    return testItemName;
            }
        }

        private void ShowResult(DataGridView gridView, ReturnParamSendResult returnParam, UploadManagerEntity uploadManagerEntity,
            List<string> readBarcodes, List<string> productResultList, List<string> valList, List<string> maxList,
            List<string> minList, List<string> resList)
        {
            Invoke(new Action(() =>
            {
                string processName = uploadManagerEntity.Name;

                // 【序号管理】初始化或获取当前工序的计数器
                if (!_processCounters.ContainsKey(processName))
                {
                    _processCounters[processName] = 0;
                }

                // 【行数限制】
                if (gridView.RowCount > 500)
                {
                    gridView.Rows.RemoveAt(dgvResult1.Rows.Count - 1);
                }

                // 序号、时间、流程、条码、结果、型号、操作员、测试项...
                //string time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string time = System.DateTime.Now.ToString("MM-dd HH:mm:ss");
                string productModel = txtProductModel.Text;
                string loginUser = txtUser.Text;

                //DataGridViewRow dgvRow = new DataGridViewRow();
                //dgvRow.CreateCells(gridView);

                for (int i = 0; i < readBarcodes.Count; i++)
                {
                    // --- 计数器自增 (针对当前工序) ---
                    _processCounters[processName]++;
                    int currentNum = _processCounters[processName];

                    // --- 【关键修复】在循环内部创建新行对象 ---
                    DataGridViewRow dgvRow = new DataGridViewRow();
                    dgvRow.CreateCells(gridView);

                    string curBarcode = readBarcodes[i];
                    string mesStatus = returnParam?.Result ?? "Offline";
                    string curResult = productResultList[i] == "3" ? "OK" : "NG";

                    // --- 填充固定列 ---
                    dgvRow.Cells[0].Value = currentNum;         // 序号（独立计数）
                    dgvRow.Cells[1].Value = curBarcode;         // 产品条码
                    dgvRow.Cells[2].Value = mesStatus;          // 过站状态
                    dgvRow.Cells[3].Value = curResult;          // 产品结果
                    if (curResult == "NG")
                    {
                        dgvRow.Cells[3].Style.BackColor = Color.Red;
                        dgvRow.Cells[3].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        dgvRow.Cells[3].Style.BackColor = Color.Chartreuse;
                    }
                    dgvRow.Cells[4].Value = productModel;       // 生产型号
                    dgvRow.Cells[5].Value = loginUser;          // 操作员
                    dgvRow.Cells[6].Value = time;               // 时间

                    // --- 填充动态测试项数据 ---
                    int cellIndex = 7;

                    if (resList != null && resList.Count > 0)
                    {
                        for (int j = 0; j < resList.Count; j++)
                        {
                            // 越界检查：防止表格列数少于数据量导致报错
                            if (cellIndex >= gridView.Columns.Count) break;

                            // 1. 实际值
                            if (valList != null && valList.Count > j && valList[j] != "null")
                            {
                                dgvRow.Cells[cellIndex].Value = valList[j];
                                cellIndex++;
                            }

                            // 2. 上限
                            if (maxList != null && maxList.Count > j && maxList[j] != "null")
                            {
                                dgvRow.Cells[cellIndex].Value = maxList[j];
                                cellIndex++;
                            }

                            // 3. 下限
                            if (minList != null && minList.Count > j && minList[j] != "null")
                            {
                                dgvRow.Cells[cellIndex].Value = minList[j];
                                cellIndex++;
                            }

                            // 4. 结果
                            if (resList.Count > j && resList[j] != "null")
                            {
                                dgvRow.Cells[cellIndex].Value = resList[j];
                                cellIndex++;
                            }
                        }
                    }

                    gridView.Rows.Insert(0, dgvRow);
                }
            }));
        }

        #endregion

        #region ---------- MES接口 ----------

        /// <summary>
        /// 访问接口的统一接口
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <typeparam name="T2">传入类型</typeparam>
        /// <param name="inputParam">访问接口必备的内容实体</param>
        /// <param name="url">读取的url</param>
        /// <param name="function">固定参数</param>
        /// <param name="logFile"></param>
        /// <param name="logString"></param>
        /// <returns>返回接口返回的内容渲染成功的实体</returns>
        private T1 UniformInterface<T1, T2>(string url, string function, T2 inputParam, string logFile, string logString)
        {
            T1 returnParam = _request.GetResponseSerializeResult<T1, T2>(url, _httpClient, function, inputParam, logFile);

            if (returnParam == null)
            {
                if (!(logString is null))
                    writeLog.WriteLogToComponent(rtbErrorLog, string.Format(programLogString, logString));
                return default;
            }

            return returnParam;
        }

        /// <summary>
        /// 打印接口
        /// </summary>
        /// <returns></returns>
        private PrintBarCodeReturnParam PrintBarCodeInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            PrintBarCodeInputParam inputParam = JsonConvert.DeserializeObject<PrintBarCodeInputParam>(jsonString);

            // 在装配机工序2获取打印
            inputParam.Line = Line2.Text;
            inputParam.Process = Process2.Text;
            inputParam.Station = Station2.Text;
            inputParam.Device = Device2.Text;

            return UniformInterface<PrintBarCodeReturnParam, PrintBarCodeInputParam>(Url_PrintTemplate.Text, "GETPRINTDATA", inputParam, logFile, showString);
        }

        /// <summary>
        /// FTP信息获取接口
        /// </summary>
        /// <returns></returns>
        private FtpMessageGetReturnParam FtpMessageGetInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            FtpMessageGetInputParam inputParam = JsonConvert.DeserializeObject<FtpMessageGetInputParam>(jsonString);
            return UniformInterface<FtpMessageGetReturnParam, FtpMessageGetInputParam>(Url_DataUpload.Text, "GETTESTFILEFTPPATH", inputParam, logFile, showString);
        }

        /// <summary>
        /// 获取产品名称接口
        /// </summary>
        /// <returns></returns>
        private GetProductNameReturnParam GetProductNameInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            GetProductNameInputParam inputParam = JsonConvert.DeserializeObject<GetProductNameInputParam>(jsonString);
            return UniformInterface<GetProductNameReturnParam, GetProductNameInputParam>(Url_GetProductName.Text, "GETPRDNAME", inputParam, logFile, showString);
        }

        /// <summary>
        /// 设备心跳接口
        /// </summary>
        /// <returns></returns>
        private DeviceHeartBeatReturnParam DeviceHeartBeatInterface(string logFile, string showString)
        {
            DeviceHeartBeatInputParam inputParam = new DeviceHeartBeatInputParam();
            return UniformInterface<DeviceHeartBeatReturnParam, DeviceHeartBeatInputParam>(Url_Heartbeat.Text, "HEARTBEAT", inputParam, logFile, showString);
        }

        /// <summary>
        /// 设备状态上传接口
        /// </summary>
        /// <returns></returns>
        private ReturnParamDeviceStatus DeviceStatusUploadInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            InputParamDeviceStatus inputParam = JsonConvert.DeserializeObject<InputParamDeviceStatus>(jsonString);
            return UniformInterface<ReturnParamDeviceStatus, InputParamDeviceStatus>(Url_DeviceStatus.Text, "REPORTMACHINESTATUS", inputParam, logFile, showString);
        }

        /// <summary>
        /// 设备故障和预警数据上传接口
        /// </summary>
        /// <returns></returns>
        private DeviceErrorReturnParam DeviceErrorMessageUploadInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            DeviceEroorInputParam inputParam = JsonConvert.DeserializeObject<DeviceEroorInputParam>(jsonString);
            return UniformInterface<DeviceErrorReturnParam, DeviceEroorInputParam>(Url_ErrorInterface.Text, "REPORTMACHINEALARM", inputParam, logFile, showString);
        }

        /// <summary>
        /// 设备程序关键参数接口
        /// </summary>
        /// <returns></returns>
        private DeviceProgramKeyArgsReturnParam DeviceKeyArgsInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            DeviceProgramKeyArgsInputParam inputParam = JsonConvert.DeserializeObject<DeviceProgramKeyArgsInputParam>(jsonString);
            return UniformInterface<DeviceProgramKeyArgsReturnParam, DeviceProgramKeyArgsInputParam>(Url_KeyArgs.Text, "REPORTMACHINECONFIGPARAM", inputParam, logFile, showString);
        }

        /// <summary>
        /// 设备程序实时参数接口
        /// </summary>
        /// <returns></returns>
        private DeviceProgramRealtimeArgsReturnParam DeviceRealtimeArgsInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            DeviceProgramRealtimeArgsInputParam inputParam = JsonConvert.DeserializeObject<DeviceProgramRealtimeArgsInputParam>(jsonString);
            return UniformInterface<DeviceProgramRealtimeArgsReturnParam, DeviceProgramRealtimeArgsInputParam>(Url_ErrorInterface.Text, "REPORTMACHINEREALTIMEPARAM", inputParam, logFile, showString);
        }

        /// <summary>
        /// 更换工装接口
        /// </summary>
        /// <returns></returns>
        private DeviceProgramRealtimeArgsReturnParam ChangeToolingInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            ChangeToolingInputParam inputParam = JsonConvert.DeserializeObject<ChangeToolingInputParam>(jsonString);
            return UniformInterface<DeviceProgramRealtimeArgsReturnParam, ChangeToolingInputParam>(Url_ToolingChange.Text, "REPORTFIXTURECHANGE", inputParam, logFile, showString);
        }

        /// <summary>
        /// 更换铣刀接口
        /// </summary>
        /// <returns></returns>
        private DeviceProgramRealtimeArgsReturnParam ChangeMillingCutterInterface(JObject json, string logFile, string showString)
        {
            string jsonString = JsonConvert.SerializeObject(json);
            ChangeMillingCuuterInputParam inputParam = JsonConvert.DeserializeObject<ChangeMillingCuuterInputParam>(jsonString);
            return UniformInterface<DeviceProgramRealtimeArgsReturnParam, ChangeMillingCuuterInputParam>(Url_ToolingChange.Text, "REPORTFIXTURECHANGE", inputParam, logFile, showString);

        }

        #endregion

        #region ---------- 程序报警管理 ----------

        private readonly object _errorLock = new object();  // 线程锁
        private bool isBlockingMode = true;                 //  默认为阻塞模式
        private bool existErrorInErrorTip;                  // 全局阻塞锁：当前已经有错误在显示，为True时所有调用当前字段的方法都被暂停
        private ErrorEntity _currentActiveError;            // 当前处理的错误对象
        private readonly Queue<ErrorEntity> ErrorQueue = new Queue<ErrorEntity>();   // 错误队列

        /// <summary>
        /// 业务逻辑错误处理。
        /// <para>
        /// 此方法用于处理所有可预见的业务流程错误（如扫码失败、MES接口返回FAIL、生产结果异常等）。
        /// 它会执行以下操作：
        /// <para>1. 将错误信息写入UI日志（errorLog）和Log4net日志。 </para>
        /// 2. 向指定的PLC地址（feedbackAddress）写入NG信号（固定值2）。
        /// <para>3. 根据 isBlockingError 参数，决定是“阻塞”还是“非阻塞”错误。</para>
        /// 4. 更新UI界面（OperTip 或 errorTip）以向操作员显示错误。
        /// </para>
        /// </summary>
        /// <param name="feedbackAddress">需要写入NG信号（值 2）的PLC地址。如果为 null，则不写入PLC。</param>
        /// <param name="isBlockingError">
        /// 控制错误模式：
        /// <para>
        /// <b>true (阻塞模式):</b> 
        /// 视为严重错误。程序将设置 existErrorInErrorTip = true 来暂停其他后台任务，
        /// 并在UI上显示“手动清除”按钮。产线将停止，直到操作员手动点击报警清除按钮（ManualClear_Click）确认。
        /// 后续错误将进入队列（ErrorQueue）。
        /// </para>
        /// <para>
        /// <b>false (非阻塞模式):</b> 
        /// 视为非严重错误（如打印失败）。
        /// 程序仍会向PLC写入NG信号（2），但在 OperTip 中显示提示，不会停止产线，也不会显示“手动清除”按钮。
        /// </para>
        /// </param>
        /// <param name="userMessage">显示在UI上的、面向操作员的错误消息。</param>
        /// <param name="logMessage">（可选）写入日志文件的更详细的技术性错误信息。如果为 null，将默认使用 userMessage 的内容。</param>
        /// <returns>始终返回 <b>false</b>，以便调用方中断当前操作</returns>
        private bool HandleOperationError(string feedbackAddress, bool isBlockingError, string userMessage, string logMessage = null)
        {
            // 构建错误实体
            var errorData = new ErrorEntity()
            {
                FeedBackAddress = feedbackAddress,
                IsBlockingError = isBlockingError,
                UserMessage = userMessage,
                LogMessage = logMessage ?? userMessage,
                timeStamp = System.DateTime.Now
            };

            lock (_errorLock)   // 防止多线程并发竞争状态
            {
                // 如果当前已经有错误在显示（existErrorInErrorTip 为 true），
                // 或者当前有正在处理的错误对象 (_currentActiveError 不为空)
                // 存在错误提示 -> 暂存提示到错误列表
                if (existErrorInErrorTip || _currentActiveError != null)
                {
                    ErrorQueue.Enqueue(errorData);
                    //Log4netHelper.Info($"错误已加入队列，当前队列长度: {ErrorQueue.Count}");
                    return false;
                }

                // 如果没有错误，则立即显示当前错误
                ShowErrorToUi(errorData);
                return false;
            }
            /*// ---------- 1. 将错误信息写入UI日志（errorLog） ----------

            // 默认记录logMessage信息，当logMessage=null时记录userMessage
            string errMessage = string.Format(programLogString, System.DateTime.Now.ToString(), logMessage ?? userMessage);
            writeLog.WriteLogToComponent(errorLog, errMessage);

            // 阻塞错误
            if (isBlockingError)
            {
                // 记录阻塞错误到Error日志中
                Log4netHelper.Error($"触发错误报警:{errMessage},手动清除触发plc地址:{feedbackAddress}");

                // 显示当前报警信息到lblErrorTip控件
                CallUiSafely.ExecuteControlSafely(lblErrorTip, c => { c.Text = userMessage; c.ForeColor = Color.Red; });

                // 将反馈地址记录到按钮中
                CallUiSafely.ExecuteControlSafely(btnManualClear, c => c.Tag = feedbackAddress);

                // 如果当前为阻塞模式，通过 existErrorInErrorTip 暂停线程
                if (isBlockingMode)
                {
                    CallUiSafely.ExecuteControlSafely(btnManualClear, c => c.Visible = true);
                    existErrorInErrorTip = true;
                }
            }
            // 非阻塞错误
            else
            {
                Log4netHelper.Info($"触发错误提示:{errMessage},非堵塞错误无需手动清除");

                CallUiSafely.ExecuteControlSafely(OperTip, c => { c.Text = userMessage; c.ForeColor = Color.Red; });

                if (feedbackAddress == null) return false;

                if (!TryWriteInt16Value(feedbackAddress, 2))
                {
                    writeLog.WriteLogToComponent(errorLog, string.Format(programLogString, System.DateTime.Now.ToString(), $"【处理非阻塞错误】PLC连接异常，向'{feedbackAddress}'反馈错误信号失败"));

                    CallUiSafely.ExecuteControlSafely(lblErrorTip, c => { c.Text = "PLC通信失败,检查PLC连接后点击手动清除重试"; c.ForeColor = Color.Red; });

                    CallUiSafely.ExecuteControlSafely(btnManualClear, c => c.Tag = feedbackAddress);
                    CallUiSafely.ExecuteControlSafely(btnManualClear, c => c.Visible = true);
                    existErrorInErrorTip = true;
                }
            }

            // readBarcodes.Clear();         // 条码数组
            // productResultList.Clear();    // 结果数组
            return false;*/
        }

        /// <summary>
        /// 展示错误到UI界面
        /// </summary>
        /// <param name="errorData"></param>
        private void ShowErrorToUi(ErrorEntity errorData)
        {
            _currentActiveError = errorData; // 记录当前错误上下文
            //existErrorInErrorTip = true;     // 设置阻塞标志

            // 1.记录错误日志
            writeLog.WriteLogToComponent(rtbErrorLog, errorData.LogMessage);

            // 判断是否为阻塞错误
            if (errorData.IsBlockingError)
            {
                Log4netHelper.Error($"触发阻塞报警: {errorData.LogMessage}, 触发地址: {errorData.FeedBackAddress}");

                CallUiSafely.ExecuteControlSafely(lblErrorTip, c => { c.Text = errorData.UserMessage; c.ForeColor = Color.Red; });

                // 如果为阻塞错误，再根据当前程序的运行模式决定是否阻塞
                // 判断当前程序的运行模式
                // isBlockingMode==true：阻塞模式，
                // isBlockingMode==false：放行模式
                if (isBlockingMode)
                {
                    existErrorInErrorTip = true;     // 设置阻塞标志
                    CallUiSafely.ExecuteControlSafely(btnManualClear, c => c.Visible = true);
                }
            }
            else
            {
                // 非阻塞错误逻辑
                Log4netHelper.Info($"触发非阻塞提示: {errorData.LogMessage}");
                CallUiSafely.ExecuteControlSafely(lblOperTip, c => { c.Text = errorData.UserMessage; c.ForeColor = Color.Red; });

                // 尝试写入PLC (非阻塞也需要反馈)
                if (!string.IsNullOrEmpty(errorData.FeedBackAddress))
                {
                    if (!TryWriteInt16Value(errorData.FeedBackAddress, 2))
                    {
                        // 如果非阻塞反馈失败，升级为阻塞错误！
                        errorData.UserMessage = "PLC通信失败, 请检查连接后点击手动清除";
                        errorData.LogMessage = $"【通信失败】原错误: {errorData.LogMessage}";
                        errorData.IsBlockingError = true; // 强制转为需要手动清除
                        ShowErrorToUi(errorData); // 递归调用自己来显示阻塞界面
                        return;
                    }
                }

                // 非阻塞错误处理完后，立即释放状态，检查队列
                ClearCurrentErrorAndCheckQueue();
            }
        }

        private void ManualClear_Click(object sender, EventArgs e)
        {
            lock (_errorLock) // 加锁读取当前错误
            {
                if (_currentActiveError == null)
                {
                    // 防御性编程：理论上不该进这里，但如果进了，强制复位界面
                    ResetErrorUi();
                    return;
                }

                string feedbackAddress = _currentActiveError.FeedBackAddress;

                // 1. 写入 PLC 复位信号
                if (_readWriteNet is null || !TryWriteInt16Value(feedbackAddress, 2))
                {
                    MessageBox.Show($"无法清除错误，写入PLC地址 {feedbackAddress} 失败，请检查连接。");
                    return;
                }

                Log4netHelper.Info($"手动清除报警完成: {_currentActiveError.UserMessage}");

                // 2. 清除当前错误并检查队列
                ClearCurrentErrorAndCheckQueue();
            }

            /*Button btn = sender as Button;

            string feedBackAddress = CallUiSafely.GetControlPropertyValueSafely(btnManualClear, c => c.Tag.ToString());
            if (readWriteNet is null || !TryWriteInt16Value(feedBackAddress, 2))
            {
                MessageBox.Show("无法手动清除错误信息,请检查PLC连接后重试");
                return;
            }

            Log4netHelper.Error($"手动清除报警信息:{OperTip.Text}, Plc触发地址{btn.Tag},发送值2");

            // 清空所有控件
            // CallUiSafely.ExecuteControlSafely(ProductResult1, c => c.Text = "");
            CallUiSafely.ExecuteControlSafely(lblErrorTip, c => c.Text = string.Empty);
            CallUiSafely.ExecuteControlSafely(OperTip, c => c.Text = string.Empty);
            CallUiSafely.ExecuteControlSafely(barCode, c => c.Text = string.Empty);
            CallUiSafely.ExecuteControlSafely(ToolingNumber, c => c.Text = string.Empty);

            CallUiSafely.ExecuteControlSafely(btnManualClear, c => c.Visible = false);

            // ==========================================================
            // 【核心修复】: 无论队列是否有数据，当前这一条错误已经算“清除完成”了。
            // 必须立刻释放标志位，否则 HandleOperationError 会认为还在报错中，拒绝显示下一条。
            // ==========================================================
            existErrorInErrorTip = false;

            // 4. 检查队列是否有积压的错误
            if (ErrorQueue.Count > 0)
            {
                // 取出下一个错误
                var nextError = ErrorQueue.Dequeue();

                // 重新调用报错处理逻辑
                // 因为上面已经将 existErrorInErrorTip 设为 false，
                // HandleOperationError 会正常执行：显示错误文案 -> 将 manualClear 设为 Visible -> 将 existErrorInErrorTip 设为 true
                HandleOperationError(nextError.FeedBackAddress, nextError.IsBlockingError, nextError.UserMessage, nextError.LogMessage);
            }*/

            // 从队列中取下一个异常
            //if (ErrorQueue.Count == 0)
            //{
            //    existErrorInErrorTip = false;
            //    return;
            //}

            //var nextError = ErrorQueue.Dequeue();
            //HandleOperationError(nextError.FeedBackAddress, nextError.IsBlockingError, nextError.UserMessage, nextError.LogMessage);
        }

        // 【新增】统一的清理与队列检查逻辑
        private void ClearCurrentErrorAndCheckQueue()
        {
            // 1. 清理当前状态
            _currentActiveError = null;
            ResetErrorUi(); // 清空界面文字、隐藏按钮

            // 2. 检查队列
            lock (_errorLock)
            {
                if (ErrorQueue.Count > 0)
                {
                    var nextError = ErrorQueue.Dequeue();
                    // 显示下一个错误
                    // 注意：这里不需要再判断 existErrorInErrorTip，因为我们要无缝衔接
                    ShowErrorToUi(nextError);
                }
                else
                {
                    // 3. 只有队列真的空了，才释放全局阻塞锁，允许业务线程继续跑
                    existErrorInErrorTip = false;
                }
            }
        }

        // 辅助方法：重置UI
        private void ResetErrorUi()
        {
            CallUiSafely.ExecuteControlSafely(lblErrorTip, c => c.Text = string.Empty);
            CallUiSafely.ExecuteControlSafely(lblOperTip, c => c.Text = string.Empty);
            CallUiSafely.ExecuteControlSafely(barCode, c => c.Text = string.Empty);
            CallUiSafely.ExecuteControlSafely(ToolingNumber, c => c.Text = string.Empty);
            CallUiSafely.ExecuteControlSafely(btnManualClear, c => c.Visible = false);
        }

        #endregion

        #region ---------- 暂时不用的方法 ----------

        /// <summary>
        /// 调用更换工装接口
        /// </summary>
        /// <returns></returns>
        public DeviceProgramRealtimeArgsReturnParam ChangeToolingChange()
        {
            JObject json = new JObject
            {
                { "ProgramName", Program },
                { "User", txtUser.Text },
                { "Type", "A" },
                { "Pos", "LEFT" },
                { "OldFixtureNo", "" },
                { "OldPrdNo", "" },
                { "NewFixtureNo", "" },
                { "NwPrdNo", "" }
            };
            return ChangeToolingInterface(json, "更换工装", "访问更换工装接口失败");
        }

        /// <summary>
        /// 调用更换铣刀接口
        /// </summary>
        /// <returns></returns>
        public DeviceProgramRealtimeArgsReturnParam ChangeMillingCutter()
        {
            JObject json = new JObject
            {
                {"ProgramName", Program},
                {"User", "admin"},
                {"Type", "B"},
                {"Pos", "RIGHT"},
                {"Code", "1"},
                {"Description", "正常使用老化换刀"},
                {"Size", "" }
            };
            return ChangeMillingCutterInterface(json, "更换铣刀", "访问更换铣刀接口失败");
        }

        /// <summary>
        /// 调用FTP信息获取接口
        /// </summary>
        /// <returns></returns>
        public FtpMessageGetReturnParam FtpMessageGet()
        {
            JObject json = new JObject
            {
                {"QueryProcss", "AOI" },
                {"PrdSN", "810221-00451V1.6622435907866" },
                {"FileType", "T" }
            };
            return FtpMessageGetInterface(json, "FTP信息获取", "访问FTP信息获取接口失败");
        }

        /// <summary>
        /// 调用获取产品名称接口
        /// </summary>
        /// <returns></returns>
        public GetProductNameReturnParam GetProductName()
        {
            JObject json = new JObject
            {
                {"PrdSN", " 560220-01416-DP-V01-009" }
            };
            return GetProductNameInterface(json, "获取产品名称", "访问获取产品名称接口失败");
        }

        #endregion

        #region --------- 按钮事件处理器 ---------

        /// <summary>
        /// mes配置区域 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            DataTable table1 = curMdb.Find("select * from MesSetting where ID = '1'");
            if (table1.Rows.Count > 0)
            {
                string sql = "update [MesSetting] set [url]='" + url.Text + "',[Line]='" + Line.Text + "',[Process]='" + Process.Text + "',[Station]='" + Station.Text + "'" +
                              ",[MesKey]='" + MesKey.Text + "',[Security]='" + Security.Text + "',[Device]='" + Device.Text + "'" +
                              ",[PlanNo]='" + PlanNo.Text + "',[FTPlog]='" + FTPlog.Text + "',[FTPPIC]='" + FTPPIC.Text + "',[FTPID] = '" + FTPID.Text +
                              "',[FTPCODE] = '" + FTPCODE.Text + "',[SWVer] = '" + SWVer.Text + "',[HWVer] = '" + HWVer.Text + "'" +
                              " where [id] = '1'";
                var result = curMdb.Change(sql);
                curMdb.Del("delete from [interface]");
                int i = curMdb.DatatableToMdb("interface", InsertTable());
                if (result && i > 0)
                {
                    SaveSuccessRestartApp();
                }
                else
                {
                    MessageBox.Show("保存失败");
                }
            }
            else
            {
                string sql = "INSERT INTO MesSetting([id],[Line],[url],[Process],[Station],[MesKey]," +
                    "[Security],[Device],[PlanNo],[FTPlog],[FTPPIC],[FTPID],[FTPCODE],[SWVer],[HWVer]) " +
                    "values(1,'" + url.Text + "','" + Line.Text + "','" + Process.Text + "','" + Station.Text + "','" +
                    MesKey.Text + "','" + Security.Text + "','" + Device.Text + "','" + PlanNo.Text + "','" + FTPlog.Text +
                    "','" + FTPPIC.Text + "','" + FTPID.Text + "','" + FTPCODE.Text + "','" + SWVer.Text + "','" + HWVer.Text + "')";
                var result = curMdb.Change(sql);

                bool rl = curMdb.Del("delete from [interface]");
                int i = curMdb.DatatableToMdb("interface", InsertTable());
                if (result && i > 0)
                {
                    SaveSuccessRestartApp();
                }
                else
                {
                    MessageBox.Show("保存失败");
                }
            }
        }

        /// <summary>
        /// 生产配置页面保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductConfig_SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "UPDATE ProductConfig SET " +
                //$"token_verify={EnableTokenVerify.Checked}," +
                //$"handler_picture={EnableHandlerPicture.Checked}," +
                $"heartbeat_rate={int.Parse(HeartbeatUploadRate.Text)}," +
                $"barcode_rule='{BarcodeRule.Text}'," +
                $"KA_upload_rate={int.Parse(RealtimeArgsUploadRate.Text)}," +
                $"torque_prot='{torque_prot.Text}'," +
                $"status_upload={EnableDeviceStatusUpload.Checked}," +
                $"warning_upload={EnableWarningUpload.Checked}," +
                $"realtime_upload={EnableRealtimeArgsUpload.Checked}," +
                $"key_args_upload={EnablekeyArgsUpload.Checked}," +
                $"shield_barcode={BanReadBarcode.Checked}," +
                $"change_type={EnableTypeChangedVerify.Checked}," +
                $"barcode_verify={EnableBarcodeRuleVerify.Checked}," +
                $"get_next_board={EnableGetNextBoard.Checked}," +
                $"fluence_verify={EnableFluentVerify.Checked}," +
                $"result_upload={EnableResultUpload.Checked}," +
                $"print_code={EnablePrintCode.Checked}," +
                $"result_pass={EnableResultPass.Checked}," +
                $"tooling_program={EnableUpperTooling.Checked}," +
                $"enable_torque={enable_torque.Checked}," +
                $"mes_product_mode='{cboProductMode.Text}'," +
                $"EnforcePass='{cboEnforcePass.Text}'" +
                " where id=1";

                //SetTaskExit();

                bool result = curMdb.Change(sql);
                if (result)
                {
                    SaveSuccessRestartApp();

                }
                else
                {
                    MessageBox.Show("保存失败");
                    Load_ProductConfig();
                }
            }
            catch (Exception xe)
            {
                MessageBox.Show($"保存失败:{xe}");
            }
        }

        /// <summary>
        /// 只允许数据数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyAllowDigital_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 允许输入数字0-9和小数点（如果需要）  
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // 如果需要限制小数点只能输入一个  
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = false;
            //}
        }

        private void PrintSetSave_Click(object sender, EventArgs e)
        {
            string sql = "update PrinterSet set " +
                         $"printer_name='{printerName.Text}'," +
                         $"print_code_path='{printTemplatePath.Text}'" +
                         " where id=1";

            MessageBox.Show(curMdb.Change(sql) ? "保存成功" : "保存失败");
        }

        private void ManualChangeManufacturingOrder_Click(object sender, EventArgs e)
        {
            if (_readWriteNet is null || isPlcConnected == false)
            {
                MessageBox.Show("请先连接plc");
                return;
            }

            ManageOrderSwitch();
        }

        private void CopyDataGatherTable_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("引用会导致当前数据全部被删除，确定引用吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            //删除所有数据
            curMdb.Del("delete from KeyArgsPreserve");

            string sql = "select * from Board";
            DataTable dt = curMdb.Find(sql);

            foreach (DataRow row in dt.Rows)
            {
                sql = "insert into KeyArgsPreserve(name,standard,USL,LSL,unit) values(" +
                    $"'{row["BoardName"]}'," +
                    $"'{row["StandardCode"]}'," +
                    $"'{row["MaxBoardCode"]}'," +
                    $"'{row["MinBoardCode"]}'," +
                    $"'{row["BoardA1"]}'" +
                    ")";
                curMdb.Add(sql);
            }
            keyArgsRefreshButton.PerformClick();
        }

        private void Save_Process2_Click(object sender, EventArgs e)
        {
            if (Line2.Text == String.Empty || Process2.Text == String.Empty || Station2.Text == String.Empty || MesKey2.Text == String.Empty ||
                Security2.Text == String.Empty || Device2.Text == String.Empty)
            {
                MessageBox.Show("当前界面内容均为必填项、请先填写完善");
                return;
            }

            DataTable table1 = curMdb.Find("select * from BeforeProcess where ID = 1");
            string sql;
            if (table1.Rows.Count > 0)
            {
                sql = "update BeforeProcess set line='" + Line2.Text + "',process='" + Process2.Text + "'" +
                              ",station='" + Station2.Text + "'" + ",mes_key='" + MesKey2.Text + "'" +
                              ",security='" + Security2.Text + "'" + ",device='" + Device2.Text + "'" +
                              " where id = 1";
            }
            else
            {
                sql = $"insert into BeforeProcess(line,process,station,mes_key,security,device)value('{Line2.Text}','{Process2.Text}','{Station2.Text}','{MesKey2.Text}','{Security2.Text}','{Device2.Text}')";
            }

            MessageBox.Show(curMdb.Change(sql) ? "保存成功" : "保存失败");
        }

        private void Save_Process3_Click(object sender, EventArgs e)
        {
            if (Line3.Text == String.Empty || Process3.Text == String.Empty || Station3.Text == String.Empty || MesKey3.Text == String.Empty ||
                Security3.Text == String.Empty || Device3.Text == String.Empty)
            {
                MessageBox.Show("当前界面内容均为必填项、请先填写完善");
                return;
            }

            DataTable table1 = curMdb.Find("select * from Process3 where ID = 1");
            string sql;
            if (table1.Rows.Count > 0)
            {
                sql = "update Process3 set line='" + Line3.Text + "',process='" + Process3.Text + "'" +
                              ",station='" + Station3.Text + "'" + ",mes_key='" + MesKey3.Text + "'" +
                              ",security='" + Security3.Text + "'" + ",device='" + Device3.Text + "'" +
                              " where id = 1";
            }
            else
            {
                sql = $"insert into Process3(line,process,station,mes_key,security,device)value('{Line3.Text}','{Process3.Text}','{Station3.Text}','{MesKey3.Text}','{Security3.Text}','{Device3.Text}')";
            }

            MessageBox.Show(curMdb.Change(sql) ? "保存成功" : "保存失败");
        }

        private void ChangeDataBase_Click(object sender, EventArgs e)
        {
            AccessHelper mdbTemp = new AccessHelper(Global.Instance.SourceDataBase);
            string sql = $"update SystemDataBase set database_name='{deviceDataBase.Text}' where id=1";
            if (mdbTemp.Change(sql))
            {
                SaveSuccessRestartApp();
            }
            else
            {
                MessageBox.Show("切换失败");
            }
        }

        private void manualInputBarcode_Click(object sender, EventArgs e)
        {
            if (!isPlcConnected || _readWriteNet is null)
            {
                MessageBox.Show("请先连接plc");
                return;
            }

            TryWriteInt16Value(plcAddressInfo.ManualInputBarcodeTip, 0);

            btnManualInputBarcode.Visible = false;
            ManualInputBarcode inputBarcodeWindow = new ManualInputBarcode(_readWriteNet, plcAddressInfo);
            inputBarcodeWindow.Show();
        }

        private void ImportProductModelByCsv_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("是否覆盖当前数据，否则追加写入数据", "确认写入方式", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV文件 (*.csv)|*.csv";
            openFileDialog.Title = "选择文件";
            // 打开选择文件对话框
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            if (confirm == DialogResult.Yes)
            {
                curMdb.Del("delete from ChangeProductType");
                curMdb.Change("ALTER TABLE ChangeProductType ALTER COLUMN id COUNTER (1, 1)");
            }

            string filePath = openFileDialog.FileName;

            bool readSuccess = ReadCsv(filePath, out string errorMessage, out List<KeyValueEntity> fileContent);

            if (!readSuccess)
            {
                MessageBox.Show(errorMessage);
                return;
            }

            // 导入数据到数据库
            foreach (KeyValueEntity content in fileContent)
            {
                string sql = $"insert into ChangeProductType(product_type,barcode_match) values('{content.Key}','{content.Value}')";
                if (!curMdb.Add(sql))
                {
                    MessageBox.Show("无法将数据{content.Key},{content.Value}添加到数据库");
                    return;
                }
            }

            // 刷新数据
            changeTypeRefresh.PerformClick();
            MessageBox.Show("导入成功");
        }

        private bool ReadCsv(string csvFilePath, out string errorMessage, out List<KeyValueEntity> fileInfo)
        {
            errorMessage = null;
            fileInfo = new List<KeyValueEntity>();
            try
            {
                // 使用StreamReader打开文件  
                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    long lineNum = 1;
                    string line;
                    // 逐行读取文件  
                    while ((line = reader.ReadLine()) != null)
                    {
                        //忽略第一行
                        if (lineNum == 1)
                        {
                            lineNum++;
                            continue;
                        }

                        string[] values = line.Split(',');

                        if (values.Length != 2)
                        {
                            errorMessage = $"校验不通过,行:{lineNum}\n{line}";
                            return false;
                        }

                        fileInfo.Add(new KeyValueEntity()
                        {
                            Key = values[0],
                            Value = values[1]
                        });
                        lineNum++;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // 处理可能发生的任何异常  
                errorMessage = $"导入文件发生错误: {ex.Message}";
                return false;
            }
        }

        private void ManualRecovery_Click(object sender, EventArgs e)
        {
            if (isPlcConnected != true || _readWriteNet is null)
            {
                MessageBox.Show("请先连接PLC");
                return;
            }
            DialogResult confirm = MessageBox.Show("请确认手动复位", "确认复位", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                _readWriteNet.Write(plcAddressInfo.RecoverySignal, 1);
            }
        }

        private async void printTest_Click(object sender, EventArgs e)
        {
            // 1. 获取界面参数（在主线程获取，防止跨线程异常）
            string inputPath = printTemplatePath.Text;
            string targetPrinterName = printerName.Text.Trim();

            // 2. 路径预检查
            string targetFile;
            if (File.Exists(inputPath))
            {
                targetFile = inputPath;
            }
            else if (Directory.Exists(inputPath))
            {
                try
                {
                    string[] files = Directory.GetFiles(inputPath, "*.lab");
                    if (files.Length == 0)
                    {
                        MessageBox.Show($"在目录[{inputPath}]中未找到 .lab 模板文件");
                        return;
                    }
                    targetFile = files[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"读取目录失败: {ex.Message}");
                    return;
                }
            }
            else
            {
                MessageBox.Show($"路径无效或文件不存在：\r\n{inputPath}");
                return;
            }

            // 禁用按钮，防止重复点击
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 显示等待提示（可选，更新UI状态）
            // OperTip.Text = "正在后台处理打印，请稍候...";

            try
            {
                // 3. 【核心优化】将繁重的打印逻辑放入后台 Task 运行
                await Task.Run(() =>
                {
                    ApplicationClass csApp = null;
                    Document doc = null;

                    try
                    {
                        // 初始化 Codesoft
                        csApp = new ApplicationClass
                        {
                            Visible = false
                        };

                        // 打开模板 (ReadOnly = true)
                        csApp.Documents.Open(targetFile, true);
                        doc = csApp.ActiveDocument;

                        // 4. 【核心优化】处理打印机设置
                        // 逻辑：只有当“指定的打印机”与“模板当前保存的打印机”不一致时，才进行切换。
                        // 这样可以最大程度保留模板里设定的 参数（如深度、速度）。
                        if (!string.IsNullOrEmpty(targetPrinterName) &&
                            !doc.Printer.Name.Equals(targetPrinterName, StringComparison.OrdinalIgnoreCase))
                        {
                            doc.Printer.SwitchTo(targetPrinterName);
                        }

                        // 5. 执行打印
                        // 参数1：打印份数
                        doc.PrintDocument();

                        // 6. 资源释放
                        // 必须在后台线程中关闭，否则会拖慢前台
                        doc.Close(false); // 不保存修改
                        Marshal.ReleaseComObject(doc);
                        doc = null;

                        csApp.Quit();
                        Marshal.ReleaseComObject(csApp);
                        csApp = null;

                        // 7. 打印成功通知（切回UI线程）
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("打印指令已发送！");
                        }));
                    }
                    catch (Exception ex)
                    {
                        // 异常处理（切回UI线程显示错误）
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show($"后台打印发生异常:\r\n{ex.Message}");
                        }));
                    }
                    finally
                    {
                        // 确保资源被清理（双重保险）
                        try
                        {
                            if (doc != null) { doc.Close(false); Marshal.ReleaseComObject(doc); }
                            if (csApp != null) { csApp.Quit(); Marshal.ReleaseComObject(csApp); }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动打印任务失败: {ex.Message}");
            }
            finally
            {
                // 恢复按钮状态
                if (btn != null) btn.Enabled = true;
            }
        }

        private void UserAdd_Click(object sender, EventArgs e)
        {
            if (UId.Text == "" || UPwd.Text == "" || Priv.Text == "")
            {
                MessageBox.Show("请将信息填写完整");
                return;
            }

            DataTable result = sourceMdb.Find($"select * from userinfo where work_id='{UId.Text}'");
            if (result.Rows.Count != 0)
            {
                MessageBox.Show("工号已存在");
                return;
            }

            bool isSuccess = sourceMdb.Add($"insert into userinfo(work_id,pwd,privilege) values('{UId.Text}','{UPwd.Text}','{Priv.Text}')");
            if (!isSuccess)
            {
                MessageBox.Show("创建失败");
                return;
            }

            UserRefresh.PerformClick();
            MessageBox.Show("创建成功");
        }

        private void LogOut_Click(object sender, EventArgs e)
        {
            //// 启动应用程序的新实例  
            //System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);

            //// 终止当前进程  
            //System.Diagnostics.Process.GetCurrentProcess().Kill();

            //// 确保应用程序退出  
            //System.Windows.Forms.Application.Exit();

            // 1. 获取当前可执行文件路径
            string appPath = System.Windows.Forms.Application.ExecutablePath;

            // 2. 构建CMD命令：
            // "ping ... > nul" 用于制造约 1.5 秒的延时，确保旧进程完全退出
            // "start" 用于启动新程序
            string cmd = $"/c ping 127.0.0.1 -n 2 > nul & start \"\" \"{appPath}\"";

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("cmd.exe", cmd)
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden, // 隐藏黑框
                CreateNoWindow = true
            };

            // 3. 启动CMD代理进程
            System.Diagnostics.Process.Start(info);

            // 4. 彻底终止当前进程
            // 建议使用 Environment.Exit(0) 代替 Kill()，前者更安全，但 Kill() 也没问题因为有CMD延时兜底
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void SwitchBlockMode(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Text)
            {
                case "放行 模式":
                    btn.Text = "阻塞 模式";
                    isBlockingMode = false;
                    groupboxx.Text = "运行状态异常提示:当前放行模式";
                    break;
                default:
                    btn.Text = "放行 模式";
                    isBlockingMode = true;
                    groupboxx.Text = "运行状态异常提示:当前阻塞模式";
                    break;
            }
        }

        private void StartTask_Click(object sender, EventArgs e)
        {
            SetDynamicTaskStart();

            permanentTaskCts?.Dispose();
            permanentTaskCts = new CancellationTokenSource();
        }

        private void EndTask_Click(object sender, EventArgs e)
        {
            //if (_dynamicTaskCts != null && !_dynamicTaskCts.IsCancellationRequested)
            //    _dynamicTaskCts.Cancel();

            if (permanentTaskCts != null && !permanentTaskCts.IsCancellationRequested)
                permanentTaskCts.Cancel();

            //_allDynamicTaskList.Clear();
        }

        /// <summary>
        /// 保存按钮 > 系统设置页面 > 其它设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemSetSaveButton_Click(object sender, EventArgs e)
        {
            SYS_Model_Write(true);
        }

        #endregion

        /// <summary>
        /// 管理工单切换
        /// </summary>
        private void ManageOrderSwitch()
        {
            var form = new Form3();
            form.MdiParent = ParentForm;
            form.ValueSelected += (selectedValue) =>
            {
                // 通知PLC继续生产
                _readWriteNet.Write(plcAddressInfo.ContinueProduce, Convert.ToInt16(1));

                var form3Entity = selectedValue;
                OrderNo.Text = form3Entity.GDH;
                OrderNum.Text = form3Entity.GDSL.ToString();
                txtUser.Text = form3Entity.CZY;

                lblOperTip.Text = "工单切换完成！";
                lblOperTip.ForeColor = Color.Green;
            };

            form.ShowDialog();
        }

        /// <summary>
        /// 移动文件，将文件从localPath移动到以
        /// </summary>
        /// <param name="address"></param>
        /// <param name="prdSn"></param>
        /// <param name="folderName"></param>
        private void MoveFile(string prdSn, string address, string folderName = "PrdSNPictures")
        {
            // 拼接本地目录
            string localPath = Path.Combine(CallUiSafely.GetControlPropertyValueSafely(LocalFilePath, c => c.Text), folderName);

            // 获取已经存在的图片
            Dictionary<string, string> paths = GetPrintDirectories(address);
            if (paths is null) return;

            // 创建当前prdSN的文件夹
            string curPrdSnDirectory = Path.Combine(localPath, prdSn);
            Directory.CreateDirectory(curPrdSnDirectory);

            foreach (KeyValuePair<string, string> kv in paths)
            {
                string fileFullName = Path.Combine(kv.Value, kv.Key);

                // 复制文件
                FileInfo fileInfo = new FileInfo(fileFullName);
                fileInfo.CopyTo(Path.Combine(curPrdSnDirectory, kv.Key), true);

                // 强制删除文件，可以会误杀线程
                Resource.ForceDeleteFile(fileFullName);
            }
        }

        /// <summary>
        /// 获取打印文件和文件目录
        /// </summary>
        /// <param name="plcAddress">plc地址</param>
        /// <returns>如果Count为0就说明没有获取到,文件名:路径</returns>
        private Dictionary<string, string> GetPrintDirectories(string plcAddress)
        {
            lock (_getPathLock)
            {
                string orderNum = CallUiSafely.GetControlPropertyValueSafely(txtProductModel, c => c.Text);
                string sql = $"select * from PrinterDirectory where order_num='{orderNum}'";
                DataTable dt = curMdb.Find(sql);

                if (dt.Rows.Count < 1) return new Dictionary<string, string>();

                // 确认当前这个type下的文件路径
                string orderDir = dt.Rows[0]["order_directory"].ToString();
                string type = dt.Rows[0]["type"].ToString();

                Dictionary<string, string> paths = new Dictionary<string, string>();
                string path = dt.Rows[0]["path"].ToString();
                //mutiple_photo:标签检测,定位检测,封口检测
                string[] folders = dt.Rows[0]["mutiple_photo"].ToString().Split(',');
                string[] plcAddresses = dt.Rows[0]["plc_address"].ToString().Split(',');
                string[] pictureNum = dt.Rows[0]["picture_num"].ToString().Split(',');

                // 记录下当前plc维护地址的索引，方便找出目录名
                int index = Array.IndexOf(plcAddresses, plcAddress);
                //如果index为-1就两种情况，1:这是结果上传时赋值的信号，直接跳过，2:维护出错. 所有情况直接返回null就好了
                if (index == -1) return null;

                string curPath = Path.Combine(path, folders[index], orderDir, type);

                DirectoryInfo dirInfo = new DirectoryInfo(curPath);

                // 图片维护数据分割出来的长度大于1，说明是装配机的，需要等待全部的图片数
                if (pictureNum.Length > 1)
                {
                    System.DateTime startTime = System.DateTime.Now;
                    while (true)
                    {
                        if (dirInfo.GetFiles().Length == int.Parse(pictureNum[index]))
                            break;
                        if ((System.DateTime.Now - startTime).Seconds >= 20) return null;
                    }
                }

                //找出以相关命名开头的全部文件，后面的第几张是不确定的，所以只能筛选出所有与当前prdSN相关的文件
                //List<string> curPicutres = fileInfo.GetFiles().Where(x => x.Name.StartsWith($"{PrdSNInfo.PrdSN}_{okOrNg}")).Select(x => x.Name).ToList();
                List<string> curPictures = dirInfo.GetFiles().Select(x => x.Name).ToList();
                foreach (string file in curPictures)
                {
                    paths.Add(file, curPath);  //文件名: 路径
                }

                return paths;
            }
        }

        /// <summary>
        /// 通过plc地址创建本地数据库记录并获得本地数据库记录的id
        /// </summary>
        /// <param name="plcAddress"></param>
        /// <param name="errorReferenceTable"></param>
        /// <returns></returns>
        private string _ByPlcAdressInsertToDbGetId(string plcAddress, DataTable errorReferenceTable)
        {
            //从之前查询过的数据中获取到plc地址对应的数据id,防止多次查询数据库
            DataRow[] foundRows = errorReferenceTable.Select($"plc_point = '{plcAddress}'");
            //通过plc地址查出ErrorReferenceTable中的id号
            try
            {
                int ref_id = (int)foundRows[foundRows.Length - 1]["id"];
                string uniqueKey = GenerateUniqueGuid();
                //插入数据到ErrorMessage
                string sql = $"insert into ErrorMessage(ref_id,unique_key) values({ref_id},'{uniqueKey}')";
                if (curMdb.Add(sql)) return uniqueKey;
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 生成唯一标识
        /// </summary>
        /// <returns></returns>
        private string GenerateUniqueGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 解析配置字符串并从 PLC 读取处理后的数据
        /// </summary>
        /// <param name="configString">
        /// 配置字符串，格式："[PLC地址]:[数据类型]-[计算规则]"。
        /// <para>示例："D1020:I-0" 或 "D1021:F-2"</para>
        /// </param>
        /// <param name="resultValue">输出处理后的值（动态类型）</param>
        /// <returns>读取并处理成功返回 true，否则返回 false</returns>
        private bool TryGetProcessedValue(string configString, out dynamic resultValue)
        {
            resultValue = null;

            // 1. 基础校验
            if (string.IsNullOrWhiteSpace(configString)) return false;

            try
            {
                // 2. 解析配置字符串
                // 格式: Address:Type-Compute
                string[] mainParts = configString.Split(':');

                string plcAddress = mainParts[0];
                string[] typeParts = mainParts[1].Split('-');

                string dataType = typeParts[0].ToUpper();   // 数据类型 (H, I, F)
                string computeRule = typeParts[1];          // 计算规则 (0-4)

                // 3. 定义临时变量用于接收原始读取结果
                // 使用 dynamic 避免重复写 if (isSuccess) 逻辑，或者使用 object 存储 Content
                bool isReadSuccess;
                dynamic rawContent;

                // 4. 执行读取 (合并读取逻辑)
                switch (dataType)
                {
                    case "H": // Int16
                        var shortRes = _readWriteNet.ReadInt16(plcAddress);
                        isReadSuccess = shortRes.IsSuccess;
                        rawContent = shortRes.Content;
                        break;
                    case "I": // Int32
                        var intRes = _readWriteNet.ReadInt32(plcAddress);
                        isReadSuccess = intRes.IsSuccess;
                        rawContent = intRes.Content;
                        break;
                    case "F": // Float
                        var floatRes = _readWriteNet.ReadFloat(plcAddress);
                        isReadSuccess = floatRes.IsSuccess;
                        rawContent = floatRes.Content;
                        break;
                    default:  // 默认按 Int16 处理
                        var defRes = _readWriteNet.ReadInt16(plcAddress);
                        isReadSuccess = defRes.IsSuccess;
                        rawContent = defRes.Content;
                        break;
                }

                // 5. 如果读取成功，进行数值计算
                if (isReadSuccess)
                {
                    resultValue = CalculateValue(rawContent, computeRule);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                // 建议记录日志: Log($"解析或读取异常: {configString}, {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 根据规则对原始数据进行计算转换
        /// </summary>
        /// <param name="rawValue">PLC 读取的原始值（可能是 short, int, float）</param>
        /// <param name="ruleCode">计算规则代码 (0-4)</param>
        /// <returns>计算后的值</returns>
        private dynamic CalculateValue(dynamic rawValue, string ruleCode)
        {
            // 预处理：将原始值转为 double 以防止整数除法精度丢失
            // 注意：如果是字符串比较逻辑（如 ruleCode="4"），则不需要转换
            bool isNumber = double.TryParse(rawValue.ToString(), out double val);

            switch (ruleCode)
            {
                case "0": // 原值
                    return rawValue;

                case "1": // ÷ 10
                    return isNumber ? val / 10.0 : 0;

                case "2": // ÷ 100
                    return isNumber ? val / 100.0 : 0;

                case "3": // ÷ 1000
                    return isNumber ? val / 1000.0 : 0;

                case "4": // 状态判断 (OK/NG)
                          // 假设 PLC 值为 3 代表 OK，其他代表 NG
                          // 这里建议兼容数值类型比较，避免类型不匹配
                    return Convert.ToInt32(rawValue) == 3 ? "OK" : "NG";

                default: // 默认原值
                    return rawValue;
            }
        }

        /// <summary>
        /// 关键参数和实时参数获取请求的json数据
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        private JObject ArgsGetRequestJson(JArray jArray)
        {
            return new JObject
                {
                    { "ProgramName",Program },
                    {"Swver",SWVer.Text },
                    {"User",txtUser.Text },
                    {"Datas", new JObject{ { "Data", jArray } } }
                };
        }

        /// <summary>
        /// 删除图片文件夹
        /// </summary>
        /// <param name="barCodes"></param>
        /// <param name="folder"></param>
        private void DeletePicture(List<string> barCodes, string folder = "PrdSNPictures")
        {
            foreach (string PrdSN in barCodes)
            {
                string localPath = Path.Combine(CallUiSafely.GetControlPropertyValueSafely(LocalFilePath, c => c.Text), folder, PrdSN);
                if (!Directory.Exists(localPath)) continue;
                Directory.Delete(localPath, true);
            }
        }

        /// <summary>
        /// 保存json数据到本地文件
        /// </summary>
        private void SaveTxtFileToLocal(string file, string str)
        {
            Byte[] txtBytes = Encoding.UTF8.GetBytes(str);

            try
            {
                //创建目录
                Directory.CreateDirectory(Path.Combine(LocalFilePath.Text, "Txt"));

                using (FileStream ioStream = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    // 将字节流写入文件
                    ioStream.Write(txtBytes, 0, txtBytes.Length);
                }
            }
            catch (IOException)
            {
                // 文件I/O相关的异常，例如文件已存在且不允许覆盖，磁盘空间不足等  
                throw new FileException("处理文件I/O相关的异常");
            }
            catch (Exception ex)
            {
                // 处理其他类型的异常  
                throw new FileException($"保存文件发生其它错误：{ex.Message}");
            }
        }

        private readonly object _getPathLock = new object();

        private void SaveSuccessRestartApp()
        {
            DialogResult confirm = MessageBox.Show("成功，重启后重新登录生效", "确认重启", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                // 1. 获取当前可执行文件路径
                string appPath = System.Windows.Forms.Application.ExecutablePath;

                // 2. 构建CMD命令：
                // "ping ... > nul" 用于制造约 1.5 秒的延时，确保旧进程完全退出
                // "start" 用于启动新程序
                string cmd = $"/c ping 127.0.0.1 -n 2 > nul & start \"\" \"{appPath}\"";

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("cmd.exe", cmd)
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden, // 隐藏黑框
                    CreateNoWindow = true
                };

                // 3. 启动CMD代理进程
                System.Diagnostics.Process.Start(info);

                // 4. 彻底终止当前进程
                // 建议使用 Environment.Exit(0) 代替 Kill()，前者更安全，但 Kill() 也没问题因为有CMD延时兜底
                //System.Diagnostics.Process.GetCurrentProcess().Kill();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 将接口数据写入 datable
        /// </summary>
        /// <returns></returns>
        public DataTable InsertTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("InterfaceName");
            dt.Columns.Add("InterfaceUrl");

            string[] interfaceNames = { "GetToken", "GetSnCode", "checkPath", "UploadResults", "GetFtp", "GetProductName", "heartbeat", "DeviceStatus", "fault", "AlterProcedure", "ActualTimeParam", "FixtureReplacement", "PrintAddress", "LocalFileDir" };
            string[] textBoxes = { Url_Token.Text, UrlPanelization.Text, Url_RouteCheck.Text, Url_DataUpload.Text, Url_FTPMessGet.Text, Url_GetProductName.Text, Url_Heartbeat.Text, Url_DeviceStatus.Text, Url_ErrorInterface.Text, Url_KeyArgs.Text, Url_RealtimeArgs.Text, Url_ToolingChange.Text, Url_PrintTemplate.Text, LocalFilePath.Text };

            for (int i = 0; i < interfaceNames.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dr["InterfaceName"] = interfaceNames[i];
                dr["InterfaceUrl"] = textBoxes[i];
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public enum Result
        {
            Pass,
            Fail
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ---------------- 条码验证 ----------------
            plcAddressInfo.HasBarcodeTag = txtHasBarcodeTag.Text;
            plcAddressInfo.BarcodeVerifyTag = txtBarcodeVerifyTag.Text;
            plcAddressInfo.BarcodeType = txtBarcodeType.Text;

            plcAddressInfo.PlcScannedBarcode = txtPlcScanned.Text;
            plcAddressInfo.PlcScannedBarcodeLength = txtScannedLength.Text;
            plcAddressInfo.PanalizationBarcode = txtPanalizationBarcode.Text;
            plcAddressInfo.PanalizationBarcodeLength = txtPanalizationLength.Text;

            plcAddressInfo.ManualInputBarcodeTip = txtManualInput.Text;
            plcAddressInfo.ManualInputBarcode = txtManualBarcode.Text;
            plcAddressInfo.ManualInputBarcodeLength = txtManualLength.Text;

            // ---------------- 数据上传 ----------------
            plcAddressInfo.TriggerUpload1 = txtTriggerUpload1.Text;
            plcAddressInfo.TriggerUpload2 = txtTriggerUpload2.Text;
            plcAddressInfo.TriggerUpload3 = txtTriggerUpload3.Text;

            plcAddressInfo.Feedback1 = txtFeedback1.Text;
            plcAddressInfo.Feedback2 = txtFeedback2.Text;
            plcAddressInfo.Feedback3 = txtFeedback3.Text;

            plcAddressInfo.ProductResult1 = txtProductResult1.Text;
            plcAddressInfo.ProductResult2 = txtProductResult2.Text;
            plcAddressInfo.ProductResult3 = txtProductResult3.Text;

            plcAddressInfo.BarcodeToUpload1 = txtBarcodeToUpload1.Text;
            plcAddressInfo.BarcodeToUpload2 = txtBarcodeToUpload2.Text;
            plcAddressInfo.BarcodeToUpload3 = txtBarcodeToUpload3.Text;

            plcAddressInfo.BarcodeToUploadLength1 = txtBarcodeToUploadLength1.Text;
            plcAddressInfo.BarcodeToUploadLength2 = txtBarcodeToUploadLength2.Text;
            plcAddressInfo.BarcodeToUploadLength3 = txtBarcodeToUploadLength3.Text;

            // ---------------- 打印条码 ----------------
            plcAddressInfo.PrintTrigger = txtPrintTrigger.Text;
            plcAddressInfo.PrintFeedback = txtPrintFeedback.Text;
            plcAddressInfo.BarcodeToPrint = txtBarcodeToPrint.Text;
            plcAddressInfo.BarcodeToPrintLenght = txtBarcodeToPrintLength.Text;

            // 设备参数
            plcAddressInfo.GoodsProducts = txtGoodsProducts.Text;
            plcAddressInfo.NotGoodsProducts = txtNotGoodsProducts.Text;
            plcAddressInfo.ProduceCount = txtProduceCount.Text;
            plcAddressInfo.DeviceStatus = txtDeviceStatus.Text;
            plcAddressInfo.DeviceProgramName = txtDeviceProgramName.Text;
            plcAddressInfo.ProgramNameLength = txtProgramNameLength.Text;
            plcAddressInfo.ProductType = txtProductType.Text;
            plcAddressInfo.ProductTypeLength = txtProductTypeLength.Text;
            plcAddressInfo.BarcodeRule = txtBarcodeRule.Text;
            plcAddressInfo.BarcodeRuleLength = txtBarcodeRuleLength.Text;
            plcAddressInfo.ModelSwitch = txtModelSwitch.Text;
            plcAddressInfo.ContinueProduce = txtContinueProduce.Text;
            plcAddressInfo.PlcHeartBeat = txtPlcHeartBeat.Text;
            plcAddressInfo.PcHeartBeat = txtPcHeartBeat.Text;
            plcAddressInfo.RecoverySignal = txtRecoverySignal.Text;

            // ---------------- 扭力转发 ----------------
            plcAddressInfo.TorqueValue1 = txtTorqueValue1.Text;
            plcAddressInfo.TorqueValue3 = txtTorqueValue3.Text;

            plcAddressInfo.TorqueResult1 = txtTorqueResult1.Text;
            plcAddressInfo.TorqueResult3 = txtTorqueResult3.Text;

            plcAddressInfo.TorqueMax1 = txtToqueMax1.Text;
            plcAddressInfo.TorqueMax3 = txtToqueMax3.Text;

            plcAddressInfo.TorqueMin1 = txtToqueMin1.Text;
            plcAddressInfo.TorqueMin3 = txtToqueMin3.Text;

            plcAddressInfo.Request1 = txtRequest1.Text;
            plcAddressInfo.Request3 = txtRequest3.Text;

            plcAddressInfo.Acknowledge1 = txtAcknowledge1.Text;
            plcAddressInfo.Acknowledge3 = txtAcknowledge3.Text;

            MessageBox.Show(plcAddressInfo.Save());
        }

        private void LoadPlcAddress()
        {
            if (plcAddressInfo != null)
            {
                // ---------------- 条码验证 ----------------
                txtHasBarcodeTag.Text = plcAddressInfo.HasBarcodeTag;
                txtBarcodeVerifyTag.Text = plcAddressInfo.BarcodeVerifyTag;
                txtBarcodeType.Text = plcAddressInfo.BarcodeType;

                txtPlcScanned.Text = plcAddressInfo.PlcScannedBarcode;
                txtScannedLength.Text = plcAddressInfo.PlcScannedBarcodeLength;
                txtPanalizationBarcode.Text = plcAddressInfo.PanalizationBarcode;
                txtPanalizationLength.Text = plcAddressInfo.PanalizationBarcodeLength;

                txtManualInput.Text = plcAddressInfo.ManualInputBarcodeTip;
                txtManualBarcode.Text = plcAddressInfo.ManualInputBarcode;
                txtManualLength.Text = plcAddressInfo.ManualInputBarcodeLength;

                // ---------------- 数据上传 ----------------
                txtTriggerUpload1.Text = plcAddressInfo.TriggerUpload1;
                txtTriggerUpload2.Text = plcAddressInfo.TriggerUpload2;
                txtTriggerUpload3.Text = plcAddressInfo.TriggerUpload3;

                txtFeedback1.Text = plcAddressInfo.Feedback1;
                txtFeedback2.Text = plcAddressInfo.Feedback2;
                txtFeedback3.Text = plcAddressInfo.Feedback3;

                txtProductResult1.Text = plcAddressInfo.ProductResult1;
                txtProductResult2.Text = plcAddressInfo.ProductResult2;
                txtProductResult3.Text = plcAddressInfo.ProductResult3;

                txtBarcodeToUpload1.Text = plcAddressInfo.BarcodeToUpload1;
                txtBarcodeToUpload2.Text = plcAddressInfo.BarcodeToUpload2;
                txtBarcodeToUpload3.Text = plcAddressInfo.BarcodeToUpload3;

                txtBarcodeToUploadLength1.Text = plcAddressInfo.BarcodeToUploadLength1;
                txtBarcodeToUploadLength2.Text = plcAddressInfo.BarcodeToUploadLength2;
                txtBarcodeToUploadLength3.Text = plcAddressInfo.BarcodeToUploadLength3;

                // ---------------- 打印条码 ----------------
                txtPrintTrigger.Text = plcAddressInfo.PrintTrigger;
                txtPrintFeedback.Text = plcAddressInfo.PrintFeedback;
                txtBarcodeToPrint.Text = plcAddressInfo.BarcodeToPrint;
                txtBarcodeToPrintLength.Text = plcAddressInfo.BarcodeToPrintLenght;

                // ---------------- 设备参数 ----------------
                txtGoodsProducts.Text = plcAddressInfo.GoodsProducts;
                txtNotGoodsProducts.Text = plcAddressInfo.NotGoodsProducts;
                txtProduceCount.Text = plcAddressInfo.ProduceCount;
                txtDeviceStatus.Text = plcAddressInfo.DeviceStatus;
                txtDeviceProgramName.Text = plcAddressInfo.DeviceProgramName;
                txtProgramNameLength.Text = plcAddressInfo.ProgramNameLength;
                txtProductType.Text = plcAddressInfo.ProductType;
                txtProductTypeLength.Text = plcAddressInfo.ProductTypeLength;
                txtBarcodeRule.Text = plcAddressInfo.BarcodeRule;
                txtBarcodeRuleLength.Text = plcAddressInfo.BarcodeRuleLength;
                txtModelSwitch.Text = plcAddressInfo.ModelSwitch;
                txtContinueProduce.Text = plcAddressInfo.ContinueProduce;
                txtPlcHeartBeat.Text = plcAddressInfo.PlcHeartBeat;
                txtPcHeartBeat.Text = plcAddressInfo.PcHeartBeat;
                txtRecoverySignal.Text = plcAddressInfo.RecoverySignal;

                // ---------------- 扭力转发 ----------------
                txtTorqueValue1.Text = plcAddressInfo.TorqueValue1;
                txtTorqueValue3.Text = plcAddressInfo.TorqueValue3;

                txtTorqueResult1.Text = plcAddressInfo.TorqueResult1;
                txtTorqueResult3.Text = plcAddressInfo.TorqueResult3;

                txtToqueMax1.Text = plcAddressInfo.TorqueMax1;
                txtToqueMax3.Text = plcAddressInfo.TorqueMax3;

                txtToqueMin1.Text = plcAddressInfo.TorqueMin1;
                txtToqueMin3.Text = plcAddressInfo.TorqueMin3;

                txtRequest1.Text = plcAddressInfo.Request1;
                txtRequest3.Text = plcAddressInfo.Request3;

                txtAcknowledge1.Text = plcAddressInfo.Acknowledge1;
                txtAcknowledge3.Text = plcAddressInfo.Acknowledge3;
            }
        }

        private void btnChangePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Codesoft模板 (*.lab)|*.lab|所有文件 (*.*)|*.*";
            dlg.Title = "选择打印文件";

            if (dlg.ShowDialog() != DialogResult.OK) return;

            printTemplatePath.Text = dlg.FileName;

            string sql = "update PrinterSet set " +
                         $"printer_name='{printerName.Text}'," +
                         $"print_code_path='{printTemplatePath.Text}'" +
                         " where id=1";

            MessageBox.Show(curMdb.Change(sql) ? "保存成功" : "保存失败");
        }

        /// <summary>
        /// 快速定位至文件所在位置
        /// </summary>
        /// <param name="filePath"></param>
        private static void ShowFileInExplorer(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = "/select," + filePath // 使用/select参数定位到文件本身
                        };
                        System.Diagnostics.Process.Start(processStartInfo);
                        Console.WriteLine("文件已在'此电脑'中定位并选中。");
                    }
                    catch (Exception ex)
                    {
                        // 处理异常  
                        Console.WriteLine("定位文件时发生错误：");
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("文件不存在。");
                }
            }
        }

        private void btnShowPath_Click(object sender, EventArgs e)
        {
            string filePath = printTemplatePath.Text;
            ShowFileInExplorer(filePath);
        }
    }
}