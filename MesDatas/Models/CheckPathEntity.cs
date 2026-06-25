using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesDatas.Views;

namespace MesDatas.Models
{
    class CheckPathEntity
    {
        public static string nowTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 流程检查  传入参数
    /// </summary>
    public class RouteCheckInputParam : MesInputBasicEntity
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; } = CheckPathEntity.nowTime();
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
        /// 印刷面别 TOP 或者 BOTTOM
        /// </summary>
        public string BoardSide { get; set; }
        /// <summary>
        /// 程序名
        /// </summary>
        public string Program { get; set; } = Form1.Program;
        /// <summary>
        /// 工单号
        /// </summary>
        public string PlanNo { get; set; }
        /// <summary>
        /// 轨道
        /// </summary>
        public string TrackNo { get; set; }
        /// <summary>
        /// 板边码
        /// </summary>
        public string BoardSideSN { get; set; }
        /// <summary>
        /// 子板条码集合
        /// </summary>
        public PrdSNCollection PrdSNCollection { get; set; }
    }

    public class PrdSNCollection
    {
        /// <summary>
        /// 子板条码列表，不包含子板序号
        /// </summary>
        public List<string> PrdSN { get; set; }
    }

    /// <summary>
    /// MES 返回的数据
    /// </summary>
    public class RouteCheckReturnParam : MesReturnBasicEntity
    {
        public SkipBoards SkipBoards { get; set; }

        /// <summary>
        /// 程序名
        /// </summary>
        public string Program { get; set; }
    }

    /// <summary>
    /// 跳板码集合
    /// </summary>
    public class SkipBoards
    {
        /// <summary>
        /// 跳板码集合
        /// </summary>
        public List<string> SkipBoard { get; set; }
    }
}
