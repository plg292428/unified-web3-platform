# 一键验证所有配置
Write-Host "=== 🔍 验证 Tunnel 配置 ===" -ForegroundColor Cyan
Write-Host ""

# 步骤 1: 检查后端服务
Write-Host "[步骤 1] 检查后端服务..." -ForegroundColor Yellow
Write-Host ""
$backendPort = netstat -ano | findstr ":5000" | findstr "LISTENING"
if ($backendPort) {
    Write-Host "  ✅ 后端服务: 运行中 (http://localhost:5000)" -ForegroundColor Green
    Write-Host ""
    Write-Host "  测试本地 API..." -ForegroundColor Gray
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 5 -ErrorAction Stop
        Write-Host "  ✅ 本地 API: 可访问" -ForegroundColor Green
        Write-Host "  ✅ 响应: $($response.Content)" -ForegroundColor Gray
    } catch {
        Write-Host "  ⚠️ 本地 API: 无法访问" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ❌ 后端服务: 未运行" -ForegroundColor Red
    Write-Host "  💡 请先启动后端服务" -ForegroundColor Gray
}
Write-Host ""

# 步骤 2: 检查 Tunnel 进程
Write-Host "[步骤 2] 检查 Tunnel 进程..." -ForegroundColor Yellow
Write-Host ""
$cloudflaredProcesses = Get-Process -Name "cloudflared" -ErrorAction SilentlyContinue
if ($cloudflaredProcesses) {
    Write-Host "  ✅ Cloudflare Tunnel: 进程运行中 (PID: $($cloudflaredProcesses[0].Id))" -ForegroundColor Green
} else {
    Write-Host "  ❌ Cloudflare Tunnel: 进程未运行" -ForegroundColor Red
    Write-Host "  💡 请运行: 启动后端API_Tunnel.bat" -ForegroundColor Gray
}
Write-Host ""

# 步骤 3: 检查 Tunnel 连接状态
Write-Host "[步骤 3] 检查 Tunnel 连接状态..." -ForegroundColor Yellow
Write-Host ""
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
if (Test-Path (Join-Path $scriptPath "cloudflared.exe")) {
    $cloudflared = Join-Path $scriptPath "cloudflared.exe"
} else {
    $cloudflared = "cloudflared"
}

try {
    $tunnelInfo = & $cloudflared tunnel info api 2>&1
    if ($tunnelInfo -match "active connection" -or $tunnelInfo -match "Connection established") {
        Write-Host "  ✅ Tunnel: 有活动连接" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️ Tunnel: 无活动连接" -ForegroundColor Yellow
        Write-Host "  💡 请检查:" -ForegroundColor Gray
        Write-Host "     1. Tunnel 窗口是否显示 'Connection established'" -ForegroundColor Gray
        Write-Host "     2. Dashboard 中 Tunnel 是否已启用" -ForegroundColor Gray
    }
} catch {
    Write-Host "  ⚠️ 无法获取 Tunnel 信息" -ForegroundColor Yellow
}
Write-Host ""

# 步骤 4: 测试公网 API
Write-Host "[步骤 4] 测试公网 API..." -ForegroundColor Yellow
Write-Host ""
Write-Host "  等待 3 秒..." -ForegroundColor Gray
Start-Sleep -Seconds 3
Write-Host ""
try {
    $response = Invoke-WebRequest -Uri "https://api.a292428dsj.dpdns.org/health" -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
    Write-Host "  ✅✅✅ 公网 API: 可访问！" -ForegroundColor Green
    Write-Host "  ✅ 状态码: $($response.StatusCode)" -ForegroundColor Gray
    Write-Host "  ✅ 响应: $($response.Content)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  🎉🎉🎉 配置成功！整个系统完全可用！" -ForegroundColor Green
} catch {
    Write-Host "  ⚠️ 公网 API: 无法访问或返回错误" -ForegroundColor Yellow
    Write-Host "  💡 错误: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  📋 可能原因:" -ForegroundColor Gray
    Write-Host "     1. Dashboard 中 Tunnel 未启用" -ForegroundColor Gray
    Write-Host "     2. Public Hostname 未配置" -ForegroundColor Gray
    Write-Host "     3. Tunnel 未建立连接" -ForegroundColor Gray
    Write-Host "     4. 配置需要更多时间生效（等待 2-5 分钟）" -ForegroundColor Gray
}
Write-Host ""

