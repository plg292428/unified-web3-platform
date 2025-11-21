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
        <!-- Loading Temporary Cart -->
        <template v-if="temporaryCartLoading">
          <v-card class="primary-border mt-4" variant="outlined">
            <v-card-text>
              <v-skeleton-loader type="list-item-three-line@3"></v-skeleton-loader>
            </v-card-text>
          </v-card>
        </template>

        <!-- Empty Temporary Cart -->
        <template v-else-if="cartItems.length === 0">
          <v-card class="primary-border mt-4" variant="outlined">
            <v-card-text class="text-center py-8">
              <v-icon size="64" color="primary" class="mb-4">mdi-wallet</v-icon>
              <div class="text-h6 mt-4 text-grey-lighten-1">Ready to Pay with Web3</div>
              <div class="text-body-2 text-grey-lighten-2 mt-2">
                Your cart is empty. Add items to your cart and connect your wallet to proceed with payment.
              </div>
              <v-btn color="primary" variant="tonal" class="mt-4" @click="goToHome">
                <v-icon start>mdi-shopping</v-icon>
                Go Shopping
              </v-btn>
            </v-card-text>
          </v-card>
        </template>

        <!-- Temporary Cart with Items -->
        <template v-else>
          <v-card class="primary-border mt-4" variant="outlined">
            <v-card-title class="d-flex align-center">
              <span class="text-subtitle-1">Temporary Cart</span>
              <v-spacer></v-spacer>
              <v-chip size="small" color="info" variant="tonal">
                Connect wallet to checkout
              </v-chip>
            </v-card-title>
            <v-divider></v-divider>
            <v-card-text>
              <v-alert type="info" variant="tonal" class="mb-4">
                <div class="text-caption">
                  Items are saved in your browser. Connect your wallet to create an account and proceed with payment.
                </div>
              </v-alert>
              <v-list density="comfortable">
                <v-list-item
                  v-for="item in cartItems"
                  :key="item.cartItemId"
                  :value="item.cartItemId"
                >
                  <template #prepend>
                    <v-avatar size="64" rounded="lg" class="mr-4">
                      <v-img
                        :src="resolveProductImageForCart(item)"
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
                    <div class="d-flex align-center quantity-controls">
                      <v-btn
                        icon="mdi-minus"
                        variant="outlined"
                        size="small"
                        color="primary"
                        :disabled="temporaryCartLoading || item.quantity <= 1"
                        @click="handleUpdateQuantity(item, item.quantity - 1)"
                      ></v-btn>
                      <v-text-field
                        class="mx-2 quantity-input"
                        style="max-width: 90px"
                        type="number"
                        density="compact"
                        variant="outlined"
                        hide-details
                        :model-value="item.quantity"
                        :disabled="temporaryCartLoading"
                        :min="1"
                        :max="item.inventoryAvailable"
                        @update:model-value="(value: number) => handleQuantityInput(item, value)"
                      />
                      <v-btn
                        icon="mdi-plus"
                        variant="outlined"
                        size="small"
                        color="primary"
                        :disabled="temporaryCartLoading || item.quantity >= item.inventoryAvailable"
                        @click="handleUpdateQuantity(item, item.quantity + 1)"
                      ></v-btn>
                    </div>
                    <v-btn
                      icon="mdi-delete"
                      color="error"
                      variant="text"
                      size="small"
                      class="ml-3"
                      :disabled="temporaryCartLoading"
                      @click="handleRemoveItem(item)"
                    ></v-btn>
                  </div>
                </template>
                </v-list-item>
              </v-list>
            </v-card-text>
          </v-card>

          <!-- Order Summary for Temporary Cart -->
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
                :disabled="placingOrder || temporaryCartLoading"
              />
            </v-card-text>
            
            <v-card-actions class="pa-4">
              <v-btn
                color="primary"
                size="large"
                block
                :loading="placingOrder"
                :disabled="cartTotalQuantity === 0 || !walletStore.state.provider"
                append-icon="mdi-arrow-right"
                @click="handlePlaceOrder"
              >
                <v-icon start>mdi-wallet</v-icon>
                Connect Wallet & Pay
              </v-btn>
            </v-card-actions>
            <v-card-text v-if="!walletStore.state.provider" class="text-center">
              <v-chip size="small" color="warning" variant="tonal" class="mb-2">
                No wallet detected. Please install a wallet first.
              </v-chip>
              <div class="mt-3">
                <v-btn
                  size="small"
                  variant="outlined"
                  color="primary"
                  class="mr-2"
                  @click="installWallet('metamask')"
                >
                  <v-icon start size="small">mdi-wallet</v-icon>
                  Install MetaMask
                </v-btn>
                <v-btn
                  size="small"
                  variant="outlined"
                  color="primary"
                  @click="installWallet('bitget')"
                >
                  <v-icon start size="small">mdi-wallet</v-icon>
                  Install Bitget Wallet
                </v-btn>
              </div>
            </v-card-text>
          </v-card>
        </template>
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
                      :src="resolveProductImageForCart(item)"
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
                    <div class="d-flex align-center quantity-controls">
                      <v-btn
                        icon="mdi-minus"
                        variant="outlined"
                        size="small"
                        color="primary"
                        :disabled="cartStore.processing || item.quantity <= 1"
                        @click="handleUpdateQuantity(item, item.quantity - 1)"
                      ></v-btn>
                      <v-text-field
                        class="mx-2 quantity-input"
                        style="max-width: 90px"
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
                        variant="outlined"
                        size="small"
                        color="primary"
                        :disabled="cartStore.processing || item.quantity >= item.inventoryAvailable"
                        @click="handleUpdateQuantity(item, item.quantity + 1)"
                      ></v-btn>
                    </div>
                    <v-btn
                      icon="mdi-delete"
                      color="error"
                      variant="text"
                      size="small"
                      class="ml-3"
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

        <!-- Product List Section -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-store" class="mr-2" color="primary"></v-icon>
            <span>Add More Products</span>
            <v-spacer></v-spacer>
            <v-btn
              icon
              size="small"
              variant="text"
              @click="showProductList = !showProductList"
            >
              <v-icon>{{ showProductList ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
            </v-btn>
          </v-card-title>
          <v-expand-transition>
            <div v-show="showProductList">
              <v-divider></v-divider>
              <v-card-text>
                <!-- Product Loading -->
                <div v-if="productListLoading" class="text-center py-8">
                  <v-progress-circular indeterminate color="primary"></v-progress-circular>
                  <div class="text-body-2 text-grey-lighten-1 mt-2">Loading products...</div>
                </div>

                <!-- Product Grid -->
                <v-row v-else-if="productList.length > 0" dense>
                  <v-col
                    v-for="product in productList"
                    :key="product.productId"
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <v-card
                      variant="outlined"
                      class="product-card"
                      :class="{ 'product-in-cart': isProductInCart(product.productId) }"
                    >
                      <v-img
                        :src="resolveProductImage(product)"
                        height="120"
                        cover
                        class="bg-grey-darken-3"
                        @click="viewProductDetail(product.productId)"
                        style="cursor: pointer;"
                      >
                        <template v-slot:placeholder>
                          <div class="d-flex align-center justify-center fill-height">
                            <v-icon color="grey">mdi-image</v-icon>
                          </div>
                        </template>
                      </v-img>
                      <v-card-text class="pa-3">
                        <div class="text-subtitle-2 font-weight-medium mb-1" style="min-height: 40px;">
                          {{ product.name }}
                        </div>
                        <div class="text-body-2 text-primary font-weight-bold mb-2">
                          {{ Filter.formatPrice(product.price) }} {{ product.currency }}
                        </div>
                        <div class="d-flex align-center">
                          <v-chip
                            v-if="product.inventoryAvailable > 0"
                            size="x-small"
                            color="success"
                            variant="tonal"
                          >
                            Stock: {{ product.inventoryAvailable }}
                          </v-chip>
                          <v-chip
                            v-else
                            size="x-small"
                            color="error"
                            variant="tonal"
                          >
                            Out of Stock
                          </v-chip>
                        </div>
                      </v-card-text>
                      <v-card-actions class="pa-3 pt-0">
                        <!-- If product is in cart, show quantity controls -->
                        <template v-if="isProductInCart(product.productId)">
                          <div class="d-flex align-center w-100">
                            <v-btn
                              icon="mdi-minus"
                              variant="outlined"
                              size="small"
                              color="primary"
                              :disabled="cartStore.processing || getCartItemQuantity(product.productId) <= 1"
                              @click="decreaseProductQuantity(product)"
                            ></v-btn>
                            <v-text-field
                              class="mx-2 quantity-input"
                              style="max-width: 70px"
                              type="number"
                              density="compact"
                              variant="outlined"
                              hide-details
                              :model-value="getCartItemQuantity(product.productId)"
                              :disabled="cartStore.processing || product.inventoryAvailable <= 0"
                              :min="1"
                              :max="product.inventoryAvailable"
                              @update:model-value="(value: number) => updateProductQuantity(product, value)"
                            />
                            <v-btn
                              icon="mdi-plus"
                              variant="outlined"
                              size="small"
                              color="primary"
                              :disabled="cartStore.processing || getCartItemQuantity(product.productId) >= product.inventoryAvailable"
                              @click="increaseProductQuantity(product)"
                            ></v-btn>
                            <v-spacer></v-spacer>
                            <v-btn
                              icon="mdi-delete"
                              color="error"
                              variant="text"
                              size="small"
                              :disabled="cartStore.processing"
                              @click="removeProductFromCart(product)"
                            ></v-btn>
                          </div>
                        </template>
                        <!-- If product is not in cart, show add button -->
                        <v-btn
                          v-else
                          color="primary"
                          variant="flat"
                          size="small"
                          block
                          :disabled="cartStore.processing || product.inventoryAvailable <= 0"
                          @click="addProductToCart(product)"
                        >
                          <v-icon start size="18">mdi-cart-plus</v-icon>
                          Add to Cart
                        </v-btn>
                      </v-card-actions>
                    </v-card>
                  </v-col>
                </v-row>

                <!-- Empty Product List -->
                <div v-else class="text-center py-8">
                  <v-icon size="64" color="grey">mdi-package-variant</v-icon>
                  <div class="text-body-1 text-grey-lighten-1 mt-2">No products available</div>
                </div>

                <!-- Pagination -->
                <v-pagination
                  v-if="productTotalPages > 1"
                  v-model="productPage"
                  :length="productTotalPages"
                  :total-visible="5"
                  class="mt-4"
                  @update:model-value="loadProducts"
                ></v-pagination>
              </v-card-text>
            </div>
          </v-expand-transition>
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
import { fetchProductList } from '@/services/storeApi'
import type {
  StoreCartItemResult,
  StoreOrderDetailResult,
  StoreOrderStatus,
  StorePaymentMode,
  StorePaymentStatus,
  StoreProductSummaryResult,
  StoreProductListResult
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

// Product list for adding to cart
const showProductList = ref(false)
const productList = ref<StoreProductSummaryResult[]>([])
const productListLoading = ref(false)
const productPage = ref(1)
const productPageSize = ref(6)
const productTotal = ref(0)
const productTotalPages = computed(() => {
  const total = Math.ceil(productTotal.value / productPageSize.value)
  return total > 0 ? total : 1
})

// Temporary cart for unauthenticated users
const temporaryCartItems = ref<Array<{ productId: number; quantity: number; product?: any }>>([])
const temporaryCartLoading = ref(false)

// Load temporary cart items
async function loadTemporaryCartItems() {
  if (userUid.value) {
    temporaryCartItems.value = []
    return
  }
  
  temporaryCartLoading.value = true
  try {
    const { getTemporaryCartItems } = await import('@/utils/temporaryCart')
    const items = getTemporaryCartItems()
    
    // Fetch product details for temporary cart items
    const { fetchProductDetail } = await import('@/services/storeApi')
    const productPromises = items.map(async (item) => {
      try {
        const product = await fetchProductDetail(item.productId)
        return {
          productId: item.productId,
          quantity: item.quantity,
          product
        }
      } catch (error) {
        console.error(`Failed to load product ${item.productId}:`, error)
        return null
      }
    })
    
    const results = await Promise.all(productPromises)
    temporaryCartItems.value = results.filter((item): item is NonNullable<typeof item> => item !== null)
  } catch (error) {
    console.error('Failed to load temporary cart items:', error)
  } finally {
    temporaryCartLoading.value = false
  }
}

// Combined cart items (user cart + temporary cart)
const cartItems = computed(() => {
  if (userUid.value) {
    return cartStore.items
  }
  // For unauthenticated users, return temporary cart items formatted as cart items
  return temporaryCartItems.value.map((item) => ({
    cartItemId: `temp_${item.productId}`, // Temporary ID
    productId: item.productId,
    productName: item.product?.name ?? 'Unknown Product',
    unitPrice: item.product?.price ?? 0,
    quantity: item.quantity,
    subtotal: (item.product?.price ?? 0) * item.quantity,
    currency: item.product?.currency ?? 'USDT',
    inventoryAvailable: item.product?.inventoryAvailable ?? 0,
    thumbnailUrl: item.product?.thumbnailUrl
  }))
})

const cartTotalAmount = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.subtotal, 0)
})

