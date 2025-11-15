@echo off
chcp 65001 >nul
echo ========================================
echo   手动安装 Cloudflared
echo ========================================
echo.

echo 方法 1: 使用 Chocolatey (需要管理员权限)
echo   1. 右键以管理员身份运行 PowerShell
echo   2. 执行: choco install cloudflared -y
echo.
echo 方法 2: 手动下载安装 (推荐)
echo   1. 访问: https://github.com/cloudflare/cloudflared/releases/latest
echo   2. 下载: cloudflared-windows-amd64.exe
echo   3. 重命名为: cloudflared.exe
echo   4. 放到系统 PATH 目录或项目目录
echo.
echo 方法 3: 使用 winget (Windows 10/11)
echo   1. 打开 PowerShell
echo   2. 执行: winget install --id Cloudflare.cloudflared
echo.

set /p choice="是否现在打开下载页面? (Y/N): "
if /i "%choice%"=="Y" (
    start https://github.com/cloudflare/cloudflared/releases/latest
    echo.
    echo 下载完成后，请将 cloudflared.exe 放到以下任一位置:
    echo   - 系统 PATH 目录 (如 C:\Windows\System32)
    echo   - 项目目录 (D:\claude code\plg\UnifiedWeb3Platform)
    echo.
    echo 或者直接放到项目目录，然后运行配置脚本
    pause
)