# 步骤 5: 测试前端网站
Write-Host "[步骤 5] 测试前端网站..." -ForegroundColor Yellow
Write-Host ""
try {
    $response = Invoke-WebRequest -Uri "https://www.a292428dsj.dpdns.org" -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
    Write-Host "  ✅ 前端网站: 可访问 (状态码: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "  ⚠️ 前端网站: 无法访问" -ForegroundColor Yellow
}
Write-Host ""

# 总结
Write-Host "=== 📊 验证总结 ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ 已验证项目:" -ForegroundColor Green
Write-Host "  - 后端服务状态" -ForegroundColor Gray
Write-Host "  - Tunnel 进程状态" -ForegroundColor Gray
Write-Host "  - Tunnel 连接状态" -ForegroundColor Gray
Write-Host "  - 公网 API 可访问性" -ForegroundColor Gray
Write-Host "  - 前端网站可访问性" -ForegroundColor Gray
Write-Host ""
Write-Host "📋 访问地址:" -ForegroundColor Cyan
Write-Host "  本地后端: http://localhost:5000" -ForegroundColor Gray
Write-Host "  公网 API:  https://api.a292428dsj.dpdns.org" -ForegroundColor Gray
Write-Host "  前端网站: https://www.a292428dsj.dpdns.org" -ForegroundColor Gray
Write-Host ""
Write-Host "👥 买家访问:" -ForegroundColor Cyan
Write-Host "  https://www.a292428dsj.dpdns.org" -ForegroundColor Gray
Write-Host ""

Write-Host "=== 🔍 验证 Tunnel 配置 ===" -ForegroundColor Cyan
Write-Host ""

# 步骤 1: 检查后端服务
Write-Host "[步骤 1] 检查后端服务..." -ForegroundColor Yellow
Write-Host ""
$backendPort = netstat -ano | findstr ":5000" | findstr "LISTENING"
if ($backendPort) {
    Write-Host "  ✅ 后端服务: 运行中 (http://localhost:5000)" -ForegroundColor Green
    Write-Host ""
    Write-Host "  测试本地 API..." -ForegroundColor Gray
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 5 -ErrorAction Stop
        Write-Host "  ✅ 本地 API: 可访问" -ForegroundColor Green
        Write-Host "  ✅ 响应: $($response.Content)" -ForegroundColor Gray
    } catch {
        Write-Host "  ⚠️ 本地 API: 无法访问" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ❌ 后端服务: 未运行" -ForegroundColor Red
    Write-Host "  💡 请先启动后端服务" -ForegroundColor Gray
}
Write-Host ""

# 步骤 2: 检查 Tunnel 进程
Write-Host "[步骤 2] 检查 Tunnel 进程..." -ForegroundColor Yellow
Write-Host ""
$cloudflaredProcesses = Get-Process -Name "cloudflared" -ErrorAction SilentlyContinue
if ($cloudflaredProcesses) {
    Write-Host "  ✅ Cloudflare Tunnel: 进程运行中 (PID: $($cloudflaredProcesses[0].Id))" -ForegroundColor Green
} else {
    Write-Host "  ❌ Cloudflare Tunnel: 进程未运行" -ForegroundColor Red
    Write-Host "  💡 请运行: 启动后端API_Tunnel.bat" -ForegroundColor Gray
}
Write-Host ""

