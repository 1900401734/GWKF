using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    class ErrorWaringEntity
    {
        //plc的地址
        public string plcAddress { get; set; }
        /// <summary>
        /// Alert：预警，Alarm:故障 
        /// </summary>
        public string dataType { get; set; }

        public string errorId { get; set; }

        public override bool Equals(object obj)
        {
            // 检查是否为null和是否为相同类型的实例  
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            //上面判断了相同类型，直接转类型即可
            ErrorWaringEntity errorWaring = (ErrorWaringEntity)obj;
            return plcAddress == errorWaring.plcAddress && dataType == errorWaring.dataType;  //只需要比较地址
        }

        public override int GetHashCode()
        {
            // 使用StringComparer.Ordinal.GetHashCode来避免文化敏感性  
            // 并确保与Equals方法中的比较逻辑一致  
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + StringComparer.Ordinal.GetHashCode(plcAddress);
                hash = hash * 23 + StringComparer.Ordinal.GetHashCode(plcAddress);
                hash = hash * 23 + errorId.GetHashCode();
                return hash;
            }
        }

    }
}
