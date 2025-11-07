using System;
using System.Collections.Generic;

namespace SmallTarget.DbService.Entities;

public partial class ManagerLoginLog
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 客户端IP
    /// </summary>
    public string ClientIp { get; set; } = null!;

    /// <summary>
    /// 最后登录IP归属地
    /// </summary>
    public string? ClientIpRegion { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual Manager UidNavigation { get; set; } = null!;
}
