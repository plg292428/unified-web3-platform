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

import '@/libs/DateExtends';

export const app = createApp(App)

registerPlugins(app)

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
