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
import WebApi from './libs/WebApi'

const route = useRoute()
const router = useRouter()

const walletStore = useWalletStore()
const userStore = useUserStore()
const serverConfigsStore = useServerConfigsStore()

const appLoading = ref(true)
let routerGuardsInitialized = false
let loadingTimeout: ReturnType<typeof setTimeout> | null = null

onBeforeMount(async () => {
  // 添加超时保护，确保页面不会一直加载
  loadingTimeout = setTimeout(() => {
    if (appLoading.value) {
      console.warn('加载超时，强制进入访客模式')
      appLoading.value = false
      router.push({ name: 'Home' })
    }
  }, 10000) // 10秒超时

  try {
    // 等待路由就绪
    await router.isReady()

    // 添加调试日志
    console.log('App初始化开始，当前路由:', route.name, '路径:', route.path)

  // 推荐令牌
  const invitationLinkToken = route.query.inviter?.toString() ?? null
  if (invitationLinkToken) {
    localStorage.setItem('invitationLinkToken', invitationLinkToken)
  }

  // 处于错误页面时先返回首页（但如果是 Error500，稍后会在 guestReady 中处理）
  if (route.meta?.errorPage && route.name !== 'Error404' && route.name !== 'Error500') {
    router.push({ name: 'Go' })
  }
  
  // 如果访问 no-wallet-detected 路径，立即使用 window.location 强制跳转到首页
  if (route.path === '/error/no-wallet-detected' || route.path.includes('no-wallet-detected') || window.location.pathname.includes('no-wallet-detected')) {
    console.log('检测到 no-wallet-detected 路径，立即强制跳转到首页')
    // 立即使用 window.location 强制跳转，确保不加载任何组件
    window.location.replace(window.location.origin)
    return
  }

  // 获取服务器区块链配置
  const configResult = await serverConfigsStore.getChainNetworkConfigs()
  if (!configResult.success) {
    // 获取配置失败时，进入访客模式（允许浏览，但功能受限）
    console.warn('获取区块链配置失败，进入访客模式')
    console.warn('错误信息:', configResult.errorMessage)
    await guestReady()
    return
  }

  // 初始化钱包提供方
  await walletStore.initialize()
  console.log('钱包初始化完成，provider:', walletStore.state.provider)

  // 未检测到钱包，进入访客模式
  if (walletStore.state.provider === null) {
    console.log('未检测到钱包，进入访客模式')
    await guestReady()
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
    .catch(async (error: any) => {
      if (error.code == -32002) {
        FastDialog.errorSnackbar('Unable to obtain wallet information, please connect to the wallet first.')
      } else if (error.code == 4001) {
        FastDialog.errorSnackbar('Unable to obtain wallet information, please connect to the wallet first.')
      }
      // 钱包连接失败，进入访客模式
      await guestReady()
      return
    })
  } catch (error) {
    console.error('应用初始化出错:', error)
    // 出错时也进入访客模式
    await guestReady()
  } finally {
    if (loadingTimeout) {
      clearTimeout(loadingTimeout)
      loadingTimeout = null
    }
  }
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

  // 获取全局配置（失败时仅警告，不阻止应用运行）
  if (!(await serverConfigsStore.getGlobalConfigs())) {
    console.warn('获取全局配置失败，但不影响基本功能')
    // 不跳转到错误页面，允许继续使用
  }

  // 获取服务器代币配置集（需要当前链配置，失败时仅警告）
  if (serverConfigsStore.currentChainNetworkConfig) {
    if (!(await serverConfigsStore.getChainTokenConfigs())) {
      console.warn('获取代币配置失败，但不影响基本功能')
      // 不跳转到错误页面，允许继续使用
    }
  } else {
    console.warn('未检测到当前链配置，跳过代币配置获取')
  }

  // 获取服务器用户等级配置（失败时仅警告，不阻止应用运行）
  if (!(await serverConfigsStore.getUserLevelConfigs())) {
    console.warn('获取用户等级配置失败，但不影响基本功能')
    // 不跳转到错误页面，允许继续使用
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

  setupRouteGuards()

  // 取消加载状态
  appLoading.value = false
}

async function guestReady() {
  try {
    console.log('开始进入访客模式...')
    // 访客模式仅加载必要基础配置
    // 静默处理错误，不显示在控制台（后端服务未运行时是正常的）
    await serverConfigsStore.getGlobalConfigs().catch(() => {
      // 静默处理，不输出错误
    })
    console.log('全局配置加载完成')
    setupRouteGuards()
    console.log('路由守卫设置完成')
    
    // 确保路由正确（访客模式应该显示 Home 页面）
    await router.isReady()
    console.log('路由就绪，当前路由:', route.name)
    // 如果当前在错误页面或登录页面，跳转到首页
    if (route.meta?.errorPage || route.name === 'Go' || route.path.includes('no-wallet-detected') || window.location.pathname.includes('no-wallet-detected')) {
      console.log('跳转到首页...')
      // 如果路径包含 no-wallet-detected，使用 window.location 强制跳转
      if (route.path.includes('no-wallet-detected') || window.location.pathname.includes('no-wallet-detected')) {
        window.location.replace(window.location.origin)
        return
      }
      try {
        await router.push({ name: 'Home' })
      } catch (error) {
        console.error('路由跳转失败，使用 window.location 强制跳转:', error)
        window.location.replace(window.location.origin)
      }
    }
    
    appLoading.value = false
    console.log('访客模式已就绪，可以浏览商品（只读模式）')
    console.log('最终路由:', route.name)
  } catch (error) {
    console.error('访客模式初始化出错:', error)
    // 即使出错也显示页面
    appLoading.value = false
    await router.push({ name: 'Home' })
  } finally {
    if (loadingTimeout) {
      clearTimeout(loadingTimeout)
      loadingTimeout = null
    }
  }
}

function setupRouteGuards() {
  if (routerGuardsInitialized) {
    return
  }
  router.beforeEach(async (to, _) => {
    if (to.meta?.requireSigned && to.name !== 'Go' && !userStore.state.signed) {
      return { name: 'Go' }
    }
  })

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

  routerGuardsInitialized = true
}
</script>
