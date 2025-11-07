# 项目完成状态检测脚本
$ErrorActionPreference = "Continue"
Write-Host "========================================" -ForegroundColor Green
Write-Host "  UnifiedWeb3Platform 项目完成状态检测" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$basePath = $PSScriptRoot
$completed = 0
$total = 0

# 1. 检查目录结构
Write-Host "[1] 检查项目目录结构..." -ForegroundColor Cyan
$total++
$dirs = @(
    "src\Frontend\web-app",
    "src\Backend\UnifiedPlatform.WebApi",
    "src\Backend\UnifiedPlatform.DbService",
    "src\Backend\UnifiedPlatform.Shared",
    "src\Libraries\HFastKit",
    "src\Libraries\Nblockchain"
)

$dirsOk = $true
foreach ($dir in $dirs) {
    $fullPath = Join-Path $basePath $dir
    if (Test-Path $fullPath) {
        $fileCount = (Get-ChildItem -Path $fullPath -Recurse -File -ErrorAction SilentlyContinue).Count
        Write-Host "  [OK] $dir (包含 $fileCount 个文件)" -ForegroundColor Green
    } else {
        Write-Host "  [失败] $dir 不存在" -ForegroundColor Red
        $dirsOk = $false
    }
}

if ($dirsOk) {
    $completed++
    Write-Host "  [✓] 目录结构检查通过" -ForegroundColor Green
} else {
    Write-Host "  [✗] 目录结构不完整" -ForegroundColor Red
}
Write-Host ""

# 2. 检查解决方案文件
Write-Host "[2] 检查解决方案文件..." -ForegroundColor Cyan
$total++
$slnPath = Join-Path $basePath "UnifiedPlatform.sln"
if (Test-Path $slnPath) {
    Write-Host "  [OK] UnifiedPlatform.sln 存在" -ForegroundColor Green
    $completed++
} else {
    Write-Host "  [失败] UnifiedPlatform.sln 不存在" -ForegroundColor Red
}
Write-Host ""

# 3. 检查后端项目文件
Write-Host "[3] 检查后端项目文件..." -ForegroundColor Cyan
$total++
$projects = @(
    "src\Backend\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj",
    "src\Backend\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj",
    "src\Backend\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj"
)

$projectsOk = $true
foreach ($proj in $projects) {
    $fullPath = Join-Path $basePath $proj
    if (Test-Path $fullPath) {
        Write-Host "  [OK] $proj" -ForegroundColor Green
    } else {
        Write-Host "  [失败] $proj 不存在" -ForegroundColor Red
        $projectsOk = $false
    }
}

if ($projectsOk) {
    $completed++
    Write-Host "  [✓] 后端项目文件检查通过" -ForegroundColor Green
} else {
    Write-Host "  [✗] 后端项目文件不完整" -ForegroundColor Red
}
Write-Host ""

# 4. 检查配置文件
Write-Host "[4] 检查配置文件..." -ForegroundColor Cyan
$total++
$configs = @(
    "src\Backend\UnifiedPlatform.WebApi\appsettings.json",
    "src\Backend\UnifiedPlatform.WebApi\Program.cs",
    "src\Backend\UnifiedPlatform.WebApi\Properties\launchSettings.json"
)

$configsOk = $true
foreach ($config in $configs) {
    $fullPath = Join-Path $basePath $config
    if (Test-Path $fullPath) {
        Write-Host "  [OK] $config" -ForegroundColor Green
    } else {
        Write-Host "  [失败] $config 不存在" -ForegroundColor Red
        $configsOk = $false
    }
}

# 检查数据库连接字符串
if ($configsOk) {
    $appsettingsPath = Join-Path $basePath "src\Backend\UnifiedPlatform.WebApi\appsettings.json"
    $appsettings = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
    if ($appsettings.ConnectionStrings.DefaultConnection -like "*SmallTarget*") {
        Write-Host "  [OK] 数据库连接字符串已配置" -ForegroundColor Green
    } else {
        Write-Host "  [警告] 数据库连接字符串可能未正确配置" -ForegroundColor Yellow
    }
    $completed++
} else {
    Write-Host "  [✗] 配置文件不完整" -ForegroundColor Red
}
Write-Host ""

