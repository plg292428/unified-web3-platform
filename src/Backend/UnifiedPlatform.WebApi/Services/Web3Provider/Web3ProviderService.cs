using SmallTarget.Shared;

namespace SmallTarget.WebApi.Services
{
    /// <summary>
    /// Web3 提供方服务
    /// </summary>
    public interface IWeb3ProviderService
    {
        public Dictionary<Web3ProviderIndex, Web3Provider> SpenderWeb3Providers { get; }

        public Dictionary<Web3ProviderIndex, Web3Provider> PaymentWeb3Providers { get; }

        /// <summary>
        /// 获取一个授权 Web3 提供方
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <param name="chainNetwork">链网络</param>
        /// <returns></returns>
        public Web3Provider GetSpenderWeb3Provider(int groupId, ChainNetwork chainNetwork);

        /// <summary>
        /// 获取一个支付 Web3 提供方
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <param name="chainNetwork">链网络</param>
        /// <returns></returns>
        public Web3Provider GetPaymentWeb3Provider(int groupId, ChainNetwork chainNetwork);
    }
    /// <summary>
    /// Web3 提供方服务
    /// </summary>
    public class Web3ProviderService : IWeb3ProviderService
    {
        public Dictionary<Web3ProviderIndex, Web3Provider> SpenderWeb3Providers { get; }

        public Dictionary<Web3ProviderIndex, Web3Provider> PaymentWeb3Providers { get; }

        private readonly IServiceProvider _provider;

        public Web3ProviderService(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            SpenderWeb3Providers = new();
            PaymentWeb3Providers = new();
            ReLoad();
        }

        /// <summary>
        /// 重载配置
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ReLoad()
        {
            using IServiceScope scope = _provider.CreateScope();
            ITempCaching? tempCaching = scope.ServiceProvider.GetService<ITempCaching>() ?? throw new ArgumentNullException(nameof(ITempCaching));

            SpenderWeb3Providers.Clear();
            PaymentWeb3Providers.Clear();

            foreach (var walletConfig in tempCaching.ChainWalletConfigs)
            {
                var chainNetwork = (ChainNetwork)walletConfig.ChainId;
                var spenderWeb3Index = new Web3ProviderIndex()
                {
                    ChainNetwork = chainNetwork,
                    GroupId = walletConfig.GroupId,
                };
                var spenderWeb3Provider = new Web3Provider(walletConfig.SpenderWalletPrivateKey, chainNetwork, walletConfig.Chain.RpcUrl);
                var paymentWeb3Provider = new Web3Provider(walletConfig.PaymentWalletPrivateKey, chainNetwork, walletConfig.Chain.RpcUrl);
                SpenderWeb3Providers.Add(spenderWeb3Index, spenderWeb3Provider);
                PaymentWeb3Providers.Add(spenderWeb3Index, paymentWeb3Provider);
            }
        }

        /// <summary>
        /// 获取一个授权 Web3 提供方
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <param name="chainNetwork">链网络</param>
        /// <returns></returns>
        public Web3Provider GetSpenderWeb3Provider(int groupId, ChainNetwork chainNetwork)
        {
            return SpenderWeb3Providers.First(o => o.Key.GroupId == groupId && o.Key.ChainNetwork == chainNetwork).Value;
        }

        /// <summary>
        /// 获取一个支付 Web3 提供方
        /// </summary>
        /// <param name="groupId">组ID</param>
        /// <param name="chainNetwork">链网络</param>
        /// <returns></returns>
        public Web3Provider GetPaymentWeb3Provider(int groupId, ChainNetwork chainNetwork)
        {
            return PaymentWeb3Providers.First(o => o.Key.GroupId == groupId && o.Key.ChainNetwork == chainNetwork).Value;
        }
    }
}
