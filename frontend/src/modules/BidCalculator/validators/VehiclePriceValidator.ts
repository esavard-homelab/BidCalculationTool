/**
 * Validator for vehicle price input fields.
 * Ensures vehicle prices are within acceptable business rules.
 */
export class VehiclePriceValidator {
  /**
   * Validates a vehicle price against business rules.
   *
   * @param price - The vehicle price to validate
   * @returns Error message string if validation fails, null if valid
   *
   * @example
   * ```typescript
   * VehiclePriceValidator.validate(1000) // Returns null (valid)
   * VehiclePriceValidator.validate(0) // Returns "Vehicle price must be greater than 0"
   * VehiclePriceValidator.validate(20000000) // Returns "Vehicle price cannot exceed $10,000,000"
   * ```
   */
  static validate(price: number): string | null {
    if (price <= 0) {
      return 'Vehicle price must be greater than 0'
    }
    if (price > 10000000) {
      return 'Vehicle price cannot exceed $10,000,000'
    }
    return null
  }
}
