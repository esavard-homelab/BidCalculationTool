import { describe, it, expect, vi, beforeEach } from 'vitest'
import { bidCalculationService } from '@/modules/BidCalculator/services/BidCalculationService'
import type { BidCalculationRequest, BidCalculationResponse } from '@/modules/BidCalculator/dto/BidCalculationDto'
import type { VehicleTypeOption } from '@/modules/BidCalculator/services/BidCalculationService'

// Mock global fetch
const mockFetch = vi.fn()
global.fetch = mockFetch

describe('BidCalculationService', () => {
  beforeEach(() => {
    mockFetch.mockClear()
  })

  describe('calculateBid', () => {
    const mockRequest: BidCalculationRequest = {
      vehiclePrice: 1000,
      vehicleType: 'Common'
    }

    const mockResponse: BidCalculationResponse = {
      vehiclePrice: 1000,
      vehicleType: 'Common',
      basicBuyerFee: 50,
      sellerSpecialFee: 25,
      associationFee: 15,
      storageFee: 100,
      totalCost: 1190
    }

    it('should make successful POST request and return calculation result', async () => {
      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(mockResponse)
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      const result = await bidCalculationService.calculateBid(mockRequest)

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/bidcalculation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(mockRequest),
      })
      expect(mockFetchResponse.json).toHaveBeenCalled()
      expect(result).toEqual(mockResponse)
    })

    it('should throw error when calculateBid request fails with 400', async () => {
      const mockFetchResponse = {
        ok: false,
        status: 400
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      await expect(bidCalculationService.calculateBid(mockRequest))
        .rejects.toThrow('HTTP error! status: 400')

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/bidcalculation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(mockRequest),
      })
    })

    it('should throw error when calculateBid request fails with 500', async () => {
      const mockFetchResponse = {
        ok: false,
        status: 500
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      await expect(bidCalculationService.calculateBid(mockRequest))
        .rejects.toThrow('HTTP error! status: 500')
    })

    it('should handle network errors', async () => {
      mockFetch.mockRejectedValue(new Error('Network error'))

      await expect(bidCalculationService.calculateBid(mockRequest))
        .rejects.toThrow('Network error')
    })

    it('should properly serialize complex request data', async () => {
      const complexRequest: BidCalculationRequest = {
        vehiclePrice: 1234.56,
        vehicleType: 'Luxury'
      }

      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(mockResponse)
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      await bidCalculationService.calculateBid(complexRequest)

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/bidcalculation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(complexRequest),
      })
    })

    it('should handle different vehicle types', async () => {
      const luxuryRequest: BidCalculationRequest = {
        vehiclePrice: 5000,
        vehicleType: 'Luxury'
      }

      const luxuryResponse: BidCalculationResponse = {
        vehiclePrice: 5000,
        vehicleType: 'Luxury',
        basicBuyerFee: 200,
        sellerSpecialFee: 100,
        associationFee: 20,
        storageFee: 100,
        totalCost: 5420
      }

      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(luxuryResponse)
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      const result = await bidCalculationService.calculateBid(luxuryRequest)

      expect(result).toEqual(luxuryResponse)
      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/bidcalculation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(luxuryRequest),
      })
    })
  })

  describe('getVehicleTypes', () => {
    const mockVehicleTypes: VehicleTypeOption[] = [
      { value: 'common', label: 'Common' },
      { value: 'luxury', label: 'Luxury' }
    ]

    it('should make successful GET request and return vehicle types', async () => {
      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(mockVehicleTypes)
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      const result = await bidCalculationService.getVehicleTypes()

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/bidcalculation/vehicle-types')
      expect(mockFetchResponse.json).toHaveBeenCalled()
      expect(result).toEqual(mockVehicleTypes)
    })

    it('should throw error when getVehicleTypes request fails with 404', async () => {
      const mockFetchResponse = {
        ok: false,
        status: 404
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      await expect(bidCalculationService.getVehicleTypes())
        .rejects.toThrow('HTTP error! status: 404')

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/bidcalculation/vehicle-types')
    })

    it('should throw error when getVehicleTypes request fails with 500', async () => {
      const mockFetchResponse = {
        ok: false,
        status: 500
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      await expect(bidCalculationService.getVehicleTypes())
        .rejects.toThrow('HTTP error! status: 500')
    })

    it('should handle network errors in getVehicleTypes', async () => {
      mockFetch.mockRejectedValue(new Error('Network error'))

      await expect(bidCalculationService.getVehicleTypes())
        .rejects.toThrow('Network error')
    })

    it('should handle empty vehicle types array', async () => {
      const emptyResponse: VehicleTypeOption[] = []
      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(emptyResponse)
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      const result = await bidCalculationService.getVehicleTypes()

      expect(result).toEqual([])
      expect(Array.isArray(result)).toBe(true)
    })

    it('should handle multiple vehicle types', async () => {
      const multipleVehicleTypes: VehicleTypeOption[] = [
        { value: 'common', label: 'Common' },
        { value: 'luxury', label: 'Luxury' },
        { value: 'classic', label: 'Classic' },
        { value: 'sport', label: 'Sport' }
      ]

      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue(multipleVehicleTypes)
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      const result = await bidCalculationService.getVehicleTypes()

      expect(result).toEqual(multipleVehicleTypes)
      expect(result).toHaveLength(4)
      expect(result[0]).toHaveProperty('value')
      expect(result[0]).toHaveProperty('label')
    })
  })

  describe('Service Instance', () => {
    it('should export a singleton instance', () => {
      expect(bidCalculationService).toBeDefined()
      expect(typeof bidCalculationService.calculateBid).toBe('function')
      expect(typeof bidCalculationService.getVehicleTypes).toBe('function')
    })

    it('should use correct base URL', async () => {
      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockResolvedValue([])
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      await bidCalculationService.getVehicleTypes()

      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/bidcalculation/vehicle-types')
    })
  })

  describe('Error Handling Edge Cases', () => {
    it('should handle JSON parsing errors in calculateBid', async () => {
      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockRejectedValue(new Error('Invalid JSON'))
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      const mockRequest: BidCalculationRequest = {
        vehiclePrice: 1000,
        vehicleType: 'Common'
      }

      await expect(bidCalculationService.calculateBid(mockRequest))
        .rejects.toThrow('Invalid JSON')
    })

    it('should handle JSON parsing errors in getVehicleTypes', async () => {
      const mockFetchResponse = {
        ok: true,
        json: vi.fn().mockRejectedValue(new Error('Invalid JSON'))
      }
      mockFetch.mockResolvedValue(mockFetchResponse)

      await expect(bidCalculationService.getVehicleTypes())
        .rejects.toThrow('Invalid JSON')
    })

    it('should handle different HTTP error codes', async () => {
      const errorCodes = [401, 403, 422, 503]

      for (const errorCode of errorCodes) {
        const mockFetchResponse = {
          ok: false,
          status: errorCode
        }
        mockFetch.mockResolvedValue(mockFetchResponse)

        await expect(bidCalculationService.getVehicleTypes())
          .rejects.toThrow(`HTTP error! status: ${errorCode}`)
      }
    })
  })
})
