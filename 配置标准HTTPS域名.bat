@echo off
chcp 65001 >nul
echo ========================================
echo   前端标准 HTTPS 域名配置
echo ========================================
echo.

echo 请选择配置方案:
echo.
echo [1] Cloudflare Tunnel (推荐 - 最简单)
echo [2] Nginx 反向代理
echo [3] IIS 反向代理
echo [4] 查看配置说明
echo.
set /p choice="请输入选项 (1-4): "

if "%choice%"=="1" (
    echo.
    echo ========================================
    echo   使用 Cloudflare Tunnel
    echo ========================================
    echo.
    echo 步骤:
    echo 1. 安装 Cloudflared: choco install cloudflared
    echo 2. 登录: cloudflared tunnel login
    echo 3. 创建 Tunnel: cloudflared tunnel create a292428dsj-frontend
    echo 4. 在 Cloudflare Dashboard 配置路由
    echo 5. 运行: cloudflared tunnel run a292428dsj-frontend
    echo.
    echo 详细说明: 前端标准HTTPS域名配置方案.md
    echo.
    pause
) else if "%choice%"=="2" (
    echo.
    echo ========================================
    echo   使用 Nginx 反向代理
    echo ========================================
    echo.
    echo 步骤:
    echo 1. 安装 Nginx: choco install nginx
    echo 2. 配置 nginx.conf (已创建)
    echo 3. 配置 SSL 证书
    echo 4. 启动 Nginx: nginx
    echo.
    echo 详细说明: 前端标准HTTPS域名配置方案.md
    echo.
    pause
) else if "%choice%"=="3" (
    echo.
    echo ========================================
    echo   使用 IIS 反向代理
    echo ========================================
    echo.
    echo 正在运行 IIS 配置脚本...
    echo.
    powershell -ExecutionPolicy Bypass -File "IIS反向代理配置.ps1"
    pause
) else if "%choice%"=="4" (
    echo.
    echo 正在打开配置说明文档...
    start "" "前端标准HTTPS域名配置方案.md"
    pause
) else (
    echo.
    echo 无效选项，请重新运行脚本
    pause
    exit /b 1
)

