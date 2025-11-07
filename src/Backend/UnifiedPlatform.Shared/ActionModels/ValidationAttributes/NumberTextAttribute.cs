using HFastKit.AspNetCore.Shared.Text;
using System.ComponentModel.DataAnnotations;

namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 数字文本验证特性
    /// </summary>
    public class NumberTextAttribute : ValidationAttribute
    {
        public NumberTextAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string numberText && FormatValidate.IsNumber(numberText))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"Number text format error.");
        }
    }
}
