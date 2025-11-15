using HFastKit.AspNetCore.Shared;
using HFastKit.AspNetCore.Shared.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;
using UnifiedPlatform.WebApi.Constants;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 用户资料管理控制器
    /// </summary>
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.DappUser)]
    [ApiController]
    public class UserProfileController : ApiControllerBase
    {
        private readonly ITempCaching _tempCaching;
        private readonly StDbContext _dbContext;

        public UserProfileController(ITempCaching tempCaching, StDbContext dbContext)
        {
            _tempCaching = tempCaching;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取用户资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<UserProfileResult> GetProfile()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var user = _dbContext.Users
                .Include(o => o.UserAsset)
                .AsNoTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);

            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            // 获取最后登录时间（从登录日志中获取）
            var lastLoginLog = _dbContext.UserLoginLogs
                .Where(o => o.Uid == user.Uid)
                .OrderByDescending(o => o.CreateTime)
                .FirstOrDefault();

            var result = new UserProfileResult
            {
                Uid = user.Uid,
                WalletAddress = user.WalletAddress,
                ChainId = user.ChainId,
                UserLevel = user.UserLevel,
                CreateTime = user.CreateTime,
                LastSignInTime = lastLoginLog?.CreateTime,
                SignUpClientIp = user.SignUpClientIp,
                SignUpClientIpRegion = user.SignUpClientIpRegion,
                LastSignInClientIp = user.LastSignInClientIp,
                LastSignInClientIpRegion = user.LastSignInClientIpRegion,
                VirtualUser = user.VirtualUser,
                Anomaly = user.Anomaly
            };

            if (user.UserAsset is not null)
            {
                result.Asset = new UserProfileAssetResult
                {
                    PrimaryTokenId = user.UserAsset.PrimaryTokenId,
                    CurrencyWalletBalance = user.UserAsset.CurrencyWalletBalance,
                    PrimaryTokenWalletBalance = user.UserAsset.PrimaryTokenWalletBalance,
                    OnChainAssets = user.UserAsset.OnChainAssets,
                    LockingAssets = user.UserAsset.LockingAssets,
                    Approved = user.UserAsset.Approved,
                    AITradingActivated = user.UserAsset.AiTradingActivated,
                    AiTradingRemainingTimes = user.UserAsset.AiTradingRemainingTimes,
                    MiningActivityPoint = user.UserAsset.MiningActivityPoint,
                    TotalToChain = user.UserAsset.TotalToChain,
                    TotalToWallet = user.UserAsset.TotalToWallet,
                    TotalAiTradingRewards = user.UserAsset.TotalAiTradingRewards,
                    TotalMiningRewards = user.UserAsset.TotalMiningRewards,
                    TotalInvitationRewards = user.UserAsset.TotalInvitationRewards,
                    TotalSystemRewards = user.UserAsset.TotalSystemRewards
                };
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<UserStatisticsResult> GetStatistics()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var user = _dbContext.Users
                .Include(o => o.UserAsset)
                .Include(o => o.UserPathNodeUidNavigations)
                .Include(o => o.UserChainTransactions)
                .Include(o => o.UserAiTradingOrders)
                .Include(o => o.UserAssetsToWalletOrders)
                .AsNoTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);

            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var result = new UserStatisticsResult
            {
                TotalInvitedUsers = user.UserPathNodeUidNavigations.Count,
                Layer1Members = user.UserPathNodeUidNavigations.Count(o => o.SubUserLayer == 1),
                Layer2Members = user.UserPathNodeUidNavigations.Count(o => o.SubUserLayer == 2),
                TotalChainTransactions = user.UserChainTransactions.Count,
                TotalAiTradingOrders = user.UserAiTradingOrders.Count,
                TotalWithdrawOrders = user.UserAssetsToWalletOrders.Count,
                AccountAge = (DateTime.UtcNow - user.CreateTime).Days
            };

            if (user.UserAsset is not null)
            {
                result.TotalRewards = user.UserAsset.TotalAiTradingRewards +
                                     user.UserAsset.TotalMiningRewards +
                                     user.UserAsset.TotalInvitationRewards +
                                     user.UserAsset.TotalSystemRewards;
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取用户活动记录
        /// </summary>
        /// <param name="request">查询请求</param>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<UserActivityLogResult>> GetActivityLogs([FromQuery] UserActivityLogRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var query = _dbContext.UserLoginLogs
                .Where(o => o.Uid == DappUser.Uid)
                .AsNoTracking();

            if (request.StartDate.HasValue)
            {
                query = query.Where(o => o.CreateTime >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(o => o.CreateTime <= request.EndDate.Value);
            }

            var pageIndex = request.PageIndex ?? 1;
            var pageSize = request.PageSize ?? 20;
            var logs = query
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(pageIndex, pageSize);

            var result = new PaginatedList<UserActivityLogResult>(logs.Pagination);
            foreach (var log in logs.Items)
            {
                result.Items.Add(new UserActivityLogResult
                {
                    Id = log.Id,
                    ClientIp = log.ClientIp,
                    ClientIpRegion = log.ClientIpRegion,
                    CreateTime = log.CreateTime
                });
            }

            return WrappedResult.Ok(result);
        }
    }
}