# 5. 检查数据库连接
Write-Host "[5] 检查数据库连接..." -ForegroundColor Cyan
$total++
try {
    $connectionString = $appsettings.ConnectionStrings.DefaultConnection
    if ($connectionString -like "*LocalDB*") {
        # 检查 LocalDB 是否运行
        $localdbStatus = sqllocaldb info MSSQLLocalDB 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [OK] LocalDB 已安装" -ForegroundColor Green
            
            # 尝试连接数据库
            $dbName = "SmallTarget"
            $sqlcmd = "sqlcmd -S (localdb)\MSSQLLocalDB -Q `"SELECT 1 FROM sys.databases WHERE name = '$dbName'`" -h -1 -W"
            $result = cmd /c $sqlcmd 2>&1
            if ($result -match $dbName) {
                Write-Host "  [OK] 数据库 '$dbName' 存在" -ForegroundColor Green
                $completed++
            } else {
                Write-Host "  [警告] 数据库 '$dbName' 可能不存在" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  [失败] LocalDB 未安装或未运行" -ForegroundColor Red
        }
    } else {
        Write-Host "  [信息] 使用非 LocalDB 数据库" -ForegroundColor Yellow
        $completed++
    }
} catch {
    Write-Host "  [警告] 无法检查数据库连接: $($_.Exception.Message)" -ForegroundColor Yellow
}
Write-Host ""

# 6. 检查编译状态
Write-Host "[6] 检查项目编译状态..." -ForegroundColor Cyan
$total++
$webApiPath = Join-Path $basePath "src\Backend\UnifiedPlatform.WebApi"
Push-Location $webApiPath
try {
    $buildOutput = dotnet build --no-incremental 2>&1 | Out-String
    if ($buildOutput -match "0 个错误") {
        Write-Host "  [OK] 项目编译成功，无错误" -ForegroundColor Green
        if ($buildOutput -match "(\d+) 个警告") {
            $warnings = [regex]::Match($buildOutput, "(\d+) 个警告").Groups[1].Value
            Write-Host "  [信息] 编译警告: $warnings 个" -ForegroundColor Yellow
        }
        $completed++
    } else {
        Write-Host "  [失败] 项目编译有错误" -ForegroundColor Red
        if ($buildOutput -match "error") {
            Write-Host "  请查看上面的编译错误信息" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "  [失败] 无法编译项目: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    Pop-Location
}
Write-Host ""

# 7. 检查服务运行状态
Write-Host "[7] 检查服务运行状态..." -ForegroundColor Cyan
$total++
$port = 5195
$portStatus = netstat -ano | Select-String ":$port"
if ($portStatus) {
    Write-Host "  [OK] 端口 $port 正在监听" -ForegroundColor Green
    $pid = ($portStatus | Select-Object -First 1).ToString() -replace '.*\s+(\d+)$', '$1'
    Write-Host "  进程 ID: $pid" -ForegroundColor Gray
    
    # 测试健康检查
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$port/health" -TimeoutSec 3 -ErrorAction Stop
        Write-Host "  [OK] 健康检查成功 (HTTP $($response.StatusCode))" -ForegroundColor Green
        Write-Host "  响应: $($response.Content)" -ForegroundColor Gray
        
        # 测试 Swagger
        try {
            $swaggerResponse = Invoke-WebRequest -Uri "http://localhost:$port/swagger" -TimeoutSec 3 -ErrorAction Stop
            Write-Host "  [OK] Swagger UI 可访问" -ForegroundColor Green
        } catch {
            Write-Host "  [警告] Swagger UI 可能无法访问" -ForegroundColor Yellow
        }
        
        $completed++
    } catch {
        Write-Host "  [失败] 服务未正常响应: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "  [失败] 端口 $port 未监听，服务未运行" -ForegroundColor Red
}
Write-Host ""

# 8. 检查前端
Write-Host "[8] 检查前端项目..." -ForegroundColor Cyan
$total++
$frontendPath = Join-Path $basePath "src\Frontend\web-app"
if (Test-Path $frontendPath) {
    $packageJson = Join-Path $frontendPath "package.json"
    $nodeModules = Join-Path $frontendPath "node_modules"
    
    if (Test-Path $packageJson) {
        Write-Host "  [OK] package.json 存在" -ForegroundColor Green
        if (Test-Path $nodeModules) {
            Write-Host "  [OK] node_modules 已安装" -ForegroundColor Green
            $completed++
        } else {
            Write-Host "  [警告] node_modules 未安装，需要运行 npm install" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  [警告] package.json 不存在" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [失败] 前端目录不存在" -ForegroundColor Red
}
Write-Host ""

# 9. 检查库项目
Write-Host "[9] 检查库项目..." -ForegroundColor Cyan
$total++
$librariesOk = $true
$libraries = @(
    @{Path = "src\Libraries\HFastKit\HFastKit\HFastKit.csproj"; Name = "HFastKit"},
    @{Path = "src\Libraries\HFastKit\HFastKit.AspNetCore\HFastKit.AspNetCore.csproj"; Name = "HFastKit.AspNetCore"},
    @{Path = "src\Libraries\Nblockchain\Nblockchain.Tron\Nblockchain.Tron.csproj"; Name = "Nblockchain.Tron"}
)

foreach ($lib in $libraries) {
    $fullPath = Join-Path $basePath $lib.Path
    if (Test-Path $fullPath) {
        Write-Host "  [OK] $($lib.Name) 项目存在" -ForegroundColor Green
    } else {
        Write-Host "  [失败] $($lib.Name) 项目不存在" -ForegroundColor Red
        $librariesOk = $false
    }
}

if ($librariesOk) {
    $completed++
    Write-Host "  [✓] 库项目检查通过" -ForegroundColor Green
} else {
    Write-Host "  [✗] 库项目不完整" -ForegroundColor Red
}
Write-Host ""

# 总结
Write-Host "========================================" -ForegroundColor Green
Write-Host "  检测完成" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
$percentage = [math]::Round(($completed / $total) * 100, 1)
Write-Host "完成度: $completed / $total ($percentage%)" -ForegroundColor $(if ($percentage -ge 80) { "Green" } elseif ($percentage -ge 50) { "Yellow" } else { "Red" })
Write-Host ""

if ($percentage -ge 80) {
    Write-Host "项目状态: 基本完成，可以开始使用" -ForegroundColor Green
} elseif ($percentage -ge 50) {
    Write-Host "项目状态: 部分完成，需要继续完善" -ForegroundColor Yellow
} else {
    Write-Host "项目状态: 未完成，需要继续搭建" -ForegroundColor Red
}

Write-Host ""
Write-Host "服务访问地址:" -ForegroundColor Cyan
Write-Host "  • HTTP:  http://localhost:$port" -ForegroundColor White
Write-Host "  • Swagger: http://localhost:$port/swagger" -ForegroundColor White
Write-Host "  • Health: http://localhost:$port/health" -ForegroundColor White
Write-Host ""

