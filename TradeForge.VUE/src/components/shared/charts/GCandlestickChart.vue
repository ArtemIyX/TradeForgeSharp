<template>
  <div class="candlestick-container">
    <v-chart
      :option="chartOption"
      :autoresize="true"
      class="candlestick-chart"
    />
  </div>
</template>

<script setup lang="ts">
import {computed} from 'vue';
import type {OhlcData, ChartDrawing} from '@/types/OhlcData.interface.ts';

interface Props {
  data: OhlcData[];
  drawings?: ChartDrawing[];
  height?: string;
  showVolume?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  drawings: () => [],
  height: '600px',
  showVolume: true
});

const chartOption = computed(() => {
  const timestamps = props.data.map(d =>
    d.timestamp instanceof Date ? d.timestamp : new Date(d.timestamp)
  );

  const ohlcValues = props.data.map(d => [d.open, d.close, d.low, d.high]);
  const volumeValues = props.data.map(d => d.volume || 0);

  const series: any[] = [
    {
      name: 'Candlestick',
      type: 'candlestick',
      data: ohlcValues,
      itemStyle: {
        color: '#00ff00',
        color0: '#ff0000',
        borderColor: '#00ff00',
        borderColor0: '#ff0000',
      },
      xAxisIndex: 0,
      yAxisIndex: 0,
    }
  ];

  if (props.showVolume) {
    series.push({
      name: 'Volume',
      type: 'bar',
      data: volumeValues,
      xAxisIndex: 1,
      yAxisIndex: 1,
      itemStyle: {
        color: (params: any) => {
          const idx = params.dataIndex;
          const item = props.data[idx];
          if (!item) return 'rgba(128, 128, 128, 0.3)';

          return item.close >= item.open
            ? 'rgba(0, 255, 0, 0.3)'
            : 'rgba(255, 0, 0, 0.3)';
        }
      }
    });
  }

  // Add custom drawings
  const markLines: any[] = [];

  props.drawings.forEach(drawing => {
    if (drawing.type === 'horizontal') {
      markLines.push({
        yAxis: drawing.data.value,
        lineStyle: {
          color: drawing.style?.color || '#ffffff',
          width: drawing.style?.width || 1,
          type: drawing.style?.lineStyle || 'solid'
        }
      });
    } else if (drawing.type === 'vertical') {
      markLines.push({
        xAxis: drawing.data.value,
        lineStyle: {
          color: drawing.style?.color || '#ffffff',
          width: drawing.style?.width || 1,
          type: drawing.style?.lineStyle || 'solid'
        }
      });
    }
  });

  if (markLines.length > 0) {
    series[0].markLine = {
      symbol: 'none',
      data: markLines
    };
  }

  return {
    backgroundColor: 'transparent',
    grid: props.showVolume ? [
      {left: '3%', right: '3%', top: '5%', height: '65%'},
      {left: '3%', right: '3%', top: '75%', height: '15%'}
    ] : [
      {left: '3%', right: '3%', top: '5%', bottom: '10%'}
    ],
    xAxis: props.showVolume ? [
      {
        type: 'category',
        data: timestamps,
        axisLine: {lineStyle: {color: '#666'}},
        axisLabel: {
          color: '#999',
          formatter: (value: any, index: number) => {
            const date = timestamps[index];
            return date?.toLocaleDateString('en-US', {
              month: 'short',
              day: 'numeric'
            });
          }
        },
        gridIndex: 0
      },
      {
        type: 'category',
        data: timestamps,
        axisLine: {lineStyle: {color: '#666'}},
        axisLabel: {
          color: '#999',
          formatter: (value: any, index: number) => {
            const date = timestamps[index];
            return date?.toLocaleDateString('en-US', {
              month: 'short',
              day: 'numeric'
            });
          }
        },
        gridIndex: 1
      }
    ] : [
      {
        type: 'category',
        data: timestamps,
        axisLine: {lineStyle: {color: '#666'}},
        axisLabel: {
          color: '#999',
          formatter: (value: Date) => {
            return value.toLocaleDateString('en-US', {
              month: 'short',
              day: 'numeric'
            });
          }
        }
      }
    ],
    yAxis: props.showVolume ? [
      {
        scale: true,
        splitLine: {lineStyle: {color: '#333'}},
        axisLine: {lineStyle: {color: '#666'}},
        axisLabel: {color: '#999'},
        gridIndex: 0
      },
      {
        scale: true,
        splitLine: {show: false},
        axisLine: {lineStyle: {color: '#666'}},
        axisLabel: {color: '#999'},
        gridIndex: 1
      }
    ] : [
      {
        scale: true,
        splitLine: {lineStyle: {color: '#333'}},
        axisLine: {lineStyle: {color: '#666'}},
        axisLabel: {color: '#999'}
      }
    ],
    dataZoom: [
      {
        type: 'inside',
        xAxisIndex: props.showVolume ? [0, 1] : [0],
        start: 0,
        end: 100
      },
      {
        show: true,
        xAxisIndex: props.showVolume ? [0, 1] : [0],
        type: 'slider',
        bottom: '2%',
        height: '3%',
        borderColor: '#666',
        fillerColor: 'rgba(255, 255, 255, 0.1)',
        handleStyle: {
          color: '#999'
        },
        textStyle: {
          color: '#999'
        }
      }
    ],
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross',
        lineStyle: {
          color: '#666',
          type: 'dashed'
        }
      },
      backgroundColor: 'rgba(0, 0, 0, 0.9)',
      borderColor: '#666',
      borderWidth: 1,
      textStyle: {
        color: '#fff'
      },
      formatter: (params: any) => {
        const candleData = params.find((p: any) => p.seriesType === 'candlestick');
        if (!candleData) return '';

        const [open, close, low, high] = candleData.data;
        const volumeData = params.find((p: any) => p.seriesType === 'bar');
        const volume = volumeData ? volumeData.data : null;
        const date = timestamps[candleData.dataIndex];

        return `
    <div style="padding: 0.5em;">
      <div style="margin-bottom: 0.5em; font-weight: bold;">
        ${date?.toDateString()}
      </div>
      <div>O: ${open.toFixed(2)}</div>
      <div>H: ${high.toFixed(2)}</div>
      <div>L: ${low.toFixed(2)}</div>
      <div>C: ${close.toFixed(2)}</div>
      ${volume !== null ? `<div style="margin-top: 0.5em;">Vol: ${volume.toLocaleString()}</div>` : ''}
    </div>
  `;
      }
    },
    series
  };
});
</script>

<style scoped>
.candlestick-container {
  width: 100%;
  height: v-bind(height);
  background-color: transparent;
  padding-left: 1rem;
  border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
}

.candlestick-chart {
  width: 100%;
  height: 100%;
}
</style>
