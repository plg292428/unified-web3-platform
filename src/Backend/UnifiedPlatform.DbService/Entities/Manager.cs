using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class Manager
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 管理者类型
    /// </summary>
    public int ManagerType { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 归属组长UID
    /// </summary>
    public int? AttributionGroupLeaderUid { get; set; }

    /// <summary>
    /// 归属代理UID
    /// </summary>
    public int? AttributionAgentUid { get; set; }

    /// <summary>
    /// 是否开启客服系统
    /// </summary>
    public bool OnlineCustomerServiceEnabled { get; set; }

    /// <summary>
    /// ChatWoot客服系统Key
    /// </summary>
    public string? OnlineCustomerServiceChatWootKey { get; set; }

    /// <summary>
    /// 余额资产
    /// </summary>
    public decimal BalanceAssets { get; set; }

    /// <summary>
    /// 锁定资产
    /// </summary>
    public decimal LockingAssets { get; set; }

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
    /// 是否仅开发者可见
    /// </summary>
    public bool OnlyDeveloperVisible { get; set; }

    /// <summary>
    /// 是否已封停
    /// </summary>
    public bool Blocked { get; set; }

    /// <summary>
    /// 是否已删除
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

    public virtual ICollection<Manager> InverseAttributionAgentU { get; set; } = new List<Manager>();

    public virtual ICollection<Manager> InverseAttributionGroupLeaderU { get; set; } = new List<Manager>();

    public virtual ICollection<ManagerAiTradingActivationCode> ManagerAiTradingActivationCodes { get; set; } = new List<ManagerAiTradingActivationCode>();

    public virtual ICollection<ManagerBalanceChange> ManagerBalanceChanges { get; set; } = new List<ManagerBalanceChange>();

    public virtual ICollection<ManagerLoginLog> ManagerLoginLogs { get; set; } = new List<ManagerLoginLog>();

    public virtual ICollection<ManagerOperationLog> ManagerOperationLogOperatorUs { get; set; } = new List<ManagerOperationLog>();

    public virtual ICollection<ManagerOperationLog> ManagerOperationLogTargetManagerUs { get; set; } = new List<ManagerOperationLog>();

    public virtual ICollection<ManagerTransferFromUserOrder> ManagerTransferFromUserOrders { get; set; } = new List<ManagerTransferFromUserOrder>();

    public virtual ManagerTypeConfig ManagerTypeNavigation { get; set; } = null!;

    public virtual ICollection<UserAssetsToWalletOrder> UserAssetsToWalletOrders { get; set; } = new List<UserAssetsToWalletOrder>();

    public virtual ICollection<User> UserAttributionAgentUs { get; set; } = new List<User>();

    public virtual ICollection<User> UserAttributionGroupLeaderUs { get; set; } = new List<User>();

    public virtual ICollection<User> UserAttributionSalesmanUs { get; set; } = new List<User>();
}

