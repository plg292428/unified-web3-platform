<template>
  <v-container class="fill-height">
    <v-responsive class="align-center text-center fill-height">
      <v-img height="256" src="@/assets/ether.svg" />

      <div class="text-body-1 mt-6 text-grey-darken-1">Bring Ethereum to</div>

      <h1 class="text-h3 font-weight-bold">Everyone</h1>

      <div class="py-3"></div>

      <div class="text-body-1 text-grey-darken-1">Earn rewards while securing the Ethereum layer2 network.</div>

      <div class="py-3"></div>

      <v-row class="d-flex align-center justify-center">
        <v-col cols="auto">
          <v-btn color="primary" min-width="196" :loading="buttonLoading" @click="getStarted">
            <v-icon icon="mdi-speedometer" start />
            Get Started
          </v-btn>
        </v-col>
      </v-row>

      <div class="text-body-2 text-grey-darken-1 mt-9">Partner Wallets</div>
      <div class="mt-1">
        <template v-for="(wallet, index) in wallts" :key="wallet.alt">
          <v-avatar :class="index > 0 ? 'ml-2' : ''" size="28" rounded>
            <v-img :src="wallet.src" :alt="wallet.alt"></v-img>
          </v-avatar>
        </template>
      </div>

      <div class="text-body-2 text-grey-darken-1 mt-3">Supported Networks</div>
      <div class="mt-1" v-if="chainNetworkConfigs">
        <template v-for="config in chainNetworkConfigs" :key="config.chainId">
          <v-avatar :class="config.chainId > 0 ? 'ml-2' : ''" size="28">
            <v-img :src="`${$baseUrl}${config.chainIconPath}`" :alt="config.abbrNetworkName ?? 'network'"></v-img>
          </v-avatar>
        </template>
      </div>
    </v-responsive>

    <!-- Wallet Installation Dialog -->
    <v-dialog v-model="showWalletDialog" max-width="520" persistent>
      <v-card>
        <v-card-title class="d-flex align-center">
          <v-icon icon="mdi-wallet" class="mr-2" color="primary"></v-icon>
          <span>Install Wallet</span>
        </v-card-title>
        <v-divider></v-divider>
        <v-card-text class="pt-6">
          <div class="text-body-1 text-center mb-4">
            No wallet detected. Please choose a wallet to install:
          </div>
          <v-row dense>
            <v-col cols="12" sm="6">
              <v-card
                variant="outlined"
                class="wallet-option-card"
                :class="{ 'selected': selectedWallet === 'metamask' }"
                @click="selectWallet('metamask')"
                style="cursor: pointer; transition: all 0.2s;"
              >
                <v-card-text class="text-center pa-4">
                  <v-avatar size="64" class="mb-3">
                    <v-img :src="getWalletIcon('MetaMask')" :alt="'MetaMask'"></v-img>
                  </v-avatar>
                  <div class="text-subtitle-1 font-weight-medium">MetaMask</div>
                  <div class="text-caption text-grey-lighten-1 mt-1">
                    The most popular Web3 wallet
                  </div>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12" sm="6">
              <v-card
                variant="outlined"
                class="wallet-option-card"
                :class="{ 'selected': selectedWallet === 'bitget' }"
                @click="selectWallet('bitget')"
                style="cursor: pointer; transition: all 0.2s;"
              >
                <v-card-text class="text-center pa-4">
                  <v-avatar size="64" class="mb-3">
                    <v-img :src="getWalletIcon('Bitget')" :alt="'Bitget Wallet'"></v-img>
                  </v-avatar>
                  <div class="text-subtitle-1 font-weight-medium">Bitget Wallet</div>
                  <div class="text-caption text-grey-lighten-1 mt-1">
                    Secure multi-chain wallet
                  </div>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </v-card-text>
        <v-divider></v-divider>
        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn variant="text" @click="closeWalletDialog">Cancel</v-btn>
          <v-btn
            color="primary"
            variant="flat"
            :disabled="!selectedWallet"
            @click="installSelectedWallet"
          >
            Install
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script lang="ts" setup>
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { useWalletStore } from '@/store/wallet'
import { ref, onBeforeMount } from 'vue'
import { useRouter } from 'vue-router'
import { ethers } from "ethers";
import FastDialog from '@/libs/FastDialog'

