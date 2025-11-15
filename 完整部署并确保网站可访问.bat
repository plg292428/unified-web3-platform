@echo off
chcp 65001 >nul
title 完整部署并确保网站可访问

echo.
echo ========================================
echo   完整部署并确保网站可访问
echo ========================================
echo.

cd /d "%~dp0"

echo [步骤 1/6] 检查后端服务状态...
netstat -ano | findstr ":5000" | findstr "LISTENING" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] 后端服务正在运行
) else (
    echo [启动] 正在启动后端服务...
    start "后端服务" /min powershell -NoExit -Command "cd '%~dp0src\Backend\UnifiedPlatform.WebApi'; Write-Host '后端服务运行中...' -ForegroundColor Green; Write-Host '请保持此窗口打开' -ForegroundColor Yellow; Write-Host ''; dotnet run"
    timeout /t 10 /nobreak >nul
    echo [OK] 后端服务已启动，等待初始化...
    timeout /t 5 /nobreak >nul
)

echo.
echo [步骤 2/6] 检查 Cloudflare Tunnel 状态...
tasklist | findstr "cloudflared.exe" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Cloudflare Tunnel 正在运行
) else (
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
echo [步骤 3/6] 检查本地后端 API...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://127.0.0.1:5000/health' -Method GET -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop; Write-Host '[OK] 本地后端 API 可访问' -ForegroundColor Green } catch { Write-Host '[错误] 本地后端 API 不可访问' -ForegroundColor Red; Write-Host '      请检查后端服务是否正常启动' -ForegroundColor Yellow }"

echo.
echo [步骤 4/6] 检查公网后端 API...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'https://api.a292428dsj.dpdns.org/health' -Method GET -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop; Write-Host '[OK] 公网后端 API 可访问' -ForegroundColor Green } catch { Write-Host '[警告] 公网后端 API 不可访问' -ForegroundColor Yellow; Write-Host '      请检查 Cloudflare Tunnel 是否正常连接' -ForegroundColor Yellow }"

echo.
echo [步骤 5/6] 检查前端网站...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'https://www.a292428dsj.dpdns.org' -Method GET -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop; Write-Host '[OK] 前端网站可以访问' -ForegroundColor Green; Write-Host '      状态码:' $response.StatusCode -ForegroundColor Gray; if ($response.Content -match 'Cloudflare Access' -or $response.Content -match 'Enter code') { Write-Host '[警告] 检测到 Cloudflare Access 验证页面' -ForegroundColor Red; Write-Host '      需要禁用 Access 策略' -ForegroundColor Yellow } } catch { Write-Host '[错误] 前端网站无法访问' -ForegroundColor Red; Write-Host '      错误:' $_.Exception.Message -ForegroundColor Gray }"

echo.
echo [步骤 6/6] 检查前端代码版本...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'https://www.a292428dsj.dpdns.org' -Method GET -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop; if ($response.Content -match 'index-D-5dwbfa.js') { Write-Host '[警告] 检测到旧版本代码 (index-D-5dwbfa.js)' -ForegroundColor Red; Write-Host '      需要清除 Cloudflare 和浏览器缓存' -ForegroundColor Yellow } else { Write-Host '[OK] 代码版本正常' -ForegroundColor Green } } catch { Write-Host '[跳过] 无法检查代码版本' -ForegroundColor Gray }"

echo.
echo ========================================
echo   检查完成
echo ========================================
echo.
echo 重要提示:
echo.
echo [必须完成] 禁用 Cloudflare Access 策略
echo   1. 访问: https://one.dash.cloudflare.com
echo   2. 进入: Access -^> Applications
echo   3. 找到并删除/禁用包含以下关键词的应用:
echo      - unified-web3-platform
echo      - a292428dsj.dpdns.org
echo      - www.a292428dsj.dpdns.org
echo   4. 等待 1-2 分钟让更改生效
echo.
echo [重要] 清除 Cloudflare 缓存
echo   1. 访问: https://dash.cloudflare.com
echo   2. 选择域名: a292428dsj.dpdns.org
echo   3. 进入: Caching -^> Configuration
echo   4. 点击: Purge Everything
echo   5. 等待 1-2 分钟
echo.
echo [推荐] 清除浏览器缓存
echo   方法 1: 使用无痕模式
echo     按 Ctrl+Shift+N 打开无痕模式
echo   方法 2: 清除所有缓存
echo     按 Ctrl+Shift+Delete 打开清除数据对话框
echo.
echo 服务地址:
echo   - 前端网站: https://www.a292428dsj.dpdns.org
echo   - 后端 API: https://api.a292428dsj.dpdns.org
echo   - 本地后端: http://127.0.0.1:5000
echo.
echo 测试步骤:
echo   1. 使用无痕模式访问: https://www.a292428dsj.dpdns.org
echo   2. 应该直接显示首页，不需要验证
echo   3. 不应该显示错误页面
echo   4. 打开 Console (F12) 查看日志
echo   5. 应该看到 App初始化开始 和 开始进入访客模式 日志
echo.
pause

