<template>
  <v-card class="pa-4">
    <!-- Ticker Information -->
    <v-card-title class="text-h5 pb-2">
      <span class="text-accent">{{ ticker.ticker }}</span> <span class="text-subtitle-2 text-medium-emphasis">{{ ticker.name}} </span>
    </v-card-title>
    <v-card-subtitle class="pb-4">
      <v-chip size="small" class="mr-2">
        <span class="text-success">{{ ticker.category }} </span>
      </v-chip>
      <v-chip size="small"  class="mr-2">
        <span class="text-warning">{{ ticker.type }} </span>
      </v-chip>
      <span class="text-medium-emphasis">{{ ticker.description }}</span>
    </v-card-subtitle>

    <div class="d-flex justify-center my-4" v-if="loading">
      <v-progress-circular
        indeterminate
      />
    </div>
    <OHLCTable v-else
               :ohlc-data="ohlcData"
               :rows-per-page="10"
               :decimal-precision="2"
               :ticker="ticker"/>


  </v-card>
</template>

<script setup>

import OHLCTable from "@/components/history/OHLCTable.vue";

import {ref, onMounted} from "vue";
import { useRoute } from 'vue-router';

const route = useRoute();

const loading = ref(true);
const ohlcData = ref([]);
const ticker = ref({
  id: 10,
  ticker: "USOIL",
  category: "Commodity",
  name: "WTI Crude",
  type: "CFD",
  description: "US oil benchmark",
});

const fetchData = async () => {
  loading.value = true
  try {
    const res = await fetch('/dummy/ohlc.json')
    await new Promise(resolve => setTimeout(resolve, 1000))
    ohlcData.value = await res.json()
  } catch (e) {
    console.error('Failed to load local JSON:', e)
    ohlcData.value = []
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  ticker.id = route.params.id;
  console.log('Details of ' + ticker.id);
  fetchData();
})

</script>
