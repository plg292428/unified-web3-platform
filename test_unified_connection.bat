@echo off
chcp 65001 >nul
echo ========================================
echo   测试 UnifiedWeb3Platform 数据库连接
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [读取连接字符串]...
findstr /C:"DefaultConnection" appsettings.json
echo.

echo [测试数据库连接]...
powershell -Command "$conn = 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UnifiedWeb3Platform;Integrated Security=True;TrustServerCertificate=True;'; try { $connection = New-Object System.Data.SqlClient.SqlConnection($conn); $connection.Open(); Write-Host '[OK] 数据库连接成功!' -ForegroundColor Green; $connection.Close() } catch { Write-Host '[ERROR] 连接失败:' $_.Exception.Message -ForegroundColor Red }"

echo.
pause


