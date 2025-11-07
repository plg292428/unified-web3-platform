<template>
  <v-app-bar
    v-if="!route.meta?.errorPage && route.name !== 'Go' && walletState.active"
    density="comfortable"
    color="#0a090d"
    style="border-bottom: 1px solid rgba(255, 255, 255, 0.08)"
  >
    <template v-slot:prepend v-if="route.name !== 'Home'">
      <v-btn density="default" icon="mdi-chevron-left " size="small" @click="back"></v-btn>
    </template>

    <v-spacer></v-spacer>

    <v-chip
      v-if="walletState.active && serverConfigsStore.currentChainNetworkConfig"
      label
      :color="serverConfigsStore.currentChainNetworkConfig.color ?? '#ffffff'"
      variant="tonal"
      class="mr-4"
    >
      {{ abbrAddress }}
      <v-avatar end size="24">
        <v-img
          :src="`${$baseUrl}${serverConfigsStore.currentChainNetworkConfig.chainIconPath}`"
          alt="network"
          max-height="24"
        ></v-img>
      </v-avatar>
    </v-chip>

    <v-badge v-if="route.name !== 'SystemMessages' && route.name !== 'SystemMessageDetails'" dot color="error" class="mr-4" :model-value="userStore.state?.userInfo?.newSystemMessage">
      <v-btn color="white" variant="text" icon="mdi-email" size="small" @click="toSystemMessage"></v-btn>
    </v-badge>
  </v-app-bar>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useWalletStore } from '@/store/wallet'
import { useServerConfigsStore } from '@/store/severConfigs'
import { storeToRefs } from 'pinia'
import { useUserStore } from '@/store/user'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const serverConfigsStore = useServerConfigsStore()
const walletStore = useWalletStore()
const { state: walletState } = storeToRefs(walletStore)

const abbrAddress = computed(() =>
  walletState.value.active == false
    ? 'none'
    : `${walletState.value.address?.substring(0, 3)}...${walletState.value.address?.substring(
        walletState.value.address.length - 3,
        walletState.value.address.length
      )}`
)

// 返回
async function back() {
  const back = window.history.state?.back
  if (back) {
    router.go(-1)
    return
  }
  router.push({ name: 'Home' })
}

//
async function toSystemMessage() {
  router.push({ name: 'SystemMessages' })
}
</script>
