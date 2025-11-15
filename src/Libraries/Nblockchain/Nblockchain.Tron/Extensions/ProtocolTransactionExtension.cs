using Google.Protobuf;
using Nblockchain.Signer;
using Nblockchain.Tron.Protocol;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;

namespace Nblockchain.Tron
{
    public static class ProtocolTransactionExtension
    {
        /// <summary>
        /// 获取 Transaction ID
        /// </summary>
        /// <param name="transaction">交易</param>
        /// <returns></returns>
        public static string GetTxid(this Transaction transaction)
        {
            var txid = transaction.RawData.ToByteArray().ToSHA256Hash().ToHex();
            return txid;
        }

        /// <summary>
        /// 交易签名
        /// </summary>
        /// <param name="transaction">交易</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static void Sign(this Transaction transaction, string privateKey)
        {
            var ecKey = new TronECKey(privateKey.HexToByteArray(), true);
            var rawdata = transaction.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash);
            transaction.Signature.Add(ByteString.CopyFrom(sign.ToByteArray()));
        }
    }
}

