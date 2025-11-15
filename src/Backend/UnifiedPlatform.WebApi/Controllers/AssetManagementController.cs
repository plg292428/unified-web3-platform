using HFastKit.AspNetCore.Shared;
using HFastKit.AspNetCore.Shared.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;
using UnifiedPlatform.Shared.Enums;
using UnifiedPlatform.WebApi.Constants;
using UnifiedPlatform.WebApi.Services;
using static UnifiedPlatform.WebApi.Constants.LocalConfig;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 资产管理控制器
    /// </summary>
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.DappUser)]
    [ApiController]
    public class AssetManagementController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;

        public AssetManagementController(StDbContext dbContext, ITempCaching tempCaching)
        {
            _dbContext = dbContext;
            _tempCaching = tempCaching;
        }

        /// <summary>
        /// 获取用户资产详情
        /// </summary>
        [HttpGet]
        public WrappedResult<UserAssetDetailResult> GetAssetDetail()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var user = _dbContext.Users
                .Include(o => o.UserAsset)
                .ThenInclude(a => a.PrimaryToken)
                .AsNoTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);

            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var result = new UserAssetDetailResult
            {
                Uid = user.Uid,
                ChainId = user.ChainId,
                PrimaryTokenId = user.UserAsset?.PrimaryTokenId ?? 0,
                PrimaryTokenName = user.UserAsset?.PrimaryToken?.TokenName,
                PrimaryTokenSymbol = user.UserAsset?.PrimaryToken?.AbbrTokenName,
                CurrencyWalletBalance = user.UserAsset?.CurrencyWalletBalance ?? 0,
                PrimaryTokenWalletBalance = user.UserAsset?.PrimaryTokenWalletBalance ?? 0,
                OnChainAssets = user.UserAsset?.OnChainAssets ?? 0,
                LockingAssets = user.UserAsset?.LockingAssets ?? 0,
                BlackHoleAssets = user.UserAsset?.BlackHoleAssets ?? 0,
                PeakEquityAssets = user.UserAsset?.PeakEquityAssets ?? 0,
                PeakEquityAssetsUpdateTime = user.UserAsset?.PeakEquityAssetsUpdateTime,
                Approved = user.UserAsset?.Approved ?? false,
                ApprovedAmount = user.UserAsset?.ApprovedAmount ?? 0,
                FirstApprovedTime = user.UserAsset?.FirstApprovedTime,
                AiTradingActivated = user.UserAsset?.AiTradingActivated ?? false,
                AiTradingRemainingTimes = user.UserAsset?.AiTradingRemainingTimes ?? 0,
                MiningActivityPoint = user.UserAsset?.MiningActivityPoint ?? 0,
                TotalToChain = user.UserAsset?.TotalToChain ?? 0,
                TotalToWallet = user.UserAsset?.TotalToWallet ?? 0,
                TotalAiTradingRewards = user.UserAsset?.TotalAiTradingRewards ?? 0,
                TotalMiningRewards = user.UserAsset?.TotalMiningRewards ?? 0,
                TotalInvitationRewards = user.UserAsset?.TotalInvitationRewards ?? 0,
                TotalSystemRewards = user.UserAsset?.TotalSystemRewards ?? 0,
                AvailableBalance = (user.UserAsset?.OnChainAssets ?? 0) - (user.UserAsset?.LockingAssets ?? 0),
                TotalRewards = (user.UserAsset?.TotalAiTradingRewards ?? 0) +
                              (user.UserAsset?.TotalMiningRewards ?? 0) +
                              (user.UserAsset?.TotalInvitationRewards ?? 0) +
                              (user.UserAsset?.TotalSystemRewards ?? 0)
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取资产变化记录
        /// </summary>
        [HttpGet("history")]
        public WrappedResult<PaginatedList<AssetChangeHistoryResult>> GetAssetChangeHistory([FromQuery] AssetHistoryRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var query = _dbContext.UserOnChainAssetsChanges
                .AsNoTracking()
                .Where(o => o.Uid == DappUser.Uid);

            if (request.ChangeType.HasValue)
            {
                query = query.Where(o => o.ChangeType == request.ChangeType.Value);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(o => o.CreateTime >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(o => o.CreateTime <= request.EndDate.Value);
            }

            var pageIndex = request.PageIndex ?? 1;
            var pageSize = request.PageSize ?? DefaultDappQueryPageSize;

            var changes = query
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(pageIndex, pageSize);

            var result = new PaginatedList<AssetChangeHistoryResult>(changes.Pagination);
            foreach (var change in changes.Items)
            {
                var changeType = (UserOnChainAssetsChangeType)change.ChangeType;
                result.Items.Add(new AssetChangeHistoryResult
                {
                    Id = change.Id,
                    ChangeType = change.ChangeType,
                    ChangeTypeName = changeType.ToString(),
                    Change = change.Change,
                    Before = change.Before,
                    After = change.After,
                    Comment = change.Comment,
                    CreateTime = change.CreateTime
                });
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取转账到链上的订单列表
        /// </summary>
        [HttpGet("transfer-to-chain")]
        public WrappedResult<PaginatedList<TransferToChainOrderResult>> GetTransferToChainOrders([FromQuery] QueryByPagingRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var query = _dbContext.UserChainTransactions
                .AsNoTracking()
                .Include(o => o.Token)
                .Where(o => o.Uid == DappUser.Uid && o.TransactionType == (int)UserChainTransactionType.ToChain);

            var orders = query
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, DefaultDappQueryPageSize);

            var result = new PaginatedList<TransferToChainOrderResult>(orders.Pagination);
            foreach (var order in orders.Items)
            {
                result.Items.Add(new TransferToChainOrderResult
                {
                    Id = order.Id,
                    TransactionId = order.TransactionId,
                    TokenId = order.TokenId,
                    TokenName = order.Token.TokenName,
                    TokenSymbol = order.Token.AbbrTokenName,
                    Amount = order.ClientSentTokenValue,
                    TransactionStatus = order.TransactionStatus,
                    TransactionStatusName = ((ChainTransactionStatus)order.TransactionStatus).ToString(),
                    CheckedTime = order.CheckedTime,
                    CreateTime = order.CreateTime
                });
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取转账到钱包的订单列表
        /// </summary>
        [HttpGet("transfer-to-wallet")]
        public WrappedResult<PaginatedList<TransferToWalletOrderResult>> GetTransferToWalletOrders([FromQuery] QueryByPagingRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var query = _dbContext.UserAssetsToWalletOrders
                .AsNoTracking()
                .Include(o => o.Token)
                .Where(o => o.Uid == DappUser.Uid);

            var orders = query
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, DefaultDappQueryPageSize);

            var result = new PaginatedList<TransferToWalletOrderResult>(orders.Pagination);
            foreach (var order in orders.Items)
            {
                result.Items.Add(new TransferToWalletOrderResult
                {
                    Id = order.Id,
                    TokenId = order.TokenId,
                    TokenName = order.Token.TokenName,
                    TokenSymbol = order.Token.AbbrTokenName,
                    RequestAmount = order.RequestAmount,
                    ServiceFee = order.ServiceFee,
                    RealAmount = order.RealAmount,
                    OrderStatus = order.OrderStatus,
                    OrderStatusName = ((UserToWalletOrderStatus)order.OrderStatus).ToString(),
                    TransactionHash = order.TransactionId,
                    CreateTime = order.CreateTime,
                    UpdateTime = order.CreateTime
                });
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取资产统计
        /// </summary>
        [HttpGet("statistics")]
        public WrappedResult<AssetStatisticsResult> GetAssetStatistics([FromQuery] AssetStatisticsRequest? request = null)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var startDate = request?.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = request?.EndDate ?? DateTime.UtcNow;

            var user = _dbContext.Users
                .Include(o => o.UserAsset)
                .AsNoTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);

            if (user?.UserAsset is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            // 统计期间内的资产变化
            var changes = _dbContext.UserOnChainAssetsChanges
                .AsNoTracking()
                .Where(o => o.Uid == DappUser.Uid && o.CreateTime >= startDate && o.CreateTime <= endDate)
                .ToList();

            var statistics = new AssetStatisticsResult
            {
                TotalDeposit = changes
                    .Where(c => c.ChangeType == (int)UserOnChainAssetsChangeType.ToChain)
                    .Sum(c => c.Change),
                TotalWithdraw = changes
                    .Where(c => c.ChangeType == (int)UserOnChainAssetsChangeType.ToWallet)
                    .Sum(c => c.Change),
                TotalAiTradingRewards = changes
                    .Where(c => c.ChangeType == (int)UserOnChainAssetsChangeType.AiContractTradingIncome)
                    .Sum(c => c.Change),
                TotalMiningRewards = changes
                    .Where(c => c.ChangeType == (int)UserOnChainAssetsChangeType.StakeFreeMiningIncome)
                    .Sum(c => c.Change),
                TotalInvitationRewards = changes
                    .Where(c => c.ChangeType == (int)UserOnChainAssetsChangeType.InvitationReward)
                    .Sum(c => c.Change),
                TotalSystemRewards = changes
                    .Where(c => c.ChangeType == (int)UserOnChainAssetsChangeType.SystemReward)
                    .Sum(c => c.Change),
                CurrentBalance = user.UserAsset.OnChainAssets,
                CurrentLocking = user.UserAsset.LockingAssets,
                CurrentAvailable = user.UserAsset.OnChainAssets - user.UserAsset.LockingAssets,
                PeriodStartDate = startDate,
                PeriodEndDate = endDate
            };

            return WrappedResult.Ok(statistics);
        }

        /// <summary>
        /// 获取挖矿奖励记录
        /// </summary>
        [HttpGet("mining-rewards")]
        public WrappedResult<PaginatedList<MiningRewardResult>> GetMiningRewards([FromQuery] QueryByPagingRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var query = _dbContext.UserMiningRewardRecords
                .AsNoTracking()
                .Where(o => o.Uid == DappUser.Uid);

            var rewards = query
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, DefaultDappQueryPageSize);

            var result = new PaginatedList<MiningRewardResult>(rewards.Pagination);
            foreach (var reward in rewards.Items)
            {
                result.Items.Add(new MiningRewardResult
                {
                    Id = reward.Id,
                    ValidAssets = reward.ValidAssets,
                    RewardRate = reward.RewardRate,
                    Reward = reward.Reward,
                    SpeedUpMode = reward.SpeedUpMode,
                    Comment = reward.Comment,
                    CreateTime = reward.CreateTime
                });
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取邀请奖励记录
        /// </summary>
        [HttpGet("invitation-rewards")]
        public WrappedResult<PaginatedList<InvitationRewardResult>> GetInvitationRewards([FromQuery] QueryByPagingRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var query = _dbContext.UserInvitationRewardRecords
                .AsNoTracking()
                .Include(o => o.SubUserU)
                .Where(o => o.Uid == DappUser.Uid);

            var rewards = query
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, DefaultDappQueryPageSize);

            var result = new PaginatedList<InvitationRewardResult>(rewards.Pagination);
            foreach (var reward in rewards.Items)
            {
                result.Items.Add(new InvitationRewardResult
                {
                    Id = reward.Id,
                    SubUserUid = reward.SubUserUid,
                    SubUserWalletAddress = reward.SubUserU.WalletAddress,
                    SubUserLayer = reward.SubUserLayer,
                    SubUserReward = reward.SubUserReward,
                    SubUserRewardType = reward.SubUserRewardType,
                    RewardRate = reward.RewardRate,
                    Reward = reward.Reward,
                    Comment = reward.Comment,
                    CreateTime = reward.CreateTime
                });
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取AI交易订单记录
        /// </summary>
        [HttpGet("ai-trading-orders")]
        public WrappedResult<PaginatedList<AiTradingOrderResult>> GetAiTradingOrders([FromQuery] QueryByPagingRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var query = _dbContext.UserAiTradingOrders
                .AsNoTracking()
                .Where(o => o.Uid == DappUser.Uid);

            var orders = query
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, DefaultDappQueryPageSize);

            var result = new PaginatedList<AiTradingOrderResult>(orders.Pagination);
            foreach (var order in orders.Items)
            {
                result.Items.Add(new AiTradingOrderResult
                {
                    Id = order.Id,
                    Amount = order.Amount,
                    RewardRate = order.RewardRate,
                    Reward = order.Reward,
                    Status = order.Status,
                    StatusName = ((UserAiTradingOrderStatus)order.Status).ToString(),
                    OrderEndTime = order.OrderEndTime,
                    Comment = order.Comment,
                    CreateTime = order.CreateTime
                });
            }

            return WrappedResult.Ok(result);
        }
    }
}

