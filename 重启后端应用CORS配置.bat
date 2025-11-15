@echo off
chcp 65001 >nul
echo.
echo ========================================
echo   重启后端服务以应用 CORS 配置
echo ========================================
echo.

echo [1/3] 停止当前后端服务...
taskkill /F /IM dotnet.exe >nul 2>&1
timeout /t 2 /nobreak >nul
echo [OK] 后端服务已停止
echo.

echo [2/3] 等待服务完全停止...
timeout /t 2 /nobreak >nul
echo.

echo [3/3] 重新启动后端服务...
cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"
start "后端服务" cmd /k "dotnet run"
echo [OK] 后端服务正在启动...
echo.

echo ========================================
echo   重启完成
echo ========================================
echo.
echo 服务地址:
echo   后端 API:  http://localhost:5000
echo   Swagger:   http://localhost:5000/swagger
echo   前端应用:  http://localhost:5173
echo.
echo 提示:
echo   1. 等待约 5-10 秒让后端服务完全启动
echo   2. 然后刷新前端页面 (F5)
echo   3. CORS 错误应该已经解决
echo.
pause

