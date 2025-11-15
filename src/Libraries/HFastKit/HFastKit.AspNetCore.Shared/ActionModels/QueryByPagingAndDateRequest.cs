using System.ComponentModel.DataAnnotations;

namespace HFastKit.AspNetCore.Shared
{
    /// <summary>
    /// 分页和日期查询请求
    /// </summary>
    public class QueryByPagingAndDateRequest : QueryByDateRequest
    {
        /// <summary>
        /// 查询页索引
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; }
    }
}

