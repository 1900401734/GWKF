using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class DeviceProgramRealtimeArgsInputParam:MesInputBasicEntity
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; } = CheckPathEntity.nowTime();

        /// <summary>
        /// 程序名
        /// </summary>
        public string ProgramName {get;set;}

        /// <summary>
        /// 软件版本
        /// </summary>
        public string SWVer {get;set;}

        /// <summary>
        /// 员工工号
        /// </summary>
        public string User {get;set;}

        /// <summary>
        /// 数据集
        /// </summary>
        public JObject Datas {get;set;}
    }

    public class DeviceProgramRealtimeArgsReturnParam : MesReturnBasicEntity
    {

    }
}
