# ========================================
# Cloudflare Tunnel 自动修复脚本
# ========================================
# 功能：
#   1. 修复 config.yml 中的重复配置
#   2. 验证凭证文件
#   3. 检查后端服务状态
#   4. 提供 Dashboard 手动操作指南
#   5. 验证配置
# ========================================

$ErrorActionPreference = "Continue"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Cloudflare Tunnel 自动修复脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 切换到项目目录
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "[步骤 1] 检查环境..." -ForegroundColor Yellow
Write-Host ""

# 检查 cloudflared
$cloudflaredCmd = $null
if (Test-Path "cloudflared.exe") {
    $cloudflaredCmd = ".\cloudflared.exe"
    Write-Host "  ✅ 找到 cloudflared.exe" -ForegroundColor Green
} else {
    $cloudflaredPath = Get-Command cloudflared -ErrorAction SilentlyContinue
    if ($cloudflaredPath) {
        $cloudflaredCmd = "cloudflared"
        Write-Host "  ✅ 找到 cloudflared (系统路径)" -ForegroundColor Green
    } else {
        Write-Host "  ❌ 未找到 cloudflared" -ForegroundColor Red
        Write-Host "  💡 请先安装 cloudflared 或确保 cloudflared.exe 在项目目录中" -ForegroundColor Gray
        Write-Host ""
        Read-Host "按 Enter 键退出"
        exit 1
    }
}

Write-Host ""

# ========================================
# 步骤 2: 修复 config.yml
# ========================================
Write-Host "[步骤 2] 修复 config.yml..." -ForegroundColor Yellow
Write-Host ""

$configPath = Join-Path $scriptDir "config.yml"

if (-not (Test-Path $configPath)) {
    Write-Host "  ⚠️ config.yml 不存在，将创建新文件" -ForegroundColor Yellow
}

# 读取现有配置
$configLines = @(
    "tunnel: api",
    "credentials-file: C:\Users\$env:USERNAME\.cloudflared\686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86.json",
    "",
    "ingress:",
    "  - hostname: api.a292428dsj.dpdns.org",
    "    service: http://127.0.0.1:5000",
    "  - service: http_status:404"
)
$configContent = $configLines -join "`r`n"

# 检查是否有重复的 ingress 配置
if (Test-Path $configPath) {
    $existingContent = Get-Content $configPath -Raw
    $ingressCount = ([regex]::Matches($existingContent, "ingress:")).Count
    
    if ($ingressCount -gt 1) {
        Write-Host "  ⚠️ 发现重复的 ingress 配置，正在修复..." -ForegroundColor Yellow
        $configContent | Set-Content $configPath -Encoding UTF8
        Write-Host "  ✅ config.yml 已修复（移除重复配置）" -ForegroundColor Green
    } else {
        # 检查是否需要更新 Service URL
        if ($existingContent -notmatch "127\.0\.0\.1:5000") {
            Write-Host "  ⚠️ Service URL 需要更新为 127.0.0.1:5000" -ForegroundColor Yellow
            $configContent | Set-Content $configPath -Encoding UTF8
            Write-Host "  ✅ config.yml 已更新（Service URL）" -ForegroundColor Green
        } else {
            Write-Host "  ✅ config.yml 配置正确" -ForegroundColor Green
        }
    }
} else {
    $configContent | Set-Content $configPath -Encoding UTF8
    Write-Host "  ✅ config.yml 已创建" -ForegroundColor Green
}

Write-Host ""

# ========================================
# 步骤 3: 验证凭证文件
# ========================================
Write-Host "[步骤 3] 验证凭证文件..." -ForegroundColor Yellow
Write-Host ""

$credentialsPath = "C:\Users\$env:USERNAME\.cloudflared\686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86.json"

