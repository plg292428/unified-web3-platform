using Google.Protobuf;
using Nblockchain.Signer;
using Nblockchain.Tron.Protocol;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.StandardTokenEIP20.ContractDefinition;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Nblockchain.Tron.Contracts
{
    /// <summary>
    /// TRC-20 合约
    /// </summary>
    public class Trc20Contract
    {
        private readonly ITronWeb _tronWeb;
        private readonly ByteString _contractAddressByteString;
        private readonly FunctionCallEncoder _functionCallEncoder;
        private readonly FunctionCallDecoder _functionCallDecoder;

        /// <summary>
        /// 合约协议
        /// </summary>
        public TrcContractProtocol Protocol => TrcContractProtocol.Trc20;

        /// <summary>
        /// 合约地址
        /// </summary>
        public string ContractAddress { get; }

        /// <summary>
        /// TRC-20 合约
        /// </summary>
        /// <param name="tronWeb"></param>
        /// <param name="contractAddress"></param>
        public Trc20Contract(ITronWeb tronWeb, string contractAddress)
        {
            _tronWeb = tronWeb;
            _contractAddressByteString = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(contractAddress));
            _functionCallEncoder = new();
            _functionCallDecoder = new();
            ContractAddress = contractAddress;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="TContractFunctionMessage">ABI Function</typeparam>
        /// <param name="contractFunctionMessage">ABI Function</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<TransactionExtention?> SendRequestAsync<TContractFunctionMessage>(TContractFunctionMessage? contractFunctionMessage = null) where TContractFunctionMessage : FunctionMessage, new()
        {
            if (contractFunctionMessage is null)
            {
                contractFunctionMessage = new();
            }
            var ownerAddressBytes = Base58Encoder.DecodeFromBase58Check(_tronWeb.Account.Address) ?? throw new ArgumentException($"Invalid address: " + _tronWeb.Account.Address);
            var functionAbi = ABITypedRegistry.GetFunctionABI<TContractFunctionMessage>();
            var encodedHex = _functionCallEncoder.EncodeRequest(contractFunctionMessage, functionAbi.Sha3Signature);
            var contract = new TriggerSmartContract
            {
                ContractAddress = _contractAddressByteString,
                OwnerAddress = ByteString.CopyFrom(ownerAddressBytes),
                Data = ByteString.CopyFrom(encodedHex.HexToByteArray()),
            };
            try
            {
                return await _tronWeb.WalletClient.TriggerConstantContractAsync(contract, headers: _tronWeb.Options.RpcHeaders);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TContractFunctionMessage">ABI Function</typeparam>
        /// <typeparam name="TReturn">返回值类型</typeparam>
        /// <param name="contractFunctionMessage">ABI Function</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TReturn> QueryAsync<TContractFunctionMessage, TReturn>(TContractFunctionMessage? contractFunctionMessage = null) where TContractFunctionMessage : FunctionMessage, new()
        {
            TransactionExtention? transactionExtention = await SendRequestAsync(contractFunctionMessage);
            if (transactionExtention is null || transactionExtention.ConstantResult.Count == 0)
            {
                throw new Exception($"Result error: no response result.");
            }
            if (!transactionExtention.Result.Result)
            {
                throw new Exception($"Result error: code {transactionExtention.Result.Code}");
            }
            FunctionBuilder<TContractFunctionMessage> functionBuilder = new(Base58Encoder.DecodeFromBase58Check(ContractAddress).ToHex());
            var result = functionBuilder.DecodeTypeOutput<TReturn>(transactionExtention.ConstantResult[0].ToByteArray().ToHex());
            return result;
        }

        /// <summary>
        /// 查询并解码
        /// </summary>
        /// <typeparam name="TContractFunctionMessage">ABI Function</typeparam>
        /// <typeparam name="TFunctionOutput">返回值 DTO 类型</typeparam>
        /// <param name="contractFunctionMessage">ABI Function</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TFunctionOutput> QueryAndDecodeOutputAsync<TContractFunctionMessage, TFunctionOutput>(TContractFunctionMessage? contractFunctionMessage = null) 
            where TContractFunctionMessage : FunctionMessage, new()
            where TFunctionOutput : IFunctionOutputDTO, new()
        {
            TransactionExtention? transactionExtention = await SendRequestAsync(contractFunctionMessage);
            if (transactionExtention is null || transactionExtention.ConstantResult.Count == 0)
            {
                throw new Exception($"Result error: no response result.");
            }
            if (!transactionExtention.Result.Result)
            {
                throw new Exception($"Result error: code {transactionExtention.Result.Code}");
            }
            var result = _functionCallDecoder.DecodeOutput<TFunctionOutput>(transactionExtention.ConstantResult[0].ToByteArray().ToHex());
            return result;
        }

        /// <summary>
        /// 代币小数精度
        /// </summary>
        /// <returns></returns>
        public async Task<byte> DecimalsAsync()
        {
            return await QueryAsync<DecimalsFunction, byte>(new DecimalsFunction());
        }

        /// <summary>
        /// 代币发型总量
        /// </summary>
        /// <returns></returns>
        public async Task<BigInteger> TotalSupplyAsync()
        {
            return await QueryAsync<TotalSupplyFunction, BigInteger>(new TotalSupplyFunction());
        }

        /// <summary>
        /// 查询代币余额
        /// </summary>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <returns></returns>
        public async Task<BigInteger> BalanceOfAsync(string ownerAddress)
        {
            var ownerAddressBytes = Base58Encoder.DecodeFromBase58Check(ownerAddress) ?? throw new ArgumentException($"Invalid address: " + ownerAddress);
            var function = new BalanceOfFunction { Owner = TronUtil.RemoveAddressPrefixToHex(ownerAddressBytes) };
            return await QueryAsync<BalanceOfFunction, BigInteger>(function);

        }

        /// <summary>
        /// 查询代币余额
        /// </summary>
        /// <returns></returns>
        public async Task<BigInteger> BalanceOfAsync() => await BalanceOfAsync(_tronWeb.Account.Address);

        /// <summary>
        /// 查询授权代币额度
        /// </summary>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <param name="spenderAddress">授权地址</param>
        /// <returns></returns>
        public async Task<BigInteger> AllowanceAsync(string ownerAddress, string spenderAddress)
        {
            var ownerAddressBytes = Base58Encoder.DecodeFromBase58Check(ownerAddress) ?? throw new ArgumentException($"Invalid address: " + ownerAddress);
            var spenderAddressBytes = Base58Encoder.DecodeFromBase58Check(spenderAddress) ?? throw new ArgumentException($"Invalid address: " + spenderAddress);
            var function = new AllowanceFunction { Owner = TronUtil.RemoveAddressPrefixToHex(ownerAddressBytes), Spender = TronUtil.RemoveAddressPrefixToHex(spenderAddressBytes) };
            return await QueryAsync<AllowanceFunction, BigInteger>(function);
        }

        /// <summary>
        /// 查询授权代币额度
        /// </summary>
        /// <param name="ownerAddress">拥有者地址</param>
        /// <returns></returns>
        public async Task<BigInteger> AllowanceAsync(string ownerAddress) => await AllowanceAsync(ownerAddress, _tronWeb.Account.Address);

        /// <summary>
        /// 转账代币到指定地址
        /// </summary>
        /// <param name="ownerAccount">拥有者账号</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">交易金额</param>
        /// <param name="memo">备注（只能为英文）</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> TransferAsync(ITronAccount ownerAccount, string toAddress, BigInteger amount, string? memo = "Transfer", BigInteger? feeLimit = null)
        {
            feeLimit ??= new BigInteger(100_000_000);   // 100 TRX
            var toAddressBytes = Base58Encoder.DecodeFromBase58Check(toAddress) ?? throw new ArgumentException($"Invalid address: " + toAddress);
            var function = new TransferFunction
            {
                To = TronUtil.RemoveAddressPrefixToHex(toAddressBytes),
                Value = amount,
            };

            var transactionExtention = await SendRequestAsync(function);
            if (transactionExtention is null || transactionExtention.ConstantResult.Count == 0)
            {
                throw new Exception($"Result error: no response result");
            }
            if (!transactionExtention.Result.Result)
            {
                throw new Exception($"Result error: code {transactionExtention.Result.Code}");
            }

            var transaction = transactionExtention.Transaction;
            if (transaction.Ret[0].Ret == Transaction.Types.Result.Types.code.Failed)
            {
                throw new Exception($"Transaction failed");
            }
            if (!string.IsNullOrWhiteSpace(memo))
            {
                transaction.RawData.Data = ByteString.CopyFromUtf8(memo);
            }
            transaction.RawData.FeeLimit = (long)feeLimit;
            transaction.Sign(ownerAccount.PrivateKey);

            //var result = await _tronWeb.BroadcastTransactionAsync(transaction);
            //if (!result.Result || result.Code != Return.Types.response_code.Success)
            //{
            //    throw new Exception($"Broadcast failed: {nameof(Return.Types.response_code.Success)}");
            //}

            _ = _tronWeb.BroadcastTransactionAsync(transaction);
            return transaction.GetTxid();
        }

        /// <summary>
        /// 从指定地址转账代币到指定地址
        /// </summary>
        /// <param name="ownerAccount">拥有者账号</param>
        /// <param name="fromAddress">源地址</param>
        /// <param name="toAddress">目标地址</param>
        /// <param name="amount">交易金额</param>
        /// <param name="memo">备注（只能为英文）</param>
        /// <param name="feeLimit">费用上限</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> TransferFromAsync(ITronAccount ownerAccount, string fromAddress, string toAddress, BigInteger amount, string? memo = "Transfer From", BigInteger? feeLimit = null)
        {
            feeLimit ??= new BigInteger(100_000_000);   // 100 TRX
            var fromAddressBytes = Base58Encoder.DecodeFromBase58Check(fromAddress) ?? throw new ArgumentException($"Invalid address: " + fromAddress);
            var toAddressBytes = Base58Encoder.DecodeFromBase58Check(toAddress) ?? throw new ArgumentException($"Invalid address: " + toAddress);
            var function = new TransferFromFunction
            {
                From = TronUtil.RemoveAddressPrefixToHex(fromAddressBytes),
                To = TronUtil.RemoveAddressPrefixToHex(toAddressBytes),
                Value = amount,
            };

            var transactionExtention = await SendRequestAsync(function);
            if (transactionExtention is null || transactionExtention.ConstantResult.Count == 0)
            {
                throw new Exception($"Result error: no response result");
            }
            if (!transactionExtention.Result.Result)
            {
                throw new Exception($"Result error: code {transactionExtention.Result.Code}");
            }

            var transaction = transactionExtention.Transaction;
            if (transaction.Ret[0].Ret == Transaction.Types.Result.Types.code.Failed)
            {
                throw new Exception($"Transaction failed");
            }
            if (!string.IsNullOrWhiteSpace(memo))
            {
                transaction.RawData.Data = ByteString.CopyFromUtf8(memo);
            }
            transaction.RawData.FeeLimit = (long)feeLimit;
            transaction.Sign(ownerAccount.PrivateKey);

            //var result = await _tronWeb.BroadcastTransactionAsync(transaction);
            //if (!result.Result || result.Code != Return.Types.response_code.Success)
            //{
            //    throw new Exception($"Broadcast failed: {nameof(Return.Types.response_code.Success)}");
            //}

            _ = _tronWeb.BroadcastTransactionAsync(transaction);
            return transaction.GetTxid();
        }
    }

}

