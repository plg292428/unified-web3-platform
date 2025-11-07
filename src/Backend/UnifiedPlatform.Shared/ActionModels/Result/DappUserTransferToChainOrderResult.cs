namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户转账到链上订单响应数据
    /// </summary>
    public class DappUserTransferToChainOrderResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 区块链交易ID
        /// </summary>
        public required string TransactionId { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public int TransactionStatus { get; set; }

        /// <summary>
        /// 交易状态名
        /// </summary>
        public required string TransactionStatusName { get; set; }

        /// <summary>
        /// 转账金额
        /// </summary>
        public decimal ClientSentToken { get; set; }

        /// <summary>
        /// 实际到账金额
        /// </summary>
        public decimal ServerCheckedToken { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
