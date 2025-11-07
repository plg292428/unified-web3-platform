using Google.Protobuf;
using Grpc.Core;
using Nblockchain.Signer;
using Nblockchain.Tron.Contracts;
using Nblockchain.Tron.Protocol;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Nblockchain.Tron
{
    /// <summary>
    /// Tron Web
    /// </summary>
    public interface ITronWeb
    {
        /// <summary>
        /// 账号
        /// </summary>
        public ITronAccount Account { get; }

        /// <summary>
        /// Tron Web 选项
        /// </summary>
        public TronWebOptions Options { get; }

        /// <summary>
        /// 钱包客户端
        /// </summary>
        public Wallet.WalletClient WalletClient { get; }

        /// <summary>
        /// 合约地址
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <returns></returns>
        public Trc20Contract Trc20Contract(string contractAddress);

        /// <summary>
        /// 获取最新区块
        /// </summary>
        /// <returns></returns>
        public Task<BlockExtention?> GetNowBlockAsync();

        /// <summary>
        /// 查询交易状态
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns></returns>
        public Task<TransactionStatus> QueryTransactionStatus(string transactionId);

        /// <summary>
        /// 获取账号 TRX 余额
        /// </summary>
        /// <param name="ownerAddress"></param>
        /// <returns></returns>
        public Task<BigInteger> GetTrxBalanceAsync(string ownerAddress);

        /// <summary>
        /// 创建Trx交易并签名
        /// </summary>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<Transaction> CreateTrxTransactionAndSignAsync(string ownerAddress, string privateKey, string toAddress, long amount, string? memo = "Transfer", BigInteger? feeLimit = null);

        /// <summary>
        /// 创建Trx交易并签名
        /// </summary>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<Transaction> CreateTrxTransactionAndSignAsync(string toAddress, long amount, string? memo = "Transfer", BigInteger? feeLimit = null);

        /// <summary>
        /// 转账 TRX
        /// </summary>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<string> TransferTrxAsync(string ownerAddress, string privateKey, string toAddress, long amount, string? memo = "Transfer", BigInteger? feeLimit = null);

        /// <summary>
        /// 转账 TRX
        /// </summary>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<string> TransferTrxAsync(string toAddress, long amount, string? memo = "Transfer", BigInteger? feeLimit = null);

        /// <summary>
        /// 广播交易
        /// </summary>
        /// <param name="transaction">交易</param>
        /// <returns></returns>
        public Task<Return> BroadcastTransactionAsync(Transaction transaction);
    }

    /// <summary>
    /// Tron Web
    /// </summary>
    public class TronWeb : ITronWeb
    {
        /// <summary>
        /// Tron Web 选项
        /// </summary>
        private Channel _channel;

        /// <summary>
        /// 账号
        /// </summary>
        public ITronAccount Account { get; }

        /// <summary>
        /// Tron Web 选项
        /// </summary>
        public TronWebOptions Options { get; }

        /// <summary>
        /// 原生 RPC 钱包客户端
        /// </summary>
        public Wallet.WalletClient WalletClient { get; }

        /// <summary>
        /// Tron Web
        /// </summary>
        /// <param name="account">Tron 账号</param>
        /// <param name="options">Tron Web 选项</param>
        public TronWeb(ITronAccount account, TronWebOptions options)
        {
            _channel = new Channel(options.RpcUrl, options.Credentials);
            WalletClient = new(_channel);
            Account = account;
            Options = options;
        }

        /// <summary>
        /// Tron Web
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="options">Tron Web 选项</param>
        public TronWeb(string privateKey, TronWebOptions options) : this(new TronAccount(privateKey, options.Network), options)
        {
        }

        /// <summary>
        /// 合约地址
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <returns></returns>
        public Trc20Contract Trc20Contract(string contractAddress)
        {
            return new Trc20Contract(this, contractAddress);
        }

        /// <summary>
        /// 获取最新区块
        /// </summary>
        /// <returns></returns>
        public async Task<BlockExtention?> GetNowBlockAsync() => await WalletClient.GetNowBlock2Async(new EmptyMessage(), headers: Options.RpcHeaders);

        /// <summary>
        /// 查询交易状态
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns></returns>
        public async Task<TransactionStatus> QueryTransactionStatus(string transactionId)
        {
            var message = new BytesMessage()
            {
                Value = ByteString.CopyFrom(transactionId.HexToByteArray())
            };
            Transaction transaction = await WalletClient.GetTransactionByIdAsync(message, headers: Options.RpcHeaders);
            try
            {
                if (transaction.ToByteString().Length < 1)
                {
                    return TransactionStatus.None;
                }
                if (transaction.Ret.Count < 0)
                {
                    return TransactionStatus.Pending;
                }
                if (transaction.Ret[0].Ret != Transaction.Types.Result.Types.code.Sucess || transaction.Ret[0].ContractRet != Transaction.Types.Result.Types.contractResult.Success)
                {
                    return TransactionStatus.Failed;
                }
                return TransactionStatus.Succeed;
            }
            catch
            {
                return TransactionStatus.NetworkError;
            }
        }

        /// <summary>
        /// 获取账号 TRX 余额
        /// </summary>
        /// <param name="ownerAddress"></param>
        /// <returns></returns>
        public async Task<BigInteger> GetTrxBalanceAsync(string ownerAddress)
        {
            Account request = new ();
            request.Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(ownerAddress));
            var balance = await WalletClient.GetAccountAsync(request, headers: Options.RpcHeaders);
            return new BigInteger(balance.Balance);
        }

        ///// <summary>
        ///// 创建离线交易
        ///// </summary>
        ///// <param name="ownerAddress">拥有者地址</param>
        ///// <param name="toAddress">对方地址</param>
        ///// <param name="amount">金额</param>
        ///// <returns></returns>
        //public async Task<TransactionExtention> CreateTransactionAsync(string ownerAddress, string toAddress, long amount)
        //{
        //    var transferContract = new TransferContract
        //    {
        //        OwnerAddress = TronUtil.ParseAddress(ownerAddress),
        //        ToAddress = TronUtil.ParseAddress(toAddress),
        //        Amount = amount
        //    };

        //    var contract = new Transaction.Types.Contract();
        //    BlockExtention? nowBlock;
        //    try
        //    {
        //        nowBlock = await GetNowBlockAsync();
        //        if (nowBlock is null)
        //        {
        //            return new TransactionExtention
        //            {
        //                Result = new Return { Result = false, Code = Return.Types.response_code.OtherError },
        //            };
        //        }
        //        contract.Parameter = Any.Pack(transferContract);
        //        contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
        //    }
        //    catch (Exception)
        //    {
        //        return new TransactionExtention
        //        {
        //            Result = new Return { Result = false, Code = Return.Types.response_code.OtherError },
        //        };
        //    }
        //    var transaction = new Transaction();
        //    transaction.RawData = new ();
        //    transaction.RawData.Contract.Add(contract);
        //    transaction.RawData.Timestamp = DateTime.Now.Ticks;
        //    transaction.RawData.Expiration = nowBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000;
        //    var blockHeight = nowBlock.BlockHeader.RawData.Number;
        //    var blockHash = Sha256Sm3Hash.Of(nowBlock.BlockHeader.RawData.ToByteArray()).GetBytes();
        //    var buffer = ByteBuffer.Allocate(8);
        //    buffer.PutLong(blockHeight);
        //    var refBlockNum = buffer.ToArray();
        //    transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8));
        //    transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2));
        //    var transactionExtension = new TransactionExtention
        //    {
        //        Transaction = transaction,
        //        Txid = ByteString.CopyFromUtf8(transaction.GetTxid()),
        //        Result = new Return { Result = true, Code = Return.Types.response_code.Success },
        //    };
        //    return transactionExtension;
        //}

        /// <summary>
        /// 创建Trx交易并签名
        /// </summary>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Transaction> CreateTrxTransactionAndSignAsync(string ownerAddress, string privateKey, string toAddress, long amount, string? memo = "Transfer", BigInteger? feeLimit = null)
        {
            feeLimit ??= new BigInteger(100_000_000);   // 100 TRX
            var transferContract = new TransferContract
            {
                OwnerAddress = TronUtil.ParseAddress(ownerAddress),
                ToAddress = TronUtil.ParseAddress(toAddress),
                Amount = amount,
            };
            var transactionExtention =  await WalletClient.CreateTransaction2Async(transferContract, headers: Options.RpcHeaders);
            if (transactionExtention is null)
            {
                throw new Exception($"Result error: no response result");
            }
            if (!transactionExtention.Result.Result)
            {
                throw new Exception($"Result error: code {transactionExtention.Result.Code}");
            }

            var transaction = transactionExtention.Transaction;
            if (!string.IsNullOrWhiteSpace(memo))
            {
                transaction.RawData.Data = ByteString.CopyFromUtf8(memo);
            }
            transaction.RawData.FeeLimit = (long)feeLimit;
            transaction.Sign(privateKey);
            return transaction;
        }

        /// <summary>
        /// 创建Trx交易并签名
        /// </summary>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Transaction> CreateTrxTransactionAndSignAsync(string toAddress, long amount, string? memo = "Transfer", BigInteger? feeLimit = null)
        {
            return await CreateTrxTransactionAndSignAsync(Account.Address, Account.PrivateKey, toAddress, amount, memo, feeLimit);
        }

        /// <summary>
        /// 转账 TRX
        /// </summary>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> TransferTrxAsync(string ownerAddress, string privateKey, string toAddress, long amount, string? memo = "Transfer TRX", BigInteger? feeLimit = null)
        { 
            var transaction = await CreateTrxTransactionAndSignAsync(ownerAddress, privateKey, toAddress, amount, memo, feeLimit);
            //var result = await BroadcastTransactionAsync(transaction);
            //if (!result.Result || result.Code != Return.Types.response_code.Success)
            //{
            //    throw new Exception($"Broadcast failed: {nameof(Return.Types.response_code.Success)}");
            //}
            _ = BroadcastTransactionAsync(transaction);
            return transaction.GetTxid();
        }

        /// <summary>
        /// 转账 TRX
        /// </summary>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">金额</param>
        /// <param name="memo">备注</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> TransferTrxAsync(string toAddress, long amount, string? memo = "Transfer TRX", BigInteger? feeLimit = null)
        {
            return await TransferTrxAsync(Account.Address, Account.PrivateKey, toAddress, amount, memo, feeLimit);
        }

        /// <summary>
        /// 广播交易
        /// </summary>
        /// <param name="transaction">交易</param>
        /// <returns></returns>
        public async Task<Return> BroadcastTransactionAsync(Transaction transaction)
        {
            Return result = new Return { Result = false, Code = Return.Types.response_code.OtherError };
            try
            {
                result = await WalletClient.BroadcastTransactionAsync(transaction, headers: Options.RpcHeaders);
                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
