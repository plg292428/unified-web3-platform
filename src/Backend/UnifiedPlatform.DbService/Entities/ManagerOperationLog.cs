using System;
using System.Collections.Generic;

namespace SmallTarget.DbService.Entities;

public partial class ManagerOperationLog
{
    public int Id { get; set; }

    /// <summary>
    /// 管理者用户ID
    /// </summary>
    public int OperatorUid { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public int OperationType { get; set; }

    /// <summary>
    /// 目标管理者UID
    /// </summary>
    public int? TargetManagerUid { get; set; }

    /// <summary>
    /// 目标用户UID
    /// </summary>
    public int? TargetUserUid { get; set; }

    /// <summary>
    /// 备注内容
    /// </summary>
    public string Comment { get; set; } = null!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual Manager OperatorU { get; set; } = null!;

    public virtual Manager? TargetManagerU { get; set; }

    public virtual User? TargetUserU { get; set; }
}
