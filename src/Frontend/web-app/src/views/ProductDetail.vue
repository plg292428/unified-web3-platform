<template>
  <v-container>
    <v-responsive>
      <!-- 返回按钮 -->
      <div class="mt-2 d-flex align-center">
        <v-btn icon="mdi-arrow-left" variant="text" @click="goBack"></v-btn>
        <span class="text-subtitle-2">Product Details</span>
      </div>

      <!-- 加载中 -->
      <template v-if="loading">
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text>
            <v-skeleton-loader type="image, article, actions"></v-skeleton-loader>
          </v-card-text>
        </v-card>
      </template>

      <!-- 产品详情 -->
      <template v-else-if="product">
        <!-- 产品图片 -->
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

        <!-- 产品信息 -->
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-title class="text-h6">{{ product.name }}</v-card-title>
          <v-card-subtitle class="text-caption text-grey-lighten-1">
            {{ product.subtitle ?? product.categoryName }}
          </v-card-subtitle>
          <v-card-text>
            <!-- 价格 -->
            <div class="d-flex align-center mb-4">
              <span class="text-h5 text-primary font-weight-medium">
                {{ Filter.formatToken(product.price) }} {{ product.currency }}
              </span>
              <v-chip v-if="product.inventoryAvailable > 0" class="ml-2" size="small" color="success" variant="tonal">
                In Stock
              </v-chip>
              <v-chip v-else class="ml-2" size="small" color="error" variant="tonal">
                Out of Stock
              </v-chip>
            </div>

            <!-- 库存信息 -->
            <div class="text-body-2 text-grey-lighten-1 mb-4">
              <div>Stock Available: {{ product.inventoryAvailable }}</div>
              <div v-if="product.sku" class="mt-1">SKU: {{ product.sku }}</div>
            </div>

            <!-- 分类面包屑 -->
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

            <!-- 产品描述 -->
            <div v-if="product.description" class="text-body-2">
              <div class="text-subtitle-2 mb-2">Description</div>
              <div class="text-grey-lighten-2" style="white-space: pre-wrap;">{{ product.description }}</div>
            </div>
          </v-card-text>

          <v-divider></v-divider>

          <v-card-actions class="pa-4">
            <v-btn
              color="primary"
              size="large"
              block
              append-icon="mdi-cart-plus"
              :loading="cartProcessing"
              :disabled="product.inventoryAvailable <= 0"
              @click="handleAddToCart"
            >
              Add to Cart
            </v-btn>
          </v-card-actions>
        </v-card>

        <!-- 产品信息卡片 -->
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

      <!-- 错误状态 -->
      <template v-else>
        <v-card class="primary-border mt-4" variant="outlined">
          <v-card-text class="text-center">
            <v-img height="96" src="@/assets/no_data.svg" class="mx-auto"></v-img>
            <div class="text-grey-lighten-1 text-subtitle-2 mt-4">Product not found</div>
            <v-btn color="primary" variant="tonal" class="mt-4" @click="goBack">Go Back</v-btn>
          </v-card-text>
        </v-card>
      </template>
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
import { fetchProductDetail } from '@/services/storeApi'
import type { StoreProductDetailResult } from '@/types'

const route = useRoute()
const router = useRouter()
const cartStore = useCartStore()
const userStore = useUserStore()
const walletStore = useWalletStore()

const loading = ref(true)
const product = ref<StoreProductDetailResult | null>(null)
const cartProcessing = computed(() => cartStore.processing)

const userUid = computed(() => userStore.state.userInfo?.uid ?? null)

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

async function handleAddToCart() {
  if (!product.value) {
    return
  }

  if (product.value.inventoryAvailable <= 0) {
    FastDialog.warningSnackbar('Product is out of stock.')
    return
  }

  const uid = userUid.value
  if (!uid) {
    // 未登录，引导用户登录
    const loginSuccess = await handleLoginForAddToCart()
    if (!loginSuccess) {
      return
    }
    // 登录成功后，重新获取 uid 并添加到购物车
    const newUid = userUid.value
    if (newUid && product.value) {
      try {
        await cartStore.addItem(newUid, product.value.productId, 1)
        FastDialog.successSnackbar('Added to cart')
      } catch (error) {
        console.warn(error)
        FastDialog.errorSnackbar((error as Error).message ?? 'Failed to add to cart')
      }
    }
    return
  }

  try {
    await cartStore.addItem(uid, product.value.productId, 1)
    FastDialog.successSnackbar('Added to cart')
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to add to cart')
  }
}

// 为添加到购物车功能引导登录
async function handleLoginForAddToCart(): Promise<boolean> {
  // 检查是否有钱包提供方
  if (!walletStore.state.provider) {
    FastDialog.warningSnackbar('Please connect your wallet first. If you don\'t have a wallet, please install MetaMask or Bitget Wallet.')
    return false
  }

  // 检查钱包是否已连接账户
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

  // 检查是否已登录
  const checkSignedResult = await userStore.checkSigned()
  if (!checkSignedResult.succeed) {
    FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
    return false
  }

  // 如果已登录，直接返回成功
  if (checkSignedResult.data?.singined) {
    return true
  }

  // 需要签名登录
  try {
    FastDialog.infoSnackbar('Please sign the message in your wallet to login...')
    const from = walletStore.state.address
    const provider = walletStore.state.provider
    const message = `0x${checkSignedResult.data.tokenText}`
    
    const signedText = await provider.request({
      method: 'personal_sign',
      params: [message, from]
    })

    // 执行登录
    const signResult = await userStore.signIn(signedText)
    if (!signResult.succeed) {
      FastDialog.errorSnackbar(signResult.errorMessage as string)
      return false
    }

    // 获取用户信息
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

onBeforeMount(async () => {
  await loadProduct()
})
</script>

