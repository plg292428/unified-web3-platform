using Microsoft.EntityFrameworkCore;
using Quartz;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.Enums;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Services.ScheduleJob.Jobs
{
    /// <summary>
    /// 处理订单支付验证和状态更新任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class HandleOrderPaymentVerificationJob : IJob
    {
        private readonly ILogger<HandleOrderPaymentVerificationJob> _logger;
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;
        private readonly IWeb3ProviderService _web3ProviderService;

        public HandleOrderPaymentVerificationJob(
            ILogger<HandleOrderPaymentVerificationJob> logger,
            StDbContext dbContext,
            ITempCaching tempCaching,
            IWeb3ProviderService web3ProviderService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _tempCaching = tempCaching;
            _web3ProviderService = web3ProviderService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation($"{now:yyyy-MM-dd HH:mm:ss} - HandleOrderPaymentVerificationJob started");

            try
            {
                // 1. 处理待验证的 Web3 支付订单
                await VerifyWeb3Payments(now);

                // 2. 处理过期的支付订单
                await HandleExpiredPayments(now);

                // 3. 更新订单确认数
                await UpdatePaymentConfirmations(now);

                _logger.LogInformation($"{now:yyyy-MM-dd HH:mm:ss} - HandleOrderPaymentVerificationJob completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{now:yyyy-MM-dd HH:mm:ss} - HandleOrderPaymentVerificationJob error");
            }
        }

        /// <summary>
        /// 验证 Web3 支付订单的链上交易状态
        /// </summary>
        private async Task VerifyWeb3Payments(DateTime now)
        {
            // 查询需要验证的订单：Web3 支付模式，有交易哈希，状态为等待链上确认
            // 使用 Select 明确指定要查询的列，避免 EF Core 尝试查询不存在的导航属性列
            var ordersToVerify = await _dbContext.Orders
                .Where(o => o.PaymentMode == StorePaymentMode.Web3
                    && !string.IsNullOrWhiteSpace(o.PaymentTransactionHash)
                    && o.PaymentStatus == StorePaymentStatus.AwaitingOnChainConfirmation
                    && o.Status == StoreOrderStatus.PendingPayment)
                .Select(o => new
                {
                    o.OrderId,
                    o.ChainId,
                    o.PaymentTransactionHash
                })
                .ToListAsync();

            if (ordersToVerify.Count == 0)
            {
                return;
            }

            _logger.LogInformation($"Found {ordersToVerify.Count} orders to verify");

            foreach (var order in ordersToVerify)
            {
                try
                {
                    if (order.ChainId == null)
                    {
                        _logger.LogWarning($"Order {order.OrderId} has no ChainId");
                        continue;
                    }

                    var chainNetwork = (ChainNetwork)order.ChainId.Value;
                    var web3Provider = _web3ProviderService.GetSpenderWeb3Provider(
                        _tempCaching.GlobalConfig.ChainWalletConfigGroupId,
                        chainNetwork);

                    if (web3Provider == null)
                    {
                        _logger.LogWarning($"Cannot get Web3Provider for ChainId {order.ChainId}");
                        continue;
                    }

                    // 查询交易状态
                    if (string.IsNullOrWhiteSpace(order.PaymentTransactionHash) || 
                        !web3Provider.QueryTransactionStatus(order.PaymentTransactionHash, out var transactionStatus))
                    {
                        _logger.LogWarning($"Failed to query transaction status for Order {order.OrderId}, TxHash: {order.PaymentTransactionHash}");
                        continue;
                    }

                    // 重新加载订单以进行更新
                    var orderToUpdate = await _dbContext.Orders.FindAsync(order.OrderId);
                    if (orderToUpdate == null)
                    {
                        _logger.LogWarning($"Order {order.OrderId} not found");
                        continue;
                    }

                    // 更新订单状态
                    var updated = false;
                    switch (transactionStatus)
                    {
                        case ChainTransactionStatus.Succeed:
                            // 交易成功，更新订单为已支付
                            orderToUpdate.PaymentStatus = StorePaymentStatus.Confirmed;
                            orderToUpdate.Status = StoreOrderStatus.Paid;
                            orderToUpdate.PaidTime = now;
                            orderToUpdate.PaymentConfirmedTime = now;
                            updated = true;

                            // 记录支付日志
                            _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
                            {
                                OrderId = orderToUpdate.OrderId,
                                PaymentStatus = orderToUpdate.PaymentStatus,
                                EventType = "payment_verified",
                                Message = "链上交易验证成功",
                                CreateTime = now
                            });

                            _logger.LogInformation($"Order {orderToUpdate.OrderId} payment verified successfully");
                            break;

                        case ChainTransactionStatus.Failed:
                            // 交易失败
                            orderToUpdate.PaymentStatus = StorePaymentStatus.Failed;
                            orderToUpdate.PaymentFailureReason = "链上交易失败";
                            updated = true;

                            // 恢复库存
                            await RestoreInventory(orderToUpdate.OrderId, now);

                            // 记录支付日志
                            _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
                            {
                                OrderId = orderToUpdate.OrderId,
                                PaymentStatus = orderToUpdate.PaymentStatus,
                                EventType = "payment_failed",
                                Message = "链上交易失败",
                                CreateTime = now
                            });

                            _logger.LogWarning($"Order {orderToUpdate.OrderId} payment failed on chain");
                            break;

                        case ChainTransactionStatus.Pending:
                            // 交易仍在等待确认，更新确认数
                            await UpdateConfirmationCount(orderToUpdate, web3Provider, now);
                            break;

                        case ChainTransactionStatus.None:
                            // 交易不存在，可能是交易哈希错误
                            _logger.LogWarning($"Transaction not found for Order {orderToUpdate.OrderId}, TxHash: {order.PaymentTransactionHash}");
                            break;
                    }

                    if (updated)
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error verifying payment for Order {order.OrderId}");
                }
            }
        }

        /// <summary>
        /// 处理过期的支付订单
        /// </summary>
        private async Task HandleExpiredPayments(DateTime now)
        {
            // 使用 Select 明确指定要查询的列，避免 EF Core 尝试查询不存在的导航属性列
            var expiredOrders = await _dbContext.Orders
                .Where(o => o.PaymentMode == StorePaymentMode.Web3
                    && o.PaymentExpiresAt.HasValue
                    && o.PaymentExpiresAt.Value < now
                    && o.Status == StoreOrderStatus.PendingPayment
                    && o.PaymentStatus != StorePaymentStatus.Confirmed
                    && o.PaymentStatus != StorePaymentStatus.Cancelled)
                .Select(o => new
                {
                    o.OrderId
                })
                .ToListAsync();

            if (expiredOrders.Count == 0)
            {
                return;
            }

            _logger.LogInformation($"Found {expiredOrders.Count} expired payment orders");

            foreach (var order in expiredOrders)
            {
                // 重新加载订单以进行更新
                var orderToUpdate = await _dbContext.Orders.FindAsync(order.OrderId);
                if (orderToUpdate == null)
                {
                    continue;
                }

                orderToUpdate.PaymentStatus = StorePaymentStatus.Cancelled;
                orderToUpdate.Status = StoreOrderStatus.Cancelled;
                orderToUpdate.CancelTime = now;

                // 恢复库存
                await RestoreInventory(orderToUpdate.OrderId, now);

                // 记录支付日志
                _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
                {
                    OrderId = orderToUpdate.OrderId,
                    PaymentStatus = orderToUpdate.PaymentStatus,
                    EventType = "payment_expired",
                    Message = "支付已过期",
                    CreateTime = now
                });
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Cancelled {expiredOrders.Count} expired payment orders");
        }

        /// <summary>
        /// 更新支付确认数
        /// </summary>
        private async Task UpdatePaymentConfirmations(DateTime now)
        {
            // 使用 Select 明确指定要查询的列，避免 EF Core 尝试查询不存在的导航属性列
            var ordersToUpdate = await _dbContext.Orders
                .Where(o => o.PaymentMode == StorePaymentMode.Web3
                    && !string.IsNullOrWhiteSpace(o.PaymentTransactionHash)
                    && o.PaymentStatus == StorePaymentStatus.AwaitingOnChainConfirmation
                    && o.ChainId.HasValue)
                .Select(o => new
                {
                    o.OrderId,
                    o.ChainId,
                    o.PaymentTransactionHash,
                    o.PaymentConfirmations
                })
                .ToListAsync();

            if (ordersToUpdate.Count == 0)
            {
                return;
            }

            foreach (var order in ordersToUpdate)
            {
                try
                {
                    if (order.ChainId == null)
                    {
                        continue;
                    }

                    // 重新加载订单以进行更新
                    var orderToUpdate = await _dbContext.Orders.FindAsync(order.OrderId);
                    if (orderToUpdate == null)
                    {
                        continue;
                    }

                    var chainNetwork = (ChainNetwork)orderToUpdate.ChainId.Value;
                    var web3Provider = _web3ProviderService.GetSpenderWeb3Provider(
                        _tempCaching.GlobalConfig.ChainWalletConfigGroupId,
                        chainNetwork);

                    if (web3Provider == null)
                    {
                        continue;
                    }

                    await UpdateConfirmationCount(orderToUpdate, web3Provider, now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating confirmation count for Order {order.OrderId}");
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 更新订单的确认数
        /// </summary>
        private async Task UpdateConfirmationCount(Order order, IWeb3Provider web3Provider, DateTime now)
        {
            try
            {
                // 获取当前区块号
                var currentBlockNumber = await GetCurrentBlockNumber(web3Provider);
                if (currentBlockNumber == null)
                {
                    return;
                }

                // 获取交易所在区块号
                var transactionBlockNumber = await GetTransactionBlockNumber(web3Provider, order.PaymentTransactionHash);
                if (transactionBlockNumber == null)
                {
                    return;
                }

                // 计算确认数
                var confirmations = (int)(currentBlockNumber.Value - transactionBlockNumber.Value);
                if (confirmations < 0)
                {
                    confirmations = 0;
                }

                // 更新确认数
                if (order.PaymentConfirmations != confirmations)
                {
                    order.PaymentConfirmations = confirmations;

                    // 如果确认数达到要求（例如 12 个确认），可以标记为已确认
                    // 这里可以根据业务需求调整确认数要求
                    const int requiredConfirmations = 12;
                    if (confirmations >= requiredConfirmations && order.PaymentStatus == StorePaymentStatus.AwaitingOnChainConfirmation)
                    {
                        order.PaymentStatus = StorePaymentStatus.Confirmed;
                        order.Status = StoreOrderStatus.Paid;
                        order.PaidTime = now;
                        order.PaymentConfirmedTime = now;

                        _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
                        {
                            OrderId = order.OrderId,
                            PaymentStatus = order.PaymentStatus,
                            EventType = "payment_confirmed",
                            Message = $"交易已确认，确认数: {confirmations}",
                            CreateTime = now
                        });

                        _logger.LogInformation($"Order {order.OrderId} payment confirmed with {confirmations} confirmations");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating confirmation count for Order {order.OrderId}");
            }
        }

        /// <summary>
        /// 获取当前区块号
        /// </summary>
        private async Task<long?> GetCurrentBlockNumber(IWeb3Provider web3Provider)
        {
            try
            {
                // 通过 Web3ProviderService 获取 Web3 实例
                var chainNetwork = web3Provider.ChainNetwork;
                var web3ProviderInstance = _web3ProviderService.GetSpenderWeb3Provider(
                    _tempCaching.GlobalConfig.ChainWalletConfigGroupId,
                    chainNetwork);

                if (web3ProviderInstance is Web3Provider provider)
                {
                    // 使用反射获取私有 web3 字段
                    var web3Field = typeof(Web3Provider).GetField("web3", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (web3Field?.GetValue(provider) is Nethereum.Web3.Web3 web3)
                    {
                        var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                        return (long)blockNumber.Value;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current block number");
                return null;
            }
        }

        /// <summary>
        /// 获取交易所在区块号
        /// </summary>
        private async Task<long?> GetTransactionBlockNumber(IWeb3Provider web3Provider, string? transactionHash)
        {
            if (string.IsNullOrWhiteSpace(transactionHash))
            {
                return null;
            }

            try
            {
                // 直接使用传入的 web3Provider 查询交易状态，从中获取区块号
                // 由于 QueryTransactionStatus 已经查询了交易，我们可以通过查询交易详情来获取区块号
                var chainNetwork = web3Provider.ChainNetwork;
                var web3ProviderInstance = _web3ProviderService.GetSpenderWeb3Provider(
                    _tempCaching.GlobalConfig.ChainWalletConfigGroupId,
                    chainNetwork);

                if (web3ProviderInstance is Web3Provider provider)
                {
                    // 使用反射获取私有 web3 字段
                    var web3Field = typeof(Web3Provider).GetField("web3", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (web3Field?.GetValue(provider) is Nethereum.Web3.Web3 web3)
                    {
                        var transaction = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
                        if (transaction?.BlockNumber != null)
                        {
                            return (long)transaction.BlockNumber.Value;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting transaction block number for {transactionHash}");
                return null;
            }
        }

        /// <summary>
        /// 恢复订单库存
        /// </summary>
        private async Task RestoreInventory(long orderId, DateTime now)
        {
            var orderItems = await _dbContext.OrderItems
                .Include(i => i.Product)
                    .ThenInclude(p => p.Inventory)
                .Where(i => i.OrderId == orderId)
                .ToListAsync();

            foreach (var item in orderItems)
            {
                if (item.Product.Inventory != null)
                {
                    item.Product.Inventory.QuantityAvailable += item.Quantity;
                    item.Product.Inventory.UpdateTime = now;
                }
            }
        }
    }
}

