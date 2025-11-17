<template>
  <v-container>
    <v-responsive>
      <!-- Back Button -->
      <div class="mt-2 d-flex align-center">
        <v-btn icon="mdi-arrow-left" variant="text" @click="goBack"></v-btn>
        <span class="text-subtitle-2">Order Details</span>
        <v-spacer></v-spacer>
        <v-btn
          color="primary"
          variant="outlined"
          size="small"
          @click="refreshOrder"
          :loading="loading"
        >
          <v-icon start size="18">mdi-refresh</v-icon>
          Refresh
        </v-btn>
      </div>

      <!-- Loading -->
      <template v-if="loading && !order">
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text>
            <v-skeleton-loader type="article, table"></v-skeleton-loader>
          </v-card-text>
        </v-card>
      </template>

      <!-- Order Not Found -->
      <template v-else-if="!order">
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text class="text-center py-8">
            <v-img height="120" src="@/assets/no_data.svg" class="mx-auto"></v-img>
            <div class="text-h6 mt-4 text-grey-lighten-1">Order not found</div>
            <div class="text-body-2 text-grey-lighten-2 mt-2">
              The order you are looking for does not exist or has been removed
            </div>
            <v-btn color="primary" variant="tonal" class="mt-4" @click="goToOrders">
              <v-icon start>mdi-format-list-bulleted</v-icon>
              View All Orders
            </v-btn>
          </v-card-text>
        </v-card>
      </template>

      <!-- Order Details -->
      <template v-else>
        <!-- Order Basic Information -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="d-flex align-center">
            <span class="text-subtitle-1">Order Information</span>
            <v-spacer></v-spacer>
            <v-chip
              size="small"
              :color="getOrderStatusColor(order.status)"
              variant="tonal"
            >
              {{ getOrderStatusText(order.status) }}
            </v-chip>
          </v-card-title>
          <v-divider></v-divider>
          <v-card-text>
            <v-row dense>
              <v-col cols="12" md="6">
                <div class="text-caption text-grey-lighten-1">Order Number</div>
                <div class="text-body-2 font-weight-medium">{{ order.orderNumber }}</div>
              </v-col>
              <v-col cols="12" md="6">
                <div class="text-caption text-grey-lighten-1">Created At</div>
                <div class="text-body-2">{{ formatDateTime(order.createTime) }}</div>
              </v-col>
              <v-col cols="12" md="6">
                <div class="text-caption text-grey-lighten-1">Payment Method</div>
                <div class="text-body-2">
                  {{ order.paymentMode === StorePaymentMode.Web3 ? 'Web3 Payment' : 'Traditional Payment' }}
                </div>
              </v-col>
              <v-col cols="12" md="6">
                <div class="text-caption text-grey-lighten-1">Total Amount</div>
                <div class="text-body-2 font-weight-medium text-primary">
                  {{ formatAmount(order.totalAmount) }} {{ order.currency }}
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>

        <!-- Payment Information -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="text-subtitle-1">Payment Information</v-card-title>
          <v-divider></v-divider>
          <v-card-text>
            <v-row dense>
              <v-col cols="12" md="6">
                <div class="text-caption text-grey-lighten-1">Payment Status</div>
                <v-chip
                  size="small"
                  :color="getPaymentStatusColor(order.paymentStatus)"
                  variant="tonal"
                  class="mt-1"
                >
                  {{ getPaymentStatusText(order.paymentStatus) }}
                </v-chip>
              </v-col>
              <v-col v-if="order.paymentConfirmations" cols="12" md="6">
                <div class="text-caption text-grey-lighten-1">Confirmations</div>
                <div class="text-body-2">{{ order.paymentConfirmations }}</div>
              </v-col>
              <v-col v-if="order.paymentConfirmedTime" cols="12" md="6">
                <div class="text-caption text-grey-lighten-1">Confirmed At</div>
                <div class="text-body-2">{{ formatDateTime(order.paymentConfirmedTime) }}</div>
              </v-col>
              <v-col v-if="order.paymentTransactionHash" cols="12">
                <div class="text-caption text-grey-lighten-1">Transaction Hash</div>
                <div class="text-body-2 font-family-monospace word-break-all">
                  {{ order.paymentTransactionHash }}
                </div>
                <v-btn
                  v-if="order.paymentTransactionHash && order.chainId"
                  size="x-small"
                  variant="text"
                  class="mt-1"
                  @click="viewTransactionOnExplorer"
                >
                  <v-icon start size="14">mdi-open-in-new</v-icon>
                  View on Explorer
                </v-btn>
              </v-col>
              <v-col v-if="order.paymentWalletAddress" cols="12">
                <div class="text-caption text-grey-lighten-1">Payment Wallet Address</div>
                <div class="text-body-2 font-family-monospace word-break-all">
                  {{ order.paymentWalletAddress }}
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>

        <!-- Order Items -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="text-subtitle-1">Order Items</v-card-title>
          <v-divider></v-divider>
          <v-card-text>
            <v-table density="compact">
              <thead>
                <tr>
                  <th>Product Name</th>
                  <th>Unit Price</th>
                  <th>Quantity</th>
                  <th class="text-right">Subtotal</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in order.orderItems" :key="item.orderItemId">
                  <td>{{ item.productName }}</td>
                  <td>{{ formatAmount(item.unitPrice) }} {{ order.currency }}</td>
                  <td>{{ item.quantity }}</td>
                  <td class="text-right">{{ formatAmount(item.subtotal) }} {{ order.currency }}</td>
                </tr>
                <tr>
                  <td colspan="3" class="text-right font-weight-medium">Total</td>
                  <td class="text-right font-weight-medium">
                    {{ formatAmount(order.totalAmount) }} {{ order.currency }}
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-card-text>
        </v-card>

        <!-- Action Buttons -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn
              v-if="order.status === StoreOrderStatus.PendingPayment && order.paymentMode === StorePaymentMode.Web3"
              color="error"
              variant="outlined"
              @click="handleCancelOrder"
              :loading="cancelling"
            >
              <v-icon start>mdi-cancel</v-icon>
              Cancel Order
            </v-btn>
            <v-btn color="primary" variant="tonal" @click="goToOrders">
              <v-icon start>mdi-format-list-bulleted</v-icon>
              View All Orders
            </v-btn>
          </v-card-actions>
        </v-card>
      </template>

      <!-- Order Payment Dialog -->
      <OrderPaymentDialog
        v-model="orderPaymentDialog"
        :order="order"
        @payment-success="handlePaymentSuccess"
      />
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import { onBeforeMount, ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Filter from '@/libs/Filter'
import FastDialog from '@/libs/FastDialog'
import { useUserStore } from '@/store/user'
import { useCartStore } from '@/store/cart'
import OrderPaymentDialog from '@/components/OrderPaymentDialog.vue'
import { fetchOrderDetail, cancelOrder } from '@/services/storeApi'
import type { StoreOrderDetailResult } from '@/types'
import { StoreOrderStatus, StorePaymentStatus, StorePaymentMode } from '@/types'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const cartStore = useCartStore()

const loading = ref(true)
const cancelling = ref(false)
const order = ref<StoreOrderDetailResult | null>(null)
const orderPaymentDialog = ref(false)

const userUid = computed(() => userStore.state.userInfo?.uid ?? null)

function goBack() {
  router.back()
}

function goToOrders() {
  router.push({ name: 'Orders' })
}

function formatDateTime(value?: string | null) {
  if (!value) return '-'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString()
}

function formatAmount(amount: number | string) {
  return Filter.formatToken(amount)
}

function getOrderStatusColor(status: StoreOrderStatus | number) {
  const statusMap: Record<StoreOrderStatus, string> = {
    [StoreOrderStatus.PendingPayment]: 'warning',
    [StoreOrderStatus.Paid]: 'success',
    [StoreOrderStatus.Shipped]: 'info',
    [StoreOrderStatus.Delivered]: 'success',
    [StoreOrderStatus.Cancelled]: 'error',
    [StoreOrderStatus.Refunded]: 'grey'
  }
  return statusMap[status as StoreOrderStatus] ?? 'grey'
}

function getOrderStatusText(status: StoreOrderStatus | number) {
  const statusMap: Record<StoreOrderStatus, string> = {
    [StoreOrderStatus.PendingPayment]: 'Pending Payment',
    [StoreOrderStatus.Paid]: 'Paid',
    [StoreOrderStatus.Shipped]: 'Shipped',
    [StoreOrderStatus.Delivered]: 'Delivered',
    [StoreOrderStatus.Cancelled]: 'Cancelled',
    [StoreOrderStatus.Refunded]: 'Refunded'
  }
  return statusMap[status as StoreOrderStatus] ?? 'Unknown'
}

function getPaymentStatusColor(status: StorePaymentStatus | number) {
  const statusMap: Record<StorePaymentStatus, string> = {
    [StorePaymentStatus.PendingSignature]: 'warning',
    [StorePaymentStatus.AwaitingOnChainConfirmation]: 'info',
    [StorePaymentStatus.Confirmed]: 'success',
    [StorePaymentStatus.Failed]: 'error',
    [StorePaymentStatus.Cancelled]: 'grey'
  }
  return statusMap[status as StorePaymentStatus] ?? 'grey'
}

function getPaymentStatusText(status: StorePaymentStatus | number) {
  const statusMap: Record<StorePaymentStatus, string> = {
    [StorePaymentStatus.PendingSignature]: 'Pending Signature',
    [StorePaymentStatus.AwaitingOnChainConfirmation]: 'Awaiting Confirmation',
    [StorePaymentStatus.Confirmed]: 'Confirmed',
    [StorePaymentStatus.Failed]: 'Failed',
    [StorePaymentStatus.Cancelled]: 'Cancelled'
  }
  return statusMap[status as StorePaymentStatus] ?? 'Unknown'
}

function viewTransactionOnExplorer() {
  if (!order.value?.paymentTransactionHash || !order.value?.chainId) {
    return
  }
  
  let explorerUrl = ''
  const txHash = order.value.paymentTransactionHash
  
  // Select corresponding blockchain explorer based on chainId
  if (order.value.chainId === 1) {
    // Ethereum Mainnet
    explorerUrl = `https://etherscan.io/tx/${txHash}`
  } else if (order.value.chainId === 137) {
    // Polygon Mainnet
    explorerUrl = `https://polygonscan.com/tx/${txHash}`
  } else if (order.value.chainId === 80001) {
    // Mumbai Testnet
    explorerUrl = `https://mumbai.polygonscan.com/tx/${txHash}`
  } else {
    FastDialog.warningSnackbar('Explorer not available for this network')
    return
  }
  
  window.open(explorerUrl, '_blank')
}

async function refreshOrder() {
  await loadOrder()
}

async function loadOrder() {
  const orderId = Number(route.params.orderId)
  const uid = userUid.value
  
  if (!orderId || Number.isNaN(orderId)) {
    FastDialog.errorSnackbar('Invalid order ID')
    router.push({ name: 'Orders' })
    return
  }
  
  if (!uid) {
    FastDialog.warningSnackbar('Please login to view order details')
    router.push({ name: 'Home' })
    return
  }
  
  loading.value = true
  try {
    order.value = await fetchOrderDetail(uid, orderId)
  } catch (error) {
    console.error('Failed to load order:', error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to load order details')
    order.value = null
  } finally {
    loading.value = false
  }
}

async function handleCancelOrder() {
  if (!order.value || !userUid.value) {
    return
  }
  
  const confirmed = window.confirm('Are you sure you want to cancel this order? This action cannot be undone.')
  
  if (!confirmed) {
    return
  }
  
  cancelling.value = true
  try {
    await cancelOrder(userUid.value, order.value.orderId, { reason: 'User cancelled' })
    FastDialog.successSnackbar('Order cancelled')
    await loadOrder()
  } catch (error) {
    console.error('Failed to cancel order:', error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to cancel order')
  } finally {
    cancelling.value = false
  }
}

function handlePaymentSuccess(updatedOrder: StoreOrderDetailResult) {
  order.value = updatedOrder
  orderPaymentDialog.value = false
}

onBeforeMount(async () => {
  await loadOrder()
})
</script>

<style scoped>
.word-break-all {
  word-break: break-all;
}
</style>

