namespace SmallTarget.WebApi.Constants
{
    /// <summary>
    /// 查询配置
    /// </summary>
    public static class LocalConfig
    {
        /// <summary>
        /// 系统内置管理员UID
        /// </summary>
        public const int SystemAdministratorUid = 1;

        /// <summary>
        /// 默认 Dapp 查询页面大小
        /// </summary>
        public const int DefaultDappQueryPageSize = 5;

        /// <summary>
        /// 默认 管理端 查询页面大小
        /// </summary>
        public const int DefaulManagementQueryPageSize = 15;

        /// <summary>
        /// 钱包更新限制分钟
        /// </summary>
        public const int WalletUpdateIntervalLimitMinutes = 5;

        /// <summary>
        /// 未授权用户钱包更新限制分钟
        /// </summary>
        public const int NotApprovedUserWalletUpdateIntervalLimitMinutes = 60;

        /// <summary>
        /// 超时交易判定时间（分钟）
        /// </summary>
        public const int TransactionTimeoutJudgmentMinutes = 300;

        /// <summary>
        /// 新用户消息标题
        /// </summary>
        public const string NewUserMessageTempLateTitle = $"Welcome to join us";

        /// <summary>
        /// 新用户消息内容
        /// </summary>
        public const string NewUserMessageTempLateContent = $"Dear user, if you have any questions during using, you could contact our online service staff.\r\nTo ensure the security of your account, please keep your mnemonic phrase and private key safe and do not share them with anyone.\r\nPlease keep in mind that our online service staff will not ask you for mnemonic phrases and private keys.";
    }
}
