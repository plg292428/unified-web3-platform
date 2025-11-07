# HTTPS 服务启动问题排查

## 当前状态

- ✅ **HTTP 服务**: 运行正常 (端口 5195)
- ❌ **HTTPS 服务**: 端口 5196 未启动

## 可能的原因

1. **后台启动可能未正确使用 HTTPS profile**
2. **服务启动时可能有错误**
3. **需要在前台查看启动日志**

## 解决方案

### 方法 1: 在前台运行服务查看日志（推荐）

在前台运行服务可以查看详细的启动日志和错误信息：

```bash
cd src\Backend\UnifiedPlatform.WebApi
dotnet run --launch-profile https
```

**查看启动日志中的关键信息**:
- 是否显示 "Now listening on: https://localhost:5196"
- 是否有任何错误信息
- 证书绑定是否成功

### 方法 2: 使用批处理脚本

运行批处理脚本，它会自动停止旧服务并启动 HTTPS 服务：

```batch
.\重启HTTPS服务.bat
```

### 方法 3: 检查配置

确保以下配置正确：

1. **launchSettings.json** 中的 https profile:
```json
"https": {
  "applicationUrl": "http://localhost:5195;https://localhost:5196"
}
```

2. **Program.cs** 中已启用 HTTPS 重定向:
```csharp
app.UseHttpsRedirection();
```

3. **开发证书已安装并信任**:
```bash
dotnet dev-certs https --check --trust
```

## 验证 HTTPS 是否启动

### 检查端口监听

```bash
netstat -ano | findstr ":5196"
```

如果看到端口 5196 在监听，说明 HTTPS 已启动。

### 测试 HTTPS 连接

在 PowerShell 中:
```powershell
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
Invoke-WebRequest -Uri "https://localhost:5196/health"
```

或在浏览器中访问:
```
https://localhost:5196/swagger
```

## 常见问题

### 问题 1: 端口 5196 未监听

**可能原因**:
- 服务未使用 HTTPS profile 启动
- 证书绑定失败
- 端口被占用

**解决方案**:
1. 确保使用 `--launch-profile https` 参数
2. 检查证书是否正确安装: `dotnet dev-certs https --check --trust`
3. 检查端口是否被占用: `netstat -ano | findstr ":5196"`

### 问题 2: 证书绑定失败

**解决方案**:
1. 重新安装证书: `dotnet dev-certs https --clean && dotnet dev-certs https --trust`
2. 检查是否有其他进程占用端口

### 问题 3: 服务启动失败

**解决方案**:
1. 在前台运行服务查看详细错误信息
2. 检查 Program.cs 中的配置
3. 检查 appsettings.json 中的配置

## 推荐的启动方式

### 在前台运行（查看日志）

```bash
cd src\Backend\UnifiedPlatform.WebApi
dotnet run --launch-profile https
```

这样可以看到完整的启动日志，包括：
- 服务启动信息
- 端口绑定信息
- 任何错误信息

### 正常启动后应该看到

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5195
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5196
```

如果看到这两行，说明 HTTPS 已成功启动。

## 下一步

1. **在前台运行服务**查看启动日志
2. **检查是否有错误信息**
3. **验证端口 5196 是否监听**
4. **测试 HTTPS 连接**


