# 批量更新命名空间脚本
# 使用前请先备份项目！

param(
    [string]$ProjectPath = "src\Backend",
    [switch]$DryRun = $false
)

Write-Host "========================================" -ForegroundColor Green
Write-Host "  批量更新命名空间" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

if ($DryRun) {
    Write-Host "[模式] 试运行模式（不会实际修改文件）" -ForegroundColor Yellow
    Write-Host ""
}

$replacements = @(
    @{ Old = "namespace SmallTarget.DbService"; New = "namespace UnifiedPlatform.DbService" },
    @{ Old = "namespace SmallTarget.Shared"; New = "namespace UnifiedPlatform.Shared" },
    @{ Old = "namespace SmallTarget.WebApi"; New = "namespace UnifiedPlatform.WebApi" },
    @{ Old = "using SmallTarget.DbService"; New = "using UnifiedPlatform.DbService" },
    @{ Old = "using SmallTarget.Shared"; New = "using UnifiedPlatform.Shared" },
    @{ Old = "using SmallTarget.WebApi"; New = "using UnifiedPlatform.WebApi" }
)

$files = Get-ChildItem -Path $ProjectPath -Filter "*.cs" -Recurse

$totalFiles = 0
$modifiedFiles = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $modified = $false
    
    foreach ($replacement in $replacements) {
        if ($content -match [regex]::Escape($replacement.Old)) {
            $content = $content -replace [regex]::Escape($replacement.Old), $replacement.New
            $modified = $true
        }
    }
    
    if ($modified) {
        $totalFiles++
        Write-Host "[更新] $($file.FullName)" -ForegroundColor Cyan
        
        if (-not $DryRun) {
            Set-Content -Path $file.FullName -Value $content -NoNewline
            $modifiedFiles++
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
if ($DryRun) {
    Write-Host "  试运行完成：找到 $totalFiles 个文件需要更新" -ForegroundColor Yellow
    Write-Host "  运行脚本时去掉 -DryRun 参数以实际更新" -ForegroundColor Yellow
} else {
    Write-Host "  更新完成：已修改 $modifiedFiles 个文件" -ForegroundColor Green
}
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "使用方法：" -ForegroundColor Cyan
Write-Host "  试运行: .\update_namespaces.ps1 -DryRun" -ForegroundColor White
Write-Host "  实际更新: .\update_namespaces.ps1" -ForegroundColor White
Write-Host ""

