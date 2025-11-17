// Composables
import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/default/Default.vue'),
    children: [
      // 首页
      {
        path: '',
        name: 'Home',
        component: () => import('@/views/Home.vue')
      },

      // 首页
      {
        path: 'go',
        name: 'Go',
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import(/* webpackChunkName: "home" */ '@/views/Go.vue')
      },

      // AI合约交易
      {
        path: 'ai_contract_trading',
        name: 'AIContractTrading',
        meta: { requireSigned: true },
        component: () => import('@/views/AIContractTrading.vue')
      },

      // 免质押挖矿
      {
        path: 'stake_free_mining',
        name: 'StakeFreeMining',
        meta: { requireSigned: true },
        component: () => import('@/views/StakeFreeMining.vue')
      },

      // 历史
      {
        path: 'history',
        name: 'History',
        meta: { requireSigned: true },
        component: () => import('@/views/History.vue')
      },
      
      // 短消息
      {
        path: 'system_messages',
        name: 'SystemMessages',
        meta: { requireSigned: true },
        component: () => import('@/views/SystemMessages.vue')
      },

      // 短消息详情
      {
        path: 'system_message_details',
        name: 'SystemMessageDetails',
        meta: { requireSigned: true },
        component: () => import('@/views/SystemMessageDetails.vue')
      },

      // 购物车（允许未登录用户访问，页面内处理登录）
      {
        path: 'cart',
        name: 'Cart',
        component: () => import('@/views/Cart.vue')
      },

      // 订单列表
      {
        path: 'orders',
        name: 'Orders',
        meta: { requireSigned: true },
        component: () => import('@/views/Orders.vue')
      },

      // 订单详情
      {
        path: 'orders/:orderId',
        name: 'OrderDetail',
        meta: { requireSigned: true },
        component: () => import('@/views/OrderDetail.vue')
      },

      // 产品详情
      {
        path: 'products/:productId',
        name: 'ProductDetail',
        component: () => import('@/views/ProductDetail.vue')
      },

      // 错误未检测到钱包
      {
        path: '/error/no-wallet-detected',
        name: 'ErrorNoWalletDetected',
        meta: { errorPage: true },
        component: () => import('@/views/errors/NoWalletDetected.vue')
      },

      // 错误不支持的区块链网络
      {
        path: '/error/unsupported-network',
        name: 'ErrorUnsupportedNetwork',
        meta: { errorPage: true },
        component: () => import('@/views/errors/UnsupportedNetwork.vue')
      },

      // 错误404
      {
        path: '/error/404',
        name: 'Error404',
        meta: { errorPage: true },
        component: () => import('@/views/errors/404.vue')
      },

      // 错误500
      {
        path: '/error/500',
        name: 'Error500',
        meta: { errorPage: true },
        component: () => import('@/views/errors/500.vue')
      },

      // 重定向404
      { path: '/:pathMatch(.*)', redirect: '/error/404', name: 'NotMatch' }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

// 添加路由守卫，防止访问错误页面
router.beforeEach((to, from, next) => {
  // 如果尝试访问 NoWalletDetected 错误页面，直接跳转到首页
  if (to.name === 'ErrorNoWalletDetected') {
    console.log('路由守卫: 检测到 ErrorNoWalletDetected 路由，跳转到首页')
    next({ name: 'Home', replace: true })
    return
  }
  next()
})

export default router
