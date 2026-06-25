using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class DeviceToolingChangeInputParam : MesInputBasicEntity
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; } = CheckPathEntity.nowTime();
        /// <summary>
        /// 程序名
        /// </summary>
        public string ProgramName { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 工装位置
        /// </summary>
        public string Pos { get; set; }
    }


    /// <summary>
    /// 更换工装
    /// </summary>
    public class ChangeToolingInputParam : DeviceToolingChangeInputParam
    {
        /// <summary>
        /// 旧工装
        /// </summary>
        public string OldFixtureNo { get; set; }
        /// <summary>
        /// 旧产品
        /// </summary>
        public string OldPrdNo { get; set; }
        /// <summary>
        /// 新工装
        /// </summary>
        public string NewFixtureNo { get; set; }
        /// <summary>
        /// 新产品
        /// </summary>
        public string NewPrdNo { get; set; }
    }


    /// <summary>
    /// 更换铣刀
    /// </summary>
    public class ChangeMillingCuuterInputParam : DeviceToolingChangeInputParam
    {
        /// <summary>
        /// 铣刀更换代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 铣刀更换描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 铣刀尺寸
        /// </summary>
        public string Size { get; set; }
    }


    public class DeviceToolingChangeReturnParam : MesReturnBasicEntity
    {

    }
}
