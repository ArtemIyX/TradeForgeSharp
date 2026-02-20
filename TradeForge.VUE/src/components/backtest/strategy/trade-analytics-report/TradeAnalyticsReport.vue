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
        <AnalyticsCard title="P&L by Year">
          <GBarChart
            v-if="data?.pnlByYear"
            :data="data.pnlByYear"
            positive-color="#4CAF50"
            negative-color="#F44336"
            :y-formatter="fmtMoney"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Long vs Short P&L">
          <GBarChart
            v-if="data?.longShortPnl"
            :data="longShortPnlData"
            positive-color="#4CAF50"
            negative-color="#F44336"
            :y-formatter="fmtMoney"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>
      </div>

      <!-- Row 2: Pie charts -->
      <div class="analytics-report__section-title">Composition</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <AnalyticsCard title="Long vs Short Trades">
          <GPieChart
            v-if="data?.longShortTrades"
            :data="longShortTradesPie"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Profit / Loss">
          <GPieChart
            v-if="data?.profitLossPie"
            :data="profitLossPieSlices"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Long Profit / Loss">
          <GPieChart
            v-if="data?.longProfitLossPie"
            :data="longProfitLossSlices"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Short Profit / Loss">
          <GPieChart
            v-if="data?.shortProfitLossPie"
            :data="shortProfitLossSlices"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>
      </div>

      <!-- Row 3: Duration scatter -->
      <div class="analytics-report__section-title">Duration Analysis</div>
      <div class="analytics-report__grid analytics-report__grid--full">
        <AnalyticsCard title="P&L Growth by Duration">
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
        </AnalyticsCard>
      </div>

      <!-- Row 4: P&L by time period -->
      <div class="analytics-report__section-title">P&amp;L by Period</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <AnalyticsCard title="P&L by Hour">
          <GBarChart
            v-if="data?.pnlByHour"
            :data="data.pnlByHour"
            positive-color="#4CAF50"
            negative-color="#F44336"
            :y-formatter="fmtMoney"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="P&L by Weekday">
          <GBarChart
            v-if="data?.pnlByWeekday"
            :data="data.pnlByWeekday"
            positive-color="#4CAF50"
            negative-color="#F44336"
            :y-formatter="fmtMoney"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="P&L by Month">
          <GBarChart
            v-if="data?.pnlByMonth"
            :data="data.pnlByMonth"
            positive-color="#4CAF50"
            negative-color="#F44336"
            :y-formatter="fmtMoney"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="P&L by Day">
          <GBarChart
            v-if="data?.pnlByDay"
            :data="data.pnlByDay"
            positive-color="#4CAF50"
            negative-color="#F44336"
            :y-formatter="fmtMoney"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>
      </div>

      <!-- Row 5: Trade count by period -->
      <div class="analytics-report__section-title">Trades by Period</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <AnalyticsCard title="Trades by Hour">
          <GBarChart
            v-if="data?.tradesByDay"
            :data="data.tradesByDay"
            bar-color="#5C6BC0"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Trades by Weekday">
          <GBarChart
            v-if="data?.tradesByWeekday"
            :data="data.tradesByWeekday"
            bar-color="#5C6BC0"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Trades by Month">
          <GBarChart
            v-if="data?.tradesByMonth"
            :data="data.tradesByMonth"
            bar-color="#5C6BC0"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Trades by Day">
          <GBarChart
            v-if="data?.tradesByDay"
            :data="data.tradesByDay"
            bar-color="#5C6BC0"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>
      </div>

      <div class="analytics-report__grid analytics-report__grid--half">
        <AnalyticsCard title="Trades by Year">
          <GBarChart
            v-if="data?.tradesByYear"
            :data="data.tradesByYear"
            bar-color="#5C6BC0"
            height="16rem"
          />
          <NoData v-else />
        </AnalyticsCard>
      </div>

      <!-- Row 6: Win/Loss counts grouped -->
      <div class="analytics-report__section-title">Win / Loss Count</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <AnalyticsCard title="Wins / Losses by Hour">
          <GGroupBarChart
            v-if="data?.winLossByHour"
            :series="buildWinLossSeries(data.winLossByHour)"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Wins / Losses by Weekday">
          <GGroupBarChart
            v-if="data?.winLossByWeekday"
            :series="buildWinLossSeries(data.winLossByWeekday)"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Wins / Losses by Month">
          <GGroupBarChart
            v-if="data?.winLossByMonth"
            :series="buildWinLossSeries(data.winLossByMonth)"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Wins / Losses by Day">
          <GGroupBarChart
            v-if="data?.winLossByDay"
            :series="buildWinLossSeries(data.winLossByDay)"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>
      </div>

      <!-- Row 7: Win/Loss profit grouped -->
      <div class="analytics-report__section-title">Win / Loss Profit</div>
      <div class="analytics-report__grid analytics-report__grid--quad">
        <AnalyticsCard title="Wins / Losses Profit by Hour">
          <GGroupBarChart
            v-if="data?.winLossProfitByHour"
            :series="buildWinLossProfitSeries(data.winLossProfitByHour)"
            :y-formatter="fmtMoney"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Wins / Losses Profit by Weekday">
          <GGroupBarChart
            v-if="data?.winLossProfitByWeekday"
            :series="buildWinLossProfitSeries(data.winLossProfitByWeekday)"
            :y-formatter="fmtMoney"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Wins / Losses Profit by Month">
          <GGroupBarChart
            v-if="data?.winLossProfitByMonth"
            :series="buildWinLossProfitSeries(data.winLossProfitByMonth)"
            :y-formatter="fmtMoney"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>

        <AnalyticsCard title="Wins / Losses Profit by Day">
          <GGroupBarChart
            v-if="data?.winLossProfitByDay"
            :series="buildWinLossProfitSeries(data.winLossProfitByDay)"
            :y-formatter="fmtMoney"
            height="16rem"
            :show-legend="true"
          />
          <NoData v-else />
        </AnalyticsCard>
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

// ── Inline sub-components ─────────────────────────────────────────────────────

const AnalyticsCard = defineComponent({
  props: { title: String },
  setup(p, { slots }) {
    return () =>
      h('div', { class: 'analytics-card' }, [
        h('div', { class: 'analytics-card__title' }, p.title),
        h('div', { class: 'analytics-card__body' }, slots.default?.()),
      ])
  },
})

const NoData = defineComponent({
  setup() {
    return () => h('div', { class: 'analytics-no-data' }, 'No data')
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

/* ── Card ── */
.analytics-card {
  border: thin solid rgba(var(--v-theme-on-surface), 0.12);
  border-radius: 0.25rem;
  overflow: hidden;
  background-color: rgb(var(--v-theme-surface));
  display: flex;
  flex-direction: column;
}

.analytics-card__title {
  font-size: 0.75rem;
  font-weight: 600;
  letter-spacing: 0.04em;
  color: rgba(var(--v-theme-on-surface), 0.6);
  padding: 0.5rem 0.75rem;
  border-bottom: thin solid rgba(var(--v-theme-on-surface), 0.08);
  background-color: rgba(var(--v-theme-on-surface), 0.02);
}

.analytics-card__body {
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
  color: rgba(var(--v-theme-on-surface), 0.3);
  font-size: 0.875rem;
  font-style: italic;
}
</style>
