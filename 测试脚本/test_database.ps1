# 数据库测试脚本
# 用于测试数据库连接和迁移状态

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Continue"

Write-Host "========================================" -ForegroundColor Green
Write-Host "  数据库测试脚本" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$projectRoot = Split-Path -Parent $PSScriptRoot
$dbServicePath = Join-Path $projectRoot "src\Backend\UnifiedPlatform.DbService"

if (-not (Test-Path $dbServicePath)) {
    Write-Host "[FAIL] 找不到 DbService 项目路径: $dbServicePath" -ForegroundColor Red
    exit 1
}

Write-Host "[1] 检查迁移文件..." -ForegroundColor Cyan
Set-Location $dbServicePath

try {
    $migrations = dotnet ef migrations list --context StDbContext 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] 迁移列表获取成功" -ForegroundColor Green
        $migrationCount = ($migrations | Where-Object { $_ -match "^\s+\d{14}_" }).Count
        Write-Host "  [INFO] 找到 $migrationCount 个迁移文件" -ForegroundColor Yellow
    } else {
        Write-Host "  [FAIL] 无法获取迁移列表" -ForegroundColor Red
        Write-Host "  错误信息: $migrations" -ForegroundColor Red
    }
} catch {
    Write-Host "  [FAIL] 执行迁移列表命令失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n[2] 检查数据库连接..." -ForegroundColor Cyan
Write-Host "  [INFO] 数据库连接测试需要后端服务运行" -ForegroundColor Yellow
Write-Host "  [INFO] 请启动后端服务后测试 API 接口" -ForegroundColor Yellow

Write-Host "`n[3] 检查迁移文件..." -ForegroundColor Cyan
$migrationsPath = Join-Path $dbServicePath "Migrations"
if (Test-Path $migrationsPath) {
    $migrationFiles = Get-ChildItem -Path $migrationsPath -Filter "*.cs" | Where-Object { $_.Name -match "^\d{14}_" }
    Write-Host "  [OK] 找到 $($migrationFiles.Count) 个迁移文件" -ForegroundColor Green
    
    # 检查关键迁移
    $keyMigrations = @(
        "*AddShoppingTables*",
        "*AddProductReviewImageSpecification*"
    )
    
    foreach ($pattern in $keyMigrations) {
        $found = $migrationFiles | Where-Object { $_.Name -like $pattern }
        if ($found) {
            Write-Host "  [OK] $($found.Name)" -ForegroundColor Green
        } else {
            Write-Host "  [WARN] 未找到匹配 $pattern 的迁移文件" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "  [FAIL] 迁移目录不存在: $migrationsPath" -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  数据库测试完成" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

