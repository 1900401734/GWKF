using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{

    /// <summary>
    /// 获取token 输入
    /// </summary>
    public class TokenInputParameter : MesInputBasicEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Security
        /// </summary>
        public string Security { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        //public string Language { get; set; } = "CH";
    }

    /// <summary>
    /// 获取token返回参数
    /// </summary>
    public class TokenReturnParameter : MesReturnBasicEntity
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Access_Token { get; set; }
        /// <summary>
        /// 服务器时间
        /// </summary>
        public string ServerTime { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public string Expiredtime_Token { get; set; }
    }
}
