@echo off
chcp 65001 >nul
echo ========================================
echo   一键下载 Cloudflared (Windows)
echo ========================================
echo.

REM 检查是否已存在
if exist "cloudflared.exe" (
    echo [OK] cloudflared.exe 已存在于项目目录
    echo.
    echo 可以直接运行: 配置CloudflareTunnel.bat
    pause
    exit /b 0
)

echo [信息] 正在打开下载页面...
echo.
echo 请在浏览器中:
echo   1. 找到 "Assets" 部分
echo   2. 下载: cloudflared-windows-amd64.exe
echo   3. 重命名为: cloudflared.exe
echo   4. 放到项目目录: %CD%
echo.

REM 打开下载页面
start https://github.com/cloudflare/cloudflared/releases/latest

echo.
echo 下载页面已打开
echo.
echo 重要提示:
echo   - 选择 cloudflared-windows-amd64.exe (Windows 64位)
echo   - 不要选择 .pkg 文件 (那是 macOS 版本)
echo   - 不要选择 .deb 或 .rpm 文件 (那是 Linux 版本)
echo.

pause

echo.
echo 下载完成后，请:
echo   1. 将文件重命名为: cloudflared.exe
echo   2. 复制到项目目录: %CD%
echo   3. 然后运行: 配置CloudflareTunnel.bat
echo.
pause


