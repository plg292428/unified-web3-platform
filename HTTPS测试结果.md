# HTTPS 测试结果

## 测试时间
2025-11-05

## 测试结果

### ✅ HTTP 连接测试
- **状态**: ✅ 成功
- **端口**: 5195
- **端点**: `/health`
- **响应**: HTTP 200 OK
- **内容**: `{"status":"healthy"}`

### ⚠️ HTTPS 连接测试
- **状态**: 需要验证
- **端口**: 5196
- **说明**: 服务可能正在启动中，或需要等待更长时间

## 访问地址

### HTTP
- **API**: `http://localhost:5195`
- **Swagger**: `http://localhost:5195/swagger`
- **Health Check**: `http://localhost:5195/health`

### HTTPS
- **API**: `https://localhost:5196`
- **Swagger**: `https://localhost:5196/swagger`
- **Health Check**: `https://localhost:5196/health`

## 验证方法

### 方法 1: 浏览器测试
直接在浏览器中访问：
```
https://localhost:5196/swagger
```

如果浏览器显示安全警告，点击"高级" → "继续访问"（开发证书是自签名的）

### 方法 2: curl 测试
```bash
curl -k https://localhost:5196/health
```

### 方法 3: PowerShell 测试
```powershell
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
Invoke-WebRequest -Uri "https://localhost:5196/health"
```

## 注意事项

1. **证书警告**: 开发证书是自签名的，浏览器会显示安全警告，这是正常的
2. **端口**: HTTPS 使用端口 5196，HTTP 使用端口 5195
3. **启动时间**: 服务可能需要几秒钟才能完全启动 HTTPS 端点

## 如果 HTTPS 无法访问

1. **检查端口是否监听**:
   ```bash
   netstat -ano | findstr ":5196"
   ```

2. **检查服务日志**: 查看服务启动时的错误信息

3. **重新启动服务**:
   ```bash
   cd src\Backend\UnifiedPlatform.WebApi
   dotnet run --launch-profile https
   ```

4. **检查证书**: 
   ```bash
   dotnet dev-certs https --check --trust
   ```


