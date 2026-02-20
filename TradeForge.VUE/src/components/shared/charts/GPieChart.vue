<template>
  <div v-if="!props.data || props.data.length === 0" class="no-data">
    No data
  </div>
  <v-chart v-else :option="chartOption" :style="{ height: height, width: '100%' }" autoresize/>
</template>
<style scoped>
.no-data {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  min-height: 10rem;
  color: rgba(255, 255, 255, 0.3);
  font-size: 0.875rem;
  font-style: italic;
}
</style>
<script setup lang="ts">
import {computed} from 'vue'

export interface PieChartSlice {
  name: string
  value: number
  /** Optional color override for this slice */
  color?: string
}

export interface PieChartProps {
  data: PieChartSlice[]

  title?: string
  height?: string

  /** 'pie' = filled pie, 'donut' = ring chart */
  variant?: 'pie' | 'donut'
  /** Inner radius percent for donut (e.g. '50%') */
  innerRadius?: string
  /** Outer radius percent (e.g. '70%') */
  outerRadius?: string

  /** Show legend */
  showLegend?: boolean
  /** Legend position */
  legendPosition?: 'top' | 'bottom' | 'left' | 'right'

  /** Show percentage label on slices */
  showLabels?: boolean
  /** Format for slice labels */
  labelFormatter?: (name: string, value: number, percent: number) => string

  /** Value formatter used in tooltip */
  valueFormatter?: (value: number) => string

  /** Center text lines for donut (e.g. ['Total', '662']) */
  centerText?: [string, string]

  /** Animate on data change */
  animate?: boolean

  /** Fallback colors when slice.color not provided */
  colors?: string[]
}

const DEFAULT_COLORS = [
  '#4CAF50',
  '#F44336',
  '#2196F3',
  '#FF9800',
  '#9C27B0',
  '#00BCD4',
  '#FFEB3B',
  '#795548',
  '#E91E63',
  '#607D8B',
]

const props = withDefaults(defineProps<PieChartProps>(), {
  height: '400px',
  variant: 'pie',
  innerRadius: '50%',
  outerRadius: '70%',
  showLegend: true,
  legendPosition: 'bottom',
  showLabels: true,
  valueFormatter: (v: number) => String(v),
  labelFormatter: (name, value, percent) => `${name}\n${percent.toFixed(1)}%`,
  animate: true,
})

const isDonut = computed(() => props.variant === 'donut')

const radius = computed(() =>
  isDonut.value
    ? [props.innerRadius, props.outerRadius]
    : props.outerRadius
)

const seriesData = computed(() =>
  props.data.map((slice, i) => ({
    name: slice.name,
    value: slice.value,
    itemStyle: {
      color: slice.color ?? (props.colors?.[i] ?? DEFAULT_COLORS[i % DEFAULT_COLORS.length]),
    },
  }))
)

const chartOption = computed(() => ({
  title: props.title
    ? {
      text: props.title,
      left: 'center',
      textStyle: {fontSize: 16, fontWeight: 'normal'},
    }
    : undefined,

  legend: props.showLegend
    ? {
      orient: ['left', 'right'].includes(props.legendPosition) ? 'vertical' : 'horizontal',
      [props.legendPosition]: props.legendPosition === 'bottom' ? '2%' : props.legendPosition === 'top' ? '10%' : '2%',
      left: ['left', 'right'].includes(props.legendPosition) ? props.legendPosition : 'center',
      top: props.legendPosition === 'top' ? (props.title ? '12%' : '3%') : undefined,
      bottom: props.legendPosition === 'bottom' ? '2%' : undefined,
      data: (props.data ?? []).map(d => d.name),
    }
    : undefined,

  tooltip: {
    trigger: 'item',
    formatter: (p: any) =>
      `${p.marker}${p.name}<br/>${props.valueFormatter(p.value)} (${p.percent.toFixed(1)}%)`,
  },

  graphic: isDonut.value && props.centerText
    ? [
      {
        type: 'text',
        left: 'center',
        top: 'middle',
        style: {
          text: props.centerText[0],
          textAlign: 'center',
          fill: 'rgba(255,255,255,0.5)',
          fontSize: 13,
          fontWeight: 400,
        },
        silent: true,
      },
      {
        type: 'text',
        left: 'center',
        top: '54%',
        style: {
          text: props.centerText[1],
          textAlign: 'center',
          fill: 'rgba(255,255,255,0.9)',
          fontSize: 22,
          fontWeight: 700,
        },
        silent: true,
      },
    ]
    : undefined,

  series: [
    {
      type: 'pie',
      radius: radius.value,
      center: ['50%', props.showLegend && props.legendPosition === 'bottom' ? '45%' : '50%'],
      data: seriesData.value,
      emphasis: {
        itemStyle: {
          shadowBlur: 12,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.5)',
        },
        scaleSize: 6,
      },
      label: props.showLabels
        ? {
          show: true,
          formatter: (p: any) => props.labelFormatter(p.name, p.value, p.percent),
        }
        : {show: false},
      labelLine: {show: props.showLabels},
      animationType: props.animate ? 'expansion' : 'scale',
      animationEasing: 'cubicOut',
    },
  ],
}))
</script>
