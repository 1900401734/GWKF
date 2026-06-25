using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.MyEnum
{
    public enum ProcessName
    {
        [Description("非装配机设备")]
        Non_Assembly = 0,
        [Description("Scan_ASSY")]
        Scan_ASSY = 1,
        [Description("Weight")]
        Weight = 2,
        [Description("Screw_BA")]
        Screw_BA = 3,
        [Description("不强制")]
        None = 4,
        [Description("全部工序")]
        All = 5
    }
}
