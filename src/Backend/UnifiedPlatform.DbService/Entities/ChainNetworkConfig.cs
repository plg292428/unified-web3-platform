using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class ChainNetworkConfig
{
    /// <summary>
    /// 链ID
    /// </summary>
    public int ChainId { get; set; }

    /// <summary>
    /// 链图标路径
    /// </summary>
    public string ChainIconPath { get; set; } = null!;

    /// <summary>
    /// 网络名称
    /// </summary>
    public string NetworkName { get; set; } = null!;

    /// <summary>
    /// 缩写网络名称
    /// </summary>
    public string AbbrNetworkName { get; set; } = null!;

    /// <summary>
    /// 前端主题颜色
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// 主要通货名称
    /// </summary>
    public string CurrencyName { get; set; } = null!;

    /// <summary>
    /// 通货小数精度
    /// </summary>
    public byte CurrencyDecimals { get; set; }

    /// <summary>
    /// 通货图标路径
    /// </summary>
    public string CurrencyIconPath { get; set; } = null!;

    /// <summary>
    /// RPC服务链接
    /// </summary>
    public string RpcUrl { get; set; } = null!;

    /// <summary>
    /// RPC服务KEY
    /// </summary>
    public string? RpcApiKey { get; set; }

    /// <summary>
    /// 客户端Gas费提醒值
    /// </summary>
    public decimal ClientGasFeeAlertValue { get; set; }

    /// <summary>
    /// 客户端Gas费提醒值
    /// </summary>
    public decimal ServerGasFeeAlertValue { get; set; }

    /// <summary>
    /// 最小充值限制
    /// </summary>
    public decimal MinAssetsToChainLimit { get; set; }

    /// <summary>
    /// 最大充值限制
    /// </summary>
    public decimal MaxAssetsToChaintLimit { get; set; }

    /// <summary>
    /// 最小提币限制
    /// </summary>
    public decimal MinAssetsToWalletLimit { get; set; }

    /// <summary>
    /// 最大提币限制
    /// </summary>
    public decimal MaxAssetsToWalletLimit { get; set; }

    /// <summary>
    /// 最小用户提币服务费
    /// </summary>
    public decimal AssetsToWalletServiceFeeBase { get; set; }

    /// <summary>
    /// 用户提币服务费率
    /// </summary>
    public decimal AssetsToWalletServiceFeeRate { get; set; }

    /// <summary>
    /// 管理者转移服务费
    /// </summary>
    public decimal ManagerTransferFromServiceFee { get; set; }

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

    public virtual ICollection<ChainTokenConfig> ChainTokenConfigs { get; set; } = new List<ChainTokenConfig>();

    public virtual ICollection<ChainWalletConfig> ChainWalletConfigs { get; set; } = new List<ChainWalletConfig>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

