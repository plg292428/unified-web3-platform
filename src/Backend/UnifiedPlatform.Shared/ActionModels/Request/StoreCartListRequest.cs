using System.ComponentModel.DataAnnotations;namespace UnifiedPlatform.Shared.ActionModels.Request;public class StoreCartListRequest{    [Range(1, int.MaxValue)]    public int Uid { get; set; }}

