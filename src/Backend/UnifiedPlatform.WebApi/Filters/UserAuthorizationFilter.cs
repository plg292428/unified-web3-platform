using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmallTarget.DbService.Entities;
using SmallTarget.Shared;
using System.Security.Claims;

namespace SmallTarget.WebApi.Filters
{
    /// <summary>
    /// 用户授权过滤器
    /// </summary>
    public class UserAuthorizationFilter : IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsPrincipal contextUser = context.HttpContext.User;

            // 当有AllowAnonymous特性时跳过验证
            if (HasAllowAnonymous(context))
            {
                return;
            }

            // 是否经过认证
            bool? isAuthenticated = contextUser.Identity?.IsAuthenticated;
            if (!isAuthenticated.HasValue || !isAuthenticated.Value)
            {
                return;
            }

            var result = WrappedResult.Failed();

            // 验证Claims信息
            if(!contextUser.TryGetInt(JwtClaimKeyName.Uid, out int? uid) || !Guid.TryParse(contextUser.GetString(JwtClaimKeyName.AccesTokenGuid), out Guid accesTokenGuid))
            {
                context.Result = new JsonResult(result);
                return;
            }

            // 服务容器
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            if (!memoryCache.TryGetValue(accesTokenGuid, out DateTime cacheEntry))
            {
                // 根据请求用户类型通过不同数据库判断
                StDbContext dbContext = serviceProvider.GetRequiredService<StDbContext>();
                if (!contextUser.TryGetInt(JwtClaimKeyName.RequestUserType, out int? requestUserTypeValue))
                {
                    result.ErrorMessage = "Please sign in first";
                    context.Result = new JsonResult(result);
                    return;
                }
                WebApiRequestUserType requestUserType = (WebApiRequestUserType)requestUserTypeValue;
                if (requestUserType == WebApiRequestUserType.DappUser)
                {
                    User? user = dbContext.Users
                        .AsNoTracking()
                        .FirstOrDefault(o => !o.Deleted && o.Uid == uid);
                    if (user is null || user.AccesTokenGuid is null)
                    {
                        result.ErrorMessage = "Please sign in first";
                        context.Result = new JsonResult(result);
                        return;
                    }
                    if (user.AccesTokenGuid != accesTokenGuid)
                    {
                        result.ErrorMessage = "Your account is already logged in elsewhere";
                        context.Result = new JsonResult(result);
                        return;
                    }
                    if (user.Blocked)
                    {
                        result.ErrorMessage = "Your account has been disabled";
                        context.Result = new JsonResult(result);
                        return;
                    }
                    if (user.Deleted)
                    {
                        result.ErrorMessage = "Unable to obtain user information";
                        context.Result = new JsonResult(result);
                        return;
                    }
                }
                else
                {
                    Manager? manager = dbContext.Managers
                        .AsNoTracking()
                        .FirstOrDefault(o => !o.Deleted && o.Uid == uid);
                    if (manager is null || manager.AccesTokenGuid is null)
                    {
                        result.ErrorMessage = "请先登录后再进行访问";
                        context.Result = new JsonResult(result);
                        return;
                    }
                    if (manager.AccesTokenGuid != accesTokenGuid)
                    {
                        result.ErrorMessage = "您的账号已再别处登录";
                        context.Result = new JsonResult(result);
                        return;
                    }
                    if (manager.Blocked)
                    {
                        result.ErrorMessage = "您的账号已被禁用";
                        context.Result = new JsonResult(result);
                        return;
                    }
                    if (manager.Deleted)
                    {
                        result.ErrorMessage = "无法获取用户信息";
                        context.Result = new JsonResult(result);
                        return;
                    }
                }
                memoryCache.Set(accesTokenGuid, DateTime.Now, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30)));
            }
        }

        /// <summary>
        /// 判断是否用有AllowAnonymous特性（摘自微软源码）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool HasAllowAnonymous(AuthorizationFilterContext context)
        {
            var filters = context.Filters;
            for (var i = 0; i < filters.Count; i++)
            {
                if (filters[i] is IAllowAnonymousFilter)
                {
                    return true;
                }
            }

            // When doing endpoint routing, MVC does not add AllowAnonymousFilters for AllowAnonymousAttributes that
            // were discovered on controllers and actions. To maintain compat with 2.x,
            // we'll check for the presence of IAllowAnonymous in endpoint metadata.
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            {
                return true;
            }

            return false;
        }
    }
}
