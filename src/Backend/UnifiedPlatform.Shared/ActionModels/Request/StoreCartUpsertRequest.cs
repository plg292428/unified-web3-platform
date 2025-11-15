using System.ComponentModel.DataAnnotations;namespace UnifiedPlatform.Shared.ActionModels.Request;public class StoreCartUpsertRequest{    [Range(1, int.MaxValue)]    public int Uid { get; set; }    [Range(1, long.MaxValue)]    public long ProductId { get; set; }    [Range(1, 9999)]    public int Quantity { get; set; }}

