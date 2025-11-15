using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;
using UnifiedPlatform.WebApi.Constants;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 商品评价管理控制器（管理端）
    /// </summary>
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.Manager)]
    [ApiController]
    public class ProductReviewManagementController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;

        public ProductReviewManagementController(StDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 审核评价（通过/拒绝）
        /// </summary>
        [HttpPost("{reviewId:long}/approve")]
        public async Task<WrappedResult> ApproveReview(long reviewId, [FromBody] ReviewApproveRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var review = await _dbContext.ProductReviews
                .AsTracking()
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review is null)
            {
                return WrappedResult.Failed("评价不存在");
            }

            review.IsApproved = request.Approved;
            review.IsVisible = request.Approved; // 审核通过后才显示
            review.UpdateTime = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 删除评价
        /// </summary>
        [HttpDelete("{reviewId:long}")]
        public async Task<WrappedResult> DeleteReview(long reviewId)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var review = await _dbContext.ProductReviews
                .Include(r => r.ReviewImages)
                .Include(r => r.Replies)
                .Include(r => r.Votes)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review is null)
            {
                return WrappedResult.Failed("评价不存在");
            }

            // 删除关联数据
            _dbContext.ProductReviewImages.RemoveRange(review.ReviewImages);
            _dbContext.ProductReviewReplies.RemoveRange(review.Replies);
            _dbContext.ProductReviewVotes.RemoveRange(review.Votes);
            _dbContext.ProductReviews.Remove(review);

            await _dbContext.SaveChangesAsync();
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 商家回复评价
        /// </summary>
        [HttpPost("{reviewId:long}/reply")]
        public async Task<WrappedResult<ReviewReplyResult>> ReplyReview(long reviewId, [FromBody] ReviewReplyRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var review = await _dbContext.ProductReviews
                .AsTracking()
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review is null)
            {
                return WrappedResult.Failed("评价不存在");
            }

            var now = DateTime.UtcNow;
            var reply = new ProductReviewReply
            {
                ReviewId = reviewId,
                ManagerUid = Manager.Uid,
                Content = request.Content,
                CreateTime = now,
                UpdateTime = now
            };

            await _dbContext.ProductReviewReplies.AddAsync(reply);
            await _dbContext.SaveChangesAsync();

            var result = new ReviewReplyResult
            {
                ReplyId = reply.ReplyId,
                ManagerUid = reply.ManagerUid,
                Content = reply.Content,
                CreateTime = reply.CreateTime
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 查询待审核评价列表
        /// </summary>
        [HttpGet("pending")]
        public async Task<WrappedResult<List<StoreProductReviewResult>>> GetPendingReviews([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var reviews = await _dbContext.ProductReviews
                .AsNoTracking()
                .Include(r => r.UidNavigation)
                .Include(r => r.Product)
                .Include(r => r.ReviewImages)
                .Include(r => r.Replies)
                .Where(r => !r.IsApproved)
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

            return WrappedResult.Ok(reviews);
        }
    }
}

