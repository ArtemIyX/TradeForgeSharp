<style scoped src="./DataManagerTable.css"/>

<template>
  <div class="table-container">
    <div class="filters-header">
      <v-btn
        @click="filtersExpanded = !filtersExpanded"
        variant="text"
        size="small"
        prepend-icon="mdi-filter-variant"
      >
        Filters
        <v-icon :icon="filtersExpanded ? 'mdi-chevron-up' : 'mdi-chevron-down'" end />
      </v-btn>

      <v-chip
        v-if="hasActiveFilters"
        size="small"
        color="primary"
        class="filter-badge"
      >
        {{ activeFiltersCount }}
      </v-chip>
    </div>

    <v-expand-transition>
      <div v-show="filtersExpanded" class="filters-section">
        <v-text-field
          v-model="nameFilter"
          label="Search by Symbol or Instrument"
          density="compact"
          clearable
          hide-details
          class="filter-field"
        />

        <v-select
          v-model="categoryFilter"
          :items="categoryOptions"
          label="Category"
          density="compact"
          clearable
          hide-details
          class="filter-field"
        />

        <v-select
          v-model="timeFrameFilter"
          :items="timeFrameOptions"
          label="Timeframes"
          density="compact"
          multiple
          chips
          clearable
          hide-details
          class="filter-field"
        />

        <v-btn
          @click="resetFilters"
          variant="outlined"
          size="small"
          class="reset-btn"
        >
          Reset
        </v-btn>
      </div>
    </v-expand-transition>

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
        <tr v-else v-for="ticker in filteredAndSortedTickers" :key="ticker.id" class="data-row">
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
  </div>
</template>


<script setup lang="ts">
import { ref, computed } from 'vue';
import type { Ticker } from '@/types/Ticker.ts';
import { TimeFrame } from '@/types/Ticker.ts';

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
const nameFilter = ref('');
const categoryFilter = ref<string | null>(null);
const timeFrameFilter = ref<string[]>([]);
const filtersExpanded = ref(false);

const hasActiveFilters = computed(() => {
  return !!(nameFilter.value ||
    (categoryFilter.value && categoryFilter.value !== 'None') ||
    timeFrameFilter.value.length > 0);
});

const activeFiltersCount = computed(() => {
  let count = 0;
  if (nameFilter.value) count++;
  if (categoryFilter.value && categoryFilter.value !== 'None') count++;
  if (timeFrameFilter.value.length > 0) count++;
  return count;
});

const categoryOptions = computed(() => {
  const categories = new Set(props.tickers.map(t => t.category));
  return ['None', ...Array.from(categories)].sort();
});

const timeFrameOptions = computed(() => {
  return Object.values(TimeFrame);
});

const resetFilters = () => {
  nameFilter.value = '';
  categoryFilter.value = null;
  timeFrameFilter.value = [];
};

const filteredTickers = computed(() => {
  let result = props.tickers;

  // Filter by name (symbol or instrument)
  if (nameFilter.value) {
    const search = nameFilter.value.toLowerCase();
    result = result.filter(t =>
      t.symbol.toLowerCase().includes(search) ||
      t.instrument.toLowerCase().includes(search)
    );
  }

  // Filter by category
  if (categoryFilter.value && categoryFilter.value !== 'None') {
    result = result.filter(t => t.category === categoryFilter.value);
  }

  // Filter by timeframes
  if (timeFrameFilter.value.length > 0) {
    result = result.filter(t => timeFrameFilter.value.includes(t.timeFrame));
  }

  return result;
});

const filteredAndSortedTickers = computed(() => {
  if (!sortBy.value) return filteredTickers.value;

  return [...filteredTickers.value].sort((a, b) => {
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

const toggleSort = (key: string) => {
  if (sortBy.value === key) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
  } else {
    sortBy.value = key;
    sortDirection.value = 'asc';
  }
};

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString();
};
</script>
