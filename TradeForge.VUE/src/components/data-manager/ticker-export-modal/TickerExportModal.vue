<template>
  <v-dialog v-model="isOpen" max-width="560" persistent>
    <v-card class="export-card" rounded="lg" elevation="0">

      <!-- Header -->
      <div class="export-header px-6 pt-5 pb-4">
        <div class="d-flex align-center justify-space-between">
          <div class="d-flex align-center gap-3">
            <div class="header-icon">
              <v-icon size="18" color="primary">mdi-database-export-outline</v-icon>
            </div>
            <div>
              <div class="text-subtitle-1 font-weight-semibold">Export Data</div>
              <div class="text-caption text-medium-emphasis">{{ ticker?.symbol }} · {{ ticker?.timeFrame }}</div>
            </div>
          </div>
          <v-btn
            icon="mdi-close"
            variant="text"
            size="small"
            :disabled="isExporting"
            @click="handleCancel"
          />
        </div>
      </div>

      <v-divider />

      <v-card-text class="px-6 pt-5 pb-2">

        <!-- Output Path -->
        <div class="field-group mb-5">
          <div class="field-label mb-2">Output Path</div>
          <v-text-field
            v-model="exportPath"
            placeholder="e.g. /exports/EURUSD_1h.csv"
            variant="outlined"
            density="compact"
            hide-details
            :disabled="isExporting"
            prepend-inner-icon="mdi-folder-outline"
            class="mono-input"
          >
            <template #append-inner>
              <v-btn
                icon="mdi-folder-open-outline"
                variant="text"
                size="x-small"
                :disabled="isExporting"
                @click="browsePath"
              />
            </template>
          </v-text-field>
        </div>

        <!-- Export Pattern -->
        <div class="field-group mb-5">
          <div class="d-flex align-center justify-space-between mb-2">
            <div class="field-label">Column Pattern</div>
            <div class="d-flex gap-1">
              <v-chip
                v-for="token in availableTokens"
                :key="token.value"
                size="x-small"
                variant="tonal"
                class="token-chip"
                :disabled="isExporting"
                @click="insertToken(token.value)"
              >{{ token.label }}</v-chip>
            </div>
          </div>
          <v-text-field
            v-model="exportPattern"
            variant="outlined"
            density="compact"
            hide-details
            :disabled="isExporting"
            prepend-inner-icon="mdi-code-braces"
            class="mono-input"
          />
          <div class="pattern-preview mt-2">
            <span class="text-caption text-medium-emphasis">Preview: </span>
            <span class="text-caption mono-text">{{ patternPreview }}</span>
          </div>
        </div>

        <!-- Timestamp Format -->
        <div class="field-group mb-5">
          <div class="field-label mb-2">Timestamp Format</div>
          <v-select
            v-model="selectedTimestampFormat"
            :items="timestampFormats"
            item-title="label"
            item-value="value"
            variant="outlined"
            density="compact"
            hide-details
            :disabled="isExporting"
            prepend-inner-icon="mdi-clock-outline"
          >
            <template #item="{ props, item }">
              <v-list-item v-bind="props">
                <template #subtitle>
                  <span class="mono-text text-caption">{{ item.example }}</span>
                </template>
              </v-list-item>
            </template>
            <template #selection="{ item }">
              <span class="text-body-2">{{ item.label }}</span>
              <span class="mono-text text-caption text-medium-emphasis ml-2">{{ item.example }}</span>
            </template>
          </v-select>
        </div>

        <!-- Records info -->
        <div class="records-info d-flex align-center gap-2 pa-3 rounded-lg mb-2">
          <v-icon size="16" color="primary">mdi-chart-bar</v-icon>
          <span class="text-caption">
            <strong>{{ ticker?.totalRecords?.toLocaleString() ?? '—' }}</strong> records will be exported
          </span>
          <v-spacer />
          <span class="text-caption text-medium-emphasis">
            {{ ticker?.dateFrom ? formatDate(ticker.dateFrom) : '—' }}
            → {{ ticker?.dateTo ? formatDate(ticker.dateTo) : '—' }}
          </span>
        </div>

        <!-- Progress Section -->
        <Transition name="slide-down">
          <div v-if="isExporting || exportDone" class="progress-section mt-4 pa-3 rounded-lg">
            <div class="d-flex align-center justify-space-between mb-2">
              <span class="text-caption font-weight-medium">
                {{ exportDone ? 'Export complete' : exportStatus }}
              </span>
              <span class="text-caption mono-text text-medium-emphasis">{{ progressPercent }}%</span>
            </div>
            <v-progress-linear
              v-model="progressPercent"
              :color="exportDone ? 'success' : 'primary'"
              height="6"
              rounded
              :indeterminate="progressPercent === 0 && isExporting"
            />
            <div v-if="!exportDone" class="d-flex align-center gap-1 mt-2">
              <v-icon size="12" color="medium-emphasis" class="spin-icon">mdi-loading</v-icon>
              <span class="text-caption text-medium-emphasis">{{ exportedRows.toLocaleString() }} / {{ (ticker?.totalRecords ?? 0).toLocaleString() }} rows</span>
            </div>
            <div v-else class="d-flex align-center gap-1 mt-2">
              <v-icon size="12" color="success">mdi-check-circle-outline</v-icon>
              <span class="text-caption text-success">Saved to {{ exportPath }}</span>
            </div>
          </div>
        </Transition>

      </v-card-text>

      <v-card-actions class="px-6 pb-5 pt-3 d-flex gap-2">
        <v-btn
          variant="outlined"
          :text="isExporting ? 'Cancel Export' : 'Cancel'"
          :color="isExporting ? 'error' : undefined"
          :prepend-icon="isExporting ? 'mdi-stop-circle-outline' : undefined"
          @click="handleCancel"
        />
        <v-spacer />
        <v-btn
          v-if="!exportDone"
          color="primary"
          variant="flat"
          :disabled="!canExport"
          :loading="isExporting"
          prepend-icon="mdi-export"
          text="Export"
          @click="startExport"
        />
        <v-btn
          v-else
          color="success"
          variant="flat"
          prepend-icon="mdi-check"
          text="Done"
          @click="handleDone"
        />
      </v-card-actions>

    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import type { Ticker } from '@/types/Ticker'

