using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class FtpMessageGetInputParam : MesInputBasicEntity
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; } = CheckPathEntity.nowTime();

        /// <summary>
        /// 查询工序
        /// </summary>
        public string QueryProcss { get; set; }

        /// <summary>
        /// 查询条码
        /// </summary>
        public string PrdSN { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

    }


    public class FtpMessageGetReturnParam : MesReturnBasicEntity
    {

        public Dictionary<string, JObject> FTPPath { get; set; }

    }
}