# 步骤 3: 检查 Tunnel 连接状态
Write-Host "[步骤 3] 检查 Tunnel 连接状态..." -ForegroundColor Yellow
Write-Host ""
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
if (Test-Path (Join-Path $scriptPath "cloudflared.exe")) {
    $cloudflared = Join-Path $scriptPath "cloudflared.exe"
} else {
    $cloudflared = "cloudflared"
}

try {
    $tunnelInfo = & $cloudflared tunnel info api 2>&1
    if ($tunnelInfo -match "active connection" -or $tunnelInfo -match "Connection established") {
        Write-Host "  ✅ Tunnel: 有活动连接" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️ Tunnel: 无活动连接" -ForegroundColor Yellow
        Write-Host "  💡 请检查:" -ForegroundColor Gray
        Write-Host "     1. Tunnel 窗口是否显示 'Connection established'" -ForegroundColor Gray
        Write-Host "     2. Dashboard 中 Tunnel 是否已启用" -ForegroundColor Gray
    }
} catch {
    Write-Host "  ⚠️ 无法获取 Tunnel 信息" -ForegroundColor Yellow
}
Write-Host ""

# 步骤 4: 测试公网 API
Write-Host "[步骤 4] 测试公网 API..." -ForegroundColor Yellow
Write-Host ""
Write-Host "  等待 3 秒..." -ForegroundColor Gray
Start-Sleep -Seconds 3
Write-Host ""
try {
    $response = Invoke-WebRequest -Uri "https://api.a292428dsj.dpdns.org/health" -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
    Write-Host "  ✅✅✅ 公网 API: 可访问！" -ForegroundColor Green
    Write-Host "  ✅ 状态码: $($response.StatusCode)" -ForegroundColor Gray
    Write-Host "  ✅ 响应: $($response.Content)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  🎉🎉🎉 配置成功！整个系统完全可用！" -ForegroundColor Green
} catch {
    Write-Host "  ⚠️ 公网 API: 无法访问或返回错误" -ForegroundColor Yellow
    Write-Host "  💡 错误: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  📋 可能原因:" -ForegroundColor Gray
    Write-Host "     1. Dashboard 中 Tunnel 未启用" -ForegroundColor Gray
    Write-Host "     2. Public Hostname 未配置" -ForegroundColor Gray
    Write-Host "     3. Tunnel 未建立连接" -ForegroundColor Gray
    Write-Host "     4. 配置需要更多时间生效（等待 2-5 分钟）" -ForegroundColor Gray
}
Write-Host ""

# 步骤 5: 测试前端网站
Write-Host "[步骤 5] 测试前端网站..." -ForegroundColor Yellow
Write-Host ""
try {
    $response = Invoke-WebRequest -Uri "https://www.a292428dsj.dpdns.org" -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
    Write-Host "  ✅ 前端网站: 可访问 (状态码: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "  ⚠️ 前端网站: 无法访问" -ForegroundColor Yellow
}
Write-Host ""

# 总结
Write-Host "=== 📊 验证总结 ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ 已验证项目:" -ForegroundColor Green
Write-Host "  - 后端服务状态" -ForegroundColor Gray
Write-Host "  - Tunnel 进程状态" -ForegroundColor Gray
Write-Host "  - Tunnel 连接状态" -ForegroundColor Gray
Write-Host "  - 公网 API 可访问性" -ForegroundColor Gray
Write-Host "  - 前端网站可访问性" -ForegroundColor Gray
Write-Host ""
Write-Host "📋 访问地址:" -ForegroundColor Cyan
Write-Host "  本地后端: http://localhost:5000" -ForegroundColor Gray
Write-Host "  公网 API:  https://api.a292428dsj.dpdns.org" -ForegroundColor Gray
Write-Host "  前端网站: https://www.a292428dsj.dpdns.org" -ForegroundColor Gray
Write-Host ""
Write-Host "👥 买家访问:" -ForegroundColor Cyan
Write-Host "  https://www.a292428dsj.dpdns.org" -ForegroundColor Gray
Write-Host ""

