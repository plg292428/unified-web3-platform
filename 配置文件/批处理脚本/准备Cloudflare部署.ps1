# 准备Cloudflare部署脚本
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  准备Cloudflare部署" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$frontendPath = "src\Frontend\web-app"
$backendPath = "src\Backend\UnifiedPlatform.WebApi"

# 1. 检查项目结构
Write-Host "[1] 检查项目结构..." -ForegroundColor Yellow
Write-Host ""

if (-not (Test-Path $frontendPath)) {
    Write-Host "  [X] 前端项目不存在: $frontendPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $backendPath)) {
    Write-Host "  [X] 后端项目不存在: $backendPath" -ForegroundColor Red
    exit 1
}

Write-Host "  [OK] 项目结构检查通过" -ForegroundColor Green
Write-Host ""

# 2. 更新前端配置
Write-Host "[2] 更新前端配置..." -ForegroundColor Yellow
Write-Host ""

Set-Location $frontendPath

# 检查.env.production
if (-not (Test-Path ".env.production")) {
    Write-Host "  创建 .env.production 文件..." -ForegroundColor Cyan
    @"
VITE_API_BASE_URL=https://your-api-domain.com
VITE_APP_NAME=UnifiedWeb3Platform
"@ | Out-File -FilePath ".env.production" -Encoding utf8
    Write-Host "  [OK] .env.production 已创建" -ForegroundColor Green
    Write-Host "  [提示] 请更新 VITE_API_BASE_URL 为实际后端地址" -ForegroundColor Yellow
} else {
    Write-Host "  [OK] .env.production 已存在" -ForegroundColor Green
}

Write-Host ""

# 3. 构建前端
Write-Host "[3] 构建前端项目..." -ForegroundColor Yellow
Write-Host ""

if (Test-Path "node_modules") {
    Write-Host "  依赖已安装，开始构建..." -ForegroundColor Gray
} else {
    Write-Host "  安装依赖..." -ForegroundColor Gray
    npm install
}

Write-Host "  执行构建..." -ForegroundColor Gray
npm run build

if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] 前端构建成功" -ForegroundColor Green
    Write-Host "  输出目录: dist" -ForegroundColor Gray
} else {
    Write-Host "  [X] 前端构建失败" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 4. 检查构建输出
Write-Host "[4] 检查构建输出..." -ForegroundColor Yellow
Write-Host ""

if (Test-Path "dist\index.html") {
    $distSize = (Get-ChildItem -Path "dist" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host "  [OK] 构建输出存在" -ForegroundColor Green
    Write-Host "  总大小: $([math]::Round($distSize, 2)) MB" -ForegroundColor Gray
} else {
    Write-Host "  [X] 构建输出不存在" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 5. 创建部署说明
Write-Host "[5] 生成部署说明..." -ForegroundColor Yellow
Write-Host ""

$deployGuide = @"
# Cloudflare Pages 部署指南

## 构建输出
- 目录: dist
- 大小: $([math]::Round($distSize, 2)) MB

## 部署方式

### 方式1: 通过GitHub自动部署（推荐）
1. 推送代码到GitHub
2. 在Cloudflare Pages中连接仓库
3. 配置构建设置:
   - Framework preset: Vite
   - Build command: npm run build
   - Build output directory: dist
   - Root directory: src/Frontend/web-app

### 方式2: 通过Wrangler CLI部署
\`\`\`bash
npm install -g wrangler
wrangler login
wrangler pages project create unified-web3-platform
wrangler pages deploy dist --project-name=unified-web3-platform
\`\`\`

## 环境变量配置
在Cloudflare Pages中配置:
- VITE_API_BASE_URL=https://your-api-domain.com
- VITE_APP_NAME=UnifiedWeb3Platform

## 注意事项
- 确保后端API已部署并配置CORS
- 更新 .env.production 中的API地址
- 配置自定义域名（可选）
"@

$deployGuide | Out-File -FilePath "..\..\..\配置文件\报告文档\Cloudflare_Pages部署指南.md" -Encoding utf8

Write-Host "  [OK] 部署指南已生成" -ForegroundColor Green
Write-Host "  位置: 配置文件\报告文档\Cloudflare_Pages部署指南.md" -ForegroundColor Gray

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  准备完成" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "下一步:" -ForegroundColor Yellow
Write-Host "  1. 更新 .env.production 中的API地址" -ForegroundColor Gray
Write-Host "  2. 访问 https://dash.cloudflare.com/ 创建Pages项目" -ForegroundColor Gray
Write-Host "  3. 按照部署指南进行部署" -ForegroundColor Gray
Write-Host ""

Set-Location "..\..\.."

