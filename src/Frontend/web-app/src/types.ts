// API 响应数据
export interface ApiResponse {
  statusCode: number
  succeed: boolean
  errorMessage?: string | null
  data?: any
}

// Web3 Helper API 结果
export interface Web3HelperResult<T> {
  succeed: boolean
  errorMessage?: string
  data?: T
}

// 分页数据
export interface Pagination {
  pageIndex: number
  pageSize: number
  totalRecords: number
  readonly totalPages: number
  readonly hasPreviousPage: boolean
  readonly hasNextPage: boolean
}

// 分页列表
export interface PaginatedList {
  pagination: Pagination
  items: any[] | null
}

// 主代币设置状态
export enum PrimaryTokenStatus {
  NotSet = 0,
  Pending = 1,
  Completed = 200
}

// 资产管理模式
export enum AssetsManagementMode {
  Invalid = 0,
  ToChain = 1,
  ToWallet = 2
}

// 用户挖矿状态
export enum UserMiningStatus {
  MiningStopped = -1,
  StandardMining = 10,
  HighSpeedMining = 11
}

// 用户 AI 交易状态
export enum UserAiTradingStatus {
  Error = -1,
  None = 0,
  Pending = 1
}

// 用户 AI 交易订单状态
export enum UserAiTradingOrderStatus {
  Trading = 0,
  Failed = 2,
  Completed = 200,
}

// 历史记录类型
export enum RecordType {
  OnChainAssets = 1,
  TransferToChain = 2,
  TransferToWallet = 3,
  AiContractTrading = 4,
  StakeFreeMining = 5,
  InvitationRewards = 6
}

// 区块链交易状态
export enum ChainTransactionStatus {
  Error = -1,
  None = 0,
  Pending = 1,
  Failed = 2,
  Succeed = 200,
}

// 用户转账到钱包订单状态
export enum UserToWalletOrderStatus {
  Error = -1,
  Waiting = 0,
  Pending = 1,
  Failed = 2,
  Succeed = 200
}

// 钱包状态
export interface WalletState {
  initialized: boolean
  provider?: any
  providerName?: string | null
  providerType?: string | null
  address?: string | null
  active: boolean
  chainId: number
  networkName?: string | null
  rpcStatuses: RpcStatus[]
}

export interface RpcStatus {
  name: string
  url: string
  status: 'ok' | 'fail' | 'unknown'
  latency?: number
  lastChecked?: number
  errorMessage?: string
}

// 用户状态
export interface UserState {
  signed: boolean
  userInfo?: UserInfo | null
}

// 服务器配置状态
export interface ServerConfigsState {
  globalConfig: GlobalConfig | null
  chainNetworkConfigs: ChainNetworkConfig[] | null
  chainTokenConfigs: ChainTokenConfig[] | null
  userLevelConfigs: UserLevelConfig[] | null
  chainWalletConfigs: ChainWalletConfig[] | null
  customerServiceConfig: CustomerServiceConfig | null
}

// 全局配置
export interface GlobalConfig {
  miningRewardIntervalHours: number
  miningSpeedUpRequiredOnChainAssetsRate: number
  miningSpeedUpRewardIncreaseRate: number
  minAiTradingMinutes: number
  maxAiTradingMinutes: number
  invitedRewardRateLayer1: number
  invitedRewardRateLayer2: number
  invitedRewardRateLayer3: number
}

// 区块链网络配置
export interface ChainNetworkConfig {
  chainId: number
  chainIconPath: string | null
  networkName: string | null
  abbrNetworkName: string | null
  color: string | null
  currencyName: string | null
  currencyDecimals: number
  currencyIconPath: string | null
  clientGasFeeAlertValue: number
  minAssetsToChainLimit: number
  maxAssetsToChaintLimit: number
  minAssetsToWalletLimit: number
  maxAssetsToWalletLimit: number
  assetsToWalletServiceFeeBase: number
  assetsToWalletServiceFeeRate: number
}

// 区块链代币配置
export interface ChainTokenConfig {
  tokenId: number
  chainId: number
  tokenName: string | null
  abbrTokenName: string | null
  iconPath: string | null
  contractAddress: string | null
  approveAbiFunctionName: string | null
  decimals: number
}

