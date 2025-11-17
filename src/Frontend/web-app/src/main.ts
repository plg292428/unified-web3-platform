/**
 * main.ts
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Components
import App from './App.vue'

// Composables
import { createApp } from 'vue'

// Plugins
import { registerPlugins } from '@/plugins'
import WebApi from '@/libs/WebApi'
import { getRpcUrls } from '@/utils/rpcClients'

import '@/libs/DateExtends';

export const app = createApp(App)

registerPlugins(app)

// 暴露环境变量检查函数到全局（用于浏览器控制台调试）
// 使用立即执行确保函数在模块加载时就可用
if (typeof window !== 'undefined') {
  // 直接挂载到 window，确保立即可用
  window.checkRpcConfig = function() {
    const config = getRpcUrls()
    console.log('=== RPC 环境变量检查 ===')
    console.log('Ethereum RPC:')
    console.log('  - 配置状态:', config.ethereumConfigured ? '✅ 已配置' : '❌ 未配置')
    console.log('  - 环境变量值:', config.ethereumEnv || '(未设置)')
    console.log('  - 实际使用 URL:', config.ethereum)
    console.log('')
    console.log('Polygon RPC:')
    console.log('  - 配置状态:', config.polygonConfigured ? '✅ 已配置' : '❌ 未配置')
    console.log('  - 环境变量值:', config.polygonEnv || '(未设置)')
    console.log('  - 实际使用 URL:', config.polygon)
    console.log('')
    
    if (config.polygonEnv) {
      const isInfura = config.polygonEnv.includes('infura')
      const hasApiKey = config.polygonEnv.includes('/v3/')
      console.log('Polygon RPC 详情:')
      console.log('  - 是否 Infura:', isInfura ? '✅ 是' : '❌ 否')
      console.log('  - 包含 API 密钥:', hasApiKey ? '✅ 是' : '❌ 否')
      
      if (hasApiKey) {
        const apiKeyMatch = config.polygonEnv.match(/\/v3\/([a-zA-Z0-9]+)/)
        if (apiKeyMatch) {
          const apiKey = apiKeyMatch[1]
          console.log('  - API 密钥前缀:', apiKey.substring(0, 8) + '...')
          console.log('  - API 密钥长度:', apiKey.length, '(正常应该是32个字符)')
          if (apiKey.length < 20) {
            console.warn('  ⚠️ API 密钥长度异常，可能无效')
          }
        }
      }
    } else {
      console.log('  ℹ️ 未配置环境变量，使用默认公共 RPC:', config.polygon)
    }
    
    console.log('=== 检查完成 ===')
    return config
  }
  
  // 同时暴露到全局类型
  ;(window as any).checkRpcConfig = window.checkRpcConfig
}

// 初始化（为了兼容性，不使用顶级 await）
const webApi = WebApi.getInstance()
webApi.initialize().then(() => {
  app.config.globalProperties.$baseUrl = webApi.baseUrl as string
  app.mount('#app')
})

app.config.globalProperties.$filters = {
  currencyUSD(value: string) {
    return '$' + value
  }
}
