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
        const metamaskProvider = await detectEthereumProvider({ timeout: 3000 })
        if (metamaskProvider) {
          detectedProvider = metamaskProvider as Eip1193Provider
          if ((metamaskProvider as any)?.isMetaMask) {
            detectedType = 'metamask'
            detectedName = 'MetaMask'
          } else {
            detectedType = 'ethereum'
            detectedName = 'Injected Provider'
          }
        }
      } catch (error) {
        // 静默处理检测错误，继续执行
        console.warn('检测钱包提供方时出错:', error)
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
        status.status = 'fail'
        status.errorMessage = error instanceof Error ? error.message : 'Unknown error'
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
