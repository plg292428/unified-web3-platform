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
          <div class="font-weight-medium mb-1">{{ getPaymentStatusText(paymentStatus.paymentStatus) }}</div>
          <div v-if="paymentStatus.isExpired" class="text-caption mt-1 text-error">
            ⚠️ Payment expired. Order has been cancelled.
          </div>
          <div v-else-if="paymentStatus.remainingSeconds > 0" class="text-caption mt-1">
            ⏱️ Remaining time: <strong>{{ formatRemainingTime(paymentStatus.remainingSeconds) }}</strong>
          </div>
          <div v-if="paymentStatus.paymentConfirmations !== undefined && paymentStatus.paymentConfirmations > 0" class="text-caption mt-1">
            ✅ Confirmations: {{ paymentStatus.paymentConfirmations }} / 12
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
          closable
          @click:close="errorMessage = null"
        >
          <div class="font-weight-medium mb-1">Payment Error</div>
          <div>{{ errorMessage }}</div>
          <div v-if="retryCount > 0" class="text-caption mt-2">
            Retry attempts: {{ retryCount }} / {{ maxRetries }}
          </div>
        </v-alert>
        
        <!-- 重试提示 -->
        <v-alert
          v-if="paymentStatus?.paymentStatus === StorePaymentStatus.Failed && retryCount < maxRetries"
          type="warning"
          variant="tonal"
          class="mb-4"
        >
          <div class="text-caption">
            Payment failed. You can retry the payment. ({{ maxRetries - retryCount }} attempts remaining)
          </div>
        </v-alert>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <!-- 重试按钮（支付失败时显示） -->
        <v-btn
          v-if="paymentStatus?.paymentStatus === StorePaymentStatus.Failed && currentStep === 2"
          color="warning"
          variant="outlined"
          :loading="processing"
          @click="handleRetryPayment"
          class="mr-2"
        >
          <v-icon start>mdi-refresh</v-icon>
          Retry Payment
        </v-btn>
        <v-btn
          v-if="currentStep === 1 && !processing"
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
          <v-icon start>mdi-wallet</v-icon>
          Sign & Pay
        </v-btn>
        <v-btn
          v-else-if="paymentStatus?.paymentStatus === StorePaymentStatus.Confirmed"
          color="success"
          @click="handleClose"
        >
          <v-icon start>mdi-check-circle</v-icon>
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
          v-else-if="currentStep === 3"
          variant="text"
          :disabled="processing"
          @click="handleClose"
        >
          Close (Payment in progress)
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
// 动态导入 ethers，避免在非 Web3 支付时加载
// import { BrowserProvider, Contract, parseEther, parseUnits } from 'ethers'

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
const retryCount = ref(0)
const maxRetries = 3

const StorePaymentStatus = PaymentStatusEnum