const router = useRouter()
const walletStore = useWalletStore()
const serverConfigsStore = useServerConfigsStore()

const buttonLoading = ref(false)
const showWalletDialog = ref(false)
const selectedWallet = ref<'metamask' | 'bitget' | null>(null)
const chainNetworkConfigs = serverConfigsStore.state.chainNetworkConfigs
const wallts = [
  { src: getWalletIcon('Trust'), alt: 'Trust' },
  { src: getWalletIcon('Bitget'), alt: 'Bitget' },
  { src: getWalletIcon('MetaMask'), alt: 'MetaMask' },
  { src: getWalletIcon('Coinbase'), alt: 'Coinbase' },
  { src: getWalletIcon('TokenPocket'), alt: 'TokenPocket' }
]

function getWalletIcon(name: string) {
  return new URL(`../assets/wallets/${name}.webp`, import.meta.url).href
}

function selectWallet(wallet: 'metamask' | 'bitget') {
  selectedWallet.value = wallet
}

function closeWalletDialog() {
  showWalletDialog.value = false
  selectedWallet.value = null
}

function installSelectedWallet() {
  if (!selectedWallet.value) return
  
  if (selectedWallet.value === 'metamask') {
    window.open('https://metamask.io/download/', '_blank', 'noopener')
  } else if (selectedWallet.value === 'bitget') {
    window.open('https://web3.bitget.com/en/tools/wallet', '_blank', 'noopener')
  }
  
  FastDialog.infoSnackbar('Opening wallet download page in a new tab...')
  closeWalletDialog()
}

// Login handling
async function getStarted() {
  buttonLoading.value = true

  try {
    // Initialize wallet if not already initialized
    if (!walletStore.state.initialized) {
      await walletStore.initialize()
    }

    // Check if wallet provider exists
    if (!walletStore.state.provider) {
      buttonLoading.value = false
      // Show wallet selection dialog
      showWalletDialog.value = true
      return
    }

    // Check if wallet is connected
    if (!walletStore.state.active || !walletStore.state.address) {
      try {
        FastDialog.infoSnackbar('Connecting wallet...')
        const accounts = await walletStore.connect()
        if (!accounts || accounts.length === 0) {
          FastDialog.errorSnackbar('Failed to connect wallet. Please unlock your wallet and try again.')
          buttonLoading.value = false
          return
        }
      } catch (error: any) {
        console.error('Wallet connection error:', error)
        buttonLoading.value = false
        if (error.code === 4001) {
          FastDialog.warningSnackbar('Connection cancelled. Please approve the connection request to continue.')
        } else if (error.code === -32002) {
          FastDialog.warningSnackbar('Connection request already pending. Please check your wallet.')
        } else {
          FastDialog.errorSnackbar('Failed to connect wallet. Please check your wallet and try again.')
        }
        return
      }
    }

    // Check login
    const userStore = useUserStore()
    const checkSignedResult = await userStore.checkSigned()

    if (!checkSignedResult.succeed) {
      FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
      buttonLoading.value = false
      return
    }

    if (checkSignedResult.data?.singined) {
      // Already logged in, check if there is pending product purchase operation
      const pendingProductId = localStorage.getItem('pendingProductId')
      const pendingAction = localStorage.getItem('pendingAction')
      
      if (pendingProductId && pendingAction === 'buyNow') {
        // Clear pending operation markers
        localStorage.removeItem('pendingProductId')
        localStorage.removeItem('pendingAction')
        
        // Navigate to product detail page and trigger purchase operation
        router.push({ 
          name: 'ProductDetail', 
          params: { productId: pendingProductId },
          query: { autoBuy: 'true' }
        })
      } else {
        // Check if there are temporary cart items to transfer
        const { getTemporaryCartItems } = await import('@/utils/temporaryCart')
        const tempItems = getTemporaryCartItems()
        if (tempItems.length > 0) {
          // Redirect to cart to transfer temporary cart items
          router.push({ name: 'Cart' })
        } else {
          router.push({ name: 'Home' })
        }
      }
      buttonLoading.value = false
      return
    }

    // Sign message for login
    const from = walletStore.state.address
    const provider = walletStore.state.provider
    if (!from || !provider) {
      FastDialog.errorSnackbar('Wallet not connected. Please connect your wallet first.')
      buttonLoading.value = false
      return
    }

    const message = `0x${checkSignedResult.data.tokenText}`
    
    try {
      FastDialog.infoSnackbar('Please sign the message in your wallet to login...')
      const signedText = await provider.request({
        method: 'personal_sign',
        params: [message, from]
      })
      await signIn(signedText)
    } catch (error: any) {
      console.error('Sign error:', error)
      buttonLoading.value = false
      if (error.code === 4001) {
        FastDialog.warningSnackbar('Signing cancelled. Please sign the message to continue.')
      } else {
        FastDialog.errorSnackbar('Sign in failed, signature verification not completed.')
      }
    }
  } catch (error) {
    console.error('Get started error:', error)
    FastDialog.errorSnackbar('An error occurred. Please try again.')
    buttonLoading.value = false
  }
}

