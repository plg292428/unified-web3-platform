using Grpc.Core;
using Nblockchain.Signer;

namespace Nblockchain.Tron
{
    /// <summary>
    /// Tron Web 选项
    /// </summary>
    public class TronWebOptions
    {
        /// <summary>
        /// 网络
        /// </summary>
        public TronNetwork Network { get; set; } = TronNetwork.MainNet;

        /// <summary>
        /// gRPC 地址
        /// </summary>
        public string RpcUrl { get; set; } = "grpc.trongrid.io:50051";

        /// <summary>
        /// API KEY （非trongrid网络下无需设置）
        /// </summary>
        public string? ApiKey { get; set; } = null;

        /// <summary>
        /// 证书
        /// </summary>
        public ChannelCredentials Credentials { get; set; } = ChannelCredentials.Insecure;

        /// <summary>
        /// gRPC 请求头 
        /// </summary>
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        public Metadata RpcHeaders => new() { { "TRON-PRO-API-KEY", ApiKey } };
#pragma warning restore CS8604 // 引用类型参数可能为 null。
    }
}
