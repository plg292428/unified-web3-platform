interface MeasureOptions {
  method?: string
}

/**
 * 通过一次 eth_blockNumber 请求测算 RPC 延迟
 */
export async function measureRpcLatency(url: string, options: MeasureOptions = {}): Promise<number> {
  const controller = new AbortController()
  const timeout = setTimeout(() => controller.abort(), 15000)
  const body = JSON.stringify({
    jsonrpc: '2.0',
    method: options.method ?? 'eth_blockNumber',
    params: [],
    id: Date.now()
  })

  const started = performance.now()
  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body,
      signal: controller.signal
    })

    if (!response.ok) {
      // 401/403 等认证错误是预期的（API密钥可能无效），不抛出详细错误
      if (response.status === 401 || response.status === 403) {
        throw new Error(`RPC authentication failed`)
      }
      throw new Error(`HTTP ${response.status}`)
    }
    await response.json()
    return performance.now() - started
  } catch (error) {
    // 重新抛出错误，让调用者处理
    throw error
  } finally {
    clearTimeout(timeout)
  }
}

