using System.ComponentModel.DataAnnotations;namespace UnifiedPlatform.Shared.ActionModels.Request;public class StoreCartUpdateQuantityRequest{    [Range(1, int.MaxValue)]    public int Uid { get; set; }    [Range(1, long.MaxValue)]    public long CartItemId { get; set; }    [Range(0, 9999)]    public int Quantity { get; set; }}

