@echo off
chcp 65001 >nul
echo ========================================
echo   Install Frontend Dependencies
echo ========================================
echo.

cd /d "%~dp0src\Frontend\web-app"

if exist "package.json" (
    echo [Step] Installing npm dependencies...
    echo.
    npm install
    echo.
    echo ========================================
    echo   Frontend Dependencies Installed!
    echo ========================================
) else (
    echo [ERROR] package.json not found!
)

echo.
pause

