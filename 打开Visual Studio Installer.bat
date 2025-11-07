@echo off
chcp 65001 >nul
echo ========================================
echo   Open Visual Studio Installer
echo ========================================
echo.

echo [提示] 正在尝试打开Visual Studio Installer...
echo.

REM 方法1: 尝试通过开始菜单打开
start "" "shell:AppsFolder\Microsoft.VisualStudio.Installer_8wekyb3d8bbwe!VisualStudioInstaller" 2>nul

REM 等待一下
timeout /t 2 >nul

REM 方法2: 尝试通过程序路径打开
set "VS_INSTALLER_PATH=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vs_installer.exe"
if exist "%VS_INSTALLER_PATH%" (
    start "" "%VS_INSTALLER_PATH%"
    echo [OK] 已通过程序路径打开Visual Studio Installer
    goto :done
)

REM 方法3: 尝试通过Program Files路径打开
set "VS_INSTALLER_PATH=%ProgramFiles%\Microsoft Visual Studio\Installer\vs_installer.exe"
if exist "%VS_INSTALLER_PATH%" (
    start "" "%VS_INSTALLER_PATH%"
    echo [OK] 已通过程序路径打开Visual Studio Installer
    goto :done
)

REM 方法4: 尝试通过AppData路径打开
set "VS_INSTALLER_PATH=%LOCALAPPDATA%\Microsoft\VisualStudio\Installer\vs_installer.exe"
if exist "%VS_INSTALLER_PATH%" (
    start "" "%VS_INSTALLER_PATH%"
    echo [OK] 已通过AppData路径打开Visual Studio Installer
    goto :done
)

echo [WARN] 无法自动打开Visual Studio Installer
echo [提示] 请手动打开：
echo   1. 按 Win 键，搜索 "Visual Studio Installer"
echo   2. 或在开始菜单中找到并打开
echo.
echo [安装步骤]
echo   1. 在Visual Studio Installer中，找到您的Visual Studio版本
echo   2. 点击"修改"按钮
echo   3. 切换到"单个组件"标签
echo   4. 搜索"LocalDB"
echo   5. 勾选"SQL Server Express LocalDB"
echo   6. 点击右下角的"修改"完成安装
echo.

:done
echo.
echo [提示] 安装完成后，请运行:
echo   .\检查LocalDB安装.bat
echo   验证LocalDB是否安装成功
echo.
pause

