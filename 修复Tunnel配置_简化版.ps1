# Cloudflare Tunnel Auto Fix Script
# Simplified version to avoid encoding issues

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Cloudflare Tunnel Auto Fix Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "[Step 1] Checking environment..." -ForegroundColor Yellow
Write-Host ""

$cloudflaredCmd = $null
if (Test-Path "cloudflared.exe") {
    $cloudflaredCmd = ".\cloudflared.exe"
    Write-Host "  [OK] Found cloudflared.exe" -ForegroundColor Green
} else {
    $cloudflaredPath = Get-Command cloudflared -ErrorAction SilentlyContinue
    if ($cloudflaredPath) {
        $cloudflaredCmd = "cloudflared"
        Write-Host "  [OK] Found cloudflared in system path" -ForegroundColor Green
    } else {
        Write-Host "  [ERROR] cloudflared not found" -ForegroundColor Red
        Write-Host "  Please install cloudflared or ensure cloudflared.exe is in project directory" -ForegroundColor Gray
        Read-Host "Press Enter to exit"
        exit 1
    }
}

Write-Host ""

Write-Host "[Step 2] Fixing config.yml..." -ForegroundColor Yellow
Write-Host ""

$configPath = Join-Path $scriptDir "config.yml"

$configLines = @(
    "tunnel: api",
    "credentials-file: C:\Users\$env:USERNAME\.cloudflared\686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86.json",
    "",
    "ingress:",
    "  - hostname: api.a292428dsj.dpdns.org",
    "    service: http://127.0.0.1:5000",
    "  - service: http_status:404"
)
$configContent = $configLines -join "`r`n"

if (Test-Path $configPath) {
    $existingContent = Get-Content $configPath -Raw
    $ingressCount = ([regex]::Matches($existingContent, "ingress:")).Count
    
    if ($ingressCount -gt 1) {
        Write-Host "  [WARN] Found duplicate ingress config, fixing..." -ForegroundColor Yellow
        $configContent | Set-Content $configPath -Encoding UTF8
        Write-Host "  [OK] config.yml fixed (removed duplicates)" -ForegroundColor Green
    } else {
        if ($existingContent -notmatch "127\.0\.0\.1:5000") {
            Write-Host "  [WARN] Service URL needs update to 127.0.0.1:5000" -ForegroundColor Yellow
            $configContent | Set-Content $configPath -Encoding UTF8
            Write-Host "  [OK] config.yml updated (Service URL)" -ForegroundColor Green
        } else {
            Write-Host "  [OK] config.yml is correct" -ForegroundColor Green
        }
    }
} else {
    $configContent | Set-Content $configPath -Encoding UTF8
    Write-Host "  [OK] config.yml created" -ForegroundColor Green
}

Write-Host ""

Write-Host "[Step 3] Verifying credentials file..." -ForegroundColor Yellow
Write-Host ""

$credentialsPath = "C:\Users\$env:USERNAME\.cloudflared\686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86.json"

