@echo off
chcp 65001 >nul
title 验证 Tunnel 配置
echo ========================================
echo   验证 Tunnel 配置
echo ========================================
echo.

cd /d "%~dp0"

echo [步骤 1] 检查后端服务...
echo.
netstat -ano | findstr ":5000" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (
    echo [✓] 后端服务: 运行中 (http://localhost:5000)
    echo.
    echo     测试本地 API...
    curl -s http://localhost:5000/health >nul 2>&1
    if %errorlevel% equ 0 (
        echo [✓] 本地 API: 可访问
    ) else (
        echo [⚠] 本地 API: 无法访问
    )
) else (
    echo [✗] 后端服务: 未运行
    echo.
    echo     请先启动后端服务
)
echo.

echo [步骤 2] 检查 Tunnel 进程...
echo.
tasklist | findstr "cloudflared.exe" >nul
if %errorlevel% equ 0 (
    echo [✓] Cloudflare Tunnel: 进程运行中
) else (
    echo [✗] Cloudflare Tunnel: 进程未运行
    echo.
    echo     请运行: 启动后端API_Tunnel.bat
)
echo.

echo [步骤 3] 检查 Tunnel 连接状态...
echo.
if exist "cloudflared.exe" (
    set CLOUDFLARED_CMD=cloudflared.exe
) else (
    where cloudflared >nul 2>&1
    if %errorlevel% equ 0 (
        set CLOUDFLARED_CMD=cloudflared
    ) else (
        echo [✗] cloudflared 未找到
        goto :end
    )
)

%CLOUDFLARED_CMD% tunnel info api 2>&1 | findstr "active connection" >nul
if %errorlevel% equ 0 (
    echo [✓] Tunnel: 有活动连接
) else (
    echo [⚠] Tunnel: 无活动连接
    echo.
    echo     请检查:
    echo     1. Tunnel 窗口是否显示 'Connection established'
    echo     2. Dashboard 中 Tunnel 是否已启用
)
echo.

echo [步骤 4] 测试公网 API...
echo.
echo     等待 3 秒...
timeout /t 3 /nobreak >nul
echo.
curl -s -o nul -w "状态码: %%{http_code}\n" https://api.a292428dsj.dpdns.org/health 2>nul
if %errorlevel% equ 0 (
    echo [✓] 公网 API: 可访问
    echo.
    echo     完整响应:
    curl -s https://api.a292428dsj.dpdns.org/health
    echo.
) else (
    echo [⚠] 公网 API: 无法访问或返回错误
    echo.
    echo     可能原因:
    echo     1. Dashboard 中 Tunnel 未启用
    echo     2. Public Hostname 未配置
    echo     3. Tunnel 未建立连接
    echo     4. 配置需要更多时间生效（等待 2-5 分钟）
)
echo.

:end
echo ========================================
echo   验证完成
echo ========================================
echo.
echo 请检查上述结果，如有问题请参考配置方案
echo.
pause

chcp 65001 >nul
title 验证 Tunnel 配置
echo ========================================
echo   验证 Tunnel 配置
echo ========================================
echo.

cd /d "%~dp0"

echo [步骤 1] 检查后端服务...
echo.
netstat -ano | findstr ":5000" | findstr "LISTENING" >nul
if %errorlevel% equ 0 (
    echo [✓] 后端服务: 运行中 (http://localhost:5000)
    echo.
    echo     测试本地 API...
    curl -s http://localhost:5000/health >nul 2>&1
    if %errorlevel% equ 0 (
        echo [✓] 本地 API: 可访问
    ) else (
        echo [⚠] 本地 API: 无法访问
    )
) else (
    echo [✗] 后端服务: 未运行
    echo.
    echo     请先启动后端服务
)
echo.

echo [步骤 2] 检查 Tunnel 进程...
echo.
tasklist | findstr "cloudflared.exe" >nul
if %errorlevel% equ 0 (
    echo [✓] Cloudflare Tunnel: 进程运行中
) else (
    echo [✗] Cloudflare Tunnel: 进程未运行
    echo.
    echo     请运行: 启动后端API_Tunnel.bat
)
echo.

echo [步骤 3] 检查 Tunnel 连接状态...
echo.
if exist "cloudflared.exe" (
    set CLOUDFLARED_CMD=cloudflared.exe
) else (
    where cloudflared >nul 2>&1
    if %errorlevel% equ 0 (
        set CLOUDFLARED_CMD=cloudflared
    ) else (
        echo [✗] cloudflared 未找到
        goto :end
    )
)

%CLOUDFLARED_CMD% tunnel info api 2>&1 | findstr "active connection" >nul
if %errorlevel% equ 0 (
    echo [✓] Tunnel: 有活动连接
) else (
    echo [⚠] Tunnel: 无活动连接
    echo.
    echo     请检查:
    echo     1. Tunnel 窗口是否显示 'Connection established'
    echo     2. Dashboard 中 Tunnel 是否已启用
)
echo.

echo [步骤 4] 测试公网 API...
echo.
echo     等待 3 秒...
timeout /t 3 /nobreak >nul
echo.
curl -s -o nul -w "状态码: %%{http_code}\n" https://api.a292428dsj.dpdns.org/health 2>nul
if %errorlevel% equ 0 (
    echo [✓] 公网 API: 可访问
    echo.
    echo     完整响应:
    curl -s https://api.a292428dsj.dpdns.org/health
    echo.
) else (
    echo [⚠] 公网 API: 无法访问或返回错误
    echo.
    echo     可能原因:
    echo     1. Dashboard 中 Tunnel 未启用
    echo     2. Public Hostname 未配置
    echo     3. Tunnel 未建立连接
    echo     4. 配置需要更多时间生效（等待 2-5 分钟）
)
echo.

:end
echo ========================================
echo   验证完成
echo ========================================
echo.
echo 请检查上述结果，如有问题请参考配置方案
echo.
pause

