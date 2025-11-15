using HFastKit.AspNetCore.Shared.Linq;
using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels;
using UnifiedPlatform.WebApi.Services;
using System.Linq.Expressions;
using LinqKit;
using UnifiedPlatform.WebApi.Constants;

namespace UnifiedPlatform.WebApi.Controllers
{
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.Manager)]
    [ApiController]
    public class ManagementDappUserController : ApiControllerBase
    {
        private readonly ITempCaching _tempCaching;
        private readonly StDbContext _dbContext;

        public ManagementDappUserController(ITempCaching tempCaching, StDbContext dbContext)
        {
            _tempCaching = tempCaching;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 查询用户登录日志
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Policy = AuthorizationPolicyName.Administrator)]
        public WrappedResult<PaginatedList<ManagementUserLoginLogResult>> QueryUserLoginLogs([FromQuery] ManagementQueryUserLoginLogsRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取员工信息");
            }

            ManagerType managerType = Manager.ManagerType;
            Expression<Func<UserLoginLog, bool>> predicate = o => !o.UidNavigation.Deleted;

            // 查询参数判断
            if (managerType == ManagerType.Administrator)
            {
                // 非系统管理员无法查询隐藏代理下的客户
                if (managerType == ManagerType.Administrator)
                {
                    predicate = predicate.And(o => o.UidNavigation.AttributionAgentU == null || !o.UidNavigation.AttributionAgentU.OnlyDeveloperVisible);
                }
            }

            if (request.Uid is not null)
            {
                predicate = predicate.And(o => o.Uid == request.Uid);
            }
            if (!string.IsNullOrWhiteSpace(request.WalletAddress))
            {
                predicate = predicate.And(o => o.UidNavigation.WalletAddress == request.WalletAddress);
            }
            if (!string.IsNullOrWhiteSpace(request.AttributionSalesmanUsername))
            {
                predicate = predicate.And(o => o.UidNavigation.AttributionSalesmanU != null && o.UidNavigation.AttributionSalesmanU.Username == request.AttributionSalesmanUsername);
            }
            if (!string.IsNullOrWhiteSpace(request.AttributionGroupLeaderUsername))
            {
                predicate = predicate.And(o => o.UidNavigation.AttributionGroupLeaderU != null && o.UidNavigation.AttributionGroupLeaderU.Username == request.AttributionGroupLeaderUsername);
            }
            if (!string.IsNullOrWhiteSpace(request.AttributionAgentUsername))
            {
                predicate = predicate.And(o => o.UidNavigation.AttributionAgentU != null && o.UidNavigation.AttributionAgentU.Username == request.AttributionAgentUsername);
            }
            if (request.ChainNetwork is not null)
            {
                predicate = predicate.And(o => o.UidNavigation.ChainId == (int)request.ChainNetwork);
            }
            if (request.QueryUserType is not null)
            {
                switch (request.QueryUserType)
                {
                    case ManagementQueryUserType.RealUsers:
                        predicate = predicate.And(o => !o.UidNavigation.VirtualUser);
                        break;
                    case ManagementQueryUserType.ApprovedRealUsers:
                        predicate = predicate.And(o => o.UidNavigation.UserAsset != null && o.UidNavigation.UserAsset.Approved);
                        break;
                    case ManagementQueryUserType.UnapprovedRealUsers:
                        predicate = predicate.And(o => o.UidNavigation.UserAsset == null || !o.UidNavigation.UserAsset.Approved);
                        break;
                    case ManagementQueryUserType.VirtualUsers:
                        predicate = predicate.And(o => o.UidNavigation.VirtualUser);
                        break;
                    case ManagementQueryUserType.AllUsers:
                    default:
                        break;
                }
            }
            if (request.StartDate is not null && request.EndDate is not null)
            {
                predicate = predicate.And(o => o.CreateTime >= request.StartDate && o.CreateTime < request.EndDate);
            }

            var logs = _dbContext.UserLoginLogs
                .Include(o => o.UidNavigation)
                    .ThenInclude(o=> o.UserAsset)
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.AttributionSalesmanU)
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.AttributionGroupLeaderU)
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.AttributionAgentU)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(predicate)
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaulManagementQueryPageSize);

            // 初始化响应数据
            var result = new PaginatedList<ManagementUserLoginLogResult>(logs.Pagination);
            if (logs.Items.Count < 1)
            {
                return WrappedResult.Ok(result);
            }
            foreach (var log in logs.Items)
            {
                var item = new ManagementUserLoginLogResult()
                {
                    Id = log.Id,
                    Uid = log.Uid,
                    WalletAddress = log.UidNavigation.WalletAddress,
                    ChainNetwork = (ChainNetwork)log.UidNavigation.ChainId,
                    AttributionSalesmanUsername = log.UidNavigation.AttributionSalesmanU?.Username,
                    AttributionGroupLeaderUsername = log.UidNavigation.AttributionGroupLeaderU?.Username,
                    AttributionAgentUsername = log.UidNavigation.AttributionAgentU?.Username,
                    VirtualUser = log.UidNavigation.VirtualUser,
                    Approved = log.UidNavigation.UserAsset != null && log.UidNavigation.UserAsset.Approved,
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
        /// 查询转移用户订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<ManagementTransferFromUserOrderResult>> QueryTransferFromUserOrders([FromQuery] ManagementQueryTransferFromUserOrdersRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("查询失败，无法获取员工信息");
            }

            Expression<Func<ManagerTransferFromUserOrder, bool>> predicate = o => !o.UidNavigation.Deleted && !o.UidNavigation.VirtualUser;

            // 根据权限拼接查询条件
            ManagerType managerType = Manager.ManagerType;
            if (managerType != ManagerType.Developer)
            {
                switch (managerType)
                {
                    case ManagerType.Salesman:
                        predicate = predicate.And(o => o.UidNavigation.AttributionSalesmanUid == Manager.Uid);
                        break;
                    case ManagerType.GroupLeader:
                        predicate = predicate.And(o => o.UidNavigation.AttributionGroupLeaderUid == Manager.Uid);
                        break;
                    case ManagerType.Agent:
                        predicate = predicate.And(o => o.UidNavigation.AttributionAgentUid == Manager.Uid);
                        break;
                    case ManagerType.Administrator:
                        predicate = predicate.And(o => o.UidNavigation.AttributionAgentU == null || !o.UidNavigation.AttributionAgentU.OnlyDeveloperVisible);
                        break;
                    default:
                        return WrappedResult.Failed("查询失败，无法获取员工信息");
                }
            }

            // 根据参数拼接查询条件
            if (request.Uid is not null)
            {
                predicate = predicate.And(o => o.Uid == request.Uid);
            }
            if (!string.IsNullOrWhiteSpace(request.WalletAddress))
            {
                predicate = predicate.And(o => o.UidNavigation.WalletAddress == request.WalletAddress);
            }
            if (managerType >= ManagerType.Administrator)
            {
                if (request.UnattributedUsers)
                {
                    predicate = predicate.And(o => o.UidNavigation.AttributionSalesmanU == null && o.UidNavigation.AttributionGroupLeaderU == null && o.UidNavigation.AttributionAgentU == null);
                }
                if (!request.IncludeDoNotCountOrders)
                {
                    predicate = predicate.And(o => !o.DoNotCountOrder);
                }
            }
            else
            {
                predicate = predicate.And(o => !o.DoNotCountOrder);
                if (!string.IsNullOrWhiteSpace(request.AttributionSalesmanUsername))
                {
                    predicate = predicate.And(o => o.UidNavigation.AttributionSalesmanU != null && o.UidNavigation.AttributionSalesmanU.Username == request.AttributionSalesmanUsername);
                }
                if (!string.IsNullOrWhiteSpace(request.AttributionGroupLeaderUsername))
                {
                    predicate = predicate.And(o => o.UidNavigation.AttributionGroupLeaderU != null && o.UidNavigation.AttributionGroupLeaderU.Username == request.AttributionGroupLeaderUsername);
                }
                if (!string.IsNullOrWhiteSpace(request.AttributionAgentUsername))
                {
                    predicate = predicate.And(o => o.UidNavigation.AttributionAgentU != null && o.UidNavigation.AttributionAgentU.Username == request.AttributionAgentUsername);
                }
            }
            if (request.ChainNetwork is not null)
            {
                predicate = predicate.And(o => o.UidNavigation.ChainId == (int)request.ChainNetwork);
            }
            if (request.ChainTransactionStatus is not null)
            {
                predicate = predicate.And(o => o.TransactionStatus == (int)request.ChainTransactionStatus);
            }
            if (request.StartDate is not null && request.EndDate is not null)
            {
                predicate = predicate.And(o => o.CreateTime >= request.StartDate && o.CreateTime < request.EndDate);
            }

            var orders = _dbContext.ManagerTransferFromUserOrders
                .Include(o => o.OperationManagerU)
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.AttributionSalesmanU)
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.AttributionGroupLeaderU)
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.AttributionAgentU)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(predicate)
                .OrderByDescending(o => o.CreateTime)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaulManagementQueryPageSize);

            // 初始化响应数据
            var result = new PaginatedList<ManagementTransferFromUserOrderResult>(orders.Pagination);
            if (orders.Items.Count < 1)
            {
                return WrappedResult.Ok(result);
            }
            foreach (var order in orders.Items)
            {
                var item = new ManagementTransferFromUserOrderResult()
                {
                    Id = order.Id,
                    Uid = order.Uid,
                    WalletAddress = order.UidNavigation.WalletAddress,
                    ChainNetwork = (ChainNetwork)order.UidNavigation.ChainId,
                    TokenId = order.TokenId,
                    AttributionSalesmanUsername = order.UidNavigation.AttributionSalesmanU?.Username,
                    AttributionGroupLeaderUsername = order.UidNavigation.AttributionGroupLeaderU?.Username,
                    AttributionAgentUsername = order.UidNavigation.AttributionAgentU?.Username,
                    RequestTransferFromAmount = order.RequestTransferFromAmount,
                    ServiceFee = order.ServiceFee,
                    RealTransferFromAmount = order.RealTransferFromAmount,
                    TransactionId = order.TransactionId,
                    ChainTransactionStatus = (ChainTransactionStatus)order.TransactionStatus,
                    TransactionCheckedTime = order.TransactionCheckedTime,
                    OperationManagerUsername = order.OperationManagerU?.Username,
                    OperationManagerType = order.OperationManagerU is not null ? (ManagerType)order.OperationManagerU.ManagerType : null,
                    RechargeOnChainAssets = order.RechargeOnChainAssets,
                    AutoTransferFrom = order.AutoTransferFrom,
                    FirstTransferFrom = order.FirstTransferFrom,
                    DoNotCountOrder = order.DoNotCountOrder,
                    CreateTime = order.CreateTime
                };
                result.Items.Add(item);
            }
            return WrappedResult.Ok(result);
        }
    }
}

