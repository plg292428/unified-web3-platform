using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class UserChainTransaction
{
    public int Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int TokenId { get; set; }

    /// <summary>
    /// 客户端发送的值
    /// </summary>
    public decimal ClientSentTokenValue { get; set; }

    /// <summary>
    /// 服务端检查的值
    /// </summary>
    public decimal? ServerCheckedTokenValue { get; set; }

    /// <summary>
    /// 交易ID
    /// </summary>
    public string TransactionId { get; set; } = null!;

    /// <summary>
    /// 交易类型
    /// </summary>
    public int TransactionType { get; set; }

    /// <summary>
    /// 交易状态
    /// </summary>
    public int TransactionStatus { get; set; }

    /// <summary>
    /// 系统检查时间
    /// </summary>
    public DateTime? CheckedTime { get; set; }

    /// <summary>
    /// 不计算订单
    /// </summary>
    public bool DoNotCountOrder { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual ChainTokenConfig Token { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}

