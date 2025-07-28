<template>
  <div class="fee-breakdown">
    <table class="breakdown-table">
      <tbody>
        <tr>
          <td>Vehicle Price:</td>
          <td class="amount">{{ formatCurrency(calculation.vehiclePrice) }}</td>
        </tr>
        <tr>
          <td>Basic Buyer Fee:</td>
          <td class="amount">{{ formatCurrency(calculation.basicBuyerFee) }}</td>
        </tr>
        <tr>
          <td>Seller's Special Fee:</td>
          <td class="amount">{{ formatCurrency(calculation.sellerSpecialFee) }}</td>
        </tr>
        <tr>
          <td>Association Fee:</td>
          <td class="amount">{{ formatCurrency(calculation.associationFee) }}</td>
        </tr>
        <tr>
          <td>Storage Fee:</td>
          <td class="amount">{{ formatCurrency(calculation.storageFee) }}</td>
        </tr>
        <tr class="total-row">
          <td><strong>Total Cost:</strong></td>
          <td class="amount"><strong>{{ formatCurrency(calculation.totalCost) }}</strong></td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import type { BidCalculationResponse } from '../dto/BidCalculationDto'
import {CurrencyFormatter} from '@/core/utils/CurrencyFormatter.ts';

/**
 * Props interface for the FeeBreakdown component.
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
.fee-breakdown {
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
