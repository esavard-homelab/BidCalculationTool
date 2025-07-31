import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { nextTick } from 'vue'
import BidCalculatorView from '@/modules/BidCalculator/views/BidCalculatorView.vue'
import { bidCalculationService } from '@/modules/BidCalculator/services/BidCalculationService'
import { VehiclePriceValidator } from '@/modules/BidCalculator/validators/VehiclePriceValidator'
import type { BidCalculationResponse } from '@/modules/BidCalculator/dto/BidCalculationDto'
import type { VehicleTypeOption } from '@/modules/BidCalculator/services/BidCalculationService'

// Mock des services et validateurs
vi.mock('@/modules/BidCalculator/services/BidCalculationService', () => ({
  bidCalculationService: {
    getVehicleTypes: vi.fn(),
    calculateBid: vi.fn(),
  },
}))

vi.mock('@/modules/BidCalculator/validators/VehiclePriceValidator', () => ({
  VehiclePriceValidator: {
    validate: vi.fn(),
  },
}))

// Mock CalculationBreakdown component
vi.mock('@/modules/BidCalculator/components/CalculationBreakdown.vue', () => ({
  default: {
    name: 'CalculationBreakdown',
    props: ['calculation'],
    template: '<div class="calculation-breakdown-mock">Calculation Breakdown Mock</div>',
  },
}))

