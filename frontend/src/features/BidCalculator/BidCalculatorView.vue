<template>
  <div class="bid-calculator">
    <h1>Bid Calculation Tool</h1>

    <div class="input-section">
      <div class="form-group">
        <label for="vehiclePrice">Vehicle Price ($):</label>
        <input
          id="vehiclePrice"
          v-model.number="vehiclePrice"
          type="number"
          min="0"
          step="0.01"
          placeholder="Enter vehicle price"
          @input="calculateBid"
        />
        <span v-if="priceError" class="error">{{ priceError }}</span>
      </div>

      <div class="form-group">
        <label for="vehicleType">Vehicle Type:</label>
        <select id="vehicleType" v-model="vehicleType" @change="calculateBid">
          <option
            v-for="type in vehicleTypes"
            :key="type.value"
            :value="type.value"
          >
            {{ type.label }}
          </option>
        </select>
      </div>
    </div>

    <div v-if="calculation" class="results-section">
      <h2>Fee Breakdown</h2>
      <fee-breakdown :calculation="calculation" />
    </div>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { bidCalculationService } from './services/BidCalculationService'
import type { BidCalculationResponse } from './dto/BidCalculationDto'
import type { VehicleTypeOption } from './services/BidCalculationService'
import FeeBreakdown from './components/FeeBreakdown.vue'
import {VehiclePriceValidator} from "@/features/BidCalculator/validators/VehiclePriceValidator.ts";

const vehiclePrice = ref<number | null>(null)
const vehicleType = ref<string>('Common')
const vehicleTypes = ref<VehicleTypeOption[]>([])
const calculation = ref<BidCalculationResponse | null>(null)
const priceError = ref<string | null>(null)
const error = ref<string | null>(null)
const loading = ref<boolean>(false)

const loadVehicleTypes = async () => {
  try {
    loading.value = true
    error.value = null
    vehicleTypes.value = await bidCalculationService.getVehicleTypes()
  } catch (err) {
    error.value = 'Failed to load vehicle types'
    console.error('Error loading vehicle types:', err)
  } finally {
    loading.value = false
  }
}

async function calculateBid() {
  // Reset errors
  priceError.value = null
  error.value = null

  // Do not calculate if vehicle price is not set
  if (vehiclePrice.value === null || vehiclePrice.value === undefined ||
    typeof vehiclePrice.value !== 'number') {
    calculation.value = null
    return
  }

  // Validate price
  const validationError = VehiclePriceValidator.validate(vehiclePrice.value)
  if (validationError) {
    priceError.value = validationError
    calculation.value = null
    return
  }

  try {
    calculation.value = await bidCalculationService.calculateBid({
      vehiclePrice: vehiclePrice.value,
      vehicleType: vehicleType.value
    })
  } catch (err) {
    error.value = 'Failed to calculate bid. Please try again.'
    console.error('Calculation error:', err)
  }
}

// Load vehicle types and calculate on component mount
onMounted(async () => {
  loading.value = true
  await loadVehicleTypes()
  loading.value = false
})
</script>

<style scoped>
.bid-calculator {
  min-width: 600px;
  margin: 0 auto;
  padding: 2rem;
  background-color: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.input-section {
  margin-bottom: 2rem;
}

.form-group {
  margin-bottom: 1rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

input, select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 1rem;
}

.error {
  color: #dc3545;
  font-size: 0.875rem;
  margin-top: 0.25rem;
  display: block;
}

.error-message {
  background-color: #f8d7da;
  color: #721c24;
  padding: 1rem;
  border-radius: 4px;
  margin-top: 1rem;
}

.results-section {
  margin-top: 2rem;
}

h1, h2 {
  color: #333;
}
</style>
