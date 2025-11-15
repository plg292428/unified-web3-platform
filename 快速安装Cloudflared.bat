@echo off
chcp 65001 >nul
echo ========================================
echo   快速安装 Cloudflared
echo ========================================
echo.

REM 检查项目目录中是否已有 cloudflared.exe
if exist "cloudflared.exe" (
    echo [OK] 在项目目录中找到 cloudflared.exe
    echo.
    echo 可以直接运行: 配置CloudflareTunnel.bat
    pause
    exit /b 0
)

echo [!] Cloudflared 未安装
echo.
echo 请手动下载并安装:
echo.
echo 方法 1: 使用 winget (Windows 10/11)
echo   1. 打开 PowerShell
echo   2. 执行: winget install --id Cloudflare.cloudflared
echo.
echo 方法 2: 手动下载 (推荐)
echo   1. 访问: https://github.com/cloudflare/cloudflared/releases/latest
echo   2. 下载: cloudflared-windows-amd64.exe
echo   3. 重命名为: cloudflared.exe
echo   4. 放到项目目录: %CD%
echo.
echo 方法 3: 使用 Chocolatey (需要管理员权限)
echo   1. 右键以管理员身份运行 PowerShell
echo   2. 执行: choco install cloudflared -y
echo.

set /p choice="是否现在打开下载页面? (Y/N): "
if /i "%choice%"=="Y" (
    start https://github.com/cloudflare/cloudflared/releases/latest
    echo.
    echo 下载完成后，请将 cloudflared.exe 放到项目目录: %CD%
    echo.
    pause
)


