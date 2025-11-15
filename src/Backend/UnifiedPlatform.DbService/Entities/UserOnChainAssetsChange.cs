using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class UserOnChainAssetsChange
{
    public int Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 变动类型
    /// </summary>
    public int ChangeType { get; set; }

    /// <summary>
    /// 变动金额
    /// </summary>
    public decimal Change { get; set; }

    /// <summary>
    /// 变动前金额
    /// </summary>
    public decimal Before { get; set; }

    /// <summary>
    /// 变动后金额
    /// </summary>
    public decimal After { get; set; }

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

    public virtual User UidNavigation { get; set; } = null!;
}

