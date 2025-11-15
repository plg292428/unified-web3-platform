using HFastKit.AspNetCore.Shared.Text;
using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 设置主要代币请求
    /// </summary>
    public class DappUserTransactionRequest : IValidatableObject
    {
        /// <summary>
        /// 代币ID
        /// </summary>
        public int TokenId { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
        public required string TransactionId { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public required string ValueText { get; set; }


        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!FormatValidate.IsTransactionId(TransactionId))
            {
                yield return new ValidationResult($"Transaction id format error");
            }
        }
    }
}

