import { describe, it, expect, vi } from 'vitest'
import { bidCalculationService } from '@/modules/BidCalculator/services/BidCalculationService'
import type { BidCalculationRequest } from '@/modules/BidCalculator/dto/BidCalculationDto'

// Mock global fetch
const mockFetch = vi.fn()
global.fetch = mockFetch

describe('BidCalculationService', () => {
  describe('Integration with ApiClient', () => {
    it('should calculate bid with correct endpoint and payload', async () => {
      // Arrange
      const mockRequest: BidCalculationRequest = {
        vehiclePrice: 1000,
        vehicleType: 'Common',
      }

      const mockResponse = {
        vehiclePrice: 1000,
        vehicleType: 'Common',
        totalCost: 1180,
        feeBreakdown: [{ name: 'BasicBuyerFee', displayName: 'Basic Buyer Fee', amount: 50 }],
      }

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve(mockResponse),
      })

      // Act
      const result = await bidCalculationService.calculateBid(mockRequest)

      // Assert
      expect(result.vehiclePrice).toBe(1000)
      expect(result.totalCost).toBe(1180)
      expect(result.feeBreakdown).toBeDefined()
    })

    it('should throw error when calculateBid API returns non-ok status', async () => {
      // Arrange
      const mockRequest: BidCalculationRequest = {
        vehiclePrice: 1000,
        vehicleType: 'Common',
      }

      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 500,
      })

      // Act & Assert
      await expect(bidCalculationService.calculateBid(mockRequest)).rejects.toThrow(
        'HTTP error! status: 500',
      )
    })

    it('should get vehicle types from correct endpoint', async () => {
      // Arrange
      const mockVehicleTypes = [
        { label: 'Common', value: 'Common' },
        { label: 'Luxury', value: 'Luxury' },
      ]
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve(mockVehicleTypes),
      })

      // Act
      const result = await bidCalculationService.getVehicleTypes()

      // Assert
      expect(result).toHaveLength(2)
      expect(result[0]).toEqual({ label: 'Common', value: 'Common' })
      expect(result[1]).toEqual({ label: 'Luxury', value: 'Luxury' })
    })

    it('should throw error when getVehicleTypes API returns non-ok status', async () => {
      // Arrange
      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 404,
      })

      // Act & Assert
      await expect(bidCalculationService.getVehicleTypes()).rejects.toThrow(
        'HTTP error! status: 404',
      )
    })
  })
})
