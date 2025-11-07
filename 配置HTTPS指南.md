# 配置 HTTPS 指南

## 概述

HTTPS 是生产环境的标准配置，可以保护数据传输安全。

## 开发环境 HTTPS

### 安装开发证书

```bash
dotnet dev-certs https --trust
```

### 验证证书安装

```bash
dotnet dev-certs https --check --trust
```

### 清除并重新安装证书

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

## 生产环境 HTTPS

### 方法 1: 使用 Kestrel 配置 HTTPS

在 `appsettings.Production.json` 中：
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

### 方法 2: 在 Program.cs 中配置

```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        listenOptions.UseHttps("path/to/certificate.pfx", "password");
    });
});
```

### 方法 3: 使用环境变量

```bash
export ASPNETCORE_Kestrel__Certificates__Default__Path="/path/to/certificate.pfx"
export ASPNETCORE_Kestrel__Certificates__Default__Password="password"
```

## 获取 SSL 证书

### 选项 1: Let's Encrypt（免费）

```bash
# 使用 certbot
sudo apt-get install certbot
sudo certbot certonly --standalone -d yourdomain.com
```

### 选项 2: 商业证书

从证书颁发机构（CA）购买 SSL 证书

### 选项 3: 自签名证书（仅测试）

```bash
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes
```

## 启用 HTTPS 重定向

在 `Program.cs` 中取消注释：
```csharp
app.UseHttpsRedirection();
```

## 验证 HTTPS 配置

1. 启动服务
2. 访问 `https://localhost:5001`
3. 检查浏览器是否显示安全连接
4. 验证证书是否有效

## 常见问题

### 问题 1: 证书绑定失败
**解决方案**: 确保证书路径正确，密码正确

### 问题 2: 浏览器显示不安全
**解决方案**: 使用有效的 SSL 证书（非自签名）

### 问题 3: 端口被占用
**解决方案**: 更改端口配置或停止占用端口的进程


