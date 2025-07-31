<template>
  <div class="dynamic-fee-breakdown">
    <h3 class="breakdown-title">Fee Breakdown</h3>
    <div class="fee-list">
      <div v-for="fee in feeBreakdown" :key="fee.name" class="fee-item" :title="fee.description">
        <span class="fee-name">{{ fee.displayName }}</span>
        <span class="fee-amount">{{ formatCurrency(fee.amount) }}</span>
        <div v-if="fee.description" class="fee-description">
          {{ fee.description }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { FeeBreakdownItem } from '../dto/BidCalculationDto'
import { CurrencyFormatter } from '@/core/utils/CurrencyFormatter'

interface Props {
  feeBreakdown: FeeBreakdownItem[]
}

defineProps<Props>()

// Helper function to format currency using the static method
const formatCurrency = (amount: number) => CurrencyFormatter.formatUSD(amount)
</script>

<style scoped>
.dynamic-fee-breakdown {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 1rem;
  margin-top: 1rem;
}

.breakdown-title {
  margin: 0 0 1rem 0;
  color: #2c3e50;
  font-size: 1.1rem;
  font-weight: 600;
}

.fee-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.fee-item {
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 1rem;
  padding: 0.5rem;
  background: white;
  border-radius: 4px;
  border-left: 3px solid #3498db;
  transition: background-color 0.2s ease;
}

.fee-item:hover {
  background-color: #f0f0f0;
}

.fee-name {
  font-weight: 500;
  color: #2c3e50;
}

.fee-amount {
  font-weight: 600;
  color: #27ae60;
  text-align: right;
}

.fee-description {
  grid-column: 1 / -1;
  font-size: 0.85rem;
  color: #7f8c8d;
  margin-top: 0.25rem;
  font-style: italic;
}

/* Responsive design */
@media (max-width: 640px) {
  .fee-item {
    grid-template-columns: 1fr;
    gap: 0.5rem;
  }

  .fee-amount {
    text-align: left;
  }
}
</style>
