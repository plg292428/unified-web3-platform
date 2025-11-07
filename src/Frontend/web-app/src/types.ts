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
  address?: string | null
  active: boolean
  chainId: number
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
