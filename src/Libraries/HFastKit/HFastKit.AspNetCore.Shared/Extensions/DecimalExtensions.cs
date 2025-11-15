namespace HFastKit.AspNetCore.Shared.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal FixedToZero(this decimal value, int decimals = 4) => decimal.Round(value, decimals, MidpointRounding.ToZero);
    }
}

