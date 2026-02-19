<template>
  <div class="equity-tab">
    <v-tabs v-model="activeTab" density="compact" class="equity-tab__tabs">
      <v-tab value="close-by-close">Close by Close</v-tab>
      <v-tab value="by-time">By Time</v-tab>
      <v-tab value="open-equity">Open Equity</v-tab>
    </v-tabs>

    <v-divider />

    <v-window v-model="activeTab" class="equity-tab__window">

      <!-- Tab 1: Equity close by close -->
      <v-window-item value="close-by-close" class="equity-tab__item">
        <GLineChart
          :data="closeByCloseData"
          :x-formatter="(v) => `#${v}`"
          :y-formatter="formatBalance"
          line-color="#4CAF50"
          :fill-gradient="true"
          :zoom="true"
          x-axis-name="Trade Number"
          y-axis-name="Balance ($)"
          height="100%"
        />
      </v-window-item>

      <!-- Tab 2: Equity by time -->
      <v-window-item value="by-time" class="equity-tab__item">
        <GLineChart
          :data="byTimeData"
          :x-formatter="formatDate"
          :y-formatter="formatBalance"
          line-color="#2196F3"
          :fill-gradient="false"
          :zoom="true"
          x-axis-name="Date"
          y-axis-name="Balance ($)"
          height="100%"
        />
      </v-window-item>

      <!-- Tab 3: Open equity (equity + balance by time) -->
      <v-window-item value="open-equity" class="equity-tab__item">
        <GMultiLineChart
          :series="openEquitySeries"
          :x-formatter="formatDate"
          :y-formatter="formatBalance"
          :zoom="true"
          :show-legend="true"
          x-axis-name="Date"
          y-axis-name="Value ($)"
          height="100%"
        />
      </v-window-item>

    </v-window>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import GLineChart, { type LineChartDataPoint } from '@/components/shared/charts/GLineChart.vue';
import GMultiLineChart, { type SeriesData } from '@/components/shared/charts/GMultiLineChart.vue';

export interface EquityCloseByClosePoint {
  tradeNumber: number;
  balance: number;
}

export interface EquityByTimePoint {
  date: string; // ISO date string
  balance: number;
}

export interface OpenEquityPoint {
  date: string; // ISO date string
  equity: number;  // unrealized P&L included
  balance: number; // closed trades only
}

interface Props {
  closeByClose: EquityCloseByClosePoint[];
  byTime: EquityByTimePoint[];
  openEquity: OpenEquityPoint[];
}

const props = defineProps<Props>();

const activeTab = ref('close-by-close');

// ── Data adapters ─────────────────────────────────────────────────────────────

const closeByCloseData = computed<LineChartDataPoint[]>(() =>
  props.closeByClose.map(p => ({ x: p.tradeNumber, y: p.balance }))
);

const byTimeData = computed<LineChartDataPoint[]>(() =>
  props.byTime.map(p => ({ x: p.date, y: p.balance }))
);

const openEquitySeries = computed<SeriesData[]>(() => [
  {
    name: 'Equity',
    color: '#FF9800',
    data: props.openEquity.map(p => ({ x: p.date, y: p.equity })),
  },
  {
    name: 'Balance',
    color: '#2196F3',
    data: props.openEquity.map(p => ({ x: p.date, y: p.balance })),
  },
]);

// ── Formatters ────────────────────────────────────────────────────────────────

function formatBalance(value: number): string {
  const abs = Math.abs(value);
  const formatted = `$ ${abs.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  return value < 0 ? `- ${formatted}` : formatted;
}

function formatDate(value: any): string {
  const date = new Date(value);
  return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
}
</script>

<style scoped>
.equity-tab {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  border: thin solid rgba(var(--v-theme-on-surface), 0.12);
  border-radius: 0.25rem;
  overflow: hidden;
}

.equity-tab__tabs {
  flex-shrink: 0;
  background-color: rgba(var(--v-theme-on-surface), 0.02);
}

.equity-tab__window {
  flex: 1;
  min-height: 0;
  padding: 0.75rem;
  display: flex;
  flex-direction: column;
}

.equity-tab__item {
  height: 100%;
}
</style>
