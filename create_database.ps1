# 创建 UnifiedWeb3Platform 数据库

Write-Host "========================================" -ForegroundColor Green
Write-Host "  创建 UnifiedWeb3Platform 数据库" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$connectionString = "Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;"

try {
    Write-Host "[步骤 1] 连接 LocalDB..." -ForegroundColor Cyan
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "  [OK] LocalDB 连接成功" -ForegroundColor Green
    Write-Host ""
    
    # 检查数据库是否已存在
    Write-Host "[步骤 2] 检查数据库是否已存在..." -ForegroundColor Cyan
    $checkQuery = "SELECT name FROM sys.databases WHERE name = 'UnifiedWeb3Platform'"
    $checkCommand = New-Object System.Data.SqlClient.SqlCommand($checkQuery, $connection)
    $result = $checkCommand.ExecuteScalar()
    
    if ($result) {
        Write-Host "  [INFO] 数据库 UnifiedWeb3Platform 已存在" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "是否要删除并重新创建？(Y/N)" -ForegroundColor Yellow
        $response = Read-Host
        if ($response -eq "Y" -or $response -eq "y") {
            Write-Host ""
            Write-Host "[步骤 3] 删除现有数据库..." -ForegroundColor Cyan
            # 设置数据库为单用户模式并删除
            $dropQuery = @"
USE master;
ALTER DATABASE UnifiedWeb3Platform SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE UnifiedWeb3Platform;
"@
            $dropCommand = New-Object System.Data.SqlClient.SqlCommand($dropQuery, $connection)
            $dropCommand.ExecuteNonQuery()
            Write-Host "  [OK] 数据库已删除" -ForegroundColor Green
            Write-Host ""
        } else {
            Write-Host "  取消操作" -ForegroundColor Gray
            $connection.Close()
            exit 0
        }
    }
    
    # 创建数据库
    if (-not $result -or $response -eq "Y" -or $response -eq "y") {
        Write-Host "[步骤 3] 创建数据库..." -ForegroundColor Cyan
        $createQuery = "CREATE DATABASE UnifiedWeb3Platform"
        $createCommand = New-Object System.Data.SqlClient.SqlCommand($createQuery, $connection)
        $createCommand.ExecuteNonQuery()
        Write-Host "  [OK] 数据库创建成功!" -ForegroundColor Green
        Write-Host ""
    }
    
    # 验证数据库
    Write-Host "[步骤 4] 验证数据库..." -ForegroundColor Cyan
    $verifyQuery = @"
SELECT 
    name AS DatabaseName,
    database_id AS DatabaseID,
    create_date AS CreateDate,
    state_desc AS State
FROM sys.databases
WHERE name = 'UnifiedWeb3Platform'
"@
    $verifyCommand = New-Object System.Data.SqlClient.SqlCommand($verifyQuery, $connection)
    $verifyReader = $verifyCommand.ExecuteReader()
    
    if ($verifyReader.Read()) {
        $dbName = $verifyReader["DatabaseName"]
        $createDate = $verifyReader["CreateDate"]
        $state = $verifyReader["State"]
        
        Write-Host "  [OK] 数据库验证成功!" -ForegroundColor Green
        Write-Host "  数据库名称: $dbName" -ForegroundColor Gray
        Write-Host "  创建日期: $createDate" -ForegroundColor Gray
        Write-Host "  状态: $state" -ForegroundColor Gray
    }
    $verifyReader.Close()
    
    $connection.Close()
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  数据库创建完成！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "下一步操作:" -ForegroundColor Cyan
    Write-Host "  1. 更新 appsettings.json 中的连接字符串" -ForegroundColor Gray
    Write-Host "  2. 运行数据库迁移: dotnet ef database update" -ForegroundColor Gray
    
} catch {
    Write-Host "  [ERROR] 操作失败: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "请检查:" -ForegroundColor Yellow
    Write-Host "  1. LocalDB 是否已安装并运行" -ForegroundColor Gray
    Write-Host "  2. 运行: .\check_localdb.ps1" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}


