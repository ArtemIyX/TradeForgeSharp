<template>
  <v-chart :option="chartOption" :style="{ height: height, width: '100%' }" autoresize/>
</template>

<script setup lang="ts">
import {computed} from 'vue'

export interface LineChartDataPoint {
  x: number | string | Date
  y: number
}

export interface SeriesData {
  name: string
  data: LineChartDataPoint[]
  color?: string
}

export interface LineChartProps {
  // Single series mode (backward compatible)
  data?: LineChartDataPoint[]
  lineColor?: string
  fillGradient?: boolean

  // Multi-series mode
  series?: SeriesData[]

  // Common options
  xFormatter?: (value: any) => string
  yFormatter?: (value: any) => string
  title?: string
  height?: string
  xAxisName?: string
  yAxisName?: string
  zoom?: boolean
  smoothed?: boolean
  showLegend?: boolean
}

const props = withDefaults(defineProps<LineChartProps>(), {
  lineColor: '#2196F3',
  fillGradient: false,
  height: '400px',
  xFormatter: (value: any) => String(value),
  yFormatter: (value: any) => String(value),
  zoom: false,
  smoothed: false,
  showLegend: false
})

// Default colors for multi-series
const defaultColors = [
  '#2196F3', // Blue
  '#4CAF50', // Green
  '#FF9800', // Orange
  '#9C27B0', // Purple
  '#F44336', // Red
  '#00BCD4', // Cyan
  '#FFEB3B', // Yellow
  '#795548', // Brown
]

const chartOption = computed(() => {
  let allSeries: any[] = []
  let xAxisData: any[] = []

  // Multi-series mode
  if (props.series && props.series.length > 0) {
    // Collect all unique x values across all series
    const xSet = new Set<any>()
    props.series.forEach(s => {
      s.data.forEach(d => xSet.add(d.x))
    })
    xAxisData = Array.from(xSet).sort()

    // Create series for each dataset
    allSeries = props.series.map((seriesItem, index) => {
      const color = seriesItem.color || defaultColors[index % defaultColors.length]
      const yData = seriesItem.data.map(d => d.y)

      return {
        name: seriesItem.name,
        type: 'line',
        data: yData,
        smooth: props.smoothed,
        symbol: 'none',
        lineStyle: {
          color: color,
          width: 2
        },
        itemStyle: {
          color: color
        },
        // No gradient for multi-series
        areaStyle: undefined
      }
    })
  }
  // Single series mode (backward compatible)
  else if (props.data && props.data.length > 0) {
    xAxisData = props.data.map(d => d.x)
    const yData = props.data.map(d => d.y)

    allSeries = [{
      type: 'line',
      data: yData,
      smooth: props.smoothed,
      symbol: 'none',
      lineStyle: {
        color: props.lineColor,
        width: 2
      },
      itemStyle: {
        color: props.lineColor
      },
      areaStyle: props.fillGradient ? {
        color: {
          type: 'linear',
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            {
              offset: 0,
              color: props.lineColor + '80'
            },
            {
              offset: 1,
              color: props.lineColor + '10'
            }
          ]
        }
      } : undefined
    }]
  }

  return {
    title: props.title ? {
      text: props.title,
      left: 'center',
      textStyle: {
        fontSize: 16,
        fontWeight: 'normal'
      }
    } : undefined,
    legend: (props.series && props.series.length > 1 && props.showLegend) ? {
      top: props.title ? '10%' : '3%',
      left: 'center',
      data: props.series.map(s => s.name)
    } : undefined,
    tooltip: {
      trigger: 'axis',
      formatter: (params: any) => {
        if (params.length === 0) return ''

        const xLabel = props.xFormatter(params[0].axisValue)
        let tooltipContent = `${xLabel}<br/>`

        params.forEach((param: any) => {
          const yLabel = props.yFormatter(param.data)
          const seriesName = param.seriesName ? `${param.seriesName}: ` : ''
          tooltipContent += `${param.marker}${seriesName}${yLabel}<br/>`
        })

        return tooltipContent
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: props.zoom ? '15%' : '3%',
      top: props.title ? (props.showLegend && props.series && props.series.length > 1 ? '20%' : '15%') : '3%',
      containLabel: true
    },
    dataZoom: [
      ...(props.zoom ? [
        {
          type: 'slider',
          show: true,
          xAxisIndex: [0],
          start: 0,
          end: 100
        },
        {
          type: 'inside',
          xAxisIndex: [0],
          start: 0,
          end: 100,
          zoomOnMouseWheel: 'shift'
        }
      ] : []),
      {
        type: 'inside',
        yAxisIndex: [0],
        start: 0,
        end: 100,
        zoomOnMouseWheel: true,
        moveOnMouseWheel: false
      }
    ],
    xAxis: {
      type: 'category',
      data: xAxisData,
      boundaryGap: false,
      name: props.xAxisName,
      nameLocation: 'middle',
      nameGap: 30,
      axisLabel: {
        formatter: props.xFormatter
      }
    },
    yAxis: {
      type: 'value',
      name: props.yAxisName,
      nameLocation: 'middle',
      nameGap: 50,
      axisLabel: {
        formatter: props.yFormatter
      }
    },
    series: allSeries
  }
})
</script>

<style scoped>
/* Add any custom styles here if needed */
</style>
