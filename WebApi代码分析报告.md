# WebApi 代码分析报告

## 一、代码结构分析

### 1. 设计模式
- **单例模式**: 使用静态 `instance` 确保全局只有一个 WebApi 实例
- **工厂方法**: `getInstance()` 方法创建/返回实例

### 2. 核心功能
- HTTP 请求封装（GET、POST、PUT、DELETE）
- 请求/响应拦截器
- 自动 Token 管理
- 错误处理

---

## 二、问题分析

### 🔴 问题 1: 初始化失败处理不当

**位置**: `initialize()` 方法（第 65-89 行）

**问题**:
```typescript
public async initialize(): Promise<void> {
  try {
    // ... 初始化逻辑
    if (envApiUrl) {
      // ... 设置 baseURL
      return
    }
    // 如果没有环境变量，从 serverConfig.json 读取
    const response = await this.axiosInstance.get('/serverConfig.json')
    // ...
    this.ready = true
  } catch (error) {
    console.error(error)  // ❌ 只打印错误，但不设置 ready
    // ready 仍然是 false，但没有任何提示
  }
}
```

**影响**:
- 如果初始化失败（后端服务未运行），`ready` 仍然是 `false`
- 后续所有 API 调用都会返回 `Promise.reject(new Error('Instance not initialized'))`
- 用户看不到明确的错误提示

---

### 🔴 问题 2: 错误处理返回默认值

**位置**: 所有 HTTP 方法（get、post、put、delete）

**问题**:
```typescript
.catch((error) => {
  console.log(error)
  if (error.response && error.response.data) {
    // 处理有响应的错误
  }
  resolve(WebApi.baseResponseData)  // ❌ 总是返回默认错误
})
```

**影响**:
- 网络错误（如 `ERR_CONNECTION_REFUSED`）时，总是返回相同的默认错误
- 错误信息不够详细，难以调试
- 无法区分不同类型的错误（网络错误、超时、服务器错误等）

---

### 🟡 问题 3: 初始化时的 baseURL 问题

**位置**: `initialize()` 方法（第 78 行）

**问题**:
```typescript
// 如果没有环境变量，则从serverConfig.json读取
const response = await this.axiosInstance.get('/serverConfig.json')
```

**影响**:
- 如果后端服务未运行，这个请求会失败
- 此时 `axiosInstance` 还没有设置 `baseURL`，会使用相对路径
- 如果前端运行在 `http://localhost:5173`，请求会发送到 `http://localhost:5173/serverConfig.json`
- 如果这个文件不存在，初始化会失败

---

### 🟡 问题 4: 环境变量检查

**位置**: `initialize()` 方法（第 68 行）

**问题**:
```typescript
const envApiUrl = import.meta.env.VITE_API_BASE_URL
if (envApiUrl) {
  // ...
}
```

**影响**:
- 如果环境变量未设置，会尝试从 `serverConfig.json` 读取
- 但 `serverConfig.json` 需要后端服务运行才能访问（如果放在后端）
- 或者需要放在前端 `public` 目录下

---

## 三、当前错误原因

### 错误信息
```
GET http://localhost:5000/DappCommon/GetChainNetworkConfigs 
net::ERR_CONNECTION_REFUSED
```

### 根本原因
1. **后端服务未运行**: 端口 5000 没有服务监听
2. **初始化可能失败**: 如果 `serverConfig.json` 无法访问，初始化会失败
3. **错误处理不当**: 即使初始化失败，应用仍然尝试调用 API

---

## 四、改进建议

### ✅ 建议 1: 改进初始化错误处理

```typescript
public async initialize(): Promise<void> {
  try {
    // 优先使用环境变量配置
    const envApiUrl = import.meta.env.VITE_API_BASE_URL
    if (envApiUrl) {
      this.axiosInstance.defaults.baseURL = envApiUrl
      this.baseUrl = envApiUrl
      this.ready = true
      console.log('API Base URL from .env:', envApiUrl)
      return
    }

    // 如果没有环境变量，尝试从 serverConfig.json 读取
    // 注意：serverConfig.json 应该在 public 目录下
    const response = await this.axiosInstance.get('/serverConfig.json')
    if (process.env.NODE_ENV === 'production') {
      this.axiosInstance.defaults.baseURL = response.data.productionBaseUrl
    } else {
      this.axiosInstance.defaults.baseURL = response.data.developmentBaseUrl
    }
    this.baseUrl = this.axiosInstance.defaults.baseURL
    this.ready = true
    console.log('API Base URL from serverConfig.json:', this.baseUrl)
  } catch (error) {
    console.error('WebApi 初始化失败:', error)
    // 设置默认值（开发环境）
    if (process.env.NODE_ENV === 'development') {
      this.axiosInstance.defaults.baseURL = 'http://localhost:5000'
      this.baseUrl = 'http://localhost:5000'
      this.ready = true
      console.warn('使用默认开发环境 API 地址: http://localhost:5000')
    } else {
      // 生产环境初始化失败，抛出错误
      throw new Error('无法初始化 API 配置，请检查环境变量或 serverConfig.json')
    }
  }
}
```

