<template>
  <v-chart :option="chartOption" :style="{ height: height, width: '100%' }" autoresize/>
</template>

<script setup lang="ts">
import {computed} from 'vue'

export interface LineChartDataPoint {
  x: number | string | Date
  y: number
}

export interface LineChartProps {
  data: LineChartDataPoint[]
  xFormatter?: (value: any) => string
  yFormatter?: (value: any) => string
  lineColor?: string
  fillGradient?: boolean
  title?: string
  height?: string
  xAxisName?: string
  yAxisName?: string
  zoom?: boolean
  smoothed?: boolean
}

const props = withDefaults(defineProps<LineChartProps>(), {
  lineColor: '#2196F3',
  fillGradient: false,
  height: '400px',
  xFormatter: (value: any) => String(value),
  yFormatter: (value: any) => String(value),
  zoom: false,
  smoothed: false
})

const chartOption = computed(() => {
  const xData = props.data.map(d => d.x)
  const yData = props.data.map(d => d.y)

  return {
    title: props.title ? {
      text: props.title,
      left: 'center',
      textStyle: {
        fontSize: 16,
        fontWeight: 'normal'
      }
    } : undefined,
    tooltip: {
      trigger: 'axis',
      formatter: (params: any) => {
        const param = params[0]
        const xLabel = props.xFormatter(param.axisValue)
        const yLabel = props.yFormatter(param.data)
        return `${xLabel}<br/>${param.marker}${yLabel}`
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: props.zoom ? '15%' : '3%',
      top: props.title ? '15%' : '3%',
      containLabel: true
    },
    dataZoom: props.zoom ? [
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
        end: 100
      }
    ] : undefined,
    xAxis: {
      type: 'category',
      data: xData,
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
    series: [
      {
        type: 'line',
        data: yData,
        smooth: props.smoothed,
        symbol: 'circle',
        symbolSize: 6,
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
                color: props.lineColor + '80' // 50% opacity at top
              },
              {
                offset: 1,
                color: props.lineColor + '10' // 6% opacity at bottom
              }
            ]
          }
        } : undefined
      }
    ]
  }
})
</script>

<style scoped>
/* Add any custom styles here if needed */
</style>
