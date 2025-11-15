using Microsoft.Extensions.Options;

namespace Nblockchain.Tron
{
    /// <summary>
    /// Tron Web 工厂接口
    /// </summary>
    public interface ITronWebFactory
    {
        public TronWeb Create(ITronAccount account);

        public TronWeb Create(string privateKey);
    }

    /// <summary>
    /// Tron Web 工厂
    /// </summary>
    public class TronWebFactory : ITronWebFactory
    {
        private readonly IOptions<TronWebOptions> _options;

        public TronWebFactory(IOptions<TronWebOptions> options)
        {
            _options = options;
        }

        public TronWeb Create(ITronAccount account)
        {
            return new TronWeb(account, _options.Value);
        }

        public TronWeb Create(string privateKey)
        {
            return Create(new TronAccount(privateKey, _options.Value.Network));
        }
    }
}

