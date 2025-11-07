# Check LocalDB Installation
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Check LocalDB Installation" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Check if sqllocaldb command exists
Write-Host "[Check 1] Checking sqllocaldb command..." -ForegroundColor Yellow
$sqllocaldbCmd = Get-Command sqllocaldb -ErrorAction SilentlyContinue
if ($sqllocaldbCmd) {
    Write-Host "  [OK] sqllocaldb command is available" -ForegroundColor Green
    Write-Host "  Location: $($sqllocaldbCmd.Path)" -ForegroundColor Cyan
} else {
    Write-Host "  [ERROR] sqllocaldb command not found" -ForegroundColor Red
    Write-Host "  [INFO] LocalDB is not installed or PATH is not configured" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  [Installation Methods]:" -ForegroundColor Cyan
    Write-Host "  1. Through Visual Studio Installer (Recommended)" -ForegroundColor White
    Write-Host "     - Open Visual Studio Installer" -ForegroundColor Gray
    Write-Host "     - Click 'Modify' on your Visual Studio installation" -ForegroundColor Gray
    Write-Host "     - Go to 'Individual components' tab" -ForegroundColor Gray
    Write-Host "     - Search for 'LocalDB'" -ForegroundColor Gray
    Write-Host "     - Check 'SQL Server Express LocalDB'" -ForegroundColor Gray
    Write-Host "     - Click 'Modify' to install" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  2. Install SQL Server Express separately" -ForegroundColor White
    Write-Host "     - Visit: https://www.microsoft.com/sql-server/sql-server-downloads" -ForegroundColor Gray
    Write-Host "     - Download Express version" -ForegroundColor Gray
    Write-Host "     - Select LocalDB feature during installation" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  [Next Step]: Run '.\打开Visual Studio Installer.bat' to open VS Installer" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# List all LocalDB instances
Write-Host "[Check 2] Listing all LocalDB instances..." -ForegroundColor Yellow
try {
    $instances = sqllocaldb info 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] Found LocalDB instances:" -ForegroundColor Green
        $instances | ForEach-Object { Write-Host "    $_" -ForegroundColor Cyan }
    } else {
        Write-Host "  [WARN] No instances found or error occurred" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  [ERROR] Failed to list instances: $_" -ForegroundColor Red
}
Write-Host ""

# Check MSSQLLocalDB instance
Write-Host "[Check 3] Checking MSSQLLocalDB instance..." -ForegroundColor Yellow
try {
    $instanceInfo = sqllocaldb info MSSQLLocalDB 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] MSSQLLocalDB instance exists" -ForegroundColor Green
        Write-Host ""
        
        # Check if instance is running
        Write-Host "[Check 4] Checking instance status..." -ForegroundColor Yellow
        $instanceInfo | Select-String "State" | Select-String "Running" | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [OK] MSSQLLocalDB is running" -ForegroundColor Green
        } else {
            Write-Host "  [INFO] MSSQLLocalDB is not running, starting..." -ForegroundColor Yellow
            sqllocaldb start MSSQLLocalDB 2>&1 | Out-Null
            Start-Sleep -Seconds 2
            Write-Host "  [OK] MSSQLLocalDB started" -ForegroundColor Green
        }
    } else {
        Write-Host "  [WARN] MSSQLLocalDB instance does not exist" -ForegroundColor Yellow
        Write-Host "  [INFO] Attempting to create instance..." -ForegroundColor Yellow
        sqllocaldb create MSSQLLocalDB 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [OK] MSSQLLocalDB instance created" -ForegroundColor Green
            sqllocaldb start MSSQLLocalDB 2>&1 | Out-Null
            Start-Sleep -Seconds 2
        } else {
            Write-Host "  [ERROR] Failed to create instance, please check permissions" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "  [ERROR] Failed to check instance: $_" -ForegroundColor Red
}
Write-Host ""

# Test database connection
Write-Host "[Check 5] Testing database connection..." -ForegroundColor Yellow
try {
    $sqlcmdResult = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" -W -h -1 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] Database connection successful" -ForegroundColor Green
        Write-Host "  Version Info:" -ForegroundColor Cyan
        $sqlcmdResult | ForEach-Object { Write-Host "    $_" -ForegroundColor White }
    } else {
        Write-Host "  [ERROR] Database connection failed" -ForegroundColor Red
        Write-Host "  [HINT] Please check:" -ForegroundColor Yellow
        Write-Host "    1. LocalDB is running" -ForegroundColor White
        Write-Host "    2. Connection string is correct" -ForegroundColor White
    }
} catch {
    Write-Host "  [ERROR] Failed to test connection: $_" -ForegroundColor Red
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "  LocalDB Check Complete" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "[Next Step]: If all checks pass, run:" -ForegroundColor Yellow
Write-Host "  .\fix_database_connection.bat" -ForegroundColor White
Write-Host "  to create SmallTarget database" -ForegroundColor White
Write-Host ""

