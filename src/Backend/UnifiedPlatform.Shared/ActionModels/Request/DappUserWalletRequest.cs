using HFastKit.AspNetCore.Shared.Text;
using System.ComponentModel.DataAnnotations;

namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户钱包请求
    /// </summary>
    public class DappUserWalletRequest : IValidatableObject
    {
        /// <summary>
        /// 钱包地址
        /// </summary>
        public string WalletAddress { get; set; } = string.Empty;

        /// <summary>
        /// 区块链网络ID
        /// </summary>
        public ChainNetwork ChainId { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.IsDefined(ChainId))
            {
                yield return new ValidationResult($"Chain network error");
            }
            if (ChainId == ChainNetwork.Invalid)
            {
                yield return new ValidationResult($"Chain network error");
            }
            if (!FormatValidate.IsEthereumAddress(WalletAddress))
            {
                yield return new ValidationResult($"Wallet address format error");
            }
        }
    }
}
