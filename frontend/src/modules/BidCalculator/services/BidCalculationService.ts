import type { BidCalculationRequest, BidCalculationResponse } from '../dto/BidCalculationDto'

/**
 * Represents a vehicle type option for UI dropdowns.
 */
export interface VehicleTypeOption {
  /** The value used in API requests */
  value: string
  /** The human-readable label for display */
  label: string
}

/**
 * Service for handling bid calculation operations.
 * Manages communication with the backend API for vehicle bid calculations and related data.
 */
class BidCalculationService {
  private readonly baseUrl = 'http://localhost:5000'

  /**
   * Calculates the total bid amount for a vehicle.
   *
   * @param request - The bid calculation request containing vehicle price and type
   * @returns Promise that resolves to the calculation response with total price and fee breakdown
   * @throws Error when the API request fails or returns a non-ok status
   *
   * @example
   * ```typescript
   * const result = await bidCalculationService.calculateBid({
   *   vehiclePrice: 1000,
   *   vehicleType: 'Common'
   * })
   * console.log(result.totalPrice) // 1180
   * ```
   */
  async calculateBid(request: BidCalculationRequest): Promise<BidCalculationResponse> {
    const response = await fetch(`${this.baseUrl}/api/bidcalculation`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  }

  /**
   * Retrieves the available vehicle types from the backend.
   *
   * @returns Promise that resolves to an array of vehicle type options for UI components
   * @throws Error when the API request fails or returns a non-ok status
   *
   * @example
   * ```typescript
   * const vehicleTypes = await bidCalculationService.getVehicleTypes()
   * // Returns: [{ value: 'Common', label: 'Common' }, { value: 'Luxury', label: 'Luxury' }]
   * ```
   */
  async getVehicleTypes(): Promise<VehicleTypeOption[]> {
    const response = await fetch(`${this.baseUrl}/api/bidcalculation/vehicle-types`)

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  }
}

/**
 * Singleton instance of BidCalculationService for use throughout the application.
 * Pre-configured with the default backend URL.
 */
export const bidCalculationService = new BidCalculationService()
