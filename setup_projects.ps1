# UnifiedWeb3Platform项目初始化脚本
# 复制项目文件、创建.NET解决方案和项目文件

Write-Host "========================================" -ForegroundColor Green
Write-Host "  UnifiedWeb3Platform项目初始化" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# 获取脚本所在目录
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$plgDir = Split-Path -Parent $scriptDir
$projectDir = Join-Path $plgDir "UnifiedWeb3Platform"
$srcDir = Join-Path $projectDir "src"

Set-Location $projectDir
Write-Host "[信息] 项目目录: $projectDir" -ForegroundColor Cyan
Write-Host ""

# 步骤1: 复制HFastKit项目
Write-Host "[步骤1] 复制HFastKit项目..." -ForegroundColor Yellow
$hfastkitSource = Join-Path $plgDir "Projects\HFastKit\HFastKit-master"
$hfastkitTarget = Join-Path $srcDir "Libraries\HFastKit"

if (Test-Path $hfastkitSource) {
    if (Test-Path $hfastkitTarget) {
        Write-Host "  ℹ  目标目录已存在，跳过复制" -ForegroundColor Yellow
    } else {
        Copy-Item -Path $hfastkitSource -Destination $hfastkitTarget -Recurse -Force
        Write-Host "  ✓ 已复制HFastKit项目" -ForegroundColor Green
    }
} else {
    Write-Host "  ⚠  HFastKit源目录不存在: $hfastkitSource" -ForegroundColor Red
}
Write-Host ""

# 步骤2: 复制Nblockchain项目
Write-Host "[步骤2] 复制Nblockchain项目..." -ForegroundColor Yellow
$nblockchainSource = Join-Path $plgDir "Project20251105\Nblockchain-master"
$nblockchainTarget = Join-Path $srcDir "Libraries\Nblockchain"

if (Test-Path $nblockchainSource) {
    if (Test-Path $nblockchainTarget) {
        Write-Host "  ℹ  目标目录已存在，跳过复制" -ForegroundColor Yellow
    } else {
        Copy-Item -Path $nblockchainSource -Destination $nblockchainTarget -Recurse -Force
        Write-Host "  ✓ 已复制Nblockchain项目" -ForegroundColor Green
    }
} else {
    Write-Host "  ⚠  Nblockchain源目录不存在: $nblockchainSource" -ForegroundColor Red
}
Write-Host ""

# 步骤3: 复制SmallTarget前端项目
Write-Host "[步骤3] 复制SmallTarget前端项目..." -ForegroundColor Yellow
$smalltargetSource = Join-Path $plgDir "Projects\SmallTarget\SmallTarget-master\SmallTarget.DappFrontEnd"
$smalltargetTarget = Join-Path $srcDir "Frontend\web-app"

if (Test-Path $smalltargetSource) {
    if (Test-Path $smalltargetTarget) {
        $existingFiles = Get-ChildItem -Path $smalltargetTarget -File
        if ($existingFiles.Count -gt 0) {
            Write-Host "  ℹ  目标目录已存在文件，跳过复制" -ForegroundColor Yellow
        } else {
            Copy-Item -Path "$smalltargetSource\*" -Destination $smalltargetTarget -Recurse -Force
            Write-Host "  ✓ 已复制SmallTarget前端项目" -ForegroundColor Green
        }
    } else {
        Copy-Item -Path "$smalltargetSource\*" -Destination $smalltargetTarget -Recurse -Force
        Write-Host "  ✓ 已复制SmallTarget前端项目" -ForegroundColor Green
    }
} else {
    Write-Host "  ⚠  SmallTarget前端源目录不存在: $smalltargetSource" -ForegroundColor Red
}
Write-Host ""

# 步骤4: 检查.NET SDK
Write-Host "[步骤4] 检查.NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "  ✓ .NET SDK版本: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "  ❌ .NET SDK未安装，请先安装.NET 8 SDK" -ForegroundColor Red
    Write-Host "  下载地址: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
    exit 1
}
Write-Host ""

