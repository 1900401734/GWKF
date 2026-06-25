
using System.Windows.Forms;

namespace MesDatas.Views
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.mySqlCommand1 = new MySql.Data.MySqlClient.MySqlCommand();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.PrinterSignal = new System.Windows.Forms.RichTextBox();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.rtbReadBarCode = new System.Windows.Forms.RichTextBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.UploadMes = new System.Windows.Forms.RichTextBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel25 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.printTemplatePath = new System.Windows.Forms.TextBox();
            this.printerName = new System.Windows.Forms.TextBox();
            this.printTest = new System.Windows.Forms.Button();
            this.btnShowPath = new System.Windows.Forms.Button();
            this.btnChangePath = new System.Windows.Forms.Button();
            this.printSetSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Device3 = new System.Windows.Forms.TextBox();
            this.Security3 = new System.Windows.Forms.TextBox();
            this.MesKey3 = new System.Windows.Forms.TextBox();
            this.Station3 = new System.Windows.Forms.TextBox();
            this.Process3 = new System.Windows.Forms.TextBox();
            this.Line3 = new System.Windows.Forms.TextBox();
            this.label114 = new System.Windows.Forms.Label();
            this.label119 = new System.Windows.Forms.Label();
            this.label115 = new System.Windows.Forms.Label();
            this.label116 = new System.Windows.Forms.Label();
            this.label118 = new System.Windows.Forms.Label();
            this.label117 = new System.Windows.Forms.Label();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.btnSaveAtAssemblyMachine = new System.Windows.Forms.Button();
            this.Device2 = new System.Windows.Forms.TextBox();
            this.Security2 = new System.Windows.Forms.TextBox();
            this.MesKey2 = new System.Windows.Forms.TextBox();
            this.Station2 = new System.Windows.Forms.TextBox();
            this.Process2 = new System.Windows.Forms.TextBox();
            this.Line2 = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel32 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave_KeyArgs = new System.Windows.Forms.Button();
            this.keyArgsRefreshButton = new System.Windows.Forms.Button();
            this.dgvKeyArgs = new System.Windows.Forms.DataGridView();
            this.copyDataGatherTable = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel31 = new System.Windows.Forms.TableLayoutPanel();
            this.errorPreserveRefreshButton = new System.Windows.Forms.Button();
            this.dgvErrorPreserve = new System.Windows.Forms.DataGridView();
            this.btnSave_WarmError = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel29 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave_dgvDataAcquisition = new System.Windows.Forms.Button();
            this.dgvDataAcquisition = new System.Windows.Forms.DataGridView();
            this.dataGatherBoardRefreshButton = new System.Windows.Forms.Button();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel30 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave_dgvDefect = new System.Windows.Forms.Button();
            this.deviceDefectsRefreshButton = new System.Windows.Forms.Button();
            this.dgvDeviceDefects = new System.Windows.Forms.DataGridView();
            this.label60 = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.panel9 = new System.Windows.Forms.Panel();
            this.tlpProductConfig = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.BarcodeRule = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.EnableReportConfigParam = new System.Windows.Forms.CheckBox();
            this.EnableReportRealTimeParam = new System.Windows.Forms.CheckBox();
            this.EnableReportMachineAlarm = new System.Windows.Forms.CheckBox();
            this.label52 = new System.Windows.Forms.Label();
            this.RealtimeArgsUploadRate = new System.Windows.Forms.TextBox();
            this.EnableReportMachineStatus = new System.Windows.Forms.CheckBox();
            this.HeartbeatUploadRate = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.cboProductMode = new System.Windows.Forms.ComboBox();
            this.cboBanUpload = new System.Windows.Forms.ComboBox();
            this.cboEnforcePass = new System.Windows.Forms.ComboBox();
            this.label61 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.EnableUpperTooling = new System.Windows.Forms.CheckBox();
            this.EnableGetNextBoard = new System.Windows.Forms.CheckBox();
            this.EnableTypeChangedVerify = new System.Windows.Forms.CheckBox();
            this.EnablePrintCode = new System.Windows.Forms.CheckBox();
            this.chkBanFixtureUpload = new System.Windows.Forms.CheckBox();
            this.EnableResultUpload = new System.Windows.Forms.CheckBox();
            this.EnableFluentVerify = new System.Windows.Forms.CheckBox();
            this.BanReadBarcode = new System.Windows.Forms.CheckBox();
            this.EnableBarcodeRuleVerify = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel33 = new System.Windows.Forms.TableLayoutPanel();
            this.grpTorqueConfig = new System.Windows.Forms.GroupBox();
            this.grpTorqueControllerConfig2 = new System.Windows.Forms.GroupBox();
            this.txtControllerPort2 = new System.Windows.Forms.TextBox();
            this.txtControllerIP2 = new System.Windows.Forms.TextBox();
            this.lblIP2 = new System.Windows.Forms.Label();
            this.lblPort2 = new System.Windows.Forms.Label();
            this.grpTorqueControllerConfig1 = new System.Windows.Forms.GroupBox();
            this.txtControllerPort1 = new System.Windows.Forms.TextBox();
            this.txtControllerIP1 = new System.Windows.Forms.TextBox();
            this.lblPort1 = new System.Windows.Forms.Label();
            this.lblIP1 = new System.Windows.Forms.Label();
            this.ProductConfig_SaveButton = new System.Windows.Forms.Button();
            this.grpTorqueMeterConfig = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cmbCOM2 = new System.Windows.Forms.ComboBox();
            this.lblCOM2 = new System.Windows.Forms.Label();
            this.cmbCOM1 = new System.Windows.Forms.ComboBox();
            this.lblCOM1 = new System.Windows.Forms.Label();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.dgvPrintDirectory = new System.Windows.Forms.DataGridView();
            this.printRefresh = new System.Windows.Forms.Button();
            this.lblTips = new System.Windows.Forms.Label();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel28 = new System.Windows.Forms.TableLayoutPanel();
            this.txtToqueMin3 = new System.Windows.Forms.TextBox();
            this.label123 = new System.Windows.Forms.Label();
            this.label124 = new System.Windows.Forms.Label();
            this.txtTorqueResult3 = new System.Windows.Forms.TextBox();
            this.label125 = new System.Windows.Forms.Label();
            this.txtTorqueValue3 = new System.Windows.Forms.TextBox();
            this.label126 = new System.Windows.Forms.Label();
            this.txtToqueMax3 = new System.Windows.Forms.TextBox();
            this.label127 = new System.Windows.Forms.Label();
            this.txtRequest3 = new System.Windows.Forms.TextBox();
            this.label128 = new System.Windows.Forms.Label();
            this.txtAcknowledge3 = new System.Windows.Forms.TextBox();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel27 = new System.Windows.Forms.TableLayoutPanel();
            this.txtToqueMin1 = new System.Windows.Forms.TextBox();
            this.label122 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.txtTorqueResult1 = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.txtTorqueValue1 = new System.Windows.Forms.TextBox();
            this.label67 = new System.Windows.Forms.Label();
            this.txtToqueMax1 = new System.Windows.Forms.TextBox();
            this.label120 = new System.Windows.Forms.Label();
            this.txtRequest1 = new System.Windows.Forms.TextBox();
            this.label121 = new System.Windows.Forms.Label();
            this.txtAcknowledge1 = new System.Windows.Forms.TextBox();
            this.groupBox28 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel23 = new System.Windows.Forms.TableLayoutPanel();
            this.txtRecoverySignal = new System.Windows.Forms.TextBox();
            this.label113 = new System.Windows.Forms.Label();
            this.txtContinueProduce = new System.Windows.Forms.TextBox();
            this.label112 = new System.Windows.Forms.Label();
            this.txtProgramNameLength = new System.Windows.Forms.TextBox();
            this.label111 = new System.Windows.Forms.Label();
            this.txtDeviceStatus = new System.Windows.Forms.TextBox();
            this.label110 = new System.Windows.Forms.Label();
            this.txtNotGoodsProducts = new System.Windows.Forms.TextBox();
            this.label109 = new System.Windows.Forms.Label();
            this.label97 = new System.Windows.Forms.Label();
            this.label100 = new System.Windows.Forms.Label();
            this.label101 = new System.Windows.Forms.Label();
            this.label102 = new System.Windows.Forms.Label();
            this.txtGoodsProducts = new System.Windows.Forms.TextBox();
            this.txtProduceCount = new System.Windows.Forms.TextBox();
            this.txtDeviceProgramName = new System.Windows.Forms.TextBox();
            this.txtProductType = new System.Windows.Forms.TextBox();
            this.label103 = new System.Windows.Forms.Label();
            this.txtProductTypeLength = new System.Windows.Forms.TextBox();
            this.label104 = new System.Windows.Forms.Label();
            this.txtBarcodeRule = new System.Windows.Forms.TextBox();
            this.label105 = new System.Windows.Forms.Label();
            this.txtBarcodeRuleLength = new System.Windows.Forms.TextBox();
            this.label106 = new System.Windows.Forms.Label();
            this.txtModelSwitch = new System.Windows.Forms.TextBox();
            this.label107 = new System.Windows.Forms.Label();
            this.txtPlcHeartBeat = new System.Windows.Forms.TextBox();
            this.label108 = new System.Windows.Forms.Label();
            this.txtPcHeartBeat = new System.Windows.Forms.TextBox();
            this.groupBox27 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel22 = new System.Windows.Forms.TableLayoutPanel();
            this.label95 = new System.Windows.Forms.Label();
            this.label96 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.txtPrintTrigger = new System.Windows.Forms.TextBox();
            this.txtPrintFeedback = new System.Windows.Forms.TextBox();
            this.txtBarcodeToPrint = new System.Windows.Forms.TextBox();
            this.label99 = new System.Windows.Forms.Label();
            this.txtBarcodeToPrintLength = new System.Windows.Forms.TextBox();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel21 = new System.Windows.Forms.TableLayoutPanel();
            this.label90 = new System.Windows.Forms.Label();
            this.txtTriggerUpload3 = new System.Windows.Forms.TextBox();
            this.label91 = new System.Windows.Forms.Label();
            this.txtFeedback3 = new System.Windows.Forms.TextBox();
            this.label92 = new System.Windows.Forms.Label();
            this.txtProductResult3 = new System.Windows.Forms.TextBox();
            this.label93 = new System.Windows.Forms.Label();
            this.txtBarcodeToUpload3 = new System.Windows.Forms.TextBox();
            this.label94 = new System.Windows.Forms.Label();
            this.txtBarcodeToUploadLength3 = new System.Windows.Forms.TextBox();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel20 = new System.Windows.Forms.TableLayoutPanel();
            this.label85 = new System.Windows.Forms.Label();
            this.txtTriggerUpload2 = new System.Windows.Forms.TextBox();
            this.label86 = new System.Windows.Forms.Label();
            this.txtFeedback2 = new System.Windows.Forms.TextBox();
            this.label87 = new System.Windows.Forms.Label();
            this.txtProductResult2 = new System.Windows.Forms.TextBox();
            this.label88 = new System.Windows.Forms.Label();
            this.txtBarcodeToUpload2 = new System.Windows.Forms.TextBox();
            this.label89 = new System.Windows.Forms.Label();
            this.txtBarcodeToUploadLength2 = new System.Windows.Forms.TextBox();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel19 = new System.Windows.Forms.TableLayoutPanel();
            this.label79 = new System.Windows.Forms.Label();
            this.txtTriggerUpload1 = new System.Windows.Forms.TextBox();
            this.label81 = new System.Windows.Forms.Label();
            this.txtFeedback1 = new System.Windows.Forms.TextBox();
            this.label82 = new System.Windows.Forms.Label();
            this.txtProductResult1 = new System.Windows.Forms.TextBox();
            this.label83 = new System.Windows.Forms.Label();
            this.txtBarcodeToUpload1 = new System.Windows.Forms.TextBox();
            this.label84 = new System.Windows.Forms.Label();
            this.txtBarcodeToUploadLength1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel18 = new System.Windows.Forms.TableLayoutPanel();
            this.label68 = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.label72 = new System.Windows.Forms.Label();
            this.txtHasBarcodeTag = new System.Windows.Forms.TextBox();
            this.txtBarcodeVerifyTag = new System.Windows.Forms.TextBox();
            this.txtBarcodeType = new System.Windows.Forms.TextBox();
            this.txtPlcScanned = new System.Windows.Forms.TextBox();
            this.label78 = new System.Windows.Forms.Label();
            this.txtScannedLength = new System.Windows.Forms.TextBox();
            this.label73 = new System.Windows.Forms.Label();
            this.txtPanalizationBarcode = new System.Windows.Forms.TextBox();
            this.label77 = new System.Windows.Forms.Label();
            this.txtPanalizationLength = new System.Windows.Forms.TextBox();
            this.label74 = new System.Windows.Forms.Label();
            this.txtManualInput = new System.Windows.Forms.TextBox();
            this.label75 = new System.Windows.Forms.Label();
            this.txtManualBarcode = new System.Windows.Forms.TextBox();
            this.label76 = new System.Windows.Forms.Label();
            this.txtManualLength = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.接口设置panel = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.Url_ToolingChange = new System.Windows.Forms.TextBox();
            this.Url_RealtimeArgs = new System.Windows.Forms.TextBox();
            this.Url_KeyArgs = new System.Windows.Forms.TextBox();
            this.Url_ErrorInterface = new System.Windows.Forms.TextBox();
            this.Url_DeviceStatus = new System.Windows.Forms.TextBox();
            this.Url_Heartbeat = new System.Windows.Forms.TextBox();
            this.Url_GetProductName = new System.Windows.Forms.TextBox();
            this.Url_FTPMessGet = new System.Windows.Forms.TextBox();
            this.Url_DataUpload = new System.Windows.Forms.TextBox();
            this.Url_RouteCheck = new System.Windows.Forms.TextBox();
            this.UrlPanelization = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.url = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.Line = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.Process = new System.Windows.Forms.TextBox();
            this.Station = new System.Windows.Forms.TextBox();
            this.MesKey = new System.Windows.Forms.TextBox();
            this.Security = new System.Windows.Forms.TextBox();
            this.Device = new System.Windows.Forms.TextBox();
            this.PlanNo = new System.Windows.Forms.TextBox();
            this.FTPlog = new System.Windows.Forms.TextBox();
            this.FTPPIC = new System.Windows.Forms.TextBox();
            this.FTPID = new System.Windows.Forms.TextBox();
            this.FTPCODE = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.Url_Token = new System.Windows.Forms.TextBox();
            this.btnSave_InterfaceConfig = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.SWVer = new System.Windows.Forms.TextBox();
            this.HWVer = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.Url_PrintTemplate = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.LocalFilePath = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvUserInfo = new System.Windows.Forms.DataGridView();
            this.panel6 = new System.Windows.Forms.Panel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.UPwd = new System.Windows.Forms.TextBox();
            this.UId = new System.Windows.Forms.TextBox();
            this.Priv = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.UserRefresh = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.UserAdd = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.系统设置panel3 = new System.Windows.Forms.Panel();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.panel20 = new System.Windows.Forms.Panel();
            this.dgvProductModel = new System.Windows.Forms.DataGridView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.ImportFile = new System.Windows.Forms.Button();
            this.changeTypeRefresh = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.panel19 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.SaveDataBase = new System.Windows.Forms.Button();
            this.deviceDataBase = new System.Windows.Forms.ComboBox();
            this.label59 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.PlcInputAutoSave = new System.Windows.Forms.CheckBox();
            this.PlcConnectType = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.btnStartTask = new System.Windows.Forms.Button();
            this.EndTask = new System.Windows.Forms.Button();
            this.ManualConnect = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PlcPort = new System.Windows.Forms.TextBox();
            this.PlcIP = new System.Windows.Forms.TextBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.DeviceName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.OtherSettingsSave = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.rtbErrorLog = new System.Windows.Forms.RichTextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.整页面 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.InterfaceTipLabel = new System.Windows.Forms.Label();
            this.DeviceStatusDisplay = new System.Windows.Forms.Label();
            this.DeviceStatusSignalLight = new System.Windows.Forms.Label();
            this.PlcTipLabel = new System.Windows.Forms.Label();
            this.PlcSignalLight = new System.Windows.Forms.Label();
            this.InterfaceSignalLight = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl_UploadData = new System.Windows.Forms.TabControl();
            this.tabPageResult1 = new System.Windows.Forms.TabPage();
            this.dgvResult1 = new System.Windows.Forms.DataGridView();
            this.tabPageResult2 = new System.Windows.Forms.TabPage();
            this.dgvResult2 = new System.Windows.Forms.DataGridView();
            this.tabPageResult3 = new System.Windows.Forms.TabPage();
            this.dgvResult3 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txtProductModel = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtTotalQuality = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtOkQuality = new System.Windows.Forms.TextBox();
            this.txtNgQuanlity = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtYieldRate = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.OrderNo = new System.Windows.Forms.TextBox();
            this.ManualChangeMO = new System.Windows.Forms.Button();
            this.OrderNum = new System.Windows.Forms.TextBox();
            this.label80 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.btnLogOut = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.lblProductResult = new System.Windows.Forms.Label();
            this.ToolingNumberPanel = new System.Windows.Forms.GroupBox();
            this.ToolingNumber = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.条码数据 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel24 = new System.Windows.Forms.TableLayoutPanel();
            this.barCode = new System.Windows.Forms.Label();
            this.btnManualInputBarcode = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel16 = new System.Windows.Forms.TableLayoutPanel();
            this.ManualRecovery = new System.Windows.Forms.Button();
            this.lblRunningStatus = new System.Windows.Forms.Label();
            this.groupboxx = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel17 = new System.Windows.Forms.TableLayoutPanel();
            this.btnBlockMode = new System.Windows.Forms.Button();
            this.btnManualClear = new System.Windows.Forms.Button();
            this.lblStatusErrorTip = new System.Windows.Forms.Label();
            this.TabContorl = new System.Windows.Forms.TabControl();
            this.tabPageTorqueMonitor = new System.Windows.Forms.TabPage();
            this.label63 = new System.Windows.Forms.Label();
            this.tlpScan_ASSY = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox31 = new System.Windows.Forms.GroupBox();
            this.lblAssyVal = new System.Windows.Forms.Label();
            this.label130 = new System.Windows.Forms.Label();
            this.lblAssyMax = new System.Windows.Forms.Label();
            this.label129 = new System.Windows.Forms.Label();
            this.lblAssyRes = new System.Windows.Forms.Label();
            this.lblAssyMin = new System.Windows.Forms.Label();
            this.label131 = new System.Windows.Forms.Label();
            this.label132 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.ASSY = new System.Windows.Forms.Label();
            this.tlpTorqueMonitor = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.lblBaRes = new System.Windows.Forms.Label();
            this.lblBaMin = new System.Windows.Forms.Label();
            this.lblBaVal = new System.Windows.Forms.Label();
            this.lblBaMax = new System.Windows.Forms.Label();
            this.label135 = new System.Windows.Forms.Label();
            this.label133 = new System.Windows.Forms.Label();
            this.label136 = new System.Windows.Forms.Label();
            this.label134 = new System.Windows.Forms.Label();
            this.BA = new System.Windows.Forms.Label();
            this.rtbBALog = new System.Windows.Forms.RichTextBox();
            this.rtbASSYLog = new System.Windows.Forms.RichTextBox();
            this.tlpScrew_BA = new System.Windows.Forms.TableLayoutPanel();
            this.panelAS = new System.Windows.Forms.Panel();
            this.panelASSY = new System.Windows.Forms.Panel();
            this.panelTorqueMeter1 = new System.Windows.Forms.Panel();
            this.tlpTorqueMeter1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTorque1 = new System.Windows.Forms.Label();
            this.lblSerialLight1 = new System.Windows.Forms.Label();
            this.grpTorqueMeter1 = new System.Windows.Forms.GroupBox();
            this.label138 = new System.Windows.Forms.Label();
            this.label140 = new System.Windows.Forms.Label();
            this.label143 = new System.Windows.Forms.Label();
            this.label144 = new System.Windows.Forms.Label();
            this.rtbTorqueMeter1 = new System.Windows.Forms.RichTextBox();
            this.panelTorqueMonitor2 = new System.Windows.Forms.Panel();
            this.tlpTorqueMeter2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTorque2 = new System.Windows.Forms.Label();
            this.lblSerialLight2 = new System.Windows.Forms.Label();
            this.grpTorqueMonitor2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label137 = new System.Windows.Forms.Label();
            this.label139 = new System.Windows.Forms.Label();
            this.label141 = new System.Windows.Forms.Label();
            this.rtbTorqueMeter2 = new System.Windows.Forms.RichTextBox();
            this.tabPage9.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.tableLayoutPanel25.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel32.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeyArgs)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel31.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrorPreserve)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.tableLayoutPanel29.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataAcquisition)).BeginInit();
            this.groupBox11.SuspendLayout();
            this.tableLayoutPanel30.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeviceDefects)).BeginInit();
            this.tabPage6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage10.SuspendLayout();
            this.panel9.SuspendLayout();
            this.tlpProductConfig.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel33.SuspendLayout();
            this.grpTorqueConfig.SuspendLayout();
            this.grpTorqueControllerConfig2.SuspendLayout();
            this.grpTorqueControllerConfig1.SuspendLayout();
            this.grpTorqueMeterConfig.SuspendLayout();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrintDirectory)).BeginInit();
            this.tabPage11.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.tableLayoutPanel28.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.tableLayoutPanel27.SuspendLayout();
            this.groupBox28.SuspendLayout();
            this.tableLayoutPanel23.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.tableLayoutPanel22.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.tableLayoutPanel21.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.tableLayoutPanel20.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.tableLayoutPanel19.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tableLayoutPanel18.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.接口设置panel.SuspendLayout();
            this.panel8.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserInfo)).BeginInit();
            this.panel6.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.系统设置panel3.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            this.panel20.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductModel)).BeginInit();
            this.panel5.SuspendLayout();
            this.panel19.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.整页面.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl_UploadData.SuspendLayout();
            this.tabPageResult1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult1)).BeginInit();
            this.tabPageResult2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult2)).BeginInit();
            this.tabPageResult3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult3)).BeginInit();
            this.tableLayoutPanel12.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.ToolingNumberPanel.SuspendLayout();
            this.条码数据.SuspendLayout();
            this.tableLayoutPanel24.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel16.SuspendLayout();
            this.groupboxx.SuspendLayout();
            this.tableLayoutPanel17.SuspendLayout();
            this.TabContorl.SuspendLayout();
            this.tabPageTorqueMonitor.SuspendLayout();
            this.tlpScan_ASSY.SuspendLayout();
            this.groupBox31.SuspendLayout();
            this.tlpTorqueMonitor.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.tlpScrew_BA.SuspendLayout();
            this.panelAS.SuspendLayout();
            this.panelASSY.SuspendLayout();
            this.panelTorqueMeter1.SuspendLayout();
            this.tlpTorqueMeter1.SuspendLayout();
            this.grpTorqueMeter1.SuspendLayout();
            this.panelTorqueMonitor2.SuspendLayout();
            this.tlpTorqueMeter2.SuspendLayout();
            this.grpTorqueMonitor2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mySqlCommand1
            // 
            this.mySqlCommand1.CacheAge = 0;
            this.mySqlCommand1.Connection = null;
            this.mySqlCommand1.EnableCaching = false;
            this.mySqlCommand1.Transaction = null;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.tableLayoutPanel13);
            this.tabPage9.Location = new System.Drawing.Point(4, 4);
            this.tabPage9.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage9.Size = new System.Drawing.Size(1894, 984);
            this.tabPage9.TabIndex = 17;
            this.tabPage9.Text = "生产日志";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 3;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel13.Controls.Add(this.groupBox23, 2, 0);
            this.tableLayoutPanel13.Controls.Add(this.groupBox21, 0, 0);
            this.tableLayoutPanel13.Controls.Add(this.groupBox22, 1, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel13.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 1;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(1886, 976);
            this.tableLayoutPanel13.TabIndex = 2;
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.PrinterSignal);
            this.groupBox23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox23.Location = new System.Drawing.Point(1260, 4);
            this.groupBox23.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox23.Size = new System.Drawing.Size(622, 968);
            this.groupBox23.TabIndex = 2;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "标签打印日志";
            // 
            // PrinterSignal
            // 
            this.PrinterSignal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PrinterSignal.Location = new System.Drawing.Point(4, 27);
            this.PrinterSignal.Margin = new System.Windows.Forms.Padding(4);
            this.PrinterSignal.Name = "PrinterSignal";
            this.PrinterSignal.Size = new System.Drawing.Size(614, 937);
            this.PrinterSignal.TabIndex = 0;
            this.PrinterSignal.Text = "";
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.rtbReadBarCode);
            this.groupBox21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox21.Location = new System.Drawing.Point(4, 4);
            this.groupBox21.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox21.Size = new System.Drawing.Size(620, 968);
            this.groupBox21.TabIndex = 0;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "流程检查日志";
            // 
            // rtbReadBarCode
            // 
            this.rtbReadBarCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbReadBarCode.Location = new System.Drawing.Point(4, 27);
            this.rtbReadBarCode.Margin = new System.Windows.Forms.Padding(4);
            this.rtbReadBarCode.Name = "rtbReadBarCode";
            this.rtbReadBarCode.Size = new System.Drawing.Size(612, 937);
            this.rtbReadBarCode.TabIndex = 0;
            this.rtbReadBarCode.Text = "";
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.UploadMes);
            this.groupBox22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox22.Location = new System.Drawing.Point(632, 4);
            this.groupBox22.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox22.Size = new System.Drawing.Size(620, 968);
            this.groupBox22.TabIndex = 1;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "产品过站日志";
            // 
            // UploadMes
            // 
            this.UploadMes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UploadMes.Location = new System.Drawing.Point(4, 27);
            this.UploadMes.Margin = new System.Windows.Forms.Padding(4);
            this.UploadMes.Name = "UploadMes";
            this.UploadMes.Size = new System.Drawing.Size(612, 937);
            this.UploadMes.TabIndex = 0;
            this.UploadMes.Text = "";
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.tableLayoutPanel6);
            this.tabPage8.Location = new System.Drawing.Point(4, 4);
            this.tabPage8.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage8.Size = new System.Drawing.Size(1894, 984);
            this.tabPage8.TabIndex = 15;
            this.tabPage8.Text = "装配机配置";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.groupBox15, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel9, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.7551F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.2449F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1890, 980);
            this.tableLayoutPanel6.TabIndex = 7;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.tableLayoutPanel25);
            this.groupBox15.Controls.Add(this.printTest);
            this.groupBox15.Controls.Add(this.btnShowPath);
            this.groupBox15.Controls.Add(this.btnChangePath);
            this.groupBox15.Controls.Add(this.printSetSave);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(2, 2);
            this.groupBox15.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox15.Size = new System.Drawing.Size(1886, 169);
            this.groupBox15.TabIndex = 0;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "打印设置";
            // 
            // tableLayoutPanel25
            // 
            this.tableLayoutPanel25.ColumnCount = 2;
            this.tableLayoutPanel25.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel25.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel25.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel25.Controls.Add(this.label38, 0, 1);
            this.tableLayoutPanel25.Controls.Add(this.printTemplatePath, 1, 1);
            this.tableLayoutPanel25.Controls.Add(this.printerName, 1, 0);
            this.tableLayoutPanel25.Location = new System.Drawing.Point(86, 50);
            this.tableLayoutPanel25.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel25.Name = "tableLayoutPanel25";
            this.tableLayoutPanel25.RowCount = 2;
            this.tableLayoutPanel25.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel25.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel25.Size = new System.Drawing.Size(752, 114);
            this.tableLayoutPanel25.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 57);
            this.label1.TabIndex = 2;
            this.label1.Text = "打印机名称:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label38
            // 
            this.label38.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label38.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F);
            this.label38.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label38.Location = new System.Drawing.Point(2, 57);
            this.label38.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(116, 57);
            this.label38.TabIndex = 6;
            this.label38.Text = "模板路径:";
            this.label38.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // printTemplatePath
            // 
            this.printTemplatePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printTemplatePath.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F);
            this.printTemplatePath.Location = new System.Drawing.Point(122, 59);
            this.printTemplatePath.Margin = new System.Windows.Forms.Padding(2);
            this.printTemplatePath.Name = "printTemplatePath";
            this.printTemplatePath.Size = new System.Drawing.Size(628, 31);
            this.printTemplatePath.TabIndex = 5;
            // 
            // printerName
            // 
            this.printerName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printerName.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F);
            this.printerName.Location = new System.Drawing.Point(122, 2);
            this.printerName.Margin = new System.Windows.Forms.Padding(2);
            this.printerName.Name = "printerName";
            this.printerName.Size = new System.Drawing.Size(628, 31);
            this.printerName.TabIndex = 0;
            // 
            // printTest
            // 
            this.printTest.Font = new System.Drawing.Font("Microsoft YaHei", 12.8F);
            this.printTest.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.printTest.Location = new System.Drawing.Point(868, 42);
            this.printTest.Margin = new System.Windows.Forms.Padding(4);
            this.printTest.Name = "printTest";
            this.printTest.Size = new System.Drawing.Size(155, 49);
            this.printTest.TabIndex = 7;
            this.printTest.Text = "打印测试";
            this.printTest.UseVisualStyleBackColor = true;
            this.printTest.Click += new System.EventHandler(this.printTest_Click);
            // 
            // btnShowPath
            // 
            this.btnShowPath.AutoSize = true;
            this.btnShowPath.Font = new System.Drawing.Font("Microsoft YaHei", 12.8F);
            this.btnShowPath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnShowPath.Location = new System.Drawing.Point(1083, 100);
            this.btnShowPath.Margin = new System.Windows.Forms.Padding(2);
            this.btnShowPath.Name = "btnShowPath";
            this.btnShowPath.Size = new System.Drawing.Size(194, 50);
            this.btnShowPath.TabIndex = 1;
            this.btnShowPath.Text = "打开文件位置";
            this.btnShowPath.UseVisualStyleBackColor = true;
            this.btnShowPath.Click += new System.EventHandler(this.btnShowPath_Click);
            // 
            // btnChangePath
            // 
            this.btnChangePath.AutoSize = true;
            this.btnChangePath.Font = new System.Drawing.Font("Microsoft YaHei", 12.8F);
            this.btnChangePath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnChangePath.Location = new System.Drawing.Point(868, 100);
            this.btnChangePath.Margin = new System.Windows.Forms.Padding(2);
            this.btnChangePath.Name = "btnChangePath";
            this.btnChangePath.Size = new System.Drawing.Size(194, 50);
            this.btnChangePath.TabIndex = 1;
            this.btnChangePath.Text = "变更存放路径";
            this.btnChangePath.UseVisualStyleBackColor = true;
            this.btnChangePath.Click += new System.EventHandler(this.btnChangePath_Click);
            // 
            // printSetSave
            // 
            this.printSetSave.Font = new System.Drawing.Font("Microsoft YaHei", 12.8F);
            this.printSetSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.printSetSave.Location = new System.Drawing.Point(1083, 43);
            this.printSetSave.Margin = new System.Windows.Forms.Padding(2);
            this.printSetSave.Name = "printSetSave";
            this.printSetSave.Size = new System.Drawing.Size(155, 46);
            this.printSetSave.TabIndex = 1;
            this.printSetSave.Text = "保存";
            this.printSetSave.UseVisualStyleBackColor = true;
            this.printSetSave.Visible = false;
            this.printSetSave.Click += new System.EventHandler(this.SaveAtPrintSet_Click);
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Controls.Add(this.groupBox9, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.groupBox19, 0, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(4, 177);
            this.tableLayoutPanel9.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(1882, 799);
            this.tableLayoutPanel9.TabIndex = 1;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.button1);
            this.groupBox9.Controls.Add(this.Device3);
            this.groupBox9.Controls.Add(this.Security3);
            this.groupBox9.Controls.Add(this.MesKey3);
            this.groupBox9.Controls.Add(this.Station3);
            this.groupBox9.Controls.Add(this.Process3);
            this.groupBox9.Controls.Add(this.Line3);
            this.groupBox9.Controls.Add(this.label114);
            this.groupBox9.Controls.Add(this.label119);
            this.groupBox9.Controls.Add(this.label115);
            this.groupBox9.Controls.Add(this.label116);
            this.groupBox9.Controls.Add(this.label118);
            this.groupBox9.Controls.Add(this.label117);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(945, 4);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox9.Size = new System.Drawing.Size(933, 791);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "装配机工序3：Screw-BA";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft YaHei", 12.8F);
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(136, 461);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 55);
            this.button1.TabIndex = 24;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Save_Process3_Click);
            // 
            // Device3
            // 
            this.Device3.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Device3.Location = new System.Drawing.Point(136, 392);
            this.Device3.Margin = new System.Windows.Forms.Padding(4);
            this.Device3.Name = "Device3";
            this.Device3.Size = new System.Drawing.Size(546, 36);
            this.Device3.TabIndex = 23;
            // 
            // Security3
            // 
            this.Security3.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Security3.Location = new System.Drawing.Point(136, 319);
            this.Security3.Margin = new System.Windows.Forms.Padding(4);
            this.Security3.Name = "Security3";
            this.Security3.Size = new System.Drawing.Size(546, 36);
            this.Security3.TabIndex = 22;
            // 
            // MesKey3
            // 
            this.MesKey3.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.MesKey3.Location = new System.Drawing.Point(136, 248);
            this.MesKey3.Margin = new System.Windows.Forms.Padding(4);
            this.MesKey3.Name = "MesKey3";
            this.MesKey3.Size = new System.Drawing.Size(546, 36);
            this.MesKey3.TabIndex = 21;
            // 
            // Station3
            // 
            this.Station3.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Station3.Location = new System.Drawing.Point(136, 176);
            this.Station3.Margin = new System.Windows.Forms.Padding(4);
            this.Station3.Name = "Station3";
            this.Station3.Size = new System.Drawing.Size(546, 36);
            this.Station3.TabIndex = 20;
            // 
            // Process3
            // 
            this.Process3.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Process3.Location = new System.Drawing.Point(136, 110);
            this.Process3.Margin = new System.Windows.Forms.Padding(4);
            this.Process3.Name = "Process3";
            this.Process3.Size = new System.Drawing.Size(546, 36);
            this.Process3.TabIndex = 19;
            // 
            // Line3
            // 
            this.Line3.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Line3.Location = new System.Drawing.Point(136, 46);
            this.Line3.Margin = new System.Windows.Forms.Padding(4);
            this.Line3.Name = "Line3";
            this.Line3.Size = new System.Drawing.Size(546, 36);
            this.Line3.TabIndex = 18;
            // 
            // label114
            // 
            this.label114.AutoSize = true;
            this.label114.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label114.Location = new System.Drawing.Point(82, 50);
            this.label114.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label114.Name = "label114";
            this.label114.Size = new System.Drawing.Size(46, 24);
            this.label114.TabIndex = 12;
            this.label114.Text = "线体";
            // 
            // label119
            // 
            this.label119.AutoSize = true;
            this.label119.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label119.Location = new System.Drawing.Point(64, 400);
            this.label119.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label119.Name = "label119";
            this.label119.Size = new System.Drawing.Size(64, 24);
            this.label119.TabIndex = 17;
            this.label119.Text = "设备名";
            // 
            // label115
            // 
            this.label115.AutoSize = true;
            this.label115.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label115.Location = new System.Drawing.Point(82, 114);
            this.label115.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label115.Name = "label115";
            this.label115.Size = new System.Drawing.Size(46, 24);
            this.label115.TabIndex = 13;
            this.label115.Text = "工序";
            // 
            // label116
            // 
            this.label116.AutoSize = true;
            this.label116.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label116.Location = new System.Drawing.Point(82, 184);
            this.label116.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label116.Name = "label116";
            this.label116.Size = new System.Drawing.Size(46, 24);
            this.label116.TabIndex = 14;
            this.label116.Text = "站点";
            // 
            // label118
            // 
            this.label118.AutoSize = true;
            this.label118.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label118.Location = new System.Drawing.Point(50, 328);
            this.label118.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label118.Name = "label118";
            this.label118.Size = new System.Drawing.Size(78, 24);
            this.label118.TabIndex = 16;
            this.label118.Text = "MD加密";
            // 
            // label117
            // 
            this.label117.AutoSize = true;
            this.label117.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label117.Location = new System.Drawing.Point(44, 256);
            this.label117.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label117.Name = "label117";
            this.label117.Size = new System.Drawing.Size(84, 24);
            this.label117.TabIndex = 15;
            this.label117.Text = "MES账号";
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.btnSaveAtAssemblyMachine);
            this.groupBox19.Controls.Add(this.Device2);
            this.groupBox19.Controls.Add(this.Security2);
            this.groupBox19.Controls.Add(this.MesKey2);
            this.groupBox19.Controls.Add(this.Station2);
            this.groupBox19.Controls.Add(this.Process2);
            this.groupBox19.Controls.Add(this.Line2);
            this.groupBox19.Controls.Add(this.label58);
            this.groupBox19.Controls.Add(this.label57);
            this.groupBox19.Controls.Add(this.label56);
            this.groupBox19.Controls.Add(this.label55);
            this.groupBox19.Controls.Add(this.label53);
            this.groupBox19.Controls.Add(this.label42);
            this.groupBox19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox19.Location = new System.Drawing.Point(4, 4);
            this.groupBox19.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox19.Size = new System.Drawing.Size(933, 791);
            this.groupBox19.TabIndex = 0;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "装配机工序2：Weight";
            // 
            // btnSaveAtAssemblyMachine
            // 
            this.btnSaveAtAssemblyMachine.Font = new System.Drawing.Font("Microsoft YaHei", 12.8F);
            this.btnSaveAtAssemblyMachine.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSaveAtAssemblyMachine.Location = new System.Drawing.Point(136, 461);
            this.btnSaveAtAssemblyMachine.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveAtAssemblyMachine.Name = "btnSaveAtAssemblyMachine";
            this.btnSaveAtAssemblyMachine.Size = new System.Drawing.Size(118, 55);
            this.btnSaveAtAssemblyMachine.TabIndex = 24;
            this.btnSaveAtAssemblyMachine.Text = "保存";
            this.btnSaveAtAssemblyMachine.UseVisualStyleBackColor = true;
            this.btnSaveAtAssemblyMachine.Click += new System.EventHandler(this.Save_Process2_Click);
            // 
            // Device2
            // 
            this.Device2.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Device2.Location = new System.Drawing.Point(136, 392);
            this.Device2.Margin = new System.Windows.Forms.Padding(4);
            this.Device2.Name = "Device2";
            this.Device2.Size = new System.Drawing.Size(546, 36);
            this.Device2.TabIndex = 23;
            // 
            // Security2
            // 
            this.Security2.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Security2.Location = new System.Drawing.Point(136, 319);
            this.Security2.Margin = new System.Windows.Forms.Padding(4);
            this.Security2.Name = "Security2";
            this.Security2.Size = new System.Drawing.Size(546, 36);
            this.Security2.TabIndex = 22;
            // 
            // MesKey2
            // 
            this.MesKey2.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.MesKey2.Location = new System.Drawing.Point(136, 248);
            this.MesKey2.Margin = new System.Windows.Forms.Padding(4);
            this.MesKey2.Name = "MesKey2";
            this.MesKey2.Size = new System.Drawing.Size(546, 36);
            this.MesKey2.TabIndex = 21;
            // 
            // Station2
            // 
            this.Station2.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Station2.Location = new System.Drawing.Point(136, 176);
            this.Station2.Margin = new System.Windows.Forms.Padding(4);
            this.Station2.Name = "Station2";
            this.Station2.Size = new System.Drawing.Size(546, 36);
            this.Station2.TabIndex = 20;
            // 
            // Process2
            // 
            this.Process2.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Process2.Location = new System.Drawing.Point(136, 110);
            this.Process2.Margin = new System.Windows.Forms.Padding(4);
            this.Process2.Name = "Process2";
            this.Process2.Size = new System.Drawing.Size(546, 36);
            this.Process2.TabIndex = 19;
            // 
            // Line2
            // 
            this.Line2.Font = new System.Drawing.Font("Microsoft YaHei", 13F);
            this.Line2.Location = new System.Drawing.Point(136, 46);
            this.Line2.Margin = new System.Windows.Forms.Padding(4);
            this.Line2.Name = "Line2";
            this.Line2.Size = new System.Drawing.Size(546, 36);
            this.Line2.TabIndex = 18;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label58.Location = new System.Drawing.Point(58, 400);
            this.label58.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(64, 24);
            this.label58.TabIndex = 17;
            this.label58.Text = "设备名";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label57.Location = new System.Drawing.Point(44, 328);
            this.label57.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(78, 24);
            this.label57.TabIndex = 16;
            this.label57.Text = "MD加密";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label56.Location = new System.Drawing.Point(38, 256);
            this.label56.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(84, 24);
            this.label56.TabIndex = 15;
            this.label56.Text = "MES账号";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label55.Location = new System.Drawing.Point(76, 184);
            this.label55.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(46, 24);
            this.label55.TabIndex = 14;
            this.label55.Text = "站点";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label53.Location = new System.Drawing.Point(76, 114);
            this.label53.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(46, 24);
            this.label53.TabIndex = 13;
            this.label53.Text = "工序";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label42.Location = new System.Drawing.Point(76, 50);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(46, 24);
            this.label42.TabIndex = 12;
            this.label42.Text = "线体";
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.tableLayoutPanel1);
            this.tabPage7.Controls.Add(this.label60);
            this.tabPage7.Location = new System.Drawing.Point(4, 4);
            this.tabPage7.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage7.Size = new System.Drawing.Size(1894, 984);
            this.tabPage7.TabIndex = 14;
            this.tabPage7.Text = "数据维护";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox10, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox11, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 93);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1890, 889);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel32);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(2, 2);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(941, 440);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "关键参数信息维护";
            // 
            // tableLayoutPanel32
            // 
            this.tableLayoutPanel32.AutoSize = true;
            this.tableLayoutPanel32.ColumnCount = 3;
            this.tableLayoutPanel32.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel32.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel32.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel32.Controls.Add(this.btnSave_KeyArgs, 1, 0);
            this.tableLayoutPanel32.Controls.Add(this.keyArgsRefreshButton, 0, 0);
            this.tableLayoutPanel32.Controls.Add(this.dgvKeyArgs, 0, 1);
            this.tableLayoutPanel32.Controls.Add(this.copyDataGatherTable, 2, 0);
            this.tableLayoutPanel32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel32.Location = new System.Drawing.Point(2, 25);
            this.tableLayoutPanel32.Name = "tableLayoutPanel32";
            this.tableLayoutPanel32.RowCount = 2;
            this.tableLayoutPanel32.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutPanel32.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88F));
            this.tableLayoutPanel32.Size = new System.Drawing.Size(937, 413);
            this.tableLayoutPanel32.TabIndex = 10;
            // 
            // btnSave_KeyArgs
            // 
            this.btnSave_KeyArgs.AutoSize = true;
            this.btnSave_KeyArgs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave_KeyArgs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave_KeyArgs.Location = new System.Drawing.Point(314, 2);
            this.btnSave_KeyArgs.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave_KeyArgs.Name = "btnSave_KeyArgs";
            this.btnSave_KeyArgs.Size = new System.Drawing.Size(308, 45);
            this.btnSave_KeyArgs.TabIndex = 10;
            this.btnSave_KeyArgs.Text = "保存";
            this.btnSave_KeyArgs.UseVisualStyleBackColor = true;
            // 
            // keyArgsRefreshButton
            // 
            this.keyArgsRefreshButton.AutoSize = true;
            this.keyArgsRefreshButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyArgsRefreshButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.keyArgsRefreshButton.Location = new System.Drawing.Point(2, 2);
            this.keyArgsRefreshButton.Margin = new System.Windows.Forms.Padding(2);
            this.keyArgsRefreshButton.Name = "keyArgsRefreshButton";
            this.keyArgsRefreshButton.Size = new System.Drawing.Size(308, 45);
            this.keyArgsRefreshButton.TabIndex = 8;
            this.keyArgsRefreshButton.Text = "刷新";
            this.keyArgsRefreshButton.UseVisualStyleBackColor = true;
            // 
            // dgvKeyArgs
            // 
            this.dgvKeyArgs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvKeyArgs.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvKeyArgs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel32.SetColumnSpan(this.dgvKeyArgs, 3);
            this.dgvKeyArgs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvKeyArgs.Location = new System.Drawing.Point(2, 51);
            this.dgvKeyArgs.Margin = new System.Windows.Forms.Padding(2);
            this.dgvKeyArgs.Name = "dgvKeyArgs";
            this.dgvKeyArgs.RowHeadersWidth = 51;
            this.dgvKeyArgs.RowTemplate.Height = 27;
            this.dgvKeyArgs.Size = new System.Drawing.Size(933, 360);
            this.dgvKeyArgs.TabIndex = 6;
            this.dgvKeyArgs.Tag = "KeyArgsPreserve";
            // 
            // copyDataGatherTable
            // 
            this.copyDataGatherTable.AutoSize = true;
            this.copyDataGatherTable.BackColor = System.Drawing.Color.PaleTurquoise;
            this.copyDataGatherTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.copyDataGatherTable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.copyDataGatherTable.Location = new System.Drawing.Point(628, 4);
            this.copyDataGatherTable.Margin = new System.Windows.Forms.Padding(4);
            this.copyDataGatherTable.Name = "copyDataGatherTable";
            this.copyDataGatherTable.Size = new System.Drawing.Size(305, 41);
            this.copyDataGatherTable.TabIndex = 9;
            this.copyDataGatherTable.Text = "引用数据采集信息维护表";
            this.copyDataGatherTable.UseVisualStyleBackColor = false;
            this.copyDataGatherTable.Click += new System.EventHandler(this.CopyDataGatherTable_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel31);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(947, 2);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(941, 440);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "预警故障信息维护";
            // 
            // tableLayoutPanel31
            // 
            this.tableLayoutPanel31.AutoSize = true;
            this.tableLayoutPanel31.ColumnCount = 2;
            this.tableLayoutPanel31.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel31.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel31.Controls.Add(this.errorPreserveRefreshButton, 0, 0);
            this.tableLayoutPanel31.Controls.Add(this.dgvErrorPreserve, 0, 1);
            this.tableLayoutPanel31.Controls.Add(this.btnSave_WarmError, 1, 0);
            this.tableLayoutPanel31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel31.Location = new System.Drawing.Point(2, 25);
            this.tableLayoutPanel31.Name = "tableLayoutPanel31";
            this.tableLayoutPanel31.RowCount = 2;
            this.tableLayoutPanel31.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutPanel31.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88F));
            this.tableLayoutPanel31.Size = new System.Drawing.Size(937, 413);
            this.tableLayoutPanel31.TabIndex = 5;
            // 
            // errorPreserveRefreshButton
            // 
            this.errorPreserveRefreshButton.AutoSize = true;
            this.errorPreserveRefreshButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorPreserveRefreshButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.errorPreserveRefreshButton.Location = new System.Drawing.Point(2, 2);
            this.errorPreserveRefreshButton.Margin = new System.Windows.Forms.Padding(2);
            this.errorPreserveRefreshButton.Name = "errorPreserveRefreshButton";
            this.errorPreserveRefreshButton.Size = new System.Drawing.Size(464, 45);
            this.errorPreserveRefreshButton.TabIndex = 3;
            this.errorPreserveRefreshButton.Text = "刷新";
            this.errorPreserveRefreshButton.UseVisualStyleBackColor = true;
            // 
            // dgvErrorPreserve
            // 
            this.dgvErrorPreserve.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvErrorPreserve.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvErrorPreserve.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel31.SetColumnSpan(this.dgvErrorPreserve, 2);
            this.dgvErrorPreserve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvErrorPreserve.Location = new System.Drawing.Point(2, 51);
            this.dgvErrorPreserve.Margin = new System.Windows.Forms.Padding(2);
            this.dgvErrorPreserve.Name = "dgvErrorPreserve";
            this.dgvErrorPreserve.RowHeadersWidth = 51;
            this.dgvErrorPreserve.RowTemplate.Height = 27;
            this.dgvErrorPreserve.Size = new System.Drawing.Size(933, 360);
            this.dgvErrorPreserve.TabIndex = 2;
            this.dgvErrorPreserve.Tag = "ErrorReferenceTable";
            // 
            // btnSave_WarmError
            // 
            this.btnSave_WarmError.AutoSize = true;
            this.btnSave_WarmError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave_WarmError.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave_WarmError.Location = new System.Drawing.Point(470, 2);
            this.btnSave_WarmError.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave_WarmError.Name = "btnSave_WarmError";
            this.btnSave_WarmError.Size = new System.Drawing.Size(465, 45);
            this.btnSave_WarmError.TabIndex = 4;
            this.btnSave_WarmError.Text = "保存";
            this.btnSave_WarmError.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.tableLayoutPanel29);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Location = new System.Drawing.Point(2, 446);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox10.Size = new System.Drawing.Size(941, 441);
            this.groupBox10.TabIndex = 12;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "数据采集信息维护";
            // 
            // tableLayoutPanel29
            // 
            this.tableLayoutPanel29.AutoSize = true;
            this.tableLayoutPanel29.ColumnCount = 2;
            this.tableLayoutPanel29.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel29.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel29.Controls.Add(this.btnSave_dgvDataAcquisition, 1, 0);
            this.tableLayoutPanel29.Controls.Add(this.dgvDataAcquisition, 0, 1);
            this.tableLayoutPanel29.Controls.Add(this.dataGatherBoardRefreshButton, 0, 0);
            this.tableLayoutPanel29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel29.Location = new System.Drawing.Point(2, 25);
            this.tableLayoutPanel29.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel29.Name = "tableLayoutPanel29";
            this.tableLayoutPanel29.RowCount = 2;
            this.tableLayoutPanel29.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tableLayoutPanel29.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87F));
            this.tableLayoutPanel29.Size = new System.Drawing.Size(937, 414);
            this.tableLayoutPanel29.TabIndex = 2;
            // 
            // btnSave_dgvDataAcquisition
            // 
            this.btnSave_dgvDataAcquisition.AutoSize = true;
            this.btnSave_dgvDataAcquisition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave_dgvDataAcquisition.ForeColor = System.Drawing.Color.Black;
            this.btnSave_dgvDataAcquisition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave_dgvDataAcquisition.Location = new System.Drawing.Point(470, 2);
            this.btnSave_dgvDataAcquisition.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave_dgvDataAcquisition.Name = "btnSave_dgvDataAcquisition";
            this.btnSave_dgvDataAcquisition.Size = new System.Drawing.Size(465, 49);
            this.btnSave_dgvDataAcquisition.TabIndex = 2;
            this.btnSave_dgvDataAcquisition.Text = "保存";
            this.btnSave_dgvDataAcquisition.UseVisualStyleBackColor = true;
            // 
            // dgvDataAcquisition
            // 
            this.dgvDataAcquisition.AllowUserToDeleteRows = false;
            this.dgvDataAcquisition.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvDataAcquisition.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvDataAcquisition.ColumnHeadersHeight = 29;
            this.tableLayoutPanel29.SetColumnSpan(this.dgvDataAcquisition, 2);
            this.dgvDataAcquisition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDataAcquisition.Location = new System.Drawing.Point(2, 55);
            this.dgvDataAcquisition.Margin = new System.Windows.Forms.Padding(2);
            this.dgvDataAcquisition.Name = "dgvDataAcquisition";
            this.dgvDataAcquisition.RowHeadersVisible = false;
            this.dgvDataAcquisition.RowHeadersWidth = 51;
            this.dgvDataAcquisition.Size = new System.Drawing.Size(933, 357);
            this.dgvDataAcquisition.TabIndex = 0;
            // 
            // dataGatherBoardRefreshButton
            // 
            this.dataGatherBoardRefreshButton.AutoSize = true;
            this.dataGatherBoardRefreshButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGatherBoardRefreshButton.ForeColor = System.Drawing.Color.Black;
            this.dataGatherBoardRefreshButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dataGatherBoardRefreshButton.Location = new System.Drawing.Point(2, 2);
            this.dataGatherBoardRefreshButton.Margin = new System.Windows.Forms.Padding(2);
            this.dataGatherBoardRefreshButton.Name = "dataGatherBoardRefreshButton";
            this.dataGatherBoardRefreshButton.Size = new System.Drawing.Size(464, 49);
            this.dataGatherBoardRefreshButton.TabIndex = 1;
            this.dataGatherBoardRefreshButton.Text = "刷新";
            this.dataGatherBoardRefreshButton.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.tableLayoutPanel30);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(947, 446);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox11.Size = new System.Drawing.Size(941, 441);
            this.groupBox11.TabIndex = 13;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "设备缺陷信息维护";
            // 
            // tableLayoutPanel30
            // 
            this.tableLayoutPanel30.AutoSize = true;
            this.tableLayoutPanel30.ColumnCount = 2;
            this.tableLayoutPanel30.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel30.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel30.Controls.Add(this.btnSave_dgvDefect, 1, 0);
            this.tableLayoutPanel30.Controls.Add(this.deviceDefectsRefreshButton, 0, 0);
            this.tableLayoutPanel30.Controls.Add(this.dgvDeviceDefects, 0, 1);
            this.tableLayoutPanel30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel30.Location = new System.Drawing.Point(2, 25);
            this.tableLayoutPanel30.Name = "tableLayoutPanel30";
            this.tableLayoutPanel30.RowCount = 2;
            this.tableLayoutPanel30.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tableLayoutPanel30.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87F));
            this.tableLayoutPanel30.Size = new System.Drawing.Size(937, 414);
            this.tableLayoutPanel30.TabIndex = 2;
            // 
            // btnSave_dgvDefect
            // 
            this.btnSave_dgvDefect.AutoSize = true;
            this.btnSave_dgvDefect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave_dgvDefect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave_dgvDefect.Location = new System.Drawing.Point(470, 2);
            this.btnSave_dgvDefect.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave_dgvDefect.Name = "btnSave_dgvDefect";
            this.btnSave_dgvDefect.Size = new System.Drawing.Size(465, 49);
            this.btnSave_dgvDefect.TabIndex = 2;
            this.btnSave_dgvDefect.Text = "保存";
            this.btnSave_dgvDefect.UseVisualStyleBackColor = true;
            // 
            // deviceDefectsRefreshButton
            // 
            this.deviceDefectsRefreshButton.AutoSize = true;
            this.deviceDefectsRefreshButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceDefectsRefreshButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.deviceDefectsRefreshButton.Location = new System.Drawing.Point(2, 2);
            this.deviceDefectsRefreshButton.Margin = new System.Windows.Forms.Padding(2);
            this.deviceDefectsRefreshButton.Name = "deviceDefectsRefreshButton";
            this.deviceDefectsRefreshButton.Size = new System.Drawing.Size(464, 49);
            this.deviceDefectsRefreshButton.TabIndex = 1;
            this.deviceDefectsRefreshButton.Text = "刷新";
            this.deviceDefectsRefreshButton.UseVisualStyleBackColor = true;
            // 
            // dgvDeviceDefects
            // 
            this.dgvDeviceDefects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvDeviceDefects.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvDeviceDefects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel30.SetColumnSpan(this.dgvDeviceDefects, 2);
            this.dgvDeviceDefects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDeviceDefects.Location = new System.Drawing.Point(2, 55);
            this.dgvDeviceDefects.Margin = new System.Windows.Forms.Padding(2);
            this.dgvDeviceDefects.Name = "dgvDeviceDefects";
            this.dgvDeviceDefects.RowHeadersWidth = 51;
            this.dgvDeviceDefects.RowTemplate.Height = 27;
            this.dgvDeviceDefects.Size = new System.Drawing.Size(933, 357);
            this.dgvDeviceDefects.TabIndex = 0;
            // 
            // label60
            // 
            this.label60.Dock = System.Windows.Forms.DockStyle.Top;
            this.label60.ForeColor = System.Drawing.Color.Red;
            this.label60.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.label60.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label60.Location = new System.Drawing.Point(2, 2);
            this.label60.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(1890, 91);
            this.label60.TabIndex = 1;
            this.label60.Text = "填写格式：[PLC地址]:[数据类型]-[计算规则]      示例：D1020:I-0，D1021:H-4\r\n数据类型：H=Int16，I=Int32，F=Fl" +
    "oat，其它情况均为按Int16读取\r\n计算规则：0=实际值，1÷10，2÷100，3÷1000，4=状态判断(3=OK，其余为NG)，其它均为实际值";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.tabControl1);
            this.tabPage6.Location = new System.Drawing.Point(4, 4);
            this.tabPage6.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(1894, 984);
            this.tabPage6.TabIndex = 11;
            this.tabPage6.Text = "生产配置";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage10);
            this.tabControl1.Controls.Add(this.tabPage11);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1894, 984);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.panel9);
            this.tabPage10.Controls.Add(this.groupBox16);
            this.tabPage10.Location = new System.Drawing.Point(4, 32);
            this.tabPage10.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage10.Size = new System.Drawing.Size(1886, 948);
            this.tabPage10.TabIndex = 0;
            this.tabPage10.Text = "功能变更";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.tlpProductConfig);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(2, 2);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(1882, 305);
            this.panel9.TabIndex = 83;
            // 
            // tlpProductConfig
            // 
            this.tlpProductConfig.ColumnCount = 3;
            this.tlpProductConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.01594F));
            this.tlpProductConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.90436F));
            this.tlpProductConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.13284F));
            this.tlpProductConfig.Controls.Add(this.groupBox13, 0, 0);
            this.tlpProductConfig.Controls.Add(this.groupBox12, 1, 0);
            this.tlpProductConfig.Controls.Add(this.panel3, 2, 0);
            this.tlpProductConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProductConfig.Location = new System.Drawing.Point(0, 0);
            this.tlpProductConfig.Margin = new System.Windows.Forms.Padding(2);
            this.tlpProductConfig.Name = "tlpProductConfig";
            this.tlpProductConfig.RowCount = 1;
            this.tlpProductConfig.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProductConfig.Size = new System.Drawing.Size(1882, 305);
            this.tlpProductConfig.TabIndex = 0;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.BarcodeRule);
            this.groupBox13.Controls.Add(this.label19);
            this.groupBox13.Controls.Add(this.EnableReportConfigParam);
            this.groupBox13.Controls.Add(this.EnableReportRealTimeParam);
            this.groupBox13.Controls.Add(this.EnableReportMachineAlarm);
            this.groupBox13.Controls.Add(this.label52);
            this.groupBox13.Controls.Add(this.RealtimeArgsUploadRate);
            this.groupBox13.Controls.Add(this.EnableReportMachineStatus);
            this.groupBox13.Controls.Add(this.HeartbeatUploadRate);
            this.groupBox13.Controls.Add(this.label22);
            this.groupBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox13.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.groupBox13.Location = new System.Drawing.Point(2, 2);
            this.groupBox13.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox13.Size = new System.Drawing.Size(654, 301);
            this.groupBox13.TabIndex = 77;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "保存后生效";
            // 
            // BarcodeRule
            // 
            this.BarcodeRule.Location = new System.Drawing.Point(255, 229);
            this.BarcodeRule.Margin = new System.Windows.Forms.Padding(2);
            this.BarcodeRule.Name = "BarcodeRule";
            this.BarcodeRule.Size = new System.Drawing.Size(166, 30);
            this.BarcodeRule.TabIndex = 9;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(65, 52);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(187, 24);
            this.label19.TabIndex = 1;
            this.label19.Text = "心跳上传频率(单位: s):";
            // 
            // EnableReportConfigParam
            // 
            this.EnableReportConfigParam.AutoSize = true;
            this.EnableReportConfigParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableReportConfigParam.Location = new System.Drawing.Point(438, 243);
            this.EnableReportConfigParam.Margin = new System.Windows.Forms.Padding(2);
            this.EnableReportConfigParam.Name = "EnableReportConfigParam";
            this.EnableReportConfigParam.Size = new System.Drawing.Size(212, 28);
            this.EnableReportConfigParam.TabIndex = 69;
            this.EnableReportConfigParam.Text = "勾选启用关键参数上传";
            this.EnableReportConfigParam.UseVisualStyleBackColor = true;
            // 
            // EnableReportRealTimeParam
            // 
            this.EnableReportRealTimeParam.AutoSize = true;
            this.EnableReportRealTimeParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableReportRealTimeParam.Location = new System.Drawing.Point(438, 179);
            this.EnableReportRealTimeParam.Margin = new System.Windows.Forms.Padding(2);
            this.EnableReportRealTimeParam.Name = "EnableReportRealTimeParam";
            this.EnableReportRealTimeParam.Size = new System.Drawing.Size(212, 28);
            this.EnableReportRealTimeParam.TabIndex = 68;
            this.EnableReportRealTimeParam.Text = "勾选启用实时参数上传";
            this.EnableReportRealTimeParam.UseVisualStyleBackColor = true;
            // 
            // EnableReportMachineAlarm
            // 
            this.EnableReportMachineAlarm.AutoSize = true;
            this.EnableReportMachineAlarm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableReportMachineAlarm.Location = new System.Drawing.Point(438, 112);
            this.EnableReportMachineAlarm.Margin = new System.Windows.Forms.Padding(2);
            this.EnableReportMachineAlarm.Name = "EnableReportMachineAlarm";
            this.EnableReportMachineAlarm.Size = new System.Drawing.Size(212, 28);
            this.EnableReportMachineAlarm.TabIndex = 67;
            this.EnableReportMachineAlarm.Text = "勾选启用预警信息上传";
            this.EnableReportMachineAlarm.UseVisualStyleBackColor = true;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label52.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label52.Location = new System.Drawing.Point(3, 146);
            this.label52.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(251, 27);
            this.label52.TabIndex = 65;
            this.label52.Text = "实时参数上传频率(单位: s):";
            // 
            // RealtimeArgsUploadRate
            // 
            this.RealtimeArgsUploadRate.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F);
            this.RealtimeArgsUploadRate.Location = new System.Drawing.Point(254, 145);
            this.RealtimeArgsUploadRate.Margin = new System.Windows.Forms.Padding(2);
            this.RealtimeArgsUploadRate.Name = "RealtimeArgsUploadRate";
            this.RealtimeArgsUploadRate.Size = new System.Drawing.Size(167, 31);
            this.RealtimeArgsUploadRate.TabIndex = 66;
            // 
            // EnableReportMachineStatus
            // 
            this.EnableReportMachineStatus.AutoSize = true;
            this.EnableReportMachineStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableReportMachineStatus.Location = new System.Drawing.Point(438, 47);
            this.EnableReportMachineStatus.Margin = new System.Windows.Forms.Padding(2);
            this.EnableReportMachineStatus.Name = "EnableReportMachineStatus";
            this.EnableReportMachineStatus.Size = new System.Drawing.Size(212, 28);
            this.EnableReportMachineStatus.TabIndex = 10;
            this.EnableReportMachineStatus.Text = "勾选启用设备状态上传";
            this.EnableReportMachineStatus.UseVisualStyleBackColor = true;
            // 
            // HeartbeatUploadRate
            // 
            this.HeartbeatUploadRate.Location = new System.Drawing.Point(256, 49);
            this.HeartbeatUploadRate.Margin = new System.Windows.Forms.Padding(2);
            this.HeartbeatUploadRate.Name = "HeartbeatUploadRate";
            this.HeartbeatUploadRate.Size = new System.Drawing.Size(166, 30);
            this.HeartbeatUploadRate.TabIndex = 8;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label22.Location = new System.Drawing.Point(154, 232);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(100, 24);
            this.label22.TabIndex = 4;
            this.label22.Text = "条码规则：";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.cboProductMode);
            this.groupBox12.Controls.Add(this.cboBanUpload);
            this.groupBox12.Controls.Add(this.cboEnforcePass);
            this.groupBox12.Controls.Add(this.label61);
            this.groupBox12.Controls.Add(this.label62);
            this.groupBox12.Controls.Add(this.label21);
            this.groupBox12.Controls.Add(this.EnableUpperTooling);
            this.groupBox12.Controls.Add(this.EnableGetNextBoard);
            this.groupBox12.Controls.Add(this.EnableTypeChangedVerify);
            this.groupBox12.Controls.Add(this.EnablePrintCode);
            this.groupBox12.Controls.Add(this.chkBanFixtureUpload);
            this.groupBox12.Controls.Add(this.EnableResultUpload);
            this.groupBox12.Controls.Add(this.EnableFluentVerify);
            this.groupBox12.Controls.Add(this.BanReadBarcode);
            this.groupBox12.Controls.Add(this.EnableBarcodeRuleVerify);
            this.groupBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox12.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.groupBox12.Location = new System.Drawing.Point(660, 2);
            this.groupBox12.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox12.Size = new System.Drawing.Size(746, 301);
            this.groupBox12.TabIndex = 80;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "切换状态即生效";
            // 
            // cboProductMode
            // 
            this.cboProductMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProductMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboProductMode.Font = new System.Drawing.Font("Microsoft YaHei", 9.8F);
            this.cboProductMode.FormattingEnabled = true;
            this.cboProductMode.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cboProductMode.Items.AddRange(new object[] {
            "不显示NG且阻塞",
            "显示NG且阻塞",
            "显示NG且不阻塞"});
            this.cboProductMode.Location = new System.Drawing.Point(103, 243);
            this.cboProductMode.Margin = new System.Windows.Forms.Padding(4);
            this.cboProductMode.Name = "cboProductMode";
            this.cboProductMode.Size = new System.Drawing.Size(141, 29);
            this.cboProductMode.TabIndex = 73;
            // 
            // cboBanUpload
            // 
            this.cboBanUpload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBanUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboBanUpload.Font = new System.Drawing.Font("Microsoft YaHei", 9.8F);
            this.cboBanUpload.FormattingEnabled = true;
            this.cboBanUpload.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cboBanUpload.Items.AddRange(new object[] {
            "Scan_ASSY",
            "Weight",
            "Screw_BA",
            "None",
            "All"});
            this.cboBanUpload.Location = new System.Drawing.Point(609, 242);
            this.cboBanUpload.Margin = new System.Windows.Forms.Padding(4);
            this.cboBanUpload.Name = "cboBanUpload";
            this.cboBanUpload.Size = new System.Drawing.Size(128, 29);
            this.cboBanUpload.TabIndex = 73;
            // 
            // cboEnforcePass
            // 
            this.cboEnforcePass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEnforcePass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEnforcePass.Font = new System.Drawing.Font("Microsoft YaHei", 9.8F);
            this.cboEnforcePass.FormattingEnabled = true;
            this.cboEnforcePass.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cboEnforcePass.Items.AddRange(new object[] {
            "Scan_ASSY",
            "Weight",
            "Screw_BA",
            "None",
            "All"});
            this.cboEnforcePass.Location = new System.Drawing.Point(377, 242);
            this.cboEnforcePass.Margin = new System.Windows.Forms.Padding(4);
            this.cboEnforcePass.Name = "cboEnforcePass";
            this.cboEnforcePass.Size = new System.Drawing.Size(105, 29);
            this.cboEnforcePass.TabIndex = 73;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label61.Location = new System.Drawing.Point(517, 244);
            this.label61.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(100, 24);
            this.label61.TabIndex = 74;
            this.label61.Text = "禁用上传：";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label62.Location = new System.Drawing.Point(289, 246);
            this.label62.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(100, 24);
            this.label62.TabIndex = 74;
            this.label62.Text = "强制过站：";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label21.Location = new System.Drawing.Point(10, 246);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(100, 24);
            this.label21.TabIndex = 74;
            this.label21.Text = "过站失败：";
            // 
            // EnableUpperTooling
            // 
            this.EnableUpperTooling.AutoSize = true;
            this.EnableUpperTooling.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableUpperTooling.Location = new System.Drawing.Point(521, 112);
            this.EnableUpperTooling.Margin = new System.Windows.Forms.Padding(2);
            this.EnableUpperTooling.Name = "EnableUpperTooling";
            this.EnableUpperTooling.Size = new System.Drawing.Size(212, 28);
            this.EnableUpperTooling.TabIndex = 5;
            this.EnableUpperTooling.Text = "勾选启用上工装机程序";
            this.EnableUpperTooling.UseVisualStyleBackColor = true;
            // 
            // EnableGetNextBoard
            // 
            this.EnableGetNextBoard.AutoSize = true;
            this.EnableGetNextBoard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableGetNextBoard.Location = new System.Drawing.Point(293, 49);
            this.EnableGetNextBoard.Margin = new System.Windows.Forms.Padding(2);
            this.EnableGetNextBoard.Name = "EnableGetNextBoard";
            this.EnableGetNextBoard.Size = new System.Drawing.Size(176, 28);
            this.EnableGetNextBoard.TabIndex = 0;
            this.EnableGetNextBoard.Text = "勾选启用获取拼版";
            this.EnableGetNextBoard.UseVisualStyleBackColor = true;
            // 
            // EnableTypeChangedVerify
            // 
            this.EnableTypeChangedVerify.AutoSize = true;
            this.EnableTypeChangedVerify.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableTypeChangedVerify.Location = new System.Drawing.Point(32, 112);
            this.EnableTypeChangedVerify.Margin = new System.Windows.Forms.Padding(4);
            this.EnableTypeChangedVerify.Name = "EnableTypeChangedVerify";
            this.EnableTypeChangedVerify.Size = new System.Drawing.Size(212, 28);
            this.EnableTypeChangedVerify.TabIndex = 72;
            this.EnableTypeChangedVerify.Text = "勾选启用型号切换校验";
            this.EnableTypeChangedVerify.UseVisualStyleBackColor = true;
            // 
            // EnablePrintCode
            // 
            this.EnablePrintCode.AutoSize = true;
            this.EnablePrintCode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnablePrintCode.Location = new System.Drawing.Point(521, 47);
            this.EnablePrintCode.Margin = new System.Windows.Forms.Padding(2);
            this.EnablePrintCode.Name = "EnablePrintCode";
            this.EnablePrintCode.Size = new System.Drawing.Size(176, 28);
            this.EnablePrintCode.TabIndex = 70;
            this.EnablePrintCode.Text = "勾选启用打印模板";
            this.EnablePrintCode.UseVisualStyleBackColor = true;
            // 
            // chkBanFixtureUpload
            // 
            this.chkBanFixtureUpload.AutoSize = true;
            this.chkBanFixtureUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkBanFixtureUpload.Location = new System.Drawing.Point(521, 179);
            this.chkBanFixtureUpload.Margin = new System.Windows.Forms.Padding(2);
            this.chkBanFixtureUpload.Name = "chkBanFixtureUpload";
            this.chkBanFixtureUpload.Size = new System.Drawing.Size(212, 28);
            this.chkBanFixtureUpload.TabIndex = 2;
            this.chkBanFixtureUpload.Text = "勾选屏蔽工装编号上传";
            this.chkBanFixtureUpload.UseVisualStyleBackColor = true;
            // 
            // EnableResultUpload
            // 
            this.EnableResultUpload.AutoSize = true;
            this.EnableResultUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableResultUpload.Location = new System.Drawing.Point(293, 179);
            this.EnableResultUpload.Margin = new System.Windows.Forms.Padding(2);
            this.EnableResultUpload.Name = "EnableResultUpload";
            this.EnableResultUpload.Size = new System.Drawing.Size(176, 28);
            this.EnableResultUpload.TabIndex = 2;
            this.EnableResultUpload.Text = "勾选启用上传结果";
            this.EnableResultUpload.UseVisualStyleBackColor = true;
            // 
            // EnableFluentVerify
            // 
            this.EnableFluentVerify.AutoSize = true;
            this.EnableFluentVerify.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableFluentVerify.Location = new System.Drawing.Point(293, 112);
            this.EnableFluentVerify.Margin = new System.Windows.Forms.Padding(2);
            this.EnableFluentVerify.Name = "EnableFluentVerify";
            this.EnableFluentVerify.Size = new System.Drawing.Size(176, 28);
            this.EnableFluentVerify.TabIndex = 1;
            this.EnableFluentVerify.Text = "勾选启用流程验证";
            this.EnableFluentVerify.UseVisualStyleBackColor = true;
            // 
            // BanReadBarcode
            // 
            this.BanReadBarcode.AutoSize = true;
            this.BanReadBarcode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BanReadBarcode.Location = new System.Drawing.Point(32, 49);
            this.BanReadBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.BanReadBarcode.Name = "BanReadBarcode";
            this.BanReadBarcode.Size = new System.Drawing.Size(176, 28);
            this.BanReadBarcode.TabIndex = 5;
            this.BanReadBarcode.Text = "勾选屏蔽条码读取";
            this.BanReadBarcode.UseVisualStyleBackColor = true;
            // 
            // EnableBarcodeRuleVerify
            // 
            this.EnableBarcodeRuleVerify.AutoSize = true;
            this.EnableBarcodeRuleVerify.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnableBarcodeRuleVerify.Location = new System.Drawing.Point(32, 179);
            this.EnableBarcodeRuleVerify.Margin = new System.Windows.Forms.Padding(2);
            this.EnableBarcodeRuleVerify.Name = "EnableBarcodeRuleVerify";
            this.EnableBarcodeRuleVerify.Size = new System.Drawing.Size(212, 28);
            this.EnableBarcodeRuleVerify.TabIndex = 5;
            this.EnableBarcodeRuleVerify.Text = "勾选启用条码规则验证";
            this.EnableBarcodeRuleVerify.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tableLayoutPanel33);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(1411, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(468, 299);
            this.panel3.TabIndex = 83;
            // 
            // tableLayoutPanel33
            // 
            this.tableLayoutPanel33.ColumnCount = 2;
            this.tableLayoutPanel33.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.05983F));
            this.tableLayoutPanel33.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.94017F));
            this.tableLayoutPanel33.Controls.Add(this.ProductConfig_SaveButton, 1, 1);
            this.tableLayoutPanel33.Controls.Add(this.grpTorqueMeterConfig, 1, 0);
            this.tableLayoutPanel33.Controls.Add(this.grpTorqueConfig, 0, 0);
            this.tableLayoutPanel33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel33.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel33.Name = "tableLayoutPanel33";
            this.tableLayoutPanel33.RowCount = 2;
            this.tableLayoutPanel33.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.4F));
            this.tableLayoutPanel33.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 29.6F));
            this.tableLayoutPanel33.Size = new System.Drawing.Size(468, 299);
            this.tableLayoutPanel33.TabIndex = 75;
            // 
            // grpTorqueConfig
            // 
            this.grpTorqueConfig.Controls.Add(this.grpTorqueControllerConfig2);
            this.grpTorqueConfig.Controls.Add(this.grpTorqueControllerConfig1);
            this.grpTorqueConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTorqueConfig.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grpTorqueConfig.Location = new System.Drawing.Point(3, 3);
            this.grpTorqueConfig.Name = "grpTorqueConfig";
            this.tableLayoutPanel33.SetRowSpan(this.grpTorqueConfig, 2);
            this.grpTorqueConfig.Size = new System.Drawing.Size(247, 293);
            this.grpTorqueConfig.TabIndex = 83;
            this.grpTorqueConfig.TabStop = false;
            this.grpTorqueConfig.Text = "扭力控制器连接参数";
            // 
            // grpTorqueControllerConfig2
            // 
            this.grpTorqueControllerConfig2.Controls.Add(this.txtControllerPort2);
            this.grpTorqueControllerConfig2.Controls.Add(this.txtControllerIP2);
            this.grpTorqueControllerConfig2.Controls.Add(this.lblIP2);
            this.grpTorqueControllerConfig2.Controls.Add(this.lblPort2);
            this.grpTorqueControllerConfig2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTorqueControllerConfig2.Location = new System.Drawing.Point(3, 152);
            this.grpTorqueControllerConfig2.Name = "grpTorqueControllerConfig2";
            this.grpTorqueControllerConfig2.Size = new System.Drawing.Size(241, 138);
            this.grpTorqueControllerConfig2.TabIndex = 1;
            this.grpTorqueControllerConfig2.TabStop = false;
            this.grpTorqueControllerConfig2.Text = "Screw-BA";
            // 
            // txtControllerPort2
            // 
            this.txtControllerPort2.Location = new System.Drawing.Point(87, 88);
            this.txtControllerPort2.Name = "txtControllerPort2";
            this.txtControllerPort2.Size = new System.Drawing.Size(124, 30);
            this.txtControllerPort2.TabIndex = 1;
            // 
            // txtControllerIP2
            // 
            this.txtControllerIP2.Location = new System.Drawing.Point(87, 37);
            this.txtControllerIP2.Name = "txtControllerIP2";
            this.txtControllerIP2.Size = new System.Drawing.Size(124, 30);
            this.txtControllerIP2.TabIndex = 1;
            // 
            // lblIP2
            // 
            this.lblIP2.AutoSize = true;
            this.lblIP2.Location = new System.Drawing.Point(54, 40);
            this.lblIP2.Name = "lblIP2";
            this.lblIP2.Size = new System.Drawing.Size(27, 24);
            this.lblIP2.TabIndex = 0;
            this.lblIP2.Text = "Ip";
            // 
            // lblPort2
            // 
            this.lblPort2.AutoSize = true;
            this.lblPort2.Location = new System.Drawing.Point(37, 90);
            this.lblPort2.Name = "lblPort2";
            this.lblPort2.Size = new System.Drawing.Size(46, 24);
            this.lblPort2.TabIndex = 0;
            this.lblPort2.Text = "Port";
            // 
            // grpTorqueControllerConfig1
            // 
            this.grpTorqueControllerConfig1.Controls.Add(this.txtControllerPort1);
            this.grpTorqueControllerConfig1.Controls.Add(this.txtControllerIP1);
            this.grpTorqueControllerConfig1.Controls.Add(this.lblPort1);
            this.grpTorqueControllerConfig1.Controls.Add(this.lblIP1);
            this.grpTorqueControllerConfig1.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTorqueControllerConfig1.Location = new System.Drawing.Point(3, 26);
            this.grpTorqueControllerConfig1.Name = "grpTorqueControllerConfig1";
            this.grpTorqueControllerConfig1.Size = new System.Drawing.Size(241, 126);
            this.grpTorqueControllerConfig1.TabIndex = 0;
            this.grpTorqueControllerConfig1.TabStop = false;
            this.grpTorqueControllerConfig1.Text = "Scan-ASSY";
            // 
            // txtControllerPort1
            // 
            this.txtControllerPort1.Location = new System.Drawing.Point(87, 80);
            this.txtControllerPort1.Name = "txtControllerPort1";
            this.txtControllerPort1.Size = new System.Drawing.Size(124, 30);
            this.txtControllerPort1.TabIndex = 1;
            // 
            // txtControllerIP1
            // 
            this.txtControllerIP1.Location = new System.Drawing.Point(87, 29);
            this.txtControllerIP1.Name = "txtControllerIP1";
            this.txtControllerIP1.Size = new System.Drawing.Size(124, 30);
            this.txtControllerIP1.TabIndex = 1;
            // 
            // lblPort1
            // 
            this.lblPort1.AutoSize = true;
            this.lblPort1.Location = new System.Drawing.Point(37, 82);
            this.lblPort1.Name = "lblPort1";
            this.lblPort1.Size = new System.Drawing.Size(46, 24);
            this.lblPort1.TabIndex = 0;
            this.lblPort1.Text = "Port";
            // 
            // lblIP1
            // 
            this.lblIP1.AutoSize = true;
            this.lblIP1.Location = new System.Drawing.Point(54, 32);
            this.lblIP1.Name = "lblIP1";
            this.lblIP1.Size = new System.Drawing.Size(26, 24);
            this.lblIP1.TabIndex = 0;
            this.lblIP1.Text = "IP";
            // 
            // ProductConfig_SaveButton
            // 
            this.ProductConfig_SaveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProductConfig_SaveButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ProductConfig_SaveButton.Location = new System.Drawing.Point(255, 212);
            this.ProductConfig_SaveButton.Margin = new System.Windows.Forms.Padding(2);
            this.ProductConfig_SaveButton.Name = "ProductConfig_SaveButton";
            this.ProductConfig_SaveButton.Size = new System.Drawing.Size(211, 85);
            this.ProductConfig_SaveButton.TabIndex = 81;
            this.ProductConfig_SaveButton.Text = "全部保存";
            this.ProductConfig_SaveButton.UseVisualStyleBackColor = true;
            this.ProductConfig_SaveButton.Click += new System.EventHandler(this.SaveAtProductConfig_Click);
            // 
            // grpTorqueMeterConfig
            // 
            this.grpTorqueMeterConfig.Controls.Add(this.btnRefresh);
            this.grpTorqueMeterConfig.Controls.Add(this.cmbCOM2);
            this.grpTorqueMeterConfig.Controls.Add(this.lblCOM2);
            this.grpTorqueMeterConfig.Controls.Add(this.cmbCOM1);
            this.grpTorqueMeterConfig.Controls.Add(this.lblCOM1);
            this.grpTorqueMeterConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTorqueMeterConfig.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grpTorqueMeterConfig.Location = new System.Drawing.Point(256, 3);
            this.grpTorqueMeterConfig.Name = "grpTorqueMeterConfig";
            this.grpTorqueMeterConfig.Size = new System.Drawing.Size(209, 204);
            this.grpTorqueMeterConfig.TabIndex = 82;
            this.grpTorqueMeterConfig.TabStop = false;
            this.grpTorqueMeterConfig.Text = "扭力仪串口设定";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(86, 141);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(94, 31);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "刷新串口";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cmbCOM2
            // 
            this.cmbCOM2.FormattingEnabled = true;
            this.cmbCOM2.Location = new System.Drawing.Point(86, 92);
            this.cmbCOM2.Name = "cmbCOM2";
            this.cmbCOM2.Size = new System.Drawing.Size(94, 31);
            this.cmbCOM2.TabIndex = 1;
            // 
            // lblCOM2
            // 
            this.lblCOM2.AutoSize = true;
            this.lblCOM2.Location = new System.Drawing.Point(23, 96);
            this.lblCOM2.Name = "lblCOM2";
            this.lblCOM2.Size = new System.Drawing.Size(57, 24);
            this.lblCOM2.TabIndex = 0;
            this.lblCOM2.Text = "工序2";
            // 
            // cmbCOM1
            // 
            this.cmbCOM1.FormattingEnabled = true;
            this.cmbCOM1.Location = new System.Drawing.Point(86, 41);
            this.cmbCOM1.Name = "cmbCOM1";
            this.cmbCOM1.Size = new System.Drawing.Size(94, 31);
            this.cmbCOM1.TabIndex = 1;
            // 
            // lblCOM1
            // 
            this.lblCOM1.AutoSize = true;
            this.lblCOM1.Location = new System.Drawing.Point(23, 45);
            this.lblCOM1.Name = "lblCOM1";
            this.lblCOM1.Size = new System.Drawing.Size(57, 24);
            this.lblCOM1.TabIndex = 0;
            this.lblCOM1.Text = "工序1";
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.dgvPrintDirectory);
            this.groupBox16.Controls.Add(this.printRefresh);
            this.groupBox16.Controls.Add(this.lblTips);
            this.groupBox16.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox16.Location = new System.Drawing.Point(2, 312);
            this.groupBox16.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox16.Size = new System.Drawing.Size(1882, 634);
            this.groupBox16.TabIndex = 82;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "工单号对应文件夹关系";
            // 
            // dgvPrintDirectory
            // 
            this.dgvPrintDirectory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrintDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPrintDirectory.Location = new System.Drawing.Point(2, 174);
            this.dgvPrintDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.dgvPrintDirectory.Name = "dgvPrintDirectory";
            this.dgvPrintDirectory.RowHeadersWidth = 51;
            this.dgvPrintDirectory.RowTemplate.Height = 27;
            this.dgvPrintDirectory.Size = new System.Drawing.Size(1878, 458);
            this.dgvPrintDirectory.TabIndex = 64;
            // 
            // printRefresh
            // 
            this.printRefresh.Dock = System.Windows.Forms.DockStyle.Top;
            this.printRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.printRefresh.Location = new System.Drawing.Point(2, 121);
            this.printRefresh.Margin = new System.Windows.Forms.Padding(2);
            this.printRefresh.Name = "printRefresh";
            this.printRefresh.Size = new System.Drawing.Size(1878, 53);
            this.printRefresh.TabIndex = 65;
            this.printRefresh.Text = "刷新";
            this.printRefresh.UseVisualStyleBackColor = true;
            // 
            // lblTips
            // 
            this.lblTips.AutoSize = true;
            this.lblTips.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTips.ForeColor = System.Drawing.Color.Red;
            this.lblTips.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTips.Location = new System.Drawing.Point(2, 25);
            this.lblTips.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTips.Name = "lblTips";
            this.lblTips.Size = new System.Drawing.Size(1213, 96);
            this.lblTips.TabIndex = 63;
            this.lblTips.Text = "备注：\r\n1.拍照存放目录如有多个请以英文状态下逗号分割，如”封口图片,外壳图片,其它图片“,必须对应PLC地址，有几个目录就要对应几个PLC地址以及图片数量\r\n" +
    "2.默认读取路径格式为“路径/多次拍照存放目录/工单号对应目录名/文件名”\r\n\r\n";
            // 
            // tabPage11
            // 
            this.tabPage11.Controls.Add(this.groupBox30);
            this.tabPage11.Controls.Add(this.groupBox29);
            this.tabPage11.Controls.Add(this.groupBox28);
            this.tabPage11.Controls.Add(this.groupBox27);
            this.tabPage11.Controls.Add(this.groupBox26);
            this.tabPage11.Controls.Add(this.groupBox25);
            this.tabPage11.Controls.Add(this.groupBox24);
            this.tabPage11.Controls.Add(this.button2);
            this.tabPage11.Controls.Add(this.groupBox14);
            this.tabPage11.Location = new System.Drawing.Point(4, 32);
            this.tabPage11.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage11.Size = new System.Drawing.Size(1886, 948);
            this.tabPage11.TabIndex = 1;
            this.tabPage11.Text = "地址维护";
            this.tabPage11.UseVisualStyleBackColor = true;
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.tableLayoutPanel28);
            this.groupBox30.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox30.Location = new System.Drawing.Point(448, 534);
            this.groupBox30.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox30.Name = "groupBox30";
            this.groupBox30.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox30.Size = new System.Drawing.Size(432, 169);
            this.groupBox30.TabIndex = 7;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "工序3扭力采集与转发";
            // 
            // tableLayoutPanel28
            // 
            this.tableLayoutPanel28.ColumnCount = 4;
            this.tableLayoutPanel28.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel28.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel28.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel28.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel28.Controls.Add(this.txtToqueMin3, 3, 1);
            this.tableLayoutPanel28.Controls.Add(this.label123, 2, 1);
            this.tableLayoutPanel28.Controls.Add(this.label124, 0, 0);
            this.tableLayoutPanel28.Controls.Add(this.txtTorqueResult3, 1, 0);
            this.tableLayoutPanel28.Controls.Add(this.label125, 2, 0);
            this.tableLayoutPanel28.Controls.Add(this.txtTorqueValue3, 3, 0);
            this.tableLayoutPanel28.Controls.Add(this.label126, 0, 1);
            this.tableLayoutPanel28.Controls.Add(this.txtToqueMax3, 1, 1);
            this.tableLayoutPanel28.Controls.Add(this.label127, 0, 2);
            this.tableLayoutPanel28.Controls.Add(this.txtRequest3, 1, 2);
            this.tableLayoutPanel28.Controls.Add(this.label128, 2, 2);
            this.tableLayoutPanel28.Controls.Add(this.txtAcknowledge3, 3, 2);
            this.tableLayoutPanel28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel28.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel28.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel28.Name = "tableLayoutPanel28";
            this.tableLayoutPanel28.RowCount = 3;
            this.tableLayoutPanel28.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel28.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel28.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel28.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel28.Size = new System.Drawing.Size(428, 141);
            this.tableLayoutPanel28.TabIndex = 0;
            // 
            // txtToqueMin3
            // 
            this.txtToqueMin3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtToqueMin3.Location = new System.Drawing.Point(306, 49);
            this.txtToqueMin3.Margin = new System.Windows.Forms.Padding(2);
            this.txtToqueMin3.Name = "txtToqueMin3";
            this.txtToqueMin3.Size = new System.Drawing.Size(120, 31);
            this.txtToqueMin3.TabIndex = 4;
            // 
            // label123
            // 
            this.label123.AutoSize = true;
            this.label123.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label123.Location = new System.Drawing.Point(216, 47);
            this.label123.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label123.Name = "label123";
            this.label123.Padding = new System.Windows.Forms.Padding(2);
            this.label123.Size = new System.Drawing.Size(86, 47);
            this.label123.TabIndex = 3;
            this.label123.Text = "扭力下限";
            this.label123.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label124
            // 
            this.label124.AutoSize = true;
            this.label124.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label124.Location = new System.Drawing.Point(2, 0);
            this.label124.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label124.Name = "label124";
            this.label124.Padding = new System.Windows.Forms.Padding(2);
            this.label124.Size = new System.Drawing.Size(86, 47);
            this.label124.TabIndex = 0;
            this.label124.Text = "扭力结果";
            this.label124.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTorqueResult3
            // 
            this.txtTorqueResult3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTorqueResult3.Location = new System.Drawing.Point(92, 2);
            this.txtTorqueResult3.Margin = new System.Windows.Forms.Padding(2);
            this.txtTorqueResult3.Name = "txtTorqueResult3";
            this.txtTorqueResult3.Size = new System.Drawing.Size(120, 31);
            this.txtTorqueResult3.TabIndex = 2;
            // 
            // label125
            // 
            this.label125.AutoSize = true;
            this.label125.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label125.Location = new System.Drawing.Point(216, 0);
            this.label125.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label125.Name = "label125";
            this.label125.Padding = new System.Windows.Forms.Padding(2);
            this.label125.Size = new System.Drawing.Size(86, 47);
            this.label125.TabIndex = 1;
            this.label125.Text = "扭力值";
            this.label125.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTorqueValue3
            // 
            this.txtTorqueValue3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTorqueValue3.Location = new System.Drawing.Point(306, 2);
            this.txtTorqueValue3.Margin = new System.Windows.Forms.Padding(2);
            this.txtTorqueValue3.Name = "txtTorqueValue3";
            this.txtTorqueValue3.Size = new System.Drawing.Size(120, 31);
            this.txtTorqueValue3.TabIndex = 2;
            // 
            // label126
            // 
            this.label126.AutoSize = true;
            this.label126.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label126.Location = new System.Drawing.Point(2, 47);
            this.label126.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label126.Name = "label126";
            this.label126.Padding = new System.Windows.Forms.Padding(2);
            this.label126.Size = new System.Drawing.Size(86, 47);
            this.label126.TabIndex = 1;
            this.label126.Text = "扭力上限";
            this.label126.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtToqueMax3
            // 
            this.txtToqueMax3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtToqueMax3.Location = new System.Drawing.Point(92, 49);
            this.txtToqueMax3.Margin = new System.Windows.Forms.Padding(2);
            this.txtToqueMax3.Name = "txtToqueMax3";
            this.txtToqueMax3.Size = new System.Drawing.Size(120, 31);
            this.txtToqueMax3.TabIndex = 2;
            // 
            // label127
            // 
            this.label127.AutoSize = true;
            this.label127.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label127.Location = new System.Drawing.Point(2, 94);
            this.label127.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label127.Name = "label127";
            this.label127.Padding = new System.Windows.Forms.Padding(2);
            this.label127.Size = new System.Drawing.Size(86, 47);
            this.label127.TabIndex = 1;
            this.label127.Text = "转发成功";
            this.label127.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtRequest3
            // 
            this.txtRequest3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRequest3.Location = new System.Drawing.Point(92, 96);
            this.txtRequest3.Margin = new System.Windows.Forms.Padding(2);
            this.txtRequest3.Name = "txtRequest3";
            this.txtRequest3.Size = new System.Drawing.Size(120, 31);
            this.txtRequest3.TabIndex = 2;
            // 
            // label128
            // 
            this.label128.AutoSize = true;
            this.label128.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label128.Location = new System.Drawing.Point(216, 94);
            this.label128.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label128.Name = "label128";
            this.label128.Size = new System.Drawing.Size(86, 47);
            this.label128.TabIndex = 1;
            this.label128.Text = "接收成功";
            this.label128.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtAcknowledge3
            // 
            this.txtAcknowledge3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAcknowledge3.Location = new System.Drawing.Point(306, 96);
            this.txtAcknowledge3.Margin = new System.Windows.Forms.Padding(2);
            this.txtAcknowledge3.Name = "txtAcknowledge3";
            this.txtAcknowledge3.Size = new System.Drawing.Size(120, 31);
            this.txtAcknowledge3.TabIndex = 2;
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.tableLayoutPanel27);
            this.groupBox29.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox29.Location = new System.Drawing.Point(9, 534);
            this.groupBox29.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox29.Size = new System.Drawing.Size(432, 169);
            this.groupBox29.TabIndex = 6;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "工序1扭力采集与转发";
            // 
            // tableLayoutPanel27
            // 
            this.tableLayoutPanel27.ColumnCount = 4;
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel27.Controls.Add(this.txtToqueMin1, 3, 1);
            this.tableLayoutPanel27.Controls.Add(this.label122, 2, 1);
            this.tableLayoutPanel27.Controls.Add(this.label64, 0, 0);
            this.tableLayoutPanel27.Controls.Add(this.txtTorqueResult1, 1, 0);
            this.tableLayoutPanel27.Controls.Add(this.label65, 2, 0);
            this.tableLayoutPanel27.Controls.Add(this.txtTorqueValue1, 3, 0);
            this.tableLayoutPanel27.Controls.Add(this.label67, 0, 1);
            this.tableLayoutPanel27.Controls.Add(this.txtToqueMax1, 1, 1);
            this.tableLayoutPanel27.Controls.Add(this.label120, 0, 2);
            this.tableLayoutPanel27.Controls.Add(this.txtRequest1, 1, 2);
            this.tableLayoutPanel27.Controls.Add(this.label121, 2, 2);
            this.tableLayoutPanel27.Controls.Add(this.txtAcknowledge1, 3, 2);
            this.tableLayoutPanel27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel27.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel27.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel27.Name = "tableLayoutPanel27";
            this.tableLayoutPanel27.RowCount = 3;
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel27.Size = new System.Drawing.Size(428, 141);
            this.tableLayoutPanel27.TabIndex = 0;
            // 
            // txtToqueMin1
            // 
            this.txtToqueMin1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtToqueMin1.Location = new System.Drawing.Point(306, 49);
            this.txtToqueMin1.Margin = new System.Windows.Forms.Padding(2);
            this.txtToqueMin1.Name = "txtToqueMin1";
            this.txtToqueMin1.Size = new System.Drawing.Size(120, 31);
            this.txtToqueMin1.TabIndex = 4;
            // 
            // label122
            // 
            this.label122.AutoSize = true;
            this.label122.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label122.Location = new System.Drawing.Point(216, 47);
            this.label122.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label122.Name = "label122";
            this.label122.Padding = new System.Windows.Forms.Padding(2);
            this.label122.Size = new System.Drawing.Size(86, 47);
            this.label122.TabIndex = 3;
            this.label122.Text = "扭力下限";
            this.label122.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label64.Location = new System.Drawing.Point(2, 0);
            this.label64.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label64.Name = "label64";
            this.label64.Padding = new System.Windows.Forms.Padding(2);
            this.label64.Size = new System.Drawing.Size(86, 47);
            this.label64.TabIndex = 0;
            this.label64.Text = "扭力结果";
            this.label64.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTorqueResult1
            // 
            this.txtTorqueResult1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTorqueResult1.Location = new System.Drawing.Point(92, 2);
            this.txtTorqueResult1.Margin = new System.Windows.Forms.Padding(2);
            this.txtTorqueResult1.Name = "txtTorqueResult1";
            this.txtTorqueResult1.Size = new System.Drawing.Size(120, 31);
            this.txtTorqueResult1.TabIndex = 2;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label65.Location = new System.Drawing.Point(216, 0);
            this.label65.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label65.Name = "label65";
            this.label65.Padding = new System.Windows.Forms.Padding(2);
            this.label65.Size = new System.Drawing.Size(86, 47);
            this.label65.TabIndex = 1;
            this.label65.Text = "扭力值";
            this.label65.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTorqueValue1
            // 
            this.txtTorqueValue1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTorqueValue1.Location = new System.Drawing.Point(306, 2);
            this.txtTorqueValue1.Margin = new System.Windows.Forms.Padding(2);
            this.txtTorqueValue1.Name = "txtTorqueValue1";
            this.txtTorqueValue1.Size = new System.Drawing.Size(120, 31);
            this.txtTorqueValue1.TabIndex = 2;
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label67.Location = new System.Drawing.Point(2, 47);
            this.label67.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label67.Name = "label67";
            this.label67.Padding = new System.Windows.Forms.Padding(2);
            this.label67.Size = new System.Drawing.Size(86, 47);
            this.label67.TabIndex = 1;
            this.label67.Text = "扭力上限";
            this.label67.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtToqueMax1
            // 
            this.txtToqueMax1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtToqueMax1.Location = new System.Drawing.Point(92, 49);
            this.txtToqueMax1.Margin = new System.Windows.Forms.Padding(2);
            this.txtToqueMax1.Name = "txtToqueMax1";
            this.txtToqueMax1.Size = new System.Drawing.Size(120, 31);
            this.txtToqueMax1.TabIndex = 2;
            // 
            // label120
            // 
            this.label120.AutoSize = true;
            this.label120.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label120.Location = new System.Drawing.Point(2, 94);
            this.label120.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label120.Name = "label120";
            this.label120.Padding = new System.Windows.Forms.Padding(2);
            this.label120.Size = new System.Drawing.Size(86, 47);
            this.label120.TabIndex = 1;
            this.label120.Text = "转发成功";
            this.label120.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtRequest1
            // 
            this.txtRequest1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRequest1.Location = new System.Drawing.Point(92, 96);
            this.txtRequest1.Margin = new System.Windows.Forms.Padding(2);
            this.txtRequest1.Name = "txtRequest1";
            this.txtRequest1.Size = new System.Drawing.Size(120, 31);
            this.txtRequest1.TabIndex = 2;
            // 
            // label121
            // 
            this.label121.AutoSize = true;
            this.label121.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label121.Location = new System.Drawing.Point(216, 94);
            this.label121.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label121.Name = "label121";
            this.label121.Size = new System.Drawing.Size(86, 47);
            this.label121.TabIndex = 1;
            this.label121.Text = "接收成功";
            this.label121.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtAcknowledge1
            // 
            this.txtAcknowledge1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAcknowledge1.Location = new System.Drawing.Point(306, 96);
            this.txtAcknowledge1.Margin = new System.Windows.Forms.Padding(2);
            this.txtAcknowledge1.Name = "txtAcknowledge1";
            this.txtAcknowledge1.Size = new System.Drawing.Size(120, 31);
            this.txtAcknowledge1.TabIndex = 2;
            // 
            // groupBox28
            // 
            this.groupBox28.Controls.Add(this.tableLayoutPanel23);
            this.groupBox28.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox28.Location = new System.Drawing.Point(884, 6);
            this.groupBox28.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox28.Size = new System.Drawing.Size(438, 344);
            this.groupBox28.TabIndex = 7;
            this.groupBox28.TabStop = false;
            this.groupBox28.Text = "设备参数";
            // 
            // tableLayoutPanel23
            // 
            this.tableLayoutPanel23.ColumnCount = 4;
            this.tableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel23.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel23.Controls.Add(this.txtRecoverySignal, 1, 7);
            this.tableLayoutPanel23.Controls.Add(this.label113, 0, 7);
            this.tableLayoutPanel23.Controls.Add(this.txtContinueProduce, 3, 5);
            this.tableLayoutPanel23.Controls.Add(this.label112, 2, 5);
            this.tableLayoutPanel23.Controls.Add(this.txtProgramNameLength, 3, 2);
            this.tableLayoutPanel23.Controls.Add(this.label111, 2, 2);
            this.tableLayoutPanel23.Controls.Add(this.txtDeviceStatus, 3, 1);
            this.tableLayoutPanel23.Controls.Add(this.label110, 2, 1);
            this.tableLayoutPanel23.Controls.Add(this.txtNotGoodsProducts, 3, 0);
            this.tableLayoutPanel23.Controls.Add(this.label109, 2, 0);
            this.tableLayoutPanel23.Controls.Add(this.label97, 0, 0);
            this.tableLayoutPanel23.Controls.Add(this.label100, 0, 1);
            this.tableLayoutPanel23.Controls.Add(this.label101, 0, 2);
            this.tableLayoutPanel23.Controls.Add(this.label102, 0, 3);
            this.tableLayoutPanel23.Controls.Add(this.txtGoodsProducts, 1, 0);
            this.tableLayoutPanel23.Controls.Add(this.txtProduceCount, 1, 1);
            this.tableLayoutPanel23.Controls.Add(this.txtDeviceProgramName, 1, 2);
            this.tableLayoutPanel23.Controls.Add(this.txtProductType, 1, 3);
            this.tableLayoutPanel23.Controls.Add(this.label103, 2, 3);
            this.tableLayoutPanel23.Controls.Add(this.txtProductTypeLength, 3, 3);
            this.tableLayoutPanel23.Controls.Add(this.label104, 0, 4);
            this.tableLayoutPanel23.Controls.Add(this.txtBarcodeRule, 1, 4);
            this.tableLayoutPanel23.Controls.Add(this.label105, 2, 4);
            this.tableLayoutPanel23.Controls.Add(this.txtBarcodeRuleLength, 3, 4);
            this.tableLayoutPanel23.Controls.Add(this.label106, 0, 5);
            this.tableLayoutPanel23.Controls.Add(this.txtModelSwitch, 1, 5);
            this.tableLayoutPanel23.Controls.Add(this.label107, 0, 6);
            this.tableLayoutPanel23.Controls.Add(this.txtPlcHeartBeat, 1, 6);
            this.tableLayoutPanel23.Controls.Add(this.label108, 2, 6);
            this.tableLayoutPanel23.Controls.Add(this.txtPcHeartBeat, 3, 6);
            this.tableLayoutPanel23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel23.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel23.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel23.Name = "tableLayoutPanel23";
            this.tableLayoutPanel23.RowCount = 8;
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel23.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.50328F));
            this.tableLayoutPanel23.Size = new System.Drawing.Size(434, 316);
            this.tableLayoutPanel23.TabIndex = 0;
            // 
            // txtRecoverySignal
            // 
            this.tableLayoutPanel23.SetColumnSpan(this.txtRecoverySignal, 3);
            this.txtRecoverySignal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecoverySignal.Location = new System.Drawing.Point(110, 275);
            this.txtRecoverySignal.Margin = new System.Windows.Forms.Padding(2);
            this.txtRecoverySignal.Name = "txtRecoverySignal";
            this.txtRecoverySignal.Size = new System.Drawing.Size(322, 31);
            this.txtRecoverySignal.TabIndex = 12;
            // 
            // label113
            // 
            this.label113.AutoSize = true;
            this.label113.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label113.Location = new System.Drawing.Point(2, 273);
            this.label113.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label113.Name = "label113";
            this.label113.Padding = new System.Windows.Forms.Padding(2);
            this.label113.Size = new System.Drawing.Size(104, 43);
            this.label113.TabIndex = 11;
            this.label113.Text = "复位信号";
            this.label113.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtContinueProduce
            // 
            this.txtContinueProduce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtContinueProduce.Location = new System.Drawing.Point(327, 197);
            this.txtContinueProduce.Margin = new System.Windows.Forms.Padding(2);
            this.txtContinueProduce.Name = "txtContinueProduce";
            this.txtContinueProduce.Size = new System.Drawing.Size(105, 31);
            this.txtContinueProduce.TabIndex = 10;
            // 
            // label112
            // 
            this.label112.AutoSize = true;
            this.label112.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label112.Location = new System.Drawing.Point(219, 195);
            this.label112.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label112.Name = "label112";
            this.label112.Padding = new System.Windows.Forms.Padding(2);
            this.label112.Size = new System.Drawing.Size(104, 39);
            this.label112.TabIndex = 9;
            this.label112.Text = "继续生产";
            this.label112.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProgramNameLength
            // 
            this.txtProgramNameLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProgramNameLength.Location = new System.Drawing.Point(327, 80);
            this.txtProgramNameLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtProgramNameLength.Name = "txtProgramNameLength";
            this.txtProgramNameLength.Size = new System.Drawing.Size(105, 31);
            this.txtProgramNameLength.TabIndex = 8;
            // 
            // label111
            // 
            this.label111.AutoSize = true;
            this.label111.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label111.Location = new System.Drawing.Point(219, 78);
            this.label111.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label111.Name = "label111";
            this.label111.Padding = new System.Windows.Forms.Padding(2);
            this.label111.Size = new System.Drawing.Size(104, 39);
            this.label111.TabIndex = 7;
            this.label111.Text = "程序名长度";
            this.label111.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDeviceStatus
            // 
            this.txtDeviceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDeviceStatus.Location = new System.Drawing.Point(327, 41);
            this.txtDeviceStatus.Margin = new System.Windows.Forms.Padding(2);
            this.txtDeviceStatus.Name = "txtDeviceStatus";
            this.txtDeviceStatus.Size = new System.Drawing.Size(105, 31);
            this.txtDeviceStatus.TabIndex = 6;
            // 
            // label110
            // 
            this.label110.AutoSize = true;
            this.label110.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label110.Location = new System.Drawing.Point(219, 39);
            this.label110.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label110.Name = "label110";
            this.label110.Padding = new System.Windows.Forms.Padding(2);
            this.label110.Size = new System.Drawing.Size(104, 39);
            this.label110.TabIndex = 5;
            this.label110.Text = "设备状态";
            this.label110.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNotGoodsProducts
            // 
            this.txtNotGoodsProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotGoodsProducts.Location = new System.Drawing.Point(327, 2);
            this.txtNotGoodsProducts.Margin = new System.Windows.Forms.Padding(2);
            this.txtNotGoodsProducts.Name = "txtNotGoodsProducts";
            this.txtNotGoodsProducts.Size = new System.Drawing.Size(105, 31);
            this.txtNotGoodsProducts.TabIndex = 4;
            // 
            // label109
            // 
            this.label109.AutoSize = true;
            this.label109.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label109.Location = new System.Drawing.Point(219, 0);
            this.label109.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label109.Name = "label109";
            this.label109.Padding = new System.Windows.Forms.Padding(2);
            this.label109.Size = new System.Drawing.Size(104, 39);
            this.label109.TabIndex = 3;
            this.label109.Text = "不良数";
            this.label109.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label97.Location = new System.Drawing.Point(2, 0);
            this.label97.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label97.Name = "label97";
            this.label97.Padding = new System.Windows.Forms.Padding(2);
            this.label97.Size = new System.Drawing.Size(104, 39);
            this.label97.TabIndex = 0;
            this.label97.Text = "良品数";
            this.label97.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label100.Location = new System.Drawing.Point(2, 39);
            this.label100.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label100.Name = "label100";
            this.label100.Padding = new System.Windows.Forms.Padding(2);
            this.label100.Size = new System.Drawing.Size(104, 39);
            this.label100.TabIndex = 1;
            this.label100.Text = "生产总数";
            this.label100.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label101
            // 
            this.label101.AutoSize = true;
            this.label101.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label101.Location = new System.Drawing.Point(2, 78);
            this.label101.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label101.Name = "label101";
            this.label101.Padding = new System.Windows.Forms.Padding(2);
            this.label101.Size = new System.Drawing.Size(104, 39);
            this.label101.TabIndex = 1;
            this.label101.Text = "设备程序名";
            this.label101.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label102
            // 
            this.label102.AutoSize = true;
            this.label102.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label102.Location = new System.Drawing.Point(2, 117);
            this.label102.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label102.Name = "label102";
            this.label102.Padding = new System.Windows.Forms.Padding(2);
            this.label102.Size = new System.Drawing.Size(104, 39);
            this.label102.TabIndex = 1;
            this.label102.Text = "产品型号";
            this.label102.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGoodsProducts
            // 
            this.txtGoodsProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGoodsProducts.Location = new System.Drawing.Point(110, 2);
            this.txtGoodsProducts.Margin = new System.Windows.Forms.Padding(2);
            this.txtGoodsProducts.Name = "txtGoodsProducts";
            this.txtGoodsProducts.Size = new System.Drawing.Size(105, 31);
            this.txtGoodsProducts.TabIndex = 2;
            // 
            // txtProduceCount
            // 
            this.txtProduceCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProduceCount.Location = new System.Drawing.Point(110, 41);
            this.txtProduceCount.Margin = new System.Windows.Forms.Padding(2);
            this.txtProduceCount.Name = "txtProduceCount";
            this.txtProduceCount.Size = new System.Drawing.Size(105, 31);
            this.txtProduceCount.TabIndex = 2;
            // 
            // txtDeviceProgramName
            // 
            this.txtDeviceProgramName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDeviceProgramName.Location = new System.Drawing.Point(110, 80);
            this.txtDeviceProgramName.Margin = new System.Windows.Forms.Padding(2);
            this.txtDeviceProgramName.Name = "txtDeviceProgramName";
            this.txtDeviceProgramName.Size = new System.Drawing.Size(105, 31);
            this.txtDeviceProgramName.TabIndex = 2;
            // 
            // txtProductType
            // 
            this.txtProductType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductType.Location = new System.Drawing.Point(110, 119);
            this.txtProductType.Margin = new System.Windows.Forms.Padding(2);
            this.txtProductType.Name = "txtProductType";
            this.txtProductType.Size = new System.Drawing.Size(105, 31);
            this.txtProductType.TabIndex = 2;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label103.Location = new System.Drawing.Point(219, 117);
            this.label103.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(104, 39);
            this.label103.TabIndex = 1;
            this.label103.Text = "型号长度";
            this.label103.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtProductTypeLength
            // 
            this.txtProductTypeLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductTypeLength.Location = new System.Drawing.Point(327, 119);
            this.txtProductTypeLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtProductTypeLength.Name = "txtProductTypeLength";
            this.txtProductTypeLength.Size = new System.Drawing.Size(105, 31);
            this.txtProductTypeLength.TabIndex = 2;
            // 
            // label104
            // 
            this.label104.AutoSize = true;
            this.label104.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label104.Location = new System.Drawing.Point(2, 156);
            this.label104.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label104.Name = "label104";
            this.label104.Padding = new System.Windows.Forms.Padding(2);
            this.label104.Size = new System.Drawing.Size(104, 39);
            this.label104.TabIndex = 1;
            this.label104.Text = "条码规则";
            this.label104.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBarcodeRule
            // 
            this.txtBarcodeRule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeRule.Location = new System.Drawing.Point(110, 158);
            this.txtBarcodeRule.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeRule.Name = "txtBarcodeRule";
            this.txtBarcodeRule.Size = new System.Drawing.Size(105, 31);
            this.txtBarcodeRule.TabIndex = 2;
            // 
            // label105
            // 
            this.label105.AutoSize = true;
            this.label105.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label105.Location = new System.Drawing.Point(219, 156);
            this.label105.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label105.Name = "label105";
            this.label105.Size = new System.Drawing.Size(104, 39);
            this.label105.TabIndex = 1;
            this.label105.Text = "规则长度";
            this.label105.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBarcodeRuleLength
            // 
            this.txtBarcodeRuleLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeRuleLength.Location = new System.Drawing.Point(327, 158);
            this.txtBarcodeRuleLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeRuleLength.Name = "txtBarcodeRuleLength";
            this.txtBarcodeRuleLength.Size = new System.Drawing.Size(105, 31);
            this.txtBarcodeRuleLength.TabIndex = 2;
            // 
            // label106
            // 
            this.label106.AutoSize = true;
            this.label106.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label106.Location = new System.Drawing.Point(2, 195);
            this.label106.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label106.Name = "label106";
            this.label106.Padding = new System.Windows.Forms.Padding(2);
            this.label106.Size = new System.Drawing.Size(104, 39);
            this.label106.TabIndex = 1;
            this.label106.Text = "产品换型";
            this.label106.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtModelSwitch
            // 
            this.txtModelSwitch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtModelSwitch.Location = new System.Drawing.Point(110, 197);
            this.txtModelSwitch.Margin = new System.Windows.Forms.Padding(2);
            this.txtModelSwitch.Name = "txtModelSwitch";
            this.txtModelSwitch.Size = new System.Drawing.Size(105, 31);
            this.txtModelSwitch.TabIndex = 2;
            // 
            // label107
            // 
            this.label107.AutoSize = true;
            this.label107.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label107.Location = new System.Drawing.Point(2, 234);
            this.label107.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label107.Name = "label107";
            this.label107.Padding = new System.Windows.Forms.Padding(2);
            this.label107.Size = new System.Drawing.Size(104, 39);
            this.label107.TabIndex = 1;
            this.label107.Text = "PLC心跳";
            this.label107.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPlcHeartBeat
            // 
            this.txtPlcHeartBeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPlcHeartBeat.Location = new System.Drawing.Point(110, 236);
            this.txtPlcHeartBeat.Margin = new System.Windows.Forms.Padding(2);
            this.txtPlcHeartBeat.Name = "txtPlcHeartBeat";
            this.txtPlcHeartBeat.Size = new System.Drawing.Size(105, 31);
            this.txtPlcHeartBeat.TabIndex = 2;
            // 
            // label108
            // 
            this.label108.AutoSize = true;
            this.label108.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label108.Location = new System.Drawing.Point(219, 234);
            this.label108.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label108.Name = "label108";
            this.label108.Size = new System.Drawing.Size(104, 39);
            this.label108.TabIndex = 1;
            this.label108.Text = "PC心跳";
            this.label108.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPcHeartBeat
            // 
            this.txtPcHeartBeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPcHeartBeat.Location = new System.Drawing.Point(327, 236);
            this.txtPcHeartBeat.Margin = new System.Windows.Forms.Padding(2);
            this.txtPcHeartBeat.Name = "txtPcHeartBeat";
            this.txtPcHeartBeat.Size = new System.Drawing.Size(105, 31);
            this.txtPcHeartBeat.TabIndex = 2;
            // 
            // groupBox27
            // 
            this.groupBox27.Controls.Add(this.tableLayoutPanel22);
            this.groupBox27.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox27.Location = new System.Drawing.Point(6, 359);
            this.groupBox27.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox27.Size = new System.Drawing.Size(432, 169);
            this.groupBox27.TabIndex = 6;
            this.groupBox27.TabStop = false;
            this.groupBox27.Text = "打印条码";
            // 
            // tableLayoutPanel22
            // 
            this.tableLayoutPanel22.ColumnCount = 4;
            this.tableLayoutPanel22.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel22.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel22.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel22.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel22.Controls.Add(this.label95, 0, 0);
            this.tableLayoutPanel22.Controls.Add(this.label96, 0, 1);
            this.tableLayoutPanel22.Controls.Add(this.label98, 0, 2);
            this.tableLayoutPanel22.Controls.Add(this.txtPrintTrigger, 1, 0);
            this.tableLayoutPanel22.Controls.Add(this.txtPrintFeedback, 1, 1);
            this.tableLayoutPanel22.Controls.Add(this.txtBarcodeToPrint, 1, 2);
            this.tableLayoutPanel22.Controls.Add(this.label99, 2, 2);
            this.tableLayoutPanel22.Controls.Add(this.txtBarcodeToPrintLength, 3, 2);
            this.tableLayoutPanel22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel22.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel22.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel22.Name = "tableLayoutPanel22";
            this.tableLayoutPanel22.RowCount = 3;
            this.tableLayoutPanel22.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel22.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel22.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel22.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel22.Size = new System.Drawing.Size(428, 141);
            this.tableLayoutPanel22.TabIndex = 0;
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label95.Location = new System.Drawing.Point(2, 0);
            this.label95.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label95.Name = "label95";
            this.label95.Padding = new System.Windows.Forms.Padding(2);
            this.label95.Size = new System.Drawing.Size(86, 47);
            this.label95.TabIndex = 0;
            this.label95.Text = "触发打印";
            this.label95.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label96.Location = new System.Drawing.Point(2, 47);
            this.label96.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label96.Name = "label96";
            this.label96.Padding = new System.Windows.Forms.Padding(2);
            this.label96.Size = new System.Drawing.Size(86, 47);
            this.label96.TabIndex = 1;
            this.label96.Text = "打印结果";
            this.label96.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label98.Location = new System.Drawing.Point(2, 94);
            this.label98.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label98.Name = "label98";
            this.label98.Padding = new System.Windows.Forms.Padding(2);
            this.label98.Size = new System.Drawing.Size(86, 47);
            this.label98.TabIndex = 1;
            this.label98.Text = "条码地址";
            this.label98.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtPrintTrigger
            // 
            this.tableLayoutPanel22.SetColumnSpan(this.txtPrintTrigger, 3);
            this.txtPrintTrigger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrintTrigger.Location = new System.Drawing.Point(92, 2);
            this.txtPrintTrigger.Margin = new System.Windows.Forms.Padding(2);
            this.txtPrintTrigger.Name = "txtPrintTrigger";
            this.txtPrintTrigger.Size = new System.Drawing.Size(334, 31);
            this.txtPrintTrigger.TabIndex = 2;
            // 
            // txtPrintFeedback
            // 
            this.tableLayoutPanel22.SetColumnSpan(this.txtPrintFeedback, 3);
            this.txtPrintFeedback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrintFeedback.Location = new System.Drawing.Point(92, 49);
            this.txtPrintFeedback.Margin = new System.Windows.Forms.Padding(2);
            this.txtPrintFeedback.Name = "txtPrintFeedback";
            this.txtPrintFeedback.Size = new System.Drawing.Size(334, 31);
            this.txtPrintFeedback.TabIndex = 2;
            // 
            // txtBarcodeToPrint
            // 
            this.txtBarcodeToPrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToPrint.Location = new System.Drawing.Point(92, 96);
            this.txtBarcodeToPrint.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToPrint.Name = "txtBarcodeToPrint";
            this.txtBarcodeToPrint.Size = new System.Drawing.Size(122, 31);
            this.txtBarcodeToPrint.TabIndex = 2;
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label99.Location = new System.Drawing.Point(218, 94);
            this.label99.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(82, 47);
            this.label99.TabIndex = 1;
            this.label99.Text = "条码长度";
            this.label99.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBarcodeToPrintLength
            // 
            this.txtBarcodeToPrintLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToPrintLength.Location = new System.Drawing.Point(304, 96);
            this.txtBarcodeToPrintLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToPrintLength.Name = "txtBarcodeToPrintLength";
            this.txtBarcodeToPrintLength.Size = new System.Drawing.Size(122, 31);
            this.txtBarcodeToPrintLength.TabIndex = 2;
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.tableLayoutPanel21);
            this.groupBox26.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox26.Location = new System.Drawing.Point(448, 359);
            this.groupBox26.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox26.Size = new System.Drawing.Size(432, 169);
            this.groupBox26.TabIndex = 5;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "工序3过站";
            // 
            // tableLayoutPanel21
            // 
            this.tableLayoutPanel21.ColumnCount = 4;
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel21.Controls.Add(this.label90, 0, 0);
            this.tableLayoutPanel21.Controls.Add(this.txtTriggerUpload3, 1, 0);
            this.tableLayoutPanel21.Controls.Add(this.label91, 2, 0);
            this.tableLayoutPanel21.Controls.Add(this.txtFeedback3, 3, 0);
            this.tableLayoutPanel21.Controls.Add(this.label92, 0, 1);
            this.tableLayoutPanel21.Controls.Add(this.txtProductResult3, 1, 1);
            this.tableLayoutPanel21.Controls.Add(this.label93, 0, 2);
            this.tableLayoutPanel21.Controls.Add(this.txtBarcodeToUpload3, 1, 2);
            this.tableLayoutPanel21.Controls.Add(this.label94, 2, 2);
            this.tableLayoutPanel21.Controls.Add(this.txtBarcodeToUploadLength3, 3, 2);
            this.tableLayoutPanel21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel21.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel21.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel21.Name = "tableLayoutPanel21";
            this.tableLayoutPanel21.RowCount = 3;
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel21.Size = new System.Drawing.Size(428, 141);
            this.tableLayoutPanel21.TabIndex = 0;
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label90.Location = new System.Drawing.Point(2, 0);
            this.label90.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label90.Name = "label90";
            this.label90.Padding = new System.Windows.Forms.Padding(2);
            this.label90.Size = new System.Drawing.Size(86, 47);
            this.label90.TabIndex = 0;
            this.label90.Text = "触发上传";
            this.label90.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTriggerUpload3
            // 
            this.txtTriggerUpload3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTriggerUpload3.Location = new System.Drawing.Point(92, 2);
            this.txtTriggerUpload3.Margin = new System.Windows.Forms.Padding(2);
            this.txtTriggerUpload3.Name = "txtTriggerUpload3";
            this.txtTriggerUpload3.Size = new System.Drawing.Size(120, 31);
            this.txtTriggerUpload3.TabIndex = 2;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label91.Location = new System.Drawing.Point(216, 0);
            this.label91.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label91.Name = "label91";
            this.label91.Padding = new System.Windows.Forms.Padding(2);
            this.label91.Size = new System.Drawing.Size(86, 47);
            this.label91.TabIndex = 1;
            this.label91.Text = "上传结果";
            this.label91.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtFeedback3
            // 
            this.txtFeedback3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFeedback3.Location = new System.Drawing.Point(306, 2);
            this.txtFeedback3.Margin = new System.Windows.Forms.Padding(2);
            this.txtFeedback3.Name = "txtFeedback3";
            this.txtFeedback3.Size = new System.Drawing.Size(120, 31);
            this.txtFeedback3.TabIndex = 2;
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label92.Location = new System.Drawing.Point(2, 47);
            this.label92.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label92.Name = "label92";
            this.label92.Padding = new System.Windows.Forms.Padding(2);
            this.label92.Size = new System.Drawing.Size(86, 47);
            this.label92.TabIndex = 1;
            this.label92.Text = "产品状态";
            this.label92.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtProductResult3
            // 
            this.tableLayoutPanel21.SetColumnSpan(this.txtProductResult3, 3);
            this.txtProductResult3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductResult3.Location = new System.Drawing.Point(92, 49);
            this.txtProductResult3.Margin = new System.Windows.Forms.Padding(2);
            this.txtProductResult3.Name = "txtProductResult3";
            this.txtProductResult3.Size = new System.Drawing.Size(334, 31);
            this.txtProductResult3.TabIndex = 2;
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label93.Location = new System.Drawing.Point(2, 94);
            this.label93.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label93.Name = "label93";
            this.label93.Padding = new System.Windows.Forms.Padding(2);
            this.label93.Size = new System.Drawing.Size(86, 47);
            this.label93.TabIndex = 1;
            this.label93.Text = "上传条码";
            this.label93.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBarcodeToUpload3
            // 
            this.txtBarcodeToUpload3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToUpload3.Location = new System.Drawing.Point(92, 96);
            this.txtBarcodeToUpload3.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToUpload3.Name = "txtBarcodeToUpload3";
            this.txtBarcodeToUpload3.Size = new System.Drawing.Size(120, 31);
            this.txtBarcodeToUpload3.TabIndex = 2;
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label94.Location = new System.Drawing.Point(216, 94);
            this.label94.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(86, 47);
            this.label94.TabIndex = 1;
            this.label94.Text = "条码长度";
            this.label94.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBarcodeToUploadLength3
            // 
            this.txtBarcodeToUploadLength3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToUploadLength3.Location = new System.Drawing.Point(306, 96);
            this.txtBarcodeToUploadLength3.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToUploadLength3.Name = "txtBarcodeToUploadLength3";
            this.txtBarcodeToUploadLength3.Size = new System.Drawing.Size(120, 31);
            this.txtBarcodeToUploadLength3.TabIndex = 2;
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.tableLayoutPanel20);
            this.groupBox25.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox25.Location = new System.Drawing.Point(445, 181);
            this.groupBox25.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox25.Size = new System.Drawing.Size(432, 169);
            this.groupBox25.TabIndex = 4;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "工序2过站";
            // 
            // tableLayoutPanel20
            // 
            this.tableLayoutPanel20.ColumnCount = 4;
            this.tableLayoutPanel20.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel20.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel20.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel20.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel20.Controls.Add(this.label85, 0, 0);
            this.tableLayoutPanel20.Controls.Add(this.txtTriggerUpload2, 1, 0);
            this.tableLayoutPanel20.Controls.Add(this.label86, 2, 0);
            this.tableLayoutPanel20.Controls.Add(this.txtFeedback2, 3, 0);
            this.tableLayoutPanel20.Controls.Add(this.label87, 0, 1);
            this.tableLayoutPanel20.Controls.Add(this.txtProductResult2, 1, 1);
            this.tableLayoutPanel20.Controls.Add(this.label88, 0, 2);
            this.tableLayoutPanel20.Controls.Add(this.txtBarcodeToUpload2, 1, 2);
            this.tableLayoutPanel20.Controls.Add(this.label89, 2, 2);
            this.tableLayoutPanel20.Controls.Add(this.txtBarcodeToUploadLength2, 3, 2);
            this.tableLayoutPanel20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel20.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel20.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel20.Name = "tableLayoutPanel20";
            this.tableLayoutPanel20.RowCount = 3;
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel20.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel20.Size = new System.Drawing.Size(428, 141);
            this.tableLayoutPanel20.TabIndex = 0;
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label85.Location = new System.Drawing.Point(2, 0);
            this.label85.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label85.Name = "label85";
            this.label85.Padding = new System.Windows.Forms.Padding(2);
            this.label85.Size = new System.Drawing.Size(86, 47);
            this.label85.TabIndex = 0;
            this.label85.Text = "触发上传";
            this.label85.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTriggerUpload2
            // 
            this.txtTriggerUpload2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTriggerUpload2.Location = new System.Drawing.Point(92, 2);
            this.txtTriggerUpload2.Margin = new System.Windows.Forms.Padding(2);
            this.txtTriggerUpload2.Name = "txtTriggerUpload2";
            this.txtTriggerUpload2.Size = new System.Drawing.Size(120, 31);
            this.txtTriggerUpload2.TabIndex = 2;
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label86.Location = new System.Drawing.Point(216, 0);
            this.label86.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label86.Name = "label86";
            this.label86.Padding = new System.Windows.Forms.Padding(2);
            this.label86.Size = new System.Drawing.Size(86, 47);
            this.label86.TabIndex = 1;
            this.label86.Text = "上传结果";
            this.label86.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtFeedback2
            // 
            this.txtFeedback2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFeedback2.Location = new System.Drawing.Point(306, 2);
            this.txtFeedback2.Margin = new System.Windows.Forms.Padding(2);
            this.txtFeedback2.Name = "txtFeedback2";
            this.txtFeedback2.Size = new System.Drawing.Size(120, 31);
            this.txtFeedback2.TabIndex = 2;
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label87.Location = new System.Drawing.Point(2, 47);
            this.label87.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label87.Name = "label87";
            this.label87.Padding = new System.Windows.Forms.Padding(2);
            this.label87.Size = new System.Drawing.Size(86, 47);
            this.label87.TabIndex = 1;
            this.label87.Text = "产品状态";
            this.label87.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtProductResult2
            // 
            this.tableLayoutPanel20.SetColumnSpan(this.txtProductResult2, 3);
            this.txtProductResult2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductResult2.Location = new System.Drawing.Point(92, 49);
            this.txtProductResult2.Margin = new System.Windows.Forms.Padding(2);
            this.txtProductResult2.Name = "txtProductResult2";
            this.txtProductResult2.Size = new System.Drawing.Size(334, 31);
            this.txtProductResult2.TabIndex = 2;
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label88.Location = new System.Drawing.Point(2, 94);
            this.label88.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label88.Name = "label88";
            this.label88.Padding = new System.Windows.Forms.Padding(2);
            this.label88.Size = new System.Drawing.Size(86, 47);
            this.label88.TabIndex = 1;
            this.label88.Text = "上传条码";
            this.label88.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBarcodeToUpload2
            // 
            this.txtBarcodeToUpload2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToUpload2.Location = new System.Drawing.Point(92, 96);
            this.txtBarcodeToUpload2.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToUpload2.Name = "txtBarcodeToUpload2";
            this.txtBarcodeToUpload2.Size = new System.Drawing.Size(120, 31);
            this.txtBarcodeToUpload2.TabIndex = 2;
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label89.Location = new System.Drawing.Point(216, 94);
            this.label89.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(86, 47);
            this.label89.TabIndex = 1;
            this.label89.Text = "条码长度";
            this.label89.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBarcodeToUploadLength2
            // 
            this.txtBarcodeToUploadLength2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToUploadLength2.Location = new System.Drawing.Point(306, 96);
            this.txtBarcodeToUploadLength2.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToUploadLength2.Name = "txtBarcodeToUploadLength2";
            this.txtBarcodeToUploadLength2.Size = new System.Drawing.Size(120, 31);
            this.txtBarcodeToUploadLength2.TabIndex = 2;
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.tableLayoutPanel19);
            this.groupBox24.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox24.Location = new System.Drawing.Point(445, 6);
            this.groupBox24.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox24.Size = new System.Drawing.Size(432, 169);
            this.groupBox24.TabIndex = 3;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "工序1过站";
            // 
            // tableLayoutPanel19
            // 
            this.tableLayoutPanel19.ColumnCount = 4;
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel19.Controls.Add(this.label79, 0, 0);
            this.tableLayoutPanel19.Controls.Add(this.txtTriggerUpload1, 1, 0);
            this.tableLayoutPanel19.Controls.Add(this.label81, 2, 0);
            this.tableLayoutPanel19.Controls.Add(this.txtFeedback1, 3, 0);
            this.tableLayoutPanel19.Controls.Add(this.label82, 0, 1);
            this.tableLayoutPanel19.Controls.Add(this.txtProductResult1, 1, 1);
            this.tableLayoutPanel19.Controls.Add(this.label83, 0, 2);
            this.tableLayoutPanel19.Controls.Add(this.txtBarcodeToUpload1, 1, 2);
            this.tableLayoutPanel19.Controls.Add(this.label84, 2, 2);
            this.tableLayoutPanel19.Controls.Add(this.txtBarcodeToUploadLength1, 3, 2);
            this.tableLayoutPanel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel19.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel19.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel19.Name = "tableLayoutPanel19";
            this.tableLayoutPanel19.RowCount = 3;
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel19.Size = new System.Drawing.Size(428, 141);
            this.tableLayoutPanel19.TabIndex = 0;
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label79.Location = new System.Drawing.Point(2, 0);
            this.label79.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label79.Name = "label79";
            this.label79.Padding = new System.Windows.Forms.Padding(2);
            this.label79.Size = new System.Drawing.Size(86, 47);
            this.label79.TabIndex = 0;
            this.label79.Text = "触发上传";
            this.label79.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTriggerUpload1
            // 
            this.txtTriggerUpload1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTriggerUpload1.Location = new System.Drawing.Point(92, 2);
            this.txtTriggerUpload1.Margin = new System.Windows.Forms.Padding(2);
            this.txtTriggerUpload1.Name = "txtTriggerUpload1";
            this.txtTriggerUpload1.Size = new System.Drawing.Size(120, 31);
            this.txtTriggerUpload1.TabIndex = 2;
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label81.Location = new System.Drawing.Point(216, 0);
            this.label81.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label81.Name = "label81";
            this.label81.Padding = new System.Windows.Forms.Padding(2);
            this.label81.Size = new System.Drawing.Size(86, 47);
            this.label81.TabIndex = 1;
            this.label81.Text = "上传结果";
            this.label81.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtFeedback1
            // 
            this.txtFeedback1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFeedback1.Location = new System.Drawing.Point(306, 2);
            this.txtFeedback1.Margin = new System.Windows.Forms.Padding(2);
            this.txtFeedback1.Name = "txtFeedback1";
            this.txtFeedback1.Size = new System.Drawing.Size(120, 31);
            this.txtFeedback1.TabIndex = 2;
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label82.Location = new System.Drawing.Point(2, 47);
            this.label82.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label82.Name = "label82";
            this.label82.Padding = new System.Windows.Forms.Padding(2);
            this.label82.Size = new System.Drawing.Size(86, 47);
            this.label82.TabIndex = 1;
            this.label82.Text = "产品状态";
            this.label82.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtProductResult1
            // 
            this.tableLayoutPanel19.SetColumnSpan(this.txtProductResult1, 3);
            this.txtProductResult1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductResult1.Location = new System.Drawing.Point(92, 49);
            this.txtProductResult1.Margin = new System.Windows.Forms.Padding(2);
            this.txtProductResult1.Name = "txtProductResult1";
            this.txtProductResult1.Size = new System.Drawing.Size(334, 31);
            this.txtProductResult1.TabIndex = 2;
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label83.Location = new System.Drawing.Point(2, 94);
            this.label83.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label83.Name = "label83";
            this.label83.Padding = new System.Windows.Forms.Padding(2);
            this.label83.Size = new System.Drawing.Size(86, 47);
            this.label83.TabIndex = 1;
            this.label83.Text = "上传条码";
            this.label83.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBarcodeToUpload1
            // 
            this.txtBarcodeToUpload1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToUpload1.Location = new System.Drawing.Point(92, 96);
            this.txtBarcodeToUpload1.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToUpload1.Name = "txtBarcodeToUpload1";
            this.txtBarcodeToUpload1.Size = new System.Drawing.Size(120, 31);
            this.txtBarcodeToUpload1.TabIndex = 2;
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label84.Location = new System.Drawing.Point(216, 94);
            this.label84.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(86, 47);
            this.label84.TabIndex = 1;
            this.label84.Text = "条码长度";
            this.label84.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBarcodeToUploadLength1
            // 
            this.txtBarcodeToUploadLength1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeToUploadLength1.Location = new System.Drawing.Point(306, 96);
            this.txtBarcodeToUploadLength1.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeToUploadLength1.Name = "txtBarcodeToUploadLength1";
            this.txtBarcodeToUploadLength1.Size = new System.Drawing.Size(120, 31);
            this.txtBarcodeToUploadLength1.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.Font = new System.Drawing.Font("Microsoft YaHei", 20F, System.Drawing.FontStyle.Bold);
            this.button2.Location = new System.Drawing.Point(894, 362);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(432, 166);
            this.button2.TabIndex = 2;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SaveAtAddressMatain_Click);
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.tableLayoutPanel18);
            this.groupBox14.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.groupBox14.Location = new System.Drawing.Point(6, 6);
            this.groupBox14.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox14.Size = new System.Drawing.Size(432, 341);
            this.groupBox14.TabIndex = 1;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "条码验证";
            // 
            // tableLayoutPanel18
            // 
            this.tableLayoutPanel18.ColumnCount = 4;
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel18.Controls.Add(this.label68, 0, 0);
            this.tableLayoutPanel18.Controls.Add(this.label69, 0, 1);
            this.tableLayoutPanel18.Controls.Add(this.label70, 0, 2);
            this.tableLayoutPanel18.Controls.Add(this.label72, 0, 3);
            this.tableLayoutPanel18.Controls.Add(this.txtHasBarcodeTag, 1, 0);
            this.tableLayoutPanel18.Controls.Add(this.txtBarcodeVerifyTag, 1, 1);
            this.tableLayoutPanel18.Controls.Add(this.txtBarcodeType, 1, 2);
            this.tableLayoutPanel18.Controls.Add(this.txtPlcScanned, 1, 3);
            this.tableLayoutPanel18.Controls.Add(this.label78, 2, 3);
            this.tableLayoutPanel18.Controls.Add(this.txtScannedLength, 3, 3);
            this.tableLayoutPanel18.Controls.Add(this.label73, 0, 4);
            this.tableLayoutPanel18.Controls.Add(this.txtPanalizationBarcode, 1, 4);
            this.tableLayoutPanel18.Controls.Add(this.label77, 2, 4);
            this.tableLayoutPanel18.Controls.Add(this.txtPanalizationLength, 3, 4);
            this.tableLayoutPanel18.Controls.Add(this.label74, 0, 5);
            this.tableLayoutPanel18.Controls.Add(this.txtManualInput, 1, 5);
            this.tableLayoutPanel18.Controls.Add(this.label75, 0, 6);
            this.tableLayoutPanel18.Controls.Add(this.txtManualBarcode, 1, 6);
            this.tableLayoutPanel18.Controls.Add(this.label76, 2, 6);
            this.tableLayoutPanel18.Controls.Add(this.txtManualLength, 3, 6);
            this.tableLayoutPanel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel18.Location = new System.Drawing.Point(2, 26);
            this.tableLayoutPanel18.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel18.Name = "tableLayoutPanel18";
            this.tableLayoutPanel18.RowCount = 7;
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel18.Size = new System.Drawing.Size(428, 313);
            this.tableLayoutPanel18.TabIndex = 0;
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label68.Location = new System.Drawing.Point(2, 0);
            this.label68.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label68.Name = "label68";
            this.label68.Padding = new System.Windows.Forms.Padding(2);
            this.label68.Size = new System.Drawing.Size(86, 44);
            this.label68.TabIndex = 0;
            this.label68.Text = "触发验证";
            this.label68.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label69.Location = new System.Drawing.Point(2, 44);
            this.label69.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label69.Name = "label69";
            this.label69.Padding = new System.Windows.Forms.Padding(2);
            this.label69.Size = new System.Drawing.Size(86, 44);
            this.label69.TabIndex = 1;
            this.label69.Text = "验证反馈";
            this.label69.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label70.Location = new System.Drawing.Point(2, 88);
            this.label70.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label70.Name = "label70";
            this.label70.Padding = new System.Windows.Forms.Padding(2);
            this.label70.Size = new System.Drawing.Size(86, 44);
            this.label70.TabIndex = 1;
            this.label70.Text = "条码类型";
            this.label70.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label72.Location = new System.Drawing.Point(2, 132);
            this.label72.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label72.Name = "label72";
            this.label72.Padding = new System.Windows.Forms.Padding(2);
            this.label72.Size = new System.Drawing.Size(86, 44);
            this.label72.TabIndex = 1;
            this.label72.Text = "产品条码";
            this.label72.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtHasBarcodeTag
            // 
            this.tableLayoutPanel18.SetColumnSpan(this.txtHasBarcodeTag, 3);
            this.txtHasBarcodeTag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHasBarcodeTag.Location = new System.Drawing.Point(92, 2);
            this.txtHasBarcodeTag.Margin = new System.Windows.Forms.Padding(2);
            this.txtHasBarcodeTag.Name = "txtHasBarcodeTag";
            this.txtHasBarcodeTag.Size = new System.Drawing.Size(334, 31);
            this.txtHasBarcodeTag.TabIndex = 2;
            // 
            // txtBarcodeVerifyTag
            // 
            this.tableLayoutPanel18.SetColumnSpan(this.txtBarcodeVerifyTag, 3);
            this.txtBarcodeVerifyTag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeVerifyTag.Location = new System.Drawing.Point(92, 46);
            this.txtBarcodeVerifyTag.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeVerifyTag.Name = "txtBarcodeVerifyTag";
            this.txtBarcodeVerifyTag.Size = new System.Drawing.Size(334, 31);
            this.txtBarcodeVerifyTag.TabIndex = 2;
            // 
            // txtBarcodeType
            // 
            this.tableLayoutPanel18.SetColumnSpan(this.txtBarcodeType, 3);
            this.txtBarcodeType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcodeType.Location = new System.Drawing.Point(92, 90);
            this.txtBarcodeType.Margin = new System.Windows.Forms.Padding(2);
            this.txtBarcodeType.Name = "txtBarcodeType";
            this.txtBarcodeType.Size = new System.Drawing.Size(334, 31);
            this.txtBarcodeType.TabIndex = 2;
            // 
            // txtPlcScanned
            // 
            this.txtPlcScanned.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPlcScanned.Location = new System.Drawing.Point(92, 134);
            this.txtPlcScanned.Margin = new System.Windows.Forms.Padding(2);
            this.txtPlcScanned.Name = "txtPlcScanned";
            this.txtPlcScanned.Size = new System.Drawing.Size(122, 31);
            this.txtPlcScanned.TabIndex = 2;
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label78.Location = new System.Drawing.Point(218, 132);
            this.label78.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(82, 44);
            this.label78.TabIndex = 1;
            this.label78.Text = "条码长度";
            this.label78.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtScannedLength
            // 
            this.txtScannedLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtScannedLength.Location = new System.Drawing.Point(304, 134);
            this.txtScannedLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtScannedLength.Name = "txtScannedLength";
            this.txtScannedLength.Size = new System.Drawing.Size(122, 31);
            this.txtScannedLength.TabIndex = 2;
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label73.Location = new System.Drawing.Point(2, 176);
            this.label73.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label73.Name = "label73";
            this.label73.Padding = new System.Windows.Forms.Padding(2);
            this.label73.Size = new System.Drawing.Size(86, 44);
            this.label73.TabIndex = 1;
            this.label73.Text = "拼版条码";
            this.label73.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtPanalizationBarcode
            // 
            this.txtPanalizationBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPanalizationBarcode.Location = new System.Drawing.Point(92, 178);
            this.txtPanalizationBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.txtPanalizationBarcode.Name = "txtPanalizationBarcode";
            this.txtPanalizationBarcode.Size = new System.Drawing.Size(122, 31);
            this.txtPanalizationBarcode.TabIndex = 2;
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label77.Location = new System.Drawing.Point(218, 176);
            this.label77.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(82, 44);
            this.label77.TabIndex = 1;
            this.label77.Text = "条码长度";
            this.label77.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtPanalizationLength
            // 
            this.txtPanalizationLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPanalizationLength.Location = new System.Drawing.Point(304, 178);
            this.txtPanalizationLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtPanalizationLength.Name = "txtPanalizationLength";
            this.txtPanalizationLength.Size = new System.Drawing.Size(122, 31);
            this.txtPanalizationLength.TabIndex = 2;
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label74.Location = new System.Drawing.Point(2, 220);
            this.label74.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label74.Name = "label74";
            this.label74.Padding = new System.Windows.Forms.Padding(2);
            this.label74.Size = new System.Drawing.Size(86, 44);
            this.label74.TabIndex = 1;
            this.label74.Text = "触发手动";
            this.label74.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtManualInput
            // 
            this.tableLayoutPanel18.SetColumnSpan(this.txtManualInput, 3);
            this.txtManualInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtManualInput.Location = new System.Drawing.Point(92, 222);
            this.txtManualInput.Margin = new System.Windows.Forms.Padding(2);
            this.txtManualInput.Name = "txtManualInput";
            this.txtManualInput.Size = new System.Drawing.Size(334, 31);
            this.txtManualInput.TabIndex = 2;
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label75.Location = new System.Drawing.Point(2, 264);
            this.label75.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label75.Name = "label75";
            this.label75.Padding = new System.Windows.Forms.Padding(2);
            this.label75.Size = new System.Drawing.Size(86, 49);
            this.label75.TabIndex = 1;
            this.label75.Text = "手动条码";
            this.label75.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtManualBarcode
            // 
            this.txtManualBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtManualBarcode.Location = new System.Drawing.Point(92, 266);
            this.txtManualBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.txtManualBarcode.Name = "txtManualBarcode";
            this.txtManualBarcode.Size = new System.Drawing.Size(122, 31);
            this.txtManualBarcode.TabIndex = 2;
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label76.Location = new System.Drawing.Point(218, 264);
            this.label76.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(82, 49);
            this.label76.TabIndex = 1;
            this.label76.Text = "条码长度";
            this.label76.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtManualLength
            // 
            this.txtManualLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtManualLength.Location = new System.Drawing.Point(304, 266);
            this.txtManualLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtManualLength.Name = "txtManualLength";
            this.txtManualLength.Size = new System.Drawing.Size(122, 31);
            this.txtManualLength.TabIndex = 2;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.接口设置panel);
            this.tabPage5.Location = new System.Drawing.Point(4, 4);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1894, 984);
            this.tabPage5.TabIndex = 10;
            this.tabPage5.Text = "接口配置";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // 接口设置panel
            // 
            this.接口设置panel.Controls.Add(this.panel8);
            this.接口设置panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.接口设置panel.Location = new System.Drawing.Point(0, 0);
            this.接口设置panel.Margin = new System.Windows.Forms.Padding(2);
            this.接口设置panel.Name = "接口设置panel";
            this.接口设置panel.Size = new System.Drawing.Size(1894, 984);
            this.接口设置panel.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.tableLayoutPanel5);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(2);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(1894, 984);
            this.panel8.TabIndex = 3;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 4;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.Url_ToolingChange, 3, 11);
            this.tableLayoutPanel5.Controls.Add(this.Url_RealtimeArgs, 3, 10);
            this.tableLayoutPanel5.Controls.Add(this.Url_KeyArgs, 3, 9);
            this.tableLayoutPanel5.Controls.Add(this.Url_ErrorInterface, 3, 8);
            this.tableLayoutPanel5.Controls.Add(this.Url_DeviceStatus, 3, 7);
            this.tableLayoutPanel5.Controls.Add(this.Url_Heartbeat, 3, 6);
            this.tableLayoutPanel5.Controls.Add(this.Url_GetProductName, 3, 5);
            this.tableLayoutPanel5.Controls.Add(this.Url_FTPMessGet, 3, 4);
            this.tableLayoutPanel5.Controls.Add(this.Url_DataUpload, 3, 3);
            this.tableLayoutPanel5.Controls.Add(this.Url_RouteCheck, 3, 2);
            this.tableLayoutPanel5.Controls.Add(this.UrlPanelization, 3, 1);
            this.tableLayoutPanel5.Controls.Add(this.label27, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.url, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label28, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.Line, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.label39, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.label41, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.label43, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.label45, 0, 5);
            this.tableLayoutPanel5.Controls.Add(this.label46, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.label47, 0, 7);
            this.tableLayoutPanel5.Controls.Add(this.label48, 0, 8);
            this.tableLayoutPanel5.Controls.Add(this.label49, 0, 9);
            this.tableLayoutPanel5.Controls.Add(this.label50, 0, 10);
            this.tableLayoutPanel5.Controls.Add(this.label51, 0, 11);
            this.tableLayoutPanel5.Controls.Add(this.Process, 1, 2);
            this.tableLayoutPanel5.Controls.Add(this.Station, 1, 3);
            this.tableLayoutPanel5.Controls.Add(this.MesKey, 1, 4);
            this.tableLayoutPanel5.Controls.Add(this.Security, 1, 5);
            this.tableLayoutPanel5.Controls.Add(this.Device, 1, 6);
            this.tableLayoutPanel5.Controls.Add(this.PlanNo, 1, 7);
            this.tableLayoutPanel5.Controls.Add(this.FTPlog, 1, 8);
            this.tableLayoutPanel5.Controls.Add(this.FTPPIC, 1, 9);
            this.tableLayoutPanel5.Controls.Add(this.FTPID, 1, 10);
            this.tableLayoutPanel5.Controls.Add(this.FTPCODE, 1, 11);
            this.tableLayoutPanel5.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.label4, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.label6, 2, 2);
            this.tableLayoutPanel5.Controls.Add(this.label12, 2, 3);
            this.tableLayoutPanel5.Controls.Add(this.label13, 2, 4);
            this.tableLayoutPanel5.Controls.Add(this.label15, 2, 5);
            this.tableLayoutPanel5.Controls.Add(this.label25, 2, 6);
            this.tableLayoutPanel5.Controls.Add(this.label29, 2, 7);
            this.tableLayoutPanel5.Controls.Add(this.label31, 2, 8);
            this.tableLayoutPanel5.Controls.Add(this.label33, 2, 9);
            this.tableLayoutPanel5.Controls.Add(this.label34, 2, 10);
            this.tableLayoutPanel5.Controls.Add(this.label35, 2, 11);
            this.tableLayoutPanel5.Controls.Add(this.Url_Token, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnSave_InterfaceConfig, 1, 14);
            this.tableLayoutPanel5.Controls.Add(this.label36, 0, 12);
            this.tableLayoutPanel5.Controls.Add(this.label37, 0, 13);
            this.tableLayoutPanel5.Controls.Add(this.SWVer, 1, 12);
            this.tableLayoutPanel5.Controls.Add(this.HWVer, 1, 13);
            this.tableLayoutPanel5.Controls.Add(this.label23, 2, 12);
            this.tableLayoutPanel5.Controls.Add(this.Url_PrintTemplate, 3, 12);
            this.tableLayoutPanel5.Controls.Add(this.label40, 2, 13);
            this.tableLayoutPanel5.Controls.Add(this.LocalFilePath, 3, 13);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 15;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1894, 984);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // Url_ToolingChange
            // 
            this.Url_ToolingChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_ToolingChange.Location = new System.Drawing.Point(1161, 485);
            this.Url_ToolingChange.Margin = new System.Windows.Forms.Padding(2);
            this.Url_ToolingChange.Multiline = true;
            this.Url_ToolingChange.Name = "Url_ToolingChange";
            this.Url_ToolingChange.Size = new System.Drawing.Size(731, 39);
            this.Url_ToolingChange.TabIndex = 48;
            // 
            // Url_RealtimeArgs
            // 
            this.Url_RealtimeArgs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_RealtimeArgs.Location = new System.Drawing.Point(1161, 442);
            this.Url_RealtimeArgs.Margin = new System.Windows.Forms.Padding(2);
            this.Url_RealtimeArgs.Multiline = true;
            this.Url_RealtimeArgs.Name = "Url_RealtimeArgs";
            this.Url_RealtimeArgs.Size = new System.Drawing.Size(731, 39);
            this.Url_RealtimeArgs.TabIndex = 47;
            // 
            // Url_KeyArgs
            // 
            this.Url_KeyArgs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_KeyArgs.Location = new System.Drawing.Point(1161, 399);
            this.Url_KeyArgs.Margin = new System.Windows.Forms.Padding(2);
            this.Url_KeyArgs.Multiline = true;
            this.Url_KeyArgs.Name = "Url_KeyArgs";
            this.Url_KeyArgs.Size = new System.Drawing.Size(731, 39);
            this.Url_KeyArgs.TabIndex = 46;
            // 
            // Url_ErrorInterface
            // 
            this.Url_ErrorInterface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_ErrorInterface.Location = new System.Drawing.Point(1161, 356);
            this.Url_ErrorInterface.Margin = new System.Windows.Forms.Padding(2);
            this.Url_ErrorInterface.Multiline = true;
            this.Url_ErrorInterface.Name = "Url_ErrorInterface";
            this.Url_ErrorInterface.Size = new System.Drawing.Size(731, 39);
            this.Url_ErrorInterface.TabIndex = 45;
            // 
            // Url_DeviceStatus
            // 
            this.Url_DeviceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_DeviceStatus.Location = new System.Drawing.Point(1161, 313);
            this.Url_DeviceStatus.Margin = new System.Windows.Forms.Padding(2);
            this.Url_DeviceStatus.Multiline = true;
            this.Url_DeviceStatus.Name = "Url_DeviceStatus";
            this.Url_DeviceStatus.Size = new System.Drawing.Size(731, 39);
            this.Url_DeviceStatus.TabIndex = 44;
            // 
            // Url_Heartbeat
            // 
            this.Url_Heartbeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_Heartbeat.Location = new System.Drawing.Point(1161, 270);
            this.Url_Heartbeat.Margin = new System.Windows.Forms.Padding(2);
            this.Url_Heartbeat.Multiline = true;
            this.Url_Heartbeat.Name = "Url_Heartbeat";
            this.Url_Heartbeat.Size = new System.Drawing.Size(731, 39);
            this.Url_Heartbeat.TabIndex = 43;
            // 
            // Url_GetProductName
            // 
            this.Url_GetProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_GetProductName.Location = new System.Drawing.Point(1161, 227);
            this.Url_GetProductName.Margin = new System.Windows.Forms.Padding(2);
            this.Url_GetProductName.Multiline = true;
            this.Url_GetProductName.Name = "Url_GetProductName";
            this.Url_GetProductName.Size = new System.Drawing.Size(731, 39);
            this.Url_GetProductName.TabIndex = 42;
            // 
            // Url_FTPMessGet
            // 
            this.Url_FTPMessGet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_FTPMessGet.Location = new System.Drawing.Point(1161, 185);
            this.Url_FTPMessGet.Margin = new System.Windows.Forms.Padding(2);
            this.Url_FTPMessGet.Multiline = true;
            this.Url_FTPMessGet.Name = "Url_FTPMessGet";
            this.Url_FTPMessGet.Size = new System.Drawing.Size(731, 38);
            this.Url_FTPMessGet.TabIndex = 41;
            // 
            // Url_DataUpload
            // 
            this.Url_DataUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_DataUpload.Location = new System.Drawing.Point(1161, 141);
            this.Url_DataUpload.Margin = new System.Windows.Forms.Padding(2);
            this.Url_DataUpload.Multiline = true;
            this.Url_DataUpload.Name = "Url_DataUpload";
            this.Url_DataUpload.Size = new System.Drawing.Size(731, 40);
            this.Url_DataUpload.TabIndex = 40;
            // 
            // Url_RouteCheck
            // 
            this.Url_RouteCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_RouteCheck.Location = new System.Drawing.Point(1161, 98);
            this.Url_RouteCheck.Margin = new System.Windows.Forms.Padding(2);
            this.Url_RouteCheck.Multiline = true;
            this.Url_RouteCheck.Name = "Url_RouteCheck";
            this.Url_RouteCheck.Size = new System.Drawing.Size(731, 39);
            this.Url_RouteCheck.TabIndex = 39;
            // 
            // UrlPanelization
            // 
            this.UrlPanelization.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UrlPanelization.Location = new System.Drawing.Point(1161, 55);
            this.UrlPanelization.Margin = new System.Windows.Forms.Padding(2);
            this.UrlPanelization.Multiline = true;
            this.UrlPanelization.Name = "UrlPanelization";
            this.UrlPanelization.Size = new System.Drawing.Size(731, 39);
            this.UrlPanelization.TabIndex = 38;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label27.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label27.Location = new System.Drawing.Point(2, 0);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(185, 53);
            this.label27.TabIndex = 0;
            this.label27.Text = "Url地址：";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // url
            // 
            this.url.Dock = System.Windows.Forms.DockStyle.Fill;
            this.url.Location = new System.Drawing.Point(191, 2);
            this.url.Margin = new System.Windows.Forms.Padding(2);
            this.url.Multiline = true;
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(730, 49);
            this.url.TabIndex = 1;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label28.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label28.Location = new System.Drawing.Point(2, 53);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(185, 43);
            this.label28.TabIndex = 2;
            this.label28.Text = "线体：";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Line
            // 
            this.Line.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Line.Location = new System.Drawing.Point(191, 55);
            this.Line.Margin = new System.Windows.Forms.Padding(2);
            this.Line.Multiline = true;
            this.Line.Name = "Line";
            this.Line.Size = new System.Drawing.Size(730, 39);
            this.Line.TabIndex = 3;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label39.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label39.Location = new System.Drawing.Point(2, 96);
            this.label39.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(185, 43);
            this.label39.TabIndex = 4;
            this.label39.Text = "工序：";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label41.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label41.Location = new System.Drawing.Point(2, 139);
            this.label41.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(185, 44);
            this.label41.TabIndex = 5;
            this.label41.Text = "站点：";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label43.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label43.Location = new System.Drawing.Point(2, 183);
            this.label43.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(185, 42);
            this.label43.TabIndex = 6;
            this.label43.Text = "MES账号：";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label45.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label45.Location = new System.Drawing.Point(2, 225);
            this.label45.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(185, 43);
            this.label45.TabIndex = 7;
            this.label45.Text = "MD加密：";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label46.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label46.Location = new System.Drawing.Point(2, 268);
            this.label46.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(185, 43);
            this.label46.TabIndex = 8;
            this.label46.Text = "设备名：";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label47.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label47.Location = new System.Drawing.Point(2, 311);
            this.label47.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(185, 43);
            this.label47.TabIndex = 9;
            this.label47.Text = "工单：";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label48.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label48.Location = new System.Drawing.Point(2, 354);
            this.label48.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(185, 43);
            this.label48.TabIndex = 10;
            this.label48.Text = "文件FTP上传URL：";
            this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label49.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label49.Location = new System.Drawing.Point(2, 397);
            this.label49.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(185, 43);
            this.label49.TabIndex = 11;
            this.label49.Text = "图片FTP上传URL：";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label50.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label50.Location = new System.Drawing.Point(2, 440);
            this.label50.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(185, 43);
            this.label50.TabIndex = 12;
            this.label50.Text = "FTP账号：";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label51.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label51.Location = new System.Drawing.Point(2, 483);
            this.label51.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(185, 43);
            this.label51.TabIndex = 13;
            this.label51.Text = "FTP密码：";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Process
            // 
            this.Process.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Process.Location = new System.Drawing.Point(191, 98);
            this.Process.Margin = new System.Windows.Forms.Padding(2);
            this.Process.Multiline = true;
            this.Process.Name = "Process";
            this.Process.Size = new System.Drawing.Size(730, 39);
            this.Process.TabIndex = 14;
            // 
            // Station
            // 
            this.Station.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Station.Location = new System.Drawing.Point(191, 141);
            this.Station.Margin = new System.Windows.Forms.Padding(2);
            this.Station.Multiline = true;
            this.Station.Name = "Station";
            this.Station.Size = new System.Drawing.Size(730, 40);
            this.Station.TabIndex = 15;
            // 
            // MesKey
            // 
            this.MesKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MesKey.Location = new System.Drawing.Point(191, 185);
            this.MesKey.Margin = new System.Windows.Forms.Padding(2);
            this.MesKey.Multiline = true;
            this.MesKey.Name = "MesKey";
            this.MesKey.Size = new System.Drawing.Size(730, 38);
            this.MesKey.TabIndex = 16;
            // 
            // Security
            // 
            this.Security.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Security.Location = new System.Drawing.Point(191, 227);
            this.Security.Margin = new System.Windows.Forms.Padding(2);
            this.Security.Multiline = true;
            this.Security.Name = "Security";
            this.Security.Size = new System.Drawing.Size(730, 39);
            this.Security.TabIndex = 17;
            // 
            // Device
            // 
            this.Device.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Device.Location = new System.Drawing.Point(191, 270);
            this.Device.Margin = new System.Windows.Forms.Padding(2);
            this.Device.Multiline = true;
            this.Device.Name = "Device";
            this.Device.Size = new System.Drawing.Size(730, 39);
            this.Device.TabIndex = 18;
            // 
            // PlanNo
            // 
            this.PlanNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlanNo.Location = new System.Drawing.Point(191, 313);
            this.PlanNo.Margin = new System.Windows.Forms.Padding(2);
            this.PlanNo.Multiline = true;
            this.PlanNo.Name = "PlanNo";
            this.PlanNo.Size = new System.Drawing.Size(730, 39);
            this.PlanNo.TabIndex = 19;
            // 
            // FTPlog
            // 
            this.FTPlog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FTPlog.Location = new System.Drawing.Point(191, 356);
            this.FTPlog.Margin = new System.Windows.Forms.Padding(2);
            this.FTPlog.Multiline = true;
            this.FTPlog.Name = "FTPlog";
            this.FTPlog.Size = new System.Drawing.Size(730, 39);
            this.FTPlog.TabIndex = 20;
            // 
            // FTPPIC
            // 
            this.FTPPIC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FTPPIC.Location = new System.Drawing.Point(191, 399);
            this.FTPPIC.Margin = new System.Windows.Forms.Padding(2);
            this.FTPPIC.Multiline = true;
            this.FTPPIC.Name = "FTPPIC";
            this.FTPPIC.Size = new System.Drawing.Size(730, 39);
            this.FTPPIC.TabIndex = 21;
            // 
            // FTPID
            // 
            this.FTPID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FTPID.Location = new System.Drawing.Point(191, 442);
            this.FTPID.Margin = new System.Windows.Forms.Padding(2);
            this.FTPID.Multiline = true;
            this.FTPID.Name = "FTPID";
            this.FTPID.Size = new System.Drawing.Size(730, 39);
            this.FTPID.TabIndex = 22;
            // 
            // FTPCODE
            // 
            this.FTPCODE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FTPCODE.Location = new System.Drawing.Point(191, 485);
            this.FTPCODE.Margin = new System.Windows.Forms.Padding(2);
            this.FTPCODE.Multiline = true;
            this.FTPCODE.Name = "FTPCODE";
            this.FTPCODE.Size = new System.Drawing.Size(730, 39);
            this.FTPCODE.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(925, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(232, 53);
            this.label3.TabIndex = 25;
            this.label3.Text = "获取 Token地址：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(925, 53);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(232, 43);
            this.label4.TabIndex = 26;
            this.label4.Text = "获取拼板条码地址：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(925, 96);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(232, 43);
            this.label6.TabIndex = 27;
            this.label6.Text = "流程检查地址：";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(925, 139);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(232, 44);
            this.label12.TabIndex = 28;
            this.label12.Text = "数据上传地址：";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(925, 183);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(232, 42);
            this.label13.TabIndex = 29;
            this.label13.Text = "FTP 信息获取地址：";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(925, 225);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(232, 43);
            this.label15.TabIndex = 30;
            this.label15.Text = "获取产品名称地址：";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label25.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label25.Location = new System.Drawing.Point(925, 268);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(232, 43);
            this.label25.TabIndex = 31;
            this.label25.Text = "设备心跳地址：";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label29.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label29.Location = new System.Drawing.Point(925, 311);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(232, 43);
            this.label29.TabIndex = 32;
            this.label29.Text = "设备状态地址：";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label31.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label31.Location = new System.Drawing.Point(925, 354);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(232, 43);
            this.label31.TabIndex = 33;
            this.label31.Text = "故障/预警接口地址：";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label33.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label33.Location = new System.Drawing.Point(925, 397);
            this.label33.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(232, 43);
            this.label33.TabIndex = 34;
            this.label33.Text = "程序关键参数地址：";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label34.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label34.Location = new System.Drawing.Point(925, 440);
            this.label34.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(232, 43);
            this.label34.TabIndex = 35;
            this.label34.Text = "程序实时参数地址：";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label35.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label35.Location = new System.Drawing.Point(925, 483);
            this.label35.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(232, 43);
            this.label35.TabIndex = 36;
            this.label35.Text = "工装更换地址：";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Url_Token
            // 
            this.Url_Token.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_Token.Location = new System.Drawing.Point(1161, 2);
            this.Url_Token.Margin = new System.Windows.Forms.Padding(2);
            this.Url_Token.Multiline = true;
            this.Url_Token.Name = "Url_Token";
            this.Url_Token.Size = new System.Drawing.Size(731, 49);
            this.Url_Token.TabIndex = 37;
            // 
            // btnSave_InterfaceConfig
            // 
            this.btnSave_InterfaceConfig.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave_InterfaceConfig.Location = new System.Drawing.Point(191, 624);
            this.btnSave_InterfaceConfig.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave_InterfaceConfig.Name = "btnSave_InterfaceConfig";
            this.btnSave_InterfaceConfig.Size = new System.Drawing.Size(158, 74);
            this.btnSave_InterfaceConfig.TabIndex = 24;
            this.btnSave_InterfaceConfig.Text = "保存";
            this.btnSave_InterfaceConfig.UseVisualStyleBackColor = true;
            this.btnSave_InterfaceConfig.Click += new System.EventHandler(this.SaveAtMesConfig_Click);
            // 
            // label36
            // 
            this.label36.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label36.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label36.Location = new System.Drawing.Point(2, 526);
            this.label36.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(185, 48);
            this.label36.TabIndex = 49;
            this.label36.Text = "软件版本：";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label37
            // 
            this.label37.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label37.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label37.Location = new System.Drawing.Point(2, 574);
            this.label37.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(185, 48);
            this.label37.TabIndex = 50;
            this.label37.Text = "硬件版本：";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SWVer
            // 
            this.SWVer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SWVer.Location = new System.Drawing.Point(191, 528);
            this.SWVer.Margin = new System.Windows.Forms.Padding(2);
            this.SWVer.Multiline = true;
            this.SWVer.Name = "SWVer";
            this.SWVer.Size = new System.Drawing.Size(730, 44);
            this.SWVer.TabIndex = 51;
            // 
            // HWVer
            // 
            this.HWVer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWVer.Location = new System.Drawing.Point(191, 576);
            this.HWVer.Margin = new System.Windows.Forms.Padding(2);
            this.HWVer.Multiline = true;
            this.HWVer.Name = "HWVer";
            this.HWVer.Size = new System.Drawing.Size(730, 44);
            this.HWVer.TabIndex = 52;
            // 
            // label23
            // 
            this.label23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label23.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label23.Location = new System.Drawing.Point(925, 526);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(232, 48);
            this.label23.TabIndex = 53;
            this.label23.Text = "打印模板地址：";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Url_PrintTemplate
            // 
            this.Url_PrintTemplate.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Url_PrintTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Url_PrintTemplate.Location = new System.Drawing.Point(1161, 528);
            this.Url_PrintTemplate.Margin = new System.Windows.Forms.Padding(2);
            this.Url_PrintTemplate.Multiline = true;
            this.Url_PrintTemplate.Name = "Url_PrintTemplate";
            this.Url_PrintTemplate.Size = new System.Drawing.Size(731, 44);
            this.Url_PrintTemplate.TabIndex = 54;
            // 
            // label40
            // 
            this.label40.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label40.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label40.Location = new System.Drawing.Point(925, 574);
            this.label40.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(232, 48);
            this.label40.TabIndex = 55;
            this.label40.Text = "本地文件保存目录：";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LocalFilePath
            // 
            this.LocalFilePath.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.LocalFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocalFilePath.Location = new System.Drawing.Point(1161, 576);
            this.LocalFilePath.Margin = new System.Windows.Forms.Padding(2);
            this.LocalFilePath.Multiline = true;
            this.LocalFilePath.Name = "LocalFilePath";
            this.LocalFilePath.Size = new System.Drawing.Size(731, 44);
            this.LocalFilePath.TabIndex = 56;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel1);
            this.tabPage4.Controls.Add(this.panel6);
            this.tabPage4.Location = new System.Drawing.Point(4, 4);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1894, 984);
            this.tabPage4.TabIndex = 6;
            this.tabPage4.Text = "用户管理";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvUserInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 179);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1894, 805);
            this.panel1.TabIndex = 1;
            // 
            // dgvUserInfo
            // 
            this.dgvUserInfo.AllowUserToAddRows = false;
            this.dgvUserInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUserInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUserInfo.Location = new System.Drawing.Point(0, 0);
            this.dgvUserInfo.Margin = new System.Windows.Forms.Padding(2);
            this.dgvUserInfo.Name = "dgvUserInfo";
            this.dgvUserInfo.ReadOnly = true;
            this.dgvUserInfo.RowHeadersWidth = 51;
            this.dgvUserInfo.RowTemplate.Height = 27;
            this.dgvUserInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUserInfo.Size = new System.Drawing.Size(1894, 805);
            this.dgvUserInfo.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.groupBox6);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(2);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1894, 179);
            this.panel6.TabIndex = 2;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.UPwd);
            this.groupBox6.Controls.Add(this.UId);
            this.groupBox6.Controls.Add(this.Priv);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Controls.Add(this.UserRefresh);
            this.groupBox6.Controls.Add(this.label20);
            this.groupBox6.Controls.Add(this.label18);
            this.groupBox6.Controls.Add(this.UserAdd);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(0, 0);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox6.Size = new System.Drawing.Size(1894, 179);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "用户信息";
            // 
            // UPwd
            // 
            this.UPwd.Location = new System.Drawing.Point(94, 112);
            this.UPwd.Margin = new System.Windows.Forms.Padding(4);
            this.UPwd.Name = "UPwd";
            this.UPwd.Size = new System.Drawing.Size(279, 30);
            this.UPwd.TabIndex = 6;
            // 
            // UId
            // 
            this.UId.Location = new System.Drawing.Point(94, 49);
            this.UId.Margin = new System.Windows.Forms.Padding(4);
            this.UId.Name = "UId";
            this.UId.Size = new System.Drawing.Size(279, 30);
            this.UId.TabIndex = 5;
            // 
            // Priv
            // 
            this.Priv.FormattingEnabled = true;
            this.Priv.Items.AddRange(new object[] {
            "管理员",
            "技术员",
            "作业员"});
            this.Priv.Location = new System.Drawing.Point(479, 49);
            this.Priv.Margin = new System.Windows.Forms.Padding(4);
            this.Priv.Name = "Priv";
            this.Priv.Size = new System.Drawing.Size(150, 31);
            this.Priv.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(409, 52);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 24);
            this.label14.TabIndex = 3;
            this.label14.Text = "权限";
            // 
            // UserRefresh
            // 
            this.UserRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UserRefresh.Location = new System.Drawing.Point(414, 100);
            this.UserRefresh.Margin = new System.Windows.Forms.Padding(2);
            this.UserRefresh.Name = "UserRefresh";
            this.UserRefresh.Size = new System.Drawing.Size(98, 44);
            this.UserRefresh.TabIndex = 0;
            this.UserRefresh.Text = "刷新";
            this.UserRefresh.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(39, 112);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(46, 24);
            this.label20.TabIndex = 1;
            this.label20.Text = "密码";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(39, 52);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(46, 24);
            this.label18.TabIndex = 1;
            this.label18.Text = "工号";
            // 
            // UserAdd
            // 
            this.UserAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UserAdd.Location = new System.Drawing.Point(532, 100);
            this.UserAdd.Margin = new System.Windows.Forms.Padding(2);
            this.UserAdd.Name = "UserAdd";
            this.UserAdd.Size = new System.Drawing.Size(98, 44);
            this.UserAdd.TabIndex = 0;
            this.UserAdd.Text = "添加用户";
            this.UserAdd.UseVisualStyleBackColor = true;
            this.UserAdd.Click += new System.EventHandler(this.UserAdd_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.系统设置panel3);
            this.tabPage3.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabPage3.Location = new System.Drawing.Point(4, 4);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1894, 984);
            this.tabPage3.TabIndex = 13;
            this.tabPage3.Text = "系统设置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // 系统设置panel3
            // 
            this.系统设置panel3.Controls.Add(this.groupBox18);
            this.系统设置panel3.Controls.Add(this.panel19);
            this.系统设置panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.系统设置panel3.Location = new System.Drawing.Point(0, 0);
            this.系统设置panel3.Margin = new System.Windows.Forms.Padding(2);
            this.系统设置panel3.Name = "系统设置panel3";
            this.系统设置panel3.Size = new System.Drawing.Size(1894, 984);
            this.系统设置panel3.TabIndex = 0;
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.tableLayoutPanel14);
            this.groupBox18.Controls.Add(this.label16);
            this.groupBox18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox18.ForeColor = System.Drawing.Color.DimGray;
            this.groupBox18.Location = new System.Drawing.Point(0, 162);
            this.groupBox18.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox18.Size = new System.Drawing.Size(1894, 822);
            this.groupBox18.TabIndex = 68;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "产品型号维护";
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Controls.Add(this.panel20, 0, 0);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(2, 25);
            this.tableLayoutPanel14.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 1;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(1890, 795);
            this.tableLayoutPanel14.TabIndex = 63;
            // 
            // panel20
            // 
            this.panel20.Controls.Add(this.dgvProductModel);
            this.panel20.Controls.Add(this.panel5);
            this.panel20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel20.ForeColor = System.Drawing.Color.Black;
            this.panel20.Location = new System.Drawing.Point(2, 2);
            this.panel20.Margin = new System.Windows.Forms.Padding(2);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(941, 791);
            this.panel20.TabIndex = 68;
            // 
            // dgvProductModel
            // 
            this.dgvProductModel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductModel.Location = new System.Drawing.Point(0, 41);
            this.dgvProductModel.Margin = new System.Windows.Forms.Padding(4);
            this.dgvProductModel.Name = "dgvProductModel";
            this.dgvProductModel.RowHeadersWidth = 51;
            this.dgvProductModel.RowTemplate.Height = 23;
            this.dgvProductModel.Size = new System.Drawing.Size(941, 750);
            this.dgvProductModel.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.ImportFile);
            this.panel5.Controls.Add(this.changeTypeRefresh);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(941, 41);
            this.panel5.TabIndex = 0;
            // 
            // ImportFile
            // 
            this.ImportFile.BackColor = System.Drawing.Color.PaleTurquoise;
            this.ImportFile.Dock = System.Windows.Forms.DockStyle.Left;
            this.ImportFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ImportFile.Location = new System.Drawing.Point(249, 0);
            this.ImportFile.Margin = new System.Windows.Forms.Padding(4);
            this.ImportFile.Name = "ImportFile";
            this.ImportFile.Size = new System.Drawing.Size(191, 41);
            this.ImportFile.TabIndex = 1;
            this.ImportFile.Text = "导入文件";
            this.ImportFile.UseVisualStyleBackColor = false;
            this.ImportFile.Click += new System.EventHandler(this.ImportProductModelByCsv_Click);
            // 
            // changeTypeRefresh
            // 
            this.changeTypeRefresh.Dock = System.Windows.Forms.DockStyle.Left;
            this.changeTypeRefresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.changeTypeRefresh.Location = new System.Drawing.Point(0, 0);
            this.changeTypeRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.changeTypeRefresh.Name = "changeTypeRefresh";
            this.changeTypeRefresh.Size = new System.Drawing.Size(249, 41);
            this.changeTypeRefresh.TabIndex = 0;
            this.changeTypeRefresh.Text = "刷新";
            this.changeTypeRefresh.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.Color.Red;
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(18, 79);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(0, 24);
            this.label16.TabIndex = 62;
            // 
            // panel19
            // 
            this.panel19.Controls.Add(this.tableLayoutPanel11);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel19.Location = new System.Drawing.Point(0, 0);
            this.panel19.Margin = new System.Windows.Forms.Padding(2);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(1894, 162);
            this.panel19.TabIndex = 67;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 4;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.46154F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.89743F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.64103F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.Controls.Add(this.groupBox20, 2, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox5, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox17, 1, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel11.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(1894, 162);
            this.tableLayoutPanel11.TabIndex = 0;
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.SaveDataBase);
            this.groupBox20.Controls.Add(this.deviceDataBase);
            this.groupBox20.Controls.Add(this.label59);
            this.groupBox20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox20.Location = new System.Drawing.Point(1411, 4);
            this.groupBox20.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox20.Size = new System.Drawing.Size(477, 154);
            this.groupBox20.TabIndex = 64;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "设备";
            // 
            // SaveDataBase
            // 
            this.SaveDataBase.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SaveDataBase.Location = new System.Drawing.Point(128, 92);
            this.SaveDataBase.Margin = new System.Windows.Forms.Padding(4);
            this.SaveDataBase.Name = "SaveDataBase";
            this.SaveDataBase.Size = new System.Drawing.Size(114, 44);
            this.SaveDataBase.TabIndex = 66;
            this.SaveDataBase.Text = "执行";
            this.SaveDataBase.UseVisualStyleBackColor = true;
            this.SaveDataBase.Click += new System.EventHandler(this.ChangeDataBase_Click);
            // 
            // deviceDataBase
            // 
            this.deviceDataBase.FormattingEnabled = true;
            this.deviceDataBase.Items.AddRange(new object[] {
            "上工装1",
            "上工装2",
            "螺钉机",
            "装配机"});
            this.deviceDataBase.Location = new System.Drawing.Point(128, 32);
            this.deviceDataBase.Margin = new System.Windows.Forms.Padding(4);
            this.deviceDataBase.Name = "deviceDataBase";
            this.deviceDataBase.Size = new System.Drawing.Size(150, 31);
            this.deviceDataBase.TabIndex = 64;
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label59.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label59.Location = new System.Drawing.Point(24, 35);
            this.label59.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(97, 27);
            this.label59.TabIndex = 65;
            this.label59.Text = "切换设备:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.PlcInputAutoSave);
            this.groupBox5.Controls.Add(this.PlcConnectType);
            this.groupBox5.Controls.Add(this.label24);
            this.groupBox5.Controls.Add(this.btnStartTask);
            this.groupBox5.Controls.Add(this.EndTask);
            this.groupBox5.Controls.Add(this.ManualConnect);
            this.groupBox5.Controls.Add(this.label30);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.PlcPort);
            this.groupBox5.Controls.Add(this.PlcIP);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(2, 2);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(724, 158);
            this.groupBox5.TabIndex = 60;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "PLC参数";
            // 
            // PlcInputAutoSave
            // 
            this.PlcInputAutoSave.AutoSize = true;
            this.PlcInputAutoSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PlcInputAutoSave.Location = new System.Drawing.Point(352, 28);
            this.PlcInputAutoSave.Margin = new System.Windows.Forms.Padding(2);
            this.PlcInputAutoSave.Name = "PlcInputAutoSave";
            this.PlcInputAutoSave.Size = new System.Drawing.Size(104, 28);
            this.PlcInputAutoSave.TabIndex = 65;
            this.PlcInputAutoSave.Text = "自动保存";
            this.PlcInputAutoSave.UseVisualStyleBackColor = true;
            // 
            // PlcConnectType
            // 
            this.PlcConnectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PlcConnectType.FormattingEnabled = true;
            this.PlcConnectType.Items.AddRange(new object[] {
            "TCP",
            "UDP",
            "MC",
            "Modbus"});
            this.PlcConnectType.Location = new System.Drawing.Point(88, 100);
            this.PlcConnectType.Margin = new System.Windows.Forms.Padding(2);
            this.PlcConnectType.Name = "PlcConnectType";
            this.PlcConnectType.Size = new System.Drawing.Size(256, 31);
            this.PlcConnectType.TabIndex = 64;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label24.Location = new System.Drawing.Point(31, 102);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(50, 24);
            this.label24.TabIndex = 63;
            this.label24.Text = "类型:";
            // 
            // btnStartTask
            // 
            this.btnStartTask.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStartTask.Location = new System.Drawing.Point(566, 64);
            this.btnStartTask.Margin = new System.Windows.Forms.Padding(2);
            this.btnStartTask.Name = "btnStartTask";
            this.btnStartTask.Size = new System.Drawing.Size(68, 68);
            this.btnStartTask.TabIndex = 61;
            this.btnStartTask.Text = "启动任务";
            this.btnStartTask.UseVisualStyleBackColor = false;
            this.btnStartTask.Click += new System.EventHandler(this.StartTask_Click);
            // 
            // EndTask
            // 
            this.EndTask.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EndTask.Location = new System.Drawing.Point(468, 64);
            this.EndTask.Margin = new System.Windows.Forms.Padding(2);
            this.EndTask.Name = "EndTask";
            this.EndTask.Size = new System.Drawing.Size(68, 68);
            this.EndTask.TabIndex = 61;
            this.EndTask.Text = "结束任务";
            this.EndTask.UseVisualStyleBackColor = false;
            this.EndTask.Click += new System.EventHandler(this.EndTask_Click);
            // 
            // ManualConnect
            // 
            this.ManualConnect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ManualConnect.Location = new System.Drawing.Point(372, 64);
            this.ManualConnect.Margin = new System.Windows.Forms.Padding(2);
            this.ManualConnect.Name = "ManualConnect";
            this.ManualConnect.Size = new System.Drawing.Size(68, 68);
            this.ManualConnect.TabIndex = 61;
            this.ManualConnect.Text = "手动连接";
            this.ManualConnect.UseVisualStyleBackColor = true;
            this.ManualConnect.Click += new System.EventHandler(this.ManualConnect_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label30.Location = new System.Drawing.Point(31, 68);
            this.label30.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(50, 24);
            this.label30.TabIndex = 60;
            this.label30.Text = "端口:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(51, 32);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 24);
            this.label5.TabIndex = 0;
            this.label5.Text = "IP:";
            // 
            // PlcPort
            // 
            this.PlcPort.Location = new System.Drawing.Point(88, 64);
            this.PlcPort.Margin = new System.Windows.Forms.Padding(2);
            this.PlcPort.Name = "PlcPort";
            this.PlcPort.Size = new System.Drawing.Size(256, 30);
            this.PlcPort.TabIndex = 2;
            this.PlcPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyAllowDigital_KeyPress);
            // 
            // PlcIP
            // 
            this.PlcIP.Location = new System.Drawing.Point(88, 28);
            this.PlcIP.Margin = new System.Windows.Forms.Padding(2);
            this.PlcIP.Name = "PlcIP";
            this.PlcIP.Size = new System.Drawing.Size(256, 30);
            this.PlcIP.TabIndex = 1;
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.DeviceName);
            this.groupBox17.Controls.Add(this.label8);
            this.groupBox17.Controls.Add(this.OtherSettingsSave);
            this.groupBox17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox17.Location = new System.Drawing.Point(730, 2);
            this.groupBox17.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox17.Size = new System.Drawing.Size(675, 158);
            this.groupBox17.TabIndex = 63;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "其它设置";
            // 
            // DeviceName
            // 
            this.DeviceName.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.DeviceName.Location = new System.Drawing.Point(109, 21);
            this.DeviceName.Margin = new System.Windows.Forms.Padding(2);
            this.DeviceName.Name = "DeviceName";
            this.DeviceName.Size = new System.Drawing.Size(309, 38);
            this.DeviceName.TabIndex = 61;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(14, 28);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 27);
            this.label8.TabIndex = 1;
            this.label8.Text = "设备名称：";
            // 
            // OtherSettingsSave
            // 
            this.OtherSettingsSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OtherSettingsSave.Location = new System.Drawing.Point(245, 90);
            this.OtherSettingsSave.Margin = new System.Windows.Forms.Padding(2);
            this.OtherSettingsSave.Name = "OtherSettingsSave";
            this.OtherSettingsSave.Size = new System.Drawing.Size(161, 54);
            this.OtherSettingsSave.TabIndex = 61;
            this.OtherSettingsSave.Text = "保存";
            this.OtherSettingsSave.UseVisualStyleBackColor = true;
            this.OtherSettingsSave.Click += new System.EventHandler(this.SystemSetSaveButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel10);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1894, 984);
            this.tabPage2.TabIndex = 5;
            this.tabPage2.Text = "异常详情";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 1;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.groupBox7, 1, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 984F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(1894, 984);
            this.tableLayoutPanel10.TabIndex = 0;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.rtbErrorLog);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(2, 2);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox7.Size = new System.Drawing.Size(1890, 980);
            this.groupBox7.TabIndex = 108;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "程序异常详细信息显示";
            // 
            // rtbErrorLog
            // 
            this.rtbErrorLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbErrorLog.ForeColor = System.Drawing.Color.OrangeRed;
            this.rtbErrorLog.Location = new System.Drawing.Point(2, 25);
            this.rtbErrorLog.Margin = new System.Windows.Forms.Padding(2);
            this.rtbErrorLog.Name = "rtbErrorLog";
            this.rtbErrorLog.Size = new System.Drawing.Size(1886, 953);
            this.rtbErrorLog.TabIndex = 0;
            this.rtbErrorLog.Text = "";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.整页面);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.tabPage1.Size = new System.Drawing.Size(1894, 984);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "运行界面";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // 整页面
            // 
            this.整页面.Controls.Add(this.tableLayoutPanel3);
            this.整页面.Dock = System.Windows.Forms.DockStyle.Fill;
            this.整页面.Location = new System.Drawing.Point(4, 2);
            this.整页面.Margin = new System.Windows.Forms.Padding(2);
            this.整页面.Name = "整页面";
            this.整页面.Size = new System.Drawing.Size(1886, 980);
            this.整页面.TabIndex = 105;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel12, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1886, 980);
            this.tableLayoutPanel3.TabIndex = 102;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tabControl_UploadData, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1127, 976);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.82077F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.767F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.41223F));
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.label54, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(1123, 113);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Controls.Add(this.InterfaceTipLabel, 2, 1);
            this.tableLayoutPanel8.Controls.Add(this.DeviceStatusDisplay, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.DeviceStatusSignalLight, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.PlcTipLabel, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.PlcSignalLight, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.InterfaceSignalLight, 2, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(771, 2);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(350, 109);
            this.tableLayoutPanel8.TabIndex = 44;
            // 
            // InterfaceTipLabel
            // 
            this.InterfaceTipLabel.AutoSize = true;
            this.InterfaceTipLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InterfaceTipLabel.Font = new System.Drawing.Font("SimSun", 15.2F, System.Drawing.FontStyle.Bold);
            this.InterfaceTipLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.InterfaceTipLabel.Location = new System.Drawing.Point(241, 83);
            this.InterfaceTipLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.InterfaceTipLabel.Name = "InterfaceTipLabel";
            this.InterfaceTipLabel.Size = new System.Drawing.Size(107, 26);
            this.InterfaceTipLabel.TabIndex = 118;
            this.InterfaceTipLabel.Text = "MES";
            this.InterfaceTipLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DeviceStatusDisplay
            // 
            this.DeviceStatusDisplay.AutoSize = true;
            this.DeviceStatusDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DeviceStatusDisplay.Font = new System.Drawing.Font("SimSun", 15.2F, System.Drawing.FontStyle.Bold);
            this.DeviceStatusDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DeviceStatusDisplay.Location = new System.Drawing.Point(113, 83);
            this.DeviceStatusDisplay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DeviceStatusDisplay.Name = "DeviceStatusDisplay";
            this.DeviceStatusDisplay.Size = new System.Drawing.Size(124, 26);
            this.DeviceStatusDisplay.TabIndex = 116;
            this.DeviceStatusDisplay.Text = "DOWNTIME";
            this.DeviceStatusDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DeviceStatusSignalLight
            // 
            this.DeviceStatusSignalLight.AutoSize = true;
            this.DeviceStatusSignalLight.BackColor = System.Drawing.Color.Transparent;
            this.DeviceStatusSignalLight.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DeviceStatusSignalLight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DeviceStatusSignalLight.Font = new System.Drawing.Font("Microsoft YaHei", 22F, System.Drawing.FontStyle.Bold);
            this.DeviceStatusSignalLight.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.DeviceStatusSignalLight.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DeviceStatusSignalLight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DeviceStatusSignalLight.Location = new System.Drawing.Point(111, 0);
            this.DeviceStatusSignalLight.Margin = new System.Windows.Forms.Padding(0);
            this.DeviceStatusSignalLight.Name = "DeviceStatusSignalLight";
            this.DeviceStatusSignalLight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.DeviceStatusSignalLight.Size = new System.Drawing.Size(128, 83);
            this.DeviceStatusSignalLight.TabIndex = 115;
            this.DeviceStatusSignalLight.Text = "██";
            this.DeviceStatusSignalLight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlcTipLabel
            // 
            this.PlcTipLabel.AutoSize = true;
            this.PlcTipLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlcTipLabel.Font = new System.Drawing.Font("SimSun", 15.2F, System.Drawing.FontStyle.Bold);
            this.PlcTipLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PlcTipLabel.Location = new System.Drawing.Point(2, 83);
            this.PlcTipLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.PlcTipLabel.Name = "PlcTipLabel";
            this.PlcTipLabel.Size = new System.Drawing.Size(107, 26);
            this.PlcTipLabel.TabIndex = 114;
            this.PlcTipLabel.Text = "PLC";
            this.PlcTipLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlcSignalLight
            // 
            this.PlcSignalLight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlcSignalLight.Font = new System.Drawing.Font("Microsoft YaHei", 22F, System.Drawing.FontStyle.Bold);
            this.PlcSignalLight.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.PlcSignalLight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PlcSignalLight.Location = new System.Drawing.Point(2, 0);
            this.PlcSignalLight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.PlcSignalLight.Name = "PlcSignalLight";
            this.PlcSignalLight.Size = new System.Drawing.Size(107, 83);
            this.PlcSignalLight.TabIndex = 113;
            this.PlcSignalLight.Text = "██";
            this.PlcSignalLight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InterfaceSignalLight
            // 
            this.InterfaceSignalLight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InterfaceSignalLight.Font = new System.Drawing.Font("Microsoft YaHei", 22F);
            this.InterfaceSignalLight.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.InterfaceSignalLight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.InterfaceSignalLight.Location = new System.Drawing.Point(241, 0);
            this.InterfaceSignalLight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.InterfaceSignalLight.Name = "InterfaceSignalLight";
            this.InterfaceSignalLight.Size = new System.Drawing.Size(107, 83);
            this.InterfaceSignalLight.TabIndex = 117;
            this.InterfaceSignalLight.Text = "██";
            this.InterfaceSignalLight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label54
            // 
            this.label54.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label54.Font = new System.Drawing.Font("Microsoft YaHei", 38.2F, System.Drawing.FontStyle.Bold);
            this.label54.ForeColor = System.Drawing.Color.SteelBlue;
            this.label54.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label54.Location = new System.Drawing.Point(336, 0);
            this.label54.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(431, 113);
            this.label54.TabIndex = 43;
            this.label54.Text = "机台的名称";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::MesDatas.Properties.Resources.kaifa;
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(330, 109);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl_UploadData
            // 
            this.tabControl_UploadData.Controls.Add(this.tabPageResult1);
            this.tabControl_UploadData.Controls.Add(this.tabPageResult2);
            this.tabControl_UploadData.Controls.Add(this.tabPageResult3);
            this.tabControl_UploadData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_UploadData.Location = new System.Drawing.Point(2, 119);
            this.tabControl_UploadData.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl_UploadData.Name = "tabControl_UploadData";
            this.tabControl_UploadData.SelectedIndex = 0;
            this.tabControl_UploadData.Size = new System.Drawing.Size(1123, 855);
            this.tabControl_UploadData.TabIndex = 1;
            // 
            // tabPageResult1
            // 
            this.tabPageResult1.Controls.Add(this.dgvResult1);
            this.tabPageResult1.Location = new System.Drawing.Point(4, 32);
            this.tabPageResult1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageResult1.Name = "tabPageResult1";
            this.tabPageResult1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageResult1.Size = new System.Drawing.Size(1115, 819);
            this.tabPageResult1.TabIndex = 0;
            this.tabPageResult1.Text = "result1";
            this.tabPageResult1.UseVisualStyleBackColor = true;
            // 
            // dgvResult1
            // 
            this.dgvResult1.AllowUserToAddRows = false;
            this.dgvResult1.AllowUserToDeleteRows = false;
            this.dgvResult1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvResult1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvResult1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvResult1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvResult1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult1.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgvResult1.Location = new System.Drawing.Point(2, 2);
            this.dgvResult1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvResult1.Name = "dgvResult1";
            this.dgvResult1.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvResult1.RowHeadersVisible = false;
            this.dgvResult1.RowHeadersWidth = 51;
            this.dgvResult1.RowTemplate.Height = 23;
            this.dgvResult1.Size = new System.Drawing.Size(1111, 815);
            this.dgvResult1.TabIndex = 104;
            // 
            // tabPageResult2
            // 
            this.tabPageResult2.Controls.Add(this.dgvResult2);
            this.tabPageResult2.Location = new System.Drawing.Point(4, 32);
            this.tabPageResult2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageResult2.Name = "tabPageResult2";
            this.tabPageResult2.Size = new System.Drawing.Size(1115, 819);
            this.tabPageResult2.TabIndex = 1;
            this.tabPageResult2.Text = "result2";
            this.tabPageResult2.UseVisualStyleBackColor = true;
            // 
            // dgvResult2
            // 
            this.dgvResult2.AllowUserToAddRows = false;
            this.dgvResult2.AllowUserToDeleteRows = false;
            this.dgvResult2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvResult2.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvResult2.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvResult2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult2.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvResult2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult2.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgvResult2.Location = new System.Drawing.Point(0, 0);
            this.dgvResult2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvResult2.Name = "dgvResult2";
            this.dgvResult2.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult2.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvResult2.RowHeadersVisible = false;
            this.dgvResult2.RowHeadersWidth = 51;
            this.dgvResult2.RowTemplate.Height = 23;
            this.dgvResult2.Size = new System.Drawing.Size(1115, 819);
            this.dgvResult2.TabIndex = 105;
            // 
            // tabPageResult3
            // 
            this.tabPageResult3.Controls.Add(this.dgvResult3);
            this.tabPageResult3.Location = new System.Drawing.Point(4, 32);
            this.tabPageResult3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageResult3.Name = "tabPageResult3";
            this.tabPageResult3.Size = new System.Drawing.Size(1115, 819);
            this.tabPageResult3.TabIndex = 2;
            this.tabPageResult3.Text = "result3";
            this.tabPageResult3.UseVisualStyleBackColor = true;
            // 
            // dgvResult3
            // 
            this.dgvResult3.AllowUserToAddRows = false;
            this.dgvResult3.AllowUserToDeleteRows = false;
            this.dgvResult3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvResult3.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvResult3.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult3.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvResult3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult3.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvResult3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult3.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgvResult3.Location = new System.Drawing.Point(0, 0);
            this.dgvResult3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvResult3.Name = "dgvResult3";
            this.dgvResult3.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult3.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvResult3.RowHeadersVisible = false;
            this.dgvResult3.RowHeadersWidth = 51;
            this.dgvResult3.RowTemplate.Height = 23;
            this.dgvResult3.Size = new System.Drawing.Size(1115, 819);
            this.dgvResult3.TabIndex = 105;
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 1;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.groupBox8, 0, 5);
            this.tableLayoutPanel12.Controls.Add(this.ToolingNumberPanel, 0, 4);
            this.tableLayoutPanel12.Controls.Add(this.条码数据, 0, 3);
            this.tableLayoutPanel12.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel12.Controls.Add(this.groupboxx, 0, 1);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(1133, 2);
            this.tableLayoutPanel12.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 6;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(751, 976);
            this.tableLayoutPanel12.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold);
            this.groupBox2.Location = new System.Drawing.Point(2, 2);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(747, 288);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "生产信息";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.30612F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.69388F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.30612F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.69388F));
            this.tableLayoutPanel2.Controls.Add(this.txtProductModel, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label32, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label44, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label17, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.txtTotalQuality, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label9, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.txtOkQuality, 3, 4);
            this.tableLayoutPanel2.Controls.Add(this.txtNgQuanlity, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.txtYieldRate, 3, 5);
            this.tableLayoutPanel2.Controls.Add(this.panel4, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.OrderNum, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label80, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel7, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 22);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.3295F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.7341F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.7341F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.7341F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.7341F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.7341F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(743, 264);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // txtProductModel
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.txtProductModel, 3);
            this.txtProductModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductModel.Enabled = false;
            this.txtProductModel.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.txtProductModel.Location = new System.Drawing.Point(115, 45);
            this.txtProductModel.Margin = new System.Windows.Forms.Padding(2);
            this.txtProductModel.Multiline = true;
            this.txtProductModel.Name = "txtProductModel";
            this.txtProductModel.Size = new System.Drawing.Size(626, 40);
            this.txtProductModel.TabIndex = 149;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label32.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label32.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label32.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label32.Location = new System.Drawing.Point(2, 87);
            this.label32.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(109, 44);
            this.label32.TabIndex = 1;
            this.label32.Text = "工单数量:";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.BackColor = System.Drawing.Color.Transparent;
            this.label44.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label44.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label44.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label44.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label44.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label44.Location = new System.Drawing.Point(2, 0);
            this.label44.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(109, 43);
            this.label44.TabIndex = 60;
            this.label44.Text = "当前用户:";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label17.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(2, 131);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(109, 44);
            this.label17.TabIndex = 1;
            this.label17.Text = "生产工单:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(2, 175);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(109, 44);
            this.label7.TabIndex = 128;
            this.label7.Text = "生产总数:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTotalQuality
            // 
            this.txtTotalQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTotalQuality.Enabled = false;
            this.txtTotalQuality.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.txtTotalQuality.Location = new System.Drawing.Point(115, 177);
            this.txtTotalQuality.Margin = new System.Windows.Forms.Padding(2);
            this.txtTotalQuality.Multiline = true;
            this.txtTotalQuality.Name = "txtTotalQuality";
            this.txtTotalQuality.Size = new System.Drawing.Size(253, 40);
            this.txtTotalQuality.TabIndex = 131;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(372, 175);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(109, 44);
            this.label9.TabIndex = 129;
            this.label9.Text = "良品数:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOkQuality
            // 
            this.txtOkQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOkQuality.Enabled = false;
            this.txtOkQuality.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.txtOkQuality.Location = new System.Drawing.Point(485, 177);
            this.txtOkQuality.Margin = new System.Windows.Forms.Padding(2);
            this.txtOkQuality.Multiline = true;
            this.txtOkQuality.Name = "txtOkQuality";
            this.txtOkQuality.Size = new System.Drawing.Size(256, 40);
            this.txtOkQuality.TabIndex = 132;
            // 
            // txtNgQuanlity
            // 
            this.txtNgQuanlity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNgQuanlity.Enabled = false;
            this.txtNgQuanlity.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.txtNgQuanlity.Location = new System.Drawing.Point(115, 221);
            this.txtNgQuanlity.Margin = new System.Windows.Forms.Padding(2);
            this.txtNgQuanlity.Multiline = true;
            this.txtNgQuanlity.Name = "txtNgQuanlity";
            this.txtNgQuanlity.Size = new System.Drawing.Size(253, 41);
            this.txtNgQuanlity.TabIndex = 133;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(2, 219);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 45);
            this.label10.TabIndex = 130;
            this.label10.Text = "不良数:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(372, 219);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 45);
            this.label2.TabIndex = 134;
            this.label2.Text = "良率:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtYieldRate
            // 
            this.txtYieldRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtYieldRate.Enabled = false;
            this.txtYieldRate.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.txtYieldRate.Location = new System.Drawing.Point(485, 221);
            this.txtYieldRate.Margin = new System.Windows.Forms.Padding(2);
            this.txtYieldRate.Multiline = true;
            this.txtYieldRate.Name = "txtYieldRate";
            this.txtYieldRate.Size = new System.Drawing.Size(256, 41);
            this.txtYieldRate.TabIndex = 135;
            // 
            // panel4
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel4, 3);
            this.panel4.Controls.Add(this.OrderNo);
            this.panel4.Controls.Add(this.ManualChangeMO);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(115, 133);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(626, 40);
            this.panel4.TabIndex = 141;
            // 
            // OrderNo
            // 
            this.OrderNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OrderNo.Enabled = false;
            this.OrderNo.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.OrderNo.Location = new System.Drawing.Point(0, 0);
            this.OrderNo.Margin = new System.Windows.Forms.Padding(2);
            this.OrderNo.Multiline = true;
            this.OrderNo.Name = "OrderNo";
            this.OrderNo.Size = new System.Drawing.Size(524, 40);
            this.OrderNo.TabIndex = 138;
            // 
            // ManualChangeMO
            // 
            this.ManualChangeMO.Dock = System.Windows.Forms.DockStyle.Right;
            this.ManualChangeMO.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.ManualChangeMO.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ManualChangeMO.Location = new System.Drawing.Point(524, 0);
            this.ManualChangeMO.Margin = new System.Windows.Forms.Padding(2);
            this.ManualChangeMO.Name = "ManualChangeMO";
            this.ManualChangeMO.Size = new System.Drawing.Size(102, 40);
            this.ManualChangeMO.TabIndex = 129;
            this.ManualChangeMO.Text = "手动切换";
            this.ManualChangeMO.UseVisualStyleBackColor = true;
            this.ManualChangeMO.Click += new System.EventHandler(this.ManualChangeManufacturingOrder_Click);
            // 
            // OrderNum
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.OrderNum, 3);
            this.OrderNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OrderNum.Enabled = false;
            this.OrderNum.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.OrderNum.Location = new System.Drawing.Point(115, 89);
            this.OrderNum.Margin = new System.Windows.Forms.Padding(2);
            this.OrderNum.Multiline = true;
            this.OrderNum.Name = "OrderNum";
            this.OrderNum.Size = new System.Drawing.Size(626, 40);
            this.OrderNum.TabIndex = 142;
            // 
            // label80
            // 
            this.label80.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label80.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            this.label80.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label80.Location = new System.Drawing.Point(2, 43);
            this.label80.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(109, 44);
            this.label80.TabIndex = 125;
            this.label80.Text = "产品型号:";
            this.label80.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel7
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel7, 3);
            this.panel7.Controls.Add(this.txtUser);
            this.panel7.Controls.Add(this.btnLogOut);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(117, 4);
            this.panel7.Margin = new System.Windows.Forms.Padding(4);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(622, 35);
            this.panel7.TabIndex = 150;
            // 
            // txtUser
            // 
            this.txtUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUser.Enabled = false;
            this.txtUser.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F);
            this.txtUser.Location = new System.Drawing.Point(0, 0);
            this.txtUser.Margin = new System.Windows.Forms.Padding(2);
            this.txtUser.Multiline = true;
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(521, 35);
            this.txtUser.TabIndex = 139;
            // 
            // btnLogOut
            // 
            this.btnLogOut.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnLogOut.Font = new System.Drawing.Font("Microsoft YaHei", 11F);
            this.btnLogOut.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLogOut.Location = new System.Drawing.Point(521, 0);
            this.btnLogOut.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogOut.Name = "btnLogOut";
            this.btnLogOut.Size = new System.Drawing.Size(101, 35);
            this.btnLogOut.TabIndex = 140;
            this.btnLogOut.Text = "退出登录";
            this.btnLogOut.UseVisualStyleBackColor = true;
            this.btnLogOut.Click += new System.EventHandler(this.LogOut_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.lblProductResult);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold);
            this.groupBox8.Location = new System.Drawing.Point(2, 798);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox8.Size = new System.Drawing.Size(747, 176);
            this.groupBox8.TabIndex = 72;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "产品结果";
            // 
            // lblProductResult
            // 
            this.lblProductResult.BackColor = System.Drawing.Color.Transparent;
            this.lblProductResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProductResult.Font = new System.Drawing.Font("SimSun", 72F, System.Drawing.FontStyle.Bold);
            this.lblProductResult.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblProductResult.Location = new System.Drawing.Point(2, 22);
            this.lblProductResult.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProductResult.Name = "lblProductResult";
            this.lblProductResult.Size = new System.Drawing.Size(743, 152);
            this.lblProductResult.TabIndex = 31;
            this.lblProductResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ToolingNumberPanel
            // 
            this.ToolingNumberPanel.Controls.Add(this.ToolingNumber);
            this.ToolingNumberPanel.Controls.Add(this.label71);
            this.ToolingNumberPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolingNumberPanel.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold);
            this.ToolingNumberPanel.Location = new System.Drawing.Point(2, 672);
            this.ToolingNumberPanel.Margin = new System.Windows.Forms.Padding(2);
            this.ToolingNumberPanel.Name = "ToolingNumberPanel";
            this.ToolingNumberPanel.Padding = new System.Windows.Forms.Padding(2);
            this.ToolingNumberPanel.Size = new System.Drawing.Size(747, 122);
            this.ToolingNumberPanel.TabIndex = 51;
            this.ToolingNumberPanel.TabStop = false;
            this.ToolingNumberPanel.Text = "工装编号";
            // 
            // ToolingNumber
            // 
            this.ToolingNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolingNumber.Font = new System.Drawing.Font("SimSun", 22.2F, System.Drawing.FontStyle.Bold);
            this.ToolingNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ToolingNumber.Location = new System.Drawing.Point(2, 22);
            this.ToolingNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ToolingNumber.Name = "ToolingNumber";
            this.ToolingNumber.Size = new System.Drawing.Size(743, 98);
            this.ToolingNumber.TabIndex = 44;
            this.ToolingNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Font = new System.Drawing.Font("SimSun", 20F, System.Drawing.FontStyle.Bold);
            this.label71.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label71.Location = new System.Drawing.Point(8, 21);
            this.label71.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(0, 34);
            this.label71.TabIndex = 43;
            // 
            // 条码数据
            // 
            this.条码数据.Controls.Add(this.tableLayoutPanel24);
            this.条码数据.Dock = System.Windows.Forms.DockStyle.Fill;
            this.条码数据.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold);
            this.条码数据.Location = new System.Drawing.Point(2, 546);
            this.条码数据.Margin = new System.Windows.Forms.Padding(2);
            this.条码数据.Name = "条码数据";
            this.条码数据.Padding = new System.Windows.Forms.Padding(2);
            this.条码数据.Size = new System.Drawing.Size(747, 122);
            this.条码数据.TabIndex = 52;
            this.条码数据.TabStop = false;
            this.条码数据.Text = "条码数据";
            // 
            // tableLayoutPanel24
            // 
            this.tableLayoutPanel24.ColumnCount = 2;
            this.tableLayoutPanel24.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel24.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel24.Controls.Add(this.barCode, 0, 0);
            this.tableLayoutPanel24.Controls.Add(this.btnManualInputBarcode, 1, 0);
            this.tableLayoutPanel24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel24.Location = new System.Drawing.Point(2, 22);
            this.tableLayoutPanel24.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel24.Name = "tableLayoutPanel24";
            this.tableLayoutPanel24.RowCount = 1;
            this.tableLayoutPanel24.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel24.Size = new System.Drawing.Size(743, 98);
            this.tableLayoutPanel24.TabIndex = 48;
            // 
            // barCode
            // 
            this.barCode.BackColor = System.Drawing.Color.White;
            this.barCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.barCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.barCode.Font = new System.Drawing.Font("SimSun", 19.8F, System.Drawing.FontStyle.Bold);
            this.barCode.ForeColor = System.Drawing.SystemColors.Highlight;
            this.barCode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.barCode.Location = new System.Drawing.Point(2, 0);
            this.barCode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.barCode.Name = "barCode";
            this.barCode.Size = new System.Drawing.Size(657, 98);
            this.barCode.TabIndex = 2;
            this.barCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnManualInputBarcode
            // 
            this.btnManualInputBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnManualInputBarcode.Font = new System.Drawing.Font("SimSun", 13F, System.Drawing.FontStyle.Bold);
            this.btnManualInputBarcode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnManualInputBarcode.Location = new System.Drawing.Point(665, 4);
            this.btnManualInputBarcode.Margin = new System.Windows.Forms.Padding(4);
            this.btnManualInputBarcode.Name = "btnManualInputBarcode";
            this.btnManualInputBarcode.Size = new System.Drawing.Size(74, 90);
            this.btnManualInputBarcode.TabIndex = 47;
            this.btnManualInputBarcode.Text = "手动\r\n输入";
            this.btnManualInputBarcode.UseVisualStyleBackColor = true;
            this.btnManualInputBarcode.Visible = false;
            this.btnManualInputBarcode.Click += new System.EventHandler(this.manualInputBarcode_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(2, 420);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(747, 122);
            this.groupBox1.TabIndex = 53;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "运行状态";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("SimSun", 20F, System.Drawing.FontStyle.Bold);
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(8, 21);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(0, 34);
            this.label26.TabIndex = 43;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel16);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(2, 22);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(743, 98);
            this.panel2.TabIndex = 44;
            // 
            // tableLayoutPanel16
            // 
            this.tableLayoutPanel16.ColumnCount = 2;
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 89F));
            this.tableLayoutPanel16.Controls.Add(this.ManualRecovery, 0, 0);
            this.tableLayoutPanel16.Controls.Add(this.lblRunningStatus, 1, 0);
            this.tableLayoutPanel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel16.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel16.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel16.Name = "tableLayoutPanel16";
            this.tableLayoutPanel16.RowCount = 1;
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel16.Size = new System.Drawing.Size(743, 98);
            this.tableLayoutPanel16.TabIndex = 48;
            // 
            // ManualRecovery
            // 
            this.ManualRecovery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ManualRecovery.Font = new System.Drawing.Font("SimSun", 13F, System.Drawing.FontStyle.Bold);
            this.ManualRecovery.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ManualRecovery.Location = new System.Drawing.Point(4, 4);
            this.ManualRecovery.Margin = new System.Windows.Forms.Padding(4);
            this.ManualRecovery.Name = "ManualRecovery";
            this.ManualRecovery.Size = new System.Drawing.Size(73, 90);
            this.ManualRecovery.TabIndex = 47;
            this.ManualRecovery.Text = "手动\r\n复位";
            this.ManualRecovery.UseVisualStyleBackColor = true;
            this.ManualRecovery.Click += new System.EventHandler(this.ManualRecovery_Click);
            // 
            // lblRunningStatus
            // 
            this.lblRunningStatus.AutoSize = true;
            this.lblRunningStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRunningStatus.Font = new System.Drawing.Font("SimSun", 18F, System.Drawing.FontStyle.Bold);
            this.lblRunningStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblRunningStatus.Location = new System.Drawing.Point(83, 0);
            this.lblRunningStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRunningStatus.Name = "lblRunningStatus";
            this.lblRunningStatus.Size = new System.Drawing.Size(658, 98);
            this.lblRunningStatus.TabIndex = 45;
            this.lblRunningStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupboxx
            // 
            this.groupboxx.Controls.Add(this.tableLayoutPanel17);
            this.groupboxx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupboxx.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold);
            this.groupboxx.Location = new System.Drawing.Point(4, 296);
            this.groupboxx.Margin = new System.Windows.Forms.Padding(4);
            this.groupboxx.Name = "groupboxx";
            this.groupboxx.Padding = new System.Windows.Forms.Padding(4);
            this.groupboxx.Size = new System.Drawing.Size(743, 118);
            this.groupboxx.TabIndex = 73;
            this.groupboxx.TabStop = false;
            this.groupboxx.Text = "运行状态异常提示:当前阻塞模式";
            // 
            // tableLayoutPanel17
            // 
            this.tableLayoutPanel17.ColumnCount = 3;
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78F));
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableLayoutPanel17.Controls.Add(this.btnBlockMode, 0, 0);
            this.tableLayoutPanel17.Controls.Add(this.btnManualClear, 2, 0);
            this.tableLayoutPanel17.Controls.Add(this.lblStatusErrorTip, 1, 0);
            this.tableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel17.Location = new System.Drawing.Point(4, 24);
            this.tableLayoutPanel17.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel17.Name = "tableLayoutPanel17";
            this.tableLayoutPanel17.RowCount = 1;
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Size = new System.Drawing.Size(735, 90);
            this.tableLayoutPanel17.TabIndex = 49;
            // 
            // btnBlockMode
            // 
            this.btnBlockMode.BackColor = System.Drawing.Color.Gainsboro;
            this.btnBlockMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBlockMode.Font = new System.Drawing.Font("SimSun", 13F, System.Drawing.FontStyle.Bold);
            this.btnBlockMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnBlockMode.Location = new System.Drawing.Point(4, 4);
            this.btnBlockMode.Margin = new System.Windows.Forms.Padding(4);
            this.btnBlockMode.Name = "btnBlockMode";
            this.btnBlockMode.Size = new System.Drawing.Size(72, 82);
            this.btnBlockMode.TabIndex = 48;
            this.btnBlockMode.Text = "放行\r\n模式";
            this.btnBlockMode.UseVisualStyleBackColor = false;
            this.btnBlockMode.Click += new System.EventHandler(this.SwitchBlockMode);
            // 
            // btnManualClear
            // 
            this.btnManualClear.BackColor = System.Drawing.Color.Red;
            this.btnManualClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnManualClear.Font = new System.Drawing.Font("SimSun", 13F, System.Drawing.FontStyle.Bold);
            this.btnManualClear.ForeColor = System.Drawing.Color.White;
            this.btnManualClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnManualClear.Location = new System.Drawing.Point(657, 4);
            this.btnManualClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnManualClear.Name = "btnManualClear";
            this.btnManualClear.Size = new System.Drawing.Size(74, 82);
            this.btnManualClear.TabIndex = 46;
            this.btnManualClear.Text = "报警\r\n清除";
            this.btnManualClear.UseVisualStyleBackColor = false;
            this.btnManualClear.Visible = false;
            this.btnManualClear.Click += new System.EventHandler(this.ManualClear_Click);
            // 
            // lblStatusErrorTip
            // 
            this.lblStatusErrorTip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatusErrorTip.Font = new System.Drawing.Font("SimSun", 17.25F, System.Drawing.FontStyle.Bold);
            this.lblStatusErrorTip.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStatusErrorTip.Location = new System.Drawing.Point(82, 0);
            this.lblStatusErrorTip.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatusErrorTip.Name = "lblStatusErrorTip";
            this.lblStatusErrorTip.Size = new System.Drawing.Size(569, 90);
            this.lblStatusErrorTip.TabIndex = 46;
            this.lblStatusErrorTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TabContorl
            // 
            this.TabContorl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.TabContorl.Controls.Add(this.tabPage1);
            this.TabContorl.Controls.Add(this.tabPage2);
            this.TabContorl.Controls.Add(this.tabPage3);
            this.TabContorl.Controls.Add(this.tabPage4);
            this.TabContorl.Controls.Add(this.tabPage5);
            this.TabContorl.Controls.Add(this.tabPage6);
            this.TabContorl.Controls.Add(this.tabPage7);
            this.TabContorl.Controls.Add(this.tabPage8);
            this.TabContorl.Controls.Add(this.tabPage9);
            this.TabContorl.Controls.Add(this.tabPageTorqueMonitor);
            this.TabContorl.Cursor = System.Windows.Forms.Cursors.Default;
            this.TabContorl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabContorl.ItemSize = new System.Drawing.Size(100, 40);
            this.TabContorl.Location = new System.Drawing.Point(0, 0);
            this.TabContorl.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.TabContorl.Name = "TabContorl";
            this.TabContorl.Padding = new System.Drawing.Point(20, 3);
            this.TabContorl.SelectedIndex = 0;
            this.TabContorl.Size = new System.Drawing.Size(1902, 1032);
            this.TabContorl.TabIndex = 0;
            // 
            // tabPageTorqueMonitor
            // 
            this.tabPageTorqueMonitor.Controls.Add(this.tlpTorqueMonitor);
            this.tabPageTorqueMonitor.Location = new System.Drawing.Point(4, 4);
            this.tabPageTorqueMonitor.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageTorqueMonitor.Name = "tabPageTorqueMonitor";
            this.tabPageTorqueMonitor.Size = new System.Drawing.Size(1894, 984);
            this.tabPageTorqueMonitor.TabIndex = 18;
            this.tabPageTorqueMonitor.Text = "扭力监测";
            this.tabPageTorqueMonitor.UseVisualStyleBackColor = true;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label63.Font = new System.Drawing.Font("SimSun", 15.2F, System.Drawing.FontStyle.Bold);
            this.label63.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label63.Location = new System.Drawing.Point(3, 119);
            this.label63.Margin = new System.Windows.Forms.Padding(3);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(153, 26);
            this.label63.TabIndex = 118;
            this.label63.Text = "Screw-BA";
            this.label63.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpScan_ASSY
            // 
            this.tlpScan_ASSY.ColumnCount = 2;
            this.tlpScan_ASSY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.85421F));
            this.tlpScan_ASSY.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.14579F));
            this.tlpScan_ASSY.Controls.Add(this.groupBox31, 1, 0);
            this.tlpScan_ASSY.Controls.Add(this.label66, 0, 1);
            this.tlpScan_ASSY.Controls.Add(this.ASSY, 0, 0);
            this.tlpScan_ASSY.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpScan_ASSY.Location = new System.Drawing.Point(0, 0);
            this.tlpScan_ASSY.Margin = new System.Windows.Forms.Padding(2);
            this.tlpScan_ASSY.Name = "tlpScan_ASSY";
            this.tlpScan_ASSY.RowCount = 2;
            this.tlpScan_ASSY.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpScan_ASSY.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScan_ASSY.Size = new System.Drawing.Size(479, 148);
            this.tlpScan_ASSY.TabIndex = 45;
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.lblAssyVal);
            this.groupBox31.Controls.Add(this.label130);
            this.groupBox31.Controls.Add(this.lblAssyMax);
            this.groupBox31.Controls.Add(this.label129);
            this.groupBox31.Controls.Add(this.lblAssyRes);
            this.groupBox31.Controls.Add(this.lblAssyMin);
            this.groupBox31.Controls.Add(this.label131);
            this.groupBox31.Controls.Add(this.label132);
            this.groupBox31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox31.Location = new System.Drawing.Point(159, 2);
            this.groupBox31.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Padding = new System.Windows.Forms.Padding(2);
            this.tlpScan_ASSY.SetRowSpan(this.groupBox31, 2);
            this.groupBox31.Size = new System.Drawing.Size(318, 144);
            this.groupBox31.TabIndex = 47;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "Scan-ASSY监控区";
            // 
            // lblAssyVal
            // 
            this.lblAssyVal.AutoSize = true;
            this.lblAssyVal.Location = new System.Drawing.Point(75, 100);
            this.lblAssyVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAssyVal.Name = "lblAssyVal";
            this.lblAssyVal.Size = new System.Drawing.Size(55, 24);
            this.lblAssyVal.TabIndex = 0;
            this.lblAssyVal.Text = "value";
            // 
            // label130
            // 
            this.label130.AutoSize = true;
            this.label130.Location = new System.Drawing.Point(8, 100);
            this.label130.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label130.Name = "label130";
            this.label130.Size = new System.Drawing.Size(82, 24);
            this.label130.TabIndex = 2;
            this.label130.Text = "实际值：";
            // 
            // lblAssyMax
            // 
            this.lblAssyMax.AutoSize = true;
            this.lblAssyMax.Location = new System.Drawing.Point(75, 44);
            this.lblAssyMax.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAssyMax.Name = "lblAssyMax";
            this.lblAssyMax.Size = new System.Drawing.Size(46, 24);
            this.lblAssyMax.TabIndex = 0;
            this.lblAssyMax.Text = "max";
            // 
            // label129
            // 
            this.label129.AutoSize = true;
            this.label129.Location = new System.Drawing.Point(2, 44);
            this.label129.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label129.Name = "label129";
            this.label129.Size = new System.Drawing.Size(82, 24);
            this.label129.TabIndex = 1;
            this.label129.Text = "上限值：";
            // 
            // lblAssyRes
            // 
            this.lblAssyRes.AutoSize = true;
            this.lblAssyRes.Location = new System.Drawing.Point(246, 100);
            this.lblAssyRes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAssyRes.Name = "lblAssyRes";
            this.lblAssyRes.Size = new System.Drawing.Size(35, 24);
            this.lblAssyRes.TabIndex = 0;
            this.lblAssyRes.Text = "res";
            // 
            // lblAssyMin
            // 
            this.lblAssyMin.AutoSize = true;
            this.lblAssyMin.Location = new System.Drawing.Point(242, 44);
            this.lblAssyMin.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAssyMin.Name = "lblAssyMin";
            this.lblAssyMin.Size = new System.Drawing.Size(43, 24);
            this.lblAssyMin.TabIndex = 0;
            this.lblAssyMin.Text = "min";
            // 
            // label131
            // 
            this.label131.AutoSize = true;
            this.label131.Location = new System.Drawing.Point(171, 44);
            this.label131.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label131.Name = "label131";
            this.label131.Size = new System.Drawing.Size(82, 24);
            this.label131.TabIndex = 3;
            this.label131.Text = "下限值：";
            // 
            // label132
            // 
            this.label132.AutoSize = true;
            this.label132.Location = new System.Drawing.Point(189, 100);
            this.label132.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label132.Name = "label132";
            this.label132.Size = new System.Drawing.Size(64, 24);
            this.label132.TabIndex = 2;
            this.label132.Text = "结果：";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label66.Font = new System.Drawing.Font("SimSun", 15.2F, System.Drawing.FontStyle.Bold);
            this.label66.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label66.Location = new System.Drawing.Point(3, 119);
            this.label66.Margin = new System.Windows.Forms.Padding(3);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(151, 26);
            this.label66.TabIndex = 114;
            this.label66.Text = "Scan-ASSY";
            this.label66.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ASSY
            // 
            this.ASSY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ASSY.Font = new System.Drawing.Font("Microsoft YaHei", 22F, System.Drawing.FontStyle.Bold);
            this.ASSY.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.ASSY.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ASSY.Location = new System.Drawing.Point(2, 0);
            this.ASSY.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ASSY.Name = "ASSY";
            this.ASSY.Size = new System.Drawing.Size(153, 116);
            this.ASSY.TabIndex = 113;
            this.ASSY.Text = "██";
            this.ASSY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpTorqueMonitor
            // 
            this.tlpTorqueMonitor.ColumnCount = 4;
            this.tlpTorqueMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.65998F));
            this.tlpTorqueMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.92397F));
            this.tlpTorqueMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.70961F));
            this.tlpTorqueMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.81204F));
            this.tlpTorqueMonitor.Controls.Add(this.panelASSY, 0, 0);
            this.tlpTorqueMonitor.Controls.Add(this.panelAS, 1, 0);
            this.tlpTorqueMonitor.Controls.Add(this.panelTorqueMeter1, 2, 0);
            this.tlpTorqueMonitor.Controls.Add(this.panelTorqueMonitor2, 3, 0);
            this.tlpTorqueMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTorqueMonitor.Location = new System.Drawing.Point(0, 0);
            this.tlpTorqueMonitor.Name = "tlpTorqueMonitor";
            this.tlpTorqueMonitor.RowCount = 1;
            this.tlpTorqueMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTorqueMonitor.Size = new System.Drawing.Size(1894, 984);
            this.tlpTorqueMonitor.TabIndex = 48;
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.lblBaRes);
            this.groupBox32.Controls.Add(this.lblBaMin);
            this.groupBox32.Controls.Add(this.lblBaVal);
            this.groupBox32.Controls.Add(this.lblBaMax);
            this.groupBox32.Controls.Add(this.label135);
            this.groupBox32.Controls.Add(this.label133);
            this.groupBox32.Controls.Add(this.label136);
            this.groupBox32.Controls.Add(this.label134);
            this.groupBox32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox32.Location = new System.Drawing.Point(161, 2);
            this.groupBox32.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Padding = new System.Windows.Forms.Padding(2);
            this.tlpScrew_BA.SetRowSpan(this.groupBox32, 2);
            this.groupBox32.Size = new System.Drawing.Size(321, 144);
            this.groupBox32.TabIndex = 47;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Screw-BA监控区";
            // 
            // lblBaRes
            // 
            this.lblBaRes.AutoSize = true;
            this.lblBaRes.Location = new System.Drawing.Point(258, 100);
            this.lblBaRes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaRes.Name = "lblBaRes";
            this.lblBaRes.Size = new System.Drawing.Size(35, 24);
            this.lblBaRes.TabIndex = 0;
            this.lblBaRes.Text = "res";
            // 
            // lblBaMin
            // 
            this.lblBaMin.AutoSize = true;
            this.lblBaMin.Location = new System.Drawing.Point(258, 44);
            this.lblBaMin.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaMin.Name = "lblBaMin";
            this.lblBaMin.Size = new System.Drawing.Size(43, 24);
            this.lblBaMin.TabIndex = 0;
            this.lblBaMin.Text = "min";
            // 
            // lblBaVal
            // 
            this.lblBaVal.AutoSize = true;
            this.lblBaVal.Location = new System.Drawing.Point(85, 98);
            this.lblBaVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaVal.Name = "lblBaVal";
            this.lblBaVal.Size = new System.Drawing.Size(34, 24);
            this.lblBaVal.TabIndex = 0;
            this.lblBaVal.Text = "val";
            // 
            // lblBaMax
            // 
            this.lblBaMax.AutoSize = true;
            this.lblBaMax.Location = new System.Drawing.Point(84, 44);
            this.lblBaMax.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaMax.Name = "lblBaMax";
            this.lblBaMax.Size = new System.Drawing.Size(46, 24);
            this.lblBaMax.TabIndex = 0;
            this.lblBaMax.Text = "max";
            // 
            // label135
            // 
            this.label135.AutoSize = true;
            this.label135.Location = new System.Drawing.Point(188, 44);
            this.label135.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label135.Name = "label135";
            this.label135.Size = new System.Drawing.Size(82, 24);
            this.label135.TabIndex = 7;
            this.label135.Text = "下限值：";
            // 
            // label133
            // 
            this.label133.AutoSize = true;
            this.label133.Location = new System.Drawing.Point(12, 100);
            this.label133.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label133.Name = "label133";
            this.label133.Size = new System.Drawing.Size(82, 24);
            this.label133.TabIndex = 5;
            this.label133.Text = "实际值：";
            // 
            // label136
            // 
            this.label136.AutoSize = true;
            this.label136.Location = new System.Drawing.Point(202, 100);
            this.label136.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label136.Name = "label136";
            this.label136.Size = new System.Drawing.Size(64, 24);
            this.label136.TabIndex = 6;
            this.label136.Text = "结果：";
            // 
            // label134
            // 
            this.label134.AutoSize = true;
            this.label134.Location = new System.Drawing.Point(9, 44);
            this.label134.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label134.Name = "label134";
            this.label134.Size = new System.Drawing.Size(82, 24);
            this.label134.TabIndex = 4;
            this.label134.Text = "上限值：";
            // 
            // BA
            // 
            this.BA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BA.Font = new System.Drawing.Font("Microsoft YaHei", 22F);
            this.BA.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.BA.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BA.Location = new System.Drawing.Point(2, 0);
            this.BA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BA.Name = "BA";
            this.BA.Size = new System.Drawing.Size(155, 116);
            this.BA.TabIndex = 117;
            this.BA.Text = "██";
            this.BA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rtbBALog
            // 
            this.rtbBALog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbBALog.Location = new System.Drawing.Point(0, 148);
            this.rtbBALog.Margin = new System.Windows.Forms.Padding(2);
            this.rtbBALog.Name = "rtbBALog";
            this.rtbBALog.Size = new System.Drawing.Size(484, 830);
            this.rtbBALog.TabIndex = 46;
            this.rtbBALog.Text = "";
            // 
            // rtbASSYLog
            // 
            this.rtbASSYLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbASSYLog.Location = new System.Drawing.Point(0, 148);
            this.rtbASSYLog.Margin = new System.Windows.Forms.Padding(2);
            this.rtbASSYLog.Name = "rtbASSYLog";
            this.rtbASSYLog.Size = new System.Drawing.Size(479, 830);
            this.rtbASSYLog.TabIndex = 0;
            this.rtbASSYLog.Text = "";
            // 
            // tlpScrew_BA
            // 
            this.tlpScrew_BA.ColumnCount = 2;
            this.tlpScrew_BA.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.85421F));
            this.tlpScrew_BA.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.14579F));
            this.tlpScrew_BA.Controls.Add(this.label63, 0, 1);
            this.tlpScrew_BA.Controls.Add(this.BA, 0, 0);
            this.tlpScrew_BA.Controls.Add(this.groupBox32, 1, 0);
            this.tlpScrew_BA.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpScrew_BA.Location = new System.Drawing.Point(0, 0);
            this.tlpScrew_BA.Margin = new System.Windows.Forms.Padding(2);
            this.tlpScrew_BA.Name = "tlpScrew_BA";
            this.tlpScrew_BA.RowCount = 2;
            this.tlpScrew_BA.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpScrew_BA.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpScrew_BA.Size = new System.Drawing.Size(484, 148);
            this.tlpScrew_BA.TabIndex = 46;
            // 
            // panelAS
            // 
            this.panelAS.Controls.Add(this.rtbBALog);
            this.panelAS.Controls.Add(this.tlpScrew_BA);
            this.panelAS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAS.Location = new System.Drawing.Point(488, 3);
            this.panelAS.Name = "panelAS";
            this.panelAS.Size = new System.Drawing.Size(484, 978);
            this.panelAS.TabIndex = 47;
            // 
            // panelASSY
            // 
            this.panelASSY.Controls.Add(this.rtbASSYLog);
            this.panelASSY.Controls.Add(this.tlpScan_ASSY);
            this.panelASSY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelASSY.Location = new System.Drawing.Point(3, 3);
            this.panelASSY.Name = "panelASSY";
            this.panelASSY.Size = new System.Drawing.Size(479, 978);
            this.panelASSY.TabIndex = 48;
            // 
            // panelTorqueMeter1
            // 
            this.panelTorqueMeter1.Controls.Add(this.rtbTorqueMeter1);
            this.panelTorqueMeter1.Controls.Add(this.tlpTorqueMeter1);
            this.panelTorqueMeter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTorqueMeter1.Location = new System.Drawing.Point(978, 3);
            this.panelTorqueMeter1.Name = "panelTorqueMeter1";
            this.panelTorqueMeter1.Size = new System.Drawing.Size(461, 978);
            this.panelTorqueMeter1.TabIndex = 49;
            // 
            // tlpTorqueMeter1
            // 
            this.tlpTorqueMeter1.ColumnCount = 2;
            this.tlpTorqueMeter1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.85421F));
            this.tlpTorqueMeter1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.14579F));
            this.tlpTorqueMeter1.Controls.Add(this.lblTorque1, 0, 1);
            this.tlpTorqueMeter1.Controls.Add(this.lblSerialLight1, 0, 0);
            this.tlpTorqueMeter1.Controls.Add(this.grpTorqueMeter1, 1, 0);
            this.tlpTorqueMeter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpTorqueMeter1.Location = new System.Drawing.Point(0, 0);
            this.tlpTorqueMeter1.Margin = new System.Windows.Forms.Padding(2);
            this.tlpTorqueMeter1.Name = "tlpTorqueMeter1";
            this.tlpTorqueMeter1.RowCount = 2;
            this.tlpTorqueMeter1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTorqueMeter1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTorqueMeter1.Size = new System.Drawing.Size(461, 148);
            this.tlpTorqueMeter1.TabIndex = 47;
            // 
            // lblTorque1
            // 
            this.lblTorque1.AutoSize = true;
            this.lblTorque1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTorque1.Font = new System.Drawing.Font("SimSun", 15.2F, System.Drawing.FontStyle.Bold);
            this.lblTorque1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTorque1.Location = new System.Drawing.Point(3, 119);
            this.lblTorque1.Margin = new System.Windows.Forms.Padding(3);
            this.lblTorque1.Name = "lblTorque1";
            this.lblTorque1.Size = new System.Drawing.Size(145, 26);
            this.lblTorque1.TabIndex = 118;
            this.lblTorque1.Text = "扭力仪1";
            this.lblTorque1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSerialLight1
            // 
            this.lblSerialLight1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSerialLight1.Font = new System.Drawing.Font("Microsoft YaHei", 22F);
            this.lblSerialLight1.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblSerialLight1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSerialLight1.Location = new System.Drawing.Point(2, 0);
            this.lblSerialLight1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSerialLight1.Name = "lblSerialLight1";
            this.lblSerialLight1.Size = new System.Drawing.Size(147, 116);
            this.lblSerialLight1.TabIndex = 117;
            this.lblSerialLight1.Text = "██";
            this.lblSerialLight1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpTorqueMeter1
            // 
            this.grpTorqueMeter1.Controls.Add(this.label138);
            this.grpTorqueMeter1.Controls.Add(this.label140);
            this.grpTorqueMeter1.Controls.Add(this.label143);
            this.grpTorqueMeter1.Controls.Add(this.label144);
            this.grpTorqueMeter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTorqueMeter1.Location = new System.Drawing.Point(153, 2);
            this.grpTorqueMeter1.Margin = new System.Windows.Forms.Padding(2);
            this.grpTorqueMeter1.Name = "grpTorqueMeter1";
            this.grpTorqueMeter1.Padding = new System.Windows.Forms.Padding(2);
            this.tlpTorqueMeter1.SetRowSpan(this.grpTorqueMeter1, 2);
            this.grpTorqueMeter1.Size = new System.Drawing.Size(306, 144);
            this.grpTorqueMeter1.TabIndex = 47;
            this.grpTorqueMeter1.TabStop = false;
            this.grpTorqueMeter1.Text = "扭力仪点检1";
            // 
            // label138
            // 
            this.label138.AutoSize = true;
            this.label138.Location = new System.Drawing.Point(106, 100);
            this.label138.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label138.Name = "label138";
            this.label138.Size = new System.Drawing.Size(35, 24);
            this.label138.TabIndex = 0;
            this.label138.Text = "res";
            // 
            // label140
            // 
            this.label140.AutoSize = true;
            this.label140.Location = new System.Drawing.Point(104, 42);
            this.label140.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label140.Name = "label140";
            this.label140.Size = new System.Drawing.Size(34, 24);
            this.label140.TabIndex = 0;
            this.label140.Text = "val";
            // 
            // label143
            // 
            this.label143.AutoSize = true;
            this.label143.Location = new System.Drawing.Point(31, 44);
            this.label143.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label143.Name = "label143";
            this.label143.Size = new System.Drawing.Size(82, 24);
            this.label143.TabIndex = 5;
            this.label143.Text = "实际值：";
            // 
            // label144
            // 
            this.label144.AutoSize = true;
            this.label144.Location = new System.Drawing.Point(13, 100);
            this.label144.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label144.Name = "label144";
            this.label144.Size = new System.Drawing.Size(100, 24);
            this.label144.TabIndex = 6;
            this.label144.Text = "上传结果：";
            // 
            // rtbTorqueMeter1
            // 
            this.rtbTorqueMeter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbTorqueMeter1.Location = new System.Drawing.Point(0, 148);
            this.rtbTorqueMeter1.Margin = new System.Windows.Forms.Padding(2);
            this.rtbTorqueMeter1.Name = "rtbTorqueMeter1";
            this.rtbTorqueMeter1.Size = new System.Drawing.Size(461, 830);
            this.rtbTorqueMeter1.TabIndex = 48;
            this.rtbTorqueMeter1.Text = "";
            // 
            // panelTorqueMonitor2
            // 
            this.panelTorqueMonitor2.Controls.Add(this.rtbTorqueMeter2);
            this.panelTorqueMonitor2.Controls.Add(this.tlpTorqueMeter2);
            this.panelTorqueMonitor2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTorqueMonitor2.Location = new System.Drawing.Point(1445, 3);
            this.panelTorqueMonitor2.Name = "panelTorqueMonitor2";
            this.panelTorqueMonitor2.Size = new System.Drawing.Size(446, 978);
            this.panelTorqueMonitor2.TabIndex = 50;
            // 
            // tlpTorqueMeter2
            // 
            this.tlpTorqueMeter2.ColumnCount = 2;
            this.tlpTorqueMeter2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.85421F));
            this.tlpTorqueMeter2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.14579F));
            this.tlpTorqueMeter2.Controls.Add(this.lblTorque2, 0, 1);
            this.tlpTorqueMeter2.Controls.Add(this.lblSerialLight2, 0, 0);
            this.tlpTorqueMeter2.Controls.Add(this.grpTorqueMonitor2, 1, 0);
            this.tlpTorqueMeter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpTorqueMeter2.Location = new System.Drawing.Point(0, 0);
            this.tlpTorqueMeter2.Margin = new System.Windows.Forms.Padding(2);
            this.tlpTorqueMeter2.Name = "tlpTorqueMeter2";
            this.tlpTorqueMeter2.RowCount = 2;
            this.tlpTorqueMeter2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTorqueMeter2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTorqueMeter2.Size = new System.Drawing.Size(446, 148);
            this.tlpTorqueMeter2.TabIndex = 48;
            // 
            // lblTorque2
            // 
            this.lblTorque2.AutoSize = true;
            this.lblTorque2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTorque2.Font = new System.Drawing.Font("SimSun", 15.2F, System.Drawing.FontStyle.Bold);
            this.lblTorque2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTorque2.Location = new System.Drawing.Point(3, 119);
            this.lblTorque2.Margin = new System.Windows.Forms.Padding(3);
            this.lblTorque2.Name = "lblTorque2";
            this.lblTorque2.Size = new System.Drawing.Size(140, 26);
            this.lblTorque2.TabIndex = 118;
            this.lblTorque2.Text = "扭力仪2";
            this.lblTorque2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSerialLight2
            // 
            this.lblSerialLight2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSerialLight2.Font = new System.Drawing.Font("Microsoft YaHei", 22F);
            this.lblSerialLight2.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblSerialLight2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSerialLight2.Location = new System.Drawing.Point(2, 0);
            this.lblSerialLight2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSerialLight2.Name = "lblSerialLight2";
            this.lblSerialLight2.Size = new System.Drawing.Size(142, 116);
            this.lblSerialLight2.TabIndex = 117;
            this.lblSerialLight2.Text = "██";
            this.lblSerialLight2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpTorqueMonitor2
            // 
            this.grpTorqueMonitor2.Controls.Add(this.label139);
            this.grpTorqueMonitor2.Controls.Add(this.label141);
            this.grpTorqueMonitor2.Controls.Add(this.label137);
            this.grpTorqueMonitor2.Controls.Add(this.label11);
            this.grpTorqueMonitor2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTorqueMonitor2.Location = new System.Drawing.Point(148, 2);
            this.grpTorqueMonitor2.Margin = new System.Windows.Forms.Padding(2);
            this.grpTorqueMonitor2.Name = "grpTorqueMonitor2";
            this.grpTorqueMonitor2.Padding = new System.Windows.Forms.Padding(2);
            this.tlpTorqueMeter2.SetRowSpan(this.grpTorqueMonitor2, 2);
            this.grpTorqueMonitor2.Size = new System.Drawing.Size(296, 144);
            this.grpTorqueMonitor2.TabIndex = 47;
            this.grpTorqueMonitor2.TabStop = false;
            this.grpTorqueMonitor2.Text = "扭力仪点检2";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(23, 98);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 24);
            this.label11.TabIndex = 6;
            this.label11.Text = "上传结果：";
            // 
            // label137
            // 
            this.label137.AutoSize = true;
            this.label137.Location = new System.Drawing.Point(41, 42);
            this.label137.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label137.Name = "label137";
            this.label137.Size = new System.Drawing.Size(82, 24);
            this.label137.TabIndex = 5;
            this.label137.Text = "实际值：";
            // 
            // label139
            // 
            this.label139.AutoSize = true;
            this.label139.Location = new System.Drawing.Point(115, 43);
            this.label139.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label139.Name = "label139";
            this.label139.Size = new System.Drawing.Size(34, 24);
            this.label139.TabIndex = 0;
            this.label139.Text = "val";
            // 
            // label141
            // 
            this.label141.AutoSize = true;
            this.label141.Location = new System.Drawing.Point(116, 98);
            this.label141.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label141.Name = "label141";
            this.label141.Size = new System.Drawing.Size(35, 24);
            this.label141.TabIndex = 0;
            this.label141.Text = "res";
            // 
            // rtbTorqueMeter2
            // 
            this.rtbTorqueMeter2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbTorqueMeter2.Location = new System.Drawing.Point(0, 148);
            this.rtbTorqueMeter2.Margin = new System.Windows.Forms.Padding(2);
            this.rtbTorqueMeter2.Name = "rtbTorqueMeter2";
            this.rtbTorqueMeter2.Size = new System.Drawing.Size(446, 830);
            this.rtbTorqueMeter2.TabIndex = 49;
            this.rtbTorqueMeter2.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1902, 1032);
            this.Controls.Add(this.TabContorl);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设备数据采集系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.tabPage9.ResumeLayout(false);
            this.tableLayoutPanel13.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox21.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.tableLayoutPanel25.ResumeLayout(false);
            this.tableLayoutPanel25.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tableLayoutPanel32.ResumeLayout(false);
            this.tableLayoutPanel32.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeyArgs)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tableLayoutPanel31.ResumeLayout(false);
            this.tableLayoutPanel31.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrorPreserve)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tableLayoutPanel29.ResumeLayout(false);
            this.tableLayoutPanel29.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataAcquisition)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.tableLayoutPanel30.ResumeLayout(false);
            this.tableLayoutPanel30.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeviceDefects)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage10.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.tlpProductConfig.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel33.ResumeLayout(false);
            this.grpTorqueConfig.ResumeLayout(false);
            this.grpTorqueControllerConfig2.ResumeLayout(false);
            this.grpTorqueControllerConfig2.PerformLayout();
            this.grpTorqueControllerConfig1.ResumeLayout(false);
            this.grpTorqueControllerConfig1.PerformLayout();
            this.grpTorqueMeterConfig.ResumeLayout(false);
            this.grpTorqueMeterConfig.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrintDirectory)).EndInit();
            this.tabPage11.ResumeLayout(false);
            this.tabPage11.PerformLayout();
            this.groupBox30.ResumeLayout(false);
            this.tableLayoutPanel28.ResumeLayout(false);
            this.tableLayoutPanel28.PerformLayout();
            this.groupBox29.ResumeLayout(false);
            this.tableLayoutPanel27.ResumeLayout(false);
            this.tableLayoutPanel27.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.tableLayoutPanel23.ResumeLayout(false);
            this.tableLayoutPanel23.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.tableLayoutPanel22.ResumeLayout(false);
            this.tableLayoutPanel22.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.tableLayoutPanel21.ResumeLayout(false);
            this.tableLayoutPanel21.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            this.tableLayoutPanel20.ResumeLayout(false);
            this.tableLayoutPanel20.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.tableLayoutPanel19.ResumeLayout(false);
            this.tableLayoutPanel19.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.tableLayoutPanel18.ResumeLayout(false);
            this.tableLayoutPanel18.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.接口设置panel.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserInfo)).EndInit();
            this.panel6.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.系统设置panel3.ResumeLayout(false);
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            this.tableLayoutPanel14.ResumeLayout(false);
            this.panel20.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductModel)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel19.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.整页面.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl_UploadData.ResumeLayout(false);
            this.tabPageResult1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult1)).EndInit();
            this.tabPageResult2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult2)).EndInit();
            this.tabPageResult3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult3)).EndInit();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.ToolingNumberPanel.ResumeLayout(false);
            this.ToolingNumberPanel.PerformLayout();
            this.条码数据.ResumeLayout(false);
            this.tableLayoutPanel24.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel16.ResumeLayout(false);
            this.tableLayoutPanel16.PerformLayout();
            this.groupboxx.ResumeLayout(false);
            this.tableLayoutPanel17.ResumeLayout(false);
            this.TabContorl.ResumeLayout(false);
            this.tabPageTorqueMonitor.ResumeLayout(false);
            this.tlpScan_ASSY.ResumeLayout(false);
            this.tlpScan_ASSY.PerformLayout();
            this.groupBox31.ResumeLayout(false);
            this.groupBox31.PerformLayout();
            this.tlpTorqueMonitor.ResumeLayout(false);
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            this.tlpScrew_BA.ResumeLayout(false);
            this.tlpScrew_BA.PerformLayout();
            this.panelAS.ResumeLayout(false);
            this.panelASSY.ResumeLayout(false);
            this.panelTorqueMeter1.ResumeLayout(false);
            this.tlpTorqueMeter1.ResumeLayout(false);
            this.tlpTorqueMeter1.PerformLayout();
            this.grpTorqueMeter1.ResumeLayout(false);
            this.grpTorqueMeter1.PerformLayout();
            this.panelTorqueMonitor2.ResumeLayout(false);
            this.tlpTorqueMeter2.ResumeLayout(false);
            this.tlpTorqueMeter2.PerformLayout();
            this.grpTorqueMonitor2.ResumeLayout(false);
            this.grpTorqueMonitor2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private MySql.Data.MySqlClient.MySqlCommand mySqlCommand1;
        private TabPage tabPage9;
        private TableLayoutPanel tableLayoutPanel13;
        private GroupBox groupBox23;
        private RichTextBox PrinterSignal;
        private GroupBox groupBox21;
        private RichTextBox rtbReadBarCode;
        private GroupBox groupBox22;
        private RichTextBox UploadMes;
        private TabPage tabPage8;
        private TableLayoutPanel tableLayoutPanel6;
        private GroupBox groupBox15;
        private Button printTest;
        private Label label1;
        private Label label38;
        private TextBox printerName;
        private TextBox printTemplatePath;
        private Button printSetSave;
        private TableLayoutPanel tableLayoutPanel9;
        private GroupBox groupBox19;
        private Button btnSaveAtAssemblyMachine;
        private TextBox Device2;
        private TextBox Security2;
        private TextBox MesKey2;
        private TextBox Station2;
        private TextBox Process2;
        private TextBox Line2;
        private Label label58;
        private Label label57;
        private Label label56;
        private Label label55;
        private Label label53;
        private Label label42;
        private TabPage tabPage7;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox4;
        private DataGridView dgvKeyArgs;
        private Button keyArgsRefreshButton;
        private Button copyDataGatherTable;
        private GroupBox groupBox3;
        private DataGridView dgvErrorPreserve;
        private Button errorPreserveRefreshButton;
        private GroupBox groupBox10;
        private DataGridView dgvDataAcquisition;
        private Button dataGatherBoardRefreshButton;
        private GroupBox groupBox11;
        private DataGridView dgvDeviceDefects;
        private Button deviceDefectsRefreshButton;
        private Label label60;
        private TabPage tabPage6;
        private TableLayoutPanel tlpProductConfig;
        private TabPage tabPage5;
        private Panel 接口设置panel;
        private TableLayoutPanel tableLayoutPanel5;
        private TextBox Url_ToolingChange;
        private TextBox Url_RealtimeArgs;
        private TextBox Url_KeyArgs;
        private TextBox Url_ErrorInterface;
        private TextBox Url_DeviceStatus;
        private TextBox Url_Heartbeat;
        private TextBox Url_GetProductName;
        private TextBox Url_FTPMessGet;
        private TextBox Url_DataUpload;
        private TextBox Url_RouteCheck;
        private TextBox UrlPanelization;
        private Label label27;
        private TextBox url;
        private Label label28;
        private TextBox Line;
        private Label label39;
        private Label label41;
        private Label label43;
        private Label label45;
        private Label label46;
        private Label label47;
        private Label label48;
        private Label label49;
        private Label label50;
        private Label label51;
        private TextBox Process;
        private TextBox Station;
        private TextBox MesKey;
        private TextBox Security;
        private TextBox Device;
        private TextBox PlanNo;
        private TextBox FTPlog;
        private TextBox FTPPIC;
        private TextBox FTPID;
        private TextBox FTPCODE;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label12;
        private Label label13;
        private Label label15;
        private Label label25;
        private Label label29;
        private Label label31;
        private Label label33;
        private Label label34;
        private Label label35;
        private TextBox Url_Token;
        private Button btnSave_InterfaceConfig;
        private Label label36;
        private Label label37;
        private TextBox SWVer;
        private TextBox HWVer;
        private Label label23;
        private TextBox Url_PrintTemplate;
        private Label label40;
        private TabPage tabPage4;
        private Panel panel1;
        private DataGridView dgvUserInfo;
        private Panel panel6;
        private GroupBox groupBox6;
        private TextBox UPwd;
        private TextBox UId;
        private ComboBox Priv;
        private Label label14;
        private Button UserRefresh;
        private Label label20;
        private Label label18;
        private Button UserAdd;
        private TabPage tabPage3;
        private Panel 系统设置panel3;
        private GroupBox groupBox18;
        private TableLayoutPanel tableLayoutPanel14;
        private Panel panel20;
        private DataGridView dgvProductModel;
        private Panel panel5;
        private Button ImportFile;
        private Button changeTypeRefresh;
        private Label label16;
        private Panel panel19;
        private TableLayoutPanel tableLayoutPanel11;
        private GroupBox groupBox20;
        private Button SaveDataBase;
        private ComboBox deviceDataBase;
        private Label label59;
        private GroupBox groupBox5;
        private CheckBox PlcInputAutoSave;
        private ComboBox PlcConnectType;
        private Label label24;
        private Button ManualConnect;
        private Label label30;
        private Label label5;
        private TextBox PlcPort;
        private TextBox PlcIP;
        private GroupBox groupBox17;
        private TextBox DeviceName;
        private Label label8;
        private Button OtherSettingsSave;
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanel10;
        private GroupBox groupBox7;
        private RichTextBox rtbErrorLog;
        private TabPage tabPage1;
        private Panel 整页面;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;
        private Label InterfaceTipLabel;
        private Label DeviceStatusDisplay;
        private Label DeviceStatusSignalLight;
        private Label PlcTipLabel;
        private Label PlcSignalLight;
        private Label InterfaceSignalLight;
        private Label label54;
        private PictureBox pictureBox1;
        private TableLayoutPanel tableLayoutPanel12;
        private GroupBox groupBox2;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox txtProductModel;
        private Label label32;
        private Label label44;
        private Label label17;
        private Label label7;
        private TextBox txtTotalQuality;
        private Label label9;
        private TextBox txtOkQuality;
        private TextBox txtNgQuanlity;
        private Label label10;
        private Label label2;
        private TextBox txtYieldRate;
        private Panel panel4;
        private TextBox OrderNo;
        private Button ManualChangeMO;
        private TextBox OrderNum;
        private Label label80;
        private Panel panel7;
        private TextBox txtUser;
        private Button btnLogOut;
        private GroupBox groupBox8;
        private Label lblProductResult;
        private GroupBox ToolingNumberPanel;
        private Label ToolingNumber;
        private Label label71;
        private GroupBox 条码数据;
        private Label barCode;
        private Button btnManualInputBarcode;
        private GroupBox groupBox1;
        private Label label26;
        private Panel panel2;
        private Button ManualRecovery;
        private Label lblRunningStatus;
        private GroupBox groupboxx;
        private Label lblStatusErrorTip;
        private Button btnManualClear;
        private Button btnBlockMode;
        private TabControl TabContorl;
        private GroupBox groupBox16;
        private DataGridView dgvPrintDirectory;
        private Button printRefresh;
        private Label lblTips;
        private GroupBox groupBox13;
        private Label label19;
        private CheckBox EnableReportConfigParam;
        private Label label22;
        private CheckBox EnableReportRealTimeParam;
        private CheckBox EnableReportMachineAlarm;
        private Label label52;
        private TextBox RealtimeArgsUploadRate;
        private CheckBox EnableReportMachineStatus;
        private TextBox HeartbeatUploadRate;
        private TextBox BarcodeRule;
        private GroupBox groupBox12;
        private Label label21;
        private ComboBox cboProductMode;
        private CheckBox EnableTypeChangedVerify;
        private CheckBox EnablePrintCode;
        private CheckBox EnableResultUpload;
        private CheckBox EnableFluentVerify;
        private CheckBox EnableGetNextBoard;
        private CheckBox BanReadBarcode;
        private CheckBox EnableUpperTooling;
        private CheckBox EnableBarcodeRuleVerify;
        private Button ProductConfig_SaveButton;
        private TextBox LocalFilePath;
        private Button EndTask;
        private Button btnStartTask;
        private TabControl tabControl_UploadData;
        private TabPage tabPageResult1;
        private TabPage tabPageResult2;
        private TabPage tabPageResult3;
        private DataGridView dgvResult1;
        private GroupBox groupBox9;
        private Button button1;
        private TextBox Device3;
        private TextBox Security3;
        private TextBox MesKey3;
        private TextBox Station3;
        private TextBox Process3;
        private TextBox Line3;
        private TabControl tabControl1;
        private TabPage tabPage10;
        private TabPage tabPage11;
        private TableLayoutPanel tableLayoutPanel16;
        private TableLayoutPanel tableLayoutPanel17;
        private GroupBox groupBox14;
        private Label label68;
        private TableLayoutPanel tableLayoutPanel18;
        private Label label69;
        private Label label70;
        private Label label72;
        private Label label73;
        private Label label74;
        private Label label75;
        private Label label76;
        private Label label77;
        private Label label78;
        private TextBox txtHasBarcodeTag;
        private TextBox txtBarcodeVerifyTag;
        private TextBox txtBarcodeType;
        private TextBox txtPlcScanned;
        private TextBox txtScannedLength;
        private TextBox txtPanalizationBarcode;
        private TextBox txtPanalizationLength;
        private TextBox txtManualInput;
        private TextBox txtManualBarcode;
        private TextBox txtManualLength;
        private Button button2;
        private GroupBox groupBox24;
        private TableLayoutPanel tableLayoutPanel19;
        private Label label79;
        private Label label81;
        private Label label82;
        private Label label83;
        private TextBox txtTriggerUpload1;
        private TextBox txtFeedback1;
        private TextBox txtProductResult1;
        private TextBox txtBarcodeToUpload1;
        private Label label84;
        private TextBox txtBarcodeToUploadLength1;
        private GroupBox groupBox25;
        private TableLayoutPanel tableLayoutPanel20;
        private Label label85;
        private Label label86;
        private Label label87;
        private Label label88;
        private TextBox txtTriggerUpload2;
        private TextBox txtFeedback2;
        private TextBox txtProductResult2;
        private TextBox txtBarcodeToUpload2;
        private Label label89;
        private TextBox txtBarcodeToUploadLength2;
        private GroupBox groupBox26;
        private TableLayoutPanel tableLayoutPanel21;
        private Label label90;
        private Label label91;
        private Label label92;
        private Label label93;
        private TextBox txtTriggerUpload3;
        private TextBox txtFeedback3;
        private TextBox txtProductResult3;
        private TextBox txtBarcodeToUpload3;
        private Label label94;
        private TextBox txtBarcodeToUploadLength3;
        private GroupBox groupBox27;
        private TableLayoutPanel tableLayoutPanel22;
        private Label label95;
        private Label label96;
        private Label label98;
        private TextBox txtPrintTrigger;
        private TextBox txtPrintFeedback;
        private TextBox txtBarcodeToPrint;
        private Label label99;
        private TextBox txtBarcodeToPrintLength;
        private GroupBox groupBox28;
        private TableLayoutPanel tableLayoutPanel23;
        private Label label97;
        private Label label100;
        private Label label101;
        private Label label102;
        private TextBox txtGoodsProducts;
        private TextBox txtProduceCount;
        private TextBox txtDeviceProgramName;
        private TextBox txtProductType;
        private Label label103;
        private TextBox txtProductTypeLength;
        private Label label104;
        private TextBox txtBarcodeRule;
        private Label label105;
        private TextBox txtBarcodeRuleLength;
        private Label label106;
        private TextBox txtModelSwitch;
        private Label label107;
        private TextBox txtPlcHeartBeat;
        private Label label108;
        private TextBox txtPcHeartBeat;
        private TextBox txtNotGoodsProducts;
        private Label label109;
        private TextBox txtDeviceStatus;
        private Label label110;
        private TextBox txtProgramNameLength;
        private Label label111;
        private TextBox txtContinueProduce;
        private Label label112;
        private Label label113;
        private TextBox txtRecoverySignal;
        private Button btnChangePath;
        private Button btnShowPath;
        private TableLayoutPanel tableLayoutPanel24;
        private TableLayoutPanel tableLayoutPanel25;
        private Label label114;
        private Label label119;
        private Label label115;
        private Label label116;
        private Label label118;
        private Label label117;
        private ComboBox cboEnforcePass;
        private Label label62;
        private TabPage tabPageTorqueMonitor;
        private RichTextBox rtbASSYLog;
        private TableLayoutPanel tlpScan_ASSY;
        private Label label63;
        private Label label66;
        private Label ASSY;
        private Label BA;
        private GroupBox groupBox29;
        private TableLayoutPanel tableLayoutPanel27;
        private Label label64;
        private TextBox txtTorqueResult1;
        private Label label65;
        private TextBox txtTorqueValue1;
        private Label label67;
        private TextBox txtToqueMax1;
        private Label label120;
        private TextBox txtRequest1;
        private Label label121;
        private TextBox txtAcknowledge1;
        private Label label122;
        private GroupBox groupBox30;
        private TableLayoutPanel tableLayoutPanel28;
        private TextBox txtToqueMin3;
        private Label label123;
        private Label label124;
        private TextBox txtTorqueResult3;
        private Label label125;
        private TextBox txtTorqueValue3;
        private Label label126;
        private TextBox txtToqueMax3;
        private Label label127;
        private TextBox txtRequest3;
        private Label label128;
        private TextBox txtAcknowledge3;
        private TextBox txtToqueMin1;
        private RichTextBox rtbBALog;
        private GroupBox groupBox32;
        private GroupBox groupBox31;
        private Label lblAssyVal;
        private Label lblAssyRes;
        private Label lblAssyMin;
        private Label lblAssyMax;
        private Label lblBaRes;
        private Label lblBaMin;
        private Label lblBaVal;
        private Label lblBaMax;
        private Label label131;
        private Label label130;
        private Label label129;
        private Label label135;
        private Label label133;
        private Label label136;
        private Label label134;
        private Label label132;
        private DataGridView dgvResult2;
        private DataGridView dgvResult3;
        private TableLayoutPanel tableLayoutPanel29;
        private Button btnSave_dgvDataAcquisition;
        private ComboBox cboBanUpload;
        private Label label61;
        private CheckBox chkBanFixtureUpload;
        private Panel panel8;
        private TableLayoutPanel tableLayoutPanel30;
        private Button btnSave_dgvDefect;
        private TableLayoutPanel tableLayoutPanel32;
        private TableLayoutPanel tableLayoutPanel31;
        private Button btnSave_WarmError;
        private Button btnSave_KeyArgs;
        private Panel panel3;
        private GroupBox grpTorqueMeterConfig;
        private ComboBox cmbCOM2;
        private Label lblCOM2;
        private ComboBox cmbCOM1;
        private Label lblCOM1;
        private TableLayoutPanel tableLayoutPanel33;
        private Button btnRefresh;
        private GroupBox grpTorqueConfig;
        private GroupBox grpTorqueControllerConfig2;
        private TextBox txtControllerPort2;
        private TextBox txtControllerIP2;
        private Label lblIP2;
        private Label lblPort2;
        private GroupBox grpTorqueControllerConfig1;
        private TextBox txtControllerPort1;
        private TextBox txtControllerIP1;
        private Label lblPort1;
        private Label lblIP1;
        private Panel panel9;
        private TableLayoutPanel tlpTorqueMonitor;
        private Panel panelAS;
        private Panel panelASSY;
        private TableLayoutPanel tlpScrew_BA;
        private Panel panelTorqueMeter1;
        private RichTextBox rtbTorqueMeter1;
        private TableLayoutPanel tlpTorqueMeter1;
        private Label lblTorque1;
        private Label lblSerialLight1;
        private GroupBox grpTorqueMeter1;
        private Label label138;
        private Label label140;
        private Label label143;
        private Label label144;
        private Panel panelTorqueMonitor2;
        private RichTextBox rtbTorqueMeter2;
        private TableLayoutPanel tlpTorqueMeter2;
        private Label lblTorque2;
        private Label lblSerialLight2;
        private GroupBox grpTorqueMonitor2;
        private Label label141;
        private Label label137;
        private Label label139;
        private Label label11;
    }
}