const cartTotalQuantity = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.quantity, 0)
})

const cartCurrency = computed(() => cartItems.value[0]?.currency ?? 'USDT')

const apiBaseUrl = WebApi.getInstance().baseUrl ?? ''

// Product image resolver for cart items
function resolveProductImageForCart(item: StoreCartItemResult) {
  if (item.thumbnailUrl) {
    if (item.thumbnailUrl.startsWith('http')) {
      return item.thumbnailUrl
    }
    if (apiBaseUrl) {
      const normalizedBase = apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl
      const normalizedPath = item.thumbnailUrl.startsWith('/') ? item.thumbnailUrl : `/${item.thumbnailUrl}`
      return `${normalizedBase}${normalizedPath}`
    }
    return item.thumbnailUrl
  }
  return productPlaceholderImage
}

// Product image resolver for product list
function resolveProductImage(product: StoreProductSummaryResult | StoreCartItemResult) {
  const url = 'thumbnailUrl' in product ? product.thumbnailUrl : (product as StoreCartItemResult).thumbnailUrl
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

function goBack() {
  router.back()
}

function goToHome() {
  router.push({ name: 'Home' })
}

function goToOrders() {
  router.push({ name: 'Orders' })
}

function installWallet(walletType: 'metamask' | 'bitget') {
  if (walletType === 'metamask') {
    window.open('https://metamask.io/download/', '_blank', 'noopener')
  } else if (walletType === 'bitget') {
    window.open('https://web3.bitget.com/en/tools/wallet', '_blank', 'noopener')
  }
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
    // Update temporary cart
    if (item.cartItemId.toString().startsWith('temp_')) {
      const { updateTemporaryCartItem } = await import('@/utils/temporaryCart')
      const productId = parseInt(item.cartItemId.toString().replace('temp_', ''))
      updateTemporaryCartItem(productId, nextQuantity)
      await loadTemporaryCartItems()
      return
    }
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
    // If invalid input, restore to current quantity (will be handled by v-model)
    return
  }
  // Validate against inventory
  if (quantity > item.inventoryAvailable) {
    FastDialog.warningSnackbar(`Maximum quantity is ${item.inventoryAvailable}`)
    return
  }
  handleUpdateQuantity(item, quantity)
}

