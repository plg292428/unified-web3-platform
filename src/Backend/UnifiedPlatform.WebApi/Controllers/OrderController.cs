using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;
using PaymentTokenInfo = UnifiedPlatform.Shared.ActionModels.Result.PaymentTokenInfo;
using UnifiedPlatform.Shared.Enums;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/orders")]
    public class OrderController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;

        public OrderController(StDbContext dbContext, ITempCaching tempCaching)
        {
            _dbContext = dbContext;
            _tempCaching = tempCaching;
        }

        /// <summary>
        /// 创建订单，支持传统支付与 Web3 支付模式。
        /// </summary>
        [HttpPost]
        public async Task<WrappedResult<StoreOrderDetailResult>> CreateAsync([FromBody] StoreOrderCreateRequest request)
        {
            var now = DateTime.UtcNow;

            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Uid == request.Uid);

            if (user is null)
            {
                return WrappedResult.Failed("用户不存在");
            }

            var cartItems = await _dbContext.ShoppingCartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Inventory)
                .Where(c => c.Uid == request.Uid)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return WrappedResult.Failed("购物车为空");
            }

            foreach (var item in cartItems)
            {
                if (item.Product.Inventory is null)
                {
                    return WrappedResult.Failed($"商品 {item.Product.Name} 缺少库存信息");
                }

                int available = item.Product.Inventory.QuantityAvailable - item.Product.Inventory.QuantityReserved + item.Quantity;
                if (item.Quantity > available)
                {
                    return WrappedResult.Failed($"商品 {item.Product.Name} 库存不足");
                }
            }

            string orderNumber = GenerateOrderNumber();
            decimal totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);

            // 验证 ChainId 是否存在（如果提供）
            int? chainId = null;
            if (request.ChainId.HasValue)
            {
                var chainExists = _tempCaching.ChainNetworkConfigs.Any(c => c.ChainId == request.ChainId.Value);
                if (chainExists)
                {
                    chainId = request.ChainId.Value;
                }
                else
                {
                    // ChainId 不存在，设为 NULL 避免外键约束错误
                    chainId = null;
                }
            }

            var order = new Order
            {
                OrderNumber = orderNumber,
                Uid = request.Uid,
                TotalAmount = totalAmount,
                Currency = cartItems.First().Product.Currency,
                Status = StoreOrderStatus.PendingPayment,
                PaymentMode = request.PaymentMode,
                PaymentMethod = request.PaymentMethod,
                PaymentProviderType = request.PaymentProviderType,
                PaymentProviderName = request.PaymentProviderName,
                PaymentWalletAddress = request.PaymentWalletAddress,
                PaymentWalletLabel = request.PaymentWalletLabel,
                ChainId = chainId, // 使用验证后的 ChainId
                PaymentTransactionHash = request.PaymentTransactionHash,
                PaymentSignaturePayload = request.PaymentSignaturePayload,
                PaymentSignatureResult = request.PaymentSignatureResult,
                CreateTime = now,
                Remark = request.Remark,
                PaymentSubmittedTime = now
            };

            if (request.PaymentMode == StorePaymentMode.Web3)
            {
                order.PaymentStatus = string.IsNullOrWhiteSpace(request.PaymentSignatureResult)
                    ? StorePaymentStatus.PendingSignature
                    : StorePaymentStatus.AwaitingOnChainConfirmation;
                
                // 设置支付过期时间（默认 30 分钟）
                order.PaymentExpiresAt = now.AddMinutes(30);
                
                // Web3 支付模式需要 ChainId，如果未提供或无效，使用第一个可用的链
                if (!order.ChainId.HasValue && _tempCaching.ChainNetworkConfigs.Any())
                {
                    order.ChainId = _tempCaching.ChainNetworkConfigs.First().ChainId;
                }
            }
            else
            {
                order.PaymentStatus = StorePaymentStatus.Confirmed;
                order.Status = StoreOrderStatus.Paid;
                order.PaidTime = now;
                order.PaymentConfirmedTime = now;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            List<OrderItem> orderItems = new();
            foreach (var cartItem in cartItems)
            {
                var inventory = cartItem.Product.Inventory!;
                inventory.QuantityReserved = Math.Max(0, inventory.QuantityReserved - cartItem.Quantity);
                inventory.QuantityAvailable = Math.Max(0, inventory.QuantityAvailable - cartItem.Quantity);
                inventory.UpdateTime = now;

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    UnitPrice = cartItem.Product.Price,
                    Quantity = cartItem.Quantity,
                    Subtotal = cartItem.Product.Price * cartItem.Quantity
                };

                orderItems.Add(orderItem);
            }

            await _dbContext.OrderItems.AddRangeAsync(orderItems);
            _dbContext.ShoppingCartItems.RemoveRange(cartItems);

            _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
            {
                OrderId = order.OrderId,
                PaymentStatus = order.PaymentStatus,
                EventType = "order_created",
                Message = request.PaymentMode == StorePaymentMode.Web3
                    ? "创建 Web3 支付订单"
                    : "创建传统支付订单",
                RawData = request.PaymentSignaturePayload,
                CreateTime = now
            });

            if (!string.IsNullOrWhiteSpace(request.PaymentWalletAddress))
            {
                string walletAddress = request.PaymentWalletAddress.Trim();
                var walletProfile = await _dbContext.WalletUserProfiles
                    .FirstOrDefaultAsync(w => w.Uid == request.Uid && w.WalletAddress == walletAddress && w.ProviderType == (request.PaymentProviderType ?? string.Empty));

                if (walletProfile is null)
                {
                    _dbContext.WalletUserProfiles.Add(new WalletUserProfile
                    {
                        Uid = request.Uid,
                        ProviderType = request.PaymentProviderType ?? "unknown",
                        ProviderName = request.PaymentProviderName,
                        WalletAddress = walletAddress,
                        AddressLabel = request.PaymentWalletLabel,
                        PreferredChainId = request.ChainId,
                        CreateTime = now,
                        LastUsedTime = now,
                        IsPrimary = false
                    });
                }
                else
                {
                    walletProfile.ProviderName ??= request.PaymentProviderName;
                    walletProfile.AddressLabel = request.PaymentWalletLabel ?? walletProfile.AddressLabel;
                    walletProfile.PreferredChainId = request.ChainId;
                    walletProfile.LastUsedTime = now;
                }
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var detail = await BuildOrderDetailAsync(order.OrderId);
            return WrappedResult.Ok(detail);
        }

        /// <summary>
        /// 订单列表。
        /// </summary>
        [HttpGet]
        public async Task<WrappedResult<StoreOrderListResult>> ListAsync([FromQuery] StoreOrderListRequest request, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            pageSize = Math.Clamp(pageSize, 1, 50);

            var query = _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Uid == request.Uid);

            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            if (request.PaymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == request.PaymentStatus.Value);
            }

            if (request.PaymentMode.HasValue)
            {
                query = query.Where(o => o.PaymentMode == request.PaymentMode.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.OrderNumber))
            {
                string orderNumber = request.OrderNumber.Trim();
                query = query.Where(o => o.OrderNumber.Contains(orderNumber));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreateTime)
                .ThenByDescending(o => o.OrderId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new StoreOrderSummaryResult
                {
                    OrderId = o.OrderId,
                    OrderNumber = o.OrderNumber,
                    TotalAmount = o.TotalAmount,
                    Currency = o.Currency,
                    Status = o.Status,
                    PaymentMode = o.PaymentMode,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    PaymentProviderType = o.PaymentProviderType,
                    PaymentProviderName = o.PaymentProviderName,
                    PaymentWalletAddress = o.PaymentWalletAddress,
                    ChainId = o.ChainId,
                    PaymentTransactionHash = o.PaymentTransactionHash,
                    CreateTime = o.CreateTime,
                    PaidTime = o.PaidTime
                })
                .ToListAsync();

            StoreOrderListResult result = new()
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 订单详情。
        /// </summary>
        [HttpGet("{orderId:long}")]
        public async Task<WrappedResult<StoreOrderDetailResult>> DetailAsync(long orderId, [FromQuery] StoreOrderListRequest request)
        {
            var order = await _dbContext.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == request.Uid);

            if (order is null)
            {
                return WrappedResult.Failed("订单不存在");
            }

            var detail = await BuildOrderDetailAsync(orderId);
            return WrappedResult.Ok(detail);
        }

        /// <summary>
        /// 准备 Web3 支付签名（生成签名原文）。
        /// </summary>
        [HttpPost("{orderId:long}/prepare-payment")]
        public async Task<WrappedResult<StoreOrderPreparePaymentResult>> PreparePaymentAsync(long orderId, [FromBody] StoreOrderPreparePaymentRequest request)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == request.Uid);

            if (order is null)
            {
                return WrappedResult.Failed("订单不存在");
            }

            if (order.PaymentMode != StorePaymentMode.Web3)
            {
                return WrappedResult.Failed("该订单不是 Web3 支付模式");
            }

            if (order.Status != StoreOrderStatus.PendingPayment)
            {
                return WrappedResult.Failed("订单状态不允许支付");
            }

            int chainId = request.ChainId ?? order.ChainId ?? throw new InvalidOperationException("链ID不能为空");
            var chainConfig = _tempCaching.ChainNetworkConfigs.FirstOrDefault(c => c.ChainId == chainId);
            if (chainConfig is null)
            {
                return WrappedResult.Failed("链配置不存在");
            }

            // 获取收款地址（从 ChainWalletConfig 中获取）
            var walletConfig = _tempCaching.ChainWalletConfigs
                .FirstOrDefault(w => w.ChainId == chainId);
            
            if (walletConfig is null)
            {
                return WrappedResult.Failed("钱包配置不存在");
            }

            string paymentAddress = walletConfig.ReceiveWalletAddress;
            if (string.IsNullOrWhiteSpace(paymentAddress))
            {
                return WrappedResult.Failed("收款地址未配置");
            }

            // 获取支持的代币列表
            var supportedTokens = _tempCaching.ChainTokenConfigs
                .Where(t => t.ChainId == chainId)
                .Select(t => new PaymentTokenInfo
                {
                    TokenId = t.TokenId,
                    TokenName = t.TokenName,
                    TokenSymbol = t.AbbrTokenName,
                    IconPath = t.IconPath,
                    Decimals = t.Decimals,
                    ContractAddress = t.ContractAddress
                })
                .ToList();

            // 获取选择的代币信息（如果指定）
            int? tokenId = request.TokenId;
            string? tokenContractAddress = null;
            string? tokenSymbol = null;

            if (tokenId.HasValue)
            {
                var tokenConfig = _tempCaching.ChainTokenConfigs
                    .FirstOrDefault(t => t.TokenId == tokenId.Value && t.ChainId == chainId);
                if (tokenConfig != null)
                {
                    tokenContractAddress = tokenConfig.ContractAddress;
                    tokenSymbol = tokenConfig.AbbrTokenName;
                }
                else
                {
                    tokenId = null; // 如果代币不存在，使用原生币
                }
            }

            // 生成支付签名原文（JSON 格式）
            var signaturePayload = new
            {
                orderId = order.OrderId,
                orderNumber = order.OrderNumber,
                amount = order.TotalAmount,
                currency = order.Currency,
                chainId = chainId,
                tokenId = tokenId,
                tokenContractAddress = tokenContractAddress,
                paymentAddress = paymentAddress,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                nonce = Guid.NewGuid().ToString("N")
            };

            string payloadJson = JsonSerializer.Serialize(signaturePayload);
            
            // 更新订单的支付签名原文和过期时间
            order.PaymentSignaturePayload = payloadJson;
            order.PaymentExpiresAt = DateTime.UtcNow.AddMinutes(30);
            order.ChainId = chainId;
            order.PaymentWalletAddress = request.PaymentWalletAddress;
            order.PaymentProviderType = request.PaymentProviderType;
            order.PaymentProviderName = request.PaymentProviderName;

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            return WrappedResult.Ok(new StoreOrderPreparePaymentResult
            {
                PaymentSignaturePayload = payloadJson,
                Amount = order.TotalAmount,
                Currency = order.Currency,
                ChainId = chainId,
                ChainName = chainConfig.NetworkName,
                TokenId = tokenId,
                TokenContractAddress = tokenContractAddress,
                TokenSymbol = tokenSymbol,
                PaymentAddress = paymentAddress,
                PaymentExpiresAt = ((DateTimeOffset)order.PaymentExpiresAt!.Value).ToUnixTimeSeconds(),
                OrderNumber = order.OrderNumber,
                SupportedTokens = supportedTokens
            });
        }

        /// <summary>
        /// 查询支付状态。
        /// </summary>
        [HttpGet("{orderId:long}/payment-status")]
        public async Task<WrappedResult<StoreOrderPaymentStatusResult>> GetPaymentStatusAsync(long orderId, [FromQuery] int uid)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == uid);

            if (order is null)
            {
                return WrappedResult.Failed("订单不存在");
            }

            bool isExpired = false;
            long remainingSeconds = 0;

            if (order.PaymentExpiresAt.HasValue)
            {
                var now = DateTime.UtcNow;
                var expiresAt = order.PaymentExpiresAt.Value;
                
                if (now >= expiresAt)
                {
                    isExpired = true;
                    
                    // 如果已过期但订单状态还是待支付，自动更新状态
                    if (order.Status == StoreOrderStatus.PendingPayment && order.PaymentStatus != StorePaymentStatus.Confirmed)
                    {
                        order.PaymentStatus = StorePaymentStatus.Cancelled;
                        order.Status = StoreOrderStatus.Cancelled;
                        order.CancelTime = now;
                        
                        _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
                        {
                            OrderId = order.OrderId,
                            PaymentStatus = order.PaymentStatus,
                            EventType = "payment_expired",
                            Message = "支付已过期",
                            CreateTime = now
                        });
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    remainingSeconds = (long)(expiresAt - now).TotalSeconds;
                }
            }

            return WrappedResult.Ok(new StoreOrderPaymentStatusResult
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.Status,
                PaymentTransactionHash = order.PaymentTransactionHash,
                PaymentConfirmations = order.PaymentConfirmations,
                PaymentSubmittedTime = order.PaymentSubmittedTime,
                PaymentConfirmedTime = order.PaymentConfirmedTime,
                PaymentExpiresAt = order.PaymentExpiresAt,
                PaymentFailureReason = order.PaymentFailureReason,
                IsExpired = isExpired,
                RemainingSeconds = remainingSeconds
            });
        }

        /// <summary>
        /// 取消订单。
        /// </summary>
        [HttpPost("{orderId:long}/cancel")]
        public async Task<WrappedResult<bool>> CancelOrderAsync(long orderId, [FromBody] StoreOrderCancelRequest request)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == request.Uid);

            if (order is null)
            {
                return WrappedResult.Failed("订单不存在");
            }

            // 只有待支付或支付中的订单可以取消
            if (order.Status != StoreOrderStatus.PendingPayment)
            {
                return WrappedResult.Failed("订单状态不允许取消");
            }

            var now = DateTime.UtcNow;
            order.Status = StoreOrderStatus.Cancelled;
            order.CancelTime = now;
            order.PaymentStatus = StorePaymentStatus.Cancelled;

            // 恢复库存
            var orderItems = await _dbContext.OrderItems
                .Include(i => i.Product)
                    .ThenInclude(p => p.Inventory)
                .Where(i => i.OrderId == orderId)
                .ToListAsync();

            foreach (var item in orderItems)
            {
                if (item.Product.Inventory is not null)
                {
                    item.Product.Inventory.QuantityAvailable += item.Quantity;
                    item.Product.Inventory.UpdateTime = now;
                }
            }

            _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
            {
                OrderId = order.OrderId,
                PaymentStatus = order.PaymentStatus,
                EventType = "order_cancelled",
                Message = request.Reason ?? "用户取消订单",
                CreateTime = now
            });

            await _dbContext.SaveChangesAsync();

            return WrappedResult.Ok(true);
        }

        /// <summary>
        /// Web3 支付确认回调/轮询接口。
        /// </summary>
        [HttpPost("{orderId:long}/web3/confirm")]
        public async Task<WrappedResult<bool>> ConfirmWeb3PaymentAsync(long orderId, [FromBody] StoreOrderWeb3ConfirmRequest request)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == request.Uid);

            if (order is null)
            {
                return WrappedResult.Failed("订单不存在");
            }

            if (order.PaymentMode != StorePaymentMode.Web3)
            {
                return WrappedResult.Failed("该订单不是 Web3 支付模式");
            }

            if (!string.IsNullOrWhiteSpace(request.PaymentTransactionHash))
            {
                order.PaymentTransactionHash = request.PaymentTransactionHash;
            }

            order.PaymentStatus = request.PaymentStatus;
            order.PaymentConfirmations = request.PaymentConfirmations;
            order.PaymentSignatureResult = request.PaymentSignatureResult ?? order.PaymentSignatureResult;
            order.PaymentFailureReason = request.PaymentFailureReason ?? order.PaymentFailureReason;

            if (request.PaymentConfirmedTime.HasValue)
            {
                order.PaymentConfirmedTime = request.PaymentConfirmedTime;
            }

            if (order.PaymentStatus == StorePaymentStatus.Confirmed)
            {
                order.Status = StoreOrderStatus.Paid;
                order.PaidTime ??= request.PaymentConfirmedTime ?? DateTime.UtcNow;
                order.PaymentConfirmedTime ??= request.PaymentConfirmedTime ?? DateTime.UtcNow;
            }
            else if (order.PaymentStatus == StorePaymentStatus.Cancelled)
            {
                order.Status = StoreOrderStatus.Cancelled;
                order.CancelTime ??= DateTime.UtcNow;
            }
            else if (order.PaymentStatus == StorePaymentStatus.Failed)
            {
                order.Status = StoreOrderStatus.PendingPayment;
            }

            _dbContext.OrderPaymentLogs.Add(new OrderPaymentLog
            {
                OrderId = order.OrderId,
                PaymentStatus = order.PaymentStatus,
                EventType = "web3_confirmation",
                Message = request.PaymentFailureReason,
                RawData = request.RawData,
                CreateTime = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            return WrappedResult.Ok(true);
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }

        private async Task<StoreOrderDetailResult> BuildOrderDetailAsync(long orderId)
        {
            var order = await _dbContext.Orders
                .AsNoTracking()
                .FirstAsync(o => o.OrderId == orderId);

            var items = await _dbContext.OrderItems
                .AsNoTracking()
                .Where(i => i.OrderId == orderId)
                .Select(i => new StoreOrderItemResult
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                })
                .ToListAsync();

            var paymentLogs = await _dbContext.OrderPaymentLogs
                .AsNoTracking()
                .Where(l => l.OrderId == orderId)
                .OrderByDescending(l => l.CreateTime)
                .Select(l => new StoreOrderPaymentLogResult
                {
                    OrderPaymentLogId = l.OrderPaymentLogId,
                    PaymentStatus = l.PaymentStatus,
                    EventType = l.EventType,
                    Message = l.Message,
                    RawData = l.RawData,
                    CreateTime = l.CreateTime
                })
                .ToListAsync();

            return new StoreOrderDetailResult
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Uid = order.Uid,
                TotalAmount = order.TotalAmount,
                Currency = order.Currency,
                Status = order.Status,
                PaymentMode = order.PaymentMode,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentProviderType = order.PaymentProviderType,
                PaymentProviderName = order.PaymentProviderName,
                PaymentWalletAddress = order.PaymentWalletAddress,
                PaymentWalletLabel = order.PaymentWalletLabel,
                ChainId = order.ChainId,
                PaymentTransactionHash = order.PaymentTransactionHash,
                PaymentConfirmations = order.PaymentConfirmations,
                PaymentSubmittedTime = order.PaymentSubmittedTime,
                PaymentConfirmedTime = order.PaymentConfirmedTime,
                PaymentSignaturePayload = order.PaymentSignaturePayload,
                PaymentSignatureResult = order.PaymentSignatureResult,
                PaymentFailureReason = order.PaymentFailureReason,
                CreateTime = order.CreateTime,
                PaidTime = order.PaidTime,
                CancelTime = order.CancelTime,
                CompleteTime = order.CompleteTime,
                Remark = order.Remark,
                Items = items,
                PaymentLogs = paymentLogs
            };
        }
    }
}

