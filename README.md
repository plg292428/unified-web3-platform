# UnifiedWeb3Platform 统一Web3平台

## 📋 项目概述

UnifiedWeb3Platform是一个统一的Web3全栈平台，整合了以下项目：

- **HFastKit**: .NET 8工具库
- **Nblockchain**: TRON区块链SDK
- **SmallTarget**: Vue 3企业应用框架
- **PolygonDapp**: Polygon区块链DApp

## 🏗️ 项目结构

```
UnifiedWeb3Platform/
├── src/
│   ├── Frontend/
│   │   └── web-app/              # Vue 3前端应用
│   ├── Backend/
│   │   ├── UnifiedPlatform.WebApi/    # Web API服务
│   │   ├── UnifiedPlatform.DbService/ # 数据库服务
│   │   └── UnifiedPlatform.Shared/    # 共享库
│   └── Libraries/
│       ├── HFastKit/             # .NET 8工具库
│       └── Nblockchain/          # TRON区块链SDK
```

## 🚀 快速开始

### 环境要求

- .NET 8 SDK
- Node.js 18+ (npm)
- SQL Server LocalDB 或 SQL Server Express
- Visual Studio 2022 或 VS Code（推荐）

### 安装步骤

1. **克隆或下载项目**
   ```bash
   cd "D:\claude code\plg\UnifiedWeb3Platform"
   ```

2. **恢复后端依赖**
   ```bash
   cd src\Backend
   dotnet restore
   ```

3. **安装前端依赖**
   ```bash
   cd ..\Frontend\web-app
   npm install
   ```

### 运行项目

#### 启动后端服务

```bash
# 使用脚本
.\run_backend.bat

# 或手动运行
cd src\Backend\UnifiedPlatform.WebApi
dotnet run
```

**访问地址：**
- API: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`
- 健康检查: `https://localhost:5001/health`

#### 启动前端服务

```bash
# 使用脚本
.\run_frontend.bat

# 或手动运行
cd src\Frontend\web-app
npm run dev
```

**访问地址：**
- 前端应用: `http://localhost:5173`

### 数据库配置

1. **检查数据库连接**
   ```bash
   .\check_database.bat
   ```

2. **更新连接字符串**
   编辑 `src\Backend\UnifiedPlatform.WebApi\appsettings.json`：
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UnifiedPlatform;..."
     }
   }
   ```

3. **运行数据库迁移**（如果需要）
   ```bash
   cd src\Backend\UnifiedPlatform.DbService
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## 📝 项目配置

### 后端配置

- **JWT设置**: `appsettings.json` 中的 `JwtSettings`
- **数据库连接**: `ConnectionStrings:DefaultConnection`
- **TRON设置**: `TronSettings`（可选）

### 前端配置

- **API地址**: `public/serverConfig.json`
  ```json
  {
    "developmentBaseUrl": "http://localhost:5195",
    "productionBaseUrl": "https://api.layer2farming.ai"
  }
  ```

## 🛠️ 开发工具

### 可用脚本

- `run_backend.bat` - 启动后端服务
- `run_frontend.bat` - 启动前端服务
- `check_database.bat` - 检查数据库连接
- `test_api.bat` - 测试API端点
- `update_namespaces.ps1` - 批量更新命名空间（可选）

### 编译项目

```bash
cd src\Backend
dotnet build
```

## ⚠️ 注意事项

### 命名空间

当前项目代码仍使用 `SmallTarget.*` 命名空间，项目可以正常编译和运行。如果需要统一命名空间，可以使用：

```powershell
# 试运行
.\update_namespaces.ps1 -DryRun

# 实际更新
.\update_namespaces.ps1
```

### IP区域服务

`ip2region.xdb` 文件是可选的。如果文件不存在，IP区域查询功能会被跳过。

### 数据库

首次运行需要：
1. 确保SQL Server LocalDB已安装
2. 创建数据库或运行迁移
3. 更新连接字符串（如果需要）

## 📚 相关文档

- `step2_summary.md` - 步骤2完成报告
- `step3_summary.md` - 步骤3完成报告
- `step4_summary.md` - 步骤4完成报告
- `UnifiedWeb3Platform项目目录结构创建操作流程.docx` - 详细操作流程

## 🔧 技术栈

### 后端
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- HFastKit（工具库）
- Nblockchain（TRON SDK）

### 前端
- Vue 3
- Vite
- TypeScript
- Vuetify 3
- Pinia

## 📄 许可证

本项目整合了多个开源项目，请遵守各项目的许可证要求。

## 🤝 贡献

欢迎提交Issue和Pull Request。

