using System.Collections.Generic;using System.Linq;namespace UnifiedPlatform.Shared.ActionModels.Result;public class StoreCartListResult{    public IReadOnlyList<StoreCartItemResult> Items { get; set; } = new List<StoreCartItemResult>();    public decimal TotalAmount => Items.Sum(i => i.Subtotal);    public int TotalQuantity => Items.Sum(i => i.Quantity);}

