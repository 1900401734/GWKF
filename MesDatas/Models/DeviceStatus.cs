using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class DeviceStatus
    {
        public static string nowTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 设备状态 输入参数
    /// </summary>
    public class InputParamDeviceStatus :MesInputBasicEntity
    {

        /// <summary>
        /// 设备状态 DOWNTIME/WAITPREVIOUS/WAITNEXT/RUN/POWEROFF/STOP
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 前置状态
        /// </summary>
        public string LastType { get; set; }
        /// <summary>
        /// 前置时间
        /// </summary>
        public string LastTypeTime { get; set; }
        /// <summary>
        /// 时间间隔
        /// </summary>
        public double Interval { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; } = DeviceStatus.nowTime();

    }
    /// <summary>
    /// 设备状态  输出
    /// </summary>
    public class ReturnParamDeviceStatus : MesReturnBasicEntity
    {
    }


}

