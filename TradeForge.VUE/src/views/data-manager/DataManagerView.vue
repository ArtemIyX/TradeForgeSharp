<template>
  <DeleteTickerModal ref="deleteTickerModalRef" @delete="handleModalDelete"/>
  <ClearTickerModal ref="clearTickerDataModalRef" @clear="handleModalClearData"/>
  <OhlcModal ref="ohlcModalRef"/>
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
      <DataManagerTable ref="dataManagerTableRef"
                        :tickers="tickers"
                        :loading="tickersLoading"
                        :disabled-buttons="disabledButtons"
                        @edit="handleEdit"
                        @delete="handleDelete"
                        @view-data="handleViewData"
                        @import="handleImport"
                        @export="handleExport"
                        @clear-data="handleClearData"
                        @clear-selection="handleClearSelection"/>

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
// ============================================================================
// IMPORTS
// ============================================================================

// Component imports
import DataManagerToolbar
  from "@/components/data-manager/data-manager-toolbar/DataManagerToolbar.vue";
import DataManagerTable from "@/components/data-manager/data-manager-table/DataManagerTable.vue";
import DeleteTickerModal from "@/components/data-manager/delete-ticker-modal/DeleteTickerModal.vue";
import ClearTickerModal from "@/components/data-manager/clear-ticker-modal/ClearTickerModal.vue";
import OhlcModal from "@/components/data-manager/ohlc-modal/OhlcModal.vue";

// Vue composition API imports
import {computed, onMounted, ref} from "vue";

// Store imports
import {useSnackbarStore} from '@/stores/Snackbar.store';

// Type and utility imports
import {convertToTickers, type Ticker, type TickerResponseModel} from "@/types/Ticker";

// Dummy Data imports
import tickersData from '@/assets/dummy/tickers-dummy.json';

// ============================================================================
// STATE MANAGEMENT
// ============================================================================

// Loading state for ticker data fetching
const tickersLoading = ref<boolean>(false);

// Array of ticker objects to display in the table
const tickers = ref<Ticker[]>([]);

// Template refs for child components
const dataManagerTableRef = ref<InstanceType<typeof DataManagerTable> | null>(null);
const deleteTickerModalRef = ref<InstanceType<typeof DeleteTickerModal> | null>(null);
const clearTickerDataModalRef = ref<InstanceType<typeof ClearTickerModal> | null>(null);
const ohlcModalRef = ref<InstanceType<typeof OhlcModal> | null>(null);

// Snackbar store instance for displaying notifications
const snackbar = useSnackbarStore();

// ============================================================================
// COMPUTED PROPERTIES
// ============================================================================

/**
 * Returns the currently selected ticker from the data table
 * @returns {Ticker | null | undefined} The selected ticker or null/undefined if none selected
 */
const currentTicker = computed(() => {
  return dataManagerTableRef.value?.getSelectedTicker();
});

/**
 * Determines which toolbar buttons should be disabled based on the current selection state
 * @returns {Object} Object with button names as keys and boolean disabled states as values
 */
const disabledButtons = computed(() => {
  const ticker = dataManagerTableRef.value?.getSelectedTicker();
  const hasTicker = ticker !== null && ticker !== undefined;
  const hasNoRecords = hasTicker && ticker?.totalRecords === 0;

  return {
    'edit': !hasTicker,                      // Disabled if no ticker selected
    'delete': !hasTicker,                    // Disabled if no ticker selected
    'view-data': !(hasTicker && !hasNoRecords), // Disabled if no ticker or no records
    'import': !hasTicker,                    // Disabled if no ticker selected
    'export': !(hasTicker && !hasNoRecords), // Disabled if no ticker or no records
    'clear-data': !(hasTicker && !hasNoRecords), // Disabled if no ticker or no records
    'clear-selection': !hasTicker            // Disabled if no ticker selected
  };
});

// ============================================================================
// EVENT HANDLERS - Toolbar Actions (Placeholders)
// ============================================================================

