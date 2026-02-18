<template>
  <div class="strategy-report-tab">
    <!-- Performance -->
    <v-row no-gutters class="mb-3">
      <v-col cols="12">
        <StrategyPerformanceReport
          v-if="data.performance"
          :data="data.performance"
        />
        <div v-else class="empty-section">No performance data</div>
      </v-col>
    </v-row>

    <!-- Stats & Ratios -->
    <v-row no-gutters class="mb-3">
      <v-col cols="12">
        <StrategyStatsReport
          v-if="data.stats && data.ratio"
          :stats="data.stats"
          :ratio="data.ratio"
        />
        <div v-else class="empty-section">No strategy stats data</div>
      </v-col>
    </v-row>

    <!-- Trades -->
    <v-row no-gutters class="mb-3">
      <v-col cols="12">
        <TradeStatsReport
          v-if="data.trades"
          :data="data.trades"
        />
        <div v-else class="empty-section">No trades data</div>
      </v-col>
    </v-row>

    <!-- Monthly Performance -->
    <v-row no-gutters>
      <v-col cols="12">
        <div class="monthly-wrapper">
          <MonthlyPerformanceTable
            v-if="data.months"
            :data="data.months"
          />
          <div v-else class="empty-section">No monthly data</div>
        </div>
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
import type { StrategyReportData } from '@/types/strategy/StategyReport.interface';
import StrategyPerformanceReport from '@/components/backtest/strategy/strategy-performance-report/StrategyPerformanceReport.vue';
import StrategyStatsReport from '@/components/backtest/strategy/strategy-stats-report/StrategyStatsReport.vue';
import TradeStatsReport from '@/components/backtest/strategy/trades-stats-report/TradeStatsReport.vue';
import MonthlyPerformanceTable from '@/components/backtest/monthly-perfomance-table/MonthlyPerformanceTable.vue';

interface Props {
  data: StrategyReportData;
}

defineProps<Props>();
</script>

<style scoped>
.strategy-report-tab {
  display: flex;
  flex-direction: column;
  width: 100%;
  padding: 0.75rem;
}

.monthly-wrapper {
  border: thin solid rgba(var(--v-theme-on-surface), 0.12);
  border-radius: 0.25rem;
  overflow: hidden;
  min-height: 12rem;
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