describe('BidCalculatorView.vue', () => {
  const mockVehicleTypes: VehicleTypeOption[] = [
    { value: 'Common', label: 'Common' },
    { value: 'Luxury', label: 'Luxury' },
  ]

  const mockCalculationResponse: BidCalculationResponse = {
    vehiclePrice: 1000,
    vehicleType: 'Common',
    totalCost: 1190,
    feeBreakdown: [
      {
        name: 'BasicBuyerFee',
        displayName: 'Basic Buyer Fee',
        amount: 50,
        description: 'Basic buyer fee calculation',
      },
      {
        name: 'SellerSpecialFee',
        displayName: "Seller's Special Fee",
        amount: 25,
        description: 'Special fee for sellers',
      },
      {
        name: 'AssociationFee',
        displayName: 'Association Fee',
        amount: 15,
        description: 'Association membership fee',
      },
      {
        name: 'StorageFee',
        displayName: 'Storage Fee',
        amount: 100,
        description: 'Vehicle storage fee',
      },
    ],
  }

  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(bidCalculationService.getVehicleTypes).mockResolvedValue(mockVehicleTypes)
    vi.mocked(bidCalculationService.calculateBid).mockResolvedValue(mockCalculationResponse)
    vi.mocked(VehiclePriceValidator.validate).mockReturnValue(null)
  })

  const createWrapper = async () => {
    const wrapper = mount(BidCalculatorView)
    // Wait for the component to fully mount and load vehicle types
    await flushPromises()
    await nextTick()
    return wrapper
  }

  describe('Component Structure', () => {
    it('should render the main title', async () => {
      const wrapper = await createWrapper()
      expect(wrapper.find('h1').text()).toBe('Bid Calculation Tool')
    })

    it('should render vehicle price input field', async () => {
      const wrapper = await createWrapper()
      const priceInput = wrapper.find('#vehiclePrice')

      expect(priceInput.exists()).toBe(true)
      expect(priceInput.attributes('type')).toBe('number')
      expect(priceInput.attributes('min')).toBe('0')
      expect(priceInput.attributes('step')).toBe('0.01')
    })

    it('should render vehicle type select field', async () => {
      const wrapper = await createWrapper()
      const typeSelect = wrapper.find('#vehicleType')

      expect(typeSelect.exists()).toBe(true)
      expect(typeSelect.element.tagName).toBe('SELECT')
    })

    it('should have correct CSS classes', async () => {
      const wrapper = await createWrapper()

      expect(wrapper.find('.bid-calculator').exists()).toBe(true)
      expect(wrapper.find('.input-section').exists()).toBe(true)
      expect(wrapper.findAll('.form-group')).toHaveLength(2)
    })
  })

  describe('Component Initialization', () => {
    it('should load vehicle types on mount', async () => {
      await createWrapper()

      expect(bidCalculationService.getVehicleTypes).toHaveBeenCalledTimes(1)
    })

    it('should populate vehicle types in select options', async () => {
      const wrapper = await createWrapper()

      const options = wrapper.findAll('option')
      expect(options).toHaveLength(2)
      expect(options[0].text()).toBe('Common')
      expect(options[1].text()).toBe('Luxury')
    })

    it('should set default vehicle type to Common', async () => {
      const wrapper = await createWrapper()
      const typeSelect = wrapper.find('#vehicleType')

      expect((typeSelect.element as HTMLSelectElement).value).toBe('Common')
    })

    it('should handle error when loading vehicle types fails', async () => {
      vi.mocked(bidCalculationService.getVehicleTypes).mockRejectedValue(new Error('Network error'))

      const wrapper = mount(BidCalculatorView)
      await flushPromises()
      await nextTick()

      const errorDiv = wrapper.find('.error-message')
      expect(errorDiv.exists()).toBe(true)
      expect(errorDiv.text()).toBe('Failed to load vehicle types')
    })
  })

  describe('Vehicle Price Input', () => {
    it('should update vehiclePrice when input changes', async () => {
      const wrapper = await createWrapper()
      const priceInput = wrapper.find('#vehiclePrice')

      await priceInput.setValue('1500')

      expect((priceInput.element as HTMLInputElement).value).toBe('1500')
    })

    it('should trigger calculateBid when price input changes', async () => {
      const wrapper = await createWrapper()

      const priceInput = wrapper.find('#vehiclePrice')
      await priceInput.setValue('1000')
      await priceInput.trigger('input')
      await nextTick()

      expect(bidCalculationService.calculateBid).toHaveBeenCalledWith({
        vehiclePrice: 1000,
        vehicleType: 'Common',
      })
    })

    it('should not calculate when price is null', async () => {
      const wrapper = await createWrapper()

      vi.clearAllMocks()

      const priceInput = wrapper.find('#vehiclePrice')
      await priceInput.setValue('')
      await priceInput.trigger('input')
      await nextTick()

      expect(bidCalculationService.calculateBid).not.toHaveBeenCalled()
    })

    it('should display price validation error', async () => {
      vi.mocked(VehiclePriceValidator.validate).mockReturnValue('Price must be positive')

      const wrapper = await createWrapper()

      const priceInput = wrapper.find('#vehiclePrice')
      await priceInput.setValue('-100')
      await priceInput.trigger('input')
      await nextTick()

      expect(wrapper.find('.error').text()).toBe('Price must be positive')
    })

    it('should clear calculation when price validation fails', async () => {
      vi.mocked(VehiclePriceValidator.validate).mockReturnValue('Invalid price')

      const wrapper = await createWrapper()

      // First set a valid price to get a calculation
      await wrapper.find('#vehiclePrice').setValue('1000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      // Then set an invalid price
      await wrapper.find('#vehiclePrice').setValue('-100')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      expect(wrapper.find('.results-section').exists()).toBe(false)
    })
  })

  describe('Vehicle Type Selection', () => {
    it('should update vehicleType when selection changes', async () => {
      const wrapper = await createWrapper()

      const typeSelect = wrapper.find('#vehicleType')
      await typeSelect.setValue('Luxury')

      expect((typeSelect.element as HTMLSelectElement).value).toBe('Luxury')
    })

    it('should trigger calculateBid when vehicle type changes', async () => {
      const wrapper = await createWrapper()

      // Set a price first
      await wrapper.find('#vehiclePrice').setValue('1000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      vi.clearAllMocks()

      const typeSelect = wrapper.find('#vehicleType')
      await typeSelect.setValue('Luxury')
      await typeSelect.trigger('change')
      await nextTick()

      expect(bidCalculationService.calculateBid).toHaveBeenCalledWith({
        vehiclePrice: 1000,
        vehicleType: 'Luxury',
      })
    })
  })

  describe('Bid Calculation', () => {
    it('should display calculation results when successful', async () => {
      const wrapper = await createWrapper()

      const priceInput = wrapper.find('#vehiclePrice')
      await priceInput.setValue('1000')
      await priceInput.trigger('input')
      await nextTick()

      expect(wrapper.find('.results-section').exists()).toBe(true)
      expect(wrapper.find('h2').text()).toBe('Calculation Breakdown')
      expect(wrapper.find('.calculation-breakdown-mock').exists()).toBe(true)
    })

    it('should pass correct calculation data to CalculationBreakdown component', async () => {
      const wrapper = await createWrapper()

      const priceInput = wrapper.find('#vehiclePrice')
      await priceInput.setValue('1000')
      await priceInput.trigger('input')
      await nextTick()

      const calculationBreakdown = wrapper.findComponent({ name: 'CalculationBreakdown' })
      expect(calculationBreakdown.props('calculation')).toEqual(mockCalculationResponse)
    })

    it('should handle calculation service errors', async () => {
      vi.mocked(bidCalculationService.calculateBid).mockRejectedValue(
        new Error('Calculation failed'),
      )

      const wrapper = await createWrapper()

      const priceInput = wrapper.find('#vehiclePrice')
      await priceInput.setValue('1000')
      await priceInput.trigger('input')
      await flushPromises()
      await nextTick()

      expect(wrapper.find('.error-message').text()).toBe(
        'Failed to calculate bid. Please try again.',
      )
    })

    it('should clear previous errors before new calculation', async () => {
      // First trigger an error
      vi.mocked(bidCalculationService.calculateBid).mockRejectedValue(new Error('Error'))

      const wrapper = await createWrapper()

      await wrapper.find('#vehiclePrice').setValue('1000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await flushPromises()
      await nextTick()

      expect(wrapper.find('.error-message').exists()).toBe(true)

      // Then make a successful call
      vi.mocked(bidCalculationService.calculateBid).mockResolvedValue(mockCalculationResponse)

      await wrapper.find('#vehiclePrice').setValue('1500')
      await wrapper.find('#vehiclePrice').trigger('input')
      await flushPromises()
      await nextTick()

      expect(wrapper.find('.error-message').exists()).toBe(false)
    })
  })

  describe('Error Handling', () => {
    it('should display error message when present', async () => {
      vi.mocked(bidCalculationService.getVehicleTypes).mockRejectedValue(new Error('Network error'))

      const wrapper = mount(BidCalculatorView)
      await flushPromises()
      await nextTick()

      const errorDiv = wrapper.find('.error-message')
      expect(errorDiv.exists()).toBe(true)
      expect(errorDiv.text()).toBe('Failed to load vehicle types')
    })

    it('should not display error message when no error', async () => {
      const wrapper = await createWrapper()

      expect(wrapper.find('.error-message').exists()).toBe(false)
    })

    it('should reset errors when new calculation starts', async () => {
      // Start with an error
      vi.mocked(bidCalculationService.calculateBid).mockRejectedValue(new Error('Error'))

      const wrapper = await createWrapper()

      await wrapper.find('#vehiclePrice').setValue('1000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await flushPromises()
      await nextTick()

      expect(wrapper.find('.error-message').exists()).toBe(true)

      // Change vehicle type (should reset errors)
      vi.mocked(bidCalculationService.calculateBid).mockResolvedValue(mockCalculationResponse)

      await wrapper.find('#vehicleType').setValue('Luxury')
      await wrapper.find('#vehicleType').trigger('change')
      await flushPromises()
      await nextTick()

      expect(wrapper.find('.error-message').exists()).toBe(false)
    })
  })

  describe('Integration Tests', () => {
    it('should complete full workflow: load types -> enter price -> calculate -> display results', async () => {
      const wrapper = await createWrapper()

      // Verify vehicle types loaded
      expect(wrapper.findAll('option')).toHaveLength(2)

      // Enter price
      await wrapper.find('#vehiclePrice').setValue('2000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      // Verify calculation was called
      expect(bidCalculationService.calculateBid).toHaveBeenCalledWith({
        vehiclePrice: 2000,
        vehicleType: 'Common',
      })

      // Verify results displayed
      expect(wrapper.find('.results-section').exists()).toBe(true)
      expect(wrapper.findComponent({ name: 'CalculationBreakdown' }).exists()).toBe(true)
    })

    it('should handle complete error recovery workflow', async () => {
      // Start with service error
      vi.mocked(bidCalculationService.calculateBid).mockRejectedValue(new Error('Service error'))

      const wrapper = await createWrapper()

      // Enter price (triggers error)
      await wrapper.find('#vehiclePrice').setValue('1000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await flushPromises()
      await nextTick()

      expect(wrapper.find('.error-message').exists()).toBe(true)

      // Fix service and retry
      vi.mocked(bidCalculationService.calculateBid).mockResolvedValue(mockCalculationResponse)

      await wrapper.find('#vehiclePrice').setValue('1500')
      await wrapper.find('#vehiclePrice').trigger('input')
      await flushPromises()
      await nextTick()

      // Verify error cleared and results shown
      expect(wrapper.find('.error-message').exists()).toBe(false)
      expect(wrapper.find('.results-section').exists()).toBe(true)
    })
  })

  describe('Reactive Behavior', () => {
    it('should hide results when price becomes invalid', async () => {
      const wrapper = await createWrapper()

      // First show results with valid price
      await wrapper.find('#vehiclePrice').setValue('1000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      expect(wrapper.find('.results-section').exists()).toBe(true)

      // Then make price invalid
      vi.mocked(VehiclePriceValidator.validate).mockReturnValue('Invalid price')

      await wrapper.find('#vehiclePrice').setValue('-100')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      expect(wrapper.find('.results-section').exists()).toBe(false)
    })

    it('should clear results when price is removed', async () => {
      const wrapper = await createWrapper()

      // First show results
      await wrapper.find('#vehiclePrice').setValue('1000')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      expect(wrapper.find('.results-section').exists()).toBe(true)

      // Clear price
      await wrapper.find('#vehiclePrice').setValue('')
      await wrapper.find('#vehiclePrice').trigger('input')
      await nextTick()

      expect(wrapper.find('.results-section').exists()).toBe(false)
    })
  })
})
