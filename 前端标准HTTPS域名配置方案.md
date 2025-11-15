# 前端标准 HTTPS 域名配置方案

## 📋 配置目标

将前端配置为使用 `https://www.a292428dsj.dpdns.org/` 访问（标准 HTTPS 端口 443，无端口号）。

---

## 🎯 方案选择

### 方案 1: 使用 Nginx 反向代理（推荐 - 跨平台）

**优点**:
- ✅ 跨平台支持（Windows/Linux）
- ✅ 性能优秀
- ✅ 配置灵活
- ✅ 支持 WebSocket（HMR 需要）

**适用场景**: 生产环境、Linux 服务器

---

### 方案 2: 使用 IIS 反向代理（Windows 原生）

**优点**:
- ✅ Windows 原生支持
- ✅ 图形界面管理
- ✅ 集成 Windows 证书管理

**适用场景**: Windows 服务器、本地开发

---

### 方案 3: 使用 Cloudflare Tunnel（最简单）

**优点**:
- ✅ 无需配置反向代理
- ✅ 自动处理 SSL 证书
- ✅ 无需开放端口
- ✅ 支持标准 HTTPS

**适用场景**: 快速部署、无需服务器配置

---

## 🚀 方案 1: Nginx 反向代理配置

### 步骤 1: 安装 Nginx（Windows）

**方法 A: 使用 Chocolatey**
```powershell
choco install nginx
```

**方法 B: 手动安装**
1. 下载: https://nginx.org/en/download.html
2. 解压到 `C:\nginx`
3. 添加到系统 PATH

---

### 步骤 2: 配置 Nginx

**配置文件**: `nginx.conf`（已创建）

**关键配置**:
```nginx
server {
    listen 443 ssl http2;
    server_name www.a292428dsj.dpdns.org;
    
    # SSL 证书配置
    ssl_certificate /path/to/certificate.pem;
    ssl_certificate_key /path/to/private-key.pem;
    
    # 代理到 Vite 开发服务器
    location / {
        proxy_pass https://localhost:8443;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        
        # WebSocket 支持
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
    }
}
```

---

### 步骤 3: 配置 SSL 证书

#### 选项 A: 使用 Let's Encrypt（免费）

**Windows 上使用 Certbot**:
```powershell
# 安装 Certbot
choco install certbot

# 获取证书
certbot certonly --standalone -d www.a292428dsj.dpdns.org
```

**证书位置**:
- 证书: `C:\Certbot\live\www.a292428dsj.dpdns.org\fullchain.pem`
- 私钥: `C:\Certbot\live\www.a292428dsj.dpdns.org\privkey.pem`

#### 选项 B: 使用 Cloudflare 证书

如果域名使用 Cloudflare DNS，可以使用 Cloudflare Origin Certificate。

---

### 步骤 4: 启动 Nginx

```powershell
# 测试配置
nginx -t

# 启动 Nginx
nginx

# 重新加载配置
nginx -s reload
```

---

## 🚀 方案 2: IIS 反向代理配置

### 步骤 1: 安装必要组件

**运行配置脚本**:
```powershell
# 以管理员身份运行
.\IIS反向代理配置.ps1
```

**手动安装**:
1. 安装 IIS（通过 Windows 功能）
2. 安装 URL Rewrite 模块: https://www.iis.net/downloads/microsoft/url-rewrite
3. 安装 Application Request Routing: https://www.iis.net/downloads/microsoft/application-request-routing

---

### 步骤 2: 配置 SSL 证书

1. 打开 IIS 管理器
2. 选择网站 → 绑定 → 添加
3. 类型: HTTPS
4. 端口: 443
5. 选择 SSL 证书

---

### 步骤 3: 配置反向代理

脚本会自动创建 `web.config` 文件，配置反向代理规则。

---

## 🚀 方案 3: Cloudflare Tunnel（推荐 - 最简单）

### 步骤 1: 安装 Cloudflared

```powershell
# 使用 Chocolatey
choco install cloudflared

# 或下载: https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/install-and-setup/installation/
```

---

### 步骤 2: 登录 Cloudflare

```powershell
cloudflared tunnel login
```

---

### 步骤 3: 创建 Tunnel

