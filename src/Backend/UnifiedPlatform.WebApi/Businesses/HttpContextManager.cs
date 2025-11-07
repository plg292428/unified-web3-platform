using SmallTarget.Shared;

namespace SmallTarget.WebApi.Businesses
{
    /// <summary>
    /// Http请求上下文 Manager
    /// </summary>
    public class HttpContextManager
    {
        public int Uid { get; init; }

        public required string Username { get; init; }

        public required string AccesTokenGuid { get; init; }

        public ManagerType ManagerType { get; init; }
    }
}
