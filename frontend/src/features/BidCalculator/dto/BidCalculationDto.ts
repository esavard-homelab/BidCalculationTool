export interface BidCalculationRequest {
  vehiclePrice: number;
  vehicleType: string;
}

export interface BidCalculationResponse {
  vehiclePrice: number;
  vehicleType: string;
  basicBuyerFee: number;
  sellerSpecialFee: number;
  associationFee: number;
  storageFee: number;
  totalCost: number;
}

export interface FeeBreakdown {
  basicBuyerFee: number;
  sellerSpecialFee: number;
  associationFee: number;
  storageFee: number;
}
