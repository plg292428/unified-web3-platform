@echo off
chcp 65001 >nul
echo ========================================
echo   功能测试脚本
echo ========================================
echo.

echo [测试 1] 检查后端服务...
powershell -Command "try { $response = Invoke-RestMethod -Uri 'http://localhost:5000/api/store/categories' -Method Get -TimeoutSec 5 -ErrorAction Stop; if ($response.succeed) { Write-Host '  [OK] 后端服务正常运行' -ForegroundColor Green } else { Write-Host '  [FAIL] API 返回失败' -ForegroundColor Red } } catch { Write-Host '  [FAIL] 后端服务未响应' -ForegroundColor Red }"

echo.
echo [测试 2] 测试商品分类 API...
powershell -Command "try { $response = Invoke-RestMethod -Uri 'http://localhost:5000/api/store/categories' -Method Get -TimeoutSec 5 -ErrorAction Stop; if ($response.succeed) { $count = $response.data.Count; Write-Host \"  [OK] 获取到 $count 个商品分类\" -ForegroundColor Green } else { Write-Host '  [FAIL] API 返回失败' -ForegroundColor Red } } catch { Write-Host '  [SKIP] 无法测试' -ForegroundColor Yellow }"

echo.
echo [测试 3] 测试商品列表 API...
powershell -Command "try { $response = Invoke-RestMethod -Uri 'http://localhost:5000/api/store/products?page=1&pageSize=5' -Method Get -TimeoutSec 5 -ErrorAction Stop; if ($response.succeed) { $count = $response.data.items.Count; $total = $response.data.totalCount; Write-Host \"  [OK] 获取到 $count 个商品（共 $total 个）\" -ForegroundColor Green } else { Write-Host '  [FAIL] API 返回失败' -ForegroundColor Red } } catch { Write-Host '  [SKIP] 无法测试' -ForegroundColor Yellow }"

echo.
echo ========================================
echo   测试完成
echo ========================================
echo.
echo 下一步操作:
echo   1. 在浏览器打开: http://localhost:5173
echo   2. 进行前端功能测试
echo   3. 查看 Swagger: http://localhost:5000/swagger
echo.
pause

