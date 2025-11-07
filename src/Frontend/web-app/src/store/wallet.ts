// Utilities
import { defineStore } from 'pinia'
import detectEthereumProvider from '@metamask/detect-provider'
import { reactive } from 'vue'
import { WalletState } from '@/types'
import { toNumber } from 'ethers'

export const useWalletStore = defineStore('wallet', () => {
  const state = reactive<WalletState>({
    initialized: false,
    provider: null,
    address: null,
    active: false,
    chainId: -1
  })

  const initialize = async (): Promise<boolean> => {
    if (state.initialized) {
      return false
    }

    // 检测Eth钱包;
    const ethProvider = await detectEthereumProvider({ timeout: 3000 })

    if (ethProvider) {
      state.provider = ethProvider
      state.initialized = true
      return true
    }
    state.initialized = true
    return true
  }

  // 账号改变
  const accountsChange = async (accounts: string[]): Promise<void> => {
    if (!accounts || accounts.length < 1) {
      state.active = false
      state.chainId = -1
      return
    }
    state.address = accounts[0]
    const provider = state.provider
    const chainId = await provider.request({ method: "eth_chainId" });
    state.chainId = toNumber(chainId) as number
    state.active = true
  }

  return { state, initialize, accountsChange }
})
