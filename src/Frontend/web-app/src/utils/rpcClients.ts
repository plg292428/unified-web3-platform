import { ethers } from "ethers";

// 延迟初始化 provider，避免在模块加载时立即连接 RPC
// 只有在实际使用时才创建 provider

// 公共 RPC 端点作为备选（如果环境变量未配置或无效）
const DEFAULT_ETHEREUM_RPC = "https://eth.llamarpc.com"
const DEFAULT_POLYGON_RPC = "https://polygon.llamarpc.com"

let _ethProvider: ethers.JsonRpcProvider | null = null
let _polygonProvider: ethers.JsonRpcProvider | null = null

function getEthereumRpcUrl(): string {
  const envUrl = import.meta.env.VITE_RPC_ETHEREUM
  if (envUrl && typeof envUrl === 'string' && envUrl.trim()) {
    return envUrl.trim()
  }
  return DEFAULT_ETHEREUM_RPC
}

function getPolygonRpcUrl(): string {
  const envUrl = import.meta.env.VITE_RPC_POLYGON
  if (envUrl && typeof envUrl === 'string' && envUrl.trim()) {
    return envUrl.trim()
  }
  return DEFAULT_POLYGON_RPC
}

// 懒加载 Ethereum Provider
export function getEthProvider(): ethers.JsonRpcProvider {
  if (!_ethProvider) {
    const rpcUrl = getEthereumRpcUrl()
    try {
      _ethProvider = new ethers.JsonRpcProvider(rpcUrl)
      console.log('[RPC] Ethereum provider initialized:', rpcUrl)
    } catch (error) {
      console.error('[RPC] Failed to create Ethereum provider:', error)
      // 使用默认 RPC 作为备选
      _ethProvider = new ethers.JsonRpcProvider(DEFAULT_ETHEREUM_RPC)
    }
  }
  return _ethProvider
}

// 懒加载 Polygon Provider
export function getPolygonProvider(): ethers.JsonRpcProvider {
  if (!_polygonProvider) {
    const rpcUrl = getPolygonRpcUrl()
    try {
      _polygonProvider = new ethers.JsonRpcProvider(rpcUrl)
      console.log('[RPC] Polygon provider initialized:', rpcUrl)
    } catch (error) {
      console.error('[RPC] Failed to create Polygon provider:', error)
      // 使用默认 RPC 作为备选
      _polygonProvider = new ethers.JsonRpcProvider(DEFAULT_POLYGON_RPC)
    }
  }
  return _polygonProvider
}

// 获取 RPC URL（用于调试和检查）
export function getRpcUrls() {
  return {
    ethereum: getEthereumRpcUrl(),
    polygon: getPolygonRpcUrl(),
    ethereumConfigured: !!import.meta.env.VITE_RPC_ETHEREUM,
    polygonConfigured: !!import.meta.env.VITE_RPC_POLYGON,
    ethereumEnv: import.meta.env.VITE_RPC_ETHEREUM || null,
    polygonEnv: import.meta.env.VITE_RPC_POLYGON || null
  }
}

// 直接导出 provider（延迟初始化）
// 注意：这些会在首次访问时初始化，而不是在模块加载时
// 使用 Proxy 实现懒加载，保持向后兼容
export const ethProvider = (() => {
  let provider: ethers.JsonRpcProvider | null = null
  return new Proxy({} as any, {
    get(_target, prop) {
      if (!provider) {
        provider = getEthProvider()
      }
      const value = provider[prop as keyof ethers.JsonRpcProvider]
      if (typeof value === 'function') {
        return value.bind(provider)
      }
      return value
    }
  })
})()

export const polygonProvider = (() => {
  let provider: ethers.JsonRpcProvider | null = null
  return new Proxy({} as any, {
    get(_target, prop) {
      if (!provider) {
        provider = getPolygonProvider()
      }
      const value = provider[prop as keyof ethers.JsonRpcProvider]
      if (typeof value === 'function') {
        return value.bind(provider)
      }
      return value
    }
  })
})()