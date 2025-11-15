using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.StandardTokenEIP20;
using Nethereum.Contracts;
using UnifiedPlatform.Shared;
using UnifiedPlatform.WebApi.Services;
using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;

namespace UnifiedPlatform.WebApi.Services.Polygon
{
    /// <summary>
    /// Polygon 服务实现
    /// </summary>
    public class PolygonService : IPolygonService
    {
        private readonly IWeb3ProviderService _web3ProviderService;
        private readonly ITempCaching _tempCaching;
        private readonly ILogger<PolygonService> _logger;

        public PolygonService(
            IWeb3ProviderService web3ProviderService,
            ITempCaching tempCaching,
            ILogger<PolygonService> logger)
        {
            _web3ProviderService = web3ProviderService ?? throw new ArgumentNullException(nameof(web3ProviderService));
            _tempCaching = tempCaching ?? throw new ArgumentNullException(nameof(tempCaching));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取 Polygon RPC URL
        /// </summary>
        private string GetPolygonRpcUrl()
        {
            var polygonConfig = _tempCaching.ChainNetworkConfigs.FirstOrDefault(c => c.ChainId == (int)ChainNetwork.Polygon);
            if (polygonConfig == null || string.IsNullOrWhiteSpace(polygonConfig.RpcUrl))
            {
                throw new Exception("Polygon RPC URL 未配置");
            }
            return polygonConfig.RpcUrl;
        }

        /// <summary>
        /// 创建只读 Web3 实例（用于查询）
        /// </summary>
        private Web3 CreateReadOnlyWeb3()
        {
            var rpcUrl = GetPolygonRpcUrl();
            return new Web3(rpcUrl);
        }

        /// <summary>
        /// 获取 MATIC 余额
        /// </summary>
        public async Task<decimal> GetMaticBalanceAsync(string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    throw new ArgumentException("地址不能为空", nameof(address));
                }

                var web3 = CreateReadOnlyWeb3();
                var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
                
                // 转换为 MATIC 单位（1 MATIC = 10^18 Wei）
                var maticBalance = Web3.Convert.FromWei(balance);

                _logger.LogDebug("查询MATIC余额: {Address} = {Balance} MATIC", address, maticBalance);
                return maticBalance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询MATIC余额失败: {Address}", address);
                throw;
            }
        }

        /// <summary>
        /// 获取 ERC20 代币余额
        /// </summary>
        public async Task<decimal> GetErc20BalanceAsync(string address, string contractAddress)
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

                var web3 = CreateReadOnlyWeb3();
                var service = new Nethereum.StandardTokenEIP20.StandardTokenService(web3, contractAddress);

                var balance = await service.BalanceOfQueryAsync(address);

                // 获取代币精度
                var decimals = await service.DecimalsQueryAsync();

                // 转换为小数单位
                var divisor = BigInteger.Pow(10, decimals);
                var decimalBalance = (decimal)balance / (decimal)divisor;

