# 后端自动部署脚本
# 功能：从 GitHub 下载最新构建，停止旧服务，部署新版本，启动服务

param(
    [string]$GitHubToken = "",
    [string]$DownloadPath = "$PSScriptRoot\..\..\src\publish\backend",
    [string]$ServicePath = "$PSScriptRoot\..\..\src\Backend\UnifiedPlatform.WebApi",
    [switch]$SkipBackup = $false,
    [switch]$SkipMigration = $false
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   后端自动部署脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 dotnet 是否安装
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "[错误] 未找到 dotnet 命令" -ForegroundColor Red
    Write-Host "请先安装 .NET SDK 8.0" -ForegroundColor Yellow
    exit 1
}

# 检查是否在正确的目录
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not (Test-Path "$ProjectRoot\src\Backend\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj")) {
    Write-Host "[错误] 未找到后端项目文件" -ForegroundColor Red
    exit 1
}

Set-Location $ProjectRoot

# 1. 停止现有服务
Write-Host "[步骤 1/5] 停止现有后端服务..." -ForegroundColor Yellow
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -like "*UnifiedPlatform.WebApi*" -or 
    $_.CommandLine -like "*UnifiedPlatform.WebApi*"
}

if ($dotnetProcesses) {
    Write-Host "找到 $($dotnetProcesses.Count) 个后端服务进程，正在停止..." -ForegroundColor Yellow
    $dotnetProcesses | Stop-Process -Force
    Start-Sleep -Seconds 2
    Write-Host "[OK] 已停止现有服务" -ForegroundColor Green
} else {
    Write-Host "[信息] 未找到运行中的后端服务" -ForegroundColor Gray
}

# 2. 备份现有部署（如果存在）
if (-not $SkipBackup) {
    Write-Host "[步骤 2/5] 备份现有部署..." -ForegroundColor Yellow
    $backupPath = "$DownloadPath.backup.$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    if (Test-Path $DownloadPath) {
        Copy-Item -Path $DownloadPath -Destination $backupPath -Recurse -Force
        Write-Host "[OK] 已备份到: $backupPath" -ForegroundColor Green
    } else {
        Write-Host "[信息] 未找到现有部署，跳过备份" -ForegroundColor Gray
    }
}

# 3. 编译和发布
Write-Host "[步骤 3/5] 编译和发布后端..." -ForegroundColor Yellow
$SolutionPath = "src\Backend\UnifiedPlatform.sln"
$ProjectPath = "src\Backend\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj"

Write-Host "正在还原依赖..." -ForegroundColor Gray
dotnet restore $SolutionPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "[错误] 依赖还原失败" -ForegroundColor Red
    exit 1
}

Write-Host "正在编译项目..." -ForegroundColor Gray
dotnet build $SolutionPath --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "[错误] 编译失败" -ForegroundColor Red
    exit 1
}

Write-Host "正在发布项目..." -ForegroundColor Gray
$publishPath = $DownloadPath
if (Test-Path $publishPath) {
    Remove-Item -Path $publishPath -Recurse -Force
}
New-Item -ItemType Directory -Path $publishPath -Force | Out-Null

dotnet publish $ProjectPath `
    --configuration Release `
    --output $publishPath `
    --no-build

if ($LASTEXITCODE -ne 0) {
    Write-Host "[错误] 发布失败" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] 编译和发布完成" -ForegroundColor Green

# 4. 运行数据库迁移（可选）
if (-not $SkipMigration) {
    Write-Host "[步骤 4/5] 检查数据库迁移..." -ForegroundColor Yellow
    $migrationCheck = & dotnet ef migrations list `
        --project "src\Backend\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj" `
        --startup-project $ProjectPath `
        --context StDbContext 2>&1
    
    $pendingMigrations = $migrationCheck | Select-String "Pending"
    if ($pendingMigrations) {
        Write-Host "[警告] 发现待应用的迁移:" -ForegroundColor Yellow
        $pendingMigrations | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
        $apply = Read-Host "是否应用这些迁移? (y/n)"
        if ($apply -eq "y" -or $apply -eq "Y") {
            Write-Host "正在应用数据库迁移..." -ForegroundColor Gray
            dotnet ef database update `
                --project "src\Backend\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj" `
                --startup-project $ProjectPath `
                --context StDbContext
            if ($LASTEXITCODE -ne 0) {
                Write-Host "[错误] 数据库迁移失败" -ForegroundColor Red
                exit 1
            }
            Write-Host "[OK] 数据库迁移完成" -ForegroundColor Green
        }
    } else {
        Write-Host "[OK] 数据库迁移状态正常" -ForegroundColor Green
    }
}

# 5. 启动服务
Write-Host "[步骤 5/5] 启动后端服务..." -ForegroundColor Yellow
Write-Host ""
Write-Host "服务将在以下地址运行:" -ForegroundColor Cyan
Write-Host "  - HTTP: http://localhost:5000" -ForegroundColor White
Write-Host "  - Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  - Health: http://localhost:5000/health" -ForegroundColor White
Write-Host ""

$startChoice = Read-Host "是否现在启动服务? (y/n)"
if ($startChoice -eq "y" -or $startChoice -eq "Y") {
    Set-Location $publishPath
    Start-Process -FilePath "dotnet" -ArgumentList "UnifiedPlatform.WebApi.dll" -NoNewWindow
    Write-Host "[OK] 服务已启动" -ForegroundColor Green
    Write-Host ""
    Write-Host "提示: 服务在后台运行，可以使用 '检查服务状态.bat' 查看状态" -ForegroundColor Yellow
} else {
    Write-Host "[信息] 跳过启动，您可以稍后手动启动服务" -ForegroundColor Gray
    Write-Host "启动命令: cd $publishPath && dotnet UnifiedPlatform.WebApi.dll" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   部署完成" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

