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

      // 如果没有环境变量，则从serverConfig.json读取
      const response = await this.axiosInstance.get('/serverConfig.json')
      if (process.env.NODE_ENV === 'production') {
        this.axiosInstance.defaults.baseURL = response.data.productionBaseUrl
      } else {
        this.axiosInstance.defaults.baseURL = response.data.developmentBaseUrl
      }
      this.baseUrl = this.axiosInstance.defaults.baseURL
      this.ready = true
    } catch (error) {
      console.error(error)
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
          console.log(error)
          if (error.response && error.response.data) {
            const response = error.response
            const result = response.data
            resolve({
              statusCode: response.status,
              data: result.data,
              succeed: result.succeed,
              errorMessage: result.errorMessage
            })
          }
          resolve(WebApi.baseResponseData)
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
          console.log(error)
          if (error.response && error.response.data) {
            const response = error.response
            const result = response.data
            resolve({
              statusCode: response.status,
              data: result.data,
              succeed: result.succeed,
              errorMessage: result.errorMessage
            })
          }
          resolve(WebApi.baseResponseData)
        })
    })
  }
}
