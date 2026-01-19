<template>
  <v-chart class="chart" :option="chartOption" autoresize />
</template>
<script setup>
import { ref } from 'vue'

const chartOption = ref({
  xAxis: {
    type: 'category',
    data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri']
  },
  yAxis: {
    type: 'value'
  },
  series: [{
    data: [120, 200, 150, 80, 70],
    type: 'bar'
  }]
})
</script>

<style>
.chart {
  height: 400px;
}
</style>
