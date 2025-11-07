using System.ComponentModel.DataAnnotations;

namespace HFastKit.AspNetCore.Shared
{
    /// <summary>
    /// 分页查询请求
    /// </summary>
    public class QueryByPagingRequest
    {
        /// <summary>
        /// 查询页索引
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage ="Query page index error")]
        public int PageIndex { get; set; }
    }
}