                _logger.LogDebug("查询ERC20余额: {Address} = {Balance} (合约: {Contract})", address, decimalBalance, contractAddress);
                return decimalBalance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询ERC20余额失败: {Address}, 合约: {Contract}", address, contractAddress);
                throw;
            }
        }

        /// <summary>
        /// 转账 MATIC
        /// </summary>
        public async Task<string> TransferMaticAsync(string fromPrivateKey, string toAddress, decimal amount)
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

                var account = new Account(fromPrivateKey, (int)ChainNetwork.Polygon);
                var rpcUrl = GetPolygonRpcUrl();
                var web3 = new Web3(account, rpcUrl);

                var amountInWei = Web3.Convert.ToWei(amount);
                var transactionReceipt = await web3.Eth.GetEtherTransferService()
                    .TransferEtherAndWaitForReceiptAsync(toAddress, amount);

                _logger.LogInformation("MATIC转账成功: {From} -> {To}, 金额: {Amount} MATIC, 交易: {TxHash}",
                    account.Address, toAddress, amount, transactionReceipt.TransactionHash);

                return transactionReceipt.TransactionHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MATIC转账失败: {To}, 金额: {Amount}", toAddress, amount);
                throw;
            }
        }

        /// <summary>
        /// 转账 ERC20 代币
        /// </summary>
        public async Task<string> TransferErc20Async(string fromPrivateKey, string toAddress, string contractAddress, decimal amount)
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

                var account = new Account(fromPrivateKey, (int)ChainNetwork.Polygon);
                var rpcUrl = GetPolygonRpcUrl();
                var web3 = new Web3(account, rpcUrl);

                var service = new Nethereum.StandardTokenEIP20.StandardTokenService(web3, contractAddress);

                // 获取代币精度
                var decimals = await service.DecimalsQueryAsync();

                // 转换为最小单位
                var amountInWei = Web3.Convert.ToWei(amount, decimals);

                // 转账代币
                var transactionReceipt = await service.TransferRequestAndWaitForReceiptAsync(toAddress, amountInWei);

                _logger.LogInformation("ERC20转账成功: {From} -> {To}, 金额: {Amount}, 合约: {Contract}, 交易: {TxHash}",
                    account.Address, toAddress, amount, contractAddress, transactionReceipt.TransactionHash);

                return transactionReceipt.TransactionHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERC20转账失败: {To}, 金额: {Amount}, 合约: {Contract}", toAddress, amount, contractAddress);
                throw;
            }
        }

        /// <summary>
        /// 查询交易状态
        /// </summary>
        public async Task<PolygonTransactionStatus> GetTransactionStatusAsync(string transactionHash)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transactionHash))
                {
                    throw new ArgumentException("交易哈希不能为空", nameof(transactionHash));
                }

                var web3 = CreateReadOnlyWeb3();
                var transaction = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);

                if (transaction == null)
                {
                    return new PolygonTransactionStatus
                    {
                        TransactionHash = transactionHash,
                        Status = "PENDING",
                        IsSuccess = false
                    };
                }

                var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

                var status = new PolygonTransactionStatus
                {
                    TransactionHash = transactionHash,
                    Status = receipt?.Status?.Value == 1 ? "SUCCESS" : "FAILED",
                    IsSuccess = receipt?.Status?.Value == 1,
                    Confirmations = receipt != null ? 1 : 0 // 简化处理，实际应该计算确认数
                };

                _logger.LogDebug("查询交易状态: {TxHash} = {Status}", transactionHash, status.Status);
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询交易状态失败: {TxHash}", transactionHash);
                throw;
            }
        }

        /// <summary>
        /// 获取 Gas 价格估算
        /// </summary>
        public async Task<decimal> GetGasPriceAsync()
        {
            try
            {
                var web3 = CreateReadOnlyWeb3();
                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                var gasPriceInGwei = Web3.Convert.FromWei(gasPrice.Value, 9); // Gwei = 10^9 Wei

                _logger.LogDebug("当前Gas价格: {GasPrice} Gwei", gasPriceInGwei);
                return gasPriceInGwei;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取Gas价格失败");
                throw;
            }
        }

        /// <summary>
        /// 估算交易 Gas 费用
        /// </summary>
        public async Task<decimal> EstimateGasFeeAsync(string fromAddress, string toAddress, decimal amount, string? contractAddress = null)
        {
            try
            {
                var gasPrice = await GetGasPriceAsync();
                var gasPriceInWei = Web3.Convert.ToWei(gasPrice, 9);

                BigInteger estimatedGas;

                if (string.IsNullOrEmpty(contractAddress))
                {
                    // MATIC 转账
                    estimatedGas = 21000; // 标准 ETH/MATIC 转账 Gas
                }
                else
                {
                    // ERC20 转账
                    estimatedGas = 65000; // 标准 ERC20 转账 Gas
                }

                var totalGasFee = estimatedGas * gasPriceInWei;
                var gasFeeInMatic = Web3.Convert.FromWei(totalGasFee);

                _logger.LogDebug("估算Gas费用: {GasFee} MATIC (Gas: {Gas}, Price: {Price} Gwei)",
                    gasFeeInMatic, estimatedGas, gasPrice);

                return gasFeeInMatic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "估算Gas费用失败");
                throw;
            }
        }
    }
}

