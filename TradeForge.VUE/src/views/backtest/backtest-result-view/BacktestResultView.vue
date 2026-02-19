<template>
  <v-card class="scrollable-card">
    <!-- loading overlay -->
    <v-overlay
      :model-value="bLoading"
      contained
      class="d-flex align-center justify-center"
    >
      <v-progress-circular indeterminate color="warning" size="48"/>
    </v-overlay>
    <v-card-title class="d-flex align-center gap-2">
      <span class="text-subtitle-1 font-weight-medium">Backtest Report</span>
      <v-chip size="x-small" color="warning" variant="outlined" label style="margin-left: 0.5rem">
        #{{ id }}
      </v-chip>
    </v-card-title>
    <v-tabs v-model="activeTab" density="compact">
      <v-tab value="stats" prepend-icon="mdi-file-chart-outline">Stats</v-tab>
      <v-tab value="trades" prepend-icon="mdi-format-list-bulleted">List of trades</v-tab>
    </v-tabs>

    <v-divider/>

    <v-window v-model="activeTab">
      <v-window-item value="stats">
       <StrategyReportTab :data="report"/>
      </v-window-item>

      <v-window-item value="trades">

      </v-window-item>
    </v-window>
  </v-card>
</template>

<style scoped>
.scrollable-card {
  height: 100vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.scrollable-card :deep(.v-window) {
  flex: 1;
  min-height: 0;
  overflow: hidden;
}

.scrollable-card :deep(.v-window__container) {
  height: 100%;
}

.scrollable-card :deep(.v-window-item) {
  height: 100%;
  overflow-y: auto;
}
</style>
<script setup lang="ts">
import {useRoute} from 'vue-router'
import OhlcTable from "@/components/data-manager/ohlc-table/OhlcTable.vue";
import GCandlestickChart from "@/components/shared/charts/GCandlestickChart.vue";
import {onMounted, ref} from "vue";

import type {StrategyReportData} from "@/types/strategy/StategyReport.interface.ts";
import type {MonthlyReportItem} from "@/types/strategy/MonthlyReport.interface.ts";
import StrategyReportTab
  from "@/components/backtest/strategy/strategy-report-tab/StrategyReportTab.vue";

const route = useRoute()
const id = route.params.id // string

const activeTab = ref<string>('stats');

const report = ref<StrategyReportData>({});

const bLoading = ref<boolean>(false);

const fetchReport = async (reportId) => {
  bLoading.value = true;
  await new Promise(resolve => setTimeout(resolve, 250));
  report.value = {
    performance: {
      totaProfit: 6213.5,
      profitInPips: 6213.5,
      yearlyAvgProfit: 388.31,
      yearlyAvgProfitPercent: 3.88,
      cagr: 3.07,

      numberOfTrades: 662,
      profitFactor: 1.36,
      returnDdRatio: 5.37,
      winningPercentage: 38.37,

      drawdown: 1156.4,
      drawdownPercent: 8.13,
      dailyAvgProfit: 1.01,
      monthlyAvgProfit: 30.76,
      avgTradeProfit: 9.39,
    },
    ratio: {
      sharpeRatio: 1.82, sortinoRatio: 2.41, calmarRatio: 0.94,
      sterlingRatio: 1.13, omegaRatio: 1.57, marRatio: 0.87,
    },
    stats: {
      winLossRatio: 0.2, payoutRatio: 0.2, avgBarsInTrade: 19.65,
      ahpr: 3.45, zScore: 1, zProbability: 24.51,
      expectancy: 9.39, deviation: 77.9, exposure: 8.75,
      stagnationInDays: 5, stagnationInPercent: 37.84,

    },
    trades: {
      wins: 254, losses: 408, canceledOrExpired: 0,
      grossProfit: 23316.1, grossLoss: -17102.6,
      avgWin: 91.8, avgLoss: -41.92,
      largestWin: 175, largestLoss: -46,
      maxConsWins: 7, maxConsLosses: 14,
      avgConsWins: 1.58, avgConsLosses: 2.53,
      avgBarsInWins: 32.44, avgBarsInLosses: 11.69,
    },
    months: {
      data: new Map<number, MonthlyReportItem>([
        [2024, {profits: [105.1, 100, 100.2, 0, 0, 0, 0, 0, 0, 0, 0, 0]}],
        [2023, {profits: [253.3, 79.6, -184, -92, 61.8, -80.4, 3.6, 35.5, -73.5, -9, 14, 206.2]}],
        [2022, {profits: [261.6, 83, -7.4, -11.9, -92, -47.1, -39.7, -139.5, -145.2, 103.1, -138, -144.3]}],
        [2021, {profits: [21.2, -92, 183.9, -45.6, -160.1, -221.8, 145.4, 29.8, 241.3, 88.5, 155.2, -90.5]}],
        [2020, {profits: [-9.2, 144.6, 304, -59.4, 123.3, -24.2, -138, -55, 91.8, -46, 37, -28.3]}],
        [2019, {profits: [-144.8, -12.9, 129, 203.5, 258, 113.8, -50.3, -144.2, 288.7, -36.3, 196.8, -40]}],
        [2018, {profits: [105.1, 100, 100.2, 0, 0, 0, 0, 0, 0, 0, 0, 0]}],
        [2017, {profits: [253.3, 79.6, -184, -92, 61.8, -80.4, 3.6, 35.5, -73.5, -9, 14, 206.2]}],
        [2016, {profits: [261.6, 83, -7.4, -11.9, -92, -47.1, -39.7, -139.5, -145.2, 103.1, -138, -144.3]}],
        [2015, {profits: [21.2, -92, 183.9, -45.6, -160.1, -221.8, 145.4, 29.8, 241.3, 88.5, 155.2, -90.5]}],
        [2014, {profits: [-9.2, 144.6, 304, -59.4, 123.3, -24.2, -138, -55, 91.8, -46, 37, -28.3]}],
        [2013, {profits: [-144.8, -12.9, 129, 203.5, 258, 113.8, -50.3, -144.2, 288.7, -36.3, 196.8, -40]}],
      ])
    }
  }
  bLoading.value = false;
}

onMounted(() => {
  if (id) {
    fetchReport(id);
  }

});

</script>
