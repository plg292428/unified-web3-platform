# Fix Database Connection Script
# 数据库修复脚本

$localdbPath = "C:\Program Files\Microsoft SQL Server\150\Tools\Binn\sqllocaldb.exe"
$sqlcmdPath = "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"

# 如果路径不存在，尝试在PATH中查找
if (-not (Test-Path $localdbPath)) {
    $found = Get-Command sqllocaldb -ErrorAction SilentlyContinue
    if ($found) {
        $localdbPath = "sqllocaldb"
    }
}

if (-not (Test-Path $sqlcmdPath)) {
    $found = Get-Command sqlcmd -ErrorAction SilentlyContinue
    if ($found) {
        $sqlcmdPath = "sqlcmd"
    }
}

Write-Host "========================================" -ForegroundColor Green
Write-Host "  Fix Database Connection" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Step 1: Check LocalDB
Write-Host "[Step 1] Checking LocalDB status..." -ForegroundColor Cyan
try {
    $result = & $localdbPath info MSSQLLocalDB 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] LocalDB is installed" -ForegroundColor Green
        
        $state = $result | Select-String "State"
        if ($state -match "Running") {
            Write-Host "  [OK] LocalDB is running" -ForegroundColor Green
        } else {
            Write-Host "  [INFO] Starting LocalDB..." -ForegroundColor Yellow
            & $localdbPath start MSSQLLocalDB | Out-Null
            Start-Sleep -Seconds 3
            Write-Host "  [OK] LocalDB started" -ForegroundColor Green
        }
    } else {
        Write-Host "  [ERROR] LocalDB not installed or PATH not configured" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "  [ERROR] Failed to check LocalDB: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 2: Check SmallTarget Database
Write-Host "[Step 2] Checking SmallTarget database..." -ForegroundColor Cyan
try {
    $dbCheck = & $sqlcmdPath -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'" -W -h -1 2>&1
    if ($dbCheck -match "SmallTarget") {
        Write-Host "  [OK] SmallTarget database exists" -ForegroundColor Green
    } else {
        Write-Host "  [WARN] SmallTarget database does not exist" -ForegroundColor Yellow
        Write-Host "  [CREATE] Creating SmallTarget database..." -ForegroundColor Cyan
        
        $createResult = & $sqlcmdPath -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE SmallTarget" 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [OK] Database created successfully" -ForegroundColor Green
            Write-Host "  [NOTE] Database is empty, need to run migrations or import data" -ForegroundColor Yellow
        } else {
            Write-Host "  [ERROR] Failed to create database" -ForegroundColor Red
            Write-Host "  Error: $createResult" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "  [ERROR] Failed to check database: $_" -ForegroundColor Red
}

Write-Host ""

# Step 3: Test Database Connection
Write-Host "[Step 3] Testing database connection..." -ForegroundColor Cyan
try {
    $testResult = & $sqlcmdPath -S "(localdb)\MSSQLLocalDB" -d "SmallTarget" -Q "SELECT DB_NAME() AS CurrentDatabase" -W -h -1 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] Database connection successful" -ForegroundColor Green
        Write-Host "  $testResult" -ForegroundColor Gray
    } else {
        Write-Host "  [ERROR] Database connection failed" -ForegroundColor Red
        Write-Host "  Error: $testResult" -ForegroundColor Red
    }
} catch {
    Write-Host "  [ERROR] Failed to test connection: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Database Fix Complete" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green





