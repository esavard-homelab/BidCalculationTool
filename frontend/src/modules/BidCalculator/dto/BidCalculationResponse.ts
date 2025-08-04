import type { FeeBreakdownItem } from './FeeBreakdownItem'

/**
 * Response payload from bid calculation API containing the breakdown of all fees.
 */
export interface BidCalculationResponse {
  /**
   * Dynamic breakdown of all fees applied to this calculation.
   * This future-proofs the frontend against new fee types being added.
   */
  feeBreakdown: FeeBreakdownItem[]

  /** The original vehicle price */
  vehiclePrice: number

  /** The vehicle type used in calculation */
  vehicleType: string

  /** Final price including all fees */
  totalCost: number
}
