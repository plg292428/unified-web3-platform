# IIS 反向代理配置脚本
# 用于将 https://www.a292428dsj.dpdns.org/ 代理到 Vite 开发服务器 (localhost:8443)
# 需要管理员权限运行

Write-Host "========================================" -ForegroundColor Green
Write-Host "  IIS 反向代理配置" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# 检查管理员权限
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "错误: 需要管理员权限运行此脚本" -ForegroundColor Red
    Write-Host "请右键选择 '以管理员身份运行'" -ForegroundColor Yellow
    exit 1
}

# 检查 IIS 是否安装
$iisFeature = Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
if ($iisFeature.State -ne "Enabled") {
    Write-Host "IIS 未安装，正在安装..." -ForegroundColor Yellow
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer -All
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures -All
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors -All
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationInit -All
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-URLRewriting -All
}

# 检查并安装 URL Rewrite 模块
$urlRewritePath = "C:\Program Files\IIS\URL Rewrite\rewrite.dll"
if (-not (Test-Path $urlRewritePath)) {
    Write-Host "URL Rewrite 模块未安装" -ForegroundColor Yellow
    Write-Host "请下载并安装: https://www.iis.net/downloads/microsoft/url-rewrite" -ForegroundColor Yellow
    Write-Host "或运行: winget install Microsoft.IIS.URLRewrite" -ForegroundColor Yellow
    Write-Host ""
    $install = Read-Host "是否已安装 URL Rewrite? (Y/N)"
    if ($install -ne "Y" -and $install -ne "y") {
        Write-Host "请先安装 URL Rewrite 模块" -ForegroundColor Red
        exit 1
    }
}

# 检查并安装 Application Request Routing (ARR)
$arrPath = "C:\Program Files\IIS\Application Request Routing\requestRouter.dll"
if (-not (Test-Path $arrPath)) {
    Write-Host "Application Request Routing (ARR) 未安装" -ForegroundColor Yellow
    Write-Host "请下载并安装: https://www.iis.net/downloads/microsoft/application-request-routing" -ForegroundColor Yellow
    Write-Host ""
    $install = Read-Host "是否已安装 ARR? (Y/N)"
    if ($install -ne "Y" -and $install -ne "y") {
        Write-Host "请先安装 Application Request Routing 模块" -ForegroundColor Red
        exit 1
    }
}

# 导入 WebAdministration 模块
Import-Module WebAdministration -ErrorAction SilentlyContinue

# 创建网站配置
$siteName = "a292428dsj.dpdns.org"
$bindingPort = 443
$proxyUrl = "https://localhost:8443"

Write-Host "正在配置 IIS 反向代理..." -ForegroundColor Cyan

# 检查网站是否已存在
$existingSite = Get-Website -Name $siteName -ErrorAction SilentlyContinue
if ($existingSite) {
    Write-Host "网站 '$siteName' 已存在，正在删除..." -ForegroundColor Yellow
    Remove-Website -Name $siteName
}

# 创建网站
New-Website -Name $siteName -Port 80 -PhysicalPath "C:\inetpub\wwwroot" -Force | Out-Null

# 添加 HTTPS 绑定（需要 SSL 证书）
Write-Host "添加 HTTPS 绑定..." -ForegroundColor Cyan
New-WebBinding -Name $siteName -Protocol https -Port $bindingPort -HostHeader "www.a292428dsj.dpdns.org"

# 配置反向代理规则
$webConfigPath = "C:\inetpub\wwwroot\web.config"
$webConfig = @"
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.webServer>
        <rewrite>
            <rules>
                <rule name="ReverseProxyInboundRule1" stopProcessing="true">
                    <match url="(.*)" />
                    <action type="Rewrite" url="$proxyUrl{R:1}" />
                    <serverVariables>
                        <set name="HTTP_X_FORWARDED_PROTO" value="https" />
                        <set name="HTTP_X_FORWARDED_HOST" value="{HTTP_HOST}" />
                    </serverVariables>
                </rule>
            </rules>
        </rewrite>
        <urlCompression doStaticCompression="true" doDynamicCompression="true" />
    </system.webServer>
</configuration>
"@

Set-Content -Path $webConfigPath -Value $webConfig -Force

# 启用代理功能
Set-WebConfigurationProperty -PSPath "MACHINE/WEBROOT/APPHOST" -Filter "system.webServer/proxy" -Name "enabled" -Value "True" -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  配置完成" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "下一步:" -ForegroundColor Yellow
Write-Host "1. 配置 SSL 证书（在 IIS 管理器中）" -ForegroundColor White
Write-Host "2. 确保 Vite 开发服务器运行在 https://localhost:8443" -ForegroundColor White
Write-Host "3. 访问: https://www.a292428dsj.dpdns.org/" -ForegroundColor White
Write-Host ""

