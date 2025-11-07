using System.Numerics;
using System.Threading.Tasks;

namespace UnifiedPlatform.WebApi.Services.Tron
{
    /// <summary>
    /// TRON 服务接口
    /// </summary>
    public interface ITronService
    {
        /// <summary>
        /// 创建新钱包
        /// </summary>
        /// <returns>钱包信息（地址、私钥、公钥）</returns>
        Task<TronWalletInfo> CreateWalletAsync();

        /// <summary>
        /// 从私钥获取钱包信息
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <returns>钱包信息</returns>
        TronWalletInfo GetWalletFromPrivateKey(string privateKey);

        /// <summary>
        /// 获取 TRX 余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <returns>TRX 余额（SUN单位，1 TRX = 1,000,000 SUN）</returns>
        Task<decimal> GetTrxBalanceAsync(string address);

        /// <summary>
        /// 获取 TRC20 代币余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">TRC20 合约地址</param>
        /// <returns>代币余额</returns>
        Task<decimal> GetTrc20BalanceAsync(string address, string contractAddress);

        /// <summary>
        /// 转账 TRX
        /// </summary>
        /// <param name="fromPrivateKey">发送方私钥</param>
        /// <param name="toAddress">接收方地址</param>
        /// <param name="amount">转账金额（TRX单位）</param>
        /// <param name="memo">备注（可选）</param>
        /// <returns>交易ID</returns>
        Task<string> TransferTrxAsync(string fromPrivateKey, string toAddress, decimal amount, string? memo = null);

        /// <summary>
        /// 转账 TRC20 代币
        /// </summary>
        /// <param name="fromPrivateKey">发送方私钥</param>
        /// <param name="toAddress">接收方地址</param>
        /// <param name="contractAddress">TRC20 合约地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="memo">备注（可选）</param>
        /// <returns>交易ID</returns>
        Task<string> TransferTrc20Async(string fromPrivateKey, string toAddress, string contractAddress, decimal amount, string? memo = null);

        /// <summary>
        /// 查询交易状态
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns>交易状态</returns>
        Task<TronTransactionStatus> GetTransactionStatusAsync(string transactionId);
    }

    /// <summary>
    /// TRON 钱包信息
    /// </summary>
    public class TronWalletInfo
    {
        /// <summary>
        /// 钱包地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// TRON 交易状态
    /// </summary>
    public class TronTransactionStatus
    {
        /// <summary>
        /// 交易ID
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// 交易状态（SUCCESS, FAILED, PENDING）
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息（如果有）
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}


