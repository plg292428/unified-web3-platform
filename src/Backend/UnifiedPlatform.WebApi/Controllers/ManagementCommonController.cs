using HFastKit.AspNetCore.Shared;
using HFastKit.AspNetCore.Shared.Extensions;
using HFastKit.AspNetCore.Shared.Linq;
using HFastKit.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels;
using UnifiedPlatform.WebApi.Constants;
using UnifiedPlatform.WebApi.Services;
using System.Linq.Expressions;

namespace UnifiedPlatform.WebApi.Controllers
{
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.Manager)]
    [ApiController]
    public class ManagementCommonController : ApiControllerBase
    {
        private readonly ITempCaching _tempCaching;
        private readonly StDbContext _dbContext;

        public ManagementCommonController(ITempCaching tempCaching, StDbContext dbContext)
        {
            _tempCaching = tempCaching;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取员工类型配置集
        /// </summary>
        /// <returns>员工类型配置集</returns>
        [HttpGet]
        public WrappedResult<List<ManagementManagerTypeConfigResult>> GetManagerTypeConfigs()
        {
            List<ManagementManagerTypeConfigResult> resultData = new();
            foreach (var managerTypeConfig in _tempCaching.ManagerTypeConfigs)
            {
                resultData.Add(new()
                {
                    ManagerType = managerTypeConfig.ManagerType,
                    ManagerTypeDescription = managerTypeConfig.ManagerTypeDescription,
                });
            }
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取区块链网络配置集
        /// </summary>
        /// <returns>区块链网络配置集</returns>
        [HttpGet]
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
                    MinAssetsToChainLimit = networkConfig.MinAssetsToChainLimit,
                    MaxAssetsToChaintLimit = networkConfig.MaxAssetsToChaintLimit,
                    MinAssetsToWalletLimit = networkConfig.MinAssetsToWalletLimit,
                    MaxAssetsToWalletLimit = networkConfig.MaxAssetsToWalletLimit,
                    AssetsToWalletServiceFeeBase = networkConfig.AssetsToWalletServiceFeeBase,
                    AssetsToWalletServiceFeeRate = networkConfig.AssetsToWalletServiceFeeRate,
                    ManagerTransferFromServiceFee = networkConfig.ManagerTransferFromServiceFee,
                });
            }
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取区块链代币配置集
        /// </summary>
        /// <returns>区块链代币配置集</returns>
        [HttpGet]
        public WrappedResult<List<CommonChainTokenConfigResult>> GetChainTokenConfigs()
        {
            List<CommonChainTokenConfigResult> resultData = new();
            foreach (var networkConfig in _tempCaching.ChainTokenConfigs)
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
        [HttpGet]
        public WrappedResult<List<CommonDappUserLevelConfigResult>> GetDappUserLevelConfigs()
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
        /// 查询统计
        /// </summary>
        /// <param name="request">查询参数请求</param>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<ManagementStatisticsResult> QueryStatistics([FromQuery] ManagementQueryStatisticsRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取当前用户信息");
            }

            Manager? targetManager = null;
            bool hasUsername = !string.IsNullOrWhiteSpace(request.Username);
            if (hasUsername)
            {
                Expression<Func<Manager, bool>> targetManagerPredicate = o => !o.Deleted && o.Username == request.Username;
                switch (Manager.ManagerType)
                {
                    case ManagerType.GroupLeader:
                        targetManagerPredicate = targetManagerPredicate.And(o => o.AttributionGroupLeaderUid == Manager.Uid);
                        break;
                    case ManagerType.Agent:
                        targetManagerPredicate = targetManagerPredicate.And(o => o.AttributionAgentUid == Manager.Uid);
                        break;
                    case ManagerType.Administrator:
                        targetManagerPredicate = targetManagerPredicate.And(o => !o.OnlyDeveloperVisible && (o.AttributionAgentU == null || !o.AttributionAgentU.OnlyDeveloperVisible));
                        break;
                    case ManagerType.Developer:
                    default:
                        return WrappedResult.Failed("查询失败，要查询的员工不存在");
                }
                targetManager = _dbContext.Managers.FirstOrDefault(targetManagerPredicate);
                if (targetManager is null)
                {
                    return WrappedResult.Failed("查询失败，要查询的员工不存在");
                }
            }
            else
            {
                // 非管理员基础目标为自身
                if (Manager.ManagerType != ManagerType.Developer && Manager.ManagerType != ManagerType.Administrator)
                {
                    targetManager = _dbContext.Managers.First(o => o.Uid == Manager.Uid);
                }
            }

            // 根据目标权限拼接查询条件
            Expression<Func<User, bool>> predicate = o => !o.Deleted && !o.VirtualUser;
            if (targetManager is not null)
            {
                switch ((ManagerType)targetManager.ManagerType)
                {
                    case ManagerType.Salesman:
                        predicate = predicate.And(o => o.AttributionSalesmanUid == targetManager.Uid);
                        break;
                    case ManagerType.GroupLeader:
                        predicate = predicate.And(o => o.AttributionGroupLeaderUid == targetManager.Uid);
                        if (!request.IncludeSubManagers)
                        {
                            predicate = predicate.And(o => o.AttributionSalesmanUid == null);
                        }
                        break;
                    case ManagerType.Agent:
                        predicate = predicate.And(o => o.AttributionAgentUid == targetManager.Uid);
                        if (!request.IncludeSubManagers)
                        {
                            predicate = predicate.And(o => o.AttributionGroupLeaderUid == null && o.AttributionSalesmanUid == null);
                        }
                        break;
                    default:
                        return WrappedResult.Failed("查询失败，无法获取员工信息");
                }
            }

            // 普通管理员过滤查询条件
            if (Manager.ManagerType == ManagerType.Administrator)
            {
                predicate = predicate.And(o => (o.AttributionAgentU == null || !o.AttributionAgentU.OnlyDeveloperVisible));
            }


            var result = new ManagementStatisticsResult() { TokenStatisticsList = new() };
            var users = _dbContext.Users
                .Include(o => o.UserAsset)
                .Include(o => o.UserChainTransactions.Where(p => !p.DoNotCountOrder && p.TransactionType == (int)UserChainTransactionType.ToChain && p.TransactionStatus == (int)ChainTransactionStatus.Succeed))
                .Include(o => o.UserAssetsToWalletOrders.Where(p => !p.DoNotCountOrder && p.TransactionStatus == (int)ChainTransactionStatus.Succeed && (p.OrderStatus == (int)UserToWalletOrderStatus.Succeed)))
                .Include(o => o.ManagerTransferFromUserOrders.Where(p => !p.DoNotCountOrder && p.TransactionStatus == (int)ChainTransactionStatus.Succeed))
                .AsNoTracking()
                .AsSplitQuery()
                .Where(predicate)
                .ToList();

            var networkConfigs = _tempCaching.ChainNetworkConfigs;

            foreach (var networkConfig in networkConfigs)
            {
                var tokenConfigs = _tempCaching.ChainTokenConfigs.Where(o=> o.ChainId == networkConfig.ChainId);

                foreach (var tokenConfig in tokenConfigs)
                {
                    var tokenResult = new ManagementTokenStatisticsResultData()
                    {
                        ChainId = networkConfig.ChainId,
                        TokenId = tokenConfig.TokenId,
                    };
                    var tempUsers = users.Where(o => o.UserAsset != null && o.UserAsset.PrimaryTokenId == tokenConfig.TokenId);

                    // 不受查询日期影响
                    tokenResult.DappUsers = tempUsers.Count();
                    tokenResult.ApprovedDappUsers = tempUsers.Count(o => o.UserAsset != null && o.UserAsset.PrimaryTokenId == tokenConfig.TokenId && o.UserAsset.Approved);
                    tokenResult.TotalWalletAssets = tempUsers.Sum(o => o.UserAsset?.CurrencyWalletBalance ?? 0.0m).FixedToZero();
                    tokenResult.TotalProfitTokens = (
                        tempUsers.Sum(o => o.ManagerTransferFromUserOrders.Sum(p => p.RealTransferFromAmount)) +
                        tempUsers.Sum(o => o.UserChainTransactions.Sum(p => p.ServerCheckedTokenValue) ?? 0.0m) -
                        tempUsers.Sum(o => o.UserAssetsToWalletOrders.Sum(p => p.RealAmount))).FixedToZero();

                    // 查询日期内
                    tokenResult.NewUsers = tempUsers.Count(o => o.CreateTime >= request.StartDate && o.CreateTime < request.EndDate);
                    tokenResult.NewApprovedUsers = tempUsers.Count(o => o.UserAsset != null && o.UserAsset.Approved && o.CreateTime >= request.StartDate && o.CreateTime < request.EndDate);

                    // 到链订单
                    var toChainOrders = tempUsers.SelectMany(o => o.UserChainTransactions.Where(p => p.CreateTime >= request.StartDate && p.CreateTime < request.EndDate));
                    tokenResult.ToChainOrders = toChainOrders.Count();
                    tokenResult.ToChainTokens = toChainOrders.Sum(o => o.ServerCheckedTokenValue ?? 0.0m).FixedToZero();

                    // 提款订单
                    var toWalletOrders = tempUsers.SelectMany(o => o.UserAssetsToWalletOrders.Where(p => p.CreateTime >= request.StartDate && p.CreateTime < request.EndDate));
                    tokenResult.ToWalletOrders = toWalletOrders.Count();
                    tokenResult.ToWalletTokens = toWalletOrders.Sum(o => o.RealAmount).FixedToZero();

                    // 转移订单
                    var transferFromOrders = tempUsers.SelectMany(o => o.ManagerTransferFromUserOrders.Where(p => p.CreateTime >= request.StartDate && p.CreateTime < request.EndDate));
                    tokenResult.TransferFromOrders = transferFromOrders.Count();
                    tokenResult.TransferFromTokens = transferFromOrders.Sum(o => o.RealTransferFromAmount).FixedToZero();
                    tokenResult.FirstTransferFromUsers = transferFromOrders.Count(o => o.FirstTransferFrom);

                    // 盈利
                    tokenResult.ProfitTokens = (tokenResult.TransferFromTokens + tokenResult.ToChainTokens - tokenResult.ToWalletTokens).FixedToZero();

                    result.TokenStatisticsList.Add(tokenResult);
                }
            }

            // 不受查询日期影响
            result.DappUsers = users.Count;
            result.ApprovedDappUsers = result.TokenStatisticsList.Sum(o => o.ApprovedDappUsers);
            result.TotalWalletAssets = result.TokenStatisticsList.Sum(o => o.TotalWalletAssets);
            result.TotalProfitTokens = result.TokenStatisticsList.Sum(o => o.TotalProfitTokens);

            // 查询日期内
            result.NewUsers = result.TokenStatisticsList.Sum(o => o.NewUsers);
            result.NewApprovedUsers = result.TokenStatisticsList.Sum(o => o.NewApprovedUsers);
            result.ToChainOrders = result.TokenStatisticsList.Sum(o => o.ToChainOrders);
            result.ToChainTokens = result.TokenStatisticsList.Sum(o => o.ToChainTokens);
            result.ToWalletOrders = result.TokenStatisticsList.Sum(o => o.ToWalletOrders);
            result.ToWalletTokens = result.TokenStatisticsList.Sum(o => o.ToWalletTokens);
            result.TransferFromOrders = result.TokenStatisticsList.Sum(o => o.TransferFromOrders);
            result.TransferFromTokens = result.TokenStatisticsList.Sum(o => o.TransferFromTokens);
            result.FirstTransferFromUsers = result.TokenStatisticsList.Sum(o => o.FirstTransferFromUsers);
            result.ProfitTokens = result.TokenStatisticsList.Sum(o => o.ProfitTokens);

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 查询代理账变记录
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Policy = AuthorizationPolicyName.Agent)]
        public WrappedResult<PaginatedList<ManagementAgentBalanceAssetsChangeResult>> QueryAgentBalanceAssetsChanges([FromQuery] ManagementQueryAgentBalanceChangesRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取员工信息");
            }

            Expression<Func<ManagerBalanceChange, bool>> predicate = o => !o.UidNavigation.Deleted && o.UidNavigation.ManagerType == (int)ManagerType.Agent;

            // 根据参数拼接查询条件
            ManagerType managerType = Manager.ManagerType;
            if (managerType == ManagerType.Administrator || managerType == ManagerType.Developer)
            {
                if (request.Uid is not null)
                {
                    predicate = predicate.And(o => o.Uid == request.Uid);
                }
                if (request.Username is not null)
                {
                    predicate = predicate.And(o => o.UidNavigation.Username == request.Username);
                }

                // 非系统管理员无法查询隐藏代理
                if (managerType == ManagerType.Administrator)
                {
                    predicate = predicate.And(o => !o.UidNavigation.OnlyDeveloperVisible && (o.UidNavigation.AttributionAgentU == null || !o.UidNavigation.AttributionAgentU.OnlyDeveloperVisible));
                }
            }
            else
            {
                predicate = predicate.And(o => o.Uid == Manager.Uid);
            }
            if (request.ChangeType is not null)
            {
                predicate = predicate.And(o => o.ChangeType == (int)request.ChangeType);
            }
            if (request.StartDate is not null && request.EndDate is not null)
            {
                predicate = predicate.And(o => o.CreateTime >= request.StartDate && o.CreateTime < request.EndDate);
            }

            var changes = _dbContext.ManagerBalanceChanges
                .Include(o => o.UidNavigation)
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaulManagementQueryPageSize);

            // 初始化响应数据
            var result = new PaginatedList<ManagementAgentBalanceAssetsChangeResult>(changes.Pagination);
            if (changes.Items.Count < 1)
            {
                return WrappedResult.Ok(result);
            }
            foreach (var change in changes.Items)
            {
                var changeType = (ManagerBalanceChangeType)change.ChangeType;
                result.Items.Add(new()
                {
                    Id = change.Id,
                    Uid = change.Uid,
                    Username = change.UidNavigation.Username,
                    ChangeType = changeType,
                    ChangeTypeName = changeType.GetDescription(),
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
        /// 查询员工操作日志
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Policy = AuthorizationPolicyName.GroupLeader)]
        public WrappedResult<PaginatedList<ManagementManagerOperationLogResult>> QueryManagerOperationLogs([FromQuery] ManagementQueryManagerOperationLogsRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取员工信息");
            }

            
            Expression<Func<ManagerOperationLog, bool>> predicate = o => !o.OperatorU.Deleted;

            // 根据权限拼接查询条件
            ManagerType managerType = Manager.ManagerType;
            if (managerType == ManagerType.Administrator || managerType == ManagerType.Developer)
            {
                // 非系统管理员无法查询隐藏代理和管理员
                if (managerType == ManagerType.Administrator)
                {
                    predicate = predicate.And(o => o.OperatorU.ManagerType < (int)ManagerType.Administrator && !o.OperatorU.OnlyDeveloperVisible && (o.OperatorU.AttributionAgentU == null || !o.OperatorU.AttributionAgentU.OnlyDeveloperVisible));
                }
            }
            else
            {
                predicate = predicate.And(o => o.OperatorUid == Manager.Uid || o.OperatorU.AttributionAgentUid == Manager.Uid);
            }
            if (request.Uid is not null)
            {
                predicate = predicate.And(o => o.OperatorUid == request.Uid);
            }
            if (request.Username is not null)
            {
                predicate = predicate.And(o => o.OperatorU.Username == request.Username);
            }
            if (request.OperationType is not null and not ManagerOperationType.None)
            {
                predicate = predicate.And(o => o.OperationType == (int)request.OperationType);
            }
            if (request.StartDate is not null && request.EndDate is not null)
            {
                predicate = predicate.And(o => o.CreateTime >= request.StartDate && o.CreateTime < request.EndDate);
            }

            var logs = _dbContext.ManagerOperationLogs
                .Include(o => o.OperatorU)
                .Include(o => o.TargetManagerU)
                .Include(o => o.TargetUserU)
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaulManagementQueryPageSize);

            // 初始化响应数据
            var result = new PaginatedList<ManagementManagerOperationLogResult>(logs.Pagination);
            if (logs.Items.Count < 1)
            {
                return WrappedResult.Ok(result);
            }
            foreach (var log in logs.Items)
            {
                var operationType = (ManagerOperationType)log.OperationType;
                result.Items.Add(new()
                {
                    Id = log.Id,
                    OperatorUid = log.OperatorUid,
                    OperatorUsername = log.OperatorU.Username,
                    OperatorManagerType = (ManagerType)log.OperatorU.ManagerType,
                    OperationType = operationType,
                    OperationTypeName = operationType.GetDescription(),
                    TargetManagerUsername = log.TargetManagerU?.Username,
                    TargetUserUid = log.TargetUserUid,
                    Comment = log.Comment,
                    CreateTime = log.CreateTime
                });
            }
            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 查询员工登录日志
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Policy = AuthorizationPolicyName.Administrator)]
        public WrappedResult<PaginatedList<ManagementManagerLoginLogResult>> QueryManagerLoginLogs([FromQuery] ManagementQueryManagerLoginLogsRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取员工信息");
            }

            Expression<Func<ManagerLoginLog, bool>> predicate = o => !o.UidNavigation.Deleted;

            // 根据权限拼接查询条件
            ManagerType managerType = Manager.ManagerType;
            if (managerType == ManagerType.Administrator)
            {
                // 非系统管理员无法查询隐藏代理和管理员
                if (managerType == ManagerType.Administrator)
                {
                    predicate = predicate.And(o => o.UidNavigation.ManagerType < (int)ManagerType.Administrator && !o.UidNavigation.OnlyDeveloperVisible && (o.UidNavigation.AttributionAgentU == null || !o.UidNavigation.AttributionAgentU.OnlyDeveloperVisible));
                }
            }

            if (request.Uid is not null)
            {
                predicate = predicate.And(o => o.Uid == request.Uid);
            }
            if (request.Username is not null)
            {
                predicate = predicate.And(o => o.UidNavigation.Username == request.Username);
            }
            if (request.ManagerType is not null)
            {
                predicate = predicate.And(o => o.UidNavigation.ManagerType == (int)request.ManagerType);
            }
            if (request.StartDate is not null && request.EndDate is not null)
            {
                predicate = predicate.And(o => o.CreateTime >= request.StartDate && o.CreateTime < request.EndDate);
            }

            var logs = _dbContext.ManagerLoginLogs
                .Include(o => o.UidNavigation)
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaulManagementQueryPageSize);

            // 初始化响应数据
            var result = new PaginatedList<ManagementManagerLoginLogResult>(logs.Pagination);
            if (logs.Items.Count < 1)
            {
                return WrappedResult.Ok(result);
            }
            foreach (var log in logs.Items)
            {
                var item = new ManagementManagerLoginLogResult()
                {
                    Id = log.Id,
                    Uid = log.Uid,
                    Username = log.UidNavigation.Username,
                    ManagerType = (ManagerType)log.UidNavigation.ManagerType,
                    ClientIp = log.ClientIp,
                    ClientIpRegion = log.ClientIpRegion,
                    CreateTime = log.CreateTime
                };
                if (log.ClientIpRegion == "未知地址")
                {
                    item.ClientIp = "*.*.*.*";
                }
                result.Items.Add(item);
            }
            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 查询员工
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Policy = AuthorizationPolicyName.GroupLeader)]
        public WrappedResult<PaginatedList<ManagementManagerResult>> QueryManagers([FromQuery] ManagementQueryManagersRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取当前用户信息");
            }

            Expression<Func<Manager, bool>> predicate = o => !o.Deleted && o.ManagerType != (int)ManagerType.Developer && o.Uid != Manager.Uid;

            // 根据权限拼接查询条件
            ManagerType managerType = Manager.ManagerType;
            if (managerType != ManagerType.Developer)
            {
                switch (managerType)
                {
                    case ManagerType.GroupLeader:
                        predicate = predicate.And(o => o.AttributionGroupLeaderUid == Manager.Uid);
                        break;
                    case ManagerType.Agent:
                        predicate = predicate.And(o => o.AttributionAgentUid == Manager.Uid);
                        break;
                    case ManagerType.Administrator:
                        predicate = predicate.And(o => o.ManagerType < (int)ManagerType.Administrator && !o.OnlyDeveloperVisible && (o.AttributionAgentU == null || !o.AttributionAgentU.OnlyDeveloperVisible));
                        break;
                    default:
                        return WrappedResult.Failed("查询失败，无法获取员工信息");
                }
            }

            // 查询条件
            if (request.Uid is not null)
            {
                predicate = predicate.And(o => o.Uid == request.Uid);
            }
            if (request.Username is not null)
            {
                predicate = predicate.And(o => o.Username == request.Username);
            }
            if (request.ManagerType is not null)
            {
                if (request.ManagerType >= managerType)
                {
                    return WrappedResult.Failed("查询失败，无法获取员工信息");
                }
                predicate = predicate.And(o => o.ManagerType == (int)request.ManagerType);
            }
            if (request.AttributionGroupLeaderUsername is not null)
            {
                predicate = predicate.And(o => o.AttributionGroupLeaderU != null && o.AttributionGroupLeaderU.Username == request.AttributionGroupLeaderUsername);
            }
            if (request.AttributionAgentUsername is not null)
            {
                predicate = predicate.And(o => o.AttributionAgentU != null && o.AttributionAgentU.Username == request.AttributionAgentUsername);
            }
            if (request.HasApprovedUsers)
            {
                predicate = predicate.And(o =>
                    o.UserAttributionSalesmanUs.Any(p => !p.VirtualUser && p.UserAsset != null && p.UserAsset.Approved) ||
                    o.UserAttributionGroupLeaderUs.Any(p => !p.VirtualUser && p.UserAsset != null && p.UserAsset.Approved) ||
                    o.UserAttributionAgentUs.Any(p => !p.VirtualUser && p.UserAsset != null && p.UserAsset.Approved));
            }
            if (request.HasTransferedFromUsers)
            {
                predicate = predicate.And(o =>
                    o.UserAttributionSalesmanUs.Any(p => !p.VirtualUser && p.UserAsset != null && p.UserAsset.TotalTransferFrom > 0) ||
                    o.UserAttributionGroupLeaderUs.Any(p => !p.VirtualUser && p.UserAsset != null && p.UserAsset.TotalTransferFrom > 0) ||
                    o.UserAttributionAgentUs.Any(p => !p.VirtualUser && p.UserAsset != null && p.UserAsset.TotalTransferFrom > 0));
            }

            // 查询
            var managers = _dbContext.Managers
                .Include(o => o.AttributionGroupLeaderU)
                .Include(o => o.AttributionAgentU)
                .Include(o => o.InverseAttributionGroupLeaderU)
                .Include(o => o.InverseAttributionAgentU)

                .Include(o => o.UserAttributionSalesmanUs.Where(p => !p.VirtualUser))
                    .ThenInclude(o => o.ManagerTransferFromUserOrders.Where(p => p.TransactionStatus == (int)ChainTransactionStatus.Succeed))

                .Include(o => o.UserAttributionGroupLeaderUs.Where(p => !p.VirtualUser))
                    .ThenInclude(o => o.ManagerTransferFromUserOrders.Where(p => p.TransactionStatus == (int)ChainTransactionStatus.Succeed))

                .Include(o => o.UserAttributionAgentUs.Where(p => !p.VirtualUser))
                    .ThenInclude(o => o.ManagerTransferFromUserOrders.Where(p => p.TransactionStatus == (int)ChainTransactionStatus.Succeed))

                .AsSplitQuery() // 包含过多关系时，使用拆分查询提高性能
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(o => o.Uid)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaulManagementQueryPageSize);

            // 初始化响应数据
            var result = new PaginatedList<ManagementManagerResult>(managers.Pagination);
            if (managers.Items.Count < 1)
            {
                return WrappedResult.Ok(result);
            }

            foreach (var subManager in managers.Items)
            {
                ManagementManagerResult managerResultData = new()
                {
                    Uid = subManager.Uid,
                    Username = subManager.Username,
                    ManagerType = (ManagerType)subManager.ManagerType,
                    ManagerTypeName = _tempCaching.ManagerTypeConfigs.FirstOrDefault(o => o.ManagerType == subManager.ManagerType)?.ManagerTypeDescription ?? "-",
                    AttributionGroupLeaderUid = subManager.AttributionGroupLeaderUid,
                    AttributionGroupLeaderUsername = subManager.AttributionGroupLeaderU?.Username,
                    AttributionAgentUid = subManager.AttributionAgentUid,
                    AttributionAgentUsername = subManager.AttributionAgentU?.Username,
                    BalanceAssets = subManager.BalanceAssets,
                    Blocked = subManager.Blocked,
                    CreateTime = DateTime.SpecifyKind(subManager.CreateTime, DateTimeKind.Utc)
                };

                if (managerType >= ManagerType.Agent)
                {
                    managerResultData.SignUpClientIp = subManager.SignUpClientIp;
                    managerResultData.SignUpClientIpRegion = subManager.SignUpClientIpRegion;
                    if (subManager.SignUpClientIpRegion == "未知地址")
                    {
                        managerResultData.SignUpClientIp = "*.*.*.*";
                    }

                    managerResultData.LastSignInClientIp = subManager.LastSignInClientIp;
                    managerResultData.LastSignInClientIpRegion = subManager.LastSignInClientIpRegion;
                    if (subManager.LastSignInClientIpRegion == "未知地址")
                    {
                        managerResultData.LastSignInClientIp = "*.*.*.*";
                    }
                }

                if (managerType == ManagerType.Developer)
                {
                    managerResultData.OnlyDeveloperVisible = subManager.OnlyDeveloperVisible;
                }


                if (managerResultData.ManagerType == ManagerType.Agent)
                {
                    managerResultData.SubGroupLeaders = subManager.InverseAttributionAgentU.Count(o => o.ManagerType == (int)ManagerType.GroupLeader);
                    managerResultData.SubSalesmans = subManager.InverseAttributionAgentU.Count(o => o.ManagerType == (int)ManagerType.Salesman);
                    managerResultData.ApprovedDappUsers = subManager.UserAttributionAgentUs.Count(o => o.UserAsset != null && o.UserAsset.Approved);
                    managerResultData.OnlineCustomerServiceEnabled = subManager.OnlineCustomerServiceEnabled;
                    managerResultData.OnlineCustomerServiceChatWootKey = subManager.OnlineCustomerServiceChatWootKey;
                }
                else if (managerResultData.ManagerType == ManagerType.GroupLeader)
                {
                    managerResultData.SubSalesmans = subManager.InverseAttributionGroupLeaderU.Count(o => o.ManagerType == (int)ManagerType.Salesman);
                    managerResultData.ApprovedDappUsers = subManager.UserAttributionGroupLeaderUs.Count(o => o.UserAsset != null && o.UserAsset.Approved);
                    if (subManager.AttributionAgentU is not null)
                    {
                        managerResultData.OnlineCustomerServiceEnabled = subManager.AttributionAgentU.OnlineCustomerServiceEnabled;
                        managerResultData.OnlineCustomerServiceChatWootKey = subManager.AttributionAgentU.OnlineCustomerServiceChatWootKey;
                    }
                }
                else if (managerResultData.ManagerType == ManagerType.Salesman)
                {
                    managerResultData.ApprovedDappUsers = subManager.UserAttributionSalesmanUs.Count(o => o.UserAsset != null && o.UserAsset.Approved);
                    if (subManager.AttributionAgentU is not null)
                    {
                        managerResultData.OnlineCustomerServiceEnabled = subManager.AttributionAgentU.OnlineCustomerServiceEnabled;
                        managerResultData.OnlineCustomerServiceChatWootKey = subManager.AttributionAgentU.OnlineCustomerServiceChatWootKey;
                    }
                }
                result.Items.Add(managerResultData);
            }

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 查询下级员工
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Policy = AuthorizationPolicyName.Agent)]
        public WrappedResult<List<ManagementSubManagerResult>> QueryAttributionManagers([FromQuery] ManagerType attributionManagerType)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取员工信息");
            }
            ManagerType managerType = Manager.ManagerType;
            if (attributionManagerType >= managerType || (attributionManagerType != ManagerType.GroupLeader && attributionManagerType != ManagerType.Agent))
            {
                return WrappedResult.Failed("查询失败，无效的员工类型");
            }
            Expression<Func<Manager, bool>> predicate = o => !o.Deleted && !o.Blocked && o.ManagerType == (int)attributionManagerType;
            if (managerType != ManagerType.Administrator && managerType != ManagerType.Developer)
            {
                predicate = predicate.And(o => o.AttributionAgentUid == Manager.Uid);
            }

            // 非系统管理员无法查询隐藏代理
            if (managerType == ManagerType.Administrator)
            {
                predicate = predicate.And(o => !o.OnlyDeveloperVisible && (o.AttributionAgentU == null || !o.AttributionAgentU.OnlyDeveloperVisible));
            }

            var managers = _dbContext.Managers
                .AsNoTracking()
                .Where(predicate)
                .ToList();
            if (managers.Count < 1)
            {
                return WrappedResult.Failed("查询失败，没有找到下级员工");
            }
            var result = new List<ManagementSubManagerResult>();
            foreach (var subManager in managers)
            {
                result.Add(new()
                {
                    Uid = subManager.Uid,
                    ManagerType = (ManagerType)subManager.ManagerType,
                    Username = subManager.Username,
                });
            }
            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 修改代理客服配置
        /// </summary>
        [HttpPost, Authorize(Policy = AuthorizationPolicyName.Administrator)]
        public WrappedResult UpdateAgentCustomerServiceConfig(ManagementUpdateAgentCustomerServiceConfigRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取员工信息");
            }

            // 非系统管理员无法查询隐藏代理
            Expression<Func<Manager, bool>> predicate = o => o.Uid == request.Uid;
            if (Manager.ManagerType == ManagerType.Administrator)
            {
                predicate = predicate.And(o => !o.OnlyDeveloperVisible);
            }
            Manager? targetManager = _dbContext.Managers
                .AsNoTracking()
                .FirstOrDefault(predicate);
            if (targetManager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取目标员工信息");
            }

            if (targetManager.ManagerType != (int)ManagerType.Agent)
            {
                return WrappedResult.Failed("修改失败，无法为非代理用户配置在线客服");
            }

            if (request.OnlineCustomerServiceEnabled && (string.IsNullOrWhiteSpace(request.OnlineCustomerServiceChatWootKey) || request.OnlineCustomerServiceChatWootKey.Length > 24))
            {
                return WrappedResult.Failed("修改失败，Chat Woot 密钥长度错误");
            }

            // 修改代理客服配置
            targetManager.OnlineCustomerServiceEnabled = request.OnlineCustomerServiceEnabled;
            targetManager.OnlineCustomerServiceChatWootKey = string.IsNullOrWhiteSpace(request.OnlineCustomerServiceChatWootKey) ? null : request.OnlineCustomerServiceChatWootKey;

            // 写操作日志
            ManagerOperationLog operationLog = new()
            {
                OperatorUid = Manager.Uid,
                OperationType = (int)ManagerOperationType.UpdateAgentCustomerServiceConfig,
                TargetManagerUid = targetManager.Uid,
                CreateTime = DateTime.UtcNow,
                Comment = $"修改代理[{targetManager.Username}]客服配置，客服开关状态[{targetManager.OnlineCustomerServiceEnabled}]，Chat Woot 密钥[{targetManager.OnlineCustomerServiceChatWootKey}]"
            };
            _dbContext.ManagerOperationLogs.Add(operationLog);
            _dbContext.Managers.Update(targetManager);

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("服务器繁忙，请稍后重试");
            }
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 更新代理额度
        /// </summary>
        [HttpPost, Authorize(Policy = AuthorizationPolicyName.Administrator)]
        public WrappedResult UpdateAgentBalanceAssets(ManagementUpdateAgentBalanceAssetsRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("操作失败，无法获取员工信息");
            }

            // 非系统管理员无法查询隐藏代理
            Expression<Func<Manager, bool>> predicate = o => o.Uid == request.Uid;
            if (Manager.ManagerType == ManagerType.Administrator)
            {
                predicate = predicate.And(o => !o.OnlyDeveloperVisible);
            }

            Manager? targetManager = _dbContext.Managers
                .AsNoTracking()
                .FirstOrDefault(predicate);
            if (targetManager is null)
            {
                return WrappedResult.Failed("操作失败，无法获取目标员工信息");
            }
            if (targetManager.ManagerType != (int)ManagerType.Agent)
            {
                return WrappedResult.Failed("操作失败，目标员工类型错误");
            }

            ManagerBalanceChange? change;
            ManagerBalanceChangeType? changeType;
            var targetManagerBalance = targetManager.BalanceAssets;
            if (request.UpdateAgentBalanceAssetsType == UpdateAgentBalanceAssetsType.Increase)
            {
                if (targetManagerBalance + request.Amount > Web3Provider.MaxTokenDecimalValue)
                {
                    return WrappedResult.Failed("操作失败，目标员工额度超出最大值");
                }
                changeType = ManagerBalanceChangeType.SystemRecharge;
                change = new()
                {
                    Uid = targetManager.Uid,
                    ChangeType = (int)changeType,
                    Change = request.Amount,
                    Before = targetManagerBalance,
                    After = targetManagerBalance + request.Amount,
                    CreateTime = DateTime.UtcNow
                };
                _dbContext.ManagerBalanceChanges.Add(change);
                targetManager.BalanceAssets += request.Amount;
            }
            else
            {
                if (targetManagerBalance < request.Amount)
                {
                    return WrappedResult.Failed("操作失败，目标员工额度不足");
                }
                changeType = ManagerBalanceChangeType.SystemDeduction;
                change = new()
                {
                    Uid = targetManager.Uid,
                    ChangeType = (int)changeType,
                    Change = request.Amount,
                    Before = targetManagerBalance,
                    After = targetManagerBalance - request.Amount,
                    CreateTime = DateTime.UtcNow
                };
                _dbContext.ManagerBalanceChanges.Add(change);
                targetManager.BalanceAssets -= request.Amount;
            }
            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                if (request.Comment.Length > 256)
                {
                    return WrappedResult.Failed("操作失败，备注内容长度不可超过256个字符");
                }
                change.Comment = request.Comment;
            }

            // 写操作日志
            ManagerOperationLog operationLog = new()
            {
                OperatorUid = Manager.Uid,
                OperationType = (int)ManagerOperationType.UpdateAgentBalanceAssets,
                TargetManagerUid = targetManager.Uid,
                CreateTime = DateTime.UtcNow,
                Comment = $"{changeType.GetDescription()}，代理[{targetManager.Username}]，金额[{request.Amount}]"
            };
            _dbContext.ManagerOperationLogs.Add(operationLog);
            _dbContext.Managers.Update(targetManager);

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("服务器繁忙，请稍后重试");
            }
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 更新代理和下级员工禁用状态
        /// </summary>
        [HttpPost, Authorize(Policy = AuthorizationPolicyName.Administrator)]
        public WrappedResult UpdateAgentAndSubManagersBlockState(ManagementUpdateManagerBlockStateRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取员工信息");
            }

            // 非系统管理员无法查询隐藏代理
            Expression<Func<Manager, bool>> predicate = o => o.Uid == request.Uid;
            if (Manager.ManagerType == ManagerType.Administrator)
            {
                predicate = predicate.And(o => !o.OnlyDeveloperVisible);
            }

            Manager? targetManager = _dbContext.Managers
                .AsNoTracking()
                .FirstOrDefault(predicate);
            if (targetManager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取目标员工信息");
            }
            if (targetManager.ManagerType != (int)ManagerType.Agent)
            {
                return WrappedResult.Failed("修改失败，目标员工类型错误");
            }

            _dbContext.Managers
                .Where(o => o.Uid == targetManager.Uid || o.AttributionAgentUid == targetManager.Uid)
                .ExecuteUpdate(s => s.SetProperty(e => e.Blocked, request.Blocked));

            // 写操作日志
            ManagerOperationLog operationLog = new()
            {
                OperatorUid = Manager.Uid,
                OperationType = (int)ManagerOperationType.UpdateAgentAndSubManagersBlockState,
                TargetManagerUid = targetManager.Uid,
                CreateTime = DateTime.UtcNow,
                Comment = $"更改代理线[{targetManager.Username}]禁用状态为[{request.Blocked}]"
            };
            _dbContext.ManagerOperationLogs.Add(operationLog);

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("服务器繁忙，请稍后重试");
            }
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 更新管理员禁用状态
        /// </summary>
        [HttpPost, Authorize(Policy = AuthorizationPolicyName.Developer)]
        public WrappedResult UpdateAdministratorBlockState(ManagementUpdateManagerBlockStateRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取员工信息");
            }

            Manager? targetManager = _dbContext.Managers
                .AsNoTracking()
                .FirstOrDefault(o => o.Uid == request.Uid);
            if (targetManager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取目标员工信息");
            }
            if (targetManager.ManagerType != (int)ManagerType.Administrator)
            {
                return WrappedResult.Failed("修改失败，目标员工类型错误");
            }

            // 修改数据
            targetManager.Blocked = request.Blocked;
            _dbContext.Managers.Update(targetManager);

            // 写操作日志
            ManagerOperationLog operationLog = new()
            {
                OperatorUid = Manager.Uid,
                OperationType = (int)ManagerOperationType.UpdateAdministratorBlockState,
                TargetManagerUid = targetManager.Uid,
                CreateTime = DateTime.UtcNow,
                Comment = $"更改管理员[{targetManager.Username}]禁用状态为[{request.Blocked}]"
            };
            _dbContext.ManagerOperationLogs.Add(operationLog);

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("服务器繁忙，请稍后重试");
            }
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 更新代理仅开发者可见状态
        /// </summary>
        [HttpPost, Authorize(Policy = AuthorizationPolicyName.Developer)]
        public WrappedResult ChangeAgentOnlyDeveloperVisible(ManagementChangeAgentOnlyDeveloperVisibleRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取员工信息");
            }

            Manager? targetManager = _dbContext.Managers
                .AsNoTracking()
                .FirstOrDefault(o => o.Uid == request.Uid);
            if (targetManager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取目标员工信息");
            }
            if (targetManager.ManagerType != (int)ManagerType.Agent)
            {
                return WrappedResult.Failed("修改失败，目标员工类型错误");
            }

            // 修改数据
            targetManager.OnlyDeveloperVisible = request.OnlyDeveloperVisible;
            _dbContext.Managers.Update(targetManager);

            // 写操作日志
            ManagerOperationLog operationLog = new()
            {
                OperatorUid = Manager.Uid,
                OperationType = (int)ManagerOperationType.UpdateAdministratorBlockState,
                TargetManagerUid = targetManager.Uid,
                CreateTime = DateTime.UtcNow,
                Comment = $"修改代理[{targetManager.Username}]仅开发者可见状态为[{request.OnlyDeveloperVisible}]"
            };
            _dbContext.ManagerOperationLogs.Add(operationLog);

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("服务器繁忙，请稍后重试");
            }
            return WrappedResult.Ok();
        }
    }
}

