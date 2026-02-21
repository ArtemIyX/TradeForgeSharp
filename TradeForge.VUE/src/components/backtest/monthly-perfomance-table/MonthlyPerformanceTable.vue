<style scoped src="./MonthlyPerformanceTable.css">

</style>

<template>
  <div class="table-container">
    <div class="table-title">Monthly Performance ($)</div>

    <div class="table-wrapper">
      <table class="custom-table">
        <thead class="table-head">
        <tr>
          <th class="th-year">Year</th>
          <th v-for="month in MONTHS" :key="month">{{ month }}</th>
          <th class="th-ytd">YTD</th>
        </tr>
        </thead>
        <tbody>
        <tr v-if="!rows.length">
          <td :colspan="15" class="empty-state">No performance data available</td>
        </tr>
        <tr
          v-for="row in rows"
          :key="row.year"
          class="data-row"
        >
          <td class="td-year">{{ row.year }}</td>
          <td
            v-for="(profit, i) in row.profits"
            :key="i"
            :class="profitClass(profit)"
          >
            {{ formatProfit(profit) }}
          </td>
          <td :class="['ytd-cell', profitClass(row.ytd)]">
            {{ formatProfit(row.ytd) }}
          </td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { MonthlyReportMap } from '@/types/strategy/MonthlyReport.interface.ts';

interface Props {
  data: MonthlyReportMap;
}

const props = defineProps<Props>();

const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

const rows = computed(() => {
  return Array.from(props.data.data.entries())
    .sort(([a], [b]) => b - a) // newest year first
    .map(([year, item]) => {
      const profits = [...item.profits]; // ensure 12 elements
      const ytd = profits.reduce((sum, v) => sum + (v ?? 0), 0);
      return { year, profits, ytd };
    });
});

function profitClass(value: number): string {
  if (value < 0) return 'profit-negative';
  if (value === 0) return 'profit-zero';
  return 'profit-positive';
}

function formatProfit(value: number): string {
  if (value === 0) return '0';
  return +value.toFixed(1) + '';
}
</script>
