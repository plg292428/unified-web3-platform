using System.ComponentModel.DataAnnotations;

namespace HFastKit.AspNetCore.Shared
{
    /// <summary>
    /// 分页和日期查询请求
    /// </summary>
    public class QueryByDateRequest : IValidatableObject
    {
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        // 最小查询日期
        private static readonly DateTime _minDate = new(2020, 1, 1);

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((StartDate is null && EndDate is not null) || (StartDate is not null && EndDate is null))
            {
                yield return new ValidationResult($"查询时间参数错误，起始时间或结束时间不完整");
                yield break;
            }

            StartDate ??= _minDate;
            if (StartDate < _minDate)
            {
                yield return new ValidationResult($"查询起始时间参数错误，起始时间无效");
                yield break;
            }
            StartDate = StartDate.Value.Date.ToUniversalTime();

            EndDate ??= DateTime.Now;
            EndDate = EndDate.Value.Date.AddDays(1).ToUniversalTime();
            if (EndDate < StartDate)
            {
                yield return new ValidationResult($"查询结束时间参数错误，结束时间不可以小于起始时间");
                yield break;
            }
        }
    }
}

