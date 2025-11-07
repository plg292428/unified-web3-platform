# 检查 UnifiedWeb3Platform 数据库是否存在

Write-Host "========================================" -ForegroundColor Green
Write-Host "  检查数据库创建状态" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$connectionString = "Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True;"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "[检查 LocalDB 连接]..." -ForegroundColor Cyan
    Write-Host "  [OK] LocalDB 连接成功" -ForegroundColor Green
    Write-Host ""
    
    # 检查数据库是否存在
    Write-Host "[检查 UnifiedWeb3Platform 数据库]..." -ForegroundColor Cyan
    $checkQuery = @"
SELECT 
    name AS DatabaseName,
    database_id AS DatabaseID,
    create_date AS CreateDate,
    state_desc AS State
FROM sys.databases
WHERE name = 'UnifiedWeb3Platform'
"@
    
    $command = New-Object System.Data.SqlClient.SqlCommand($checkQuery, $connection)
    $reader = $command.ExecuteReader()
    
    if ($reader.HasRows) {
        $reader.Read()
        $dbName = $reader["DatabaseName"]
        $createDate = $reader["CreateDate"]
        $state = $reader["State"]
        
        Write-Host "  [OK] 数据库已存在!" -ForegroundColor Green
        Write-Host "  数据库名称: $dbName" -ForegroundColor Gray
        Write-Host "  创建日期: $createDate" -ForegroundColor Gray
        Write-Host "  状态: $state" -ForegroundColor Gray
        Write-Host ""
        
        $reader.Close()
        
        # 检查数据库中的表
        Write-Host "[检查数据库表]..." -ForegroundColor Cyan
        $tableQuery = @"
USE UnifiedWeb3Platform;
SELECT 
    TABLE_SCHEMA AS SchemaName,
    TABLE_NAME AS TableName
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
"@
        
        $command = New-Object System.Data.SqlClient.SqlCommand($tableQuery, $connection)
        $tableReader = $command.ExecuteReader()
        
        $tableCount = 0
        while ($tableReader.Read()) {
            if ($tableCount -eq 0) {
                Write-Host "  表列表:" -ForegroundColor Gray
            }
            $schemaName = $tableReader["SchemaName"]
            $tableName = $tableReader["TableName"]
            Write-Host "    - [$schemaName].[$tableName]" -ForegroundColor Gray
            $tableCount++
        }
        
        if ($tableCount -eq 0) {
            Write-Host "  [INFO] 数据库中没有表" -ForegroundColor Yellow
        } else {
            Write-Host "  表总数: $tableCount" -ForegroundColor Gray
        }
        
        $tableReader.Close()
        
    } else {
        Write-Host "  [X] 数据库不存在" -ForegroundColor Red
        Write-Host ""
        Write-Host "需要创建数据库吗？" -ForegroundColor Yellow
        Write-Host "  运行: .\create_database.ps1" -ForegroundColor Cyan
        Write-Host "  或使用 SQL 脚本: .\create_database.sql" -ForegroundColor Cyan
    }
    
    # 列出所有数据库
    Write-Host ""
    Write-Host "[LocalDB 中的所有数据库]..." -ForegroundColor Cyan
    $allDatabasesQuery = "SELECT name FROM sys.databases ORDER BY name"
    $command = New-Object System.Data.SqlClient.SqlCommand($allDatabasesQuery, $connection)
    $allReader = $command.ExecuteReader()
    
    $dbCount = 0
    while ($allReader.Read()) {
        if ($dbCount -eq 0) {
            Write-Host "  数据库列表:" -ForegroundColor Gray
        }
        $dbName = $allReader["name"]
        if ($dbName -eq "UnifiedWeb3Platform") {
            Write-Host "    - $dbName [目标数据库]" -ForegroundColor Green
        } elseif ($dbName -eq "SmallTarget") {
            Write-Host "    - $dbName [当前使用中]" -ForegroundColor Yellow
        } else {
            Write-Host "    - $dbName" -ForegroundColor Gray
        }
        $dbCount++
    }
    Write-Host "  数据库总数: $dbCount" -ForegroundColor Gray
    $allReader.Close()
    
    $connection.Close()
    
} catch {
    Write-Host "  [ERROR] 连接失败: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "请检查:" -ForegroundColor Yellow
    Write-Host "  1. LocalDB 是否已安装并运行" -ForegroundColor Gray
    Write-Host "  2. 运行: .\check_localdb.ps1" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green


