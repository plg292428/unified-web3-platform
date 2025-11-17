import WebApi from '@/libs/WebApi'
import type {
  ApiResponse,
  StoreCartListResult,
  StoreCartUpdateQuantityPayload,
  StoreCartUpsertPayload,
  StoreOrderCreatePayload,
  StoreOrderWeb3ConfirmPayload,
  StoreOrderDetailResult,
  StoreOrderListResult,
  StoreOrderPreparePaymentRequest,
  StoreOrderPreparePaymentResult,
  StoreOrderPaymentStatusResult,
  StoreOrderCancelRequest,
  StoreProductCategoryResult,
  StoreProductDetailResult,
  StoreProductListResult,
  StoreProductReviewListResult,
  StoreProductReviewResult,
  StoreProductReviewCreateRequest,
  StoreProductImageResult,
  StoreProductSpecificationGroupResult,
  StorePaymentMode,
  StorePaymentStatus,
  StoreOrderStatus
} from '@/types'

// 使用函数动态获取实例，确保在调用时WebApi已初始化
function getWebApi() {
  return WebApi.getInstance()
}

function ensureSuccess<T>(response: ApiResponse, defaultMessage = '请求失败'): T {
  if (!response.succeed) {
    // 输出详细错误信息用于调试
    console.error('[storeApi] API 请求失败:', {
      succeed: response.succeed,
      errorMessage: response.errorMessage,
      statusCode: response.statusCode,
      data: response.data
    })
    throw new Error(response.errorMessage || defaultMessage)
  }
  return response.data as T
}

export async function fetchProductCategories(): Promise<StoreProductCategoryResult[]> {
  const response = await getWebApi().get('api/store/categories')
  return ensureSuccess<StoreProductCategoryResult[]>(response, '获取商品分类失败')
}

export async function fetchProductList(params: {
  page?: number
  pageSize?: number
  categoryId?: number | null
  keyword?: string | null
  sortBy?: string | null
  minPrice?: number | null
  maxPrice?: number | null
  chainId?: number | null
}): Promise<StoreProductListResult> {
  const query: Record<string, unknown> = {
    page: params.page ?? 1,
    pageSize: params.pageSize ?? 9
  }
  if (params.categoryId) {
    query.categoryId = params.categoryId
  }
  if (params.keyword) {
    query.keyword = params.keyword
  }
  if (params.sortBy) {
    query.sortBy = params.sortBy
  }
  if (params.minPrice != null) {
    query.minPrice = params.minPrice
  }
  if (params.maxPrice != null) {
    query.maxPrice = params.maxPrice
  }
  if (params.chainId) {
    query.chainId = params.chainId
  }
  const response = await getWebApi().get('api/store/products', query)
  return ensureSuccess<StoreProductListResult>(response, '获取商品列表失败')
}

export async function fetchProductDetail(productId: number): Promise<StoreProductDetailResult> {
  const response = await getWebApi().get(`api/store/products/${productId}`)
  return ensureSuccess<StoreProductDetailResult>(response, '获取商品详情失败')
}

export async function fetchCartList(uid: number): Promise<StoreCartListResult> {
  const response = await getWebApi().get('api/cart', { uid })
  return ensureSuccess<StoreCartListResult>(response, '获取购物车失败')
}

export async function addCartItem(payload: StoreCartUpsertPayload): Promise<StoreCartListResult> {
  const response = await getWebApi().post('api/cart/items', payload)
  return ensureSuccess<StoreCartListResult>(response, '加入购物车失败')
}

export async function updateCartItem(
  cartItemId: number,
  payload: StoreCartUpdateQuantityPayload
): Promise<StoreCartListResult> {
  const response = await getWebApi().put(`api/cart/items/${cartItemId}`, payload)
  return ensureSuccess<StoreCartListResult>(response, '更新购物车数量失败')
}

export async function removeCartItem(cartItemId: number, uid: number): Promise<StoreCartListResult> {
  const response = await getWebApi().delete(`api/cart/items/${cartItemId}`, { uid })
  return ensureSuccess<StoreCartListResult>(response, '移除购物车商品失败')
}

