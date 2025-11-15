using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class UserInvitationRewardRecord
{
    public int Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 下级用户ID
    /// </summary>
    public int SubUserUid { get; set; }

    /// <summary>
    /// 下级用户层级
    /// </summary>
    public int SubUserLayer { get; set; }

    /// <summary>
    /// 下级用户奖励
    /// </summary>
    public decimal SubUserReward { get; set; }

    /// <summary>
    /// 奖励类型
    /// </summary>
    public int SubUserRewardType { get; set; }

    /// <summary>
    /// 奖励比例
    /// </summary>
    public decimal RewardRate { get; set; }

    /// <summary>
    /// 奖励金额
    /// </summary>
    public decimal Reward { get; set; }

    /// <summary>
    /// 备注内容
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual User SubUserU { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}

