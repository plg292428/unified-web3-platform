<template>
  <v-container>
    <v-responsive>
      <!-- Page Title -->
      <div class="mt-2 d-flex align-center">
        <v-btn icon="mdi-arrow-left" variant="text" @click="goBack"></v-btn>
        <span class="text-subtitle-2">Shopping Cart</span>
        <v-spacer></v-spacer>
        <v-btn
          color="primary"
          variant="outlined"
          size="small"
          @click="refreshCart"
          :loading="cartStore.loading"
        >
          <v-icon start size="18">mdi-refresh</v-icon>
          Refresh
        </v-btn>
      </div>

      <!-- Not Logged In -->
      <template v-if="!userUid">
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text class="text-center py-8">
            <v-icon size="64" color="primary" class="mb-4">mdi-lock</v-icon>
            <div class="text-h6 mt-4 text-grey-lighten-1">Please Login to View Your Cart</div>
            <div class="text-body-2 text-grey-lighten-2 mt-2">
              Connect your wallet and sign in to access your shopping cart
            </div>
            <v-btn color="primary" class="mt-4" @click="handleLogin" :loading="loggingIn">
              <v-icon start>mdi-wallet</v-icon>
              Connect Wallet & Login
            </v-btn>
            <div v-if="!walletStore.state.initialized || !walletStore.state.provider" class="text-caption text-grey-lighten-1 mt-2">
              No wallet detected. Click to set up your wallet.
            </div>
            <v-btn color="primary" variant="tonal" class="mt-2" @click="goToHome">
              <v-icon start>mdi-shopping</v-icon>
              Continue Shopping
            </v-btn>
          </v-card-text>
        </v-card>
      </template>

      <!-- Loading -->
      <template v-else-if="cartStore.loading && cartItems.length === 0">
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text>
            <v-skeleton-loader type="list-item-three-line@3"></v-skeleton-loader>
          </v-card-text>
        </v-card>
      </template>

      <!-- Empty Cart -->
      <template v-else-if="cartItems.length === 0">
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text class="text-center py-8">
            <v-img height="120" src="@/assets/no_data.svg" class="mx-auto"></v-img>
            <div class="text-h6 mt-4 text-grey-lighten-1">Your cart is empty</div>
            <div class="text-body-2 text-grey-lighten-2 mt-2">
              Start shopping to add items to your cart
            </div>
            <v-btn color="primary" variant="tonal" class="mt-4" @click="goToHome">
              <v-icon start>mdi-shopping</v-icon>
              Go Shopping
            </v-btn>
          </v-card-text>
        </v-card>
      </template>

      <!-- Cart Content -->
      <template v-else>
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="d-flex align-center">
            <span class="text-subtitle-1">Cart Items</span>
            <v-spacer></v-spacer>
            <span class="text-caption text-grey-lighten-1">
              {{ cartTotalQuantity }} {{ cartTotalQuantity === 1 ? 'item' : 'items' }}
            </span>
          </v-card-title>
          <v-divider></v-divider>
          <v-card-text>
            <v-list density="comfortable">
              <v-list-item
                v-for="item in cartItems"
                :key="item.cartItemId"
                :value="item.cartItemId"
              >
                <template #prepend>
                  <v-avatar size="64" rounded="lg" class="mr-4">
                    <v-img
                      :src="resolveProductImage(item)"
                      cover
                      class="bg-grey-darken-3"
                    >
                      <template v-slot:placeholder>
                        <div class="d-flex align-center justify-center fill-height">
                          <v-icon color="grey">mdi-image</v-icon>
                        </div>
                      </template>
                    </v-img>
                  </v-avatar>
                </template>
                <template #title>
                  <div class="d-flex align-center">
                    <span class="font-weight-medium">{{ item.productName }}</span>
                    <v-spacer></v-spacer>
                    <v-chip
                      v-if="item.inventoryAvailable > 0"
                      size="x-small"
                      color="success"
                      variant="tonal"
                      class="mr-2"
                    >
                      Stock {{ item.inventoryAvailable }}
                    </v-chip>
                    <v-chip
                      v-else
                      size="x-small"
                      color="error"
                      variant="tonal"
                      class="mr-2"
                    >
                      Out of Stock
                    </v-chip>
                  </div>
                </template>
                <template #subtitle>
                  <div class="d-flex align-center mt-1">
                    <span class="text-caption text-grey-lighten-1">
                      Unit Price: {{ Filter.formatPrice(item.unitPrice) }} {{ item.currency }}
                    </span>
                    <v-spacer></v-spacer>
                    <span class="text-body-2 text-primary font-weight-medium">
                      Subtotal: {{ Filter.formatPrice(item.subtotal) }} {{ item.currency }}
                    </span>
                  </div>
                </template>
                <template #append>
                  <div class="d-flex align-center">
                    <v-btn
                      icon="mdi-minus"
                      variant="text"
                      size="small"
                      :disabled="cartStore.processing"
                      @click="handleUpdateQuantity(item, item.quantity - 1)"
                    ></v-btn>
                    <v-text-field
                      class="mx-2"
                      style="max-width: 80px"
                      type="number"
                      density="compact"
                      variant="outlined"
                      hide-details
                      :model-value="item.quantity"
                      :disabled="cartStore.processing || item.inventoryAvailable <= 0"
                      :min="1"
                      :max="item.inventoryAvailable"
                      @update:model-value="(value: number) => handleQuantityInput(item, value)"
                    />
                    <v-btn
                      icon="mdi-plus"
                      variant="text"
                      size="small"
                      :disabled="cartStore.processing || item.quantity >= item.inventoryAvailable"
                      @click="handleUpdateQuantity(item, item.quantity + 1)"
                    ></v-btn>
                    <v-btn
                      icon="mdi-delete"
                      color="error"
                      variant="text"
                      size="small"
                      class="ml-2"
                      :disabled="cartStore.processing"
                      @click="handleRemoveItem(item)"
                    ></v-btn>
                  </div>
                </template>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>

        <!-- Order Summary -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="text-subtitle-1">Order Summary</v-card-title>
          <v-divider></v-divider>
          <v-card-text>
            <v-list density="compact">
              <v-list-item>
                <v-list-item-title class="text-body-2">Total Items</v-list-item-title>
                <v-list-item-subtitle class="text-right">
                  {{ cartTotalQuantity }} {{ cartTotalQuantity === 1 ? 'item' : 'items' }}
                </v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title class="text-body-2">Total Amount</v-list-item-title>
                <v-list-item-subtitle class="text-right">
                  <span class="text-h6 text-primary font-weight-medium">
                    {{ Filter.formatPrice(cartTotalAmount) }} {{ cartCurrency }}
                  </span>
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card-text>
          <v-divider></v-divider>
          
          <!-- Payment Method Selection -->
          <v-card-text class="pt-4">
            <PaymentMethodSelector
              v-model="selectedPaymentMode"
              :disabled="placingOrder || cartStore.processing"
            />
          </v-card-text>
          
          <v-card-actions class="pa-4">
            <v-btn
              color="primary"
              size="large"
              block
              :loading="placingOrder || cartStore.processing"
              :disabled="cartTotalQuantity === 0"
              append-icon="mdi-arrow-right"
              @click="handlePlaceOrder"
            >
              Place Order
            </v-btn>
          </v-card-actions>
        </v-card>
      </template>

      <!-- Order Payment Dialog -->
      <OrderPaymentDialog
        v-model="orderPaymentDialog"
        :order="orderPaymentDetail"
        @payment-success="handlePaymentSuccess"
      />

      <!-- Order Success Dialog -->
      <v-dialog v-model="orderSuccessDialog" max-width="520" persistent>
        <v-card>
          <v-card-title class="d-flex align-center">
            Order Created Successfully
            <v-spacer></v-spacer>
            <v-btn icon="mdi-close" variant="text" @click="closeOrderSuccessDialog"></v-btn>
          </v-card-title>
          <v-divider></v-divider>
          <v-card-text v-if="orderSuccessDetail">
            <div class="text-body-2">Order Number: {{ orderSuccessDetail.orderNumber }}</div>
            <div class="text-body-2 mt-2">
              Amount: {{ Filter.formatPrice(orderSuccessDetail.totalAmount) }} {{ orderSuccessDetail.currency }}
            </div>
            <div class="text-body-2 mt-2">
              Payment Method: {{ orderSuccessDetail.paymentMethod ?? 'PayFi' }}
            </div>
            <div class="text-body-2 mt-2">
              Status: {{ getOrderStatusMeta(orderSuccessDetail.status).text }}
            </div>
            <v-divider class="my-3"></v-divider>
            <div class="text-body-2 font-weight-medium mb-2">Product Information</div>
            <v-list density="compact">
              <v-list-item v-for="item in orderSuccessDetail.items" :key="item.orderItemId">
                <v-list-item-title>{{ item.productName }}</v-list-item-title>
                <v-list-item-subtitle>
                  {{ item.quantity }} pcs · {{ Filter.formatPrice(item.unitPrice) }} {{ orderSuccessDetail.currency }}
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
            <div class="text-caption text-grey-lighten-1 mt-3">
              Please refresh the page after payment to view the latest status.
            </div>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary" variant="tonal" @click="closeOrderSuccessDialog">OK</v-btn>
            <v-btn color="primary" @click="goToOrders">View Orders</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import { onBeforeMount, ref, computed, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import Filter from '@/libs/Filter'
import FastDialog from '@/libs/FastDialog'
import WebApi from '@/libs/WebApi'
import productPlaceholderImage from '@/assets/online_transactions.svg?url'
import { useCartStore } from '@/store/cart'
import { useUserStore } from '@/store/user'
import { useWalletStore } from '@/store/wallet'
import OrderPaymentDialog from '@/components/OrderPaymentDialog.vue'
import PaymentMethodSelector from '@/components/PaymentMethodSelector.vue'
import type {
  StoreCartItemResult,
  StoreOrderDetailResult,
  StoreOrderStatus,
  StorePaymentMode,
  StorePaymentStatus
} from '@/types'

const router = useRouter()
const cartStore = useCartStore()
const userStore = useUserStore()
const walletStore = useWalletStore()

const placingOrder = ref(false)
const loggingIn = ref(false)
const orderPaymentDialog = ref(false)
const orderPaymentDetail = ref<StoreOrderDetailResult | null>(null)
const orderSuccessDialog = ref(false)
const orderSuccessDetail = ref<StoreOrderDetailResult | null>(null)
// Initialize payment mode based on wallet status
const walletState = computed(() => walletStore.state)
const initialPaymentMode = computed(() => {
  if (walletState.value.active && walletState.value.provider !== null) {
    return StorePaymentMode.Web3
  }
  return StorePaymentMode.Traditional
})
const selectedPaymentMode = ref<StorePaymentMode>(initialPaymentMode.value)

const userUid = computed(() => userStore.state.userInfo?.uid ?? null)
const cartItems = computed(() => cartStore.items)
const cartTotalAmount = computed(() => cartStore.totalAmount)
const cartTotalQuantity = computed(() => cartStore.totalQuantity)
const cartCurrency = computed(() => cartItems.value[0]?.currency ?? 'USDT')

const apiBaseUrl = WebApi.getInstance().baseUrl ?? ''

function resolveProductImage(item: StoreCartItemResult) {
  // Cart items may not have image URL, use placeholder
  return productPlaceholderImage
}

function goBack() {
  router.back()
}

function goToHome() {
  router.push({ name: 'Home' })
}

function goToOrders() {
  router.push({ name: 'Orders' })
}

async function refreshCart() {
  const uid = userUid.value
  if (!uid) {
    FastDialog.warningSnackbar('Please login first.')
    return
  }
  try {
    await cartStore.refresh(uid)
    FastDialog.successSnackbar('Cart refreshed')
  } catch (error) {
    console.error('Failed to refresh cart:', error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to refresh cart')
  }
}

async function handleUpdateQuantity(item: StoreCartItemResult, nextQuantity: number) {
  const uid = userUid.value
  if (!uid) {
    FastDialog.warningSnackbar('Please login first.')
    return
  }
  if (nextQuantity <= 0) {
    await handleRemoveItem(item)
    return
  }
  if (nextQuantity > item.inventoryAvailable) {
    FastDialog.warningSnackbar(`Maximum quantity is ${item.inventoryAvailable}`)
    return
  }
  try {
    await cartStore.updateItem(uid, item.cartItemId, nextQuantity)
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to update quantity')
  }
}

function handleQuantityInput(item: StoreCartItemResult, value: string | number) {
  const quantity = Math.floor(Number(value))
  if (!Number.isFinite(quantity) || quantity < 1) {
    return
  }
  handleUpdateQuantity(item, quantity)
}

async function handleRemoveItem(item: StoreCartItemResult) {
  const uid = userUid.value
  if (!uid) {
    FastDialog.warningSnackbar('Please login first.')
    return
  }
  try {
    await cartStore.removeItem(uid, item.cartItemId)
    FastDialog.successSnackbar('Item removed')
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to remove item')
  }
}

async function handlePlaceOrder() {
  const uid = userUid.value
  if (!uid) {
    FastDialog.warningSnackbar('Please login first.')
    return
  }
  if (cartItems.value.length === 0) {
    FastDialog.warningSnackbar('Your cart is empty.')
    return
  }
  
  // Validate payment method selection
  if (selectedPaymentMode.value === StorePaymentMode.Web3) {
    const walletState = walletStore.state
    if (!walletState.active || !walletState.provider || !walletState.address) {
      FastDialog.warningSnackbar('Please connect your wallet first to use Web3 payment.')
      return
    }
  }
  
  placingOrder.value = true
  try {
    const walletState = walletStore.state
    const isWeb3Payment = selectedPaymentMode.value === StorePaymentMode.Web3
    
    const order = await cartStore.placeOrder({
      uid,
      paymentMode: selectedPaymentMode.value,
      paymentMethod: isWeb3Payment 
        ? (walletState.providerName ?? walletState.providerType ?? 'Web3 Wallet')
        : 'Traditional Payment',
      paymentProviderType: isWeb3Payment ? walletState.providerType : undefined,
      paymentProviderName: isWeb3Payment ? walletState.providerName : undefined,
      paymentWalletAddress: isWeb3Payment ? walletState.address : undefined,
      paymentWalletLabel: isWeb3Payment && walletState.address 
        ? formatWalletAddress(walletState.address) 
        : undefined,
      chainId: isWeb3Payment && walletState.chainId > 0 ? walletState.chainId : undefined,
      remark: 'Cart checkout'
    })
    
    // If Web3 payment and signature required, open payment dialog
    if (isWeb3Payment && order.paymentStatus === StorePaymentStatus.PendingSignature) {
      orderPaymentDetail.value = order
      orderPaymentDialog.value = true
    } else {
      // Traditional payment or completed payment, show success dialog
      orderSuccessDetail.value = order
      orderSuccessDialog.value = true
      FastDialog.successSnackbar('Order created successfully')
    }
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to create order')
  } finally {
    placingOrder.value = false
  }
}

function handlePaymentSuccess(order: StoreOrderDetailResult) {
  orderPaymentDialog.value = false
  orderSuccessDetail.value = order
  orderSuccessDialog.value = true
}

function closeOrderSuccessDialog() {
  orderSuccessDialog.value = false
  orderSuccessDetail.value = null
  // Refresh cart
  const uid = userUid.value
  if (uid) {
    cartStore.refresh(uid)
  }
}

function formatWalletAddress(address: string): string {
  if (!address || address.length < 10) return address
  return `${address.substring(0, 6)}...${address.substring(address.length - 4)}`
}

function getOrderStatusMeta(status: StoreOrderStatus) {
  const statusMap: Record<StoreOrderStatus, { text: string; color: string }> = {
    [StoreOrderStatus.PendingPayment]: { text: 'Pending Payment', color: 'warning' },
    [StoreOrderStatus.Paid]: { text: 'Paid', color: 'success' },
    [StoreOrderStatus.Shipped]: { text: 'Shipped', color: 'info' },
    [StoreOrderStatus.Delivered]: { text: 'Delivered', color: 'success' },
    [StoreOrderStatus.Cancelled]: { text: 'Cancelled', color: 'error' },
    [StoreOrderStatus.Refunded]: { text: 'Refunded', color: 'grey' }
  }
  return statusMap[status] ?? { text: 'Unknown', color: 'grey' }
}

async function handleLogin() {
  loggingIn.value = true
  try {
    // First, try to initialize wallet if not already initialized
    if (!walletStore.state.initialized) {
      await walletStore.initialize()
    }

    // Check if wallet provider exists
    if (!walletStore.state.provider) {
      console.log('No wallet provider detected, redirecting to Go page')
      console.log('Current route:', router.currentRoute.value.name, router.currentRoute.value.path)
      FastDialog.warningSnackbar('No wallet detected. Redirecting to wallet setup page...')
      // Use nextTick to ensure the dialog is shown before navigation
      await nextTick()
      // Small delay to ensure user sees the message
      await new Promise(resolve => setTimeout(resolve, 500))
      // Redirect to wallet setup page - use window.location directly to bypass any route guards
      console.log('Redirecting to /go using window.location.href')
      console.log('Current location:', window.location.href)
      // Use replace to prevent back button issues
      window.location.replace('/go')
      return
    }

    // Check if wallet is connected
    if (!walletStore.state.active || !walletStore.state.address) {
      try {
        FastDialog.infoSnackbar('Connecting wallet...')
        const accounts = await walletStore.connect()
        if (!accounts || accounts.length === 0) {
          FastDialog.errorSnackbar('Failed to connect wallet. Please unlock your wallet and try again.')
          return
        }
      } catch (error: any) {
        console.error('Wallet connection error:', error)
        // Handle specific error codes
        if (error.code === 4001) {
          FastDialog.warningSnackbar('Connection cancelled. Please approve the connection request to continue.')
        } else if (error.code === -32002) {
          FastDialog.warningSnackbar('Connection request already pending. Please check your wallet.')
        } else {
          FastDialog.errorSnackbar('Failed to connect wallet. Please check your wallet and try again.')
        }
        return
      }
    }

    // Check if already logged in
    const checkSignedResult = await userStore.checkSigned()
    if (!checkSignedResult.succeed) {
      FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
      return
    }

    // If logged in, refresh cart
    if (checkSignedResult.data?.singined) {
      const uid = userUid.value
      if (uid) {
        await cartStore.refresh(uid)
        FastDialog.successSnackbar('Login successful!')
      }
      return
    }

    // Need to sign in
    FastDialog.infoSnackbar('Please sign the message in your wallet to login...')
    const from = walletStore.state.address
    const provider = walletStore.state.provider
    const message = `0x${checkSignedResult.data.tokenText}`
    
    const signedText = await provider.request({
      method: 'personal_sign',
      params: [message, from]
    })

    // Execute login
    const signResult = await userStore.signIn(signedText)
    if (!signResult.succeed) {
      FastDialog.errorSnackbar(signResult.errorMessage as string)
      return
    }

    // Get user info
    if (!(await userStore.updateUserInfo())) {
      userStore.signOut()
      FastDialog.errorSnackbar('Failed to get user information. Please try again.')
      return
    }

    // Login successful, refresh cart
    const uid = userUid.value
    if (uid) {
      await cartStore.refresh(uid)
      FastDialog.successSnackbar('Login successful!')
    }
  } catch (error) {
    console.error('Login error:', error)
    if ((error as any)?.code === 4001) {
      FastDialog.warningSnackbar('Login cancelled. Please sign the message to access your cart.')
    } else {
      FastDialog.errorSnackbar('Login failed. Please try again.')
    }
  } finally {
    loggingIn.value = false
  }
}

onBeforeMount(async () => {
  // Initialize wallet if not already initialized
  if (!walletStore.state.initialized) {
    await walletStore.initialize()
  }

  const uid = userUid.value
  if (uid) {
    try {
      await cartStore.refresh(uid)
    } catch (error) {
      console.error('Failed to load cart:', error)
      FastDialog.errorSnackbar((error as Error).message ?? 'Failed to load cart')
    }
  }
  // Don't redirect if not logged in, show login prompt
})
</script>

