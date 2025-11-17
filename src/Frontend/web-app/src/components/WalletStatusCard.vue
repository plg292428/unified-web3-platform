<template>
  <v-card class="primary-border mt-4" variant="outlined">
    <v-card-title class="text-subtitle-2 d-flex align-center">
      Wallet Status
      <v-chip
        class="ml-2"
        size="x-small"
        :color="walletProviderColor"
        variant="tonal"
        label
      >
        {{ walletProviderLabel }}
      </v-chip>
      <v-spacer></v-spacer>
      <v-btn
        v-if="!walletActive"
        size="small"
        color="primary"
        @click="handleConnect"
        :loading="connectLoading"
      >
        Connect Wallet
      </v-btn>
      <v-btn
        icon="mdi-refresh"
        variant="text"
        size="small"
        :loading="rpcLoading"
        @click="refreshRpc"
        :title="'Refresh RPC Status'"
      ></v-btn>
    </v-card-title>
    <v-card-text class="text-caption text-grey-lighten-2">
      <v-row dense>
        <v-col cols="12" md="4">
          <div class="text-overline text-grey-lighten-1">Address</div>
          <div class="text-body-2">{{ displayAddress }}</div>
        </v-col>
        <v-col cols="12" md="4">
          <div class="text-overline text-grey-lighten-1">Network</div>
          <div class="text-body-2">{{ displayNetwork }}</div>
        </v-col>
        <v-col cols="12" md="4">
          <div class="text-overline text-grey-lighten-1">Chain ID</div>
          <div class="text-body-2">{{ chainIdText }}</div>
        </v-col>
      </v-row>

      <v-alert
        v-if="providerTip"
        :type="providerTip.type"
        variant="tonal"
        class="mt-3"
        density="compact"
      >
        <template #text>
          <div class="d-flex align-center">
            <span>{{ providerTip.message }}</span>
            <v-spacer></v-spacer>
            <v-btn
              v-if="providerTip.link"
              :href="providerTip.link"
              target="_blank"
              rel="noopener"
              size="x-small"
              color="primary"
              variant="text"
            >
              {{ providerTip.linkLabel ?? 'Go' }}
            </v-btn>
          </div>
        </template>
      </v-alert>

      <v-divider class="my-4" v-if="rpcStatuses.length"></v-divider>

      <div v-if="rpcStatuses.length">
        <div class="text-overline text-grey-lighten-1 mb-2">RPC Latency</div>
        <v-chip
          v-for="status in rpcStatuses"
          :key="status.name"
          class="mr-2 mb-2"
          :color="status.status === 'ok' ? 'success' : status.status === 'fail' ? 'error' : 'grey'"
          size="small"
          variant="outlined"
          label
        >
          {{ status.name }} · {{ formatLatency(status) }}
        </v-chip>
      </div>
      <div v-else class="text-grey text-body-2">Click the refresh button in the top right corner to detect RPC latency.</div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue'
import { useWalletStore } from '@/store/wallet'
import { storeToRefs } from 'pinia'
import type { RpcStatus } from '@/types'

const walletStore = useWalletStore()
const { state } = storeToRefs(walletStore)

const rpcLoading = ref(false)
const connectLoading = ref(false)

const walletProviderLabel = computed(() => {
  if (!state.value.initialized) {
    return 'Not Detected'
  }
  if (!state.value.providerName) {
    return 'No Wallet Detected'
  }
  return state.value.providerName
})

const walletProviderColor = computed(() => {
  if (!state.value.providerName) {
    return 'grey-darken-1'
  }
  return 'primary'
})

const walletActive = computed(() => state.value.active && !!state.value.address)

const displayAddress = computed(() => {
  if (!state.value.address) {
    return 'Not Connected'
  }
  return shortenAddress(state.value.address)
})

const displayNetwork = computed(() => {
  if (!state.value.active || !state.value.networkName) {
    return 'Not Connected'
  }
  return state.value.networkName
})

const chainIdText = computed(() => {
  if (!state.value.active || !state.value.chainId || state.value.chainId < 0) {
    return '-'
  }
  return state.value.chainId
})

const rpcStatuses = computed(() => state.value.rpcStatuses ?? [])

const providerTip = computed(() => {
  if (!state.value.initialized) {
    return null
  }
  if (!state.value.providerName) {
    return {
      type: 'warning' as const,
      message: 'No browser wallet detected. Please install Bitget Wallet extension for full experience.',
      link: 'https://web3.bitget.com/en/tools/wallet',
      linkLabel: 'Download Bitget Wallet'
    }
  }
  if (state.value.providerType !== 'bitget') {
    return {
      type: 'info' as const,
      message: `${state.value.providerName} detected. We recommend using Bitget Wallet to support more chains and features.`,
      link: 'https://web3.bitget.com/en/tools/wallet',
      linkLabel: 'Learn About Bitget Wallet'
    }
  }
  return null
})

const refreshRpc = async () => {
  rpcLoading.value = true
  try {
    await walletStore.refreshRpcStatuses()
  } finally {
    rpcLoading.value = false
  }
}

const handleConnect = async () => {
  connectLoading.value = true
  try {
    await walletStore.connect()
  } catch (error) {
    console.error(error)
  } finally {
    connectLoading.value = false
  }
}

const shortenAddress = (address: string, left = 6, right = 4) => {
  if (!address) {
    return ''
  }
  if (address.length <= left + right + 3) {
    return address
  }
  return `${address.substring(0, left)}...${address.substring(address.length - right)}`
}

const formatLatency = (status: RpcStatus) => {
  if (status.status === 'ok' && typeof status.latency === 'number') {
    return `${status.latency} ms`
  }
  if (status.status === 'fail') {
    return 'Failed'
  }
  return 'Pending'
}

onMounted(async () => {
  await walletStore.initialize()
  await refreshRpc()
})
</script>

