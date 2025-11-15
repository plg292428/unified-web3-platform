namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户链上资产变动记录响应数据
    /// </summary>
    public class DappUserOnChainAssetsChangeRecordResult
    {
        /// <summary>
        /// 账变ID
        /// </summary>
        public int Id { get; set;}

        /// <summary>
        /// 变动类型
        /// </summary>
        public int ChangeType { get; set; }

        /// <summary>
        /// 变动类型名称
        /// </summary>
        public required string ChangeTypeName { get; set; }

        /// <summary>
        /// 变动金额
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// 变动前金额
        /// </summary>
        public decimal Before { get; set; }

        /// <summary>
        /// 变动后金额
        /// </summary>
        public decimal After { get; set; }

        /// <summary>
        /// 备注内容
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 账变时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

