using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmallTarget.DbService.Entities;

public partial class StDbContext : DbContext
{
    public StDbContext(DbContextOptions<StDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChainNetworkConfig> ChainNetworkConfigs { get; set; }

    public virtual DbSet<ChainTokenConfig> ChainTokenConfigs { get; set; }

    public virtual DbSet<ChainWalletConfig> ChainWalletConfigs { get; set; }

    public virtual DbSet<GlobalConfig> GlobalConfigs { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<ManagerAiTradingActivationCode> ManagerAiTradingActivationCodes { get; set; }

    public virtual DbSet<ManagerBalanceChange> ManagerBalanceChanges { get; set; }

    public virtual DbSet<ManagerLoginLog> ManagerLoginLogs { get; set; }

    public virtual DbSet<ManagerOperationLog> ManagerOperationLogs { get; set; }

    public virtual DbSet<ManagerTransferFromUserOrder> ManagerTransferFromUserOrders { get; set; }

    public virtual DbSet<ManagerTypeConfig> ManagerTypeConfigs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAiTradingOrder> UserAiTradingOrders { get; set; }

    public virtual DbSet<UserAsset> UserAssets { get; set; }

    public virtual DbSet<UserAssetsToWalletOrder> UserAssetsToWalletOrders { get; set; }

    public virtual DbSet<UserChainTransaction> UserChainTransactions { get; set; }

    public virtual DbSet<UserInvitationRewardRecord> UserInvitationRewardRecords { get; set; }

    public virtual DbSet<UserLevelConfig> UserLevelConfigs { get; set; }

    public virtual DbSet<UserLoginLog> UserLoginLogs { get; set; }

    public virtual DbSet<UserMiningRewardRecord> UserMiningRewardRecords { get; set; }

    public virtual DbSet<UserOnChainAssetsChange> UserOnChainAssetsChanges { get; set; }

    public virtual DbSet<UserPathNode> UserPathNodes { get; set; }

    public virtual DbSet<UserSysteamMessage> UserSysteamMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChainNetworkConfig>(entity =>
        {
            entity.HasKey(e => e.ChainId).HasName("PK_ChainNetworkConfig_ChainId");

            entity.ToTable("ChainNetworkConfig");

            entity.Property(e => e.ChainId)
                .ValueGeneratedNever()
                .HasComment("链ID");
            entity.Property(e => e.AbbrNetworkName)
                .HasMaxLength(32)
                .HasComment("缩写网络名称");
            entity.Property(e => e.AssetsToWalletServiceFeeBase)
                .HasComment("最小用户提币服务费")
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.AssetsToWalletServiceFeeRate)
                .HasComment("用户提币服务费率")
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.ChainIconPath)
                .HasMaxLength(128)
                .HasComment("链图标路径");
            entity.Property(e => e.ClientGasFeeAlertValue)
                .HasComment("客户端Gas费提醒值")
                .HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Color)
                .HasMaxLength(9)
                .HasDefaultValueSql("(N'#00000000')")
                .HasComment("前端主题颜色");
            entity.Property(e => e.CurrencyDecimals).HasComment("通货小数精度");
            entity.Property(e => e.CurrencyIconPath)
                .HasMaxLength(128)
                .HasComment("通货图标路径");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(32)
                .HasComment("主要通货名称");
            entity.Property(e => e.Enabled).HasComment("是否启用");
            entity.Property(e => e.ManagerTransferFromServiceFee)
                .HasComment("管理者转移服务费")
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MaxAssetsToChaintLimit)
                .HasComment("最大充值限制")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.MaxAssetsToWalletLimit)
                .HasComment("最大提币限制")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.MinAssetsToChainLimit)
                .HasComment("最小充值限制")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.MinAssetsToWalletLimit)
                .HasComment("最小提币限制")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.NetworkName)
                .HasMaxLength(64)
                .HasComment("网络名称");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.RpcApiKey)
                .HasMaxLength(128)
                .HasComment("RPC服务KEY");
            entity.Property(e => e.RpcUrl)
                .HasMaxLength(128)
                .HasComment("RPC服务链接");
            entity.Property(e => e.ServerGasFeeAlertValue)
                .HasComment("客户端Gas费提醒值")
                .HasColumnType("decimal(9, 6)");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("更新时间")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<ChainTokenConfig>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK_ChainTokenConfig_TokenId");

            entity.ToTable("ChainTokenConfig");

            entity.Property(e => e.TokenId)
                .ValueGeneratedNever()
                .HasComment("代币ID");
            entity.Property(e => e.AbbrTokenName)
                .HasMaxLength(64)
                .HasComment("缩写代币名称");
            entity.Property(e => e.ApproveAbiFunctionName)
                .HasMaxLength(32)
                .HasDefaultValueSql("((1))")
                .HasComment("授权ABI方法名称");
            entity.Property(e => e.ChainId).HasComment("链ID");
            entity.Property(e => e.ContractAddress)
                .HasMaxLength(42)
                .HasComment("合约地址");
            entity.Property(e => e.Decimals).HasComment("小数精度");
            entity.Property(e => e.Enabled).HasComment("是否启用");
            entity.Property(e => e.IconPath)
                .HasMaxLength(256)
                .HasComment("图标路径");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.TokenName)
                .HasMaxLength(64)
                .HasComment("代币名称");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("更新时间")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Chain).WithMany(p => p.ChainTokenConfigs)
                .HasForeignKey(d => d.ChainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChainNetworkConfig_ChainTokenConfig");
        });

        modelBuilder.Entity<ChainWalletConfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ChainWalletConfig_Id");

            entity.ToTable("ChainWalletConfig");

            entity.Property(e => e.ChainId).HasComment("链ID");
            entity.Property(e => e.GroupId).HasComment("组ID");
            entity.Property(e => e.PaymentWalletAddress)
                .HasMaxLength(42)
                .HasComment("支付钱包地址");
            entity.Property(e => e.PaymentWalletPrivateKey)
                .HasMaxLength(128)
                .HasComment("支付钱包私钥");
            entity.Property(e => e.ReceiveWalletAddress)
                .HasMaxLength(42)
                .HasComment("接收钱包地址");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.SpenderWalletAddress)
                .HasMaxLength(42)
                .HasComment("授权钱包地址");
            entity.Property(e => e.SpenderWalletPrivateKey)
                .HasMaxLength(128)
                .HasComment("授权钱包私钥");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("更新时间")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Chain).WithMany(p => p.ChainWalletConfigs)
                .HasForeignKey(d => d.ChainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChainNetworkConfig_ChainWalletConfig_ChainId");
        });

        modelBuilder.Entity<GlobalConfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_GlobalConfig_Id");

            entity.ToTable("GlobalConfig");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ChainWalletConfigGroupId).HasComment("钱包配置组ID");
            entity.Property(e => e.DappUrls).HasComment("Dapp链接");
            entity.Property(e => e.InvitedRewardRateLayer1)
                .HasDefaultValueSql("((0.000))")
                .HasComment("1层邀请奖励比例")
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.InvitedRewardRateLayer2)
                .HasDefaultValueSql("((0.000))")
                .HasComment("2层邀请奖励比例")
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.InvitedRewardRateLayer3)
                .HasDefaultValueSql("((0.000))")
                .HasComment("3层邀请奖励比例")
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.MaxAiTradingMinutes).HasComment("AI合约交易最高需要时间（分钟）");
            entity.Property(e => e.MinAiTradingMinutes).HasComment("AI合约交易最低需要时间（分钟）");
            entity.Property(e => e.MiningRewardIntervalHours).HasComment("挖矿奖励间隔时间（小时）");
            entity.Property(e => e.MiningSpeedUpRequiredOnChainAssetsRate)
                .HasComment("加速挖矿需要的链上资产比例")
                .HasColumnType("decimal(2, 1)");
            entity.Property(e => e.MiningSpeedUpRewardIncreaseRate)
                .HasComment("加速挖矿提高的奖励率")
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.OnlineCustomerServiceChatWootKey)
                .HasMaxLength(256)
                .HasComment("ChatWoot客服系统Key");
            entity.Property(e => e.OnlineCustomerServiceEnabled).HasComment("是否开启全局客服系统");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("更新时间")
                .HasColumnType("datetime");
            entity.Property(e => e.VirtualUserGenerationRewardDays).HasComment("虚拟用户生成奖励天数");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK_Manager_Uid");

            entity.ToTable("Manager");

            entity.HasIndex(e => e.Username, "IX_Manager_Username").IsUnique();

            entity.Property(e => e.Uid).HasComment("用户ID");
            entity.Property(e => e.AccesTokenGuid).HasComment("授权令牌");
            entity.Property(e => e.AttributionAgentUid).HasComment("归属代理UID");
            entity.Property(e => e.AttributionGroupLeaderUid).HasComment("归属组长UID");
            entity.Property(e => e.BalanceAssets)
                .HasComment("余额资产")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.Blocked).HasComment("是否已封停");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Deleted).HasComment("是否已删除");
            entity.Property(e => e.LastSignInClientIp)
                .HasMaxLength(16)
                .HasComment("最后登录IP");
            entity.Property(e => e.LastSignInClientIpRegion)
                .HasMaxLength(256)
                .HasComment("最后登录IP归属地");
            entity.Property(e => e.LockingAssets)
                .HasComment("锁定资产")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.ManagerType).HasComment("管理者类型");
            entity.Property(e => e.OnlineCustomerServiceChatWootKey)
                .HasMaxLength(256)
                .HasComment("ChatWoot客服系统Key");
            entity.Property(e => e.OnlineCustomerServiceEnabled).HasComment("是否开启客服系统");
            entity.Property(e => e.OnlyDeveloperVisible).HasComment("是否仅开发者可见");
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .HasComment("密码");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.SignUpClientIp)
                .HasMaxLength(16)
                .HasComment("注册IP");
            entity.Property(e => e.SignUpClientIpRegion)
                .HasMaxLength(256)
                .HasComment("注册IP归属地");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasComment("用户名");

            entity.HasOne(d => d.AttributionAgentU).WithMany(p => p.InverseAttributionAgentU).HasForeignKey(d => d.AttributionAgentUid);

            entity.HasOne(d => d.AttributionGroupLeaderU).WithMany(p => p.InverseAttributionGroupLeaderU).HasForeignKey(d => d.AttributionGroupLeaderUid);

            entity.HasOne(d => d.ManagerTypeNavigation).WithMany(p => p.Managers)
                .HasForeignKey(d => d.ManagerType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ManagerTypeConfig_Manager");
        });

        modelBuilder.Entity<ManagerAiTradingActivationCode>(entity =>
        {
            entity.HasKey(e => e.ActivationCodeGuid).HasName("PK_ManagerAiTradingActivationCode_ActivationCodeGuid");

            entity.ToTable("ManagerAiTradingActivationCode");

            entity.Property(e => e.ActivationCodeGuid)
                .ValueGeneratedNever()
                .HasComment("授权令牌");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpirationTime)
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.UseTime)
                .HasComment("使用时间")
                .HasColumnType("datetime");

            entity.HasOne(d => d.GeneratorU).WithMany(p => p.ManagerAiTradingActivationCodes)
                .HasForeignKey(d => d.GeneratorUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manager_ManagerAiTradingActivationCode");

            entity.HasOne(d => d.UserU).WithMany(p => p.ManagerAiTradingActivationCodes)
                .HasForeignKey(d => d.UserUid)
                .HasConstraintName("FK_User_ManagerAiTradingActivationCode");
        });

        modelBuilder.Entity<ManagerBalanceChange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ManagerUsdtChange_Id");

            entity.ToTable("ManagerBalanceChange");

            entity.Property(e => e.After)
                .HasComment("变动后金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.Before)
                .HasComment("变动前金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.Change)
                .HasComment("变动金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.ChangeType).HasComment("变动类型");
            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasComment("备注内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.ManagerBalanceChanges)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manager_ManagerBalanceChange");
        });

        modelBuilder.Entity<ManagerLoginLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ManagerLoginLog_Id");

            entity.ToTable("ManagerLoginLog");

            entity.Property(e => e.Id).HasComment("ID");
            entity.Property(e => e.ClientIp)
                .HasMaxLength(16)
                .HasComment("客户端IP");
            entity.Property(e => e.ClientIpRegion)
                .HasMaxLength(256)
                .HasComment("最后登录IP归属地");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.ManagerLoginLogs)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manager_ManagerLoginLog");
        });

        modelBuilder.Entity<ManagerOperationLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ManagerOperationLog_Id");

            entity.ToTable("ManagerOperationLog");

            entity.Property(e => e.Comment).HasComment("备注内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.OperationType).HasComment("操作类型");
            entity.Property(e => e.OperatorUid).HasComment("管理者用户ID");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.TargetManagerUid).HasComment("目标管理者UID");
            entity.Property(e => e.TargetUserUid).HasComment("目标用户UID");

            entity.HasOne(d => d.OperatorU).WithMany(p => p.ManagerOperationLogOperatorUs)
                .HasForeignKey(d => d.OperatorUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manager_ManagerOperationLog_OperatorUid");

            entity.HasOne(d => d.TargetManagerU).WithMany(p => p.ManagerOperationLogTargetManagerUs)
                .HasForeignKey(d => d.TargetManagerUid)
                .HasConstraintName("FK_Manager_ManagerOperationLog_TragetManagerUid");

            entity.HasOne(d => d.TargetUserU).WithMany(p => p.ManagerOperationLogs)
                .HasForeignKey(d => d.TargetUserUid)
                .HasConstraintName("FK_User_ManagerOperationLog");
        });

        modelBuilder.Entity<ManagerTransferFromUserOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ManagerTransferFromUserOrder_Id");

            entity.ToTable("ManagerTransferFromUserOrder");

            entity.Property(e => e.AutoTransferFrom).HasComment("是否为自动转移");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.DoNotCountOrder).HasComment("不计算订单");
            entity.Property(e => e.FirstTransferFrom).HasComment("首次转移");
            entity.Property(e => e.OperationManagerUid).HasComment("操作管理者UID");
            entity.Property(e => e.RealTransferFromAmount)
                .HasComment("实际转移代币")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.RechargeOnChainAssets).HasComment("是否充值链上资产");
            entity.Property(e => e.RequestTransferFromAmount)
                .HasComment("请求转移代币")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.ServiceFee)
                .HasComment("转移服务费")
                .HasColumnType("decimal(8, 6)");
            entity.Property(e => e.TokenId).HasComment("代币ID");
            entity.Property(e => e.TransactionCheckedTime)
                .HasComment("交易检查时间")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(128)
                .HasComment("交易ID");
            entity.Property(e => e.TransactionStatus).HasComment("交易状态");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.OperationManagerU).WithMany(p => p.ManagerTransferFromUserOrders)
                .HasForeignKey(d => d.OperationManagerUid)
                .HasConstraintName("FK_Manager_ManagerTransferFromUserOrder");

            entity.HasOne(d => d.Token).WithMany(p => p.ManagerTransferFromUserOrders)
                .HasForeignKey(d => d.TokenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChainTokenConfig_ManagerTransferFromUserOrder");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.ManagerTransferFromUserOrders)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_ManagerTransferFromUserOrder");
        });

        modelBuilder.Entity<ManagerTypeConfig>(entity =>
        {
            entity.HasKey(e => e.ManagerType).HasName("PK_ManagerTypeConfig_ManagerType");

            entity.ToTable("ManagerTypeConfig");

            entity.Property(e => e.ManagerType)
                .ValueGeneratedNever()
                .HasComment("管理者类型");
            entity.Property(e => e.ManagerTypeDescription)
                .HasMaxLength(32)
                .HasComment("管理者类型描述");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK_User_Uid");

            entity.ToTable("User");

            entity.Property(e => e.Uid).HasComment("用户ID");
            entity.Property(e => e.AccesTokenGuid).HasComment("授权令牌");
            entity.Property(e => e.Anomaly).HasComment("是否异常");
            entity.Property(e => e.AttributionAgentUid).HasComment("归属代理UID");
            entity.Property(e => e.AttributionGroupLeaderUid).HasComment("归属组长UID");
            entity.Property(e => e.AttributionSalesmanUid).HasComment("归属业务员UID");
            entity.Property(e => e.Blocked).HasComment("是否封停");
            entity.Property(e => e.ChainId).HasComment("链ID");
            entity.Property(e => e.ChainWalletConfigGroupId).HasComment("区块链钱包配置组ID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Deleted).HasComment("是否已逻辑删除");
            entity.Property(e => e.LastSignInClientIp)
                .HasMaxLength(16)
                .HasComment("最后登录IP");
            entity.Property(e => e.LastSignInClientIpRegion)
                .HasMaxLength(256)
                .HasComment("最后登录IP归属地");
            entity.Property(e => e.ParentUserDepth).HasComment("父级用户深度");
            entity.Property(e => e.ParentUserUid).HasComment("父级用户ID");
            entity.Property(e => e.ParentUsersPath).HasComment("父级用户ID路径");
            entity.Property(e => e.Remark)
                .HasMaxLength(64)
                .HasComment("备注标识");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.SignUpClientIp)
                .HasMaxLength(16)
                .HasComment("注册IP");
            entity.Property(e => e.SignUpClientIpRegion)
                .HasMaxLength(256)
                .HasComment("注册IP归属地");
            entity.Property(e => e.UserLevel).HasComment("用户等级");
            entity.Property(e => e.VirtualUser).HasComment("是否为虚拟用户");
            entity.Property(e => e.WalletAddress)
                .HasMaxLength(42)
                .HasComment("钱包地址");

            entity.HasOne(d => d.AttributionAgentU).WithMany(p => p.UserAttributionAgentUs)
                .HasForeignKey(d => d.AttributionAgentUid)
                .HasConstraintName("FK_Manager_User_AttributionAgentUid");

            entity.HasOne(d => d.AttributionGroupLeaderU).WithMany(p => p.UserAttributionGroupLeaderUs)
                .HasForeignKey(d => d.AttributionGroupLeaderUid)
                .HasConstraintName("FK_Manager_User_AttributionGroupLeaderUid");

            entity.HasOne(d => d.AttributionSalesmanU).WithMany(p => p.UserAttributionSalesmanUs)
                .HasForeignKey(d => d.AttributionSalesmanUid)
                .HasConstraintName("FK_Manager_User_AttributionSalesmanUid");

            entity.HasOne(d => d.Chain).WithMany(p => p.Users)
                .HasForeignKey(d => d.ChainId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ParentUserU).WithMany(p => p.InverseParentUserU)
                .HasForeignKey(d => d.ParentUserUid)
                .HasConstraintName("FK_User_User");

            entity.HasOne(d => d.UserLevelNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserLevel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLevelConfig_User");
        });

        modelBuilder.Entity<UserAiTradingOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserAiTradingOrder_Id");

            entity.ToTable("UserAiTradingOrder");

            entity.Property(e => e.Amount)
                .HasComment("交易金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasComment("备注内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderEndTime)
                .HasComment("订单结束时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Reward)
                .HasComment("奖励金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.RewardRate)
                .HasComment("奖励比例")
                .HasColumnType("decimal(5, 4)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserAiTradingOrders)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserAiTradingOrder");
        });

        modelBuilder.Entity<UserAsset>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK_UserToken_Uid");

            entity.ToTable("UserAsset");

            entity.Property(e => e.Uid)
                .ValueGeneratedNever()
                .HasComment("用户ID");
            entity.Property(e => e.AiTradingActivated).HasComment("已激活AI合约交易功能");
            entity.Property(e => e.AiTradingRemainingTimes).HasComment("剩余AI交易次数");
            entity.Property(e => e.Approved).HasComment("已授权");
            entity.Property(e => e.ApprovedAmount)
                .HasComment("已授权额度")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.AutoTransferFromEnabled).HasComment("自动转移模式开关");
            entity.Property(e => e.BlackHoleAssets)
                .HasComment("黑洞资产")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyWalletBalance)
                .HasComment("通货钱包余额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.FirstApprovedTime)
                .HasComment("首次授权时间")
                .HasColumnType("datetime");
            entity.Property(e => e.LockingAssets)
                .HasComment("锁定中资产")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.MiningActivityPoint).HasComment("挖矿活跃值");
            entity.Property(e => e.OnChainAssets)
                .HasComment("链上资产（平台余额）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.PeakEquityAssets)
                .HasComment("净资产峰值")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.PeakEquityAssetsUpdateTime)
                .HasComment("峰值净资产更新时间")
                .HasColumnType("datetime");
            entity.Property(e => e.PrimaryTokenId).HasComment("主要代币ID");
            entity.Property(e => e.PrimaryTokenWalletBalance)
                .HasComment("主要代币钱包余额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.TotalAiTradingRewards)
                .HasComment("合计AI合约交易奖励（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TotalInvitationRewards)
                .HasComment("合计邀请奖励（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TotalMiningRewards)
                .HasComment("合计挖矿奖励（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TotalSystemRewards)
                .HasComment("合计系统奖励（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TotalToChain)
                .HasComment("合计转入链上（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TotalToWallet)
                .HasComment("合计转出钱包（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TotalTransferFrom)
                .HasComment("合计被转移（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TotalTransferFromRechargeToChain)
                .HasComment("合计转移后充值链上（仅用于客户端统计）")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("更新时间")
                .HasColumnType("datetime");

            entity.HasOne(d => d.PrimaryToken).WithMany(p => p.UserAssets)
                .HasForeignKey(d => d.PrimaryTokenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChainTokenConfig_UserAsset");

            entity.HasOne(d => d.UidNavigation).WithOne(p => p.UserAsset)
                .HasForeignKey<UserAsset>(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserAsset");
        });

        modelBuilder.Entity<UserAssetsToWalletOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserAssetsToWalletOrder_Id");

            entity.ToTable("UserAssetsToWalletOrder");

            entity.Property(e => e.AutoTransfer).HasComment("自动出款订单");
            entity.Property(e => e.Comment)
                .HasMaxLength(512)
                .HasComment("备注内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.DoNotCountOrder).HasComment("不计算订单");
            entity.Property(e => e.OperationManagerUid).HasComment("操作管理者UID");
            entity.Property(e => e.OperationTime)
                .HasComment("管理员操作时间")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderStatus).HasComment("订单状态");
            entity.Property(e => e.RealAmount)
                .HasComment("实际金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.Refunded).HasComment("是否退还代币");
            entity.Property(e => e.RequestAmount)
                .HasComment("申请金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.ServiceFee)
                .HasComment("服务费")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TokenId).HasComment("代币ID");
            entity.Property(e => e.TransactionCheckedTime)
                .HasComment("交易检查时间")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(128)
                .HasComment("交易ID");
            entity.Property(e => e.TransactionStatus).HasComment("交易状态");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.OperationManagerU).WithMany(p => p.UserAssetsToWalletOrders)
                .HasForeignKey(d => d.OperationManagerUid)
                .HasConstraintName("FK_Manager_UserAssetsToWalletOrder");

            entity.HasOne(d => d.Token).WithMany(p => p.UserAssetsToWalletOrders)
                .HasForeignKey(d => d.TokenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChainTokenConfig_UserAssetsToWalletOrder");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserAssetsToWalletOrders)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Use_UserAssetsToWalletOrder");
        });

        modelBuilder.Entity<UserChainTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserChainTransaction_Id");

            entity.ToTable("UserChainTransaction");

            entity.HasIndex(e => e.TransactionId, "IX_UserChainTransaction_TransactionId").IsUnique();

            entity.Property(e => e.CheckedTime)
                .HasComment("系统检查时间")
                .HasColumnType("datetime");
            entity.Property(e => e.ClientSentTokenValue)
                .HasComment("客户端发送的值")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.DoNotCountOrder).HasComment("不计算订单");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.ServerCheckedTokenValue)
                .HasComment("服务端检查的值")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.TokenId).HasComment("用户ID");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(128)
                .HasComment("交易ID");
            entity.Property(e => e.TransactionStatus).HasComment("交易状态");
            entity.Property(e => e.TransactionType).HasComment("交易类型");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.Token).WithMany(p => p.UserChainTransactions)
                .HasForeignKey(d => d.TokenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChainTokenConfig_UserChainTransaction");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserChainTransactions)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserChainTransaction");
        });

        modelBuilder.Entity<UserInvitationRewardRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserInvitationRewardRecord_Id");

            entity.ToTable("UserInvitationRewardRecord");

            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasComment("备注内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Reward)
                .HasComment("奖励金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.RewardRate)
                .HasComment("奖励比例")
                .HasColumnType("decimal(5, 4)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.SubUserLayer).HasComment("下级用户层级");
            entity.Property(e => e.SubUserReward)
                .HasComment("下级用户奖励")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.SubUserRewardType).HasComment("奖励类型");
            entity.Property(e => e.SubUserUid).HasComment("下级用户ID");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.SubUserU).WithMany(p => p.UserInvitationRewardRecordSubUserUs)
                .HasForeignKey(d => d.SubUserUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserInvitationRewardRecord_SubUserUid");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserInvitationRewardRecordUidNavigations)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserInvitationRewardRecord_Uid");
        });

        modelBuilder.Entity<UserLevelConfig>(entity =>
        {
            entity.HasKey(e => e.UserLevel).HasName("PK_UserLevelConfig_UserLevel");

            entity.ToTable("UserLevelConfig");

            entity.Property(e => e.UserLevel)
                .ValueGeneratedNever()
                .HasComment("用户等级");
            entity.Property(e => e.AvailableAiTradingAssetsRate)
                .HasComment("可用于AI合约交易的资产比例")
                .HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Color)
                .HasMaxLength(9)
                .HasDefaultValueSql("(N'#00000000')")
                .HasComment("前端主题颜色");
            entity.Property(e => e.DailyAiTradingLimitTimes).HasComment("每日可进行AI合约交易次数");
            entity.Property(e => e.IconPath)
                .HasMaxLength(128)
                .HasComment("图标路径");
            entity.Property(e => e.MaxEachAiTradingRewardRate)
                .HasComment("每次AI合约交易最大奖励比例")
                .HasColumnType("decimal(5, 4)");
            entity.Property(e => e.MaxEachMiningRewardRate)
                .HasComment("每次挖矿奖励最大比例")
                .HasColumnType("decimal(5, 4)");
            entity.Property(e => e.MinEachAiTradingRewardRate)
                .HasComment("每次AI合约交易最小奖励比例")
                .HasColumnType("decimal(5, 4)");
            entity.Property(e => e.MinEachMiningRewardRate)
                .HasComment("每次挖矿奖励最小比例")
                .HasColumnType("decimal(5, 4)");
            entity.Property(e => e.RequiresValidAsset)
                .HasComment("需要有效资产")
                .HasColumnType("decimal(11, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("更新时间")
                .HasColumnType("datetime");
            entity.Property(e => e.UserLevelName)
                .HasMaxLength(32)
                .HasComment("用户等级名称");
        });

        modelBuilder.Entity<UserLoginLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserLoginLog_Id");

            entity.ToTable("UserLoginLog");

            entity.Property(e => e.Id).HasComment("ID");
            entity.Property(e => e.ClientIp)
                .HasMaxLength(16)
                .HasComment("客户端IP");
            entity.Property(e => e.ClientIpRegion)
                .HasMaxLength(256)
                .HasComment("最后登录IP归属地");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserLoginLogs)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserLoginLog");
        });

        modelBuilder.Entity<UserMiningRewardRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserMiningRewardRecord_Id");

            entity.ToTable("UserMiningRewardRecord");

            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasComment("备注内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Reward)
                .HasComment("奖励金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.RewardRate)
                .HasComment("奖励比例")
                .HasColumnType("decimal(5, 4)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.SpeedUpMode).HasComment("加速模式");
            entity.Property(e => e.ValidAssets).HasColumnType("decimal(19, 6)");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserMiningRewardRecords)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserMiningRewardRecord");
        });

        modelBuilder.Entity<UserOnChainAssetsChange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserOnChainAssetsChange_Id");

            entity.ToTable("UserOnChainAssetsChange");

            entity.Property(e => e.After)
                .HasComment("变动后金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.Before)
                .HasComment("变动前金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.Change)
                .HasComment("变动金额")
                .HasColumnType("decimal(19, 6)");
            entity.Property(e => e.ChangeType).HasComment("变动类型");
            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasComment("备注内容");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserOnChainAssetsChanges)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserOnChainAssetsChange");
        });

        modelBuilder.Entity<UserPathNode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserPathNode_Id");

            entity.ToTable("UserPathNode");

            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");
            entity.Property(e => e.SubUserLayer).HasComment("下级用户层级");
            entity.Property(e => e.SubUserUid).HasComment("下级用户ID");
            entity.Property(e => e.Uid).HasComment("用户ID");

            entity.HasOne(d => d.SubUserU).WithMany(p => p.UserPathNodeSubUserUs)
                .HasForeignKey(d => d.SubUserUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserPathNode_SubUserUid");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserPathNodeUidNavigations)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserPathNode_Uid");
        });

        modelBuilder.Entity<UserSysteamMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserSysteamMessage_Id");

            entity.ToTable("UserSysteamMessage");

            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("创建时间")
                .HasColumnType("datetime");
            entity.Property(e => e.Deleted).HasComment("是否已逻辑删除");
            entity.Property(e => e.IsActivationCodeMessage).HasComment("激活码消息");
            entity.Property(e => e.IsRead).HasComment("是否已读");
            entity.Property(e => e.MessageTitle).HasMaxLength(128);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasComment("行版本号（EF CORE 自动维护）");

            entity.HasOne(d => d.ActivationCode).WithMany(p => p.UserSysteamMessages)
                .HasForeignKey(d => d.ActivationCodeGuid)
                .HasConstraintName("FK_ManagerAiTradingActivationCode_UserSysteamMessage");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.UserSysteamMessages)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSysteamMessage_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