export async function createOrder(payload: StoreOrderCreatePayload): Promise<StoreOrderDetailResult> {
  const response = await getWebApi().post('api/orders', payload)
  return ensureSuccess<StoreOrderDetailResult>(response, '创建订单失败')
}

export async function fetchOrderList(
  uid: number,
  page = 1,
  pageSize = 10,
  filters?: {
    status?: StoreOrderStatus
    paymentStatus?: StorePaymentStatus
    paymentMode?: StorePaymentMode
    orderNumber?: string | null
  }
): Promise<StoreOrderListResult> {
  const query: Record<string, unknown> = { uid, page, pageSize }
  if (filters?.status !== undefined) {
    query.status = filters.status
  }
  if (filters?.paymentStatus !== undefined) {
    query.paymentStatus = filters.paymentStatus
  }
  if (filters?.paymentMode !== undefined) {
    query.paymentMode = filters.paymentMode
  }
  if (filters?.orderNumber) {
    query.orderNumber = filters.orderNumber
  }
  const response = await getWebApi().get('api/orders', query)
  return ensureSuccess<StoreOrderListResult>(response, '获取订单列表失败')
}

export async function fetchOrderDetail(orderId: number, uid: number): Promise<StoreOrderDetailResult> {
  const response = await getWebApi().get(`api/orders/${orderId}`, { uid })
  return ensureSuccess<StoreOrderDetailResult>(response, '获取订单详情失败')
}

export async function preparePayment(
  orderId: number,
  payload: StoreOrderPreparePaymentRequest
): Promise<StoreOrderPreparePaymentResult> {
  const response = await getWebApi().post(`api/orders/${orderId}/prepare-payment`, payload)
  return ensureSuccess<StoreOrderPreparePaymentResult>(response, '准备支付失败')
}

export async function getPaymentStatus(
  orderId: number,
  uid: number
): Promise<StoreOrderPaymentStatusResult> {
  const response = await getWebApi().get(`api/orders/${orderId}/payment-status`, { uid })
  return ensureSuccess<StoreOrderPaymentStatusResult>(response, '获取支付状态失败')
}

export async function cancelOrder(
  orderId: number,
  payload: StoreOrderCancelRequest
): Promise<boolean> {
  const response = await getWebApi().post(`api/orders/${orderId}/cancel`, payload)
  return ensureSuccess<boolean>(response, '取消订单失败')
}

export async function confirmWeb3Payment(
  orderId: number,
  payload: StoreOrderWeb3ConfirmPayload
): Promise<boolean> {
  const response = await getWebApi().post(`api/orders/${orderId}/web3/confirm`, payload)
  return ensureSuccess<boolean>(response, '确认链上支付失败')
}

// 商品评价相关 API
export async function fetchProductReviews(
  productId: number,
  page = 1,
  pageSize = 10
): Promise<StoreProductReviewListResult> {
  const response = await getWebApi().get(`api/store/products/${productId}/reviews`, { page, pageSize })
  return ensureSuccess<StoreProductReviewListResult>(response, '获取商品评价失败')
}

export async function createProductReview(
  productId: number,
  payload: StoreProductReviewCreateRequest
): Promise<StoreProductReviewResult> {
  const response = await getWebApi().post(`api/store/products/${productId}/reviews`, payload)
  return ensureSuccess<StoreProductReviewResult>(response, '提交评价失败')
}

// 商品图片相关 API
export async function fetchProductImages(productId: number): Promise<StoreProductImageResult[]> {
  const response = await getWebApi().get(`api/store/products/${productId}/images`)
  return ensureSuccess<StoreProductImageResult[]>(response, '获取商品图片失败')
}

// 商品规格相关 API
export async function fetchProductSpecifications(
  productId: number
): Promise<StoreProductSpecificationGroupResult[]> {
  const response = await getWebApi().get(`api/store/products/${productId}/specifications`)
  return ensureSuccess<StoreProductSpecificationGroupResult[]>(response, '获取商品规格失败')
}
