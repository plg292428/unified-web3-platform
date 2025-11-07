using System.Text.Json.Serialization;

namespace HFastKit.AspNetCore.Shared.Linq
{
    /// <summary>
    /// 分页 List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T>
    {
        private const int _minTotalRecords = 0;
        private const int _minPageIndex = 1;
        private const int _minPageSize =1;

        /// <summary>
        /// 分页信息
        /// </summary>
        [JsonInclude]
        public Pagination Pagination { get; private set; }

        /// <summary>
        /// 项目
        /// </summary>
        [JsonInclude]
        public List<T> Items { get; private set; }

        /// <summary>
        /// 分页 List
        /// </summary>
        /// <param name="items">项目</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>

        public PaginatedList(List<T> items, int totalRecords, int pageIndex, int pageSize)
        {
            Pagination = new(totalRecords, pageIndex, pageSize);
            Items = items;
        }

        /// <summary>
        /// 分页 List
        /// </summary>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        public PaginatedList(int totalRecords, int pageIndex, int pageSize) : this(new List<T>(), totalRecords, pageIndex, pageSize)
        {
        }

        /// <summary>
        /// 分页 List
        /// </summary>
        /// <param name="items">项目</param>
        /// <param name="pagination">分页信息</param>

        public PaginatedList(List<T> items, Pagination pagination) : this(items, pagination.TotalRecords, pagination.PageIndex, pagination.PageSize)
        {

        }

        /// <summary>
        /// 分页 List
        /// </summary>
        /// <param name="pagination">分页信息</param>
        public PaginatedList(Pagination pagination) : this(new List<T>(), pagination)
        {
        }

        /// <summary>
        /// 分页 List
        /// </summary>
        public PaginatedList() : this(new List<T>(), new(_minTotalRecords, _minPageIndex, _minPageSize))
        { 
        }

        public static PaginatedList<T> Empty()
        {
            return new PaginatedList<T>();
        }
    }
}
