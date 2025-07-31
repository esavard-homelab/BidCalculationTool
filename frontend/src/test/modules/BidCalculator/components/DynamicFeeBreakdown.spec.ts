import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import DynamicFeeBreakdown from '@/modules/BidCalculator/components/DynamicFeeBreakdown.vue'
import type { FeeBreakdownItem } from '@/modules/BidCalculator/dto/BidCalculationDto'

describe('DynamicFeeBreakdown.vue', () => {
  const mockFeeBreakdown: FeeBreakdownItem[] = [
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
  ]

  it('should render the title correctly', () => {
    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: mockFeeBreakdown },
    })

    expect(wrapper.find('.breakdown-title').text()).toBe('Fee Breakdown')
  })

  it('should render all fee items', () => {
    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: mockFeeBreakdown },
    })

    const feeItems = wrapper.findAll('.fee-item')
    expect(feeItems).toHaveLength(4)
  })

  it('should display fee names correctly', () => {
    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: mockFeeBreakdown },
    })

    const feeNames = wrapper.findAll('.fee-name')
    expect(feeNames[0].text()).toBe('Basic Buyer Fee')
    expect(feeNames[1].text()).toBe("Seller's Special Fee")
    expect(feeNames[2].text()).toBe('Association Fee')
    expect(feeNames[3].text()).toBe('Storage Fee')
  })

  it('should format fee amounts as currency', () => {
    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: mockFeeBreakdown },
    })

    const feeAmounts = wrapper.findAll('.fee-amount')
    expect(feeAmounts[0].text()).toBe('$50.00')
    expect(feeAmounts[1].text()).toBe('$20.00')
    expect(feeAmounts[2].text()).toBe('$10.00')
    expect(feeAmounts[3].text()).toBe('$100.00')
  })

  it('should show fee descriptions when present', () => {
    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: mockFeeBreakdown },
    })

    const descriptions = wrapper.findAll('.fee-description')
    expect(descriptions).toHaveLength(4)
    expect(descriptions[0].text()).toBe('10% of vehicle price with minimum and maximum limits')
    expect(descriptions[1].text()).toBe('2% for Common vehicles, 4% for Luxury vehicles')
  })

  it('should set title attribute for tooltips', () => {
    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: mockFeeBreakdown },
    })

    const feeItems = wrapper.findAll('.fee-item')
    expect(feeItems[0].attributes('title')).toBe(
      '10% of vehicle price with minimum and maximum limits',
    )
    expect(feeItems[1].attributes('title')).toBe('2% for Common vehicles, 4% for Luxury vehicles')
  })

  it('should handle empty fee breakdown', () => {
    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: [] },
    })

    expect(wrapper.find('.breakdown-title').text()).toBe('Fee Breakdown')
    expect(wrapper.findAll('.fee-item')).toHaveLength(0)
  })

  it('should handle fees without descriptions', () => {
    const feesWithoutDesc: FeeBreakdownItem[] = [
      {
        name: 'SimpleFee',
        displayName: 'Simple Fee',
        amount: 25,
        description: undefined,
      },
    ]

    const wrapper = mount(DynamicFeeBreakdown, {
      props: { feeBreakdown: feesWithoutDesc },
    })

    const feeItems = wrapper.findAll('.fee-item')
    expect(feeItems).toHaveLength(1)
    expect(wrapper.findAll('.fee-description')).toHaveLength(0)
  })
})
