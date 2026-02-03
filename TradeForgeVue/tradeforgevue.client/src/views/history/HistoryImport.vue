

<template>
  <v-container fluid class="fill-height">
    <v-row class="fill-height">
      <v-col cols="12">
        <v-card class="fill-height d-flex flex-column">
          <v-card-title>
            <span class="text-h5">Import OHLC Data - {{ ticker.name }} ({{ ticker.ticker }})</span>
          </v-card-title>

          <v-card-text class="flex-grow-1 d-flex flex-column">
            <!-- Import Section -->
            <div v-if="!importComplete">
              <!-- File Upload -->
              <v-file-input
                v-model="selectedFile"
                label="Select CSV file"
                accept=".csv"
                prepend-icon="mdi-file-delimited"
                :disabled="isImporting"
                @change="onFileSelected"
              ></v-file-input>

              <!-- Format Instructions -->
              <v-alert type="info" variant="tonal" class="mb-4">
                <div class="text-subtitle-2 mb-2">Expected CSV Format:</div>
                <code>Date,Open,High,Low,Close,Volume</code>
                <div class="text-caption mt-2">
                  Date format: YYYY-MM-DD or DD/MM/YYYY
                </div>
              </v-alert>

              <!-- Preview Table Header -->
              <div v-if="headerPreview.length > 0 && !isImporting" class="mb-4">
                <div class="text-subtitle-2 mb-2">Detected Headers:</div>
                <v-chip
                  v-for="(header, index) in headerPreview"
                  :key="index"
                  class="ma-1"
                  :color="validateHeader(header) ? 'success' : 'error'"
                >
                  {{ header }}
                </v-chip>
              </div>

              <!-- Import Button -->
              <v-btn
                color="primary"
                :disabled="!selectedFile || isImporting"
                :loading="isImporting"
                @click="startImport"
                block
                size="large"
              >
                <v-icon left>mdi-upload</v-icon>
                Start Import
              </v-btn>

              <!-- Progress Section -->
              <div v-if="isImporting" class="mt-6">
                <div class="d-flex justify-space-between mb-2">
                  <span class="text-subtitle-2">{{ importStatus }}</span>
                  <span class="text-subtitle-2">{{ importProgress }}%</span>
                </div>
                <v-progress-linear
                  :model-value="importProgress"
                  color="primary"
                  height="25"
                  striped
                >
                  <template v-slot:default>
                    <strong>{{ Math.ceil(importProgress) }}%</strong>
                  </template>
                </v-progress-linear>

                <div class="text-caption mt-2 text-center">
                  {{ importedRows }} / {{ totalRows }} rows processed
                </div>
              </div>
            </div>

            <!-- Success Message and Table -->
            <div v-else class="d-flex flex-column flex-grow-1">
              <v-alert type="success" variant="tonal" class="mb-4">
                <div class="d-flex align-center">
                  <v-icon size="large" class="mr-2">mdi-check-circle</v-icon>
                  <div>
                    <div class="text-subtitle-1">Import Completed Successfully!</div>
                    <div class="text-caption">
                      Imported {{ importedRows }} rows for {{ ticker.ticker }}
                    </div>
                  </div>
                  <v-spacer></v-spacer>
                  <v-btn
                    variant="text"
                    color="success"
                    @click="resetImport"
                  >
                    Import More Data
                  </v-btn>
                </div>
              </v-alert>

              <!-- OHLC Table -->
              <div class="flex-grow-1" style="min-height: 0;">
                <OHLCTable
                  :ohlc-data="ohlcData"
                  :rows-per-page="10"
                  :decimal-precision="2"
                  :ticker="ticker"
                  class="h-100"
                />
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import OHLCTable from "@/components/history/OHLCTable.vue";

const route = useRoute();

const ticker = ref({
  id: 10,
  ticker: "USOIL",
  category: "Commodity",
  name: "WTI Crude",
  type: "CFD",
  description: "US oil benchmark",
});

// Import state
const selectedFile = ref(null);
const headerPreview = ref([]);
const isImporting = ref(false);
const importComplete = ref(false);
const importProgress = ref(0);
const importStatus = ref('');
const importedRows = ref(0);
const totalRows = ref(0);
const ohlcData = ref([]);

// Required headers
const requiredHeaders = ['date', 'open', 'high', 'low', 'close'];

onMounted(() => {
  ticker.value.id = route.params.id;
});

// Validate header
const validateHeader = (header) => {
  const normalized = header.toLowerCase().trim();
  return requiredHeaders.includes(normalized) || normalized === 'volume';
};

// File selected handler
const onFileSelected = (event) => {
  if (!selectedFile.value) {
    headerPreview.value = [];
    return;
  }

  // Read first line to show headers
  const reader = new FileReader();
  reader.onload = (e) => {
    const text = e.target.result;
    const firstLine = text.split('\n')[0];
    headerPreview.value = firstLine.split(',').map(h => h.trim());
  };
  reader.readAsText(selectedFile.value);
};

// Generate fake OHLC data
const generateFakeOHLCData = (rows) => {
  const data = [];
  const startDate = new Date('2024-01-01');
  let basePrice = 75.0;

  for (let i = 0; i < rows; i++) {
    const date = new Date(startDate);
    date.setDate(date.getDate() + i);

    // Random walk for price
    const change = (Math.random() - 0.5) * 2;
    basePrice = Math.max(50, Math.min(100, basePrice + change));

    const open = basePrice;
    const close = basePrice + (Math.random() - 0.5) * 1.5;
    const high = Math.max(open, close) + Math.random() * 0.5;
    const low = Math.min(open, close) - Math.random() * 0.5;
    const volume = Math.floor(Math.random() * 1000000) + 500000;

    data.push({
      date: date.toISOString().split('T')[0],
      open: parseFloat(open.toFixed(2)),
      high: parseFloat(high.toFixed(2)),
      low: parseFloat(low.toFixed(2)),
      close: parseFloat(close.toFixed(2)),
      volume: volume,
    });

    basePrice = close;
  }

  return data;
};

// Start import with fake progress
const startImport = async () => {
  isImporting.value = true;
  importProgress.value = 0;
  importedRows.value = 0;
  totalRows.value = Math.floor(Math.random() * 200) + 100; // Random between 100-300 rows

  // Simulate import stages
  const stages = [
    { progress: 15, status: 'Reading file...', delay: 500 },
    { progress: 30, status: 'Validating headers...', delay: 400 },
    { progress: 45, status: 'Parsing data...', delay: 600 },
    { progress: 60, status: 'Validating OHLC values...', delay: 800 },
    { progress: 75, status: 'Checking for duplicates...', delay: 500 },
    { progress: 90, status: 'Saving to database...', delay: 700 },
    { progress: 100, status: 'Import complete!', delay: 300 },
  ];

  for (const stage of stages) {
    await new Promise(resolve => setTimeout(resolve, stage.delay));
    importProgress.value = stage.progress;
    importStatus.value = stage.status;
    importedRows.value = Math.floor((stage.progress / 100) * totalRows.value);
  }

  // Generate fake data
  ohlcData.value = generateFakeOHLCData(totalRows.value);

  await new Promise(resolve => setTimeout(resolve, 500));
  isImporting.value = false;
  importComplete.value = true;
};

// Reset import
const resetImport = () => {
  selectedFile.value = null;
  headerPreview.value = [];
  importComplete.value = false;
  importProgress.value = 0;
  importStatus.value = '';
  importedRows.value = 0;
  totalRows.value = 0;
};
</script>

<style scoped>
.fill-height {
  height: 100%;
}

</style>