async function handleRemoveItem(item: StoreCartItemResult) {
  const uid = userUid.value
  if (!uid) {
    // Remove from temporary cart
    if (item.cartItemId.toString().startsWith('temp_')) {
      const { removeFromTemporaryCart } = await import('@/utils/temporaryCart')
      const productId = parseInt(item.cartItemId.toString().replace('temp_', ''))
      removeFromTemporaryCart(productId)
      await loadTemporaryCartItems()
      FastDialog.successSnackbar('Item removed')
      return
    }
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

/**
 * Auto-login user if not logged in (for Web3 payment)
 * This allows users to pay directly without pre-registration
 */
async function autoLoginForPayment(): Promise<number | null> {
  // Check if already logged in
  const currentUid = userUid.value
  if (currentUid) {
    return currentUid
  }

  // For Web3 payment, we can auto-create/login user
  if (selectedPaymentMode.value !== StorePaymentMode.Web3) {
    FastDialog.warningSnackbar('Please login first for traditional payment.')
    return null
  }

  const walletState = walletStore.state

    // Check if wallet provider exists
    if (!walletState.provider) {
      console.log('No wallet provider detected, redirecting to Go page')
      FastDialog.warningSnackbar('No wallet detected. Please install MetaMask or Bitget Wallet to continue.')
      await nextTick()
      await new Promise(resolve => setTimeout(resolve, 500))
      // Redirect to Go page which has wallet installation options
      window.location.replace('/go')
      return null
    }

  // Check if wallet is connected
  if (!walletState.active || !walletState.address) {
    try {
      FastDialog.infoSnackbar('Connecting wallet...')
      const accounts = await walletStore.connect()
      if (!accounts || accounts.length === 0) {
        FastDialog.errorSnackbar('Failed to connect wallet. Please unlock your wallet and try again.')
        return null
      }
    } catch (error: any) {
      console.error('Wallet connection error:', error)
      if (error.code === 4001) {
        FastDialog.warningSnackbar('Connection cancelled. Please approve the connection request to continue.')
      } else if (error.code === -32002) {
        FastDialog.warningSnackbar('Connection request already pending. Please check your wallet.')
      } else {
        FastDialog.errorSnackbar('Failed to connect wallet. Please check your wallet and try again.')
      }
      return null
    }
  }

  // Auto-login: check signed status and sign in if needed
  try {
    const checkSignedResult = await userStore.checkSigned()
    if (!checkSignedResult.succeed) {
      FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
      return null
    }

    // If not logged in, sign in automatically
    if (!checkSignedResult.data?.singined) {
      FastDialog.infoSnackbar('Creating account and signing in...')
      const from = walletState.address
      const provider = walletState.provider
      const message = `0x${checkSignedResult.data.tokenText}`
      
      const signedText = await provider.request({
        method: 'personal_sign',
        params: [message, from]
      })

      const signResult = await userStore.signIn(signedText)
      if (!signResult.succeed) {
        FastDialog.errorSnackbar(signResult.errorMessage as string)
        return null
      }

      if (!(await userStore.updateUserInfo())) {
        userStore.signOut()
        FastDialog.errorSnackbar('Failed to get user information. Please try again.')
        return null
      }

      // Transfer temporary cart to user account
      const newUid = userUid.value
      if (newUid) {
        const { transferTemporaryCartToUser } = await import('@/utils/temporaryCart')
        const tempItems = transferTemporaryCartToUser()
        
        // Add temporary cart items to user cart
        if (tempItems.length > 0) {
          try {
            for (const tempItem of tempItems) {
              await cartStore.addItem(newUid, tempItem.productId, tempItem.quantity)
            }
            FastDialog.successSnackbar(`Account created! ${tempItems.length} item(s) transferred to your cart.`)
          } catch (error) {
            console.error('Failed to transfer temporary cart items:', error)
            FastDialog.warningSnackbar('Account created, but some items could not be transferred. Please add them again.')
          }
        }
        
        await cartStore.refresh(newUid)
        await loadTemporaryCartItems() // Clear temporary cart display
        if (tempItems.length === 0) {
          FastDialog.successSnackbar('Account created and logged in successfully!')
        }
        return newUid
      }
    } else {
      // Already logged in, refresh cart
      const uid = userUid.value
      if (uid) {
        await cartStore.refresh(uid)
        return uid
      }
    }
  } catch (error: any) {
    console.error('Auto-login error:', error)
    if (error.code === 4001) {
      FastDialog.warningSnackbar('Signing cancelled. Please sign the message to continue payment.')
    } else {
      FastDialog.errorSnackbar('Auto-login failed. Please try again.')
    }
    return null
  }

  return null
}

async function handlePlaceOrder() {
  if (cartItems.value.length === 0) {
    FastDialog.warningSnackbar('Your cart is empty.')
    return
  }
  
  // Auto-login if not logged in (for Web3 payment)
  const uid = await autoLoginForPayment()
  if (!uid) {
    // Auto-login failed or cancelled
    return
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
      FastDialog.warningSnackbar('No wallet detected. Please install MetaMask or Bitget Wallet to continue.')
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
      
      // Auto-enter payment flow if cart has items
      if (cartStore.items.length > 0) {
        // Small delay to ensure UI is ready
        await nextTick()
        await new Promise(resolve => setTimeout(resolve, 500))
        await autoEnterPaymentFlow()
      }
    } catch (error) {
      console.error('Failed to load cart:', error)
      FastDialog.errorSnackbar((error as Error).message ?? 'Failed to load cart')
    }
  } else {
    // Not logged in, load temporary cart
    await loadTemporaryCartItems()
  }
  
  // Load product list when product list section is expanded
  // We'll load it on first expansion to save resources
})

