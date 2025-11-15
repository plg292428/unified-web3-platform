using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nblockchain;
using Nblockchain.Signer;
using Nblockchain.Tron;
using Nblockchain.Tron.Contracts;
using HFastKit.AspNetCore.Shared.Text;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace UnifiedPlatform.WebApi.Services.Tron
{
    /// <summary>
    /// TRON 服务实现
    /// </summary>
    public class TronService : ITronService
    {
        private readonly ITronWebFactory _tronWebFactory;
        private readonly ILogger<TronService> _logger;
        private readonly TronWebOptions _options;

        public TronService(ITronWebFactory tronWebFactory, IOptions<TronWebOptions> options, ILogger<TronService> logger)
        {
            _tronWebFactory = tronWebFactory ?? throw new ArgumentNullException(nameof(tronWebFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// 创建新钱包
        /// </summary>
        public Task<TronWalletInfo> CreateWalletAsync()
        {
            try
            {
                // 生成新的私钥
                var ecKey = TronECKey.GenerateKey(_options.Network);
                var account = new TronAccount(ecKey);

                var walletInfo = new TronWalletInfo
                {
                    Address = account.Address,
                    PrivateKey = account.PrivateKey,
                    PublicKey = account.PublicKey
                };

                _logger.LogInformation("创建新TRON钱包: {Address}", walletInfo.Address);
                return Task.FromResult(walletInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建TRON钱包失败");
                throw;
            }
        }

        /// <summary>
        /// 从私钥获取钱包信息
        /// </summary>
        public TronWalletInfo GetWalletFromPrivateKey(string privateKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(privateKey))
                {
                    throw new ArgumentException("私钥不能为空", nameof(privateKey));
                }

                var account = new TronAccount(privateKey, _options.Network);

                return new TronWalletInfo
                {
                    Address = account.Address,
                    PrivateKey = account.PrivateKey,
                    PublicKey = account.PublicKey
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从私钥获取钱包信息失败");
                throw;
            }
        }

        /// <summary>
        /// 获取 TRX 余额
        /// </summary>
        public async Task<decimal> GetTrxBalanceAsync(string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    throw new ArgumentException("地址不能为空", nameof(address));
                }

                // 创建一个临时账户用于查询（不需要私钥）
                var tempEcKey = TronECKey.GenerateKey(_options.Network);
                var tempAccount = new TronAccount(tempEcKey);
                var tronWeb = _tronWebFactory.Create(tempAccount);

                var balance = await tronWeb.GetTrxBalanceAsync(address);
                
                // 转换为 TRX 单位（1 TRX = 1,000,000 SUN）
                var trxBalance = (decimal)balance / 1_000_000m;

                _logger.LogDebug("查询TRX余额: {Address} = {Balance} TRX", address, trxBalance);
                return trxBalance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询TRX余额失败: {Address}", address);
                throw;
            }
        }

        /// <summary>
        /// 获取 TRC20 代币余额
        /// </summary>
        public async Task<decimal> GetTrc20BalanceAsync(string address, string contractAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    throw new ArgumentException("地址不能为空", nameof(address));
                }

                if (string.IsNullOrWhiteSpace(contractAddress))
                {
                    throw new ArgumentException("合约地址不能为空", nameof(contractAddress));
                }

                // 创建一个临时账户用于查询
                var tempEcKey = TronECKey.GenerateKey(_options.Network);
                var tempAccount = new TronAccount(tempEcKey);
                var tronWeb = _tronWebFactory.Create(tempAccount);
                var trc20Contract = tronWeb.Trc20Contract(contractAddress);

                // 查询代币精度
                var decimals = await trc20Contract.DecimalsAsync();
                
                // 查询余额
                var balance = await trc20Contract.BalanceOfAsync(address);

                // 转换为小数单位
                var divisor = BigInteger.Pow(10, decimals);
                var decimalBalance = (decimal)balance / (decimal)divisor;

                _logger.LogDebug("查询TRC20余额: {Address} = {Balance} (合约: {Contract})", address, decimalBalance, contractAddress);
                return decimalBalance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询TRC20余额失败: {Address}, 合约: {Contract}", address, contractAddress);
                throw;
            }
        }

        /// <summary>
        /// 转账 TRX
        /// </summary>
        public async Task<string> TransferTrxAsync(string fromPrivateKey, string toAddress, decimal amount, string? memo = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromPrivateKey))
                {
                    throw new ArgumentException("私钥不能为空", nameof(fromPrivateKey));
                }

                if (string.IsNullOrWhiteSpace(toAddress))
                {
                    throw new ArgumentException("接收地址不能为空", nameof(toAddress));
                }

                if (amount <= 0)
                {
                    throw new ArgumentException("转账金额必须大于0", nameof(amount));
                }

                // 创建发送方账户
                var fromAccount = new TronAccount(fromPrivateKey, _options.Network);
                var tronWeb = _tronWebFactory.Create(fromAccount);

                // 转换为 SUN 单位（1 TRX = 1,000,000 SUN）
                var amountInSun = (long)(amount * 1_000_000m);

                // 执行转账
                var transactionId = await tronWeb.TransferTrxAsync(
                    fromAccount.Address,
                    fromPrivateKey,
                    toAddress,
                    amountInSun,
                    memo ?? "Transfer",
                    null
                );

                _logger.LogInformation("TRX转账成功: {From} -> {To}, 金额: {Amount} TRX, 交易ID: {TxId}", 
                    fromAccount.Address, toAddress, amount, transactionId);

                return transactionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TRX转账失败: {To}, 金额: {Amount}", toAddress, amount);
                throw;
            }
        }

        /// <summary>
        /// 转账 TRC20 代币
        /// </summary>
        public async Task<string> TransferTrc20Async(string fromPrivateKey, string toAddress, string contractAddress, decimal amount, string? memo = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromPrivateKey))
                {
                    throw new ArgumentException("私钥不能为空", nameof(fromPrivateKey));
                }

                if (string.IsNullOrWhiteSpace(toAddress))
                {
                    throw new ArgumentException("接收地址不能为空", nameof(toAddress));
                }

                if (string.IsNullOrWhiteSpace(contractAddress))
                {
                    throw new ArgumentException("合约地址不能为空", nameof(contractAddress));
                }

                if (amount <= 0)
                {
                    throw new ArgumentException("转账金额必须大于0", nameof(amount));
                }

                // 创建发送方账户
                var fromAccount = new TronAccount(fromPrivateKey, _options.Network);
                var tronWeb = _tronWebFactory.Create(fromAccount);
                var trc20Contract = tronWeb.Trc20Contract(contractAddress);

                // 查询代币精度
                var decimals = await trc20Contract.DecimalsAsync();
                
                // 转换为代币最小单位
                var divisor = BigInteger.Pow(10, decimals);
                var amountInTokenUnit = (BigInteger)(amount * (decimal)divisor);

                // 执行转账
                var transactionId = await trc20Contract.TransferAsync(
                    fromAccount,
                    toAddress,
                    amountInTokenUnit,
                    memo ?? "Transfer",
                    null
                );

                _logger.LogInformation("TRC20转账成功: {From} -> {To}, 金额: {Amount}, 合约: {Contract}, 交易ID: {TxId}", 
                    fromAccount.Address, toAddress, amount, contractAddress, transactionId);

                return transactionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TRC20转账失败: {To}, 金额: {Amount}, 合约: {Contract}", toAddress, amount, contractAddress);
                throw;
            }
        }

        /// <summary>
        /// 查询交易状态
        /// </summary>
        public async Task<TronTransactionStatus> GetTransactionStatusAsync(string transactionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transactionId))
                {
                    throw new ArgumentException("交易ID不能为空", nameof(transactionId));
                }

                // 验证交易ID格式（64个字符的十六进制字符串，可选0x前缀）
                if (!FormatValidate.IsTransactionId(transactionId))
                {
                    throw new ArgumentException($"交易ID格式无效: {transactionId}。交易ID必须是64个字符的十六进制字符串（可选0x前缀）", nameof(transactionId));
                }

                // 创建一个临时账户用于查询
                var tempEcKey = TronECKey.GenerateKey(_options.Network);
                var tempAccount = new TronAccount(tempEcKey);
                var tronWeb = _tronWebFactory.Create(tempAccount);

                var status = await tronWeb.QueryTransactionStatus(transactionId);

                var result = new TronTransactionStatus
                {
                    TransactionId = transactionId,
                    Status = status.ToString(),
                    IsSuccess = status == TransactionStatus.Succeed,
                    ErrorMessage = status != TransactionStatus.Succeed ? $"交易状态: {status}" : null
                };

                _logger.LogDebug("查询交易状态: {TxId} = {Status}", transactionId, status);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询交易状态失败: {TxId}", transactionId);
                throw;
            }
        }
    }
}


