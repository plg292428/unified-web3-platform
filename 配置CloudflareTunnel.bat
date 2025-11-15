@echo off
chcp 65001 >nul
echo ========================================
echo   Cloudflare Tunnel 配置向导
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
echo.
echo 请先安装 Cloudflared:
echo   方法 1: 运行 快速安装Cloudflared.bat
echo   方法 2: 手动下载并放到项目目录
echo   方法 3: choco install cloudflared (需要管理员权限)
echo.
pause
exit /b 1

:check_done

echo [OK] Cloudflared 已找到
%CLOUDFLARED_CMD% --version
echo.

echo ========================================
echo   配置步骤
echo ========================================
echo.
echo 步骤 1: 登录 Cloudflare
echo   命令: cloudflared tunnel login
echo   说明: 在浏览器中完成登录，授权访问您的 Cloudflare 账户
echo.
echo 步骤 2: 创建 Tunnel
echo   命令: cloudflared tunnel create a292428dsj-frontend
echo   说明: 创建一个名为 a292428dsj-frontend 的 Tunnel
echo.
echo 步骤 3: 配置路由（在 Cloudflare Dashboard）
echo   1. 访问: https://one.dash.cloudflare.com/
echo   2. 进入: Zero Trust ^> Networks ^> Tunnels
echo   3. 选择创建的 Tunnel: a292428dsj-frontend
echo   4. 点击 "Configure" 或 "Public Hostname"
echo   5. 添加 Public Hostname:
echo      - Subdomain: www
echo      - Domain: a292428dsj.dpdns.org
echo      - Service: https://localhost:8443
echo.
echo 步骤 4: 运行 Tunnel
echo   命令: cloudflared tunnel run a292428dsj-frontend
echo   说明: 启动 Tunnel，保持窗口打开
echo.
echo ========================================
echo   快速配置
echo ========================================
echo.
set /p choice="是否现在开始配置? (Y/N): "

if /i "%choice%"=="Y" (
    echo.
    echo 步骤 1: 登录 Cloudflare...
    echo 请在浏览器中完成登录
    %CLOUDFLARED_CMD% tunnel login
    echo.
    
    set /p continue="登录完成? (Y/N): "
    if /i not "%continue%"=="Y" (
        echo 配置已取消
        pause
        exit /b 0
    )
    
    echo.
    echo 步骤 2: 创建 Tunnel...
    %CLOUDFLARED_CMD% tunnel create a292428dsj-frontend
    echo.
    
    echo 步骤 3: 配置路由
    echo.
    echo 请在 Cloudflare Dashboard 中配置路由:
    echo   1. 访问: https://one.dash.cloudflare.com/
    echo   2. 进入: Zero Trust ^> Networks ^> Tunnels
    echo   3. 选择: a292428dsj-frontend
    echo   4. 添加 Public Hostname:
    echo      - Subdomain: www
    echo      - Domain: a292428dsj.dpdns.org
    echo      - Service: https://localhost:8443
    echo.
    set /p continue="路由配置完成? (Y/N): "
    if /i not "%continue%"=="Y" (
        echo 请完成路由配置后再运行 Tunnel
        pause
        exit /b 0
    )
    
    echo.
    echo 步骤 4: 启动 Tunnel...
    echo 注意: 请保持此窗口打开，Tunnel 需要持续运行
    echo.
    echo 同时确保:
    echo   - 前端服务运行在 https://localhost:8443
    echo   - 后端服务运行在 http://localhost:5000
    echo.
    pause
    
    echo.
    echo 正在启动 Tunnel...
    %CLOUDFLARED_CMD% tunnel run a292428dsj-frontend
) else (
    echo.
    echo 配置已取消
    echo.
    echo 手动配置步骤请查看: 前端标准HTTPS域名配置方案.md
    pause
)

