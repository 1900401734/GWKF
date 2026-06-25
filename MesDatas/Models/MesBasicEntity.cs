using MesDatas.Views;

namespace MesDatas.Models
{
    /// <summary>
    /// MES接口  通用字段
    /// </summary>
    public class MesInputBasicEntity
    {
        /// <summary>
        /// 线体
        /// </summary>
        public string Line { get; set; } = Form1.GlobalData["Line"].ToString();
        /// <summary>
        /// 工序
        /// </summary>
        public string Process { get; set; } = Form1.GlobalData["Process"].ToString();
        /// <summary>
        /// 工站
        /// </summary>
        public string Station { get; set; } = Form1.GlobalData["Station"].ToString();
        /// <summary>
        /// 设备
        /// </summary>
        public string Device { get; set; } = Form1.GlobalData["Device"].ToString();
        /// <summary>
        /// 电脑名
        /// </summary>
        public string ComputerName { get; set; } = System.Windows.Forms.SystemInformation.ComputerName;
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; } = "CH";
    }

    public class MesReturnBasicEntity
    {
        /// <summary>
        /// 结果 Pass/Fail
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 错误代码 10001 是 Token 异常，可重新获取 Token 并再次调用此接口
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误详情
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
