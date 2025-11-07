using HFastKit.AspNetCore.Http;
using HFastKit.AspNetCore.Services.Captcha;
using HFastKit.AspNetCore.Services.Jwt;
using HFastKit.AspNetCore.Shared;
using HFastKit.AspNetCore.Shared.Extensions;
using HFastKit.AspNetCore.Shared.Linq;
using HFastKit.Extensions;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmallTarget.DbService.Entities;
using SmallTarget.Shared;
using SmallTarget.Shared.ActionModels;
using SmallTarget.WebApi.Constants;
using SmallTarget.WebApi.Services;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace SmallTarget.WebApi.Controllers
{
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.DappUser)]
    [ApiController]
    public class DappUserController : ApiControllerBase
    {
        private readonly ITempCaching _tempCaching;
        private readonly ITempSignToken _tempSignToken;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly ISearcher _searcher;
        private readonly StDbContext _dbContext;

        public DappUserController(ITempCaching tempCaching, ITempSignToken tempSignToken, IOptions<JwtOptions> jwtOptions, ISearcher searcher, StDbContext dbContext)
        {
            _tempCaching = tempCaching;
            _tempSignToken = tempSignToken;
            _jwtOptions = jwtOptions;
            _searcher = searcher;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public WrappedResult<DappUserCheckSinginedResult> CheckSignined(DappUserWalletRequest request)
        {
            DappUserCheckSinginedResult resultData = new()
            {
                Singined = false
            };

            // 是否经过认证
            var requestWalletAddress = request.WalletAddress.ToLower();
            if (User.Identity?.IsAuthenticated is true)
            {
                if (DappUser is not null && DappUser.ChainId == (int)request.ChainId && DappUser.WalletAddress.ToLower() == requestWalletAddress)
                {
                    User? user = _dbContext.Users
                        .FirstOrDefault(o => o.ChainId == DappUser.ChainId && o.WalletAddress.ToLower() == requestWalletAddress);
                    if (user is null)
                    {
                        return WrappedResult.Failed("Unable to obtain user information");
                    }
                    string clientIp = HttpContext.GetRemoteIp();
                    user.LastSignInClientIp = clientIp;

                    UserLoginLog loginLog = new()
                    {
                        Uid = user.Uid,
                        ClientIp = clientIp,
                        ClientIpRegion = _searcher.SearchAndFix(clientIp),
                        CreateTime = DateTime.UtcNow
                    };
                    _dbContext.UserLoginLogs.Add(loginLog);
                    try
                    {
                        _dbContext.SaveChanges();
                    }
                    catch
                    {
                        return WrappedResult.Failed("Server is busy, please try again later");
                    }

                    resultData.Singined = true;
                    return WrappedResult.Ok(resultData);
                }
            }

            // 获取或创建临时登录令牌
            if (_tempSignToken.TryCreateOrGet(request.ChainId, requestWalletAddress, 600, out SignToken? signToken))
            {
                resultData.TokenText = Convert.ToHexString(Encoding.UTF8.GetBytes(signToken.SignatureContent));
                resultData.Singined = false;
                return WrappedResult.Ok(resultData);
            }

            return WrappedResult.Failed("Unable to obtain signature content, please try again later");
        }

        /// <summary>
        /// 请求登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public WrappedResult<DappSigninResult> Signin(DappUserSigninRequest request)
        {
            string requestWalletAddress = request.WalletAddress.ToLower();

            // 验证签名
            SignTokenVerifyResult verifyResult = _tempSignToken.VerifySign(request.ChainId, requestWalletAddress, request.SignedText);
            if (!verifyResult.IsVerified)
            {
                return WrappedResult.Failed(verifyResult.ErrorMessage);
            }
            Guid accesTokenGuid = (Guid)verifyResult.Guid!;

            // 验证区块链网络
            var chainId = (int)request.ChainId;
            var networkConfig = _tempCaching.ChainNetworkConfigs.FirstOrDefault(o => o.ChainId == chainId);
            if (networkConfig is null)
            {
                return WrappedResult.Failed("Unsupported network node");
            }

            // 查找已存在用户
            User? user = _dbContext.Users
                .AsNoTracking()
                .FirstOrDefault(o => o.ChainId == chainId && o.WalletAddress.ToLower() == requestWalletAddress);
            string clientIp = HttpContext.GetRemoteIp();
            string? clientIpRegion = _searcher.SearchAndFix(clientIp);

            /*****新用户*****/
            if (user is null)
            {
                // 新用户验证推荐令牌
                var invitationLinkTokenText = request.InvitationLinkToken;

                int? attrSalesmanUid = null;
                int? attrGroupLeaderUid = null;
                int? attrAgentUid = null;
                int? parentUserUid = null;
                string? parentUsersPath = null;
                int parentUserDepth = 0;

                if (InvitationLinkToken.TryDeserialize(invitationLinkTokenText, out InvitationLinkToken? invitationLinkToken))
                {
                    // 有推荐人
                    if (invitationLinkToken.InviterType == InviterType.DappUser)
                    {
                        // 用户推荐
                        User? inviter = _dbContext.Users
                            .AsNoTracking()
                            .FirstOrDefault(o => !o.Deleted && o.Uid == invitationLinkToken.Uid);
                        if (inviter is not null)
                        {
                            parentUserUid = inviter.Uid;
                            attrSalesmanUid = inviter.AttributionSalesmanUid;
                            attrGroupLeaderUid = inviter.AttributionGroupLeaderUid;
                            attrAgentUid = inviter.AttributionAgentUid;
                            parentUsersPath = inviter.ParentUsersPath is null ? $"/{inviter.Uid}/" : $"{inviter.ParentUsersPath}{inviter.Uid}/";
                            parentUserDepth = ++inviter.ParentUserDepth;
                        }
                    }
                    else
                    {
                        // 员工推荐
                        Manager? manager = _dbContext.Managers
                            .AsNoTracking()
                            .FirstOrDefault(o => !o.Blocked && !o.Deleted && o.Uid == invitationLinkToken.Uid);
                        if (manager is not null)
                        {
                            ManagerType managerType = (ManagerType)manager.ManagerType;
                            switch (managerType)
                            {
                                case ManagerType.Salesman:
                                    attrSalesmanUid = manager.Uid;
                                    attrGroupLeaderUid = manager.AttributionGroupLeaderUid;
                                    attrAgentUid = manager.AttributionAgentUid;
                                    break;
                                case ManagerType.GroupLeader:
                                    attrGroupLeaderUid = manager.Uid;
                                    attrAgentUid = manager.AttributionAgentUid;
                                    break;
                                case ManagerType.Agent:
                                    attrAgentUid = manager.Uid;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                // 创建新用户
                user = new User
                {
                    WalletAddress = requestWalletAddress,
                    ChainId = chainId,
                    ChainWalletConfigGroupId = _tempCaching.GlobalConfig.ChainWalletConfigGroupId,
                    UserLevel = _tempCaching.UserLevelConfigs.Min(o => o.UserLevel),
                    AttributionSalesmanUid = attrSalesmanUid,
                    AttributionGroupLeaderUid = attrGroupLeaderUid,
                    AttributionAgentUid = attrAgentUid,
                    ParentUserUid = parentUserUid,
                    ParentUsersPath = parentUsersPath,
                    ParentUserDepth = parentUserDepth,
                    SignUpClientIp = clientIp,
                    SignUpClientIpRegion = clientIpRegion,
                    CreateTime = DateTime.UtcNow,
                };

                _dbContext.Users.Add(user);

                // 先保存到数据库，否则处理用户路径节点时会报约束错误
                try
                {
                    _dbContext.SaveChanges();
                }
                catch
                {
                    return WrappedResult.Failed("Server is busy, please try again later");
                }

                // 新用户消息
                var message = new UserSysteamMessage()
                {
                    Uid = user.Uid,
                    MessageTitle = LocalConfig.NewUserMessageTempLateTitle,
                    MessageContent = LocalConfig.NewUserMessageTempLateContent,
                };
                _dbContext.UserSysteamMessages.Add(message);

                // 处理用户路径节点
                if (!string.IsNullOrEmpty(parentUsersPath))
                {
                    List<UserPathNode> userPathNodes = new();
                    List<int>? parentList = parentUsersPath.Split("/")?.ToList()?.Where(o => !string.IsNullOrEmpty(o))?.ToList()?.ConvertAll(o => Convert.ToInt32(o));
                    if (parentList is null || parentList.Count < 1)
                    {
                        return WrappedResult.Failed("An error occurred while saving data");
                    }
                    int layer = parentList.Count;
                    foreach (var parent in parentList)
                    {
                        UserPathNode userPathNode = new()
                        {
                            Uid = parent,
                            SubUserUid = user.Uid,
                            SubUserLayer = layer,
                            CreateTime = DateTime.UtcNow,
                        };
                        layer--;
                        userPathNodes.Add(userPathNode);
                    }
                    _dbContext.AddRange(userPathNodes);
                }
            }
            /*****新用户*****/

            if (user is null)
            {
                return WrappedResult.Failed("An unknown error has occurred");
            }
            if (user.Blocked)
            {
                return WrappedResult.Failed("The user has been banned");
            }
            if (user.Deleted)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            // 登录
            user.AccesTokenGuid = accesTokenGuid;
            user.LastSignInClientIp = clientIp;
            user.LastSignInClientIpRegion = clientIpRegion;
            _dbContext.Update(user);
            UserLoginLog loginLog = new()
            {
                Uid = user.Uid,
                ClientIp = clientIp,
                ClientIpRegion = clientIpRegion,
                CreateTime = DateTime.UtcNow
            };
            _dbContext.UserLoginLogs.Add(loginLog);
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("Server is busy, please try again later");
            }

            // 生成Jwt令牌
            List<Claim> claims = new()
            {
                new(JwtClaimKeyName.Uid, user.Uid.ToString()),
                new(JwtClaimKeyName.AccesTokenGuid, accesTokenGuid.ToString()),
                new(JwtClaimKeyName.WalletAddress, requestWalletAddress),
                new(JwtClaimKeyName.ChainId, chainId.ToString()),
                new(JwtClaimKeyName.RequestUserType, ((int)WebApiRequestUserType.DappUser).ToString()),
                new(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.DappUser),
            };
            string accessToken = JwtHelper.CreateToken(_jwtOptions.Value.Audience, _jwtOptions.Value.Issuer, _jwtOptions.Value.SecurityKey, claims.ToArray());

            return WrappedResult.Ok(new DappSigninResult()
            {
                AccessToken = accessToken,
            });
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<DappUserResult> GetUser()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            User? user = _dbContext.Users
                .Include(o => o.UserAsset)
                .Include(o => o.UserChainTransactions.Where(o => o.TransactionStatus == (int)ChainTransactionStatus.None || o.TransactionStatus == (int)ChainTransactionStatus.Pending))
                .Include(o => o.UserPathNodeUidNavigations.Where(o => o.SubUserLayer <= 2))
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            // 响应数据
            DappUserResult resultData = new DappUserResult
            {
                Uid = user.Uid,
                ChainId = user.ChainId,
                UserLevel = user.UserLevel,
                SignUpClientIp = user.SignUpClientIp,
                LastSignInClientIp = user.LastSignInClientIp,
                VirtualUser = user.VirtualUser,
                Anomaly = user.Anomaly,
                PrimaryTokenStatus = PrimaryTokenStatus.NotSet,
                Layer1Members = user.UserPathNodeUidNavigations.Count(o => o.SubUserLayer == 1),
                Layer2Members = user.UserPathNodeUidNavigations.Count(o => o.SubUserLayer == 2),
            };
            resultData.NewSystemMessage = _dbContext.UserSysteamMessages.Any(o => o.Uid == user.Uid && !o.IsRead);
            if (user.UserAsset is not null)
            {
                var userAsset = user.UserAsset;
                resultData.Asset = new()
                {
                    PrimaryTokenId = userAsset.PrimaryTokenId,
                    CurrencyWalletBalance = userAsset.CurrencyWalletBalance,
                    PrimaryTokenWalletBalance = userAsset.PrimaryTokenWalletBalance,
                    OnChainAssets = userAsset.OnChainAssets,
                    Approved = userAsset.Approved,
                    AITradingActivated = userAsset.AiTradingActivated,
                    AiTradingRemainingTimes = userAsset.AiTradingRemainingTimes,
                    MiningActivityPoint = userAsset.MiningActivityPoint,
                    TotalToChain = userAsset.TotalToChain,
                    TotalToWallet = userAsset.TotalToWallet,
                    TotalAiTradingRewards = userAsset.TotalAiTradingRewards,
                    TotalMiningRewards = userAsset.TotalMiningRewards,
                    TotalInvitationRewards = userAsset.TotalInvitationRewards,
                    TotalSystemRewards = userAsset.TotalSystemRewards
                };
                if (userAsset.Approved)
                {
                    resultData.PrimaryTokenStatus = PrimaryTokenStatus.Completed;
                }
            }
            else
            {
                if (user.UserChainTransactions.Any())
                {
                    resultData.PrimaryTokenStatus = PrimaryTokenStatus.Pending;
                }
                else
                {
                    resultData.PrimaryTokenStatus = PrimaryTokenStatus.NotSet;
                }

            }

            // 邀请链接
            var baseUri = _tempCaching.DappUrls[0];
            string invitationLinkToken = InvitationLinkToken.Create(user.Uid, InviterType.DappUser).Token;
            resultData.InvitationLink = $"{baseUri.AbsoluteUri}?inviter={invitationLinkToken}";

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取挖矿状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<DappUserMiningStateResult> GetMiningState()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            User? user = _dbContext.Users
                .Include(o => o.UserAsset)
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            // 响应数据
            DappUserMiningStateResult resultData = new DappUserMiningStateResult
            {
                MiningStatus = UserMiningStatus.MiningStopped,
                MiningStatusName = UserMiningStatus.MiningStopped.GetDescription(),
            };

            // 账户异常
            if (user.Anomaly)
            {
                resultData.StopedTip = "Your account is at risk and cannot interact with the smart contract";
                return WrappedResult.Ok(resultData);
            }

            // 未激活账户
            if (user.UserLevel < 1 || user.UserAsset is null)
            {
                resultData.StopedTip = "You are not a delegator and do not have permission to interact with the smart contract";
                return WrappedResult.Ok(resultData);
            }

            // 活跃度不足
            if (user.UserAsset.MiningActivityPoint < 1)
            {
                resultData.StopedTip = "You don't have activity points remaining to interact with the smart contract";
                return WrappedResult.Ok(resultData);
            }

            var validAssets = user.UserAsset.OnChainAssets + user.UserAsset.PrimaryTokenWalletBalance;
            if (user.UserAsset.OnChainAssets / validAssets >= _tempCaching.GlobalConfig.MiningSpeedUpRequiredOnChainAssetsRate)
            {
                resultData.MiningStatus = UserMiningStatus.HighSpeedMining;
                resultData.MiningStatusName = UserMiningStatus.HighSpeedMining.GetDescription();
                return WrappedResult.Ok(resultData);
            }
            resultData.MiningStatus = UserMiningStatus.StandardMining;
            resultData.MiningStatusName = UserMiningStatus.StandardMining.GetDescription();
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取AI交易状态状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<DappUserAiTradingStateResult> GetAiTradingState()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            User? user = _dbContext.Users
                .Include(o => o.UserAsset)
                .Include(o => o.UserAiTradingOrders.Where(o => o.Status == (int)UserAiTradingOrderStatus.Trading))
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            // 响应数据
            DappUserAiTradingStateResult resultData = new()
            {
                AiTradingStatus = UserAiTradingStatus.Error,
                AiTradingStatusName = UserAiTradingStatus.Error.GetDescription(),
                AiTradingStatusTip = null
            };

            // 账户异常
            if (user.Anomaly)
            {
                resultData.AiTradingStatusTip = "Your account is at risk and cannot interact with the smart contract";
                return WrappedResult.Ok(resultData);
            }

            // 未激活账户
            if (user.UserLevel < 1 || user.UserAsset is null)
            {
                resultData.AiTradingStatusTip = "You are not a delegator and do not have permission to interact with the smart contract";
                return WrappedResult.Ok(resultData);
            }

            // 有交易中的订单
            if (user.UserAiTradingOrders.Any())
            {
                var aiTradingOrder = user.UserAiTradingOrders.First();
                var orderCreateTime = aiTradingOrder.CreateTime;
                var orderEndTime = aiTradingOrder.OrderEndTime;
                var now = DateTime.UtcNow;
                if (orderEndTime <= orderCreateTime || now < orderCreateTime)
                {
                    return WrappedResult.Failed("An unknown error occurred");
                }
                var intervalMinutes = (orderEndTime - orderCreateTime).TotalSeconds;
                var elapsedMinutes = (now - orderCreateTime).TotalSeconds;
                var diffRate = elapsedMinutes / intervalMinutes;
                switch (diffRate)
                {
                    case <= 0.15:
                        resultData.AiTradingStatusTip = "Calculating route...";
                        break;
                    case > 0.15 and <= 0.3:
                        resultData.AiTradingStatusTip = "Creating order...";
                        break;
                    case > 0.3 and <= 0.5:
                        resultData.AiTradingStatusTip = "Waiting for market traders...";
                        break;
                    case > 0.5 and <= 0.8:
                        resultData.AiTradingStatusTip = "Cross-chain transaction in progress...";
                        break;
                    case > 0.8:
                        resultData.AiTradingStatusTip = "Verifying transaction signature...";
                        break;
                }

                resultData.AiTradingStatus = UserAiTradingStatus.Pending;
                resultData.AiTradingStatusName = UserAiTradingStatus.Pending.GetDescription();
                resultData.TransactionProgressValue = (int)Math.Floor(diffRate * 100);
                if (resultData.TransactionProgressValue > 99)
                {
                    resultData.TransactionProgressValue = 99;
                }
            }
            else
            {
                resultData.AiTradingStatus = UserAiTradingStatus.None;
                resultData.AiTradingStatusName = UserAiTradingStatus.None.GetDescription();
            }

            var levelConfig = _tempCaching.UserLevelConfigs.First(o => o.UserLevel == user.UserLevel);
            var validassets = user.UserAsset.OnChainAssets + user.UserAsset.PrimaryTokenWalletBalance;
            resultData.RequiresOnChainAssets = (validassets * levelConfig.AvailableAiTradingAssetsRate).FixedToZero();
            resultData.MinEstimatedIncome = (resultData.RequiresOnChainAssets.Value * levelConfig.MinEachAiTradingRewardRate).FixedToZero();
            resultData.MaxEstimatedIncome = (resultData.RequiresOnChainAssets.Value * levelConfig.MaxEachAiTradingRewardRate).FixedToZero();

            // 响应数据
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取系统短消息详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<DappUserSystemMessageDetailsResult> GetSystemMessageDetails([FromQuery] int messageId)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var message = _dbContext.UserSysteamMessages
                .Include(o => o.ActivationCode)
                .FirstOrDefault(o => o.Uid == DappUser.Uid && !o.Deleted && o.Id == messageId);

            if (message is null)
            {
                return WrappedResult.Failed("Message does not exist");
            }

            var resultData = new DappUserSystemMessageDetailsResult()
            {
                Id = messageId,
                Title = message.MessageTitle,
                Content = string.Empty,
                IsActivationCodeMessage = message.IsActivationCodeMessage,
                IsRead = message.IsRead,
                CreateTime = message.CreateTime,
            };

            // 处理短消息HTML格式
            if (message.MessageContent.Length > 0)
            {
                var contentList = message.MessageContent.Split(Environment.NewLine).ToList();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var content in contentList)
                {
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        stringBuilder.AppendLine("<br />");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"<p class=\"mt-2\" style=\"text-indent: 16px\">{content.Trim()}</p>");
                    }
                }
                resultData.Content = stringBuilder.ToString();
            }

            if (resultData.IsActivationCodeMessage)
            {
                if (message.ActivationCode is null)
                {
                    return WrappedResult.Failed("Message does not exist");
                }
                resultData.ActivationCodeGuid = message.ActivationCode.ActivationCodeGuid.ToString();
                resultData.ActivationCodeExpirationTime = message.ActivationCode.ExpirationTime;
            }

            message.IsRead = true;
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 查询系统短消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<DappUserSystemMessageResult>> QuerySystemMessages([FromQuery] DappUserQuerySystemMessagesRequest request)
        {
            var resultData = PaginatedList<DappUserSystemMessageResult>.Empty();
            if (DappUser is null)
            {
                return WrappedResult.Ok(resultData);
            }

            Expression<Func<UserSysteamMessage, bool>> predicate = o => o.Uid == DappUser.Uid && !o.Deleted;
            if (request.OnlyUnread.HasValue && request.OnlyUnread.Value)
            {
                predicate = predicate.And(o => !o.IsRead);
            }

            var messages = _dbContext.UserSysteamMessages
                .AsNoTracking()
                .OrderByDescending(o => o.CreateTime)
                .Where(predicate)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaultDappQueryPageSize);
            resultData = new PaginatedList<DappUserSystemMessageResult>(messages.Pagination);
            if (messages.Items.Count < 1)
            {
                return WrappedResult.Ok(resultData);
            }
            foreach (var message in messages.Items)
            {
                DappUserSystemMessageResult messageResult = new()
                {
                    Id = message.Id,
                    Title = message.MessageTitle,
                    Content = message.MessageContent.Length < 64 ? message.MessageContent : message.MessageContent.Substring(0, 64),
                    IsActivationCodeMessage = message.IsActivationCodeMessage,
                    IsRead = message.IsRead,
                    CreateTime = message.CreateTime,
                };
                resultData.Items.Add(messageResult);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 查询链上资产账变记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<DappUserOnChainAssetsChangeRecordResult>> QueryOnChainAssetsChangeRecords([FromQuery] QueryByPagingRequest request)
        {
            var resultData = PaginatedList<DappUserOnChainAssetsChangeRecordResult>.Empty();
            if (DappUser is null)
            {
                return WrappedResult.Ok(resultData);
            }

            var changeRecords = _dbContext.UserOnChainAssetsChanges
                .AsNoTracking()
                .OrderByDescending(o => o.CreateTime)
                .Where(o => o.Uid == DappUser.Uid)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaultDappQueryPageSize);
            resultData = new PaginatedList<DappUserOnChainAssetsChangeRecordResult>(changeRecords.Pagination);
            if (changeRecords.Items.Count < 1)
            {
                return WrappedResult.Ok(resultData);
            }
            foreach (var rewardRecord in changeRecords.Items)
            {
                DappUserOnChainAssetsChangeRecordResult rewardRecordResult = new()
                {
                    Id = rewardRecord.Id,
                    ChangeType = rewardRecord.ChangeType,
                    ChangeTypeName = ((UserOnChainAssetsChangeType)rewardRecord.ChangeType).GetDescription(),
                    Change = rewardRecord.Change,
                    Before = rewardRecord.Before,
                    After = rewardRecord.After,
                    Comment = rewardRecord.Comment,
                    CreateTime = DateTime.SpecifyKind(rewardRecord.CreateTime, DateTimeKind.Utc),
                };
                resultData.Items.Add(rewardRecordResult);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 查询挖矿奖励记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<DappUserMiningRewardRecordResult>> QueryMiningRewardRecords([FromQuery] QueryByPagingRequest request)
        {

            var resultData = PaginatedList<DappUserMiningRewardRecordResult>.Empty();
            if (DappUser is null)
            {
                return WrappedResult.Ok(resultData);
            }

            var rewardRecords = _dbContext.UserMiningRewardRecords
                .AsNoTracking()
                .OrderByDescending(o => o.CreateTime)
                .Where(o => o.Uid == DappUser.Uid)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaultDappQueryPageSize);
            resultData = new PaginatedList<DappUserMiningRewardRecordResult>(rewardRecords.Pagination);
            if (rewardRecords.Items.Count < 1)
            {
                return WrappedResult.Ok(resultData);
            }
            foreach (var rewardRecord in rewardRecords.Items)
            {
                DappUserMiningRewardRecordResult rewardRecordResult = new()
                {
                    Id = rewardRecord.Id,
                    ValidAssets = rewardRecord.ValidAssets,
                    RewardRate = rewardRecord.RewardRate,
                    Reward = rewardRecord.Reward,
                    SpeedUpMode = rewardRecord.SpeedUpMode,
                    Comment = rewardRecord.Comment,
                    CreateTime = DateTime.SpecifyKind(rewardRecord.CreateTime, DateTimeKind.Utc),
                };
                resultData.Items.Add(rewardRecordResult);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 查询 AI 合约交易订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<DappUserAiTradingOrderResult>> QueryAiTradingOrders([FromQuery] QueryByPagingRequest request)
        {

            var resultData = PaginatedList<DappUserAiTradingOrderResult>.Empty();
            if (DappUser is null)
            {
                return WrappedResult.Ok(resultData);
            }

            var orders = _dbContext.UserAiTradingOrders
                .AsNoTracking()
                .OrderByDescending(o => o.CreateTime)
                .Where(o => o.Uid == DappUser.Uid)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaultDappQueryPageSize);
            resultData = new PaginatedList<DappUserAiTradingOrderResult>(orders.Pagination);
            if (orders.Items.Count < 1)
            {
                return WrappedResult.Ok(resultData);
            }
            foreach (var order in orders.Items)
            {
                DappUserAiTradingOrderResult orderResult = new()
                {
                    Id = order.Id,
                    Amount = order.Amount,
                    RewardRate = order.RewardRate,
                    Reward = order.Reward,
                    Status = order.Status,
                    StatusName = ((UserAiTradingOrderStatus)order.Status).GetDescription(),
                    Comment = order.Comment,
                    CreateTime = DateTime.SpecifyKind(order.CreateTime, DateTimeKind.Utc),
                };
                resultData.Items.Add(orderResult);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 查询邀请奖励记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<DappUserInvitationRewardRecordResult>> QueryInvitationRewardRecords([FromQuery] QueryByPagingRequest request)
        {

            var resultData = PaginatedList<DappUserInvitationRewardRecordResult>.Empty();
            if (DappUser is null)
            {
                return WrappedResult.Ok(resultData);
            }

            var records = _dbContext.UserInvitationRewardRecords
                .AsNoTracking()
                .OrderByDescending(o => o.CreateTime)
                .Where(o => o.Uid == DappUser.Uid)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaultDappQueryPageSize);
            resultData = new PaginatedList<DappUserInvitationRewardRecordResult>(records.Pagination);
            if (records.Items.Count < 1)
            {
                return WrappedResult.Ok(resultData);
            }
            foreach (var record in records.Items)
            {
                DappUserInvitationRewardRecordResult recordResult = new()
                {
                    Id = record.Id,
                    SubUserLayer = record.SubUserLayer,
                    SubUserRewardType = record.SubUserRewardType,
                    SubUserRewardTypeName = $"Sub User {((UserInvitationRewardType)record.SubUserRewardType).GetDescription()}",
                    RewardRate = record.RewardRate,
                    Reward = record.Reward,
                    Comment = record.Comment,
                    CreateTime = DateTime.SpecifyKind(record.CreateTime, DateTimeKind.Utc),
                };
                resultData.Items.Add(recordResult);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 查询转账到链上订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<DappUserTransferToChainOrderResult>> QueryTransferToChainOrders([FromQuery] QueryByPagingRequest request)
        {

            var resultData = PaginatedList<DappUserTransferToChainOrderResult>.Empty();
            if (DappUser is null)
            {
                return WrappedResult.Ok(resultData);
            }

            var orders = _dbContext.UserChainTransactions
                .AsNoTracking()
                .OrderByDescending(o => o.CreateTime)
                .Where(o => o.Uid == DappUser.Uid && o.TransactionType == (int)UserChainTransactionType.ToChain)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaultDappQueryPageSize);
            resultData = new PaginatedList<DappUserTransferToChainOrderResult>(orders.Pagination);
            if (orders.Items.Count < 1)
            {
                return WrappedResult.Ok(resultData);
            }
            foreach (var order in orders.Items)
            {
                DappUserTransferToChainOrderResult orderResult = new()
                {
                    Id = order.Id,
                    TransactionId = order.TransactionId,
                    TransactionStatus = order.TransactionStatus,
                    TransactionStatusName = ((ChainTransactionStatus)order.TransactionStatus).GetDescription(),
                    ClientSentToken = order.ClientSentTokenValue,
                    ServerCheckedToken = order.ServerCheckedTokenValue ?? 0,
                    CreateTime = DateTime.SpecifyKind(order.CreateTime, DateTimeKind.Utc),
                };
                resultData.Items.Add(orderResult);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 查询转账到钱包订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<PaginatedList<DappUserTransferToWallerOrderResult>> QueryTransferToWalletOrders([FromQuery] QueryByPagingRequest request)
        {

            var resultData = PaginatedList<DappUserTransferToWallerOrderResult>.Empty();
            if (DappUser is null)
            {
                return WrappedResult.Ok(resultData);
            }

            var orders = _dbContext.UserAssetsToWalletOrders
                .AsNoTracking()
                .OrderByDescending(o => o.CreateTime)
                .Where(o => o.Uid == DappUser.Uid)
                .ToPaginatedList(request.PageIndex, LocalConfig.DefaultDappQueryPageSize);
            resultData = new PaginatedList<DappUserTransferToWallerOrderResult>(orders.Pagination);
            if (orders.Items.Count < 1)
            {
                return WrappedResult.Ok(resultData);
            }
            foreach (var order in orders.Items)
            {
                DappUserTransferToWallerOrderResult ordersResult = new()
                {
                    Id = order.Id,
                    OrderStatus = order.OrderStatus,
                    OrderStatusName = $"{((UserToWalletOrderStatus)order.OrderStatus).GetDescription()}",
                    RequestAmount = order.RequestAmount,
                    RealAmount = order.RealAmount,
                    ServiceFee = order.ServiceFee,
                    CreateTime = DateTime.SpecifyKind(order.CreateTime, DateTimeKind.Utc),
                };
                resultData.Items.Add(ordersResult);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 设置主要代币
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public WrappedResult SetPrimaryToken(DappUserTransactionRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            User? user = _dbContext.Users
                .Include(o => o.UserAsset)
                .Include(o => o.UserChainTransactions.Where(o => o.TransactionType == (int)UserChainTransactionType.Approve))
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var tokenConfig = _tempCaching.ChainTokenConfigs.FirstOrDefault(o => o.TokenId == request.TokenId && o.ChainId == user.ChainId);
            if (tokenConfig is null)
            {
                return WrappedResult.Failed("Unable to obtain token information");
            }

            // 验证是否有处理中的授权交易
            if (user.UserChainTransactions.Any(o => o.TransactionStatus == (int)ChainTransactionStatus.None || o.TransactionStatus == (int)ChainTransactionStatus.Pending))
            {
                return WrappedResult.Failed("You have set up your primary token, please wait for the blockchain network to sync");
            }

            // 验证是否已经是授权状态
            if (user.UserAsset is not null && user.UserAsset.Approved)
            {
                return WrappedResult.Failed("You have set up your primary token, please wait for the blockchain network to sync");
            }

            // 验证授权代币是否和以前相同（防止客户取消过授权补充授权）
            if (user.UserAsset is not null && user.UserAsset.PrimaryTokenId != request.TokenId)
            {
                return WrappedResult.Failed("You have set up your primary token, only the primary token you set up previously can be selected");
            }

            if (_dbContext.UserChainTransactions.Any(o => o.TransactionId.ToLower() == request.TransactionId.ToLower()))
            {
                return WrappedResult.Failed("requested data is illegal");
            }

            if (!BigInteger.TryParse(request.ValueText, out BigInteger bigIntegerValue))
            {
                return WrappedResult.Failed("requested data is illegal");
            }

            var decimalValue = Web3Provider.BigIntegerSafeToDecimal(bigIntegerValue, tokenConfig.Decimals);
            UserChainTransaction transaction = new()
            {
                Uid = user.Uid,
                TransactionType = (int)UserChainTransactionType.Approve,
                TokenId = request.TokenId,
                TransactionId = request.TransactionId,
                ClientSentTokenValue = decimalValue,
                TransactionStatus = (int)ChainTransactionStatus.None
            };
            _dbContext.UserChainTransactions.Add(transaction);
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("Server is busy, please try again later");
            }

            return WrappedResult.Ok();
        }

        /// <summary>
        /// 激活 AI 合约交易
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public WrappedResult ActivateAiTrading(DappUserActivateAITradingRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            UserAsset? userAsset = _dbContext.UserAssets
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (userAsset is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            if (userAsset.AiTradingActivated)
            {
                return WrappedResult.Failed("You have activated the AI contract trading function.");
            }

            var activationCode = _dbContext.ManagerAiTradingActivationCodes.FirstOrDefault(o => o.ActivationCodeGuid.ToString() == request.ActivationCode);
            if (activationCode is null)
            {
                return WrappedResult.Failed("The activation code you provided does not exist.");
            }

            if (activationCode.UserUid is not null)
            {
                return WrappedResult.Failed("The activation code you provided has been used.");
            }

            var now = DateTime.UtcNow;
            if (activationCode.ExpirationTime is not null && activationCode.ExpirationTime < DateTime.UtcNow)
            {
                return WrappedResult.Failed("The activation code you provided has expired.");
            }

            userAsset.AiTradingActivated = true;
            userAsset.AiTradingRemainingTimes = 20;
            _dbContext.UserAssets.Update(userAsset);

            activationCode.UserUid = userAsset.Uid;
            activationCode.UseTime = now;

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("Server is busy, please try again later");
            }

            return WrappedResult.Ok();
        }

        /// <summary>
        /// 转账到链上
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public WrappedResult TransferToChain(DappUserTransactionRequest request)
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            User? user = _dbContext.Users
                .Include(o => o.UserAsset)
                .Include(o => o.UserChainTransactions.Where(o => o.TransactionStatus == (int)ChainTransactionStatus.None || o.TransactionStatus == (int)ChainTransactionStatus.Pending))
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (user is null || user.UserAsset is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            var tokenConfig = _tempCaching.ChainTokenConfigs.FirstOrDefault(o => o.TokenId == request.TokenId && o.ChainId == user.ChainId);
            var networkConfig = _tempCaching.ChainNetworkConfigs.FirstOrDefault(o => o.ChainId == user.ChainId);
            if (tokenConfig is null || networkConfig is null)
            {
                return WrappedResult.Failed("Unable to obtain token config information");
            }

            if (user.UserChainTransactions.Any(o => o.TransactionType == (int)UserChainTransactionType.ToChain))
            {
                return WrappedResult.Failed("You have a pending on-chain transaction, please wait for the blockchain network to sync");
            }

            if (_dbContext.UserChainTransactions.Any(o => o.TransactionId.ToLower() == request.TransactionId.ToLower()))
            {
                return WrappedResult.Failed("requested data is illegal");
            }

            if (!BigInteger.TryParse(request.ValueText, out BigInteger bigIntegerValue))
            {
                return WrappedResult.Failed("requested data is illegal");
            }

            var decimalValue = Web3Provider.BigIntegerSafeToDecimal(bigIntegerValue, tokenConfig.Decimals);
            if (decimalValue < networkConfig.MinAssetsToChainLimit)
            {
                return WrappedResult.Failed($"The minimum transfer amount is ${networkConfig.MinAssetsToChainLimit.ToString("0.00")}");
            }
            UserChainTransaction transaction = new()
            {
                Uid = user.Uid,
                TransactionType = (int)UserChainTransactionType.ToChain,
                TokenId = request.TokenId,
                TransactionId = request.TransactionId,
                ClientSentTokenValue = decimalValue,
                TransactionStatus = (int)ChainTransactionStatus.None
            };
            _dbContext.UserChainTransactions.Add(transaction);
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("Server is busy, please try again later");
            }

            return WrappedResult.Ok();
        }

        /// <summary>
        /// 转账到钱包
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public WrappedResult TransferToWallet(DappUserAmountRequest request)
        {
            if (request.Amount < Web3Provider.MinTokenDecimalValue || request.Amount > Web3Provider.MaxTokenDecimalValue)
            {
                return WrappedResult.Failed("requested data is illegal");
            }
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            User? user = _dbContext.Users
                .Include(o => o.UserAsset)
                .AsNoTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (user is null || user.UserAsset is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            if (user.Anomaly || !user.UserAsset.Approved)
            {
                return WrappedResult.Failed("Your account is at risk and this operation cannot be performed");
            }

            // 是否有进行中的提款
            if (_dbContext.UserAssetsToWalletOrders.Any(o => o.Uid == DappUser.Uid && (o.OrderStatus == (int)UserToWalletOrderStatus.Waiting || o.OrderStatus == (int)UserToWalletOrderStatus.Pending)))
            {
                return WrappedResult.Failed("You have a transaction in progress, please wait for the transaction to complete and try again.");
            }

            // 验证余额
            if (user.UserAsset.OnChainAssets < request.Amount)
            {
                return WrappedResult.Failed("Your on-chain assets are insufficient");
            }

            // 计算服务费
            var networkConfig = _tempCaching.ChainNetworkConfigs.FirstOrDefault(o => o.ChainId == user.ChainId);
            if (networkConfig is null)
            {
                return WrappedResult.Failed("Unable to obtain token config information");
            }
            var serviceFee = networkConfig.AssetsToWalletServiceFeeBase;
            var calculatedServiceFee = Math.Round(request.Amount * networkConfig.AssetsToWalletServiceFeeRate, 4);
            if (serviceFee < calculatedServiceFee)
            {
                serviceFee = calculatedServiceFee;
            }

            // 添加订单
            UserAssetsToWalletOrder order = new()
            {
                Uid = user.Uid,
                TokenId = user.UserAsset.PrimaryTokenId,
                RequestAmount = request.Amount,
                ServiceFee = serviceFee,
                RealAmount = request.Amount - serviceFee,
                OrderStatus = (int)UserToWalletOrderStatus.Waiting
            };
            _dbContext.UserAssetsToWalletOrders.Add(order);

            // 添加账变
            UserOnChainAssetsChange toWalletChange = new()
            {
                Uid = user.Uid,
                ChangeType = (int)UserOnChainAssetsChangeType.ToWallet,
                Change = request.Amount,
                Before = user.UserAsset.OnChainAssets,
                After = user.UserAsset.OnChainAssets - request.Amount,
                Comment = "Transfer assets from chain to wallet.",
            };
            _dbContext.UserOnChainAssetsChanges.Add(toWalletChange);

            // 操作资产
            user.UserAsset.OnChainAssets -= request.Amount;
            user.UserAsset.LockingAssets += request.Amount;
            _dbContext.Update(user.UserAsset);

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("Server is busy, please try again later");
            }

            return WrappedResult.Ok();
        }

        /// <summary>
        /// 创建 AI 合约交易
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public WrappedResult CreateAiTradingOrder()
        {
            if (DappUser is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }
            User? user = _dbContext.Users
                .Include(o => o.UserAsset)
                .Include(o => o.UserAiTradingOrders.Where(o => o.Status == (int)UserAiTradingOrderStatus.Trading))
                .AsTracking()
                .FirstOrDefault(o => o.Uid == DappUser.Uid);
            if (user is null)
            {
                return WrappedResult.Failed("Unable to obtain user information");
            }

            // 账户异常
            if (user.Anomaly)
            {
                return WrappedResult.Failed("Your account is at risk and cannot interact with the smart contract");
            }

            // 未激活账户
            if (user.UserLevel < 1 || user.UserAsset is null)
            {
                return WrappedResult.Failed("You are not a delegator and do not have permission to interact with the smart contract");
            }

            // 没有权限
            if (!user.UserAsset.AiTradingActivated)
            {
                return WrappedResult.Failed("You do not have permission to use AI contract trading, please activate it first");
            }

            // 有交易中的订单
            if (user.UserAiTradingOrders.Any())
            {
                return WrappedResult.Failed("You have an order that is currently being traded, please wait for the transaction to be completed and try again");
            }

            // 交易次数不足
            if (user.UserAsset.AiTradingRemainingTimes < 1)
            {
                return WrappedResult.Failed("You do not have enough available transactions remaining");
            }

            // 验证余额
            var levelConfig = _tempCaching.UserLevelConfigs.First(o => o.UserLevel == user.UserLevel);
            var validassets = user.UserAsset.OnChainAssets + user.UserAsset.PrimaryTokenWalletBalance;
            var availableTransactionAmount = (validassets * levelConfig.AvailableAiTradingAssetsRate).FixedToZero();
            if (user.UserAsset.OnChainAssets < availableTransactionAmount)
            {
                return WrappedResult.Failed("You don't have sufficient on-chain assets to create a transaction");
            }

            // 创建订单
#if (DEBUG)
            var tradingMinutes = 1;
#else
            var tradingMinutes = Random.Shared.Next(_tempCaching.GlobalConfig.MinAiTradingMinutes, _tempCaching.GlobalConfig.MaxAiTradingMinutes + 1);
#endif
            var now = DateTime.UtcNow;
            var orderEndTime = now.AddMinutes(tradingMinutes);
            orderEndTime = new DateTime(orderEndTime.Year, orderEndTime.Month, orderEndTime.Day, orderEndTime.Hour, orderEndTime.Minute, 59);
            UserAiTradingOrder aiTradingOrder = new()
            {
                Uid = user.Uid,
                Amount = availableTransactionAmount,
                Status = (int)UserAiTradingOrderStatus.Trading,
                OrderEndTime = orderEndTime,
                CreateTime = now,
            };
            _dbContext.UserAiTradingOrders.Add(aiTradingOrder);

            // 账变
            UserOnChainAssetsChange aiTradingChange = new()
            {
                Uid = user.Uid,
                ChangeType = (int)UserOnChainAssetsChangeType.AiContractTrading,
                Change = availableTransactionAmount,
                Before = user.UserAsset.OnChainAssets,
                After = user.UserAsset.OnChainAssets - availableTransactionAmount,
                Comment = "Create AI contract trading order."
            };
            _dbContext.UserOnChainAssetsChanges.Add(aiTradingChange);

            // 更新资产
            user.UserAsset.OnChainAssets -= availableTransactionAmount;
            user.UserAsset.LockingAssets += availableTransactionAmount;
            user.UserAsset.AiTradingRemainingTimes--;
            _dbContext.UserAssets.Update(user.UserAsset);

            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("Server is busy, please try again later");
            }

            // 响应数据
            return WrappedResult.Ok();
        }
    }
}
