using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class UserAiTradingOrder
{
    public int Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 交易金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 奖励比例
    /// </summary>
    public decimal? RewardRate { get; set; }

    /// <summary>
    /// 奖励金额
    /// </summary>
    public decimal? Reward { get; set; }

    public int Status { get; set; }

    /// <summary>
    /// 备注内容
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 订单结束时间
    /// </summary>
    public DateTime OrderEndTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}

