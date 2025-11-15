import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import type {
  StoreCartItemResult,
  StoreCartListResult,
  StoreOrderCreatePayload,
  StoreOrderDetailResult,
  StoreOrderPreparePaymentRequest,
  StoreOrderPreparePaymentResult,
  StoreOrderPaymentStatusResult,
  StoreOrderCancelRequest
} from '@/types'
import {
  addCartItem,
  createOrder,
  fetchCartList,
  removeCartItem,
  updateCartItem,
  preparePayment,
  getPaymentStatus,
  cancelOrder
} from '@/services/storeApi'

export const useCartStore = defineStore('cart', () => {
  const items = ref<StoreCartItemResult[]>([])
  const loading = ref(false)
  const processing = ref(false)
  const lastOrder = ref<StoreOrderDetailResult | null>(null)

  const totalAmount = computed(() => items.value.reduce((sum, item) => sum + item.subtotal, 0))
  const totalQuantity = computed(() => items.value.reduce((sum, item) => sum + item.quantity, 0))

  async function refresh(uid: number): Promise<StoreCartListResult> {
    loading.value = true
    try {
      const result = await fetchCartList(uid)
      items.value = result.items
      return result
    } finally {
      loading.value = false
    }
  }

  async function addItem(uid: number, productId: number, quantity = 1): Promise<StoreCartListResult> {
    processing.value = true
    try {
      const result = await addCartItem({ uid, productId, quantity })
      items.value = result.items
      return result
    } finally {
      processing.value = false
    }
  }

  async function updateItem(uid: number, cartItemId: number, quantity: number): Promise<StoreCartListResult> {
    processing.value = true
    try {
      const result = await updateCartItem(cartItemId, { uid, cartItemId, quantity })
      items.value = result.items
      return result
    } finally {
      processing.value = false
    }
  }

  async function removeItem(uid: number, cartItemId: number): Promise<StoreCartListResult> {
    processing.value = true
    try {
      const result = await removeCartItem(cartItemId, uid)
      items.value = result.items
      return result
    } finally {
      processing.value = false
    }
  }

  async function placeOrder(payload: StoreOrderCreatePayload): Promise<StoreOrderDetailResult> {
    processing.value = true
    try {
      const result = await createOrder(payload)
      lastOrder.value = result
      items.value = []
      return result
    } finally {
      processing.value = false
    }
  }

  async function prepareOrderPayment(
    orderId: number,
    payload: StoreOrderPreparePaymentRequest
  ): Promise<StoreOrderPreparePaymentResult> {
    processing.value = true
    try {
      return await preparePayment(orderId, payload)
    } finally {
      processing.value = false
    }
  }

  async function checkPaymentStatus(
    orderId: number,
    uid: number
  ): Promise<StoreOrderPaymentStatusResult> {
    return await getPaymentStatus(orderId, uid)
  }

  async function cancelOrderById(
    orderId: number,
    payload: StoreOrderCancelRequest
  ): Promise<boolean> {
    processing.value = true
    try {
      return await cancelOrder(orderId, payload)
    } finally {
      processing.value = false
    }
  }

  function reset(): void {
    items.value = []
    lastOrder.value = null
  }

  return {
    items,
    loading,
    processing,
    totalAmount,
    totalQuantity,
    lastOrder,
    refresh,
    addItem,
    updateItem,
    removeItem,
    placeOrder,
    prepareOrderPayment,
    checkPaymentStatus,
    cancelOrderById,
    reset
  }
})
