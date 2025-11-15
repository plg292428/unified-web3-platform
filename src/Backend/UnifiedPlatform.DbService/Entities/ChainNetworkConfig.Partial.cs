namespace UnifiedPlatform.DbService.Entities;public partial class ChainNetworkConfig{    public virtual ICollection<Product> Products { get; set; } = new List<Product>();    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();}

