using HFastKit.AspNetCore.Shared.Text;
using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 密码验证特性
    /// </summary>
    public class PasswordAttribute : ValidationAttribute
    {
        public PasswordAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string password && FormatValidate.IsPassword(password, 6, 30))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"Password format error");
        }
    }
}

