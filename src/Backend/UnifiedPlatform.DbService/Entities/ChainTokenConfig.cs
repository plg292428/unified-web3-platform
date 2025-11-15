using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class ChainTokenConfig
{
    /// <summary>
    /// 代币ID
    /// </summary>
    public int TokenId { get; set; }

    /// <summary>
    /// 链ID
    /// </summary>
    public int ChainId { get; set; }

    /// <summary>
    /// 代币名称
    /// </summary>
    public string TokenName { get; set; } = null!;

    /// <summary>
    /// 缩写代币名称
    /// </summary>
    public string AbbrTokenName { get; set; } = null!;

    /// <summary>
    /// 图标路径
    /// </summary>
    public string IconPath { get; set; } = null!;

    /// <summary>
    /// 合约地址
    /// </summary>
    public string ContractAddress { get; set; } = null!;

    /// <summary>
    /// 授权ABI方法名称
    /// </summary>
    public string ApproveAbiFunctionName { get; set; } = null!;

    /// <summary>
    /// 小数精度
    /// </summary>
    public byte Decimals { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual ChainNetworkConfig Chain { get; set; } = null!;

    public virtual ICollection<ManagerTransferFromUserOrder> ManagerTransferFromUserOrders { get; set; } = new List<ManagerTransferFromUserOrder>();

    public virtual ICollection<UserAsset> UserAssets { get; set; } = new List<UserAsset>();

    public virtual ICollection<UserAssetsToWalletOrder> UserAssetsToWalletOrders { get; set; } = new List<UserAssetsToWalletOrder>();

    public virtual ICollection<UserChainTransaction> UserChainTransactions { get; set; } = new List<UserChainTransaction>();
}

