<script setup lang="ts">
import BidCalculatorView from './modules/BidCalculator/views/BidCalculatorView.vue'
</script>

<template>
  <div id="app">
    <main>
      <BidCalculatorView />
    </main>
  </div>
</template>

<style scoped>
#app {
  min-height: 100vh;
  background-color: #f5f5f5;
}

main {
  padding: 2rem 0;
}
</style>
