<template>
  <div class="strategy-report-tab">

    <!-- Always on top: Performance -->
    <div class="mb-3">
      <StrategyPerformanceReport
        v-if="data.performance"
        :data="data.performance"
      />
      <div v-else class="empty-section">No performance data</div>
    </div>

    <!-- Tabs: Stats / Trades / Monthly -->
    <v-tabs v-model="activeTab" density="compact" class="report-tabs">
      <v-tab value="stats">Stats</v-tab>
      <v-tab value="monthly">Monthly</v-tab>
    </v-tabs>

    <v-tabs-window v-model="activeTab" class="tabs-window">

      <v-tabs-window-item value="stats">
        <div class="stats-combined">
          <StrategyStatsReport
            v-if="data.stats && data.ratio"
            :stats="data.stats"
            :ratio="data.ratio"
          />
          <div v-else class="empty-section">No strategy stats data</div>

          <TradeStatsReport
            v-if="data.trades"
            :data="data.trades"
          />
          <div v-else class="empty-section">No trades data</div>
        </div>
      </v-tabs-window-item>

      <v-tabs-window-item value="monthly">
        <div class="monthly-wrapper">
          <MonthlyPerformanceTable
            v-if="data.months"
            :data="data.months"
          />
          <div v-else class="empty-section">No monthly data</div>
        </div>
      </v-tabs-window-item>

    </v-tabs-window>

  </div>
</template>



<style scoped>

.strategy-report-tab {
  height: 100%;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* The inner tabs window fills remaining space */
.tabs-window {
  flex: 1;
  min-height: 0;
  overflow: hidden;
}

/* Target the inner vuetify wrappers */
.tabs-window :deep(.v-tabs-window__container) {
  height: 100%;
}

.tabs-window :deep(.v-window-item) {
  height: 100%;
  overflow-y: auto;
}

/* your existing styles */
.monthly-wrapper {
  border: thin solid rgba(var(--v-theme-on-surface), 0.12);
}

.empty-section {
  padding: 1.5rem;
  text-align: center;
  font-size: 0.8125rem;
  color: rgba(var(--v-theme-on-surface), 0.38);
  font-style: italic;
  border: thin solid rgba(var(--v-theme-on-surface), 0.12);
  border-radius: 0.25rem;
}
</style>

<script setup lang="ts">
import { ref } from 'vue';
import type { StrategyReportData } from '@/types/strategy/StategyReport.interface';
import StrategyPerformanceReport from '@/components/backtest/strategy/strategy-performance-report/StrategyPerformanceReport.vue';
import StrategyStatsReport from '@/components/backtest/strategy/strategy-stats-report/StrategyStatsReport.vue';
import TradeStatsReport from '@/components/backtest/strategy/trades-stats-report/TradeStatsReport.vue';
import MonthlyPerformanceTable from '@/components/backtest/monthly-perfomance-table/MonthlyPerformanceTable.vue';

interface Props {
  data: StrategyReportData;
}

defineProps<Props>();

const activeTab = ref('stats');
</script>
