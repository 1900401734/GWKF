using Newtonsoft.Json.Linq;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class DeviceProgramRealtimeArgsInputParam : MesInputBasicEntity
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
        /// 软件版本
        /// </summary>
        public string SWVer { get; set; }

        /// <summary>
        /// 员工工号
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 参数明细集合
        /// </summary>
        public ParamDatas Datas { get; set; }
    }

    public class ParamDatas
    {
        /// <summary>
        /// 参数明细列表
        /// </summary>
        public List<Data> Data { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数实际值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 参数标准值
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        /// 参数单位（没有可置空）
        /// </summary>
        public string Unit { get; set; } = "N.m";

        /// <summary>
        /// 参数下限（没有可置空）
        /// </summary>
        public string LSL { get; set; } = string.Empty;

        /// <summary>
        /// 参数上限（没有可置空）
        /// </summary>
        public string USL { get; set; } = string.Empty;
    }

    public class DeviceProgramRealtimeArgsReturnParam : MesReturnBasicEntity
    {

    }
}
