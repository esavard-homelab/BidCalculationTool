import type { BidCalculationRequest, BidCalculationResponse } from '../dto/BidCalculationDto'

export interface VehicleTypeOption {
  value: string
  label: string
}

class BidCalculationService {
  private readonly baseUrl = 'http://localhost:5000'

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

  async getVehicleTypes(): Promise<VehicleTypeOption[]> {
    const response = await fetch(`${this.baseUrl}/api/bidcalculation/vehicle-types`)

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    return await response.json()
  }
}

export const bidCalculationService = new BidCalculationService()
