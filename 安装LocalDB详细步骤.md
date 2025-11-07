# 安装SQL Server LocalDB详细步骤

## 🎯 安装方法选择

### 方法1：通过Visual Studio Installer安装（推荐，最简单）

如果您已经安装了Visual Studio，这是最简单的方法。

#### 步骤1：打开Visual Studio Installer

1. **按 `Win + R`** 打开运行对话框
2. **输入**：`appwiz.cpl`
3. **按回车**，打开"程序和功能"
4. **找到** "Microsoft Visual Studio" 相关程序
5. **右键点击** → **更改**

或者：

1. **在开始菜单搜索** "Visual Studio Installer"
2. **点击打开**

#### 步骤2：修改Visual Studio安装

1. **在Visual Studio Installer中**，找到您的Visual Studio版本（如：Visual Studio 2022）
2. **点击"修改"按钮**

#### 步骤3：选择LocalDB组件

1. **切换到"单个组件"标签**
2. **在搜索框中输入**：`LocalDB`
3. **找到并勾选**：
   - ✅ **SQL Server Express LocalDB**
   - 或者：**SQL Server 2019 Express LocalDB**（根据版本）

#### 步骤4：完成安装

1. **点击右下角的"修改"按钮**
2. **等待安装完成**（可能需要几分钟）
3. **安装完成后，关闭Visual Studio Installer**

---

### 方法2：独立安装SQL Server Express（如果没有Visual Studio）

#### 步骤1：下载SQL Server Express

1. **访问微软官网**：
   - 中文：https://www.microsoft.com/zh-cn/sql-server/sql-server-downloads
   - 英文：https://www.microsoft.com/sql-server/sql-server-downloads

2. **选择"Express"版本**
3. **点击"立即下载"**

#### 步骤2：运行安装程序

1. **运行下载的安装程序**（通常是 `SQLEXPR_x64_CHS.exe` 或类似名称）
2. **选择安装类型**：选择"基本"或"自定义"
3. **如果选择"自定义"**：
   - 在"功能选择"页面
   - **勾选"LocalDB"**
   - 取消其他不需要的功能（如SQL Server引擎、管理工具等）

#### 步骤3：完成安装

1. **按照安装向导完成安装**
2. **安装完成后，重启计算机**（推荐）

---

## ✅ 验证安装

安装完成后，运行以下命令验证：

```powershell
sqllocaldb info
```

如果显示LocalDB实例列表（如 `MSSQLLocalDB`），说明安装成功。

或者运行：

```powershell
sqllocaldb info MSSQLLocalDB
```

如果显示实例信息，说明安装成功。

---

## 🚀 安装后操作

### 1. 启动LocalDB

```powershell
sqllocaldb start MSSQLLocalDB
```

### 2. 验证连接

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION"
```

如果显示SQL Server版本信息，说明LocalDB运行正常。

### 3. 创建SmallTarget数据库

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE SmallTarget"
```

### 4. 验证数据库

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'"
```

如果显示 `SmallTarget`，说明数据库创建成功。

---

## 🔧 自动验证脚本

安装完成后，运行以下脚本自动验证：

```powershell
cd "D:\claude code\plg\UnifiedWeb3Platform"
.\fix_database_connection.bat
```

这个脚本会：
- ✅ 检查LocalDB是否安装
- ✅ 启动LocalDB（如果未运行）
- ✅ 检查SmallTarget数据库是否存在
- ✅ 创建数据库（如果不存在）
- ✅ 测试数据库连接

---

## ❓ 常见问题

### Q1: 安装后找不到 `sqllocaldb` 命令？

**A**: 需要将SQL Server的bin目录添加到PATH环境变量，或者重启计算机。

### Q2: 如何知道LocalDB是否已安装？

**A**: 运行 `sqllocaldb info`，如果显示实例列表，说明已安装。

### Q3: 安装后无法启动LocalDB？

**A**: 尝试以管理员身份运行命令，或者重启计算机。

### Q4: 如何卸载LocalDB？

**A**: 通过"程序和功能"卸载"Microsoft SQL Server"相关程序。

---

## 📝 下一步

安装完成并验证后：

1. **运行 `fix_database_connection.bat`** 创建数据库
2. **运行 `debug_run.bat`** 启动服务
3. **访问 Swagger UI**: https://localhost:7266/swagger

