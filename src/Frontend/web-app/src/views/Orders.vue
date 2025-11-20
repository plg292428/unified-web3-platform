<template>
  <v-container>
    <v-responsive>
      <div class="mt-2 ml-1 text-subtitle-2 d-flex align-center">
        My Orders
        <v-spacer></v-spacer>
        <v-btn
          color="primary"
          variant="outlined"
          size="small"
          @click="refreshOrders"
          :loading="orderListLoading"
        >
          <v-icon start size="18">mdi-refresh</v-icon>
          Refresh
        </v-btn>
      </div>

      <!-- Order Filter -->
      <v-row dense class="mt-3">
        <v-col cols="12" md="4">
          <v-select
            v-model="filterStatus"
            density="compact"
            variant="outlined"
            :items="statusOptions"
            item-title="label"
            item-value="value"
            label="Order Status"
            clearable
            hide-details
            @update:model-value="onFilterChange"
          ></v-select>
        </v-col>
        <v-col cols="12" md="4">
          <v-select
            v-model="filterPaymentMode"
            density="compact"
            variant="outlined"
            :items="paymentModeOptions"
            item-title="label"
            item-value="value"
            label="Payment Method"
            clearable
            hide-details
            @update:model-value="onFilterChange"
          ></v-select>
        </v-col>
        <v-col cols="12" md="4">
          <v-text-field
            v-model="searchOrderNumber"
            density="compact"
            variant="outlined"
            placeholder="Search order number..."
            prepend-inner-icon="mdi-magnify"
            clearable
            hide-details
            @update:model-value="onSearchChange"
          ></v-text-field>
        </v-col>
      </v-row>

      <!-- Order List -->
      <div class="mt-4">
        <template v-if="orderListLoading && orderList.length === 0">
          <v-card v-for="n in 3" :key="`order-skel-${n}`" class="primary-border mt-3" variant="outlined">
            <v-card-text>
              <v-skeleton-loader type="article, actions"></v-skeleton-loader>
            </v-card-text>
          </v-card>
        </template>

        <template v-else-if="orderList.length > 0">
          <v-card
            v-for="order in orderList"
            :key="order.orderId"
            class="primary-border mt-3"
            variant="outlined"
            @click="viewOrderDetail(order.orderId)"
          >
            <v-card-text>
              <v-row dense align="center">
                <v-col cols="12" md="6">
                  <div class="d-flex align-center">
                    <v-chip
                      size="small"
                      :color="getOrderStatusColor(order.status)"
                      variant="tonal"
                      class="mr-2"
                    >
                      {{ getOrderStatusText(order.status) }}
                    </v-chip>
                    <v-chip
                      v-if="order.paymentMode === StorePaymentMode.Web3"
                      size="x-small"
                      color="info"
                      variant="tonal"
                      class="mr-2"
                    >
                      Web3
                    </v-chip>
                    <span class="text-caption text-grey-lighten-1">{{ order.orderNumber }}</span>
                  </div>
                  <div class="mt-2 text-subtitle-2">
                    {{ formatDateTime(order.createTime) }}
                  </div>
                </v-col>
                <v-col cols="12" md="6" class="text-right">
                  <div class="text-subtitle-1 font-weight-medium">
                    {{ formatAmount(order.totalAmount) }} {{ order.currency }}
                  </div>
                  <div class="mt-1">
                    <v-chip
                      size="x-small"
                      :color="getPaymentStatusColor(order.paymentStatus)"
                      variant="tonal"
                    >
                      {{ getPaymentStatusText(order.paymentStatus) }}
                    </v-chip>
                  </div>
                </v-col>
              </v-row>

              <!-- Order Status Flow -->
              <v-timeline v-if="order.paymentMode === StorePaymentMode.Web3" density="compact" class="mt-3">
                <v-timeline-item
                  :dot-color="order.paymentStatus >= StorePaymentStatus.PendingSignature ? 'primary' : 'grey'"
                  size="small"
                >
                  <div class="text-caption">Pending Signature</div>
                </v-timeline-item>
                <v-timeline-item
                  :dot-color="order.paymentStatus >= StorePaymentStatus.AwaitingOnChainConfirmation ? 'primary' : 'grey'"
                  size="small"
                >
                  <div class="text-caption">Awaiting On-Chain Confirmation</div>
                  <div v-if="order.paymentConfirmations" class="text-caption text-grey-lighten-1">
                    Confirmations: {{ order.paymentConfirmations }}
                  </div>
                </v-timeline-item>
                <v-timeline-item
                  :dot-color="order.paymentStatus >= StorePaymentStatus.Confirmed ? 'success' : 'grey'"
                  size="small"
                >
                  <div class="text-caption">Payment Completed</div>
                </v-timeline-item>
              </v-timeline>

              <!-- Order Actions -->
              <v-card-actions class="mt-2">
                <v-spacer></v-spacer>
                <v-btn
                  v-if="order.status === StoreOrderStatus.PendingPayment && order.paymentMode === StorePaymentMode.Web3"
                  color="error"
                  variant="text"
                  size="small"
                  @click.stop="handleCancelOrder(order)"
                >
                  Cancel Order
                </v-btn>
                <v-btn
                  color="primary"
                  variant="text"
                  size="small"
                  @click.stop="viewOrderDetail(order.orderId)"
                >
                  View Details
                </v-btn>
              </v-card-actions>
            </v-card-text>
          </v-card>

          <!-- Pagination -->
          <v-pagination
            v-if="orderTotal > orderPageSize"
            class="mt-4"
            v-model="orderPage"
            :length="orderTotalPages"
            color="primary"
            density="comfortable"
            @update:modelValue="loadOrders"
          />
        </template>

        <template v-else>
          <v-card class="primary-border mt-3" variant="outlined">
            <v-card-text class="text-center py-8">
              <v-img height="96" src="@/assets/no_data.svg" class="mx-auto"></v-img>
              <div class="text-grey-lighten-1 text-subtitle-2 mt-4">No orders yet</div>
            </v-card-text>
          </v-card>
        </template>
      </div>

    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import { computed, onBeforeMount, onUnmounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import { useUserStore } from '@/store/user'
import { useCartStore } from '@/store/cart'
import { fetchOrderList, cancelOrder } from '@/services/storeApi'
import type { StoreOrderSummaryResult } from '@/types'
import { StoreOrderStatus, StorePaymentStatus, StorePaymentMode } from '@/types'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const cartStore = useCartStore()

const userUid = computed(() => userStore.state.userInfo?.uid ?? null)

// Order list
const orderList = ref<StoreOrderSummaryResult[]>([])
const orderListLoading = ref(false)
const orderPage = ref(1)
const orderPageSize = ref(10)
const orderTotal = ref(0)

// Filter
const filterStatus = ref<StoreOrderStatus | null>(null)
const filterPaymentMode = ref<StorePaymentMode | null>(null)
const searchOrderNumber = ref<string>('')


// Status options
const statusOptions = [
  { label: 'Pending Payment', value: StoreOrderStatus.PendingPayment },
  { label: 'Paid', value: StoreOrderStatus.Paid },
  { label: 'Cancelled', value: StoreOrderStatus.Cancelled },
  { label: 'Completed', value: StoreOrderStatus.Completed }
]

const paymentModeOptions = [
  { label: 'Traditional Payment', value: StorePaymentMode.Traditional },
  { label: 'Web3 Payment', value: StorePaymentMode.Web3 }
]

const orderTotalPages = computed(() => {
  const total = Math.ceil(orderTotal.value / orderPageSize.value)
  return total > 0 ? total : 1
})

// Search debounce
let searchDebounceTimer: ReturnType<typeof setTimeout> | null = null
function onSearchChange() {
  if (searchDebounceTimer) {
    clearTimeout(searchDebounceTimer)
  }
  searchDebounceTimer = setTimeout(() => {
    orderPage.value = 1
    loadOrders()
  }, 500)
}

function onFilterChange() {
  orderPage.value = 1
  loadOrders()
}

async function loadOrders() {
  if (!userUid.value) {
    return
  }

  orderListLoading.value = true
  try {
    const result = await fetchOrderList(userUid.value, orderPage.value, orderPageSize.value, {
      status: filterStatus.value ?? undefined,
      paymentMode: filterPaymentMode.value ?? undefined,
      orderNumber: searchOrderNumber.value || undefined
    })
    orderList.value = result.items
    orderTotal.value = result.totalCount
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to load orders')
  } finally {
    orderListLoading.value = false
  }
}

async function refreshOrders() {
  await loadOrders()
}

function viewOrderDetail(orderId: number) {
  router.push({ name: 'OrderDetail', params: { orderId: orderId.toString() } })
}

async function handleCancelOrder(order: StoreOrderSummaryResult) {
  if (!userUid.value) {
    return
  }

  const confirmed = window.confirm('Are you sure you want to cancel this order? This action cannot be undone.')
  
  if (!confirmed) {
    return
  }

  try {
    await cancelOrder(userUid.value, order.orderId, { reason: 'User cancelled' })
    FastDialog.successSnackbar('Order cancelled')
    await loadOrders()
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to cancel order')
  }
}

function getOrderStatusColor(status: StoreOrderStatus): string {
  switch (status) {
    case StoreOrderStatus.PendingPayment:
      return 'warning'
    case StoreOrderStatus.Paid:
      return 'info'
    case StoreOrderStatus.Completed:
      return 'success'
    case StoreOrderStatus.Cancelled:
      return 'error'
    default:
      return 'grey'
  }
}

function getOrderStatusText(status: StoreOrderStatus): string {
  switch (status) {
    case StoreOrderStatus.PendingPayment:
      return 'Pending Payment'
    case StoreOrderStatus.Paid:
      return 'Paid'
    case StoreOrderStatus.Completed:
      return 'Completed'
    case StoreOrderStatus.Cancelled:
      return 'Cancelled'
    default:
      return 'Unknown'
  }
}

function getPaymentStatusColor(status: StorePaymentStatus): string {
  switch (status) {
    case StorePaymentStatus.PendingSignature:
      return 'warning'
    case StorePaymentStatus.AwaitingOnChainConfirmation:
      return 'info'
    case StorePaymentStatus.Confirmed:
      return 'success'
    case StorePaymentStatus.Failed:
      return 'error'
    case StorePaymentStatus.Cancelled:
      return 'grey'
    default:
      return 'grey'
  }
}

function getPaymentStatusText(status: StorePaymentStatus): string {
  switch (status) {
    case StorePaymentStatus.PendingSignature:
      return 'Pending Signature'
    case StorePaymentStatus.AwaitingOnChainConfirmation:
      return 'Awaiting Confirmation'
    case StorePaymentStatus.Confirmed:
      return 'Payment Completed'
    case StorePaymentStatus.Failed:
      return 'Payment Failed'
    case StorePaymentStatus.Cancelled:
      return 'Cancelled'
    default:
      return 'Unknown'
  }
}

function formatDateTime(date: Date | string): string {
  return Filter.formatShotDateTime(new Date(date))
}

function formatAmount(amount: number): string {
  return Filter.formatPrice(amount)
}

// Order status auto-refresh (smart polling)
let orderRefreshInterval: ReturnType<typeof setInterval> | null = null
const orderRefreshEnabled = ref(true)

function startOrderAutoRefresh() {
  if (orderRefreshInterval) {
    return
  }

  // Auto-refresh order list every 30 seconds (only refresh pending payment orders)
  orderRefreshInterval = setInterval(async () => {
    if (orderRefreshEnabled.value && userUid.value) {
      // Only refresh orders with pending payment status
      const hasPendingOrders = orderList.value.some(
        o => o.status === StoreOrderStatus.PendingPayment
      )
      if (hasPendingOrders) {
        await loadOrders()
      }
    }
  }, 30000) // 30 seconds
}

function stopOrderAutoRefresh() {
  if (orderRefreshInterval) {
    clearInterval(orderRefreshInterval)
    orderRefreshInterval = null
  }
}

// Watch user login status
watch(userUid, async (uid) => {
  if (uid) {
    await loadOrders()
    startOrderAutoRefresh()
  } else {
    orderList.value = []
    stopOrderAutoRefresh()
  }
}, { immediate: true })

// Component mounted
onBeforeMount(async () => {
  if (userUid.value) {
    await loadOrders()
    startOrderAutoRefresh()
  }
})

// Component unmounted
onUnmounted(() => {
  stopOrderAutoRefresh()
})
</script>

