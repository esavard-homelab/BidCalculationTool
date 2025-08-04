/**
 * Request payload for bid calculation API.
 */
export interface BidCalculationRequest {
  /** The price of the vehicle in USD */
  vehiclePrice: number
  /** The type of vehicle ('Common' or 'Luxury') */
  vehicleType: string
}

