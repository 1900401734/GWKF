using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class PrintBarCodeInputParam : MesInputBasicEntity
    {
        /// <summary>
        /// 工单编号
        /// </summary>
        public string PlanNo { get; set; }

        /// <summary>
        /// 包装箱号或产品条码
        /// </summary>
        public string PrdSN { get; set; }

        /// <summary>
        /// 时间日期
        /// </summary>
        public string DateTime { get; set; } = CheckPathEntity.nowTime();

        /// <summary>
        /// 工号/使用者账号
        /// </summary>
        public string Employee { get; set; }
    }

    public class PrintBarCodeReturnParam : MesReturnBasicEntity
    {

        /// <summary>
        /// 打印模板集合
        /// </summary>
        public JArray PrintTempLate { get; set; }

        /// <summary>
        /// 打印参数列表
        /// </summary>
        public JArray PrintParameterList { get; set; }
    }
}
        

