# 项目完成情况检测脚本
# ========================================

Write-Host "========================================" -ForegroundColor Green
Write-Host "   UnifiedWeb3Platform 项目完成情况检测" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$basePath = $PSScriptRoot
$checkResults = @()

# 1. 检查项目目录结构
Write-Host "[1] 检查项目目录结构..." -ForegroundColor Cyan
$requiredDirs = @(
    "src\Frontend\web-app",
    "src\Backend\UnifiedPlatform.WebApi",
    "src\Backend\UnifiedPlatform.DbService",
    "src\Backend\UnifiedPlatform.Shared",
    "src\Libraries\HFastKit",
    "src\Libraries\Nblockchain"
)

foreach ($dir in $requiredDirs) {
    $fullPath = Join-Path $basePath $dir
    $exists = Test-Path $fullPath
    $status = if ($exists) { "✓" } else { "✗" }
    $color = if ($exists) { "Green" } else { "Red" }
    Write-Host "  $status $dir" -ForegroundColor $color
    $checkResults += @{
        Item = "目录: $dir"
        Status = $exists
    }
}
Write-Host ""

# 2. 检查关键文件
Write-Host "[2] 检查关键文件..." -ForegroundColor Cyan
$requiredFiles = @(
    "src\Backend\UnifiedPlatform.WebApi\Program.cs",
    "src\Backend\UnifiedPlatform.WebApi\appsettings.json",
    "src\Backend\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj",
    "src\Backend\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj",
    "src\Backend\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj",
    "UnifiedPlatform.sln"
)

foreach ($file in $requiredFiles) {
    $fullPath = Join-Path $basePath $file
    $exists = Test-Path $fullPath
    $status = if ($exists) { "✓" } else { "✗" }
    $color = if ($exists) { "Green" } else { "Red" }
    Write-Host "  $status $file" -ForegroundColor $color
    $checkResults += @{
        Item = "文件: $file"
        Status = $exists
    }
}
Write-Host ""

# 3. 检查数据库配置
Write-Host "[3] 检查数据库配置..." -ForegroundColor Cyan
$appsettingsPath = Join-Path $basePath "src\Backend\UnifiedPlatform.WebApi\appsettings.json"
if (Test-Path $appsettingsPath) {
    $config = Get-Content $appsettingsPath | ConvertFrom-Json
    $connStr = $config.ConnectionStrings.DefaultConnection
    if ($connStr) {
        Write-Host "  ✓ 数据库连接字符串已配置" -ForegroundColor Green
        Write-Host "    连接: $($connStr.Substring(0, [Math]::Min(80, $connStr.Length)))..." -ForegroundColor Gray
        $checkResults += @{
            Item = "数据库连接字符串"
            Status = $true
        }
    } else {
        Write-Host "  ✗ 数据库连接字符串未配置" -ForegroundColor Red
        $checkResults += @{
            Item = "数据库连接字符串"
            Status = $false
        }
    }
} else {
    Write-Host "  ✗ appsettings.json 不存在" -ForegroundColor Red
    $checkResults += @{
        Item = "数据库连接字符串"
        Status = $false
    }
}
Write-Host ""

# 4. 检查数据库连接
Write-Host "[4] 检查数据库连接..." -ForegroundColor Cyan
try {
    $dbName = "SmallTarget"
    $sqlcmd = "SELECT COUNT(*) FROM sys.databases WHERE name = '$dbName'"
    $result = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q $sqlcmd -h -1 -W 2>&1
    if ($result -match "1") {
        Write-Host "  ✓ 数据库 '$dbName' 存在" -ForegroundColor Green
        $checkResults += @{
            Item = "数据库存在"
            Status = $true
        }
    } else {
        Write-Host "  ✗ 数据库 '$dbName' 不存在" -ForegroundColor Red
        $checkResults += @{
            Item = "数据库存在"
            Status = $false
        }
    }
} catch {
    Write-Host "  ⚠ 无法检查数据库（可能需要安装 LocalDB）" -ForegroundColor Yellow
    $checkResults += @{
        Item = "数据库存在"
        Status = $false
    }
}
Write-Host ""

