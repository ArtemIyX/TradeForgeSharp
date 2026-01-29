<template>
  <v-card class="pa-4 d-flex flex-column" style="height: 100%;">
    <!-- Ticker Information -->
    <v-card-title class="text-h5 pb-2 flex-shrink-0">
      <back-button/>
      <span class="text-accent">{{ ticker.ticker }}</span>
      <span class="text-subtitle-2 text-medium-emphasis">{{ ticker.name }}</span>
    </v-card-title>

    <v-card-subtitle class="pb-4 flex-shrink-0">
      <v-chip size="small" class="mr-2">
        <span class="text-success">{{ ticker.category }}</span>
      </v-chip>
      <v-chip size="small" class="mr-2">
        <span class="text-warning">{{ ticker.type }}</span>
      </v-chip>
      <span class="text-medium-emphasis">{{ ticker.description }}</span>
    </v-card-subtitle>

    <v-tabs color="primary" v-model="tab" class="flex-shrink-0">
      <v-tab value="ohlc">OHLC Table</v-tab>
      <v-tab value="chart">Candlestick Chart</v-tab>
    </v-tabs>

    <v-divider class="flex-shrink-0"></v-divider>

    <v-tabs-window v-model="tab" class="flex-grow-1" style="min-height: 0;">
      <v-tabs-window-item value="ohlc" class="h-100">
        <div class="d-flex justify-center align-center h-100" v-if="loading">
          <v-progress-circular indeterminate />
        </div>
        <OHLCTable
          v-else
          :ohlc-data="ohlcData"
          :rows-per-page="10"
          :decimal-precision="2"
          :ticker="ticker"
          class="h-100"
        />
      </v-tabs-window-item>

      <v-tabs-window-item value="chart" class="h-100">
        <div class="d-flex justify-center align-center h-100" v-if="loading">
          <v-progress-circular indeterminate />
        </div>
        <v-chart
          v-else
          :option="chartOption"
          :loading="loading"
          autoresize
          style="width: 100%; height: 100%;"
        />
      </v-tabs-window-item>
    </v-tabs-window>
  </v-card>
</template>

<script setup>

import OHLCTable from "@/components/history/OHLCTable.vue";

import {ref, onMounted, computed} from "vue";
import {useRoute} from 'vue-router';
import BackButton from "@/components/BackButton.vue";

const route = useRoute();

const tab = ref();
const loading = ref(true);
const ohlcData = ref([]);
const ticker = ref({
  id: 10,
  ticker: "USOIL",
  category: "Commodity",
  name: "WTI Crude",
  type: "CFD",
  description: "US oil benchmark",
});

const fetchData = async () => {
  loading.value = true
  try {
    const res = await fetch('/dummy/ohlc.json')
    await new Promise(resolve => setTimeout(resolve, 1000))
    ohlcData.value = await res.json()
  } catch (e) {
    console.error('Failed to load local JSON:', e)
    ohlcData.value = []
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  ticker.id = route.params.id;
  fetchData();
})


const chartOption = computed(() => {
  const categoryData = ohlcData.value.map(item => {
    const date = new Date(item.timestamp)
    return date.toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  })

  // ECharts candlestick format: [open, close, low, high]
  const candlestickData = ohlcData.value.map(item => [
    item.open,
    item.close,
    item.low,
    item.high
  ])

  const volumeData = ohlcData.value.map((item, index) => ({
    value: item.volume,
    itemStyle: {
      color: item.close >= item.open ? '#26a69a' : '#ef5350'
    }
  }))

  return {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      },
      formatter: (params) => {
        const candlestick = params.find(p => p.seriesType === 'candlestick')
        const volume = params.find(p => p.seriesName === 'Volume')

        if (!candlestick) return ''

        const [open, close, low, high] = candlestick.data
        return `
          <strong>${candlestick.axisValue}</strong><br/>
          Open: ${open.toFixed(2)}<br/>
          Close: ${close.toFixed(2)}<br/>
          Low: ${low.toFixed(2)}<br/>
          High: ${high.toFixed(2)}<br/>
          ${volume ? `Volume: ${volume.data.value.toLocaleString()}` : ''}
        `
      }
    },
    legend: {
      data: ['Price', 'Volume'],
      top: 10,
      textStyle: {
        color: '#fff'  // Legend text color
      }
    },
    grid: [
      {
        left: '10%',
        right: '10%',
        top: 60,
        height: '50%'
      },
      {
        left: '10%',
        right: '10%',
        top: '70%',
        height: '16%'
      }
    ],
    xAxis: [
      {
        type: 'category',
        data: categoryData,
        gridIndex: 0,
        axisLine: { lineStyle: { color: '#777' } },
        axisLabel: {
          show: false,
          color: '#fff'  // X-axis labels (if shown)
        }
      },
      {
        type: 'category',
        data: categoryData,
        gridIndex: 1,
        axisLine: { lineStyle: { color: '#777' } },
        axisLabel: {
          rotate: 45,
          fontSize: 10,
          color: '#fff'  // X-axis labels
        }
      }
    ],
    yAxis: [
      {
        scale: true,
        gridIndex: 0,
        splitArea: { show: true },
        axisLine: { lineStyle: { color: '#777' } },
        axisLabel: {
          color: '#fff'  // Y-axis labels
        }
      },
      {
        scale: true,
        gridIndex: 1,
        splitNumber: 2,
        axisLabel: {
          show: false,
          color: '#fff'  // Y-axis labels (if shown)
        },
        axisLine: { show: false },
        axisTick: { show: false },
        splitLine: { show: false }
      },
    ],
    dataZoom: [
      {
        type: 'inside',
        xAxisIndex: [0, 1],
        start: 0,
        end: 100
      },
      {
        show: true,
        xAxisIndex: [0, 1],
        type: 'slider',
        bottom: 10,
        start: 0,
        end: 100
      }
    ],
    series: [
      {
        name: 'Price',
        type: 'candlestick',
        data: candlestickData,
        xAxisIndex: 0,
        yAxisIndex: 0,
        itemStyle: {
          color: '#26a69a',      // bullish (close > open)
          color0: '#ef5350',     // bearish (close < open)
          borderColor: '#26a69a',
          borderColor0: '#ef5350'
        }
      },
      {
        name: 'Volume',
        type: 'bar',
        data: volumeData,
        xAxisIndex: 1,
        yAxisIndex: 1
      }
    ]
  }
})

</script>
