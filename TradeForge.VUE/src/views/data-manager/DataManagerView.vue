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
        @clear-selection="handleClearSelection"

        :disabled-buttons="disabledButtons"
      />
      <v-divider/>
      <DataManagerTable ref="dataManagerTableRef" :tickers="tickers" :loading="tickersLoading"
                        :disabled-buttons="disabledButtons"/>

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
import {computed, onMounted, ref} from "vue";

import tickersData from './tickers-dummy.json';

import {convertToTickers, type Ticker, type TickerResponseModel} from "@/types/Ticker";

const tickersLoading = ref<boolean>(false);
const tickers = ref<Ticker[]>([]);
const dataManagerTableRef = ref<InstanceType<typeof DataManagerTable> | null>(null);

const disabledButtons = computed(() => {
  const ticker: Ticker | null | undefined = dataManagerTableRef.value?.getSelectedTicker();
  const hasTicker = ticker !== null && ticker !== undefined;
  const noRecords = hasTicker && ticker?.totalRecords === 0;
  return {
    'edit': !hasTicker,
    'delete': !hasTicker,
    'view-data': !(hasTicker && !noRecords),
    'import': !hasTicker,
    'export': !(hasTicker && !noRecords),
    'clear-data': !(hasTicker && !noRecords),
    'clear-selection': !hasTicker
  };
});

const handleCreate = () => console.log('Create')
const handleEdit = () => console.log('Edit')
const handleDelete = () => console.log('Delete')
const handleViewData = () => console.log('View Data')
const handleImport = () => console.log('Import')
const handleExport = () => console.log('Export')
const handleClearData = () => console.log('Clear Data')

const handleClearSelection = () => {
  dataManagerTableRef?.value?.clearSelectedTicker();
}

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

