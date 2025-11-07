# 测试TRON完整功能
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TRON完整功能测试" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5195"

# 1. 测试查询TRC20余额
Write-Host "[1] 测试查询TRC20余额..." -ForegroundColor Yellow
Write-Host ""

# USDT测试网合约地址 (TestNet)
$usdtTestNetContract = "TXYZopYRdj2D9XRtbG411XZZ3kM5VkAeBf"
# 或者使用Shasta测试网的USDT: TXLAQ63Xg1NAzckPwKHvzw7CSEmLMEqcdj

# 先创建一个测试钱包
Write-Host "  创建测试钱包..." -ForegroundColor Cyan
try {
    $walletResponse = Invoke-RestMethod -Uri "$baseUrl/api/tron/wallet/create" -Method Post -ContentType "application/json" -TimeoutSec 10
    if ($walletResponse.success) {
        $testAddress = $walletResponse.data.address
        Write-Host "  [OK] 钱包创建成功" -ForegroundColor Green
        Write-Host "  地址: $testAddress" -ForegroundColor Gray
        
        # 测试查询TRC20余额
        Write-Host ""
        Write-Host "  查询TRC20余额 (USDT TestNet)..." -ForegroundColor Cyan
        try {
            $trc20BalanceResponse = Invoke-RestMethod -Uri "$baseUrl/api/tron/balance/trc20/$testAddress" -Method Get -Body @{contractAddress=$usdtTestNetContract} -TimeoutSec 10 -ErrorAction Stop
            if ($trc20BalanceResponse.success) {
                Write-Host "  [OK] 查询TRC20余额成功" -ForegroundColor Green
                Write-Host "  余额: $($trc20BalanceResponse.data.balance) USDT" -ForegroundColor Gray
            } else {
                Write-Host "  [X] 查询TRC20余额失败" -ForegroundColor Red
            }
        } catch {
            Write-Host "  [X] 查询TRC20余额API调用失败" -ForegroundColor Red
            Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
            Write-Host "  注意: 可能需要有效的测试网合约地址" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  [X] 钱包创建失败" -ForegroundColor Red
    }
} catch {
    Write-Host "  [X] 创建钱包API调用失败" -ForegroundColor Red
    Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
}

Write-Host ""

# 2. 测试转账TRX（需要授权）
Write-Host "[2] 测试转账TRX..." -ForegroundColor Yellow
Write-Host ""

Write-Host "  注意: 转账功能需要JWT授权" -ForegroundColor Yellow
Write-Host "  需要先登录获取Token" -ForegroundColor Yellow
Write-Host "  或者临时移除[Authorize]特性进行测试" -ForegroundColor Yellow
Write-Host ""

# 检查是否有登录端点
Write-Host "  检查认证端点..." -ForegroundColor Cyan
try {
    $swaggerJson = Invoke-RestMethod -Uri "$baseUrl/swagger/v1/swagger.json" -Method Get -TimeoutSec 5
    $authPaths = $swaggerJson.paths.PSObject.Properties | Where-Object { $_.Name -like "*auth*" -or $_.Name -like "*login*" }
    if ($authPaths) {
        Write-Host "  [OK] 发现认证端点:" -ForegroundColor Green
        $authPaths | ForEach-Object { Write-Host "    - $($_.Name)" -ForegroundColor Gray }
    } else {
        Write-Host "  [X] 未发现认证端点" -ForegroundColor Red
    }
} catch {
    Write-Host "  [X] 无法获取Swagger信息" -ForegroundColor Red
}

Write-Host ""

# 3. 测试转账TRC20（需要授权）
Write-Host "[3] 测试转账TRC20..." -ForegroundColor Yellow
Write-Host ""

Write-Host "  注意: 转账功能需要JWT授权" -ForegroundColor Yellow
Write-Host "  需要先登录获取Token" -ForegroundColor Yellow
Write-Host "  或者临时移除[Authorize]特性进行测试" -ForegroundColor Yellow
Write-Host ""

# 4. 测试查询交易状态
Write-Host "[4] 测试查询交易状态..." -ForegroundColor Yellow
Write-Host ""

# 使用一个示例交易ID（测试网）
# 注意: 这里使用一个示例格式，实际使用时需要替换为真实的交易ID
# TRON交易ID格式: 64个字符的十六进制字符串（不带0x前缀）
# 示例格式: "a1b2c3d4e5f6789012345678901234567890abcdef1234567890abcdef12345678"
$testTxId = "a1b2c3d4e5f6789012345678901234567890abcdef1234567890abcdef12345678"
Write-Host "  注意: 这是示例交易ID，实际使用时需要替换为真实的交易ID" -ForegroundColor Yellow
Write-Host "  格式要求: 64个字符的十六进制字符串（可选0x前缀）" -ForegroundColor Yellow
Write-Host "  获取方式: 在测试网执行转账后获取交易ID" -ForegroundColor Yellow
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  测试完成" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "建议:" -ForegroundColor Yellow
Write-Host "  1. 配置测试网络RPC节点" -ForegroundColor Gray
Write-Host "  2. 获取测试网TRX（从水龙头）" -ForegroundColor Gray
Write-Host "  3. 配置JWT认证或临时移除授权" -ForegroundColor Gray
Write-Host "  4. 执行实际转账测试" -ForegroundColor Gray
Write-Host ""

