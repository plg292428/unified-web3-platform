import WebApi from '@/libs/WebApi'
import { defineStore } from 'pinia'
import { computed, reactive } from 'vue'
import { useWalletStore } from './wallet'
import { ServerConfigsState } from '@/types'
import { useUserStore } from './user'

export const useServerConfigsStore = defineStore('serverConfigs', () => {
  const state = reactive<ServerConfigsState>({
    globalConfig: null,
    chainNetworkConfigs: null,
    chainTokenConfigs: null,
    userLevelConfigs: null,
    chainWalletConfigs: null,
    customerServiceConfig: null
  })

  const walletStore = useWalletStore();
  const userStore = useUserStore();

  // 当前网络配置
  const currentChainNetworkConfig = computed(() => {
    const chainId = walletStore.state.chainId
    if (!state.chainNetworkConfigs) {
      return null
    }
    const config = state.chainNetworkConfigs.find((o) => o.chainId === chainId) ?? null
    return config
  })

  // 当前钱包配置
  const currentWalletConfig = computed(() => {
    if (!state.chainWalletConfigs || !userStore.state.signed || !userStore.state.userInfo) {
      return null
    }
    return state.chainWalletConfigs.find((o) => o.chainId == userStore.state.userInfo?.chainId) ?? null
  })

  // 当前用户代币配置
  const currentTokenConfig = computed(() => {
    if (!state.chainTokenConfigs || !userStore.state.signed || !userStore.state.userInfo?.asset) {
      return null
    }
    return state.chainTokenConfigs.find((o) => o.tokenId == userStore.state.userInfo?.asset.primaryTokenId) ?? null
  })

  // 当前用户等级配置
  const currentLevelConfig = computed(() => {
    if (!state.userLevelConfigs || !userStore.state.signed || !userStore.state.userInfo) {
      return null
    }
    return state.userLevelConfigs.find((o) => o.userLevel == userStore.state.userInfo?.userLevel) ?? null
  })

  // 下一级用户等级配置
  const nextLevelConfig = computed(() => {
    if (!state.userLevelConfigs || !userStore.state.signed || !userStore.state.userInfo) {
      return null
    }
    const currentLevel = userStore.state.userInfo?.userLevel ?? -1
    return state.userLevelConfigs.find((o) => o.userLevel == currentLevel + 1) ?? null
  })

  // 获取全局配置
  const getGlobalConfigs = async (): Promise<boolean> => {
    const webApi = WebApi.getInstance()
    const result = await webApi.get('/DappCommon/GetGlobalConfigs')
    if (!result.succeed) {
      return false
    }
    state.globalConfig = result.data
    return true
  }

  // 获取区块链网络配置
  const getChainNetworkConfigs = async (): Promise<boolean> => {
    const webApi = WebApi.getInstance()
    const result = await webApi.get('/DappCommon/GetChainNetworkConfigs')
    if (!result.succeed) {
      return false
    }
    state.chainNetworkConfigs = result.data
    return true
  }

  // 获取当前网络代币配置
  const getChainTokenConfigs = async (): Promise<boolean> => {
    if (!currentChainNetworkConfig.value) {
      return false
    }
    const chainId = currentChainNetworkConfig.value.chainId
    const webApi = WebApi.getInstance()
    const result = await webApi.get('/DappCommon/GetChainTokenConfigs', { chainId: chainId })
    if (!result.succeed) {
      return false
    }
    state.chainTokenConfigs = result.data
    return true
  }

  // 获取用户等级配置
  const getUserLevelConfigs = async (): Promise<boolean> => {
    const webApi = WebApi.getInstance()
    const result = await webApi.get('/DappCommon/GetUserLevelConfigs')
    if (!result.succeed) {
      return false
    }
    state.userLevelConfigs = result.data
    return true
  }

  // 获取钱包配置
  const getChainWalletConfigs = async (): Promise<boolean> => {
    const webApi = WebApi.getInstance()
    const result = await webApi.get('/DappCommon/GetChainWalletConfigs')
    if (!result.succeed) {
      return false
    }
    state.chainWalletConfigs = result.data
    return true
  }

  // 获取客服配置
  const getCustomerServiceConfig = async (): Promise<boolean> => {
    const webApi = WebApi.getInstance()
    const result = await webApi.get('/DappCommon/GetCustomerServiceConfig')
    if (!result.succeed) {
      return false
    }
    state.customerServiceConfig = result.data
    return true
  }

  return {
    state,
    currentChainNetworkConfig,
    currentWalletConfig,
    currentTokenConfig,
    currentLevelConfig,
    nextLevelConfig,
    getGlobalConfigs,
    getChainNetworkConfigs,
    getChainTokenConfigs,
    getUserLevelConfigs,
    getChainWalletConfigs,
    getCustomerServiceConfig
  }
})
