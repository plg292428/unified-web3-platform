# HTTPS 域名配置指南

## 📋 配置目标

将前端配置为使用 `https://www.a292428dsj.dpdns.org/` 访问。

## 🔧 配置步骤

### 1. Vite 开发服务器 HTTPS 配置

**文件**: `src/Frontend/web-app/vite.config.ts`

已更新配置：
- ✅ 端口: `443` (HTTPS 标准端口)
- ✅ 启用 HTTPS: `https: true`
- ✅ 允许外部访问: `host: '0.0.0.0'`

**注意**: 
- 如果 443 端口需要管理员权限，可以使用其他端口（如 8443）
- 需要配置 SSL 证书才能正常使用 HTTPS

---

### 2. SSL 证书配置

#### 选项 A: 使用开发证书（本地测试）

Vite 会自动生成自签名证书，但浏览器会显示警告。

#### 选项 B: 使用真实证书（生产环境）

**获取证书**:
1. 使用 Let's Encrypt（免费）
2. 从证书颁发机构购买
3. 使用 Cloudflare 等 CDN 提供的证书

**配置证书**:
```typescript
// vite.config.ts
import fs from 'fs'

server: {
  https: {
    key: fs.readFileSync('path/to/private-key.pem'),
    cert: fs.readFileSync('path/to/certificate.pem'),
  },
}
```

---

### 3. 后端 CORS 配置

**文件**: `src/Backend/UnifiedPlatform.WebApi/Program.cs`

后端 CORS 已包含以下域名：
- ✅ `https://www.a292428dsj.dpdns.org`
- ✅ `https://a292428dsj.dpdns.org`

**无需修改**，已正确配置。

---

### 4. 端口配置说明

#### 开发环境

如果 443 端口需要管理员权限，可以使用其他端口：

**方案 1: 使用 8443 端口**
```typescript
server: {
  port: 8443,
  https: true,
}
```
访问: `https://www.a292428dsj.dpdns.org:8443`

**方案 2: 使用反向代理（推荐）**

使用 Nginx 或 IIS 作为反向代理：
- 反向代理监听 443 端口（HTTPS）
- 转发到 Vite 开发服务器（5173 端口）
- 在反向代理层面处理 SSL 证书

---

### 5. Hosts 文件配置

**文件**: `C:\Windows\System32\drivers\etc\hosts`

添加以下内容（如果域名未解析到服务器）:
```
127.0.0.1    www.a292428dsj.dpdns.org
127.0.0.1    a292428dsj.dpdns.org
```

**注意**: 需要管理员权限编辑 hosts 文件

---

## 🚀 启动服务

### 方法 1: 直接启动（使用 443 端口）

**需要管理员权限**:

```bash
cd src/Frontend/web-app
npm run dev
```

访问: `https://www.a292428dsj.dpdns.org`

### 方法 2: 使用其他端口（推荐）

修改 `vite.config.ts` 使用 8443 端口：

```typescript
server: {
  port: 8443,
  https: true,
}
```

访问: `https://www.a292428dsj.dpdns.org:8443`

### 方法 3: 使用反向代理（生产环境推荐）

配置 Nginx 或 IIS 作为反向代理，处理 HTTPS 和 SSL 证书。

---

## 🔍 验证配置

### 1. 检查服务是否启动

```bash
# 检查端口监听
netstat -ano | findstr ":443"
# 或
netstat -ano | findstr ":8443"
```

### 2. 测试 HTTPS 连接

在浏览器中访问:
- `https://www.a292428dsj.dpdns.org` (如果使用 443 端口)
- `https://www.a292428dsj.dpdns.org:8443` (如果使用 8443 端口)

### 3. 检查证书

- 开发环境: 浏览器可能显示证书警告（正常，点击继续）
- 生产环境: 应该显示有效的 SSL 证书

### 4. 检查 CORS

打开浏览器开发者工具，查看 Network 标签：
- API 请求应该返回 200 OK
- 不应该有 CORS 错误

---

## ⚠️ 常见问题

### 问题 1: 端口 443 需要管理员权限

**解决方案**:
- 使用其他端口（如 8443）
- 或使用反向代理

### 问题 2: 浏览器显示证书警告

**开发环境**: 这是正常的，因为使用自签名证书
- 点击"高级" → "继续访问"

**生产环境**: 需要使用有效的 SSL 证书

### 问题 3: 无法访问域名

**可能原因**:
- DNS 未解析
- hosts 文件未配置
- 防火墙阻止

**解决方法**:
1. 检查 hosts 文件配置
2. 检查 DNS 解析
3. 检查防火墙设置

### 问题 4: CORS 错误

**解决方法**:
1. 检查后端 CORS 配置
2. 确保包含 `https://www.a292428dsj.dpdns.org`
3. 重启后端服务

---

## 📝 配置检查清单

- [ ] Vite 配置已更新（HTTPS 启用）
- [ ] 端口配置正确（443 或 8443）
- [ ] SSL 证书已配置（开发或生产）
- [ ] 后端 CORS 已包含域名
- [ ] Hosts 文件已配置（如果需要）
- [ ] 服务可以正常启动
- [ ] 浏览器可以访问 HTTPS 地址
- [ ] API 请求没有 CORS 错误

---

## 🎯 推荐配置（生产环境）

### 使用反向代理（Nginx）

```nginx
server {
    listen 443 ssl;
    server_name www.a292428dsj.dpdns.org;

    ssl_certificate /path/to/certificate.pem;
    ssl_certificate_key /path/to/private-key.pem;

    location / {
        proxy_pass http://localhost:5173;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 使用 Cloudflare

1. 在 Cloudflare 配置域名
2. 启用 SSL/TLS（自动证书）
3. 配置页面规则或 Workers
4. 转发到本地开发服务器

---

## 📚 相关文档

- Vite HTTPS 配置: https://vitejs.dev/config/server-options.html#server-https
- SSL 证书获取: https://letsencrypt.org/
- Nginx 反向代理: https://nginx.org/en/docs/http/ngx_http_proxy_module.html

