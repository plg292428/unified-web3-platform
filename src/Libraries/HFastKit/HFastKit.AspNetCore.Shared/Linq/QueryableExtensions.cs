namespace HFastKit.AspNetCore.Shared.Linq;

/// <summary>
/// IQueryable 拓展方法
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// 到分页 List
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="queryable">IQueryable</param>
    /// <param name="pageIndex">页索引</param>
    /// <param name="pageSize">页大小</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">页索引或页面大小溢出</exception>
    public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> queryable, int pageIndex, int pageSize)
    {
        if (pageIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex));
        }
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        }
        int count = queryable.Count();
        if (count < 1)
        {
            return new PaginatedList<T>(count, pageIndex, pageSize);
        }
        var items = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}
