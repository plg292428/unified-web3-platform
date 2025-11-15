namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 调整库存请求
    /// </summary>
    public class InventoryAdjustRequest
    {
        /// <summary>
        /// 调整数量（正数为增加，负数为减少）
        /// </summary>
        public int Adjustment { get; set; }
    }
}

