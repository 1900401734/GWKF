using HslCommunication;
using HslCommunication.Core;
using LabelManager2;
using MesDatas.DataAcess;
using MesDatas.Models;
using MesDatas.MyEnum;
using MesDatas.Services;
using MesDatas.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Res = MesDatas.Properties.Resources;

namespace MesDatas.Views
{
    public partial class Form1 : Form
    {
        private static RequestMes _request;         // mes访问类
        private static JObject _getTokenJson;       // 获取token的json，用于初始化HttpClientUtil对象
        private static HttpClientUtil _httpClient;
        private const string ProductModeUploadAfterFeedback = "先反馈再上传"; // 采集完成后先反馈PLC，再后台上传MES
        private const string WeightMesPassConfirmed = "ConfirmedPass";       // Weight工序MES确认PASS后，才允许打印线程调用标签接口
        private const string DefaultMesSaveResultTimeoutSeconds = "30";      // MES过站接口默认超时时间，单位：秒
        private const string DefaultTorqueAckTimeoutSeconds = "3";           // PLC接收扭力ACK默认超时时间，单位：秒
        private const string TorqueAckTimeoutModeResetAndAlarm = "超时清Req并报警"; // PLC接收扭力ACK超时后清零Req并提示异常
        private const int MinMesSaveResultTimeoutSeconds = 5;                // 过小会导致现场网络抖动时误判超时
        private const int MaxMesSaveResultTimeoutSeconds = 300;              // 过大会导致同步过站模式长时间阻塞
        private const int MinTorqueAckTimeoutSeconds = 1;                    // 过小会导致PLC扫描周期抖动时误判ACK超时
        private const int MaxTorqueAckTimeoutSeconds = 60;                   // 过大会导致扭力转发锁长时间占用
        private const int TorqueAckInitialTimeoutMs = 3000;                  // PLC接收扭力ACK初始等待时间
        private const int TorqueAckPollIntervalMs = 50;                      // ACK轮询间隔，避免错过PLC短暂置位
        private const int WeightMesStatusCacheLoadDays = 7;                  // 启动时加载最近Weight MES状态，覆盖周末和短期停机重启
        private const int WeightMesStatusCacheRetentionDays = 30;            // 轻量缓存保留天数，避免本地文件长期堆积
        private const int ProductionUiLogMaxLines = 500;                       // 限制生产日志控件体积，避免切页和重绘卡顿
        private Assembly assembly;
        private ResourceManager resources;
        private PlcAddressInfo addrInfo;
        private DataAcess.SystemInfo systemInfo;
        private PLCAdress plcAddress;
        private readonly MesOutboxStore _mesOutboxStore = new MesOutboxStore();
        private readonly WeightMesStatusStore _weightMesStatusStore = new WeightMesStatusStore();
        private readonly object _weightMesStatusLock = new object();
        private readonly Dictionary<string, WeightMesStatusInfo> _weightMesStatus = new Dictionary<string, WeightMesStatusInfo>(StringComparer.OrdinalIgnoreCase);
        private bool _isMesOutboxRetryTaskStarted;

        /// <summary>
        /// Weight工序MES确认状态。
        /// <para>同步过站模式不再创建补传记录，因此需要独立缓存供打印前置校验使用。</para>
        /// </summary>
        private sealed class WeightMesStatusInfo
        {
            public MesOutboxStatus Status { get; set; }
            public string ErrorMessage { get; set; }
            public string FailureSource { get; set; }
            public System.DateTime UpdatedAt { get; set; } = System.DateTime.Now;
        }

        public static readonly Dictionary<string, object> GlobalData = new Dictionary<string, object>(); // 接口配置信息 动态全局变量

        private string[] id = { };                     // 数量
        private string[] stationIdArray = { };         // 工位号
        private string[] testNameArray = { };          // 名称
        private string[] realValuePointArray = { };    // 实际值
        private string[] standardValuePointArray = { };// 标准值
        private string[] maxValuePointArray = { };     // 上限
        private string[] minValuePointArray = { };     // 下限
        private string[] testResultPointArray = { };   // 结果
        private string[] unitNameArray = { };          // 单位

        private AccessHelper curDb;        // 当前数据对象
        private AccessHelper sourceDb;     // 原始数据库对象
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
            WindowState = FormWindowState.Maximized;

            InitializeComponent();

            rtbReadBarCode.SetUiLogLineLimit(ProductionUiLogMaxLines);
            UploadMes.SetUiLogLineLimit(ProductionUiLogMaxLines);
            PrinterSignal.SetUiLogLineLimit(ProductionUiLogMaxLines);

            // 注册统一流程日志的 UI 输出槽：过站流程行同时落 UI 与产品过站文件，逐字一致。
            ProductPassTraceContext.UiSink = line => UploadMes.AppendRaw(line);

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

            // 恢复Weight工序MES确认状态，避免软件重启后打印前置判断只剩空内存。
            LoadRecentWeightMesStatusCache();

            // 读取生产信息
            //GetProduction_Info();

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

            // 加载串口
            TorqueSerialClient.AutoRefreshComboBoxes(cmbCOM1, cmbCOM2);

            // 初始化PLC地址维护界面
            LoadPlcAddress();

            // 初始状态为待机
            lblProductResult.Text = Res.standby;
            lblProductResult.ForeColor = Color.Black;

            System.Windows.Forms.Timer statusTimer = new System.Windows.Forms.Timer();
            statusTimer.Interval = 500;
            statusTimer.Tick += UiUpdateTimer_Tick;
            statusTimer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 取消任务
            permanentTaskCts.Cancel();

            _plcManager?.Close();

            // 关闭数据库连接
            curDb.CloseConnection();
            sourceDb.CloseConnection();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Environment.Exit(0);
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

                tabPageResult1.Text = nameof(ProcessName.Scan_ASSY);
                tabPageResult2.Text = nameof(ProcessName.Weight);
                tabPageResult3.Text = nameof(ProcessName.Screw_BA);

                switch (cboBanUpload.Text)
                {
                    case "None":
                        CreateHeaderText(dgvResult1, ((int)ProcessName.Scan_ASSY).ToString(), true);
                        CreateHeaderText(dgvResult2, ((int)ProcessName.Weight).ToString(), true);
                        CreateHeaderText(dgvResult3, ((int)ProcessName.Screw_BA).ToString(), true);
                        break;
                    case "All":
                        tabPageResult1.Parent = null;
                        tabPageResult2.Parent = null;
                        tabPageResult3.Parent = null;
                        break;
                    case "Scan_ASSY":
                        tabPageResult1.Parent = null;
                        CreateHeaderText(dgvResult2, ((int)ProcessName.Weight).ToString(), true);
                        CreateHeaderText(dgvResult3, ((int)ProcessName.Screw_BA).ToString(), true);
                        break;
                    case "Weight":
                        CreateHeaderText(dgvResult1, ((int)ProcessName.Scan_ASSY).ToString(), true);
                        tabPageResult2.Parent = null;
                        CreateHeaderText(dgvResult3, ((int)ProcessName.Screw_BA).ToString(), true);
                        break;
                    case "Screw_BA":
                        CreateHeaderText(dgvResult1, ((int)ProcessName.Scan_ASSY).ToString(), true);
                        CreateHeaderText(dgvResult2, ((int)ProcessName.Weight).ToString(), true);
                        tabPageResult3.Parent = null;
                        break;
                }
            }
            else
            {
                tabControl_UploadData.SizeMode = TabSizeMode.Fixed;
                tabControl_UploadData.ItemSize = new Size(0, 1);

                tabPageResult2.Parent = null;
                tabPageResult3.Parent = null;
                tabPage8.Parent = null;     // 移除装配机设置界面
                tabPageTorqueMonitor.Parent = null;    // 移除扭力监测界面

                CreateHeaderText(dgvResult1);
            }
        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        private void InitializeVariables()
        {
            _request = new RequestMes();

            sourceDb = new AccessHelper(Global.Instance.SourceDataBase);
            DataTable database = sourceDb.Find("SELECT database_name FROM SystemDataBase where id=1");
            Global.Instance.DataBase = database.Rows[0]["database_name"].ToString();
            curDb = new AccessHelper(Global.Instance.DataBase);

            assembly = Assembly.GetExecutingAssembly();
            resources = new ResourceManager("MesDatas.Language_Resources.language_Chinese", assembly);

            plcAddress = new PLCAdress();

            PlcAddressServer.InitTable();
            addrInfo = PlcAddressServer.GetPlcAddressInfo(1);

            SystemInfoServer.InitTable();
            systemInfo = SystemInfoServer.GetSystemInfo(1);

            _plcManager = new PlcConnectionManager(addrInfo);
            _plcManager.OnConnectionStatusChanged += (status, errorMsg) =>
            {
                Invoke((Action)(() =>
                {
                    _readWriteNet = _plcManager.ReadWriteNet;

                    isPlcConnected = status;

                    if (!status)
                    {
                        rtbErrorLog.AppendToComponent(errorMsg);
                        Log4netHelper.LogDataException("PLC_CONNECTION_CHANGED", errorMsg, new Dictionary<string, object>
                        {
                            { "connected", status }
                        });
                    }

                    PlcSignalLight.ForeColor = isPlcConnected ? Color.Green : Color.Red;
                }));
            };
            // PLC业务心跳只用于连接管理器内部健康判断。
            // 现场确认不需要把心跳异常/恢复刷到异常详情，避免D7107波动干扰操作员判断。

            userInfoDataGridObject = new DataGridViewData(dgvUserInfo, "userinfo", sourceDb);
            errorPreserveDataGridObject = new DataGridViewData(dgvErrorPreserve, "ErrorReferenceTable", curDb);
            keyArgsDataGridObject = new DataGridViewData(dgvKeyArgs, "KeyArgsPreserve", curDb);
            gatherDataGridObject = new DataGridViewData(dgvDataAcquisition, "Board", curDb);
            defectDataGridObject = new DataGridViewData(dgvDeviceDefects, "Defect", curDb);
            printDirectoryObject = new DataGridViewData(dgvPrintDirectory, "PrinterDirectory", curDb);
            changeTypeDataGridObject = new DataGridViewData(dgvProductModel, "ChangeProductType", curDb);
        }

        /// <summary>
        /// 检查用户权限
        /// </summary>
        private void CheckUserPrivilege()
        {
            // 首先移除所有的TabPage
            TabPage[] tabPages = { tabPage1, tabPage2, tabPage3, tabPage4, tabPage5, tabPage6, tabPage7, tabPage8, tabPage9, tabPage9 };
            foreach (TabPage tabPage in tabPages)
            {
                if (!TabContorl.TabPages.Contains(tabPage))
                    TabContorl.TabPages.Add(tabPage);
            }

            switch (Global.Instance.LoginMessage.Privilege)
            {
                case 3:  //作业员
                    //TabContorl1.TabPages.Remove(tabPage2);
                    TabContorl.TabPages.Remove(tabPage3);
                    TabContorl.TabPages.Remove(tabPage4);
                    TabContorl.TabPages.Remove(tabPage5);
                    TabContorl.TabPages.Remove(tabPage6);
                    TabContorl.TabPages.Remove(tabPage7);
                    TabContorl.TabPages.Remove(tabPage8);
                    TabContorl.TabPages.Remove(tabPage9);
                    //TabContorl1.TabPages.Remove(tabPage10);
                    break;
                case 2:  //操作员
                    TabContorl.TabPages.Remove(tabPage3);
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
            DataTable order = curDb.Find(sql);
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
            TokenInputParameter token = new TokenInputParameter
            {
                Key = MesKey.GetPropertySafely(c => c.Text),
                Security = Security.GetPropertySafely(c => c.Text).ToLower()
            };

            return JObject.FromObject(token);
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
                if (row.Cells["权限"].Value.ToString() != "管理员") return true;

                if (sourceDb.Find("select * from userinfo where privilege='管理员'").Rows.Count > 1) return true;

                MessageBox.Show("请至少保留一个管理员账号");

                return false;
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
            errorPreserveDataGridObject.BindEventHandlerButton(btnSave_WarmError, errorPreserveDataGridObject.SaveAllData);

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
            keyArgsDataGridObject.BindEventHandlerButton(btnSave_KeyArgs, keyArgsDataGridObject.SaveAllData);

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
            defectDataGridObject.BindEventHandlerButton(btnSave_dgvDefect, defectDataGridObject.SaveAllData);

            // 初始化数据采集维护信息
            dataMap = new Dictionary<string, string>
            {
                { "id", "id" },
                { "工位号", "WorkID" },
                //{ "型号", "ProductModel" },
                { "是否启用", "IsActive" },
                { "名称", "BoardName" },
                { "实际值", "BoardCode" },
                { "缺陷类型", "StandardCode" },
                { "上限", "MaxBoardCode" },
                { "下限", "MinBoardCode" },
                { "结果", "ResultBoardCode" },
                { "单位", "BoardA1" },
            };

            gatherDataGridObject.BindDataToDataGridView(dataMap);
            gatherDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", gatherDataGridObject.SaveButton_Click);
            gatherDataGridObject.AddOperatorColumnsButton("操作2", "Delete", "删除", gatherDataGridObject.DeleteButton_Click);
            gatherDataGridObject.BindEventHandlerButton(dataGatherBoardRefreshButton, gatherDataGridObject.RefreshButton);
            gatherDataGridObject.BindEventHandlerButton(btnSave_dgvDataAcquisition, gatherDataGridObject.SaveAllData);

            // 初始化打印目录维护信息
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

            // 初始化型号维护信息
            dataMap = new Dictionary<string, string>
            {
                {"id","id" },
                {"机器型号", "product_type" },
                {"匹配SN码前缀", "barcode_match" }
            };
            changeTypeDataGridObject.BindDataToDataGridView(dataMap);
            changeTypeDataGridObject.AddOperatorColumnsButton("操作1", "Save", "保存", changeTypeDataGridObject.SaveButton_Click, row =>
            {
                string productType = row.Cells["机器型号"].Value.ToString();
                string barcodeMatch = row.Cells["匹配SN码前缀"].Value.ToString();
                return !(string.IsNullOrEmpty(productType) || string.IsNullOrEmpty(barcodeMatch));
            });
            changeTypeDataGridObject.AddOperatorColumnsButton("操作2", "Delete", "删除", changeTypeDataGridObject.DeleteButton_Click);
            changeTypeDataGridObject.BindEventHandlerButton(changeTypeRefresh, changeTypeDataGridObject.RefreshButton);
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
            // 读取条码
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadBarcode()));

            // 读取工单号等生产数据
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadValue()));     // Non_Assembly
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadValue1()));    // 装配机工序1
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadValue2()));    // 装配机工序2
            _allDynamicTaskList.Add(Task.Run(() => ProcessPlc_ReadValue3()));    // 装配机工序3

            // 启用关键参数上传
            _allDynamicTaskList.Add(Task.Run(() => CallKeyArgsInterface(token)));

            // 实时读取设备运行参数
            _allDynamicTaskList.Add(Task.Run(() => ReadDeviceArgsRealtime(token)));

            // 实时判断型号是否变更，输入工单
            _allDynamicTaskList.Add(Task.Run(() => MonitorModelSwitchFromPlc(token)));

            // 监听预警数据
            _allDynamicTaskList.Add(Task.Run(async () => await CallDeviceErrorUpload(token)));

            // 启用实时参数上传
            _allDynamicTaskList.Add(Task.Run(() => CallRealtimeArgsInterface(token)));

            // 初始化扭力采集
            _allDynamicTaskList.Add(Task.Run(() => InitTorqueSystem()));

            // 扭力仪数据采集
            _allDynamicTaskList.Add(Task.Run(() => InitSerialTorqueSystem()));

