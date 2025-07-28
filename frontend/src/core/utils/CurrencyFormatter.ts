/**
 * Utility class for formatting currency values.
 * Provides consistent currency formatting across the application.
 */
export class CurrencyFormatter {
  /**
   * Formats a numeric amount as USD currency.
   *
   * @param amount - The numeric value to format
   * @returns The formatted currency string (e.g., "$1,234.56")
   *
   * @example
   * ```typescript
   * CurrencyFormatter.formatUSD(1234.56) // Returns "$1,234.56"
   * CurrencyFormatter.formatUSD(0) // Returns "$0.00"
   * ```
   */
  static formatUSD(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 2,
    }).format(amount)
  }
}
