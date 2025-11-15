using UnifiedPlatform.Shared;

namespace UnifiedPlatform.WebApi
{
    /// <summary>
    /// Http请求上下文 Dapp 用户
    /// </summary>
    public class HttpContextDappUser
    {
        public int Uid { get; init; }

        public required string AccesTokenGuid { get; init; }

        public required string WalletAddress { get; init; }

        public int ChainId { get; init; }

        public ChainNetwork ChainNetwork { get; init; }

        public WebApiRequestUserType RequestUserType { get; init; }
    }
}