// ─── Emits ────────────────────────────────────────────────────────────────────
const emit = defineEmits<{
  'exported': [path: string]
}>()

// ─── Dialog state ─────────────────────────────────────────────────────────────
const isOpen = ref(false)
const ticker = ref<Ticker | null>(null)

function open(t: Ticker) {
  ticker.value = t
  isOpen.value = true
}

defineExpose({ open })

// ─── Form state ───────────────────────────────────────────────────────────────
const exportPath = ref('/exports/')
const exportPattern = ref('timestamp:o:h:l:c:v')

const availableTokens = [
  { label: 'ts', value: 'timestamp' },
  { label: 'o',  value: 'o' },
  { label: 'h',  value: 'h' },
  { label: 'l',  value: 'l' },
  { label: 'c',  value: 'c' },
  { label: 'v',  value: 'v' },
]

// ─── Timestamp formats ────────────────────────────────────────────────────────
interface TimestampFormat { label: string; value: string; example: string }

const timestampFormats: TimestampFormat[] = [
  { label: 'UTC Ticks',         value: 'utc_ticks',        example: '1708732800000'        },
  { label: 'ISO 8601',          value: 'iso8601',          example: '2024-02-23T16:00:00Z' },
  { label: 'yyyy-MM-dd HH:mm',  value: 'yyyy-MM-dd HH:mm', example: '2024-02-23 16:00'     },
  { label: 'dd-MM-yyyy HH:mm',  value: 'dd-MM-yyyy HH:mm', example: '23-02-2024 16:00'     },
  { label: 'MM/dd/yyyy HH:mm',  value: 'MM/dd/yyyy HH:mm', example: '02/23/2024 16:00'     },
  { label: 'yyyy-MM-dd',        value: 'yyyy-MM-dd',       example: '2024-02-23'           },
  { label: 'dd-MM-yyyy',        value: 'dd-MM-yyyy',       example: '23-02-2024'           },
  { label: 'Unix Seconds',      value: 'unix_seconds',     example: '1708732800'           },
]

const selectedTimestampFormat = ref<string>('yyyy-MM-dd HH:mm')

// ─── Pattern preview ──────────────────────────────────────────────────────────
const patternPreview = computed(() => {
  const tsFormat = timestampFormats.find(f => f.value === selectedTimestampFormat.value)
  const tsExample = tsFormat?.example ?? '...'

  return exportPattern.value
    .split(':')
    .map(token => {
      switch (token.trim()) {
        case 'timestamp': return tsExample
        case 'o': return '1.08452'
        case 'h': return '1.08790'
        case 'l': return '1.08100'
        case 'c': return '1.08563'
        case 'v': return '98420'
        default: return token
      }
    })
    .join(',')
})

