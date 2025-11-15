namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 库存结果
    /// </summary>
    public class InventoryResult
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 可用库存数量
        /// </summary>
        public int QuantityAvailable { get; set; }

        /// <summary>
        /// 预留库存数量
        /// </summary>
        public int QuantityReserved { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

