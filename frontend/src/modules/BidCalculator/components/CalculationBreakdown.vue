<template>
  <div class="calculation-breakdown">
    <table class="breakdown-table">
      <tbody>
        <!-- Vehicle Price (not a fee, but part of calculation) -->
        <tr>
          <td>Vehicle Price:</td>
          <td class="amount">{{ formatCurrency(calculation.vehiclePrice) }}</td>
        </tr>

        <!-- Dynamic fees from feeBreakdown -->
        <tr
          v-for="fee in calculation.feeBreakdown"
          :key="fee.name"
          :title="fee.description"
          class="fee-row"
        >
          <td>{{ fee.displayName }}:</td>
          <td class="amount">{{ formatCurrency(fee.amount) }}</td>
        </tr>

        <!-- Total Cost (highlighted) -->
        <tr class="total-row">
          <td><strong>Total Cost:</strong></td>
          <td class="amount">
            <strong>{{ formatCurrency(calculation.totalCost) }}</strong>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import type { BidCalculationResponse } from '../dto/BidCalculationDto'
import { CurrencyFormatter } from '@/core/utils/CurrencyFormatter'

/**
 * Props interface for the CalculationBreakdown component.
 */
interface Props {
  /** The bid calculation response containing all fee details */
  calculation: BidCalculationResponse
}

defineProps<Props>()

/**
 * Formats a numeric amount as USD currency for display.
 *
 * @param amount - The numeric value to format
 * @returns The formatted currency string (e.g., "$1,234.56")
 */
function formatCurrency(amount: number): string {
  return CurrencyFormatter.formatUSD(amount)
}
</script>

<style scoped>
.calculation-breakdown {
  background-color: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  border: 1px solid #dee2e6;
}

.breakdown-table {
  width: 100%;
  border-collapse: collapse;
}

.breakdown-table td {
  padding: 0.75rem 0;
  border-bottom: 1px solid #dee2e6;
}

.breakdown-table td:first-child {
  font-weight: 500;
}

.amount {
  text-align: right;
  font-family: 'Courier New', monospace;
}

/* Hover effect for fee rows with descriptions */
.fee-row {
  cursor: help;
  transition: background-color 0.2s ease;
}

.fee-row:hover {
  background-color: #e9ecef;
}

.total-row {
  border-top: 2px solid #343a40;
  margin-top: 0.5rem;
}

.total-row td {
  padding-top: 1rem;
  border-bottom: none;
  font-size: 1.1rem;
  color: #28a745;
}
</style>
