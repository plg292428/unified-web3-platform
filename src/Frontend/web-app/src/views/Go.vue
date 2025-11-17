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
  </v-container>
</template>

<script lang="ts" setup>
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { useWalletStore } from '@/store/wallet'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { ethers } from "ethers";
import FastDialog from '@/libs/FastDialog'

const router = useRouter()
const walletStore = useWalletStore()
const serverConfigsStore = useServerConfigsStore()

const buttonLoading = ref(false)
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

// 登录处理
async function getStarted() {
  buttonLoading.value = true

  // 检查登录
  const userStore = useUserStore()
  const checkSignedResult = await userStore.checkSigned()

  if (!checkSignedResult.succeed) {
    FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
    buttonLoading.value = false
    return
  }

  if (checkSignedResult.data?.singined) {
    // 检查是否有待处理的产品购买操作
    const pendingProductId = localStorage.getItem('pendingProductId')
    const pendingAction = localStorage.getItem('pendingAction')
    
    if (pendingProductId && pendingAction === 'buyNow') {
      // 清除待处理操作标记
      localStorage.removeItem('pendingProductId')
      localStorage.removeItem('pendingAction')
      
      // 跳转到产品详情页，并触发购买操作
      router.push({ 
        name: 'ProductDetail', 
        params: { productId: pendingProductId },
        query: { autoBuy: 'true' }
      })
    } else {
      router.push({ name: 'Home' })
    }
    return
  }

  // 签名
  const from = walletStore.state.address
  const provider = walletStore.state.provider
  const message = `0x${checkSignedResult.data.tokenText}`;
  provider
    .request({
      method: 'personal_sign',
      params: [message, from]
    })
    .then((signedText: string) => {
      signIn(signedText)
    })
    .catch(() => {
      FastDialog.errorSnackbar('Sign in failed, signature verification not completed.')
      buttonLoading.value = false
      return
    })
}

// 登入
async function signIn(signedText: string) {
  const userStore = useUserStore()
  const signResult = await userStore.signIn(signedText)
  if (!signResult.succeed) {
    FastDialog.errorSnackbar(signResult.errorMessage as string)
    buttonLoading.value = false
    return
  }

  // 获取一次用户信息
  if (!(await userStore.updateUserInfo())) {
    userStore.signOut()
    router.push({ name: 'Error500' })
    return
  }

  buttonLoading.value = false
  
  // 检查是否有待处理的产品购买操作
  const pendingProductId = localStorage.getItem('pendingProductId')
  const pendingAction = localStorage.getItem('pendingAction')
  
  if (pendingProductId && pendingAction === 'buyNow') {
    // 清除待处理操作标记
    localStorage.removeItem('pendingProductId')
    localStorage.removeItem('pendingAction')
    
    // 跳转到产品详情页，并触发购买操作
    router.push({ 
      name: 'ProductDetail', 
      params: { productId: pendingProductId },
      query: { autoBuy: 'true' }
    })
  } else {
    router.push({ name: 'Home' })
  }
}
</script>