# 5. 检查项目编译
Write-Host "[5] 检查项目编译..." -ForegroundColor Cyan
$slnPath = Join-Path $basePath "UnifiedPlatform.sln"
if (Test-Path $slnPath) {
    Push-Location $basePath
    try {
        $buildOutput = dotnet build $slnPath --no-incremental 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✓ 项目编译成功" -ForegroundColor Green
            $checkResults += @{
                Item = "项目编译"
                Status = $true
            }
        } else {
            Write-Host "  ✗ 项目编译失败" -ForegroundColor Red
            $errorLines = $buildOutput | Select-String -Pattern "error" | Select-Object -First 3
            foreach ($line in $errorLines) {
                Write-Host "    $line" -ForegroundColor Red
            }
            $checkResults += @{
                Item = "项目编译"
                Status = $false
            }
        }
    } catch {
        Write-Host "  ✗ 编译检查出错: $_" -ForegroundColor Red
        $checkResults += @{
            Item = "项目编译"
            Status = $false
        }
    } finally {
        Pop-Location
    }
} else {
    Write-Host "  ✗ 解决方案文件不存在" -ForegroundColor Red
    $checkResults += @{
        Item = "项目编译"
        Status = $false
    }
}
Write-Host ""

# 6. 检查服务运行状态
Write-Host "[6] 检查服务运行状态..." -ForegroundColor Cyan
try {
    $port = 5195
    $listening = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($listening) {
        Write-Host "  ✓ 服务正在运行 (端口 $port)" -ForegroundColor Green
        $processId = $listening.OwningProcess
        $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "    进程: $($process.ProcessName) (PID: $processId)" -ForegroundColor Gray
        }
        
        # 测试健康检查
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:$port/health" -TimeoutSec 2 -ErrorAction Stop
            Write-Host "  ✓ 健康检查端点正常 (HTTP $($response.StatusCode))" -ForegroundColor Green
            $checkResults += @{
                Item = "服务运行"
                Status = $true
            }
            $checkResults += @{
                Item = "健康检查"
                Status = $true
            }
        } catch {
            Write-Host "  ✗ 健康检查失败: $_" -ForegroundColor Red
            $checkResults += @{
                Item = "健康检查"
                Status = $false
            }
        }
    } else {
        Write-Host "  ✗ 服务未运行 (端口 $port 未监听)" -ForegroundColor Red
        $checkResults += @{
            Item = "服务运行"
            Status = $false
        }
    }
} catch {
    Write-Host "  ⚠ 无法检查服务状态: $_" -ForegroundColor Yellow
    $checkResults += @{
        Item = "服务运行"
        Status = $false
    }
}
Write-Host ""

# 7. 检查脚本文件
Write-Host "[7] 检查辅助脚本..." -ForegroundColor Cyan
$scripts = @(
    "前台启动服务.bat",
    "停止服务.bat",
    "debug_run.bat",
    "fix_database_connection.ps1"
)

foreach ($script in $scripts) {
    $scriptPath = Join-Path $basePath $script
    $exists = Test-Path $scriptPath
    $status = if ($exists) { "✓" } else { "✗" }
    $color = if ($exists) { "Green" } else { "Yellow" }
    Write-Host "  $status $script" -ForegroundColor $color
}
Write-Host ""

# 8. 检查前端项目
Write-Host "[8] 检查前端项目..." -ForegroundColor Cyan
$frontendPath = Join-Path $basePath "src\Frontend\web-app"
if (Test-Path $frontendPath) {
    $packageJson = Join-Path $frontendPath "package.json"
    $nodeModules = Join-Path $frontendPath "node_modules"
    
    if (Test-Path $packageJson) {
        Write-Host "  ✓ package.json 存在" -ForegroundColor Green
    } else {
        Write-Host "  ✗ package.json 不存在" -ForegroundColor Red
    }
    
    if (Test-Path $nodeModules) {
        Write-Host "  ✓ node_modules 已安装" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ node_modules 未安装" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ✗ 前端目录不存在" -ForegroundColor Red
}
Write-Host ""

# 总结
Write-Host "========================================" -ForegroundColor Green
Write-Host "  检测结果总结" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

$total = $checkResults.Count
$passed = ($checkResults | Where-Object { $_.Status -eq $true }).Count
$failed = $total - $passed

Write-Host "总检查项: $total" -ForegroundColor White
Write-Host "通过: $passed" -ForegroundColor Green
Write-Host "失败: $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($failed -eq 0) {
    Write-Host "✓ 所有检查项通过！项目状态良好。" -ForegroundColor Green
} else {
    Write-Host "⚠ 部分检查项未通过，请查看上面的详细信息。" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "服务访问地址:" -ForegroundColor Cyan
Write-Host "  • HTTP:  http://localhost:5195" -ForegroundColor White
Write-Host "  • Swagger: http://localhost:5195/swagger" -ForegroundColor White
Write-Host "  • Health: http://localhost:5195/health" -ForegroundColor White
Write-Host ""

