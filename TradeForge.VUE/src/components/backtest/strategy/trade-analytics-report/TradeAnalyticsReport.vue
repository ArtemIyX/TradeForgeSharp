<template>
  <div class="analytics-report">
    <!-- Loading overlay -->
    <div v-if="loading" class="analytics-report__loader">
      <v-progress-circular indeterminate color="primary" size="48" />
    </div>

    <template v-else>
      <!-- Row 1: PnL Overview -->
      <div class="analytics-report__section-title">P&amp;L Overview</div>
      <div class="analytics-report__grid analytics-report__grid--wide">
        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">P&L by Year</v-card-title>
          <v-card-text class="analytics-card__body">
            <GBarChart
              v-if="data?.pnlByYear"
              :data="data.pnlByYear"
              positive-color="#4CAF50"
              negative-color="#F44336"
              :y-formatter="fmtMoney"
              height="16rem"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>

        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">Long vs Short P&L</v-card-title>
          <v-card-text class="analytics-card__body">
            <GBarChart
              v-if="data?.longShortPnl"
              :data="longShortPnlData"
              positive-color="#4CAF50"
              negative-color="#F44336"
              :y-formatter="fmtMoney"
              height="16rem"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>

      <!-- Row 2: Pie charts -->
      <div class="analytics-report__section-title">Composition</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">Long vs Short Trades</v-card-title>
          <v-card-text class="analytics-card__body">
            <GPieChart v-if="data?.longShortTrades" :data="longShortTradesPie" height="16rem" />
            <NoData v-else />
          </v-card-text>
        </v-card>

        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">Profit / Loss</v-card-title>
          <v-card-text class="analytics-card__body">
            <GPieChart v-if="data?.profitLossPie" :data="profitLossPieSlices" height="16rem" />
            <NoData v-else />
          </v-card-text>
        </v-card>

        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">Long Profit / Loss</v-card-title>
          <v-card-text class="analytics-card__body">
            <GPieChart v-if="data?.longProfitLossPie" :data="longProfitLossSlices" height="16rem" />
            <NoData v-else />
          </v-card-text>
        </v-card>

        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">Short Profit / Loss</v-card-title>
          <v-card-text class="analytics-card__body">
            <GPieChart v-if="data?.shortProfitLossPie" :data="shortProfitLossSlices" height="16rem" />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>

      <!-- Row 3: Duration scatter -->
      <div class="analytics-report__section-title">Duration Analysis</div>
      <div class="analytics-report__grid analytics-report__grid--full">
        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">P&L Growth by Duration</v-card-title>
          <v-card-text class="analytics-card__body">
            <GScatterChart
              v-if="data?.pnlByDuration"
              :series="pnlByDurationSeries"
              :x-formatter="fmtDuration"
              :y-formatter="fmtMoney"
              x-axis-name="Duration"
              y-axis-name="Profit / Loss ($)"
              height="18rem"
              :zoom="true"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>

      <!-- Row 4: P&L by time period -->
      <div class="analytics-report__section-title">P&amp;L by Period</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <v-card class="analytics-card" v-for="item in pnlByPeriodCards" :key="item.title">
          <v-card-title class="analytics-card__title">{{ item.title }}</v-card-title>
          <v-card-text class="analytics-card__body">
            <GBarChart
              v-if="item.chartData"
              :data="item.chartData"
              positive-color="#4CAF50"
              negative-color="#F44336"
              :y-formatter="fmtMoney"
              height="16rem"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>

      <!-- Row 5: Trade count by period -->
      <div class="analytics-report__section-title">Trades by Period</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <v-card class="analytics-card" v-for="item in tradesByPeriodCards" :key="item.title">
          <v-card-title class="analytics-card__title">{{ item.title }}</v-card-title>
          <v-card-text class="analytics-card__body">
            <GBarChart
              v-if="item.chartData"
              :data="item.chartData"
              bar-color="#5C6BC0"
              height="16rem"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>

      <div class="analytics-report__grid analytics-report__grid--half">
        <v-card class="analytics-card">
          <v-card-title class="analytics-card__title">Trades by Year</v-card-title>
          <v-card-text class="analytics-card__body">
            <GBarChart
              v-if="data?.tradesByYear"
              :data="data.tradesByYear"
              bar-color="#5C6BC0"
              height="16rem"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>

      <!-- Row 6: Win/Loss counts grouped -->
      <div class="analytics-report__section-title">Win / Loss Count</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <v-card class="analytics-card" v-for="item in winLossCountCards" :key="item.title">
          <v-card-title class="analytics-card__title">{{ item.title }}</v-card-title>
          <v-card-text class="analytics-card__body">
            <GGroupBarChart
              v-if="item.groupedData"
              :series="buildWinLossSeries(item.groupedData)"
              height="16rem"
              :show-legend="true"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>

      <!-- Row 7: Win/Loss profit grouped -->
      <div class="analytics-report__section-title">Win / Loss Profit</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <v-card class="analytics-card" v-for="item in winLossProfitCards" :key="item.title">
          <v-card-title class="analytics-card__title">{{ item.title }}</v-card-title>
          <v-card-text class="analytics-card__body">
            <GGroupBarChart
              v-if="item.groupedData"
              :series="buildWinLossProfitSeries(item.groupedData)"
              :y-formatter="fmtMoney"
              height="16rem"
              :show-legend="true"
            />
            <NoData v-else />
          </v-card-text>
        </v-card>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, defineComponent, h } from 'vue'
