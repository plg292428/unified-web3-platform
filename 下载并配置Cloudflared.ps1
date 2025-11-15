# Download and configure Cloudflared
# No admin rights required

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "========================================" -ForegroundColor Green
Write-Host "  Download and Configure Cloudflared" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Check if already installed
$cloudflaredPath = Get-Command cloudflared -ErrorAction SilentlyContinue
if ($cloudflaredPath) {
    Write-Host "[OK] Cloudflared is already installed" -ForegroundColor Green
    cloudflared --version
    Write-Host ""
    Write-Host "You can now run: 配置CloudflareTunnel.bat" -ForegroundColor Cyan
    exit 0
}

# Project directory
$projectDir = "D:\claude code\plg\UnifiedWeb3Platform"
$cloudflaredExe = Join-Path $projectDir "cloudflared.exe"

# Check if cloudflared.exe exists in project directory
if (Test-Path $cloudflaredExe) {
    Write-Host "[OK] Found cloudflared.exe in project directory" -ForegroundColor Green
    Write-Host "Adding to PATH for current session..." -ForegroundColor Cyan
    
    # Add to PATH for current session
    $env:Path = $env:Path + ";" + $projectDir
    
    Write-Host "[OK] Added to PATH" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run: 配置CloudflareTunnel.bat" -ForegroundColor Cyan
    exit 0
}

# Download Cloudflared
Write-Host "[!] Cloudflared is not installed" -ForegroundColor Yellow
Write-Host ""
Write-Host "Getting latest version information..." -ForegroundColor Cyan

try {
    # Get latest version
    $latestRelease = Invoke-RestMethod -Uri "https://api.github.com/repos/cloudflare/cloudflared/releases/latest"
    $version = $latestRelease.tag_name
    
    Write-Host "Latest version: $version" -ForegroundColor Green
    Write-Host ""
    
    # Download URL
    $downloadUrl = "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe"
    $downloadPath = Join-Path $env:TEMP "cloudflared-windows-amd64.exe"
    
    Write-Host "Downloading Cloudflared..." -ForegroundColor Cyan
    Write-Host "Download URL: $downloadUrl" -ForegroundColor Gray
    Write-Host "Save location: $downloadPath" -ForegroundColor Gray
    Write-Host ""
    
    # Download file
    Invoke-WebRequest -Uri $downloadUrl -OutFile $downloadPath -UseBasicParsing
    
    Write-Host "[OK] Download completed" -ForegroundColor Green
    Write-Host ""
    
    # Copy to project directory
    Write-Host "Copying to project directory..." -ForegroundColor Cyan
    Copy-Item -Path $downloadPath -Destination $cloudflaredExe -Force
    
    Write-Host "[OK] Copied to: $cloudflaredExe" -ForegroundColor Green
    Write-Host ""
    
    # Add to PATH for current session
    $env:Path = $env:Path + ";" + $projectDir
    
    Write-Host "[OK] Added to PATH" -ForegroundColor Green
    Write-Host ""
    
    # Verify installation
    Write-Host "Verifying installation..." -ForegroundColor Cyan
    & $cloudflaredExe --version
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Installation Complete" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next step: Run 配置CloudflareTunnel.bat" -ForegroundColor Cyan
    Write-Host ""
    
} catch {
    Write-Host "[ERROR] Download failed: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please download manually:" -ForegroundColor Yellow
    Write-Host "1. Visit: https://github.com/cloudflare/cloudflared/releases/latest" -ForegroundColor White
    Write-Host "2. Download: cloudflared-windows-amd64.exe" -ForegroundColor White
    Write-Host "3. Rename to: cloudflared.exe" -ForegroundColor White
    Write-Host "4. Place in project directory: $projectDir" -ForegroundColor White
    Write-Host ""
    exit 1
}
