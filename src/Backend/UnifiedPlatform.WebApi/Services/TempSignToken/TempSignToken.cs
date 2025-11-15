using Nethereum.Signer;
using UnifiedPlatform.Shared;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace UnifiedPlatform.WebApi.Services
{
    /// <summary>
    /// 临时签名令牌接口
    /// </summary>
    public interface ITempSignToken
    {
        /// <summary>
        /// 登录令牌缓存
        /// </summary>
        public ConcurrentDictionary<WalletInfo, SignToken> Caching { get; }

        /// <summary>
        /// 尝试创建或获取签名令牌
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="walletAddress"></param>
        /// <param name="expirationSeconds"></param>
        /// <param name="signinToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool TryCreateOrGet(ChainNetwork chainId, string walletAddress, int expirationSeconds, [NotNullWhen(true)] out SignToken? signinToken);

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="chainId">链Id</param>
        /// <param name="walletAddress">钱包地址</param>
        /// <param name="signText">签名内容</param>
        /// <returns></returns>
        public SignTokenVerifyResult VerifySign(ChainNetwork chainId, string walletAddress, string? signedText);
    }

    /// <summary>
    /// 临时签名令牌
    /// </summary>
    public class TempSignToken : ITempSignToken
    {
        /// <summary>
        /// 登录令牌缓存
        /// </summary>
        public ConcurrentDictionary<WalletInfo, SignToken> Caching { get; } = new();

        /// <summary>
        /// 尝试创建或获取签名令牌
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="walletAddress"></param>
        /// <param name="expirationSeconds"></param>
        /// <param name="signinToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool TryCreateOrGet(ChainNetwork chainId, string walletAddress, int expirationSeconds, [NotNullWhen(true)] out SignToken? signinToken)
        {
            // 清理过期令牌
            ClearExpiredSigninToken();

            signinToken = Caching.FirstOrDefault(o => o.Key.ChainId == chainId && o.Key.WalletAddress == walletAddress).Value;
            if (signinToken is not null)
            {
                return true;
            }
            if (expirationSeconds < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(expirationSeconds));
            }
            TimeSpan expirationTime = new TimeSpan(0, 0, expirationSeconds);
            signinToken = new(expirationTime);
            if (!Caching.TryAdd(new(chainId, walletAddress), signinToken))
            {
                signinToken = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="chainId">链Id</param>
        /// <param name="walletAddress">钱包地址</param>
        /// <param name="signedText">签名内容</param>
        /// <returns></returns>
        public SignTokenVerifyResult VerifySign(ChainNetwork chainId, string walletAddress, string? signedText)
        {
            // 清理过期令牌
            ClearExpiredSigninToken();

            SignTokenVerifyResult result = new()
            {
                IsVerified = false
            };

            if (string.IsNullOrWhiteSpace(signedText))
            {
                result.ErrorMessage = "Signature verification error";
                return result;
            }

            if (!Caching.TryGetValue(new(chainId, walletAddress), out SignToken? signinToken))
            {
                result.ErrorMessage = "Token does not exist or has expired";
                return result;
            }

            var signer = new EthereumMessageSigner();
            var addressRec = signer.EcRecover(Encoding.UTF8.GetBytes(signinToken.SignatureContent), signedText);
            if (addressRec.ToLower() != walletAddress.ToLower())
            {
                result.ErrorMessage = "Signature verification error";
                return result;
            }
            result.IsVerified = true;
            result.Guid = signinToken.Guid;
            return result;
        }

        // <summary>
        /// 清理过期登录令牌缓存
        /// </summary>
        private void ClearExpiredSigninToken()
        {
            if (Caching.IsEmpty || Caching.Count < 1)
            {
                return;
            }

            foreach (var item in Caching.Where(o => o.Value.ExpirationTime < DateTime.UtcNow))
            {
                Caching.TryRemove(item.Key, out _);
            }
        }
    }
}

