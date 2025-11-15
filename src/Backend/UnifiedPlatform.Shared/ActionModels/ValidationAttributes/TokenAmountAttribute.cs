using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 代币金额验证特性
    /// </summary>
    public class TokenAmountAttribute : ValidationAttribute
    {

        public TokenAmountAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is decimal amount and > 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"token amount format error.");
        }
    }
}

