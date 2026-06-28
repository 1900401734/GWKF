using MesDatas.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MesDatas.Services
{
    /// <summary>
    /// Weight工序MES状态轻量缓存。
    /// <para>该缓存只服务打印前置判断，不保存MES请求体，不具备补传和重试能力。</para>
    /// </summary>
    public sealed class WeightMesStatusStore
    {
        /// <summary>
        /// 本地状态缓存根目录，独立于MES补传目录。
        /// </summary>
        public const string CacheRootPath = @"D:\KaiFaLogs\WeightMES状态缓存";

        private const string DateFolderFormat = "yyyy-MM-dd";
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 保存一条Weight状态。相同日期、相同条码会覆盖为最新状态。
        /// </summary>
        public WeightMesStatusRecord Save(WeightMesStatusRecord record)
        {
            if (record == null) return null;
            if (string.IsNullOrWhiteSpace(record.Barcode)) return null;

            lock (_syncRoot)
            {
                if (record.UpdatedAt == default(DateTime))
                    record.UpdatedAt = DateTime.Now;

                EnsureDirectory();

                string dayFolder = Path.Combine(CacheRootPath, record.UpdatedAt.ToString(DateFolderFormat));
                Directory.CreateDirectory(dayFolder);

                string json = JsonConvert.SerializeObject(record, Formatting.Indented);
                File.WriteAllText(GetRecordPath(dayFolder, record.Barcode), json, Encoding.UTF8);
                return record;
            }
        }

        /// <summary>
        /// 加载最近若干天的Weight状态缓存。
        /// </summary>
        public List<WeightMesStatusRecord> LoadRecent(int days)
        {
            lock (_syncRoot)
            {
                EnsureDirectory();

                var records = new List<WeightMesStatusRecord>();
                int safeDays = Math.Max(days, 1);
                DateTime today = DateTime.Today;

                for (int i = 0; i < safeDays; i++)
                {
                    string dayFolder = Path.Combine(CacheRootPath, today.AddDays(-i).ToString(DateFolderFormat));
                    records.AddRange(LoadFolder(dayFolder));
                }

                return records
                    .Where(item => item != null && !string.IsNullOrWhiteSpace(item.Barcode))
                    .OrderByDescending(item => item.UpdatedAt)
                    .ToList();
            }
        }

        /// <summary>
        /// 查找指定条码最近一次Weight状态。
        /// </summary>
        public WeightMesStatusRecord FindLatestByBarcode(string barcode, int days)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;

            return LoadRecent(days)
                .Where(item => string.Equals(item.Barcode, barcode, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(item => item.UpdatedAt)
                .FirstOrDefault();
        }

        /// <summary>
        /// 清理超过保留天数的旧缓存目录。
        /// </summary>
        public void PruneOlderThan(int retentionDays)
        {
            lock (_syncRoot)
            {
                EnsureDirectory();

                int safeRetentionDays = Math.Max(retentionDays, 1);
                DateTime cutoffDate = DateTime.Today.AddDays(-safeRetentionDays);

                foreach (string folder in Directory.GetDirectories(CacheRootPath))
                {
                    DateTime folderDate;
                    string folderName = Path.GetFileName(folder);
                    bool parsed = DateTime.TryParseExact(
                        folderName,
                        DateFolderFormat,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out folderDate);

                    if (parsed && folderDate < cutoffDate)
                        Directory.Delete(folder, true);
                }
            }
        }

        private static List<WeightMesStatusRecord> LoadFolder(string folder)
        {
            var records = new List<WeightMesStatusRecord>();
            if (!Directory.Exists(folder)) return records;

            foreach (string path in Directory.GetFiles(folder, "*.json"))
            {
                WeightMesStatusRecord record = TryLoadFile(path);
                if (record != null) records.Add(record);
            }

            return records;
        }

        private static WeightMesStatusRecord TryLoadFile(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<WeightMesStatusRecord>(File.ReadAllText(path, Encoding.UTF8));
            }
            catch
            {
                return null;
            }
        }

        private static void EnsureDirectory()
        {
            if (!Directory.Exists(CacheRootPath))
                Directory.CreateDirectory(CacheRootPath);
        }

        private static string GetRecordPath(string dayFolder, string barcode)
        {
            return Path.Combine(dayFolder, CreateSafeFileName(barcode) + ".json");
        }

        private static string CreateSafeFileName(string barcode)
        {
            string safeName = string.Concat((barcode ?? string.Empty).Split(Path.GetInvalidFileNameChars()));
            if (string.IsNullOrWhiteSpace(safeName))
                safeName = "NoBarcode";

            return $"{safeName}_{CreateShortHash(barcode)}";
        }

        private static string CreateShortHash(string text)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
                byte[] hash = sha1.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", string.Empty).Substring(0, 8);
            }
        }
    }
}
