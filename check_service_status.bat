@echo off
chcp 65001 >nul
echo ========================================
echo   服务启动状态检查
echo ========================================
echo.

echo [检查端口占用]:
netstat -ano | findstr :5000 >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 端口 5000 : 已被占用 (服务运行中)
) else (
    echo   [INFO] 端口 5000 : 空闲
)

netstat -ano | findstr :5001 >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 端口 5001 : 已被占用 (服务运行中)
) else (
    echo   [INFO] 端口 5001 : 空闲
)

netstat -ano | findstr :5195 >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 端口 5195 : 已被占用 (服务运行中)
) else (
    echo   [INFO] 端口 5195 : 空闲
)
echo.

echo [测试API连接]:
curl -s -o nul -w "状态码: %%{http_code}\n" http://localhost:5000/health 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 服务响应正常
) else (
    echo   [INFO] 服务可能还在启动中或使用了其他端口...
)
echo.

echo [服务地址]:
echo   HTTP:  http://localhost:5000
echo   HTTPS: https://localhost:5001
echo   Swagger: http://localhost:5000/swagger
echo.
pause

