import { expect, test } from 'vitest'
import { VehiclePriceValidator } from '@/modules/BidCalculator/validators/VehiclePriceValidator'

test('VehiclePriceValidator.validate with valid price', () => {
  const price = 5000
  const result = VehiclePriceValidator.validate(price)
  expect(result).toBeNull()
})

test('VehiclePriceValidator.validate with zero price', () => {
  const price = 0
  const result = VehiclePriceValidator.validate(price)
  expect(result).toBe('Vehicle price must be $1 or greater')
})

test('VehiclePriceValidator.validate with negative price', () => {
  const price = -1000
  const result = VehiclePriceValidator.validate(price)
  expect(result).toBe('Vehicle price must be $1 or greater')
})

test('VehiclePriceValidator.validate with price exceeding limit', () => {
  const price = 10000001
  const result = VehiclePriceValidator.validate(price)
  expect(result).toBe('Vehicle price cannot exceed $10,000,000')
})

test('VehiclePriceValidator.validate with maximum valid price', () => {
  const price = 10000000
  const result = VehiclePriceValidator.validate(price)
  expect(result).toBeNull()
})
