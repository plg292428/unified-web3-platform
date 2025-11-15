@echo off
chcp 65001 >nul
title 部署前端到 Cloudflare Pages
echo ========================================
echo   部署前端到 Cloudflare Pages
echo ========================================
echo.

cd /d "%~dp0"

REM 检查 dist 目录
if not exist "src\Frontend\web-app\dist" (
    echo [错误] dist 目录不存在
    echo 请先运行: npm run build
    echo.
    pause
    exit /b 1
)

echo [✓] dist 目录存在
echo.

REM 检查 wrangler
where wrangler >nul 2>&1
if %errorlevel% neq 0 (
    echo [提示] Wrangler 未安装
    echo.
    echo 安装 Wrangler:
    echo   npm install -g wrangler
    echo.
    echo 或使用 Dashboard 方式部署（见下方说明）
    echo.
    pause
    exit /b 0
)

echo [✓] Wrangler 已安装
echo.

echo ========================================
echo   部署步骤
echo ========================================
echo.
echo [1] 登录 Cloudflare
echo   执行: wrangler login
echo.
echo [2] 部署到 Pages
echo   执行: wrangler pages deploy src\Frontend\web-app\dist --project-name=unified-web3-platform
echo.
echo [3] 配置自定义域名（在 Dashboard 中）
echo   1. 进入 Pages 项目: unified-web3-platform
echo   2. Custom domains → Add a domain
echo   3. 添加: www.a292428dsj.dpdns.org
echo.
echo ========================================
echo.

set /p choice="是否现在开始部署? (Y/N): "

if /i "%choice%"=="Y" (
    echo.
    echo [步骤 1] 登录 Cloudflare...
    wrangler login
    echo.
    
    set /p continue="登录完成? (Y/N): "
    if /i not "%continue%"=="Y" (
        echo 部署已取消
        pause
        exit /b 0
    )
    
    echo.
    echo [步骤 2] 部署到 Pages...
    wrangler pages deploy src\Frontend\web-app\dist --project-name=unified-web3-platform
    echo.
    
    echo [✓] 部署完成
    echo.
    echo [下一步] 在 Dashboard 中配置自定义域名:
    echo   1. 访问: https://dash.cloudflare.com/
    echo   2. Workers ^& Pages → Pages → unified-web3-platform
    echo   3. Custom domains → Add a domain
    echo   4. 添加: www.a292428dsj.dpdns.org
    echo.
) else (
    echo.
    echo 部署已取消
    echo.
    echo 手动部署步骤:
    echo   1. 访问: https://dash.cloudflare.com/
    echo   2. Workers ^& Pages → Create application → Pages
    echo   3. Upload assets → 选择 dist 文件夹
    echo   4. 项目名称: unified-web3-platform
    echo   5. Deploy site
    echo   6. Custom domains → 添加 www.a292428dsj.dpdns.org
    echo.
)

pause


    echo.
)

pause

