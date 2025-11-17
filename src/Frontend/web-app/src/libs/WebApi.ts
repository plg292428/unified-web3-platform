import { ApiResponse } from '@/types'
import axios, { AxiosInstance } from 'axios'

// API 工具
export default class WebApi {
  private static instance: WebApi
  private static baseResponseData: ApiResponse = {
    statusCode: 500,
    succeed: false,
    errorMessage: 'net::ERR_CONNECTION_REFUSED',
    data: null
  }

  private axiosInstance: AxiosInstance
  private ready: boolean = false
  public baseUrl: string | undefined
  
  // 获取 ready 状态
  public get isReady(): boolean {
    return this.ready
  }

  //构造方法
  private constructor() {
    this.axiosInstance = axios.create({
      timeout: 20000
    })
    this.axiosInstance.defaults.headers.common['Authorization'] = 'Bearer None'
    this.axiosInstance.defaults.headers.post['Content-Type'] = 'application/json'

    // 添加请求拦截器
    this.axiosInstance.interceptors.request.use(
      (config) => {
        // 请求
        const accessToken = localStorage.getItem('accessToken')
        if (accessToken) {
          config.headers.Authorization = 'Bearer ' + accessToken
        }
        return config
      },
      (error) => {
        // 请求错误
        return Promise.reject(error)
      }
    )

    // 添加响应拦截器
    this.axiosInstance.interceptors.response.use(
      (response) => {
        // 响应数据
        return response
      },
      (error) => {
        // 对响应错误
        return Promise.reject(error)
      }
    )
  }

  // 获取实例
  public static getInstance(): WebApi {
    // 初始化本类实例
    if (!this.instance) {
      this.instance = new WebApi()
    }
    return this.instance
  }

  // 初始化
  public async initialize(): Promise<void> {
    try {
      console.error('[WebApi] 开始初始化...')
      console.error('[WebApi] 环境信息:', {
        PROD: import.meta.env.PROD,
        DEV: import.meta.env.DEV,
        MODE: import.meta.env.MODE,
        VITE_API_BASE_URL: import.meta.env.VITE_API_BASE_URL
      })
      
      // 优先使用环境变量配置
      const envApiUrl = import.meta.env.VITE_API_BASE_URL
      if (envApiUrl) {
        this.axiosInstance.defaults.baseURL = envApiUrl
        this.baseUrl = envApiUrl
        this.ready = true
        console.error('[WebApi] ✅ 使用环境变量配置:', envApiUrl)
        return
      }

      // 如果没有环境变量，则从 serverConfig.json 读取
      // 注意：serverConfig.json 在 public 目录下，使用相对路径
      console.error('[WebApi] 尝试加载 serverConfig.json...')
      const response = await fetch('/serverConfig.json')
      if (!response.ok) {
        throw new Error(`无法加载 serverConfig.json: ${response.status} ${response.statusText}`)
      }
      const config = await response.json()
      console.error('[WebApi] serverConfig.json 内容:', config)
      
      // 判断环境并选择对应的 API 地址
      const isProduction = import.meta.env.PROD
      const apiUrl = isProduction ? config.productionBaseUrl : config.developmentBaseUrl
      
      console.error('[WebApi] 环境判断:', {
        isProduction,
        selectedUrl: apiUrl,
        productionBaseUrl: config.productionBaseUrl,
        developmentBaseUrl: config.developmentBaseUrl
      })
      
      if (!apiUrl) {
        throw new Error(`serverConfig.json 中缺少 ${isProduction ? 'productionBaseUrl' : 'developmentBaseUrl'} 配置`)
      }
      
      this.axiosInstance.defaults.baseURL = apiUrl
      this.baseUrl = apiUrl
      this.ready = true
      console.error('[WebApi] ✅ 初始化完成，API Base URL:', this.baseUrl)
    } catch (error) {
      console.error('[WebApi] ❌ 初始化失败:', error)
      // 开发环境：设置默认值
      if (import.meta.env.DEV) {
        this.axiosInstance.defaults.baseURL = 'http://localhost:5000'
        this.baseUrl = 'http://localhost:5000'
        this.ready = true
        console.error('[WebApi] ⚠️ 使用默认开发环境 API 地址: http://localhost:5000')
        console.error('[WebApi] 提示: 请确保后端服务已启动')
      } else {
        // 生产环境初始化失败，尝试使用默认值
        console.error('[WebApi] ⚠️ 生产环境初始化失败，尝试使用默认配置')
        // 根据当前域名判断 API 地址
        const hostname = window.location.hostname
        if (hostname.includes('a292428dsj.dpdns.org')) {
          this.axiosInstance.defaults.baseURL = 'https://api.a292428dsj.dpdns.org'
          this.baseUrl = 'https://api.a292428dsj.dpdns.org'
          this.ready = true
          console.error('[WebApi] ✅ 使用默认生产环境 API 地址: https://api.a292428dsj.dpdns.org')
        } else {
          // 如果无法确定，抛出错误
          throw new Error('无法初始化 API 配置，请检查 serverConfig.json 文件或环境变量')
        }
      }
    }
  }

