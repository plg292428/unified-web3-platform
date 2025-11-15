using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    public class StoreProductListRequest
    {
        private const int MaxPageSize = 50;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, MaxPageSize)]
        public int PageSize { get; set; } = 12;

        [StringLength(128)]
        public string? Keyword { get; set; }

        public int? CategoryId { get; set; }

        public int? ChainId { get; set; }

        /// <summary>
        /// 排序方式：price_asc, price_desc, time_desc, time_asc, name_asc, name_desc
        /// </summary>
        [StringLength(32)]
        public string? SortBy { get; set; }

        /// <summary>
        /// 价格范围：最低价格
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// 价格范围：最高价格
        /// </summary>
        public decimal? MaxPrice { get; set; }

        public void Normalize()
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1) PageSize = 1;
            if (PageSize > MaxPageSize) PageSize = MaxPageSize;
        }
    }
}