/**
 * Auto-enter payment flow when cart has items
 */
async function autoEnterPaymentFlow() {
  // Check wallet first
  if (!walletStore.state.provider) {
    console.log('No wallet detected, redirecting to wallet setup page')
    FastDialog.warningSnackbar('No wallet detected. Redirecting to wallet setup page...')
    await nextTick()
    await new Promise(resolve => setTimeout(resolve, 500))
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

  // Check if logged in
  const checkSignedResult = await userStore.checkSigned()
  if (!checkSignedResult.succeed) {
    FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
    return
  }

  // If not logged in, need to sign in first
  if (!checkSignedResult.data?.singined) {
    try {
      FastDialog.infoSnackbar('Please sign the message in your wallet to login...')
      const from = walletStore.state.address
      const provider = walletStore.state.provider
      const message = `0x${checkSignedResult.data.tokenText}`
      
      const signedText = await provider.request({
        method: 'personal_sign',
        params: [message, from]
      })

      const signResult = await userStore.signIn(signedText)
      if (!signResult.succeed) {
        FastDialog.errorSnackbar(signResult.errorMessage as string)
        return
      }

      if (!(await userStore.updateUserInfo())) {
        userStore.signOut()
        FastDialog.errorSnackbar('Failed to get user information. Please try again.')
        return
      }

      // Refresh cart after login
      const uid = userUid.value
      if (uid) {
        await cartStore.refresh(uid)
      }
    } catch (error: any) {
      console.error('Auto-login error:', error)
      if (error.code === 4001) {
        FastDialog.warningSnackbar('Login cancelled. Please sign the message to continue.')
      } else {
        FastDialog.errorSnackbar('Login failed. Please try again.')
      }
      return
    }
  }

  // Now proceed to payment - automatically place order
  if (cartStore.items.length > 0 && userUid.value) {
    // Auto-place order to enter payment flow
    await handlePlaceOrder()
  }
}

// Load products
async function loadProducts() {
  productListLoading.value = true
  try {
    const result: StoreProductListResult = await fetchProductList({
      page: productPage.value,
      pageSize: productPageSize.value
    })
    productList.value = result.items || []
    productTotal.value = result.totalCount || 0
  } catch (error) {
    console.error('Failed to load products:', error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to load products')
    productList.value = []
    productTotal.value = 0
  } finally {
    productListLoading.value = false
  }
}

// Check if product is in cart
function isProductInCart(productId: number): boolean {
  return cartItems.value.some(item => item.productId === productId)
}

// Get cart item quantity for a product
function getCartItemQuantity(productId: number): number {
  const item = cartItems.value.find(item => item.productId === productId)
  return item?.quantity ?? 0
}

// Add product to cart
async function addProductToCart(product: StoreProductSummaryResult) {
  const uid = userUid.value
  if (!uid) {
    // Add to temporary cart
    const { addToTemporaryCart } = await import('@/utils/temporaryCart')
    addToTemporaryCart(product, 1)
    await loadTemporaryCartItems()
    FastDialog.successSnackbar('Added to cart')
    return
  }
  try {
    await cartStore.addItem(uid, product.productId, 1)
    FastDialog.successSnackbar('Added to cart')
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to add to cart')
  }
}

// Increase product quantity
async function increaseProductQuantity(product: StoreProductSummaryResult) {
  const currentQuantity = getCartItemQuantity(product.productId)
  if (currentQuantity >= product.inventoryAvailable) {
    FastDialog.warningSnackbar(`Maximum quantity is ${product.inventoryAvailable}`)
    return
  }
  await updateProductQuantity(product, currentQuantity + 1)
}

// Decrease product quantity
async function decreaseProductQuantity(product: StoreProductSummaryResult) {
  const currentQuantity = getCartItemQuantity(product.productId)
  if (currentQuantity <= 1) {
    await removeProductFromCart(product)
    return
  }
  await updateProductQuantity(product, currentQuantity - 1)
}

// Update product quantity
async function updateProductQuantity(product: StoreProductSummaryResult, quantity: number) {
  const quantityNum = Math.floor(Number(quantity))
  if (!Number.isFinite(quantityNum) || quantityNum < 1) {
    return
  }
  if (quantityNum > product.inventoryAvailable) {
    FastDialog.warningSnackbar(`Maximum quantity is ${product.inventoryAvailable}`)
    return
  }
  
  const uid = userUid.value
  if (!uid) {
    // Update temporary cart
    const { updateTemporaryCartItem } = await import('@/utils/temporaryCart')
    updateTemporaryCartItem(product.productId, quantityNum)
    await loadTemporaryCartItems()
    return
  }
  
  const cartItem = cartItems.value.find(item => item.productId === product.productId)
  if (!cartItem) {
    // If not in cart, add it
    await addProductToCart(product)
    return
  }
  
  try {
    await cartStore.updateItem(uid, cartItem.cartItemId, quantityNum)
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to update quantity')
  }
}

// Remove product from cart
async function removeProductFromCart(product: StoreProductSummaryResult) {
  const uid = userUid.value
  if (!uid) {
    // Remove from temporary cart
    const { removeTemporaryCartItem } = await import('@/utils/temporaryCart')
    removeTemporaryCartItem(product.productId)
    await loadTemporaryCartItems()
    FastDialog.successSnackbar('Item removed')
    return
  }
  
  const cartItem = cartItems.value.find(item => item.productId === product.productId)
  if (!cartItem) {
    return
  }
  
  try {
    await cartStore.removeItem(uid, cartItem.cartItemId)
    FastDialog.successSnackbar('Item removed')
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to remove item')
  }
}

// View product detail
function viewProductDetail(productId: number) {
  router.push({ name: 'ProductDetail', params: { productId } })
}

// Toggle product list and load if needed
async function toggleProductList() {
  showProductList.value = !showProductList.value
  if (showProductList.value && productList.value.length === 0 && !productListLoading.value) {
    await loadProducts()
  }
}
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

.product-card {
  transition: all 0.2s ease;
}

.product-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.product-in-cart {
  border-color: rgb(var(--v-theme-primary));
  background-color: rgba(var(--v-theme-primary), 0.05);
}

.product-card .quantity-input :deep(.v-field__input) {
  text-align: center;
  font-weight: 500;
}
</style>

