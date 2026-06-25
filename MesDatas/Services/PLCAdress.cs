namespace MesDatas.Services
{
    public class AddressCombine
    {
        public string Address { get; set; }

        public ushort Length { get; set; }
    }

    public class AddressRange
    {
        public string PLC { get; set; }

        public string PC { get; set; }
    }

    public class UpplerAndLower
    {
        /// <summary>
        /// upper to Down - 上位机给下位机
        /// <para>1=开始打印，2=取消打印</para>
        /// </summary>
        public string FeedbackPrint { get; set; }

        /// <summary>
        /// Down to Upper - 下位机给上位机
        /// <pqra>1=触发打印</pqra>
        /// </summary>
        public string TriggerPrint { get; set; }
    }

    public class Args
    {
        public AddressCombine Name { get; set; }        // 参数名（浮点）

        public AddressCombine Standard { get; set; }    // 参数值（浮点）

        public AddressCombine Upper { get; set; }       // 参数上限（浮点）

        public AddressCombine Lower { get; set; }       // 参数下限（浮点）

        public AddressCombine Unit { get; set; }        // 参数单位（浮点）
    }

    public class PLCAdress
    {
        #region ----- 线程信号（13个） -----

        #region 线程：实时读取设备参数（7个）

        // ----- 生产指标（3个） -----

        /// <summary>
        /// D7010 良品数(Int32)
        /// </summary>
        public AddressCombine GoodsProducts { get; } = new AddressCombine { Address = "D7010", Length = 2 };

        /// <summary>
        /// D7012 不良数(Int32)
        /// </summary>
        public AddressCombine NotGoodsProducts { get; } = new AddressCombine { Address = "D7012", Length = 2 };

        /// <summary>
        /// D7014 生产总数(Int32)
        /// </summary>
        public AddressCombine ProduceCount { get; } = new AddressCombine { Address = "D7014", Length = 2 };

        // ----- 设备信息（2个） -----

        /// <summary>
        /// D7002 设备状态(Int16)
        /// </summary>
        public string DeviceStatus { get; } = "D7002";

        /// <summary>
        ///  D7080,10 设备使用的程序名(字符串)
        /// </summary>
        public AddressCombine DeviceProgramName { get; } = new AddressCombine { Address = "D7080", Length = 10 };

        // ---- 产品信息（2个） -----

        /// <summary>
        /// D7020,20 产品型号(string)
        /// </summary>
        public AddressCombine ProductType { get; } = new AddressCombine { Address = "D7020", Length = 20 };

        /// <summary>
        /// D7042,15 条码规则(string)
        /// </summary>
        public AddressCombine BarcodeRule { get; } = new AddressCombine { Address = "D7042", Length = 15 };

        #endregion

        #region 线程：实时读取换型信号（2个）

        /// <summary>
        ///  D7004 型号切换(Int16) PLC通知PC切换型号，1=需要切换型号
        /// </summary>
        public string ModelSwitch { get; } = "D7004";

        /// <summary>
        /// D7003 工单切换完成后通知PLC继续生产(Int16)
        /// </summary>
        public string ContinueProduce { get; } = "D7003";

        #endregion

        #region 线程：管理PLC连接与心跳检测（1个）

        /// <summary>
        /// 心跳信号
        /// <para>PLC每隔1秒钟写入1次心跳信号到D7107</para>
        /// PC每隔1秒钟写入1次心跳信号到D7108
        /// </summary>
        public AddressRange HeartBeat { get; } = new AddressRange { PLC = "D7107", PC = "D7108" };

        #endregion

        #region 线程：实时读取复位信号（1个）

        /// <summary>
        /// D7514,Int16 复位信号
        /// <para>1=需要复位</para>
        /// </summary>
        public string RecoverySignal { get; } = "D7514";

        #endregion

        #region 线程：打印条码（2个）

        /// <summary>
        /// 标签打印
        /// <para>D7111 PLC->PC 1=触发打印;</para>
        /// D7112 PC->PLC 1=开始打印;
        /// </summary>
        public UpplerAndLower PrintTag { get; } = new UpplerAndLower { TriggerPrint = "D7111", FeedbackPrint = "D7112" };

        /// <summary>
        /// D7430,17 获取需要上传到MES进行打印请求的条码
        /// </summary>
        public AddressCombine BarcodeToPrint { get; } = new AddressCombine { Address = "D7430", Length = 17 };

        #endregion

        #endregion

        #region 条码验证（7个） 已PlcAddressInfo管理

        // ---- 条码验证基本信息（3个）-----

        /// <summary>
        /// D7000 有无条码标志(Int16)
        /// </summary>
        public string HasBarcodeTag { get; } = "D7000"; //有无条码标志(16位整型)

        /// <summary>
        /// 条码验证标识(Int16)
        /// D7001 = 1 验证成功，D7001 = 2 验证失败
        /// </summary>
        public string BarcodeVerifyTag { get; } = "D7001";

        /// <summary>
        /// D7059 条码类型(Int16)
        /// </summary>
        public string BarcodeType { get; } = "D7059";

        // ----- PLC与上位机条码传输相关（2个） -----

        /// <summary>
        /// D7061,17 PLC->PC 读取到的产品条码（PCB板条码）(string)
        /// </summary>
        public AddressCombine PlcScannedBarcode { get; } = new AddressCombine { Address = "D7061", Length = 17 };

        /// <summary>
        /// D7130,17 PC->PLC 拼版中的另一块产品的条码(string)
        /// </summary>
        public AddressCombine PanalizationBarcode { get; } = new AddressCombine { Address = "D7130", Length = 17 };

        // ----- 手动输入条码相关（2个） -----

        /// <summary>
        /// D7516 提示手动输入条码的PLC信号
        /// </summary>
        public string ManualInputBarcodeTip = "D7516";

        public AddressCombine ManualInputBarcode = new AddressCombine { Address = "D7518", Length = 17 };

        #endregion

        #region 产品过站（16个）已PlcAddressInfo管理

        #region 非装配机（4个）

        /// <summary>
        /// PLC触发上位机上传数据
        /// </summary>
        public string TriggerUpload { get; } = "D7115";

        /// <summary>
        /// Int16 数据上传完成反馈给PLC
        /// </summary>
        public string Feedback { get; } = "D7116";

        /// <summary>
        /// Int16 读取产品结果
        /// </summary>
        public string ProductResult { get; } = "D7119";

        /// <summary>
        /// 读取当前工序数据上传时所需的条码 
        /// </summary>
        public AddressCombine BarcodeToUpload { get; } = new AddressCombine { Address = "D7149", Length = 17 };

        #endregion

        #region 装配机（12个）

        // ========== 工序 1（4个）==========

        /// <summary>
        /// D7115,Int16 PLC触发上位机上传数据
        /// </summary>
        public string TriggerUpload1 { get; } = "D7115";

        /// <summary>
        /// D7116,Int16 数据上传完成反馈给PLC
        /// </summary>
        public string Feedback1 { get; } = "D7116";

        /// <summary>
        /// D7119,Int16 读取产品结果
        /// </summary>
        public string ProductResult1 { get; } = "D7119";

        /// <summary>
        /// D7149,17 读取当前工序数据上传时所需的条码
        /// </summary>
        public AddressCombine BarcodeToUpload1 { get; } = new AddressCombine { Address = "D7149", Length = 17 };

        // ========== 工序 2（4个）==========

        /// <summary>
        /// D7490,Int16 PLC触发上位机上传数据
        /// </summary>
        /// </summary>
        public string TriggerUpload2 { get; } = "D7490";

        /// <summary>
        /// D7491,Int16 数据上传完成反馈给PLC
        /// </summary>
        public string Feedback2 { get; } = "D7491";

        /// <summary>
        /// D7492,Int16 读取产品结果
        /// </summary>
        public string ProductResult2 { get; } = "D7492";

        /// <summary>
        /// D7493,17 读取当前工序数据上传时所需的条码 
        /// </summary>
        public AddressCombine BarcodeToUpload2 { get; } = new AddressCombine { Address = "D7493", Length = 17 };

        // ========== 工序 3（4个） ==========

        /// <summary>
        /// PLC触发上位机上传数据
        /// </summary>
        public string TriggerUpload3 { get; } = "D7600";

        /// <summary>
        /// Int16 数据上传完成反馈给PLC
        /// </summary>
        public string Feedback3 { get; } = "D7601";

        /// <summary>
        /// Int16 读取产品结果
        /// </summary>
        public string ProductResult3 { get; } = "D7602";

        /// <summary>
        /// 读取当前工序数据上传时所需的条码 
        /// </summary>
        public AddressCombine BarcodeToUpload3 { get; } = new AddressCombine { Address = "D7603", Length = 17 };

        #endregion

        #endregion

        #region MyRegion

        /// <summary>
        /// D7094 装配机第一次拍照的PLC信号
        /// </summary>
        public string ReadPicSignalFirst { get; } = "D7094";

        /// <summary>
        /// D7095 装配机第二次拍照的PLC信号
        /// </summary>
        public string ReadPicSignalSecond { get; } = "D7095";

        /// <summary>
        /// D7096 装配机第三次拍照的PLC信号
        /// </summary>
        public string ReadPicSignalThird { get; } = "D7096";

        /// <summary>
        /// D7097 写入第一次拍照完成的PLC信号
        /// </summary>
        public string WritePicSignalFirst { get; } = "D7097";

        /// <summary>
        /// D7098 写入第二次拍照完成的PLC信号
        /// </summary>
        public string WritePicSignalSecond { get; } = "D7098";

        /// <summary>
        /// D7099 写入第三次拍照完成的PLC信号
        /// </summary>
        public string WritePicSignalThird { get; } = "D7099";

        public AddressCombine ReadSNFirst { get; } = new AddressCombine { Address = "D7450", Length = 17 };
        public AddressCombine ReadSNSecond { get; } = new AddressCombine { Address = "D7470", Length = 17 };
        public AddressCombine ReadSNThird { get; } = new AddressCombine { Address = "D7430", Length = 17 };

        #endregion

        /// <summary>
        /// 每个故障提供一个点位、且需要提供每个点位对应的故障编号和名称,16位单字节，1=XX故障  0=无故障  2=XX预警
        /// </summary>
        public AddressRange ErrorPoint { get; } = new AddressRange { PLC = "D7170", PC = "D7270" };

        /// <summary>
        /// 参数采集标识 1=更还程序、变更生产参数时 0=无变化(16位整型)
        /// </summary>
        public string ArgsGatherTag { get; } = "D7277";

        #region MyRegion


        public Args Args1 = new Args
        {
            Name = new AddressCombine { Address = "D7280", Length = 2 },
            Standard = new AddressCombine { Address = "D7282", Length = 2 },
            Upper = new AddressCombine { Address = "D7284", Length = 2 },
            Lower = new AddressCombine { Address = "D7286", Length = 2 },
            Unit = new AddressCombine { Address = "D7288", Length = 2 },
        };

        public Args Args2 = new Args
        {
            Name = new AddressCombine { Address = "D7292", Length = 2 },
            Standard = new AddressCombine { Address = "D7294", Length = 2 },
            Upper = new AddressCombine { Address = "D7296", Length = 2 },
            Lower = new AddressCombine { Address = "D7298", Length = 2 },
            Unit = new AddressCombine { Address = "D7300", Length = 2 },
        };

        public Args Args3 = new Args
        {
            Name = new AddressCombine { Address = "D7304", Length = 2 },
            Standard = new AddressCombine { Address = "D7306", Length = 2 },
            Upper = new AddressCombine { Address = "D7308", Length = 2 },
            Lower = new AddressCombine { Address = "D7310", Length = 2 },
            Unit = new AddressCombine { Address = "D7312", Length = 2 },
        };

        public Args Args4 = new Args
        {
            Name = new AddressCombine { Address = "D7316", Length = 2 },
            Standard = new AddressCombine { Address = "D7318", Length = 2 },
            Upper = new AddressCombine { Address = "D7320", Length = 2 },
            Lower = new AddressCombine { Address = "D7322", Length = 2 },
            Unit = new AddressCombine { Address = "D7324", Length = 2 },
        };

        public Args Args5 = new Args
        {
            Name = new AddressCombine { Address = "D7328", Length = 2 },
            Standard = new AddressCombine { Address = "D7330", Length = 2 },
            Upper = new AddressCombine { Address = "D7332", Length = 2 },
            Lower = new AddressCombine { Address = "D7334", Length = 2 },
            Unit = new AddressCombine { Address = "D7336", Length = 2 },
        };

        public Args Args6 = new Args
        {
            Name = new AddressCombine { Address = "D7340", Length = 2 },
            Standard = new AddressCombine { Address = "D7342", Length = 2 },
            Upper = new AddressCombine { Address = "D7344", Length = 2 },
            Lower = new AddressCombine { Address = "D7346", Length = 2 },
            Unit = new AddressCombine { Address = "D7348", Length = 2 },
        };

        public Args Args7 = new Args
        {
            Name = new AddressCombine { Address = "D7352", Length = 2 },
            Standard = new AddressCombine { Address = "D7354", Length = 2 },
            Upper = new AddressCombine { Address = "D7356", Length = 2 },
            Lower = new AddressCombine { Address = "D7358", Length = 2 },
            Unit = new AddressCombine { Address = "D7360", Length = 2 },
        };

        public Args Args8 = new Args
        {
            Name = new AddressCombine { Address = "D7364", Length = 2 },
            Standard = new AddressCombine { Address = "D7366", Length = 2 },
            Upper = new AddressCombine { Address = "D7368", Length = 2 },
            Lower = new AddressCombine { Address = "D7370", Length = 2 },
            Unit = new AddressCombine { Address = "D7372", Length = 2 },
        };

        public Args Args9 = new Args
        {
            Name = new AddressCombine { Address = "D7376", Length = 2 },
            Standard = new AddressCombine { Address = "D7378", Length = 2 },
            Upper = new AddressCombine { Address = "D7380", Length = 2 },
            Lower = new AddressCombine { Address = "D7382", Length = 2 },
            Unit = new AddressCombine { Address = "D7384", Length = 2 },
        };

        public Args Args10 = new Args
        {
            Name = new AddressCombine { Address = "D7388", Length = 2 },
            Standard = new AddressCombine { Address = "D7390", Length = 2 },
            Upper = new AddressCombine { Address = "D7392", Length = 2 },
            Lower = new AddressCombine { Address = "D7394", Length = 2 },
            Unit = new AddressCombine { Address = "D7396", Length = 2 },
        };

        #endregion
    }
}
