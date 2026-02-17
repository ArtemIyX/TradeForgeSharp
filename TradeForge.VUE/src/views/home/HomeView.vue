<style scoped>


</style>
<template>
  <v-container>
    <v-row>
      <OhlcTable :items="ohlcItems"/>
    </v-row>
  </v-container>
  <v-container>
    <v-row>
      <v-col cols="12">
        <h2>Example 1: Trade Number vs Cumulative Profit</h2>
        <GLineChart
          :data="tradeData"
          :x-formatter="(v) => `Trade #${v}`"
          :y-formatter="(v) => `$${v.toFixed(2)}`"
          line-color="#4CAF50"
          :fill-gradient="true"
          title="Cumulative Profit by Trade"
          x-axis-name="Trade Number"
          y-axis-name="Profit ($)"
          :smoothed="true"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h2>Example 2: Date vs Balance</h2>
        <GLineChart
          :data="balanceData"
          :x-formatter="formatDate"
          :y-formatter="(v) => `$${v.toLocaleString()}`"
          line-color="#FF9800"
          :fill-gradient="false"
          title="Account Balance Over Time"
          x-axis-name="Date"
          y-axis-name="Balance ($)"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h2>Example 3: Date vs Asset Price</h2>
        <GLineChart
          :data="priceData"
          :x-formatter="formatDate"
          :y-formatter="(v) => `$${v.toFixed(2)}`"
          line-color="#2196F3"
          :fill-gradient="true"
          title="Asset Price History"
          x-axis-name="Date"
          y-axis-name="Price ($)"
          height="500px"
          :zoom="true"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h2>Example 4: Multi line chart</h2>
        <GMultiLineChart
          :series="strategyData"
          title="Strategy Performance Comparison"
          xAxisName="Month"
          yAxisName="Return (%)"
          :yFormatter="(v) => v.toFixed(2) + '$'"
          height="500px"
          :zoom="true"
          :smoothed="false"
        />
      </v-col>
    </v-row>
  </v-container>

</template>
<script setup lang="ts">
import {onMounted, ref} from 'vue'
import GLineChart, {type LineChartDataPoint} from "@/components/shared/charts/GLineChart.vue";
import GMultiLineChart from "@/components/shared/charts/GMultiLineChart.vue";

import strategiesJSON from './strategies.json'
import OhlcTable from "@/components/data-manager/ohlc-table/OhlcTable.vue";

import ohlcDummy from "./ohlc-dummy.json";
import {
  convertToOhlc,
  convertToOhlcArray,
  type OhlcData,
  type OhlcResponseModel
} from "@/types/OhlcData.ts";

const strategyData = ref([]);
const ohlcItems = ref<OhlcData[]>([]);

onMounted(() => {
  strategyData.value = strategiesJSON;
  ohlcItems.value = convertToOhlcArray(ohlcDummy as OhlcResponseModel[]);
})


// Example 1: Trade number to cumulative profit
const tradeData = ref<LineChartDataPoint[]>([
  {x: 1, y: 150},
  {x: 2, y: 280},
  {x: 3, y: 220},
  {x: 4, y: 390},
  {x: 5, y: 520},
  {x: 6, y: 480},
  {x: 7, y: 650},
  {x: 8, y: 780},
  {x: 9, y: 920},
  {x: 10, y: 1100}
])

// Example 2: Date to balance
const balanceData = ref<LineChartDataPoint[]>([
  {x: '2024-01-01', y: 10000},
  {x: '2024-02-01', y: 12500},
  {x: '2024-03-01', y: 11800},
  {x: '2024-04-01', y: 15200},
  {x: '2024-05-01', y: 17600},
  {x: '2024-06-01', y: 16900},
  {x: '2024-07-01', y: 19300},
  {x: '2024-08-01', y: 21500}
])

// Example 3: Date to asset price
const priceData = ref<LineChartDataPoint[]>([
  {x: '2024-01-01', y: 45.23},
  {x: '2024-01-08', y: 47.56},
  {x: '2024-01-15', y: 46.12},
  {x: '2024-01-22', y: 49.87},
  {x: '2024-01-29', y: 52.34},
  {x: '2024-02-05', y: 51.23},
  {x: '2024-02-12', y: 54.67},
  {x: '2024-02-19', y: 56.89},
  {x: '2024-02-26', y: 55.45}
])

const formatDate = (value: any): string => {
  const date = new Date(value)
  return date.toLocaleDateString('en-US', {month: 'short', day: 'numeric'})
}

</script>

