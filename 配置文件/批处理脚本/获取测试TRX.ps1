# 获取TRON测试网TRX脚本
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  获取TRON测试网TRX" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. 创建测试钱包
Write-Host "[1] 创建测试钱包..." -ForegroundColor Yellow
Write-Host ""

try {
    $walletResponse = Invoke-RestMethod -Uri "http://localhost:5195/api/tron/wallet/create" -Method Post -ContentType "application/json" -TimeoutSec 10
    
    if ($walletResponse.success) {
        $address = $walletResponse.data.address
        $privateKey = $walletResponse.data.privateKey
        
        Write-Host "  [OK] 钱包创建成功" -ForegroundColor Green
        Write-Host "  地址: $address" -ForegroundColor Cyan
        Write-Host "  私钥: $privateKey" -ForegroundColor Gray
        Write-Host ""
        
        # 保存钱包信息
        $walletInfo = @{
            address = $address
            privateKey = $privateKey
            publicKey = $walletResponse.data.publicKey
            createdAt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        } | ConvertTo-Json -Depth 10
        
        $walletFile = "配置文件\测试钱包信息.json"
        $walletInfo | Out-File -FilePath $walletFile -Encoding UTF8
        Write-Host "  钱包信息已保存到: $walletFile" -ForegroundColor Gray
        Write-Host ""
        
        # 2. 获取测试TRX
        Write-Host "[2] 获取测试TRX..." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "  方法1: 使用TronGrid水龙头" -ForegroundColor Cyan
        Write-Host "  网址: https://www.trongrid.io/faucet" -ForegroundColor Gray
        Write-Host "  步骤:" -ForegroundColor Gray
        Write-Host "    1. 访问上述网址" -ForegroundColor Gray
        Write-Host "    2. 输入钱包地址: $address" -ForegroundColor Gray
        Write-Host "    3. 完成验证码验证" -ForegroundColor Gray
        Write-Host "    4. 点击获取测试TRX" -ForegroundColor Gray
        Write-Host ""
        
        Write-Host "  方法2: 使用DataWallet水龙头" -ForegroundColor Cyan
        Write-Host "  网址: https://www.datawallet.com/zh/%E9%9A%90%E8%94%BD%E6%80%A7/get-tron-testnet-tokens" -ForegroundColor Gray
        Write-Host "  说明: 每天最多可获取2000个测试TRX" -ForegroundColor Gray
        Write-Host ""
        
        Write-Host "  方法3: 使用TronLink钱包" -ForegroundColor Cyan
        Write-Host "  步骤:" -ForegroundColor Gray
        Write-Host "    1. 安装TronLink浏览器扩展" -ForegroundColor Gray
        Write-Host "    2. 切换到Shasta测试网" -ForegroundColor Gray
        Write-Host "    3. 导入钱包（使用上述私钥）" -ForegroundColor Gray
        Write-Host "    4. 使用TronLink内置的水龙头功能" -ForegroundColor Gray
        Write-Host ""
        
        # 3. 查询余额
        Write-Host "[3] 查询TRX余额..." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "  等待5秒后查询余额..." -ForegroundColor Gray
        Start-Sleep -Seconds 5
        
        try {
            $balanceResponse = Invoke-RestMethod -Uri "http://localhost:5195/api/tron/balance/trx/$address" -Method Get -TimeoutSec 10
            
            if ($balanceResponse.success) {
                $balance = $balanceResponse.data.balance
                Write-Host "  [OK] 查询余额成功" -ForegroundColor Green
                Write-Host "  余额: $balance TRX" -ForegroundColor Cyan
                
                if ([decimal]$balance -eq 0) {
                    Write-Host ""
                    Write-Host "  [提示] 余额为0，请先获取测试TRX" -ForegroundColor Yellow
                }
            } else {
                Write-Host "  [X] 查询余额失败" -ForegroundColor Red
            }
        } catch {
            Write-Host "  [X] 查询余额API调用失败" -ForegroundColor Red
            Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
        }
        
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "  完成" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "重要提示:" -ForegroundColor Yellow
        Write-Host "  1. 请妥善保管私钥，不要泄露" -ForegroundColor Gray
        Write-Host "  2. 测试网TRX仅用于测试，无实际价值" -ForegroundColor Gray
        Write-Host "  3. 获取测试TRX后，可以使用转账功能进行测试" -ForegroundColor Gray
        Write-Host ""
        
    } else {
        Write-Host "  [X] 钱包创建失败" -ForegroundColor Red
        Write-Host "  错误: $($walletResponse.message)" -ForegroundColor Gray
    }
} catch {
    Write-Host "  [X] 创建钱包API调用失败" -ForegroundColor Red
    Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  请确保服务已启动: http://localhost:5195" -ForegroundColor Yellow
}

Write-Host ""

