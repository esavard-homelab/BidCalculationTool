/**
 * Request payload for bid calculation API.
 */
export interface BidCalculationRequest {
  /** The price of the vehicle in USD */
  vehiclePrice: number;
  /** The type of vehicle ('Common' or 'Luxury') */
  vehicleType: string;
}

/**
 * Response payload from bid calculation API containing the breakdown of all fees.
 */
export interface BidCalculationResponse {
  /** The original vehicle price */
  vehiclePrice: number;
  /** The vehicle type used in calculation */
  vehicleType: string;
  //TODO: replace by FeeBreakdown: dynamic list of fees
  /** Basic buyer fee (10% of vehicle price with min/max limits) */
  basicBuyerFee: number;
  /** Seller's special fee (2% for Common, 4% for Luxury) */
  sellerSpecialFee: number;
  /** Association fee based on price ranges ($5-$20) */
  associationFee: number;
  /** Fixed storage fee ($100) */
  storageFee: number;
  /** Total cost including all fees */
  totalCost: number;
}

/**
 * Breakdown of individual fees for detailed display.
 */
export interface FeeBreakdown {
  /** Basic buyer fee amount */
  basicBuyerFee: number;
  /** Seller's special fee amount */
  sellerSpecialFee: number;
  /** Association fee amount */
  associationFee: number;
  /** Storage fee amount */
  storageFee: number;
}
