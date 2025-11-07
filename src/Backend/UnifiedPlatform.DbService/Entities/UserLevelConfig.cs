using System;
using System.Collections.Generic;

namespace SmallTarget.DbService.Entities;

public partial class UserLevelConfig
{
    /// <summary>
    /// 用户等级
    /// </summary>
    public int UserLevel { get; set; }

    /// <summary>
    /// 用户等级名称
    /// </summary>
    public string UserLevelName { get; set; } = null!;

    /// <summary>
    /// 前端主题颜色
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// 图标路径
    /// </summary>
    public string IconPath { get; set; } = null!;

    /// <summary>
    /// 需要有效资产
    /// </summary>
    public decimal RequiresValidAsset { get; set; }

    /// <summary>
    /// 每日可进行AI合约交易次数
    /// </summary>
    public int DailyAiTradingLimitTimes { get; set; }

    /// <summary>
    /// 可用于AI合约交易的资产比例
    /// </summary>
    public decimal AvailableAiTradingAssetsRate { get; set; }

    /// <summary>
    /// 每次AI合约交易最小奖励比例
    /// </summary>
    public decimal MinEachAiTradingRewardRate { get; set; }

    /// <summary>
    /// 每次AI合约交易最大奖励比例
    /// </summary>
    public decimal MaxEachAiTradingRewardRate { get; set; }

    /// <summary>
    /// 每次挖矿奖励最小比例
    /// </summary>
    public decimal MinEachMiningRewardRate { get; set; }

    /// <summary>
    /// 每次挖矿奖励最大比例
    /// </summary>
    public decimal MaxEachMiningRewardRate { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