import type {
  TradeAnalyticsData,
  WinLossGroupedData,
  WinLossProfitGroupedData,
} from '@/types/strategy/TradeAnalyticsReport.interface'
import GBarChart from '@/components/shared/charts/GBarChart.vue'
import GGroupBarChart from '@/components/shared/charts/GGroupBarChart.vue'
import GScatterChart from '@/components/shared/charts/GScatterChart.vue'
import GPieChart from '@/components/shared/charts/GPieChart.vue'
import type { GroupedBarSeries } from '@/components/shared/charts/GGroupBarChart.vue'
import type { ScatterChartSeries } from '@/components/shared/charts/GScatterChart.vue'

// ── Props ─────────────────────────────────────────────────────────────────────

interface Props {
  data: TradeAnalyticsData | null
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
})

// ── Inline no-data component ──────────────────────────────────────────────────

const NoData = defineComponent({
  setup() {
    return () =>
      h(
        'div',
        { class: 'analytics-no-data' },
        h('span', { class: 'text-medium-emphasis font-italic text-body-2' }, 'No data'),
      )
  },
})

// ── Formatters ────────────────────────────────────────────────────────────────

function fmtMoney(v: number): string {
  const abs = Math.abs(v)
  const str = `$${abs.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
  return v < 0 ? `-${str}` : str
}

function fmtDuration(seconds: number): string {
  if (seconds < 60) return `${seconds}s`
  if (seconds < 3600) return `${Math.floor(seconds / 60)}m`
  if (seconds < 86400) return `${Math.floor(seconds / 3600)}h`
  return `${Math.floor(seconds / 86400)}d`
}

// ── Computed data transforms ──────────────────────────────────────────────────

const longShortPnlData = computed(() => {
  const d = props.data?.longShortPnl
  if (!d) return []
  return [
    { x: 'Long P/L', y: d.longPnl },
    { x: 'Short P/L', y: d.shortPnl },
  ]
})

const longShortTradesPie = computed(() => {
  const d = props.data?.longShortTrades
  if (!d) return []
  return [
    { name: 'Long', value: d.longCount, color: '#5C6BC0' },
    { name: 'Short', value: d.shortCount, color: '#AB47BC' },
  ]
})

const profitLossPieSlices = computed(() => {
  const d = props.data?.profitLossPie
  if (!d) return []
  return [
    { name: 'Profit', value: d.totalProfit, color: '#4CAF50' },
    { name: 'Loss', value: Math.abs(d.totalLoss), color: '#F44336' },
  ]
})

const longProfitLossSlices = computed(() => {
  const d = props.data?.longProfitLossPie
  if (!d) return []
  return [
    { name: 'Profit', value: d.profit, color: '#4CAF50' },
    { name: 'Loss', value: Math.abs(d.loss), color: '#F44336' },
  ]
})

const shortProfitLossSlices = computed(() => {
  const d = props.data?.shortProfitLossPie
  if (!d) return []
  return [
    { name: 'Profit', value: d.profit, color: '#4CAF50' },
    { name: 'Loss', value: Math.abs(d.loss), color: '#F44336' },
  ]
})

const pnlByDurationSeries = computed((): ScatterChartSeries[] => {
  const points = props.data?.pnlByDuration
  if (!points) return []
  const wins = points.filter(p => p.y >= 0)
  const losses = points.filter(p => p.y < 0)
  return [
    { name: 'Profit', data: wins, color: '#4CAF50' },
    { name: 'Loss', data: losses, color: '#F44336' },
  ]
})

// ── Card config arrays (reduces template repetition) ─────────────────────────

const pnlByPeriodCards = computed(() => [
  { title: 'P&L by Hour',    chartData: props.data?.pnlByHour },
  { title: 'P&L by Weekday', chartData: props.data?.pnlByWeekday },
  { title: 'P&L by Month',   chartData: props.data?.pnlByMonth },
  { title: 'P&L by Day',     chartData: props.data?.pnlByDay },
])

const tradesByPeriodCards = computed(() => [
  { title: 'Trades by Hour',    chartData: props.data?.tradesByHour },
  { title: 'Trades by Weekday', chartData: props.data?.tradesByWeekday },
  { title: 'Trades by Month',   chartData: props.data?.tradesByMonth },
  { title: 'Trades by Day',     chartData: props.data?.tradesByDay },
])

const winLossCountCards = computed(() => [
  { title: 'Wins / Losses by Hour',    groupedData: props.data?.winLossByHour },
  { title: 'Wins / Losses by Weekday', groupedData: props.data?.winLossByWeekday },
  { title: 'Wins / Losses by Month',   groupedData: props.data?.winLossByMonth },
  { title: 'Wins / Losses by Day',     groupedData: props.data?.winLossByDay },
])

const winLossProfitCards = computed(() => [
  { title: 'Wins / Losses Profit by Hour',    groupedData: props.data?.winLossProfitByHour },
  { title: 'Wins / Losses Profit by Weekday', groupedData: props.data?.winLossProfitByWeekday },
  { title: 'Wins / Losses Profit by Month',   groupedData: props.data?.winLossProfitByMonth },
  { title: 'Wins / Losses Profit by Day',     groupedData: props.data?.winLossProfitByDay },
])

// ── Grouped bar helpers ───────────────────────────────────────────────────────

function buildWinLossSeries(d: WinLossGroupedData): GroupedBarSeries[] {
  return [
    {
      name: 'Wins',
      color: '#4CAF50',
      data: d.categories.map((x, i) => ({ x, y: d.wins[i] ?? 0 })),
    },
    {
      name: 'Losses',
      color: '#F44336',
      data: d.categories.map((x, i) => ({ x, y: d.losses[i] ?? 0 })),
    },
  ]
}

function buildWinLossProfitSeries(d: WinLossProfitGroupedData): GroupedBarSeries[] {
  return [
    {
      name: 'Win Profit',
      color: '#4CAF50',
      data: d.categories.map((x, i) => ({ x, y: d.winProfits[i] ?? 0 })),
    },
    {
      name: 'Loss',
      color: '#F44336',
      data: d.categories.map((x, i) => ({ x, y: d.lossProfits[i] ?? 0 })),
    },
  ]
}
</script>

<style scoped>
.analytics-report {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  padding: 1rem;
  min-height: 10rem;
}

/* ── Loader ── */
.analytics-report__loader {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: rgba(var(--v-theme-surface), 0.7);
  z-index: 10;
  border-radius: 0.25rem;
}

/* ── Section title ── */
.analytics-report__section-title {
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: rgba(var(--v-theme-on-surface), 0.4);
  padding: 0.5rem 0 0.25rem;
  border-bottom: thin solid rgba(var(--v-theme-on-surface), 0.08);
  margin-top: 0.5rem;
}

/* ── Grid layouts ── */
.analytics-report__grid {
  display: grid;
  gap: 0.75rem;
}

.analytics-report__grid--full {
  grid-template-columns: 1fr;
}

.analytics-report__grid--half {
  grid-template-columns: repeat(2, 1fr);
}

.analytics-report__grid--wide {
  grid-template-columns: repeat(2, 1fr);
}

.analytics-report__grid--quad {
  grid-template-columns: repeat(4, 1fr);
}

@media (max-width: 75rem) {
  .analytics-report__grid--quad {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 40rem) {
  .analytics-report__grid--wide,
  .analytics-report__grid--half,
  .analytics-report__grid--quad {
    grid-template-columns: 1fr;
  }
}

/* ── Card overrides ── */
.analytics-card {
  display: flex;
  flex-direction: column;
}

.analytics-card :deep(.v-card-title.analytics-card__title) {
  font-size: 0.75rem;
  font-weight: 600;
  letter-spacing: 0.04em;
  color: rgba(var(--v-theme-on-surface), 0.6);
  padding: 0.5rem 0.75rem;
  border-bottom: thin solid rgba(var(--v-theme-on-surface), 0.08);
  background-color: rgba(var(--v-theme-on-surface), 0.02);
  line-height: 1.4;
}

.analytics-card :deep(.v-card-text.analytics-card__body) {
  flex: 1;
  padding: 0.5rem;
  display: flex;
  flex-direction: column;
}

/* ── No data ── */
.analytics-no-data {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 16rem;
}
</style>
