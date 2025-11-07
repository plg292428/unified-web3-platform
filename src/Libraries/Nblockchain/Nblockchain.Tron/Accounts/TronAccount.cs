using Nblockchain.Signer;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Nblockchain.Tron
{
    /// <summary>
    /// Tron 账号
    /// </summary>
    public interface ITronAccount
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; }
    }

    /// <summary>
    /// Tron 账号
    /// </summary>
    public class TronAccount : ITronAccount
    {
        private TronECKey _ecKey;

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get
            {
               return _ecKey.GetPublicAddress();
            }
        }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey => _ecKey.GetPubKey().ToHex();

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey => _ecKey.GetPrivateKey();

        /// <summary>
        /// Tron 账号
        /// </summary>
        /// <param name="key">私钥</param>
        public TronAccount(TronECKey key)
        {
            _ecKey = key;
        }

        /// <summary>
        /// Tron 账号
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="network">Tron 网络</param>
        public TronAccount(string privateKey, TronNetwork network = TronNetwork.MainNet) : this(new TronECKey(privateKey, network))
        {
        }

        /// <summary>
        /// Tron 账号
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="network">Tron 网络</param>
        public TronAccount(byte[] privateKey, TronNetwork network = TronNetwork.MainNet) : this(new TronECKey(privateKey, true, network))
        {
        }
    }
}
