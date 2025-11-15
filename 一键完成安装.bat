@echo off
chcp 65001 >nul
echo ========================================
echo   Cloudflared 一键安装助手
echo ========================================
echo.

REM 检查是否已安装
if exist "cloudflared.exe" (
    echo [OK] cloudflared.exe 已存在于项目目录
    echo.
    echo 正在验证安装...
    cloudflared.exe --version
    echo.
    echo 安装已完成！可以直接运行: 配置CloudflareTunnel.bat
    pause
    exit /b 0
)

echo [信息] 未找到 cloudflared.exe
echo.
echo ========================================
echo   安装步骤指导
echo ========================================
echo.

echo 步骤 1: 找到下载的文件
echo   位置: 下载文件夹 (通常是 C:\Users\%USERNAME%\Downloads\)
echo   文件名: cloudflared-windows-amd64 或 cloudflared-windows-amd64.exe
echo.

echo 步骤 2: 重命名文件
echo   1. 右键点击文件
echo   2. 选择"重命名"
echo   3. 改为: cloudflared.exe
echo   4. 按 Enter 确认
echo.

echo 步骤 3: 复制到项目目录
echo   1. 复制 cloudflared.exe 文件
echo   2. 粘贴到: %CD%
echo.

echo 步骤 4: 验证安装
echo   运行: cloudflared.exe --version
echo.

echo ========================================
echo   快速操作
echo ========================================
echo.

set /p choice="是否现在打开下载文件夹? (Y/N): "
if /i "%choice%"=="Y" (
    start "" "%USERPROFILE%\Downloads"
    echo.
    echo 下载文件夹已打开
    echo 请找到 cloudflared-windows-amd64 文件
    echo.
)

echo.
echo 完成以下操作后，按任意键继续验证...
pause

echo.
echo 正在检查 cloudflared.exe...
if exist "cloudflared.exe" (
    echo [OK] 找到 cloudflared.exe
    echo.
    echo 正在验证...
    cloudflared.exe --version
    echo.
    echo ========================================
    echo   [成功] 安装完成！
    echo ========================================
    echo.
    echo 下一步: 运行 配置CloudflareTunnel.bat
    echo.
) else (
    echo [错误] 未找到 cloudflared.exe
    echo.
    echo 请确认:
    echo   1. 文件已重命名为 cloudflared.exe
    echo   2. 文件已复制到项目目录: %CD%
    echo.
    echo 详细步骤请查看: Cloudflared详细安装步骤.md
    echo.
)

pause


