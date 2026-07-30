using System;
using System.Collections.Generic;

namespace MesDatas.Models
{
    /// <summary>
    /// MES过站补传记录状态。
    /// <para>每块产品必须落到一个明确状态，避免MES超时后记录丢失。</para>
    /// </summary>
    public enum MesOutboxStatus
    {
        Created,
        PendingRetry,
        ConfirmedPass,
        ConfirmedFail,
        ManualProcessing,
        OfflineBypass
    }

    /// <summary>
    /// MES过站本地补传记录。
    /// <para>记录会保存到本地文件，程序重启后仍可继续补传。</para>
    /// </summary>
    public sealed class MesOutboxRecord
    {
        /// <summary>
        /// 本地补传记录唯一编号，同时作为文件名。
        /// </summary>
        public string RecordId { get; set; }

        /// <summary>
        /// 产品过站链路追踪编号。
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// 工序名称，如 Scan_ASSY、Weight、Screw_BA。
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 主条码，默认取本次过站第一块条码。
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 本次过站的所有条码。
        /// </summary>
        public List<string> Barcodes { get; set; } = new List<string>();

        /// <summary>
        /// 产品结果列表。
        /// </summary>
        public List<string> ProductResults { get; set; } = new List<string>();

        /// <summary>
        /// 测试项实际值列表。
        /// </summary>
        public List<string> ValueList { get; set; } = new List<string>();

        /// <summary>
        /// 测试项上限列表。
        /// </summary>
        public List<string> MaxList { get; set; } = new List<string>();

        /// <summary>
        /// 测试项下限列表。
        /// </summary>
        public List<string> MinList { get; set; } = new List<string>();

        /// <summary>
        /// 测试项结果列表。
        /// </summary>
        public List<string> ResultList { get; set; } = new List<string>();

        /// <summary>
        /// 测试项标准值或缺陷代码列表。
        /// </summary>
        public List<string> StandardList { get; set; } = new List<string>();

        /// <summary>
        /// SAVERESULT请求参数JSON。
        /// </summary>
        public string PayloadJson { get; set; }

        /// <summary>
        /// 当前补传状态。
        /// </summary>
        public MesOutboxStatus Status { get; set; }

        /// <summary>
        /// 失败类型，如 TIMEOUT、DNS_ERROR、MES_FAIL。
        /// </summary>
        public string ErrorType { get; set; }

        /// <summary>
        /// 失败明细。
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 已补传次数。
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// 本地记录创建时间。
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 本地记录更新时间。
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 最近一次尝试上传时间。
        /// </summary>
        public DateTime? LastAttemptAt { get; set; }

        /// <summary>
        /// MES确认时间。
        /// </summary>
        public DateTime? ConfirmedAt { get; set; }
    }
}
