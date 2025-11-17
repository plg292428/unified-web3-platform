<template>
  <v-card variant="outlined" class="mb-4">
    <v-card-title class="text-subtitle-1">Select Payment Method</v-card-title>
    <v-card-text>
      <v-radio-group v-model="selectedMethod" @update:model-value="handleMethodChange">
        <!-- Web3 支付选项 -->
        <v-radio
          :value="StorePaymentMode.Web3"
          :disabled="!isWeb3Available"
        >
          <template #label>
            <div class="d-flex align-center">
              <v-icon class="mr-2" color="primary">mdi-wallet</v-icon>
              <div>
                <div class="font-weight-medium">Web3 Payment</div>
                <div class="text-caption text-grey-lighten-1">
                  Pay with cryptocurrency wallet
                </div>
                <div v-if="!isWeb3Available" class="text-caption text-error mt-1">
                  Please connect your wallet first
                </div>
              </div>
            </div>
          </template>
        </v-radio>

        <!-- 传统支付选项 -->
        <v-radio :value="StorePaymentMode.Traditional">
          <template #label>
            <div class="d-flex align-center">
              <v-icon class="mr-2" color="success">mdi-credit-card</v-icon>
              <div>
                <div class="font-weight-medium">Traditional Payment</div>
                <div class="text-caption text-grey-lighten-1">
                  Pay with traditional payment methods
                </div>
              </div>
            </div>
          </template>
        </v-radio>
      </v-radio-group>

      <!-- Web3 支付详情 -->
      <v-expand-transition>
        <div v-if="selectedMethod === StorePaymentMode.Web3 && isWeb3Available" class="mt-3 pa-3 bg-grey-darken-4 rounded">
          <div class="text-caption text-grey-lighten-1 mb-2">Wallet Information</div>
          <div class="text-body-2">
            <div><strong>Provider:</strong> {{ walletState.providerName ?? walletState.providerType ?? 'Unknown' }}</div>
            <div class="mt-1"><strong>Address:</strong> {{ formatAddress(walletState.address) }}</div>
            <div v-if="walletState.chainId > 0" class="mt-1">
              <strong>Network:</strong> Chain ID {{ walletState.chainId }}
            </div>
          </div>
        </div>
      </v-expand-transition>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useWalletStore } from '@/store/wallet'
import { StorePaymentMode } from '@/types'

const props = defineProps<{
  modelValue: StorePaymentMode
  disabled?: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: StorePaymentMode]
}>()

const walletStore = useWalletStore()
const selectedMethod = ref<StorePaymentMode>(props.modelValue)

const walletState = computed(() => walletStore.state)
const isWeb3Available = computed(() => 
  walletState.value.active && 
  walletState.value.provider !== null && 
  walletState.value.address !== null
)

// 如果Web3不可用，自动切换到传统支付
watch(isWeb3Available, (available) => {
  if (!available && selectedMethod.value === StorePaymentMode.Web3) {
    selectedMethod.value = StorePaymentMode.Traditional
    emit('update:modelValue', StorePaymentMode.Traditional)
  }
})

// 监听外部值变化
watch(() => props.modelValue, (newValue) => {
  if (newValue !== selectedMethod.value) {
    selectedMethod.value = newValue
  }
})

// 监听选择变化
watch(selectedMethod, (newValue) => {
  emit('update:modelValue', newValue)
})

// 加载保存的支付偏好或根据钱包状态自动选择
onMounted(() => {
  const savedMethod = localStorage.getItem('preferredPaymentMethod')
  if (savedMethod) {
    const method = parseInt(savedMethod) as StorePaymentMode
    if (method === StorePaymentMode.Web3 && isWeb3Available.value) {
      selectedMethod.value = method
      emit('update:modelValue', method)
      return
    } else if (method === StorePaymentMode.Traditional) {
      selectedMethod.value = method
      emit('update:modelValue', method)
      return
    }
  }
  
  // 如果没有保存的偏好，根据钱包状态自动选择
  if (isWeb3Available.value) {
    selectedMethod.value = StorePaymentMode.Web3
    emit('update:modelValue', StorePaymentMode.Web3)
  } else {
    selectedMethod.value = StorePaymentMode.Traditional
    emit('update:modelValue', StorePaymentMode.Traditional)
  }
})

function handleMethodChange(value: StorePaymentMode) {
  // 保存支付偏好
  localStorage.setItem('preferredPaymentMethod', value.toString())
}

function formatAddress(address: string | null | undefined): string {
  if (!address || address.length < 10) return 'Not connected'
  return `${address.substring(0, 6)}...${address.substring(address.length - 4)}`
}
</script>

