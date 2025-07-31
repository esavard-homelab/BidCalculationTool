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
   * VehiclePriceValidator.validate(0) // Returns "Vehicle price must be $1 or greater"
   * VehiclePriceValidator.validate(20000000) // Returns "Vehicle price cannot exceed $10,000,000"
   * ```
   * @remarks
   * The minimum valid price is assumed to be $1 because association fees rule states:
   * $5 for an amount between *$1* and $500. And there is no mention of a lower limit.
   * In a real corporate setting, this would have been validated with the product owner.
   */
  static validate(price: number): string | null {
    let MINIMUM_VEHICLE_PRICE = 1;
    let MAXIMUM_VEHICLE_PRICE = 10000000;

    if (price < MINIMUM_VEHICLE_PRICE) {
      return 'Vehicle price must be $1 or greater'
    }

    if (price > MAXIMUM_VEHICLE_PRICE) {
      return 'Vehicle price cannot exceed $10,000,000'
    }
    return null
  }
}
