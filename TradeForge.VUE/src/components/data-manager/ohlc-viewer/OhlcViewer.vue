<template>
  <v-card height="100%" style="padding-left: 1rem; padding-right: 1rem">
    <v-tabs v-model="activeTab" density="compact">
      <v-tab value="chart" prepend-icon="mdi-chart-waterfall">Chart</v-tab>
      <v-tab value="table" prepend-icon="mdi-table">Table</v-tab>
    </v-tabs>

    <v-divider/>

    <v-window v-model="activeTab" style=" height: 95%;">
      <v-window-item value="chart" style="height: 100%">

        <GCandlestickChart
          :data="data"
          :drawings="drawings"
          :height="chartHeight"
          :show-volume="showVolume"
        />
      </v-window-item>

      <v-window-item value="table" style="height: 100%">
        <OhlcTable
          ref="tableRef"
          :items="data"
          :loading="loading"
          :readonly="readonly"
          @insert="emit('insert', $event)"
          @update="(original, updated) => emit('update', original, updated)"
          @remove="emit('remove', $event)"
        />
      </v-window-item>
    </v-window>
  </v-card>
</template>

<script setup lang="ts">
import {ref} from 'vue';
import type {OhlcData, ChartDrawing} from '@/types/OhlcData.interface.ts';
import GCandlestickChart from '@/components/shared/charts/GCandlestickChart.vue';
import OhlcTable from '@/components/data-manager/ohlc-table/OhlcTable.vue';

interface Props {
  data: OhlcData[];
  drawings?: ChartDrawing[];
  loading?: boolean;
  readonly?: boolean;
  chartHeight?: string;
  showVolume?: boolean;
}

withDefaults(defineProps<Props>(), {
  drawings: () => [],
  loading: false,
  readonly: false,
  chartHeight: '600px',
  showVolume: true,
});

const emit = defineEmits<{
  insert: [item: Omit<OhlcData, 'timestamp'> & { timestamp: Date }];
  update: [original: OhlcData, updated: Omit<OhlcData, 'timestamp'> & { timestamp: Date }];
  remove: [item: OhlcData];
}>();

const activeTab = ref<'chart' | 'table'>('chart');
const tableRef = ref<InstanceType<typeof OhlcTable> | null>(null);

defineExpose({
  stopDeleteLoading: () => tableRef.value?.stopDeleteLoading(),
  stopFormLoading: () => tableRef.value?.stopFormLoading(),
});
</script>
