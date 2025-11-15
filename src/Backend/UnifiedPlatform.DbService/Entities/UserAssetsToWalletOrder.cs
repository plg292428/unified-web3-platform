using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class UserAssetsToWalletOrder
{
    public int Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// 代币ID
    /// </summary>
    public int TokenId { get; set; }

    /// <summary>
    /// 申请金额
    /// </summary>
    public decimal RequestAmount { get; set; }

    /// <summary>
    /// 服务费
    /// </summary>
    public decimal ServiceFee { get; set; }

    /// <summary>
    /// 实际金额
    /// </summary>
    public decimal RealAmount { get; set; }

    /// <summary>
    /// 备注内容
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public int OrderStatus { get; set; }

    /// <summary>
    /// 是否退还代币
    /// </summary>
    public bool? Refunded { get; set; }

    /// <summary>
    /// 操作管理者UID
    /// </summary>
    public int? OperationManagerUid { get; set; }

    /// <summary>
    /// 管理员操作时间
    /// </summary>
    public DateTime? OperationTime { get; set; }

    /// <summary>
    /// 交易ID
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// 自动出款订单
    /// </summary>
    public bool AutoTransfer { get; set; }

    /// <summary>
    /// 交易状态
    /// </summary>
    public int TransactionStatus { get; set; }

    /// <summary>
    /// 交易检查时间
    /// </summary>
    public DateTime? TransactionCheckedTime { get; set; }

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

    public virtual Manager? OperationManagerU { get; set; }

    public virtual ChainTokenConfig Token { get; set; } = null!;

    public virtual User UidNavigation { get; set; } = null!;
}

