import TronWeb from 'tronweb'
import { Web3HelperResult } from '@/types'

/**
 * TRON Web3 助手类
 * 提供TRON区块链的常用操作
 */
export default class TronWebHelper {
  private tronWeb: TronWeb | null = null
  private network: 'mainnet' | 'shasta' | 'nile' = 'shasta'

  /**
   * 初始化TronWeb实例
   * @param network 网络类型: 'mainnet' | 'shasta' | 'nile'
   * @param privateKey 私钥（可选，用于签名交易）
   */
  public initialize(network: 'mainnet' | 'shasta' | 'nile' = 'shasta', privateKey?: string): void {
    this.network = network

    const fullNode = this.getFullNode(network)
    const solidityNode = this.getSolidityNode(network)
    const eventServer = this.getEventServer(network)

    if (privateKey) {
      this.tronWeb = new TronWeb({
        fullHost: fullNode,
        privateKey: privateKey
      })
    } else {
      this.tronWeb = new TronWeb({
        fullHost: fullNode
      })
    }
  }

  /**
   * 获取FullNode地址
   */
  private getFullNode(network: 'mainnet' | 'shasta' | 'nile'): string {
    switch (network) {
      case 'mainnet':
        return 'https://api.trongrid.io'
      case 'shasta':
        return 'https://api.shasta.trongrid.io'
      case 'nile':
        return 'https://api.nileex.io'
      default:
        return 'https://api.shasta.trongrid.io'
    }
  }

  /**
   * 获取SolidityNode地址
   */
  private getSolidityNode(network: 'mainnet' | 'shasta' | 'nile'): string {
    switch (network) {
      case 'mainnet':
        return 'https://api.trongrid.io'
      case 'shasta':
        return 'https://api.shasta.trongrid.io'
      case 'nile':
        return 'https://api.nileex.io'
      default:
        return 'https://api.shasta.trongrid.io'
    }
  }

  /**
   * 获取EventServer地址
   */
  private getEventServer(network: 'mainnet' | 'shasta' | 'nile'): string {
    switch (network) {
      case 'mainnet':
        return 'https://api.trongrid.io'
      case 'shasta':
        return 'https://api.shasta.trongrid.io'
      case 'nile':
        return 'https://api.nileex.io'
      default:
        return 'https://api.shasta.trongrid.io'
    }
  }

  /**
   * 获取TronWeb实例
   */
  public getInstance(): TronWeb {
    if (!this.tronWeb) {
      this.initialize(this.network)
    }
    return this.tronWeb!
  }

  /**
   * 查询TRX余额
   * @param address TRON地址
   */
  public async getTrxBalance(address: string): Promise<Web3HelperResult<string>> {
    const result: Web3HelperResult<string> = { succeed: false }
    try {
      if (!this.tronWeb) {
        this.initialize(this.network)
      }
      const balance = await this.tronWeb!.trx.getBalance(address)
      result.data = this.tronWeb!.fromSun(balance)
      result.succeed = true
    } catch (error) {
      console.error('获取TRX余额失败:', error)
      result.errorMessage = error instanceof Error ? error.message : 'Unknown error'
    }
    return result
  }

  /**
   * 查询TRC20代币余额
   * @param contractAddress TRC20合约地址
   * @param address 钱包地址
   */
  public async getTrc20Balance(contractAddress: string, address: string): Promise<Web3HelperResult<string>> {
    const result: Web3HelperResult<string> = { succeed: false }
    try {
      if (!this.tronWeb) {
        this.initialize(this.network)
      }
      const contract = await this.tronWeb!.contract().at(contractAddress)
      const balance = await contract.balanceOf(address).call()
      const decimals = await contract.decimals().call()
      const balanceFormatted = balance.toString() / Math.pow(10, decimals)
      result.data = balanceFormatted.toString()
      result.succeed = true
    } catch (error) {
      console.error('获取TRC20余额失败:', error)
      result.errorMessage = error instanceof Error ? error.message : 'Unknown error'
    }
    return result
  }

