# Cloudflare部署完整流程

## 📋 部署架构

```
用户请求
    ↓
Cloudflare CDN (全球加速)
    ↓
┌─────────────────┬─────────────────┐
│ Cloudflare Pages│ Cloudflare      │
│ (前端Vue应用)   │ Workers         │
│                 │ (API网关)       │
└────────┬────────┴────────┬────────┘
         │                 │
         │                 ▼
         │         ┌──────────────┐
         │         │ 后端服务器   │
         │         │ (.NET 8 API) │
         │         │ (Railway)     │
         │         └──────────────┘
         │
         ▼
┌─────────────────┐
│ Cloudflare R2   │
│ (静态资源)      │
└─────────────────┘
```

---

## 🚀 第一步：部署前端到Cloudflare Pages

### 1.1 准备构建

```powershell
cd src\Frontend\web-app

# 安装依赖（如果未安装）
npm install

# 创建生产环境配置
# 编辑 .env.production，设置正确的API地址

# 构建项目
npm run build
```

### 1.2 通过Cloudflare Dashboard部署

1. **访问Cloudflare Dashboard**
   - 打开 https://dash.cloudflare.com/
   - 登录账户

2. **创建Pages项目**
   - 点击左侧菜单 "Workers & Pages"
   - 点击 "Create application"
   - 选择 "Pages" → "Connect to Git"

3. **连接Git仓库**
   - 选择GitHub/GitLab/Bitbucket
   - 授权访问
   - 选择仓库

4. **配置构建设置**
   ```
   Project name: unified-web3-platform
   Production branch: main
   Framework preset: Vite
   Build command: npm run build
   Build output directory: dist
   Root directory: src/Frontend/web-app
   ```

5. **配置环境变量**
   ```
   VITE_API_BASE_URL=https://your-api.railway.app
   VITE_APP_NAME=UnifiedWeb3Platform
   NODE_VERSION=20
   ```

6. **保存并部署**
   - 点击 "Save and Deploy"
   - 等待构建完成
   - 获得部署URL: `https://unified-web3-platform.pages.dev`

### 1.3 通过Wrangler CLI部署（可选）

```bash
# 安装Wrangler
npm install -g wrangler

# 登录
wrangler login

# 创建项目
wrangler pages project create unified-web3-platform

# 部署
cd src/Frontend/web-app
npm run build
wrangler pages deploy dist --project-name=unified-web3-platform
```

---

## 🔧 第二步：部署后端到Railway

### 2.1 准备Dockerfile

已创建 `src/Backend/UnifiedPlatform.WebApi/Dockerfile`

### 2.2 在Railway部署

1. **访问Railway**
   - 打开 https://railway.app/
   - 使用GitHub登录

2. **创建新项目**
   - 点击 "New Project"
   - 选择 "Deploy from GitHub repo"
   - 选择仓库

3. **配置服务**
   - Railway会自动检测Dockerfile
   - 或选择 "Empty Service" 然后配置

4. **设置环境变量**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=Server=...;Database=...;User Id=...;Password=...;
   JwtSettings__SecurityKey=your-production-jwt-key
   TronSettings__Network=MainNet
   TronSettings__ApiKey=your-tron-api-key
   ```

5. **配置数据库**
   - 在Railway中添加PostgreSQL服务
   - 更新连接字符串
   - 运行数据库迁移

6. **部署**
   - Railway会自动部署
   - 获得URL: `https://your-app.railway.app`

---

## 🌐 第三步：配置API网关（可选）

### 3.1 创建Workers脚本

```javascript
// workers/api-gateway.js
export default {
  async fetch(request, env) {
    const url = new URL(request.url);
    
    // 只处理API请求
    if (!url.pathname.startsWith('/api/')) {
      return new Response('Not Found', { status: 404 });
    }
    
    // 转发到后端
    const backendUrl = env.BACKEND_URL;
    const newUrl = new URL(url.pathname + url.search, backendUrl);
    
    const newRequest = new Request(newUrl, {
      method: request.method,
      headers: request.headers,
      body: request.body
    });
    
    const response = await fetch(newRequest);
    
    // 添加CORS头
    const newResponse = new Response(response.body, response);
    newResponse.headers.set('Access-Control-Allow-Origin', '*');
    newResponse.headers.set('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
    newResponse.headers.set('Access-Control-Allow-Headers', 'Content-Type, Authorization');
    
    return newResponse;
  }
}
```

### 3.2 部署Workers

```bash
# 创建Workers项目
wrangler init api-gateway

# 配置wrangler.toml
# [vars]
# BACKEND_URL = "https://your-api.railway.app"

# 部署
wrangler deploy
```

---

## 🔗 第四步：配置域名和DNS

### 4.1 配置前端域名

1. 在Cloudflare Pages项目设置中
2. 点击 "Custom domains"
3. 添加你的域名
4. 配置DNS记录（自动配置）

### 4.2 配置后端域名

1. 在Railway项目设置中
2. 配置自定义域名
3. 在Cloudflare DNS中添加CNAME记录

---

## ✅ 部署检查清单

### 前端（Cloudflare Pages）
- [ ] 代码已推送到Git仓库
- [ ] Cloudflare Pages项目已创建
- [ ] 构建配置正确
- [ ] 环境变量已配置
- [ ] 构建成功
- [ ] 可以访问部署URL
- [ ] 自定义域名已配置（可选）

### 后端（Railway）
- [ ] Dockerfile已创建
- [ ] Railway项目已创建
- [ ] 环境变量已配置
- [ ] 数据库已配置
- [ ] 数据库迁移已运行
- [ ] API可以访问
- [ ] Swagger可以访问

### API网关（可选）
- [ ] Workers脚本已创建
- [ ] 后端URL已配置
- [ ] Workers已部署
- [ ] API转发正常

### 测试
- [ ] 前端可以访问
- [ ] 前端可以调用后端API
- [ ] CORS配置正确
- [ ] 认证功能正常
- [ ] Web3功能正常

---

## 🔒 安全配置

### 1. 更新CORS配置

后端 `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CloudflarePages", policy =>
    {
        policy.WithOrigins(
                "https://unified-web3-platform.pages.dev",
                "https://your-custom-domain.com"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### 2. 环境变量安全

- ✅ 使用强密码和密钥
- ✅ 不要在代码中硬编码
- ✅ 使用不同密钥用于生产环境
- ✅ 定期轮换密钥

---

## 📊 监控和维护

### Cloudflare Analytics
- Pages Analytics（自动启用）
- Workers Analytics（如果使用）

### Railway监控
- 内置日志查看
- 资源使用监控
- 错误追踪

---

## 💡 优化建议

1. **启用Cloudflare缓存**
   - 静态资源缓存
   - API响应缓存（通过Workers）

2. **使用Cloudflare R2**
   - 存储大文件
   - 减少后端负载

3. **配置CDN规则**
   - 压缩响应
   - 图片优化
   - 自动HTTPS

---

## 🆘 故障排查

### 前端无法访问
- 检查构建是否成功
- 检查环境变量配置
- 查看Cloudflare Pages日志

### API调用失败
- 检查后端服务状态
- 检查CORS配置
- 检查环境变量
- 查看Railway日志

### 数据库连接失败
- 检查连接字符串
- 检查数据库服务状态
- 检查防火墙规则

---

## 📚 参考文档

- [Cloudflare Pages文档](https://developers.cloudflare.com/pages/)
- [Railway文档](https://docs.railway.app/)
- [Cloudflare Workers文档](https://developers.cloudflare.com/workers/)

