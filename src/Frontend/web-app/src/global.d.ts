/*
 *
 * 全局类型定义
 *
 */
export {}

declare global {
  interface Window {
    chatwootSDK: any
    chatwootSettings: any
    $chatwoot: any
    ethereum: any
    checkRpcConfig?: () => any
  }

  interface HTMLElement{
    src: string | null | undefined
    defer: boolean
    async: boolean 
  }

  interface Date{
    toShortTime: Function
  }
}

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $baseUrl:  (key: string) => string
  }
}
