using HFastKit.AspNetCore.Shared.Text;
using Nethereum.Contracts;
using Nethereum.Optimism.L2StandardERC20.ContractDefinition;
using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;
using SmallTarget.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SmallTarget.WebApi.Services
{
    /// <summary>
    /// Web3 提供方
    /// </summary>
    public interface IWeb3Provider
    {
        /// <summary>
        /// 链网络
        /// </summary>
        public ChainNetwork ChainNetwork { get; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// 查询指定交易状态
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <param name="transactionStatus">交易状态</param>
        /// <returns>业务逻辑是否成功</returns>
        public bool QueryTransactionStatus(string transactionId, [NotNullWhen(true)] out ChainTransactionStatus? transactionStatus);

        /// <summary>
        /// 指定交易是否为授权交易
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <param name="functionBase">授权方法对象</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsApproveTransaction(string transactionId, [NotNullWhen(true)] out ApproveTransactionData? transactionData);

        // <summary>
        /// 指定交易是否为转账交易
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsTransferTransaction(string transactionId, [NotNullWhen(true)] out TransferTransactionData? transactionData);

        /// <summary>
        /// 查询指定地址通货余额
        /// </summary>
        /// <param name="ownerAddress">拥有者钱包地址</param>
        /// <param name="balance">余额</param>
        /// <returns></returns>
        public bool QueryCurrencyBalance(string ownerAddress, [NotNullWhen(true)] out BigInteger? balance);

        /// <summary>
        /// 查询本实例账号通货余额
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        public bool SelfCurrencyBalance([NotNullWhen(true)] out BigInteger? balance);

        /// <summary>
        /// 查询指定地址指定代币余额
        /// </summary>
        /// <param name="contractAddress">合约地址</param>
        /// <param name="ownerAddress">拥有者钱包地址</param>
        /// <param name="balance">余额</param>
        /// <returns></returns>
        public bool QueryTokenBalance(string contractAddress, string ownerAddress, [NotNullWhen(true)] out BigInteger? balance);

        /// <summary>
        /// 查询本实例账号指定代币余额
        /// </summary>
        /// <param name="contractAddress">合约地址</param>
        /// <param name="balance">余额</param>
        /// <returns></returns>
        public bool SelfTokenBalance(string contractAddress, [NotNullWhen(true)] out BigInteger? balance);

        /// <summary>
        /// 查询指定地址授权到指定地址的指定代币额度
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="spenderAddress">被授权的地址</param>
        /// <param name="remaining">授权额度</param>
        /// <returns></returns>
        public bool QueryAllowance(string contractAddress, string ownerAddress, string spenderAddress, [NotNullWhen(true)] out BigInteger? remaining);

        /// <summary>
        /// 查询指定地址授权到本实例账号的指定代币额度
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="remaining">授权额度</param>
        /// <returns></returns>
        public bool QueryAllowanceBySelf(string contractAddress, string ownerAddress, [NotNullWhen(true)] out BigInteger? remaining);

        /// <summary>
        /// 本实例账号转账代币到指定地址
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="transactionId">交易 ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool TransferToken(string contractAddress, string toAddress, BigInteger amount, [NotNullWhen(true)] out string? transactionId);

        /// <summary>
        /// 从指定地址转账代币
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="fromAddress">源地址</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="transactionId">交易 ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool TransferTokenFrom(string contractAddress, string fromAddress, string toAddress, BigInteger amount, [NotNullWhen(true)] out string? transactionId);
    }

    /// <summary>
    /// Web3 提供方
    /// </summary>
    public class Web3Provider : IWeb3Provider
    {
        private readonly Web3 web3;
        private readonly Nethereum.Web3.Accounts.Account account;

        /// <summary>
        /// 最小代币金额
        /// </summary>
        public const decimal MinTokenDecimalValue = 0.000001m;

        /// <summary>
        /// 最大代币金额
        /// </summary>
        public const decimal MaxTokenDecimalValue = 9999999999999.999999m;

        /// <summary>
        /// 代币小数精度
        /// </summary>
        public const int TokenDecimalDigits = 6;

        /// <summary>
        /// 链网络
        /// </summary>
        public ChainNetwork ChainNetwork { get; private set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address => account.Address;

        public Web3Provider(string privateKey, ChainNetwork chainNetwork, string rpcUrl)
        {
            account = new Nethereum.Web3.Accounts.Account(privateKey, (int)chainNetwork);
            web3 = new Web3(account, rpcUrl);
        }

        /// <summary>
        /// BigInteger 安全转到 Decimal，不会超过本类中的限制值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal BigIntegerSafeToDecimal(BigInteger value, int decimals)
        {
            if (decimals < 1) throw new ArgumentOutOfRangeException($"Parameter out of range: {nameof(decimals)}");
            BigInteger decimalsPows = BigInteger.Pow(new BigInteger(10), decimals);
            BigInteger result = value / decimalsPows;
            decimal decimalResult;
            if (result < new BigInteger(MinTokenDecimalValue))
            {
                return MinTokenDecimalValue;
            }
            else if (result > new BigInteger(MaxTokenDecimalValue))
            {
                return MaxTokenDecimalValue;
            }
            else
            {
                try
                {
                    decimalResult = (decimal)result;
                }
                catch
                {
                    decimalResult = 0.0m;
                }

            }
            return Math.Round(decimalResult, TokenDecimalDigits);
        }

        /// <summary>
        /// 查询指定交易状态
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <param name="transactionStatus">交易状态</param>
        /// <returns>业务逻辑是否成功</returns>
        public bool QueryTransactionStatus(string transactionId, [NotNullWhen(true)] out ChainTransactionStatus? transactionStatus)
        {
            if (!FormatValidate.IsTransactionId(transactionId)) throw new ArgumentException($"Parameter format error: {nameof(transactionId)}");

            transactionStatus = null;
            try
            {
                var transaction = web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionId).Result;
                if (transaction is null)
                {
                    transactionStatus = ChainTransactionStatus.None;
                }
                else
                {

                    if (transaction.BlockNumber is null)
                    {
                        transactionStatus = ChainTransactionStatus.Pending;
                    }
                    else
                    {
                        var transactionReceipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionId).Result;
                        if (transactionReceipt.Status.HexValue != "0x1")
                        {
                            transactionStatus = ChainTransactionStatus.Failed;
                        }
                        else
                        {
                            transactionStatus = ChainTransactionStatus.Succeed;
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 指定交易是否为授权交易
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsApproveTransaction(string transactionId, [NotNullWhen(true)] out ApproveTransactionData? transactionData)
        {
            transactionData = null;
            if (!FormatValidate.IsTransactionId(transactionId)) throw new ArgumentException($"Parameter format error: {nameof(transactionId)}");
            try
            {
                var transaction = web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionId).Result;
                if (transaction is null)
                {
                    return false;
                }
                var tempFunctionBase = new ApproveFunctionBase().DecodeInput(transaction.Input);
                if (tempFunctionBase?.Spender == null)
                {
                    return false;
                }
                transactionData = new()
                {
                    FromAddress = transaction.From,
                    SpenderAddress = tempFunctionBase.Spender,
                    Remaining = tempFunctionBase.Amount
                };
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 指定交易是否为授权交易
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsIncreaseAllowanceTransaction(string transactionId, [NotNullWhen(true)] out ApproveTransactionData? transactionData)
        {
            transactionData = null;
            if (!FormatValidate.IsTransactionId(transactionId)) throw new ArgumentException($"Parameter format error: {nameof(transactionId)}");
            try
            {
                var transaction = web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionId).Result;
                if (transaction is null)
                {
                    return false;
                }
                var tempFunctionBase = new IncreaseAllowanceFunction().DecodeInput(transaction.Input);
                if (tempFunctionBase?.Spender == null)
                {
                    return false;
                }
                transactionData = new()
                {
                    FromAddress = transaction.From,
                    SpenderAddress = tempFunctionBase.Spender,
                    Remaining = tempFunctionBase.AddedValue
                };
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 指定交易是否为转账交易
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsTransferTransaction(string transactionId, [NotNullWhen(true)] out TransferTransactionData? transactionData)
        {
            transactionData = null;
            if (!FormatValidate.IsTransactionId(transactionId)) throw new ArgumentException($"Parameter format error: {nameof(transactionId)}");
            try
            {
                var transaction = web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionId).Result;
                if (transaction is null)
                {
                    return false;
                }
                var tempFunctionBase = new TransferFunctionBase().DecodeInput(transaction.Input);
                if (tempFunctionBase?.Recipient == null)
                {
                    return false;
                }
                transactionData = new()
                {
                    FromAddress = transaction.From,
                    ToAddress = tempFunctionBase.Recipient,
                    Value = tempFunctionBase.Amount
                };
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 查询指定地址通货余额
        /// </summary>
        /// <param name="ownerAddress">拥有者钱包地址</param>
        /// <param name="balance">余额</param>
        /// <returns></returns>
        public bool QueryCurrencyBalance(string ownerAddress, [NotNullWhen(true)] out BigInteger? balance)
        {
            if (!FormatValidate.IsEthereumAddress(ownerAddress)) throw new ArgumentException($"Parameter format error: {nameof(ownerAddress)}");

            balance = null;
            try
            {
                var value = web3.Eth.GetBalance.SendRequestAsync(ownerAddress).Result;
                balance = value.Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查询本实例账号通货余额
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        public bool SelfCurrencyBalance([NotNullWhen(true)] out BigInteger? balance)
        {
            return QueryCurrencyBalance(account.Address, out balance);
        }

        /// <summary>
        /// 查询指定地址指定代币余额
        /// </summary>
        /// <param name="contractAddress">合约地址</param>
        /// <param name="ownerAddress">拥有者钱包地址</param>
        /// <param name="balance">余额</param>
        /// <returns></returns>
        public bool QueryTokenBalance(string contractAddress, string ownerAddress, [NotNullWhen(true)] out BigInteger? balance)
        {
            if (!FormatValidate.IsEthereumAddress(contractAddress)) throw new ArgumentException($"Parameter format error: {nameof(contractAddress)}");
            if (!FormatValidate.IsEthereumAddress(ownerAddress)) throw new ArgumentException($"Parameter format error: {nameof(ownerAddress)}");

            balance = null;
            try
            {
                StandardTokenService service = new(web3, contractAddress);
                balance = service.BalanceOfQueryAsync(ownerAddress).Result;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查询本实例账号指定代币余额
        /// </summary>
        /// <param name="contractAddress">合约地址</param>
        /// <param name="balance">余额</param>
        /// <returns></returns>
        public bool SelfTokenBalance(string contractAddress, [NotNullWhen(true)] out BigInteger? balance)
        {
            return QueryTokenBalance(contractAddress, account.Address, out balance);
        }

        /// <summary>
        /// 查询指定地址授权到指定地址的指定代币额度
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="spenderAddress">被授权的地址</param>
        /// <param name="remaining">授权额度</param>
        /// <returns></returns>
        public bool QueryAllowance(string contractAddress, string ownerAddress, string spenderAddress, [NotNullWhen(true)] out BigInteger? remaining)
        {
            if (!FormatValidate.IsEthereumAddress(contractAddress)) throw new ArgumentException($"Parameter format error: {nameof(contractAddress)}");
            if (!FormatValidate.IsEthereumAddress(ownerAddress)) throw new ArgumentException($"Parameter format error: {nameof(ownerAddress)}");
            if (!FormatValidate.IsEthereumAddress(spenderAddress)) throw new ArgumentException($"Parameter format error: {nameof(spenderAddress)}");

            remaining = null;
            try
            {
                StandardTokenService service = new(web3, contractAddress);
                remaining = service.AllowanceQueryAsync(ownerAddress, spenderAddress).Result;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 查询指定地址授权到本实例账号的指定代币额度
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="remaining">授权额度</param>
        /// <returns></returns>
        public bool QueryAllowanceBySelf(string contractAddress, string ownerAddress, [NotNullWhen(true)] out BigInteger? remaining)
        {
            return QueryAllowance(contractAddress, ownerAddress, account.Address, out remaining);
        }

        /// <summary>
        /// 本实例账号转账代币到指定地址
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="transactionId">交易 ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool TransferToken(string contractAddress, string toAddress, BigInteger amount, [NotNullWhen(true)] out string? transactionId)
        {
            if (!FormatValidate.IsEthereumAddress(contractAddress)) throw new ArgumentException($"Parameter format error: {nameof(contractAddress)}");
            if (!FormatValidate.IsEthereumAddress(toAddress)) throw new ArgumentException($"Parameter format error: {nameof(toAddress)}");
            if (amount < 0) throw new ArgumentOutOfRangeException($"Parameter out of range: {nameof(amount)}");

            transactionId = null;
            try
            {
                StandardTokenService service = new(web3, contractAddress);
                transactionId = service.TransferRequestAsync(toAddress, amount).Result;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从指定地址转账代币
        /// </summary>
        /// <param name="contractAddress">代币合约地址</param>
        /// <param name="fromAddress">源地址</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="transactionId">交易 ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool TransferTokenFrom(string contractAddress, string fromAddress, string toAddress, BigInteger amount, [NotNullWhen(true)] out string? transactionId)
        {
            transactionId = null;
            try
            {
                if (!FormatValidate.IsEthereumAddress(contractAddress)) throw new ArgumentException($"Parameter format error: {nameof(contractAddress)}");
                if (!FormatValidate.IsEthereumAddress(fromAddress)) throw new ArgumentException($"Parameter format error: {nameof(fromAddress)}");
                if (!FormatValidate.IsEthereumAddress(toAddress)) throw new ArgumentException($"Parameter format error: {nameof(toAddress)}");
                if (amount < 0) throw new ArgumentOutOfRangeException($"Parameter out of range: {nameof(amount)}");

                //web3.TransactionManager.UseLegacyAsDefault = true;
                StandardTokenService service = new(web3, contractAddress);
                transactionId = service.TransferFromRequestAsync(fromAddress, toAddress, amount).Result;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
