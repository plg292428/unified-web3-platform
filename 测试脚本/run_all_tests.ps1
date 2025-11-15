# 运行所有测试脚本

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Continue"

Write-Host "========================================" -ForegroundColor Green
Write-Host "  运行所有测试" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$scriptPath = $PSScriptRoot

# 1. 数据库测试
Write-Host "[1/2] 运行数据库测试..." -ForegroundColor Cyan
& "$scriptPath\test_database.ps1"
Write-Host ""

# 2. API 测试
Write-Host "[2/2] 运行 API 测试..." -ForegroundColor Cyan
Write-Host "提示: 请确保后端服务正在运行 (http://localhost:5000)" -ForegroundColor Yellow
Write-Host ""
Start-Sleep -Seconds 2
& "$scriptPath\test_api.ps1"

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  所有测试完成" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

