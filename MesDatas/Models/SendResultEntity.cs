using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesDatas.Views;

namespace MesDatas.Models
{
    class SendResultEntity
    {
        public static string nowTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 上传MES参数集合
    /// </summary>
    public class InputParamSendResult : MesInputBasicEntity
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; } = SendResultEntity.nowTime();
        /// <summary>
        /// 员工工号
        /// </summary>
        public string Employee { get; set; }
        /// <summary>
        /// 软件版本
        /// </summary>
        public string SWVer { get; set; } = Form1.GlobalData["SWVer"].ToString();
        /// <summary>
        /// 硬件版本
        /// </summary>
        public string HWVer { get; set; } = Form1.GlobalData["HWVer"].ToString();
        /// <summary>
        /// 程序名
        /// </summary>
        public string Program { get; set; } = Form1.Program;
        /// <summary>
        /// 工装编号
        /// </summary>
        public string Fixture { get; set; }
        /// <summary>
        /// 轨道号
        /// </summary>
        public string TrackNo { get; set; }
        /// <summary>
        /// 文件路径  FTP
        /// </summary>
        public string ResultFileFTPPath { get; set; }
        /// <summary>
        /// 图片路径  FTP
        /// </summary>
        public string PhotoFTPPath { get; set; }
        /// <summary>
        /// 工单号
        /// </summary>
        public string PlanNo { get; set; }
        /// <summary>
        /// 板边码
        /// </summary>
        public string BoardSideSN { get; set; }
        /// <summary>
        /// 子板条码集合
        /// </summary>
        public PrdSNCollection2 PrdSNCollection { get; set; }
    }

    /// <summary>
    /// 子板条码集合
    /// </summary>
    public class PrdSNCollection2
    {
        /// <summary>
        /// 子板条码
        /// </summary>
        public List<PrdSNsItem> PrdSNs { get; set; }
    }

    public class PrdSNsItem
    {
        /// <summary>
        /// 子板条码
        /// </summary>
        public string PrdSN { get; set; }
        /// <summary>
        /// 子板序号
        /// </summary>
        public string SubBoardId { get; set; }
        /// <summary>
        /// 跳板标记
        /// </summary>
        public string BoardSkip { get; set; }
        /// <summary>
        /// 过站结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 机器结果
        /// </summary>
        public string MachineResult { get; set; }
        /// <summary>
        /// CycleTime 单位/秒
        /// </summary>
        public string CycleTime { get; set; }
        /// <summary>
        /// 结果文件名
        /// </summary>
        public string ResultFile { get; set; }
        /// <summary>
        /// 图片名集合
        /// </summary>
        public PhotoFiles PhotoFiles { get; set; }
        /// <summary>
        /// 参数明细集合
        /// </summary>
        public TestDatas TestDatas { get; set; }
        /// <summary>
        /// 缺陷明细集合
        /// </summary>
        public Defects Defects { get; set; }
    }

    public class PhotoFiles
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> PhotoFile { get; set; } = new List<string>();
    }

    public class TestDatas
    {
        public List<TestDataItem> TestData { get; set; }
    }

    public class TestDataItem
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; } = "";
        /// <summary>
        /// 参数结果
        /// </summary>
        public string Result { get; set; } = "";
        /// <summary>
        /// 参数单位
        /// </summary>
        public string Unit { get; set; } = "";
        /// <summary>
        /// 参数下限
        /// </summary>
        public string LSL { get; set; } = "";
        /// <summary>
        /// 参数上限
        /// </summary>
        public string USL { get; set; } = "";
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string Special1 { get; set; } = "";
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string Special2 { get; set; } = "";
    }

    public class Defects 
    {
        /// <summary>
        /// 缺陷明细
        /// </summary>
        public List<Defect> Defect { get; set; } = new List<Defect> { new Defect() };
    }

    public class Defect 
    {
        /// <summary>
        /// 不良位置
        /// </summary>
        public string Location { get; set; } = "";
        /// <summary>
        /// 不良代码
        /// </summary>
        public string DefectDesc { get; set; } = "";
        /// <summary>
        /// 误判 是否误判，1 表示误判，0 表示真实缺陷
        /// </summary>
        public string Missing { get; set; } = "";
    }


    /// <summary>
    /// MES 返回上传生产信息的结果
    /// </summary>
    public class ReturnParamSendResult : MesReturnBasicEntity
    {

    }
}
