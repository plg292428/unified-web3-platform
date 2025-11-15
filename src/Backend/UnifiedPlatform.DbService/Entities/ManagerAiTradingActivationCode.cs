using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class ManagerAiTradingActivationCode
{
    /// <summary>
    /// 授权令牌
    /// </summary>
    public Guid ActivationCodeGuid { get; set; }

    public int GeneratorUid { get; set; }

    public int? UserUid { get; set; }

    /// <summary>
    /// 使用时间
    /// </summary>
    public DateTime? UseTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? ExpirationTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual Manager GeneratorU { get; set; } = null!;

    public virtual ICollection<UserSysteamMessage> UserSysteamMessages { get; set; } = new List<UserSysteamMessage>();

    public virtual User? UserU { get; set; }
}

