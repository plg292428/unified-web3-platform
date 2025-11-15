export interface ChainInfo {
  chainId: number
  name: string
  shortName: string
}

const chainInfos: ChainInfo[] = [
  { chainId: 1, name: 'Ethereum Mainnet', shortName: 'ETH' },
  { chainId: 5, name: 'Goerli Testnet', shortName: 'Goerli' },
  { chainId: 11155111, name: 'Sepolia Testnet', shortName: 'Sepolia' },
  { chainId: 56, name: 'BNB Smart Chain', shortName: 'BSC' },
  { chainId: 97, name: 'BSC Testnet', shortName: 'BSC Test' },
  { chainId: 137, name: 'Polygon Mainnet', shortName: 'Polygon' },
  { chainId: 80001, name: 'Polygon Mumbai', shortName: 'Mumbai' },
  { chainId: 42161, name: 'Arbitrum One', shortName: 'Arbitrum' },
  { chainId: 421613, name: 'Arbitrum Goerli', shortName: 'Arbitrum Test' },
  { chainId: 10, name: 'Optimism', shortName: 'Optimism' },
  { chainId: 8453, name: 'Base', shortName: 'Base' },
  { chainId: 43114, name: 'Avalanche C-Chain', shortName: 'Avalanche' },
  { chainId: 43113, name: 'Avalanche Fuji', shortName: 'Fuji' }
]

const chainInfoMap = new Map<number, ChainInfo>(chainInfos.map((info) => [info.chainId, info]))

export function getChainInfo(chainId: number | null | undefined): ChainInfo | undefined {
  if (chainId === null || chainId === undefined) {
    return undefined
  }
  return chainInfoMap.get(chainId)
}

