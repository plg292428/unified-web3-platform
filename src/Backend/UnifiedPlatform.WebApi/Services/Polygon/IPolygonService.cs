using System.Numerics;
using System.Threading.Tasks;

namespace UnifiedPlatform.WebApi.Services.Polygon
{
    /// <summary>
    /// Polygon 服务接口
    /// </summary>
    public interface IPolygonService
    {
        /// <summary>
        /// 获取 MATIC 余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <returns>MATIC 余额</returns>
        Task<decimal> GetMaticBalanceAsync(string address);

        /// <summary>
        /// 获取 ERC20 代币余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">ERC20 合约地址</param>
        /// <returns>代币余额</returns>
        Task<decimal> GetErc20BalanceAsync(string address, string contractAddress);

        /// <summary>
        /// 转账 MATIC
        /// </summary>
        /// <param name="fromPrivateKey">发送方私钥</param>
        /// <param name="toAddress">接收方地址</param>
        /// <param name="amount">转账金额（MATIC单位）</param>
        /// <returns>交易ID</returns>
        Task<string> TransferMaticAsync(string fromPrivateKey, string toAddress, decimal amount);

        /// <summary>
        /// 转账 ERC20 代币
        /// </summary>
        /// <param name="fromPrivateKey">发送方私钥</param>
        /// <param name="toAddress">接收方地址</param>
        /// <param name="contractAddress">ERC20 合约地址</param>
        /// <param name="amount">转账金额</param>
        /// <returns>交易ID</returns>
        Task<string> TransferErc20Async(string fromPrivateKey, string toAddress, string contractAddress, decimal amount);

        /// <summary>
        /// 查询交易状态
        /// </summary>
        /// <param name="transactionHash">交易哈希</param>
        /// <returns>交易状态</returns>
        Task<PolygonTransactionStatus> GetTransactionStatusAsync(string transactionHash);

        /// <summary>
        /// 获取 Gas 价格估算
        /// </summary>
        /// <returns>Gas 价格（Gwei）</returns>
        Task<decimal> GetGasPriceAsync();

        /// <summary>
        /// 估算交易 Gas 费用
        /// </summary>
        /// <param name="fromAddress">发送方地址</param>
        /// <param name="toAddress">接收方地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="contractAddress">合约地址（可选，如果是 ERC20 转账）</param>
        /// <returns>Gas 费用估算（MATIC）</returns>
        Task<decimal> EstimateGasFeeAsync(string fromAddress, string toAddress, decimal amount, string? contractAddress = null);
    }

    /// <summary>
    /// Polygon 交易状态
    /// </summary>
    public class PolygonTransactionStatus
    {
        /// <summary>
        /// 交易哈希
        /// </summary>
        public string TransactionHash { get; set; } = string.Empty;

        /// <summary>
        /// 交易状态（SUCCESS, FAILED, PENDING）
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 确认数
        /// </summary>
        public int Confirmations { get; set; }

        /// <summary>
        /// 错误信息（如果有）
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}

