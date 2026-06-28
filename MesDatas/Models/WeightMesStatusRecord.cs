using System;

namespace MesDatas.Models
{
    /// <summary>
    /// Weight工序MES确认状态的轻量本地缓存记录。
    /// <para>该记录只用于打印前置判断，不保存MES请求体，也不参与后台补传。</para>
    /// </summary>
    public sealed class WeightMesStatusRecord
    {
        /// <summary>
        /// 产品条码。
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 固定为Weight，用于现场排查时明确前置工序。
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// MES确认状态，只有ConfirmedPass允许后续打印。
        /// </summary>
        public MesOutboxStatus Status { get; set; }

        /// <summary>
        /// 失败来源，如MES、网络/接口、本地。
        /// </summary>
        public string FailureSource { get; set; }

        /// <summary>
        /// 失败明细或MES返回说明。
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 本地状态更新时间。
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
