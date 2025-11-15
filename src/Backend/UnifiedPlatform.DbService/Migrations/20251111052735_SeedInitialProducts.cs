using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnifiedPlatform.DbService.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductCategory",
                columns: new[] { "CategoryId", "Name", "Slug", "Description", "ParentCategoryId", "SortOrder", "IsActive" },
                values: new object[,]
                {
                    { 1, "硬件设备", "hardware", "官方认证的 Web3 设备", null, 1, true },
                    { 2, "数字藏品", "digital-collectibles", "限量发行的 NFT 藏品", null, 2, true },
                    { 3, "增值服务", "value-services", "链上增值服务", null, 3, true }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[]
                {
                    "ProductId", "CategoryId", "Name", "Subtitle", "Description", "ThumbnailUrl", "Price", "Currency",
                    "ChainId", "Sku", "IsPublished"
                },
                values: new object[,]
                {
                    { 1L, 1, "Bitget Web3 Vault", "冷存储签名器", "专为 Bitget 用户打造的硬件 Vault", null, 399m, "USDT", 1, "BG-VLT-001", true },
                    { 2L, 2, "Bitget Genesis NFT", "限量 Genesis 系列", "Bitget 生态限量 Genesis NFT", null, 129m, "USDT", 137, "BG-NFT-001", true },
                    { 3L, 3, "链上风控加速", "链上风控增强服务", "快速提升企业链上风控能力", null, 259m, "USDT", 56, "BG-SRV-001", true }
                });

            migrationBuilder.InsertData(
                table: "ProductInventory",
                columns: new[] { "InventoryId", "ProductId", "QuantityAvailable", "QuantityReserved" },
                values: new object[,]
                {
                    { 1L, 1L, 25, 0 },
                    { 2L, 2L, 80, 0 },
                    { 3L, 3L, 120, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductInventory",
                keyColumn: "InventoryId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "ProductInventory",
                keyColumn: "InventoryId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "ProductInventory",
                keyColumn: "InventoryId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "ProductCategory",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductCategory",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductCategory",
                keyColumn: "CategoryId",
                keyValue: 3);
        }
    }
}

