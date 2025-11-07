using System;
using System.Collections.Generic;

namespace SmallTarget.DbService.Entities;

public partial class UserPathNode
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