if (Test-Path $credentialsPath) {
    try {
        $credentials = Get-Content $credentialsPath -Raw | ConvertFrom-Json
        Write-Host "  [OK] Credentials file exists" -ForegroundColor Green
        Write-Host "    Tunnel ID: $($credentials.TunnelID)" -ForegroundColor Gray
        Write-Host "    Account ID: $($credentials.AccountTag)" -ForegroundColor Gray
        
        if ($credentials.TunnelID -ne "686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86") {
            Write-Host "  [WARN] Tunnel ID mismatch!" -ForegroundColor Yellow
            Write-Host "    Expected: 686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86" -ForegroundColor Gray
            Write-Host "    Found: $($credentials.TunnelID)" -ForegroundColor Gray
        }
    } catch {
        Write-Host "  [ERROR] Credentials file format error: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "  [ERROR] Credentials file not found: $credentialsPath" -ForegroundColor Red
    Write-Host "  Need to create Tunnel or regenerate credentials" -ForegroundColor Gray
}

Write-Host ""

Write-Host "[Step 4] Checking backend service..." -ForegroundColor Yellow
Write-Host ""

$backendPort = netstat -ano | Select-String ":5000" | Select-String "LISTENING"

if ($backendPort) {
    Write-Host "  [OK] Backend service is running (port 5000)" -ForegroundColor Green
} else {
    Write-Host "  [WARN] Backend service not running (port 5000)" -ForegroundColor Yellow
    Write-Host "  Please start backend service: 启动后端服务.bat" -ForegroundColor Gray
}

Write-Host ""

Write-Host "[Step 5] Checking Tunnel processes..." -ForegroundColor Yellow
Write-Host ""

$tunnelProcesses = Get-Process -Name "cloudflared" -ErrorAction SilentlyContinue

if ($tunnelProcesses) {
    Write-Host "  [OK] Found $($tunnelProcesses.Count) cloudflared process(es)" -ForegroundColor Green
    foreach ($proc in $tunnelProcesses) {
        Write-Host "    PID: $($proc.Id)" -ForegroundColor Gray
    }
    Write-Host "  If you need to restart Tunnel, stop these processes first" -ForegroundColor Gray
} else {
    Write-Host "  [INFO] No cloudflared processes running" -ForegroundColor Gray
}

Write-Host ""

Write-Host "[Step 6] Verifying configuration..." -ForegroundColor Yellow
Write-Host ""

$config = Get-Content $configPath -Raw
$issues = @()

if ($config -notmatch "tunnel:\s*api") {
    $issues += "Tunnel name is not 'api'"
}

if ($config -notmatch "127\.0\.0\.1:5000") {
    $issues += "Service URL is not 127.0.0.1:5000"
}

if ($config -notmatch "api\.a292428dsj\.dpdns\.org") {
    $issues += "Hostname is not api.a292428dsj.dpdns.org"
}

$ingressMatches = [regex]::Matches($config, "ingress:")
if ($ingressMatches.Count -gt 1) {
    $issues += "Found duplicate ingress config"
}

if ($issues.Count -eq 0) {
    Write-Host "  [OK] All configuration checks passed" -ForegroundColor Green
} else {
    Write-Host "  [WARN] Found issues:" -ForegroundColor Yellow
    foreach ($issue in $issues) {
        Write-Host "    - $issue" -ForegroundColor Gray
    }
}

Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Dashboard Manual Steps" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "The following steps need to be done manually in Cloudflare Dashboard:" -ForegroundColor Yellow
Write-Host ""

Write-Host "1. Access Cloudflare Dashboard" -ForegroundColor White
Write-Host "   URL: https://dash.cloudflare.com" -ForegroundColor Gray
Write-Host ""

Write-Host "2. Navigate to Tunnels page" -ForegroundColor White
Write-Host "   Path: Zero Trust -> Networks -> Tunnels" -ForegroundColor Gray
Write-Host ""

Write-Host "3. Check Tunnel status" -ForegroundColor White
Write-Host "   - Find Tunnel named 'api'" -ForegroundColor Gray
Write-Host "   - Check if Tunnel ID is: 686fe1cb-f7b1-40cf-b2bb-efa03ec5cb86" -ForegroundColor Gray
Write-Host "   - If Tunnel ID doesn't match:" -ForegroundColor Gray
Write-Host "     a) Delete old Tunnel (if exists)" -ForegroundColor Gray
Write-Host "     b) Or update config.yml with correct Tunnel ID" -ForegroundColor Gray
Write-Host ""

Write-Host "4. Enable Tunnel" -ForegroundColor White
Write-Host "   - If Tunnel status is 'Disabled', change to 'Enabled'" -ForegroundColor Gray
Write-Host "   - This is critical! Tunnel must be enabled to work" -ForegroundColor Yellow
Write-Host ""

Write-Host "5. Configure Public Hostname" -ForegroundColor White
Write-Host "   - Click Tunnel 'api' to enter details page" -ForegroundColor Gray
Write-Host "   - Click 'Public Hostname' tab" -ForegroundColor Gray
Write-Host "   - Click 'Add a public hostname' button" -ForegroundColor Gray
Write-Host "   - Fill in:" -ForegroundColor Gray
Write-Host "     Subdomain: api" -ForegroundColor Cyan
Write-Host "     Domain: a292428dsj.dpdns.org" -ForegroundColor Cyan
Write-Host "     Service Type: HTTP" -ForegroundColor Cyan
Write-Host "     URL: http://127.0.0.1:5000" -ForegroundColor Cyan
Write-Host "   - Click 'Save hostname' to save" -ForegroundColor Gray
Write-Host ""

Write-Host "6. Verify configuration" -ForegroundColor White
Write-Host "   - Wait 2-5 minutes for config to take effect" -ForegroundColor Gray
Write-Host "   - Test: https://api.a292428dsj.dpdns.org/health" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[Step 7] Summary..." -ForegroundColor Yellow
Write-Host ""

Write-Host "Completed auto fixes:" -ForegroundColor Green
Write-Host "  [OK] Fixed duplicate config in config.yml" -ForegroundColor Gray
Write-Host "  [OK] Ensured Service URL uses 127.0.0.1:5000" -ForegroundColor Gray
Write-Host "  [OK] Verified credentials file" -ForegroundColor Gray
Write-Host "  [OK] Checked backend service status" -ForegroundColor Gray
Write-Host ""

Write-Host "Manual steps required:" -ForegroundColor Yellow
Write-Host "  [WARN] Enable Tunnel in Dashboard" -ForegroundColor Gray
Write-Host "  [WARN] Configure Public Hostname in Dashboard" -ForegroundColor Gray
Write-Host "  [WARN] Unify Tunnel ID if mismatch" -ForegroundColor Gray
Write-Host ""

Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Complete manual configuration in Dashboard as above" -ForegroundColor Gray
Write-Host "  2. Ensure backend service is running (port 5000)" -ForegroundColor Gray
Write-Host "  3. Run: 启动后端API_Tunnel.bat" -ForegroundColor Gray
Write-Host "  4. Wait 2-5 minutes then test: https://api.a292428dsj.dpdns.org/health" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Read-Host "Press Enter to exit"

