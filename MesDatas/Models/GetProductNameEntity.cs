using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class GetProductNameInputParam
    {
        /// <summary>
        /// 产品条码或工装条码
        /// </summary>
        public string PrdSN { get; set; }
    }

    public class GetProductNameReturnParam
    {
        public string PrdName { get; set; }

        public List<JObject[]> PrdSNCollectio;

    } 

}
