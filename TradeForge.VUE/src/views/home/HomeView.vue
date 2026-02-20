<template>
  <v-container fluid>
    <v-row>
      <v-col>
        <TradeAnalyticsReport :data="analyticsData" :loading="isLoading"/>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import {ref, onMounted} from 'vue'
import {tradeAnalyticsDummy} from './tradeanalyticsdummy.ts'
import type {TradeAnalyticsData} from '@/types/strategy/TradeAnalyticsReport.interface'

import TradeAnalyticsReport
  from "@/components/backtest/strategy/trade-analytics-report/TradeAnalyticsReport.vue";

const isLoading = ref(false)
const analyticsData = ref<TradeAnalyticsData | null>(null)

const fetchAnalyticsData = async () => {
  isLoading.value = true
  try {
    // Replace with your actual API call:
    // analyticsData.value = await backtestService.getAnalytics(strategyId)

    // Simulated delay with dummy data
    await new Promise(resolve => setTimeout(resolve, 1000))
    analyticsData.value = tradeAnalyticsDummy
    console.log("hello", analyticsData.value );
  } finally {
    isLoading.value = false
  }
}

onMounted (() => {
  fetchAnalyticsData();
})
</script>
