using System.Numerics;

namespace SmallTarget.WebApi.Services
{
    /// <summary>
    /// 授权交易数据
    /// </summary>
    public class ApproveTransactionData
    {
        /// <summary>
        /// 源地址
        /// </summary>
        public required string FromAddress { get; set; }

        /// <summary>
        /// 授权对象地址
        /// </summary>
        public required string SpenderAddress { get; set; }

        /// <summary>
        /// 授权额度
        /// </summary>
        public BigInteger Remaining { get; set; }
    }

    /// <summary>
    /// 转账交易数据
    /// </summary>
    public class TransferTransactionData
    {
        /// <summary>
        /// 源地址
        /// </summary>
        public required string FromAddress { get; set; }

        /// <summary>
        /// 授权对象地址
        /// </summary>
        public required string ToAddress { get; set; }

        /// <summary>
        /// 转账值
        /// </summary>
        public BigInteger Value { get; set; }
    }
}