/**
 * Handles the create ticker action
 * TODO: Implement ticker creation logic
 */
const handleCreate = () => {
  console.log('Create');
};

/**
 * Handles the edit ticker action
 * TODO: Implement ticker editing logic
 */
const handleEdit = () => {
  console.log('Edit');
};

/**
 * Handles the view ticker data action
 * TODO: Implement data viewing logic
 */
const handleViewData = () => {
  console.log('View Data');
  if(ohlcModalRef.value) {
    ohlcModalRef.value.show()
  }
};

/**
 * Handles the import ticker data action
 * TODO: Implement data import logic
 */
const handleImport = () => {
  console.log('Import');
};

/**
 * Handles the export ticker data action
 * TODO: Implement data export logic
 */
const handleExport = () => {
  console.log('Export');
};

// ============================================================================
// EVENT HANDLERS - Delete Operations
// ============================================================================

/**
 * Opens the delete confirmation modal for the currently selected ticker
 */
const handleDelete = () => {
  if (!currentTicker.value || !deleteTickerModalRef.value) {
    return;
  }

  deleteTickerModalRef.value.show(currentTicker.value);
};

/**
 * Executes the ticker deletion after modal confirmation
 * Shows loading state, simulates deletion, displays success notification, and clears selection
 */
const handleModalDelete = async () => {
  if (!deleteTickerModalRef.value) {
    return;
  }

  // Show loading state in modal
  deleteTickerModalRef.value.startLoading();

  // Simulate API call delay (TODO: Replace with actual API call)
  await new Promise(resolve => setTimeout(resolve, 750));

  // Hide the modal
  deleteTickerModalRef.value.hide();

  // Show success notification
  snackbar.show({
    message: `You successfully deleted ticker '${currentTicker.value?.symbol}'!`,
    color: 'warning',
    timeout: 3000
  });

  // Clear the table selection
  dataManagerTableRef.value?.clearSelectedTicker();
};

// ============================================================================
// EVENT HANDLERS - Clear Data Operations
// ============================================================================

/**
 * Opens the clear data confirmation modal for the currently selected ticker
 */
const handleClearData = () => {
  if (!currentTicker.value || !clearTickerDataModalRef.value) {
    return;
  }

  clearTickerDataModalRef.value.show(currentTicker.value);
};

/**
 * Executes the data clearing after modal confirmation
 * Shows loading state, simulates clearing, displays success notification, and clears selection
 */
const handleModalClearData = async () => {
  if (!clearTickerDataModalRef.value) {
    return;
  }

  // Show loading state in modal
  clearTickerDataModalRef.value.startLoading();

  // Simulate API call delay (TODO: Replace with actual API call)
  await new Promise(resolve => setTimeout(resolve, 750));

  // Hide the modal
  clearTickerDataModalRef.value.hide();

  // Show success notification
  snackbar.show({
    message: `You successfully cleared historical data of ticker '${currentTicker.value?.symbol}'!`,
    color: 'warning',
    timeout: 3000
  });

  // Clear the table selection
  dataManagerTableRef.value?.clearSelectedTicker();
};

/**
 * Clears the currently selected ticker from the table
 */
const handleClearSelection = () => {
  dataManagerTableRef.value?.clearSelectedTicker();
};

// ============================================================================
// DATA FETCHING
// ============================================================================

/**
 * Fetches ticker data from the server
 * Currently loads from local JSON file with simulated delay
 * TODO: Replace with actual API call
 */
const fetchTickers = async () => {
  // Set loading state
  tickersLoading.value = true;

  // Simulate API call delay
  await new Promise(resolve => setTimeout(resolve, 250));

  // Convert raw data to Ticker objects and update state
  tickers.value = convertToTickers(tickersData as TickerResponseModel[]);

  // Clear loading state
  tickersLoading.value = false;
};

// ============================================================================
// LIFECYCLE HOOKS
// ============================================================================

/**
 * Component mounted hook
 * Fetches initial ticker data when component is mounted
 */
onMounted(() => {
  fetchTickers();
});

</script>
