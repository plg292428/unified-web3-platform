using Microsoft.EntityFrameworkCore;
using SmallTarget.DbService;
using UnifiedPlatform.DbService.Entities;
using System.Diagnostics.CodeAnalysis;
using HFastKit.Text;
using Nethereum.Web3.Accounts;
using HFastKit.AspNetCore.Shared.Text;

namespace UnifiedPlatform.WebApi.Services
{
    /// <summary>
    /// 临时缓存接口
    /// </summary>
    public interface ITempCaching
    {
        /// <summary>
        /// 配置缓存
        /// </summary>
        public GlobalConfig GlobalConfig { get; }

        /// <summary>
        /// Dapp 链接
        /// </summary>
        public List<Uri> DappUrls { get; }

        /// <summary>
        /// 区块链网络配置
        /// </summary>
        public List<ChainNetworkConfig> ChainNetworkConfigs { get; }

        /// <summary>
        /// 钱包配置缓存
        /// </summary>
        public List<ChainWalletConfig> ChainWalletConfigs { get; }

        /// <summary>
        /// 钱包配置缓存（分组）
        /// </summary>
        public List<IGrouping<int, ChainWalletConfig>> ChainWalletConfigsByGroup { get; }

        /// <summary>
        /// 区块链代币配置
        /// </summary>
        public List<ChainTokenConfig> ChainTokenConfigs { get; }

        /// <summary>
        /// 用户等级配置
        /// </summary>
        public List<UserLevelConfig> UserLevelConfigs { get; }

        /// <summary>
        /// 管理类型配置缓存
        /// </summary>
        public List<ManagerTypeConfig> ManagerTypeConfigs { get; }

        public void ReLoad();
    }

