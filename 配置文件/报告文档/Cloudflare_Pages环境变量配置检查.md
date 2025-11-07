# Cloudflare Pages 环境变量配置检查

## 📋 当前配置

根据截图显示的环境变量配置：

| 变量名称 | 当前值 | 状态 | 说明 |
|---------|--------|------|------|
| `VITE_API_BASE_URL` | `https://your-api-domain.com` | ⚠️ **需要更新** | 这是占位符，需要替换为实际后端地址 |
| `VITE_APP_NAME` | `UnifiedWeb3Platform` | ✅ 正确 | 应用名称正确 |
| `NODE_VERSION` | `20` | ✅ 正确 | Node.js版本正确 |

---

## ⚠️ 发现的问题

### 问题1：VITE_API_BASE_URL 使用占位符

**当前值**: `https://your-api-domain.com`  
**问题**: 这是占位符URL，不是实际的后端API地址

**影响**:
- 前端无法连接到后端API
- API请求会失败
- 应用功能无法正常工作

**解决方案**: 
需要替换为实际的后端API地址，例如：
- Railway部署: `https://your-app.railway.app`
- Fly.io部署: `https://your-app.fly.dev`
- Render部署: `https://your-app.onrender.com`
- 或其他实际的后端服务器地址

---

## ✅ 正确的配置

### 推荐配置（根据实际部署情况）

#### 如果后端部署在Railway:
```
VITE_API_BASE_URL=https://your-app-name.railway.app
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

#### 如果后端部署在Fly.io:
```
VITE_API_BASE_URL=https://your-app-name.fly.dev
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

#### 如果后端部署在Render:
```
VITE_API_BASE_URL=https://your-app-name.onrender.com
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

#### 如果后端部署在其他服务器:
```
VITE_API_BASE_URL=https://api.yourdomain.com
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

---

## 🔍 配置验证

### 1. VITE_API_BASE_URL 验证

**检查项**:
- [ ] URL格式正确（以 `http://` 或 `https://` 开头）
- [ ] 不包含占位符文本（如 `your-api-domain.com`）
- [ ] 后端服务已部署并可访问
- [ ] 后端CORS已配置允许前端域名

**验证方法**:
```bash
# 测试后端API是否可访问
curl https://your-api-domain.com/health
# 或
curl https://your-api-domain.com/swagger
```

### 2. VITE_APP_NAME 验证

**检查项**:
- [ ] 应用名称正确
- [ ] 不包含特殊字符
- [ ] 与项目名称一致

**当前值**: `UnifiedWeb3Platform` ✅ 正确

### 3. NODE_VERSION 验证

**检查项**:
- [ ] 版本号正确（20）
- [ ] 与本地开发环境一致
- [ ] 支持项目依赖

**当前值**: `20` ✅ 正确

---

## 📝 配置检查清单

### 必须配置
- [x] `VITE_API_BASE_URL` - ⚠️ 需要更新为实际地址
- [x] `VITE_APP_NAME` - ✅ 已正确配置
- [x] `NODE_VERSION` - ✅ 已正确配置

### 可选配置
- [ ] `VITE_API_TIMEOUT` - API请求超时时间（可选）
- [ ] `VITE_ENABLE_ANALYTICS` - 是否启用分析（可选）

---

## 🔧 修正步骤

### 步骤1：确定后端API地址

1. **如果后端已部署**:
   - 查看部署平台的URL
   - 例如：Railway会提供 `https://xxx.railway.app`
   - 复制完整的URL（包含 `https://`）

2. **如果后端未部署**:
   - 先部署后端服务
   - 获得部署URL后再配置

### 步骤2：更新环境变量

1. 在Cloudflare Pages配置页面
2. 找到"环境变量"部分
3. 编辑 `VITE_API_BASE_URL`
4. 将 `https://your-api-domain.com` 替换为实际后端地址
5. 保存配置

### 步骤3：重新部署

- 环境变量更新后，需要重新触发部署
- 或等待下次代码推送自动部署

---

## 🧪 部署后验证

### 1. 检查前端是否正常加载
- 访问Cloudflare Pages部署URL
- 检查页面是否正常显示

### 2. 检查API连接
- 打开浏览器开发者工具（F12）
- 切换到"Network"标签
- 查看API请求是否成功
- 检查是否有CORS错误

### 3. 检查控制台日志
- 打开浏览器控制台（Console）
- 应该看到：`API Base URL from .env: https://your-actual-api-url`
- 不应该看到连接错误

---

## ⚠️ 常见错误

### 错误1：API地址未更新
**症状**: 前端无法连接后端
**解决**: 更新 `VITE_API_BASE_URL` 为实际地址

### 错误2：CORS错误
**症状**: 浏览器控制台显示CORS错误
**解决**: 在后端配置CORS，允许前端域名

### 错误3：HTTPS/HTTP混合
**症状**: 混合内容错误
**解决**: 确保API地址使用HTTPS

---

## 📚 相关文档

- 后端CORS配置: `src/Backend/UnifiedPlatform.WebApi/Program.cs`
- 前端API配置: `src/Frontend/web-app/src/libs/WebApi.ts`
- 环境变量说明: `src/Frontend/web-app/.env.production`

---

## 🎯 总结

### 需要修正
- ⚠️ `VITE_API_BASE_URL`: 需要更新为实际后端API地址

### 配置正确
- ✅ `VITE_APP_NAME`: UnifiedWeb3Platform
- ✅ `NODE_VERSION`: 20

**重要**: 在部署后端服务并获得实际URL后，必须更新 `VITE_API_BASE_URL`，否则前端无法正常工作。

