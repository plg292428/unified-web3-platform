# 安装LocalDB操作指南

## ✅ 当前状态

**LocalDB未安装** - 需要安装后才能运行项目。

## 🎯 安装方法（推荐：Visual Studio Installer）

### 方法1：通过Visual Studio Installer安装（最简单）

#### 步骤1：打开Visual Studio Installer

**方式A：通过开始菜单**
1. 按 `Win` 键
2. 搜索 "Visual Studio Installer"
3. 点击打开

**方式B：通过运行命令**
1. 按 `Win + R`
2. 输入：`appwiz.cpl`
3. 在"程序和功能"中找到 "Microsoft Visual Studio"
4. 右键点击 → **更改**

**方式C：运行脚本**
- 双击运行：`打开Visual Studio Installer.bat`

#### 步骤2：修改Visual Studio安装

1. 在Visual Studio Installer中，找到您的Visual Studio版本（如：Visual Studio 2022）
2. **点击"修改"按钮**（右下角）

#### 步骤3：选择LocalDB组件

1. **切换到"单个组件"标签**（顶部）
2. **在搜索框中输入**：`LocalDB`
3. **找到并勾选**：
   - ✅ **SQL Server Express LocalDB**
   - 或：**SQL Server 2019 Express LocalDB**（根据您的Visual Studio版本）

#### 步骤4：完成安装

1. **点击右下角的"修改"按钮**开始安装
2. **等待安装完成**（可能需要5-10分钟）
3. **安装完成后，关闭Visual Studio Installer**
4. **重启计算机**（推荐，确保环境变量更新）

---

### 方法2：独立安装SQL Server Express（如果没有Visual Studio）

#### 步骤1：下载SQL Server Express

1. **访问微软官网**：
   - https://www.microsoft.com/sql-server/sql-server-downloads
   - 或中文：https://www.microsoft.com/zh-cn/sql-server/sql-server-downloads

2. **选择"Express"版本**
3. **点击"立即下载"**

#### 步骤2：运行安装程序

1. **运行下载的安装程序**
2. **选择安装类型**：选择"基本"或"自定义"
3. **如果选择"自定义"**：
   - 在"功能选择"页面
   - **勾选"LocalDB"**
   - 可以取消其他不需要的功能

#### 步骤3：完成安装

1. **按照安装向导完成安装**
2. **安装完成后，重启计算机**

---

## ✅ 安装后验证

安装完成后，运行以下命令验证：

```powershell
cd "D:\claude code\plg\UnifiedWeb3Platform"
powershell -ExecutionPolicy Bypass -File "check_localdb.ps1"
```

或者：

```powershell
sqllocaldb info
```

如果显示LocalDB实例列表，说明安装成功。

---

## 🚀 安装后的下一步

### 1. 验证安装

运行检查脚本：
```powershell
cd "D:\claude code\plg\UnifiedWeb3Platform"
powershell -ExecutionPolicy Bypass -File "check_localdb.ps1"
```

### 2. 创建SmallTarget数据库

如果验证通过，运行：
```powershell
.\fix_database_connection.bat
```

### 3. 启动服务

数据库创建成功后，运行：
```powershell
.\debug_run.bat
```

### 4. 访问Swagger UI

服务启动后，访问：
- Swagger UI: https://localhost:7266/swagger
- 健康检查: http://localhost:5195/health

---

## ❓ 常见问题

### Q1: 安装后仍找不到sqllocaldb命令？

**A**: 
1. **重启计算机**（确保环境变量更新）
2. 或手动添加PATH：
   - 通常路径：`C:\Program Files\Microsoft SQL Server\130\Tools\Binn\`（SQL Server 2016）
   - 或：`C:\Program Files\Microsoft SQL Server\150\Tools\Binn\`（SQL Server 2019）

### Q2: 如何知道LocalDB是否已安装？

**A**: 运行 `sqllocaldb info`，如果显示实例列表，说明已安装。

### Q3: 安装后无法启动LocalDB？

**A**: 
1. 以管理员身份运行命令
2. 或重启计算机

### Q4: 不想安装LocalDB，可以使用其他数据库吗？

**A**: 可以，修改 `appsettings.json` 中的连接字符串，指向您的SQL Server实例。

---

## 📝 安装时间估算

- **通过Visual Studio Installer**: 约5-10分钟
- **独立安装SQL Server Express**: 约10-20分钟

---

## 🎯 推荐操作流程

1. ✅ **打开Visual Studio Installer**（运行 `打开Visual Studio Installer.bat`）
2. ✅ **修改安装**，选择LocalDB组件
3. ✅ **等待安装完成**
4. ✅ **重启计算机**（推荐）
5. ✅ **运行 `check_localdb.ps1`** 验证安装
6. ✅ **运行 `fix_database_connection.bat`** 创建数据库
7. ✅ **运行 `debug_run.bat`** 启动服务