#if UseCodesoft
            // 启用打印模板标签
            _allDynamicTaskList.Add(Task.Run(() => CallPrintBarCode()));

            // 获取多图片
            //_allDynamicTaskList.Add(Task.Run(() => MovePictureGroup()));
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
            var ip = PlcIP.GetPropertySafely(c => c.Text);
            var port = PlcPort.GetPropertySafely(c => int.Parse(c.Text));
            var type = PlcConnectType.GetPropertySafely(c => c.Text);

            // 启用接口心跳
            Task.Factory.StartNew(async () => await InterfaceHeatBeat(), TaskCreationOptions.LongRunning);

            // PLC连接与心跳管理
            Task.Factory.StartNew(async () => await _plcManager.StartConnectionTaskAsync(ip, port, type, permanentTaskCts.Token), TaskCreationOptions.LongRunning);

            // 读取复位信号
            Task.Factory.StartNew(async () => await Recovery(), TaskCreationOptions.LongRunning);

            // 实时上传设备状态
            Task.Factory.StartNew(async () => await DeviceStatusUpload(DeviceStatusSignalLight, DeviceStatusDisplay, permanentTaskCts.Token), TaskCreationOptions.LongRunning);

            // 先反馈再上传后台记录，保证该模式下 MES 请求可继续重试确认。
            StartMesOutboxRetryTask(permanentTaskCts.Token);
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

                string ip = PlcIP.GetPropertySafely(c => c.Text);
                int port = int.Parse(PlcPort.GetPropertySafely(c => c.Text));
                string connectMethod = PlcConnectType.GetPropertySafely(c => c.Text);

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
                    isPlcConnected = _plcManager.IsConnected;

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

                isDeviceAlive = heartBeat != null;

                // 解析用户设定的休眠时间
                if (!int.TryParse(HeartbeatUploadRate.GetPropertySafely(c => c.Text), out int time))
                {
                    DataTable dt = curDb.Find("select heartbeat_rate from ProductConfig where id=1");
                    time = int.Parse(dt.Rows[0]["heartbeat_rate"].ToString());
                }

                await Task.Delay(time * 1000); // 单位：秒
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
                    await Task.Delay(1000);
                    continue;
                }

                await Task.Delay(500);

                //var (isReadOk, value) = await TryReadInt16Async(addrInfo.RecoverySignal);
                //if (!isReadOk || value != 1)
                //    continue;

                var result = await _readWriteNet.ReadInt16Async(addrInfo.RecoverySignal);
                if (!result.IsSuccess || result.Content != 1)
                    continue;

                lblRunningStatus.ExecuteSafely(c => { c.Text = "正在复位中……"; c.ForeColor = Color.Blue; });

                // 开始复位。先写这个地址为0，防止重复读取。
                if (!await TryWriteInt16ValueAsync(addrInfo.RecoverySignal, 0))
                {
                    lblRunningStatus.ExecuteSafely(c => { c.Text = ""; c.ForeColor = Color.Blue; });
                    continue;
                }

                try
                {
                    if (IsHandleCreated && !IsDisposed)
                    {
                        await this.InvokeAsync(() =>
                        {
                            if (btnManualClear.Visible && btnManualClear.Enabled)
                                btnManualClear.PerformClick();
                        });

                        // 等待报警反馈写入结束，再批量复位其他信号，避免同一 PLC 连接并发写入。
                        while (_manualClearInProgress && !permanentTaskCts.IsCancellationRequested)
                            await Task.Delay(50);

                        if (permanentTaskCts.IsCancellationRequested)
                            return;
                    }

                    /*// 获取目录
                    string localPath = LocalFilePath.GetPropertySafely(c => c.Text);
                    string picturePath = Path.Combine(localPath, "PrdSNPictures");  // 里面存放SN码命名的文件夹
                    string txtPath = Path.Combine(localPath, "Txt");                // 里面存放SN码命名的文件
                    // 删除两个目录的文件
                    Resource.ForceDeleteFiles(picturePath, true);                   // 删除里面的文件和文件夹
                    Resource.ForceDeleteFiles(txtPath);*/

                    string orderNum = txtProductModel.GetPropertySafely(c => c.Text);
                    string sql = $"SELECT * FROM PrinterDirectory WHERE order_num='{orderNum}'";
                    DataTable dt = curDb.Find(sql);

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
                        ProductResultList.Clear();
                    }

                    // 重置所有反馈信号
                    if (_readWriteNet != null)
                        await ResetFeedbackSignal();

                    lblRunningStatus.ExecuteSafely(c => { c.Text = "上位机复位完成!"; c.ForeColor = Color.Green; });
                }
                catch (Exception e)
                {
                    HandleError(null, null, false, "上位机复位异常，请排除错误后再启动机器", $"上位机复位异常:{e}");
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
                addrInfo.PrintTrigger,
                addrInfo.PrintFeedback,

                addrInfo.TriggerUpload1,
                addrInfo.ProductResult1,
                addrInfo.Feedback1,

                addrInfo.TriggerUpload2,
                addrInfo.ProductResult2,
                addrInfo.Feedback2,

                addrInfo.TriggerUpload3,
                addrInfo.ProductResult3,
                addrInfo.Feedback3,

                plcAddress.WritePicSignalFirst,
                plcAddress.WritePicSignalSecond,
                plcAddress.WritePicSignalThird,

                addrInfo.HasBarcodeTag,
                addrInfo.BarcodeVerifyTag,
                addrInfo.ManualInputBarcodeTip
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
        public async Task CallKeyArgsInterface(CancellationToken ctsToken)
        {
            if (!EnableReportConfigParam.Checked) return;

            string sql = "SELECT * fROM KeyArgsPreserve";
            DataTable allData = curDb.Find(sql);

            while (ctsToken.IsCancellationRequested)
            {
                // 若断线，保持空转
                if (!isPlcConnected)
                {
                    await Task.Delay(200, dynamicTaskCts.Token);
                    continue;
                }

                if (!ushort.TryParse(addrInfo.ProgramNameLength, out var length))
                {
                    await Task.Delay(200, dynamicTaskCts.Token);
                    continue;
                }

                if (!TryReadStringValue(addrInfo.DeviceProgramName, length, out string tempProgram))
                {
                    await Task.Delay(200, dynamicTaskCts.Token);
                    continue;
                }

                // 检测是否有程序名变更
                if (Program == tempProgram)
                {
                    await Task.Delay(200, dynamicTaskCts.Token);
                    continue;
                }

                // 判断表和数据有没有发生改变，如果改变了需要重查数据库
                if (keyArgsDataGridObject.Changed)
                {
                    allData = curDb.Find(sql);
                    keyArgsDataGridObject.Changed = false;
                }

                // 更新程序名的值
                Program = tempProgram;

                JArray jArray = _GetKeyArgsDataJArray(allData);
                // null则说明读取plc失败
                if (jArray is null)
                {
                    await Task.Delay(200, dynamicTaskCts.Token);
                    continue;
                }

                // 调用接口上传数据
                DeviceKeyArgsInterface(ArgsGetRequestJson(jArray), "设备关键参数", "访问关键参数接口失败");

                await Task.Delay(500, dynamicTaskCts.Token);
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
        private static int DeviceStatus;            // 设备状态

        /// <summary>
        /// 实时读取设备参数
        /// <para>1.生产指标：良品数、不良数、生产总数，并根据读取到的指标计算良率</para>
        /// 2.设备状态：停机、运行、空闲
        /// <para>3.产品型号、条码规则、程序名称</para>
        /// </summary>
        private async Task ReadDeviceArgsRealtime(CancellationToken token)
        {
            int okNum, ngNum, totalNum; double okRate;

            while (!token.IsCancellationRequested)
            {
                // 若断线，保持空转
                if (!isPlcConnected)
                {
                    await Task.Delay(500);
                    continue;
                }

                okNum = _readWriteNet.ReadInt32(addrInfo.GoodsProducts).Content;                                           // 良品数
                ngNum = _readWriteNet.ReadInt32(addrInfo.NotGoodsProducts).Content;                                        // 不良数
                totalNum = _readWriteNet.ReadInt32(addrInfo.ProduceCount).Content;                                         // 生产总数
                okRate = totalNum != 0 ? double.Parse(okNum.ToString()) / double.Parse(totalNum.ToString()) * 100 : 0;           // 良率
                DeviceStatus = _readWriteNet.ReadInt16(addrInfo.DeviceStatus).Content;                                      // 设备状态
                TryReadStringValue(addrInfo.ProductType, addrInfo.ProductTypeLength, out ProductModel);              // 产品型号
                TryReadStringValue(addrInfo.BarcodeRule, addrInfo.BarcodeRuleLength, out barcodeRule);               // 条码规则
                TryReadStringValue(addrInfo.DeviceProgramName, addrInfo.ProgramNameLength, out Program);             // PLC设备使用的程序名称

                if (ProductModel is null || barcodeRule is null || Program is null || !isPlcConnected)
                {
                    // 读取失败时短暂让出线程，避免 PLC 异常期间形成无间隔轮询并占满 CPU。
                    await Task.Delay(200, token);
                    continue;
                }

                Invoke(new Action(() =>
                {
                    txtOkQuality.Text = okNum.ToString();                   // 良品数
                    txtNgQuanlity.Text = ngNum.ToString();                  // 不良数
                    txtTotalQuality.Text = totalNum.ToString();             // 生产总数
                    txtYieldRate.Text = $"{Math.Round(okRate, 2)}%";        // 良率
                    txtProductModel.Text = ProductModel;                    // 产品型号
                }));

                await Task.Delay(500);
            }
        }

        #endregion

        #region 工单切换

        /// <summary>
        /// 实时监测PLC型号切换信号，并弹出工单输入界面，输入完成后通知PLC继续生产
        /// </summary>
        public async Task MonitorModelSwitchFromPlc(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!isPlcConnected || _readWriteNet == null)
                {
                    await Task.Delay(200, token);
                    continue;
                }

                /*var (isReadOk, value) = await TryReadInt16Async(addrInfo.ModelSwitch);
                if (isReadOk && value == 1)
                {
                    ToolingNumber.ExecuteSafely(c => { c.Text = "型号变更请先输入生产信息！"; c.ForeColor = Color.Red; });

                    ManageOrderSwitch();
                }*/

                var result = await _readWriteNet.ReadInt16Async(addrInfo.ModelSwitch);
                if (result.IsSuccess && result.Content == 1)
                {
                    ToolingNumber.ExecuteSafely(c => { c.Text = "型号变更请先输入生产信息！"; c.ForeColor = Color.Red; });

                    ManageOrderSwitch();
                }

                await Task.Delay(200);
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
        private async Task DeviceStatusUpload(Label colorLabel, Label textLabel, CancellationToken token)
        {
            if (!EnableReportMachineStatus.Checked) return;

            string lastType = "UNKNOW";
            System.DateTime lastUploadTime = System.DateTime.Now;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // 获取设备当前状态（设备状态从PLC读取并存储在DeviceStatus字段中）
                    string currentType = (string)Invoke(new Func<string>(() => UpdateDeviceStatus(colorLabel, textLabel)));

                    // --- 校验逻辑 ---
                    // 1. 如果状态是 UNKNOWN，不上传
                    // 2. 如果用户中途取消了勾选，不上传 (continue 而不是 return，允许用户重新勾选恢复)
                    if (currentType.Equals("UNKNOWN")) continue;

                    // --- 触发上传的条件：状态发生改变 OR 超过5分钟未上传 ---
                    bool statusChanged = !currentType.Equals(lastType);
                    double timeSpanMinutes = (System.DateTime.Now - lastUploadTime).TotalMinutes;
                    if (!statusChanged && !(timeSpanMinutes >= 5.0)) continue;

                    JObject uploadData = new JObject
                    {
                        { "Type", currentType },
                        { "LastType", lastType },
                        { "DateTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                        { "Interval", (System.DateTime.Now - lastUploadTime).TotalMilliseconds },
                        { "LastTypeTime", lastUploadTime.ToString("yyyy-MM-dd HH:mm:ss.fff") }
                    };

                    // 执行上传操作
                    DeviceStatusUploadInterface(uploadData, "设备状态", "设备状态变更，数据无法上传至接口");

                    // --- 更新本地状态 ---
                    lastType = currentType;
                    lastUploadTime = System.DateTime.Now;

                    await Task.Delay(1000, token);
                }
                catch (Exception ex)
                {
                    Log4netHelper.LogDataException("DEVICE_STATUS_UPLOAD_ERROR", "设备状态上传线程异常", exception: ex);
                    await Task.Delay(1000, token);
                }
            }
        }

        /// <summary>
        /// 改变设备状态的颜色和label
        /// </summary>
        /// <param name="colorLabel"></param>
        /// <param name="textLabel"></param>
        /// <returns></returns>
        private string UpdateDeviceStatus(Label colorLabel, Label textLabel)
        {
            switch (DeviceStatus)
            {
                case 1: // 运行
                    colorLabel.ExecuteSafely(c => c.ForeColor = Color.Green);
                    textLabel.ExecuteSafely(c => c.Text = "RUN");
                    break;
                case 2: // 空闲
                    colorLabel.ExecuteSafely(c => c.ForeColor = Color.Orange);
                    textLabel.ExecuteSafely(c => c.Text = "IDLE");
                    break;
                case 3: // 停机：通常指系统、机器或设备因故障、维护或升级而处于停止运行、无法工作的时段
                    colorLabel.ExecuteSafely(c => c.ForeColor = Color.Red);
                    textLabel.ExecuteSafely(c => c.Text = "DOWNTIME");
                    break;
                default:
                    colorLabel.ExecuteSafely(c => c.ForeColor = Color.Black);
                    textLabel.ExecuteSafely(c => c.Text = "UNKNOWN");
                    break;
            }

            return textLabel.GetPropertySafely(c => c.Text);
        }

        #endregion

        #region 故障、预警信息上传

        /// <summary>
        /// 异步任务：监控并上传设备故障与预警信息
        /// <para>周期性轮询配置的PLC地址，检测状态变更并上报MES接口</para>
        /// </summary>
        /// <param name="token">取消令牌</param>
        public async Task CallDeviceErrorUpload(CancellationToken token)
        {
            if (!EnableReportMachineAlarm.Checked) return;

            // 1. 初始化配置数据
            string querySql = "SELECT * FROM ErrorReferenceTable";
            DataTable errorConfigTable = curDb.Find(querySql);

            // 提取所有需要监控的PLC地址（Distinct去重）
            var plcAddressList = errorConfigTable.AsEnumerable().Select(row => row.Field<string>("plc_point")).Distinct().ToList();

            // 维护当前活跃的故障队列
            var activeErrorQueue = new List<ErrorWaringEntity>();

            while (!token.IsCancellationRequested)
            {
                // 断线保护
                if (!isPlcConnected)
                {
                    await Task.Delay(500, token);
                    continue;
                }

                // 2. 动态重载配置（如果用户修改了表格）
                if (errorPreserveDataGridObject.Changed)
                {
                    errorConfigTable = curDb.Find(querySql);
                    plcAddressList = errorConfigTable.AsEnumerable().Select(row => row.Field<string>("plc_point")).Distinct().ToList();
                    errorPreserveDataGridObject.Changed = false;
                }

                // 3. 遍历所有点位进行检测
                foreach (var address in plcAddressList)
                {
                    // 读取PLC状态：0=正常, 1=预警, 2=故障
                    if (!TryReadInt16Value(address, out int plcValue) || plcValue == -1)
                    {
                        await Task.Delay(200, token); // 读取失败稍作等待
                        continue;
                    }

                    // 核心逻辑：判断是否需要上传，并构建JSON数据
                    var (actionResult, uploadJson, currentEntity) = _CheckAndBuildUploadData(address, plcValue, errorConfigTable, activeErrorQueue);

                    switch (actionResult)
                    {
                        // -1: 数据库错误
                        case ActionType.ACTION_ERROR:
                            rtbErrorLog.AppendToComponent("[{address}] 生成故障记录失败，无法获取ErrorID");
                            break;

                        // 1: 需要上传
                        case ActionType.ACTION_UPLOAD:
                            // 调用MES接口上传
                            DeviceErrorReturnParam mesResult = DeviceErrorMessageUploadInterface(uploadJson, "设备预警信息", "预警接口上传失败");

                            if (mesResult == null || mesResult.Result.Equals(nameof(MyEnum.Result.FAIL), StringComparison.OrdinalIgnoreCase))
                            {
                                // 上传失败仅记录日志，不更新队列（等待下一次轮询重试）
                                string msg = mesResult == null ? "[预警接口上传失败] 接口返回null" : mesResult.ErrorMessage;
                                rtbErrorLog.AppendToComponent(msg);

                                // 上传成功：更新本地内存队列状态
                                //_UpdateErrorQueue(activeErrorQueue, currentEntity, uploadJson["ErrorType"]?.ToString());
                            }
                            else
                            {
                                // 上传成功：更新本地内存队列状态
                                _UpdateErrorQueue(activeErrorQueue, currentEntity, uploadJson["ErrorType"]?.ToString());
                            }
                            break;

                        default:
                            continue;
                    }
                }

                // 降低轮询频率，避免占用过多CPU
                await Task.Delay(500, token);
            }
        }

        /// <summary>
        /// 核心逻辑：分析PLC状态，判断是否需要上传，并组装上传数据
        /// </summary>
        /// <param name="plcAddress">PLC地址</param>
        /// <param name="plcValue">PLC当前值 (0:正常, 1:预警, 2:故障)</param>
        /// <param name="configTable">故障配置表</param>
        /// <param name="activeQueue">当前活跃的故障队列</param>
        /// <param name="json">输出：构建好的上传JSON</param>
        /// <param name="entity">输出：当前处理的故障实体</param>
        /// <returns>返回操作指令：1(上传), 0(忽略), -1(数据库错误)</returns>
        private (ActionType, JObject, ErrorWaringEntity) _CheckAndBuildUploadData(string plcAddress, int plcValue, DataTable configTable, List<ErrorWaringEntity> activeQueue)
        {
            // 初始化输出参数
            var json = new JObject();
            var entity = new ErrorWaringEntity();

            // ----------- 场景A：PLC恢复正常 (0) -----------
            // 逻辑：如果队列中存在该地址的故障记录，则说明是“清除”动作
            if (plcValue == 0)
            {
                // 查找队列中该地址对应的活跃故障
                var existingError = activeQueue.FirstOrDefault(x => x.plcAddress == plcAddress);

                if (existingError != null)
                {
                    // 找到了之前的故障，准备“清除”
                    DataRow[] rows = configTable.Select($"plc_point = '{plcAddress}'");
                    if (rows.Length > 0)
                    {
                        json["DataType"] = existingError.dataType.ToString();
                        json["ErrorCode"] = (string)rows[0]["error_code"];
                        json["ErrorMessage"] = (string)rows[0]["error_Name"];
                        json["ErrorType"] = nameof(ErrorType.Clear);
                        json["ErrorID"] = existingError.errorId;

                        // 构建实体用于后续队列移除比对
                        return (ActionType.ACTION_UPLOAD, json, existingError);
                    }
                }

                // 如果队列里没有，说明本身就是正常的，忽略
                return (ActionType.ACTION_IGNORE, null, null);
            }

            // ----------- 场景B：PLC触发报警 (1或2) -----------
            // 逻辑：如果队列中没有该记录，或者是状态变更，则说明是“发生”动作
            else if (plcValue == 1 || plcValue == 2)
            {
                // Alert：预警；Alarm：故障；
                DataType curDataType = plcValue == 1 ? DataType.Alert : DataType.Alarm;

                // 检查队列是否已存在完全相同的记录
                // 注意：这里同时比较了地址和类型。如果类型变了(如预警变故障)，也视为新事件。
                var matchEntity = new ErrorWaringEntity { plcAddress = plcAddress, dataType = curDataType };
                if (activeQueue.Contains(matchEntity))
                {
                    return (ActionType.ACTION_IGNORE, null, null); // 已存在且状态未变，忽略
                }

                // 是新发生的故障
                DataRow[] rows = configTable.Select($"plc_point = '{plcAddress}'");
                if (rows.Length > 0)
                {
                    // 生成唯一ID并记录到本地数据库
                    string newId = _GenerateErrorRecordAndGetId(rows[0]);
                    if (string.IsNullOrEmpty(newId))
                    {
                        return (ActionType.ACTION_ERROR, json, entity); // ID生成失败
                    }

                    json["DataType"] = curDataType.ToString();
                    json["ErrorType"] = nameof(ErrorType.Occur);
                    json["ErrorID"] = newId;
                    json["ErrorCode"] = rows[0]["error_code"].ToString();
                    json["ErrorMessage"] = rows[0]["error_Name"].ToString();

                    // 构建实体用于后续加入队列
                    entity = new ErrorWaringEntity
                    {
                        plcAddress = plcAddress,
                        dataType = curDataType,
                        errorId = newId
                    };

                    return (ActionType.ACTION_UPLOAD, json, entity);
                }
            }

            return (ActionType.ACTION_IGNORE, null, null);
        }

        /// <summary>
        /// 辅助方法：更新内存队列状态
        /// </summary>
        private void _UpdateErrorQueue(List<ErrorWaringEntity> queue, ErrorWaringEntity entity, string errorType)
        {
            if (errorType == "Clear")
            {
                // 清除：从队列移除
                // 注意：这里依赖 ErrorWaringEntity 重写的 Equals 方法
                queue.Remove(entity);
            }
            else if (errorType == "Occur")
            {
                // 发生：加入队列
                // 先移除该地址可能存在的旧状态（例如从Alert变成了Alarm，先清掉Alert）
                queue.RemoveAll(x => x.plcAddress == entity.plcAddress);
                queue.Add(entity);
            }
        }

        /// <summary>
        /// 在本地数据库创建故障记录，并返回生成的唯一GUID
        /// </summary>
        /// <param name="configRow">故障配置行数据</param>
        /// <returns>成功返回GUID字符串，失败返回空字符串</returns>
        private string _GenerateErrorRecordAndGetId(DataRow configRow)
        {
            try
            {
                int refId = Convert.ToInt32(configRow["id"]);
                string uniqueKey = Guid.NewGuid().ToString();

                string sql = $"INSERT INTO ErrorMessage(ref_id, unique_key) VALUES({refId}, '{uniqueKey}')";

                if (curDb.Add(sql))
                {
                    return uniqueKey;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

        #region 实时参数上传

        /// <summary>
        /// 调用实时参数接口
        /// </summary>
        /// <returns></returns>
        private async Task CallRealtimeArgsInterface(CancellationToken token)
        {
            if (!EnableReportRealTimeParam.Checked) return;

            int uploadRate = int.Parse(RealtimeArgsUploadRate.Text.Trim());

            while (!token.IsCancellationRequested)
            {
                if (!isPlcConnected)
                {
                    await Task.Delay(200, token);
                    continue;
                }

                // 按照用户的设定时间定时上传
                await Task.Delay(uploadRate * 1000);

                JArray jArray = _GetRealtimeArgsDataLst(boardTable);
                // null则说明读取plc失败
                if (jArray is null)
                {
                    await Task.Delay(200, token);
                    continue;
                }

                // 调用接口上传数据
                DeviceRealtimeArgsInterface(ArgsGetRequestJson(jArray), "设备程序实时参数", "访问设备程序实时参数接口失败");

                await Task.Delay(200, token);
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

        // 定义 Codesoft 对象（作用域提升至整个类，预加载以避免打印延迟）
        private ApplicationClass csApp;
        private Document doc;
        private readonly object _printerLock = new object();

#if UseCodesoft

        /// <summary>
        /// 条码打印线程（优化版：开机即预热 + 防重复打印）
        /// </summary>
        public void CallPrintBarCode()
        {
            if (!EnablePrintCode.Checked && Global.Instance.CurDataBaseName != "装配机") return;

            try
            {
                // ==================== 0. 打印引擎预加载 ====================
                try
                {
                    PrinterSignal.AppendToComponent("正在后台预加载打印引擎……");
                    Log4netHelper.LogLabelPrint("ENGINE_PRELOAD_START", "正在后台预加载打印引擎");

                    // 初始化 Codesoft
                    csApp = new LabelManager2.ApplicationClass { Visible = false  /*防止弹窗干扰*/ };

                    // 获取路径配置
                    string fileName = printTemplatePath.GetPropertySafely(c => c.Text);
                    string printer = printerName.GetPropertySafely(c => c.Text);

                    if (File.Exists(fileName))
                    {
                        csApp.Documents.Open(fileName, true);
                        doc = csApp.ActiveDocument;

                        if (!string.IsNullOrEmpty(printer))
                        {
                            doc.Printer.SwitchTo(printer);
                        }

                        PrinterSignal.AppendToComponent("打印引擎预加载完成，随时待命！");
                        Log4netHelper.LogLabelPrint("ENGINE_PRELOAD_DONE", "打印引擎预加载完成", new Dictionary<string, object>
                        {
                            { "template", fileName },
                            { "printer", printer }
                        });
                    }
                    else
                    {
                        rtbErrorLog.AppendToComponent("预加载跳过：模板文件未找到");
                        Log4netHelper.LogLabelPrint("ENGINE_PRELOAD_TEMPLATE_MISSING", "预加载跳过：模板文件未找到", new Dictionary<string, object>
                        {
                            { "template", fileName }
                        }, level: "WARN");
                    }
                }
                catch (Exception ex)
                {
                    // 预加载失败不应该阻断线程，后面主循环有自愈机制会重试
                    rtbErrorLog.AppendToComponent($"打印引擎预加载异常(将在主循环重试): {ex.Message}");
                    Log4netHelper.LogLabelPrint("ENGINE_PRELOAD_ERROR", "打印引擎预加载异常，主循环将重试", exception: ex, level: "ERROR");
                    // 确保半途而废的对象被清理，防止干扰后续逻辑
                    csApp = null;
                    doc = null;
                }

                // ==================== 1. 等待 PLC 连接 ====================
                while (!isPlcConnected)
                {
                    PrinterSignal.AppendToComponent($"打印机准备就绪，等待PLC连接……");
                    if (DelayAndCheckStop(500)) return; // 这里 return 会触发 finally 里的资源释放
                }

                int failCount = 0;
                const int maxFailCount = 3;

                // 【新增】：用于缓存上一次成功打印的条码，防止重复触发
                string lastPrintedBarcode = string.Empty;

                // ==================== 2. 主业务循环 ====================
                while (true)
                {
                    // 线程停止检测
                    if (DelayAndCheckStop(100)) return;

                    try
                    {
                        // -------------------- 3. 【自愈机制】确保 Codesoft 可用 --------------------

                        // 如果预加载成功，这里会直接跳过；如果预加载失败或中途崩溃，这里会尝试重建
                        if (failCount < maxFailCount && (csApp == null || doc == null))
                        {
                            try
                            {
                                // 销毁旧对象（双重保险）
                                if (csApp != null) try { csApp.Quit(); }
                                    catch
                                    {
                                        // ignored
                                    }

                                csApp = new LabelManager2.ApplicationClass { Visible = false };

                                string filename = printTemplatePath.GetPropertySafely(c => c.Text);

                                if (!File.Exists(filename))
                                {
                                    PrinterSignal.AppendToComponent("模板文件不存在，暂停5秒");
                                    Log4netHelper.LogLabelPrint("TEMPLATE_MISSING", "模板文件不存在，暂停5秒", new Dictionary<string, object>
                                    {
                                        { "template", filename }
                                    }, level: "WARN");
                                    Thread.Sleep(5000);
                                    continue;
                                }

                                csApp.Documents.Open(filename, true);
                                doc = csApp.ActiveDocument;
                                doc.Printer.SwitchTo(printerName.GetPropertySafely(c => c.Text));

                                PrinterSignal.AppendToComponent("打印引擎初始化/恢复成功");
                                Log4netHelper.LogLabelPrint("ENGINE_RECOVERED", "打印引擎初始化/恢复成功", new Dictionary<string, object>
                                {
                                    { "template", filename },
                                    { "printer", printerName.GetPropertySafely(c => c.Text) }
                                });
                                failCount = 0; // 重置失败计数
                            }
                            catch (Exception ex)
                            {
                                failCount++;
                                rtbErrorLog.AppendToComponent($"[重试次数{failCount}]初始化打印机失败: {ex.Message}");
                                Log4netHelper.LogLabelPrint("ENGINE_RECOVER_ERROR", "初始化打印机失败", new Dictionary<string, object>
                                {
                                    { "retry", failCount }
                                }, ex, "ERROR");
                                csApp = null; // 置空以触发下次重试
                                Thread.Sleep(3000);
                                continue;
                            }
                        }

                        // 3.1 重试指定次数后仍失败
                        if (csApp == null || doc == null) return;

                        // -------------------- 4. 读取 PLC 触发信号 --------------------

                        //PrinterSignal.AppendToComponent($"持续监测[{addrInfo.PrintTrigger}]中……");
                        if (!TryReadInt16Value(addrInfo.PrintTrigger, out int triggerValue))
                        {
                            Thread.Sleep(100);
                            HandleError(addrInfo.PrintTrigger, 2, false, $"读取打印触发信号失败({addrInfo.PrintTrigger})，请检查PLC连接");
                            continue;
                        }

                        if (triggerValue != 1)
                        {
                            if (triggerValue == 2)
                            {
                                Log4netHelper.LogLabelPrint("PRINT_CANCEL_BY_STATION", "工位2过站异常，取消本次打印", new Dictionary<string, object>
                                {
                                    { "trigger", addrInfo.PrintTrigger },
                                    { "value", triggerValue },
                                    { "feedback", addrInfo.PrintFeedback }
                                }, level: "WARN");
                                HandleError(addrInfo.PrintFeedback, 2, false, "工位2过站异常，取消本次打印");
                            }

                            // 确保复位
                            TryWriteInt16Value(addrInfo.PrintFeedback, 0);
                            continue;
                        }

                        // ==================== 开始打印业务 ====================

                        // 5. 获取条码
                        if (!TryReadStringValue(addrInfo.BarcodeToPrint, addrInfo.BarcodeToPrintLenght, out string sn2UploadMes4Print))
                            continue;

                        if (!WaitForWeightMesPassBeforePrint(sn2UploadMes4Print, out string weightBlockReason))
                        {
                            WeightMesStatusInfo weightStatusInfo = GetWeightMesStatusInfo(sn2UploadMes4Print);
                            PrinterSignal.AppendToComponent(weightBlockReason);
                            Log4netHelper.LogLabelPrint("PRINT_BLOCKED_BY_WEIGHT", weightBlockReason, new Dictionary<string, object>
                            {
                                { "process", "打印" },
                                { "barcode", sn2UploadMes4Print },
                                { "previousProcess", "Weight" },
                                { "previousStatus", weightStatusInfo?.Status.ToString() ?? "未找到本地Weight MES确认记录" },
                                { "failureSource", weightStatusInfo?.FailureSource ?? "本地" },
                                { "reason", weightStatusInfo?.ErrorMessage ?? "可能该条码未完成Weight或记录产生于轻量缓存上线前" },
                                { "trigger", addrInfo.PrintTrigger },
                                { "feedback", addrInfo.PrintFeedback },
                                { "requiredStatus", WeightMesPassConfirmed }
                            }, level: "WARN");
                            TryWriteInt16Value(addrInfo.PrintFeedback, 2);
                            continue;
                        }

                        Log4netHelper.LogLabelPrint("PRINT_TRIGGER", "收到打印触发", new Dictionary<string, object>
                        {
                            { "barcode", sn2UploadMes4Print },
                            { "trigger", addrInfo.PrintTrigger },
                            { "value", triggerValue }
                        });

                        bool isPrintSuccess = false;
                        string failReason;

                        // ---------------------------------------------------------
                        // 【核心新增】：重复打印拦截逻辑
                        // ---------------------------------------------------------
                        if (sn2UploadMes4Print == lastPrintedBarcode && !string.IsNullOrEmpty(sn2UploadMes4Print))
                        {
                            PrinterSignal.AppendToComponent($"【拦截重复触发】条码 [{sn2UploadMes4Print}] 已打印过，直接放行PLC。");
                            Log4netHelper.LogLabelPrint("PRINT_DUPLICATE_SKIP", "条码已打印过，直接放行PLC", new Dictionary<string, object>
                            {
                                { "barcode", sn2UploadMes4Print }
                            });
                            lblRunningStatus.ExecuteSafely(c => { c.Text = $"重复跳过: {sn2UploadMes4Print}"; c.ForeColor = Color.DarkOrange; });

                            // 伪装成成功，跳过下方物理打印，直接走第9步反馈PLC
                            isPrintSuccess = true;
                        }
                        else
                        {
                            // 只有当条码不同时，才走物理打印与MES请求流程
                            PrinterSignal.AppendToComponent($"收到新打印请求: {sn2UploadMes4Print}");
                            Log4netHelper.LogLabelPrint("PRINT_REQUEST", "收到新打印请求", new Dictionary<string, object>
                            {
                                { "barcode", sn2UploadMes4Print }
                            });
                            lblRunningStatus.ExecuteSafely(c => { c.Text = $"正在处理: {sn2UploadMes4Print}"; c.ForeColor = Color.Blue; });

                            // 6. 调用 MES 接口
                            var json = new JObject
                            {
                                {"PlanNo", OrderNo.GetPropertySafely( c=>c.Text) },
                                {"PrdSN", sn2UploadMes4Print },
                                {"Employee", txtUser.GetPropertySafely( c=>c.Text) }
                            };
                            PrintBarCodeReturnParam barCodeParam = PrintBarCodeInterface(json, "打印接口", "访问打印接口失败");

                            if (barCodeParam != null && barCodeParam.Result.Equals(nameof(MyEnum.Result.PASS), StringComparison.OrdinalIgnoreCase))
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

                                // 8. 物理打印
                                doc.PrintDocument();
                                isPrintSuccess = true;

                                // 【新增】：更新缓存记录，记住这个已成功的条码
                                lastPrintedBarcode = sn2UploadMes4Print;

                                lblRunningStatus.ExecuteSafely(c => { c.Text = $"{sn2UploadMes4Print} 打印完成"; c.ForeColor = Color.Green; });
                                PrinterSignal.AppendToComponent("打印指令已发送");
                                Log4netHelper.LogLabelPrint("PRINT_DONE", "打印指令已发送", new Dictionary<string, object>
                                {
                                    { "barcode", sn2UploadMes4Print },
                                    { "printer", printerName.GetPropertySafely(c => c.Text) }
                                });
                            }
                            else
                            {
                                failReason = barCodeParam == null ? BuildMesFailReason("打印接口返回空") : barCodeParam.ErrorMessage;
                                lblRunningStatus.ExecuteSafely(c => { c.Text = $"打印失败: {failReason}"; c.ForeColor = Color.Red; });
                                PrinterSignal.AppendToComponent($"打印失败: {failReason}");
                                Log4netHelper.LogLabelPrint("PRINT_DATA_FAIL", failReason, new Dictionary<string, object>
                                {
                                    { "barcode", sn2UploadMes4Print }
                                }, level: "ERROR");

                                // 注意：失败时不要更新 lastPrintedBarcode，这样下次PLC重试时还能进来
                            }
                        }

                        // 9. 反馈 PLC (不管是真实成功，还是拦截放行，这里都会写 1)
                        if (!TryWriteInt16Value(addrInfo.PrintFeedback, isPrintSuccess ? 1 : 2))
                        {
                            PrinterSignal.AppendToComponent($"写入打印信号失败({addrInfo.PrintFeedback}={isPrintSuccess}，请检查PLC连接");
                        }
                        Log4netHelper.LogLabelPrint("PRINT_FEEDBACK", "打印结果已反馈PLC", new Dictionary<string, object>
                        {
                            { "feedback", addrInfo.PrintFeedback },
                            { "value", isPrintSuccess ? 1 : 2 },
                            { "success", isPrintSuccess }
                        });

                        // 10. 等待 PLC 复位 (防重复打印)
                        PrinterSignal.AppendToComponent("等待PLC复位信号……");
                        int waitTimeOut = 0;
                        while (true)
                        {
                            if (DelayAndCheckStop(200)) return;

                            if (TryReadInt16Value(addrInfo.PrintTrigger, out int currentDtu))
                            {
                                if (currentDtu == 0)
                                {
                                    TryWriteInt16Value(addrInfo.PrintFeedback, 0);
                                    PrinterSignal.AppendToComponent("流程闭环完成");
                                    Log4netHelper.LogLabelPrint("PRINT_CLOSED", "打印流程闭环完成", new Dictionary<string, object>
                                    {
                                        { "trigger", addrInfo.PrintTrigger },
                                        { "feedback", addrInfo.PrintFeedback }
                                    });
                                    break;
                                }
                            }

                            waitTimeOut++;
                            if (waitTimeOut > 50) // 10秒超时
                            {
                                PrinterSignal.AppendToComponent("警告：PLC 复位信号超时，强制重置");
                                Log4netHelper.LogLabelPrint("PRINT_RESET_TIMEOUT", "PLC复位信号超时，强制重置", new Dictionary<string, object>
                                {
                                    { "trigger", addrInfo.PrintTrigger }
                                }, level: "WARN");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PrinterSignal.AppendToComponent($"打印线程异常: {ex.Message}");
                        Log4netHelper.LogLabelPrint("PRINT_THREAD_ERROR", "打印线程异常", exception: ex, level: "ERROR");
                        csApp = null;
                        doc = null;
                        TryWriteInt16Value(addrInfo.PrintFeedback, 2);
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
                    PrinterSignal.AppendToComponent("打印线程已退出，资源已释放");
                    Log4netHelper.LogLabelPrint("PRINT_THREAD_EXIT", "打印线程已退出，资源已释放");
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
            if (!EnablePrintCode.Checked && Global.Instance.CurDataBaseName != "装配机") return;

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
                            rtbErrorLog.AppendToComponent("无法读取PLC条码");
                            break;
                        }

                        try
                        {
                            MoveFile(prdSN, plcAddr.ReadPlcAddress);
                            TryWriteInt16Value(plcAddr.WritePlcAddress, 1);
                        }
                        catch (Exception e)
                        {
                            Log4netHelper.LogDataException("MOVE_PICTURE_ERROR", "移动图片失败", exception: e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log4netHelper.LogDataException("MOVE_PICTURE_LOOP_ERROR", "移动图片过程中出现错误", exception: e);
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
                rtbReadBarCode.AppendToComponent("等待PLC连接……");

                if (DelayAndCheckStop(500)) return;
            }

            bool isEnableManualInputBarcode = false;         // 需要手动输入条码标志：true按钮已启用，false为未启用，默认为false；

            while (true)
            {
                if (DelayAndCheckStop(100)) return;     // 检测是否应该退出当前任务
                if (existErrorInErrorTip) continue;          // 当前有错误，需要先清除错误后再次访问

                //rtbReadBarCode.AppendToComponent($"持续监测'{addrInfo.HasBarcodeTag}'信号中...");

                int triggerValue = _readWriteNet.ReadInt16(addrInfo.HasBarcodeTag).Content;    // 条码标识（1=触发条码验证）
                var barcodeType = _readWriteNet.ReadInt16(addrInfo.BarcodeType).Content;       // 条码类型 (1=产品条码, 2=工装条码)

                if (triggerValue == 1)
                {
                    // 如果检查到新的扫码成功信号，则自动屏蔽掉手动输入
                    if (isEnableManualInputBarcode)
                    {
                        isEnableManualInputBarcode = false;
                        btnManualInputBarcode.ExecuteSafely(c => c.Visible = false);
                    }

                    try
                    {
                        Log4netHelper.LogRouteCheck("SCAN_TRIGGER", "检测到扫码读码信号", new Dictionary<string, object>
                        {
                            { "address", addrInfo.HasBarcodeTag },
                            { "value", 1 }
                        });

                        // 首先清除触发信号
                        _readWriteNet.Write(addrInfo.HasBarcodeTag, 0);
                        Log4netHelper.LogRouteCheck("SCAN_TRIGGER_CLEAR", "清除扫码读码信号", new Dictionary<string, object>
                        {
                            { "address", addrInfo.HasBarcodeTag },
                            { "value", 0 }
                        });

                        rtbReadBarCode.AppendToComponent($"监测到来自'{addrInfo.HasBarcodeTag}'的信号:{triggerValue}");

                        HandlePlcScanRequest(barcodeType);

                        rtbReadBarCode.AppendToComponent($"来自'{addrInfo.HasBarcodeTag}'的信号处理完成");
                    }
                    catch (Exception e)
                    {
                        HandleError(addrInfo.BarcodeVerifyTag, 2, true, $"扫码读取异常:${e.Message}");
                        rtbReadBarCode.AppendToComponent($"来自'{addrInfo.HasBarcodeTag}'的信号处理异常");
                    }
                }
                else
                {
                    // 更新当前状态，
                    isEnableManualInputBarcode = false;

                    // 手动输入条码信号=1
                    TryReadInt16Value(addrInfo.ManualInputBarcodeTip, out int value);

                    if (value == 1)
                    {
                        //HandleError(null, false, "扫码失败，请手动输入条码");
                        btnManualInputBarcode.ExecuteSafely(c => c.Visible = true);
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
                //if (ShieldBarcode.Checked) return;    // 是否启用读码功能

                // --- 1. 初始化 --- 
                lblRunningStatus.ExecuteSafely(c => { c.Text = "准备读码"; c.ForeColor = Color.Green; });
                lblProductResult.ExecuteSafely(c => { c.Text = "待机"; c.ForeColor = Color.Black; c.BackColor = Color.White; });

                // --- 2. 从PLC读取条码 ---
                ushort barcodeLength = Convert.ToUInt16(addrInfo.PlcScannedBarcodeLength);
                if (!TryReadStringValue(addrInfo.PlcScannedBarcode, barcodeLength, out string scannedBarcode))
                {
                    HandleError(addrInfo.BarcodeVerifyTag, 2, true, "无法读取PLC条码信息，请检查连接");
                    return;
                }

                rtbReadBarCode.AppendToComponent($"读取条码{scannedBarcode}");
                Log4netHelper.LogRouteCheck("BARCODE_READ", "读取到PLC条码", new Dictionary<string, object>
                {
                    { "address", addrInfo.PlcScannedBarcode },
                    { "barcode", scannedBarcode }
                });

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
                barCode.ExecuteSafely(c => c.Text = scannedBarcode);

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
                    if (!TryGetPanelizationBarcodes(ref PrdSNInfo, scannedBarcode)) return;
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
                    _readWriteNet.Write($"{addrInfo.BarcodeVerifyTag}", 1);
                    Log4netHelper.LogRouteCheck("ROUTE_PASS_FEEDBACK", "流程检查成功，通知PLC继续生产", new Dictionary<string, object>
                    {
                        { "barcode", scannedBarcode },
                        { "feedback", addrInfo.BarcodeVerifyTag },
                        { "value", 1 }
                    });
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
            ToolingNumber.ExecuteSafely(c => c.Text = FixtureCode);

            // 反馈读码完成信号给PLC [ 工装编号  ]
            Log4netHelper.LogRouteCheck("TOOLING_BARCODE_FEEDBACK", "工装条码已反馈PLC", new Dictionary<string, object>
            {
                { "barcode", toolingBarcode },
                { "feedback", addrInfo.BarcodeVerifyTag },
                { "value", 1 }
            });
            _readWriteNet.Write(addrInfo.BarcodeVerifyTag, 1);
        }

        /// <summary>
        /// 验证扫描的条码是否与当前选择的产品型号匹配
        /// <para>原理：查询数据库中与当前产品型号相关联的前缀，检查条码是否包含该前缀。</para>
        /// </summary>
        /// <param name="scannedBarcode">扫码枪读取到的完整条码字符串</param>
        /// <returns>验证通过返回 true，失败（包括数据库无记录或不匹配）返回 false</returns>
        private bool VerifyProductModelMatch(string scannedBarcode)
        {
            // 1.获取运行界面中当前生产的产品型号
            string currentModel = txtProductModel.GetPropertySafely(c => c.Text);

            // 2.[参数校验]如果界面没选型号，直接报错
            if (string.IsNullOrEmpty(currentModel))
            {
                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, "未选择产品型号，无法进行校验");
            }

            // 3.查询该型号对应的条码匹配规则
            string sqlQuery = $"select product_type,barcode_match from ChangeProductType where product_type='{currentModel}'";

            // 4. [数据查询] 执行查询，获取结果集 DataTable
            DataTable matchTable = curDb.Find(sqlQuery);

            // 5. [空值检查] 如果数据库没查到该型号的配置，视为校验失败
            if (matchTable == null || matchTable.Rows.Count == 0)
            {
                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, $"数据库中未找到型号[{currentModel}]的条码规则配置");
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
                    lblRunningStatus.ExecuteSafely(c =>
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
            return HandleError(addrInfo.BarcodeVerifyTag, 2, true, $"条码{scannedBarcode}产品型号验证不通过");
        }

        /// <summary>
        /// 处理本地条码规则校验失败的逻辑。
        /// </summary>
        private void HandleBarcodeRuleMismatch()
        {
            lblRunningStatus.ExecuteSafely(c =>
                { c.Text = "验证失败、条码规则验证失败！"; c.ForeColor = Color.Red; });

            HandleError(addrInfo.BarcodeVerifyTag, 2, isBlockingError: true, userMessage: "验证失败、条码规则验证失败！");

            //readWriteNet.Write($"{plcAddress.BarcodeVerifyTag}", 2);
            //LogMsg($"判断条码规则【{plcAddress.BarcodeVerifyTag}】 = 2");
        }

        /// <summary>
        /// (MES 交互 1) 调用MES获取拼板条码。
        /// </summary>
        /// <param name="prdSNs">传入已扫到的子板条码，传出MES返回的完整拼板列表</param>
        /// <param name="scannedBarcode">当前扫码枪读取到的条码</param>
        /// <returns>true 表示成功, false 表示失败 (内部已调用 HandleError)</returns>
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
                Log4netHelper.LogRouteCheck("PANELIZATION_NULL", "连接错误，无法获取拼版条码", new Dictionary<string, object>
                {
                    { "barcode", scannedBarcode },
                    { "result", "NULL" }
                }, level: "ERROR");

                return HandleError(
                    addrInfo.BarcodeVerifyTag, 2, true, "连接错误，无法获取拼版条码");
            }

            // 2. 处理MES返回FAIL
            if (mesResponse.Result.Equals(nameof(MyEnum.Result.FAIL), StringComparison.OrdinalIgnoreCase))
            {
                Log4netHelper.LogRouteCheck("PANELIZATION_FAIL", mesResponse.ErrorMessage, new Dictionary<string, object>
                {
                    { "barcode", scannedBarcode },
                    { "result", mesResponse.Result }
                }, level: "WARN");

                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, $"获取拼版条码错误:{mesResponse.ErrorMessage}");
            }

            // 3. 处理MES返回PASS，但数据不合规（如非拼板）
            if (mesResponse.PrdSNInfo.PrdSNs.Count <= 1)
            {
                Log4netHelper.LogRouteCheck("PANELIZATION_EMPTY", "获取拼版接口验证通过但没返回拼版条码", new Dictionary<string, object>
                {
                    { "barcode", scannedBarcode },
                    { "result", mesResponse.Result }
                }, level: "WARN");

                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, "获取拼版接口验证通过但没返回拼版条码");
            }

            // 4. MES 返回 Pass 且数据合规
            lblRunningStatus.ExecuteSafely(c => { c.Text = "拼版条码获取成功!"; c.ForeColor = Color.Green; });

            // 更新拼板列表
            prdSNs = mesResponse.PrdSNInfo.PrdSNs;
            Log4netHelper.LogRouteCheck("PANELIZATION_PASS", "拼版条码获取成功", new Dictionary<string, object>
            {
                { "barcode", scannedBarcode },
                { "count", prdSNs.Count }
            });
            return true;
        }

        /// <summary>
        /// (MES 交互 2) 调用MES进行流程检查。
        /// </summary>
        /// <param name="snList">子板条码集合，不包含子板序号</param>
        /// <param name="snCollection">子板条码集合，不包含子板序号。这是对snList的封装</param>
        /// <param name="scannedBarcode">从PLC获取的条码</param>
        /// <returns>true 表示成功, false 表示失败 (内部已调用 HandleError)</returns>
        private bool CheckRouteWithMes(ref List<string> snList, ref PrdSNCollection snCollection, string scannedBarcode)
        {
            // ----------- 1.构造MES接口输入参数 -----------

            RouteCheckInputParam inputParam = new RouteCheckInputParam
            {
                Employee = txtUser.Text,
                BoardSide = "",  // OP或者BOTTOM ，非强制可为空
                PlanNo = OrderNo.GetPropertySafely(c => c.Text), //"12025666-40-01",  // OrderNum.Text, //装配机12061441-80-01  原始12033377-40-01
                BoardSideSN = snList[0],
                PrdSNCollection = snCollection
            };

            // ----------- 2.调用MES流程检查接口 -----------

            rtbReadBarCode.AppendToComponent($"开始访问MES流程检查{scannedBarcode}");
            RouteCheckReturnParam mesResponse = _request.GetResponseSerializeResult<RouteCheckReturnParam,
                                                RouteCheckInputParam>(Url_RouteCheck.Text, _httpClient, "CHECKROUTE", inputParam, "流程检查");
            rtbReadBarCode.AppendToComponent($"收到MES流程检查反馈{scannedBarcode}");

            // ----------- 3.处理MES接口返回结果 -----------

            // 3a.接口连接失败
            if (mesResponse == null)
            {
                Log4netHelper.LogRouteCheck("CHECKROUTE_NULL", "访问接口错误，无法进行流程检查", new Dictionary<string, object>
                {
                    { "barcode", scannedBarcode },
                    { "result", "NULL" }
                }, level: "ERROR");

                rtbErrorLog.AppendToComponent("访问接口错误，无法进行流程检查");

                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, "访问接口错误，无法进行流程检查（返回null）");
            }

            // 3b.MES返回FAIL
            if (mesResponse.Result.Equals(nameof(MyEnum.Result.FAIL), StringComparison.OrdinalIgnoreCase))
            {
                Log4netHelper.LogRouteCheck("CHECKROUTE_FAIL", mesResponse.ErrorMessage, new Dictionary<string, object>
                {
                    { "barcode", scannedBarcode },
                    { "result", mesResponse.Result }
                }, level: "WARN");

                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, $"流程检查:{mesResponse.ErrorMessage}");
            }

            // 3c.MES返回PASS
            lblRunningStatus.ExecuteSafely(c => { c.Text = "流程检查成功!"; c.ForeColor = Color.Green; });

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
                Log4netHelper.LogRouteCheck("PANELIZATION_SEND_EMPTY", "流程检查成功，但是查找的拼版结果为空，无法发送到PLC", new Dictionary<string, object>
                {
                    { "barcode", scannedBarcode }
                }, level: "WARN");

                return HandleError(null, 2, false, "流程检查：无法将拼版条码发送给PLC");
            }

            OperateResult result = _readWriteNet.Write(addrInfo.PanalizationBarcode, anotherBarcode);
            Log4netHelper.LogRouteCheck("PANELIZATION_SEND", "拼版条码已发送至PLC", new Dictionary<string, object>
            {
                { "barcode", scannedBarcode },
                { "anotherBarcode", anotherBarcode },
                { "address", addrInfo.PanalizationBarcode },
                { "success", result.IsSuccess }
            });
            return true;
        }

        /// <summary>
        /// 在未勾选流程检查时，直接向PLC反馈OK。
        /// </summary>
        private void BypassRouteCheck(string readPlcSn)
        {
            lblRunningStatus.ExecuteSafely(c => { c.Text = "跳过流程检查成功!"; c.ForeColor = Color.Green; });

            _readWriteNet.Write($"{addrInfo.BarcodeVerifyTag}", 1);

            Log4netHelper.LogRouteCheck("ROUTE_CHECK_BYPASS", "跳过条码验证并反馈PLC", new Dictionary<string, object>
            {
                { "barcode", readPlcSn },
                { "feedback", addrInfo.BarcodeVerifyTag },
                { "value", 1 }
            });
        }

        #endregion

        #region ---------- 生产数据读取、上传（产品过站） ----------

        List<string> scannedBarcodeList = new List<string>();       // 读取的条码
        List<string> ProductResultList = new List<string>();        // 产品生产结果
        private readonly object _barcodeLock = new object();

        /// <summary>
        /// 非装配机产品过站
        /// </summary>
        private void ProcessPlc_ReadValue()
        {
            string process = cboBanUpload.GetPropertySafely(c => c.Text);
            if (process.Equals(nameof(ProcessName.All))) return;
            if (Global.Instance.CurDataBaseName == "装配机") return;

            while (!isPlcConnected)
            {
                UploadMes.AppendToComponent("等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity
            {
                Name = ProcessName.Non_Assembly,
                triggerPoint = addrInfo.TriggerUpload1,
                feedbackPoint = addrInfo.Feedback1,
                ProductResult = addrInfo.ProductResult1,
                BarcodeToUpload = addrInfo.BarcodeToUpload1,
                BarcodeToUploadLength = addrInfo.BarcodeToUploadLength1,
                Line = Line.GetPropertySafely(c => c.Text),
                Process = Process.GetPropertySafely(c => c.Text),
                Staiton = Station.GetPropertySafely(c => c.Text),
                Key = MesKey.GetPropertySafely(c => c.Text),
                Pwd = Security.GetPropertySafely(c => c.Text),
                Device = Device.GetPropertySafely(c => c.Text),
                DeleteFile = false
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                //UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                TryReadInt16Value(uploadManager.triggerPoint, out int triggerValue);
                triggerWatch.Stop();
                if (triggerValue != 1 || !isPlcConnected) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogFlow($"数据准备就绪，{uploadManager.triggerPoint}={triggerValue}");

                    try
                    {
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 触发数据上传信号：{uploadManager.triggerPoint} = {triggerValue}");

                        lock (_barcodeLock)
                        {
                            var prdSN = GetProductResult(uploadManager, scannedBarcodeList, ProductResultList, trace);

                            UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传处理完成：{prdSN}");
                        }

                    }
                    catch (Exception ex)
                    {
                        trace.Diag("UPLOAD_LOOP_ERROR", "数据上传流程异常", ex);
                        HandleError(uploadManager.feedbackPoint, 2, true, $"生产结果读取异常:${ex.Message}");
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传发生异常：{ex}");

                        // 直接用新对象，防止值为null
                        scannedBarcodeList = new List<string>();
                        ProductResultList = new List<string>();
                    }
                }
            }
        }

        /// <summary>
        /// 装配机产品过站1（装配机工序1:Scan_ASSY）
        /// </summary>
        private void ProcessPlc_ReadValue1()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            string processName = cboBanUpload.GetPropertySafely(c => c.Text);
            if (processName.Equals(nameof(ProcessName.Scan_ASSY))) return;
            if (processName.Equals(nameof(ProcessName.All))) return;

            while (!isPlcConnected)
            {
                UploadMes.AppendToComponent("等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity
            {
                Name = ProcessName.Scan_ASSY,
                triggerPoint = addrInfo.TriggerUpload1,
                feedbackPoint = addrInfo.Feedback1,
                ProductResult = addrInfo.ProductResult1,
                BarcodeToUpload = addrInfo.BarcodeToUpload1,
                BarcodeToUploadLength = addrInfo.BarcodeToUploadLength1,
                Line = Line.GetPropertySafely(c => c.Text),
                Process = Process.GetPropertySafely(c => c.Text),
                Staiton = Station.GetPropertySafely(c => c.Text),
                Key = MesKey.GetPropertySafely(c => c.Text),
                Pwd = Security.GetPropertySafely(c => c.Text),
                Device = Device.GetPropertySafely(c => c.Text),
                DeleteFile = true,
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                //UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                triggerWatch.Stop();
                if (triggerValue != 1 || isPlcConnected != true) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogFlow($"数据准备就绪，{uploadManager.triggerPoint}={triggerValue}");

                    try
                    {
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 监听到触发数据上传信号：{uploadManager.triggerPoint} = {triggerValue}");

                        var prdSN = GetProductResult(uploadManager, new List<string>(), new List<string>(), trace);

                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程处理完成：{prdSN}");
                    }
                    catch (Exception ex)
                    {
                        trace.Diag("UPLOAD_LOOP_ERROR", "数据上传流程异常", ex);
                        HandleError(uploadManager.feedbackPoint, 2, true, $"生产结果读取异常:${ex.Message}");
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程发生异常：{ex}");
                    }
                }
            }
        }

        /// <summary>
        /// 装配机产品过站2（装配机工序2:Weight）
        /// </summary>
        private void ProcessPlc_ReadValue2()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            string processName = cboBanUpload.GetPropertySafely(c => c.Text);
            if (processName.Equals(nameof(ProcessName.Weight))) return;
            if (processName.Equals(nameof(ProcessName.All))) return;

            while (!isPlcConnected)
            {
                UploadMes.AppendToComponent("等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity
            {
                Name = ProcessName.Weight,
                triggerPoint = addrInfo.TriggerUpload2,
                feedbackPoint = addrInfo.Feedback2,
                ProductResult = addrInfo.ProductResult2,
                BarcodeToUpload = addrInfo.BarcodeToUpload2,
                BarcodeToUploadLength = addrInfo.BarcodeToUploadLength2,
                Line = Line2.GetPropertySafely(c => c.Text),
                Process = Process2.GetPropertySafely(c => c.Text),
                Staiton = Station.GetPropertySafely(c => c.Text),
                Key = MesKey2.GetPropertySafely(c => c.Text),
                Pwd = Security2.GetPropertySafely(c => c.Text),
                Device = Device2.GetPropertySafely(c => c.Text),
                DeleteFile = true,
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                //UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                triggerWatch.Stop();
                if (triggerValue != 1 || isPlcConnected != true) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogFlow($"数据准备就绪，{uploadManager.triggerPoint}={triggerValue}");

                    try
                    {
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 监听到触发数据上传信号：{uploadManager.triggerPoint} = {triggerValue}");

                        var prdSN = GetProductResult(uploadManager, new List<string>(), new List<string>(), trace);

                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程处理完成：{prdSN}");
                    }
                    catch (Exception ex)
                    {
                        trace.Diag("UPLOAD_LOOP_ERROR", "数据上传流程异常", ex);
                        HandleError(uploadManager.feedbackPoint, 2, true, $"生产结果读取异常:${ex.Message}");
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程发生异常：{ex}");
                    }
                }
            }
        }

        /// <summary>
        /// 装配机产品过站3（装配机工序3:Screw_BA）
        /// </summary>
        private void ProcessPlc_ReadValue3()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            string processName = cboBanUpload.GetPropertySafely(c => c.Text);
            if (processName.Equals(nameof(ProcessName.Screw_BA))) return;
            if (processName.Equals(nameof(ProcessName.All))) return;

            while (!isPlcConnected)
            {
                UploadMes.AppendToComponent("等待PLC连接……");

                if (DelayAndCheckStop(500))
                    return;
            }

            var uploadManager = new UploadManagerEntity
            {
                Name = ProcessName.Screw_BA,
                triggerPoint = addrInfo.TriggerUpload3,
                feedbackPoint = addrInfo.Feedback3,
                ProductResult = addrInfo.ProductResult3,
                BarcodeToUpload = addrInfo.BarcodeToUpload3,
                BarcodeToUploadLength = addrInfo.BarcodeToUploadLength3,
                Line = Line3.GetPropertySafely(c => c.Text),
                Process = Process3.GetPropertySafely(c => c.Text),
                Staiton = Station3.GetPropertySafely(c => c.Text),
                Key = MesKey3.GetPropertySafely(c => c.Text),
                Pwd = Security3.GetPropertySafely(c => c.Text),
                Device = Device3.GetPropertySafely(c => c.Text),
                DeleteFile = false,
            };

            while (true)
            {
                if (DelayAndCheckStop(200)) return;
                if (existErrorInErrorTip) continue;

                //UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                triggerWatch.Stop();
                if (triggerValue != 1 || isPlcConnected != true) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogFlow($"数据准备就绪，{uploadManager.triggerPoint}={triggerValue}");

                    try
                    {
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 监听到触发数据上传信号：{uploadManager.triggerPoint} = {triggerValue}");

                        var prdSN = GetProductResult(uploadManager, new List<string>(), new List<string>(), trace);

                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程处理完成：{prdSN}");
                    }
                    catch (Exception ex)
                    {
                        trace.Diag("UPLOAD_LOOP_ERROR", "数据上传流程异常", ex);
                        HandleError(uploadManager.feedbackPoint, 2, true, $"生产结果读取异常:${ex.Message}");
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程发生异常：{ex}");
                    }
                }
            }
        }

        /// <summary>
        /// 获取生产结果
        /// </summary>
        private string GetProductResult(UploadManagerEntity uploadEntity, List<string> scannedBarcodeList, List<string> productResultList, ProductPassTraceContext trace = null)
        {
            string prdSN = "null";
            string traceResult = "未完成";
            lblRunningStatus.ExecuteSafely(c => { c.Text = "产品开始过站"; c.ForeColor = Color.Green; });

            try
            {
                #region --------- 准备当前需要上传的条码和测试结果 ----------

                Stopwatch productInfoWatch = Stopwatch.StartNew();

                // 上工装机程序没有测试项，直接上传条码和OK结果
                // 如果条码被存储就说明流程检查通过，直接给测试结果赋值OK信号
                if (EnableUpperTooling.Checked)
                {
                    for (int i = 0; i < scannedBarcodeList.Count; i++)
                    {
                        productResultList.Add("3");
                        UploadMes.AppendToComponent($"条码{i + 1}：{scannedBarcodeList[i]}");
                    }
                }
                else
                {
                    // 读取产品状态/结果
                    if (!TryReadInt16Value(uploadEntity.ProductResult, out int ProductResult))
                    {
                        var log = $"[{uploadEntity.Name}] 产品结果读取失败({uploadEntity.ProductResult})，请检查PLC连接";
                        trace?.LogFlowFailure("读取产品信息", $"产品结果读取失败({uploadEntity.ProductResult})，请检查PLC连接");
                        traceResult = "产品结果读取失败";
                        HandleError(uploadEntity.feedbackPoint, 2, true, userMessage: log);
                        UploadMes.AppendToComponent(log);
                        return prdSN;
                    }
                    productResultList.Add(ProductResult.ToString());

                    // 获取过站所需的条码
                    if (!TryReadStringValue(uploadEntity.BarcodeToUpload, uploadEntity.BarcodeToUploadLength, out prdSN))
                    {
                        var log = $"[{uploadEntity.Name}] 产品条码读取失败({uploadEntity.BarcodeToUpload})，请检查PLC连接";
                        trace?.LogFlowFailure("读取产品信息", $"产品条码读取失败({uploadEntity.BarcodeToUpload})，请检查PLC连接");
                        traceResult = "产品条码读取失败";
                        HandleError(uploadEntity.feedbackPoint, 2, true, userMessage: log);
                        UploadMes.AppendToComponent(log);
                        return prdSN;
                    }

                    if (string.IsNullOrWhiteSpace(prdSN))
                    {
                        var log = $"[{uploadEntity.Name}] 获取的条码数据为空";
                        trace?.LogFlowFailure("读取产品信息", "获取的条码数据为空");
                        traceResult = "条码为空";
                        UploadMes.AppendToComponent(log);
                        HandleError(uploadEntity.feedbackPoint, 2, true, log);
                        return prdSN;
                    }

                    UploadMes.AppendToComponent($"[{uploadEntity.Name}] 读取到条码：{prdSN}");
                    scannedBarcodeList.Add(prdSN);

                    // 将读取到的结果写在界面上
                    barCode.ExecuteSafely(c => c.Text = prdSN);

                    // 将图片文件移动到PrdSN命名的文件目录，如果已经被移动了会跳过
                    //MoveFile(prdSN, addrInfo.TriggerUpload3);
                }

                if (scannedBarcodeList.Count == 0)
                {
                    trace?.LogFlowFailure("读取产品信息", "未获取到条码");
                    traceResult = "未获取到条码";
                    // 丢给外层捕捉
                    throw new Exception("未获取到条码");
                }

                string productInfoResult = (EnableUpperTooling.Checked
                        || (productResultList.Count > 0 && productResultList[productResultList.Count - 1] == "3"))
                    ? "OK" : "NG";
                trace?.LogFlowElapsed("读取产品信息", productInfoWatch, $"，SN={prdSN}，Result={productInfoResult}");

                #endregion

                #region ---------- 动态读取测试项数据 ----------

                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 准备读取测试数据");

                var valList = new List<string>();
                var maxList = new List<string>();
                var minList = new List<string>();
                var resList = new List<string>();
                var staList = new List<string>();

                Stopwatch testDataWatch = Stopwatch.StartNew();
                if (!TryReadDataByStation(uploadEntity, out dynamic failReason, ref valList, ref maxList, ref minList, ref resList, ref staList))
                {
                    var log = $"[{uploadEntity.Name}] 读取测试数据异常: {failReason}";
                    trace?.LogFlowFailure("读测试项完成", $"{failReason}");
                    traceResult = "读取测试数据失败";
                    HandleError(uploadEntity.feedbackPoint, 2, true, log);
                    rtbErrorLog.AppendToComponent(log);
                    return prdSN;
                }

                trace?.LogFlowElapsed("读测试项完成", testDataWatch);

                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 测试数据读取完成");

                #endregion

                #region ---------- 上传结果（带重试） ----------

                ReturnParamSendResult returnParam = null;
                bool showResultNow = true; // 同步模式立即显示结果；后台上传模式等MES返回后再显示

                // 离线模式：直接反馈生产完成信号给PLC
                if (!EnableResultUpload.Checked)
                {
                    bool feedbackOk = TryWriteInt16Value(uploadEntity.feedbackPoint, 1);
                    trace?.LogFlowElapsedMs("数据采集完成", trace.TotalElapsedMs, "，D7116=1");
                    Log4netHelper.LogProductPass("OFFLINE_BYPASS", "离线模式未上传MES，已按本地结果反馈PLC", new Dictionary<string, object>
                    {
                        { "process", uploadEntity.Name },
                        { "barcode", prdSN },
                        { "source", "本地" },
                        { "result", feedbackOk ? "PASS" : "PLC反馈失败" },
                        { "feedback", uploadEntity.feedbackPoint }
                    });
                    if (!feedbackOk)
                        trace?.Diag("OFFLINE_FEEDBACK_FAIL", $"离线模式反馈{uploadEntity.feedbackPoint}=1写入失败");
                    traceResult = "Offline";
                }
                else if (IsUploadAfterFeedbackMode())
                {
                    var uploadSnapshot = new ProductUploadSnapshot(
                        uploadEntity,
                        scannedBarcodeList,
                        productResultList,
                        valList,
                        maxList,
                        minList,
                        resList,
                        staList);

                    MesOutboxRecord outboxRecord = CreateMesOutboxRecord(uploadEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList, staList, trace);

                    // 先反馈再上传模式：本地采集已完成，先让PLC继续动作，MES结果只做后台记录。
                    bool feedbackOk = TryWriteInt16Value(uploadEntity.feedbackPoint, 1);

                    if (!feedbackOk)
                    {
                        string log = $"[{uploadEntity.Name}] 采集完成后反馈{uploadEntity.feedbackPoint}=1失败，请检查PLC连接";
                        trace?.LogFlowFailure("数据采集完成", $"反馈{uploadEntity.feedbackPoint}=1失败，请检查PLC连接");
                        MarkOutboxPendingRetry(outboxRecord, "PLC_FEEDBACK_FAIL", log);
                        traceResult = "PLC反馈失败";
                        HandleError(uploadEntity.feedbackPoint, 2, true, log);
                        UploadMes.AppendToComponent(log);
                        return prdSN;
                    }

                    trace?.LogFlowElapsedMs("数据采集完成", trace.TotalElapsedMs, "，D7116=1");

                    _ = StartMesUploadAfterFeedbackAsync(uploadSnapshot, trace, outboxRecord);
                    showResultNow = false;
                    traceResult = "PLC已反馈";
                    UploadMes.AppendToComponent($"[{uploadEntity.Name}] 采集完成，已反馈{uploadEntity.feedbackPoint}=1，MES后台上传中");
                    lblRunningStatus.ExecuteSafely(c => { c.Text = "PLC已反馈，MES后台上传中"; c.ForeColor = Color.Green; });
                }
                else
                {
                    bool isRetry = false;
                    do
                    {
                        isRetry = false;

                        // ============= 开始上传数据 =============

                        UploadMes.AppendToComponent($"[{uploadEntity.Name}] 开始执行数据上传流程 <-");

                        // 上传结果到MES（含本地txt、图片等）；请求构造/发起请求/收到响应三行在 SendResultToMes 内记录
                        returnParam = SendResultToMes(scannedBarcodeList, productResultList, valList, maxList, minList, resList, staList, uploadEntity, trace);

                        UploadMes.AppendToComponent($"[{uploadEntity.Name}] -> 数据上传流程执行结束");

                        // ============= 解析返回来的参数 =============

                        // 判断是否过站成功
                        bool isPass = returnParam != null && returnParam.Result.Equals(nameof(MyEnum.Result.PASS), StringComparison.OrdinalIgnoreCase);

                        if (isPass)
                        {
                            // --- 成功逻辑 ---
                            var feedbackResult = _readWriteNet.Write($"{uploadEntity.feedbackPoint}", 1);

                            if (!feedbackResult.IsSuccess)
                            {
                                var log = $"[{uploadEntity.Name}] MES已PASS，但反馈{uploadEntity.feedbackPoint}=1失败：{feedbackResult.Message}";
                                trace?.LogFlowFailure("数据采集完成", $"反馈{uploadEntity.feedbackPoint}=1失败：{feedbackResult.Message}");
                                traceResult = "D7116写入失败";
                                HandleError(uploadEntity.feedbackPoint, 2, true, log);
                                UploadMes.AppendToComponent(log);
                                return prdSN;
                            }

                            trace?.LogFlowElapsedMs("数据采集完成", trace.TotalElapsedMs, "，D7116=1");
                            traceResult = "PASS";
                            UploadMes.AppendToComponent($"[{uploadEntity.Name}] 过站成功，反馈{uploadEntity.feedbackPoint} = 1");
                            lblRunningStatus.ExecuteSafely(c => { c.Text = "生产结果上传成功"; c.ForeColor = Color.Green; });
                        }
                        else
                        {
                            // --- 失败逻辑：弹出人工选择对话框 ---
                            string errorMsg = returnParam == null ? "接口返回数据异常(Null)" : returnParam.ErrorMessage;
                            trace?.Diag("MES_NOT_PASS", $"MES未PASS，准备弹出人工处理窗口，原因={errorMsg}");

                            // 使用 Invoke 在 UI 线程显示 MessageBox，否则在 Task 中直接 Show 可能不显示或阻塞异常
                            DialogResult dr = (DialogResult)this.Invoke((Func<DialogResult>)(() =>
                            {
                                return MessageBox.Show(
                                    $"工位 [{uploadEntity.Name}] 数据上传失败！\r\n\r\n" +
                                    $"原因: {errorMsg}\r\n\r\n" +
                                    "【Yes/是】：立即重试上传\r\n" +
                                    "【No/否】：触发NG报警逻辑（按设定阻塞或放行）",
                                    "上传异常人工干预",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1); // 默认选中"是"
                            }));

                            if (dr == DialogResult.Yes)
                            {
                                isRetry = true;
                                trace?.Diag("MES_RETRY", "操作员选择立即重试MES上传");
                                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 操作员选择手动重试...");
                                Thread.Sleep(500); // 稍作延时
                                continue; // 跳回 do-while 循环开头，再次执行 SendResultToMes
                            }

                            // --- 用户选择"否"，执行原有的失败/报警逻辑 ---

                            if (returnParam is null)
                            {
                                // null情况直接不处理，在SendResultToMes里面处理过了，但是必须要有这个过程
                                //SendResultAfter(uploadEntity, scannedBarcodeList, productResultList);
                                traceResult = "MES返回NULL";
                                trace?.Diag("MES_NULL", "MES返回NULL，保持原有逻辑返回，不写PASS反馈");
                                return prdSN;
                            }

                            // 产品过站失败，处理打印机逻辑（打印机只能在上一工序过站成功后打印）
                            if (EnablePrintCode.Checked && uploadEntity.Name == ProcessName.Weight)
                            {
                                TryWriteInt16Value(addrInfo.PrintTrigger, 2);
                            }

                            // 根据界面上的设置决定NG显示和阻塞逻辑
                            string operJudge = cboProductMode.GetPropertySafely(c => c.Text);
                            traceResult = "FAIL";
                            trace?.LogFlowElapsedMs("数据采集完成", trace.TotalElapsedMs, "，D7116=2");
                            trace?.Diag("MES_FAIL", $"MES返回FAIL，程序模式={operJudge}，准备反馈{uploadEntity.feedbackPoint}=2");
                            switch (operJudge)
                            {
                                case "不显示NG且阻塞":
                                    HandleError(uploadEntity.feedbackPoint, 2, true, $"[{uploadEntity.Name}] {returnParam.ErrorMessage}");
                                    return prdSN;
                                case "显示NG且阻塞":
                                    HandleError(uploadEntity.feedbackPoint, 2, true, $"[{uploadEntity.Name}] {returnParam.ErrorMessage}");
                                    break;
                                case "显示NG且不阻塞":
                                    HandleError(uploadEntity.feedbackPoint, 2, false, $"[{uploadEntity.Name}] {returnParam.ErrorMessage}");
                                    break;
                            }
                        }

                    } while (isRetry);
                }

                #endregion

                #region ---------- 显示结果 ----------

                if (showResultNow)
                {
                    if (uploadEntity.Name == ProcessName.Scan_ASSY || uploadEntity.Name == ProcessName.Non_Assembly)
                        ShowResult(dgvResult1, returnParam, uploadEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
                    else if (uploadEntity.Name == ProcessName.Weight)
                        ShowResult(dgvResult2, returnParam, uploadEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
                    else if (uploadEntity.Name == ProcessName.Screw_BA)
                        ShowResult(dgvResult3, returnParam, uploadEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
                }

                #endregion

            }
            finally
            {
                trace?.Finish(prdSN, traceResult);
                SendResultAfter(uploadEntity, scannedBarcodeList, productResultList);
            }

            if (traceResult == "PLC已反馈")
                lblRunningStatus.ExecuteSafely(c => { c.Text = "PLC已反馈，MES后台上传中"; c.ForeColor = Color.Green; });
            else
                lblRunningStatus.ExecuteSafely(c => { c.Text = "产品过站成功!"; c.ForeColor = Color.Green; });
            return prdSN;
        }

        /// <summary>
        /// 读取PLC测试数据并添加到对应列表中（已增加工位过滤）
        /// </summary>
        /// <param name="boardTable">数据库Board表数据</param>
        /// <param name="currentProcessId">当前工序号，用于过滤不需要读取的测试项</param>
        /// <param name="value">输出调试用的中间值
        /// <para>成功时：返回读取并计算后的值</para>
        /// <para>失败时：返回错误信息字符串 (string)</para>
        /// <para>PLC地址为空时：返回字符串 "null"</para>
        /// </param>
        /// <param name="valList">实际值列表</param>
        /// <param name="maxList">上限列表</param>
        /// <param name="minList">下限列表</param>
        /// <param name="resList">结果列表</param>
        /// <returns>读取成功返回true，失败返回false，并输出value值。</returns>
        private bool TryReadDataByStation(UploadManagerEntity uploadManager, out dynamic value, ref List<string> valList, ref List<string> maxList, ref List<string> minList, ref List<string> resList, ref List<string> staList)
        {
            value = null; // 初始化 out 参数

            // BUG FIXED 2026-01-20：条码规则未配置测试项时，直接返回成功，避免后续空引用异常
            if (boardTable.Rows.Count == 0) return true;

            // 当前流程的所处工位号
            string currentStationId = ((int)uploadManager.Name).ToString();

            // 判断是否需要条件读取。true=装配机，false=Non_Assembly。
            bool shouldReadConditionally = uploadManager.Name != ProcessName.Non_Assembly;

            for (int i = 0; i < testResultPointArray.Length; i++)
            {
                // 1.获取当前测试项的工位号
                string ta = stationIdArray[i];

                // 2.【工位过滤】若需要条件读取，且当前测试项的工位号与当前流程的工位号不一致，则跳过
                if (shouldReadConditionally && ta != currentStationId) continue;

                // --- 开始读取 PLC 数据 ---

                // A. 读取实际值 (BoardCode)
                string valAddr = realValuePointArray[i];
                if (!TryGetProcessedValue(valAddr, out value)) return false;
                valList.Add($"{value}");

                // B. 读取上限 (MaxBoardCode)
                string maxAddr = maxValuePointArray[i];
                if (string.IsNullOrWhiteSpace(maxAddr))
                    maxList.Add(string.Empty);
                else
                {
                    if (!TryGetProcessedValue(maxAddr, out value)) return false;
                    maxList.Add($"{value}");
                }

                // C. 读取下限 (MinBoardCode)
                string minAddr = minValuePointArray[i];
                if (string.IsNullOrWhiteSpace(minAddr))
                    minList.Add(string.Empty);
                else
                {
                    if (!TryGetProcessedValue(minAddr, out value)) return false;
                    minList.Add($"{value}");
                }

                // D.读取结果(ResultBoardCode)
                string resAddr = testResultPointArray[i];
                if (!TryGetProcessedValue(resAddr, out value)) return false;
                resList.Add($"{value}");

                // E.读取标准值(StandardCode)
                string staAddr = standardValuePointArray[i];
                if (string.IsNullOrWhiteSpace(staAddr))
                    staList.Add(string.Empty);
                else
                {
                    if (!TryGetProcessedValue(staAddr, out value)) return false;
                    staList.Add($"{value}");
                }
            }

            value = true;
            return true;
        }

        /// <summary>
        /// 判断当前是否为“先反馈再上传”模式。
        /// </summary>
        private bool IsUploadAfterFeedbackMode()
        {
            string productMode = cboProductMode.GetPropertySafely(c => c.Text);
            return string.Equals(productMode, ProductModeUploadAfterFeedback, StringComparison.Ordinal);
        }

        /// <summary>
        /// 启动后台 MES 上传任务。
        /// <para>该方法只负责启动任务，不等待 MES 返回，避免阻塞 PLC 放行。</para>
        /// </summary>
        private Task StartMesUploadAfterFeedbackAsync(ProductUploadSnapshot snapshot, ProductPassTraceContext trace, MesOutboxRecord outboxRecord)
        {
            if (snapshot == null || snapshot.UploadEntity == null)
            {
                trace?.Diag("BG_UPLOAD_START_FAIL", "先反馈再上传模式启动失败：上传快照为空");
                return Task.CompletedTask;
            }

            return Task.Run(() => ExecuteMesUploadAfterFeedback(snapshot, trace, outboxRecord));
        }

        /// <summary>
        /// 执行后台 MES 上传。
        /// <para>后台上传失败只记录日志和界面提示，不再写 PLC NG，也不弹出阻塞窗口。</para>
        /// </summary>
        private void ExecuteMesUploadAfterFeedback(ProductUploadSnapshot snapshot, ProductPassTraceContext trace, MesOutboxRecord outboxRecord)
        {
            IDisposable traceScope = null;

            try
            {
                traceScope = trace?.EnterScope();
                UploadManagerEntity uploadEntity = snapshot.UploadEntity;

                UploadMes.AppendToComponent($"[{uploadEntity.Name}] MES后台上传开始");
                trace?.LogFlow("MES后台上传开始");

                Stopwatch mesWatch = Stopwatch.StartNew();
                ReturnParamSendResult returnParam = SendResultToMes(
                    snapshot.ScannedBarcodeList,
                    snapshot.ProductResultList,
                    snapshot.ValueList,
                    snapshot.MaxList,
                    snapshot.MinList,
                    snapshot.ResultList,
                    snapshot.StandardList,
                    uploadEntity,
                    trace,
                    handleMesFailure: false,
                    outboxRecord: outboxRecord);
                mesWatch.Stop();

                bool isPass = returnParam != null && string.Equals(returnParam.Result, nameof(MyEnum.Result.PASS), StringComparison.OrdinalIgnoreCase);
                if (isPass)
                {
                    trace?.LogFlowElapsed("MES后台上传成功", mesWatch);
                    UploadMes.AppendToComponent($"[{uploadEntity.Name}] MES后台上传成功");
                    lblRunningStatus.ExecuteSafely(c => { c.Text = "MES后台上传成功"; c.ForeColor = Color.Green; });
                }
                else
                {
                    string errorMessage = returnParam == null ? "接口返回数据异常(Null)" : returnParam.ErrorMessage;
                    LogMesUploadAfterFeedbackFailure(snapshot, trace, errorMessage);
                }

                ShowUploadSnapshotResult(snapshot, returnParam);
            }
            catch (Exception ex)
            {
                LogMesUploadAfterFeedbackFailure(snapshot, trace, ex.ToString());
            }
            finally
            {
                traceScope?.Dispose();
            }
        }

        /// <summary>
        /// 记录后台 MES 上传失败。
        /// <para>PLC 已经收到成功反馈，因此这里禁止再调用 HandleError 写 NG。</para>
        /// </summary>
        private void LogMesUploadAfterFeedbackFailure(ProductUploadSnapshot snapshot, ProductPassTraceContext trace, string errorMessage)
        {
            string processName = snapshot?.UploadEntity?.Name.ToString() ?? "Unknown";
            string log = $"[{processName}] MES后台上传失败（PLC已放行）：{errorMessage}";

            trace?.LogFlow("MES后台上传失败");
            trace?.Diag("BG_UPLOAD_FAIL", log);
            UploadMes.AppendToComponent(log);
            rtbErrorLog.AppendToComponent(log);
            lblRunningStatus.ExecuteSafely(c => { c.Text = "MES后台上传失败"; c.ForeColor = Color.Red; });
        }

        /// <summary>
        /// 根据工序把后台上传结果显示到对应表格。
        /// </summary>
        private void ShowUploadSnapshotResult(ProductUploadSnapshot snapshot, ReturnParamSendResult returnParam)
        {
            if (snapshot == null || snapshot.UploadEntity == null) return;

            UploadManagerEntity uploadEntity = snapshot.UploadEntity;
            if (uploadEntity.Name == ProcessName.Scan_ASSY || uploadEntity.Name == ProcessName.Non_Assembly)
                ShowResult(dgvResult1, returnParam, uploadEntity, snapshot.ScannedBarcodeList, snapshot.ProductResultList, snapshot.ValueList, snapshot.MaxList, snapshot.MinList, snapshot.ResultList);
            else if (uploadEntity.Name == ProcessName.Weight)
                ShowResult(dgvResult2, returnParam, uploadEntity, snapshot.ScannedBarcodeList, snapshot.ProductResultList, snapshot.ValueList, snapshot.MaxList, snapshot.MinList, snapshot.ResultList);
            else if (uploadEntity.Name == ProcessName.Screw_BA)
                ShowResult(dgvResult3, returnParam, uploadEntity, snapshot.ScannedBarcodeList, snapshot.ProductResultList, snapshot.ValueList, snapshot.MaxList, snapshot.MinList, snapshot.ResultList);
        }

        /// <summary>
        /// 创建先反馈再上传记录。
        /// <para>只有先反馈再上传模式允许创建该记录，普通同步过站不得创建后台上传记录。</para>
        /// </summary>
        private MesOutboxRecord CreateMesOutboxRecord(
            UploadManagerEntity uploadEntity,
            List<string> scannedBarcodeList,
            List<string> productResultList,
            List<string> valList,
            List<string> maxList,
            List<string> minList,
            List<string> resList,
            List<string> staList,
            ProductPassTraceContext trace,
            InputParamSendResult inputParam = null,
            MesOutboxStatus status = MesOutboxStatus.Created)
        {
            if (uploadEntity == null) return null;

            var record = new MesOutboxRecord
            {
                TraceId = trace?.TraceId,
                ProcessName = uploadEntity.Name.ToString(),
                Barcode = scannedBarcodeList?.FirstOrDefault() ?? string.Empty,
                Barcodes = CopyStringList(scannedBarcodeList),
                ProductResults = CopyStringList(productResultList),
                ValueList = CopyStringList(valList),
                MaxList = CopyStringList(maxList),
                MinList = CopyStringList(minList),
                ResultList = CopyStringList(resList),
                StandardList = CopyStringList(staList),
                PayloadJson = inputParam == null ? null : JsonConvert.SerializeObject(inputParam, Formatting.None),
                Status = status,
                ErrorType = status == MesOutboxStatus.OfflineBypass ? "OFFLINE_BYPASS" : null,
                ErrorMessage = status == MesOutboxStatus.OfflineBypass ? "离线模式未上传MES" : null
            };

            MesOutboxRecord savedRecord = _mesOutboxStore.Save(record);
            UpdateWeightMesStatus(savedRecord);
            Log4netHelper.LogProductPass("MES_OUTBOX_CREATE", "先反馈再上传记录已创建，等待MES后台确认", new Dictionary<string, object>
            {
                { "traceId", savedRecord?.TraceId },
                { "process", savedRecord?.ProcessName },
                { "barcode", savedRecord?.Barcode },
                { "status", savedRecord?.Status },
                { "recordId", savedRecord?.RecordId },
                { "source", "本地" }
            });
            return savedRecord;
        }

        /// <summary>
        /// 更新补传记录中的请求payload。
        /// </summary>
        private MesOutboxRecord SaveOutboxPayload(MesOutboxRecord record, InputParamSendResult inputParam)
        {
            if (record == null || inputParam == null) return record;

            record.PayloadJson = JsonConvert.SerializeObject(inputParam, Formatting.None);
            record.Status = MesOutboxStatus.Created;
            return _mesOutboxStore.Save(record);
        }

        /// <summary>
        /// 标记MES已确认PASS。
        /// </summary>
        private void MarkOutboxConfirmedPass(MesOutboxRecord record, ReturnParamSendResult returnParam)
        {
            if (record == null) return;

            MesOutboxRecord savedRecord = _mesOutboxStore.MarkConfirmedPass(record.RecordId, returnParam?.ErrorMessage);
            UpdateWeightMesStatus(savedRecord);
            Log4netHelper.LogProductPass("MES_OUTBOX_CONFIRMED_PASS", "MES后台上传已确认PASS", new Dictionary<string, object>
            {
                { "traceId", savedRecord?.TraceId },
                { "process", savedRecord?.ProcessName },
                { "barcode", savedRecord?.Barcode },
                { "status", savedRecord?.Status },
                { "retryCount", savedRecord?.RetryCount }
            });
        }

        /// <summary>
        /// 标记MES明确FAIL。
        /// </summary>
        private void MarkOutboxConfirmedFail(MesOutboxRecord record, string errorType, string errorMessage)
        {
            if (record == null) return;

            MesOutboxRecord savedRecord = _mesOutboxStore.MarkConfirmedFail(record.RecordId, errorType, errorMessage);
            UpdateWeightMesStatus(savedRecord);
            Log4netHelper.LogProductPass("MES_OUTBOX_CONFIRMED_FAIL", "MES后台上传明确失败", new Dictionary<string, object>
            {
                { "traceId", savedRecord?.TraceId },
                { "process", savedRecord?.ProcessName },
                { "barcode", savedRecord?.Barcode },
                { "status", savedRecord?.Status },
                { "errorType", savedRecord?.ErrorType },
                { "failureType", GetMesFailureType(savedRecord?.ErrorMessage) },
                { "duplicateKey", ExtractMesDuplicateKey(savedRecord?.ErrorMessage) },
                { "errorMessage", savedRecord?.ErrorMessage }
            });
        }

        /// <summary>
        /// 标记为后台上传待重试。
        /// </summary>
        private void MarkOutboxPendingRetry(MesOutboxRecord record, string errorType, string errorMessage)
        {
            if (record == null) return;

            MesOutboxRecord savedRecord = _mesOutboxStore.MarkPendingRetry(record.RecordId, errorType, errorMessage);
            UpdateWeightMesStatus(savedRecord);
            Log4netHelper.LogProductPass("MES_OUTBOX_PENDING_RETRY", "MES后台上传结果未知，等待重试", new Dictionary<string, object>
            {
                { "traceId", savedRecord?.TraceId },
                { "process", savedRecord?.ProcessName },
                { "barcode", savedRecord?.Barcode },
                { "status", savedRecord?.Status },
                { "retryCount", savedRecord?.RetryCount },
                { "errorType", savedRecord?.ErrorType },
                { "errorMessage", savedRecord?.ErrorMessage }
            });
        }

        /// <summary>
        /// 标记为人工处理中。
        /// <para>这类记录没有完整请求payload，程序不能盲目自动补传，必须保留现场可追溯原因。</para>
        /// </summary>
        private void MarkOutboxManualProcessing(MesOutboxRecord record, string errorType, string errorMessage)
        {
            if (record == null) return;

            MesOutboxRecord savedRecord = _mesOutboxStore.MarkManualProcessing(record.RecordId, errorType, errorMessage);
            UpdateWeightMesStatus(savedRecord);
            Log4netHelper.LogProductPass("MES_OUTBOX_MANUAL_PROCESSING", "MES后台上传进入人工处理", new Dictionary<string, object>
            {
                { "traceId", savedRecord?.TraceId },
                { "process", savedRecord?.ProcessName },
                { "barcode", savedRecord?.Barcode },
                { "status", savedRecord?.Status },
                { "errorType", savedRecord?.ErrorType },
                { "errorMessage", savedRecord?.ErrorMessage }
            }, level: "ERROR");
        }

        /// <summary>
        /// 启动MES补传后台线程。
        /// </summary>
        private void StartMesOutboxRetryTask(CancellationToken token)
        {
            if (_isMesOutboxRetryTaskStarted) return;
            _isMesOutboxRetryTaskStarted = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    RetryPendingMesOutboxRecords(token).GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    // 程序关闭或永久任务取消时，这是正常退出路径。
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// 后台循环补传MES过站记录。
        /// </summary>
        private async Task RetryPendingMesOutboxRecords(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    foreach (MesOutboxRecord record in _mesOutboxStore.LoadPendingRetry())
                    {
                        if (token.IsCancellationRequested) break;
                        RetrySingleMesOutboxRecord(record);
                    }
                }
                catch (Exception ex)
                {
                    Log4netHelper.LogDataException("MES_OUTBOX_RETRY_LOOP_ERROR", "MES补传线程异常", exception: ex);
                }

                await Task.Delay(10000, token);
            }
        }

        /// <summary>
        /// 补传单条MES过站记录。
        /// </summary>
        private void RetrySingleMesOutboxRecord(MesOutboxRecord record)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.PayloadJson)) return;

            try
            {
                InputParamSendResult inputParam = JsonConvert.DeserializeObject<InputParamSendResult>(record.PayloadJson);
                ReturnParamSendResult returnParam = _request.GetResponseSerializeResult<ReturnParamSendResult, InputParamSendResult>(
                    Url_DataUpload.Text,
                    _httpClient,
                    "SAVERESULT",
                    inputParam,
                    "MES补传");

                if (IsMesPass(returnParam) || IsDuplicatePassMesResult(returnParam))
                {
                    MarkOutboxConfirmedPass(record, returnParam);
                    UploadMes.AppendToComponent($"[{record.ProcessName}] MES补传成功：{record.Barcode}");
                    return;
                }

                if (returnParam == null)
                {
                    MarkOutboxPendingRetry(record, "RESPONSE_NULL", "MES补传接口返回null或请求超时");
                    return;
                }

                MarkOutboxConfirmedFail(record, GetMesFailureErrorType(returnParam.ErrorMessage), returnParam.ErrorMessage);
            }
            catch (Exception ex)
            {
                MarkOutboxPendingRetry(record, "REQUEST_EXCEPTION", ex.Message);
            }
        }

        /// <summary>
        /// 更新Weight工序MES状态缓存，供打印线程快速判断。
        /// </summary>
        private void UpdateWeightMesStatus(MesOutboxRecord record)
        {
            if (record == null) return;
            if (!string.Equals(record.ProcessName, ProcessName.Weight.ToString(), StringComparison.OrdinalIgnoreCase)) return;

            UpdateWeightMesStatus(
                ProcessName.Weight,
                record.Barcodes,
                record.Status,
                record.ErrorMessage,
                GetMesRecordFailureSource(record));
        }

        /// <summary>
        /// 更新Weight工序MES状态缓存。
        /// <para>普通同步过站模式不创建补传记录，所以必须直接写入该缓存。</para>
        /// </summary>
        private void UpdateWeightMesStatus(ProcessName? processName, IEnumerable<string> barcodes, MesOutboxStatus status, string errorMessage, string failureSource)
        {
            if (processName != ProcessName.Weight) return;
            if (barcodes == null) return;

            System.DateTime updatedAt = System.DateTime.Now;
            var recordsToSave = new List<WeightMesStatusRecord>();

            lock (_weightMesStatusLock)
            {
                foreach (string barcode in barcodes)
                {
                    if (string.IsNullOrWhiteSpace(barcode)) continue;

                    WeightMesStatusInfo statusInfo = CreateWeightMesStatusInfo(status, errorMessage, failureSource, updatedAt);
                    _weightMesStatus[barcode] = statusInfo;
                    recordsToSave.Add(CreateWeightMesStatusRecord(barcode, statusInfo));
                }
            }

            SaveWeightMesStatusRecords(recordsToSave);
        }

        /// <summary>
        /// 启动时恢复最近Weight MES状态，避免软件重启导致打印前置判断丢失。
        /// </summary>
        private void LoadRecentWeightMesStatusCache()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            try
            {
                _weightMesStatusStore.PruneOlderThan(WeightMesStatusCacheRetentionDays);

                List<WeightMesStatusRecord> latestRecords = _weightMesStatusStore.LoadRecent(WeightMesStatusCacheLoadDays)
                    .GroupBy(item => item.Barcode, StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.OrderByDescending(item => item.UpdatedAt).First())
                    .ToList();

                lock (_weightMesStatusLock)
                {
                    foreach (WeightMesStatusRecord record in latestRecords)
                    {
                        if (string.IsNullOrWhiteSpace(record.Barcode)) continue;
                        _weightMesStatus[record.Barcode] = CreateWeightMesStatusInfo(record);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.LogDataException("WEIGHT_MES_STATUS_CACHE_LOAD_FAIL", "Weight MES本地状态缓存加载失败", exception: ex);
                rtbErrorLog.AppendToComponent($"Weight MES本地状态缓存加载失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 保存Weight MES状态到轻量缓存。缓存失败只报警，不影响MES主流程。
        /// </summary>
        private void SaveWeightMesStatusRecords(IEnumerable<WeightMesStatusRecord> records)
        {
            if (records == null) return;

            foreach (WeightMesStatusRecord record in records)
            {
                try
                {
                    _weightMesStatusStore.Save(record);
                }
                catch (Exception ex)
                {
                    Log4netHelper.LogDataException("WEIGHT_MES_STATUS_CACHE_SAVE_FAIL", "Weight MES本地状态缓存写入失败", new Dictionary<string, object>
                    {
                        { "barcode", record?.Barcode },
                        { "status", record?.Status }
                    }, ex);
                    rtbErrorLog.AppendToComponent($"Weight MES本地状态缓存写入失败：{record?.Barcode}，{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 从轻量缓存读取指定条码的Weight MES状态，并回填内存缓存。
        /// </summary>
        private WeightMesStatusInfo FindWeightMesStatusFromLightweightCache(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;

            try
            {
                WeightMesStatusRecord record = _weightMesStatusStore.FindLatestByBarcode(barcode, WeightMesStatusCacheLoadDays);
                if (record == null) return null;

                WeightMesStatusInfo statusInfo = CreateWeightMesStatusInfo(record);
                lock (_weightMesStatusLock)
                {
                    _weightMesStatus[record.Barcode] = statusInfo;
                }

                return statusInfo;
            }
            catch (Exception ex)
            {
                Log4netHelper.LogDataException("WEIGHT_MES_STATUS_CACHE_READ_FAIL", "Weight MES本地状态缓存读取失败", new Dictionary<string, object>
                {
                    { "barcode", barcode }
                }, ex);
                rtbErrorLog.AppendToComponent($"Weight MES本地状态缓存读取失败：{barcode}，{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 根据内存状态创建可持久化的轻量缓存记录。
        /// </summary>
        private static WeightMesStatusRecord CreateWeightMesStatusRecord(string barcode, WeightMesStatusInfo statusInfo)
        {
            if (statusInfo == null) return null;

            return new WeightMesStatusRecord
            {
                Barcode = barcode,
                ProcessName = ProcessName.Weight.ToString(),
                Status = statusInfo.Status,
                ErrorMessage = statusInfo.ErrorMessage,
                FailureSource = statusInfo.FailureSource,
                UpdatedAt = statusInfo.UpdatedAt
            };
        }

        /// <summary>
        /// 创建Weight MES内存状态对象。
        /// </summary>
        private static WeightMesStatusInfo CreateWeightMesStatusInfo(MesOutboxStatus status, string errorMessage, string failureSource, System.DateTime updatedAt)
        {
            return new WeightMesStatusInfo
            {
                Status = status,
                ErrorMessage = errorMessage,
                FailureSource = failureSource,
                UpdatedAt = updatedAt
            };
        }

        /// <summary>
        /// 将轻量缓存记录转换为内存状态对象。
        /// </summary>
        private static WeightMesStatusInfo CreateWeightMesStatusInfo(WeightMesStatusRecord record)
        {
            if (record == null) return null;

            return CreateWeightMesStatusInfo(record.Status, record.ErrorMessage, record.FailureSource, record.UpdatedAt);
        }

        /// <summary>
        /// 复制字符串列表，避免补传记录引用外部可变集合。
        /// </summary>
        private static List<string> CopyStringList(List<string> source)
        {
            return source == null ? new List<string>() : new List<string>(source);
        }

        /// <summary>
        /// 判断MES是否返回PASS。
        /// </summary>
        private static bool IsMesPass(ReturnParamSendResult returnParam)
        {
            return returnParam != null && string.Equals(returnParam.Result, nameof(MyEnum.Result.PASS), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 判断“重复过站/工序已完成”是否可视作先反馈再上传记录已经被MES确认。
        /// </summary>
        private static bool IsDuplicatePassMesResult(ReturnParamSendResult returnParam)
        {
            if (returnParam == null) return false;
            string message = returnParam.ErrorMessage ?? string.Empty;
            if (IsMesPrimaryKeyConflict(message)) return false;
            return message.Contains("已完成") || message.Contains("重复过站") || message.Contains("不允许重复过站");
        }

        /// <summary>
        /// 判断 MES 返回是否为数据库主键冲突。
        /// <para>这类错误说明 MES 数据库写入失败，不能按“重复过站已完成”自动放行。</para>
        /// </summary>
        private static bool IsMesPrimaryKeyConflict(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return false;

            return message.IndexOf("PRIMARY KEY", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   message.IndexOf("PK_rt_PrdSNTrace_MOInput", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   message.Contains("重复键值");
        }

        /// <summary>
        /// 获取 MES 失败类型的现场中文文案。
        /// </summary>
        private static string GetMesFailureType(string message)
        {
            return IsMesPrimaryKeyConflict(message) ? "MES数据库主键冲突" : string.Empty;
        }

        /// <summary>
        /// 获取 MES 失败类型的内部错误码。
        /// </summary>
        private static string GetMesFailureErrorType(string message)
        {
            return IsMesPrimaryKeyConflict(message) ? "MES_PRIMARY_KEY_CONFLICT" : "MES_FAIL";
        }

        /// <summary>
        /// 从 SQL 主键冲突信息中提取重复键，便于现场与 MES 数据库核对。
        /// </summary>
        private static string ExtractMesDuplicateKey(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return string.Empty;

            int markerIndex = message.IndexOf("重复键值", StringComparison.OrdinalIgnoreCase);
            if (markerIndex < 0) return string.Empty;

            int startIndex = message.IndexOf('(', markerIndex);
            int endIndex = startIndex >= 0 ? message.IndexOf(')', startIndex) : -1;
            if (startIndex < 0 || endIndex <= startIndex) return string.Empty;

            return message.Substring(startIndex, endIndex - startIndex + 1);
        }

        /// <summary>
        /// 判断当前条码是否已经完成Weight工序MES过站。
        /// </summary>
        private bool CanPrintAfterWeightMesPass(string barcode)
        {
            if (Global.Instance.CurDataBaseName != "装配机") return true;
            if (string.IsNullOrWhiteSpace(barcode)) return false;

            lock (_weightMesStatusLock)
            {
                if (_weightMesStatus.TryGetValue(barcode, out WeightMesStatusInfo statusInfo))
                    return statusInfo.Status == MesOutboxStatus.ConfirmedPass;
            }

            WeightMesStatusInfo lightweightStatusInfo = FindWeightMesStatusFromLightweightCache(barcode);
            if (lightweightStatusInfo != null)
                return lightweightStatusInfo.Status == MesOutboxStatus.ConfirmedPass;

            MesOutboxRecord record = _mesOutboxStore.FindLatestByBarcodeAndProcess(barcode, ProcessName.Weight.ToString());
            UpdateWeightMesStatus(record);
            return record != null && record.Status == MesOutboxStatus.ConfirmedPass;
        }

        /// <summary>
        /// 打印前等待Weight工序MES过站确认。
        /// <para>如果MES仍未确认，禁止调用标签打印接口，避免标签先打印但MES未过站。</para>
        /// </summary>
        private bool WaitForWeightMesPassBeforePrint(string barcode, out string blockReason)
        {
            blockReason = null;
            if (Global.Instance.CurDataBaseName != "装配机") return true;

            for (int i = 0; i < 120; i++)
            {
                if (CanPrintAfterWeightMesPass(barcode)) return true;
                Thread.Sleep(500);
            }

            WeightMesStatusInfo statusInfo = GetWeightMesStatusInfo(barcode);
            if (statusInfo == null)
            {
                blockReason = $"禁止打印，来源=本地拦截，SN={barcode}，前置工序=Weight，前置状态=未找到本地Weight MES确认记录，失败来源=本地，原因=可能该条码未完成Weight或记录产生于轻量缓存上线前";
                return false;
            }

            string failureSource = string.IsNullOrWhiteSpace(statusInfo.FailureSource) ? "MES" : statusInfo.FailureSource;
            string reason = string.IsNullOrWhiteSpace(statusInfo.ErrorMessage) ? "上一工序Weight未确认MES PASS" : statusInfo.ErrorMessage;
            blockReason = $"禁止打印，来源=本地拦截，SN={barcode}，前置工序=Weight，前置状态={statusInfo.Status}，失败来源={failureSource}，原因={reason}";
            return false;
        }

        /// <summary>
        /// 获取Weight工序MES状态，优先读内存缓存，再读轻量缓存，最后兼容先反馈再上传模式的本地队列记录。
        /// </summary>
        private WeightMesStatusInfo GetWeightMesStatusInfo(string barcode)
        {
            lock (_weightMesStatusLock)
            {
                if (_weightMesStatus.TryGetValue(barcode, out WeightMesStatusInfo statusInfo))
                    return statusInfo;
            }

            WeightMesStatusInfo lightweightStatusInfo = FindWeightMesStatusFromLightweightCache(barcode);
            if (lightweightStatusInfo != null)
                return lightweightStatusInfo;

            MesOutboxRecord record = _mesOutboxStore.FindLatestByBarcodeAndProcess(barcode, ProcessName.Weight.ToString());
            if (record == null) return null;

            UpdateWeightMesStatus(record);
            return new WeightMesStatusInfo
            {
                Status = record.Status,
                ErrorMessage = record.ErrorMessage,
                FailureSource = GetMesRecordFailureSource(record),
                UpdatedAt = record.UpdatedAt
            };
        }

        /// <summary>
        /// 根据本地MES记录推断失败来源。
        /// </summary>
        private static string GetMesRecordFailureSource(MesOutboxRecord record)
        {
            if (record == null) return "本地";
            if (record.Status == MesOutboxStatus.ConfirmedFail) return "MES";
            if (record.Status == MesOutboxStatus.Created) return "本地";
            if (record.Status == MesOutboxStatus.PendingRetry) return "网络/接口";
            if (record.Status == MesOutboxStatus.ManualProcessing) return "本地";
            if (record.Status == MesOutboxStatus.OfflineBypass) return "本地";
            return "MES";
        }

        /// <summary>
        /// Weight工序未确认PASS时，通知PLC禁止当前条码打印。
        /// </summary>
        private void NotifyWeightPrintForbidden(string reason)
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;
            if (!EnablePrintCode.Checked) return;

            TryWriteInt16Value(addrInfo.PrintTrigger, 2);
            TryWriteInt16Value(addrInfo.PrintFeedback, 2);
            Log4netHelper.LogLabelPrint("WEIGHT_FORBID_PRINT", reason, new Dictionary<string, object>
            {
                { "trigger", addrInfo.PrintTrigger },
                { "feedback", addrInfo.PrintFeedback }
            }, level: "WARN");
        }

        /// <summary>
        /// 记录普通同步过站的MES结果。
        /// <para>同步模式不创建后台上传记录，日志只表达当前过站结果和失败来源。</para>
        /// </summary>
        private void LogMesSyncResult(UploadManagerEntity uploadEntity, List<string> scannedBarcodeList, bool isPass, string source, string reason)
        {
            if (uploadEntity == null) return;

            Log4netHelper.LogProductPass(isPass ? "MES_SYNC_CONFIRMED_PASS" : "MES_SYNC_CONFIRMED_FAIL", isPass ? "同步过站成功" : "同步过站失败", new Dictionary<string, object>
            {
                { "process", uploadEntity.Name },
                { "barcode", scannedBarcodeList?.FirstOrDefault() },
                { "source", source },
                { "result", isPass ? "PASS" : "FAIL" },
                { "failureType", isPass ? string.Empty : GetMesFailureType(reason) },
                { "duplicateKey", isPass ? string.Empty : ExtractMesDuplicateKey(reason) },
                { "errorMessage", reason }
            }, level: isPass ? "INFO" : "WARN");
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
        public ReturnParamSendResult SendResultToMes(List<string> scannedBarcodeList, List<string> productResultList, List<string> valList, List<string> maxList, List<string> minList, List<string> resList, List<string> staList, UploadManagerEntity uploadEntity, ProductPassTraceContext trace = null, bool handleMesFailure = true, MesOutboxRecord outboxRecord = null)
        {
            MesOutboxRecord mesOutboxRecord = outboxRecord;
            bool useOutboxRecord = !handleMesFailure && mesOutboxRecord != null;

            // 线程中需要捕获异常，否则会直接退出
            try
            {
                // 获取当前工序需要上传的测试项名称和单位
                GetFilteredTestItems(uploadEntity, out var currentTestNameList, out var currentUnitList);

                // 请求构造计时：方法入口 → 发起HTTP之前（仅普通同步模式记录流程行）
                Stopwatch buildWatch = Stopwatch.StartNew();
                PrdSNCollection2 prdSNCollection = new PrdSNCollection2();
                List<PrdSNsItem> prdSNsItems = new List<PrdSNsItem>();

                // 1. 加载本地维护的设备缺陷库
                DataTable localDefectTable = curDb.Find("SELECT * FROM Defect");

                var inputParam = new InputParamSendResult
                {
                    Line = uploadEntity.Line,
                    Process = uploadEntity.Process,
                    Station = uploadEntity.Staiton,
                    Device = uploadEntity.Device,
                    Employee = txtUser.GetPropertySafely(c => c.Text),
                    Fixture = chkBanFixtureUpload.GetPropertySafely(c => c.Checked) ? string.Empty : FixtureCode,
                    TrackNo = string.Empty,
                    PhotoFTPPath = string.Empty,
                    ResultFileFTPPath = string.Empty,
                    PlanNo = OrderNo.Text,
                    BoardSideSN = scannedBarcodeList[0],
                    PrdSNCollection = prdSNCollection
                };

                // 判断是否强制过站
                string processName = cboEnforcePass.GetPropertySafely(it => it.Text);
                bool isSubmitPass = processName == nameof(ProcessName.All) || processName == uploadEntity.Name.ToString();

                for (int a = 0; a < scannedBarcodeList.Count; a++)
                {
                    UploadMes.AppendToComponent($"[{uploadEntity.Name}] 条码{a + 1}：{scannedBarcodeList[a]}");

                    TestDatas testDatas = new TestDatas { TestData = new List<TestDataItem>() };
                    Defects productDefects = new Defects { Defect = new List<Defect>() };

                    bool isProductNg = false; // 标记当前产品是否发生不良

                    // 当前工序没有测试项时，仍然需要上传一个空的测试项，避免接口报错
                    if (currentTestNameList.Count == 0)
                        testDatas.TestData.Add(new TestDataItem());

                    // 如果存在测试项，则拼接测试项数据
                    // 这里务必确保各个数组的长度一致，且数据对应正确
                    for (int i = 0; i < currentTestNameList.Count; i++)
                    {
                        string currentResult = isSubmitPass ? "OK" : resList[i];
                        string testName = currentTestNameList[i];
                        string stdandardValue = staList[i];  // PLC读取的标准值

                        // ==========================================
                        //  缺陷判定与匹配逻辑
                        // ==========================================
                        // 判断该测试项是否 NG（具体判断条件根据你的PLC设定，通常2代表NG）
                        if (currentResult == "NG")
                        {
                            isProductNg = true; // 触发不良标记

                            // 【核心约定】当结果为NG时，PLC写入实际值的其实是“不良代码” (如: 1, 2)
                            string defectType = stdandardValue;
                            string defectDesc = string.Empty;

                            // 从本地缺陷表匹配对应的描述
                            if (localDefectTable != null && localDefectTable.Rows.Count > 0)
                            {
                                // 根据约定好的defectType关联对应的不良描述，1：超扭力上限，2：滑牙
                                DataRow[] mappedRows = localDefectTable.Select($"id = '{defectType}'");
                                if (mappedRows.Length > 0)
                                {
                                    defectDesc = mappedRows[0]["DefectDesc"].ToString();
                                }
                            }

                            // 添加不良信息
                            productDefects.Defect.Add(new Defect
                            {
                                Location = testName,          // 发生不良的具体测试项名称（如："螺钉1"）
                                DefectDesc = defectDesc,      // 匹配出的不良描述（如："滑牙" 或 "超扭力上限"）
                                Missing = "0"                 // 自动生产无误判，固定为 0
                            });
                        }
                        else
                        {
                            // 如果该项是 OK 的，暂存到测试数据集合中
                            testDatas.TestData.Add(new TestDataItem
                            {
                                Name = testName,
                                Value = valList[i],
                                Result = currentResult,     // 测试项结果
                                USL = maxList[i],
                                LSL = minList[i],
                                Unit = currentUnitList[i]
                            });
                        }
                    }

                    // ==========================================
                    // 数据上传策略：NG时只传缺陷，不传测试项
                    // ==========================================
                    if (isProductNg)
                    {
                        testDatas.TestData.Clear(); // 发现不良，清空所有测试项数据
                    }
                    else if (testDatas.TestData.Count == 0)
                    {
                        testDatas.TestData.Add(new TestDataItem()); // 兜底空数据防止接口报错
                    }

                    // 决定产品最终的 Pass/Fail 状态
                    string productResult = isSubmitPass ? "Pass" : (productResultList[a] == "3" ? "Pass" : "Fail");
                    if (isProductNg) productResult = "Fail"; // 只要有任何一项不良，总体必须 Fail

                    PrdSNsItem prdSNs = new PrdSNsItem
                    {
                        PrdSN = scannedBarcodeList[a],
                        SubBoardId = (a + 1).ToString(),
                        BoardSkip = "False",
                        Result = productResult,         // 整个产品的总体过站结果（非单一测试项结果）
                        MachineResult = productResult,
                        CycleTime = "0",
                        ResultFile = "",                // 单个文件，_OperTxt中赋值
                        PhotoFiles = new PhotoFiles(),  // 多个文件，_OperPicture中赋值
                        // 动态赋值：有数据才赋值，无数据直接给 null
                        TestDatas = testDatas.TestData.Count > 0 ? testDatas : null,
                        Defects = productDefects.Defect.Count > 0 ? productDefects : null
                        /*TestDatas = testDatas,
                        Defects = new Defects(),*/
                    };

                    // 非上工装机时，需要保存文件和图片数据。
                    /* if (!EnableUpperTooling.Checked)
                     {
                         // 上传图片并添加PrdSNs的图片文件名数据到json
                         //if (!_OperPicture(PrdSNInfo, inputParam))
                         //{
                         //HandleError(uploadEntity.feedbackPoint, true, $"{uploadEntity.Name}:无法上传CCD图片至MES");
                         //    return null;
                         //}

                         // 再处理txt
                         if (!_OperTxt(prdSNs, inputParam))
                         {
                             HandleError(uploadEntity.feedbackPoint, true, $"{uploadEntity.Name}:无法上传txt文件至MES");
                             return null;
                         }
                     }*/

                    // 添加数据
                    prdSNsItems.Add(prdSNs);
                }

                prdSNCollection.PrdSNs = prdSNsItems;

                if (useOutboxRecord)
                    mesOutboxRecord = SaveOutboxPayload(mesOutboxRecord, inputParam);

                // 第5行：请求构造完成（仅普通同步模式）
                if (handleMesFailure) trace?.LogFlowElapsed("请求构造完成", buildWatch);

                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 请求MES流程开始");
                // 第6行：发起过站请求（仅普通同步模式）
                if (handleMesFailure) trace?.LogFlow("发起过站请求");
                Stopwatch httpWatch = Stopwatch.StartNew();
                var returnParam = _request.GetResponseSerializeResult<ReturnParamSendResult, InputParamSendResult>(Url_DataUpload.Text, _httpClient, "SAVERESULT", inputParam, nameof(uploadEntity.Name));
                httpWatch.Stop();
                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 请求MES流程结束");

                if (returnParam == null)
                {
                    string nullReason = "上传结果接口返回数据异常（null），可能是超时、网络异常或响应解析失败";
                    if (useOutboxRecord)
                        MarkOutboxPendingRetry(mesOutboxRecord, "RESPONSE_NULL", nullReason);
                    else
                        LogMesSyncResult(uploadEntity, scannedBarcodeList, false, "网络/接口", nullReason);

                    UpdateWeightMesStatus(uploadEntity?.Name, scannedBarcodeList, MesOutboxStatus.PendingRetry, nullReason, "网络/接口");
                    if (uploadEntity.Name == ProcessName.Weight)
                        NotifyWeightPrintForbidden("Weight过站结果未知，禁止当前条码打印");
                    // 第7行失败：收到响应为空（含响应头超时时附"可能已落库勿盲目重试"提示）
                    if (handleMesFailure)
                        trace?.LogFlowFailure("收到过站响应", BuildMesFailReason("接口返回数据异常(Null)"));
                    else
                        trace?.Diag("MES_NULL_RETURN", "上传结果接口返回数据异常（null），后台模式只记录，不写PLC NG");
                    if (handleMesFailure)
                    {
                        HandleError(uploadEntity.feedbackPoint, 2, true, $"[{uploadEntity.Name}] {BuildMesFailReason("上传结果接口返回数据异常（null）")}");
                    }
                    return null;
                }

                if (IsMesPass(returnParam))
                {
                    if (useOutboxRecord)
                        MarkOutboxConfirmedPass(mesOutboxRecord, returnParam);
                    else
                        LogMesSyncResult(uploadEntity, scannedBarcodeList, true, "MES", returnParam.ErrorMessage);

                    UpdateWeightMesStatus(uploadEntity?.Name, scannedBarcodeList, MesOutboxStatus.ConfirmedPass, returnParam.ErrorMessage, "MES");
                }
                else if (useOutboxRecord && mesOutboxRecord.RetryCount > 0 && IsDuplicatePassMesResult(returnParam))
                {
                    MarkOutboxConfirmedPass(mesOutboxRecord, returnParam);
                    returnParam.Result = nameof(MyEnum.Result.PASS);
                    returnParam.ErrorMessage = "MES返回重复过站，先反馈再上传记录按已过站处理";
                    UpdateWeightMesStatus(uploadEntity?.Name, scannedBarcodeList, MesOutboxStatus.ConfirmedPass, returnParam.ErrorMessage, "MES");
                }
                else
                {
                    if (useOutboxRecord)
                        MarkOutboxConfirmedFail(mesOutboxRecord, GetMesFailureErrorType(returnParam.ErrorMessage), returnParam.ErrorMessage);
                    else
                        LogMesSyncResult(uploadEntity, scannedBarcodeList, false, "MES", returnParam.ErrorMessage);

                    UpdateWeightMesStatus(uploadEntity?.Name, scannedBarcodeList, MesOutboxStatus.ConfirmedFail, returnParam.ErrorMessage, "MES");
                    if (uploadEntity.Name == ProcessName.Weight)
                        NotifyWeightPrintForbidden($"Weight过站失败，禁止当前条码打印：{returnParam.ErrorMessage}");
                }

                // 第7行：收到过站响应（仅普通同步模式）——PASS取耗时，非PASS取失败原因
                if (handleMesFailure)
                {
                    bool respPass = string.Equals(returnParam.Result, nameof(MyEnum.Result.PASS), StringComparison.OrdinalIgnoreCase);
                    if (respPass)
                        trace?.LogFlowElapsed("收到过站响应", httpWatch);
                    else
                        trace?.LogFlowFailure("收到过站响应", string.IsNullOrEmpty(returnParam.ErrorMessage) ? "MES返回未通过" : returnParam.ErrorMessage);
                }

                return returnParam;
            }
            catch (Exception ex)
            {
                if (useOutboxRecord && mesOutboxRecord != null && string.IsNullOrWhiteSpace(mesOutboxRecord.PayloadJson))
                    MarkOutboxManualProcessing(mesOutboxRecord, "REQUEST_BUILD_ERROR", ex.Message);
                else if (useOutboxRecord)
                    MarkOutboxPendingRetry(mesOutboxRecord, "REQUEST_EXCEPTION", ex.Message);
                else
                    LogMesSyncResult(uploadEntity, scannedBarcodeList, false, "本地", ex.Message);

                UpdateWeightMesStatus(uploadEntity?.Name, scannedBarcodeList, MesOutboxStatus.PendingRetry, ex.Message, "本地");
                if (uploadEntity != null && uploadEntity.Name == ProcessName.Weight)
                    NotifyWeightPrintForbidden($"Weight过站异常，禁止当前条码打印：{ex.Message}");
                if (handleMesFailure)
                    trace?.LogFlowFailure("收到过站响应", $"数据上传流程发生异常：{ex.Message}");
                trace?.Diag("MES_REQUEST_EXCEPTION", "数据上传流程发生异常", ex);
                if (handleMesFailure)
                {
                    HandleError(uploadEntity.feedbackPoint, 2, true, $"[{uploadEntity.Name}] 数据上传流程发生异常：{ex}");
                }
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
            string localPath = Path.Combine(LocalFilePath.GetPropertySafely(c => c.Text), "Txt");
            string fullPath = Path.Combine(localPath, txtFileName);

            SaveTxtFileToLocal(fullPath, fileContent);

            string url = FTPlog.GetPropertySafely(c => c.Text);
            string process = Process.GetPropertySafely(c => c.Text);
            string line = Line.GetPropertySafely(c => c.Text);
            string user = FTPID.GetPropertySafely(c => c.Text);
            string pwd = FTPCODE.GetPropertySafely(c => c.Text);

            HttpClientUtil ftpClient = new HttpClientUtil(url, process, line, user, pwd, "Log");
            //ftpClient.CheckFtpDirectory();
            string fileFTPPath = ftpClient.UploadToFtpServer(localPath, txtFileName, txtFileName);

            if (fileFTPPath is null)
            {
                rtbErrorLog.AppendToComponent($"上传txt文件:{fullPath} 至MES失败");
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
                string localPath = Path.Combine(LocalFilePath.GetPropertySafely(c => c.Text), "PrdSNPictures");
                //获取本地文件
                string[] pictureFiles = Directory.GetFiles(Path.Combine(localPath, prdSNs.PrdSN));

                //将jpg转为jpeg
                //JpgConvertJpeg(picturePaths, 20);

                List<Task<string>> uploadTask = new List<Task<string>>();

                string url = FTPPIC.GetPropertySafely(c => c.Text);
                string process = Process.GetPropertySafely(c => c.Text);
                string line = Line.GetPropertySafely(c => c.Text);
                string user = FTPID.GetPropertySafely(c => c.Text);
                string pwd = FTPCODE.GetPropertySafely(c => c.Text);

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
                    rtbErrorLog.AppendToComponent("没有检测到任何图片");
                    return false;
                }

                // 查看所有任务的返回值，如果有null则说明有图片上传失败
                if (ftpPaths.Contains(null))
                {
                    rtbErrorLog.AppendToComponent("上传单张图片至MES失败");
                    return false;
                }

                inputParam.PhotoFTPPath = ftpPaths[0];

                //Directory.Delete(Path.Combine(localPath, PrdSNInfo.PrdSN), true);

                return true;
            }
            catch (Exception)
            {
                //writeLog.AppendToComponent(errorLog, $"上传图片异常：{e}");
                //return false;
                return true;
            }
        }

        /// <summary>
        /// 上传结果后无论成功或失败都需要执行
        /// </summary>
        /// <param name="uploadManagerEntity"></param>
        /// <param name="barcodeList"></param>
        /// <param name="resultList"></param>
        private void SendResultAfter(UploadManagerEntity uploadManagerEntity, List<string> barcodeList, List<string> resultList)
        {
            // 上传结果后删除临时存储图片的文件夹
            //if (uploadManagerEntity.DeleteFile)
            //{
            //    DeletePicture(barcodeList);
            //}

            barcodeList.Clear();//条码数组
            resultList.Clear();  //结果数组
        }

        /// <summary>
        /// 筛选测试项名称和单位
        /// </summary>
        /// <param name="uploadManagerEntity"></param>
        /// <param name="testNameList">输出：筛选后的测试项名称列表</param>
        /// <param name="unitNameList">输出：筛选后的单位名称列表</param>
        private void GetFilteredTestItems(UploadManagerEntity uploadManagerEntity, out List<string> testNameList, out List<string> unitNameList)
        {
            // 1. 获取当前工序的序号
            string processNum = ((int)uploadManagerEntity.Name).ToString();

            // 2. 判断是否需要过滤（非装配机不需要过滤）
            if (uploadManagerEntity.Name == ProcessName.Non_Assembly)
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

                    // 核心比对：如果当前测试项的工位号 == 工序号
                    if (stationIdArray[i] == processNum)
                    {
                        tempNameList.Add(testNameArray[i]);
                        tempUnitList.Add(unitNameArray[i]);
                    }
                }
            }

            testNameList = tempNameList;
            unitNameList = tempUnitList;
        }

        #endregion

        #region ---------- 扭力数据采集与转发 ----------

        /// <summary>
        /// 尝试读取PLC Int16值，并输出读取到的值，最多重试3次。
        /// <para> rawContent = -1表示读取失败 </para>
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryReadInt16(string address, out short value, string failReason)
        {
            value = -1;

            if (!isPlcConnected)
            {
                rtbErrorLog.AppendToComponent($"[{failReason}] PLC未连接，无法读取地址{address}");
                Log4netHelper.LogDataException("PLC_READ_DISCONNECTED", failReason, new Dictionary<string, object>
                {
                    { "address", address }
                });
                return false;
            }

            var result = _readWriteNet.ReadInt16(address);
            if (result.IsSuccess)
            {
                value = result.Content;
                return true;
            }

            rtbErrorLog.AppendToComponent($"{failReason} | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
            Log4netHelper.LogDataException("PLC_READ_FAILED", failReason, new Dictionary<string, object>
            {
                { "address", address },
                { "errorCode", result.ErrorCode },
                { "reason", result.Message }
            });
            return false;
        }

        /// <summary>
        /// 尝试写入PLC Int32值
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryWriteInt32(string address, int value, string detailMsg = null)
        {
            string prefix = string.IsNullOrWhiteSpace(detailMsg) ? "" : $"[{detailMsg}] ";

            if (!isPlcConnected)
            {
                rtbErrorLog.AppendToComponent($"{prefix} PLC未连接，无法写入地址: {address}");
                Log4netHelper.LogDataException("PLC_WRITE_DISCONNECTED", detailMsg, new Dictionary<string, object>
                {
                    { "address", address },
                    { "value", value }
                });
                return false;
            }

            var result = _readWriteNet.Write(address, value);
            if (result.IsSuccess) return true;

            rtbErrorLog.AppendToComponent($"{prefix}PLC写入失败 | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
            Log4netHelper.LogDataException("PLC_WRITE_FAILED", detailMsg, new Dictionary<string, object>
            {
                { "address", address },
                { "value", value },
                { "errorCode", result.ErrorCode },
                { "reason", result.Message }
            });
            return false;
        }

        /// <summary>
        /// 尝试写入PLC Int16值。
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryWriteInt16(string address, short value, string failReson = null)
        {
            if (!isPlcConnected)
            {
                rtbErrorLog.AppendToComponent($"{failReson} PLC未连接，无法写入地址: {address}");
                Log4netHelper.LogDataException("PLC_WRITE_INT16_DISCONNECTED", failReson, new Dictionary<string, object>
                {
                    { "address", address },
                    { "value", value }
                });
                return false;
            }

            var result = _readWriteNet.Write(address, value);
            if (result.IsSuccess) return true;

            rtbErrorLog.AppendToComponent($"{failReson} | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
            Log4netHelper.LogDataException("PLC_WRITE_INT16_FAILED", failReson, new Dictionary<string, object>
            {
                { "address", address },
                { "value", value },
                { "errorCode", result.ErrorCode },
                { "reason", result.Message }
            });
            return false;
        }

        // 定义两个扭力控制器客户端
        private TorqueControllerClient _clientScanAssy; // 工序1 (Scan-ASSY)
        private TorqueControllerClient _clientScrewBa;  // 工序3 (Screw-BA，实际在工位5动作)
        private CancellationTokenSource _torqueCts;     // 用于取消互锁监控循环
        private readonly SemaphoreSlim _scanAssyTorqueTransferLock = new SemaphoreSlim(1, 1); // Scan_ASSY独立转发锁，避免多笔扭力并发覆盖D7620-D7629
        private readonly SemaphoreSlim _screwBaTorqueTransferLock = new SemaphoreSlim(1, 1);  // Screw_BA独立转发锁，避免多笔扭力并发覆盖D7630-D7639
        private volatile bool _isScanAssyWaitingTorqueAck; // Scan_ASSY是否正在等待PLC ACK
        private volatile bool _isScrewBaWaitingTorqueAck;  // Screw_BA是否正在等待PLC ACK

        /// <summary>
        /// 扭力转发使用的一组PLC地址和UI控件。
        /// </summary>
        private sealed class TorquePlcContext
        {
            public ProcessName ProcessName { get; set; }
            public string TorqueAddress { get; set; }
            public string MaxAddress { get; set; }
            public string MinAddress { get; set; }
            public string ResultAddress { get; set; }
            public string RequestAddress { get; set; }
            public string AckAddress { get; set; }
            public Label ValueLabel { get; set; }
            public Label MinLabel { get; set; }
            public Label MaxLabel { get; set; }
            public Label ResultLabel { get; set; }
        }

        // 初始化扭力系统
        private void InitTorqueSystem()
        {
            if (Global.Instance.CurDataBaseName != "装配机") return;

            // 1. 启动独立的后台任务，专门监控并实时写入两个工位的互锁信号
            if (_torqueCts == null)
            {
                _torqueCts = new CancellationTokenSource();
                Task.Run(() => TorqueInterlockMonitorLoopAsync(_torqueCts.Token));
            }

            # region -------- 初始化工序1 (Scan-ASSY) 控制器 --------

            string ip = txtControllerIP1.GetPropertySafely(c => c.Text);
            var port = txtControllerPort1.GetPropertySafely(c => c.Text);

            _clientScanAssy = new TorqueControllerClient(ip, int.Parse(port));  // 192.168.1.31

            // 连接状态变化事件：更新UI显示，并根据连接状态写入PLC互锁地址，触发报警
            _clientScanAssy.OnConnectionStatusChanged += (isControllerConnected, msg) =>
            {
                ASSY.ExecuteSafely(c => c.ForeColor = isControllerConnected ? Color.Green : Color.Red);
            };

            _clientScanAssy.OnLog += (msg, isErrorLog) =>
            {
                if (isErrorLog)
                {
                    ReportTorqueControllerCommunicationError(ProcessName.Scan_ASSY, msg);
                }
                else
                {
                    AppendLog(ProcessName.Scan_ASSY, msg);
                }
            };

            _clientScanAssy.OnTorqueDataReceived += (data) =>
            {
                AppendLog(ProcessName.Scan_ASSY, BuildTorqueForwardReadyMessage(data));
                Task.Run(async () => await ForwardTorqueToPlcAsync(ProcessName.Scan_ASSY, data));
            };

            _clientScanAssy.Start();

            #endregion

            #region -------- 初始化工序3 (Scan-BA) 控制器 --------

            string ip3 = txtControllerIP2.GetPropertySafely(c => c.Text);
            var port3 = txtControllerPort2.GetPropertySafely(c => c.Text);
            _clientScrewBa = new TorqueControllerClient(ip3, Convert.ToInt32(port3));   // 192.168.1.32

            _clientScrewBa.OnConnectionStatusChanged += (isControllerConnected, msg) =>
            {
                BA.ExecuteSafely(c => c.ForeColor = isControllerConnected ? Color.Green : Color.Red);
            };

            _clientScrewBa.OnLog += (msg, isErrorLog) =>
            {
                if (isErrorLog)
                {
                    ReportTorqueControllerCommunicationError(ProcessName.Screw_BA, msg);
                }
                else
                {
                    AppendLog(ProcessName.Screw_BA, msg);
                }
            };

            _clientScrewBa.OnTorqueDataReceived += (data) =>
            {
                AppendLog(ProcessName.Screw_BA, BuildTorqueForwardReadyMessage(data));
                Task.Run(async () => await ForwardTorqueToPlcAsync(ProcessName.Screw_BA, data));
            };

            _clientScrewBa.Start();

            #endregion
        }

        /// <summary>
        /// 每秒按电批连接状态写入PLC互锁信号：1=允许打螺钉，2=禁止。
        /// <para>只根据当前连接状态写入，不读取比对、不判断是否已同步。</para>
        /// </summary>
        private async Task TorqueInterlockMonitorLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);

                if (!isPlcConnected) continue;

                if (_clientScanAssy == null || _clientScrewBa == null) continue;

                // ---------- 工序1 互锁：按连接状态每秒写入 ----------

                short targetVal1 = _clientScanAssy.IsConnected ? (short)1 : (short)2;
                await WriteInterlockAsync(addrInfo.TorqueReady1, targetVal1, "工序1电批互锁信号写入失败", token);

                // ---------- 工序3 互锁：按连接状态每秒写入 ----------

                short targetVal3 = _clientScrewBa.IsConnected ? (short)1 : (short)2;
                await WriteInterlockAsync(addrInfo.TorqueReady3, targetVal3, "工序3电批互锁信号写入失败", token);
            }
        }

        /// <summary>
        /// 写入互锁信号，带3秒超时；超时或失败时上报错误。
        /// </summary>
        private async Task WriteInterlockAsync(string address, short value, string errorMessage, CancellationToken token)
        {
            var writeTask = _readWriteNet.WriteAsync(address, value);
            var completedTask = await Task.WhenAny(writeTask, Task.Delay(3000, token));

            if (token.IsCancellationRequested) return;

            if (completedTask != writeTask || !writeTask.Result.IsSuccess)
                HandleError(address, value, true, errorMessage);
        }

        /// <summary>
        /// 将接收到的扭力数据写入对应的 PLC 地址
        /// </summary>
        private async Task ForwardTorqueToPlcAsync(ProcessName processName, TorqueData data)
        {
            if (data == null || !TryGetTorquePlcContext(processName, out TorquePlcContext context)) return;

            SemaphoreSlim transferLock = GetTorqueTransferLock(processName);
            await transferLock.WaitAsync();

            try
            {
                await ForwardTorqueToPlcLockedAsync(context, data);
            }
            finally
            {
                transferLock.Release();
            }
        }

        /// <summary>
        /// 已拿到工序锁后的扭力转发流程。
        /// </summary>
        private async Task ForwardTorqueToPlcLockedAsync(TorquePlcContext context, TorqueData data)
        {
            int.TryParse(data.Torque, out int val);
            int.TryParse(data.TorqueMin, out int min);
            int.TryParse(data.TorqueMax, out int max);
            short result = data.TighteningStatus ? (short)3 : (short)2; // 3=OK, 2=NG
            string transferId = BuildTorqueTransferId(context.ProcessName);
            string resultText = data.TighteningStatus ? "OK" : "NG";
            int ackTimeoutMs = GetTorqueAckTimeoutMs();

            try
            {
                Invoke((Action)(() =>
                {
                    context.ValueLabel.Text = $"{(double)val / 100:0.00}";
                    context.MinLabel.Text = $"{(double)min / 100:0.00}";
                    context.MaxLabel.Text = $"{(double)max / 100:0.00}";
                    context.ResultLabel.Text = resultText;
                    context.ResultLabel.ForeColor = data.TighteningStatus ? Color.Green : Color.Red;
                }));

                if (!WriteTorqueDataToPlc(context, val, max, min, result))
                {
                    string message = $"[{context.ProcessName}] 扭力核心数据写入PLC失败，TransferId={transferId}，扭力={val}，结果={resultText}";
                    AppendLog(context.ProcessName, message);
                    HandleError(null, null, true, message);
                    return;
                }

                if (!TryWriteInt16(context.RequestAddress, 1, failReson: "PC请求握手(Req=1)"))
                {
                    string message = $"[{context.ProcessName}] PC请求握手写入失败，Req={context.RequestAddress}，TransferId={transferId}";
                    AppendLog(context.ProcessName, message);
                    HandleError(null, null, true, message);
                    return;
                }

                SetTorqueAckWaitingState(context.ProcessName, true);
                AppendLog(context.ProcessName, $"[转发请求] TransferId={transferId}，扭力={val}，结果={resultText}，Req={context.RequestAddress}=1，等待Ack={context.AckAddress}，超时={ackTimeoutMs}ms");

                var ackResult = await WaitForTorqueAckAsync(context, ackTimeoutMs);
                if (!ackResult.IsAckReceived)
                {
                    HandleTorqueAckTimeoutAndResetRequest(context, transferId, val, resultText, ackTimeoutMs, ackResult.LastAckValue, ackResult.ElapsedMs, ackResult.FailedReadCount);
                    return;
                }

                AppendLog(context.ProcessName, $"PLC ACK已收到，TransferId={transferId}，Ack={context.AckAddress}当前值={ackResult.LastAckValue}，等待耗时={ackResult.ElapsedMs}ms");

                await WaitForTorqueAckResetAsync(context, transferId);
                AppendLog(context.ProcessName, $"[转发成功] TransferId={transferId}，扭力:{val}，结果:{resultText}，Req={context.RequestAddress}已复位0，Ack={context.AckAddress}已回0，恢复扭力转发");
            }
            catch (Exception ex)
            {
                AppendLog(context.ProcessName, $"[扭力转发异常] TransferId={transferId}，发生未捕获异常: {ex.Message}");
            }
            finally
            {
                SetTorqueAckWaitingState(context.ProcessName, false);
            }
        }

        /// <summary>
        /// 获取 PLC 接收扭力 ACK 超时时间，单位：毫秒。
        /// </summary>
        private int GetTorqueAckTimeoutMs()
        {
            string secondsText = NormalizeTorqueAckTimeoutSeconds(txtTorqueAckTimeoutSeconds.GetPropertySafely(c => c.Text));
            return int.Parse(secondsText) * 1000;
        }

        /// <summary>
        /// 按工序获取独立串行锁，保证同一组PLC地址不会被并发覆盖。
        /// </summary>
        private SemaphoreSlim GetTorqueTransferLock(ProcessName processName)
        {
            return processName == ProcessName.Screw_BA ? _screwBaTorqueTransferLock : _scanAssyTorqueTransferLock;
        }

        /// <summary>
        /// ACK等待期间将对应工序互锁置为禁止，防止下一笔扭力继续进入同一组握手地址。
        /// </summary>
        private void SetTorqueAckWaitingState(ProcessName processName, bool isWaiting)
        {
            if (processName == ProcessName.Screw_BA)
                _isScrewBaWaitingTorqueAck = isWaiting;
            else
                _isScanAssyWaitingTorqueAck = isWaiting;
        }

        /// <summary>
        /// 将扭力值、上下限和结果一次性写入PLC连续地址。
        /// </summary>
        private bool WriteTorqueDataToPlc(TorquePlcContext context, int val, int max, int min, short result)
        {
            byte[] buffer = new byte[14];

            BitConverter.GetBytes(val).CopyTo(buffer, 0);
            BitConverter.GetBytes(max).CopyTo(buffer, 4);
            BitConverter.GetBytes(min).CopyTo(buffer, 8);
            BitConverter.GetBytes(result).CopyTo(buffer, 12);

            return TryWriteByteArray(context.TorqueAddress, buffer, "批量写入扭力核心数据(包含值、上下限、结果)");
        }

        /// <summary>
        /// 等待PLC将ACK置为1。超过配置时间后返回失败，由调用方清零Req并忽略本次转发。
        /// </summary>
        private async Task<(bool IsAckReceived, short LastAckValue, long ElapsedMs, int FailedReadCount)> WaitForTorqueAckAsync(
            TorquePlcContext context,
            int timeoutMs)
        {
            Stopwatch watch = Stopwatch.StartNew();
            short lastAckValue = -1;
            int failedReadCount = 0;

            while (watch.ElapsedMilliseconds < timeoutMs)
            {
                var readResult = await ReadTorqueAckValueAsync(context.AckAddress);
                if (readResult.IsSuccess)
                {
                    lastAckValue = readResult.Value;
                    if (readResult.Value == 1)
                        return (true, lastAckValue, watch.ElapsedMilliseconds, failedReadCount);
                }
                else
                {
                    failedReadCount++;
                }

                await Task.Delay(TorqueAckPollIntervalMs);
            }

            return (false, lastAckValue, watch.ElapsedMilliseconds, failedReadCount);
        }

        /// <summary>
        /// ACK等待超时后，上报异常并清零Req，避免本次扭力转发继续占用握手位。
        /// </summary>
        private void HandleTorqueAckTimeoutAndResetRequest(
            TorquePlcContext context,
            string transferId,
            int torqueValue,
            string resultText,
            int timeoutMs,
            short lastAckValue,
            long elapsedMs,
            int failedReadCount)
        {
            string message = string.Format(
                "[{0}] 扭力ACK超时，TransferId={1}，扭力={2}，结果={3}，Req={4}，Ack={5}当前值={6}，配置超时={7}ms，实际等待={8}ms，读取失败次数={9}，已忽略本次转发",
                context.ProcessName,
                transferId,
                torqueValue,
                resultText,
                context.RequestAddress,
                context.AckAddress,
                lastAckValue,
                timeoutMs,
                elapsedMs,
                failedReadCount);

            // 只在后台记录异常
            //lblStatusErrorTip.ExecuteSafely(c => { c.Text = message; c.ForeColor = Color.Red; });
            rtbErrorLog.AppendToComponent(message);
            AppendLog(context.ProcessName, message);

            Log4netHelper.LogDataException("TORQUE_FORWARD_ACK_TIMEOUT", message, new Dictionary<string, object>
            {
                { "process", context.ProcessName },
                { "transferId", transferId },
                { "torque", torqueValue },
                { "result", resultText },
                { "requestAddress", context.RequestAddress },
                { "ackAddress", context.AckAddress },
                { "lastAckValue", lastAckValue },
                { "timeoutMs", timeoutMs },
                { "elapsedMs", elapsedMs },
                { "failedReadCount", failedReadCount }
            });

            bool requestCleared = TryWriteInt16(context.RequestAddress, 0, failReson: "扭力转发ACK超时清零Req");
            string resetMessage = requestCleared
                ? $"[{context.ProcessName}] 扭力ACK超时处理完成，TransferId={transferId}，Req已清零，已忽略本次转发"
                : $"[{context.ProcessName}] 扭力ACK超时后Req清零失败，TransferId={transferId}，Req={context.RequestAddress}";

            AppendLog(context.ProcessName, resetMessage);
            if (!requestCleared)
            {
                lblStatusErrorTip.ExecuteSafely(c => { c.Text = resetMessage; c.ForeColor = Color.Red; });
                rtbErrorLog.AppendToComponent(resetMessage);
                Log4netHelper.LogDataException("TORQUE_FORWARD_ACK_TIMEOUT_REQ_RESET_FAILED", resetMessage, new Dictionary<string, object>
                {
                    { "process", context.ProcessName },
                    { "transferId", transferId },
                    { "requestAddress", context.RequestAddress }
                });
            }
        }

        /// <summary>
        /// PLC确认收到后，复位Req并等待PLC将ACK回零，再释放下一笔扭力转发。
        /// </summary>
        private async Task WaitForTorqueAckResetAsync(TorquePlcContext context, string transferId)
        {
            bool requestReset = false;
            bool resetErrorLogged = false;

            while (!requestReset)
            {
                requestReset = TryWriteInt16(context.RequestAddress, 0, failReson: "复位PC请求握手(Req=0)");
                if (!requestReset)
                {
                    if (!resetErrorLogged)
                    {
                        string message = $"[{context.ProcessName}] PLC ACK已收到，但Req复位失败，TransferId={transferId}，Req={context.RequestAddress}";
                        AppendLog(context.ProcessName, message);
                        HandleError(null, null, true, message);
                        resetErrorLogged = true;
                    }

                    await Task.Delay(1000);
                }
            }

            Stopwatch watch = Stopwatch.StartNew();
            bool ackResetWaitLogged = false;

            while (true)
            {
                var readResult = await ReadTorqueAckValueAsync(context.AckAddress);
                if (readResult.IsSuccess && readResult.Value == 0)
                    return;

                if (!ackResetWaitLogged && watch.ElapsedMilliseconds >= TorqueAckInitialTimeoutMs)
                {
                    short ackValue = readResult.IsSuccess ? readResult.Value : (short)-1;
                    AppendLog(context.ProcessName, $"[ACK回零等待] TransferId={transferId}，Req={context.RequestAddress}已复位0，Ack={context.AckAddress}当前值={ackValue}，继续等待PLC回零");
                    ackResetWaitLogged = true;
                }

                await Task.Delay(TorqueAckPollIntervalMs);
            }
        }

        /// <summary>
        /// 读取ACK当前值。这里不使用通用读取方法，避免后台等待时反复写入错误日志。
        /// </summary>
        private async Task<(bool IsSuccess, short Value, string ErrorMessage)> ReadTorqueAckValueAsync(string ackAddress)
        {
            if (!isPlcConnected || _readWriteNet == null)
                return (false, -1, "PLC未连接");

            var readTask = _readWriteNet.ReadInt16Async(ackAddress);
            var completedTask = await Task.WhenAny(readTask, Task.Delay(500));
            if (completedTask != readTask)
                return (false, -1, "读取ACK超时");

            var result = await readTask;
            if (result.IsSuccess)
                return (true, result.Content, null);

            return (false, -1, result.Message);
        }

        /// <summary>
        /// 构造每笔扭力转发的本地追踪号，方便日志配对。
        /// </summary>
        private string BuildTorqueTransferId(ProcessName processName)
        {
            return $"{processName}_{System.DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}".Substring(0, 42);
        }

        /// <summary>
        /// 获取工序对应的PLC地址和UI控件。
        /// </summary>
        private bool TryGetTorquePlcContext(ProcessName processName, out TorquePlcContext context)
        {
            context = null;

            switch (processName)
            {
                case ProcessName.Scan_ASSY:
                    context = new TorquePlcContext
                    {
                        ProcessName = processName,
                        TorqueAddress = addrInfo.TorqueValue1,
                        MaxAddress = addrInfo.TorqueMax1,
                        MinAddress = addrInfo.TorqueMin1,
                        ResultAddress = addrInfo.TorqueResult1,
                        RequestAddress = addrInfo.Request1,
                        AckAddress = addrInfo.Acknowledge1,
                        ValueLabel = lblAssyVal,
                        MinLabel = lblAssyMin,
                        MaxLabel = lblAssyMax,
                        ResultLabel = lblAssyRes
                    };
                    return true;
                case ProcessName.Screw_BA:
                    context = new TorquePlcContext
                    {
                        ProcessName = processName,
                        TorqueAddress = addrInfo.TorqueValue3,
                        MaxAddress = addrInfo.TorqueMax3,
                        MinAddress = addrInfo.TorqueMin3,
                        ResultAddress = addrInfo.TorqueResult3,
                        RequestAddress = addrInfo.Request3,
                        AckAddress = addrInfo.Acknowledge3,
                        ValueLabel = lblBaVal,
                        MinLabel = lblBaMin,
                        MaxLabel = lblBaMax,
                        ResultLabel = lblBaRes
                    };
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 尝试写入字节数组 (批量写入)
        /// </summary>
        private bool TryWriteByteArray(string address, byte[] value, string detailMsg)
        {
            string prefix = string.IsNullOrWhiteSpace(detailMsg) ? "" : $"[{detailMsg}] ";

            if (!isPlcConnected)
            {
                rtbErrorLog.AppendToComponent($"{prefix} PLC未连接，无法批量写入地址: {address}");
                Log4netHelper.LogDataException("PLC_WRITE_BYTES_DISCONNECTED", detailMsg, new Dictionary<string, object>
                {
                    { "address", address },
                    { "length", value?.Length ?? 0 }
                });
                return false;
            }

            var result = _readWriteNet.Write(address, value);
            if (result.IsSuccess) return true;

            rtbErrorLog.AppendToComponent($"{prefix}PLC写入失败 | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
            Log4netHelper.LogDataException("PLC_WRITE_BYTES_FAILED", detailMsg, new Dictionary<string, object>
            {
                { "address", address },
                { "length", value?.Length ?? 0 },
                { "errorCode", result.ErrorCode },
                { "reason", result.Message }
            });
            return false;
        }

        /// <summary>
        /// 上报扭力控制器通讯异常。
        /// <para>控制器断线后，PLC互锁信号仍由 TorqueInterlockMonitorLoopAsync 负责写入禁止状态。</para>
        /// </summary>
        private void ReportTorqueControllerCommunicationError(ProcessName processName, string message)
        {
            string safeMessage = string.IsNullOrWhiteSpace(message) ? "未知通讯异常" : message;
            string userMessage = $"[{processName}] 扭力控制器通讯异常：{safeMessage}";

            // 通讯异常需要稳定显示在主异常栏，便于操作员直接看到停线原因。
            //lblStatusErrorTip.ExecuteSafely(c => { c.Text = userMessage; c.ForeColor = Color.Red; });
            rtbErrorLog.AppendToComponent(userMessage);
            AppendLog(processName, userMessage);

            Log4netHelper.LogDataException("TORQUE_CONTROLLER_COMMUNICATION_ERROR", userMessage, new Dictionary<string, object>
            {
                { "process", processName },
                { "message", safeMessage }
            });
        }

        /// <summary>
        /// 生成扭力数据进入窗体层后的诊断日志，证明程序已收到解析后的数据。
        /// </summary>
        private static string BuildTorqueForwardReadyMessage(TorqueData data)
        {
            if (data == null) return "程序已收到空扭力数据，准备转发PLC前已拦截";

            string resultText = data.TighteningStatus ? "OK" : "NG";
            return $"程序已收到扭力数据，准备转发PLC，Torque={data.Torque}，Min={data.TorqueMin}，Max={data.TorqueMax}，Result={resultText}，Time={data.TimeStamp}";
        }

        // 辅助方法
        private void AppendLog(ProcessName processName, string msg)
        {
            Log4netHelper.LogTorque("TORQUE_LOG", msg, new Dictionary<string, object>
            {
                { "process", processName }
            });

            switch (processName)
            {
                case ProcessName.Scan_ASSY:
                    rtbASSYLog.AppendToComponent($"[{processName}] {msg}");
                    break;
                case ProcessName.Screw_BA:
                    rtbBALog.AppendToComponent($"[{processName}] {msg}");
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region ---------- 扭力仪点检数据上传 ----------

        private TorqueSerialClient _serialTorqueClient1;
        private TorqueSerialClient _serialTorqueClient2;

        private void InitSerialTorqueSystem()
        {
            if (Global.Instance.CurDataBaseName == "上工装1") return;

            string portName1 = cmbCOM1.GetPropertySafely(c => c.Text);
            string portName2 = cmbCOM2.GetPropertySafely(c => c.Text);

            if (string.IsNullOrEmpty(portName1) && string.IsNullOrEmpty(portName2))
            {
                Log4netHelper.LogTorque("SERIAL_PORT_MISSING", "未选择或未检测到扭力仪串口，放弃初始化", level: "ERROR");
                Log4netHelper.LogDataException("TORQUE_SERIAL_PORT_MISSING", "未选择或未检测到扭力仪串口，放弃初始化");
                return;
            }

            #region 扭力仪1

            _serialTorqueClient1 = string.IsNullOrEmpty(portName1) ? null : new TorqueSerialClient(portName1);

            _serialTorqueClient1.OnConnectionStatusChanged += (isConnected, msg) =>
            {
                lblSerialLight1.ExecuteSafely(c => c.ForeColor = isConnected ? Color.Green : Color.Red);
            };

            _serialTorqueClient1.OnLog += (msg, isError) =>
            {
                if (isError)
                {
                    rtbErrorLog.AppendToComponent($"[扭力串口] {msg}");
                    Log4netHelper.LogTorque("SERIAL_ERROR", msg, level: "ERROR");
                    Log4netHelper.LogDataException("TORQUE_SERIAL_ERROR", msg);
                }
                else
                {
                    Log4netHelper.LogTorque("SERIAL_LOG", msg);
                }
            };

            _serialTorqueClient1.OnTorqueDataReceived += _serialTorqueClient_OnTorqueDataReceived;

            _serialTorqueClient1.Start();

            #endregion

            #region 扭力仪2

            if (Global.Instance.CurDataBaseName == "装配机")
            {
                _serialTorqueClient2 = string.IsNullOrEmpty(portName1) ? null : new TorqueSerialClient(portName2);

                _serialTorqueClient2.OnConnectionStatusChanged += (isConnected, msg) =>
                {
                    lblSerialLight2.ExecuteSafely(c => c.ForeColor = isConnected ? Color.Green : Color.Red);
                };

                _serialTorqueClient2.OnLog += (msg, isError) =>
                {
                    if (isError)
                    {
                        rtbErrorLog.AppendToComponent($"[扭力串口] {msg}");
                        Log4netHelper.LogTorque("SERIAL_ERROR", msg, level: "ERROR");
                        Log4netHelper.LogDataException("TORQUE_SERIAL_ERROR", msg);
                    }
                    else
                    {
                        Log4netHelper.LogTorque("SERIAL_LOG", msg);
                    }
                };

                _serialTorqueClient2.OnTorqueDataReceived += _serialTorqueClient_OnTorqueDataReceived;

                _serialTorqueClient2.Start();
            }

            #endregion
        }

        private void _serialTorqueClient_OnTorqueDataReceived(string torqueValue)
        {
            string url = Url_RealtimeArgs.GetPropertySafely(c => c.Text);

            var dataItem = new Data
            {
                Name = "点检扭力",
                Value = torqueValue,
                Standard = torqueValue,
                // 其它字段使用属性默认值
            };

            var inputParam = new DeviceProgramRealtimeArgsInputParam
            {
                ProgramName = Program?.Replace("\0", "").Trim() ?? string.Empty,
                SWVer = SWVer.GetPropertySafely(c => c.Text),
                User = txtUser.GetPropertySafely(c => c.Text),
                Datas = new ParamDatas { Data = new List<Data> { dataItem } }
            };

            var response = UniformInterface<DeviceProgramRealtimeArgsReturnParam, DeviceProgramRealtimeArgsInputParam>
                (url, "REPORTMACHINEREALTIMEPARAM", inputParam, "设备程序实时参数", "访问扭力仪数据参数接口失败");

            Invoke((Action)(() =>
            {
                if (response == null)
                {
                    string msg = "点检扭力上传失败，接口返回null";
                    rtbErrorLog.AppendToComponent(msg);
                    Log4netHelper.LogTorque("REALTIME_TORQUE_UPLOAD_NULL", msg, new Dictionary<string, object>
                    {
                        { "value", torqueValue }
                    }, level: "ERROR");
                    Log4netHelper.LogDataException("REALTIME_TORQUE_UPLOAD_NULL", msg, new Dictionary<string, object>
                    {
                        { "value", torqueValue }
                    });
                    lblRunningStatus.Text = "点检扭力上传失败，接口返回null";
                    lblRunningStatus.ForeColor = Color.Red;
                    return;
                }

                if (response.Result.Equals(nameof(MyEnum.Result.FAIL), StringComparison.OrdinalIgnoreCase))
                {
                    string msg = $"点检扭力上传失败:{response.ErrorMessage}";
                    rtbErrorLog.AppendToComponent(msg);
                    Log4netHelper.LogTorque("REALTIME_TORQUE_UPLOAD_FAIL", response.ErrorMessage, new Dictionary<string, object>
                    {
                        { "value", torqueValue },
                        { "result", response.Result }
                    }, level: "ERROR");
                    Log4netHelper.LogDataException("REALTIME_TORQUE_UPLOAD_FAIL", response.ErrorMessage, new Dictionary<string, object>
                    {
                        { "value", torqueValue },
                        { "result", response.Result }
                    });
                    lblRunningStatus.Text = "点检扭力上传失败";
                    lblRunningStatus.ForeColor = Color.Red;
                    return;
                }

                Log4netHelper.LogTorque("REALTIME_TORQUE_UPLOAD_PASS", "点检扭力上传成功", new Dictionary<string, object>
                {
                    { "value", torqueValue },
                    { "result", response.Result }
                });
                lblRunningStatus.Text = "点检扭力上传成功";
                lblRunningStatus.ForeColor = Color.Green;
            }));
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

            DataTable table1 = curDb.Find("select * from SytemSet where ID = '1'");

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

            DataTable MESDateTable = curDb.Find("select * from MesSetting where id = '1'");

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

            interfceAddr = curDb.Find("select * from interface");
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
            systemInfo.MesSaveResultTimeoutSeconds = NormalizeMesSaveResultTimeoutSeconds(systemInfo.MesSaveResultTimeoutSeconds);
            systemInfo.TorqueAckTimeoutSeconds = NormalizeTorqueAckTimeoutSeconds(systemInfo.TorqueAckTimeoutSeconds);
            HttpClientUtil.ConfigureSaveResultTimeoutSeconds(systemInfo.MesSaveResultTimeoutSeconds);

            // -------- 保存后生效 --------
            EnableReportMachineStatus.Checked = systemInfo.ReportMachineStatus; // 勾选启用设备状态上传
            EnableReportMachineAlarm.Checked = systemInfo.ReportMachineAlarm;   // 勾选启用预警信息上传
            EnableReportRealTimeParam.Checked = systemInfo.ReportRealTimeParam; // 勾选启用实时参数上传
            EnableReportConfigParam.Checked = systemInfo.ReportConfigParam;     // 勾选启用关键参数上传
            txtBarcodeRule.Text = systemInfo.BarcodeRule;                       // 条码规则
            HeartbeatUploadRate.Text = systemInfo.HeartRate;                    // 心跳上传频率
            RealtimeArgsUploadRate.Text = systemInfo.RealTimeParamRate;         // 实时参数上传频率
            txtMesSaveResultTimeoutSeconds.Text = systemInfo.MesSaveResultTimeoutSeconds; // MES过站接口超时时间
            txtTorqueAckTimeoutSeconds.Text = systemInfo.TorqueAckTimeoutSeconds; // PLC接收扭力ACK超时时间

            systemInfo.TorqueAckTimeoutMode = NormalizeTorqueAckTimeoutMode(systemInfo.TorqueAckTimeoutMode);
            cboTorqueAckTimeoutMode.Text = systemInfo.TorqueAckTimeoutMode;      // PLC接收扭力ACK超时处理方式
            cmbCOM1.Text = systemInfo.SerialPort1;
            cmbCOM2.Text = systemInfo.SerialPort2;
            txtControllerIP1.Text = systemInfo.ControllerIP1;
            txtControllerIP2.Text = systemInfo.ControllerIP2;
            txtControllerPort1.Text = systemInfo.ControllerPort1;
            txtControllerPort2.Text = systemInfo.ControllerPort2;

            // -------- 切换状态即生效 --------
            EnableGetNextBoard.Checked = systemInfo.EnablePanelizationFetch;    // 勾选启用获取拼版
            BanReadBarcode.Checked = systemInfo.BanBarcodeFetch;                // 勾选屏蔽条码读取
            EnablePrintCode.Checked = systemInfo.EnablePrint;                   // 勾选启用打印模板
            EnableFluentVerify.Checked = systemInfo.EnableRouteCheck;           // 勾选启用流程验证
            EnableUpperTooling.Checked = systemInfo.EnableFixtureMachine;       // 勾选启用上工装机程序
            EnableTypeChangedVerify.Checked = systemInfo.EnableModelVerify;     // 勾选启用型号切换校验
            EnableResultUpload.Checked = systemInfo.EnableDataUpload;           // 勾选启用上传结果
            EnableBarcodeRuleVerify.Checked = systemInfo.EnableBarcodeRuleVarify;// 勾选启用条码规则验证
            cboProductMode.Text = systemInfo.ProductMode;                       // 1.不显示NG且阻塞；2.显示NG且阻塞；3.显示NG且不阻塞
            cboEnforcePass.Text = systemInfo.EnforcePass;                       // 强制过站选项：1.All；2.None；3.Scan-ASSY；4.Weight；5.Screw-BA
            cboBanUpload.Text = systemInfo.BanUpload;                           // 屏蔽数据上传：1.All；2.None；3.Scan-ASSY；4.Weight；5.Screw-BA
            chkBanFixtureUpload.Checked = systemInfo.BanFixtureUpload;          // 屏蔽工装编号上传
        }

        /// <summary>
        /// 加载装配机界面配置信息
        /// </summary>
        private void Load_PrinterSet()
        {
            string sql = "select * from PrinterSet where id=1";
            DataTable source = curDb.Find(sql);
            if (source.Rows.Count > 0)
            {
                printerName.Text = source.Rows[0]["printer_name"].ToString();
                printTemplatePath.Text = source.Rows[0]["print_code_path"].ToString();
            }

            sql = "select * from BeforeProcess where id=1";
            source = curDb.Find(sql);
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
            source = curDb.Find(sql);
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

        private const int PlcReadFailureLogIntervalSeconds = 5;
        private readonly object _plcReadFailureLogLock = new object();
        private readonly Dictionary<string, System.DateTime> _plcReadFailureLogTimes = new Dictionary<string, System.DateTime>(StringComparer.Ordinal);

        /// <summary>
        /// 尝试读取PLC Int16值，并输出读取到的值，最多重试3次。
        /// <para> resultValue = -1表示读取失败 </para>
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryReadInt16Value(string address, out int value)
        {
            var hasTriedRead = false;

            for (int i = 0; i < 1; i++)
            {
                if (!isPlcConnected) continue;

                hasTriedRead = true;
                var result = _readWriteNet.ReadInt16(address);

                if (result.IsSuccess)
                {
                    value = result.Content;
                    return true;
                }

                // ErrorCode < 0 属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                {
                    if (ShouldLogPlcReadFailure("COMMUNICATION", address))
                    {
                        Log4netHelper.LogDataException("PLC_READ_VALUE_COMMUNICATION_ERROR", result.Message, new Dictionary<string, object>
                        {
                            { "address", address },
                            { "errorCode", result.ErrorCode }
                        });
                    }

                    value = -1;
                    return false;
                }

                Thread.Sleep(50);
            }

            // 循环3次后仍然失败
            value = -1;
            if (hasTriedRead && ShouldLogPlcReadFailure("READ_FAILED", address))
            {
                Log4netHelper.LogDataException("PLC_READ_VALUE_FAILED", "PLC读取Int16失败", new Dictionary<string, object>
                {
                    { "address", address }
                });
            }
            return false;
        }

        private bool ShouldLogPlcReadFailure(string category, string address)
        {
            string key = category + "|" + (address ?? string.Empty);
            System.DateTime now = System.DateTime.UtcNow;

            lock (_plcReadFailureLogLock)
            {
                if (_plcReadFailureLogTimes.TryGetValue(key, out System.DateTime lastLogTime)
                    && (now - lastLogTime).TotalSeconds < PlcReadFailureLogIntervalSeconds)
                {
                    return false;
                }

                _plcReadFailureLogTimes[key] = now;
                return true;
            }
        }

        /// <summary>
        /// 尝试读取PLC String值，清理字符串中的空白后输出读取到的值，最多重试3次（内部调用CodeNum.CleanString清理字符串空白）。
        /// <para> resultValue = null表示读取失败 </para>
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="length">读取字符串的长度</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryReadStringValue(string address, dynamic length, out string value)
        {
            var hasTriedRead = false;

            for (int i = 0; i < 3; i++)
            {
                if (!isPlcConnected) continue;

                hasTriedRead = true;
                var result = _readWriteNet.ReadString(address, Convert.ToUInt16(length));

                if (result.IsSuccess)
                {
                    value = CodeNum.CleanString(result.Content);
                    return true;
                }

                // ErrorCode < 0属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                {
                    Log4netHelper.LogDataException("PLC_READ_STRING_COMMUNICATION_ERROR", result.Message, new Dictionary<string, object>
                    {
                        { "address", address },
                        { "length", length },
                        { "errorCode", result.ErrorCode }
                    });
                    value = null;
                    return false;
                }

                Thread.Sleep(50);
            }

            // 循环3次后仍然失败
            value = null;
            if (hasTriedRead)
            {
                Log4netHelper.LogDataException("PLC_READ_STRING_FAILED", "PLC读取字符串失败", new Dictionary<string, object>
                {
                    { "address", address },
                    { "length", length }
                });
            }
            return false;
        }

        /// <summary>
        /// 尝试写入PLC Int16值。
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryWriteInt16Value(string address, dynamic value)
        {
            if (_readWriteNet is null)
            {
                Log4netHelper.LogDataException("PLC_WRITE_VALUE_NO_CLIENT", "PLC连接对象为空", new Dictionary<string, object>
                {
                    { "address", address },
                    { "value", value }
                });
                return false;
            }

            var result = _readWriteNet.Write(address, Convert.ToInt16(value));

            if (result.IsSuccess)
            {
                return true;
            }

            Log4netHelper.LogDataException("PLC_WRITE_VALUE_FAILED", result.Message, new Dictionary<string, object>
            {
                { "address", address },
                { "value", value },
                { "errorCode", result.ErrorCode }
            });
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
            var hasTriedRead = false;

            for (int i = 0; i < 3; i++)
            {
                if (!isPlcConnected) continue;

                hasTriedRead = true;
                var result = await _readWriteNet.ReadInt32Async(address);

                if (result.IsSuccess)
                    return (true, result.Content);

                // ErrorCode < 0 属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                {
                    Log4netHelper.LogDataException("PLC_READ_INT32_ASYNC_COMMUNICATION_ERROR", result.Message, new Dictionary<string, object>
                    {
                        { "address", address },
                        { "errorCode", result.ErrorCode }
                    });
                    return (false, -1);
                }

                await Task.Delay(50);
            }

            // 循环3次后仍然失败
            if (hasTriedRead)
            {
                Log4netHelper.LogDataException("PLC_READ_INT32_ASYNC_FAILED", "PLC异步读取Int32失败", new Dictionary<string, object>
                {
                    { "address", address }
                });
            }
            return (false, -1);
        }

        /// <summary>
        /// 尝试异步读取 Int16 值，带 500ms 超时保护和最多3次重试。
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <returns>
        /// 一个表示异步操作的任务。
        /// 任务结果是一个元组 (bool isReadOk, short resultValue):
        /// <list processName="bullet">
        /// <item><description><c>isReadOk</c>: <c>true</c> 表示读取成功, <c>false</c> 表示3次尝试后依旧失败（包括超时）。</description></item>
        /// <item><description><c>resultValue</c>: 读取成功时的值。如果 <c>isReadOk</c> 为 <c>false</c>，则返回 -1。</description></item>
        /// </list>
        /// </returns>
        private async Task<(bool, short)> TryReadInt16Async(string address)
        {
            var hasTriedRead = false;

            for (var i = 0; i < 3; i++)
            {
                if (!isPlcConnected || _readWriteNet == null)
                {
                    await Task.Delay(100);
                    continue;
                }

                hasTriedRead = true;
                var readTask = _readWriteNet.ReadInt16Async(address);
                var completedTask = await Task.WhenAny(readTask, Task.Delay(500));

                if (readTask != completedTask)
                {
                    Log4netHelper.LogDataException("PLC_READ_INT16_ASYNC_TIMEOUT", "PLC异步读取Int16超时", new Dictionary<string, object>
                    {
                        { "address", address }
                    });
                    await Task.Delay(50);
                    continue;
                }

                var result = await readTask;
                if (result.IsSuccess && result.ErrorCode >= 0) return (true, result.Content);
                Log4netHelper.LogDataException("PLC_READ_INT16_ASYNC_FAILED", result.Message, new Dictionary<string, object>
                {
                    { "address", address },
                    { "errorCode", result.ErrorCode }
                });

                await Task.Delay(100);
            }

            // 循环3次后仍然失败
            if (hasTriedRead)
            {
                Log4netHelper.LogDataException("PLC_READ_INT16_ASYNC_RETRY_EXHAUSTED", "PLC异步读取Int16重试失败", new Dictionary<string, object>
                {
                    { "address", address }
                });
            }
            return (false, -1);
        }

        private async Task<bool> TryWriteInt16ValueAsync(string address, short value)
        {
            if (!isPlcConnected)
            {
                Log4netHelper.LogDataException("PLC_WRITE_ASYNC_DISCONNECTED", "PLC未连接，无法异步写入", new Dictionary<string, object>
                {
                    { "address", address },
                    { "value", value }
                });
                return false;
            }

            var writeTask = _readWriteNet.WriteAsync(address, value);
            var completedTask = await Task.WhenAny(writeTask, Task.Delay(500));

            // 超时
            if (writeTask != completedTask)
            {
                Log4netHelper.LogDataException("PLC_WRITE_ASYNC_TIMEOUT", "PLC异步写入超时", new Dictionary<string, object>
                {
                    { "address", address },
                    { "value", value }
                });
                return false;
            }

            var writeResult = await writeTask;

            if (writeResult.IsSuccess)
            {
                return true;
            }

            Log4netHelper.LogDataException("PLC_WRITE_ASYNC_FAILED", writeResult.Message, new Dictionary<string, object>
            {
                { "address", address },
                { "value", value },
                { "errorCode", writeResult.ErrorCode }
            });
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
            string content = label.GetPropertySafely(c => c.Text);
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

            DataTable table1 = curDb.Find("select * from SytemSet where ID = '1'");
            if (table1.Rows.Count > 0)
            {
                string sql = "update SytemSet set IP='" + PlcIP.Text + "',PortCode='" + PlcPort.Text + "'" +
                              ",DeviceName='" + DeviceName.Text + "'" +
                              ",PlcType='" + PlcConnectType.Text + "'" +
                              " where id = '1'";
                var result = curDb.Change(sql);
                if (result && alterSuccessTip)
                {
                    MessageBox.Show("保存成功");
                }
            }
            else
            {
                string sql = "insert into SytemSet(id,DeviceName,IP,PortCode,PlcType)value(1,'" + DeviceName.Text + "','" + PlcIP.Text + "','" + PlcPort.Text + "','" + PlcConnectType.Text + "')";
                var result = curDb.Change(sql);
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
                txtUser.Text = row["Operator"].ToString();  // 操作员
                OrderNum.Text = row["OrderQty"].ToString(); // 工单数量
                OrderNo.Text = row["WorkNo"].ToString();    // 工单号
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
            boardTable = curDb.Find("SELECT * FROM Board WHERE IsActive = True");

            if (boardTable == null || boardTable.Rows.Count <= 0) return;

            id = boardTable.AsEnumerable().Select(row => row["id"].ToString()).ToArray();
            stationIdArray = boardTable.AsEnumerable().Select(row => row["WorkID"].ToString()).ToArray();
            testNameArray = boardTable.AsEnumerable().Select(row => row["BoardName"].ToString()).ToArray();
            standardValuePointArray = boardTable.AsEnumerable().Select(row => row["StandardCode"].ToString()).ToArray();
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
            if (id?.Length <= 0) return columns;

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
                // 按实际工序名称分别计数，避免 Scan_ASSY、Weight 等页签共用同一个序号。
                string processCounterKey = uploadManagerEntity.Name.ToString();

                // 【序号管理】初始化或获取当前工序的计数器
                if (!_processCounters.ContainsKey(processCounterKey))
                {
                    _processCounters[processCounterKey] = 0;
                }

                // 【行数限制】
                if (gridView.RowCount > 500)
                {
                    gridView.Rows.RemoveAt(gridView.Rows.Count - 1);
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
                    _processCounters[processCounterKey]++;
                    int currentNum = _processCounters[processCounterKey];

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
                            if (valList != null && valList.Count > j && !string.IsNullOrEmpty(valList[j]) && valList[j] != "null")
                            {
                                dgvRow.Cells[cellIndex].Value = valList[j];
                                cellIndex++;
                            }

                            // 2. 上限
                            if (maxList != null && maxList.Count > j && !string.IsNullOrEmpty(maxList[j]) && maxList[j] != "null")
                            {
                                dgvRow.Cells[cellIndex].Value = maxList[j];
                                cellIndex++;
                            }

                            // 3. 下限
                            if (minList != null && minList.Count > j && !string.IsNullOrEmpty(minList[j]) && minList[j] != "null")
                            {
                                dgvRow.Cells[cellIndex].Value = minList[j];
                                cellIndex++;
                            }

                            // 4. 结果
                            if (resList.Count > j && !string.IsNullOrEmpty(resList[j]) && resList[j] != "null")
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
        /// 当 MES 返回 null 时，结合 HttpClientUtil.LastRequestFailure 给出具体的失败原因，
        /// 避免现场只看到笼统的"接口返回空"。若最近无请求失败记录则退回默认提示。
        /// </summary>
        private string BuildMesFailReason(string defaultReason)
        {
            var failure = HttpClientUtil.LastRequestFailure;
            return failure == null ? defaultReason : $"返回空 [{failure.ToDisplayString()}]";
        }

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
                    rtbErrorLog.AppendToComponent(logString);
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
            return UniformInterface<DeviceProgramRealtimeArgsReturnParam, DeviceProgramRealtimeArgsInputParam>(Url_RealtimeArgs.Text, "REPORTMACHINEREALTIMEPARAM", inputParam, logFile, showString);
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
        private const int ErrorQueueMaxCount = 100;          // 限制异常风暴期间的待处理报警数量
        private bool isBlockingMode = true;                 //  默认为阻塞模式
        private volatile bool existErrorInErrorTip;         // 全局阻塞锁：当前已经有错误在显示，为True时所有调用当前字段的方法都被暂停
        private ErrorEntity _currentActiveError;            // 当前处理的错误对象
        private readonly Queue<ErrorEntity> ErrorQueue = new Queue<ErrorEntity>();   // 错误队列
        private volatile bool _manualClearInProgress;

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
        private bool HandleError(string feedbackAddress, short? feedBackValue = null, bool isBlockingError = false, string userMessage = null, string logMessage = null)
        {
            var errorData = new ErrorEntity()
            {
                FeedBackAddress = feedbackAddress,
                FeedbackValue = Convert.ToInt16(feedBackValue),
                IsBlockingError = isBlockingError,
                UserMessage = userMessage ?? string.Empty,
                LogMessage = logMessage ?? userMessage,
                timeStamp = System.DateTime.Now
            };

            bool shouldShow = false;
            lock (_errorLock)
            {
                if (IsSameError(_currentActiveError, errorData) || ErrorQueue.Any(item => IsSameError(item, errorData)))
                    return false;

                if (existErrorInErrorTip || _currentActiveError != null)
                {
                    if (ErrorQueue.Count >= ErrorQueueMaxCount)
                        return false;

                    ErrorQueue.Enqueue(errorData);
                }
                else
                {
                    _currentActiveError = errorData;
                    existErrorInErrorTip = isBlockingMode && errorData.IsBlockingError;
                    shouldShow = true;
                }
            }

            string currentTraceId = ProductPassTraceContext.CurrentTraceId;
            if (!string.IsNullOrWhiteSpace(currentTraceId))
            {
                Log4netHelper.LogDataException("HANDLE_ERROR",
                    $"进入错误处理，反馈地址={feedbackAddress}，反馈值={feedBackValue}，阻塞={isBlockingError}，消息={errorData.LogMessage}");
            }

            // 锁内只切换报警状态，避免后台线程持锁同步等待 UI。
            if (shouldShow)
                ShowErrorToUi(errorData);

            return false;
        }

        private static bool IsSameError(ErrorEntity first, ErrorEntity second)
        {
            if (first == null || second == null)
                return false;

            return string.Equals(first.FeedBackAddress, second.FeedBackAddress, StringComparison.Ordinal)
                && first.FeedbackValue == second.FeedbackValue
                && first.IsBlockingError == second.IsBlockingError
                && string.Equals(first.UserMessage, second.UserMessage, StringComparison.Ordinal);
        }

        /// <summary>
        /// 展示错误到UI界面
        /// </summary>
        /// <param name="errorData"></param>
        private void ShowErrorToUi(ErrorEntity errorData)
        {
            if (errorData == null || IsDisposed)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke((MethodInvoker)(() => ShowErrorToUi(errorData)));
                }
                catch (InvalidOperationException)
                {
                }

                return;
            }

            lock (_errorLock)
            {
                if (!ReferenceEquals(_currentActiveError, errorData))
                    return;
            }

            rtbErrorLog.AppendToComponent(errorData.LogMessage);
            lblStatusErrorTip.Text = errorData.UserMessage;
            lblStatusErrorTip.ForeColor = Color.Red;

            if (isBlockingMode && errorData.IsBlockingError)
            {
                btnManualClear.Visible = true;
                btnManualClear.Enabled = true;

                Log4netHelper.LogDataException("BLOCKING_ERROR", errorData.LogMessage, new Dictionary<string, object>
                {
                    { "feedback", errorData.FeedBackAddress },
                    { "value", errorData.FeedbackValue },
                    { "blocking", true },
                    { "userMessage", errorData.UserMessage }
                });
            }
            else
            {
                Log4netHelper.LogDataException("NON_BLOCKING_ERROR", errorData.LogMessage, new Dictionary<string, object>
                {
                    { "feedback", errorData.FeedBackAddress },
                    { "value", errorData.FeedbackValue },
                    { "blocking", false },
                    { "userMessage", errorData.UserMessage }
                });

                Task.Run(() => CompleteNonBlockingErrorAsync(errorData));
            }
        }

        private async Task CompleteNonBlockingErrorAsync(ErrorEntity errorData)
        {
            try
            {
                if (!string.IsNullOrEmpty(errorData.FeedBackAddress) && isPlcConnected)
                {
                    var result = await _readWriteNet.WriteAsync(errorData.FeedBackAddress, Convert.ToInt16(errorData.FeedbackValue));
                    if (!result.IsSuccess)
                    {
                        Log4netHelper.LogDataException("NON_BLOCKING_FEEDBACK_FAILED",
                            $"写入PLC地址 {errorData.FeedBackAddress} 失败：{result.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.LogDataException("NON_BLOCKING_FEEDBACK_EXCEPTION",
                    $"写入PLC地址 {errorData.FeedBackAddress} 异常：{ex}");
            }
            finally
            {
                ClearCurrentErrorAndCheckQueue(errorData);
            }
        }

        private async void ManualClear_Click(object sender, EventArgs e)
        {
            if (_manualClearInProgress)
                return;

            ErrorEntity currentError;
            lock (_errorLock)
            {
                currentError = _currentActiveError;
            }

            if (currentError == null)
                return;

            _manualClearInProgress = true;
            btnManualClear.Enabled = false;

            try
            {
                string feedbackAddress = currentError.FeedBackAddress;
                short feedbackValue = Convert.ToInt16(currentError.FeedbackValue);

                if (!string.IsNullOrEmpty(feedbackAddress))
                {
                    if (!isPlcConnected)
                    {
                        MessageBox.Show("无法清除错误：PLC当前未连接，请先检查网络通讯！", "通讯异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (_readWriteNet == null)
                    {
                        MessageBox.Show("无法清除错误：PLC通讯对象尚未初始化。", "通讯异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 异步等待不会阻塞 UI；由 PLC 通讯层自身超时，避免本地超时后残留并发写任务。
                    var result = await _readWriteNet.WriteAsync(feedbackAddress, feedbackValue);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show($"清除失败：写入PLC地址 {feedbackAddress} 失败。\r\n错误码: {result.ErrorCode}\r\n原因: {result.Message}", "复位失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                Log4netHelper.LogDataException("MANUAL_CLEAR_DONE", "手动清除报警完成", new Dictionary<string, object>
                {
                    { "feedback", currentError.FeedBackAddress },
                    { "value", currentError.FeedbackValue },
                    { "userMessage", currentError.UserMessage }
                }, level: "INFO");

                ClearCurrentErrorAndCheckQueue(currentError);
            }
            catch (Exception ex)
            {
                Log4netHelper.LogDataException("MANUAL_CLEAR_EXCEPTION", "手动清除报警异常", new Dictionary<string, object>
                {
                    { "feedback", currentError.FeedBackAddress },
                    { "value", currentError.FeedbackValue },
                    { "exception", ex.ToString() }
                });

                MessageBox.Show($"清除报警时发生异常：{ex.Message}", "复位失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _manualClearInProgress = false;
                if (!btnManualClear.IsDisposed)
                    btnManualClear.Enabled = true;
            }
        }

        private void ClearCurrentErrorAndCheckQueue(ErrorEntity expectedError = null)
        {
            ErrorEntity nextError = null;

            lock (_errorLock)
            {
                if (expectedError != null && !ReferenceEquals(_currentActiveError, expectedError))
                    return;

                _currentActiveError = null;

                if (ErrorQueue.Count > 0)
                {
                    nextError = ErrorQueue.Dequeue();
                    _currentActiveError = nextError;
                    existErrorInErrorTip = isBlockingMode && nextError.IsBlockingError;
                }
                else
                {
                    existErrorInErrorTip = false;
                }
            }

            RefreshErrorUi(nextError);
        }

        private void RefreshErrorUi(ErrorEntity nextError)
        {
            if (IsDisposed)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke((MethodInvoker)(() => RefreshErrorUi(nextError)));
                }
                catch (InvalidOperationException)
                {
                }

                return;
            }

            lock (_errorLock)
            {
                if (nextError == null)
                {
                    if (_currentActiveError != null)
                        return;
                }
                else if (!ReferenceEquals(_currentActiveError, nextError))
                {
                    return;
                }
            }

            ResetErrorUi();
            if (nextError != null)
                ShowErrorToUi(nextError);
        }

        private void ResetErrorUi()
        {
            lblStatusErrorTip.Text = string.Empty;
            lblRunningStatus.Text = string.Empty;
            barCode.Text = string.Empty;
            ToolingNumber.Text = string.Empty;
            btnManualClear.Visible = false;
        }

        #endregion

        #region ---------- 读取PLC数值并按规则计算 ----------

        /// <summary>
        /// 解析配置字符串并从 PLC 读取处理后的数据
        /// </summary>
        /// <param name="configString">配置字符串，格式："[PLC地址]:[数据类型]-[计算规则]"</param>
        /// <param name="resultValue">
        /// <para>成功时：返回计算后的数值 (dynamic/object)</para>
        /// <para>失败时：返回错误信息字符串 (string)</para>
        /// <para>空配置时：返回字符串 "null"</para>
        /// </param>
        /// <returns>读取并处理成功返回 true，否则返回 false</returns>
        private bool TryGetProcessedValue(string configString, out dynamic resultValue)
        {
            // 1.基础校验：如果地址为空，返回 true，out 字符串 "null"
            if (string.IsNullOrWhiteSpace(configString))
            {
                resultValue = "null";
                return true;
            }

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
                bool isReadSuccess;         // 读取结果
                dynamic rawContent = null;  // 读取到的原始数值
                string failReason = "";     // 失败的具体原因

                // 4. 执行读取
                switch (dataType)
                {
                    case "H": // Int16
                        var shortRes = _readWriteNet.ReadInt16(plcAddress);
                        isReadSuccess = shortRes.IsSuccess;
                        if (isReadSuccess) rawContent = shortRes.Content;
                        else failReason = shortRes.Message;
                        break;

                    case "I": // Int32
                        var intRes = _readWriteNet.ReadInt32(plcAddress);
                        isReadSuccess = intRes.IsSuccess;
                        if (isReadSuccess) rawContent = intRes.Content;
                        else failReason = intRes.Message;
                        break;

                    case "F": // Float
                        var floatRes = _readWriteNet.ReadFloat(plcAddress);
                        isReadSuccess = floatRes.IsSuccess;
                        if (isReadSuccess) rawContent = floatRes.Content;
                        else failReason = floatRes.Message;
                        break;

                    default:  // 默认按 Int16 处理
                        var defRes = _readWriteNet.ReadInt16(plcAddress);
                        isReadSuccess = defRes.IsSuccess;
                        if (isReadSuccess) rawContent = defRes.Content;
                        else failReason = $"未知类型 {dataType}，默认读取失败: {defRes.Message}";
                        break;
                }

                // 5.根据读取结果处理返回值
                if (isReadSuccess)
                {
                    resultValue = CalculateValue(rawContent, computeRule);
                    return true;
                }

                resultValue = $"读取测试数据失败[{plcAddress}]: {failReason}";
                return false;
            }
            catch (Exception ex)
            {
                // 捕获解析错误或其他未预料异常
                resultValue = $"程序异常: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// 根据规则对原始数据进行计算转换
        /// <para>0：实际值</para>
        /// <para>1:÷10</para>
        /// <para>2:÷100</para>
        /// <para>3:÷1000</para>
        /// <para>4:状态判断（3=OK，其余均为NG）</para>
        /// </summary>
        /// <param name="rawValue">PLC 读取的原始值（可能是 short, int, float）</param>
        /// <param name="ruleCode">计算规则代码 (0-4)</param>
        /// <returns>计算后的值</returns>
        private dynamic CalculateValue(dynamic rawValue, string ruleCode)
        {
            // 预处理：将原始值转为 double 以防止整数除法精度丢失
            bool isNumber = double.TryParse(rawValue.ToString(), out double val);

            switch (ruleCode)
            {
                case "0": // 原值
                    return rawValue;

                case "1": // ÷ 10
                    return isNumber ? $"{val / 10.0:F2}" : "0";

                case "2": // ÷ 100
                    return isNumber ? $"{val / 100.0:F2}" : "0";

                case "3": // ÷ 1000
                    return isNumber ? $"{val / 1000.0:F2}" : "0";

                case "4": // 状态判断 (OK/NG)
                    return Convert.ToInt32(rawValue) == 3 ? "OK" : "NG";

                default: // 默认原值
                    return rawValue;
            }
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

        #region --------- Events Handler ---------

        /// <summary>
        /// UI 状态更新定时器（MES接口状态）
        /// </summary>
        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            InterfaceSignalLight.ForeColor = isDeviceAlive ? Color.Green : Color.Red;
        }

        /// <summary>
        /// mes配置区域 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAtMesConfig_Click(object sender, EventArgs e)
        {
            DataTable table1 = curDb.Find("select * from MesSetting where ID = '1'");
            if (table1.Rows.Count > 0)
            {
                string sql = "update [MesSetting] set [url]='" + url.Text + "',[Line]='" + Line.Text + "',[Process]='" + Process.Text + "',[Station]='" + Station.Text + "'" +
                              ",[MesKey]='" + MesKey.Text + "',[Security]='" + Security.Text + "',[Device]='" + Device.Text + "'" +
                              ",[PlanNo]='" + PlanNo.Text + "',[FTPlog]='" + FTPlog.Text + "',[FTPPIC]='" + FTPPIC.Text + "',[FTPID] = '" + FTPID.Text +
                              "',[FTPCODE] = '" + FTPCODE.Text + "',[SWVer] = '" + SWVer.Text + "',[HWVer] = '" + HWVer.Text + "'" +
                              " where [id] = '1'";
                var result = curDb.Change(sql);
                curDb.Del("delete from [interface]");
                int i = curDb.DatatableToMdb("interface", InsertTable());
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
                var result = curDb.Change(sql);

                bool rl = curDb.Del("delete from [interface]");
                int i = curDb.DatatableToMdb("interface", InsertTable());
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
        private void SaveAtProductConfig_Click(object sender, EventArgs e)
        {
            string mesTimeoutSeconds = NormalizeMesSaveResultTimeoutSeconds(txtMesSaveResultTimeoutSeconds.Text);
            string torqueAckTimeoutSeconds = NormalizeTorqueAckTimeoutSeconds(txtTorqueAckTimeoutSeconds.Text);
            txtMesSaveResultTimeoutSeconds.Text = mesTimeoutSeconds;
            txtTorqueAckTimeoutSeconds.Text = torqueAckTimeoutSeconds;

            // -------- 保存后生效 --------
            systemInfo.ReportMachineStatus = EnableReportMachineStatus.Checked; // 勾选启用设备状态上传
            systemInfo.ReportMachineAlarm = EnableReportMachineAlarm.Checked;   // 勾选启用预警信息上传
            systemInfo.ReportRealTimeParam = EnableReportRealTimeParam.Checked; // 勾选启用实时参数上传
            systemInfo.ReportConfigParam = EnableReportConfigParam.Checked;     // 勾选启用关键参数上传
            systemInfo.BarcodeRule = txtBarcodeRule.Text;                       // 条码规则
            systemInfo.HeartRate = HeartbeatUploadRate.Text;                    // 心跳上传频率
            systemInfo.RealTimeParamRate = RealtimeArgsUploadRate.Text;         // 实时参数上传频率
            systemInfo.MesSaveResultTimeoutSeconds = NormalizeMesSaveResultTimeoutSeconds(mesTimeoutSeconds); // MES过站接口超时时间
            systemInfo.TorqueAckTimeoutSeconds = NormalizeTorqueAckTimeoutSeconds(torqueAckTimeoutSeconds); // PLC接收扭力ACK超时时间

            systemInfo.TorqueAckTimeoutMode = NormalizeTorqueAckTimeoutMode(cboTorqueAckTimeoutMode.Text);
            cboTorqueAckTimeoutMode.Text = systemInfo.TorqueAckTimeoutMode;
            systemInfo.SerialPort1 = cmbCOM1.Text;
            systemInfo.SerialPort2 = cmbCOM2.Text;
            systemInfo.ControllerIP1 = txtControllerIP1.Text;
            systemInfo.ControllerIP2 = txtControllerIP2.Text;
            systemInfo.ControllerPort1 = txtControllerPort1.Text;
            systemInfo.ControllerPort2 = txtControllerPort2.Text;

            // -------- 切换状态即生效 --------
            systemInfo.EnablePanelizationFetch = EnableGetNextBoard.Checked;    // 勾选启用获取拼版
            systemInfo.BanBarcodeFetch = BanReadBarcode.Checked;                // 勾选屏蔽条码读取
            systemInfo.EnablePrint = EnablePrintCode.Checked;                   // 勾选启用打印模板
            systemInfo.EnableRouteCheck = EnableFluentVerify.Checked;           // 勾选启用流程验证
            systemInfo.EnableFixtureMachine = EnableUpperTooling.Checked;       // 勾选启用上工装机程序
            systemInfo.EnableModelVerify = EnableTypeChangedVerify.Checked;     // 勾选启用型号切换校验
            systemInfo.EnableDataUpload = EnableResultUpload.Checked;           // 勾选启用上传结果
            systemInfo.EnableBarcodeRuleVarify = EnableBarcodeRuleVerify.Checked;// 勾选启用条码规则验证
            systemInfo.ProductMode = cboProductMode.Text;                       // 1.不显示NG且阻塞；2.显示NG且阻塞；3.显示NG且不阻塞
            systemInfo.EnforcePass = cboEnforcePass.Text;                       // 强制过站选项：1.All；2.None；3.Scan-ASSY；4.Weight；5.Screw-BA
            systemInfo.BanUpload = cboBanUpload.Text;                           // 屏蔽数据上传：1.All；2.None；3.Scan-ASSY；4.Weight；5.Screw-BA
            systemInfo.BanFixtureUpload = chkBanFixtureUpload.Checked;          // 屏蔽工装编号上传

            if (systemInfo.Save())
            {
                HttpClientUtil.ConfigureSaveResultTimeoutSeconds(systemInfo.MesSaveResultTimeoutSeconds);
                SaveSuccessRestartApp();
            }
            else
            {

                MessageBox.Show("保存失败");
                Load_ProductConfig();
            }
        }

        /// <summary>
        /// 规范化 MES 过站接口超时时间。
        /// <para>用户输入为空或非法时使用默认 30 秒，并限制在 5-300 秒之间。</para>
        /// </summary>
        private static string NormalizeMesSaveResultTimeoutSeconds(string timeoutSecondsText)
        {
            if (!int.TryParse(timeoutSecondsText, out int timeoutSeconds))
                timeoutSeconds = int.Parse(DefaultMesSaveResultTimeoutSeconds);

            if (timeoutSeconds < MinMesSaveResultTimeoutSeconds)
                timeoutSeconds = MinMesSaveResultTimeoutSeconds;
            else if (timeoutSeconds > MaxMesSaveResultTimeoutSeconds)
                timeoutSeconds = MaxMesSaveResultTimeoutSeconds;

            return timeoutSeconds.ToString();
        }

        /// <summary>
        /// 规范化 PLC 接收扭力 ACK 超时时间。
        /// <para>用户输入为空或非法时使用默认 3 秒，并限制在 1-60 秒之间。</para>
        /// </summary>
        private static string NormalizeTorqueAckTimeoutSeconds(string timeoutSecondsText)
        {
            if (!int.TryParse(timeoutSecondsText, out int timeoutSeconds))
                timeoutSeconds = int.Parse(DefaultTorqueAckTimeoutSeconds);

            if (timeoutSeconds < MinTorqueAckTimeoutSeconds)
                timeoutSeconds = MinTorqueAckTimeoutSeconds;
            else if (timeoutSeconds > MaxTorqueAckTimeoutSeconds)
                timeoutSeconds = MaxTorqueAckTimeoutSeconds;

            return timeoutSeconds.ToString();
        }

        /// <summary>
        /// 规范化 PLC 接收扭力 ACK 超时处理模式。
        /// <para>旧数据库中的“报警并等待ACK/后台等待ACK”统一升级为“超时清Req并报警”。</para>
        /// </summary>
        private static string NormalizeTorqueAckTimeoutMode(string mode)
        {
            return TorqueAckTimeoutModeResetAndAlarm;
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
        }

        private void SaveAtPrintSet_Click(object sender, EventArgs e)
        {
            string sql = "update PrinterSet set " +
                         $"printer_name='{printerName.Text}'," +
                         $"print_code_path='{printTemplatePath.Text}'" +
                         " where id=1";

            MessageBox.Show(curDb.Change(sql) ? "保存成功" : "保存失败");
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
            curDb.Del("delete from KeyArgsPreserve");

            string sql = "select * from Board";
            DataTable dt = curDb.Find(sql);

            foreach (DataRow row in dt.Rows)
            {
                sql = "insert into KeyArgsPreserve(name,standard,USL,LSL,unit) values(" +
                    $"'{row["BoardName"]}'," +
                    $"'{row["StandardCode"]}'," +
                    $"'{row["MaxBoardCode"]}'," +
                    $"'{row["MinBoardCode"]}'," +
                    $"'{row["BoardA1"]}'" +
                    ")";
                curDb.Add(sql);
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

            DataTable table1 = curDb.Find("select * from BeforeProcess where ID = 1");
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

            MessageBox.Show(curDb.Change(sql) ? "保存成功" : "保存失败");
        }

        private void Save_Process3_Click(object sender, EventArgs e)
        {
            if (Line3.Text == String.Empty || Process3.Text == String.Empty || Station3.Text == String.Empty || MesKey3.Text == String.Empty ||
                Security3.Text == String.Empty || Device3.Text == String.Empty)
            {
                MessageBox.Show("当前界面内容均为必填项、请先填写完善");
                return;
            }

            DataTable table1 = curDb.Find("select * from Process3 where ID = 1");
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

            MessageBox.Show(curDb.Change(sql) ? "保存成功" : "保存失败");
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

            // 反馈PLC，允许手动输入条码
            TryWriteInt16Value(addrInfo.ManualInputBarcodeTip, 0);

            btnManualInputBarcode.Visible = false;
            ManualInputBarcode inputBarcodeWindow = new ManualInputBarcode(_readWriteNet, addrInfo);
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
                curDb.Del("delete from ChangeProductType");
                curDb.Change("ALTER TABLE ChangeProductType ALTER COLUMN id COUNTER (1, 1)");
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
                if (!curDb.Add(sql))
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
                _readWriteNet.Write(addrInfo.RecoverySignal, 1);
            }
        }

        private void printTest_Click(object sender, EventArgs e)
        {
            // 1. 获取界面参数
            string fileName = printTemplatePath.Text;
            string printerName = this.printerName.Text.Trim();

            if (!File.Exists(fileName))
            {
                MessageBox.Show("模板文件不存在");
            }

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

                    csApp = new LabelManager2.ApplicationClass { Visible = false };

                    // 打开模板 (ReadOnly = true)
                    csApp.Documents.Open(fileName, true);
                    doc = csApp.ActiveDocument;
                    doc.Printer.SwitchTo(printerName);
                }
                catch (Exception)
                {
                    csApp = null; // 置空以触发下次重试
                    Thread.Sleep(3000);
                }
            }

            // 禁用按钮，防止重复点击
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            try
            {
                // 5. 执行打印
                // 参数1：打印份数
                doc.PrintDocument();

                // 7. 打印成功通知（切回UI线程）
                Invoke(new Action(() =>
                {
                    MessageBox.Show("打印指令已发送！");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"后台打印发生异常:\r\n{ex.Message}");
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

            DataTable result = sourceDb.Find($"select * from userinfo where work_id='{UId.Text}'");
            if (result.Rows.Count != 0)
            {
                MessageBox.Show("工号已存在");
                return;
            }

            bool isSuccess = sourceDb.Add($"insert into userinfo(work_id,pwd,privilege) values('{UId.Text}','{UPwd.Text}','{Priv.Text}')");
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
            //System.Diagnostics.ProcessName.Start(System.Windows.Forms.Application.ExecutablePath);

            //// 终止当前进程  
            //System.Diagnostics.ProcessName.GetCurrentProcess().Kill();

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

        /// <summary>
        /// 生产配置界面 > 地址维护 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAtAddressMatain_Click(object sender, EventArgs e)
        {
            // ---------------- 条码验证 ----------------
            addrInfo.HasBarcodeTag = txtHasBarcodeTag.Text;
            addrInfo.BarcodeVerifyTag = txtBarcodeVerifyTag.Text;
            addrInfo.BarcodeType = txtBarcodeType.Text;

            addrInfo.PlcScannedBarcode = txtPlcScanned.Text;
            addrInfo.PlcScannedBarcodeLength = txtScannedLength.Text;
            addrInfo.PanalizationBarcode = txtPanalizationBarcode.Text;
            addrInfo.PanalizationBarcodeLength = txtPanalizationLength.Text;

            addrInfo.ManualInputBarcodeTip = txtManualInput.Text;
            addrInfo.ManualInputBarcode = txtManualBarcode.Text;
            addrInfo.ManualInputBarcodeLength = txtManualLength.Text;

            // ---------------- 数据上传 ----------------
            addrInfo.TriggerUpload1 = txtTriggerUpload1.Text;
            addrInfo.TriggerUpload2 = txtTriggerUpload2.Text;
            addrInfo.TriggerUpload3 = txtTriggerUpload3.Text;

            addrInfo.Feedback1 = txtFeedback1.Text;
            addrInfo.Feedback2 = txtFeedback2.Text;
            addrInfo.Feedback3 = txtFeedback3.Text;

            addrInfo.ProductResult1 = txtProductResult1.Text;
            addrInfo.ProductResult2 = txtProductResult2.Text;
            addrInfo.ProductResult3 = txtProductResult3.Text;

            addrInfo.BarcodeToUpload1 = txtBarcodeToUpload1.Text;
            addrInfo.BarcodeToUpload2 = txtBarcodeToUpload2.Text;
            addrInfo.BarcodeToUpload3 = txtBarcodeToUpload3.Text;

            addrInfo.BarcodeToUploadLength1 = txtBarcodeToUploadLength1.Text;
            addrInfo.BarcodeToUploadLength2 = txtBarcodeToUploadLength2.Text;
            addrInfo.BarcodeToUploadLength3 = txtBarcodeToUploadLength3.Text;

            // ---------------- 打印条码 ----------------
            addrInfo.PrintTrigger = txtPrintTrigger.Text;
            addrInfo.PrintFeedback = txtPrintFeedback.Text;
            addrInfo.BarcodeToPrint = txtBarcodeToPrint.Text;
            addrInfo.BarcodeToPrintLenght = txtBarcodeToPrintLength.Text;

            // 设备参数
            addrInfo.GoodsProducts = txtGoodsProducts.Text;
            addrInfo.NotGoodsProducts = txtNotGoodsProducts.Text;
            addrInfo.ProduceCount = txtProduceCount.Text;
            addrInfo.DeviceStatus = txtDeviceStatus.Text;
            addrInfo.DeviceProgramName = txtDeviceProgramName.Text;
            addrInfo.ProgramNameLength = txtProgramNameLength.Text;
            addrInfo.ProductType = txtProductType.Text;
            addrInfo.ProductTypeLength = txtProductTypeLength.Text;
            addrInfo.BarcodeRule = txtBarcodeRule.Text;
            addrInfo.BarcodeRuleLength = txtBarcodeRuleLength.Text;
            addrInfo.ModelSwitch = txtModelSwitch.Text;
            addrInfo.ContinueProduce = txtContinueProduce.Text;
            addrInfo.PlcHeartBeat = txtPlcHeartBeat.Text;
            addrInfo.PcHeartBeat = txtPcHeartBeat.Text;
            addrInfo.RecoverySignal = txtRecoverySignal.Text;

            // ---------------- 扭力转发 ----------------
            addrInfo.TorqueValue1 = txtTorqueValue1.Text;
            addrInfo.TorqueValue3 = txtTorqueValue3.Text;

            addrInfo.TorqueResult1 = txtTorqueResult1.Text;
            addrInfo.TorqueResult3 = txtTorqueResult3.Text;

            addrInfo.TorqueMax1 = txtToqueMax1.Text;
            addrInfo.TorqueMax3 = txtToqueMax3.Text;

            addrInfo.TorqueMin1 = txtToqueMin1.Text;
            addrInfo.TorqueMin3 = txtToqueMin3.Text;

            addrInfo.Request1 = txtRequest1.Text;
            addrInfo.Request3 = txtRequest3.Text;

            addrInfo.Acknowledge1 = txtAcknowledge1.Text;
            addrInfo.Acknowledge3 = txtAcknowledge3.Text;

            MessageBox.Show(addrInfo.Save());
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

            MessageBox.Show(curDb.Change(sql) ? "保存成功" : "保存失败");
        }

        private void btnShowPath_Click(object sender, EventArgs e)
        {
            string filePath = printTemplatePath.Text;
            ShowFileInExplorer(filePath);
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
                _readWriteNet.Write(addrInfo.ContinueProduce, Convert.ToInt16(1));

                var form3Entity = selectedValue;
                OrderNo.Text = form3Entity.GDH;
                OrderNum.Text = form3Entity.GDSL.ToString();
                txtUser.Text = form3Entity.CZY;

                lblRunningStatus.Text = "工单切换完成！";
                lblRunningStatus.ForeColor = Color.Green;
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
            string localPath = Path.Combine(LocalFilePath.GetPropertySafely(c => c.Text), folderName);

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
                string orderNum = txtProductModel.GetPropertySafely(c => c.Text);
                string sql = $"select * from PrinterDirectory where order_num='{orderNum}'";
                DataTable dt = curDb.Find(sql);

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
                    int expectedPictureCount = int.Parse(pictureNum[index]);
                    Stopwatch waitTimer = Stopwatch.StartNew();
                    while (dirInfo.GetFiles().Length != expectedPictureCount)
                    {
                        if (waitTimer.Elapsed >= TimeSpan.FromSeconds(20)) return null;

                        // 图片由外部程序异步生成，无需持续占用 CPU 和反复扫描磁盘。
                        Thread.Sleep(100);
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
        /// 删除图片文件夹
        /// </summary>
        /// <param name="barCodes"></param>
        /// <param name="folder"></param>
        private void DeletePicture(List<string> barCodes, string folder = "PrdSNPictures")
        {
            foreach (string PrdSN in barCodes)
            {
                string localPath = Path.Combine(LocalFilePath.GetPropertySafely(c => c.Text), folder, PrdSN);
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
                //System.Diagnostics.ProcessName.GetCurrentProcess().Kill();
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

        private void LoadPlcAddress()
        {
            if (addrInfo != null)
            {
                // ---------------- 条码验证 ----------------
                txtHasBarcodeTag.Text = addrInfo.HasBarcodeTag;
                txtBarcodeVerifyTag.Text = addrInfo.BarcodeVerifyTag;
                txtBarcodeType.Text = addrInfo.BarcodeType;

                txtPlcScanned.Text = addrInfo.PlcScannedBarcode;
                txtScannedLength.Text = addrInfo.PlcScannedBarcodeLength;
                txtPanalizationBarcode.Text = addrInfo.PanalizationBarcode;
                txtPanalizationLength.Text = addrInfo.PanalizationBarcodeLength;

                txtManualInput.Text = addrInfo.ManualInputBarcodeTip;
                txtManualBarcode.Text = addrInfo.ManualInputBarcode;
                txtManualLength.Text = addrInfo.ManualInputBarcodeLength;

                // ---------------- 数据上传 ----------------
                txtTriggerUpload1.Text = addrInfo.TriggerUpload1;
                txtTriggerUpload2.Text = addrInfo.TriggerUpload2;
                txtTriggerUpload3.Text = addrInfo.TriggerUpload3;

                txtFeedback1.Text = addrInfo.Feedback1;
                txtFeedback2.Text = addrInfo.Feedback2;
                txtFeedback3.Text = addrInfo.Feedback3;

                txtProductResult1.Text = addrInfo.ProductResult1;
                txtProductResult2.Text = addrInfo.ProductResult2;
                txtProductResult3.Text = addrInfo.ProductResult3;

                txtBarcodeToUpload1.Text = addrInfo.BarcodeToUpload1;
                txtBarcodeToUpload2.Text = addrInfo.BarcodeToUpload2;
                txtBarcodeToUpload3.Text = addrInfo.BarcodeToUpload3;

                txtBarcodeToUploadLength1.Text = addrInfo.BarcodeToUploadLength1;
                txtBarcodeToUploadLength2.Text = addrInfo.BarcodeToUploadLength2;
                txtBarcodeToUploadLength3.Text = addrInfo.BarcodeToUploadLength3;

                // ---------------- 打印条码 ----------------
                txtPrintTrigger.Text = addrInfo.PrintTrigger;
                txtPrintFeedback.Text = addrInfo.PrintFeedback;
                txtBarcodeToPrint.Text = addrInfo.BarcodeToPrint;
                txtBarcodeToPrintLength.Text = addrInfo.BarcodeToPrintLenght;

                // ---------------- 设备参数 ----------------
                txtGoodsProducts.Text = addrInfo.GoodsProducts;
                txtNotGoodsProducts.Text = addrInfo.NotGoodsProducts;
                txtProduceCount.Text = addrInfo.ProduceCount;
                txtDeviceStatus.Text = addrInfo.DeviceStatus;
                txtDeviceProgramName.Text = addrInfo.DeviceProgramName;
                txtProgramNameLength.Text = addrInfo.ProgramNameLength;
                txtProductType.Text = addrInfo.ProductType;
                txtProductTypeLength.Text = addrInfo.ProductTypeLength;
                txtBarcodeRule.Text = addrInfo.BarcodeRule;
                txtBarcodeRuleLength.Text = addrInfo.BarcodeRuleLength;
                txtModelSwitch.Text = addrInfo.ModelSwitch;
                txtContinueProduce.Text = addrInfo.ContinueProduce;
                txtPlcHeartBeat.Text = addrInfo.PlcHeartBeat;
                txtPcHeartBeat.Text = addrInfo.PcHeartBeat;
                txtRecoverySignal.Text = addrInfo.RecoverySignal;

                // ---------------- 扭力转发 ----------------
                txtTorqueValue1.Text = addrInfo.TorqueValue1;
                txtTorqueValue3.Text = addrInfo.TorqueValue3;

                txtTorqueResult1.Text = addrInfo.TorqueResult1;
                txtTorqueResult3.Text = addrInfo.TorqueResult3;

                txtToqueMax1.Text = addrInfo.TorqueMax1;
                txtToqueMax3.Text = addrInfo.TorqueMax3;

                txtToqueMin1.Text = addrInfo.TorqueMin1;
                txtToqueMin3.Text = addrInfo.TorqueMin3;

                txtRequest1.Text = addrInfo.Request1;
                txtRequest3.Text = addrInfo.Request3;

                txtAcknowledge1.Text = addrInfo.Acknowledge1;
                txtAcknowledge3.Text = addrInfo.Acknowledge3;
            }
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            TorqueSerialClient.AutoRefreshComboBoxes(cmbCOM1, cmbCOM2);
            MessageBox.Show("串口列表刷新完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // --- Windows 底层消息常量 ---
        private const int WM_DEVICECHANGE = 0x0219;           // 设备发生变化
        private const int DBT_DEVICEARRIVAL = 0x8000;         // 设备插入
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  // 设备拔出

        /// <summary>
        /// 拦截系统消息，实现热插拔自动刷新
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_DEVICECHANGE)
            {
                int wParam = m.WParam.ToInt32();

                if (wParam == DBT_DEVICEARRIVAL || wParam == DBT_DEVICEREMOVECOMPLETE)
                {
                    // 【关键修复】使用 BeginInvoke 而不是 Invoke！
                    // BeginInvoke 是异步的，它把任务塞给 UI 线程后立刻返回，
                    // 彻底避免了 VSPD 销毁虚拟端口时卡死主线程 (WndProc) 的 Bug。
                    this.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(500); // 延迟等待系统注册表完全刷新

                        // 将你的两个下拉框传进去
                        MesDatas.Services.TorqueSerialClient.AutoRefreshComboBoxes(cmbCOM1, cmbCOM2);

                        //rtbRouteLog.AppendText($"[系统] 检测到串口设备变动，列表已自动更新。\r\n");
                    }));
                }
            }
        }
    }
}
