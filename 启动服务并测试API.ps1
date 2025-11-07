# 启动服务并测试API
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  步骤4：测试API" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 切换到项目目录
$projectPath = "D:\claude code\plg\UnifiedWeb3Platform\src\Backend\UnifiedPlatform.WebApi"
Set-Location $projectPath

# 1. 停止可能运行的服务
Write-Host "[1] 停止可能运行的服务..." -ForegroundColor Yellow
$processes = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -like "*UnifiedPlatform.WebApi*" -or
    (Get-NetTCPConnection -OwningProcess $_.Id -ErrorAction SilentlyContinue | Where-Object { $_.LocalPort -eq 5195 -or $_.LocalPort -eq 5196 })
}
if ($processes) {
    foreach ($proc in $processes) {
        try {
            Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
            Write-Host "  已停止进程 PID: $($proc.Id)" -ForegroundColor Green
        } catch {
            Write-Host "  无法停止进程 PID: $($proc.Id)" -ForegroundColor Red
        }
    }
    Start-Sleep -Seconds 2
} else {
    Write-Host "  无需停止服务" -ForegroundColor Gray
}
Write-Host ""

# 2. 编译项目
Write-Host "[2] 编译项目..." -ForegroundColor Yellow
$buildResult = dotnet build 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] 编译成功" -ForegroundColor Green
} else {
    Write-Host "  [X] 编译失败" -ForegroundColor Red
    $buildResult | Select-String -Pattern "error" | Select-Object -First 5
    exit 1
}
Write-Host ""

# 3. 启动服务（后台）
Write-Host "[3] 启动服务（后台）..." -ForegroundColor Yellow
$job = Start-Job -ScriptBlock {
    Set-Location "D:\claude code\plg\UnifiedWeb3Platform\src\Backend\UnifiedPlatform.WebApi"
    dotnet run --launch-profile https 2>&1
}
Write-Host "  服务已启动，等待初始化..." -ForegroundColor Green
Start-Sleep -Seconds 10
Write-Host ""

# 4. 检查服务状态
Write-Host "[4] 检查服务状态..." -ForegroundColor Yellow
$port5195 = Get-NetTCPConnection -LocalPort 5195 -State Listen -ErrorAction SilentlyContinue
$port5196 = Get-NetTCPConnection -LocalPort 5196 -State Listen -ErrorAction SilentlyContinue

if ($port5195) {
    Write-Host "  [OK] HTTP 端口 5195: 已监听" -ForegroundColor Green
} else {
    Write-Host "  [X] HTTP 端口 5195: 未监听" -ForegroundColor Red
}

if ($port5196) {
    Write-Host "  [OK] HTTPS 端口 5196: 已监听" -ForegroundColor Green
} else {
    Write-Host "  [X] HTTPS 端口 5196: 未监听" -ForegroundColor Red
}
Write-Host ""

