<template>
  <v-chart :option="chartOption" :style="{ height: height, width: '100%' }" autoresize />
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface BarChartDataPoint {
  x: number | string | Date
  y: number
  /** Optional per-item color override */
  color?: string
}

export interface BarChartProps {
  data: BarChartDataPoint[]

  /** Default bar color */
  barColor?: string
  /** Color when value > 0. Activates value-based coloring when provided */
  positiveColor?: string
  /** Color when value < 0. Activates value-based coloring when provided */
  negativeColor?: string
  /** Color when value === 0 */
  zeroColor?: string

  xFormatter?: (value: any) => string
  yFormatter?: (value: any) => string
  xAxisName?: string
  yAxisName?: string
  title?: string
  height?: string

  /** Show data zoom slider */
  zoom?: boolean
  /** Bar border radius [topLeft, topRight, bottomRight, bottomLeft] */
  borderRadius?: [number, number, number, number]
  /** Show value label on top of each bar */
  showLabels?: boolean
  /** Max category width in percent */
  barMaxWidth?: string
  /** Background color for bars (ghost bar) */
  showBackground?: boolean
}

const props = withDefaults(defineProps<BarChartProps>(), {
  barColor: '#2196F3',
  zeroColor: '#9E9E9E',
  height: '400px',
  xFormatter: (v: any) => String(v),
  yFormatter: (v: any) => String(v),
  zoom: false,
  showLabels: false,
  barMaxWidth: '60%',
  showBackground: false,
  borderRadius: () => [3, 3, 0, 0],
})

function resolveColor(value: number, perItemColor?: string): string {
  if (perItemColor) return perItemColor
  if (props.positiveColor || props.negativeColor) {
    if (value > 0) return props.positiveColor ?? props.barColor
    if (value < 0) return props.negativeColor ?? props.barColor
    return props.zeroColor
  }
  return props.barColor
}

const chartOption = computed(() => {
  const xData = props.data.map(d => d.x)

  const seriesData = props.data.map(d => ({
    value: d.y,
    itemStyle: {
      color: resolveColor(d.y, d.color),
      borderRadius: props.borderRadius,
    },
  }))

  return {
    title: props.title
      ? {
        text: props.title,
        left: 'center',
        textStyle: { fontSize: 16, fontWeight: 'normal' },
      }
      : undefined,

    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' },
      formatter: (params: any) => {
        const p = params[0]
        return `${props.xFormatter(p.axisValue)}<br/>${p.marker}${props.yFormatter(p.data.value ?? p.data)}`
      },
    },

    grid: {
      left: '3%',
      right: '4%',
      bottom: props.zoom ? '15%' : '3%',
      top: props.title ? '15%' : '8%',
      containLabel: true,
    },

    dataZoom: props.zoom
      ? [
        { type: 'slider', xAxisIndex: [0], start: 0, end: 100 },
        { type: 'inside', xAxisIndex: [0], start: 0, end: 100 },
      ]
      : undefined,

    xAxis: {
      type: 'category',
      data: xData,
      name: props.xAxisName,
      nameLocation: 'middle',
      nameGap: 30,
      axisLabel: { formatter: props.xFormatter },
    },

    yAxis: {
      type: 'value',
      name: props.yAxisName,
      nameLocation: 'middle',
      nameGap: 50,
      axisLabel: { formatter: props.yFormatter },
    },

    series: [
      {
        type: 'bar',
        data: seriesData,
        maxBarWidth: props.barMaxWidth,
        showBackground: props.showBackground,
        backgroundStyle: { color: 'rgba(180,180,180,0.08)' },
        label: props.showLabels
          ? {
            show: true,
            position: 'top',
            formatter: (p: any) => props.yFormatter(p.data.value ?? p.data),
            fontSize: 11,
          }
          : { show: false },
      },
    ],
  }
})
</script>
