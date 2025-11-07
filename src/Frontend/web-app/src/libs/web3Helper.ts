import { Web3HelperResult } from '@/types'
import { BrowserProvider, Contract, ethers, toBigInt } from 'ethers'

export default class Web3Helper {
  private abi = [
    {
      constant: true,
      inputs: [],
      name: 'name',
      outputs: [
        {
          name: '',
          type: 'string'
        }
      ],
      payable: false,
      stateMutability: 'view',
      type: 'function'
    },
    {
      constant: false,
      inputs: [
        {
          name: '_spender',
          type: 'address'
        },
        {
          name: '_value',
          type: 'uint256'
        }
      ],
      name: 'approve',
      outputs: [
        {
          name: '',
          type: 'bool'
        }
      ],
      payable: false,
      stateMutability: 'nonpayable',
      type: 'function'
    },
    {
      constant: true,
      inputs: [],
      name: 'totalSupply',
      outputs: [
        {
          name: '',
          type: 'uint256'
        }
      ],
      payable: false,
      stateMutability: 'view',
      type: 'function'
    },
    {
      constant: false,
      inputs: [
        {
          name: '_from',
          type: 'address'
        },
        {
          name: '_to',
          type: 'address'
        },
        {
          name: '_value',
          type: 'uint256'
        }
      ],
      name: 'transferFrom',
      outputs: [
        {
          name: '',
          type: 'bool'
        }
      ],
      payable: false,
      stateMutability: 'nonpayable',
      type: 'function'
    },
    {
      constant: true,
      inputs: [],
      name: 'decimals',
      outputs: [
        {
          name: '',
          type: 'uint8'
        }
      ],
      payable: false,
      stateMutability: 'view',
      type: 'function'
    },
    {
      constant: true,
      inputs: [
        {
          name: '_owner',
          type: 'address'
        }
      ],
      name: 'balanceOf',
      outputs: [
        {
          name: 'balance',
          type: 'uint256'
        }
      ],
      payable: false,
      stateMutability: 'view',
      type: 'function'
    },
    {
      constant: true,
      inputs: [],
      name: 'symbol',
      outputs: [
        {
          name: '',
          type: 'string'
        }
      ],
      payable: false,
      stateMutability: 'view',
      type: 'function'
    },
    {
      constant: false,
      inputs: [
        {
          name: '_to',
          type: 'address'
        },
        {
          name: '_value',
          type: 'uint256'
        }
      ],
      name: 'transfer',
      outputs: [
        {
          name: '',
          type: 'bool'
        }
      ],
      payable: false,
      stateMutability: 'nonpayable',
      type: 'function'
    },
    {
      constant: true,
      inputs: [
        {
          name: '_owner',
          type: 'address'
        },
        {
          name: '_spender',
          type: 'address'
        }
      ],
      name: 'allowance',
      outputs: [
        {
          name: '',
          type: 'uint256'
        }
      ],
      payable: false,
      stateMutability: 'view',
      type: 'function'
    },
    {
      payable: true,
      stateMutability: 'payable',
      type: 'fallback'
    },
    {
      anonymous: false,
      inputs: [
        {
          indexed: true,
          name: 'owner',
          type: 'address'
        },
        {
          indexed: true,
          name: 'spender',
          type: 'address'
        },
        {
          indexed: false,
          name: 'value',
          type: 'uint256'
        }
      ],
      name: 'Approval',
      type: 'event'
    },
    {
      anonymous: false,
      inputs: [
        {
          indexed: true,
          name: 'from',
          type: 'address'
        },
        {
          indexed: true,
          name: 'to',
          type: 'address'
        },
        {
          indexed: false,
          name: 'value',
          type: 'uint256'
        }
      ],
      name: 'Transfer',
      type: 'event'
    },
    {
      constant: false,
      inputs: [
        {
          internalType: 'address',
          name: 'spender',
          type: 'address'
        },
        {
          internalType: 'uint256',
          name: 'addedValue',
          type: 'uint256'
        }
      ],
      name: 'increaseAllowance',
      outputs: [
        {
          internalType: 'bool',
          name: '',
          type: 'bool'
        }
      ],
      payable: false,
      stateMutability: 'nonpayable',
      type: 'function'
    }
  ] as const

