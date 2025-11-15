namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 资产变化历史记录结果
    /// </summary>
    public class AssetChangeHistoryResult
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 变化类型
        /// </summary>
        public int ChangeType { get; set; }

        /// <summary>
        /// 变化类型名称
        /// </summary>
        public string ChangeTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 变化金额
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// 变化前余额
        /// </summary>
        public decimal Before { get; set; }

        /// <summary>
        /// 变化后余额
        /// </summary>
        public decimal After { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

