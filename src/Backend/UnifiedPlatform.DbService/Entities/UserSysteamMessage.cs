using System;
using System.Collections.Generic;

namespace SmallTarget.DbService.Entities;

public partial class UserSysteamMessage
{
    public int Id { get; set; }

    public int Uid { get; set; }

    public string MessageTitle { get; set; } = null!;

    public string MessageContent { get; set; } = null!;

    public Guid? ActivationCodeGuid { get; set; }

    /// <summary>
    /// 激活码消息
    /// </summary>
    public bool IsActivationCodeMessage { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; set; }

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

    public virtual ManagerAiTradingActivationCode? ActivationCode { get; set; }

    public virtual User UidNavigation { get; set; } = null!;
}
