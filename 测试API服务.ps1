# 步骤4：测试API服务脚本

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  步骤4：测试API服务" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. 检查服务进程
Write-Host "[1] 检查服务进程..." -ForegroundColor Yellow
$processes = Get-Process -Name "UnifiedPlatform.WebApi" -ErrorAction SilentlyContinue
if ($processes) {
    Write-Host "  发现运行中的服务:" -ForegroundColor Green
    $processes | ForEach-Object { Write-Host "    PID: $($_.Id)" -ForegroundColor Gray }
} else {
    Write-Host "  未发现运行中的服务" -ForegroundColor Yellow
}
Write-Host ""

# 2. 检查端口
Write-Host "[2] 检查端口监听..." -ForegroundColor Yellow
$port5195 = Get-NetTCPConnection -LocalPort 5195 -State Listen -ErrorAction SilentlyContinue
if ($port5195) {
    Write-Host "  [OK] HTTP 端口 5195: 正在监听" -ForegroundColor Green
} else {
    Write-Host "  [X] HTTP 端口 5195: 未监听" -ForegroundColor Red
}
Write-Host ""

# 3. 测试Swagger访问
Write-Host "[3] 测试Swagger访问..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5195/swagger/index.html" -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "  [OK] Swagger UI 可访问" -ForegroundColor Green
        Write-Host "  地址: http://localhost:5195/swagger" -ForegroundColor Gray
    }
} catch {
    Write-Host "  [X] Swagger UI 无法访问: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 4. 测试健康检查端点
Write-Host "[4] 测试健康检查端点..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5195/health" -Method Get -TimeoutSec 5 -ErrorAction Stop
    if ($response.status -eq "healthy") {
        Write-Host "  [OK] 健康检查端点正常" -ForegroundColor Green
        Write-Host "  状态: $($response.status)" -ForegroundColor Gray
    }
} catch {
    Write-Host "  [X] 健康检查失败: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 5. 测试创建钱包API
Write-Host "[5] 测试创建钱包API..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5195/api/tron/wallet/create" -Method Post -ContentType "application/json" -TimeoutSec 10 -ErrorAction Stop
    if ($response.success -and $response.data) {
        Write-Host "  [OK] 创建钱包API测试成功" -ForegroundColor Green
        Write-Host "    地址: $($response.data.address)" -ForegroundColor Gray
        Write-Host "    私钥: $($response.data.privateKey.Substring(0, 20))..." -ForegroundColor Gray
    } else {
        Write-Host "  [X] API返回格式异常" -ForegroundColor Red
    }
} catch {
    Write-Host "  [X] API测试失败: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "    HTTP状态码: $statusCode" -ForegroundColor Gray
    }
}
Write-Host ""

# 6. 列出所有可用端点
Write-Host "[6] 可用API端点列表:" -ForegroundColor Yellow
Write-Host "  - POST /api/tron/wallet/create" -ForegroundColor Gray
Write-Host "  - POST /api/tron/wallet/from-private-key" -ForegroundColor Gray
Write-Host "  - GET  /api/tron/balance/trx/{address}" -ForegroundColor Gray
Write-Host "  - GET  /api/tron/balance/trc20/{address}?contractAddress=xxx" -ForegroundColor Gray
Write-Host "  - POST /api/tron/transfer/trx (需授权)" -ForegroundColor Gray
Write-Host "  - POST /api/tron/transfer/trc20 (需授权)" -ForegroundColor Gray
Write-Host "  - GET  /api/tron/transaction/{txId}/status" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "  测试完成" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "提示: 在浏览器中打开 http://localhost:5195/swagger 进行完整测试" -ForegroundColor Yellow
Write-Host ""

