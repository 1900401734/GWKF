using SqlSugar;

namespace MesDatas.DataAcess
{
    [SugarTable("SystemInfo")]

    public class SystemInfo
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; private set; }

        /// <summary>
        /// 条码规则
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeRule", IsNullable = true, ColumnDescription = "条码规则")]
        public string BarcodeRule { get; set; }

        /// <summary>
        /// 心跳频率
        /// </summary>
        [SugarColumn(ColumnName = "HeartRate", IsNullable = true, ColumnDescription = "心跳频率（单位：s)")]
        public string HeartRate { get; set; }

        /// <summary>
        /// 勾选启用设备状态上传
        /// </summary>
        [SugarColumn(ColumnName = "ReportMachineStatus", IsNullable = true, ColumnDescription = "勾选启用设备状态上传")]
        public bool ReportMachineStatus { get; set; }

        /// <summary>
        /// 预警信息上传
        /// </summary>
        [SugarColumn(ColumnName = "ReportMachineAlarm", IsNullable = true, ColumnDescription = "勾选启用预警信息上传")]
        public bool ReportMachineAlarm { get; set; }

        /// <summary>
        /// 实时参数上传
        /// </summary>
        [SugarColumn(ColumnName = "ReportRealTimeParam", IsNullable = true, ColumnDescription = "勾选启用实时参数上传")]
        public bool ReportRealTimeParam { get; set; }

        /// <summary>
        /// 实时参数上传频率
        /// </summary>
        [SugarColumn(ColumnName = "RealTimeParamRate", IsNullable = true, ColumnDescription = "实时参数上次频率（单位：s）")]
        public string RealTimeParamRate { get; set; }

        /// <summary>
        /// 关键参数上传
        /// </summary>
        [SugarColumn(ColumnName = "ReportConfigParam", IsNullable = true, ColumnDescription = "勾选启用关键参数上传")]
        public bool ReportConfigParam { get; set; }

        /// <summary>
        /// 勾选屏蔽条码读取
        /// </summary>
        [SugarColumn(ColumnName = "BanBarcodeFetch", IsNullable = true, ColumnDescription = "勾选屏蔽条码读取")]
        public bool BanBarcodeFetch { get; set; } = false;

        /// <summary>
        /// 勾选启用型号切换校验
        /// </summary>
        [SugarColumn(ColumnName = "EnableModelVerify", IsNullable = true, ColumnDescription = "勾选启用型号切换校验")]
        public bool EnableModelVerify { get; set; } = false;

        /// <summary>
        /// 勾选启用条码规则验证
        /// </summary>
        [SugarColumn(ColumnName = "EnableBarcodeRuleVarify", IsNullable = true, ColumnDescription = "勾选启用条码规则验证")]
        public bool EnableBarcodeRuleVarify { get; set; } = false;

        /// <summary>
        /// 勾选启用获取拼版
        /// </summary>
        [SugarColumn(ColumnName = "EnablePanelizationFetch", IsNullable = true, ColumnDescription = "勾选启用获取拼版")]
        public bool EnablePanelizationFetch { get; set; } = false;

        /// <summary>
        /// 勾选启用流程验证
        /// </summary>
        [SugarColumn(ColumnName = "EnableRouteCheck", IsNullable = true, ColumnDescription = "勾选启用流程验证")]
        public bool EnableRouteCheck { get; set; } = true;

        /// <summary>
        /// 勾选启用上传结果
        /// </summary>
        [SugarColumn(ColumnName = "EnableDataUpload", IsNullable = true, ColumnDescription = "勾选启用上传结果")]
        public bool EnableDataUpload { get; set; } = true;

        /// <summary>
        /// 勾选启用打印模板
        /// </summary>
        [SugarColumn(ColumnName = "EnablePrint", IsNullable = true, ColumnDescription = "勾选启用打印模板")]
        public bool EnablePrint { get; set; } = false;

        /// <summary>
        /// 勾选启用工装机程序
        /// </summary>
        [SugarColumn(ColumnName = "EnableFixtureMachine", IsNullable = true, ColumnDescription = "勾选启用工装机程序")]
        public bool EnableFixtureMachine { get; set; } = false;

        /// <summary>
        /// 勾选屏蔽工装编号上传
        /// </summary>
        [SugarColumn(ColumnName = "BanFixtureUpload", IsNullable = true, ColumnDescription = "勾选屏蔽工装编号上传")]
        public bool BanFixtureUpload { get; set; } = false;

        /// <summary>
        /// 产品过站模式
        /// </summary>
        [SugarColumn(ColumnName = "ProductMode", IsNullable = true, ColumnDescription = "产品过站模式")]
        public string ProductMode { get; set; } = "显示NG且阻塞";

        /// <summary>
        /// MES过站接口超时时间，单位：秒。
        /// </summary>
        [SugarColumn(ColumnName = "MesSaveResultTimeoutSeconds", IsNullable = true, ColumnDescription = "MES过站接口超时时间（单位：s）")]
        public string MesSaveResultTimeoutSeconds { get; set; } = "30";

        /// <summary>
        /// PLC接收扭力结果超时后的处理方式。
        /// <para>当前策略：超时清Req并报警，避免本次转发继续占用握手位。</para>
        /// </summary>
        [SugarColumn(ColumnName = "TorqueAckTimeoutMode", IsNullable = true, ColumnDescription = "PLC接收扭力结果超时处理方式")]
        public string TorqueAckTimeoutMode { get; set; } = "超时清Req并报警";

        /// <summary>
        /// PLC接收扭力ACK超时时间，单位：秒。
        /// </summary>
        [SugarColumn(ColumnName = "TorqueAckTimeoutSeconds", IsNullable = true, ColumnDescription = "PLC接收扭力ACK超时时间（单位：s）")]
        public string TorqueAckTimeoutSeconds { get; set; } = "3";

        /// <summary>
        /// 强制过站
        /// </summary>
        [SugarColumn(ColumnName = "EnforcePass", IsNullable = true, ColumnDescription = "强制过站")]
        public string EnforcePass { get; set; } = "None";

        /// <summary>
        /// 屏蔽数据上传
        /// </summary>
        [SugarColumn(ColumnName = "BanUpload", IsNullable = true, ColumnDescription = "屏蔽数据上传")]
        public string BanUpload { get; set; } = "None";

        /// <summary>
        /// 扭力仪串口号
        /// </summary>
        [SugarColumn(ColumnName = "SerialPort1", IsNullable = true, ColumnDescription = "扭力仪串口号1")]
        public string SerialPort1 { get; set; }

        /// <summary>
        /// 扭力仪串口号
        /// </summary>
        [SugarColumn(ColumnName = "SerialPort2", IsNullable = true, ColumnDescription = "扭力仪串口号2")]
        public string SerialPort2 { get; set; }

        /// <summary>
        /// 扭力控制器IP
        /// </summary>
        [SugarColumn(ColumnName = "ControllerIP1", IsNullable = true, ColumnDescription = "扭力控制器IP1")]
        public string ControllerIP1 { get; set; } = "192.168.1.31";

        /// <summary>
        /// 扭力控制器端口号
        /// </summary>
        [SugarColumn(ColumnName = "ControllerPort1", IsNullable = true, ColumnDescription = "扭力控制器端口号1")]
        public string ControllerPort1 { get; set; } = "4545";

        /// <summary>
        /// 扭力控制器IP
        /// </summary>
        [SugarColumn(ColumnName = "ControllerIP3", IsNullable = true, ColumnDescription = "扭力控制器IP2")]
        public string ControllerIP2 { get; set; } = "192.168.1.32";

        /// <summary>
        /// 扭力控制器端口号
        /// </summary>
        [SugarColumn(ColumnName = "ControllerPort3", IsNullable = true, ColumnDescription = "扭力控制器端口号1")]
        public string ControllerPort2 { get; set; } = "4545";

        public static SystemInfo Initalize()
        {
            SystemInfo p = new SystemInfo();
            p.ID = 1;
            return p;
        }
        public bool Save()
        {
            return SystemInfoServer.SaveSystemInfo(this);
        }
        public string Update()
        {
            return SystemInfoServer.UpdateSystemInfo(this);
        }
        public string Delete()
        {
            return SystemInfoServer.DeleteSystemInfo(this);
        }
    }
}

