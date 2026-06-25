using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesDatas.Views;

namespace MesDatas.Models
{
    public class GetBarCodeEntity
    {
        public static string nowTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 获取拼版数据
    /// </summary>
    public class GetBarCodeInputParameter : MesInputBasicEntity
    {
        /// <summary>
        /// 程序名
        /// </summary>
        public string Program { get; set; } = Form1.Program;

        /// <summary>
        /// 轨道，可为空
        /// </summary>
        public string TrackNo { get; set; }

        /// <summary>
        /// 查询条码 设备识别的条码
        /// </summary>
        public string PrdSN { get; set; }

        /// <summary>
        /// 当前时间 
        /// 格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string DateTime { get; set; } = GetBarCodeEntity.nowTime();
    }

    /// <summary>
    /// MES反馈拼板数据
    /// </summary>
    public class GetBarCodeReturnParameter : MesReturnBasicEntity
    {
        /// <summary>
        /// 子板码集合
        /// </summary>
        public PrdSNInfo PrdSNInfo { get; set; }
    }

    public class PrdSNInfo
    {
        /// <summary>
        /// 子板码结点
        /// </summary>
        public List<PrdSNs> PrdSNs { get; set; }
    }

    /// <summary>
    /// 子板码结点
    /// <para>包含多块PCB板，每一块板都有一个条码和对应的序号：称为子板条码和子板序号</para>
    /// </summary>
    public class PrdSNs
    {
        /// <summary>
        /// 子板条码
        /// </summary>
        public string PrdSN { get; set; }

        /// <summary>
        /// 子板序号
        /// </summary>
        public string SubBoardId { get; set; }
    }
}
