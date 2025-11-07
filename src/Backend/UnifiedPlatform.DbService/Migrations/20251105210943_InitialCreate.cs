using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnifiedPlatform.DbService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChainNetworkConfig",
                columns: table => new
                {
                    ChainId = table.Column<int>(type: "int", nullable: false, comment: "链ID"),
                    ChainIconPath = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "链图标路径"),
                    NetworkName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "网络名称"),
                    AbbrNetworkName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, comment: "缩写网络名称"),
                    Color = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false, defaultValueSql: "(N'#00000000')", comment: "前端主题颜色"),
                    CurrencyName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, comment: "主要通货名称"),
                    CurrencyDecimals = table.Column<byte>(type: "tinyint", nullable: false, comment: "通货小数精度"),
                    CurrencyIconPath = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "通货图标路径"),
                    RpcUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "RPC服务链接"),
                    RpcApiKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "RPC服务KEY"),
                    ClientGasFeeAlertValue = table.Column<decimal>(type: "decimal(9,6)", nullable: false, comment: "客户端Gas费提醒值"),
                    ServerGasFeeAlertValue = table.Column<decimal>(type: "decimal(9,6)", nullable: false, comment: "客户端Gas费提醒值"),
                    MinAssetsToChainLimit = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "最小充值限制"),
                    MaxAssetsToChaintLimit = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "最大充值限制"),
                    MinAssetsToWalletLimit = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "最小提币限制"),
                    MaxAssetsToWalletLimit = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "最大提币限制"),
                    AssetsToWalletServiceFeeBase = table.Column<decimal>(type: "decimal(5,2)", nullable: false, comment: "最小用户提币服务费"),
                    AssetsToWalletServiceFeeRate = table.Column<decimal>(type: "decimal(3,2)", nullable: false, comment: "用户提币服务费率"),
                    ManagerTransferFromServiceFee = table.Column<decimal>(type: "decimal(5,2)", nullable: false, comment: "管理者转移服务费"),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, comment: "是否启用"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainNetworkConfig_ChainId", x => x.ChainId);
                });

            migrationBuilder.CreateTable(
                name: "GlobalConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    DappUrls = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Dapp链接"),
                    ChainWalletConfigGroupId = table.Column<int>(type: "int", nullable: false, comment: "钱包配置组ID"),
                    OnlineCustomerServiceEnabled = table.Column<bool>(type: "bit", nullable: false, comment: "是否开启全局客服系统"),
                    OnlineCustomerServiceChatWootKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "ChatWoot客服系统Key"),
                    MiningRewardIntervalHours = table.Column<int>(type: "int", nullable: false, comment: "挖矿奖励间隔时间（小时）"),
                    MiningSpeedUpRequiredOnChainAssetsRate = table.Column<decimal>(type: "decimal(2,1)", nullable: false, comment: "加速挖矿需要的链上资产比例"),
                    MiningSpeedUpRewardIncreaseRate = table.Column<decimal>(type: "decimal(3,2)", nullable: false, comment: "加速挖矿提高的奖励率"),
                    MinAiTradingMinutes = table.Column<int>(type: "int", nullable: false, comment: "AI合约交易最低需要时间（分钟）"),
                    MaxAiTradingMinutes = table.Column<int>(type: "int", nullable: false, comment: "AI合约交易最高需要时间（分钟）"),
                    InvitedRewardRateLayer1 = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValueSql: "((0.000))", comment: "1层邀请奖励比例"),
                    InvitedRewardRateLayer2 = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValueSql: "((0.000))", comment: "2层邀请奖励比例"),
                    InvitedRewardRateLayer3 = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValueSql: "((0.000))", comment: "3层邀请奖励比例"),
                    VirtualUserGenerationRewardDays = table.Column<int>(type: "int", nullable: false, comment: "虚拟用户生成奖励天数"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalConfig_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManagerTypeConfig",
                columns: table => new
                {
                    ManagerType = table.Column<int>(type: "int", nullable: false, comment: "管理者类型"),
                    ManagerTypeDescription = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, comment: "管理者类型描述"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerTypeConfig_ManagerType", x => x.ManagerType);
                });

            migrationBuilder.CreateTable(
                name: "UserLevelConfig",
                columns: table => new
                {
                    UserLevel = table.Column<int>(type: "int", nullable: false, comment: "用户等级"),
                    UserLevelName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, comment: "用户等级名称"),
                    Color = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false, defaultValueSql: "(N'#00000000')", comment: "前端主题颜色"),
                    IconPath = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "图标路径"),
                    RequiresValidAsset = table.Column<decimal>(type: "decimal(11,2)", nullable: false, comment: "需要有效资产"),
                    DailyAiTradingLimitTimes = table.Column<int>(type: "int", nullable: false, comment: "每日可进行AI合约交易次数"),
                    AvailableAiTradingAssetsRate = table.Column<decimal>(type: "decimal(4,2)", nullable: false, comment: "可用于AI合约交易的资产比例"),
                    MinEachAiTradingRewardRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, comment: "每次AI合约交易最小奖励比例"),
                    MaxEachAiTradingRewardRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, comment: "每次AI合约交易最大奖励比例"),
                    MinEachMiningRewardRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, comment: "每次挖矿奖励最小比例"),
                    MaxEachMiningRewardRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, comment: "每次挖矿奖励最大比例"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLevelConfig_UserLevel", x => x.UserLevel);
                });

            migrationBuilder.CreateTable(
                name: "ChainTokenConfig",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "int", nullable: false, comment: "代币ID"),
                    ChainId = table.Column<int>(type: "int", nullable: false, comment: "链ID"),
                    TokenName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "代币名称"),
                    AbbrTokenName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "缩写代币名称"),
                    IconPath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "图标路径"),
                    ContractAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false, comment: "合约地址"),
                    ApproveAbiFunctionName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValueSql: "((1))", comment: "授权ABI方法名称"),
                    Decimals = table.Column<byte>(type: "tinyint", nullable: false, comment: "小数精度"),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, comment: "是否启用"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainTokenConfig_TokenId", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_ChainNetworkConfig_ChainTokenConfig",
                        column: x => x.ChainId,
                        principalTable: "ChainNetworkConfig",
                        principalColumn: "ChainId");
                });

            migrationBuilder.CreateTable(
                name: "ChainWalletConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false, comment: "组ID"),
                    ChainId = table.Column<int>(type: "int", nullable: false, comment: "链ID"),
                    SpenderWalletAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false, comment: "授权钱包地址"),
                    SpenderWalletPrivateKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "授权钱包私钥"),
                    PaymentWalletAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false, comment: "支付钱包地址"),
                    PaymentWalletPrivateKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "支付钱包私钥"),
                    ReceiveWalletAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false, comment: "接收钱包地址"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainWalletConfig_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChainNetworkConfig_ChainWalletConfig_ChainId",
                        column: x => x.ChainId,
                        principalTable: "ChainNetworkConfig",
                        principalColumn: "ChainId");
                });

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerType = table.Column<int>(type: "int", nullable: false, comment: "管理者类型"),
                    Username = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, comment: "用户名"),
                    Password = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "密码"),
                    AttributionGroupLeaderUid = table.Column<int>(type: "int", nullable: true, comment: "归属组长UID"),
                    AttributionAgentUid = table.Column<int>(type: "int", nullable: true, comment: "归属代理UID"),
                    OnlineCustomerServiceEnabled = table.Column<bool>(type: "bit", nullable: false, comment: "是否开启客服系统"),
                    OnlineCustomerServiceChatWootKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "ChatWoot客服系统Key"),
                    BalanceAssets = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "余额资产"),
                    LockingAssets = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "锁定资产"),
                    SignUpClientIp = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true, comment: "注册IP"),
                    SignUpClientIpRegion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "注册IP归属地"),
                    LastSignInClientIp = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true, comment: "最后登录IP"),
                    LastSignInClientIpRegion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "最后登录IP归属地"),
                    AccesTokenGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "授权令牌"),
                    OnlyDeveloperVisible = table.Column<bool>(type: "bit", nullable: false, comment: "是否仅开发者可见"),
                    Blocked = table.Column<bool>(type: "bit", nullable: false, comment: "是否已封停"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, comment: "是否已删除"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manager_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_ManagerTypeConfig_Manager",
                        column: x => x.ManagerType,
                        principalTable: "ManagerTypeConfig",
                        principalColumn: "ManagerType");
                    table.ForeignKey(
                        name: "FK_Manager_Manager_AttributionAgentUid",
                        column: x => x.AttributionAgentUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_Manager_Manager_AttributionGroupLeaderUid",
                        column: x => x.AttributionGroupLeaderUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "ManagerBalanceChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    ChangeType = table.Column<int>(type: "int", nullable: false, comment: "变动类型"),
                    Change = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "变动金额"),
                    Before = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "变动前金额"),
                    After = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "变动后金额"),
                    Comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "备注内容"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerUsdtChange_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manager_ManagerBalanceChange",
                        column: x => x.Uid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "ManagerLoginLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    ClientIp = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, comment: "客户端IP"),
                    ClientIpRegion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "最后登录IP归属地"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerLoginLog_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manager_ManagerLoginLog",
                        column: x => x.Uid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false, comment: "钱包地址"),
                    ChainId = table.Column<int>(type: "int", nullable: false, comment: "链ID"),
                    ChainWalletConfigGroupId = table.Column<int>(type: "int", nullable: false, comment: "区块链钱包配置组ID"),
                    UserLevel = table.Column<int>(type: "int", nullable: false, comment: "用户等级"),
                    Remark = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "备注标识"),
                    AttributionSalesmanUid = table.Column<int>(type: "int", nullable: true, comment: "归属业务员UID"),
                    AttributionGroupLeaderUid = table.Column<int>(type: "int", nullable: true, comment: "归属组长UID"),
                    AttributionAgentUid = table.Column<int>(type: "int", nullable: true, comment: "归属代理UID"),
                    ParentUserUid = table.Column<int>(type: "int", nullable: true, comment: "父级用户ID"),
                    ParentUsersPath = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "父级用户ID路径"),
                    ParentUserDepth = table.Column<int>(type: "int", nullable: false, comment: "父级用户深度"),
                    SignUpClientIp = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true, comment: "注册IP"),
                    SignUpClientIpRegion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "注册IP归属地"),
                    LastSignInClientIp = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true, comment: "最后登录IP"),
                    LastSignInClientIpRegion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "最后登录IP归属地"),
                    AccesTokenGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "授权令牌"),
                    VirtualUser = table.Column<bool>(type: "bit", nullable: false, comment: "是否为虚拟用户"),
                    Anomaly = table.Column<bool>(type: "bit", nullable: false, comment: "是否异常"),
                    Blocked = table.Column<bool>(type: "bit", nullable: false, comment: "是否封停"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, comment: "是否已逻辑删除"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_Manager_User_AttributionAgentUid",
                        column: x => x.AttributionAgentUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_Manager_User_AttributionGroupLeaderUid",
                        column: x => x.AttributionGroupLeaderUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_Manager_User_AttributionSalesmanUid",
                        column: x => x.AttributionSalesmanUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_UserLevelConfig_User",
                        column: x => x.UserLevel,
                        principalTable: "UserLevelConfig",
                        principalColumn: "UserLevel");
                    table.ForeignKey(
                        name: "FK_User_ChainNetworkConfig_ChainId",
                        column: x => x.ChainId,
                        principalTable: "ChainNetworkConfig",
                        principalColumn: "ChainId");
                    table.ForeignKey(
                        name: "FK_User_User",
                        column: x => x.ParentUserUid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "ManagerAiTradingActivationCode",
                columns: table => new
                {
                    ActivationCodeGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "授权令牌"),
                    GeneratorUid = table.Column<int>(type: "int", nullable: false),
                    UserUid = table.Column<int>(type: "int", nullable: true),
                    UseTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "使用时间"),
                    ExpirationTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "创建时间"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerAiTradingActivationCode_ActivationCodeGuid", x => x.ActivationCodeGuid);
                    table.ForeignKey(
                        name: "FK_Manager_ManagerAiTradingActivationCode",
                        column: x => x.GeneratorUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_User_ManagerAiTradingActivationCode",
                        column: x => x.UserUid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "ManagerOperationLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperatorUid = table.Column<int>(type: "int", nullable: false, comment: "管理者用户ID"),
                    OperationType = table.Column<int>(type: "int", nullable: false, comment: "操作类型"),
                    TargetManagerUid = table.Column<int>(type: "int", nullable: true, comment: "目标管理者UID"),
                    TargetUserUid = table.Column<int>(type: "int", nullable: true, comment: "目标用户UID"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "备注内容"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerOperationLog_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manager_ManagerOperationLog_OperatorUid",
                        column: x => x.OperatorUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_Manager_ManagerOperationLog_TragetManagerUid",
                        column: x => x.TargetManagerUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_User_ManagerOperationLog",
                        column: x => x.TargetUserUid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "ManagerTransferFromUserOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    TokenId = table.Column<int>(type: "int", nullable: false, comment: "代币ID"),
                    RequestTransferFromAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "请求转移代币"),
                    ServiceFee = table.Column<decimal>(type: "decimal(8,6)", nullable: false, comment: "转移服务费"),
                    RealTransferFromAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "实际转移代币"),
                    TransactionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "交易ID"),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false, comment: "交易状态"),
                    TransactionCheckedTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "交易检查时间"),
                    OperationManagerUid = table.Column<int>(type: "int", nullable: true, comment: "操作管理者UID"),
                    RechargeOnChainAssets = table.Column<bool>(type: "bit", nullable: false, comment: "是否充值链上资产"),
                    AutoTransferFrom = table.Column<bool>(type: "bit", nullable: false, comment: "是否为自动转移"),
                    FirstTransferFrom = table.Column<bool>(type: "bit", nullable: false, comment: "首次转移"),
                    DoNotCountOrder = table.Column<bool>(type: "bit", nullable: false, comment: "不计算订单"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerTransferFromUserOrder_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChainTokenConfig_ManagerTransferFromUserOrder",
                        column: x => x.TokenId,
                        principalTable: "ChainTokenConfig",
                        principalColumn: "TokenId");
                    table.ForeignKey(
                        name: "FK_Manager_ManagerTransferFromUserOrder",
                        column: x => x.OperationManagerUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_User_ManagerTransferFromUserOrder",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserAiTradingOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    Amount = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "交易金额"),
                    RewardRate = table.Column<decimal>(type: "decimal(5,4)", nullable: true, comment: "奖励比例"),
                    Reward = table.Column<decimal>(type: "decimal(19,6)", nullable: true, comment: "奖励金额"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "备注内容"),
                    OrderEndTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "订单结束时间"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAiTradingOrder_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserAiTradingOrder",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserAsset",
                columns: table => new
                {
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    CurrencyWalletBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "通货钱包余额"),
                    PrimaryTokenId = table.Column<int>(type: "int", nullable: false, comment: "主要代币ID"),
                    PrimaryTokenWalletBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "主要代币钱包余额"),
                    OnChainAssets = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "链上资产（平台余额）"),
                    LockingAssets = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "锁定中资产"),
                    BlackHoleAssets = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "黑洞资产"),
                    PeakEquityAssets = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "净资产峰值"),
                    PeakEquityAssetsUpdateTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "峰值净资产更新时间"),
                    Approved = table.Column<bool>(type: "bit", nullable: false, comment: "已授权"),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "已授权额度"),
                    FirstApprovedTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "首次授权时间"),
                    AiTradingActivated = table.Column<bool>(type: "bit", nullable: false, comment: "已激活AI合约交易功能"),
                    AiTradingRemainingTimes = table.Column<int>(type: "int", nullable: false, comment: "剩余AI交易次数"),
                    MiningActivityPoint = table.Column<int>(type: "int", nullable: false, comment: "挖矿活跃值"),
                    TotalToChain = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计转入链上（仅用于客户端统计）"),
                    TotalToWallet = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计转出钱包（仅用于客户端统计）"),
                    TotalAiTradingRewards = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计AI合约交易奖励（仅用于客户端统计）"),
                    TotalMiningRewards = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计挖矿奖励（仅用于客户端统计）"),
                    TotalInvitationRewards = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计邀请奖励（仅用于客户端统计）"),
                    TotalSystemRewards = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计系统奖励（仅用于客户端统计）"),
                    TotalTransferFrom = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计被转移（仅用于客户端统计）"),
                    TotalTransferFromRechargeToChain = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "合计转移后充值链上（仅用于客户端统计）"),
                    AutoTransferFromEnabled = table.Column<bool>(type: "bit", nullable: false, comment: "自动转移模式开关"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_ChainTokenConfig_UserAsset",
                        column: x => x.PrimaryTokenId,
                        principalTable: "ChainTokenConfig",
                        principalColumn: "TokenId");
                    table.ForeignKey(
                        name: "FK_User_UserAsset",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserAssetsToWalletOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    TokenId = table.Column<int>(type: "int", nullable: false, comment: "代币ID"),
                    RequestAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "申请金额"),
                    ServiceFee = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "服务费"),
                    RealAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "实际金额"),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "备注内容"),
                    OrderStatus = table.Column<int>(type: "int", nullable: false, comment: "订单状态"),
                    Refunded = table.Column<bool>(type: "bit", nullable: true, comment: "是否退还代币"),
                    OperationManagerUid = table.Column<int>(type: "int", nullable: true, comment: "操作管理者UID"),
                    OperationTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "管理员操作时间"),
                    TransactionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "交易ID"),
                    AutoTransfer = table.Column<bool>(type: "bit", nullable: false, comment: "自动出款订单"),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false, comment: "交易状态"),
                    TransactionCheckedTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "交易检查时间"),
                    DoNotCountOrder = table.Column<bool>(type: "bit", nullable: false, comment: "不计算订单"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssetsToWalletOrder_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChainTokenConfig_UserAssetsToWalletOrder",
                        column: x => x.TokenId,
                        principalTable: "ChainTokenConfig",
                        principalColumn: "TokenId");
                    table.ForeignKey(
                        name: "FK_Manager_UserAssetsToWalletOrder",
                        column: x => x.OperationManagerUid,
                        principalTable: "Manager",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_Use_UserAssetsToWalletOrder",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserChainTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    TokenId = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    ClientSentTokenValue = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "客户端发送的值"),
                    ServerCheckedTokenValue = table.Column<decimal>(type: "decimal(19,6)", nullable: true, comment: "服务端检查的值"),
                    TransactionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "交易ID"),
                    TransactionType = table.Column<int>(type: "int", nullable: false, comment: "交易类型"),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false, comment: "交易状态"),
                    CheckedTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "系统检查时间"),
                    DoNotCountOrder = table.Column<bool>(type: "bit", nullable: false, comment: "不计算订单"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChainTransaction_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChainTokenConfig_UserChainTransaction",
                        column: x => x.TokenId,
                        principalTable: "ChainTokenConfig",
                        principalColumn: "TokenId");
                    table.ForeignKey(
                        name: "FK_User_UserChainTransaction",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserInvitationRewardRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    SubUserUid = table.Column<int>(type: "int", nullable: false, comment: "下级用户ID"),
                    SubUserLayer = table.Column<int>(type: "int", nullable: false, comment: "下级用户层级"),
                    SubUserReward = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "下级用户奖励"),
                    SubUserRewardType = table.Column<int>(type: "int", nullable: false, comment: "奖励类型"),
                    RewardRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, comment: "奖励比例"),
                    Reward = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "奖励金额"),
                    Comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "备注内容"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInvitationRewardRecord_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserInvitationRewardRecord_SubUserUid",
                        column: x => x.SubUserUid,
                        principalTable: "User",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_User_UserInvitationRewardRecord_Uid",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserLoginLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    ClientIp = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, comment: "客户端IP"),
                    ClientIpRegion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "最后登录IP归属地"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginLog_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserLoginLog",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserMiningRewardRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false),
                    ValidAssets = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    RewardRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, comment: "奖励比例"),
                    Reward = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "奖励金额"),
                    SpeedUpMode = table.Column<bool>(type: "bit", nullable: false, comment: "加速模式"),
                    Comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "备注内容"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMiningRewardRecord_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserMiningRewardRecord",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserOnChainAssetsChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    ChangeType = table.Column<int>(type: "int", nullable: false, comment: "变动类型"),
                    Change = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "变动金额"),
                    Before = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "变动前金额"),
                    After = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "变动后金额"),
                    Comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "备注内容"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOnChainAssetsChange_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserOnChainAssetsChange",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserPathNode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    SubUserUid = table.Column<int>(type: "int", nullable: false, comment: "下级用户ID"),
                    SubUserLayer = table.Column<int>(type: "int", nullable: false, comment: "下级用户层级"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPathNode_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserPathNode_SubUserUid",
                        column: x => x.SubUserUid,
                        principalTable: "User",
                        principalColumn: "Uid");
                    table.ForeignKey(
                        name: "FK_User_UserPathNode_Uid",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "UserSysteamMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false),
                    MessageTitle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivationCodeGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActivationCodeMessage = table.Column<bool>(type: "bit", nullable: false, comment: "激活码消息"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, comment: "是否已读"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, comment: "是否已逻辑删除"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false, comment: "行版本号（EF CORE 自动维护）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSysteamMessage_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerAiTradingActivationCode_UserSysteamMessage",
                        column: x => x.ActivationCodeGuid,
                        principalTable: "ManagerAiTradingActivationCode",
                        principalColumn: "ActivationCodeGuid");
                    table.ForeignKey(
                        name: "FK_UserSysteamMessage_User",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChainTokenConfig_ChainId",
                table: "ChainTokenConfig",
                column: "ChainId");

            migrationBuilder.CreateIndex(
                name: "IX_ChainWalletConfig_ChainId",
                table: "ChainWalletConfig",
                column: "ChainId");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_AttributionAgentUid",
                table: "Manager",
                column: "AttributionAgentUid");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_AttributionGroupLeaderUid",
                table: "Manager",
                column: "AttributionGroupLeaderUid");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_ManagerType",
                table: "Manager",
                column: "ManagerType");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_Username",
                table: "Manager",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerAiTradingActivationCode_GeneratorUid",
                table: "ManagerAiTradingActivationCode",
                column: "GeneratorUid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerAiTradingActivationCode_UserUid",
                table: "ManagerAiTradingActivationCode",
                column: "UserUid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerBalanceChange_Uid",
                table: "ManagerBalanceChange",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerLoginLog_Uid",
                table: "ManagerLoginLog",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerOperationLog_OperatorUid",
                table: "ManagerOperationLog",
                column: "OperatorUid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerOperationLog_TargetManagerUid",
                table: "ManagerOperationLog",
                column: "TargetManagerUid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerOperationLog_TargetUserUid",
                table: "ManagerOperationLog",
                column: "TargetUserUid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTransferFromUserOrder_OperationManagerUid",
                table: "ManagerTransferFromUserOrder",
                column: "OperationManagerUid");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTransferFromUserOrder_TokenId",
                table: "ManagerTransferFromUserOrder",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTransferFromUserOrder_Uid",
                table: "ManagerTransferFromUserOrder",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_User_AttributionAgentUid",
                table: "User",
                column: "AttributionAgentUid");

            migrationBuilder.CreateIndex(
                name: "IX_User_AttributionGroupLeaderUid",
                table: "User",
                column: "AttributionGroupLeaderUid");

            migrationBuilder.CreateIndex(
                name: "IX_User_AttributionSalesmanUid",
                table: "User",
                column: "AttributionSalesmanUid");

            migrationBuilder.CreateIndex(
                name: "IX_User_ChainId",
                table: "User",
                column: "ChainId");

            migrationBuilder.CreateIndex(
                name: "IX_User_ParentUserUid",
                table: "User",
                column: "ParentUserUid");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserLevel",
                table: "User",
                column: "UserLevel");

            migrationBuilder.CreateIndex(
                name: "IX_UserAiTradingOrder_Uid",
                table: "UserAiTradingOrder",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserAsset_PrimaryTokenId",
                table: "UserAsset",
                column: "PrimaryTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssetsToWalletOrder_OperationManagerUid",
                table: "UserAssetsToWalletOrder",
                column: "OperationManagerUid");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssetsToWalletOrder_TokenId",
                table: "UserAssetsToWalletOrder",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssetsToWalletOrder_Uid",
                table: "UserAssetsToWalletOrder",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserChainTransaction_TokenId",
                table: "UserChainTransaction",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChainTransaction_TransactionId",
                table: "UserChainTransaction",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChainTransaction_Uid",
                table: "UserChainTransaction",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitationRewardRecord_SubUserUid",
                table: "UserInvitationRewardRecord",
                column: "SubUserUid");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitationRewardRecord_Uid",
                table: "UserInvitationRewardRecord",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginLog_Uid",
                table: "UserLoginLog",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserMiningRewardRecord_Uid",
                table: "UserMiningRewardRecord",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserOnChainAssetsChange_Uid",
                table: "UserOnChainAssetsChange",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserPathNode_SubUserUid",
                table: "UserPathNode",
                column: "SubUserUid");

            migrationBuilder.CreateIndex(
                name: "IX_UserPathNode_Uid",
                table: "UserPathNode",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_UserSysteamMessage_ActivationCodeGuid",
                table: "UserSysteamMessage",
                column: "ActivationCodeGuid");

            migrationBuilder.CreateIndex(
                name: "IX_UserSysteamMessage_Uid",
                table: "UserSysteamMessage",
                column: "Uid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChainWalletConfig");

            migrationBuilder.DropTable(
                name: "GlobalConfig");

            migrationBuilder.DropTable(
                name: "ManagerBalanceChange");

            migrationBuilder.DropTable(
                name: "ManagerLoginLog");

            migrationBuilder.DropTable(
                name: "ManagerOperationLog");

            migrationBuilder.DropTable(
                name: "ManagerTransferFromUserOrder");

            migrationBuilder.DropTable(
                name: "UserAiTradingOrder");

            migrationBuilder.DropTable(
                name: "UserAsset");

            migrationBuilder.DropTable(
                name: "UserAssetsToWalletOrder");

            migrationBuilder.DropTable(
                name: "UserChainTransaction");

            migrationBuilder.DropTable(
                name: "UserInvitationRewardRecord");

            migrationBuilder.DropTable(
                name: "UserLoginLog");

            migrationBuilder.DropTable(
                name: "UserMiningRewardRecord");

            migrationBuilder.DropTable(
                name: "UserOnChainAssetsChange");

            migrationBuilder.DropTable(
                name: "UserPathNode");

            migrationBuilder.DropTable(
                name: "UserSysteamMessage");

            migrationBuilder.DropTable(
                name: "ChainTokenConfig");

            migrationBuilder.DropTable(
                name: "ManagerAiTradingActivationCode");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.DropTable(
                name: "UserLevelConfig");

            migrationBuilder.DropTable(
                name: "ChainNetworkConfig");

            migrationBuilder.DropTable(
                name: "ManagerTypeConfig");
        }
    }
}
