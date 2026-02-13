<template>
  <div class="table-wrapper">
    <table class="custom-table">
      <thead class="table-head">
      <tr>
        <th v-for="header in headers" :key="header.key">
          {{ header.title }}
        </th>
      </tr>
      </thead>
      <tbody>
      <tr v-if="loading">
        <td :colspan="headers.length" class="text-center">
          <v-progress-circular indeterminate size="24"/>
        </td>
      </tr>
      <tr v-else-if="!tickers.length">
        <td :colspan="headers.length" class="text-center empty-state">
          No data available
        </td>
      </tr>
      <tr v-else v-for="ticker in tickers" :key="ticker.id" class="data-row">
        <td>{{ ticker.symbol }}</td>
        <td>{{ ticker.instrument }}</td>
        <td>{{ ticker.category }}</td>
        <td>{{ ticker.timeFrame }}</td>
        <td>{{ formatDate(ticker.dateFrom) }}</td>
        <td>{{ formatDate(ticker.dateTo) }}</td>
        <td>{{ ticker.totalRecords.toLocaleString() }}</td>
      </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.table-wrapper {
  width: 100%;
  height: 100%;
  overflow-y: auto;
}

.custom-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}

.table-head {
  position: sticky;
  top: 0;
  z-index: 10;
  background-color: rgb(var(--v-theme-surface));
}

.table-head th {
  text-align: left;
  padding: 0.75rem 1rem;
  font-weight: 500;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.0892857143em;
  color: rgba(var(--v-theme-on-surface), 0.6);
  background-color: rgb(var(--v-theme-surface));
  border-bottom: thin solid rgba(var(--v-theme-on-surface), 0.12);
  white-space: nowrap;
}

.custom-table tbody td {
  padding: 0.5rem 1rem;
  border-bottom: thin solid rgba(var(--v-theme-on-surface), 0.12);
  color: rgba(var(--v-theme-on-surface), 0.87);
}

.data-row {
  transition: background-color 0.2s;
}

.data-row:hover {
  background-color: rgba(var(--v-theme-on-surface), 0.04);
  cursor: pointer;
}

.data-row:nth-child(even) {
  background-color: rgba(var(--v-theme-on-surface), 0.02);
}

.data-row:nth-child(even):hover {
  background-color: rgba(var(--v-theme-on-surface), 0.06);
}

.text-center {
  text-align: center;
}

.empty-state {
  padding: 3rem 1rem;
  color: rgba(var(--v-theme-on-surface), 0.38);
  font-style: italic;
}
</style>

<script setup lang="ts">
import type {Ticker} from '@/types/Ticker.ts';

interface Props {
  tickers: Ticker[];
  loading: boolean;
}

defineProps<Props>();

const headers = [
  {title: 'Symbol', key: 'symbol', sortable: true},
  {title: 'Instrument', key: 'instrument', sortable: true},
  {title: 'Category', key: 'category', sortable: true},
  {title: 'Timeframe', key: 'timeFrame', sortable: true},
  {title: 'Date From', key: 'dateFrom', sortable: true},
  {title: 'Date To', key: 'dateTo', sortable: true},
  {title: 'Records', key: 'totalRecords', sortable: true},
];

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString();
};
</script>
