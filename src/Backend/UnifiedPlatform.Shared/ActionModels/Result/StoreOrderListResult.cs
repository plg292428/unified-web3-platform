using System.Collections.Generic;

namespace UnifiedPlatform.Shared.ActionModels.Result
{
    public class StoreOrderListResult
    {
        public IReadOnlyList<StoreOrderSummaryResult> Items { get; set; } = new List<StoreOrderSummaryResult>();

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
