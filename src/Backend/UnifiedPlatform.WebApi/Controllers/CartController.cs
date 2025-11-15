namespace UnifiedPlatform.WebApi.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;

[AllowAnonymous]
[Route("api/cart")]
public class CartController : ApiControllerBase
{
    private readonly StDbContext _dbContext;

    public CartController(StDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 获取购物车列表
    /// </summary>
    [HttpGet]
    public async Task<WrappedResult<StoreCartListResult>> ListAsync([FromQuery] StoreCartListRequest request)
    {
        var items = await QueryCartItemsAsync(request.Uid);
        return WrappedResult.Ok(new StoreCartListResult { Items = items });
    }

    /// <summary>
    /// 新增或更新购物车项
    /// </summary>
    [HttpPost("items")]
    public async Task<WrappedResult<StoreCartListResult>> AddItemAsync([FromBody] StoreCartUpsertRequest request)
    {
        var now = DateTime.UtcNow;

        var product = await _dbContext.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.ProductId == request.ProductId && p.IsPublished);

        if (product is null)
        {
            return WrappedResult.Failed("Product not found or not published");
        }

        if (product.Inventory is null)
        {
            return WrappedResult.Failed("Product inventory information not found");
        }

        if (!await _dbContext.Users.AnyAsync(u => u.Uid == request.Uid))
        {
            return WrappedResult.Failed("User not found. Please login first to create user record");
        }

        var cartItem = await _dbContext.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.Uid == request.Uid && c.ProductId == request.ProductId);

        int originalQuantity = cartItem?.Quantity ?? 0;
        int deltaQuantity = request.Quantity;
        int newQuantity = originalQuantity + deltaQuantity;

        if (deltaQuantity <= 0)
        {
            return WrappedResult.Failed("Quantity must be greater than 0");
        }

        int availableDelta = product.Inventory.QuantityAvailable - product.Inventory.QuantityReserved;
        if (deltaQuantity > availableDelta)
        {
            return WrappedResult.Failed("Insufficient inventory");
        }

        if (cartItem is null)
        {
            cartItem = new ShoppingCartItem
            {
                Uid = request.Uid,
                ProductId = request.ProductId,
                Quantity = newQuantity,
                CreateTime = now,
                UpdateTime = now
            };

            await _dbContext.ShoppingCartItems.AddAsync(cartItem);
        }
        else
        {
            cartItem.Quantity = newQuantity;
            cartItem.UpdateTime = now;
        }

        product.Inventory.QuantityReserved += deltaQuantity;
        if (product.Inventory.QuantityReserved < 0)
        {
            product.Inventory.QuantityReserved = 0;
        }

        product.Inventory.UpdateTime = now;

        await _dbContext.SaveChangesAsync();

        var items = await QueryCartItemsAsync(request.Uid);
        return WrappedResult.Ok(new StoreCartListResult { Items = items });
    }

    /// <summary>
    /// 更新购物车项数量
    /// </summary>
    [HttpPut("items/{cartItemId:long}")]
    public async Task<WrappedResult<StoreCartListResult>> UpdateQuantityAsync(long cartItemId, [FromBody] StoreCartUpdateQuantityRequest request)
    {
        if (request.CartItemId != cartItemId)
        {
            return WrappedResult.Failed("Parameter mismatch");
        }

        var now = DateTime.UtcNow;

        var cartItem = await _dbContext.ShoppingCartItems
            .Include(c => c.Product)
            .ThenInclude(p => p.Inventory)
            .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.Uid == request.Uid);

        if (cartItem is null)
        {
            return WrappedResult.Failed("Cart item not found");
        }

        var inventory = cartItem.Product.Inventory;
        if (inventory is null)
        {
            return WrappedResult.Failed("Product inventory information not found");
        }

        int originalQuantity = cartItem.Quantity;
        int delta = request.Quantity - originalQuantity;

        if (request.Quantity <= 0)
        {
            inventory.QuantityReserved -= originalQuantity;
            if (inventory.QuantityReserved < 0)
            {
                inventory.QuantityReserved = 0;
            }

            _dbContext.ShoppingCartItems.Remove(cartItem);
        }
        else
        {
            int availableDelta = inventory.QuantityAvailable - inventory.QuantityReserved;
            if (delta > 0 && delta > availableDelta)
            {
                return WrappedResult.Failed("Insufficient inventory");
            }

            cartItem.Quantity = request.Quantity;
            cartItem.UpdateTime = now;

            inventory.QuantityReserved += delta;
            if (inventory.QuantityReserved < 0)
            {
                inventory.QuantityReserved = 0;
            }
        }

        inventory.UpdateTime = now;

        await _dbContext.SaveChangesAsync();

        var items = await QueryCartItemsAsync(request.Uid);
        return WrappedResult.Ok(new StoreCartListResult { Items = items });
    }

    /// <summary>
    /// 删除购物车项
    /// </summary>
    [HttpDelete("items/{cartItemId:long}")]
    public async Task<WrappedResult<StoreCartListResult>> RemoveItemAsync(long cartItemId, [FromQuery] StoreCartListRequest request)
    {
        var cartItem = await _dbContext.ShoppingCartItems
            .Include(c => c.Product)
            .ThenInclude(p => p.Inventory)
            .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.Uid == request.Uid);

        if (cartItem is null)
        {
            return WrappedResult.Failed("Cart item not found");
        }

        var inventory = cartItem.Product.Inventory;
        if (inventory != null)
        {
            inventory.QuantityReserved -= cartItem.Quantity;
            if (inventory.QuantityReserved < 0)
            {
                inventory.QuantityReserved = 0;
            }

            inventory.UpdateTime = DateTime.UtcNow;
        }

        _dbContext.ShoppingCartItems.Remove(cartItem);
        await _dbContext.SaveChangesAsync();

        var items = await QueryCartItemsAsync(request.Uid);
        return WrappedResult.Ok(new StoreCartListResult { Items = items });
    }

    private async Task<IReadOnlyList<StoreCartItemResult>> QueryCartItemsAsync(int uid)
    {
        return await _dbContext.ShoppingCartItems
            .AsNoTracking()
            .Where(c => c.Uid == uid)
            .Include(c => c.Product)
            .ThenInclude(p => p.Inventory)
            .OrderByDescending(c => c.UpdateTime)
            .Select(c => new StoreCartItemResult
            {
                CartItemId = c.CartItemId,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                Subtitle = c.Product.Subtitle,
                ThumbnailUrl = c.Product.ThumbnailUrl,
                UnitPrice = c.Product.Price,
                Currency = c.Product.Currency,
                Quantity = c.Quantity,
                Subtotal = c.Product.Price * c.Quantity,
                InventoryAvailable = c.Product.Inventory != null
                    ? c.Product.Inventory.QuantityAvailable - c.Product.Inventory.QuantityReserved
                    : 0,
                UpdateTime = c.UpdateTime
            })
            .ToListAsync();
    }
}

