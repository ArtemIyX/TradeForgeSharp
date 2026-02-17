<template>
  <v-dialog v-model="dialog" max-width="90vw" scrollable>
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
        <span>{{ title }}</span>
        <v-btn icon="mdi-close" variant="text" @click="hide"/>
      </v-card-title>

      <v-divider/>

      <v-card-text class="pa-0">
        <OhlcViewer
          :data="data"
          :drawings="drawings"
          :loading="loading"
          :chart-height="chartHeight"
          :show-volume="showVolume"
          readonly
        />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import {ref} from 'vue';
import type {OhlcData, ChartDrawing} from '@/types/OhlcData';
import OhlcViewer from '@/components/data-manager/ohlc-viewer/OhlcViewer.vue';

interface Props {
  title?: string;
  drawings?: ChartDrawing[];
  loading?: boolean;
  chartHeight?: string;
  showVolume?: boolean;
}

withDefaults(defineProps<Props>(), {
  title: 'OHLC Data',
  drawings: () => [],
  loading: false,
  chartHeight: '600px',
  showVolume: true,
});

const dialog = ref(false);
const data = ref<OhlcData[]>([]);

const show = (ohlcData: OhlcData[]) => {
  data.value = ohlcData;
  dialog.value = true;
};

const hide = () => {
  dialog.value = false;
};

defineExpose({show, hide});
</script>