# 步骤5: 创建.NET解决方案
Write-Host "[步骤5] 创建.NET解决方案..." -ForegroundColor Yellow
$backendDir = Join-Path $srcDir "Backend"
$slnPath = Join-Path $backendDir "UnifiedPlatform.sln"

if (Test-Path $slnPath) {
    Write-Host "  ℹ  解决方案文件已存在" -ForegroundColor Yellow
} else {
    Set-Location $backendDir
    dotnet new sln -n UnifiedPlatform
    Write-Host "  ✓ 已创建解决方案: UnifiedPlatform.sln" -ForegroundColor Green
}
Write-Host ""

# 步骤6: 创建.NET项目
Write-Host "[步骤6] 创建.NET项目..." -ForegroundColor Yellow
Set-Location $backendDir

# 创建WebApi项目
$webApiPath = Join-Path $backendDir "UnifiedPlatform.WebApi"
if (-not (Test-Path (Join-Path $webApiPath "UnifiedPlatform.WebApi.csproj"))) {
    dotnet new webapi -n UnifiedPlatform.WebApi -f net8.0 -o UnifiedPlatform.WebApi
    dotnet sln add UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj
    Write-Host "  ✓ 已创建WebApi项目" -ForegroundColor Green
} else {
    Write-Host "  ℹ  WebApi项目已存在" -ForegroundColor Yellow
}

# 创建DbService项目
$dbServicePath = Join-Path $backendDir "UnifiedPlatform.DbService"
if (-not (Test-Path (Join-Path $dbServicePath "UnifiedPlatform.DbService.csproj"))) {
    dotnet new classlib -n UnifiedPlatform.DbService -f net8.0 -o UnifiedPlatform.DbService
    dotnet sln add UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj
    Write-Host "  ✓ 已创建DbService项目" -ForegroundColor Green
} else {
    Write-Host "  ℹ  DbService项目已存在" -ForegroundColor Yellow
}

# 创建Shared项目
$sharedPath = Join-Path $backendDir "UnifiedPlatform.Shared"
if (-not (Test-Path (Join-Path $sharedPath "UnifiedPlatform.Shared.csproj"))) {
    dotnet new classlib -n UnifiedPlatform.Shared -f net8.0 -o UnifiedPlatform.Shared
    dotnet sln add UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj
    Write-Host "  ✓ 已创建Shared项目" -ForegroundColor Green
} else {
    Write-Host "  ℹ  Shared项目已存在" -ForegroundColor Yellow
}
Write-Host ""

# 步骤7: 配置项目引用
Write-Host "[步骤7] 配置项目引用..." -ForegroundColor Yellow
Set-Location $backendDir

# WebApi引用DbService和Shared
Set-Location $webApiPath
dotnet add reference ..\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj 2>$null
dotnet add reference ..\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj 2>$null
Write-Host "  ✓ 已配置WebApi项目引用" -ForegroundColor Green

# DbService引用Shared
Set-Location $dbServicePath
dotnet add reference ..\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj 2>$null
Write-Host "  ✓ 已配置DbService项目引用" -ForegroundColor Green
Write-Host ""

# 步骤8: 显示项目结构
Write-Host "========================================" -ForegroundColor Green
Write-Host "  项目结构：" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Get-ChildItem -Path $projectDir -Recurse -Directory -Depth 2 | Select-Object FullName | ForEach-Object {
    $relativePath = $_.FullName.Replace($projectDir + "\", "")
    Write-Host "  $relativePath" -ForegroundColor White
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "  ✓ 项目初始化完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "[下一步] 安装前端依赖：" -ForegroundColor Cyan
Write-Host "  cd src\Frontend\web-app" -ForegroundColor White
Write-Host "  npm install" -ForegroundColor White
Write-Host ""

