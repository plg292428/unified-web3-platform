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
      throw new Error(`HTTP ${response.status}`)
    }
    await response.json()
    return performance.now() - started
  } finally {
    clearTimeout(timeout)
  }
}

