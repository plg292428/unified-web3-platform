using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 取消订单请求。
    /// </summary>
    public class StoreOrderCancelRequest
    {
        [Range(1, int.MaxValue)]
        public int Uid { get; set; }

        [MaxLength(512)]
        public string? Reason { get; set; }
    }
}

