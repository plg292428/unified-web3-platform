namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 更新库存请求
    /// </summary>
    public class InventoryUpdateRequest
    {
        /// <summary>
        /// 可用库存数量
        /// </summary>
        public int? QuantityAvailable { get; set; }

        /// <summary>
        /// 预留库存数量
        /// </summary>
        public int? QuantityReserved { get; set; }
    }
}

