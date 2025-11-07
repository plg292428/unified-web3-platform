# Cloudflare 部署分步计划

## 📋 项目准备状态

### ✅ 已就绪的内容
- ✅ 域名已托管到Cloudflare：`a292428dsj.dpdns.org`
- ✅ DNS记录已配置：
  - A记录：根域名 → `47.96.123.45`
  - A记录：www → `47.96.123.45`
  - CNAME记录：api → 根域名
- ✅ 前端项目完整：Vue 3 + Vite + Vuetify
- ✅ 构建配置已优化：`vite.config.ts`
- ✅ 依赖已安装：`node_modules`

### ⏳ 待完成的内容
- ⏳ 后端部署：Railway/Fly.io（可后续）
- ⏳ API地址配置：待后端部署后更新

---

## 🎯 分步部署计划

### 第一阶段：部署前端到Cloudflare Pages（推荐先做）

#### 步骤1：测试本地构建
```bash
cd src/Frontend/web-app
npm run build
```

**验证**：
- 检查 `dist` 文件夹是否生成
- 检查 `dist/index.html` 是否存在
- 检查构建是否有错误

#### 步骤2：准备Git仓库
如果还没有Git仓库：
```bash
git init
git add .
git commit -m "Initial commit"
git remote add origin <your-repo-url>
git push -u origin main
```

#### 步骤3：在Cloudflare创建Pages项目
1. 访问 https://dash.cloudflare.com/
2. 进入 Workers & Pages → Pages
3. 点击 "Create application"
4. 选择 "Connect to Git"

#### 步骤4：配置构建设置
```
框架预设: Vue
构建命令: npm run build
构建输出目录: dist
根目录: src/Frontend/web-app
Node版本: 20
```

#### 步骤5：配置环境变量（临时）
```
VITE_API_BASE_URL=https://api.a292428dsj.dpdns.org
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

**注意**：API地址暂时设置为域名，后端部署后无需更改前端配置。

#### 步骤6：配置自定义域名
1. 在Pages项目设置中
2. Custom domains → Add custom domain
3. 添加：`a292428dsj.dpdns.org` 或 `www.a292428dsj.dpdns.org`
4. Cloudflare会自动配置DNS

---

### 第二阶段：配置DNS（如需要）

#### 当前DNS配置
- ✅ 根域名：已指向 `47.96.123.45`
- ✅ www：已指向 `47.96.123.45`
- ✅ api：已配置CNAME

#### 如果需要调整
1. 如果前端部署到Pages，保持当前配置
2. 如果后端在 `47.96.123.45`，保持当前配置
3. 如果后端在Railway，更新api CNAME记录

---

### 第三阶段：部署后端（可后续）

#### 选项A：部署到Railway
1. 访问 https://railway.app/
2. 创建新项目，连接GitHub
3. Railway自动检测Dockerfile
4. 配置环境变量
5. 部署并获取URL

#### 选项B：使用现有服务器 `47.96.123.45`
如果后端已在此服务器运行：
1. 无需额外部署
2. 确保服务正常运行
3. 配置CORS允许前端域名

---

## 🚀 推荐执行顺序

### 优先级1：立即可做（前端部署）
1. ✅ 测试本地构建
   ```bash
   cd src/Frontend/web-app
   npm run build
   ```

2. ✅ 准备Git仓库（如果还没有）

3. ✅ 在Cloudflare Pages创建项目
   - 连接Git仓库
   - 配置构建设置
   - 配置环境变量

4. ✅ 配置自定义域名
   - 添加域名到Pages项目
   - Cloudflare自动配置DNS

5. ✅ 验证部署
   - 访问部署URL
   - 检查前端是否正常显示

### 优先级2：后续完成（后端部署）
1. ⏳ 部署后端到Railway/Fly.io
2. ⏳ 更新DNS记录（如果后端在Railway）
3. ⏳ 配置后端CORS
4. ⏳ 测试前后端连接

---

## ✅ 为什么可以先部署前端？

### 理由1：前端可以独立运行
- 前端是静态站点（HTML、CSS、JS）
- 不依赖后端即可部署
- 用户可以访问前端界面

### 理由2：API地址已预留
- 已配置API子域名：`api.a292428dsj.dpdns.org`
- 前端环境变量可以先设置此地址
- 后端部署后，前端无需修改

### 理由3：分步部署更安全
- 先验证前端部署是否正常
- 确认DNS配置是否正确
- 再部署后端，降低风险

### 理由4：便于测试和调试
- 前端部署后可以先测试界面
- 检查静态资源是否正常加载
- 验证CDN加速效果

---

## 📝 执行检查清单

### 前端部署检查
- [ ] 本地构建测试通过
- [ ] Git仓库已准备
- [ ] Cloudflare Pages项目已创建
- [ ] 构建设置已配置
- [ ] 环境变量已配置
- [ ] 自定义域名已配置
- [ ] 部署成功
- [ ] 前端可以正常访问

### 后端部署检查（后续）
- [ ] 后端服务已部署
- [ ] API URL已获取
- [ ] DNS记录已更新（如需要）
- [ ] CORS已配置
- [ ] 前后端连接测试通过

---

## 🎯 预期结果

### 第一阶段完成后
- ✅ 前端可通过 `https://a292428dsj.dpdns.org` 访问
- ✅ 通过Cloudflare CDN加速
- ✅ 自动HTTPS
- ⏳ API调用会失败（后端未部署）

### 第二阶段完成后
- ✅ 前端可以访问
- ✅ 后端API可以调用
- ✅ 前后端连接正常
- ✅ 完整功能可用

---

## 💡 建议

### 立即执行
1. 先部署前端到Cloudflare Pages
2. 验证前端是否正常显示
3. 检查DNS配置是否正确

### 后续执行
1. 部署后端到Railway
2. 配置后端CORS
3. 测试完整功能

这样的分步部署更加安全和可控。

