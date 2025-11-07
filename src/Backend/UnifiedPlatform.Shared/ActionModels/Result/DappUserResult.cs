namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户响应数据
    /// </summary>
    public class DappUserResult
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 钱包地址
        /// </summary>
        public string? WalletAddress { get; set; }

        /// <summary>
        /// 链ID
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int UserLevel { get; set; }

        /// <summary>
        /// 注册IP
        /// </summary>
        public string? SignUpClientIp { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string? LastSignInClientIp { get; set; }

        /// <summary>
        /// 是否为虚拟用户
        /// </summary>
        public bool VirtualUser { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool Anomaly { get; set; }

        /// <summary>
        /// 邀请链接
        /// </summary>
        public string? InvitationLink { get; set; }

        /// <summary>
        /// 有新系统消息
        /// </summary>
        public bool NewSystemMessage { get; set; }

        /// <summary>
        /// 团队1层成员数量
        /// </summary>
        public int Layer1Members { get; set; }

        /// <summary>
        /// 团队2层成员数量
        /// </summary>
        public int Layer2Members { get; set; }

        /// <summary>
        /// 主要代币状态
        /// </summary>
        public PrimaryTokenStatus PrimaryTokenStatus { get; set; }

        /// <summary>
        /// 财产
        /// </summary>
        public DappUserAssetsResult? Asset { get; set; }
    }
}
