namespace MesDatas.Models
{
    public class MaxMinValue
    {
        public string BoardName { get; set; }
        public string StandardCode { get; set; }
        public string MaxBoardCode { get; set; }
        public string MinBoardCode { get; set; }
        public string BoardCode { get; set; }
        public string Result { get; set; }
    }

    /// <summary>
    /// 列信息结构，用于统一管理列的创建和更新
    /// </summary>
    public class ColumnInfo
    {
        public string HeaderKey { get; set; }           // 资源键
        public string TestItemName { get; set; }        // 测试项名称
        public string ColumnType { get; set; }          // 列类型：Value/UpperLimit/LowerLimit/Result
        public string Unit { get; set; }               // 单位
        public int TestItemIndex { get; set; }         // 在测试项数组中的索引
    }
}
