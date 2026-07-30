using MesDatas.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MesDatas.Services
{
    /// <summary>
    /// 先反馈再上传记录文件存储。
    /// <para>这里不依赖数据库，避免现场数据库结构变更影响上线。</para>
    /// </summary>
    public sealed class MesOutboxStore
    {
        /// <summary>
        /// 本地记录根目录。
        /// <para>目录名保留“MES补传队列”是为了兼容现场已有文件，不代表普通同步模式会写入这里。</para>
        /// </summary>
        public const string QueueRootPath = @"D:\KaiFaLogs\MES补传队列";

        private readonly object _syncRoot = new object();

        /// <summary>
        /// 保存或更新一条先反馈再上传记录。
        /// </summary>
        public MesOutboxRecord Save(MesOutboxRecord record)
        {
            if (record == null) return null;

            lock (_syncRoot)
            {
                EnsureDirectory();

                if (string.IsNullOrWhiteSpace(record.RecordId))
                    record.RecordId = CreateRecordId(record);

                record.UpdatedAt = DateTime.Now;

                string json = JsonConvert.SerializeObject(record, Formatting.Indented);
                File.WriteAllText(GetRecordPath(record.RecordId), json);
                return record;
            }
        }

        /// <summary>
        /// 加载所有等待后台重试的记录。
        /// </summary>
        public List<MesOutboxRecord> LoadPendingRetry()
        {
            return LoadAll()
                .Where(item => item.Status == MesOutboxStatus.PendingRetry)
                .OrderBy(item => item.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// 根据记录编号加载本地记录。
        /// </summary>
        public MesOutboxRecord Load(string recordId)
        {
            if (string.IsNullOrWhiteSpace(recordId)) return null;

            lock (_syncRoot)
            {
                string path = GetRecordPath(recordId);
                if (!File.Exists(path)) return null;
                return JsonConvert.DeserializeObject<MesOutboxRecord>(File.ReadAllText(path));
            }
        }

        /// <summary>
        /// 加载所有本地记录。
        /// </summary>
        public List<MesOutboxRecord> LoadAll()
        {
            lock (_syncRoot)
            {
                EnsureDirectory();

                var records = new List<MesOutboxRecord>();
                foreach (string path in Directory.GetFiles(QueueRootPath, "*.json"))
                {
                    MesOutboxRecord record = TryLoadFile(path);
                    if (record != null) records.Add(record);
                }

                return records;
            }
        }

        /// <summary>
        /// 查找某个条码在指定工序的最近一次先反馈再上传记录。
        /// </summary>
        public MesOutboxRecord FindLatestByBarcodeAndProcess(string barcode, string processName)
        {
            if (string.IsNullOrWhiteSpace(barcode) || string.IsNullOrWhiteSpace(processName)) return null;

            return LoadAll()
                .Where(item =>
                    string.Equals(item.ProcessName, processName, StringComparison.OrdinalIgnoreCase) &&
                    item.Barcodes != null &&
                    item.Barcodes.Any(sn => string.Equals(sn, barcode, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(item => item.UpdatedAt)
                .FirstOrDefault();
        }

        /// <summary>
        /// 标记MES已确认PASS。
        /// </summary>
        public MesOutboxRecord MarkConfirmedPass(string recordId, string message = null)
        {
            MesOutboxRecord record = Load(recordId);
            if (record == null) return null;

            record.Status = MesOutboxStatus.ConfirmedPass;
            record.ErrorType = null;
            record.ErrorMessage = message;
            record.ConfirmedAt = DateTime.Now;
            return Save(record);
        }

        /// <summary>
        /// 标记MES明确FAIL。
        /// </summary>
        public MesOutboxRecord MarkConfirmedFail(string recordId, string errorType, string errorMessage)
        {
            MesOutboxRecord record = Load(recordId);
            if (record == null) return null;

            record.Status = MesOutboxStatus.ConfirmedFail;
            record.ErrorType = errorType;
            record.ErrorMessage = errorMessage;
            record.ConfirmedAt = DateTime.Now;
            return Save(record);
        }

        /// <summary>
        /// 标记为后台上传待重试，并累计重试次数。
        /// </summary>
        public MesOutboxRecord MarkPendingRetry(string recordId, string errorType, string errorMessage)
        {
            MesOutboxRecord record = Load(recordId);
            if (record == null) return null;

            record.Status = MesOutboxStatus.PendingRetry;
            record.ErrorType = errorType;
            record.ErrorMessage = errorMessage;
            record.RetryCount++;
            record.LastAttemptAt = DateTime.Now;
            return Save(record);
        }

        /// <summary>
        /// 标记为人工处理中。
        /// <para>请求构造失败、后台上传数据不完整等场景无法自动重试，必须让现场人员明确处理。</para>
        /// </summary>
        public MesOutboxRecord MarkManualProcessing(string recordId, string errorType, string errorMessage)
        {
            MesOutboxRecord record = Load(recordId);
            if (record == null) return null;

            record.Status = MesOutboxStatus.ManualProcessing;
            record.ErrorType = errorType;
            record.ErrorMessage = errorMessage;
            record.ConfirmedAt = null;
            return Save(record);
        }

        private static MesOutboxRecord TryLoadFile(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<MesOutboxRecord>(File.ReadAllText(path));
            }
            catch
            {
                return null;
            }
        }

        private static void EnsureDirectory()
        {
            if (!Directory.Exists(QueueRootPath))
                Directory.CreateDirectory(QueueRootPath);
        }

        private static string GetRecordPath(string recordId)
        {
            string fileName = string.Concat(recordId.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(QueueRootPath, fileName + ".json");
        }

        private static string CreateRecordId(MesOutboxRecord record)
        {
            string process = string.IsNullOrWhiteSpace(record.ProcessName) ? "Unknown" : record.ProcessName;
            string barcode = string.IsNullOrWhiteSpace(record.Barcode) ? "NoBarcode" : record.Barcode;
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{DateTime.Now:yyyyMMddHHmmssfff}_{process}_{barcode}_{suffix}";
        }
    }
}
