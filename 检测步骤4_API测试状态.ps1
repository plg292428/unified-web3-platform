# 步骤4：测试API 检测脚本

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  步骤4：测试API 检测" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. 检查服务运行状态
Write-Host "[1] 检查服务运行状态..." -ForegroundColor Yellow
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($dotnetProcesses) {
    Write-Host "  状态: [OK] 发现 $($dotnetProcesses.Count) 个 dotnet 进程" -ForegroundColor Green
    Write-Host "  注意: 需要确认是否有 UnifiedPlatform.WebApi 进程" -ForegroundColor Gray
} else {
    Write-Host "  状态: [X] 未发现 dotnet 进程" -ForegroundColor Red
}
Write-Host ""

# 2. 检查端口监听
Write-Host "[2] 检查端口监听状态..." -ForegroundColor Yellow
$port5195 = Get-NetTCPConnection -LocalPort 5195 -State Listen -ErrorAction SilentlyContinue
$port5196 = Get-NetTCPConnection -LocalPort 5196 -State Listen -ErrorAction SilentlyContinue

if ($port5195) {
    Write-Host "  端口 5195 (HTTP): [OK] 正在监听" -ForegroundColor Green
} else {
    Write-Host "  端口 5195 (HTTP): [X] 未监听" -ForegroundColor Red
}

if ($port5196) {
    Write-Host "  端口 5196 (HTTPS): [OK] 正在监听" -ForegroundColor Green
} else {
    Write-Host "  端口 5196 (HTTPS): [X] 未监听" -ForegroundColor Red
}
Write-Host ""

# 3. 检查Swagger配置
Write-Host "[3] 检查Swagger配置..." -ForegroundColor Yellow
$programPath = "src\Backend\UnifiedPlatform.WebApi\Program.cs"
if (Test-Path $programPath) {
    $swaggerConfig = Get-Content $programPath | Select-String -Pattern "AddSwaggerGen|UseSwagger"
    if ($swaggerConfig) {
        Write-Host "  状态: [OK] Swagger已配置" -ForegroundColor Green
    } else {
        Write-Host "  状态: [X] Swagger未配置" -ForegroundColor Red
    }
} else {
    Write-Host "  状态: [X] Program.cs 不存在" -ForegroundColor Red
}
Write-Host ""

# 4. 检查TronController
Write-Host "[4] 检查TronController..." -ForegroundColor Yellow
$controllerPath = "src\Backend\UnifiedPlatform.WebApi\Controllers\TronController.cs"
if (Test-Path $controllerPath) {
    Write-Host "  状态: [OK] TronController 存在" -ForegroundColor Green
    
    # 统计API端点
    $endpoints = Get-Content $controllerPath | Select-String -Pattern "\[Http(Get|Post|Put|Delete)\]"
    $endpointCount = ($endpoints | Measure-Object).Count
    Write-Host "  API端点数量: $endpointCount" -ForegroundColor Gray
} else {
    Write-Host "  状态: [X] TronController 不存在" -ForegroundColor Red
}
Write-Host ""

# 5. 测试Swagger可访问性
Write-Host "[5] 测试Swagger可访问性..." -ForegroundColor Yellow
if ($port5195) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5195/swagger/index.html" -Method Get -TimeoutSec 3 -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            Write-Host "  状态: [OK] Swagger可访问" -ForegroundColor Green
            Write-Host "  URL: http://localhost:5195/swagger" -ForegroundColor Gray
        }
    } catch {
        Write-Host "  状态: [X] Swagger不可访问" -ForegroundColor Red
        Write-Host "  错误: $($_.Exception.Message)" -ForegroundColor Gray
    }
} else {
    Write-Host "  状态: [X] 服务未运行，无法测试" -ForegroundColor Yellow
}
Write-Host ""

# 汇总
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  检测结果汇总" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$allChecks = @(
    @{ Name = "Swagger配置"; Status = if ($swaggerConfig) { "OK" } else { "X" } },
    @{ Name = "TronController存在"; Status = if (Test-Path $controllerPath) { "OK" } else { "X" } },
    @{ Name = "服务运行状态"; Status = if ($port5195 -or $port5196) { "OK" } else { "待启动" } },
    @{ Name = "Swagger可访问"; Status = if ($port5195) { "待测试" } else { "待启动" } }
)

foreach ($check in $allChecks) {
    $color = if ($check.Status -eq "OK") { "Green" } elseif ($check.Status -eq "待启动" -or $check.Status -eq "待测试") { "Yellow" } else { "Red" }
    Write-Host "  [$($check.Status)] $($check.Name)" -ForegroundColor $color
}

Write-Host ""
Write-Host "详细报告已保存到:" -ForegroundColor Cyan
Write-Host "  步骤4_API测试检测报告.md" -ForegroundColor Gray
Write-Host ""

