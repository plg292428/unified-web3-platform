# UnifiedWeb3Platform 完整功能清单

**文档版本**: v1.0  
**生成日期**: 2025-01-20  
**项目名称**: UnifiedWeb3Platform - 统一Web3全栈平台

---

## 📋 目录

1. [项目概述](#项目概述)
2. [技术架构](#技术架构)
3. [前端功能详细说明](#前端功能详细说明)
4. [后端API功能详细说明](#后端api功能详细说明)
5. [数据库实体说明](#数据库实体说明)
6. [核心业务流程](#核心业务流程)
7. [Web3功能](#web3功能)
8. [安全功能](#安全功能)
9. [管理端功能](#管理端功能)
10. [统计与分析功能](#统计与分析功能)

---

## 项目概述

### 项目简介

UnifiedWeb3Platform 是一个统一的Web3全栈电商平台，整合了以下核心项目：

- **HFastKit**: .NET 8 工具库
- **Nblockchain**: TRON 区块链 SDK
- **SmallTarget**: Vue 3 企业应用框架
- **PolygonDapp**: Polygon 区块链 DApp

### 核心特性

- ✅ **Web3支付**: 支持MetaMask、Bitget Wallet等多钱包
- ✅ **多链支持**: Polygon、TRON双链支持
- ✅ **电商功能**: 完整的商品管理、购物车、订单系统
- ✅ **资产管理**: 链上资产充值、提现、查询
- ✅ **AI交易**: AI合约交易功能
- ✅ **免质押挖矿**: 挖矿奖励系统
- ✅ **访客模式**: 无需钱包即可浏览商品
- ✅ **响应式设计**: 支持移动端和PC端

---

## 技术架构

### 后端技术栈

- **框架**: .NET 8
- **Web框架**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **数据库**: SQL Server (LocalDB/Express)
- **认证**: JWT Token + Web3签名验证
- **区块链SDK**: 
  - Nblockchain (TRON)
  - Web3.js (Polygon/Ethereum)

### 前端技术栈

- **框架**: Vue 3 (Composition API)
- **构建工具**: Vite
- **UI框架**: Vuetify 3
- **状态管理**: Pinia
- **路由**: Vue Router
- **HTTP客户端**: Axios
- **Web3集成**: 
  - ethers.js
  - @bitget-wallet/omni-connect
  - WalletConnect

### 项目结构

```
UnifiedWeb3Platform/
├── src/
│   ├── Frontend/
│   │   └── web-app/              # Vue 3前端应用
│   ├── Backend/
│   │   ├── UnifiedPlatform.WebApi/    # Web API服务
│   │   ├── UnifiedPlatform.DbService/ # 数据库服务
│   │   └── UnifiedPlatform.Shared/    # 共享库
│   └── Libraries/
│       ├── HFastKit/             # .NET 8工具库
│       └── Nblockchain/          # TRON区块链SDK
```

---

## 前端功能详细说明

### 1. 首页 (Home)

**路由**: `/`  
**文件**: `src/Frontend/web-app/src/views/Home.vue`

#### 功能列表

1. **商品展示**
   - 热门商品列表
   - 最新商品列表
   - 商品卡片展示（图片、名称、价格、库存）
   - 商品分类导航

2. **商品搜索**
   - 关键词搜索
   - 实时搜索建议
   - 搜索结果高亮

3. **商品筛选**
   - 按分类筛选
   - 按价格区间筛选
   - 按库存状态筛选
   - 排序功能（价格、时间、销量）

4. **购物车管理**
   - 添加商品到购物车（支持未登录用户临时购物车）
   - 查看购物车商品数量
   - 快速跳转到购物车页面

5. **用户信息展示**
   - 用户等级显示
   - 成长值进度
   - 链上资产概览
   - 钱包余额显示

6. **钱包状态显示**
   - 钱包连接状态
   - 钱包地址显示（缩短格式）
   - 网络切换提示

7. **最新订单列表**
   - 最近订单预览
   - 订单状态显示
   - 快速跳转到订单详情

8. **快速操作**
   - AI交易入口
   - 挖矿入口
   - 资产管理入口

---

### 2. 商品详情页 (ProductDetail)

**路由**: `/products/:productId`  
**文件**: `src/Frontend/web-app/src/views/ProductDetail.vue`

#### 功能列表

1. **商品信息展示**
   - 商品大图展示
   - 商品名称、副标题
   - 价格和货币显示
   - 库存状态（有货/缺货）
   - SKU信息
   - 商品描述
   - 分类面包屑导航

2. **数量选择器** ⭐ 新增功能
   - 减号按钮（-）
   - 数量输入框（支持直接输入）
   - 加号按钮（+）
   - 最大库存限制
   - 实时数量验证

3. **购物操作**
   - 添加到购物车（支持选择数量）
   - 立即购买（支持选择数量）
   - 库存不足提示

4. **商品详细信息**
   - 商品ID
   - 分类信息
   - 链ID（如果适用）
   - 创建时间
   - 更新时间

5. **访客模式支持**
   - 未登录用户可浏览
   - 未登录用户可添加到临时购物车
   - 支付时引导连接钱包

---

### 3. 购物车 (Cart)

**路由**: `/cart`  
**文件**: `src/Frontend/web-app/src/views/Cart.vue`

#### 功能列表

1. **购物车商品管理**
   - 商品列表展示
   - 商品图片、名称、价格
   - 数量调整（+/-按钮）
   - 数量输入框（支持直接输入）
   - 删除商品
   - 商品小计计算
   - 购物车总价计算

2. **临时购物车（未登录用户）**
   - localStorage存储
   - 商品列表展示
   - 数量调整
   - 连接钱包后自动转移

3. **商品列表集成** ⭐ 新增功能
   - 在购物车页面直接浏览商品
   - 可折叠的商品列表区域
   - 商品卡片展示
   - 直接添加商品到购物车
   - 在列表中调整商品数量
   - 分页支持

4. **结算功能**
   - 创建订单
   - 选择支付方式（Web3/传统支付）
   - 跳转到支付页面

5. **钱包连接引导**
   - 未登录用户显示钱包连接提示
   - MetaMask安装引导
   - Bitget Wallet安装引导
   - 美观的钱包选择对话框

---

### 4. 订单列表 (Orders)

**路由**: `/orders`  
**文件**: `src/Frontend/web-app/src/views/Orders.vue`  
**权限**: 需要登录

#### 功能列表

1. **订单列表查询**
   - 分页显示
   - 订单卡片展示
   - 订单号显示
   - 订单时间显示
   - 订单总价显示

2. **订单筛选**
   - 按订单状态筛选：
     - 待支付 (PendingPayment)
     - 已支付 (Paid)
     - 已发货 (Shipped)
     - 已完成 (Delivered)
     - 已取消 (Cancelled)
     - 已退款 (Refunded)
   - 按支付方式筛选：
     - Web3支付
     - 传统支付

3. **订单搜索**
   - 按订单号搜索
   - 实时搜索

4. **订单操作**
   - 查看订单详情
   - 支付订单（待支付状态）
   - 取消订单（待支付状态）

---

### 5. 订单详情 (OrderDetail)

**路由**: `/orders/:orderId`  
**文件**: `src/Frontend/web-app/src/views/OrderDetail.vue`  
**权限**: 需要登录

#### 功能列表

1. **订单信息展示**
   - 订单号
   - 订单状态
   - 订单时间
   - 订单总价
   - 支付方式
   - 支付状态
   - 收货地址（如果有）

2. **订单商品列表**
   - 商品图片
   - 商品名称
   - 商品数量
   - 商品单价
   - 商品小计

3. **支付功能**
   - Web3支付对话框
   - 传统支付对话框
   - 支付状态跟踪
   - 支付确认

4. **物流信息**
   - 物流公司
   - 物流单号
   - 物流状态
   - 物流跟踪记录

---

### 6. AI合约交易 (AIContractTrading)

**路由**: `/ai_contract_trading`  
**文件**: `src/Frontend/web-app/src/views/AIContractTrading.vue`  
**权限**: 需要登录

#### 功能列表

1. **AI交易状态查看**
   - 激活状态
   - 可用资产
   - 交易历史

2. **创建AI交易订单**
   - 交易金额输入
   - 聚合模式选择：
     - 中心化交易所 (CEX)
     - 去中心化交易所 (DEX)
   - 预计收益显示
   - 创建订单确认

3. **交易历史记录**
   - 订单列表
   - 订单状态
   - 收益记录
   - 时间排序

4. **激活功能**
   - 激活码输入
   - 激活确认

---

### 7. 免质押挖矿 (StakeFreeMining)

**路由**: `/stake_free_mining`  
**文件**: `src/Frontend/web-app/src/views/StakeFreeMining.vue`  
**权限**: 需要登录

#### 功能列表

1. **挖矿状态查看**
   - 挖矿激活状态
   - 有效资产显示
   - 挖矿收益统计

2. **挖矿奖励记录**
   - 奖励列表
   - 奖励金额
   - 奖励时间
   - 奖励状态

3. **资产统计**
   - 总奖励统计
   - 本月奖励
   - 历史奖励

---

### 8. 历史记录 (History)

**路由**: `/history`  
**文件**: `src/Frontend/web-app/src/views/History.vue`  
**权限**: 需要登录

#### 功能列表

1. **资产变化记录**
   - 充值记录
   - 提现记录
   - 奖励记录
   - 时间排序

2. **挖矿奖励记录**
   - 奖励详情
   - 奖励时间
   - 奖励金额

3. **AI合约交易记录**
   - 交易订单
   - 交易收益
   - 交易时间

4. **转账记录**
   - 转账到链上（充值）
   - 转账到钱包（提现）
   - 转账状态
   - 转账金额

---

### 9. 系统消息 (SystemMessages)

**路由**: `/system_messages`  
**文件**: `src/Frontend/web-app/src/views/SystemMessages.vue`  
**权限**: 需要登录

#### 功能列表

1. **消息列表**
   - 消息标题
   - 消息内容预览
   - 消息时间
   - 已读/未读状态

2. **消息操作**
   - 标记为已读
   - 查看消息详情
   - 删除消息

---

### 10. 系统消息详情 (SystemMessageDetails)

**路由**: `/system_message_details`  
**文件**: `src/Frontend/web-app/src/views/SystemMessageDetails.vue`  
**权限**: 需要登录

#### 功能列表

1. **消息详情展示**
   - 消息标题
   - 消息内容（支持富文本）
   - 消息时间
   - 消息类型

2. **消息操作**
   - 标记为已读
   - 返回消息列表

---

### 11. 钱包连接页 (Go)

**路由**: `/go`  
**文件**: `src/Frontend/web-app/src/views/Go.vue`

#### 功能列表

1. **钱包检测**
   - 自动检测已安装的钱包
   - MetaMask检测
   - Bitget Wallet检测

2. **钱包连接**
   - 连接MetaMask
   - 连接Bitget Wallet
   - 网络切换提示

3. **钱包安装引导**
   - 美观的钱包选择对话框 ⭐ 新增功能
   - MetaMask安装链接
   - Bitget Wallet安装链接
   - 安装说明

4. **用户注册/登录**
   - Web3签名登录
   - 自动创建账户
   - 登录状态保存

---

### 12. 错误页面

#### 404 页面未找到

**路由**: `/error/404`  
**文件**: `src/Frontend/web-app/src/views/errors/404.vue`

- 友好的404错误提示
- 返回首页按钮

#### 500 服务器错误

**路由**: `/error/500`  
**文件**: `src/Frontend/web-app/src/views/errors/500.vue`

- 服务器错误提示
- 刷新页面按钮

#### 无钱包检测错误

**路由**: `/error/no-wallet-detected`  
**文件**: `src/Frontend/web-app/src/views/errors/NoWalletDetected.vue`

- 钱包未检测提示
- 钱包安装引导

#### 不支持的网络错误

**路由**: `/error/unsupported-network`  
**文件**: `src/Frontend/web-app/src/views/errors/UnsupportedNetwork.vue`

- 网络不支持提示
- 网络切换引导

---

## 前端组件功能详细说明

### 1. WalletStatusCard

**文件**: `src/Frontend/web-app/src/components/WalletStatusCard.vue`

#### 功能

- 钱包连接状态显示
- 钱包地址显示（缩短格式：0x1234...5678）
- 网络切换按钮
- 余额显示
- 断开连接功能

---

### 2. AssetsManagementSheet

**文件**: `src/Frontend/web-app/src/components/AssetsManagementSheet.vue`

#### 功能

1. **转账到链上（充值）**
   - 金额输入
   - 余额验证
   - 最小转账金额提示
   - 服务费计算
   - 确认转账

2. **转账到钱包（提现）**
   - 金额输入
   - 钱包地址输入
   - 余额验证
   - 最小转账金额提示
   - 服务费计算
   - 确认转账

---

### 3. AIContractTradingSheet

**文件**: `src/Frontend/web-app/src/components/AIContractTradingSheet.vue`

#### 功能

- 创建AI交易订单
- 聚合模式选择（CEX/DEX）
- 预计收益显示
- 可用资产验证
- 交易确认

---

### 4. HistoryRecordList

**文件**: `src/Frontend/web-app/src/components/HistoryRecordList.vue`

#### 功能

- 资产变化记录展示
- 挖矿奖励记录展示
- AI交易记录展示
- 时间排序
- 记录类型筛选

---

### 5. OrderPaymentDialog

**文件**: `src/Frontend/web-app/src/components/OrderPaymentDialog.vue`

#### 功能

1. **Web3支付对话框**
   - 订单信息展示
   - 多链选择（Polygon、TRON等）
   - 多代币选择（ERC20、TRC20）
   - 支付金额显示
   - 支付签名生成
   - 支付状态跟踪
   - 支付确认

2. **支付流程**
   - 生成支付签名
   - 钱包签名确认
   - 链上交易提交
   - 交易状态查询
   - 支付完成确认

---

### 6. TraditionalPaymentDialog

**文件**: `src/Frontend/web-app/src/components/TraditionalPaymentDialog.vue`

#### 功能

- 传统支付方式选择
- 支付信息展示
- 支付确认

---

### 7. PrimaryTokenSheet

**文件**: `src/Frontend/web-app/src/components/PrimaryTokenSheet.vue`

#### 功能

- 主代币设置
- 代币授权
- 授权状态显示

---

### 8. ActivateAIContractTradingDialog

**文件**: `src/Frontend/web-app/src/components/ActivateAIContractTradingDialog.vue`

#### 功能

- AI交易激活
- 激活码输入
- 激活确认

---

### 9. PaymentMethodSelector

**文件**: `src/Frontend/web-app/src/components/PaymentMethodSelector.vue`

#### 功能

- 支付方式选择
- Web3支付选项
- 传统支付选项

---

## 后端API功能详细说明

### 用户相关API

#### DappUserController

**路由前缀**: `/api/dapp-user`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| POST | `/sign-in` | Web3签名登录 | AllowAnonymous |
| POST | `/check-signed` | 检查登录状态 | AllowAnonymous |
| GET | `/info` | 获取用户信息 | 需要登录 |
| POST | `/update-wallet-address` | 更新钱包地址 | 需要登录 |
| POST | `/activate-ai-trading` | 激活AI合约交易 | 需要登录 |
| POST | `/create-ai-trading-order` | 创建AI交易订单 | 需要登录 |
| GET | `/ai-trading-orders` | 查询AI交易订单 | 需要登录 |
| GET | `/ai-trading-status` | 获取AI交易状态 | 需要登录 |
| POST | `/transfer-to-chain` | 转账到链上（充值） | 需要登录 |
| POST | `/transfer-to-wallet` | 转账到钱包（提现） | 需要登录 |
| GET | `/transfer-orders` | 查询转账订单 | 需要登录 |
| GET | `/mining-status` | 获取挖矿状态 | 需要登录 |
| GET | `/invitation-link` | 获取邀请链接 | 需要登录 |
| GET | `/system-messages` | 查询系统消息 | 需要登录 |

---

#### UserProfileController

**路由前缀**: `/api/user-profile`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/profile` | 获取用户资料 | 需要登录 |
| GET | `/statistics` | 获取用户统计信息 | 需要登录 |
| GET | `/activity-logs` | 获取用户活动日志 | 需要登录 |

---

#### AssetManagementController

**路由前缀**: `/api/asset-management`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/` | 获取资产详情 | 需要登录 |
| GET | `/history` | 获取资产变化历史 | 需要登录 |
| GET | `/transfer-to-chain` | 获取转账到链上订单 | 需要登录 |
| GET | `/transfer-to-wallet` | 获取转账到钱包订单 | 需要登录 |
| GET | `/statistics` | 获取资产统计 | 需要登录 |
| GET | `/mining-rewards` | 获取挖矿奖励记录 | 需要登录 |
| GET | `/invitation-rewards` | 获取邀请奖励记录 | 需要登录 |
| GET | `/ai-trading-orders` | 获取AI交易订单记录 | 需要登录 |

---

### 电商相关API

#### StoreController

**路由前缀**: `/api/store`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/categories` | 获取商品分类树 | AllowAnonymous |
| GET | `/products` | 获取商品列表（分页、搜索、筛选） | AllowAnonymous |
| GET | `/products/{productId}/images` | 获取商品图片列表 | AllowAnonymous |
| GET | `/products/{productId}/specifications` | 获取商品规格列表 | AllowAnonymous |
| GET | `/products/{productId}` | 获取商品详情 | AllowAnonymous |

---

#### CartController

**路由前缀**: `/api/cart`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/` | 获取购物车列表 | 需要登录 |
| POST | `/items` | 添加/更新购物车项 | 需要登录 |
| PUT | `/items/{cartItemId}` | 更新购物车项数量 | 需要登录 |
| DELETE | `/items/{cartItemId}` | 删除购物车项 | 需要登录 |

---

#### OrderController

**路由前缀**: `/api/orders`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| POST | `/` | 创建订单 | 需要登录 |
| GET | `/` | 查询订单列表 | 需要登录 |
| GET | `/{orderId}` | 查询订单详情 | 需要登录 |
| POST | `/{orderId}/prepare-payment` | 准备Web3支付（生成签名） | 需要登录 |
| POST | `/{orderId}/web3/confirm` | 确认Web3支付 | 需要登录 |
| GET | `/{orderId}/payment-status` | 查询支付状态 | 需要登录 |
| POST | `/{orderId}/cancel` | 取消订单 | 需要登录 |

---

#### ProductManagementController

**路由前缀**: `/ProductManagement`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| POST | `/CreateProduct` | 创建商品 | Manager |
| PUT | `/UpdateProduct/{productId}` | 更新商品 | Manager |
| DELETE | `/DeleteProduct/{productId}` | 删除商品 | Manager |

---

#### ProductReviewController

**路由前缀**: `/api/product-reviews`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/` | 获取商品评价列表 | AllowAnonymous |
| POST | `/` | 创建商品评价（支持图片） | 需要登录 |
| GET | `/my` | 获取用户自己的评价 | 需要登录 |
| POST | `/{reviewId}/vote` | 评价投票（有用/无用） | 需要登录 |

---

#### ProductReviewManagementController

**路由前缀**: `/ProductReviewManagement`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| POST | `/{reviewId}/approve` | 审核评价 | Manager |
| DELETE | `/{reviewId}` | 删除评价 | Manager |
| POST | `/{reviewId}/reply` | 商家回复评价 | Manager |
| GET | `/pending` | 查询待审核评价列表 | Manager |

---

#### RecommendationController

**路由前缀**: `/api/recommendations`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/hot` | 获取热门商品 | AllowAnonymous |
| GET | `/related/{productId}` | 获取相关商品 | AllowAnonymous |
| GET | `/personalized` | 获取个性化推荐 | 需要登录 |
| GET | `/latest` | 获取最新商品 | AllowAnonymous |

---

#### ShippingController

**路由前缀**: `/Shipping`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| POST | `/Ship` | 发货 | Manager |
| GET | `/Track/{orderId}` | 查询物流信息 | 需要登录 |
| POST | `/UpdateStatus` | 更新物流状态 | Manager |
| GET | `/Companies` | 获取物流公司列表 | AllowAnonymous |

---

#### InventoryManagementController

**路由前缀**: `/InventoryManagement`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| PUT | `/{productId}` | 更新商品库存 | Manager |
| POST | `/{productId}/adjust` | 调整商品库存 | Manager |
| POST | `/{productId}/reserve` | 预留库存 | Manager |
| POST | `/{productId}/release` | 释放库存 | Manager |
| GET | `/low-stock` | 获取低库存商品 | Manager |
| GET | `/statistics` | 获取库存统计 | Manager |

---

### 区块链相关API

#### PolygonController

**路由前缀**: `/api/polygon`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/balance/matic` | 获取MATIC余额 | 需要登录 |
| GET | `/balance/erc20` | 获取ERC20代币余额 | 需要登录 |
| GET | `/transaction/{transactionHash}` | 查询交易状态 | AllowAnonymous |
| GET | `/gas/price` | 获取Gas价格 | AllowAnonymous |
| GET | `/gas/estimate` | 估算Gas费用 | AllowAnonymous |

---

#### TronController

**路由前缀**: `/api/tron`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| POST | `/wallet/create` | 创建TRON钱包 | AllowAnonymous |
| POST | `/wallet/from-private-key` | 从私钥创建钱包 | AllowAnonymous |
| GET | `/balance/trx/{address}` | 查询TRX余额 | AllowAnonymous |
| GET | `/balance/trc20/{address}` | 查询TRC20代币余额 | AllowAnonymous |
| POST | `/transfer/trx` | 转账TRX | 需要登录 |
| POST | `/transfer/trc20` | 转账TRC20代币 | 需要登录 |
| GET | `/transaction/{transactionId}/status` | 查询交易状态 | AllowAnonymous |

---

### 管理端API

#### ManagementAuthenticationController

**路由前缀**: `/ManagementAuthentication`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/Login` | 管理员登录页面 | AllowAnonymous |
| GET | `/Logout` | 管理员登出 | AllowAnonymous |
| POST | `/Login` | 管理员登录 | AllowAnonymous |
| GET | `/Info` | 获取管理员信息 | 需要登录 |
| POST | `/UpdatePassword` | 更新密码 | GroupLeader |

---

#### ManagementCommonController

**路由前缀**: `/ManagementCommon`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/GlobalConfig` | 获取全局配置 | 需要登录 |
| GET | `/UserLevelConfig` | 获取用户等级配置 | 需要登录 |
| GET | `/ChainNetworkConfig` | 获取区块链网络配置 | 需要登录 |
| GET | `/ChainTokenConfig` | 获取代币配置 | 需要登录 |
| GET | `/AgentBalanceChange` | 查询代理账变记录 | Agent |
| GET | `/GroupLeaderBalanceChange` | 查询组长账变记录 | GroupLeader |
| GET | `/AdministratorBalanceChange` | 查询管理员账变记录 | Administrator |
| GET | `/AgentOperationLog` | 查询代理操作日志 | Agent |
| GET | `/GroupLeaderOperationLog` | 查询组长操作日志 | GroupLeader |
| POST | `/UpdateAgentBalance` | 更新代理余额 | Administrator |
| POST | `/UpdateGroupLeaderBalance` | 更新组长余额 | Administrator |
| POST | `/UpdateAdministratorBalance` | 更新管理员余额 | Administrator |
| POST | `/CreateOperationLog` | 创建操作日志 | Developer |

---

#### ManagementDappUserController

**路由前缀**: `/ManagementDappUser`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/List` | 查询用户列表 | Administrator |
| GET | `/Detail` | 查询用户详情 | 需要登录 |

---

### 其他API

#### DappCommonController

**路由前缀**: `/api/dapp-common`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| GET | `/global-config` | 获取全局配置 | AllowAnonymous |
| GET | `/chain-networks` | 获取区块链网络配置 | AllowAnonymous |
| GET | `/tokens` | 获取代币配置 | AllowAnonymous |
| GET | `/user-levels` | 获取用户等级配置 | AllowAnonymous |
| GET | `/my-level` | 获取我的等级 | 需要登录 |
| GET | `/my-invitation-link` | 获取我的邀请链接 | 需要登录 |

---

#### SeedDataController

**路由前缀**: `/api/seed-data`

| 方法 | 路由 | 功能 | 权限 |
|------|------|------|------|
| POST | `/products` | 初始化种子数据（商品） | 开发环境 |

---

## 数据库实体说明

### 用户相关实体

#### User（用户）

- UserId: 用户ID（主键）
- WalletAddress: 钱包地址
- UserName: 用户名
- UserLevel: 用户等级
- GrowthValue: 成长值
- InviterId: 邀请人ID
- CreateTime: 创建时间
- UpdateTime: 更新时间

#### UserAsset（用户资产）

- AssetId: 资产ID（主键）
- UserId: 用户ID（外键）
- ChainId: 链ID
- TokenAddress: 代币地址
- Balance: 余额
- UpdateTime: 更新时间

#### UserLoginLog（登录日志）

- LogId: 日志ID（主键）
- UserId: 用户ID（外键）
- LoginTime: 登录时间
- IpAddress: IP地址
- UserAgent: 用户代理

#### UserPathNode（用户关系树）

- NodeId: 节点ID（主键）
- UserId: 用户ID（外键）
- ParentUserId: 父用户ID
- Path: 路径
- Depth: 深度

#### UserChainTransaction（链上交易）

- TransactionId: 交易ID（主键）
- UserId: 用户ID（外键）
- ChainId: 链ID
- TransactionHash: 交易哈希
- TransactionType: 交易类型
- Amount: 金额
- Status: 状态
- CreateTime: 创建时间

#### UserAssetsToWalletOrder（提现订单）

- OrderId: 订单ID（主键）
- UserId: 用户ID（外键）
- Amount: 金额
- WalletAddress: 钱包地址
- Status: 状态
- CreateTime: 创建时间

#### UserOnChainAssetsChange（资产变化记录）

- ChangeId: 变化ID（主键）
- UserId: 用户ID（外键）
- ChangeType: 变化类型
- Amount: 金额
- BeforeBalance: 变化前余额
- AfterBalance: 变化后余额
- CreateTime: 创建时间

#### UserAiTradingOrder（AI交易订单）

- OrderId: 订单ID（主键）
- UserId: 用户ID（外键）
- Amount: 金额
- AggregationMode: 聚合模式
- Status: 状态
- Profit: 收益
- CreateTime: 创建时间

#### UserMiningRewardRecord（挖矿奖励）

- RecordId: 记录ID（主键）
- UserId: 用户ID（外键）
- RewardAmount: 奖励金额
- RewardTime: 奖励时间
- Status: 状态

#### UserInvitationRewardRecord（邀请奖励）

- RecordId: 记录ID（主键）
- UserId: 用户ID（外键）
- InviteeId: 被邀请人ID
- RewardAmount: 奖励金额
- RewardTime: 奖励时间

#### UserSysteamMessage（系统消息）

- MessageId: 消息ID（主键）
- UserId: 用户ID（外键）
- Title: 标题
- Content: 内容
- IsRead: 是否已读
- CreateTime: 创建时间

---

### 电商相关实体

#### Product（商品）

- ProductId: 商品ID（主键）
- CategoryId: 分类ID（外键）
- Name: 商品名称
- Subtitle: 副标题
- Description: 描述
- ThumbnailUrl: 缩略图URL
- Price: 价格
- Currency: 货币
- ChainId: 链ID
- Sku: SKU
- IsPublished: 是否发布
- CreateTime: 创建时间
- UpdateTime: 更新时间

#### ProductCategory（商品分类）

- CategoryId: 分类ID（主键）
- ParentCategoryId: 父分类ID
- Name: 分类名称
- Slug: 分类别名
- Description: 描述
- SortOrder: 排序
- IsActive: 是否激活

#### ProductImage（商品图片）

- ImageId: 图片ID（主键）
- ProductId: 商品ID（外键）
- ImageUrl: 图片URL
- SortOrder: 排序
- IsPrimary: 是否主图

#### ProductSpecification（商品规格）

- SpecificationId: 规格ID（主键）
- ProductId: 商品ID（外键）
- Name: 规格名称
- Value: 规格值
- SortOrder: 排序

#### ProductInventory（商品库存）

- InventoryId: 库存ID（主键）
- ProductId: 商品ID（外键）
- QuantityAvailable: 可用数量
- QuantityReserved: 预留数量
- UpdateTime: 更新时间

#### ProductReview（商品评价）

- ReviewId: 评价ID（主键）
- ProductId: 商品ID（外键）
- UserId: 用户ID（外键）
- Rating: 评分
- Comment: 评论内容
- IsApproved: 是否审核通过
- CreateTime: 创建时间

#### ProductReviewImage（评价图片）

- ImageId: 图片ID（主键）
- ReviewId: 评价ID（外键）
- ImageUrl: 图片URL
- SortOrder: 排序

#### ProductReviewReply（评价回复）

- ReplyId: 回复ID（主键）
- ReviewId: 评价ID（外键）
- ManagerId: 管理员ID（外键）
- ReplyContent: 回复内容
- CreateTime: 创建时间

#### ProductReviewVote（评价投票）

- VoteId: 投票ID（主键）
- ReviewId: 评价ID（外键）
- UserId: 用户ID（外键）
- IsUseful: 是否有用
- CreateTime: 创建时间

#### ShoppingCartItem（购物车项）

- CartItemId: 购物车项ID（主键）
- UserId: 用户ID（外键）
- ProductId: 商品ID（外键）
- Quantity: 数量
- CreateTime: 创建时间
- UpdateTime: 更新时间

#### Order（订单）

- OrderId: 订单ID（主键）
- OrderNumber: 订单号
- UserId: 用户ID（外键）
- TotalAmount: 总金额
- Currency: 货币
- PaymentMode: 支付方式
- PaymentMethod: 支付方法
- PaymentStatus: 支付状态
- Status: 订单状态
- Remark: 备注
- CreateTime: 创建时间
- UpdateTime: 更新时间

#### OrderItem（订单项）

- OrderItemId: 订单项ID（主键）
- OrderId: 订单ID（外键）
- ProductId: 商品ID（外键）
- ProductName: 商品名称
- Quantity: 数量
- UnitPrice: 单价
- Subtotal: 小计

#### OrderPaymentLog（支付日志）

- LogId: 日志ID（主键）
- OrderId: 订单ID（外键）
- PaymentType: 支付类型
- PaymentData: 支付数据
- Status: 状态
- CreateTime: 创建时间

#### OrderShipping（订单物流）

- ShippingId: 物流ID（主键）
- OrderId: 订单ID（外键）
- ShippingCompany: 物流公司
- TrackingNumber: 物流单号
- Status: 状态
- CreateTime: 创建时间
- UpdateTime: 更新时间

#### ShippingTrackingLog（物流跟踪日志）

- LogId: 日志ID（主键）
- ShippingId: 物流ID（外键）
- Status: 状态
- Description: 描述
- Location: 位置
- CreateTime: 创建时间

---

### 配置相关实体

#### ChainNetworkConfig（区块链网络配置）

- ChainId: 链ID（主键）
- ChainName: 链名称
- RpcUrl: RPC URL
- ExplorerUrl: 浏览器URL
- IsActive: 是否激活

#### ChainTokenConfig（代币配置）

- TokenId: 代币ID（主键）
- ChainId: 链ID（外键）
- TokenAddress: 代币地址
- TokenSymbol: 代币符号
- TokenName: 代币名称
- Decimals: 小数位数
- IsActive: 是否激活

#### ChainWalletConfig（钱包配置）

- WalletId: 钱包ID（主键）
- ChainId: 链ID（外键）
- WalletAddress: 钱包地址
- PrivateKey: 私钥（加密）
- IsActive: 是否激活

#### UserLevelConfig（用户等级配置）

- LevelId: 等级ID（主键）
- LevelName: 等级名称
- MinGrowthValue: 最小成长值
- MaxGrowthValue: 最大成长值
- Benefits: 权益（JSON）

#### GlobalConfig（全局配置）

- ConfigId: 配置ID（主键）
- ConfigKey: 配置键
- ConfigValue: 配置值
- Description: 描述

#### ManagerTypeConfig（管理员类型配置）

- ManagerTypeId: 管理员类型ID（主键）
- TypeName: 类型名称
- Permissions: 权限（JSON）

---

### 管理端相关实体

#### Manager（管理员）

- ManagerId: 管理员ID（主键）
- ManagerType: 管理员类型
- UserName: 用户名
- PasswordHash: 密码哈希
- IsActive: 是否激活
- CreateTime: 创建时间

#### ManagerBalanceChange（管理员账变）

- ChangeId: 账变ID（主键）
- ManagerId: 管理员ID（外键）
- ChangeType: 账变类型
- Amount: 金额
- BeforeBalance: 变化前余额
- AfterBalance: 变化后余额
- CreateTime: 创建时间

#### ManagerOperationLog（操作日志）

- LogId: 日志ID（主键）
- ManagerId: 管理员ID（外键）
- OperationType: 操作类型
- OperationData: 操作数据
- CreateTime: 创建时间

#### ManagerLoginLog（登录日志）

- LogId: 日志ID（主键）
- ManagerId: 管理员ID（外键）
- LoginTime: 登录时间
- IpAddress: IP地址
- UserAgent: 用户代理

#### ManagerAiTradingActivationCode（AI交易激活码）

- CodeId: 激活码ID（主键）
- ActivationCode: 激活码
- IsUsed: 是否已使用
- UsedByUserId: 使用用户ID
- CreateTime: 创建时间
- UseTime: 使用时间

---

## 核心业务流程

### 电商购物流程

1. **浏览商品**
   - 用户访问首页
   - 浏览商品列表
   - 使用搜索和筛选功能
   - 查看商品详情

2. **加入购物车**
   - 选择商品数量
   - 添加到购物车
   - 未登录用户：添加到临时购物车（localStorage）
   - 已登录用户：添加到数据库购物车

3. **结算**
   - 进入购物车页面
   - 调整商品数量
   - 确认商品信息
   - 创建订单

4. **支付**
   - 选择支付方式：
     - Web3支付（MetaMask/Bitget Wallet）
     - 传统支付
   - Web3支付流程：
     - 生成支付签名
     - 钱包签名确认
     - 链上交易提交
     - 交易状态查询
     - 支付完成确认
   - 传统支付流程：
     - 选择支付方式
     - 提交支付信息
     - 支付确认

5. **订单处理**
   - 订单状态更新
   - 商品发货
   - 物流跟踪
   - 订单完成

6. **商品评价**
   - 用户评价商品
   - 上传评价图片
   - 商家回复评价
   - 其他用户投票

---

### 资产管理流程

1. **钱包连接**
   - 检测已安装钱包
   - 连接钱包（MetaMask/Bitget Wallet）
   - 网络切换
   - 授权代币

2. **充值（转账到链上）**
   - 选择代币
   - 输入金额
   - 验证余额
   - 确认转账
   - 链上交易提交
   - 交易状态查询
   - 资产更新

3. **提现（转账到钱包）**
   - 选择代币
   - 输入金额和钱包地址
   - 验证余额
   - 确认转账
   - 创建提现订单
   - 管理员审核
   - 链上转账执行
   - 订单完成

4. **资产查询**
   - 查看资产详情
   - 查看资产变化历史
   - 查看转账订单
   - 资产统计

---

### AI交易流程

1. **激活AI交易**
   - 输入激活码
   - 验证激活码
   - 激活成功

2. **创建交易订单**
   - 选择聚合模式（CEX/DEX）
   - 输入交易金额
   - 查看预计收益
   - 确认创建订单

3. **交易执行**
   - 系统执行交易
   - 交易状态更新
   - 收益计算

4. **查看交易记录**
   - 查看交易历史
   - 查看收益记录
   - 交易统计

---

### 挖矿流程

1. **查看挖矿状态**
   - 查看挖矿激活状态
   - 查看有效资产
   - 查看挖矿收益统计

2. **挖矿奖励**
   - 系统自动计算奖励
   - 奖励发放
   - 奖励记录

3. **查看奖励记录**
   - 查看奖励列表
   - 查看奖励详情
   - 奖励统计

---

### 用户注册/登录流程

1. **钱包连接**
   - 检测已安装钱包
   - 连接钱包
   - 获取钱包地址

2. **签名登录**
   - 获取签名消息
   - 钱包签名
   - 提交签名
   - 验证签名
   - 生成JWT Token

3. **自动注册**
   - 新用户自动创建账户
   - 设置默认等级
   - 初始化资产

4. **登录状态保持**
   - Token存储
   - 自动刷新
   - 登录状态检查

---

## Web3功能

### 多钱包支持

- **MetaMask**
  - 自动检测
  - 连接/断开
  - 网络切换
  - 交易签名

- **Bitget Wallet**
  - 自动检测
  - 连接/断开
  - 网络切换
  - 交易签名
  - OmniConnect集成

### 多链支持

- **Polygon**
  - MATIC余额查询
  - ERC20代币余额查询
  - MATIC转账
  - ERC20代币转账
  - 交易状态查询
  - Gas价格查询
  - Gas费用估算

- **TRON**
  - TRX余额查询
  - TRC20代币余额查询
  - TRX转账
  - TRC20代币转账
  - 交易状态查询
  - 钱包创建

### 多代币支持

- **ERC20代币**
  - 代币余额查询
  - 代币转账
  - 代币授权
  - 代币信息查询

- **TRC20代币**
  - 代币余额查询
  - 代币转账
  - 代币授权
  - 代币信息查询

### Web3支付

- 支付签名生成
- 钱包签名确认
- 链上交易提交
- 交易状态跟踪
- 支付完成确认
- 支付失败处理

---

## 安全功能

### 认证与授权

- **JWT认证**
  - Token生成
  - Token验证
  - Token刷新
  - Token过期处理

- **Web3签名验证**
  - 签名消息生成
  - 签名验证
  - 防重放攻击
  - 签名过期控制

- **权限控制**
  - DappUser（普通用户）
  - Manager（管理员）
  - Administrator（超级管理员）
  - Agent（代理）
  - GroupLeader（组长）
  - Developer（开发者）

### 安全措施

- **账户异常检测**
  - 登录异常检测
  - 交易异常检测
  - 自动锁定机制

- **交易签名验证**
  - 交易数据验证
  - 签名验证
  - 防篡改机制

- **支付安全**
  - 支付过期时间控制
  - 支付金额验证
  - 支付状态验证
  - 防重复支付

- **数据加密**
  - 敏感数据加密存储
  - 私钥加密存储
  - 传输加密（HTTPS）

---

## 管理端功能

### 商品管理

- 创建商品
- 更新商品
- 删除商品
- 商品图片管理
- 商品规格管理
- 商品库存管理

### 订单管理

- 订单列表查询
- 订单详情查看
- 订单状态更新
- 订单发货
- 物流信息更新

### 用户管理

- 用户列表查询
- 用户详情查看
- 用户备注更新
- 用户归属转移
- 提现订单处理

### 评价管理

- 评价审核
- 评价删除
- 商家回复评价
- 待审核评价列表

### 库存管理

- 库存更新
- 库存调整
- 库存预留
- 库存释放
- 低库存预警
- 库存统计

### 配置管理

- 全局配置
- 用户等级配置
- 区块链网络配置
- 代币配置
- 管理员类型配置

### 日志管理

- 操作日志查询
- 登录日志查询
- 账变记录查询

---

## 统计与分析功能

### 用户统计

- 邀请数统计
- 订单数统计
- 交易数统计
- 奖励统计
- 成长值统计

### 资产统计

- 充值统计
- 提现统计
- 奖励统计
- 资产变化趋势

### 订单统计

- 订单数量统计
- 订单金额统计
- 订单状态分布
- 支付方式分布

### 库存统计

- 库存总量统计
- 库存分布统计
- 低库存商品统计
- 库存变化趋势

### 商品统计

- 商品销量统计
- 商品评价统计
- 热门商品统计
- 商品分类分布

---

## 其他功能

### 访客模式

- 无需钱包即可浏览商品
- 临时购物车（localStorage）
- 连接钱包后自动转移购物车

### 多语言支持

- 前端可扩展多语言
- 语言切换功能

### 主题切换

- 深色主题
- 浅色主题
- 自动切换

### 响应式设计

- 移动端适配
- 平板适配
- PC端适配

### 错误处理

- 友好的错误提示
- 错误页面
- 错误日志记录

---

## 技术特性

### 前端特性

- Vue 3 Composition API
- TypeScript类型安全
- Pinia状态管理
- Vuetify 3 UI组件
- 响应式设计
- 代码分割
- 懒加载

### 后端特性

- .NET 8高性能
- Entity Framework Core ORM
- JWT认证
- Web3签名验证
- RESTful API
- Swagger文档
- 健康检查

### 数据库特性

- SQL Server数据库
- 关系型数据模型
- 索引优化
- 事务支持
- 数据迁移

### 区块链特性

- 多链支持
- 多钱包支持
- 多代币支持
- 交易签名
- 交易状态查询
- Gas费用估算

---

## 部署与运维

### 开发环境

- 本地开发服务器
- 热重载
- 调试工具
- 日志输出

### 生产环境

- HTTPS支持
- CORS配置
- 静态文件服务
- 健康检查
- 错误处理

### 数据库迁移

- EF Core迁移
- 自动迁移
- 手动迁移
- 数据种子

---

## 总结

UnifiedWeb3Platform 是一个功能完整的Web3全栈电商平台，集成了：

- ✅ **完整的电商功能**：商品管理、购物车、订单、支付、物流、评价
- ✅ **Web3支付**：支持MetaMask、Bitget Wallet等多钱包
- ✅ **多链支持**：Polygon、TRON双链支持
- ✅ **资产管理**：充值、提现、查询、统计
- ✅ **AI交易**：AI合约交易功能
- ✅ **免质押挖矿**：挖矿奖励系统
- ✅ **访客模式**：无需钱包即可浏览商品
- ✅ **管理端**：完整的后台管理功能
- ✅ **安全功能**：JWT认证、Web3签名验证、权限控制
- ✅ **响应式设计**：支持移动端和PC端

所有功能均已实现并通过编译检查，可以正常使用。

---

**文档结束**

