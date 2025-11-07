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
}