// ─── Export process ───────────────────────────────────────────────────────────
const isExporting   = ref(false)
const exportDone    = ref(false)
const progressPercent = ref(0)
const exportedRows  = ref(0)
const exportStatus  = ref('Preparing export...')

let cancelToken = false
let exportInterval: ReturnType<typeof setInterval> | null = null

const canExport = computed(() =>
  !!exportPath.value.trim() &&
  !!exportPattern.value.trim() &&
  !isExporting.value &&
  !exportDone.value
)

async function startExport() {
  if (!canExport.value) return

  isExporting.value   = true
  exportDone.value    = false
  progressPercent.value = 0
  exportedRows.value  = 0
  cancelToken         = false

  const total = ticker.value?.totalRecords ?? 10000
  const stepSize = Math.ceil(total / 120) // ~2s duration at ~60fps chunks

  exportStatus.value = 'Writing file header...'

  await delay(400)
  if (cancelToken) return cleanup()

  exportStatus.value = 'Exporting rows...'

  exportInterval = setInterval(() => {
    if (cancelToken) {
      cleanup()
      return
    }

    exportedRows.value = Math.min(exportedRows.value + stepSize, total)
    progressPercent.value = Math.round((exportedRows.value / total) * 100)

    if (exportedRows.value >= total) {
      clearInterval(exportInterval!)
      finishExport()
    }
  }, 30)
}

async function finishExport() {
  exportStatus.value = 'Finalizing...'
  await delay(300)
  if (cancelToken) return cleanup()
  progressPercent.value = 100
  isExporting.value   = false
  exportDone.value    = true
  emit('exported', exportPath.value)
}

function cleanup() {
  if (exportInterval) clearInterval(exportInterval)
  isExporting.value   = false
  exportDone.value    = false
  progressPercent.value = 0
  exportedRows.value  = 0
  exportStatus.value  = 'Preparing export...'
}

function handleCancel() {
  if (isExporting.value) {
    cancelToken = true
  } else {
    isOpen.value = false
  }
}

function handleDone() {
  cleanup()
  isOpen.value = false
}

// ─── Helpers ──────────────────────────────────────────────────────────────────
function delay(ms: number) {
  return new Promise(resolve => setTimeout(resolve, ms))
}

function insertToken(token: string) {
  exportPattern.value = exportPattern.value
    ? `${exportPattern.value}:${token}`
    : token
}

function browsePath() {
  // Hook to native file dialog in future (Electron / Tauri)
  console.log('Browse path clicked — wire to native dialog')
}

function formatDate(date: Date | null): string {
  if (!date) return '—'
  return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

// Reset on open
watch(isOpen, (v) => {
  if (v) {
    cleanup()
    exportDone.value = false
    if (ticker.value?.symbol) {
      exportPath.value = `/exports/${ticker.value.symbol}_${ticker.value.timeFrame ?? 'data'}.csv`
    }
  }
})
</script>

<style scoped>
.export-card {
  border: 1px solid rgba(var(--v-border-color), 0.12);
  background: rgb(var(--v-theme-surface));
}

.export-header {
  background: rgba(var(--v-theme-surface-variant), 0.4);
}

.header-icon {
  width: 2rem;
  height: 2rem;
  border-radius: 0.5rem;
  background: rgba(var(--v-theme-primary), 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
}

.field-label {
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: rgba(var(--v-theme-on-surface), 0.6);
}

.mono-input :deep(input),
.mono-text {
  font-family: 'JetBrains Mono', 'Fira Code', 'Cascadia Code', monospace;
  font-size: 0.8rem;
}

.token-chip {
  cursor: pointer;
  font-family: 'JetBrains Mono', 'Fira Code', monospace;
  font-size: 0.7rem;
}
.token-chip:hover {
  opacity: 0.8;
}

.pattern-preview {
  padding-left: 0.25rem;
}

.records-info {
  background: rgba(var(--v-theme-primary), 0.05);
  border: 1px solid rgba(var(--v-theme-primary), 0.12);
}

.progress-section {
  background: rgba(var(--v-theme-surface-variant), 0.5);
  border: 1px solid rgba(var(--v-border-color), 0.1);
}

/* Spinner */
@keyframes spin {
  to { transform: rotate(360deg); }
}
.spin-icon {
  animation: spin 1s linear infinite;
}

/* Slide transition */
.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.25s ease;
}
.slide-down-enter-from {
  opacity: 0;
  transform: translateY(-0.5rem);
}
.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-0.25rem);
}
</style>