  // Web3 实例
  private readonly browerProvider: BrowserProvider
  private readonly provider: any

  // 根据精度获取单位
  private getUnit(decimals: number) {
    if (decimals < 0) {
      return 'noether'
    }
    switch (decimals) {
      case 0:
        return 'wei'
      case 3:
        return 'kwei'
      case 6:
        return 'mwei'
      case 9:
        return 'gwei'
      case 12:
        return 'szabo'
      case 15:
        return 'finney'
      case 18:
        return 'ether'
      case 21:
        return 'kether'
      case 24:
        return 'mether'
      case 27:
        return 'gether'
      case 30:
        return 'tether'
      default:
        return 'noether'
    }
  }

  // 最小代币值
  public static readonly Token_Min_Value_BIGINT = 1n;

  // 最大代币值
  public static readonly Token_Max_Value_BIGINT = toBigInt(
    '0xffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff'
  )

  // 构造方法
  public constructor(provider: any) {
    this.browerProvider = new BrowserProvider(provider)
    this.provider = provider
  }

  // 查询通货代币余额
  public async currencyBalance(owner: string): Promise<Web3HelperResult<bigint>> {
    const result: Web3HelperResult<bigint> = { succeed: false }
    try {
      const balance = await this.browerProvider.getBalance(owner)
      result.data = balance
      result.succeed = true
    } catch (error) {
      console.log(error)
    }
    return result
  }

  // 查询Erc20代币余额
  public async balanceOf(contractAddress: string, owner: string): Promise<Web3HelperResult<bigint>> {
    const result: Web3HelperResult<bigint> = { succeed: false }
    const contract = new Contract(contractAddress, this.abi, this.browerProvider)
    try {
      const balance = await contract.balanceOf(owner)
      result.data = balance
      result.succeed = true
    } catch (error) {
      console.log(error)
    }
    return result
  }

  // 查询授权代币余额
  public async allowance(
    contractAddress: string,
    owner: string,
    spender: string
  ): Promise<Web3HelperResult<BigInt>> {
    const result: Web3HelperResult<BigInt> = { succeed: false }
    const contract = new Contract(contractAddress, this.abi, this.browerProvider)
    try {
      const value = await contract.allowance(owner, spender)
      result.data = toBigInt(value)
      result.succeed = true
    } catch (error) {
      console.log(error)
    }
    return result
  }

  // 授权
  async approve(contractAddress: string, abiFunctionName: string, spender: string, amount: bigint = Web3Helper.Token_Max_Value_BIGINT): Promise<Web3HelperResult<Object>> {
    const signer = await this.browerProvider.getSigner()
    const contract = new ethers.Contract(contractAddress, this.abi, signer)
    const result: Web3HelperResult<Object> = { succeed: false }
    try {
      let tx
      if (abiFunctionName === 'increaseAllowance') {
        tx = await contract.increaseAllowance(spender, amount)
      } else {
        tx = await contract.approve(spender, amount)
      }
      result.data = tx
      result.succeed = true
    } catch (error) {
      console.log(error)
    }
    return result
  }

  // 转账
  async transfer(
    contractAddress: string,
    to: string,
    amount: bigint
  ): Promise<Web3HelperResult<object>> {
    const signer = await this.browerProvider.getSigner()
    const contract = new Contract(contractAddress, this.abi, signer)
    const result: Web3HelperResult<object> = { succeed: false }
    try {
      const tx = await contract.transfer(to, amount)
      result.data = tx
      result.succeed = true
    } catch (error) {
      console.log(error)
    }
    return result
  }
}
