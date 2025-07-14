<script setup lang="ts">
import { ref } from 'vue'

const connectionStatus = ref<string>('')
const apiResponse = ref<any>(null)
const isLoading = ref<boolean>(false)

const testConnection = async () => {
  isLoading.value = true
  connectionStatus.value = 'Testing connection...'

  try {
    const response = await fetch('http://localhost:5000/BidCalculation/datetime')

    if (response.ok) {
      const data = await response.json()
      apiResponse.value = data
      connectionStatus.value = 'Connection successful!'
    } else {
      connectionStatus.value = `Connection failed: ${response.status}`
    }
  } catch (error) {
    connectionStatus.value = `Connection error: ${error}`
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="app-container">
    <header class="app-header">
      <h1>The Bid Calculation Tool</h1>
      <p class="subtitle">Calculate total vehicle auction prices with fees</p>
    </header>

    <main class="main-content">
      <section class="connection-test">
        <h2>Backend Connection Test</h2>
        <button
          @click="testConnection"
          :disabled="isLoading"
          class="test-button"
        >
          {{ isLoading ? 'Testing...' : 'Test Backend Connection' }}
        </button>

        <div v-if="connectionStatus" class="status-message">
          <p>{{ connectionStatus }}</p>
        </div>

        <div v-if="apiResponse" class="api-response">
          <h3>API Response:</h3>
          <div class="response-content">
            <p><strong>Message:</strong> {{ apiResponse.message }}</p>
            <p><strong>Server Time:</strong> {{ apiResponse.dateTime }}</p>
            <p><strong>Time Zone:</strong> {{ apiResponse.serverTimeZone }}</p>
          </div>
        </div>
      </section>

      <section class="coming-soon">
        <h2>Coming Soon</h2>
        <p>Vehicle price calculation functionality will be implemented here.</p>
      </section>
    </main>
  </div>
</template>

<style scoped>
.app-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.app-header {
  text-align: center;
  margin-bottom: 3rem;
  border-bottom: 2px solid #e0e0e0;
  padding-bottom: 2rem;
}

.app-header h1 {
  color: #2c3e50;
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
}

.subtitle {
  color: #7f8c8d;
  font-size: 1.1rem;
}

.main-content {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.connection-test, .coming-soon {
  background: #f8f9fa;
  padding: 2rem;
  border-radius: 8px;
  border: 1px solid #e9ecef;
}

.connection-test h2, .coming-soon h2 {
  color: #495057;
  margin-bottom: 1rem;
}

.test-button {
  background: #007bff;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.2s;
}

.test-button:hover:not(:disabled) {
  background: #0056b3;
}

.test-button:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

.status-message {
  margin-top: 1rem;
  padding: 0.75rem;
  border-radius: 4px;
  background: #d4edda;
  border: 1px solid #c3e6cb;
}

.api-response {
  margin-top: 1rem;
  padding: 1rem;
  background: white;
  border-radius: 4px;
  border: 1px solid #dee2e6;
}

.api-response h3 {
  margin-bottom: 0.5rem;
  color: #495057;
}

.response-content p {
  margin: 0.25rem 0;
  color: #6c757d;
}

.coming-soon {
  text-align: center;
  color: #6c757d;
}
</style>
