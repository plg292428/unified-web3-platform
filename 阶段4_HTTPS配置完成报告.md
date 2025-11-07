# 阶段4: HTTPS 配置完成报告

## 配置完成时间
2025-11-05

## ✅ 已完成的配置

### 1. 开发证书安装和信任

**状态**: ✅ 已完成

**执行的操作**:
```bash
dotnet dev-certs https --trust
```

**结果**:
- ✅ 开发证书已成功安装
- ✅ 证书已添加到受信任的根证书颁发机构
- ✅ 证书有效期: 2025-11-06 至 2026-11-06

### 2. launchSettings.json 配置更新

**状态**: ✅ 已完成

**更新内容**:
- ✅ 添加 HTTPS 配置 profile
- ✅ 配置 HTTPS 端口: `5196`
- ✅ 同时支持 HTTP (5195) 和 HTTPS (5196)

**配置详情**:
```json
{
  "https": {
    "commandName": "Project",
    "dotnetRunMessages": true,
    "launchBrowser": false,
    "launchUrl": "swagger",
    "applicationUrl": "http://localhost:5195;https://localhost:5196",
    "environmentVariables": {
      "ASPNETCORE_ENVIRONMENT": "Development"
    }
  }
}
```

### 3. Program.cs HTTPS 重定向配置

**状态**: ✅ 已完成

**更新内容**:
- ✅ 启用 HTTPS 重定向
- ✅ 开发和生产环境都支持 HTTPS

**配置代码**:
```csharp
// 启用HTTPS重定向（开发和生产环境都支持）
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    // 开发环境也启用 HTTPS（证书已信任）
    app.UseHttpsRedirection();
}
```

## 端口配置

### 开发环境
- **HTTP**: `http://localhost:5195`
- **HTTPS**: `https://localhost:5196`

### 访问地址
- **HTTP Swagger UI**: `http://localhost:5195/swagger`
- **HTTPS Swagger UI**: `https://localhost:5196/swagger`
- **HTTP Health Check**: `http://localhost:5195/health`
- **HTTPS Health Check**: `https://localhost:5196/health`

## 验证方法

### 方法 1: 检查证书状态
```bash
dotnet dev-certs https --check --trust
```

### 方法 2: 测试 HTTPS 连接
```bash
curl -k https://localhost:5196/health
```

或在浏览器中访问:
```
https://localhost:5196/swagger
```

### 方法 3: 检查端口监听
```bash
netstat -ano | findstr ":5196"
```

## 生产环境 HTTPS 配置

### 开发环境（当前）
- ✅ 使用开发证书
- ✅ 证书已信任
- ✅ 自动配置

### 生产环境（需要配置）

#### 选项 1: 使用 Kestrel 配置证书

在 `appsettings.Production.json` 中添加:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://0.0.0.0:5001",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "certificate-password"
        }
      }
    }
  }
}
```

#### 选项 2: 使用环境变量
```bash
export ASPNETCORE_Kestrel__Certificates__Default__Path="/path/to/certificate.pfx"
export ASPNETCORE_Kestrel__Certificates__Default__Password="password"
```

#### 选项 3: 使用反向代理（推荐）
- 使用 Nginx、IIS 或 Apache 作为反向代理
- 在反向代理层面处理 HTTPS
- 应用内部使用 HTTP

## 测试步骤

### 1. 重启服务（使用 HTTPS profile）

```bash
cd src/Backend/UnifiedPlatform.WebApi
dotnet run --launch-profile https
```

### 2. 验证 HTTPS 连接

```bash
# 测试健康检查
curl -k https://localhost:5196/health

# 或在浏览器中访问
https://localhost:5196/swagger
```

### 3. 验证 HTTPS 重定向

```bash
# HTTP 请求应该重定向到 HTTPS
curl -L http://localhost:5195/health
```

## 常见问题

### 问题 1: 浏览器显示"不安全连接"
**解决方案**: 
- 开发环境: 这是正常的，开发证书是自签名的
- 生产环境: 需要使用有效的 SSL 证书

### 问题 2: 证书绑定失败
**解决方案**:
- 检查端口是否被占用
- 确保证书路径和密码正确
- 检查证书是否有效

### 问题 3: HTTPS 重定向循环
**解决方案**:
- 检查反向代理配置
- 确保 X-Forwarded-Proto 头正确设置

## 安全建议

### 开发环境
- ✅ 使用开发证书（已配置）
- ✅ 仅用于本地开发

### 生产环境
- ⚠️ 必须使用有效的 SSL 证书
- ⚠️ 建议使用反向代理处理 HTTPS
- ⚠️ 配置 HSTS（HTTP Strict Transport Security）
- ⚠️ 定期更新证书

## 完成状态

✅ **阶段4.2: HTTPS 配置 - 开发环境已完成**

- ✅ 开发证书已安装并信任
- ✅ launchSettings.json 已配置 HTTPS
- ✅ Program.cs 已启用 HTTPS 重定向
- ✅ 同时支持 HTTP 和 HTTPS

**生产环境**: 需要根据实际部署环境配置 SSL 证书

## 下一步操作

1. **测试 HTTPS 连接**: 重启服务并测试 HTTPS 端点
2. **配置生产环境 HTTPS**: 根据部署环境配置 SSL 证书
3. **优化安全头**: 配置 HSTS、CSP 等安全响应头（可选）


