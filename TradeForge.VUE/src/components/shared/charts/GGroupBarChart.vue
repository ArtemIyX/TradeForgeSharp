<template>
  <v-chart :option="chartOption" :style="{ height: height, width: '100%' }" autoresize />
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface GroupedBarDataPoint {
  x: number | string | Date
  y: number
}

export interface GroupedBarSeries {
  name: string
  data: GroupedBarDataPoint[]
  /** Fixed color for all bars in this series */
  color?: string
  /** Enable sign-based coloring per bar (overrides color) */
  positiveColor?: string
  negativeColor?: string
  /** Stack key — bars with the same stack string will be stacked */
  stack?: string
}

export interface GroupedBarChartProps {
  series: GroupedBarSeries[]

  xFormatter?: (value: any) => string
  yFormatter?: (value: any) => string
  xAxisName?: string
  yAxisName?: string
  title?: string
  height?: string

  zoom?: boolean
  showLabels?: boolean
  showLegend?: boolean
  borderRadius?: [number, number, number, number]
  barMaxWidth?: string
}

const DEFAULT_COLORS = [
  '#2196F3',
  '#4CAF50',
  '#FF9800',
  '#9C27B0',
  '#F44336',
  '#00BCD4',
  '#FFEB3B',
  '#795548',
]

const props = withDefaults(defineProps<GroupedBarChartProps>(), {
  height: '400px',
  xFormatter: (v: any) => String(v),
  yFormatter: (v: any) => String(v),
  zoom: false,
  showLabels: false,
  showLegend: true,
  barMaxWidth: '40%',
  borderRadius: () => [3, 3, 0, 0],
})

const chartOption = computed(() => {
  // Collect all unique x values (preserve insertion order, then sort)
  const xSet = new Set<any>()
  props.series.forEach(s => s.data.forEach(d => xSet.add(d.x)))
  const xAxisData = Array.from(xSet)

  const allSeries = props.series.map((s, si) => {
    const fallbackColor = s.color ?? DEFAULT_COLORS[si % DEFAULT_COLORS.length]
    const useSignColor = !!(s.positiveColor || s.negativeColor)

    // Build a lookup map for fast access
    const lookup = new Map(s.data.map(d => [d.x, d.y]))

    const seriesData = xAxisData.map(x => {
      const value = lookup.get(x) ?? null

      let color = fallbackColor
      if (useSignColor && value !== null) {
        if (value > 0) color = s.positiveColor ?? fallbackColor
        else if (value < 0) color = s.negativeColor ?? fallbackColor
      }

      return {
        value,
        itemStyle: {
          color,
          borderRadius: props.borderRadius,
        },
      }
    })

    return {
      name: s.name,
      type: 'bar',
      stack: s.stack,
      maxBarWidth: props.barMaxWidth,
      data: seriesData,
      label: props.showLabels
        ? {
          show: true,
          position: 'top',
          formatter: (p: any) => props.yFormatter(p.data.value ?? p.data),
          fontSize: 11,
        }
        : { show: false },
    }
  })

  return {
    title: props.title
      ? {
        text: props.title,
        left: 'center',
        textStyle: { fontSize: 16, fontWeight: 'normal' },
      }
      : undefined,

    legend: props.showLegend
      ? {
        top: props.title ? '10%' : '3%',
        left: 'center',
        data: props.series.map(s => s.name),
      }
      : undefined,

    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' },
      formatter: (params: any) => {
        const xLabel = props.xFormatter(params[0].axisValue)
        let html = `${xLabel}<br/>`
        params.forEach((p: any) => {
          if (p.data.value === null) return
          html += `${p.marker}${p.seriesName}: ${props.yFormatter(p.data.value)}<br/>`
        })
        return html
      },
    },

    grid: {
      left: '3%',
      right: '4%',
      bottom: props.zoom ? '15%' : '3%',
      top: props.title ? (props.showLegend ? '20%' : '15%') : (props.showLegend ? '12%' : '8%'),
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
      data: xAxisData,
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

    series: allSeries,
  }
})
</script>
