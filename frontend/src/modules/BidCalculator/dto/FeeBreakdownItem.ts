/**
 * Individual fee item in the dynamic fee breakdown.
 * This allows the frontend to render fees generically without being coupled to specific fee types.
 */
export interface FeeBreakdownItem {
  /** Internal name/key of the fee (e.g., "BasicBuyerFee") */
  name: string
  /** Human-readable display name (e.g., "Basic Buyer Fee") */
  displayName: string
  /** The calculated fee amount */
  amount: number
  /** Optional description for tooltip or additional information */
  description?: string
}

