import { expect, test } from 'vitest'
import { CurrencyFormatter } from '@/core/utils/CurrencyFormatter'

test('CurrencyFormatter.formatUSD', () => {
  const amount = 1234.56
  const formatted = CurrencyFormatter.formatUSD(amount)
  expect(formatted).toBe('$1,234.56')
})

test('CurrencyFormatter.formatUSD with zero', () => {
  const amount = 0
  const formatted = CurrencyFormatter.formatUSD(amount)
  expect(formatted).toBe('$0.00')
})

test('CurrencyFormatter.formatUSD with negative amount', () => {
  const amount = -1234.56
  const formatted = CurrencyFormatter.formatUSD(amount)
  expect(formatted).toBe('-$1,234.56')
})

test('CurrencyFormatter.formatUSD with large amount', () => {
  const amount = 123456789.99
  const formatted = CurrencyFormatter.formatUSD(amount)
  expect(formatted).toBe('$123,456,789.99')
})

test('CurrencyFormatter.formatUSD with small amount', () => {
  const amount = 0.01
  const formatted = CurrencyFormatter.formatUSD(amount)
  expect(formatted).toBe('$0.01')
})

test('CurrencyFormatter.formatUSD with fractional cents', () => {
  const amount = 1234.567
  const formatted = CurrencyFormatter.formatUSD(amount)
  expect(formatted).toBe('$1,234.57') // Rounding to two decimal places
})
