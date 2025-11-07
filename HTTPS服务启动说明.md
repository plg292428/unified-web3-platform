# HTTPS 服务启动说明

## 配置状态

✅ **HTTPS 配置已完成**:
- ✅ 开发证书已安装并信任
- ✅ launchSettings.json 已配置 HTTPS profile
- ✅ Program.cs 已启用 HTTPS 重定向

## 启动 HTTPS 服务

### 方法 1: 使用批处理脚本（推荐）

```batch
.\使用HTTPS启动服务.bat
```

### 方法 2: 使用命令行

```bash
cd src\Backend\UnifiedPlatform.WebApi
dotnet run --launch-profile https
```

### 方法 3: 在 Visual Studio 中

1. 选择启动配置为 "https"
2. 运行项目

## 验证 HTTPS 是否启动

### 检查端口监听

```bash
netstat -ano | findstr ":5196"
```

如果看到端口 5196 在监听，说明 HTTPS 已启动。

### 测试 HTTPS 连接

在浏览器中访问:
```
https://localhost:5196/swagger
```

或使用 PowerShell:
```powershell
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
Invoke-WebRequest -Uri "https://localhost:5196/health"
```

## 常见问题

### 问题 1: HTTPS 端口未启动

**可能原因**:
- 服务未使用 `--launch-profile https` 参数启动
- 服务启动失败
- 端口被占用

**解决方案**:
1. 确保使用正确的启动命令
2. 检查服务启动日志
3. 检查端口是否被占用

### 问题 2: 浏览器显示证书警告

**这是正常的**，因为开发证书是自签名的。解决方法:
1. 点击"高级"或"Advanced"
2. 点击"继续访问"或"Proceed to localhost"

### 问题 3: 无法连接到 HTTPS

**检查项**:
1. 服务是否正在运行
2. 是否正确使用 HTTPS profile 启动
3. 防火墙是否阻止了端口

## 服务启动后的访问地址

### HTTP
- API: `http://localhost:5195`
- Swagger: `http://localhost:5195/swagger`
- Health: `http://localhost:5195/health`

### HTTPS
- API: `https://localhost:5196`
- Swagger: `https://localhost:5196/swagger`
- Health: `https://localhost:5196/health`

## 注意事项

1. **必须使用 HTTPS profile**: 使用 `--launch-profile https` 参数才能启动 HTTPS
2. **证书警告**: 开发证书是自签名的，浏览器会显示警告
3. **同时支持**: 服务会同时监听 HTTP 和 HTTPS 端口