  /**
   * 转账TRX
   * @param fromAddress 发送地址
   * @param toAddress 接收地址
   * @param amount 金额（TRX）
   * @param privateKey 私钥
   */
  public async transferTrx(
    fromAddress: string,
    toAddress: string,
    amount: string,
    privateKey: string
  ): Promise<Web3HelperResult<string>> {
    const result: Web3HelperResult<string> = { succeed: false }
    try {
      if (!this.tronWeb) {
        this.initialize(this.network, privateKey)
      } else {
        this.tronWeb.setPrivateKey(privateKey)
      }

      const amountInSun = this.tronWeb!.toSun(amount)
      const transaction = await this.tronWeb!.transactionBuilder.sendTrx(
        toAddress,
        amountInSun,
        fromAddress
      )
      const signedTransaction = await this.tronWeb!.trx.sign(transaction, privateKey)
      const broadcastResult = await this.tronWeb!.trx.broadcast(signedTransaction)
      
      if (broadcastResult.result) {
        result.data = broadcastResult.txid
        result.succeed = true
      } else {
        result.errorMessage = broadcastResult.message || 'Transaction failed'
      }
    } catch (error) {
      console.error('TRX转账失败:', error)
      result.errorMessage = error instanceof Error ? error.message : 'Unknown error'
    }
    return result
  }

  /**
   * 转账TRC20代币
   * @param contractAddress TRC20合约地址
   * @param fromAddress 发送地址
   * @param toAddress 接收地址
   * @param amount 金额
   * @param privateKey 私钥
   */
  public async transferTrc20(
    contractAddress: string,
    fromAddress: string,
    toAddress: string,
    amount: string,
    privateKey: string
  ): Promise<Web3HelperResult<string>> {
    const result: Web3HelperResult<string> = { succeed: false }
    try {
      if (!this.tronWeb) {
        this.initialize(this.network, privateKey)
      } else {
        this.tronWeb.setPrivateKey(privateKey)
      }

      const contract = await this.tronWeb!.contract().at(contractAddress)
      const decimals = await contract.decimals().call()
      const amountInSmallestUnit = BigInt(parseFloat(amount) * Math.pow(10, decimals))

      const transaction = await contract.transfer(toAddress, amountInSmallestUnit.toString()).send()
      const signedTransaction = await this.tronWeb!.trx.sign(transaction, privateKey)
      const broadcastResult = await this.tronWeb!.trx.broadcast(signedTransaction)

      if (broadcastResult.result) {
        result.data = broadcastResult.txid
        result.succeed = true
      } else {
        result.errorMessage = broadcastResult.message || 'Transaction failed'
      }
    } catch (error) {
      console.error('TRC20转账失败:', error)
      result.errorMessage = error instanceof Error ? error.message : 'Unknown error'
    }
    return result
  }

  /**
   * 查询交易状态
   * @param txId 交易ID
   */
  public async getTransactionStatus(txId: string): Promise<Web3HelperResult<any>> {
    const result: Web3HelperResult<any> = { succeed: false }
    try {
      if (!this.tronWeb) {
        this.initialize(this.network)
      }
      const txInfo = await this.tronWeb!.trx.getTransactionInfo(txId)
      result.data = {
        txId: txId,
        status: txInfo ? 'SUCCESS' : 'PENDING',
        isSuccess: txInfo?.receipt?.result === 'SUCCESS',
        blockNumber: txInfo?.blockNumber,
        blockTimeStamp: txInfo?.blockTimeStamp
      }
      result.succeed = true
    } catch (error) {
      console.error('查询交易状态失败:', error)
      result.errorMessage = error instanceof Error ? error.message : 'Unknown error'
    }
    return result
  }

  /**
   * 创建新钱包
   */
  public createWallet(): { address: string; privateKey: string; publicKey: string } {
    if (!this.tronWeb) {
      this.initialize(this.network)
    }
    const account = this.tronWeb!.utils.accounts.generateAccount()
    return {
      address: account.address.base58,
      privateKey: account.privateKey,
      publicKey: account.publicKey
    }
  }

  /**
   * 从私钥获取地址
   * @param privateKey 私钥
   */
  public getAddressFromPrivateKey(privateKey: string): string {
    if (!this.tronWeb) {
      this.initialize(this.network)
    }
    return this.tronWeb!.address.fromPrivateKey(privateKey)
  }

  /**
   * 验证TRON地址格式
   * @param address TRON地址
   */
  public isValidAddress(address: string): boolean {
    if (!this.tronWeb) {
      this.initialize(this.network)
    }
    return this.tronWeb!.isAddress(address)
  }
}

