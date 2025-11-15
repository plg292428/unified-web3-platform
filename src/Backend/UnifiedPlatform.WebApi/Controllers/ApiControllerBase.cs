using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Mvc;
using UnifiedPlatform.Shared;
using UnifiedPlatform.WebApi.Businesses;

namespace UnifiedPlatform.WebApi.Controllers
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// 上下文DappUser
        /// </summary>
        public HttpContextDappUser? DappUser
        {
            get
            {
                if (!User.TryGetInt(JwtClaimKeyName.Uid, out int? uid)) return null;
                if (!User.TryGetString(JwtClaimKeyName.AccesTokenGuid, out string? accesTokenGuid)) return null;
                if (!User.TryGetString(JwtClaimKeyName.WalletAddress, out string? walletAddress)) return null;
                if (!User.TryGetInt(JwtClaimKeyName.ChainId, out int? chainId)) return null;
                if (!Enum.IsDefined((ChainNetwork)chainId.Value)) return null;
                if (!User.TryGetInt(JwtClaimKeyName.RequestUserType, out int? requestUserType)) return null;
                if (!Enum.IsDefined((WebApiRequestUserType)requestUserType.Value)) return null;

                return new HttpContextDappUser()
                {
                    Uid = uid.Value,
                    AccesTokenGuid = accesTokenGuid,
                    WalletAddress = walletAddress,
                    ChainId = chainId.Value,
                    ChainNetwork = (ChainNetwork)chainId.Value,
                    RequestUserType = (WebApiRequestUserType)requestUserType.Value,
                };
            }
        }

        /// <summary>
        /// 上下文DappUser
        /// </summary>
        public HttpContextManager? Manager
        {
            get
            {
                if (!User.TryGetInt(JwtClaimKeyName.Uid, out int? uid)) return null;
                if (!User.TryGetString(JwtClaimKeyName.Username, out string? username)) return null;
                if (!User.TryGetString(JwtClaimKeyName.AccesTokenGuid, out string? accesTokenGuid)) return null;
                if (!User.TryGetInt(JwtClaimKeyName.RequestUserType, out int? managerType)) return null;
                if (!Enum.IsDefined((ManagerType)managerType.Value)) return null;

                return new HttpContextManager()
                {
                    Uid = uid.Value,
                    Username = username,
                    AccesTokenGuid = accesTokenGuid,
                    ManagerType = (ManagerType)managerType.Value,
                };
            }
        }
    }
}

