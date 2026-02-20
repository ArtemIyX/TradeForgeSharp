<template>
  <v-footer app height="auto" class="app-footer px-3 py-0">
    <div class="footer-inner">

      <!-- GROUP 1: Clocks -->
      <div class="footer-group">
        <div class="footer-item">
          <span class="footer-label">LOCAL</span>
          <span class="footer-value">{{ localTime }}</span>
        </div>
        <div class="footer-divider" />
        <div class="footer-item">
          <span class="footer-label">UTC</span>
          <span class="footer-value">{{ utcTime }}</span>
        </div>
        <div class="footer-divider" />
        <div class="footer-item">
          <span class="footer-label">NY</span>
          <span class="footer-value">{{ nyTime }}</span>
        </div>
      </div>

      <v-spacer />

      <!-- GROUP 2: Connections -->
      <div class="footer-group">
        <div class="footer-item">
          <v-icon :color="wsColor" size="0.55rem" class="mr-1">mdi-circle</v-icon>
          <span class="footer-label">WS</span>
          <span :class="['footer-value', 'status-text', `status-${wsStatus}`]">
            {{ wsStatus.toUpperCase() }}
          </span>
        </div>
        <div class="footer-divider" />
        <div class="footer-item">
          <v-icon :color="httpColor" size="0.55rem" class="mr-1">mdi-circle</v-icon>
          <span class="footer-label">HTTP</span>
          <span :class="['footer-value', 'status-text', `status-${httpStatus}`]">
            {{ httpStatus.toUpperCase() }}
          </span>
          <v-btn
            icon
            variant="plain"
            size="x-small"
            density="compact"
            class="refresh-btn ml-1"
            :loading="httpRefreshing"
            @click="refreshHttp"
          >
            <v-icon size="0.65rem">mdi-refresh</v-icon>
          </v-btn>
        </div>
      </div>

      <v-spacer />

      <!-- GROUP 3: System -->
      <div class="footer-group">
        <div class="footer-item">
          <v-icon size="0.65rem" class="mr-1" color="rgba(255,255,255,0.4)">mdi-cpu-64-bit</v-icon>
          <span class="footer-label">CPU</span>
          <span class="footer-value" :class="cpuWarnClass">{{ cpu }}%</span>
          <div class="mini-bar ml-1">
            <div class="mini-bar-fill" :class="cpuWarnClass" :style="{ width: cpu + '%' }" />
          </div>
        </div>
        <div class="footer-divider" />
        <div class="footer-item">
          <v-icon size="0.65rem" class="mr-1" color="rgba(255,255,255,0.4)">mdi-memory</v-icon>
          <span class="footer-label">RAM</span>
          <span class="footer-value" :class="ramWarnClass">{{ ramUsed.toFixed(1) }} / {{ ramMax }} GB</span>
          <div class="mini-bar ml-1">
            <div class="mini-bar-fill" :class="ramWarnClass" :style="{ width: ramPct + '%' }" />
          </div>
        </div>
      </div>

    </div>
  </v-footer>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

// ── Time ──────────────────────────────────────────────────────────────────────
const now = ref(new Date())
let ticker: ReturnType<typeof setInterval>

const fmt = (d: Date, tz?: string) =>
  d.toLocaleTimeString('en-US', {
    hour: '2-digit', minute: '2-digit', second: '2-digit',
    hour12: false, timeZone: tz
  })

const localTime = computed(() => fmt(now.value))
const utcTime   = computed(() => fmt(now.value, 'UTC'))
const nyTime    = computed(() => fmt(now.value, 'America/New_York'))

// ── WebSocket ─────────────────────────────────────────────────────────────────
type Status = 'connected' | 'disconnected' | 'connecting'
const wsStatus = ref<Status>('connected')
const wsColor  = computed(() => ({ connected: 'success', disconnected: 'error', connecting: 'warning' }[wsStatus.value]))

// ── HTTP ──────────────────────────────────────────────────────────────────────
const httpStatus     = ref<Status>('connected')
const httpRefreshing = ref(false)
const httpColor      = computed(() => ({ connected: 'success', disconnected: 'error', connecting: 'warning' }[httpStatus.value]))

async function refreshHttp() {
  httpRefreshing.value = true
  httpStatus.value = 'connecting'
  // Replace with real health-check call
  await new Promise(r => setTimeout(r, 800))
  httpStatus.value = 'connected'
  httpRefreshing.value = false
}

// ── System ────────────────────────────────────────────────────────────────────
const cpu    = ref(34)
const ramUsed = ref(5.2)
const ramMax  = ref(16)
const ramPct  = computed(() => (ramUsed.value / ramMax.value) * 100)

const cpuWarnClass = computed(() => cpu.value > 85 ? 'val-error' : cpu.value > 65 ? 'val-warn' : '')
const ramWarnClass = computed(() => ramPct.value > 85 ? 'val-error' : ramPct.value > 65 ? 'val-warn' : '')

// Simulate live metrics — replace with real polling/SSE
function updateMetrics() {
  cpu.value = Math.min(99, Math.max(5, cpu.value + (Math.random() * 6 - 3)))
  ramUsed.value = Math.min(ramMax.value, Math.max(0.5, ramUsed.value + (Math.random() * 0.2 - 0.1)))
}

onMounted(() => {
  ticker = setInterval(() => { now.value = new Date(); updateMetrics() }, 1000)
})
onUnmounted(() => clearInterval(ticker))
</script>

<style scoped>
.app-footer {
  background-color: rgb(var(--v-theme-surface));
  border-top: thin solid rgba(var(--v-theme-on-surface), 0.1);
  min-height: 1.75rem;
  font-family: 'JetBrains Mono', 'Fira Mono', monospace;
}

.footer-inner {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 0.25rem 0.5rem;
  width: 100%;
  min-height: 1.75rem;
  padding: 0.2rem 0;
}

.footer-group {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  flex-wrap: wrap;
}

.footer-item {
  display: flex;
  align-items: center;
  gap: 0.2rem;
  white-space: nowrap;
}

.footer-label {
  font-size: 0.6rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  color: rgba(var(--v-theme-on-surface), 0.38);
  text-transform: uppercase;
}

.footer-value {
  font-size: 0.65rem;
  font-weight: 500;
  color: rgba(var(--v-theme-on-surface), 0.75);
  letter-spacing: 0.02em;
}

.footer-divider {
  width: 1px;
  height: 0.75rem;
  background-color: rgba(var(--v-theme-on-surface), 0.12);
}

/* Status colors */
.status-connected    { color: rgb(var(--v-theme-success)) !important; }
.status-disconnected { color: rgb(var(--v-theme-error))   !important; }
.status-connecting   { color: rgb(var(--v-theme-warning)) !important; }

/* Value warning states */
.val-warn  { color: rgb(var(--v-theme-warning)) !important; }
.val-error { color: rgb(var(--v-theme-error))   !important; }

/* Mini progress bar */
.mini-bar {
  width: 2.5rem;
  height: 0.2rem;
  background-color: rgba(var(--v-theme-on-surface), 0.1);
  border-radius: 0.1rem;
  overflow: hidden;
}

.mini-bar-fill {
  height: 100%;
  background-color: rgba(var(--v-theme-on-surface), 0.4);
  border-radius: 0.1rem;
  transition: width 0.8s ease;
}

.mini-bar-fill.val-warn  { background-color: rgb(var(--v-theme-warning)); }
.mini-bar-fill.val-error { background-color: rgb(var(--v-theme-error)); }

.refresh-btn {
  opacity: 0.5;
  transition: opacity 0.15s;
}
.refresh-btn:hover { opacity: 1; }
</style>
