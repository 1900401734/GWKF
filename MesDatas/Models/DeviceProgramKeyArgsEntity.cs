using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class DeviceProgramKeyArgsInputParam : MesInputBasicEntity
    {
        public string DateTime { get; set; } = CheckPathEntity.nowTime();

        public string ProgramName { get; set; }
        public string SWVer { get; set; }
        public string User { get; set; }
        public JObject Datas { get; set; }
    }

    public class DeviceProgramKeyArgsReturnParam : MesReturnBasicEntity
    {

    }

    //public class DataPoint
    //{
    //    public string Name { get; set; }
    //    public string Standard { get; set; }
    //    public string LSL { get; set; }
    //    public string USL { get; set; }
    //    public string Unit { get; set; }
    //}
}
