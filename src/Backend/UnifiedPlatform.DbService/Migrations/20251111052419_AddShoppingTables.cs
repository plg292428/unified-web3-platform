using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnifiedPlatform.DbService.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false, comment: "分类ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "分类名称"),
                    Slug = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "分类别名"),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "分类描述"),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true, comment: "父级分类ID"),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "排序"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "是否启用"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory_CategoryId", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_ProductCategory_ProductCategory",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<long>(type: "bigint", nullable: false, comment: "商品ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false, comment: "分类ID"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "商品名称"),
                    Subtitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "副标题"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "商品描述"),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "缩略图"),
                    Price = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "销售价格"),
                    Currency = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, defaultValue: "USDT", comment: "币种"),
                    ChainId = table.Column<int>(type: "int", nullable: true, comment: "结算链ID"),
                    Sku = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "库存单位"),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, comment: "是否上架"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_ProductId", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_ProductCategory",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_Product_ChainNetworkConfig",
                        column: x => x.ChainId,
                        principalTable: "ChainNetworkConfig",
                        principalColumn: "ChainId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<long>(type: "bigint", nullable: false, comment: "订单ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, comment: "订单编号"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "订单金额"),
                    Currency = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, defaultValue: "USDT", comment: "币种"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "订单状态"),
                    PaymentMode = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "支付模式"),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "支付状态"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true, comment: "支付方式"),
                    PaymentProviderType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "支付渠道类型"),
                    PaymentProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "支付渠道名称"),
                    PaymentWalletAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "支付钱包地址"),
                    PaymentWalletLabel = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "钱包标签"),
                    ChainId = table.Column<int>(type: "int", nullable: true, comment: "链ID"),
                    PaymentTransactionHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "链上交易哈希"),
                    PaymentConfirmations = table.Column<int>(type: "int", nullable: true, comment: "确认数"),
                    PaymentSubmittedTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "提交支付时间"),
                    PaymentConfirmedTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "确认完成时间"),
                    PaymentSignaturePayload = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "签名原文"),
                    PaymentSignatureResult = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "签名结果"),
                    PaymentFailureReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "失败原因"),
                    PaymentExpiresAt = table.Column<DateTime>(type: "datetime", nullable: true, comment: "支付过期时间"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    PaidTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "支付时间"),
                    CancelTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "取消时间"),
                    CompleteTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "完成时间"),
                    Remark = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "备注")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_OrderId", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_ChainNetworkConfig",
                        column: x => x.ChainId,
                        principalTable: "ChainNetworkConfig",
                        principalColumn: "ChainId");
                    table.ForeignKey(
                        name: "FK_Orders_User",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "ProductInventory",
                columns: table => new
                {
                    InventoryId = table.Column<long>(type: "bigint", nullable: false, comment: "库存ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, comment: "商品ID"),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "可用库存"),
                    QuantityReserved = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "预留库存"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInventory_InventoryId", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_ProductInventory_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<long>(type: "bigint", nullable: false, comment: "订单商品ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false, comment: "订单ID"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, comment: "商品ID"),
                    ProductName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "商品名称"),
                    UnitPrice = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "单价"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "数量"),
                    Subtotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false, comment: "小计")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem_OrderItemId", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItem",
                columns: table => new
                {
                    CartItemId = table.Column<long>(type: "bigint", nullable: false, comment: "购物车项目ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, comment: "商品ID"),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "购买数量"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItem_CartItemId", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItem_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_ShoppingCartItem_User",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid");
                });

            migrationBuilder.CreateTable(
                name: "OrderPaymentLogs",
                columns: table => new
                {
                    OrderPaymentLogId = table.Column<long>(type: "bigint", nullable: false, comment: "支付日志ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false, comment: "订单ID"),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false, comment: "支付状态"),
                    EventType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, defaultValue: "status_change", comment: "事件类型"),
                    Message = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "记录描述"),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "原始数据"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "记录时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaymentLog_OrderPaymentLogId", x => x.OrderPaymentLogId);
                    table.ForeignKey(
                        name: "FK_OrderPaymentLog_Order",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletUserProfiles",
                columns: table => new
                {
                    WalletUserProfileId = table.Column<long>(type: "bigint", nullable: false, comment: "钱包档案ID")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uid = table.Column<int>(type: "int", nullable: false, comment: "用户ID"),
                    ProviderType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "钱包类型"),
                    ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "钱包名称"),
                    WalletAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "钱包地址"),
                    AddressLabel = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "地址标签"),
                    PreferredChainId = table.Column<int>(type: "int", nullable: true, comment: "首选链"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())", comment: "创建时间"),
                    LastUsedTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "最近使用时间"),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否主钱包")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletUserProfile_WalletUserProfileId", x => x.WalletUserProfileId);
                    table.ForeignKey(
                        name: "FK_WalletUserProfile_User",
                        column: x => x.Uid,
                        principalTable: "User",
                        principalColumn: "Uid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ChainId",
                table: "Product",
                column: "ChainId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_ParentCategoryId",
                table: "ProductCategory",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_Slug",
                table: "ProductCategory",
                column: "Slug",
                unique: true,
                filter: "([Slug] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductInventory_ProductId",
                table: "ProductInventory",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ChainId",
                table: "Orders",
                column: "ChainId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Uid",
                table: "Orders",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaymentLogs_OrderId",
                table: "OrderPaymentLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItem_ProductId",
                table: "ShoppingCartItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItem_Uid_ProductId",
                table: "ShoppingCartItem",
                columns: new[] { "Uid", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletUserProfiles_Uid",
                table: "WalletUserProfiles",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_WalletUserProfiles_Uid_ProviderType_WalletAddress",
                table: "WalletUserProfiles",
                columns: new[] { "Uid", "ProviderType", "WalletAddress" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderPaymentLogs");

            migrationBuilder.DropTable(
                name: "ProductInventory");

            migrationBuilder.DropTable(
                name: "ShoppingCartItem");

            migrationBuilder.DropTable(
                name: "WalletUserProfiles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductCategory");
        }
    }
}

