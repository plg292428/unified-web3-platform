@echo off
chcp 65001 >nul
echo ========================================
echo   启动 Cloudflare Tunnel
echo ========================================
echo.

REM 检查 cloudflared 是否安装
REM 首先检查项目目录
if exist "cloudflared.exe" (
    set CLOUDFLARED_CMD=cloudflared.exe
    goto :check_done
)

REM 检查系统 PATH
where cloudflared >nul 2>&1
if %errorlevel% equ 0 (
    set CLOUDFLARED_CMD=cloudflared
    goto :check_done
)

REM 未找到
echo [错误] Cloudflared 未安装
echo 请先运行: 快速安装Cloudflared.bat
pause
exit /b 1

:check_done

REM 检查前端服务是否运行
netstat -ano | findstr ":8443" | findstr "LISTENING" >nul
if %errorlevel% neq 0 (
    echo [警告] 前端服务未运行 (端口 8443)
    echo 请先启动前端服务: 启动HTTPS前端.bat
    echo.
    set /p continue="是否继续启动 Tunnel? (Y/N): "
    if /i not "%continue%"=="Y" (
        exit /b 0
    )
)

REM 检查后端服务是否运行
netstat -ano | findstr ":5000" | findstr "LISTENING" >nul
if %errorlevel% neq 0 (
    echo [警告] 后端服务未运行 (端口 5000)
    echo 请先启动后端服务: 快速启动后端.bat
    echo.
    set /p continue="是否继续启动 Tunnel? (Y/N): "
    if /i not "%continue%"=="Y" (
        exit /b 0
    )
)

echo [✓] 检查完成
echo.
echo 正在启动 Cloudflare Tunnel...
echo 注意: 请保持此窗口打开，Tunnel 需要持续运行
echo.
echo 访问地址: https://www.a292428dsj.dpdns.org/
echo.

%CLOUDFLARED_CMD% tunnel run a292428dsj-frontend