    /// <summary>
    /// 临时缓存
    /// </summary>
    public class TempCaching : ITempCaching
    {
        /// <summary>
        /// 全局配置缓存
        /// </summary>
        public GlobalConfig GlobalConfig 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _globalConfig ?? throw new InvalidOperationException("缓存未初始化");
            } 
            private set => _globalConfig = value; 
        }
        private GlobalConfig _globalConfig;

        /// <summary>
        /// Dapp 链接
        /// </summary>
        public List<Uri> DappUrls 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _dappUrls ?? new List<Uri>();
            } 
            private set => _dappUrls = value; 
        }
        private List<Uri> _dappUrls;

        /// <summary>
        /// 区块链网络配置缓存
        /// </summary>
        public List<ChainNetworkConfig> ChainNetworkConfigs 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _chainNetworkConfigs ?? new List<ChainNetworkConfig>();
            } 
            private set => _chainNetworkConfigs = value; 
        }
        private List<ChainNetworkConfig> _chainNetworkConfigs;

        /// <summary>
        /// 钱包配置缓存
        /// </summary>
        public List<ChainWalletConfig> ChainWalletConfigs 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _chainWalletConfigs ?? new List<ChainWalletConfig>();
            } 
            private set => _chainWalletConfigs = value; 
        }
        private List<ChainWalletConfig> _chainWalletConfigs;

        /// <summary>
        /// 钱包配置缓存（分组）
        /// </summary>
        public List<IGrouping<int, ChainWalletConfig>> ChainWalletConfigsByGroup 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _chainWalletConfigsByGroup ?? new List<IGrouping<int, ChainWalletConfig>>();
            } 
            private set => _chainWalletConfigsByGroup = value; 
        }
        private List<IGrouping<int, ChainWalletConfig>> _chainWalletConfigsByGroup;

        /// <summary>
        /// 区块链代币配置缓存
        /// </summary>
        public List<ChainTokenConfig> ChainTokenConfigs 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _chainTokenConfigs ?? new List<ChainTokenConfig>();
            } 
            private set => _chainTokenConfigs = value; 
        }
        private List<ChainTokenConfig> _chainTokenConfigs;

        /// <summary>
        /// 用户等级配置
        /// </summary>
        public List<UserLevelConfig> UserLevelConfigs 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _userLevelConfigs ?? new List<UserLevelConfig>();
            } 
            private set => _userLevelConfigs = value; 
        }
        private List<UserLevelConfig> _userLevelConfigs;

        /// <summary>
        /// 管理类型配置缓存
        /// </summary>
        public List<ManagerTypeConfig> ManagerTypeConfigs 
        { 
            get 
            { 
                InitializeIfNeeded();
                return _managerTypeConfigs ?? new List<ManagerTypeConfig>();
            } 
            private set => _managerTypeConfigs = value; 
        }
        private List<ManagerTypeConfig> _managerTypeConfigs;

        private readonly IServiceProvider _provider;
        private readonly object _lock = new object();
        private bool _isInitialized = false;

        public TempCaching(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            
            // 延迟加载：不在构造函数中立即加载，避免启动时数据库连接失败导致服务无法启动
            // 只有在第一次访问属性时才会加载数据
            InitializeIfNeeded();
        }

        /// <summary>
        /// 初始化缓存（延迟加载）
        /// </summary>
        private void InitializeIfNeeded()
        {
            if (_isInitialized) return;
            
            lock (_lock)
            {
                if (_isInitialized) return;
                
                try
                {
                    ReLoad();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    // 如果数据库连接失败，记录警告但不阻止服务启动
                    // 这允许服务在没有数据库的情况下也能启动（用于开发测试）
                    System.Diagnostics.Debug.WriteLine($"[TempCaching] 初始化失败: {ex.Message}");
                    // 初始化空数据，避免NullReferenceException
                    InitializeEmptyData();
                }
            }
        }

        /// <summary>
        /// 初始化空数据（数据库不可用时使用）
        /// </summary>
        private void InitializeEmptyData()
        {
            _globalConfig = new GlobalConfig { MiningRewardIntervalHours = 1, MinAiTradingMinutes = 1 };
            _dappUrls = new List<Uri>();
            _chainNetworkConfigs = new List<ChainNetworkConfig>();
            _chainWalletConfigs = new List<ChainWalletConfig>();
            _chainWalletConfigsByGroup = new List<IGrouping<int, ChainWalletConfig>>();
            _chainTokenConfigs = new List<ChainTokenConfig>();
            _userLevelConfigs = new List<UserLevelConfig>();
            _managerTypeConfigs = new List<ManagerTypeConfig>();
            _isInitialized = true;
        }

        /// <summary>
        /// 重新加载缓存
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DataConfigurationException"></exception>
        [MemberNotNull(nameof(_globalConfig), nameof(_dappUrls), nameof(_chainNetworkConfigs), nameof(_chainWalletConfigs), nameof(_chainWalletConfigsByGroup), nameof(_chainTokenConfigs), nameof(_userLevelConfigs), nameof(_managerTypeConfigs))]
        public void ReLoad()
        {
            using IServiceScope scope = _provider.CreateScope();
            StDbContext? dbContext = scope.ServiceProvider.GetService<StDbContext>() ?? throw new ArgumentNullException(nameof(dbContext));

            // 全局配置
            _globalConfig = dbContext.GlobalConfigs
                .AsNoTracking()
                .OrderBy(o => o.Id)
                .FirstOrDefault() ?? throw new DataConfigurationException($"Unable to read configuration data: {nameof(GlobalConfig)}");
            if (_globalConfig.MiningRewardIntervalHours < 1)
            {
                throw new DataConfigurationException($"{nameof(GlobalConfig)} error, {nameof(_globalConfig.MiningRewardIntervalHours)} invalid, minimum configuration value is 1");
            }
            if (_globalConfig.MinAiTradingMinutes < 1)
            {
                throw new DataConfigurationException($"{nameof(GlobalConfig)} error, {nameof(_globalConfig.MinAiTradingMinutes)} invalid, minimum configuration value is 1");
            }    
            if (_globalConfig.MaxAiTradingMinutes < _globalConfig.MinAiTradingMinutes)
            {
                throw new DataConfigurationException($"{nameof(GlobalConfig)} error, configuration {nameof(_globalConfig.MaxAiTradingMinutes)} must be greater than configuration {nameof(_globalConfig.MinAiTradingMinutes)}");
            }
            if (_globalConfig.MiningSpeedUpRequiredOnChainAssetsRate < 0.1m)
            {
                throw new DataConfigurationException($"{nameof(GlobalConfig)} error, {nameof(_globalConfig.MiningSpeedUpRequiredOnChainAssetsRate)} invalid, minimum configuration value is 0.1");
            }
            if (_globalConfig.MiningSpeedUpRewardIncreaseRate < 0.01m)
            {
                throw new DataConfigurationException($"{nameof(GlobalConfig)} error, {nameof(_globalConfig.MiningSpeedUpRewardIncreaseRate)} invalid, minimum configuration value is 0.01");
            }
            var urlList = _globalConfig.DappUrls.Split(',').ToList();
            _dappUrls = new();
            foreach (var url in urlList)
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                {
                    throw new DataConfigurationException($"{nameof(GlobalConfig)} error, {nameof(GlobalConfig.DappUrls)} invalid");
                }
                if (uri.Host != "localhost" && uri.Scheme != "https")
                {
                    throw new DataConfigurationException($"{nameof(GlobalConfig)} error, {uri} invalid, only supports HTTPS protocol url");
                }
                _dappUrls.Add(uri);
            }
            if(_dappUrls.Count < 1)
            {
                throw new DataConfigurationException($"Configuration data is empty: {nameof(GlobalConfig.DappUrls)}");
            }


            // 区块链网络配置
            _chainNetworkConfigs = dbContext.ChainNetworkConfigs
                .Include(o => o.ChainTokenConfigs.Where(p=> p.Enabled))
                .AsNoTracking()
                .Where(o=> o.Enabled)
                .ToList() ?? throw new DataConfigurationException($"Unable to read configuration data: {nameof(ChainNetworkConfigs)}");
            if (_chainNetworkConfigs.Count < 1)
            {
                throw new DataConfigurationException($"Configuration data is empty: {nameof(ChainNetworkConfigs)}");
            }
            foreach (var networkConfig in _chainNetworkConfigs)
            {
                if (networkConfig.MinAssetsToChainLimit <= 1)
                {
                    throw new DataConfigurationException($"{nameof(networkConfig)} error, {nameof(networkConfig.MinAssetsToChainLimit)} invalid, minimum configuration value is 1");
                }
                if (networkConfig.MaxAssetsToChaintLimit < networkConfig.MinAssetsToChainLimit)
                {
                    throw new DataConfigurationException($"{nameof(ChainNetworkConfigs)} error, configuration {nameof(networkConfig.MaxAssetsToChaintLimit)} must be greater than configuration {nameof(networkConfig.MinAssetsToChainLimit)}");
                }
                if (networkConfig.MinAssetsToWalletLimit <= 1)
                {
                    throw new DataConfigurationException($"{nameof(networkConfig)} error, {nameof(networkConfig.MinAssetsToWalletLimit)} invalid, minimum configuration value is 1");
                }
                if (networkConfig.MinAssetsToWalletLimit < networkConfig.AssetsToWalletServiceFeeBase)
                {
                    throw new DataConfigurationException($"{nameof(ChainNetworkConfigs)} error, configuration {nameof(networkConfig.MinAssetsToWalletLimit)} must be greater than configuration {nameof(networkConfig.AssetsToWalletServiceFeeBase)}");
                }
                if (networkConfig.MaxAssetsToWalletLimit < networkConfig.MinAssetsToWalletLimit)
                {
                    throw new DataConfigurationException($"{nameof(ChainNetworkConfigs)} error, configuration {nameof(networkConfig.MaxAssetsToWalletLimit)} must be greater than configuration {nameof(networkConfig.MinAssetsToWalletLimit)}");
                }
                if (!Uri.TryCreate(networkConfig.RpcUrl, UriKind.Absolute, out Uri? uri))
                {
                    throw new DataConfigurationException($"{nameof(ChainNetworkConfigs)} error, {nameof(networkConfig.RpcUrl)} invalid");
                }
            }

            // 区块链代币配置
            _chainTokenConfigs = dbContext.ChainTokenConfigs
                .Include(o => o.Chain)
                .AsNoTracking()
                .Where(o => o.Enabled)
                .ToList();
            foreach (var tokenConfig in _chainTokenConfigs)
            {
                if (!FormatValidate.IsEthereumAddress(tokenConfig.ContractAddress))
                {
                    throw new DataConfigurationException($"{nameof(ChainNetworkConfigs)} error, {nameof(tokenConfig.ContractAddress)} invalid");
                }
            }

            // 区块链钱包配置
            _chainWalletConfigs = dbContext.ChainWalletConfigs
                .Include(o => o.Chain)
                .AsNoTracking()
                .ToList();
            _chainWalletConfigsByGroup = _chainWalletConfigs.GroupBy(o => o.GroupId).ToList();

            // 检查授权钱包配置
            if (!_chainWalletConfigsByGroup.Any(o => o.Key == _globalConfig.ChainWalletConfigGroupId))
            {
                throw new DataConfigurationException($"Global chain wallet config group id error, group id does not exist");
            }
            foreach (var walletConfigs in _chainWalletConfigsByGroup)
            {
                if (walletConfigs.Count() != _chainNetworkConfigs.Count)
                {
                    throw new DataConfigurationException($"Chain wallet config error, configuration {nameof(ChainWalletConfigsByGroup)} and Configuration {nameof(ChainNetworkConfigs)} do not match");
                }
                foreach (var walletConfig in walletConfigs)
                {
                    if (!FormatValidate.IsEthereumAddress(walletConfig.ReceiveWalletAddress))
                    {
                        throw new DataConfigurationException($"{nameof(ChainWalletConfigs)} error, Group id {walletConfig.GroupId} {nameof(walletConfig.ReceiveWalletAddress)} invalid");
                    }
                    try
                    {
                        Account account = new(walletConfig.SpenderWalletPrivateKey);
                        if (account.Address.ToLower() != walletConfig.SpenderWalletAddress.ToLower())
                        {
                            throw new DataConfigurationException($"{nameof(ChainWalletConfigs)} error, Group id {walletConfig.GroupId} {nameof(walletConfig.SpenderWalletPrivateKey)} and Configuration {nameof(walletConfig.SpenderWalletAddress)} do not match");
                        }
                        account = new(walletConfig.PaymentWalletPrivateKey);
                        if (account.Address.ToLower() != walletConfig.PaymentWalletAddress.ToLower())
                        {
                            throw new DataConfigurationException($"{nameof(ChainWalletConfigs)} error, Group id {walletConfig.GroupId} {nameof(walletConfig.PaymentWalletPrivateKey)} and Configuration {nameof(walletConfig.PaymentWalletAddress)} do not match");
                        }
                    }
                    catch
                    {
                        
                        throw new DataConfigurationException($"{nameof(ChainWalletConfigs)} error, Group id {walletConfig.GroupId} {nameof(walletConfig.SpenderWalletPrivateKey)} invalid");
                    }
                    
                }
            }

            // 用户等级配置
            _userLevelConfigs = dbContext.UserLevelConfigs
                .AsNoTracking()
                .OrderBy(o=> o.UserLevel)
                .ToList() ?? throw new DataConfigurationException($"Unable to read configuration data: {nameof(UserLevelConfigs)}");
            if (_userLevelConfigs.Count < 1)
            {
                throw new DataConfigurationException($"Configuration data is empty: {nameof(UserLevelConfigs)}");
            }
            for (int i = 0; i < _userLevelConfigs.Count; i++)
            {
                var levelConfig = _userLevelConfigs[i];
                if (levelConfig.UserLevel != i)
                {
                    throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, wrong user level order");
                }

                if (i > 0)
                {
                    // 同级
                    if (levelConfig.MinEachAiTradingRewardRate <= 0)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MinEachAiTradingRewardRate)} value invalid");
                    }
                    if (levelConfig.MaxEachAiTradingRewardRate <= levelConfig.MinEachAiTradingRewardRate)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MaxEachAiTradingRewardRate)} value invalid");
                    }
                    if (levelConfig.MinEachMiningRewardRate <= 0)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MinEachMiningRewardRate)} value invalid");
                    }
                    if (levelConfig.MaxEachMiningRewardRate <= levelConfig.MinEachMiningRewardRate)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MaxEachMiningRewardRate)} value invalid");
                    }

                    // 对比上级
                    var lastLevelConfig = _userLevelConfigs[i - 1];
                    if (levelConfig.RequiresValidAsset < lastLevelConfig.RequiresValidAsset)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.RequiresValidAsset)} invalid");
                    }
                    if (levelConfig.AvailableAiTradingAssetsRate < lastLevelConfig.AvailableAiTradingAssetsRate)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.AvailableAiTradingAssetsRate)} invalid");
                    }
                    if (levelConfig.DailyAiTradingLimitTimes < lastLevelConfig.DailyAiTradingLimitTimes)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.DailyAiTradingLimitTimes)} invalid");
                    }
                    if (levelConfig.MinEachAiTradingRewardRate < lastLevelConfig.MinEachAiTradingRewardRate)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MinEachAiTradingRewardRate)} invalid");
                    }
                    if (levelConfig.MaxEachAiTradingRewardRate < lastLevelConfig.MaxEachAiTradingRewardRate)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MaxEachAiTradingRewardRate)} invalid");
                    }
                    if (levelConfig.MinEachMiningRewardRate < lastLevelConfig.MinEachMiningRewardRate)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MinEachMiningRewardRate)} invalid");
                    }
                    if (levelConfig.MaxEachMiningRewardRate < lastLevelConfig.MaxEachMiningRewardRate)
                    {
                        throw new DataConfigurationException($"{nameof(UserLevelConfigs)} error, {nameof(levelConfig.MaxEachMiningRewardRate)} invalid");
                    }
                }
            }

            // 员工类型配置
            _managerTypeConfigs = dbContext.ManagerTypeConfigs.AsNoTracking()?.ToList() ?? throw new DataConfigurationException($"Unable to read configuration data: {nameof(ManagerTypeConfigs)}");
            if (_managerTypeConfigs.Count < 1)
            {
                throw new DataConfigurationException($"Configuration data is empty: {nameof(ManagerTypeConfigs)}");
            }
        }
    }
}

