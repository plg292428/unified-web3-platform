using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnifiedPlatform.WebApi.Services.Polygon;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// Polygon 区块链控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PolygonController : ControllerBase
    {
        private readonly IPolygonService _polygonService;
        private readonly ILogger<PolygonController> _logger;

        public PolygonController(IPolygonService polygonService, ILogger<PolygonController> logger)
        {
            _polygonService = polygonService ?? throw new ArgumentNullException(nameof(polygonService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取 MATIC 余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <returns>MATIC 余额</returns>
        [HttpGet("balance/matic")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMaticBalance([FromQuery] string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    return BadRequest(new { success = false, message = "地址不能为空" });
                }

                var balance = await _polygonService.GetMaticBalanceAsync(address);
                return Ok(new { success = true, data = new { address, balance, currency = "MATIC" } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询MATIC余额失败: {Address}", address);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取 ERC20 代币余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">ERC20 合约地址</param>
        /// <returns>代币余额</returns>
        [HttpGet("balance/erc20")]
        [AllowAnonymous]
        public async Task<IActionResult> GetErc20Balance([FromQuery] string address, [FromQuery] string contractAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    return BadRequest(new { success = false, message = "地址不能为空" });
                }

                if (string.IsNullOrWhiteSpace(contractAddress))
                {
                    return BadRequest(new { success = false, message = "合约地址不能为空" });
                }

                var balance = await _polygonService.GetErc20BalanceAsync(address, contractAddress);
                return Ok(new { success = true, data = new { address, contractAddress, balance } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询ERC20余额失败: {Address}, 合约: {Contract}", address, contractAddress);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 查询交易状态
        /// </summary>
        /// <param name="transactionHash">交易哈希</param>
        /// <returns>交易状态</returns>
        [HttpGet("transaction/{transactionHash}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTransactionStatus(string transactionHash)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transactionHash))
                {
                    return BadRequest(new { success = false, message = "交易哈希不能为空" });
                }

                var status = await _polygonService.GetTransactionStatusAsync(transactionHash);
                return Ok(new { success = true, data = status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询交易状态失败: {TxHash}", transactionHash);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取 Gas 价格
        /// </summary>
        /// <returns>Gas 价格（Gwei）</returns>
        [HttpGet("gas/price")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGasPrice()
        {
            try
            {
                var gasPrice = await _polygonService.GetGasPriceAsync();
                return Ok(new { success = true, data = new { gasPrice, unit = "Gwei" } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取Gas价格失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 估算交易 Gas 费用
        /// </summary>
        /// <param name="fromAddress">发送方地址</param>
        /// <param name="toAddress">接收方地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="contractAddress">合约地址（可选，如果是 ERC20 转账）</param>
        /// <returns>Gas 费用估算（MATIC）</returns>
        [HttpGet("gas/estimate")]
        [AllowAnonymous]
        public async Task<IActionResult> EstimateGasFee(
            [FromQuery] string fromAddress,
            [FromQuery] string toAddress,
            [FromQuery] decimal amount,
            [FromQuery] string? contractAddress = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromAddress))
                {
                    return BadRequest(new { success = false, message = "发送方地址不能为空" });
                }

                if (string.IsNullOrWhiteSpace(toAddress))
                {
                    return BadRequest(new { success = false, message = "接收方地址不能为空" });
                }

                if (amount <= 0)
                {
                    return BadRequest(new { success = false, message = "转账金额必须大于0" });
                }

                var gasFee = await _polygonService.EstimateGasFeeAsync(fromAddress, toAddress, amount, contractAddress);
                return Ok(new { success = true, data = new { gasFee, unit = "MATIC", contractAddress } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "估算Gas费用失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

