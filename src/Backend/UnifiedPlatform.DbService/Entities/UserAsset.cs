using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class UserAsset
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 通货钱包余额
    /// </summary>
    public decimal CurrencyWalletBalance { get; set; }

    /// <summary>
    /// 主要代币ID
    /// </summary>
    public int PrimaryTokenId { get; set; }

    /// <summary>
    /// 主要代币钱包余额
    /// </summary>
    public decimal PrimaryTokenWalletBalance { get; set; }

    /// <summary>
    /// 链上资产（平台余额）
    /// </summary>
    public decimal OnChainAssets { get; set; }

    /// <summary>
    /// 锁定中资产
    /// </summary>
    public decimal LockingAssets { get; set; }

    /// <summary>
    /// 黑洞资产
    /// </summary>
    public decimal BlackHoleAssets { get; set; }

    /// <summary>
    /// 净资产峰值
    /// </summary>
    public decimal PeakEquityAssets { get; set; }

    /// <summary>
    /// 峰值净资产更新时间
    /// </summary>
    public DateTime? PeakEquityAssetsUpdateTime { get; set; }

    /// <summary>
    /// 已授权
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// 已授权额度
    /// </summary>
    public decimal ApprovedAmount { get; set; }

    /// <summary>
    /// 首次授权时间
    /// </summary>
    public DateTime? FirstApprovedTime { get; set; }

    /// <summary>
    /// 已激活AI合约交易功能
    /// </summary>
    public bool AiTradingActivated { get; set; }

    /// <summary>
    /// 剩余AI交易次数
    /// </summary>
    public int AiTradingRemainingTimes { get; set; }

    /// <summary>
    /// 挖矿活跃值
    /// </summary>
    public int MiningActivityPoint { get; set; }

    /// <summary>
    /// 合计转入链上（仅用于客户端统计）
    /// </summary>
    public decimal TotalToChain { get; set; }

    /// <summary>
    /// 合计转出钱包（仅用于客户端统计）
    /// </summary>
    public decimal TotalToWallet { get; set; }

    /// <summary>
    /// 合计AI合约交易奖励（仅用于客户端统计）
    /// </summary>
    public decimal TotalAiTradingRewards { get; set; }

    /// <summary>
    /// 合计挖矿奖励（仅用于客户端统计）
    /// </summary>
    public decimal TotalMiningRewards { get; set; }

    /// <summary>
    /// 合计邀请奖励（仅用于客户端统计）
    /// </summary>
    public decimal TotalInvitationRewards { get; set; }

    /// <summary>
    /// 合计系统奖励（仅用于客户端统计）
    /// </summary>
    public decimal TotalSystemRewards { get; set; }

    /// <summary>
    /// 合计被转移（仅用于客户端统计）
    /// </summary>
    public decimal TotalTransferFrom { get; set; }

    /// <summary>
    /// 合计转移后充值链上（仅用于客户端统计）
    /// </summary>
    public decimal TotalTransferFromRechargeToChain { get; set; }

    /// <summary>
    /// 自动转移模式开关
    /// </summary>
    public bool AutoTransferFromEnabled { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual ChainTokenConfig PrimaryToken { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}

