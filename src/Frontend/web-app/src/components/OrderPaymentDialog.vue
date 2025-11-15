<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card>
      <v-card-title class="d-flex align-center">
        <v-icon class="mr-2">mdi-wallet</v-icon>
        Web3 Payment
        <v-spacer></v-spacer>
        <v-btn icon="mdi-close" variant="text" size="small" @click="handleClose"></v-btn>
      </v-card-title>

      <v-card-text v-if="order">
        <!-- 订单信息 -->
        <div class="mb-4">
          <div class="text-caption text-grey-lighten-1 mb-1">Order Number</div>
          <div class="text-body-2 font-weight-medium">{{ order.orderNumber }}</div>
        </div>

        <div class="mb-4">
          <div class="text-caption text-grey-lighten-1 mb-1">Payment Amount</div>
          <div class="text-h6 text-primary">
            {{ Filter.formatToken(order.totalAmount) }} {{ order.currency }}
          </div>
        </div>

        <!-- 支付状态 -->
        <v-alert
          v-if="paymentStatus"
          :type="getPaymentStatusType(paymentStatus.paymentStatus)"
          variant="tonal"
          class="mb-4"
        >
          <div class="text-caption">{{ getPaymentStatusText(paymentStatus.paymentStatus) }}</div>
          <div v-if="paymentStatus.isExpired" class="text-caption mt-1">
            Payment expired
          </div>
          <div v-else-if="paymentStatus.remainingSeconds > 0" class="text-caption mt-1">
            Remaining time: {{ formatRemainingTime(paymentStatus.remainingSeconds) }}
          </div>
        </v-alert>

        <!-- 支付步骤 -->
        <v-stepper v-model="currentStep" class="mb-4" variant="vertical">
          <v-stepper-item
            :complete="currentStep > 1"
            :title="'Prepare Payment Signature'"
            :value="1"
          >
            <div class="text-caption text-grey-lighten-1">
              Generate payment signature payload for wallet signing
            </div>
          </v-stepper-item>

          <v-stepper-item
            :complete="currentStep > 2"
            :title="'Wallet Signature'"
            :value="2"
          >
            <div class="text-caption text-grey-lighten-1">
              Please confirm signature in wallet
            </div>
          </v-stepper-item>

          <v-stepper-item
            :complete="currentStep > 3"
            :title="'Awaiting On-Chain Confirmation'"
            :value="3"
          >
            <div class="text-caption text-grey-lighten-1">
              Waiting for blockchain confirmation
            </div>
            <div v-if="paymentStatus?.paymentConfirmations !== undefined && paymentStatus.paymentConfirmations > 0" class="text-caption text-primary mt-1">
              Confirmations: {{ paymentStatus.paymentConfirmations }} / 12
            </div>
            <v-progress-linear
              v-if="paymentStatus?.paymentConfirmations !== undefined"
              :model-value="Math.min((paymentStatus.paymentConfirmations / 12) * 100, 100)"
              color="primary"
              height="4"
              class="mt-2"
            ></v-progress-linear>
          </v-stepper-item>

          <v-stepper-item
            :complete="paymentStatus?.paymentStatus === StorePaymentStatus.Confirmed"
            :title="'Payment Completed'"
            :value="4"
          >
            <div class="text-caption text-grey-lighten-1">
              Payment confirmed
            </div>
          </v-stepper-item>
        </v-stepper>

        <!-- 交易哈希 -->
        <div v-if="paymentStatus?.paymentTransactionHash" class="mb-4">
          <div class="text-caption text-grey-lighten-1 mb-1">Transaction Hash</div>
          <div class="text-body-2 font-mono">{{ paymentStatus.paymentTransactionHash }}</div>
        </div>

        <!-- 错误信息 -->
        <v-alert
          v-if="errorMessage"
          type="error"
          variant="tonal"
          class="mb-4"
        >
          {{ errorMessage }}
        </v-alert>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
          v-if="currentStep === 1"
          color="primary"
          :loading="processing"
          @click="handlePreparePayment"
        >
          Prepare Payment
        </v-btn>
        <v-btn
          v-else-if="currentStep === 2"
          color="primary"
          :loading="processing"
          @click="handleSignPayment"
        >
          Sign Payment
        </v-btn>
        <v-btn
          v-else-if="paymentStatus?.paymentStatus === StorePaymentStatus.Confirmed"
          color="success"
          @click="handleClose"
        >
          Complete
        </v-btn>
        <v-btn
          v-else-if="paymentStatus?.paymentStatus === StorePaymentStatus.Cancelled || paymentStatus?.isExpired"
          color="error"
          @click="handleClose"
        >
          Close
        </v-btn>
        <v-btn
          v-else
          variant="text"
          @click="handleClose"
        >
          Cancel
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { computed, ref, watch } from 'vue'
import { useCartStore } from '@/store/cart'
import { useWalletStore } from '@/store/wallet'
import { useUserStore } from '@/store/user'
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import { confirmWeb3Payment } from '@/services/storeApi'
import type {
  StoreOrderDetailResult,
  StoreOrderPreparePaymentResult,
  StoreOrderPaymentStatusResult,
  StorePaymentStatus,
  StoreOrderWeb3ConfirmPayload
} from '@/types'
import { StorePaymentStatus as PaymentStatusEnum } from '@/types'