// 用户等级配置
export interface UserLevelConfig {
  userLevel: number
  userLevelName: string | null
  color: string | null
  iconPath: string | null
  requiresValidAsset: number
  dailyAiTradingLimitTimes: number
  availableAiTradingAssetsRate: number
  minEachAiTradingRewardRate: number
  maxEachAiTradingRewardRate: number
  minEachMiningRewardRate: number
  maxEachMiningRewardRate: number
}

// 区块链钱包配置
export interface ChainWalletConfig {
  groupId: number
  chainId: number
  spenderWalletAddress: string | null
  receiveWalletAddress: string | null
}

// 客服配置
export interface CustomerServiceConfig {
  customerServiceEnabled: boolean
  customerServiceChatWootKey: string | null
}

// 用户财产
export interface UserAsset {
  primaryTokenId: number
  currencyWalletBalance: number
  primaryTokenWalletBalance: number
  onChainAssets: number
  validAssets: number
  approved: boolean
  aiTradingActivated: boolean
  aiTradingRemainingTimes: number
  miningActivityPoint: number
  totalToChain: number
  totalToWallet: number
  totalAiTradingRewards: number
  totalMiningRewards: number
  totalInvitationRewards: number
  totalSystemRewards: number
}

// 用户信息
export interface UserInfo {
  uid: number
  walletAddress: string | null
  chainId: number
  userLevel: number
  signUpClientIp: string | null
  lastSignInClientIp: string | null
  virtualUser: boolean
  anomaly: boolean
  invitationLink: string | null
  newSystemMessage: boolean
  primaryTokenStatus: PrimaryTokenStatus
  asset: UserAsset
  layer1Members: number
  layer2Members: number
}

// 店铺分类
export interface StoreProductCategoryResult {
  categoryId: number
  name: string
  slug?: string | null
  description?: string | null
  parentCategoryId?: number | null
  sortOrder: number
  isActive: boolean
  children?: StoreProductCategoryResult[]
}

// 店铺商品
export interface StoreProductSummaryResult {
  productId: number
  categoryId: number
  categoryName: string
  name: string
  subtitle?: string | null
  thumbnailUrl?: string | null
  price: number
  currency: string
  isPublished: boolean
  inventoryAvailable: number
  updateTime: string
}

export interface StoreProductListResult {
  items: StoreProductSummaryResult[]
  totalCount: number
  page: number
  pageSize: number
}

export interface StoreProductDetailResult {
  productId: number
  categoryId: number
  categoryName: string
  name: string
  subtitle?: string | null
  description?: string | null
  thumbnailUrl?: string | null
  price: number
  currency: string
  isPublished: boolean
  inventoryAvailable: number
  inventoryReserved: number
  chainId?: number | null
  sku?: string | null
  createTime: string
  updateTime: string
  breadcrumb: Array<{
    categoryId: number
    name: string
    slug?: string | null
  }>
}

// 购物车
export interface StoreCartItemResult {
  cartItemId: number
  productId: number
  productName: string
  subtitle?: string | null
  thumbnailUrl?: string | null
  unitPrice: number
  currency: string
  quantity: number
  subtotal: number
  inventoryAvailable: number
  updateTime: string
}

export interface StoreCartListResult {
  items: StoreCartItemResult[]
  totalAmount: number
  totalQuantity: number
}

export interface StoreCartUpsertPayload {
  uid: number
  productId: number
  quantity: number
}

export interface StoreCartUpdateQuantityPayload {
  uid: number
  cartItemId: number
  quantity: number
}

// 订单
export enum StoreOrderStatus {
  PendingPayment = 0,
  Paid = 1,
  Cancelled = 2,
  Completed = 3
}

export enum StorePaymentMode {
  Traditional = 0,
  Web3 = 1
}

export enum StorePaymentStatus {
  PendingSignature = 0,
  AwaitingOnChainConfirmation = 1,
  Confirmed = 2,
  Failed = 3,
  Cancelled = 4
}

export interface StoreOrderSummaryResult {
  orderId: number
  orderNumber: string
  totalAmount: number
  currency: string
  status: StoreOrderStatus
  paymentMode: StorePaymentMode
  paymentStatus: StorePaymentStatus
  paymentMethod?: string | null
  paymentProviderType?: string | null
  paymentProviderName?: string | null
  paymentWalletAddress?: string | null
  chainId?: number | null
  paymentTransactionHash?: string | null
  createTime: string
  paidTime?: string | null
}

export interface StoreOrderListResult {
  items: StoreOrderSummaryResult[]
  totalCount: number
  page: number
  pageSize: number
}

