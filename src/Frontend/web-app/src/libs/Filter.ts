export default class Filter {
  private static tokenFormatter: Intl.NumberFormat = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: "USD" ,
    minimumFractionDigits: 4,
    maximumFractionDigits: 4
  })

  private static tokenFormatterAsTwoDecimalPlaces: Intl.NumberFormat = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: "USD" ,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  })

  private static intFormatter: Intl.NumberFormat = new Intl.NumberFormat('en-US', {
    style: 'decimal',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  })
  
  public static formatToken(value: number | bigint) {
    return Filter.tokenFormatter.format(value) ?? null
  }

  public static formatTokenAsTwoDecimalPlaces(value: number | bigint) {
    return Filter.tokenFormatterAsTwoDecimalPlaces.format(value) ?? null
  }

  public static formatInt(value: number) {
    if(isNaN(value))
    {
      return null;
    }
    return Filter.intFormatter.format(value) ?? null
  }

  public static formatPercent(value: number, decimals: number = 2) {
    if(isNaN(value))
    {
      return null;
    }
    return `${(value * 100).toFixed(decimals)}%` ?? null
  }

  public static formatShotDateTime(value: Date) {
   return value.toShortTime();
  }

  /**
   * Format price without trailing zeros
   * Example: 45.000000 -> 45, 15.000000 -> 15, 6.800000 -> 6.8
   */
  public static formatPrice(value: number | bigint | string): string {
    if (value === null || value === undefined) {
      return '0'
    }
    
    // Convert to number
    const numValue = typeof value === 'string' ? parseFloat(value) : Number(value)
    
    if (isNaN(numValue)) {
      return '0'
    }
    
    // Remove trailing zeros using parseFloat and toString
    // This will automatically remove trailing zeros
    return parseFloat(numValue.toString()).toString()
  }
}
