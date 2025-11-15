# API 测试脚本
# 用于测试后端 API 接口是否正常工作

param(
    [string]$BaseUrl = "http://localhost:5000"
)

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Continue"

Write-Host "========================================" -ForegroundColor Green
Write-Host "  API 测试脚本" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "测试地址: $BaseUrl`n" -ForegroundColor Cyan

$testResults = @()
$totalTests = 0
$passedTests = 0
$failedTests = 0

function Test-ApiEndpoint {
    param(
        [string]$Name,
        [string]$Url,
        [string]$Method = "Get",
        [object]$Body = $null
    )
    
    $totalTests++
    Write-Host "[测试] $Name" -ForegroundColor Yellow
    Write-Host "  URL: $Url" -ForegroundColor Gray
    
    try {
        if ($Method -eq "Get") {
            $response = Invoke-RestMethod -Uri $Url -Method Get -ErrorAction Stop
        } else {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Body ($Body | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
        }
        
        if ($response.succeed) {
            Write-Host "  [OK] 测试通过" -ForegroundColor Green
            $script:passedTests++
            $script:testResults += @{ Name = $Name; Status = "Passed"; Message = "成功" }
            return $true
        } else {
            Write-Host "  [FAIL] API 返回失败: $($response.errorMessage)" -ForegroundColor Red
            $script:failedTests++
            $script:testResults += @{ Name = $Name; Status = "Failed"; Message = $response.errorMessage }
            return $false
        }
    } catch {
        Write-Host "  [FAIL] 请求失败: $($_.Exception.Message)" -ForegroundColor Red
        $script:failedTests++
        $script:testResults += @{ Name = $Name; Status = "Failed"; Message = $_.Exception.Message }
        return $false
    }
}

# 1. 测试商品分类
Write-Host "`n[1] 商品分类 API" -ForegroundColor Cyan
Test-ApiEndpoint -Name "获取商品分类" -Url "$BaseUrl/api/store/categories"

# 2. 测试商品列表
Write-Host "`n[2] 商品列表 API" -ForegroundColor Cyan
Test-ApiEndpoint -Name "获取商品列表" -Url "$BaseUrl/api/store/products?page=1&pageSize=5"

# 3. 测试商品搜索
Write-Host "`n[3] 商品搜索 API" -ForegroundColor Cyan
Test-ApiEndpoint -Name "商品搜索" -Url "$BaseUrl/api/store/products?keyword=test&page=1&pageSize=5"

# 4. 获取第一个商品ID用于后续测试
Write-Host "`n[4] 获取商品详情" -ForegroundColor Cyan
try {
    $productsResponse = Invoke-RestMethod -Uri "$BaseUrl/api/store/products?page=1&pageSize=1" -Method Get
    if ($productsResponse.succeed -and $productsResponse.data.items.Count -gt 0) {
        $productId = $productsResponse.data.items[0].productId
        Write-Host "  找到商品 ID: $productId" -ForegroundColor Gray
        
        # 测试商品详情
        Test-ApiEndpoint -Name "商品详情" -Url "$BaseUrl/api/store/products/$productId"
        
        # 测试商品图片
        Test-ApiEndpoint -Name "商品图片" -Url "$BaseUrl/api/store/products/$productId/images"
        
        # 测试商品规格
        Test-ApiEndpoint -Name "商品规格" -Url "$BaseUrl/api/store/products/$productId/specifications"
        
        # 测试商品评价
        Test-ApiEndpoint -Name "商品评价" -Url "$BaseUrl/api/store/products/$productId/reviews?page=1&pageSize=5"
    } else {
        Write-Host "  [WARN] 没有商品数据，跳过商品详情测试" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  [WARN] 无法获取商品列表，跳过商品详情测试" -ForegroundColor Yellow
}

# 5. 测试购物车（需要用户ID，这里只测试接口是否存在）
Write-Host "`n[5] 购物车 API" -ForegroundColor Cyan
Write-Host "  [INFO] 购物车 API 需要用户认证，跳过测试" -ForegroundColor Gray

# 6. 测试订单（需要用户ID，这里只测试接口是否存在）
Write-Host "`n[6] 订单 API" -ForegroundColor Cyan
Write-Host "  [INFO] 订单 API 需要用户认证，跳过测试" -ForegroundColor Gray

# 输出测试结果
Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  测试结果汇总" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "总测试数: $totalTests" -ForegroundColor Cyan
Write-Host "通过: $passedTests" -ForegroundColor Green
Write-Host "失败: $failedTests" -ForegroundColor Red
Write-Host "成功率: $([math]::Round($passedTests / $totalTests * 100, 2))%" -ForegroundColor $(if ($failedTests -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

if ($failedTests -eq 0) {
    Write-Host "所有测试通过！✅" -ForegroundColor Green
    exit 0
} else {
    Write-Host "部分测试失败，请检查后端服务状态" -ForegroundColor Yellow
    exit 1
}

