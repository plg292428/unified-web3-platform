using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class ManagerTypeConfig
{
    /// <summary>
    /// 管理者类型
    /// </summary>
    public int ManagerType { get; set; }

    /// <summary>
    /// 管理者类型描述
    /// </summary>
    public string ManagerTypeDescription { get; set; } = null!;

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();
}

