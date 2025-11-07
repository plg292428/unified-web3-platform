# UnifiedWeb3Platform项目目录创建脚本
# 执行前请确保在正确的目录（例如：D:\claude code\plg）

Write-Host "========================================" -ForegroundColor Green
Write-Host "  UnifiedWeb3Platform项目目录创建" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# 获取当前脚本所在目录的父目录（plg目录）
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$plgDir = Split-Path -Parent $scriptDir
Set-Location $plgDir

Write-Host "[信息] 当前工作目录: $plgDir" -ForegroundColor Cyan
Write-Host ""

# 创建主项目目录
Write-Host "[步骤1] 创建主项目目录..." -ForegroundColor Yellow
if (-not (Test-Path "UnifiedWeb3Platform")) {
    New-Item -ItemType Directory -Path "UnifiedWeb3Platform" -Force | Out-Null
    Write-Host "  ✓ 已创建主项目目录" -ForegroundColor Green
} else {
    Write-Host "  ℹ  主项目目录已存在" -ForegroundColor Yellow
}

# 进入项目目录
Set-Location "UnifiedWeb3Platform"
Write-Host ""

# 创建前端目录
Write-Host "[步骤2] 创建前端目录..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path "src\Frontend\web-app" -Force | Out-Null
Write-Host "  ✓ 已创建: src\Frontend\web-app" -ForegroundColor Green
Write-Host ""

# 创建后端目录
Write-Host "[步骤3] 创建后端目录..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path "src\Backend\UnifiedPlatform.WebApi" -Force | Out-Null
Write-Host "  ✓ 已创建: src\Backend\UnifiedPlatform.WebApi" -ForegroundColor Green

New-Item -ItemType Directory -Path "src\Backend\UnifiedPlatform.DbService" -Force | Out-Null
Write-Host "  ✓ 已创建: src\Backend\UnifiedPlatform.DbService" -ForegroundColor Green

New-Item -ItemType Directory -Path "src\Backend\UnifiedPlatform.Shared" -Force | Out-Null
Write-Host "  ✓ 已创建: src\Backend\UnifiedPlatform.Shared" -ForegroundColor Green
Write-Host ""

# 创建库目录
Write-Host "[步骤4] 创建库目录..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path "src\Libraries\HFastKit" -Force | Out-Null
Write-Host "  ✓ 已创建: src\Libraries\HFastKit" -ForegroundColor Green

New-Item -ItemType Directory -Path "src\Libraries\Nblockchain" -Force | Out-Null
Write-Host "  ✓ 已创建: src\Libraries\Nblockchain" -ForegroundColor Green
Write-Host ""

# 显示目录结构
Write-Host "========================================" -ForegroundColor Green
Write-Host "  项目目录结构：" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Get-ChildItem -Path . -Recurse -Directory | ForEach-Object {
    $relativePath = $_.FullName.Replace((Get-Location).Path + "\", "")
    Write-Host "  $relativePath" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ✓ 目录创建完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "[提示] 项目根目录: $((Get-Location).Path)" -ForegroundColor Cyan

