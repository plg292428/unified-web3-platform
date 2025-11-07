using HFastKit.AspNetCore.Services.Captcha;
using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallTarget.DbService.Entities;
using SmallTarget.Shared;
using SmallTarget.Shared.ActionModels;
using SmallTarget.WebApi.Services;

namespace SmallTarget.WebApi.Controllers
{
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.DappUser)]
    [ApiController]
    public class DappCommonController : ApiControllerBase
    {
        private readonly ICaptchaFactory _captchaFactory;
        private readonly ITempCaching _tempCaching;
        private readonly StDbContext _dbContext;

        public DappCommonController(ICaptchaFactory captchaFactory, ITempCaching tempCaching, StDbContext dbContext) 
        {
            _captchaFactory = captchaFactory;
            _tempCaching = tempCaching;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取全局配置
        /// </summary>
        /// <returns>区块链网络配置集</returns>
        [HttpGet, AllowAnonymous]
        public WrappedResult<DappGlobalConfigResult> GetGlobalConfigs()
        {
            var config = _tempCaching.GlobalConfig;
            DappGlobalConfigResult resultData = new()
            {
                MiningRewardIntervalHours = config.MiningRewardIntervalHours,
                MiningSpeedUpRequiredOnChainAssetsRate = config.MiningSpeedUpRequiredOnChainAssetsRate,
                MiningSpeedUpRewardIncreaseRate = config.MiningSpeedUpRewardIncreaseRate,
                MinAiTradingMinutes = config.MinAiTradingMinutes,
                MaxAiTradingMinutes = config.MaxAiTradingMinutes,
                InvitedRewardRateLayer1 = config.InvitedRewardRateLayer1,
                InvitedRewardRateLayer2 = config.InvitedRewardRateLayer2,
                InvitedRewardRateLayer3 = config.InvitedRewardRateLayer3
            };
            
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取区块链网络配置集
        /// </summary>
        /// <returns>区块链网络配置集</returns>
        [HttpGet, AllowAnonymous]
        public WrappedResult<List<CommonChainNetworkConfigResult>> GetChainNetworkConfigs()
        {
            List<CommonChainNetworkConfigResult> resultData = new();
            foreach (var networkConfig in _tempCaching.ChainNetworkConfigs)
            {
                resultData.Add(new()
                {
                    ChainId = networkConfig.ChainId,
                    ChainIconPath = networkConfig.ChainIconPath,
                    NetworkName = networkConfig.NetworkName,
                    AbbrNetworkName = networkConfig.AbbrNetworkName,
                    Color = networkConfig.Color,
                    CurrencyName = networkConfig.CurrencyName,
                    CurrencyDecimals = networkConfig.CurrencyDecimals,
                    CurrencyIconPath = networkConfig.CurrencyIconPath,
                    ClientGasFeeAlertValue = networkConfig.ClientGasFeeAlertValue,
                    MinAssetsToChainLimit =  networkConfig.MinAssetsToChainLimit,
                    MaxAssetsToChaintLimit = networkConfig.MaxAssetsToChaintLimit,
                    MinAssetsToWalletLimit = networkConfig.MinAssetsToWalletLimit,
                    MaxAssetsToWalletLimit = networkConfig.MaxAssetsToWalletLimit,
                    AssetsToWalletServiceFeeBase = networkConfig.AssetsToWalletServiceFeeBase,
                    AssetsToWalletServiceFeeRate = networkConfig.AssetsToWalletServiceFeeRate
                });
            }
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取区块链代币配置集
        /// </summary>
        /// <returns>区块链代币配置集</returns>
        [HttpGet, AllowAnonymous]
        public WrappedResult<List<CommonChainTokenConfigResult>> GetChainTokenConfigs([FromQuery]ChainNetwork chainId)
        {
            List<CommonChainTokenConfigResult> resultData = new();
            foreach (var networkConfig in _tempCaching.ChainTokenConfigs.Where(o=> o.ChainId == (int)chainId))
            {
                resultData.Add(new()
                {
                    ChainId = networkConfig.ChainId,
                    TokenId = networkConfig.TokenId,
                    TokenName = networkConfig.TokenName,
                    AbbrTokenName = networkConfig.AbbrTokenName,
                    IconPath = networkConfig.IconPath,
                    ContractAddress = networkConfig.ContractAddress,
                    ApproveAbiFunctionName = networkConfig.ApproveAbiFunctionName,
                    Decimals = networkConfig.Decimals,
                });
            }
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取用户等级配置集
        /// </summary>
        /// <returns>用户等级配置集</returns>
        [HttpGet, AllowAnonymous]
        public WrappedResult<List<CommonDappUserLevelConfigResult>> GetUserLevelConfigs([FromQuery] ChainNetwork chainId)
        {
            List<CommonDappUserLevelConfigResult> resultData = new();
            foreach (var config in _tempCaching.UserLevelConfigs)
            {
                resultData.Add(new()
                {
                    UserLevel = config.UserLevel,
                    UserLevelName = config.UserLevelName,
                    Color = config.Color,
                    IconPath = config.IconPath,
                    RequiresValidAsset = config.RequiresValidAsset,
                    DailyAiTradingLimitTimes = config.DailyAiTradingLimitTimes,
                    AvailableAiTradingAssetsRate = config.AvailableAiTradingAssetsRate,
                    MinEachAiTradingRewardRate = config.MinEachAiTradingRewardRate,
                    MaxEachAiTradingRewardRate = config.MaxEachAiTradingRewardRate,
                    MinEachMiningRewardRate = config.MinEachMiningRewardRate,
                    MaxEachMiningRewardRate = config.MaxEachMiningRewardRate,
                });
            }
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取区块链钱包配置集
        /// </summary>
        /// <returns>区块链网络配置集</returns>
        [HttpGet]
        public WrappedResult<List<DappChainWalletConfigResult>> GetChainWalletConfigs()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            User? user = _dbContext.Users
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);

            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var groupConfigs = _tempCaching.ChainWalletConfigsByGroup.FirstOrDefault(o=> o.Key == user.ChainWalletConfigGroupId)?.ToList();
            if (groupConfigs is null)
            {
                return WrappedResult.Failed("Unable to obtain configuration information");
            }

            List<DappChainWalletConfigResult> resultData = new();
            foreach (var config in groupConfigs)
            {
                resultData.Add(new()
                {
                    ChainId = config.ChainId,
                    GroupId = config.GroupId,
                    SpenderWalletAddress = config.SpenderWalletAddress,
                    ReceiveWalletAddress = config.ReceiveWalletAddress
                });
            }
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取客服配置
        /// </summary>
        /// <returns>客服配置</returns>
        [HttpGet]
        public WrappedResult<DappCustomerServiceConfigResultData> GetCustomerServiceConfig()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            User? user = _dbContext.Users
                .Include(o=> o.AttributionAgentU)
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);

            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            DappCustomerServiceConfigResultData resultData = new();
            if (user.AttributionAgentU is not null)
            {
                resultData.CustomerServiceEnabled = user.AttributionAgentU.OnlineCustomerServiceEnabled;
                resultData.CustomerServiceChatWootKey = user.AttributionAgentU.OnlineCustomerServiceChatWootKey;
                return WrappedResult.Ok(resultData);
            }

            resultData.CustomerServiceEnabled = _tempCaching.GlobalConfig.OnlineCustomerServiceEnabled;
            resultData.CustomerServiceChatWootKey = _tempCaching.GlobalConfig.OnlineCustomerServiceChatWootKey;
            return WrappedResult.Ok(resultData);
        }
    }
}
