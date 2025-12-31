using MesDatas.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Models
{
    public class UploadManagerEntity
    {
        /// <summary>
        /// 当前工序名称
        /// </summary>
        public string Name { get; set; }
        public string triggerPoint { get; set; }
        public string feedbackPoint { get; set; }
        public string ProductResult { get; set; }
        public string BarcodeToUpload { get; set; }
        public string BarcodeToUploadLength { get; set; }
        //public PlcAddressInfo plcAddressInfo { get; set; }

        /// <summary>
        /// 线体
        /// </summary>
        public string Line { get; set; }
        /// <summary>
        /// 工序
        /// </summary>
        public string Process { get; set; }
        /// <summary>
        /// 工站
        /// </summary>
        public string Staiton { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// MES账号
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// MD加密
        /// </summary>
        public string Pwd { get; set; }
        public bool DeleteFile { get; set; }
    }
}
