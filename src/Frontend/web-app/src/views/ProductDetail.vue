<template>
  <v-container>
    <v-responsive>
      <!-- Back Button -->
      <div class="mt-2 d-flex align-center">
        <v-btn icon="mdi-arrow-left" variant="text" @click="goBack"></v-btn>
        <span class="text-subtitle-2">Product Details</span>
      </div>

      <!-- Loading -->
      <template v-if="loading">
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text>
            <v-skeleton-loader type="image, article, actions"></v-skeleton-loader>
          </v-card-text>
        </v-card>
      </template>

      <!-- Product Details -->
      <template v-else-if="product">
        <!-- Product Image -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-img
            :src="resolveProductImage(product)"
            height="400"
            cover
            class="bg-grey-darken-3"
          >
            <template v-slot:placeholder>
              <div class="d-flex align-center justify-center fill-height">
                <v-progress-circular color="primary" indeterminate></v-progress-circular>
              </div>
            </template>
          </v-img>
        </v-card>

        <!-- Product Information -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="text-h6">{{ product.name }}</v-card-title>
          <v-card-subtitle class="text-caption text-grey-lighten-1">
            {{ product.subtitle ?? product.categoryName }}
          </v-card-subtitle>
          <v-card-text>
            <!-- Price -->
            <div class="d-flex align-center mb-4">
              <span class="text-h5 text-primary font-weight-medium">
                {{ Filter.formatPrice(product.price) }} {{ product.currency }}
              </span>
              <v-chip v-if="product.inventoryAvailable > 0" class="ml-2" size="small" color="success" variant="tonal">
                In Stock
              </v-chip>
              <v-chip v-else class="ml-2" size="small" color="error" variant="tonal">
                Out of Stock
              </v-chip>
            </div>

            <!-- Stock Information -->
            <div class="text-body-2 text-grey-lighten-1 mb-4">
              <div>Stock Available: {{ product.inventoryAvailable }}</div>
              <div v-if="product.sku" class="mt-1">SKU: {{ product.sku }}</div>
            </div>

            <!-- Category Breadcrumb -->
            <div v-if="product.breadcrumb && product.breadcrumb.length > 0" class="mb-4">
              <v-chip
                v-for="(crumb, index) in product.breadcrumb"
                :key="crumb.categoryId"
                size="small"
                variant="text"
                class="mr-1"
              >
                {{ crumb.name }}
                <v-icon v-if="index < product.breadcrumb.length - 1" size="x-small" class="ml-1">mdi-chevron-right</v-icon>
              </v-chip>
            </div>

            <!-- Product Description -->
            <div v-if="product.description" class="text-body-2">
              <div class="text-subtitle-2 mb-2">Description</div>
              <div class="text-grey-lighten-2" style="white-space: pre-wrap;">{{ product.description }}</div>
            </div>
          </v-card-text>

          <v-divider></v-divider>

          <v-card-actions class="pa-4">
            <!-- Quantity Selector -->
            <div class="d-flex align-center mb-4 w-100">
              <span class="text-body-2 mr-4">Quantity:</span>
              <div class="d-flex align-center quantity-controls">
                <v-btn
                  icon="mdi-minus"
                  variant="outlined"
                  size="small"
                  color="primary"
                  :disabled="quantity <= 1 || product.inventoryAvailable <= 0"
                  @click="decreaseQuantity"
                ></v-btn>
                <v-text-field
                  class="mx-2 quantity-input"
                  style="max-width: 100px"
                  type="number"
                  density="compact"
                  variant="outlined"
                  hide-details
                  :model-value="quantity"
                  :disabled="product.inventoryAvailable <= 0"
                  :min="1"
                  :max="product.inventoryAvailable"
                  @update:model-value="handleQuantityInput"
                />
                <v-btn
                  icon="mdi-plus"
                  variant="outlined"
                  size="small"
                  color="primary"
                  :disabled="quantity >= product.inventoryAvailable || product.inventoryAvailable <= 0"
                  @click="increaseQuantity"
                ></v-btn>
              </div>
              <v-spacer></v-spacer>
              <span class="text-caption text-grey-lighten-1">
                Max: {{ product.inventoryAvailable }}
              </span>
            </div>

            <v-btn
              color="primary"
              size="large"
              block
              append-icon="mdi-cart-plus"
              :loading="cartProcessing"
              :disabled="product.inventoryAvailable <= 0 || quantity <= 0"
              @click="handleAddToCart"
            >
              Add to Cart ({{ quantity }})
            </v-btn>
            <v-btn
              color="success"
              size="large"
              block
              class="mt-2"
              append-icon="mdi-credit-card"
              :loading="buyingNow"
              :disabled="product.inventoryAvailable <= 0 || quantity <= 0"
              @click="handleBuyNow"
            >
              Buy Now ({{ quantity }})
            </v-btn>
          </v-card-actions>
        </v-card>

        <!-- Product Information Card -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="text-subtitle-2">Product Information</v-card-title>
          <v-card-text>
            <v-list density="compact">
              <v-list-item>
                <v-list-item-title class="text-caption text-grey-lighten-1">Product ID</v-list-item-title>
                <v-list-item-subtitle>{{ product.productId }}</v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title class="text-caption text-grey-lighten-1">Category</v-list-item-title>
                <v-list-item-subtitle>{{ product.categoryName }}</v-list-item-subtitle>
              </v-list-item>
              <v-list-item v-if="product.chainId">
                <v-list-item-title class="text-caption text-grey-lighten-1">Chain ID</v-list-item-title>
                <v-list-item-subtitle>{{ product.chainId }}</v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title class="text-caption text-grey-lighten-1">Created</v-list-item-title>
                <v-list-item-subtitle>{{ formatDateTime(product.createTime) }}</v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title class="text-caption text-grey-lighten-1">Updated</v-list-item-title>
                <v-list-item-subtitle>{{ formatDateTime(product.updateTime) }}</v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </template>

      <!-- Error State -->
      <template v-else>
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text class="text-center">
            <v-img height="96" src="@/assets/no_data.svg" class="mx-auto"></v-img>
            <div class="text-grey-lighten-1 text-subtitle-2 mt-4">Product not found</div>
            <v-btn color="primary" variant="tonal" class="mt-4" @click="goBack">Go Back</v-btn>
          </v-card-text>
        </v-card>
      </template>

      <!-- Order Payment Dialog -->
      <OrderPaymentDialog
        v-model="orderPaymentDialog"
        :order="orderPaymentDetail"
        @payment-success="handlePaymentSuccess"
      />

      <!-- Order Success Dialog -->
      <v-dialog v-model="orderSuccessDialog" max-width="600px" persistent>
        <v-card>
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2" color="success">mdi-check-circle</v-icon>
            Order Created Successfully
            <v-spacer></v-spacer>
            <v-btn icon="mdi-close" variant="text" @click="closeOrderSuccessDialog"></v-btn>
          </v-card-title>
          <v-divider></v-divider>
          <v-card-text v-if="orderSuccessDetail">
            <div class="text-body-2 mb-2">
              <strong>Order Number:</strong> {{ orderSuccessDetail.orderNumber }}
            </div>
            <div class="text-body-2 mb-2">
              <strong>Total Amount:</strong> 
              <span class="text-primary font-weight-medium">
                {{ Filter.formatToken(orderSuccessDetail.totalAmount) }} {{ orderSuccessDetail.currency }}
              </span>
            </div>
            <div class="text-body-2 mb-2">
              <strong>Payment Method:</strong> 
              {{ orderSuccessDetail.paymentProviderName ?? orderSuccessDetail.paymentMethod ?? 'PayFi' }}
            </div>
            <div class="text-body-2 mb-2">
              <strong>Payment Status:</strong> 
              <v-chip size="small" :color="getPaymentStatusColor(orderSuccessDetail.paymentStatus)" variant="tonal">
                {{ getPaymentStatusText(orderSuccessDetail.paymentStatus) }}
              </v-chip>
            </div>
            <div class="text-body-2 mb-2">
              <strong>Order Status:</strong> 
              <v-chip size="small" :color="getOrderStatusColor(orderSuccessDetail.status)" variant="tonal">
                {{ getOrderStatusText(orderSuccessDetail.status) }}
              </v-chip>
            </div>
            <v-divider class="my-3"></v-divider>
            <div class="text-body-2 font-weight-medium mb-2">Product Information</div>
            <v-list density="compact">
              <v-list-item v-for="item in orderSuccessDetail.items" :key="item.orderItemId">
                <v-list-item-title>{{ item.productName }}</v-list-item-title>
                <v-list-item-subtitle>
                  {{ item.quantity }} pcs × {{ Filter.formatToken(item.unitPrice) }} {{ orderSuccessDetail.currency }}
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
            <div class="text-caption text-grey-lighten-1 mt-3">
              Please refresh the page after payment to view the latest status.
            </div>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary" variant="tonal" @click="closeOrderSuccessDialog">Continue Shopping</v-btn>
            <v-btn color="primary" @click="goToOrders">View Orders</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import { onBeforeMount, ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Filter from '@/libs/Filter'
import FastDialog from '@/libs/FastDialog'
import WebApi from '@/libs/WebApi'
import productPlaceholderImage from '@/assets/online_transactions.svg?url'
import { useCartStore } from '@/store/cart'
import { useUserStore } from '@/store/user'
import { useWalletStore } from '@/store/wallet'
import OrderPaymentDialog from '@/components/OrderPaymentDialog.vue'
import { fetchProductDetail } from '@/services/storeApi'
import type { 
  StoreProductDetailResult,
  StoreOrderDetailResult,
  StoreOrderStatus,
  StorePaymentMode,
  StorePaymentStatus
} from '@/types'

const route = useRoute()
const router = useRouter()
const cartStore = useCartStore()
const userStore = useUserStore()
const walletStore = useWalletStore()

const loading = ref(true)
const product = ref<StoreProductDetailResult | null>(null)
const cartProcessing = computed(() => cartStore.processing)
const buyingNow = ref(false)
const orderPaymentDialog = ref(false)
const orderPaymentDetail = ref<StoreOrderDetailResult | null>(null)
const orderSuccessDialog = ref(false)
const orderSuccessDetail = ref<StoreOrderDetailResult | null>(null)

const userUid = computed(() => userStore.state.userInfo?.uid ?? null)

// Quantity selector
const quantity = ref(1)

const apiBaseUrl = WebApi.getInstance().baseUrl ?? ''

function resolveProductImage(product: StoreProductDetailResult) {
  const url = product.thumbnailUrl
  if (!url) {
    return productPlaceholderImage
  }
  if (url.startsWith('http')) {
    return url
  }
  if (apiBaseUrl) {
    const normalizedBase = apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl
    const normalizedPath = url.startsWith('/') ? url : `/${url}`
    return `${normalizedBase}${normalizedPath}`
  }
  return url
}

function formatDateTime(value?: string | null) {
  if (!value) return '-'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString()
}

function goBack() {
  router.back()
}

async function loadProduct() {
  const productId = Number(route.params.productId)
  if (!productId || Number.isNaN(productId)) {
    FastDialog.errorSnackbar('Invalid product ID')
    router.push({ name: 'Home' })
    return
  }

  loading.value = true
  try {
    product.value = await fetchProductDetail(productId)
  } catch (error) {
    console.error('Failed to load product:', error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to load product')
    product.value = null
  } finally {
    loading.value = false
  }
}

// Quantity control functions
function increaseQuantity() {
  if (product.value && quantity.value < product.value.inventoryAvailable) {
    quantity.value++
  }
}

function decreaseQuantity() {
  if (quantity.value > 1) {
    quantity.value--
  }
}

function handleQuantityInput(value: string | number) {
  const qty = Math.floor(Number(value))
  if (!Number.isFinite(qty) || qty < 1) {
    quantity.value = 1
    return
  }
  if (product.value && qty > product.value.inventoryAvailable) {
    FastDialog.warningSnackbar(`Maximum quantity is ${product.value.inventoryAvailable}`)
    quantity.value = product.value.inventoryAvailable
    return
  }
  quantity.value = qty
}

async function handleAddToCart() {
  if (!product.value) {
    return
  }

  if (product.value.inventoryAvailable <= 0) {
    FastDialog.warningSnackbar('Product is out of stock.')
    return
  }

  if (quantity.value <= 0) {
    FastDialog.warningSnackbar('Please select a valid quantity.')
    return
  }

  if (quantity.value > product.value.inventoryAvailable) {
    FastDialog.warningSnackbar(`Maximum quantity is ${product.value.inventoryAvailable}`)
    quantity.value = product.value.inventoryAvailable
    return
  }

  const uid = userUid.value
  if (!uid) {
    // Not logged in, add to temporary cart
    const { addToTemporaryCart, getTemporaryCartTotalQuantity } = await import('@/utils/temporaryCart')
    // Add multiple times if quantity > 1
    for (let i = 0; i < quantity.value; i++) {
      addToTemporaryCart(product.value.productId, 1)
    }
    const totalQuantity = getTemporaryCartTotalQuantity()
    FastDialog.successSnackbar(`Added ${quantity.value} item(s) to cart (${totalQuantity} total). Connect wallet to checkout.`)
    return
  }

  try {
    await cartStore.addItem(uid, product.value.productId, quantity.value)
    FastDialog.successSnackbar(`Added ${quantity.value} item(s) to cart`)
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to add to cart')
  }
}

// Guide login for add to cart functionality
async function handleLoginForAddToCart(): Promise<boolean> {
  // Check if wallet provider exists
  if (!walletStore.state.provider) {
    FastDialog.warningSnackbar('Please connect your wallet first. Redirecting to wallet setup page...')
    // Save current product ID to localStorage for return after login
    if (product.value) {
      localStorage.setItem('pendingProductId', product.value.productId.toString())
      localStorage.setItem('pendingAction', 'buyNow')
    }
    // Navigate to wallet registration/connection page
    router.push({ name: 'Go' })
    return false
  }

  // Check if wallet is connected
  if (!walletStore.state.active || !walletStore.state.address) {
    try {
      FastDialog.infoSnackbar('Connecting wallet...')
      const accounts = await walletStore.connect()
      if (!accounts || accounts.length === 0) {
        FastDialog.errorSnackbar('Failed to connect wallet. Please try again.')
        return false
      }
    } catch (error) {
      console.error('Wallet connection error:', error)
      FastDialog.errorSnackbar('Failed to connect wallet. Please check your wallet and try again.')
      return false
    }
  }

  // Check if already logged in
  const checkSignedResult = await userStore.checkSigned()
  if (!checkSignedResult.succeed) {
    FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
    return false
  }

  // If already logged in, return success directly
  if (checkSignedResult.data?.singined) {
    return true
  }

  // Need to sign in
  try {
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
      return false
    }

    // Get user info
    if (!(await userStore.updateUserInfo())) {
      userStore.signOut()
      FastDialog.errorSnackbar('Failed to get user information. Please try again.')
      return false
    }

    FastDialog.successSnackbar('Login successful!')
    return true
  } catch (error) {
    console.error('Login error:', error)
    if ((error as any)?.code === 4001) {
      FastDialog.warningSnackbar('Login cancelled. Please sign the message to add items to cart.')
    } else {
      FastDialog.errorSnackbar('Login failed. Please try again.')
    }
    return false
  }
}

async function handleBuyNow() {
  if (!product.value) {
    return
  }

  if (product.value.inventoryAvailable <= 0) {
    FastDialog.warningSnackbar('Product is out of stock.')
    return
  }

  const uid = userUid.value
  if (!uid) {
    // Not logged in, guide user to login
    const loginSuccess = await handleLoginForAddToCart()
    if (!loginSuccess) {
      return
    }
    // After successful login, get uid again and continue purchase flow
    const newUid = userUid.value
    if (newUid && product.value) {
      await proceedBuyNow(newUid)
    }
    return
  }

  await proceedBuyNow(uid)
}

async function proceedBuyNow(uid: number) {
  if (!product.value) {
    return
  }

  if (quantity.value <= 0) {
    FastDialog.warningSnackbar('Please select a valid quantity.')
    return
  }

  if (quantity.value > product.value.inventoryAvailable) {
    FastDialog.warningSnackbar(`Maximum quantity is ${product.value.inventoryAvailable}`)
    quantity.value = product.value.inventoryAvailable
    return
  }

  buyingNow.value = true
  try {
    // 1. First add product to cart with selected quantity
    await cartStore.addItem(uid, product.value.productId, quantity.value)
    
    // 2. Determine payment method (prefer Web3 if available)
    const walletState = walletStore.state
    const isWeb3Payment = walletState.active && walletState.provider !== null
    
    // 3. Create order immediately
    const order = await cartStore.placeOrder({
      uid,
      paymentMode: isWeb3Payment ? StorePaymentMode.Web3 : StorePaymentMode.Traditional,
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
      remark: 'Buy Now'
    })
    
    // 4. If Web3 payment and signature required, open payment dialog
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
    console.error('Buy now error:', error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to create order')
  } finally {
    buyingNow.value = false
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
}

function goToOrders() {
  router.push({ name: 'Orders' })
}

function formatWalletAddress(address: string): string {
  if (!address || address.length < 10) return address
  return `${address.substring(0, 6)}...${address.substring(address.length - 4)}`
}

function getOrderStatusColor(status: StoreOrderStatus): string {
  const statusMap: Record<StoreOrderStatus, string> = {
    [StoreOrderStatus.PendingPayment]: 'warning',
    [StoreOrderStatus.Paid]: 'primary',
    [StoreOrderStatus.Shipped]: 'info',
    [StoreOrderStatus.Delivered]: 'success',
    [StoreOrderStatus.Cancelled]: 'error',
    [StoreOrderStatus.Refunded]: 'grey'
  }
  return statusMap[status] ?? 'grey'
}

function getOrderStatusText(status: StoreOrderStatus): string {
  const statusMap: Record<StoreOrderStatus, string> = {
    [StoreOrderStatus.PendingPayment]: 'Pending Payment',
    [StoreOrderStatus.Paid]: 'Paid',
    [StoreOrderStatus.Shipped]: 'Shipped',
    [StoreOrderStatus.Delivered]: 'Delivered',
    [StoreOrderStatus.Cancelled]: 'Cancelled',
    [StoreOrderStatus.Refunded]: 'Refunded'
  }
  return statusMap[status] ?? 'Unknown'
}

function getPaymentStatusColor(status: StorePaymentStatus): string {
  const statusMap: Record<StorePaymentStatus, string> = {
    [StorePaymentStatus.PendingSignature]: 'warning',
    [StorePaymentStatus.AwaitingOnChainConfirmation]: 'info',
    [StorePaymentStatus.Confirmed]: 'success',
    [StorePaymentStatus.Failed]: 'error',
    [StorePaymentStatus.Cancelled]: 'grey'
  }
  return statusMap[status] ?? 'grey'
}

function getPaymentStatusText(status: StorePaymentStatus): string {
  const statusMap: Record<StorePaymentStatus, string> = {
    [StorePaymentStatus.PendingSignature]: 'Pending Signature',
    [StorePaymentStatus.AwaitingOnChainConfirmation]: 'Awaiting Confirmation',
    [StorePaymentStatus.Confirmed]: 'Confirmed',
    [StorePaymentStatus.Failed]: 'Failed',
    [StorePaymentStatus.Cancelled]: 'Cancelled'
  }
  return statusMap[status] ?? 'Unknown'
}

onBeforeMount(async () => {
  await loadProduct()
  
  // Reset quantity when product loads
  quantity.value = 1
  
  // Check if there is auto-buy parameter (when returning from wallet registration page)
  const autoBuy = route.query.autoBuy === 'true'
  if (autoBuy && product.value && userUid.value) {
    // Delay a bit to ensure page is fully loaded
    setTimeout(() => {
      handleBuyNow()
    }, 500)
  }
})
</script>

<style scoped>
.quantity-controls {
  border: 1px solid rgba(var(--v-theme-primary), 0.3);
  border-radius: 4px;
  padding: 2px;
  background-color: rgba(var(--v-theme-primary), 0.05);
}

.quantity-input :deep(.v-field__input) {
  text-align: center;
  font-weight: 500;
}

.quantity-controls .v-btn {
  min-width: 36px;
}
</style>

