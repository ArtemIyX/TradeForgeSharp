<template>
  <div class="data-manager">
    <div class="content-container">
      <DataManagerToolbar
        @create="handleCreate"
        @edit="handleEdit"
        @delete="handleDelete"
        @view-data="handleViewData"
        @import="handleImport"
        @export="handleExport"
        @clear-data="handleClearData"
      />
      <v-divider/>
      <DataManagerTable :tickers="tickers" :loading="tickersLoading"/>


    </div>
  </div>
</template>
<style scoped>
.data-manager {
  height: 100vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.content-container {
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow: hidden;

}


</style>


<script setup lang="ts">
import DataManagerToolbar
  from "@/components/data-manager/data-manager-toolbar/DataManagerToolbar.vue";
import DataManagerTable from "@/components/data-manager/data-manager-table/DataManagerTable.vue";
import {onMounted, ref} from "vue";

import tickersData from './tickers-dummy.json';

import {convertToTickers, type Ticker, type TickerResponseModel} from "@/types/Ticker";

const tickersLoading = ref<boolean>(false);
const tickers = ref<Ticker[]>([]);

const handleCreate = () => console.log('Create')
const handleEdit = () => console.log('Edit')
const handleDelete = () => console.log('Delete')
const handleViewData = () => console.log('View Data')
const handleImport = () => console.log('Import')
const handleExport = () => console.log('Export')
const handleClearData = () => console.log('Clear Data')

const fetchTickers = async () => {
  tickersLoading.value = true;

  await new Promise(resolve => setTimeout(resolve, 250));

  tickers.value = convertToTickers(tickersData as TickerResponseModel[]);
  tickersLoading.value = false;
};

onMounted(() => {
  fetchTickers();
})

</script>

