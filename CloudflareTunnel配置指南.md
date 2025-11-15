# Cloudflare Tunnel 配置指南

## 📋 配置目标

使用 Cloudflare Tunnel 将前端服务 `https://localhost:8443` 暴露为 `https://www.a292428dsj.dpdns.org/`（标准 HTTPS，无端口号）。

---

## ✅ 优势

- ✅ **自动处理 SSL 证书**：无需配置证书
- ✅ **无需开放端口**：不需要在防火墙开放 443 端口
- ✅ **标准 HTTPS**：支持 `https://www.a292428dsj.dpdns.org/`（无端口号）
- ✅ **简单配置**：只需几个命令
- ✅ **免费使用**：Cloudflare Tunnel 免费

---

## 🚀 快速开始

### 方法 1: 使用配置脚本（推荐）

```bash
配置CloudflareTunnel.bat
```

脚本会自动引导您完成所有配置步骤。

---

### 方法 2: 手动配置

#### 步骤 1: 安装 Cloudflared

**使用 Chocolatey**:
```powershell
choco install cloudflared
```

**或手动下载**:
- 访问: https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/install-and-setup/installation/
- 下载 Windows 版本
- 解压并添加到系统 PATH

**验证安装**:
```bash
cloudflared --version
```

---

#### 步骤 2: 登录 Cloudflare

```bash
cloudflared tunnel login
```

**操作说明**:
1. 命令会打开浏览器
2. 登录您的 Cloudflare 账户
3. 选择要使用的域名（`a292428dsj.dpdns.org`）
4. 授权访问

**完成后**:
- 会在 `%USERPROFILE%\.cloudflared\` 目录生成证书文件
- 证书用于后续的 Tunnel 操作

---

#### 步骤 3: 创建 Tunnel

```bash
cloudflared tunnel create a292428dsj-frontend
```

**说明**:
- `a292428dsj-frontend` 是 Tunnel 名称，可以自定义
- 创建成功后会显示 Tunnel ID 和 UUID

---

#### 步骤 4: 配置路由（在 Cloudflare Dashboard）

1. **访问 Cloudflare Dashboard**:
   ```
   https://one.dash.cloudflare.com/
   ```

2. **进入 Tunnels 页面**:
   - 左侧菜单: `Zero Trust` → `Networks` → `Tunnels`
   - 或直接访问: https://one.dash.cloudflare.com/access/tunnels

3. **选择创建的 Tunnel**:
   - 找到 `a292428dsj-frontend`
   - 点击 "Configure" 或 "Public Hostname"

4. **添加 Public Hostname**:
   - 点击 "Add a public hostname"
   - 配置如下:
     - **Subdomain**: `www`
     - **Domain**: `a292428dsj.dpdns.org`
     - **Service**: `https://localhost:8443`
   - 点击 "Save hostname"

**配置说明**:
- **Subdomain**: 子域名（`www` 表示 `www.a292428dsj.dpdns.org`）
- **Domain**: 主域名（必须是您 Cloudflare 账户中的域名）
- **Service**: 本地服务地址（Vite 开发服务器地址）

---

#### 步骤 5: 运行 Tunnel

```bash
cloudflared tunnel run a292428dsj-frontend
```

**说明**:
- 命令会启动 Tunnel 并保持运行
- **请保持此窗口打开**，关闭窗口会停止 Tunnel
- 如果看到 "Connection established"，说明 Tunnel 已成功连接

---

## 🔧 后台运行（可选）

### Windows: 使用服务运行

**创建服务**:
```bash
cloudflared service install
```

**启动服务**:
```bash
cloudflared tunnel run a292428dsj-frontend
```

**或使用配置文件**:

1. **创建配置文件**: `%USERPROFILE%\.cloudflared\config.yml`
   ```yaml
   tunnel: <TUNNEL_UUID>
   credentials-file: %USERPROFILE%\.cloudflared\<TUNNEL_ID>.json
   
   ingress:
     - hostname: www.a292428dsj.dpdns.org
       service: https://localhost:8443
     - service: http_status:404
   ```

2. **运行 Tunnel**:
   ```bash
   cloudflared tunnel run
   ```

---

## ✅ 验证配置

### 1. 检查 Tunnel 状态

在运行 `cloudflared tunnel run` 的窗口中，应该看到:
```
Connection established
```

