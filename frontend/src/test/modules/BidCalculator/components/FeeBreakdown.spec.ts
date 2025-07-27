import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import FeeBreakdown from '@/modules/BidCalculator/components/FeeBreakdown.vue'
import type { BidCalculationResponse } from '@/modules/BidCalculator/dto/BidCalculationDto'
import { CurrencyFormatter } from '@/core/utils/CurrencyFormatter'

// Mock du CurrencyFormatter
vi.mock('@/core/utils/CurrencyFormatter', () => ({
  CurrencyFormatter: {
    formatUSD: vi.fn()
  }
}))

describe('FeeBreakdown.vue', () => {
  const mockCalculation: BidCalculationResponse = {
    vehiclePrice: 1000,
    vehicleType: 'Common',
    basicBuyerFee: 50,
    sellerSpecialFee: 25,
    associationFee: 15,
    storageFee: 100,
    totalCost: 1190
  }

  // Reset mock before each test to ensure isolation
  beforeEach(() => {
    vi.mocked(CurrencyFormatter.formatUSD).mockImplementation((amount: number) => `$${amount.toFixed(2)}`)
  })

  const createWrapper = (calculation: BidCalculationResponse = mockCalculation) => {
    return mount(FeeBreakdown, {
      props: {
        calculation
      }
    })
  }

  describe('Component Structure', () => {
    it('should render the component with correct structure', () => {
      const wrapper = createWrapper()

      expect(wrapper.find('.fee-breakdown').exists()).toBe(true)
      expect(wrapper.find('.breakdown-table').exists()).toBe(true)
      expect(wrapper.find('tbody').exists()).toBe(true)
    })

    it('should render all required table rows', () => {
      const wrapper = createWrapper()
      const rows = wrapper.findAll('tbody tr')

      expect(rows).toHaveLength(6) // 5 fee rows + 1 total row
    })

    it('should have correct CSS classes', () => {
      const wrapper = createWrapper()

      expect(wrapper.find('.fee-breakdown').classes()).toContain('fee-breakdown')
      expect(wrapper.find('.breakdown-table').classes()).toContain('breakdown-table')
      expect(wrapper.find('.total-row').exists()).toBe(true)
    })
  })

  describe('Data Display', () => {
    it('should display vehicle price correctly', () => {
      const wrapper = createWrapper()
      const rows = wrapper.findAll('tbody tr')

      expect(rows[0].text()).toContain('Vehicle Price:')
      expect(rows[0].text()).toContain('$1000.00')
    })

    it('should display basic buyer fee correctly', () => {
      const wrapper = createWrapper()
      const rows = wrapper.findAll('tbody tr')

      expect(rows[1].text()).toContain('Basic Buyer Fee:')
      expect(rows[1].text()).toContain('$50.00')
    })

    it('should display seller special fee correctly', () => {
      const wrapper = createWrapper()
      const rows = wrapper.findAll('tbody tr')

      expect(rows[2].text()).toContain("Seller's Special Fee:")
      expect(rows[2].text()).toContain('$25.00')
    })

    it('should display association fee correctly', () => {
      const wrapper = createWrapper()
      const rows = wrapper.findAll('tbody tr')

      expect(rows[3].text()).toContain('Association Fee:')
      expect(rows[3].text()).toContain('$15.00')
    })

    it('should display storage fee correctly', () => {
      const wrapper = createWrapper()
      const rows = wrapper.findAll('tbody tr')

      expect(rows[4].text()).toContain('Storage Fee:')
      expect(rows[4].text()).toContain('$100.00')
    })

    it('should display total cost in the total row', () => {
      const wrapper = createWrapper()
      const totalRow = wrapper.find('.total-row')

      expect(totalRow.text()).toContain('Total Cost:')
      expect(totalRow.text()).toContain('$1190.00')
    })
  })

  describe('Currency Formatting', () => {
    it('should call CurrencyFormatter.formatUSD for each amount', () => {
      const formatUSDSpy = vi.mocked(CurrencyFormatter.formatUSD)
      formatUSDSpy.mockClear()

      createWrapper()

      expect(formatUSDSpy).toHaveBeenCalledWith(1000) // vehiclePrice
      expect(formatUSDSpy).toHaveBeenCalledWith(50)   // basicBuyerFee
      expect(formatUSDSpy).toHaveBeenCalledWith(25)   // sellerSpecialFee
      expect(formatUSDSpy).toHaveBeenCalledWith(15)   // associationFee
      expect(formatUSDSpy).toHaveBeenCalledWith(100)  // storageFee
      expect(formatUSDSpy).toHaveBeenCalledWith(1190) // totalCost

      expect(formatUSDSpy).toHaveBeenCalledTimes(6)
    })

    it('should handle zero amounts correctly', () => {
      const zeroCalculation: BidCalculationResponse = {
        vehiclePrice: 0,
        vehicleType: 'Common',
        basicBuyerFee: 0,
        sellerSpecialFee: 0,
        associationFee: 0,
        storageFee: 0,
        totalCost: 0
      }

      const wrapper = createWrapper(zeroCalculation)
      const amounts = wrapper.findAll('.amount')

      amounts.forEach(amount => {
        expect(amount.text()).toBe('$0.00')
      })
    })

    it('should handle negative amounts correctly', () => {
      const negativeCalculation: BidCalculationResponse = {
        vehiclePrice: -1000,
        vehicleType: 'Common',
        basicBuyerFee: -50,
        sellerSpecialFee: -25,
        associationFee: -15,
        storageFee: -100,
        totalCost: -1190
      }

      const formatUSDSpy = vi.mocked(CurrencyFormatter.formatUSD)
      formatUSDSpy.mockImplementation((amount: number) => `-$${Math.abs(amount).toFixed(2)}`)

      createWrapper(negativeCalculation)

      expect(formatUSDSpy).toHaveBeenCalledWith(-1000)
      expect(formatUSDSpy).toHaveBeenCalledWith(-50)
      expect(formatUSDSpy).toHaveBeenCalledWith(-25)
      expect(formatUSDSpy).toHaveBeenCalledWith(-15)
      expect(formatUSDSpy).toHaveBeenCalledWith(-100)
      expect(formatUSDSpy).toHaveBeenCalledWith(-1190)
    })
  })

  describe('Props Validation', () => {
    it('should accept valid calculation prop', () => {
      const wrapper = createWrapper()
      expect(wrapper.props('calculation')).toEqual(mockCalculation)
    })

    it('should render correctly with different calculation values', () => {
      const customCalculation: BidCalculationResponse = {
        vehiclePrice: 5000,
        vehicleType: 'Luxury',
        basicBuyerFee: 250,
        sellerSpecialFee: 125,
        associationFee: 75,
        storageFee: 200,
        totalCost: 5650
      }

      const wrapper = createWrapper(customCalculation)
      const amounts = wrapper.findAll('.amount')

      expect(amounts[0].text()).toBe('$5000.00') // vehiclePrice
      expect(amounts[1].text()).toBe('$250.00')  // basicBuyerFee
      expect(amounts[2].text()).toBe('$125.00')  // sellerSpecialFee
      expect(amounts[3].text()).toBe('$75.00')   // associationFee
      expect(amounts[4].text()).toBe('$200.00')  // storageFee
      expect(amounts[5].text()).toBe('$5650.00') // totalCost
    })

    it('should handle decimal amounts correctly', () => {
      const decimalCalculation: BidCalculationResponse = {
        vehiclePrice: 1234.56,
        vehicleType: 'Common',
        basicBuyerFee: 62.73,
        sellerSpecialFee: 31.36,
        associationFee: 18.82,
        storageFee: 100.00,
        totalCost: 1447.47
      }

      const formatUSDSpy = vi.mocked(CurrencyFormatter.formatUSD)
      formatUSDSpy.mockImplementation((amount: number) => `$${amount.toFixed(2)}`)

      createWrapper(decimalCalculation)

      expect(formatUSDSpy).toHaveBeenCalledWith(1234.56)
      expect(formatUSDSpy).toHaveBeenCalledWith(62.73)
      expect(formatUSDSpy).toHaveBeenCalledWith(31.36)
      expect(formatUSDSpy).toHaveBeenCalledWith(18.82)
      expect(formatUSDSpy).toHaveBeenCalledWith(100.00)
      expect(formatUSDSpy).toHaveBeenCalledWith(1447.47)
    })
  })

  describe('CSS Classes and Styling', () => {
    it('should apply correct CSS classes to amount cells', () => {
      const wrapper = createWrapper()
      const amountCells = wrapper.findAll('.amount')

      amountCells.forEach(cell => {
        expect(cell.classes()).toContain('amount')
      })
    })

    it('should apply total-row class to the last row', () => {
      const wrapper = createWrapper()
      const rows = wrapper.findAll('tbody tr')
      const lastRow = rows[rows.length - 1]

      expect(lastRow.classes()).toContain('total-row')
    })

    it('should render strong tags in total row', () => {
      const wrapper = createWrapper()
      const totalRow = wrapper.find('.total-row')
      const strongTags = totalRow.findAll('strong')

      expect(strongTags).toHaveLength(2)
      expect(strongTags[0].text()).toBe('Total Cost:')
      expect(strongTags[1].text()).toBe('$1190.00')
    })
  })

  describe('Accessibility', () => {
    it('should have proper table structure for screen readers', () => {
      const wrapper = createWrapper()

      expect(wrapper.find('table').exists()).toBe(true)
      expect(wrapper.find('tbody').exists()).toBe(true)

      const rows = wrapper.findAll('tbody tr')
      rows.forEach(row => {
        const cells = row.findAll('td')
        expect(cells).toHaveLength(2) // Label and amount
      })
    })

    it('should have meaningful text content', () => {
      const wrapper = createWrapper()
      const tableText = wrapper.find('.breakdown-table').text()

      expect(tableText).toContain('Vehicle Price:')
      expect(tableText).toContain('Basic Buyer Fee:')
      expect(tableText).toContain("Seller's Special Fee:")
      expect(tableText).toContain('Association Fee:')
      expect(tableText).toContain('Storage Fee:')
      expect(tableText).toContain('Total Cost:')
    })
  })
})
