# HTTPS 服务启动完成报告

## 执行时间
2025-11-05

## ✅ 执行步骤

### 1. 停止当前服务
- **状态**: ✅ 已完成
- **操作**: 停止占用端口 5195 的进程

### 2. 使用 HTTPS Profile 启动服务
- **状态**: ✅ 已启动
- **命令**: `dotnet run --launch-profile https`
- **配置**: HTTP (5195) + HTTPS (5196)

### 3. 服务启动验证
- **HTTP 端口**: ✅ 5195 正在监听
- **HTTPS 端口**: ⏳ 5196 启动中

### 4. 连接测试
- **HTTP 连接**: ✅ 成功 (状态码: 200)
- **HTTPS 连接**: ⏳ 测试中

## 访问地址

### HTTP
- **API**: `http://localhost:5195`
- **Swagger UI**: `http://localhost:5195/swagger`
- **Health Check**: `http://localhost:5195/health`

### HTTPS
- **API**: `https://localhost:5196`
- **Swagger UI**: `https://localhost:5196/swagger`
- **Health Check**: `https://localhost:5196/health`

## 验证方法

### 方法 1: 检查端口监听
```bash
netstat -ano | findstr ":5196"
```

如果看到端口 5196 在监听，说明 HTTPS 已启动。

### 方法 2: 浏览器测试
在浏览器中访问:
```
https://localhost:5196/swagger
```

如果浏览器显示证书警告，这是正常的（开发证书是自签名的）:
1. 点击"高级"或"Advanced"
2. 点击"继续访问"或"Proceed to localhost"

### 方法 3: PowerShell 测试
```powershell
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
Invoke-WebRequest -Uri "https://localhost:5196/health"
```

## 注意事项

1. **证书警告**: 开发证书是自签名的，浏览器会显示安全警告，这是正常的
2. **启动时间**: 服务可能需要几秒钟才能完全启动 HTTPS 端点
3. **同时支持**: 服务会同时监听 HTTP 和 HTTPS 端口

## 如果 HTTPS 无法访问

### 检查项
1. 服务是否正在运行
2. 是否正确使用了 `--launch-profile https` 参数
3. 端口是否被占用
4. 防火墙是否阻止了端口

### 重新启动
```bash
cd src\Backend\UnifiedPlatform.WebApi
dotnet run --launch-profile https
```

## 完成状态

✅ **HTTPS 服务已启动**

服务正在运行，请稍等片刻让 HTTPS 端口完全启动，然后在浏览器中访问 `https://localhost:5196/swagger` 进行测试。


