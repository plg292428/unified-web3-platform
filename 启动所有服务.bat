@echo off
chcp 65001 >nul
echo ========================================
echo   启动所有服务
echo ========================================
echo.

echo [步骤 1] 启动后端服务...
start "后端服务" cmd /k "cd /d %~dp0 && run_backend.bat"
timeout /t 3 /nobreak >nul
echo   [OK] 后端服务启动命令已执行
echo   等待服务启动（约 15 秒）...
timeout /t 15 /nobreak >nul

echo.
echo [步骤 2] 测试后端 API...
powershell -Command "try { $response = Invoke-RestMethod -Uri 'http://localhost:5000/api/store/categories' -Method Get -TimeoutSec 5 -ErrorAction Stop; if ($response.succeed) { Write-Host '  [OK] 后端 API 正常响应' -ForegroundColor Green } else { Write-Host '  [WARN] API 返回失败' -ForegroundColor Yellow } } catch { Write-Host '  [WARN] 无法连接到后端服务，可能还在启动中' -ForegroundColor Yellow }"

echo.
echo [步骤 3] 启动前端服务...
start "前端服务" cmd /k "cd /d %~dp0 && run_frontend.bat"
timeout /t 2 /nobreak >nul
echo   [OK] 前端服务启动命令已执行

echo.
echo ========================================
echo   启动完成
echo ========================================
echo.
echo 服务地址:
echo   前端页面: http://localhost:5173
echo   后端 API: http://localhost:5000
echo   Swagger:  http://localhost:5000/swagger
echo.
echo 下一步操作:
echo   1. 在浏览器打开: http://localhost:5173
echo   2. 测试商品浏览、搜索、筛选功能
echo   3. 测试钱包连接（Bitget Wallet / MetaMask）
echo   4. 测试购物流程（添加购物车、创建订单、支付）
echo.
echo 重要提示:
echo   如果数据库表未创建，请先执行 SQL 脚本:
echo   创建新表脚本_简化版.sql
echo.
pause

