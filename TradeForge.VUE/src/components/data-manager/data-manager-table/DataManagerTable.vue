<style scoped src="./DataManagerTable.css"/>

<template>
  <div class="table-wrapper">
    <table class="custom-table">
      <thead class="table-head">
      <tr>
        <th
          v-for="header in headers"
          :key="header.key"
          @click="toggleSort(header.key)"
          class="sortable-header"
        >
          {{ header.title }}
          <span class="sort-indicator">
            <template v-if="sortBy === header.key">
              {{ sortDirection === 'asc' ? '↑' : '↓' }}
            </template>
          </span>
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
      <tr v-else v-for="ticker in sortedTickers" :key="ticker.id" class="data-row">
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


<script setup lang="ts">
import { ref, computed } from 'vue';
import type { Ticker } from '@/types/Ticker.ts';

interface Props {
  tickers: Ticker[];
  loading: boolean;
}

const props = defineProps<Props>();

const headers = [
  { title: 'Symbol', key: 'symbol', sortable: true },
  { title: 'Instrument', key: 'instrument', sortable: true },
  { title: 'Category', key: 'category', sortable: true },
  { title: 'Timeframe', key: 'timeFrame', sortable: true },
  { title: 'Date From', key: 'dateFrom', sortable: true },
  { title: 'Date To', key: 'dateTo', sortable: true },
  { title: 'Records', key: 'totalRecords', sortable: true },
];

const sortBy = ref<string | null>(null);
const sortDirection = ref<'asc' | 'desc'>('asc');

const toggleSort = (key: string) => {
  if (sortBy.value === key) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
  } else {
    sortBy.value = key;
    sortDirection.value = 'asc';
  }
};

const sortedTickers = computed(() => {
  if (!sortBy.value) return props.tickers;

  return [...props.tickers].sort((a, b) => {
    const key = sortBy.value as keyof Ticker;
    let aVal = a[key];
    let bVal = b[key];

    // Handle dates
    if (key === 'dateFrom' || key === 'dateTo') {
      aVal = new Date(aVal as Date).getTime();
      bVal = new Date(bVal as Date).getTime();
    }

    // Handle numbers
    if (typeof aVal === 'number' && typeof bVal === 'number') {
      return sortDirection.value === 'asc' ? aVal - bVal : bVal - aVal;
    }

    // Handle strings
    const aStr = String(aVal).toLowerCase();
    const bStr = String(bVal).toLowerCase();

    if (sortDirection.value === 'asc') {
      return aStr.localeCompare(bStr);
    } else {
      return bStr.localeCompare(aStr);
    }
  });
});

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString();
};
</script>
