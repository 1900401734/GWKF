using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class DeviceHeartBeatInputParam : MesInputBasicEntity
    {
        public string DateTime { get; set; } = CheckPathEntity.nowTime();
    }

    public class DeviceHeartBeatReturnParam : MesReturnBasicEntity
    {
    }
}
