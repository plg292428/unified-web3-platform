using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;
using UnifiedPlatform.Shared.Enums;
using UnifiedPlatform.WebApi.Constants;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 物流管理控制器
    /// </summary>
    [Consumes("application/json")]
    [Route("api/shipping")]
    [ApiController]
    public class ShippingController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;

        public ShippingController(StDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 发货（管理端）
        /// </summary>
        [HttpPost("ship")]
        [Authorize(Policy = AuthorizationPolicyName.Manager)]
        public async Task<WrappedResult<ShippingResult>> ShipOrder([FromBody] ShippingCreateRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var order = await _dbContext.Orders
                .AsTracking()
                .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

            if (order is null)
            {
                return WrappedResult.Failed("订单不存在");
            }

            if (order.Status != StoreOrderStatus.Paid)
            {
                return WrappedResult.Failed("订单状态不允许发货");
            }

            // 检查是否已有物流信息
            var existingShipping = await _dbContext.OrderShippings
                .FirstOrDefaultAsync(s => s.OrderId == request.OrderId);

            if (existingShipping != null)
            {
                return WrappedResult.Failed("订单已发货");
            }

            var now = DateTime.UtcNow;

            var shipping = new OrderShipping
            {
                OrderId = request.OrderId,
                ShippingCompany = request.ShippingCompany,
                ShippingCompanyCode = request.ShippingCompanyCode,
                TrackingNumber = request.TrackingNumber,
                RecipientName = request.RecipientName,
                RecipientPhone = request.RecipientPhone,
                RecipientAddress = request.RecipientAddress,
                ShippedTime = now,
                EstimatedDeliveryTime = request.EstimatedDeliveryTime,
                Status = "shipped",
                StatusDescription = "已发货",
                ShippingFee = request.ShippingFee ?? 0,
                Remark = request.Remark,
                CreateTime = now,
                UpdateTime = now
            };

            await _dbContext.OrderShippings.AddAsync(shipping);

            // 更新订单状态
            order.Status = StoreOrderStatus.Shipped;

            await _dbContext.SaveChangesAsync();

            var result = new ShippingResult
            {
                ShippingId = shipping.ShippingId,
                OrderId = shipping.OrderId,
                ShippingCompany = shipping.ShippingCompany,
                TrackingNumber = shipping.TrackingNumber,
                Status = shipping.Status,
                ShippedTime = shipping.ShippedTime,
                EstimatedDeliveryTime = shipping.EstimatedDeliveryTime
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 查询物流信息
        /// </summary>
        [HttpGet("track/{orderId:long}")]
        [AllowAnonymous]
        public async Task<WrappedResult<ShippingTrackingResult>> TrackShipping(long orderId, [FromQuery] int? uid = null)
        {
            var query = _dbContext.OrderShippings
                .AsNoTracking()
                .Where(s => s.OrderId == orderId);

            // 如果是用户查询，验证订单归属
            if (uid.HasValue)
            {
                var order = await _dbContext.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.Uid == uid.Value);
                if (order is null)
                {
                    return WrappedResult.Failed("订单不存在");
                }
            }

            var shipping = await query
                .Include(s => s.Order)
                .FirstOrDefaultAsync();

            if (shipping is null)
            {
                return WrappedResult.Failed("物流信息不存在");
            }

            // 获取跟踪记录
            var trackingLogs = await _dbContext.ShippingTrackingLogs
                .AsNoTracking()
                .Where(t => t.ShippingId == shipping.ShippingId)
                .OrderByDescending(t => t.Timestamp)
                .Select(t => new ShippingTrackingLogResult
                {
                    Status = t.Status,
                    Description = t.Description,
                    Location = t.Location,
                    Timestamp = t.Timestamp
                })
                .ToListAsync();

            var result = new ShippingTrackingResult
            {
                ShippingId = shipping.ShippingId,
                OrderId = shipping.OrderId,
                ShippingCompany = shipping.ShippingCompany,
                TrackingNumber = shipping.TrackingNumber,
                Status = shipping.Status,
                StatusDescription = shipping.StatusDescription,
                RecipientName = shipping.RecipientName,
                RecipientPhone = shipping.RecipientPhone,
                RecipientAddress = shipping.RecipientAddress,
                ShippedTime = shipping.ShippedTime,
                EstimatedDeliveryTime = shipping.EstimatedDeliveryTime,
                DeliveredTime = shipping.DeliveredTime,
                TrackingLogs = trackingLogs
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 更新物流状态（管理端或自动同步）
        /// </summary>
        [HttpPost("update-status")]
        [Authorize(Policy = AuthorizationPolicyName.Manager)]
        public async Task<WrappedResult> UpdateShippingStatus([FromBody] ShippingStatusUpdateRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var shipping = await _dbContext.OrderShippings
                .AsTracking()
                .FirstOrDefaultAsync(s => s.ShippingId == request.ShippingId);

            if (shipping is null)
            {
                return WrappedResult.Failed("物流信息不存在");
            }

            var now = DateTime.UtcNow;

            shipping.Status = request.Status;
            shipping.StatusDescription = request.StatusDescription;
            shipping.UpdateTime = now;

            if (request.Status == "delivered" && !shipping.DeliveredTime.HasValue)
            {
                shipping.DeliveredTime = now;
                
                // 更新订单状态为已完成
                var order = await _dbContext.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == shipping.OrderId);
                if (order != null && order.Status == StoreOrderStatus.Shipped)
                {
                    order.Status = StoreOrderStatus.Completed;
                    order.CompleteTime = now;
                }
            }

            // 添加跟踪记录
            if (!string.IsNullOrWhiteSpace(request.StatusDescription))
            {
                _dbContext.ShippingTrackingLogs.Add(new ShippingTrackingLog
                {
                    ShippingId = shipping.ShippingId,
                    Status = request.Status,
                    Description = request.StatusDescription,
                    Location = request.Location,
                    Timestamp = request.Timestamp ?? now,
                    CreateTime = now
                });
            }

            await _dbContext.SaveChangesAsync();
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 获取物流公司列表
        /// </summary>
        [HttpGet("companies")]
        [AllowAnonymous]
        public WrappedResult<List<ShippingCompanyResult>> GetShippingCompanies()
        {
            var companies = new List<ShippingCompanyResult>
            {
                new() { Code = "sf", Name = "顺丰速运", ApiSupported = true },
                new() { Code = "yunda", Name = "韵达快递", ApiSupported = true },
                new() { Code = "yuantong", Name = "圆通速递", ApiSupported = true },
                new() { Code = "shentong", Name = "申通快递", ApiSupported = true },
                new() { Code = "zhongtong", Name = "中通快递", ApiSupported = true },
                new() { Code = "ems", Name = "EMS", ApiSupported = true },
                new() { Code = "usps", Name = "USPS", ApiSupported = true },
                new() { Code = "fedex", Name = "FedEx", ApiSupported = true },
                new() { Code = "dhl", Name = "DHL", ApiSupported = true },
                new() { Code = "other", Name = "其他", ApiSupported = false }
            };

            return WrappedResult.Ok(companies);
        }
    }
}

