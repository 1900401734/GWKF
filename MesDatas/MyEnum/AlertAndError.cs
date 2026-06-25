using System.ComponentModel;

namespace MesDatas.MyEnum
{
    /// <summary>
    /// 预警故障的处理动作类型
    /// </summary>
    public enum ActionType
    {
        ACTION_ERROR = -1, // 异常
        ACTION_IGNORE = 0, // 忽略
        ACTION_UPLOAD = 1, // 上传
    }

    /// <summary>
    /// 上传给MES的动作类型
    /// </summary>
    public enum ErrorType
    {
        Occur,  // 发生故障
        Clear   // 清除故障
    }

    /// <summary>
    /// 接收的来自PLC的报警状态
    /// </summary>
    public enum DataType
    {
        [Description("正常数据")]
        Normal,
        [Description("报警数据")]
        Alert,
        [Description("故障数据")]
        Alarm
    }
}
