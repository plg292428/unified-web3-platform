using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class User
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 钱包地址
    /// </summary>
    public string WalletAddress { get; set; } = null!;

    /// <summary>
    /// 链ID
    /// </summary>
    public int ChainId { get; set; }

    /// <summary>
    /// 区块链钱包配置组ID
    /// </summary>
    public int ChainWalletConfigGroupId { get; set; }

    /// <summary>
    /// 用户等级
    /// </summary>
    public int UserLevel { get; set; }

    /// <summary>
    /// 备注标识
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 归属业务员UID
    /// </summary>
    public int? AttributionSalesmanUid { get; set; }

    /// <summary>
    /// 归属组长UID
    /// </summary>
    public int? AttributionGroupLeaderUid { get; set; }

    /// <summary>
    /// 归属代理UID
    /// </summary>
    public int? AttributionAgentUid { get; set; }

    /// <summary>
    /// 父级用户ID
    /// </summary>
    public int? ParentUserUid { get; set; }

    /// <summary>
    /// 父级用户ID路径
    /// </summary>
    public string? ParentUsersPath { get; set; }

    /// <summary>
    /// 父级用户深度
    /// </summary>
    public int ParentUserDepth { get; set; }

    /// <summary>
    /// 注册IP
    /// </summary>
    public string? SignUpClientIp { get; set; }

    /// <summary>
    /// 注册IP归属地
    /// </summary>
    public string? SignUpClientIpRegion { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    public string? LastSignInClientIp { get; set; }

    /// <summary>
    /// 最后登录IP归属地
    /// </summary>
    public string? LastSignInClientIpRegion { get; set; }

    /// <summary>
    /// 授权令牌
    /// </summary>
    public Guid? AccesTokenGuid { get; set; }

    /// <summary>
    /// 是否为虚拟用户
    /// </summary>
    public bool VirtualUser { get; set; }

    /// <summary>
    /// 是否异常
    /// </summary>
    public bool Anomaly { get; set; }

    /// <summary>
    /// 是否封停
    /// </summary>
    public bool Blocked { get; set; }

    /// <summary>
    /// 是否已逻辑删除
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual Manager? AttributionAgentU { get; set; }

    public virtual Manager? AttributionGroupLeaderU { get; set; }

    public virtual Manager? AttributionSalesmanU { get; set; }

    public virtual ChainNetworkConfig Chain { get; set; } = null!;

    public virtual ICollection<User> InverseParentUserU { get; set; } = new List<User>();

    public virtual ICollection<ManagerAiTradingActivationCode> ManagerAiTradingActivationCodes { get; set; } = new List<ManagerAiTradingActivationCode>();

    public virtual ICollection<ManagerOperationLog> ManagerOperationLogs { get; set; } = new List<ManagerOperationLog>();

    public virtual ICollection<ManagerTransferFromUserOrder> ManagerTransferFromUserOrders { get; set; } = new List<ManagerTransferFromUserOrder>();

    public virtual User? ParentUserU { get; set; }

    public virtual ICollection<UserAiTradingOrder> UserAiTradingOrders { get; set; } = new List<UserAiTradingOrder>();

    public virtual UserAsset? UserAsset { get; set; }

    public virtual ICollection<UserAssetsToWalletOrder> UserAssetsToWalletOrders { get; set; } = new List<UserAssetsToWalletOrder>();

    public virtual ICollection<UserChainTransaction> UserChainTransactions { get; set; } = new List<UserChainTransaction>();

    public virtual ICollection<UserInvitationRewardRecord> UserInvitationRewardRecordSubUserUs { get; set; } = new List<UserInvitationRewardRecord>();

    public virtual ICollection<UserInvitationRewardRecord> UserInvitationRewardRecordUidNavigations { get; set; } = new List<UserInvitationRewardRecord>();

    public virtual UserLevelConfig UserLevelNavigation { get; set; } = null!;

    public virtual ICollection<UserLoginLog> UserLoginLogs { get; set; } = new List<UserLoginLog>();

    public virtual ICollection<UserMiningRewardRecord> UserMiningRewardRecords { get; set; } = new List<UserMiningRewardRecord>();

    public virtual ICollection<UserOnChainAssetsChange> UserOnChainAssetsChanges { get; set; } = new List<UserOnChainAssetsChange>();

    public virtual ICollection<UserPathNode> UserPathNodeSubUserUs { get; set; } = new List<UserPathNode>();

    public virtual ICollection<UserPathNode> UserPathNodeUidNavigations { get; set; } = new List<UserPathNode>();

    public virtual ICollection<UserSysteamMessage> UserSysteamMessages { get; set; } = new List<UserSysteamMessage>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<WalletUserProfile> WalletUserProfiles { get; set; } = new List<WalletUserProfile>();
}

