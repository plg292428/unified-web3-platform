namespace Nblockchain
{
    /// <summary>
    /// 交易状态
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// 网络错误
        /// </summary>
        NetworkError = -1,

        /// <summary>
        /// 没有交易信息
        /// </summary>
        None = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        Pending = 1,

        /// <summary>
        /// 交易失败
        /// </summary>
        Failed = 2,

        /// <summary>
        /// 交易成功
        /// </summary>
        Succeed = 200,
    }
}

