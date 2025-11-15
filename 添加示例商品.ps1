# 添加示例商品到数据库的 PowerShell 脚本
# 通过调用后端 API 或直接操作数据库

$ErrorActionPreference = "Stop"

Write-Host "`n添加示例商品到数据库..." -ForegroundColor Cyan

# 数据库连接字符串（从 appsettings.json 读取）
$connectionString = "Server=localhost;Database=UnifiedWeb3Platform;User Id=UnifiedWebAppUser;Password=P@ssword_2025!;MultipleActiveResultSets=true;TrustServerCertificate=true"

# SQL 脚本
$sqlScript = @"
-- 检查并添加分类
IF NOT EXISTS (SELECT 1 FROM ProductCategory WHERE CategoryId = 1)
BEGIN
    INSERT INTO ProductCategory (CategoryId, Name, Slug, Description, ParentCategoryId, SortOrder, IsActive, CreateTime, UpdateTime)
    VALUES 
    (1, '硬件设备', 'hardware', '官方认证的 Web3 设备', NULL, 1, 1, GETUTCDATE(), GETUTCDATE()),
    (2, '数字藏品', 'digital-collectibles', '限量发行的 NFT 藏品', NULL, 2, 1, GETUTCDATE(), GETUTCDATE()),
    (3, '增值服务', 'value-services', '链上增值服务', NULL, 3, 1, GETUTCDATE(), GETUTCDATE());
END

-- 检查并添加商品
IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductId = 1)
BEGIN
    DECLARE @Now DATETIME = GETUTCDATE();
    
    INSERT INTO Product (CategoryId, Name, Subtitle, Description, ThumbnailUrl, Price, Currency, ChainId, Sku, IsPublished, CreateTime, UpdateTime)
    VALUES 
    (1, 'Bitget Web3 Vault', '冷存储签名器', '专为 Bitget 用户打造的硬件 Vault，提供安全的冷存储解决方案', NULL, 399.00, 'USDT', 1, 'BG-VLT-001', 1, @Now, @Now),
    (2, 'Bitget Genesis NFT', '限量 Genesis 系列', 'Bitget 生态限量 Genesis NFT，具有独特的收藏价值', NULL, 129.00, 'USDT', 137, 'BG-NFT-001', 1, @Now, @Now),
    (3, '链上风控加速', '链上风控增强服务', '快速提升企业链上风控能力，保障资产安全', NULL, 259.00, 'USDT', 56, 'BG-SRV-001', 1, @Now, @Now),
    (1, 'Web3 安全钱包', '多链支持硬件钱包', '支持 Ethereum、Polygon、BSC 等多条链的硬件钱包', NULL, 299.00, 'USDT', 1, 'BG-WLT-002', 1, @Now, @Now),
    (2, 'Bitget 纪念 NFT', '平台纪念系列', 'Bitget 平台纪念 NFT，限量发行 1000 枚', NULL, 89.00, 'USDT', 137, 'BG-NFT-002', 1, @Now, @Now),
    (3, '智能合约审计服务', '专业安全审计', '提供专业的智能合约安全审计服务，保障合约安全', NULL, 599.00, 'USDT', 1, 'BG-SRV-002', 1, @Now, @Now);
    
    -- 添加库存
    INSERT INTO ProductInventory (ProductId, QuantityAvailable, QuantityReserved, UpdateTime)
    SELECT ProductId, 
           CASE ProductId 
               WHEN 1 THEN 25
               WHEN 2 THEN 80
               WHEN 3 THEN 120
               WHEN 4 THEN 50
               WHEN 5 THEN 1000
               WHEN 6 THEN 10
           END,
           0,
           @Now
    FROM Product
    WHERE ProductId BETWEEN 1 AND 6;
END

SELECT '商品添加完成！' AS Message;
SELECT COUNT(*) AS ProductCount FROM Product WHERE IsPublished = 1;
SELECT COUNT(*) AS CategoryCount FROM ProductCategory WHERE IsActive = 1;
"@

try {
    # 使用 sqlcmd 执行 SQL 脚本
    $sqlcmdPath = "sqlcmd"
    
    # 检查 sqlcmd 是否可用
    $sqlcmdAvailable = Get-Command sqlcmd -ErrorAction SilentlyContinue
    
    if (-not $sqlcmdAvailable) {
        Write-Host "`n[错误] 未找到 sqlcmd 工具" -ForegroundColor Red
        Write-Host "请安装 SQL Server 命令行工具，或使用其他方法执行 SQL 脚本" -ForegroundColor Yellow
        Write-Host "`nSQL 脚本已保存到: 添加示例商品.sql" -ForegroundColor Cyan
        
        # 保存 SQL 脚本到文件
        $sqlScript | Out-File -FilePath "添加示例商品.sql" -Encoding UTF8
        exit 1
    }
    
    # 执行 SQL 脚本
    Write-Host "正在执行 SQL 脚本..." -ForegroundColor Yellow
    
    $sqlScript | sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -C -Q "SET NOCOUNT ON; $sqlScript"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n[成功] 商品已添加到数据库！" -ForegroundColor Green
    } else {
        Write-Host "`n[错误] SQL 执行失败" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "`n[错误] 执行失败: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`n已保存 SQL 脚本到文件，您可以手动执行" -ForegroundColor Yellow
    $sqlScript | Out-File -FilePath "添加示例商品.sql" -Encoding UTF8
    exit 1
}

Write-Host "`n请刷新前端页面查看商品" -ForegroundColor Cyan
Write-Host "访问: http://localhost:5173`n" -ForegroundColor White

