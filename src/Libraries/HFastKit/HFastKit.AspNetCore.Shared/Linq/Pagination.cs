using System.Text.Json.Serialization;

namespace HFastKit.AspNetCore.Shared.Linq
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// 页面索引
        /// </summary>
        [JsonInclude]
        public int PageIndex { get; private set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        [JsonInclude]
        public int PageSize { get; private set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [JsonInclude]
        public int TotalRecords { get; private set; }

        /// <summary>
        /// 页面数
        /// </summary>
        [JsonInclude]
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);

        /// <summary>
        /// 是否有上一页
        /// </summary>
        [JsonInclude]
        public bool HasPreviousPage => (PageIndex > 1);

        /// <summary>
        /// 是否有下一页
        /// </summary>
        [JsonInclude]
        public bool HasNextPage => (PageIndex < TotalPages);

        /// <summary>
        /// 分页信息
        /// </summary>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="pageIndex">页大小</param>
        /// <param name="pageSize">页索引</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Pagination(int totalRecords, int pageIndex, int pageSize)
        {
            if (totalRecords < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalRecords));
            }
            if (pageIndex < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }
            TotalRecords = totalRecords;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
