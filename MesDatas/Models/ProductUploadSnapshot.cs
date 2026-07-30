using System.Collections.Generic;

namespace MesDatas.Models
{
    /// <summary>
    /// 产品上传快照。
    /// <para>后台 MES 上传不能直接使用原始列表，因为主流程结束时会清空这些列表。</para>
    /// </summary>
    public sealed class ProductUploadSnapshot
    {
        /// <summary>
        /// 当前工序的上传配置副本。
        /// </summary>
        public UploadManagerEntity UploadEntity { get; private set; }

        /// <summary>
        /// 本次过站读取到的产品条码副本。
        /// </summary>
        public List<string> ScannedBarcodeList { get; private set; }

        /// <summary>
        /// 本次过站读取到的产品结果副本。
        /// </summary>
        public List<string> ProductResultList { get; private set; }

        /// <summary>
        /// 测试项实际值副本。
        /// </summary>
        public List<string> ValueList { get; private set; }

        /// <summary>
        /// 测试项上限副本。
        /// </summary>
        public List<string> MaxList { get; private set; }

        /// <summary>
        /// 测试项下限副本。
        /// </summary>
        public List<string> MinList { get; private set; }

        /// <summary>
        /// 测试项结果副本。
        /// </summary>
        public List<string> ResultList { get; private set; }

        /// <summary>
        /// 测试项标准值/缺陷代码副本。
        /// </summary>
        public List<string> StandardList { get; private set; }

        public ProductUploadSnapshot(
            UploadManagerEntity uploadEntity,
            List<string> scannedBarcodeList,
            List<string> productResultList,
            List<string> valueList,
            List<string> maxList,
            List<string> minList,
            List<string> resultList,
            List<string> standardList)
        {
            UploadEntity = CopyUploadEntity(uploadEntity);
            ScannedBarcodeList = CopyList(scannedBarcodeList);
            ProductResultList = CopyList(productResultList);
            ValueList = CopyList(valueList);
            MaxList = CopyList(maxList);
            MinList = CopyList(minList);
            ResultList = CopyList(resultList);
            StandardList = CopyList(standardList);
        }

        /// <summary>
        /// 复制字符串列表，避免后台线程使用外部可变集合。
        /// </summary>
        private static List<string> CopyList(List<string> source)
        {
            return source == null ? new List<string>() : new List<string>(source);
        }

        /// <summary>
        /// 复制上传配置，避免后台线程读取到后续流程修改后的对象。
        /// </summary>
        private static UploadManagerEntity CopyUploadEntity(UploadManagerEntity source)
        {
            if (source == null) return null;

            return new UploadManagerEntity
            {
                Name = source.Name,
                triggerPoint = source.triggerPoint,
                feedbackPoint = source.feedbackPoint,
                ProductResult = source.ProductResult,
                BarcodeToUpload = source.BarcodeToUpload,
                BarcodeToUploadLength = source.BarcodeToUploadLength,
                Line = source.Line,
                Process = source.Process,
                Staiton = source.Staiton,
                Device = source.Device,
                Key = source.Key,
                Pwd = source.Pwd,
                DeleteFile = source.DeleteFile
            };
        }
    }
}
