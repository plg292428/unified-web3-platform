# 安装SQL Server LocalDB指南

## 🔍 问题原因

服务无法启动是因为：
1. **SQL Server LocalDB未安装**
2. **SmallTarget数据库不存在**
3. 服务启动时需要初始化数据库连接

## 🛠️ 解决方案

### 方案1：安装SQL Server LocalDB（推荐）

#### 方法A：通过Visual Studio安装（最简单）

1. **打开Visual Studio Installer**
2. **修改Visual Studio安装**
3. **选择"单个组件"标签**
4. **搜索"LocalDB"**
5. **勾选"SQL Server Express LocalDB"**
6. **点击"修改"安装**

#### 方法B：独立安装SQL Server Express

1. **下载SQL Server Express**
   - 访问：https://www.microsoft.com/sql-server/sql-server-downloads
   - 选择"Express"版本
   - 下载并运行安装程序

2. **安装时选择LocalDB**
   - 在功能选择中，勾选"LocalDB"
   - 完成安装

#### 方法C：使用Chocolatey安装（如果已安装Chocolatey）

```powershell
choco install sql-server-express -y
```

### 方案2：使用现有SQL Server实例

如果您已经有SQL Server实例：

1. **修改连接字符串**
   - 编辑 `appsettings.json`
   - 修改 `ConnectionStrings:DefaultConnection`
   - 指向您的SQL Server实例

2. **创建SmallTarget数据库**
   ```sql
   CREATE DATABASE SmallTarget;
   ```

### 方案3：临时测试（不推荐用于生产）

如果只是想测试服务能否启动：

1. **我已经修改了Program.cs**
   - 添加了try-catch保护
   - 如果数据库服务初始化失败，服务仍可启动
   - 但部分功能（Web3、缓存等）不可用

2. **运行服务**
   ```powershell
   .\debug_run.bat
   ```

3. **访问Swagger UI**
   - 至少可以查看API文档
   - 健康检查端点应该可用

## 📋 安装后验证步骤

### 1. 检查LocalDB是否安装

```powershell
sqllocaldb info
```

应该显示已安装的LocalDB实例列表。

### 2. 启动LocalDB

```powershell
sqllocaldb start MSSQLLocalDB
```

### 3. 检查SmallTarget数据库

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'"
```

### 4. 如果数据库不存在，创建它

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE SmallTarget"
```

### 5. 重新运行项目

```powershell
cd "D:\claude code\plg\UnifiedWeb3Platform\src\Backend\UnifiedPlatform.WebApi"
dotnet run
```

## 🎯 推荐操作流程

1. **安装LocalDB**（使用方案1的方法A或B）
2. **运行 `fix_database_connection.bat`** 检查并创建数据库
3. **运行 `debug_run.bat`** 查看详细启动日志
4. **访问 Swagger UI** 测试API

## ⚠️ 重要提示

- LocalDB是轻量级的SQL Server，适合开发环境
- 生产环境建议使用完整的SQL Server实例
- 如果无法安装LocalDB，可以使用其他SQL Server实例
- 修改连接字符串后，确保数据库存在

