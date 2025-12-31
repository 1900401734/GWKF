using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MesDatas.DataAcess
{
    [SugarTable("PlcAddressInfo")]

    public class PlcAddressInfo
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        #region 条码验证（10个）

        // ---- 条码验证基本信息（3个）-----

        /// <summary>
        /// D7000,Int16 有无条码标志
        /// </summary>
        [SugarColumn(ColumnName = "HasBarcodeTag", IsNullable = true)]
        public string HasBarcodeTag { get; set; } = "D7000";

        /// <summary>
        /// D7001,Int16 条码验证标识
        /// 1=验证成功，2=验证失败
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeVerifyTag", IsNullable = true)]
        public string BarcodeVerifyTag { get; set; } = "D7001";

        /// <summary>
        ///  D7059,Int16 条码类型
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeType", IsNullable = true)]
        public string BarcodeType { get; set; } = "D7059";

        // ----- PLC与上位机条码传输相关（4个） -----

        /// <summary>
        ///  D7061 PLC->PC 读取到的产品条码（PCB板条码）
        /// </summary>
        [SugarColumn(ColumnName = "PlcScannedBarcode", IsNullable = true)]
        public string PlcScannedBarcode { get; set; } = "D7061";

        /// <summary>
        /// PLC->PC 读取到的产品条码（PCB板条码）
        /// </summary>
        [SugarColumn(ColumnName = "PlcScannedBarcodeLength", IsNullable = true)]
        public string PlcScannedBarcodeLength { get; set; } = "17";

        /// <summary>
        /// D7130PC->PLC 拼版中的另一块产品的条码(string)
        /// </summary>
        [SugarColumn(ColumnName = "PanalizationBarcode", IsNullable = true)]
        public string PanalizationBarcode { get; set; } = "D7130";

        /// <summary>
        /// PC->PLC 拼版中的另一块产品的条码(string)
        /// </summary>
        [SugarColumn(ColumnName = "PanalizationBarcodeLength", IsNullable = true)]
        public string PanalizationBarcodeLength { get; set; } = "17";

        // ----- 手动输入条码相关（3个） -----

        /// <summary>
        /// 手动输入条码标志
        /// </summary>
        [SugarColumn(ColumnName = "ManualInputBarcodeTip", IsNullable = true)]
        public string ManualInputBarcodeTip { get; set; } = "D7516";

        /// <summary>
        /// D7518 PC->PLC 拼版中的另一块产品的条码(string)
        /// </summary>
        [SugarColumn(ColumnName = "ManualInputBarcode", IsNullable = true)]
        public string ManualInputBarcode { get; set; } = "D7518";

        /// <summary>
        /// PC->PLC 拼版中的另一块产品的条码(string)
        /// </summary>
        [SugarColumn(ColumnName = "ManualInputBarcodeLength", IsNullable = true)]
        public string ManualInputBarcodeLength { get; set; } = "17";

        #endregion

        #region 产品过站（15个）

        // ----------- 工序1过站 -----------

        /// <summary>
        /// D7115,Int16 PLC触发上位机上传数据
        /// </summary>
        [SugarColumn(ColumnName = "TriggerUpload1", IsNullable = true)]
        public string TriggerUpload1 { get; set; } = "D7115";

        /// <summary>
        /// D7116,Int16 数据上传完成反馈给PLC
        /// </summary>
        [SugarColumn(ColumnName = "Feedback1", IsNullable = true)]
        public string Feedback1 { get; set; } = "D7116";

        /// <summary>
        /// D7119,Int16 产品结果
        /// </summary>
        [SugarColumn(ColumnName = "ProductResult1", IsNullable = true)]
        public string ProductResult1 { get; set; } = "D7119";

        /// <summary>
        /// D7149,17 读取当前工序数据上传时所需的条码
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToUpload1", IsNullable = true)]
        public string BarcodeToUpload1 { get; set; } = "D7149";

        /// <summary>
        /// 17 读取当前工序数据上传时所需的条码
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToUpload1Length", IsNullable = true)]
        public string BarcodeToUploadLength1 { get; set; } = "17";

        // ----------- 工序2过站 -----------

        /// <summary>
        /// D7115,Int16 PLC触发上位机上传数据
        /// </summary>
        [SugarColumn(ColumnName = "TriggerUpload2", IsNullable = true)]
        public string TriggerUpload2 { get; set; } = "D7490";

        /// <summary>
        /// D7116,Int16 数据上传完成反馈给PLC
        /// </summary>
        [SugarColumn(ColumnName = "Feedback2", IsNullable = true)]
        public string Feedback2 { get; set; } = "D7491";

        /// <summary>
        /// D7119,Int16 产品结果
        /// </summary>
        [SugarColumn(ColumnName = "ProductResult2", IsNullable = true)]
        public string ProductResult2 { get; set; } = "D7492";

        /// <summary>
        /// D7149,17 读取当前工序数据上传时所需的条码
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToUpload2", IsNullable = true)]
        public string BarcodeToUpload2 { get; set; } = "D7493";

        /// <summary>
        /// 17 读取当前工序数据上传时所需的条码
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToUpload1Length2", IsNullable = true)]
        public string BarcodeToUploadLength2 { get; set; } = "17";

        // ----------- 工序3过站 -----------

        /// <summary>
        /// D7115,Int16 PLC触发上位机上传数据
        /// </summary>
        [SugarColumn(ColumnName = "TriggerUpload3", IsNullable = true)]
        public string TriggerUpload3 { get; set; } = "D7600";

        /// <summary>
        /// D7116,Int16 数据上传完成反馈给PLC
        /// </summary>
        [SugarColumn(ColumnName = "Feedback3", IsNullable = true)]
        public string Feedback3 { get; set; } = "D7601";

        /// <summary>
        /// D7119,Int16 产品结果
        /// </summary>
        [SugarColumn(ColumnName = "ProductResult3", IsNullable = true)]
        public string ProductResult3 { get; set; } = "D7602";

        /// <summary>
        /// D7149,17 读取当前工序数据上传时所需的条码
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToUpload3", IsNullable = true)]
        public string BarcodeToUpload3 { get; set; } = "D7603";

        /// <summary>
        /// 17 读取当前工序数据上传时所需的条码
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToUpload1Length3", IsNullable = true)]
        public string BarcodeToUploadLength3 { get; set; } = "17";

        #endregion

        #region 打印条码

        /// <summary>
        /// D7111,Int16 PLC->PC 触发上位机进入打印流程
        /// 1=开始打印 2=停止打印（当工序2进行产品过站失败后，通过该地址通知PLC取消本次产品的打印触发信号）
        /// </summary>
        [SugarColumn(ColumnName = "PrintTrigger", IsNullable = true)]
        public string PrintTrigger { get; set; } = "D7111";

        /// <summary>
        /// D7112,Int16 PC->PLC 
        /// <para> 1=打印成功，2=打印失败</para>
        /// </summary>
        [SugarColumn(ColumnName = "PrintFeedback", IsNullable = true)]
        public string PrintFeedback { get; set; } = "D7112";

        /// <summary>
        /// D7430，17 获取需要上传到MES进行打印请求的条码
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToPrint", IsNullable = true)]
        public string BarcodeToPrint { get; set; } = "D7430";

        /// <summary>
        /// 条码长度
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeToPrintLenght", IsNullable = true)]
        public string BarcodeToPrintLenght { get; set; } = "17";

        #endregion

        #region 设备参数

        // ----- 生产指标（3个） -----

        /// <summary>
        /// D7010 良品数(Int32)
        /// </summary>
        [SugarColumn(ColumnName = "GoodsProducts", IsNullable = true)]
        public string GoodsProducts { get; set; } = "D7010";

        /// <summary>
        /// D7012 不良数(Int32)
        /// </summary>
        [SugarColumn(ColumnName = "NotGoodsProducts", IsNullable = true)]
        public string NotGoodsProducts { get; set; } = "D7012";

        /// <summary>
        /// D7014 生产总数(Int32)
        /// </summary>
        [SugarColumn(ColumnName = "ProduceCount", IsNullable = true)]
        public string ProduceCount { get; set; } = "D7014";

        // ----- 设备信息（3个） -----

        /// <summary>
        /// D7002 设备状态(Int16)
        /// </summary>
        [SugarColumn(ColumnName = "DeviceStatus", IsNullable = true)]
        public string DeviceStatus { get; set; } = "D7002";

        /// <summary>
        /// D7080,10 设备使用的程序名(字符串)
        /// </summary>
        [SugarColumn(ColumnName = "DeviceProgramName", IsNullable = true)]
        public string DeviceProgramName { get; set; } = "D7080";

        /// <summary>
        /// D7080,10 设备使用的程序名(字符串)
        /// </summary>
        [SugarColumn(ColumnName = "ProgramNameLength", IsNullable = true)]
        public string ProgramNameLength { get; set; } = "10";

        // ---- 产品信息（4个） -----

        /// <summary>
        /// D7020,20 产品型号(string)
        /// </summary>
        [SugarColumn(ColumnName = "ProductType", IsNullable = true)]
        public string ProductType { get; set; } = "D7020";

        /// <summary>
        /// D7020,20 产品型号(string)
        /// </summary>
        [SugarColumn(ColumnName = "ProductTypeLength", IsNullable = true)]
        public string ProductTypeLength { get; set; } = "20";

        /// <summary>
        /// D7042,15 条码规则(string)
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeRule", IsNullable = true)]
        public string BarcodeRule { get; set; } = "D7042";

        /// <summary>
        /// D7042,15 条码规则(string)
        /// </summary>
        [SugarColumn(ColumnName = "BarcodeRuleLength", IsNullable = true)]
        public string BarcodeRuleLength { get; set; } = "15";

        #endregion

        #region 产品换型（2个）

        /// <summary>
        /// D7004 型号切换(Int16) PLC通知PC切换型号，1=需要切换型号
        /// </summary>
        [SugarColumn(ColumnName = "ModelSwitch", IsNullable = true)]
        public string ModelSwitch { get; set; } = "D7004";

        /// <summary>
        /// D7003 工单切换完成后通知PLC继续生产(Int16)
        /// </summary>
        [SugarColumn(ColumnName = "ContinueProduce", IsNullable = true)]
        public string ContinueProduce { get; set; } = "D7003";

        #endregion

        #region 心跳管理

        /// <summary>
        /// D7107 PLC每隔1秒钟写入1次心跳信号到D7107
        /// </summary>
        [SugarColumn(ColumnName = "PlcHeartBeat", IsNullable = true)]
        public string PlcHeartBeat { get; set; } = "D7107";

        /// <summary>
        /// D7108 PC每隔1秒钟写入1次心跳信号到D7108
        /// </summary>
        [SugarColumn(ColumnName = "PcHeartBeat", IsNullable = true)]
        public string PcHeartBeat { get; set; } = "D7108";

        #endregion

        #region 复位信号

        /// <summary>
        /// D7514,Int16 复位信号
        /// <para>1=需要复位</para>
        /// </summary>
        [SugarColumn(ColumnName = "RecoverySignal", IsNullable = true)]
        public string RecoverySignal { get; set; } = "D7514";

        #endregion

        #region 扭力数据转发

        // 工序1：Scan-ASSY

        /// <summary>
        /// D7620,Int32 扭力实际值1
        /// </summary>
        [SugarColumn(ColumnName = "TorqueValue1", IsNullable = true)]
        public string TorqueValue1 { get; set; } = "D7620";

        /// <summary>
        /// D7622,Int32 扭力上限值1
        /// </summary>
        [SugarColumn(ColumnName = "TorqueMax1", IsNullable = true)]
        public string TorqueMax1 { get; set; } = "D7622";

        /// <summary>
        /// D7622,Int32 扭力下限值1
        /// </summary>
        [SugarColumn(ColumnName = "TorqueMin1", IsNullable = true)]
        public string TorqueMin1 { get; set; } = "D7624";

        /// <summary>
        /// D7622,Int32 扭力结果1
        /// </summary>
        [SugarColumn(ColumnName = "TorqueResult1", IsNullable = true)]
        public string TorqueResult1 { get; set; } = "D7626";

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "Request1", IsNullable = true)]
        public string Request1 { get; set; } = "D7628";

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "Acknowledge1", IsNullable = true)]
        public string Acknowledge1 { get; set; } = "D7629";

        // 工序3：Screw-BA

        /// <summary>
        /// D7630,Int32 扭力实际值3
        /// </summary>
        [SugarColumn(ColumnName = "TorqueValue3", IsNullable = true)]
        public string TorqueValue3 { get; set; } = "D7630";

        /// <summary>
        /// D7622,Int32 扭力上限值1
        /// </summary>
        [SugarColumn(ColumnName = "TorqueMax3", IsNullable = true)]
        public string TorqueMax3 { get; set; } = "D7632";

        /// <summary>
        /// D7622,Int32 扭力上限值1
        /// </summary>
        [SugarColumn(ColumnName = "TorqueMin3", IsNullable = true)]
        public string TorqueMin3 { get; set; } = "D7634";

        /// <summary>
        /// D7622,Int32 扭力上限值1
        /// </summary>
        [SugarColumn(ColumnName = "TorqueResult3", IsNullable = true)]
        public string TorqueResult3 { get; set; } = "D7636";

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "Request3", IsNullable = true)]
        public string Request3 { get; set; } = "D7638";

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "Acknowledge3", IsNullable = true)]
        public string Acknowledge3 { get; set; } = "D7639";

        #endregion

        public static PlcAddressInfo DeviceInformationInitalize()
        {
            PlcAddressInfo p = new PlcAddressInfo();
            p.ID = 1;
            return p;
        }
        public string Save()
        {
            return PlcAddressServer.GetPlcAddressInfoSave(this);
        }
        public string Update()
        {
            return PlcAddressServer.GetPlcAddressInfoUpdate(this);
        }
        public string Delete()
        {
            return PlcAddressServer.GetPlcAddressInfoDelete(this);
        }
    }
}
