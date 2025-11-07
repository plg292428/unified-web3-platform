# Cloudflare Pages 配置检查

## 📋 当前配置

根据截图显示的配置：

| 配置项 | 当前值 | 状态 |
|--------|--------|------|
| 框架预设 | Vue | ✅ 正确 |
| 构建命令 | `npm run build` | ✅ 正确 |
| 构建输出目录 | `/dist` | ⚠️ **需要修正** |
| 根目录 | `/src/Frontend/web-app` | ✅ 正确 |

---

## ⚠️ 发现的问题

### 问题：构建输出目录路径错误

**当前配置**: `/dist`  
**正确配置**: `dist`（不带前导斜杠）

### 原因分析

在Cloudflare Pages中：
- 如果设置了**根目录**为 `src/Frontend/web-app`
- 那么**构建输出目录**应该是**相对于根目录**的路径
- `vite.config.ts` 中配置的 `outDir: 'dist'` 是相对于项目根目录的
- 由于根目录已设置为 `src/Frontend/web-app`，构建会在该目录下创建 `dist` 文件夹
- 因此输出目录应该是 `dist`（相对于根目录），而不是 `/dist`

---

## ✅ 正确的配置

### 推荐配置1：使用根目录（当前方式）

```
框架预设: Vue
构建命令: npm run build
构建输出目录: dist          ← 修正：去掉前导斜杠
根目录: src/Frontend/web-app
```

**说明**:
- 根目录设置为 `src/Frontend/web-app`
- 构建输出目录设置为 `dist`（相对于根目录）
- 最终输出路径：`src/Frontend/web-app/dist`

### 推荐配置2：不使用根目录（备选）

```
框架预设: Vue
构建命令: npm run build
构建输出目录: src/Frontend/web-app/dist
根目录: （留空或设置为 /）
```

**说明**:
- 不设置根目录（或设置为 `/`）
- 构建输出目录设置为完整路径 `src/Frontend/web-app/dist`
- 这种方式更直观，但需要输入完整路径

---

## 🔧 修正步骤

### 方法1：修正构建输出目录（推荐）

1. 在Cloudflare Pages配置页面
2. 找到"构建输出目录"字段
3. 将 `/dist` 改为 `dist`（去掉前导斜杠）
4. 保存配置

### 方法2：验证路径

如果保留 `/dist`，Cloudflare可能会：
- 在仓库根目录查找 `dist` 文件夹
- 导致找不到构建输出，部署失败

---

## ✅ 完整正确配置

```
框架预设: Vue
构建命令: npm run build
构建输出目录: dist
根目录: src/Frontend/web-app
Node版本: 20（建议设置）
```

### 环境变量配置

在"环境变量"部分添加：

```
VITE_API_BASE_URL=https://your-api-domain.com
VITE_APP_NAME=UnifiedWeb3Platform
NODE_VERSION=20
```

---

## 🧪 验证方法

### 1. 本地构建测试

```bash
cd src/Frontend/web-app
npm run build
```

检查输出：
- 应该生成 `dist` 文件夹
- 包含 `index.html` 和 `assets` 文件夹

### 2. 检查构建输出结构

```
src/Frontend/web-app/
├── dist/              ← 构建输出目录
│   ├── index.html
│   └── assets/
│       ├── index-xxx.js
│       └── index-xxx.css
├── src/
├── package.json
└── vite.config.ts
```

### 3. Cloudflare Pages构建日志

部署后查看构建日志，应该看到：
```
✓ built in xxx ms
```

如果看到错误：
```
Error: Build output directory not found
```
说明路径配置错误。

---

## 📝 配置检查清单

- [x] 框架预设：Vue ✅
- [x] 构建命令：npm run build ✅
- [ ] 构建输出目录：`dist`（需要修正为不带斜杠）⚠️
- [x] 根目录：src/Frontend/web-app ✅
- [ ] 环境变量：需要配置 VITE_API_BASE_URL
- [ ] Node版本：建议设置为 20

---

## 🎯 总结

**需要修正**:
- 构建输出目录：`/dist` → `dist`（去掉前导斜杠）

**配置正确**:
- 框架预设：Vue ✅
- 构建命令：npm run build ✅
- 根目录：src/Frontend/web-app ✅

修正后即可正常部署！