# 5. 测试Swagger
Write-Host "[5] 测试Swagger访问..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5195/swagger/index.html" -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "  [OK] Swagger UI 可访问" -ForegroundColor Green
        Write-Host "  地址: http://localhost:5195/swagger" -ForegroundColor Gray
    }
} catch {
    Write-Host "  [X] Swagger UI 无法访问" -ForegroundColor Red
    Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

# 6. 测试API端点
Write-Host "[6] 测试API端点..." -ForegroundColor Yellow
Write-Host ""

# 6.1 创建钱包
Write-Host "  测试1: 创建钱包..." -ForegroundColor Cyan
try {
    $createWalletResponse = Invoke-RestMethod -Uri "http://localhost:5195/api/tron/wallet/create" -Method Post -ContentType "application/json" -TimeoutSec 10 -ErrorAction Stop
    if ($createWalletResponse.success -and $createWalletResponse.data.address) {
        Write-Host "    [OK] 创建钱包成功" -ForegroundColor Green
        Write-Host "    地址: $($createWalletResponse.data.address)" -ForegroundColor Gray
        $script:testAddress = $createWalletResponse.data.address
        $script:testPrivateKey = $createWalletResponse.data.privateKey
    } else {
        Write-Host "    [X] 创建钱包失败" -ForegroundColor Red
    }
} catch {
    Write-Host "    [X] 创建钱包API调用失败" -ForegroundColor Red
    Write-Host "    错误: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

# 6.2 查询TRX余额
if ($script:testAddress) {
    Write-Host "  测试2: 查询TRX余额..." -ForegroundColor Cyan
    try {
        $balanceResponse = Invoke-RestMethod -Uri "http://localhost:5195/api/tron/balance/trx/$($script:testAddress)" -Method Get -TimeoutSec 10 -ErrorAction Stop
        if ($balanceResponse.success) {
            Write-Host "    [OK] 查询TRX余额成功" -ForegroundColor Green
            Write-Host "    余额: $($balanceResponse.data.balance) TRX" -ForegroundColor Gray
        } else {
            Write-Host "    [X] 查询TRX余额失败" -ForegroundColor Red
        }
    } catch {
        Write-Host "    [X] 查询TRX余额API调用失败" -ForegroundColor Red
        Write-Host "    错误: $($_.Exception.Message)" -ForegroundColor Gray
    }
    Write-Host ""
}

# 6.3 从私钥获取钱包
if ($script:testPrivateKey) {
    Write-Host "  测试3: 从私钥获取钱包..." -ForegroundColor Cyan
    try {
        $walletResponse = Invoke-RestMethod -Uri "http://localhost:5195/api/tron/wallet/from-private-key" -Method Post -Body ($script:testPrivateKey | ConvertTo-Json) -ContentType "application/json" -TimeoutSec 10 -ErrorAction Stop
        if ($walletResponse.success -and $walletResponse.data.address -eq $script:testAddress) {
            Write-Host "    [OK] 从私钥获取钱包成功" -ForegroundColor Green
            Write-Host "    地址匹配: $($walletResponse.data.address)" -ForegroundColor Gray
        } else {
            Write-Host "    [X] 从私钥获取钱包失败" -ForegroundColor Red
        }
    } catch {
        Write-Host "    [X] 从私钥获取钱包API调用失败" -ForegroundColor Red
        Write-Host "    错误: $($_.Exception.Message)" -ForegroundColor Gray
    }
    Write-Host ""
}

# 6.4 检查Swagger中的API端点
Write-Host "  测试4: 检查Swagger中的API端点..." -ForegroundColor Cyan
try {
    $swaggerJson = Invoke-RestMethod -Uri "http://localhost:5195/swagger/v1/swagger.json" -Method Get -TimeoutSec 5 -ErrorAction Stop
    $tronPaths = $swaggerJson.paths.PSObject.Properties | Where-Object { $_.Name -like "/api/tron/*" }
    if ($tronPaths) {
        Write-Host "    [OK] 发现 $($tronPaths.Count) 个TRON API端点" -ForegroundColor Green
        $tronPaths | ForEach-Object {
            Write-Host "      - $($_.Name)" -ForegroundColor Gray
        }
    } else {
        Write-Host "    [X] 未发现TRON API端点" -ForegroundColor Red
    }
} catch {
    Write-Host "    [X] 无法获取Swagger JSON" -ForegroundColor Red
    Write-Host "    错误: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

# 7. 总结
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  测试完成" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "服务运行在:" -ForegroundColor Yellow
Write-Host "  HTTP:  http://localhost:5195" -ForegroundColor Gray
Write-Host "  HTTPS: https://localhost:5196" -ForegroundColor Gray
Write-Host ""
Write-Host "Swagger UI:" -ForegroundColor Yellow
Write-Host "  http://localhost:5195/swagger" -ForegroundColor Gray
Write-Host ""
Write-Host "停止服务:" -ForegroundColor Yellow
Write-Host "  Get-Job | Stop-Job; Get-Job | Remove-Job" -ForegroundColor Gray
Write-Host ""

