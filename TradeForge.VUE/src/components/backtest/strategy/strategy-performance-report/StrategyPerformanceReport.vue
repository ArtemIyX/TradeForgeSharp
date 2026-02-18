<template>
  <div class="perf-report">
    <!-- Left: Total Profit Summary -->
    <div class="perf-report__summary">
      <div class="perf-report__label">TOTAL PROFIT</div>
      <div class="perf-report__total-profit" :class="profitColorClass(data.totaProfit)">
        {{ formatCurrency(data.totaProfit) }}
      </div>

      <v-divider class="my-2" />

      <div class="perf-report__meta-row">
        <span class="perf-report__label">PROFIT IN PIPS</span>
        <span :class="profitColorClass(data.profitInPips)">
          {{ data.profitInPips }} TICKS
        </span>
      </div>
      <div class="perf-report__meta-row">
        <span class="perf-report__label">YEARLY AVG PROFIT</span>
        <span :class="profitColorClass(data.yearlyAvgProfit)">
          {{ formatCurrency(data.yearlyAvgProfit) }}
        </span>
      </div>
      <div class="perf-report__meta-row">
        <span class="perf-report__label">YEARLY AVG % RETURN</span>
        <span :class="profitColorClass(data.yearlyAvgProfitPercent)">
          {{ formatPercent(data.yearlyAvgProfitPercent) }}
        </span>
      </div>
      <div class="perf-report__meta-row">
        <span class="perf-report__label">CAGR</span>
        <span :class="profitColorClass(data.cagr)">
          {{ formatPercent(data.cagr) }}
        </span>
      </div>
    </div>

    <!-- Right: Stats Grid -->
    <div class="perf-report__grid">
      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label"># OF TRADES</div>
        <div class="perf-report__stat-value">{{ data.numberOfTrades }}</div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">PROFIT FACTOR</div>
        <div
          class="perf-report__stat-value"
          :class="{ 'text-error': data.profitFactor < 0 }"
        >
          {{ data.profitFactor.toFixed(2) }}
        </div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">RETURN / DD RATIO</div>
        <div
          class="perf-report__stat-value"
          :class="{ 'text-error': data.returnDdRatio < 0 }"
        >
          {{ data.returnDdRatio.toFixed(2) }}
        </div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">WINNING PERCENTAGE</div>
        <div class="perf-report__stat-value">
          {{ formatPercent(data.winningPercentage) }}
        </div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">DRAWDOWN</div>
        <div class="perf-report__stat-value">
          {{ formatCurrency(data.drawdown) }}
        </div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">% DRAWDOWN</div>
        <div class="perf-report__stat-value">
          {{ formatPercent(data.drawdownPercent) }}
        </div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">DAILY AVG PROFIT</div>
        <div class="perf-report__stat-value">
          {{ formatCurrency(data.dailyAvgProfit) }}
        </div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">MONTHLY AVG PROFIT</div>
        <div class="perf-report__stat-value">
          {{ formatCurrency(data.monthlyAvgProfit) }}
        </div>
      </div>

      <div class="perf-report__stat-cell">
        <div class="perf-report__stat-label">AVERAGE TRADE</div>
        <div class="perf-report__stat-value">
          {{ formatCurrency(data.avgTradeProfit) }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { StrategyPerformanceReportData } from '@/types/strategy/StrategyPerformanceReport.interface.ts';

const props = defineProps<{
  data: StrategyPerformanceReportData
}>()

function profitColorClass(value: number): string {
  return value >= 0 ? 'text-success' : 'text-error'
}

function formatCurrency(value: number): string {
  const abs = Math.abs(value)
  const formatted = `$ ${abs.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
  return value < 0 ? `- ${formatted}` : formatted
}

function formatPercent(value: number): string {
  return `${value.toFixed(2)} %`
}
</script>

<style scoped src="./StrategyPerformanceReport.css">

</style>