const props = defineProps<{
  modelValue: boolean
  order: StoreOrderDetailResult | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'payment-success': [order: StoreOrderDetailResult]
}>()

const cartStore = useCartStore()
const walletStore = useWalletStore()
const userStore = useUserStore()

const dialog = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const currentStep = ref(1)
const processing = ref(false)
const errorMessage = ref<string | null>(null)
const paymentPrepareResult = ref<StoreOrderPreparePaymentResult | null>(null)
const paymentStatus = ref<StoreOrderPaymentStatusResult | null>(null)
const paymentStatusInterval = ref<ReturnType<typeof setTimeout> | null>(null)

const StorePaymentStatus = PaymentStatusEnum

// 监听对话框打开
watch(dialog, (newValue) => {
  if (newValue && props.order) {
    resetState()
    if (props.order.paymentStatus === StorePaymentStatus.PendingSignature) {
      currentStep.value = 1
    } else if (props.order.paymentStatus === StorePaymentStatus.AwaitingOnChainConfirmation) {
      currentStep.value = 3
      startPaymentStatusPolling()
    } else if (props.order.paymentStatus === StorePaymentStatus.Confirmed) {
      currentStep.value = 4
    }
  } else {
    stopPaymentStatusPolling()
  }
})

function resetState() {
  currentStep.value = 1
  processing.value = false
  errorMessage.value = null
  paymentPrepareResult.value = null
  paymentStatus.value = null
  stopPaymentStatusPolling()
}

async function handlePreparePayment() {
  if (!props.order || !userStore.state.userInfo) {
    return
  }

  processing.value = true
  errorMessage.value = null

  try {
    const result = await cartStore.prepareOrderPayment(props.order.orderId, {
      uid: userStore.state.userInfo.uid,
      paymentWalletAddress: walletStore.state.address ?? undefined,
      paymentProviderType: walletStore.state.providerType ?? undefined,
      paymentProviderName: walletStore.state.providerName ?? undefined,
      chainId: walletStore.state.chainId > 0 ? walletStore.state.chainId : undefined
    })

    paymentPrepareResult.value = result
    currentStep.value = 2
  } catch (error) {
    errorMessage.value = (error as Error).message ?? 'Failed to prepare payment'
    FastDialog.errorSnackbar(errorMessage.value)
  } finally {
    processing.value = false
  }
}

async function handleSignPayment() {
  if (!paymentPrepareResult.value || !walletStore.state.provider || !walletStore.state.address) {
    FastDialog.errorSnackbar('Wallet not connected')
    return
  }

  processing.value = true
  errorMessage.value = null

  try {
    // 使用钱包签名
    const signature = await walletStore.state.provider.request({
      method: 'personal_sign',
      params: [paymentPrepareResult.value.paymentSignaturePayload, walletStore.state.address]
    })

    // 确认 Web3 支付，提交签名
    const confirmPayload: StoreOrderWeb3ConfirmPayload = {
      uid: userStore.state.userInfo!.uid,
      paymentStatus: StorePaymentStatus.AwaitingOnChainConfirmation,
      paymentSignatureResult: signature
    }
    
    await confirmWeb3Payment(props.order.orderId, confirmPayload)

    currentStep.value = 3
    startPaymentStatusPolling()
  } catch (error: any) {
    if (error.code === 4001) {
      errorMessage.value = 'User cancelled signature'
    } else {
      errorMessage.value = (error as Error).message ?? 'Signature failed'
    }
    FastDialog.errorSnackbar(errorMessage.value)
  } finally {
    processing.value = false
  }
}

// 智能轮询配置
let pollingInterval = 2000 // 初始间隔 2 秒
let pollingAttempts = 0 // 轮询次数
const maxPollingInterval = 10000 // 最大间隔 10 秒
const minPollingInterval = 2000 // 最小间隔 2 秒
const backoffMultiplier = 1.5 // 退避倍数

function startPaymentStatusPolling() {
  if (!props.order || !userStore.state.userInfo) {
    return
  }

  stopPaymentStatusPolling()

  // 重置轮询配置
  pollingInterval = minPollingInterval
  pollingAttempts = 0

  // 立即查询一次
  checkPaymentStatus()

  // 开始智能轮询
  scheduleNextPoll()
}

