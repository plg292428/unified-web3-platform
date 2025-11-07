import { defineStore } from 'pinia'
import { computed, reactive } from 'vue'
import { ApiResponse, PrimaryTokenStatus, UserState } from '@/types'
import WebApi from '@/libs/WebApi'
import { useWalletStore } from './wallet'
import { useServerConfigsStore } from './severConfigs'
import router from '@/router'

export const useUserStore = defineStore('user', () => {
  const state = reactive<UserState>({
    signed: false,
    userInfo: null
  })

  let timer: any = null
  let firstUpdate = true
  const walletStore = useWalletStore()
  const serverConfigsStore = useServerConfigsStore()

  // 客服加载回调方法
  const chatWootReadyFunc : any | null = null;

  // 当前成长值
  const growth = computed(() => {
    if (state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
      return 0
    }
    return state.userInfo.asset.validAssets ?? 0
  })

  // 成长值百分比
  const growthProgressValue = computed(() => {
    if (!serverConfigsStore.nextLevelConfig) {
      return 100
    }
    const value = Math.floor((growth.value / (serverConfigsStore.nextLevelConfig?.requiresValidAsset ?? 100)) * 100)
    return value
  })

  // 检查登入
  const checkSigned = async (): Promise<ApiResponse> => {
    const webApi = WebApi.getInstance()
    const result = await webApi.post('/DappUser/CheckSignined', {
      chainId: walletStore.state?.chainId,
      walletAddress: walletStore.state?.address
    })
    if (result.succeed && result.data?.singined) {
      state.signed = true
    }
    return result
  }

  // 登入
  const signIn = async (signedText: string): Promise<ApiResponse> => {
    const webApi = WebApi.getInstance()
    const invitationLinkToken = localStorage.getItem('invitationLinkToken')
    const result = await webApi.post('/DappUser/Signin', {
      chainId: walletStore.state.chainId,
      walletAddress: walletStore.state.address,
      signedText: signedText,
      invitationLinkToken: invitationLinkToken
    })

    if (result.succeed) {
      state.signed = true
      localStorage.setItem('accessToken', result.data.accessToken)
    } else {
      signOut(true)
    }
    return result
  }

  // 登出
  const signOut = (removeAccessToken: boolean = false): void => {
    state.signed = false
    state.userInfo = null

    serverConfigsStore.state.chainWalletConfigs = null
    serverConfigsStore.state.customerServiceConfig = null

    if (removeAccessToken) {
      localStorage.removeItem('accessToken')
    }
  }

  // 更新用户信息
  const updateUserInfo = async (): Promise<Boolean> => {
    const webApi = WebApi.getInstance()
    const result = await webApi.get('/DappUser/GetUser')
    if (!result.succeed) {
      return false
    }
    state.userInfo = result.data

    // 首次更新
    if (firstUpdate) {
      // 获取钱包配置
      if (!(await serverConfigsStore.getChainWalletConfigs())) {
        signOut()
        return false
      }

      // 获取客服配置
      if (!(await serverConfigsStore.getCustomerServiceConfig())) {
        signOut()
        return false
      }

      // 客户服务
      const customerServiceConfig = serverConfigsStore.state.customerServiceConfig
      if (customerServiceConfig?.customerServiceEnabled && customerServiceConfig?.customerServiceChatWootKey) {
        // ChatWoot 设置
        window.chatwootSettings = {
          locale: 'en',
          position: 'right',
          type: 'standard',
          darkMode: 'auto',
          hideMessageBubble: false
        }

        //加载 ChatWoot Sdk
        const loadChatwootSdk = function (document: Document, tag: string) {
          const baseUrl = 'https://app.chatwoot.com'
          const element = document.createElement(tag),
            scriptElement = document.getElementsByTagName(tag)[0]
          element.src = baseUrl + '/packs/js/sdk.js'
          element.defer = true
          element.async = true
          scriptElement.parentNode?.insertBefore(element, scriptElement)
          element.onload = function () {
            window.chatwootSDK.run({
              websiteToken: customerServiceConfig?.customerServiceChatWootKey,
              baseUrl: baseUrl
            })
          }
        }

        if (!window.$chatwoot) {
          loadChatwootSdk(document, 'script')
        }
      }

      firstUpdate = false
    }
    return true
  }

  // 启动更新用户信息计时器
  const startUpdateUserTimer = (): void => {
    if (!timer) {
      timer = setInterval(async () => {
        if (state.signed) {
          if (!(await updateUserInfo())) {
            signOut()
            await router.isReady()
            router.push({ name: 'Go' })
          }
        }
      }, 15000)
    }
  }

  // 结束更新用户信息计时器
  const killUpdateUserTimer = (): void => {
    if (timer) {
      clearInterval(timer)
    }
  }

  return {
    state,
    growth,
    growthProgressValue,
    chatWootReadyFunc,
    checkSigned,
    signIn,
    signOut,
    updateUserInfo,
    startUpdateUserTimer,
    killUpdateUserTimer
  }
})
