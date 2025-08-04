import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import CalculationBreakdown from '@/modules/BidCalculator/components/CalculationBreakdown.vue'
import type { BidCalculationResponse } from '@/modules/BidCalculator/dto/BidCalculationDto'

describe('CalculationBreakdown.vue', () => {
  const mockCalculation: BidCalculationResponse = {
    vehiclePrice: 1000,
    vehicleType: 'Common',
    totalCost: 1180,
    feeBreakdown: [
      {
        name: 'BasicBuyerFee',
        displayName: 'Basic Buyer Fee',
        amount: 50,
        description: '10% of vehicle price with minimum and maximum limits',
      },
      {
        name: 'SpecialFee',
        displayName: "Seller's Special Fee",
        amount: 20,
        description: '2% for Common vehicles, 4% for Luxury vehicles',
      },
      {
        name: 'AssociationFee',
        displayName: 'Association Fee',
        amount: 10,
        description: 'Tiered fee based on vehicle price',
      },
      {
        name: 'StorageFee',
        displayName: 'Storage Fee',
        amount: 100,
        description: 'Fixed storage fee for all vehicles',
      },
    ],
  }

  it('should render all calculation items dynamically', () => {
    const wrapper = mount(CalculationBreakdown, {
      props: { calculation: mockCalculation },
    })

    // Check that all elements are rendered
    const rows = wrapper.findAll('tbody tr')
    expect(rows).toHaveLength(6) // Vehicle Price + 4 fees + Total

    // Verify dynamic content of fees
    const feeRows = rows.slice(1, 5) // Skip vehicle price, before total
    feeRows.forEach((row, index) => {
      const fee = mockCalculation.feeBreakdown[index]
      expect(row.text()).toContain(fee.displayName)
      expect(row.text()).toContain(fee.amount.toString())
    })

    // Check total
    const totalRow = rows[5]
    expect(totalRow.text()).toContain('Total Cost')
    expect(totalRow.text()).toContain('1,180')
  })

  it('should show descriptions in title attributes for tooltips', () => {
    const wrapper = mount(CalculationBreakdown, {
      props: { calculation: mockCalculation },
    })

    const feeRows = wrapper.findAll('.fee-row')
    feeRows.forEach((row, index) => {
      const fee = mockCalculation.feeBreakdown[index]
      expect(row.attributes('title')).toBe(fee.description)
    })
  })

  it('should handle empty fee breakdown gracefully', () => {
    const emptyCalculation = {
      ...mockCalculation,
      feeBreakdown: [],
    }

    const wrapper = mount(CalculationBreakdown, {
      props: { calculation: emptyCalculation },
    })

    // Should show Vehicle Price and Total only
    const rows = wrapper.findAll('tbody tr')
    expect(rows).toHaveLength(2)
  })
})
