using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class DeviceEroorInputParam: MesInputBasicEntity
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; } = CheckPathEntity.nowTime();
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 故障类型
        /// </summary>
        public string ErrorType { get; set; }
        /// <summary>
        /// 故障ID
        /// </summary>
        public string ErrorID {get;set; }
        /// <summary>
        /// 故障代码
        /// </summary>
        public string ErrorCode {get;set; }
        /// <summary>
        /// 故障信息
        /// </summary>
        public string ErrorMessage {get;set; }
        /// <summary>
        /// 清除故障员工
        /// </summary>
        public string ErrorClearUser {get;set; }
    }

    public class DeviceErrorReturnParam : MesReturnBasicEntity
    {

    }
}
