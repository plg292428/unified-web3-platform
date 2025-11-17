// Utilities
import { defineStore } from 'pinia'
import detectEthereumProvider from '@metamask/detect-provider'
import { reactive } from 'vue'
import { RpcStatus, WalletState } from '@/types'
import { toNumber } from 'ethers'
import { getChainInfo } from '@/utils/chainInfo'
import { measureRpcLatency } from '@/utils/rpcLatency'

type Eip1193Provider = {
  request: (args: { method: string; params?: unknown[] }) => Promise<any>
  on?: (event: string, handler: (...args: any[]) => void) => void
  removeListener?: (event: string, handler: (...args: any[]) => void) => void
  isMetaMask?: boolean
  isBitKeep?: boolean
  isBitget?: boolean
}

const RPC_TARGETS = [
  { name: 'Ethereum', envKey: 'VITE_RPC_ETHEREUM' },
  { name: 'Polygon', envKey: 'VITE_RPC_POLYGON' }
]

export const useWalletStore = defineStore('wallet', () => {
  const state = reactive<WalletState>({
    initialized: false,
    provider: null,
    providerName: null,
    providerType: null,
    address: null,
    active: false,
    chainId: -1,
    networkName: null,
    rpcStatuses: []
  })

  const initialize = async (): Promise<boolean> => {
    if (state.initialized) {
      return !!state.provider
    }

    let detectedProvider: Eip1193Provider | null = null
    let detectedType: string | null = null
    let detectedName: string | null = null

    try {
      const bitgetProvider =
        (window as any).bitkeep?.ethereum ?? (window as any).bitkeep?.ethereumProvider ?? (window as any).bitkeep?.wallet

      if (bitgetProvider) {
        detectedProvider = bitgetProvider
        detectedType = 'bitget'
        detectedName = 'Bitget Wallet'
      }
    } catch (error) {
      console.warn('检测 Bitget 钱包时出错:', error)
    }

    if (!detectedProvider) {
      try {
        // 先检查 window.ethereum 是否存在，避免 detect-provider 输出警告
        if (typeof window !== 'undefined' && (window as any).ethereum) {
          const metamaskProvider = await detectEthereumProvider({ timeout: 3000 })
          if (metamaskProvider) {
            detectedProvider = metamaskProvider as unknown as Eip1193Provider
            if ((metamaskProvider as any)?.isMetaMask) {
              detectedType = 'metamask'
              detectedName = 'MetaMask'
            } else {
              detectedType = 'ethereum'
              detectedName = 'Injected Provider'
            }
          }
        }
      } catch (error) {
        // 静默处理检测错误（用户可能没有安装MetaMask，这是正常的）
        // 不输出警告，避免控制台噪音
      }
    }

    if (detectedProvider) {
      bindProvider(detectedProvider, detectedName, detectedType)
    }

    state.initialized = true
    return !!state.provider
  }

  // 账号改变
  const accountsChange = async (accounts: string[]): Promise<void> => {
    if (!accounts || accounts.length < 1) {
      state.active = false
      state.chainId = -1
      return
    }
    state.address = accounts[0]
    if (state.provider) {
      const chainId = await state.provider.request({ method: 'eth_chainId' })
      updateChain(toNumber(chainId) as number)
      state.active = true
    }
  }

  const connect = async (): Promise<string[] | null> => {
    if (!state.provider) {
      return null
    }
    const accounts: string[] = await state.provider.request({ method: 'eth_requestAccounts' })
    await accountsChange(accounts)
    return accounts
  }

  const refreshRpcStatuses = async (): Promise<RpcStatus[]> => {
    const statuses: RpcStatus[] = []
    for (const item of RPC_TARGETS) {
      const url = (import.meta.env as any)[item.envKey] as string | undefined
      if (!url) {
        continue
      }
      const status: RpcStatus = {
        name: item.name,
        url,
        status: 'unknown'
      }
      try {
        const latency = await measureRpcLatency(url)
        status.status = 'ok'
        status.latency = Math.round(latency)
        status.lastChecked = Date.now()
      } catch (error) {
        // 静默处理认证错误（401/403），这些是预期的（API 密钥可能无效）
        const errorMessage = error instanceof Error ? error.message : 'Unknown error'
        const isAuthError = (error as any)?.isAuthError === true ||
                           (error as any)?.status === 401 ||
                           (error as any)?.status === 403 ||
                           errorMessage.includes('authentication') || 
                           errorMessage.includes('401') || 
                           errorMessage.includes('403')
        
        status.status = 'fail'
        status.errorMessage = isAuthError ? 'API key invalid or expired' : errorMessage
        
        // 对于认证错误，静默处理（不输出到控制台，避免噪音）
        // 只在开发环境输出警告日志
        if (isAuthError) {
          if (import.meta.env.DEV) {
            console.warn(`[RPC] ${item.name} RPC authentication failed (API key may be invalid). URL: ${url.substring(0, 50)}...`)
          }
          // 生产环境完全静默，不输出任何日志
        } else {
          // 非认证错误才输出到控制台
          console.error(`[RPC] ${item.name} RPC check failed:`, errorMessage)
        }
      }
      statuses.push(status)
    }
    state.rpcStatuses = statuses
    return statuses
  }

  const bindProvider = (provider: Eip1193Provider, name: string | null, type: string | null) => {
    state.provider = provider
    state.providerName = name
    state.providerType = type

    provider.on?.('accountsChanged', accountsChange)
    provider.on?.('chainChanged', (chainIdHex: string) => {
      updateChain(toNumber(chainIdHex) as number)
    })
  }

  const updateChain = (chainId: number) => {
    state.chainId = chainId
    const info = getChainInfo(chainId)
    state.networkName = info?.name ?? (chainId > 0 ? `Chain ${chainId}` : null)
  }

  return { state, initialize, accountsChange, connect, refreshRpcStatuses }
})