```powershell
cloudflared tunnel create a292428dsj-frontend
```

---

### 步骤 4: 配置路由

在 Cloudflare Dashboard 中:
1. 进入 Zero Trust → Networks → Tunnels
2. 选择创建的 Tunnel
3. 添加 Public Hostname:
   - **Subdomain**: `www`
   - **Domain**: `a292428dsj.dpdns.org`
   - **Service**: `https://localhost:8443`

---

### 步骤 5: 运行 Tunnel

```powershell
cloudflared tunnel run a292428dsj-frontend
```

---

## 📝 当前 Vite 配置

**文件**: `src/Frontend/web-app/vite.config.ts`

**当前配置**:
```typescript
server: {
  port: 8443,        // HTTPS 端口
  host: '0.0.0.0',    // 允许外部访问
  https: true,       // 启用 HTTPS
}
```

**保持不变**: Vite 继续使用 8443 端口，反向代理负责处理 443 端口。

---

## ✅ 验证配置

### 1. 检查服务状态

```powershell
# 检查 Vite 服务
netstat -ano | findstr ":8443"

# 检查反向代理（Nginx）
netstat -ano | findstr ":443"

# 检查反向代理（IIS）
Get-Website | Where-Object {$_.Name -like "*a292428dsj*"}
```

---

### 2. 测试访问

**浏览器访问**:
```
https://www.a292428dsj.dpdns.org/
```

**应该看到**:
- ✅ 网站正常加载
- ✅ 浏览器显示安全连接（绿色锁）
- ✅ 无端口号（标准 HTTPS）

---

### 3. 检查 API 连接

打开浏览器开发者工具（F12）:
- Network 标签中，API 请求应该返回 200 OK
- 不应该有 CORS 错误
- 不应该有证书错误

---

## 🔧 后端 CORS 配置

**文件**: `src/Backend/UnifiedPlatform.WebApi/Program.cs`

**确保包含**:
```csharp
.WithOrigins(
    "https://www.a292428dsj.dpdns.org",  // 标准 HTTPS（无端口号）
    "https://a292428dsj.dpdns.org",
    // ... 其他域名
)
```

---

## ⚠️ 常见问题

### 问题 1: 证书警告

**原因**: 使用自签名证书或证书未正确配置

**解决方法**:
- 使用 Let's Encrypt 获取有效证书
- 或使用 Cloudflare Origin Certificate
- 或使用 Cloudflare Tunnel（自动处理证书）

---

### 问题 2: 502 Bad Gateway

**原因**: Vite 开发服务器未运行或端口不匹配

**解决方法**:
1. 确保 Vite 服务运行在 `https://localhost:8443`
2. 检查反向代理配置中的 `proxy_pass` 地址
3. 检查防火墙设置

---

### 问题 3: WebSocket 连接失败（HMR 不工作）

**原因**: 反向代理未正确配置 WebSocket 支持

**解决方法**:
- Nginx: 确保包含 `Upgrade` 和 `Connection` 头
- IIS: 确保 ARR 正确配置

---

### 问题 4: CORS 错误

**原因**: 后端 CORS 配置未包含新域名

**解决方法**:
1. 更新 `Program.cs` 中的 CORS 配置
2. 重启后端服务

---

## 📋 推荐方案

### 快速部署（推荐）
**使用 Cloudflare Tunnel**:
- ✅ 最简单
- ✅ 自动处理 SSL
- ✅ 无需服务器配置
- ✅ 支持标准 HTTPS

### 生产环境
**使用 Nginx 反向代理**:
- ✅ 性能优秀
- ✅ 配置灵活
- ✅ 支持高级功能

### Windows 服务器
**使用 IIS 反向代理**:
- ✅ Windows 原生
- ✅ 图形界面管理
- ✅ 集成证书管理

---

## 🎯 下一步

1. **选择方案**（推荐 Cloudflare Tunnel）
2. **按照对应方案的步骤配置**
3. **验证访问**: `https://www.a292428dsj.dpdns.org/`
4. **测试功能**: 确保所有功能正常

---

## 📞 技术支持

如果遇到问题：
1. 检查服务是否正常运行
2. 检查反向代理配置
3. 检查 SSL 证书配置
4. 查看浏览器控制台和网络请求

