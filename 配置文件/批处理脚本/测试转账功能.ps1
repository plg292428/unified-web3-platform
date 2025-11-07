# 测试TRON转账功能（需要JWT认证）
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  测试TRON转账功能" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5195"

# 1. 生成测试JWT Token
Write-Host "[1] 生成测试JWT Token..." -ForegroundColor Yellow
Write-Host ""

try {
    $tokenResponse = Invoke-RestMethod -Uri "$baseUrl/api/testauth/generate-token" -Method Post -ContentType "application/json" -Body (@{} | ConvertTo-Json) -TimeoutSec 10
    
    if ($tokenResponse.success) {
        $token = $tokenResponse.data.token
        Write-Host "  [OK] Token生成成功" -ForegroundColor Green
        Write-Host "  Token: $($token.Substring(0, [Math]::Min(50, $token.Length)))..." -ForegroundColor Gray
        Write-Host ""
        
        # 设置请求头
        $headers = @{
            "Authorization" = "Bearer $token"
            "Content-Type" = "application/json"
        }
        
        # 2. 创建两个测试钱包
        Write-Host "[2] 创建测试钱包..." -ForegroundColor Yellow
        Write-Host ""
        
        $wallet1 = Invoke-RestMethod -Uri "$baseUrl/api/tron/wallet/create" -Method Post -ContentType "application/json" -TimeoutSec 10
        $wallet2 = Invoke-RestMethod -Uri "$baseUrl/api/tron/wallet/create" -Method Post -ContentType "application/json" -TimeoutSec 10
        
        if ($wallet1.success -and $wallet2.success) {
            $fromAddress = $wallet1.data.address
            $fromPrivateKey = $wallet1.data.privateKey
            $toAddress = $wallet2.data.address
            
            Write-Host "  [OK] 钱包创建成功" -ForegroundColor Green
            Write-Host "  发送方: $fromAddress" -ForegroundColor Cyan
            Write-Host "  接收方: $toAddress" -ForegroundColor Cyan
            Write-Host ""
            
            # 3. 查询发送方余额
            Write-Host "[3] 查询发送方余额..." -ForegroundColor Yellow
            Write-Host ""
            
            try {
                $balanceResponse = Invoke-RestMethod -Uri "$baseUrl/api/tron/balance/trx/$fromAddress" -Method Get -TimeoutSec 10
                
                if ($balanceResponse.success) {
                    $balance = [decimal]$balanceResponse.data.balance
                    Write-Host "  [OK] 余额查询成功" -ForegroundColor Green
                    Write-Host "  余额: $balance TRX" -ForegroundColor Cyan
                    Write-Host ""
                    
                    if ($balance -eq 0) {
                        Write-Host "  [X] 余额为0，无法进行转账测试" -ForegroundColor Red
                        Write-Host "  请先获取测试TRX:" -ForegroundColor Yellow
                        Write-Host "    1. 运行: 获取测试TRX.ps1" -ForegroundColor Gray
                        Write-Host "    2. 或访问: https://www.trongrid.io/faucet" -ForegroundColor Gray
                        Write-Host ""
                        exit
                    }
                    
                    # 4. 测试转账TRX
                    Write-Host "[4] 测试转账TRX..." -ForegroundColor Yellow
                    Write-Host ""
                    
                    $transferAmount = [Math]::Min(0.1, $balance / 2) # 转账金额为余额的一半，最多0.1 TRX
                    
                    $transferRequest = @{
                        fromPrivateKey = $fromPrivateKey
                        toAddress = $toAddress
                        amount = $transferAmount
                        memo = "Test Transfer"
                    } | ConvertTo-Json
                    
                    Write-Host "  转账信息:" -ForegroundColor Cyan
                    Write-Host "    金额: $transferAmount TRX" -ForegroundColor Gray
                    Write-Host "    接收方: $toAddress" -ForegroundColor Gray
                    Write-Host ""
                    
                    try {
                        $transferResponse = Invoke-RestMethod -Uri "$baseUrl/api/tron/transfer/trx" -Method Post -Headers $headers -Body $transferRequest -TimeoutSec 30
                        
                        if ($transferResponse.success) {
                            $transactionId = $transferResponse.data.transactionId
                            Write-Host "  [OK] 转账成功" -ForegroundColor Green
                            Write-Host "  交易ID: $transactionId" -ForegroundColor Cyan
                            Write-Host ""
                            
                            # 5. 查询交易状态
                            Write-Host "[5] 查询交易状态..." -ForegroundColor Yellow
                            Write-Host ""
                            Write-Host "  等待5秒后查询交易状态..." -ForegroundColor Gray
                            Start-Sleep -Seconds 5
                            
                            try {
                                $statusResponse = Invoke-RestMethod -Uri "$baseUrl/api/tron/transaction/$transactionId/status" -Method Get -TimeoutSec 10
                                
                                if ($statusResponse.success) {
                                    Write-Host "  [OK] 交易状态查询成功" -ForegroundColor Green
                                    Write-Host "  状态: $($statusResponse.data.status)" -ForegroundColor Cyan
                                    Write-Host "  成功: $($statusResponse.data.isSuccess)" -ForegroundColor Cyan
                                    
                                    if ($statusResponse.data.errorMessage) {
                                        Write-Host "  错误: $($statusResponse.data.errorMessage)" -ForegroundColor Yellow
                                    }
                                } else {
                                    Write-Host "  [X] 交易状态查询失败" -ForegroundColor Red
                                }
                            } catch {
                                Write-Host "  [X] 交易状态查询API调用失败" -ForegroundColor Red
                                Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
                            }
                            
                        } else {
                            Write-Host "  [X] 转账失败" -ForegroundColor Red
                            Write-Host "  错误: $($transferResponse.message)" -ForegroundColor Gray
                        }
                    } catch {
                        Write-Host "  [X] 转账API调用失败" -ForegroundColor Red
                        Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
                        
                        if ($_.Exception.Response) {
                            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
                            $responseBody = $reader.ReadToEnd()
                            Write-Host "  响应: $responseBody" -ForegroundColor Gray
                        }
                    }
                    
                } else {
                    Write-Host "  [X] 余额查询失败" -ForegroundColor Red
                }
            } catch {
                Write-Host "  [X] 余额查询API调用失败" -ForegroundColor Red
                Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
            }
            
        } else {
            Write-Host "  [X] 钱包创建失败" -ForegroundColor Red
        }
        
    } else {
        Write-Host "  [X] Token生成失败" -ForegroundColor Red
        Write-Host "  错误: $($tokenResponse.message)" -ForegroundColor Gray
    }
} catch {
    Write-Host "  [X] Token生成API调用失败" -ForegroundColor Red
    Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  请确保服务已启动: http://localhost:5195" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  测试完成" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

