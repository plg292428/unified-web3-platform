using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnifiedPlatform.WebApi.Services.Tron;
using HFastKit.AspNetCore.Shared.Text;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// TRON 区块链控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TronController : ControllerBase
    {
        private readonly ITronService _tronService;
        private readonly ILogger<TronController> _logger;

        public TronController(ITronService tronService, ILogger<TronController> logger)
        {
            _tronService = tronService ?? throw new ArgumentNullException(nameof(tronService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 创建新钱包
        /// </summary>
        /// <returns>钱包信息</returns>
        [HttpPost("wallet/create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateWallet()
        {
            try
            {
                var wallet = await _tronService.CreateWalletAsync();
                return Ok(new { success = true, data = wallet });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建钱包失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 从私钥获取钱包信息
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <returns>钱包信息</returns>
        [HttpPost("wallet/from-private-key")]
        [AllowAnonymous]
        public IActionResult GetWalletFromPrivateKey([FromBody] string privateKey)
        {
            try
            {
                var wallet = _tronService.GetWalletFromPrivateKey(privateKey);
                return Ok(new { success = true, data = wallet });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从私钥获取钱包信息失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 查询 TRX 余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <returns>TRX 余额</returns>
        [HttpGet("balance/trx/{address}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrxBalance(string address)
        {
            try
            {
                var balance = await _tronService.GetTrxBalanceAsync(address);
                return Ok(new { success = true, data = new { address, balance, unit = "TRX" } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询TRX余额失败: {Address}", address);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 查询 TRC20 代币余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">TRC20 合约地址</param>
        /// <returns>代币余额</returns>
        [HttpGet("balance/trc20/{address}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrc20Balance(string address, [FromQuery] string contractAddress)
        {
            try
            {
                var balance = await _tronService.GetTrc20BalanceAsync(address, contractAddress);
                return Ok(new { success = true, data = new { address, contractAddress, balance } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询TRC20余额失败: {Address}, 合约: {Contract}", address, contractAddress);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 转账 TRX
        /// </summary>
        /// <param name="request">转账请求</param>
        /// <returns>交易ID</returns>
        [HttpPost("transfer/trx")]
        [Authorize] // 需要JWT认证
        public async Task<IActionResult> TransferTrx([FromBody] TransferTrxRequest request)
        {
            try
            {
                var transactionId = await _tronService.TransferTrxAsync(
                    request.FromPrivateKey,
                    request.ToAddress,
                    request.Amount,
                    request.Memo
                );

                return Ok(new { success = true, data = new { transactionId } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TRX转账失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 转账 TRC20 代币
        /// </summary>
        /// <param name="request">转账请求</param>
        /// <returns>交易ID</returns>
        [HttpPost("transfer/trc20")]
        [Authorize] // 需要JWT认证
        public async Task<IActionResult> TransferTrc20([FromBody] TransferTrc20Request request)
        {
            try
            {
                var transactionId = await _tronService.TransferTrc20Async(
                    request.FromPrivateKey,
                    request.ToAddress,
                    request.ContractAddress,
                    request.Amount,
                    request.Memo
                );

                return Ok(new { success = true, data = new { transactionId } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TRC20转账失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 查询交易状态
        /// </summary>
        /// <param name="transactionId">交易ID</param>
        /// <returns>交易状态</returns>
        [HttpGet("transaction/{transactionId}/status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTransactionStatus(string transactionId)
        {
            try
            {
                // URL解码（处理URL编码的交易ID）
                transactionId = Uri.UnescapeDataString(transactionId);

                // 验证交易ID格式
                if (!FormatValidate.IsTransactionId(transactionId))
                {
                    return BadRequest(new { 
                        success = false, 
                        message = $"交易ID格式无效: {transactionId}。交易ID必须是64个字符的十六进制字符串（可选0x前缀）" 
                    });
                }

                var status = await _tronService.GetTransactionStatusAsync(transactionId);
                return Ok(new { success = true, data = status });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "交易ID格式验证失败: {TxId}", transactionId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询交易状态失败: {TxId}", transactionId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    /// <summary>
    /// 转账 TRX 请求
    /// </summary>
    public class TransferTrxRequest
    {
        /// <summary>
        /// 发送方私钥
        /// </summary>
        public string FromPrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// 接收方地址
        /// </summary>
        public string ToAddress { get; set; } = string.Empty;

        /// <summary>
        /// 转账金额（TRX单位）
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注（可选）
        /// </summary>
        public string? Memo { get; set; }
    }

    /// <summary>
    /// 转账 TRC20 请求
    /// </summary>
    public class TransferTrc20Request
    {
        /// <summary>
        /// 发送方私钥
        /// </summary>
        public string FromPrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// 接收方地址
        /// </summary>
        public string ToAddress { get; set; } = string.Empty;

        /// <summary>
        /// TRC20 合约地址
        /// </summary>
        public string ContractAddress { get; set; } = string.Empty;

        /// <summary>
        /// 转账金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注（可选）
        /// </summary>
        public string? Memo { get; set; }
    }
}



