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
        private Assembly assembly;
        private ResourceManager resources;
        private PlcAddressInfo addrInfo;
        private DataAcess.SystemInfo systemInfo;
        private PLCAdress plcAddress;

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
                        Log4netHelper.Error(errorMsg);
                    }

                    PlcSignalLight.ForeColor = isPlcConnected ? Color.Green : Color.Red;
                }));
            };

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

                var (isReadOk, value) = await TryReadInt16Async(addrInfo.RecoverySignal);
                if (!isReadOk || value != 1)
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
                    if (btnManualClear.GetPropertySafely(c => c.Visible))
                        btnManualClear.PerformClick();

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
                    continue;

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

                var (isReadOk, value) = await TryReadInt16Async(addrInfo.ModelSwitch);

                if (isReadOk && value == 1)
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
                    Log4netHelper.Error($"设备状态上传线程异常: {ex.Message}");
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
                    }
                    else
                    {
                        rtbErrorLog.AppendToComponent("预加载跳过：模板文件未找到");
                    }
                }
                catch (Exception ex)
                {
                    // 预加载失败不应该阻断线程，后面主循环有自愈机制会重试
                    rtbErrorLog.AppendToComponent($"打印引擎预加载异常(将在主循环重试): {ex.Message}");
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
                                    Thread.Sleep(5000);
                                    continue;
                                }

                                csApp.Documents.Open(filename, true);
                                doc = csApp.ActiveDocument;
                                doc.Printer.SwitchTo(printerName.GetPropertySafely(c => c.Text));

                                PrinterSignal.AppendToComponent("打印引擎初始化/恢复成功");
                                failCount = 0; // 重置失败计数
                            }
                            catch (Exception ex)
                            {
                                failCount++;
                                rtbErrorLog.AppendToComponent($"[重试次数{failCount}]初始化打印机失败: {ex.Message}");
                                csApp = null; // 置空以触发下次重试
                                Thread.Sleep(3000);
                                continue;
                            }
                        }

                        // 3.1 重试指定次数后仍失败
                        if (csApp == null || doc == null) return;

                        // -------------------- 4. 读取 PLC 触发信号 --------------------

                        PrinterSignal.AppendToComponent($"持续监测[{addrInfo.PrintTrigger}]中……");
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

                        bool isPrintSuccess = false;
                        string failReason;

                        // ---------------------------------------------------------
                        // 【核心新增】：重复打印拦截逻辑
                        // ---------------------------------------------------------
                        if (sn2UploadMes4Print == lastPrintedBarcode && !string.IsNullOrEmpty(sn2UploadMes4Print))
                        {
                            PrinterSignal.AppendToComponent($"【拦截重复触发】条码 [{sn2UploadMes4Print}] 已打印过，直接放行PLC。");
                            lblRunningStatus.ExecuteSafely(c => { c.Text = $"重复跳过: {sn2UploadMes4Print}"; c.ForeColor = Color.DarkOrange; });

                            // 伪装成成功，跳过下方物理打印，直接走第9步反馈PLC
                            isPrintSuccess = true;
                        }
                        else
                        {
                            // 只有当条码不同时，才走物理打印与MES请求流程
                            PrinterSignal.AppendToComponent($"收到新打印请求: {sn2UploadMes4Print}");
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
                            }
                            else
                            {
                                failReason = barCodeParam == null ? "接口返回空" : barCodeParam.ErrorMessage;
                                lblRunningStatus.ExecuteSafely(c => { c.Text = $"打印失败: {failReason}"; c.ForeColor = Color.Red; });
                                PrinterSignal.AppendToComponent($"打印失败: {failReason}");

                                // 注意：失败时不要更新 lastPrintedBarcode，这样下次PLC重试时还能进来
                            }
                        }

                        // 9. 反馈 PLC (不管是真实成功，还是拦截放行，这里都会写 1)
                        if (!TryWriteInt16Value(addrInfo.PrintFeedback, isPrintSuccess ? 1 : 2))
                        {
                            PrinterSignal.AppendToComponent($"写入打印信号失败({addrInfo.PrintFeedback}={isPrintSuccess}，请检查PLC连接");
                        }

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
                                    break;
                                }
                            }

                            waitTimeOut++;
                            if (waitTimeOut > 50) // 10秒超时
                            {
                                PrinterSignal.AppendToComponent("警告：PLC 复位信号超时，强制重置");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PrinterSignal.AppendToComponent($"打印线程异常: {ex.Message}");
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
                rtbReadBarCode.AppendToComponent("等待PLC连接……");

                if (DelayAndCheckStop(500)) return;
            }

            bool isEnableManualInputBarcode = false;         // 需要手动输入条码标志：true按钮已启用，false为未启用，默认为false；

            while (true)
            {
                if (DelayAndCheckStop(100)) return;     // 检测是否应该退出当前任务
                if (existErrorInErrorTip) continue;          // 当前有错误，需要先清除错误后再次访问

                rtbReadBarCode.AppendToComponent($"持续监测'{addrInfo.HasBarcodeTag}'信号中...");

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
                        Log4netHelper.Info($"从{addrInfo.HasBarcodeTag}检测到扫码读码信号=1");

                        // 首先清除触发信号
                        _readWriteNet.Write(addrInfo.HasBarcodeTag, 0);
                        Log4netHelper.Info($"清除扫码读码信号:{addrInfo.HasBarcodeTag}=0");

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
                Log4netHelper.Info($"{addrInfo.PlcScannedBarcode}读取到条码：{scannedBarcode}");

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
                    Log4netHelper.Info($"{scannedBarcode}流程检查：流程检查成功，通知plc继续生产:{addrInfo.BarcodeVerifyTag}=1");
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
            Log4netHelper.Info($"工装条码:{toolingBarcode} 向{addrInfo.BarcodeVerifyTag}写入: 1");
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
                Log4netHelper.Info($"{scannedBarcode}获取拼版：连接错误，无法获取拼版条码");

                return HandleError(
                    addrInfo.BarcodeVerifyTag, 2, true, "连接错误，无法获取拼版条码");
            }

            // 2. 处理MES返回FAIL
            if (mesResponse.Result.Equals(nameof(MyEnum.Result.FAIL), StringComparison.OrdinalIgnoreCase))
            {
                Log4netHelper.Info($"{scannedBarcode}获取拼版：{mesResponse.ErrorMessage}");

                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, $"获取拼版条码错误:{mesResponse.ErrorMessage}");
            }

            // 3. 处理MES返回PASS，但数据不合规（如非拼板）
            if (mesResponse.PrdSNInfo.PrdSNs.Count <= 1)
            {
                Log4netHelper.Info($"{scannedBarcode}获取拼版：获取拼版接口验证通过但没返回拼版条码");

                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, "获取拼版接口验证通过但没返回拼版条码");
            }

            // 4. MES 返回 Pass 且数据合规
            lblRunningStatus.ExecuteSafely(c => { c.Text = "拼版条码获取成功!"; c.ForeColor = Color.Green; });

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
                Log4netHelper.Info($"{scannedBarcode}流程检查：访问接口错误，无法进行流程检查（返回null）");

                rtbErrorLog.AppendToComponent("访问接口错误，无法进行流程检查");

                return HandleError(addrInfo.BarcodeVerifyTag, 2, true, "访问接口错误，无法进行流程检查（返回null）");
            }

            // 3b.MES返回FAIL
            if (mesResponse.Result.Equals(nameof(MyEnum.Result.FAIL), StringComparison.OrdinalIgnoreCase))
            {
                Log4netHelper.Info($"{scannedBarcode}流程检查：MES返回非PASS,{mesResponse.ErrorMessage}");

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
                Log4netHelper.Info($"{scannedBarcode}流程检查：流程检查成功，但是查找的拼版为结果null，无法发送到plc");

                return HandleError(null, 2, false, "流程检查：无法将拼版条码发送给PLC");
            }

            OperateResult result = _readWriteNet.Write(addrInfo.PanalizationBarcode, anotherBarcode);
            Log4netHelper.Info($"{scannedBarcode}流程检查：流程检查成功，拼版条码{anotherBarcode}发送至{addrInfo.PanalizationBarcode},发送状态：{result.IsSuccess}");
            return true;
        }

        /// <summary>
        /// 在未勾选流程检查时，直接向PLC反馈OK。
        /// </summary>
        private void BypassRouteCheck(string readPlcSn)
        {
            lblRunningStatus.ExecuteSafely(c => { c.Text = "跳过流程检查成功!"; c.ForeColor = Color.Green; });

            _readWriteNet.Write($"{addrInfo.BarcodeVerifyTag}", 1);

            Log4netHelper.Info($"{readPlcSn}跳过条码验证成功：{addrInfo.BarcodeVerifyTag}=1");
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

                UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                TryReadInt16Value(uploadManager.triggerPoint, out int triggerValue);
                triggerWatch.Stop();
                if (triggerValue != 1 || !isPlcConnected) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogElapsed("PLC触发检测耗时", triggerWatch);
                    trace.Log($"检测到PLC触发信号，{uploadManager.triggerPoint}={triggerValue}");

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
                        trace.Log($"数据上传发生异常：{ex}");
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

                UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                triggerWatch.Stop();
                if (triggerValue != 1 || isPlcConnected != true) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogElapsed("PLC触发检测耗时", triggerWatch);
                    trace.Log($"检测到PLC触发信号，{uploadManager.triggerPoint}={triggerValue}");

                    try
                    {
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 监听到触发数据上传信号：{uploadManager.triggerPoint} = {triggerValue}");

                        var prdSN = GetProductResult(uploadManager, new List<string>(), new List<string>(), trace);

                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程处理完成：{prdSN}");
                    }
                    catch (Exception ex)
                    {
                        trace.Log($"数据上传流程发生异常：{ex}");
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

                UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                triggerWatch.Stop();
                if (triggerValue != 1 || isPlcConnected != true) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogElapsed("PLC触发检测耗时", triggerWatch);
                    trace.Log($"检测到PLC触发信号，{uploadManager.triggerPoint}={triggerValue}");

                    try
                    {
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 监听到触发数据上传信号：{uploadManager.triggerPoint} = {triggerValue}");

                        var prdSN = GetProductResult(uploadManager, new List<string>(), new List<string>(), trace);

                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程处理完成：{prdSN}");
                    }
                    catch (Exception ex)
                    {
                        trace.Log($"数据上传流程发生异常：{ex}");
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

                UploadMes.AppendToComponent("持续监测中……");
                Stopwatch triggerWatch = Stopwatch.StartNew();
                var triggerValue = _readWriteNet.ReadInt16(uploadManager.triggerPoint).Content;
                triggerWatch.Stop();
                if (triggerValue != 1 || isPlcConnected != true) continue;

                ProductPassTraceContext trace = ProductPassTraceContext.Start(uploadManager.Name, uploadManager.triggerPoint, uploadManager.feedbackPoint);
                using (trace.EnterScope())
                {
                    trace.LogElapsed("PLC触发检测耗时", triggerWatch);
                    trace.Log($"检测到PLC触发信号，{uploadManager.triggerPoint}={triggerValue}");

                    try
                    {
                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 监听到触发数据上传信号：{uploadManager.triggerPoint} = {triggerValue}");

                        var prdSN = GetProductResult(uploadManager, new List<string>(), new List<string>(), trace);

                        UploadMes.AppendToComponent($"[{uploadManager.Name}] 数据上传流程处理完成：{prdSN}");
                    }
                    catch (Exception ex)
                    {
                        trace.Log($"数据上传流程发生异常：{ex}");
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
                        trace?.LogElapsed("读产品结果/条码耗时", productInfoWatch);
                        trace?.Log(log);
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
                        trace?.LogElapsed("读产品结果/条码耗时", productInfoWatch);
                        trace?.Log(log);
                        traceResult = "产品条码读取失败";
                        HandleError(uploadEntity.feedbackPoint, 2, true, userMessage: log);
                        UploadMes.AppendToComponent(log);
                        return prdSN;
                    }

                    if (string.IsNullOrWhiteSpace(prdSN))
                    {
                        var log = $"[{uploadEntity.Name}] 获取的条码数据为空";
                        trace?.LogElapsed("读产品结果/条码耗时", productInfoWatch);
                        trace?.Log(log);
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
                    trace?.LogElapsed("读产品结果/条码耗时", productInfoWatch);
                    trace?.Log("未获取到条码，准备抛出异常");
                    traceResult = "未获取到条码";
                    // 丢给外层捕捉
                    throw new Exception("未获取到条码");
                }

                trace?.LogElapsed("读产品结果/条码耗时", productInfoWatch);
                trace?.Log($"读取产品结果/条码完成，当前条码={prdSN}，待上传产品数={scannedBarcodeList.Count}");

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
                    trace?.LogElapsed("读测试项耗时", testDataWatch);
                    trace?.Log(log);
                    traceResult = "读取测试数据失败";
                    HandleError(uploadEntity.feedbackPoint, 2, true, log);
                    rtbErrorLog.AppendToComponent(log);
                    return prdSN;
                }

                trace?.LogElapsed("读测试项耗时", testDataWatch);
                trace?.Log($"测试数据读取完成，Value={valList.Count}，USL={maxList.Count}，LSL={minList.Count}，Result={resList.Count}，DefectType={staList.Count}");

                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 测试数据读取完成");

                #endregion

                #region ---------- 上传结果（带重试） ----------

                ReturnParamSendResult returnParam = null;

                // 离线模式：直接反馈生产完成信号给PLC
                if (!EnableResultUpload.Checked)
                {
                    Stopwatch feedbackWatch = Stopwatch.StartNew();
                    bool feedbackOk = TryWriteInt16Value(uploadEntity.feedbackPoint, 1);
                    trace?.LogElapsed("写D7116耗时", feedbackWatch);
                    trace?.Log($"离线模式反馈{uploadEntity.feedbackPoint}=1，写入结果={feedbackOk}");
                    traceResult = "Offline";
                }
                else
                {
                    bool isRetry = false;
                    do
                    {
                        isRetry = false;

                        // ============= 开始上传数据 =============

                        UploadMes.AppendToComponent($"[{uploadEntity.Name}] 开始执行数据上传流程 <-");

                        // 上传结果到MES（包含本地txt文件，图片等信息）
                        Stopwatch mesWatch = Stopwatch.StartNew();
                        trace?.Log($"请求MES流程开始，条码数量={scannedBarcodeList.Count}");
                        returnParam = SendResultToMes(scannedBarcodeList, productResultList, valList, maxList, minList, resList, staList, uploadEntity, trace);
                        trace?.LogElapsed("MES请求耗时", mesWatch);
                        trace?.Log($"请求MES流程结束，Result={returnParam?.Result ?? "null"}，Error={returnParam?.ErrorMessage ?? string.Empty}");

                        UploadMes.AppendToComponent($"[{uploadEntity.Name}] -> 数据上传流程执行结束");

                        // ============= 解析返回来的参数 =============

                        // 判断是否过站成功
                        bool isPass = returnParam != null && returnParam.Result.Equals(nameof(MyEnum.Result.PASS), StringComparison.OrdinalIgnoreCase);

                        if (isPass)
                        {
                            // --- 成功逻辑 ---
                            Stopwatch feedbackWatch = Stopwatch.StartNew();
                            var feedbackResult = _readWriteNet.Write($"{uploadEntity.feedbackPoint}", 1);
                            trace?.LogElapsed("写D7116耗时", feedbackWatch);
                            trace?.Log($"MES PASS后反馈{uploadEntity.feedbackPoint}=1，写入结果={feedbackResult.IsSuccess}，信息={feedbackResult.Message}");

                            if (!feedbackResult.IsSuccess)
                            {
                                var log = $"[{uploadEntity.Name}] MES已PASS，但反馈{uploadEntity.feedbackPoint}=1失败：{feedbackResult.Message}";
                                traceResult = "D7116写入失败";
                                HandleError(uploadEntity.feedbackPoint, 2, true, log);
                                UploadMes.AppendToComponent(log);
                                return prdSN;
                            }

                            traceResult = "PASS";
                            UploadMes.AppendToComponent($"[{uploadEntity.Name}] 过站成功，反馈{uploadEntity.feedbackPoint} = 1");
                            lblRunningStatus.ExecuteSafely(c => { c.Text = "生产结果上传成功"; c.ForeColor = Color.Green; });
                        }
                        else
                        {
                            // --- 失败逻辑：弹出人工选择对话框 ---
                            string errorMsg = returnParam == null ? "接口返回数据异常(Null)" : returnParam.ErrorMessage;
                            trace?.Log($"MES未PASS，准备弹出人工处理窗口，原因={errorMsg}");

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
                                trace?.Log("操作员选择立即重试MES上传");
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
                                trace?.Log("MES返回NULL，保持原有逻辑返回，不写PASS反馈");
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
                            trace?.Log($"MES返回FAIL，程序模式={operJudge}，准备反馈{uploadEntity.feedbackPoint}=2");
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

                if (uploadEntity.Name == ProcessName.Scan_ASSY || uploadEntity.Name == ProcessName.Non_Assembly)
                    ShowResult(dgvResult1, returnParam, uploadEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
                else if (uploadEntity.Name == ProcessName.Weight)
                    ShowResult(dgvResult2, returnParam, uploadEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);
                else if (uploadEntity.Name == ProcessName.Screw_BA)
                    ShowResult(dgvResult3, returnParam, uploadEntity, scannedBarcodeList, productResultList, valList, maxList, minList, resList);

                #endregion

            }
            finally
            {
                trace?.Finish(prdSN, traceResult);
                SendResultAfter(uploadEntity, scannedBarcodeList, productResultList);
            }

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
        public ReturnParamSendResult SendResultToMes(List<string> scannedBarcodeList, List<string> productResultList, List<string> valList, List<string> maxList, List<string> minList, List<string> resList, List<string> staList, UploadManagerEntity uploadEntity, ProductPassTraceContext trace = null)
        {
            // 获取当前工序需要上传的测试项名称和单位
            GetFilteredTestItems(uploadEntity, out var currentTestNameList, out var currentUnitList);

            // 线程中需要捕获异常，否则会直接退出
            try
            {
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

                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 请求MES流程开始");
                trace?.Log($"SendResultToMes开始调用MES，Function=SAVERESULT，Url={Url_DataUpload.Text}");
                var returnParam = _request.GetResponseSerializeResult<ReturnParamSendResult, InputParamSendResult>(Url_DataUpload.Text, _httpClient, "SAVERESULT", inputParam, nameof(uploadEntity.Name));
                trace?.Log($"SendResultToMes收到MES响应，Result={returnParam?.Result ?? "null"}，Error={returnParam?.ErrorMessage ?? string.Empty}");
                UploadMes.AppendToComponent($"[{uploadEntity.Name}] 请求MES流程结束");

                if (returnParam == null)
                {
                    trace?.Log("上传结果接口返回数据异常（null），准备走原有错误处理");
                    HandleError(uploadEntity.feedbackPoint, 2, true, $"[{uploadEntity.Name}] 上传结果接口返回数据异常（null）");
                    return null;
                }

                return returnParam;
            }
            catch (Exception ex)
            {
                trace?.Log($"数据上传流程发生异常：{ex}");
                HandleError(uploadEntity.feedbackPoint, 2, true, $"[{uploadEntity.Name}] 数据上传流程发生异常：{ex}");
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
                return false;
            }

            var result = _readWriteNet.ReadInt16(address);
            if (result.IsSuccess)
            {
                value = result.Content;
                return true;
            }

            rtbErrorLog.AppendToComponent($"{failReason} | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
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
                return false;
            }

            var result = _readWriteNet.Write(address, value);
            if (result.IsSuccess) return true;

            rtbErrorLog.AppendToComponent($"{prefix}PLC写入失败 | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
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
                return false;
            }

            var result = _readWriteNet.Write(address, value);
            if (result.IsSuccess) return true;

            rtbErrorLog.AppendToComponent($"{failReson} | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
            return false;
        }

        // 定义两个扭力控制器客户端
        private TorqueControllerClient _clientScanAssy; // 工序1 (Scan-ASSY)
        private TorqueControllerClient _clientScrewBa;  // 工序3 (Screw-BA，实际在工位5动作)
        private CancellationTokenSource _torqueCts;     // 用于取消互锁监控循环

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
                /*if (isErrorLog)
                {
                    rtbErrorLog.AppendToComponent($"[{ProcessName.Scan_ASSY}] {msg}");
                    Log4netHelper.Error($"[{ProcessName.Scan_ASSY}] {msg}");
                }
                else
                {
                    rtbASSYLog.AppendToComponent($"[{ProcessName.Scan_ASSY}] {msg}");
                    Log4netHelper.LogTorque($"[{ProcessName.Scan_ASSY}] {msg}");
                }*/
                AppendLog(ProcessName.Scan_ASSY, msg);
            };

            _clientScanAssy.OnTorqueDataReceived += (data) =>
            {
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
                /*if (isErrorLog)
                {
                    rtbErrorLog.AppendToComponent($"[{ProcessName.Screw_BA}] {msg}");
                    Log4netHelper.Error($"[{ProcessName.Screw_BA}] {msg}");
                }
                else
                {
                    rtbBALog.AppendToComponent($"[{ProcessName.Screw_BA}] {msg}");
                    Log4netHelper.LogTorque($"[{ProcessName.Screw_BA}] {msg}");
                }*/

                AppendLog(ProcessName.Screw_BA, msg);
            };

            _clientScrewBa.OnTorqueDataReceived += (data) =>
            {
                Task.Run(async () => await ForwardTorqueToPlcAsync(ProcessName.Screw_BA, data));
            };

            _clientScrewBa.Start();

            #endregion
        }

        /// <summary>
        /// 实时监控电批状态并静默比对PLC互锁信号
        /// </summary>
        private async Task TorqueInterlockMonitorLoopAsync(CancellationToken token)
        {
            // 目标状态：1=允许打螺钉, 2=禁止打螺钉

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);

                if (!isPlcConnected) continue;

                if (_clientScanAssy == null || _clientScrewBa == null) continue;

                // ---------- 工序1 互锁监控 ----------

                short targetVal1 = _clientScanAssy.IsConnected ? (short)1 : (short)2;

                var readRes1 = await _readWriteNet.ReadInt16Async(addrInfo.TorqueReady1);

                if (readRes1.IsSuccess && readRes1.Content != targetVal1)
                {
                    var writeTask = _readWriteNet.WriteAsync(addrInfo.TorqueReady1, targetVal1);
                    var completedTask = await Task.WhenAny(writeTask, Task.Delay(3000));

                    if (completedTask != writeTask || !writeTask.Result.IsSuccess)
                        HandleError(addrInfo.TorqueReady1, targetVal1, true, "工序1电批互锁信号写入失败");
                }

                // ---------- 工序3 互锁监控 ----------

                short targetVal3 = _clientScrewBa.IsConnected ? (short)1 : (short)2;

                var readRes3 = await _readWriteNet.ReadInt16Async(addrInfo.TorqueReady3);

                if (readRes3.IsSuccess && readRes3.Content != targetVal3)
                {
                    var writeTask = _readWriteNet.WriteAsync(addrInfo.TorqueReady3, targetVal3);
                    var completedTask = await Task.WhenAny(writeTask, Task.Delay(3000));

                    if (completedTask != writeTask || !writeTask.Result.IsSuccess)
                        HandleError(addrInfo.TorqueReady3, targetVal3, true, "工序3电批互锁信号写入失败");
                }
            }
        }

        /// <summary>
        /// 将接收到的扭力数据写入对应的 PLC 地址
        /// </summary>
        private async Task ForwardTorqueToPlcAsync(ProcessName processName, TorqueData data)
        {
            string addrTorque, addrMax, addrMin, addrResult, addrReq, addrAck;
            Label lblVal, lblMin, lblMax, lblRes;

            // 1. 获取工序对应的地址配置
            switch (processName)
            {
                case ProcessName.Scan_ASSY:
                    addrTorque = addrInfo.TorqueValue1; addrMax = addrInfo.TorqueMax1; addrMin = addrInfo.TorqueMin1;
                    addrResult = addrInfo.TorqueResult1; addrReq = addrInfo.Request1; addrAck = addrInfo.Acknowledge1;
                    lblVal = lblAssyVal; lblMin = lblAssyMin; lblMax = lblAssyMax; lblRes = lblAssyRes;
                    break;
                case ProcessName.Screw_BA:
                    addrTorque = addrInfo.TorqueValue3; addrMax = addrInfo.TorqueMax3; addrMin = addrInfo.TorqueMin3;
                    addrResult = addrInfo.TorqueResult3; addrReq = addrInfo.Request3; addrAck = addrInfo.Acknowledge3;
                    lblVal = lblBaVal; lblMin = lblBaMin; lblMax = lblBaMax; lblRes = lblBaRes;
                    break;
                default:
                    return;
            }

            // 2. 解析数据
            int.TryParse(data.Torque, out int val);
            int.TryParse(data.TorqueMin, out int min);
            int.TryParse(data.TorqueMax, out int max);
            short result = data.TighteningStatus ? (short)3 : (short)2; // 3=OK, 2=NG

            // 3. 【UI 实时更新】
            Invoke((Action)(() =>
            {
                lblVal.Text = $"{(double)val / 100:0.00}"; lblMin.Text = $"{(double)min / 100:0.00}"; lblMax.Text = $"{(double)max / 100:0.00}";
                lblRes.Text = data.TighteningStatus ? "OK" : "NG";
                lblRes.ForeColor = data.TighteningStatus ? Color.Green : Color.Red;
            }));

            bool isSuccess = false;
            bool errorTriggered = false; // 防刷屏标志位
            int attempt = 1;

            try
            {
                // 无限重试循环，直到成功为止
                while (!isSuccess)
                {
                    bool currentAttemptSuccess = false;

                    byte[] buffer = new byte[14];

                    BitConverter.GetBytes(val).CopyTo(buffer, 0);
                    BitConverter.GetBytes(max).CopyTo(buffer, 4);
                    BitConverter.GetBytes(min).CopyTo(buffer, 8);
                    BitConverter.GetBytes(result).CopyTo(buffer, 12);

                    var wAll = TryWriteByteArray(addrTorque, buffer, "批量写入扭力核心数据(包含值、上下限、结果)");

                    // --- 步骤 A：写入业务数据 ---
                    bool w1 = TryWriteInt32(addrTorque, val, "扭力实际值");
                    bool w2 = TryWriteInt32(addrMin, min, "扭力下限");
                    bool w3 = TryWriteInt32(addrMax, max, "扭力上限");
                    bool w4 = TryWriteInt16(addrResult, result, failReson: "扭力结果");

                    if (/*w1 && w2 && w3 && w4*/ wAll)
                    {
                        // --- 步骤 B：发起握手请求 Req = 1 ---
                        if (TryWriteInt16(addrReq, 1, failReson: "PC请求握手(Req=1)"))
                        {
                            // --- 步骤 C：等待 PLC 回应 Ack == 1 ---
                            var startTime = System.DateTime.Now;
                            bool ackReceived = false;

                            while ((System.DateTime.Now - startTime).TotalSeconds < 3)
                            {
                                if (TryReadInt16(addrAck, out var readAck, "读取PLC扭力确认接收信号失败") && readAck == 1)
                                {
                                    ackReceived = true;
                                    break;
                                }
                                await Task.Delay(50);
                            }

                            if (ackReceived)
                            {
                                currentAttemptSuccess = true;
                            }
                            else
                            {
                                AppendLog(processName, $"[转发超时] 第 {attempt} 次等待Ack信号超时(3秒)！");
                            }
                        }
                        else
                        {
                            AppendLog(processName, $"[转发失败] 第 {attempt} 次握手请求发送失败...");
                        }
                    }
                    else
                    {
                        AppendLog(processName, $"[转发失败] 第 {attempt} 次核心数据写入PLC失败...");
                    }

                    // --- 步骤 D：判断本轮结果 ---
                    if (currentAttemptSuccess)
                    {
                        isSuccess = true;
                        AppendLog(processName, $"[转发成功] 扭力:{val}, 结果:{(data.TighteningStatus ? "OK" : "NG")} (共尝试 {attempt} 次)");
                        break; // 握手成功，彻底跳出无限重试循环
                    }
                    else
                    {
                        // 【失败后的处理逻辑】
                        TryWriteInt16(addrReq, 0, failReson: "重试前复位Req=0"); // 必须复位，制造上升沿

                        // 首次失败时，触发一次全局阻塞报警
                        if (!errorTriggered)
                        {
                            HandleError(
                                feedbackAddress: null, // 这里给null即可，只需要阻塞界面，不需要向特定地址写复位值
                                isBlockingError: true,
                                userMessage: $"[{processName}] 扭力转发PLC失败，程序挂起并持续重试",
                                logMessage: $"工序 {processName} 扭力转发PLC失败，后台正在持续重试中..."
                            );
                            errorTriggered = true; // 标记已触发，防止无限报警把队列塞满导致内存溢出
                        }

                        attempt++;
                        await Task.Delay(1500); // 休眠 1.5 秒后继续下一轮死磕
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog(processName, $"[扭力转发异常] 发生未捕获异常: {ex.Message}");
            }
            finally
            {
                // 5. 兜底保护：不管重试了多少次，只要退出循环，必须把 Req 拉低释放 PLC。
                TryWriteInt16(addrReq, 0, failReson: "复位PC请求握手(Req=0)");
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
                return false;
            }

            var result = _readWriteNet.Write(address, value);
            if (result.IsSuccess) return true;

            rtbErrorLog.AppendToComponent($"{prefix}PLC写入失败 | 地址: {address} | 错误码: {result.ErrorCode} | 原因: {result.Message}");
            return false;
        }

        /// <summary>
        /// 将接收到的扭力数据写入对应的 PLC 地址
        /// </summary>
        /// <param name="processName">工位号 ("Scan-ASSY" 或 "Screw-BA")</param>
        /// <param name="data">扭力数据</param>
        private void ForwardTorqueToPlc(ProcessName processName, TorqueData data)
        {
            try
            {
                // 0.基础检查
                if (_readWriteNet == null)
                {
                    AppendLog(processName, $"转发失败，PLC未连接");
                    return;
                }

                // 1. 获取工序对应的地址配置
                string addrTorque, addrMax, addrMin, addrResult, addrReq, addrAck;

                // 用于UI更新的控件引用
                Label lblVal, lblMin, lblMax, lblRes;

                switch (processName)
                {
                    case ProcessName.Scan_ASSY:
                        addrTorque = addrInfo.TorqueValue1;
                        addrMax = addrInfo.TorqueMax1;
                        addrMin = addrInfo.TorqueMin1;
                        addrResult = addrInfo.TorqueResult1;
                        addrReq = addrInfo.Request1;
                        addrAck = addrInfo.Acknowledge1;

                        // 绑定工序1的UI控件
                        lblVal = lblAssyVal;
                        lblMin = lblAssyMin;
                        lblMax = lblAssyMax;
                        lblRes = lblAssyRes;
                        break;
                    case ProcessName.Screw_BA:
                        addrTorque = addrInfo.TorqueValue3;
                        addrMax = addrInfo.TorqueMax3;
                        addrMin = addrInfo.TorqueMin3;
                        addrResult = addrInfo.TorqueResult3;
                        addrReq = addrInfo.Request3;
                        addrAck = addrInfo.Acknowledge3;

                        // 绑定工序3的UI控件
                        lblVal = lblBaVal;
                        lblMin = lblBaMin;
                        lblMax = lblBaMax;
                        lblRes = lblBaRes;
                        break;
                    default:
                        return;
                }

                // 2. 解析数据
                int.TryParse(data.Torque, out int val);
                int.TryParse(data.TorqueMin, out int min);
                int.TryParse(data.TorqueMax, out int max);
                short result = data.TighteningStatus ? (short)3 : (short)2; // 3=OK, 2=NG

                // 3. 【UI 实时更新 1】显示采集到的数据
                Invoke((Action)(() =>
                {
                    lblVal.Text = $"{val:0.00}";
                    lblMin.Text = $"{min:0.00}";
                    lblMax.Text = $"{max:0.00}";
                    lblRes.Text = data.TighteningStatus ? "OK" : "NG";
                    lblRes.ForeColor = data.TighteningStatus ? Color.Green : Color.Red;
                }));

                // 3. 写入数据
                bool w1 = _readWriteNet.Write(addrTorque, val).IsSuccess;
                bool w2 = _readWriteNet.Write(addrMin, min).IsSuccess;
                bool w3 = _readWriteNet.Write(addrMax, max).IsSuccess;
                bool w4 = _readWriteNet.Write(addrResult, result).IsSuccess;

                if (!w1 || !w2 || !w3 || !w4)
                {
                    AppendLog(processName, "[转发错误] 数据写入PLC失败");
                    return;
                }

                // 4. 发起握手：置位 Req = 1
                if (!_readWriteNet.Write(addrReq, (ushort)1).IsSuccess)
                {
                    AppendLog(processName, "[转发错误] 握手请求发送失败");
                    return;
                }

                // 5. 等待握手完成：轮询 Ack == 1（超时时间：3s）
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

                string msg = isSuccess
                    ? $"[转发成功] 扭力:{val}, 上限:{max}, 下限:{min}, 结果:{(data.TighteningStatus ? "OK" : "NG")} -> PLC接收确认"
                    : $"[转发超时] PLC未在2秒内响应Ack信号 (请检查PLC逻辑)";

                // 7. 输出结果
                AppendLog(processName, msg);
            }
            catch (Exception ex)
            {
                AppendLog(processName, $"[扭力转发异常] {ex.Message}");
            }
        }

        // 辅助方法
        private void AppendLog(ProcessName processName, string msg)
        {
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
                Log4netHelper.Error("未选择或未检测到扭力仪串口，放弃初始化");
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
                    rtbErrorLog.AppendToComponent($"[扭力串口] {msg}");
                else
                    Log4netHelper.LogTorque($"[扭力串口] {msg}");
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
                        rtbErrorLog.AppendToComponent($"[扭力串口] {msg}");
                    else
                        Log4netHelper.LogTorque($"[扭力串口] {msg}");
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
                    lblRunningStatus.Text = "点检扭力上传失败，接口返回null";
                    lblRunningStatus.ForeColor = Color.Red;
                    return;
                }

                if (response.Result.Equals(nameof(MyEnum.Result.FAIL), StringComparison.OrdinalIgnoreCase))
                {
                    string msg = $"点检扭力上传失败:{response.ErrorMessage}";
                    rtbErrorLog.AppendToComponent(msg);
                    lblRunningStatus.Text = "点检扭力上传失败";
                    lblRunningStatus.ForeColor = Color.Red;
                    return;
                }

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
            // -------- 保存后生效 --------
            EnableReportMachineStatus.Checked = systemInfo.ReportMachineStatus; // 勾选启用设备状态上传
            EnableReportMachineAlarm.Checked = systemInfo.ReportMachineAlarm;   // 勾选启用预警信息上传
            EnableReportRealTimeParam.Checked = systemInfo.ReportRealTimeParam; // 勾选启用实时参数上传
            EnableReportConfigParam.Checked = systemInfo.ReportConfigParam;     // 勾选启用关键参数上传
            txtBarcodeRule.Text = systemInfo.BarcodeRule;                       // 条码规则
            HeartbeatUploadRate.Text = systemInfo.HeartRate;                    // 心跳上传频率
            RealtimeArgsUploadRate.Text = systemInfo.RealTimeParamRate;         // 实时参数上传频率

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

        /// <summary>
        /// 尝试读取PLC Int16值，并输出读取到的值，最多重试3次。
        /// <para> resultValue = -1表示读取失败 </para>
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回 true，失败返回 false</returns>
        private bool TryReadInt16Value(string address, out int value)
        {
            for (int i = 0; i < 1; i++)
            {
                if (!isPlcConnected) continue;

                var result = _readWriteNet.ReadInt16(address);

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

                Thread.Sleep(50);
            }

            // 循环3次后仍然失败
            value = -1;
            return false;
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
            for (int i = 0; i < 3; i++)
            {
                if (!isPlcConnected) continue;

                var result = _readWriteNet.ReadString(address, Convert.ToUInt16(length));

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

                Thread.Sleep(50);
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
        private bool TryWriteInt16Value(string address, dynamic value)
        {
            if (_readWriteNet is null) return false;

            var result = _readWriteNet.Write(address, Convert.ToInt16(value));

            if (result.IsSuccess)
            {
                return true;
            }

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
                if (!isPlcConnected) continue;

                var result = await _readWriteNet.ReadInt32Async(address);

                if (result.IsSuccess)
                    return (true, result.Content);

                // ErrorCode < 0 属于通信失败，不属于读写失败
                if (result.ErrorCode < 0)
                    return (false, -1);

                await Task.Delay(50);
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
        /// <list processName="bullet">
        /// <item><description><c>isReadOk</c>: <c>true</c> 表示读取成功, <c>false</c> 表示3次尝试后依旧失败（包括超时）。</description></item>
        /// <item><description><c>resultValue</c>: 读取成功时的值。如果 <c>isReadOk</c> 为 <c>false</c>，则返回 -1。</description></item>
        /// </list>
        /// </returns>
        private async Task<(bool, short)> TryReadInt16Async(string address)
        {
            for (var i = 0; i < 3; i++)
            {
                if (!isPlcConnected || _readWriteNet == null)
                {
                    await Task.Delay(100);
                    continue;
                }

                var readTask = _readWriteNet.ReadInt16Async(address);
                var completedTask = await Task.WhenAny(readTask, Task.Delay(500));

                if (readTask != completedTask)
                {
                    await Task.Delay(50);
                    continue;
                }

                var result = await readTask;
                if (result.IsSuccess && result.ErrorCode >= 0) return (true, result.Content);

                await Task.Delay(100);
            }

            // 循环3次后仍然失败
            return (false, -1);
        }

        private async Task<bool> TryWriteInt16ValueAsync(string address, short value)
        {
            if (!isPlcConnected) return false;

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
                string processName = nameof(uploadManagerEntity.Name);

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

            string currentTraceId = ProductPassTraceContext.CurrentTraceId;
            if (!string.IsNullOrWhiteSpace(currentTraceId))
            {
                Log4netHelper.LogProductPass(
                    $"TraceId={currentTraceId} 进入错误处理，反馈地址={feedbackAddress}，反馈值={feedBackValue}，阻塞={isBlockingError}，消息={errorData.LogMessage}");
            }

            lock (_errorLock)
            {
                // 如果当前有错误正在显示，则排队
                if (existErrorInErrorTip || _currentActiveError != null)
                {
                    ErrorQueue.Enqueue(errorData);
                    return false;
                }

                // 如果没有错误，则立即显示当前错误
                ShowErrorToUi(errorData);
                return false;
            }
        }

        /// <summary>
        /// 展示错误到UI界面
        /// </summary>
        /// <param name="errorData"></param>
        private void ShowErrorToUi(ErrorEntity errorData)
        {
            _currentActiveError = errorData; // 记录当前错误上下文

            // 1.记录错误日志
            rtbErrorLog.AppendToComponent(errorData.LogMessage);
            lblStatusErrorTip.ExecuteSafely(c => { c.Text = errorData.UserMessage; c.ForeColor = Color.Red; });

            // 阻塞模式 && 阻塞错误
            if (isBlockingMode && errorData.IsBlockingError)
            {
                existErrorInErrorTip = true;     // 设置阻塞标志

                btnManualClear.ExecuteSafely(c => c.Visible = true);

                Log4netHelper.Error($"阻塞报警：{errorData.LogMessage}｜地址：{errorData.FeedBackAddress}");     // 1b. 将log级错误信息保存到本地日志（D:\KaifaLogs\程序异常）

            }
            // 放行模式 || 非阻塞错误
            else
            {
                Log4netHelper.Error($"错误提示: {errorData.LogMessage} | 地址：{errorData.FeedBackAddress}");  // 1b. 将log级错误信息保存到本地日志（D:\KaifaLogs\程序异常）

                // 1. 非阻塞直接反馈，无需手动清除
                // 2. 只有当有反馈地址存在 且 PLC 在线时才尝试写入
                if (!string.IsNullOrEmpty(errorData.FeedBackAddress) && isPlcConnected)
                {
                    // 使用异步Task写入PLC，绝不阻塞当前方法的执行和UI界面
                    //Task.Run(() => _readWriteNet.Write(errorData.FeedBackAddress, Convert.ToInt16(errorData.FeedbackValue)));
                    Task.Run(async () => await _readWriteNet.WriteAsync(errorData.FeedBackAddress, Convert.ToInt16(errorData.FeedbackValue)));
                }

                // 非阻塞错误处理完后，立即释放状态，检查队列
                ClearCurrentErrorAndCheckQueue();
            }
        }

        private async void ManualClear_Click(object sender, EventArgs e)
        {
            string feedbackAddress = null;
            short? feedbackValue = null;

            lock (_errorLock)
            {
                if (_currentActiveError != null)
                {
                    feedbackAddress = _currentActiveError.FeedBackAddress;
                    feedbackValue = _currentActiveError.FeedbackValue;
                }
            }

            // 存在需要反馈的地址
            if (!string.IsNullOrEmpty(feedbackAddress))
            {
                if (!isPlcConnected)
                {
                    MessageBox.Show($"无法清除错误：PLC当前未连接，请先检查网络通讯！", "通讯异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var writeTask = _readWriteNet.WriteAsync(feedbackAddress, Convert.ToInt16(feedbackValue));
                var completedTask = await Task.WhenAny(writeTask, Task.Delay(500));

                if (completedTask != writeTask)
                {
                    MessageBox.Show($"清除失败：写入PLC地址 {feedbackAddress} 超时无响应。", "复位失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await writeTask;
                if (!result.IsSuccess)
                {
                    MessageBox.Show($"清除失败：写入PLC地址 {feedbackAddress} 失败。\r\n错误码: {result.ErrorCode}\r\n原因: {result.Message}", "复位失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Log4netHelper.Info($"手动清除报警完成: {_currentActiveError.UserMessage}");

            // 2. 反馈成功 || 无地址错误
            ClearCurrentErrorAndCheckQueue();
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
            lblStatusErrorTip.ExecuteSafely(c => c.Text = string.Empty);
            lblRunningStatus.ExecuteSafely(c => c.Text = string.Empty);
            barCode.ExecuteSafely(c => c.Text = string.Empty);
            ToolingNumber.ExecuteSafely(c => c.Text = string.Empty);
            btnManualClear.ExecuteSafely(c => c.Visible = false);
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
            // -------- 保存后生效 --------
            systemInfo.ReportMachineStatus = EnableReportMachineStatus.Checked; // 勾选启用设备状态上传
            systemInfo.ReportMachineAlarm = EnableReportMachineAlarm.Checked;   // 勾选启用预警信息上传
            systemInfo.ReportRealTimeParam = EnableReportRealTimeParam.Checked; // 勾选启用实时参数上传
            systemInfo.ReportConfigParam = EnableReportConfigParam.Checked;     // 勾选启用关键参数上传
            systemInfo.BarcodeRule = txtBarcodeRule.Text;                       // 条码规则
            systemInfo.HeartRate = HeartbeatUploadRate.Text;                    // 心跳上传频率
            systemInfo.RealTimeParamRate = RealtimeArgsUploadRate.Text;         // 实时参数上传频率

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
                SaveSuccessRestartApp();
            }
            else
            {

                MessageBox.Show("保存失败");
                Load_ProductConfig();
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
            TorqueSerialClient.AutoRefreshComboBoxes(cmbCOM1, cmbCOM1);
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
