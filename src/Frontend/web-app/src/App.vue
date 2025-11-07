<template>
  <v-overlay persistent :model-value="appLoading && !route.meta?.errorPage" class="align-center justify-center">
    <v-progress-circular color="primary" indeterminate size="64"></v-progress-circular>
  </v-overlay>

  <router-view v-if="!appLoading || route.meta?.errorPage" />
</template>

<script lang="ts" setup>
import { onBeforeMount, ref } from 'vue'
import { useWalletStore } from './store/wallet'
import { useRoute, useRouter } from 'vue-router'
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { onUnmounted } from 'vue'
import FastDialog from './libs/FastDialog'

const route = useRoute()
const router = useRouter()

const walletStore = useWalletStore()
const userStore = useUserStore()
const serverConfigsStore = useServerConfigsStore()

const appLoading = ref(true)

onBeforeMount(async () => {
  // 等待路由就绪
  await router.isReady()

  // 推荐令牌
  const invitationLinkToken = route.query.inviter?.toString() ?? null
  if (invitationLinkToken) {
    localStorage.setItem('invitationLinkToken', invitationLinkToken)
  }

  // 处于错误页面时先返回首页
  if (route.meta.errorPage && route.name !== 'Error404') {
    router.push({ name: 'Go' })
  }

  // 获取服务器区块链配置
  if (!(await serverConfigsStore.getChainNetworkConfigs())) {
    FastDialog.errorSnackbar('Unable to obtain blockchain network configuration information.')
    userStore.signOut()
    router.push({ name: 'Error500' })
    return
  }

  // 初始化钱包提供方
  await walletStore.initialize()

  // 未检测到钱包
  if (walletStore.state.provider === null) {
    router.push({ name: 'ErrorNoWalletDetected' })
    return
  }

  const provider = walletStore.state.provider

  // 链更改
  provider.on('chainChanged', (_: any) => {
    window.location.reload()
  })

  // 账户更改
  provider.on('accountsChanged', (_: string[]) => {
    window.location.reload()
  })

  // RPC连接
  provider.on('connect', (connectInfo: any) => {
    console.log(connectInfo) // example: {chainId: '0x2b6653dc' }
  })

  // RPC断开连接
  provider.on('disconnect', (providerRpcError: any) => {
    console.error(providerRpcError) // example: { code: 4900, message: 'Disconnected' }
  })

  // 请求链接钱包
  await provider
    .request({ method: 'eth_requestAccounts' })
    .then((accounts: string[]) => {
      appReady(accounts)
    })
    .catch((error: any) => {
      if (error.code == -32002) {
        FastDialog.errorSnackbar('Unable to obtain wallet information, please connect to the wallet first.')
      } else if (error.code == 4001) {
        FastDialog.errorSnackbar('Unable to obtain wallet information, please connect to the wallet first.')
      }
      return
    })
})

// 组件卸载
onUnmounted(async () => {
  console.log('App Unmounted')
  userStore.killUpdateUserTimer()
})

async function appReady(walletAccounts: string[]) {
  // 监听 ChatWoot Sdk
  window.addEventListener('chatwoot:ready', function () {
    window.$chatwoot.setCustomAttributes({
      Uid: userStore.state.userInfo?.uid,
      Address: walletStore.state.address,
      Network: serverConfigsStore.currentChainNetworkConfig?.networkName
    })
    if (route.name !== 'Home') {
      window.$chatwoot.toggleBubbleVisibility('hide')
    } else {
      userStore.chatWootReadyFunc()
    }
  })

  // 更新状态
  await walletStore.accountsChange(walletAccounts)

  // 检查是否支持网络
  if (!serverConfigsStore.currentChainNetworkConfig) {
    router.push({ name: 'ErrorUnsupportedNetwork' })
    return
  }

  // 获取全局配置
  if (!(await serverConfigsStore.getGlobalConfigs())) {
    FastDialog.errorSnackbar('Unable to obtain server configuration information.')
    userStore.signOut()
    router.push({ name: 'Error500' })
    return
  }

  // 获取服务器代币配置集
  if (!(await serverConfigsStore.getChainTokenConfigs())) {
    FastDialog.errorSnackbar('Unable to obtain server token configuration information.')
    userStore.signOut()
    router.push({ name: 'Error500' })
    return
  }

  // 获取服务器用户等级配置
  if (!(await serverConfigsStore.getUserLevelConfigs())) {
    FastDialog.errorSnackbar('Unable to obtain server level configuration information.')
    userStore.signOut()
    router.push({ name: 'Error500' })
    return
  }

  // 检查登录
  const checkSignedResult = await userStore.checkSigned()
  if (!checkSignedResult.data?.singined) {
    if (route.meta?.requireSigned) {
      router.push({ name: 'Go' })
    }
  } else {
    // 获取一次用户信息
    if (!(await userStore.updateUserInfo())) {
      FastDialog.errorSnackbar('Unable to obtain user information.')
      userStore.signOut(true)
      router.push({ name: 'Error500' })
      return
    }
  }

  // 启动计时器
  userStore.startUpdateUserTimer()

  // 路由钩子（前置）
  router.beforeEach(async (to, _) => {
    if (to.meta?.requireSigned && to.name !== 'Go' && !userStore.state.signed) {
      return { name: 'Go' }
    }
  })

  // 路由钩子（后置）
  router.afterEach(async (to, _) => {
    if (!serverConfigsStore.state.customerServiceConfig?.customerServiceEnabled) {
      return
    }
    if (to.name == 'Home' && window.$chatwoot?.hasLoaded) {
      window.$chatwoot.toggleBubbleVisibility('show')
    } else {
      window.$chatwoot.toggleBubbleVisibility('hide')
    }
  })

  // 取消加载状态
  appLoading.value = false
}
</script>
