@echo off
chcp 65001 >nul
echo ========================================
echo   修复Kestrel绑定问题
echo ========================================
echo.

echo [Step 1] 检查并信任HTTPS开发证书...
dotnet dev-certs https --trust
if %ERRORLEVEL% NEQ 0 (
    echo   [警告] 证书信任失败，将使用HTTP模式
)

echo.
echo [Step 2] 检查端口占用...
netstat -ano | findstr ":5195 :7266 :5000 :5001"
echo.

echo [Step 3] 清理并重新编译项目...
cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"
dotnet clean
dotnet build

echo.
echo [Step 4] 启动服务（使用HTTP模式，端口5195）...
echo   [提示] 如果仍有问题，请查看错误信息
echo   [提示] 按 Ctrl+C 停止服务
echo.

dotnet run --launch-profile http

pause

