using HFastKit.AspNetCore.Http;
using HFastKit.AspNetCore.Services.Captcha;
using HFastKit.AspNetCore.Services.Jwt;
using HFastKit.AspNetCore.Shared;
using HFastKit.AspNetCore.Shared.Text;
using HFastKit.Extensions;
using IP2Region.Net.Abstractions;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels;
using UnifiedPlatform.Shared.ActionModels.Result;
using UnifiedPlatform.WebApi.Services;
using System.Linq.Expressions;
using System.Security.Claims;

namespace UnifiedPlatform.WebApi.Controllers
{
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.Manager)]
    [ApiController]
    public class ManagementAuthenticationController : ApiControllerBase
    {
        private readonly ITempCaching _tempCaching;
        private readonly ICaptchaFactory _captchaFactory;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly ISearcher _searcher;
        private readonly StDbContext _dbContext;

        public ManagementAuthenticationController(ITempCaching tempCaching, ICaptchaFactory captchaFactory, IOptions<JwtOptions> jwtOptions, ISearcher searcher, StDbContext dbContext)
        {
            _tempCaching = tempCaching;
            _jwtOptions = jwtOptions;
            _captchaFactory = captchaFactory;
            _searcher = searcher;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取验证码信息
        /// </summary>
        /// <returns>验证码信息</returns>
        [HttpGet, AllowAnonymous]
        public WrappedResult<CaptchaResult> GetCaptcha()
        {
            CaptchaResult resultData = new();
            Captcha captcha = _captchaFactory.Create();
            resultData.Token = captcha.Token;
            resultData.ImageUrl = $"/ManagementAuthentication/GetCaptchaImage?token={captcha.Token}";
            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <param name="token">验证码令牌</param>
        /// <returns>验证码图片</returns>
        [HttpGet, AllowAnonymous]
        public IActionResult GetCaptchaImage([FromQuery] string token)
        {
            Captcha? captcha = _captchaFactory.Caching.FirstOrDefault(o => o.Value.Token == token).Value;
            if (captcha is null)
            {
                return NotFound();
            }
            return File(captcha.GenerateImageAsPng(100, 56, true, true), @"image/png");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request">请求内容</param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public WrappedResult<ManagementSigninedManagerResult> Login(ManagementLoginRequest request)
        {
            // 验证验证码
            CaptchaVerifyResult captchaVerifyResult = _captchaFactory.Verify(request.Captcha, request.CaptchaToken);
            if (!captchaVerifyResult.IsVerified)
            {
                return WrappedResult.Failed($"登录失败，{captchaVerifyResult.ErrorMessage}");
            }

            // 验证用户名和密码
            Manager? manager = _dbContext.Managers
                .Include(o => o.AttributionAgentU)
                .FirstOrDefault(o => !o.Deleted && o.Username == request.Username);
            if (manager is null)
            {
                return WrappedResult.Failed($"登录失败，该用户不存在");
            }
            ManagerType managerTypeEnum = (ManagerType)manager.ManagerType;
            var encryptedPassword = request.Password.DesEncrypt();
            if (manager.Password != encryptedPassword)
            {
                return WrappedResult.Failed($"登录失败，密码不正确");
            }
            if (manager.Blocked)
            {
                return WrappedResult.Failed($"登录失败，该用户已被禁用");
            }

            // 验证归属信息
            if (managerTypeEnum is ManagerType.Salesman && (manager.AttributionGroupLeaderUid is null || manager.AttributionAgentUid is null))
            {
                return WrappedResult.Failed($"登录失败，无效的用户");
            }
            else if (managerTypeEnum is ManagerType.GroupLeader && manager.AttributionAgentUid is null)
            {
                return WrappedResult.Failed($"登录失败，无效的用户");
            }
            string clientIp = HttpContext.GetRemoteIp();
            string? clientIpRegion = _searcher.SearchAndFix(clientIp);

            // 修改信息
            Guid guid = Guid.NewGuid();
            manager.AccesTokenGuid = guid;
            manager.LastSignInClientIp = clientIp;
            manager.LastSignInClientIpRegion = clientIpRegion;
            if (manager.SignUpClientIp is null)
            {
                manager.SignUpClientIp = clientIp;
                manager.SignUpClientIpRegion = clientIpRegion;
            }

            // 添加登录日志
            ManagerLoginLog loginLog = new()
            {
                Uid = manager.Uid,
                ClientIp = clientIp,
                ClientIpRegion = clientIpRegion,
                CreateTime = DateTime.UtcNow
            };
            _dbContext.ManagerLoginLogs.Add(loginLog);
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                return WrappedResult.Failed("服务器繁忙，请稍后重试");
            }

            // 生成令牌，获取未处理订单
            List<Claim> claims = new()
            {
                new(JwtClaimKeyName.Uid, manager.Uid.ToString()),
                new(JwtClaimKeyName.Username, manager.Username),
                new(JwtClaimKeyName.AccesTokenGuid, guid.ToString()),
                new(JwtClaimKeyName.RequestUserType, manager.ManagerType.ToString()),
            };
            Expression<Func<UserAssetsToWalletOrder, bool>> predicate = o => !o.UidNavigation.Deleted && !o.UidNavigation.VirtualUser && !o.UidNavigation.Anomaly && o.OrderStatus == (int)UserToWalletOrderStatus.Waiting;
            var balanceAssets = 0.0m;
            switch (managerTypeEnum)
            {
                case ManagerType.Salesman:
                    predicate = predicate.And(o => o.UidNavigation.AttributionSalesmanUid == manager.Uid);
                    claims.Add(new(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Salesman));
                    balanceAssets = manager.AttributionAgentU?.BalanceAssets ?? 0.0m;
                    break;
                case ManagerType.GroupLeader:
                    predicate = predicate.And(o => o.UidNavigation.AttributionGroupLeaderUid == manager.Uid);
                    claims.Add(new(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.GroupLeader));
                    balanceAssets = manager.AttributionAgentU?.BalanceAssets ?? 0.0m;
                    break;
                case ManagerType.Agent:
                    predicate = predicate.And(o => o.UidNavigation.AttributionAgentUid == manager.Uid);
                    claims.Add(new(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Agent));
                    balanceAssets = manager.BalanceAssets;
                    break;
                case ManagerType.Administrator:
                    claims.Add(new(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Administrator));

                    // 普通管理员过滤
                    predicate = predicate.And(o => (o.UidNavigation.AttributionAgentU == null || !o.UidNavigation.AttributionAgentU.OnlyDeveloperVisible));

                    break;
                case ManagerType.Developer:
                    claims.Add(new(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Developer));
                    break;
                default:
                    break;
            }

            int waitingConfirmOrdersCount = _dbContext.UserAssetsToWalletOrders.Count(predicate);
            string accessToken = JwtHelper.CreateToken(_jwtOptions.Value.Audience, _jwtOptions.Value.Issuer, _jwtOptions.Value.SecurityKey, claims.ToArray());

            // 响应结果
            var resultData = new ManagementSigninedManagerResult()
            {
                Uid = manager.Uid,
                Username = manager.Username,
                ManagerType = manager.ManagerType,
                ManagerTypeName = _tempCaching.ManagerTypeConfigs.FirstOrDefault(o => o.ManagerType == manager.ManagerType)?.ManagerTypeDescription ?? "None",
                WaitingConfirmOrdersCount = waitingConfirmOrdersCount,
                BalanceAssets = balanceAssets,
                AccessToken = accessToken,
            };

            // 邀请链接
            resultData.InvitationLinks = new();
            foreach (var link in _tempCaching.DappUrls)
            {
                var invitationLink = link.AbsoluteUri;
                if (manager.ManagerType < (int)ManagerType.Administrator)
                {
                    string invitationLinkToken = InvitationLinkToken.Create(manager.Uid, InviterType.Employee).Token;
                    invitationLink += $"?inviter={invitationLinkToken}";
                }
                resultData.InvitationLinks.Add(invitationLink);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 获取员工
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public WrappedResult<ManagementSigninedManagerResult> GetManager()
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("获取失败，无法获取当前用户信息");
            }

            // 获取用户
            Manager? manager = _dbContext.Managers
                .AsNoTracking()
                .Include(o => o.AttributionAgentU)
                .FirstOrDefault(o => !o.Deleted && o.Uid == Manager.Uid);
            if (manager is null)
            {
                return WrappedResult.Failed($"获取失败，无法获取当前用户信息");
            }

            // 验证归属信息
            if (Manager.ManagerType is ManagerType.Salesman && (manager.AttributionGroupLeaderUid is null || manager.AttributionAgentUid is null))
            {
                return WrappedResult.Failed($"获取失败，无效的用户");
            }
            else if (Manager.ManagerType is ManagerType.GroupLeader && manager.AttributionAgentUid is null)
            {
                return WrappedResult.Failed($"获取失败，无效的用户");
            }

            // 获取未处理订单
            Expression<Func<UserAssetsToWalletOrder, bool>> predicate = o => !o.UidNavigation.Deleted && !o.UidNavigation.VirtualUser && !o.UidNavigation.Anomaly && o.OrderStatus == (int)UserToWalletOrderStatus.Waiting;
            var balanceAssets = 0.0m;
            switch (Manager.ManagerType)
            {
                case ManagerType.Salesman:
                    predicate = predicate.And(o => o.UidNavigation.AttributionSalesmanUid == manager.Uid);
                    balanceAssets = manager.AttributionAgentU?.BalanceAssets ?? 0.0m;
                    break;
                case ManagerType.GroupLeader:
                    predicate = predicate.And(o => o.UidNavigation.AttributionGroupLeaderUid == manager.Uid);
                    balanceAssets = manager.AttributionAgentU?.BalanceAssets ?? 0.0m;
                    break;
                case ManagerType.Agent:
                    predicate = predicate.And(o => o.UidNavigation.AttributionAgentUid == manager.Uid);
                    balanceAssets = manager.BalanceAssets;
                    break;
                case ManagerType.Administrator:

                    // 普通管理员过滤
                    predicate = predicate.And(o => (o.UidNavigation.AttributionAgentU == null || !o.UidNavigation.AttributionAgentU.OnlyDeveloperVisible));

                    break;
                case ManagerType.Developer:
                    break;
                default:
                    break;
            }

            int waitingConfirmOrdersCount = _dbContext.UserAssetsToWalletOrders.Count(predicate);

            // 响应结果
            var resultData = new ManagementSigninedManagerResult()
            {
                Uid = manager.Uid,
                Username = manager.Username,
                ManagerType = manager.ManagerType,
                ManagerTypeName = _tempCaching.ManagerTypeConfigs.FirstOrDefault(o => o.ManagerType == manager.ManagerType)?.ManagerTypeDescription ?? "None",
                WaitingConfirmOrdersCount = waitingConfirmOrdersCount,
                BalanceAssets = balanceAssets,
            };

            // 邀请链接
            resultData.InvitationLinks = new();
            foreach (var link in _tempCaching.DappUrls)
            {
                var invitationLink = link.AbsoluteUri;
                if (manager.ManagerType < (int)ManagerType.Administrator)
                {
                    string invitationLinkToken = InvitationLinkToken.Create(manager.Uid, InviterType.Employee).Token;
                    invitationLink += $"?inviter={invitationLinkToken}";
                }
                resultData.InvitationLinks.Add(invitationLink);
            }

            return WrappedResult.Ok(resultData);
        }

        /// <summary>
        /// 创建员工
        /// </summary>
        /// <returns></returns>
        [HttpPost, Authorize(Policy = AuthorizationPolicyName.GroupLeader)]
        public WrappedResult CreateManager(ManagementCreateManagerRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("创建失败，无法获取当前用户信息");
            }
            ManagerType managerType = Manager.ManagerType;
            if (request.ManagerType == ManagerType.Invalid)
            {
                return WrappedResult.Failed("创建失败，要创建的员工类型错误");
            }
            if (request.ManagerType >= managerType)
            {
                return WrappedResult.Failed("创建失败，你没有权限进行该操作");
            }
            if (_dbContext.Managers.Any(o => o.Username == request.Username))
            {
                return WrappedResult.Failed("创建失败，用户名已存在");
            }

            Manager newManager = new()
            {
                Username = request.Username,
                Password = request.Password.DesEncrypt(),
                ManagerType = (int)request.ManagerType,
                CreateTime = DateTime.UtcNow
            };

            Expression<Func<Manager, bool>> predicate = o => !o.Deleted && !o.Blocked;
            switch (managerType)
            {
                case ManagerType.Developer:         // 开发者创建员工
                case ManagerType.Administrator:     // 管理员创建员工
                    {
                        if (request.ManagerType is ManagerType.Salesman)
                        {
                            if (request.AttributionManagerUid is null)
                            {
                                return WrappedResult.Failed("创建失败，请选择归属组长");
                            }
                            Manager? attributionGroupLeader = _dbContext.Managers
                                .AsNoTracking()
                                .FirstOrDefault(predicate.And(o => o.Uid == request.AttributionManagerUid && o.ManagerType == (int)ManagerType.GroupLeader));
                            if (attributionGroupLeader is null)
                            {
                                return WrappedResult.Failed("创建失败，无法获取归属组长信息");
                            }
                            newManager.AttributionGroupLeaderUid = attributionGroupLeader.Uid;
                            newManager.AttributionAgentUid = attributionGroupLeader.AttributionAgentUid;
                        }
                        else if (request.ManagerType is ManagerType.GroupLeader)
                        {
                            if (request.AttributionManagerUid is null)
                            {
                                return WrappedResult.Failed("创建失败，请选择归属代理");
                            }
                            Manager? attributionAgent = _dbContext.Managers
                                .AsNoTracking()
                                .FirstOrDefault(predicate.And(o => o.Uid == request.AttributionManagerUid && o.ManagerType == (int)ManagerType.Agent));
                            if (attributionAgent is null)
                            {
                                return WrappedResult.Failed("创建失败，无法获取归属代理信息");
                            }
                            newManager.AttributionAgentUid = attributionAgent.Uid;
                        }
                        break;
                    }
                case ManagerType.Agent:            // 代理创建员工
                    {
                        if (request.ManagerType is ManagerType.Salesman)
                        {
                            if (request.AttributionManagerUid is null)
                            {
                                return WrappedResult.Failed("创建失败，请选择归属组长");
                            }
                            Manager? attributionGroupLeader = _dbContext.Managers
                                .AsNoTracking()
                                .FirstOrDefault(predicate.And(o => o.Uid == request.AttributionManagerUid && o.AttributionAgentUid == Manager.Uid && o.ManagerType == (int)ManagerType.GroupLeader));
                            if (attributionGroupLeader is null)
                            {
                                return WrappedResult.Failed("创建失败，无法获取归属组长信息");
                            }
                            newManager.AttributionGroupLeaderUid = attributionGroupLeader.Uid;
                            newManager.AttributionAgentUid = Manager.Uid;
                        }
                        else
                        {
                            newManager.AttributionAgentUid = Manager.Uid;
                        }

                        break;
                    }
                case ManagerType.GroupLeader:      // 组长创建员工
                    {
                        Manager? attributionGroupLeader = _dbContext.Managers.FirstOrDefault(o => o.Uid == Manager.Uid);
                        if (attributionGroupLeader is null)
                        {
                            return WrappedResult.Failed("创建失败，无法获取归属组长信息");
                        }
                        newManager.AttributionGroupLeaderUid = Manager.Uid;
                        newManager.AttributionAgentUid = attributionGroupLeader.AttributionAgentUid;
                        break;
                    }
            }
            _dbContext.Managers.Add(newManager);

            // 写操作日志
            var newManagerTypeName = _tempCaching.ManagerTypeConfigs.FirstOrDefault(o => o.ManagerType == newManager.ManagerType)?.ManagerTypeDescription ?? "None";
            ManagerOperationLog operationLog = new()
            {
                OperatorUid = Manager.Uid,
                OperationType = (int)ManagerOperationType.CreateManager,
                Comment = $"创建新员工[{newManager.Username}]，用户类型[{newManagerTypeName}]",
                CreateTime = DateTime.UtcNow,
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
        /// 修改密码
        /// </summary>
        /// <param name="request">请求内容</param>
        /// <returns></returns>
        [HttpPost]
        public WrappedResult ChangePassword(ManagementChangePasswordRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("修改失败，无法获取当前用户信息");
            }

            // 验证用户
            Manager? manager = _dbContext.Managers.AsNoTracking().FirstOrDefault(o => !o.Deleted && o.Uid == Manager.Uid);
            if (manager is null)
            {
                return WrappedResult.Failed($"修改失败，无法获取当前用户信息");
            }

            bool changeSelf = string.IsNullOrWhiteSpace(request.Username);
            if (!changeSelf && (manager.ManagerType < (int)ManagerType.GroupLeader))
            {
                return WrappedResult.Failed($"修改失败，你没有权限进行此操作");
            }

            var encryptedNewPassword = request.NewPassword.DesEncrypt();

            if (changeSelf)
            {
                if (string.IsNullOrWhiteSpace(request.OldPassword) || !FormatValidate.IsPassword(request.OldPassword, 6, 30))
                {
                    return WrappedResult.Failed($"修改失败，旧密码格式错误");
                }
                var encryptedOldPassword = request.OldPassword.DesEncrypt();
                if (manager.Password != encryptedOldPassword)
                {
                    return WrappedResult.Failed($"修改失败，旧密码不正确");
                }
                if (encryptedOldPassword == encryptedNewPassword)
                {
                    return WrappedResult.Failed($"修改失败，新密码不能与旧密码相同");
                }

                // 修改数据
                manager.Password = encryptedNewPassword;
                manager.AccesTokenGuid = Guid.NewGuid();
                _dbContext.Managers.Update(manager);
                ManagerOperationLog operationLog = new()
                {
                    OperatorUid = manager.Uid,
                    OperationType = (int)ManagerOperationType.ChangeManagerSelfPassword,
                    Comment = $"修改自身管理端登录密码"
                };
                _dbContext.ManagerOperationLogs.Add(operationLog);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Username) || !FormatValidate.IsUsername(request.Username, 4, 30))
                {
                    return WrappedResult.Failed($"修改失败，用户名格式错误");
                }

                Expression<Func<Manager, bool>> predicate = o => !o.Deleted && o.Username == request.Username;
                switch (Manager.ManagerType)
                {
                    case ManagerType.Salesman:
                        return WrappedResult.Failed($"修改失败，你没有权限进行该操作");
                    case ManagerType.GroupLeader:
                        predicate = predicate.And(o => o.AttributionGroupLeaderUid == manager.Uid);
                        break;
                    case ManagerType.Agent:
                        predicate = predicate.And(o => o.AttributionAgentUid == manager.Uid);
                        break;
                    case ManagerType.Administrator:

                        // 普通管理员过滤
                        predicate = predicate.And(o => !o.OnlyDeveloperVisible && (o.AttributionAgentU == null || !o.AttributionAgentU.OnlyDeveloperVisible));

                        break;
                    case ManagerType.Developer:
                        break;
                    default:
                        break;
                }

                Manager? target = _dbContext.Managers.FirstOrDefault(o => !o.Deleted && o.Username == request.Username);
                if (target is null)
                {
                    return WrappedResult.Failed($"修改失败，该用户不存在");
                }

                // 修改数据
                target.Password = encryptedNewPassword;
                target.AccesTokenGuid = Guid.NewGuid();
                ManagerOperationLog operationLog = new()
                {
                    OperatorUid = manager.Uid,
                    OperationType = (int)ManagerOperationType.ChangeManagerPassword,
                    TargetManagerUid = target.Uid,
                    Comment = $"修改用户[{target.Uid}]管理端登录密码"
                };
                _dbContext.ManagerOperationLogs.Add(operationLog);
            }

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

