using System;
using System.Collections.Generic;

namespace SmallTarget.DbService.Entities;

public partial class GlobalConfig
{
    public int Id { get; set; }

    /// <summary>
    /// Dapp链接
    /// </summary>
    public string DappUrls { get; set; } = null!;

    /// <summary>
    /// 钱包配置组ID
    /// </summary>
    public int ChainWalletConfigGroupId { get; set; }

    /// <summary>
    /// 是否开启全局客服系统
    /// </summary>
    public bool OnlineCustomerServiceEnabled { get; set; }

    /// <summary>
    /// ChatWoot客服系统Key
    /// </summary>
    public string? OnlineCustomerServiceChatWootKey { get; set; }

    /// <summary>
    /// 挖矿奖励间隔时间（小时）
    /// </summary>
    public int MiningRewardIntervalHours { get; set; }

    /// <summary>
    /// 加速挖矿需要的链上资产比例
    /// </summary>
    public decimal MiningSpeedUpRequiredOnChainAssetsRate { get; set; }

    /// <summary>
    /// 加速挖矿提高的奖励率
    /// </summary>
    public decimal MiningSpeedUpRewardIncreaseRate { get; set; }

    /// <summary>
    /// AI合约交易最低需要时间（分钟）
    /// </summary>
    public int MinAiTradingMinutes { get; set; }

    /// <summary>
    /// AI合约交易最高需要时间（分钟）
    /// </summary>
    public int MaxAiTradingMinutes { get; set; }

    /// <summary>
    /// 1层邀请奖励比例
    /// </summary>
    public decimal InvitedRewardRateLayer1 { get; set; }

    /// <summary>
    /// 2层邀请奖励比例
    /// </summary>
    public decimal InvitedRewardRateLayer2 { get; set; }

    /// <summary>
    /// 3层邀请奖励比例
    /// </summary>
    public decimal InvitedRewardRateLayer3 { get; set; }

    /// <summary>
    /// 虚拟用户生成奖励天数
    /// </summary>
    public int VirtualUserGenerationRewardDays { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;
}