---

### ✅ 建议 2: 改进错误处理

```typescript
.catch((error) => {
  console.error('API 请求失败:', error)
  
  // 网络错误
  if (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED') {
    resolve({
      statusCode: 0,
      succeed: false,
      errorMessage: '无法连接到服务器，请检查后端服务是否运行',
      data: null
    })
    return
  }
  
  // 超时错误
  if (error.code === 'ECONNABORTED') {
    resolve({
      statusCode: 0,
      succeed: false,
      errorMessage: '请求超时，请稍后重试',
      data: null
    })
    return
  }
  
  // 服务器响应错误
  if (error.response && error.response.data) {
    const response = error.response
    const result = response.data
    resolve({
      statusCode: response.status,
      data: result.data,
      succeed: result.succeed,
      errorMessage: result.errorMessage || `服务器错误 (${response.status})`
    })
    return
  }
  
  // 其他错误
  resolve({
    statusCode: 0,
    succeed: false,
    errorMessage: error.message || '未知错误',
    data: null
  })
})
```

---

### ✅ 建议 3: 添加重试机制

```typescript
private async requestWithRetry<T>(
  requestFn: () => Promise<T>,
  retries: number = 3,
  delay: number = 1000
): Promise<T> {
  try {
    return await requestFn()
  } catch (error) {
    if (retries > 0 && (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED')) {
      await new Promise(resolve => setTimeout(resolve, delay))
      return this.requestWithRetry(requestFn, retries - 1, delay * 2)
    }
    throw error
  }
}
```

---

### ✅ 建议 4: 添加健康检查

```typescript
public async healthCheck(): Promise<boolean> {
  try {
    const response = await this.axiosInstance.get('/api/health', { timeout: 5000 })
    return response.status === 200
  } catch (error) {
    console.error('后端服务健康检查失败:', error)
    return false
  }
}
```

---

## 五、立即修复方案

### 方案 1: 创建 .env 文件（推荐）

在 `src/Frontend/web-app/` 目录下创建 `.env` 文件：

```env
VITE_API_BASE_URL=http://localhost:5000
```

这样初始化时会直接使用环境变量，不需要读取 `serverConfig.json`。

---

### 方案 2: 确保 serverConfig.json 存在

检查 `public/serverConfig.json` 文件是否存在，内容如下：

```json
{
  "developmentBaseUrl": "http://localhost:5000",
  "productionBaseUrl": "https://api.yourdomain.com"
}
```

---

### 方案 3: 启动后端服务

这是最根本的解决方案：

```cmd
cd src\Backend\UnifiedPlatform.WebApi
dotnet run
```

或使用批处理脚本：

```cmd
.\启动所有服务.bat
```

---

## 六、测试建议

### 1. 测试初始化
```typescript
const webApi = WebApi.getInstance()
await webApi.initialize()
console.log('Base URL:', webApi.baseUrl)
console.log('Ready:', webApi.ready)
```

### 2. 测试 API 调用
```typescript
const result = await webApi.get('/api/store/categories')
console.log('API 响应:', result)
```

### 3. 测试错误处理
- 停止后端服务，测试错误处理
- 测试超时情况
- 测试网络错误

---

## 七、总结

### 当前问题
1. ✅ 后端服务未运行（主要问题）
2. ⚠️ 初始化错误处理不当
3. ⚠️ 错误信息不够详细

### 优先级
1. **高**: 启动后端服务
2. **中**: 创建 `.env` 文件或确保 `serverConfig.json` 存在
3. **低**: 改进错误处理和初始化逻辑

---

**报告生成时间**: 2025-11-12

