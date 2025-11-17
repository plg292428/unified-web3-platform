@echo off
chcp 65001 >nul
title 快速部署后端（跳过迁移和备份）

echo.
echo ========================================
echo   快速部署后端服务
echo ========================================
echo.
echo [提示] 此脚本将跳过备份和数据库迁移检查
echo.

cd /d "%~dp0\..\.."

powershell -ExecutionPolicy Bypass -File "配置文件\批处理脚本\自动部署后端.ps1" -SkipBackup -SkipMigration

pause

