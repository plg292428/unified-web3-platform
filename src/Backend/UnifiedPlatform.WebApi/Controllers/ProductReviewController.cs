using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;

namespace UnifiedPlatform.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/store/products/{productId:long}/reviews")]
    public class ProductReviewController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;

        public ProductReviewController(StDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取商品评价列表
        /// </summary>
        [HttpGet]
        public async Task<WrappedResult<StoreProductReviewListResult>> ListAsync(
            long productId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var query = _dbContext.ProductReviews
                .AsNoTracking()
                .Include(r => r.UidNavigation)
                .Where(r => r.ProductId == productId && r.IsVisible && r.IsApproved);

            int totalCount = await query.CountAsync();

            var items = await query
                .Include(r => r.ReviewImages.OrderBy(img => img.SortOrder))
                .Include(r => r.Replies.OrderBy(reply => reply.CreateTime))
                .OrderByDescending(r => r.CreateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new StoreProductReviewResult
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    Uid = r.Uid,
                    UserWalletAddress = r.UidNavigation.WalletAddress,
                    OrderId = r.OrderId,
                    Rating = r.Rating,
                    Content = r.Content,
                    IsApproved = r.IsApproved,
                    IsVisible = r.IsVisible,
                    Images = r.ReviewImages.Select(img => new ReviewImageResult
                    {
                        ImageId = img.ImageId,
                        ImageUrl = img.ImageUrl,
                        SortOrder = img.SortOrder
                    }).ToList(),
                    Replies = r.Replies.Select(reply => new ReviewReplyResult
                    {
                        ReplyId = reply.ReplyId,
                        ManagerUid = reply.ManagerUid,
                        Content = reply.Content,
                        CreateTime = reply.CreateTime
                    }).ToList(),
                    HelpfulCount = r.Votes.Count(v => v.IsHelpful),
                    NotHelpfulCount = r.Votes.Count(v => !v.IsHelpful),
                    CreateTime = r.CreateTime,
                    UpdateTime = r.UpdateTime
                })
                .ToListAsync();

            // 计算平均评分和评分分布
            var allReviews = await _dbContext.ProductReviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId && r.IsVisible && r.IsApproved)
                .ToListAsync();

            double averageRating = allReviews.Count > 0
                ? allReviews.Average(r => r.Rating)
                : 0;

            var ratingDistribution = allReviews
                .GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count());

            var result = new StoreProductReviewListResult
            {
                Items = items,
                TotalCount = totalCount,
                AverageRating = Math.Round(averageRating, 1),
                RatingDistribution = ratingDistribution
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 创建商品评价
        /// </summary>
        [HttpPost]
        public async Task<WrappedResult<StoreProductReviewResult>> CreateAsync(
            long productId,
            [FromBody] StoreProductReviewCreateRequest request)
        {
            if (request.ProductId != productId)
            {
                return WrappedResult.Failed("商品ID不匹配");
            }

            // 验证用户是否存在
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Uid == request.Uid);

            if (user is null)
            {
                return WrappedResult.Failed("用户不存在");
            }

            // 验证商品是否存在
            var product = await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            // 如果提供了订单ID，验证订单是否属于该用户且已支付
            if (request.OrderId.HasValue)
            {
                var order = await _dbContext.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.OrderId == request.OrderId.Value && o.Uid == request.Uid);

                if (order is null)
                {
                    return WrappedResult.Failed("订单不存在或不属于该用户");
                }

                if (order.Status != Shared.Enums.StoreOrderStatus.Paid && order.Status != Shared.Enums.StoreOrderStatus.Completed)
                {
                    return WrappedResult.Failed("只有已支付的订单才能评价");
                }

                // 检查是否已经评价过
                var existingReview = await _dbContext.ProductReviews
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.OrderId == request.OrderId.Value && r.ProductId == productId);

                if (existingReview != null)
                {
                    return WrappedResult.Failed("该订单已评价过");
                }
            }

            var now = DateTime.UtcNow;
            var review = new ProductReview
            {
                ProductId = productId,
                Uid = request.Uid,
                OrderId = request.OrderId,
                Rating = request.Rating,
                Content = request.Content,
                IsApproved = false, // 默认需要审核
                IsVisible = false, // 审核通过后才显示
                CreateTime = now,
                UpdateTime = now
            };

            _dbContext.ProductReviews.Add(review);
            await _dbContext.SaveChangesAsync();

            // 添加评价图片
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                var reviewImages = request.ImageUrls.Select((url, index) => new ProductReviewImage
                {
                    ReviewId = review.ReviewId,
                    ImageUrl = url,
                    SortOrder = index,
                    CreateTime = now
                }).ToList();
                await _dbContext.ProductReviewImages.AddRangeAsync(reviewImages);
                await _dbContext.SaveChangesAsync();
            }

            var result = new StoreProductReviewResult
            {
                ReviewId = review.ReviewId,
                ProductId = review.ProductId,
                Uid = review.Uid,
                UserWalletAddress = user.WalletAddress,
                OrderId = review.OrderId,
                Rating = review.Rating,
                Content = review.Content,
                IsApproved = review.IsApproved,
                IsVisible = review.IsVisible,
                Images = review.ReviewImages.Select(img => new ReviewImageResult
                {
                    ImageId = img.ImageId,
                    ImageUrl = img.ImageUrl,
                    SortOrder = img.SortOrder
                }).ToList(),
                Replies = new List<ReviewReplyResult>(),
                HelpfulCount = 0,
                NotHelpfulCount = 0,
                CreateTime = review.CreateTime,
                UpdateTime = review.UpdateTime
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取用户自己的评价列表
        /// </summary>
        [HttpGet("my")]
        public async Task<WrappedResult<StoreProductReviewListResult>> GetMyReviews(
            [FromQuery] int uid,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var query = _dbContext.ProductReviews
                .AsNoTracking()
                .Include(r => r.Product)
                .Where(r => r.Uid == uid);

            int totalCount = await query.CountAsync();

            var items = await query
                .Include(r => r.ReviewImages)
                .Include(r => r.Replies)
                .OrderByDescending(r => r.CreateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new StoreProductReviewResult
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    Uid = r.Uid,
                    UserWalletAddress = r.UidNavigation.WalletAddress,
                    OrderId = r.OrderId,
                    Rating = r.Rating,
                    Content = r.Content,
                    IsApproved = r.IsApproved,
                    IsVisible = r.IsVisible,
                    Images = r.ReviewImages.Select(img => new ReviewImageResult
                    {
                        ImageId = img.ImageId,
                        ImageUrl = img.ImageUrl,
                        SortOrder = img.SortOrder
                    }).ToList(),
                    Replies = r.Replies.Select(reply => new ReviewReplyResult
                    {
                        ReplyId = reply.ReplyId,
                        ManagerUid = reply.ManagerUid,
                        Content = reply.Content,
                        CreateTime = reply.CreateTime
                    }).ToList(),
                    HelpfulCount = r.Votes.Count(v => v.IsHelpful),
                    NotHelpfulCount = r.Votes.Count(v => !v.IsHelpful),
                    CreateTime = r.CreateTime,
                    UpdateTime = r.UpdateTime
                })
                .ToListAsync();

            var result = new StoreProductReviewListResult
            {
                Items = items,
                TotalCount = totalCount,
                AverageRating = items.Any() ? items.Average(r => r.Rating) : 0,
                RatingDistribution = items.GroupBy(r => r.Rating).ToDictionary(g => g.Key, g => g.Count())
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 评价投票（有用/无用）
        /// </summary>
        [HttpPost("{reviewId:long}/vote")]
        public async Task<WrappedResult> VoteReview(long reviewId, [FromBody] ReviewVoteRequest request)
        {
            var review = await _dbContext.ProductReviews
                .AsTracking()
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review is null)
            {
                return WrappedResult.Failed("评价不存在");
            }

            // 检查是否已投票
            var existingVote = await _dbContext.ProductReviewVotes
                .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.Uid == request.Uid);

            if (existingVote != null)
            {
                // 更新投票
                existingVote.IsHelpful = request.IsHelpful;
                existingVote.CreateTime = DateTime.UtcNow;
            }
            else
            {
                // 创建新投票
                var vote = new ProductReviewVote
                {
                    ReviewId = reviewId,
                    Uid = request.Uid,
                    IsHelpful = request.IsHelpful,
                    CreateTime = DateTime.UtcNow
                };
                await _dbContext.ProductReviewVotes.AddAsync(vote);
            }

            await _dbContext.SaveChangesAsync();
            return WrappedResult.Ok();
        }
    }
}

