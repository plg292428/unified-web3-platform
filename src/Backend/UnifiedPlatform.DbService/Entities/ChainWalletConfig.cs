using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities;

public partial class ChainWalletConfig
{
    public int Id { get; set; }

    /// <summary>
    /// 组ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 链ID
    /// </summary>
    public int ChainId { get; set; }

    /// <summary>
    /// 授权钱包地址
    /// </summary>
    public string SpenderWalletAddress { get; set; } = null!;

    /// <summary>
    /// 授权钱包私钥
    /// </summary>
    public string SpenderWalletPrivateKey { get; set; } = null!;

    /// <summary>
    /// 支付钱包地址
    /// </summary>
    public string PaymentWalletAddress { get; set; } = null!;

    /// <summary>
    /// 支付钱包私钥
    /// </summary>
    public string PaymentWalletPrivateKey { get; set; } = null!;

    /// <summary>
    /// 接收钱包地址
    /// </summary>
    public string ReceiveWalletAddress { get; set; } = null!;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 行版本号（EF CORE 自动维护）
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;

    public virtual ChainNetworkConfig Chain { get; set; } = null!;
}