// Sign in
async function signIn(signedText: string) {
  try {
    const userStore = useUserStore()
    const signResult = await userStore.signIn(signedText)
    if (!signResult.succeed) {
      FastDialog.errorSnackbar(signResult.errorMessage as string)
      buttonLoading.value = false
      return
    }

    // Get user info once
    if (!(await userStore.updateUserInfo())) {
      userStore.signOut()
      router.push({ name: 'Error500' })
      buttonLoading.value = false
      return
    }

    buttonLoading.value = false
    
    // Check if there are temporary cart items to transfer
    const { getTemporaryCartItems } = await import('@/utils/temporaryCart')
    const tempItems = getTemporaryCartItems()
    
    // Check if there is pending product purchase operation
    const pendingProductId = localStorage.getItem('pendingProductId')
    const pendingAction = localStorage.getItem('pendingAction')
    
    if (pendingProductId && pendingAction === 'buyNow') {
      // Clear pending operation markers
      localStorage.removeItem('pendingProductId')
      localStorage.removeItem('pendingAction')
      
      // Navigate to product detail page and trigger purchase operation
      router.push({ 
        name: 'ProductDetail', 
        params: { productId: pendingProductId },
        query: { autoBuy: 'true' }
      })
    } else if (tempItems.length > 0) {
      // Redirect to cart to transfer temporary cart items
      FastDialog.successSnackbar('Login successful! Redirecting to cart...')
      router.push({ name: 'Cart' })
    } else {
      FastDialog.successSnackbar('Login successful!')
      router.push({ name: 'Home' })
    }
  } catch (error) {
    console.error('Sign in error:', error)
    FastDialog.errorSnackbar('An error occurred during sign in. Please try again.')
    buttonLoading.value = false
  }
}

// Initialize wallet when page loads
onBeforeMount(async () => {
  if (!walletStore.state.initialized) {
    await walletStore.initialize()
  }
})
</script>

<style scoped>
.wallet-option-card {
  border: 2px solid transparent;
  transition: all 0.2s ease;
}

.wallet-option-card:hover {
  border-color: rgba(var(--v-theme-primary), 0.5);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.wallet-option-card.selected {
  border-color: rgb(var(--v-theme-primary));
  background-color: rgba(var(--v-theme-primary), 0.1);
}
</style>