  // Get
  public async get(url: string, params?: object | null) {
    if (!this.ready) {
      console.error('[WebApi] ❌ 实例未初始化，无法发送请求:', {
        url,
        params,
        baseUrl: this.baseUrl,
        ready: this.ready
      })
      return Promise.reject(new Error('Instance not initialized'))
    }
    
    const fullUrl = `${this.baseUrl}/${url}`.replace(/([^:]\/)\/+/g, '$1')
    console.error('[WebApi] 📤 发送 GET 请求:', {
      url: fullUrl,
      params,
      baseUrl: this.baseUrl
    })
    
    return new Promise<ApiResponse>((resolve) => {
      this.axiosInstance
        .get(url, { params: params })
        .then((response) => {
          const result = response.data
          console.error('[WebApi] ✅ GET 请求成功:', {
            url: fullUrl,
            statusCode: response.status,
            succeed: result.succeed,
            hasData: !!result.data,
            errorMessage: result.errorMessage
          })
          resolve({
            statusCode: response.status,
            data: result.data,
            succeed: result.succeed,
            errorMessage: result.errorMessage
          })
        })
        .catch((error) => {
          // 网络错误（后端服务未运行）
          if (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED') {
            console.error('[WebApi] ❌ 网络错误:', {
              url: fullUrl,
              code: error.code,
              message: error.message
            })
            resolve({
              statusCode: 0,
              succeed: false,
              errorMessage: '无法连接到服务器，请检查后端服务是否运行',
              data: null
            })
            return
          }
          
          // 其他错误输出详细日志
          console.error('[WebApi] ❌ API 请求失败:', {
            url: fullUrl,
            code: error.code,
            message: error.message,
            response: error.response?.data,
            status: error.response?.status
          })
          
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
              errorMessage: result.errorMessage || `HTTP ${response.status}`
            })
            return
          }
          
          // 其他未知错误
          resolve({
            statusCode: 0,
            succeed: false,
            errorMessage: error.message || '未知错误',
            data: null
          })
        })
    })
  }

  // Post
  public async post(url: string, data?: object) {
    if (!this.ready) {
      return Promise.reject(new Error('Instance not initialized'))
    }
    return new Promise<ApiResponse>((resolve) => {
      this.axiosInstance
        .post(url, data)
        .then((response) => {
          const result = response.data
          resolve({
            statusCode: response.status,
            data: result.data,
            succeed: result.succeed,
            errorMessage: result.errorMessage
          })
        })
        .catch((error) => {
          // 网络错误（后端服务未运行）- 静默处理
          if (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED') {
            resolve({
              statusCode: 0,
              succeed: false,
              errorMessage: '无法连接到服务器，请检查后端服务是否运行（http://localhost:5000）',
              data: null
            })
            return
          }
          
          // 其他错误才输出详细日志
          console.error('API 请求失败:', error)
          
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
    })
  }

  // Put
  public async put(url: string, data?: object) {
    if (!this.ready) {
      return Promise.reject(new Error('Instance not initialized'))
    }
    return new Promise<ApiResponse>((resolve) => {
      this.axiosInstance
        .put(url, data)
        .then((response) => {
          const result = response.data
          resolve({
            statusCode: response.status,
            data: result.data,
            succeed: result.succeed,
            errorMessage: result.errorMessage
          })
        })
        .catch((error) => {
          // 网络错误（后端服务未运行）- 静默处理
          if (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED') {
            resolve({
              statusCode: 0,
              succeed: false,
              errorMessage: '无法连接到服务器，请检查后端服务是否运行（http://localhost:5000）',
              data: null
            })
            return
          }
          
          // 其他错误才输出详细日志
          console.error('API 请求失败:', error)
          
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
    })
  }

  // Delete
  public async delete(url: string, params?: object | null) {
    if (!this.ready) {
      return Promise.reject(new Error('Instance not initialized'))
    }
    return new Promise<ApiResponse>((resolve) => {
      this.axiosInstance
        .delete(url, { params })
        .then((response) => {
          const result = response.data
          resolve({
            statusCode: response.status,
            data: result.data,
            succeed: result.succeed,
            errorMessage: result.errorMessage
          })
        })
        .catch((error) => {
          // 网络错误（后端服务未运行）- 静默处理
          if (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED') {
            resolve({
              statusCode: 0,
              succeed: false,
              errorMessage: '无法连接到服务器，请检查后端服务是否运行（http://localhost:5000）',
              data: null
            })
            return
          }
          
          // 其他错误才输出详细日志
          console.error('API 请求失败:', error)
          
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
    })
  }
}