### 2. 测试访问

**浏览器访问**:
```
https://www.a292428dsj.dpdns.org/
```

**应该看到**:
- ✅ 网站正常加载
- ✅ 浏览器显示安全连接（绿色锁）
- ✅ 无端口号（标准 HTTPS）

### 3. 检查服务状态

**前端服务**:
```bash
netstat -ano | findstr ":8443"
```

**后端服务**:
```bash
netstat -ano | findstr ":5000"
```

**Tunnel 连接**:
- 在运行 Tunnel 的窗口中查看连接状态

---

## 🔄 同时配置前端和后端

如果需要同时暴露前端和后端，可以配置多个路由：

### 前端路由
- **Subdomain**: `www`
- **Domain**: `a292428dsj.dpdns.org`
- **Service**: `https://localhost:8443`

### 后端路由
- **Subdomain**: `api`
- **Domain**: `a292428dsj.dpdns.org`
- **Service**: `http://localhost:5000`

**访问地址**:
- 前端: `https://www.a292428dsj.dpdns.org/`
- 后端: `https://api.a292428dsj.dpdns.org/`

---

## ⚠️ 常见问题

### 问题 1: "Tunnel not found"

**原因**: Tunnel 未创建或名称错误

**解决方法**:
1. 检查 Tunnel 是否已创建: `cloudflared tunnel list`
2. 确认 Tunnel 名称正确
3. 如果不存在，重新创建: `cloudflared tunnel create a292428dsj-frontend`

---

### 问题 2: "Connection refused"

**原因**: 本地服务未运行

**解决方法**:
1. 确保前端服务运行在 `https://localhost:8443`
2. 确保后端服务运行在 `http://localhost:5000`
3. 检查服务是否正常启动

---

### 问题 3: "Certificate error"

**原因**: Cloudflare 证书配置问题

**解决方法**:
1. 重新登录: `cloudflared tunnel login`
2. 检查证书文件: `%USERPROFILE%\.cloudflared\cert.pem`
3. 确认域名在 Cloudflare 账户中

---

### 问题 4: "Hostname not found"

**原因**: 路由未在 Dashboard 中配置

**解决方法**:
1. 访问 Cloudflare Dashboard
2. 进入 Tunnels 页面
3. 检查 Public Hostname 配置
4. 确认 Subdomain 和 Domain 正确

---

## 📝 配置文件示例

### 完整配置文件: `config.yml`

```yaml
tunnel: <TUNNEL_UUID>
credentials-file: %USERPROFILE%\.cloudflared\<TUNNEL_ID>.json

ingress:
  # 前端路由
  - hostname: www.a292428dsj.dpdns.org
    service: https://localhost:8443
  
  # 后端路由（可选）
  - hostname: api.a292428dsj.dpdns.org
    service: http://localhost:5000
  
  # 默认路由（404）
  - service: http_status:404
```

**使用配置文件运行**:
```bash
cloudflared tunnel run
```

---

## 🎯 快速命令参考

```bash
# 登录
cloudflared tunnel login

# 创建 Tunnel
cloudflared tunnel create a292428dsj-frontend

# 列出所有 Tunnel
cloudflared tunnel list

# 运行 Tunnel
cloudflared tunnel run a292428dsj-frontend

# 删除 Tunnel
cloudflared tunnel delete a292428dsj-frontend

# 查看 Tunnel 信息
cloudflared tunnel info a292428dsj-frontend
```

---

## 📞 技术支持

如果遇到问题：
1. 检查 Cloudflare Dashboard 中的 Tunnel 状态
2. 查看 Tunnel 运行窗口的错误信息
3. 确认本地服务正常运行
4. 检查域名 DNS 配置

---

## ✅ 配置完成检查清单

- [ ] Cloudflared 已安装
- [ ] 已登录 Cloudflare 账户
- [ ] Tunnel 已创建
- [ ] 路由已在 Dashboard 中配置
- [ ] 前端服务运行在 `https://localhost:8443`
- [ ] 后端服务运行在 `http://localhost:5000`
- [ ] Tunnel 正在运行
- [ ] 可以访问 `https://www.a292428dsj.dpdns.org/`

---

配置完成后，您就可以通过 `https://www.a292428dsj.dpdns.org/` 访问网站了！

