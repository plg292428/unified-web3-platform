using HFastKit.AspNetCore.Shared.Text;
using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 用户名验证特性
    /// </summary>
    public class UserNameAttribute : ValidationAttribute
    {
        public UserNameAttribute()
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string username && FormatValidate.IsUsername(username, 4, 30))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"Username format error.");
        }
    }
}

