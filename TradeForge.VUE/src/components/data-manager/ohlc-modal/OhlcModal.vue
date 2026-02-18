<template>
  <v-dialog v-model="dialog" max-width="90vw" scrollable>
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
        <span>{{ ticker?.symbol }} — {{ ticker?.timeFrame }}</span>
        <v-btn icon="mdi-close" variant="text" @click="hide"/>
      </v-card-title>

      <v-divider/>

      <v-card-text class="pa-0 modal-body" >
        <OhlcViewer
          :data="ohlcItems"
          :loading="loading"
          chartHeight="100%"
          readonly
        />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { Ticker } from '@/types/Ticker.interface.ts';
import { convertToOhlcArray, type OhlcData, type OhlcResponseModel } from '@/types/OhlcData.interface.ts';
import OhlcViewer from '@/components/data-manager/ohlc-viewer/OhlcViewer.vue';
import ohlcDummy from '@/assets/dummy/ohlc-dummy.json';

const dialog = ref(false);
const loading = ref(false);
const ticker = ref<Ticker | null>(null);
const ohlcItems = ref<OhlcData[]>([]);

const show = async (t: Ticker) => {
  ticker.value = t;
  ohlcItems.value = [];
  dialog.value = true;
  loading.value = true;

  try {
    // TODO: replace with real API call, e.g. await ohlcService.getByTicker(t.id)
    await new Promise(resolve => setTimeout(resolve, 500));
    ohlcItems.value = convertToOhlcArray(ohlcDummy as OhlcResponseModel[]);
  } finally {
    loading.value = false;
  }
};

const hide = () => {
  dialog.value = false;
};

defineExpose({ show, hide });
</script>

<style scoped>
.modal-body {
  height: 75vh;
  overflow: auto;
}
</style>