function scheduleNextPoll() {
  if (!props.order || !userStore.state.userInfo) {
    return
  }

  // 根据支付状态动态调整轮询间隔
  if (paymentStatus.value) {
    const status = paymentStatus.value.paymentStatus
    
    // 如果支付已完成或失败，停止轮询
    if (status === StorePaymentStatus.Confirmed || 
        status === StorePaymentStatus.Failed || 
        status === StorePaymentStatus.Cancelled ||
        paymentStatus.value.isExpired) {
      stopPaymentStatusPolling()
      return
    }

    // 如果正在等待链上确认，根据确认数调整间隔
    if (status === StorePaymentStatus.AwaitingOnChainConfirmation) {
      const confirmations = paymentStatus.value.paymentConfirmations ?? 0
      
      // 确认数越多，轮询间隔越长（交易更稳定）
      if (confirmations === 0) {
        pollingInterval = 2000 // 刚开始，频繁轮询
      } else if (confirmations < 3) {
        pollingInterval = 3000 // 1-2个确认，3秒轮询
      } else if (confirmations < 6) {
        pollingInterval = 5000 // 3-5个确认，5秒轮询
      } else {
        pollingInterval = 8000 // 6+个确认，8秒轮询
      }
    } else {
      // 其他状态使用指数退避
      pollingInterval = Math.min(
        pollingInterval * backoffMultiplier,
        maxPollingInterval
      )
    }
  } else {
    // 没有状态信息，使用指数退避
    pollingInterval = Math.min(
      pollingInterval * backoffMultiplier,
      maxPollingInterval
    )
  }

  pollingAttempts++

  // 设置下一次轮询
  paymentStatusInterval.value = window.setTimeout(() => {
    checkPaymentStatus()
    scheduleNextPoll() // 递归调度下一次
  }, pollingInterval)
}

async function checkPaymentStatus() {
  if (!props.order || !userStore.state.userInfo) {
    return
  }

  try {
    const status = await cartStore.checkPaymentStatus(props.order.orderId, userStore.state.userInfo.uid)
    const previousStatus = paymentStatus.value
    paymentStatus.value = status

    // 状态变化通知
    if (previousStatus && previousStatus.paymentStatus !== status.paymentStatus) {
      console.log(`Payment status changed: ${previousStatus.paymentStatus} -> ${status.paymentStatus}`)
    }

    // 处理不同支付状态
    if (status.paymentStatus === StorePaymentStatus.Confirmed) {
      currentStep.value = 4
      stopPaymentStatusPolling()
      emit('payment-success', props.order)
      FastDialog.successSnackbar('Payment successful')
    } else if (status.paymentStatus === StorePaymentStatus.Failed) {
      stopPaymentStatusPolling()
      FastDialog.errorSnackbar(status.paymentFailureReason ?? 'Payment failed')
    } else if (status.isExpired) {
      stopPaymentStatusPolling()
      FastDialog.errorSnackbar('Payment expired, order automatically cancelled')
    } else if (status.paymentStatus === StorePaymentStatus.AwaitingOnChainConfirmation) {
      currentStep.value = 3
      // 继续轮询，间隔会根据确认数动态调整
    } else if (status.paymentStatus === StorePaymentStatus.Cancelled) {
      stopPaymentStatusPolling()
      FastDialog.warningSnackbar('Payment cancelled')
    }
  } catch (error) {
    console.warn('Failed to check payment status:', error)
    // 错误时不停止轮询，继续尝试
  }
}

function stopPaymentStatusPolling() {
  if (paymentStatusInterval.value !== null) {
    clearTimeout(paymentStatusInterval.value) // 使用 clearTimeout 因为现在是 setTimeout
    paymentStatusInterval.value = null
  }
  // 重置轮询配置
  pollingInterval = minPollingInterval
  pollingAttempts = 0
}

function handleClose() {
  stopPaymentStatusPolling()
  dialog.value = false
}

function getPaymentStatusType(status: StorePaymentStatus): 'success' | 'warning' | 'error' | 'info' {
  switch (status) {
    case StorePaymentStatus.Confirmed:
      return 'success'
    case StorePaymentStatus.PendingSignature:
    case StorePaymentStatus.AwaitingOnChainConfirmation:
      return 'info'
    case StorePaymentStatus.Failed:
    case StorePaymentStatus.Cancelled:
      return 'error'
    default:
      return 'warning'
  }
}

function getPaymentStatusText(status: StorePaymentStatus): string {
  switch (status) {
    case StorePaymentStatus.PendingSignature:
      return 'Pending Signature'
    case StorePaymentStatus.AwaitingOnChainConfirmation:
      return 'Awaiting On-Chain Confirmation'
    case StorePaymentStatus.Confirmed:
      return 'Payment Confirmed'
    case StorePaymentStatus.Failed:
      return 'Payment Failed'
    case StorePaymentStatus.Cancelled:
      return 'Payment Cancelled'
    default:
      return 'Unknown Status'
  }
}

function formatRemainingTime(seconds: number): string {
  const minutes = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${minutes}:${secs.toString().padStart(2, '0')}`
}
</script>