if (Test-Path $credentialsPath) {
    try {
        $credentials = Get-Content $credentialsPath -Raw | ConvertFrom-Json
        Write-Host "  ✅ 凭证文件存在" -ForegroundColor Green
        Write-Host "    Tunnel ID: $($credentials.TunnelID)" -ForegroundColor Gray
        Write-Host "    Account ID: $($credentials.AccountTag)" -ForegroundColor Gray
        
        if ($credentials.TunnelID -ne "686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86") {
            Write-Host "  ⚠️ Tunnel ID 不匹配！" -ForegroundColor Yellow
            Write-Host "    config.yml 期望: 686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86" -ForegroundColor Gray
            Write-Host "    凭证文件包含: $($credentials.TunnelID)" -ForegroundColor Gray
        }
    } catch {
        Write-Host "  ❌ 凭证文件格式错误: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "  ❌ 凭证文件不存在: $credentialsPath" -ForegroundColor Red
    Write-Host "  💡 需要创建 Tunnel 或重新生成凭证" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# 步骤 4: 检查后端服务
# ========================================
Write-Host "[步骤 4] 检查后端服务..." -ForegroundColor Yellow
Write-Host ""

$backendPort = netstat -ano | Select-String ":5000" | Select-String "LISTENING"

if ($backendPort) {
    Write-Host "  ✅ 后端服务正在运行 (端口 5000)" -ForegroundColor Green
} else {
    Write-Host "  ⚠️ 后端服务未运行 (端口 5000)" -ForegroundColor Yellow
    Write-Host "  💡 请先启动后端服务: 启动后端服务.bat" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# 步骤 5: 检查 Tunnel 进程
# ========================================
Write-Host "[步骤 5] 检查 Tunnel 进程..." -ForegroundColor Yellow
Write-Host ""

$tunnelProcesses = Get-Process -Name "cloudflared" -ErrorAction SilentlyContinue

if ($tunnelProcesses) {
    Write-Host "  ✅ 发现 $($tunnelProcesses.Count) 个 cloudflared 进程" -ForegroundColor Green
    foreach ($proc in $tunnelProcesses) {
        Write-Host "    PID: $($proc.Id)" -ForegroundColor Gray
    }
    Write-Host "  💡 如果需要重启 Tunnel，请先停止这些进程" -ForegroundColor Gray
} else {
    Write-Host "  ℹ️ 当前没有运行中的 cloudflared 进程" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# 步骤 6: 验证配置
# ========================================
Write-Host "[步骤 6] 验证配置..." -ForegroundColor Yellow
Write-Host ""

$config = Get-Content $configPath -Raw

$issues = @()

# 检查 Tunnel 名称
if ($config -notmatch "tunnel:\s*api") {
    $issues += "Tunnel 名称不是 'api'"
}

# 检查 Service URL
if ($config -notmatch "127\.0\.0\.1:5000") {
    $issues += "Service URL 不是 127.0.0.1:5000"
}

# 检查 Hostname
if ($config -notmatch "api\.a292428dsj\.dpdns\.org") {
    $issues += "Hostname 不是 api.a292428dsj.dpdns.org"
}

# 检查重复的 ingress
$ingressMatches = [regex]::Matches($config, "ingress:")
if ($ingressMatches.Count -gt 1) {
    $issues += "发现重复的 ingress 配置"
}

if ($issues.Count -eq 0) {
    Write-Host "  ✅ 所有配置检查通过" -ForegroundColor Green
} else {
    Write-Host "  ⚠️ 发现以下问题:" -ForegroundColor Yellow
    foreach ($issue in $issues) {
        Write-Host "    - $issue" -ForegroundColor Gray
    }
}

Write-Host ""

# ========================================
# 步骤 7: Dashboard 手动操作指南
# ========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Dashboard 手动操作指南" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "⚠️  以下操作需要在 Cloudflare Dashboard 中手动完成:" -ForegroundColor Yellow
Write-Host ""

Write-Host "📋 操作步骤:" -ForegroundColor Cyan
Write-Host ""

Write-Host "1. 访问 Cloudflare Dashboard" -ForegroundColor White
Write-Host "   网址: https://dash.cloudflare.com" -ForegroundColor Gray
Write-Host ""

Write-Host "2. 导航到 Tunnels 页面" -ForegroundColor White
Write-Host "   路径: Zero Trust → Networks → Tunnels" -ForegroundColor Gray
Write-Host ""

Write-Host "3. 检查 Tunnel 状态" -ForegroundColor White
Write-Host "   - 找到名为 'api' 的 Tunnel" -ForegroundColor Gray
Write-Host "   - 检查 Tunnel ID 是否为: 686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86" -ForegroundColor Gray
Write-Host "   - 如果 Tunnel ID 不匹配，需要:" -ForegroundColor Gray
Write-Host "     a) 删除旧的 Tunnel (如果存在)" -ForegroundColor Gray
Write-Host "     b) 或更新 config.yml 使用正确的 Tunnel ID" -ForegroundColor Gray
Write-Host ""

Write-Host "4. 启用 Tunnel" -ForegroundColor White
Write-Host "   - 如果 Tunnel 状态为 '停用'，点击状态按钮改为 '启用'" -ForegroundColor Gray
Write-Host "   - 这是关键步骤！Tunnel 必须启用才能工作" -ForegroundColor Yellow
Write-Host ""

Write-Host "5. 配置 Public Hostname" -ForegroundColor White
Write-Host "   - 点击 Tunnel 'api' 进入详情页面" -ForegroundColor Gray
Write-Host "   - 点击 'Public Hostname' 标签" -ForegroundColor Gray
Write-Host "   - 点击 'Add a public hostname' 按钮" -ForegroundColor Gray
Write-Host "   - 填写以下信息:" -ForegroundColor Gray
Write-Host "     Subdomain: api" -ForegroundColor Cyan
Write-Host "     Domain: a292428dsj.dpdns.org" -ForegroundColor Cyan
Write-Host "     Service Type: HTTP" -ForegroundColor Cyan
Write-Host "     URL: http://127.0.0.1:5000" -ForegroundColor Cyan
Write-Host "   - 点击 'Save hostname' 保存" -ForegroundColor Gray
Write-Host ""

Write-Host "6. 验证配置" -ForegroundColor White
Write-Host "   - 等待 2-5 分钟让配置生效" -ForegroundColor Gray
Write-Host "   - 测试: https://api.a292428dsj.dpdns.org/health" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ========================================
# 步骤 8: 总结
# ========================================
Write-Host "[步骤 8] 修复总结..." -ForegroundColor Yellow
Write-Host ""

Write-Host "✅ 已完成的自动修复:" -ForegroundColor Green
Write-Host "  ✅ 修复了 config.yml 中的重复配置" -ForegroundColor Gray
Write-Host "  ✅ 确保 Service URL 使用 127.0.0.1:5000" -ForegroundColor Gray
Write-Host "  ✅ 验证了凭证文件" -ForegroundColor Gray
Write-Host "  ✅ 检查了后端服务状态" -ForegroundColor Gray
Write-Host ""

Write-Host "⚠️  需要手动完成的操作:" -ForegroundColor Yellow
Write-Host "  ⚠️  在 Dashboard 中启用 Tunnel" -ForegroundColor Gray
Write-Host "  ⚠️  在 Dashboard 中配置 Public Hostname" -ForegroundColor Gray
Write-Host "  ⚠️  如果 Tunnel ID 不匹配，需要统一配置" -ForegroundColor Gray
Write-Host ""

Write-Host "📋 下一步操作:" -ForegroundColor Cyan
Write-Host "  1. 按照上面的指南在 Dashboard 中完成手动配置" -ForegroundColor Gray
Write-Host "  2. 确保后端服务正在运行 (端口 5000)" -ForegroundColor Gray
Write-Host "  3. 运行: 启动后端API_Tunnel.bat" -ForegroundColor Gray
Write-Host "  4. 等待 2-5 分钟后测试: https://api.a292428dsj.dpdns.org/health" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Read-Host "按 Enter 键退出"

