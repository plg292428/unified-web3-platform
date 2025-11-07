# 数据库配置指南

## 📋 数据库配置选项

### 选项1：使用LocalDB（推荐用于开发）

**适用场景：** 本地开发环境

**配置步骤：**

1. **检查LocalDB是否安装**
   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION"
   ```

2. **如果未安装，安装方法：**
   - 下载并安装 [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
   - 或通过 Visual Studio Installer 安装 LocalDB

3. **创建数据库**
   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE UnifiedPlatform"
   ```

4. **更新连接字符串**（`appsettings.json`）
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UnifiedPlatform;Integrated Security=True;TrustServerCertificate=True;"
     }
   }
   ```

### 选项2：使用SQL Server Express/Standard

**适用场景：** 生产环境或需要完整SQL Server功能

**配置步骤：**

1. **安装SQL Server**
   - 下载并安装 SQL Server Express/Standard

2. **创建数据库**
   ```sql
   CREATE DATABASE UnifiedPlatform;
   ```

3. **更新连接字符串**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=localhost;Initial Catalog=UnifiedPlatform;Integrated Security=True;TrustServerCertificate=True;"
     }
   }
   ```

### 选项3：使用现有SmallTarget数据库

**适用场景：** 已有SmallTarget数据库，想复用数据

**配置步骤：**

1. **更新连接字符串**（`appsettings.json`）
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SmallTarget;Integrated Security=True;TrustServerCertificate=True;"
     }
   }
   ```

2. **注意：** 使用现有数据库时，表结构应该已经存在，不需要运行迁移

## 🔧 数据库迁移

### 创建迁移

```powershell
# 方法1：使用脚本
.\create_migration.bat

# 方法2：手动执行
cd src\Backend\UnifiedPlatform.DbService
dotnet ef migrations add InitialCreate --startup-project ..\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj --context StDbContext
```

### 应用迁移

```powershell
# 方法1：使用脚本（包含在create_migration.bat中）

# 方法2：手动执行
cd src\Backend\UnifiedPlatform.DbService
dotnet ef database update --startup-project ..\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj --context StDbContext
```

### 查看迁移状态

```powershell
dotnet ef migrations list --startup-project ..\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj --context StDbContext
```

## 📝 数据库表结构

项目包含以下主要数据表：

- **用户相关**
  - Users（用户表）
  - UserAssets（用户资产）
  - UserChainTransaction（链上交易）
  - UserLoginLog（登录日志）
  - UserMiningRewardRecord（挖矿奖励）
  - UserInvitationRewardRecord（邀请奖励）

- **管理相关**
  - Managers（管理员）
  - ManagerLoginLog（管理员登录日志）
  - ManagerOperationLog（操作日志）
  - ManagerBalanceChange（余额变更）

- **配置相关**
  - GlobalConfig（全局配置）
  - ChainNetworkConfig（链网络配置）
  - ChainTokenConfig（代币配置）
  - ChainWalletConfig（钱包配置）
  - UserLevelConfig（用户等级配置）

- **业务相关**
  - UserAiTradingOrder（AI交易订单）
  - UserAssetsToWalletOrder（资产转钱包订单）

## ⚠️ 注意事项

1. **数据库名称**
   - 新项目默认使用 `UnifiedPlatform` 数据库
   - 如果使用现有 `SmallTarget` 数据库，需要修改连接字符串

2. **迁移脚本**
   - 首次运行需要创建迁移
   - 如果数据库已存在表结构，可能需要先删除迁移或手动处理

3. **连接字符串格式**
   - LocalDB: `Data Source=(localdb)\\MSSQLLocalDB;...`
   - SQL Server: `Data Source=localhost;...` 或 `Data Source=服务器名;...`
   - 包含用户名密码: `Data Source=...;User ID=用户名;Password=密码;...`

4. **TrustServerCertificate**
   - 开发环境建议添加 `TrustServerCertificate=True`
   - 生产环境应使用正式证书

## 🚀 快速开始

### 最简单的方式（使用现有SmallTarget数据库）

1. 修改 `appsettings.json`：
   ```json
   "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SmallTarget;Integrated Security=True;TrustServerCertificate=True;"
   ```

2. 直接运行项目，数据库连接将使用现有数据库

### 创建新数据库

1. 运行 `.\configure_database.bat` 创建数据库
2. 运行 `.\create_migration.bat` 创建并应用迁移
3. 运行项目

