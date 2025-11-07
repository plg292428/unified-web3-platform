# VITE_API_BASE_URL 更新方案

## 📋 当前状态分析

根据项目上下文：
- **后端服务**: .NET 8 Web API
- **本地开发地址**: http://localhost:5195
- **生产环境**: 尚未部署
- **serverConfig.json**: 包含示例URL `https://api.layer2farming.ai`

---

## 🎯 更新方案

### 方案A：后端已部署（直接更新）

如果后端已经部署到Railway/Fly.io/Render等平台：

**在Cloudflare Pages环境变量中更新为：**
```
VITE_API_BASE_URL=https://your-actual-backend-url.com
```

### 方案B：后端未部署（需要先部署）

需要先部署后端服务，然后更新环境变量。

---

## 🚀 快速部署后端到Railway（推荐）

### 步骤1：准备部署

✅ **已完成**:
- Dockerfile已创建：`src/Backend/UnifiedPlatform.WebApi/Dockerfile`
- 生产配置已创建：`appsettings.Production.json`

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
   - 或手动选择Dockerfile路径：`src/Backend/UnifiedPlatform.WebApi/Dockerfile`

4. **设置环境变量**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=你的数据库连接字符串
   JwtSettings__SecurityKey=你的JWT密钥
   TronSettings__Network=MainNet
   TronSettings__ApiKey=你的TRON API密钥（如果需要）
   ```

5. **部署并获取URL**
   - Railway会自动部署
   - 部署完成后，在项目设置中查看Public URL
   - 获得URL格式：`https://your-app-name.railway.app`

### 步骤3：更新前端环境变量

在Cloudflare Pages中更新：
```
VITE_API_BASE_URL=https://your-app-name.railway.app
```

---

## 📝 推荐的完整环境变量配置

### 生产环境（后端部署后）

```
VITE_API_BASE_URL=https://unified-web3-platform-api.railway.app
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

### 开发环境（本地测试）

```
VITE_API_BASE_URL=http://localhost:5195
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

---

## ⚠️ 重要提示

### 1. 必须先部署后端
- 前端依赖后端API
- 没有后端，前端无法正常工作
- 部署后端后才能获得实际URL

### 2. 确保CORS配置
后端 `Program.cs` 需要配置CORS允许前端域名：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CloudflarePages", policy =>
    {
        policy.WithOrigins(
                "https://your-frontend.pages.dev",
                "https://your-custom-domain.com"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### 3. 使用HTTPS
- 生产环境必须使用HTTPS
- 确保后端URL使用 `https://`
- Railway/Fly.io/Render都自动提供HTTPS

---

## 🔧 临时配置（仅测试，不推荐生产）

如果暂时无法部署后端，可以：

1. **使用serverConfig.json作为后备**
   - 前端会优先使用环境变量
   - 如果环境变量不存在，会使用serverConfig.json
   - 当前serverConfig.json中有：`https://api.layer2farming.ai`

2. **更新serverConfig.json（如果该URL可用）**
   ```json
   {
     "developmentBaseUrl": "http://localhost:5195",
     "productionBaseUrl": "https://your-actual-backend-url.com"
   }
   ```

**注意**: 这种方式不推荐，因为serverConfig.json是静态文件，需要重新构建才能更新。

---

## ✅ 推荐操作流程

### 第一步：部署后端
1. 访问 https://railway.app/
2. 创建新项目
3. 连接GitHub仓库
4. Railway自动检测Dockerfile
5. 配置环境变量
6. 部署并获取URL

### 第二步：更新前端环境变量
1. 访问 https://dash.cloudflare.com/
2. 进入你的Pages项目
3. Settings → Environment variables
4. 编辑 `VITE_API_BASE_URL`
5. 更新为Railway提供的URL
6. 保存

### 第三步：配置后端CORS
1. 更新后端 `Program.cs` 中的CORS配置
2. 添加前端域名到允许列表
3. 重新部署后端

### 第四步：验证
1. 前端重新部署后
2. 打开浏览器控制台
3. 应该看到：`API Base URL from .env: https://your-backend-url`
4. 测试API调用是否成功

---

## 📚 相关文档

- 后端部署指南: `配置文件\报告文档\Cloudflare部署完整方案.md`
- Dockerfile: `src/Backend/UnifiedPlatform.WebApi/Dockerfile`
- 生产配置: `src/Backend/UnifiedPlatform.WebApi/appsettings.Production.json`

