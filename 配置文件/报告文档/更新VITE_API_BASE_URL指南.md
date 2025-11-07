# 更新 VITE_API_BASE_URL 指南

## 📋 当前状态

根据项目上下文：
- **后端服务**: .NET 8 Web API
- **本地开发地址**: http://localhost:5195
- **生产环境**: 尚未部署
- **前端部署**: Cloudflare Pages（配置中）

---

## 🎯 更新策略

### 方案A：后端已部署（推荐）

如果后端已经部署到Railway/Fly.io/Render等平台，直接使用部署URL：

```
VITE_API_BASE_URL=https://your-actual-backend-url.com
```

### 方案B：后端未部署（需要先部署）

需要先部署后端服务，然后更新环境变量。

---

## 🚀 快速部署后端到Railway（推荐）

### 步骤1：准备部署

1. **确保Dockerfile存在**
   - 已创建：`src/Backend/UnifiedPlatform.WebApi/Dockerfile` ✅

2. **准备生产环境配置**
   - 已创建：`appsettings.Production.json` ✅

### 步骤2：部署到Railway

1. **访问Railway**
   - 打开 https://railway.app/
   - 使用GitHub登录

2. **创建新项目**
   - 点击 "New Project"
   - 选择 "Deploy from GitHub repo"
   - 选择你的仓库

3. **配置服务**
   - Railway会自动检测Dockerfile
   - 或选择 "Empty Service" 然后配置

4. **设置环境变量**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=你的数据库连接字符串
   JwtSettings__SecurityKey=你的JWT密钥
   TronSettings__Network=MainNet
   TronSettings__ApiKey=你的TRON API密钥
   ```

5. **部署**
   - Railway会自动部署
   - 获得URL：`https://your-app.railway.app`

### 步骤3：更新前端环境变量

在Cloudflare Pages中更新：
```
VITE_API_BASE_URL=https://your-app.railway.app
```

---

## 🔧 临时配置方案

如果暂时无法部署后端，可以使用以下临时方案：

### 方案1：使用本地开发服务器（仅测试）

**注意**: 这仅适用于测试，生产环境不可用

```
VITE_API_BASE_URL=http://localhost:5195
```

**限制**:
- 只能在同一台机器上访问
- 不适合生产环境

### 方案2：使用ngrok等隧道服务（临时测试）

1. **安装ngrok**
   ```bash
   # 下载ngrok
   # 启动本地后端服务
   # 运行: ngrok http 5195
   ```

2. **获得临时URL**
   ```
   VITE_API_BASE_URL=https://xxx.ngrok.io
   ```

**注意**: ngrok免费版URL会变化，不适合生产环境

---

## ✅ 推荐的完整配置

### 生产环境配置（后端部署后）

```
VITE_API_BASE_URL=https://unified-web3-platform-api.railway.app
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

### 开发环境配置（本地测试）

```
VITE_API_BASE_URL=http://localhost:5195
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

---

## 📝 更新步骤

### 在Cloudflare Pages中更新

1. **进入项目设置**
   - 访问 https://dash.cloudflare.com/
   - 进入你的Pages项目
   - 点击 "Settings" → "Environment variables"

2. **编辑环境变量**
   - 找到 `VITE_API_BASE_URL`
   - 点击编辑
   - 更新为实际后端URL
   - 保存

3. **重新部署**
   - 环境变量更新后会自动触发重新部署
   - 或手动触发部署

---

## 🔍 验证更新

### 1. 检查环境变量

部署后，在浏览器控制台检查：
```javascript
console.log('API URL:', import.meta.env.VITE_API_BASE_URL)
```

### 2. 检查API连接

- 打开浏览器开发者工具
- 切换到"Network"标签
- 查看API请求是否指向正确的URL

### 3. 检查控制台日志

应该看到：
```
API Base URL from .env: https://your-actual-backend-url.com
```

---

## ⚠️ 重要提示

1. **必须先部署后端**
   - 前端依赖后端API
   - 没有后端，前端无法正常工作

2. **确保CORS配置**
   - 后端必须配置CORS允许前端域名
   - 更新 `Program.cs` 中的CORS配置

3. **使用HTTPS**
   - 生产环境必须使用HTTPS
   - 确保后端URL使用 `https://`

4. **环境变量优先级**
   - Cloudflare Pages环境变量 > `.env.production` > `serverConfig.json`

---

## 🎯 下一步操作

### 如果后端已部署：
1. 复制后端部署URL
2. 在Cloudflare Pages中更新 `VITE_API_BASE_URL`
3. 保存并重新部署

### 如果后端未部署：
1. 按照"快速部署后端到Railway"步骤部署后端
2. 获得部署URL
3. 更新前端环境变量
4. 配置后端CORS允许前端域名

---

## 📚 相关文档

- 后端部署指南: `配置文件\报告文档\Cloudflare部署完整方案.md`
- Dockerfile: `src/Backend/UnifiedPlatform.WebApi/Dockerfile`
- 生产配置: `src/Backend/UnifiedPlatform.WebApi/appsettings.Production.json`

