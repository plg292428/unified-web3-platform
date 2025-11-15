using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class ManagerTransferFromUserOrder
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
    /// 请求转移代币
    /// </summary>
    public decimal RequestTransferFromAmount { get; set; }

    /// <summary>
    /// 转移服务费
    /// </summary>
    public decimal ServiceFee { get; set; }

    /// <summary>
    /// 实际转移代币
    /// </summary>
    public decimal RealTransferFromAmount { get; set; }

    /// <summary>
    /// 交易ID
    /// </summary>
    public string TransactionId { get; set; } = null!;

    /// <summary>
    /// 交易状态
    /// </summary>
    public int TransactionStatus { get; set; }

    /// <summary>
    /// 交易检查时间
    /// </summary>
    public DateTime? TransactionCheckedTime { get; set; }

    /// <summary>
    /// 操作管理者UID
    /// </summary>
    public int? OperationManagerUid { get; set; }

    /// <summary>
    /// 是否充值链上资产
    /// </summary>
    public bool RechargeOnChainAssets { get; set; }

    /// <summary>
    /// 是否为自动转移
    /// </summary>
    public bool AutoTransferFrom { get; set; }

    /// <summary>
    /// 首次转移
    /// </summary>
    public bool FirstTransferFrom { get; set; }

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

