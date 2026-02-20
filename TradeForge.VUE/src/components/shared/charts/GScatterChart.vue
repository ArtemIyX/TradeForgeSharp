<template>
  <v-chart :option="chartOption" :style="{ height: height, width: '100%' }" autoresize />
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface ScatterChartDataPoint {
  x: number
  y: number
  label?: string
}

export interface ScatterChartSeries {
  name: string
  data: ScatterChartDataPoint[]
  color?: string
}

export interface ScatterChartProps {
  /** Single series shorthand */
  data?: ScatterChartDataPoint[]
  /** Multi-series */
  series?: ScatterChartSeries[]
  xFormatter?: (value: any) => string
  yFormatter?: (value: any) => string
  dotColor?: string
  dotSize?: number
  title?: string
  height?: string
  xAxisName?: string
  yAxisName?: string
  zoom?: boolean
}

const props = withDefaults(defineProps<ScatterChartProps>(), {
  dotColor: '#2196F3',
  dotSize: 8,
  height: '400px',
  xFormatter: (v: any) => String(v),
  yFormatter: (v: any) => String(v),
  zoom: false,
})

const resolvedSeries = computed<ScatterChartSeries[]>(() => {
  if (props.series && props.series.length) return props.series
  if (props.data && props.data.length) {
    return [{ name: 'Data', data: props.data, color: props.dotColor }]
  }
  return []
})

const chartOption = computed(() => {
  return {
    title: props.title
      ? {
        text: props.title,
        left: 'center',
        textStyle: { fontSize: 16, fontWeight: 'normal' },
      }
      : undefined,

    legend: resolvedSeries.value.length > 1
      ? { bottom: 0 }
      : undefined,

    tooltip: {
      trigger: 'item',
      formatter: (param: any) => {
        const [x, y, label] = param.data
        const xLabel = props.xFormatter(x)
        const yLabel = props.yFormatter(y)
        return `${param.marker}${label ?? param.seriesName}<br/>X: ${xLabel}<br/>Y: ${yLabel}`
      },
    },

    grid: {
      left: '3%',
      right: '4%',
      bottom: props.zoom ? '18%' : resolvedSeries.value.length > 1 ? '10%' : '5%',
      top: props.title ? '15%' : '5%',
      containLabel: true,
    },

    dataZoom: props.zoom
      ? [
        { type: 'slider', xAxisIndex: [0], start: 0, end: 100 },
        { type: 'slider', yAxisIndex: [0], start: 0, end: 100 },
        { type: 'inside', xAxisIndex: [0] },
        { type: 'inside', yAxisIndex: [0] },
      ]
      : undefined,

    xAxis: {
      type: 'value',
      name: props.xAxisName,
      nameLocation: 'middle',
      nameGap: 30,
      axisLabel: { formatter: props.xFormatter },
      splitLine: { lineStyle: { type: 'dashed', opacity: 0.3 } },
    },

    yAxis: {
      type: 'value',
      name: props.yAxisName,
      nameLocation: 'middle',
      nameGap: 50,
      axisLabel: { formatter: props.yFormatter },
      splitLine: { lineStyle: { type: 'dashed', opacity: 0.3 } },
    },

    series: resolvedSeries.value.map(s => ({
      name: s.name,
      type: 'scatter',
      symbolSize: props.dotSize,
      itemStyle: { color: s.color ?? props.dotColor, opacity: 0.8 },
      emphasis: {
        itemStyle: { opacity: 1, shadowBlur: 6, shadowColor: 'rgba(0,0,0,0.3)' },
      },
      // [x, y, label] — label stored as 3rd element for tooltip
      data: s.data.map(d => [d.x, d.y, d.label]),
    })),
  }
})
</script>

<style scoped>
</style>
