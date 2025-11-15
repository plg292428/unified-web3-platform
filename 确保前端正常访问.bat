@echo off
chcp 65001 >nul
title 确保前端正常访问 - 完整检查

echo.
echo ========================================
echo   确保前端正常访问 - 完整检查
echo ========================================
echo.

cd /d "%~dp0"

echo [检查 1/5] 检查后端服务状态...
netstat -ano | findstr ":5000" | findstr "LISTENING" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] 后端服务正在运行
) else (
    echo [错误] 后端服务未运行
    echo [启动] 正在启动后端服务...
    start "后端服务" /min powershell -NoExit -Command "cd '%~dp0src\Backend\UnifiedPlatform.WebApi'; Write-Host '后端服务运行中...' -ForegroundColor Green; Write-Host '请保持此窗口打开' -ForegroundColor Yellow; Write-Host ''; dotnet run"
    timeout /t 10 /nobreak >nul
    echo [OK] 后端服务已启动，等待初始化...
    timeout /t 5 /nobreak >nul
)

echo.
echo [检查 2/5] 检查 Cloudflare Tunnel 状态...
tasklist | findstr "cloudflared.exe" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Cloudflare Tunnel 正在运行
) else (
    echo [错误] Cloudflare Tunnel 未运行
    echo [启动] 正在启动 Cloudflare Tunnel...
    if exist "%~dp0cloudflared.exe" (
        start "Cloudflare Tunnel" /min powershell -NoExit -Command "cd '%~dp0'; Write-Host 'Cloudflare Tunnel 运行中...' -ForegroundColor Green; Write-Host '请保持此窗口打开' -ForegroundColor Yellow; Write-Host ''; .\cloudflared.exe tunnel run api"
        timeout /t 5 /nobreak >nul
        echo [OK] Cloudflare Tunnel 已启动
    ) else (
        echo [警告] 未找到 cloudflared.exe
        echo [提示] 请手动启动 Tunnel 或检查配置
    )
)

echo.
echo [检查 3/5] 检查本地后端 API...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://127.0.0.1:5000/health' -Method GET -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop; Write-Host '[OK] 本地后端 API 可访问' -ForegroundColor Green } catch { Write-Host '[错误] 本地后端 API 不可访问' -ForegroundColor Red; Write-Host '      请检查后端服务是否正常启动' -ForegroundColor Yellow }"

echo.
echo [检查 4/5] 检查公网后端 API...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'https://api.a292428dsj.dpdns.org/health' -Method GET -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop; Write-Host '[OK] 公网后端 API 可访问' -ForegroundColor Green } catch { Write-Host '[警告] 公网后端 API 不可访问' -ForegroundColor Yellow; Write-Host '      请检查 Cloudflare Tunnel 是否正常连接' -ForegroundColor Yellow }"

echo.
echo [检查 5/5] 检查前端网站...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'https://www.a292428dsj.dpdns.org' -Method GET -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop; Write-Host '[OK] 前端网站可以访问' -ForegroundColor Green; Write-Host '      状态码:' $response.StatusCode -ForegroundColor Gray; if ($response.Content -match 'Cloudflare Access' -or $response.Content -match 'Enter code') { Write-Host '[警告] 检测到 Cloudflare Access 验证页面' -ForegroundColor Red; Write-Host '      需要禁用 Access 策略' -ForegroundColor Yellow } } catch { Write-Host '[错误] 前端网站无法访问' -ForegroundColor Red; Write-Host '      错误:' $_.Exception.Message -ForegroundColor Gray }"

echo.
echo ========================================
echo   检查完成
echo ========================================
echo.
echo 重要提示:
echo   1. 如果看到 Cloudflare Access 验证页面
echo      需要访问: https://one.dash.cloudflare.com
echo      进入: Access -^> Applications
echo      删除/禁用相关应用的 Access 策略
echo.
echo   2. 清除浏览器缓存
echo      - 按 Ctrl+Shift+Delete 清除缓存
echo      - 或使用无痕模式 (Ctrl+Shift+N)
echo.
echo   3. 清除 Cloudflare 缓存
echo      - 访问: https://dash.cloudflare.com
echo      - 选择域名 -^> Caching -^> Purge Everything
echo.
echo   4. 测试访问
echo      - 使用无痕模式访问: https://www.a292428dsj.dpdns.org
echo      - 应该直接显示首页，不需要验证
echo.
echo 服务地址:
echo   - 前端网站: https://www.a292428dsj.dpdns.org
echo   - 后端 API: https://api.a292428dsj.dpdns.org
echo   - 本地后端: http://127.0.0.1:5000
echo.
pause

