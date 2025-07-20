export class VehiclePriceValidator {
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