// 监听对话框打开
watch(dialog, (newValue) => {
  if (newValue && props.order) {
    resetState()
    if (props.order.paymentStatus === StorePaymentStatus.PendingSignature) {
      currentStep.value = 1
      // 自动开始准备支付
      setTimeout(() => {
        handlePreparePayment()
      }, 300)
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
  retryCount.value = 0
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
    // 解析支付签名原文（JSON格式）
    const payload = JSON.parse(paymentPrepareResult.value.paymentSignaturePayload)
    const paymentAddress = payload.paymentAddress
    const amount = payload.amount
    const tokenContractAddress = payload.tokenContractAddress
    const chainId = payload.chainId

    // 检查链ID是否匹配
    if (walletStore.state.chainId !== chainId) {
      errorMessage.value = `Please switch to the correct network (Chain ID: ${chainId})`
      FastDialog.errorSnackbar(errorMessage.value)
      return
    }

    let transactionHash: string | null = null

    // 如果有代币合约地址，使用代币转账；否则使用原生币转账
    if (tokenContractAddress) {
      // ERC20 代币转账
      const { BrowserProvider, Contract, parseUnits } = await import('ethers')
      const provider = new BrowserProvider(walletStore.state.provider)
      const signer = await provider.getSigner()
      
      // 获取代币信息（从支付结果中获取）
      const tokenSymbol = paymentPrepareResult.value.tokenSymbol || 'TOKEN'
      // 从支持的代币列表中查找对应的decimals
      const supportedToken = paymentPrepareResult.value.supportedTokens?.find(
        t => t.tokenContractAddress?.toLowerCase() === tokenContractAddress.toLowerCase()
      )
      const decimals = supportedToken?.decimals ?? 18 // 默认18位
      
      // ERC20 Transfer ABI
      const erc20Abi = [
        'function transfer(address to, uint256 amount) returns (bool)'
      ]
      
      const tokenContract = new Contract(tokenContractAddress, erc20Abi, signer)
      const amountWei = parseUnits(amount.toString(), decimals)
      
      // 发送代币转账交易
      const tx = await tokenContract.transfer(paymentAddress, amountWei)
      transactionHash = tx.hash
      
      FastDialog.successSnackbar(`Transaction sent: ${transactionHash.substring(0, 10)}...`)
    } else {
      // 原生币转账（ETH/MATIC等）
      const { BrowserProvider, parseEther } = await import('ethers')
      const provider = new BrowserProvider(walletStore.state.provider)
      const signer = await provider.getSigner()
      
      // 发送原生币转账
      const tx = await signer.sendTransaction({
        to: paymentAddress,
        value: parseEther(amount.toString())
      })
      transactionHash = tx.hash
      
      FastDialog.successSnackbar(`Transaction sent: ${transactionHash.substring(0, 10)}...`)
    }

    // 确认 Web3 支付，提交交易哈希和签名
    const confirmPayload: StoreOrderWeb3ConfirmPayload = {
      uid: userStore.state.userInfo!.uid,
      paymentTransactionHash: transactionHash,
      paymentStatus: StorePaymentStatus.AwaitingOnChainConfirmation,
      paymentSignatureResult: null // 签名主要用于验证，实际支付通过交易哈希确认
    }
    
    await confirmWeb3Payment(props.order.orderId, confirmPayload)

    currentStep.value = 3
    startPaymentStatusPolling()
  } catch (error: any) {
    console.error('Payment transaction error:', error)
    if (error.code === 4001) {
      errorMessage.value = 'User cancelled transaction'
    } else if (error.code === 'INSUFFICIENT_FUNDS' || error.message?.includes('insufficient funds')) {
      errorMessage.value = 'Insufficient balance for payment'
    } else if (error.code === 'NETWORK_ERROR' || error.message?.includes('network')) {
      errorMessage.value = 'Network error, please check your connection'
    } else {
      errorMessage.value = (error as Error).message ?? 'Transaction failed'
    }
    FastDialog.errorSnackbar(errorMessage.value)
    
    // 如果是可重试的错误，不重置步骤，允许重试
    if (error.code !== 4001 && retryCount.value < maxRetries) {
      // 保持在第2步，允许重试
    } else {
      // 用户取消或超过重试次数，重置到准备步骤
      currentStep.value = 1
      paymentPrepareResult.value = null
    }
  } finally {
    processing.value = false
  }
}

// 重试支付
async function handleRetryPayment() {
  if (retryCount.value >= maxRetries) {
    FastDialog.errorSnackbar(`Maximum retry attempts (${maxRetries}) reached. Please try again later.`)
    return
  }
  
  retryCount.value++
  errorMessage.value = null
  
  // 如果已经有准备结果，直接重试签名
  if (paymentPrepareResult.value) {
    await handleSignPayment()
  } else {
    // 否则重新准备支付
    await handlePreparePayment()
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
      currentStep.value = 1
      errorMessage.value = 'Payment expired. The order has been automatically cancelled. Please create a new order.'
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