export interface StoreOrderItemResult {
  orderItemId: number
  productId: number
  productName: string
  unitPrice: number
  quantity: number
  subtotal: number
}

export interface StoreOrderPaymentLogResult {
  orderPaymentLogId: number
  paymentStatus: StorePaymentStatus
  eventType: string
  message?: string | null
  rawData?: string | null
  createTime: string
}

export interface StoreOrderDetailResult {
  orderId: number
  orderNumber: string
  uid: number
  totalAmount: number
  currency: string
  status: StoreOrderStatus
  paymentMode: StorePaymentMode
  paymentStatus: StorePaymentStatus
  paymentMethod?: string | null
  paymentProviderType?: string | null
  paymentProviderName?: string | null
  paymentWalletAddress?: string | null
  paymentWalletLabel?: string | null
  chainId?: number | null
  paymentTransactionHash?: string | null
  paymentConfirmations?: number | null
  paymentSubmittedTime?: string | null
  paymentConfirmedTime?: string | null
  paymentSignaturePayload?: string | null
  paymentSignatureResult?: string | null
  paymentFailureReason?: string | null
  createTime: string
  paidTime?: string | null
  cancelTime?: string | null
  completeTime?: string | null
  remark?: string | null
  items: StoreOrderItemResult[]
  paymentLogs: StoreOrderPaymentLogResult[]
}

export interface StoreOrderCreatePayload {
  uid: number
  paymentMode: StorePaymentMode
  paymentMethod?: string | null
  paymentProviderType?: string | null
  paymentProviderName?: string | null
  paymentWalletAddress?: string | null
  paymentWalletLabel?: string | null
  paymentTransactionHash?: string | null
  chainId?: number | null
  paymentSignaturePayload?: string | null
  paymentSignatureResult?: string | null
  remark?: string | null
}

export interface StoreOrderWeb3ConfirmPayload {
  uid: number
  paymentTransactionHash?: string | null
  paymentStatus: StorePaymentStatus
  paymentConfirmations?: number | null
  paymentConfirmedTime?: string | null
  paymentSignatureResult?: string | null
  paymentFailureReason?: string | null
  rawData?: string | null
}

export interface StoreOrderPreparePaymentRequest {
  uid: number
  paymentWalletAddress?: string | null
  paymentProviderType?: string | null
  paymentProviderName?: string | null
  chainId?: number | null
}

export interface StoreOrderPreparePaymentResult {
  paymentSignaturePayload: string
  amount: number
  currency: string
  chainId: number
  paymentAddress: string
  paymentExpiresAt: number
  orderNumber: string
}

export interface StoreOrderPaymentStatusResult {
  orderId: number
  orderNumber: string
  paymentStatus: StorePaymentStatus
  orderStatus: StoreOrderStatus
  paymentTransactionHash?: string | null
  paymentConfirmations?: number | null
  paymentSubmittedTime?: string | null
  paymentConfirmedTime?: string | null
  paymentExpiresAt?: string | null
  paymentFailureReason?: string | null
  isExpired: boolean
  remainingSeconds: number
}

export interface StoreOrderCancelRequest {
  uid: number
  reason?: string | null
}

// 商品评价相关类型
export interface StoreProductReviewResult {
  reviewId: number
  productId: number
  uid: number
  userWalletAddress?: string | null
  orderId?: number | null
  rating: number
  content?: string | null
  isApproved: boolean
  isVisible: boolean
  createTime: string
  updateTime: string
}

export interface StoreProductReviewListResult {
  items: StoreProductReviewResult[]
  totalCount: number
  averageRating: number
  ratingDistribution: Record<number, number>
}

export interface StoreProductReviewCreateRequest {
  uid: number
  productId: number
  orderId?: number | null
  rating: number
  content?: string | null
}

// 商品图片相关类型
export interface StoreProductImageResult {
  imageId: number
  productId: number
  imageUrl: string
  imageType: string
  sortOrder: number
  isPrimary: boolean
  createTime: string
}

// 商品规格相关类型
export interface StoreProductSpecificationResult {
  specificationId: number
  productId: number
  specificationName: string
  specificationValue: string
  priceAdjustment: number
  stockQuantity?: number | null
  sortOrder: number
  isEnabled: boolean
  createTime: string
  updateTime: string
}

export interface StoreProductSpecificationGroupResult {
  specificationName: string
  values: StoreProductSpecificationResult[]
}
