namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户转账到钱包订单响应数据
    /// </summary>
    public class DappUserTransferToWallerOrderResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 订单状态名
        /// </summary>
        public required string OrderStatusName { get; set; }

        /// <summary>
        /// 申请转账金额
        /// </summary>
        public decimal RequestAmount { get; set; }

        /// <summary>
        /// 实际到账金额
        /// </summary>
        public decimal RealAmount { get; set; }

        /// <summary>
        /// 服务费
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

