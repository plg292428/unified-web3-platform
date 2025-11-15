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
      // 优先使用环境变量配置
      const envApiUrl = import.meta.env.VITE_API_BASE_URL
      if (envApiUrl) {
        this.axiosInstance.defaults.baseURL = envApiUrl
        this.baseUrl = envApiUrl
        this.ready = true
        console.log('API Base URL from .env:', envApiUrl)
        return
      }

      // 如果没有环境变量，则从 serverConfig.json 读取
      // 注意：serverConfig.json 在 public 目录下，使用相对路径
      const response = await fetch('/serverConfig.json')
      if (!response.ok) {
        throw new Error(`无法加载 serverConfig.json: ${response.status}`)
      }
      const config = await response.json()
      
      if (import.meta.env.PROD) {
        this.axiosInstance.defaults.baseURL = config.productionBaseUrl
      } else {
        this.axiosInstance.defaults.baseURL = config.developmentBaseUrl
      }
      this.baseUrl = this.axiosInstance.defaults.baseURL
      this.ready = true
      console.log('API Base URL from serverConfig.json:', this.baseUrl)
    } catch (error) {
      console.error('WebApi 初始化失败:', error)
      // 开发环境：设置默认值
      if (import.meta.env.DEV) {
        this.axiosInstance.defaults.baseURL = 'http://localhost:5000'
        this.baseUrl = 'http://localhost:5000'
        this.ready = true
        console.warn('使用默认开发环境 API 地址: http://localhost:5000')
        console.warn('提示: 请确保后端服务已启动')
      } else {
        // 生产环境初始化失败，抛出错误
        throw new Error('无法初始化 API 配置，请检查 serverConfig.json 文件')
      }
    }
  }

  // Get
  public async get(url: string, params?: object | null) {
    if (!this.ready) {
      return Promise.reject(new Error('Instance not initialized'))
    }
    return new Promise<ApiResponse>((resolve) => {
      this.axiosInstance
        .get(url, { params: params })
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
          // 网络错误（后端服务未运行）- 静默处理，不输出详细错误
          if (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED') {
            // 只在开发环境且需要调试时输出
            // console.error('API 请求失败:', error)
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
