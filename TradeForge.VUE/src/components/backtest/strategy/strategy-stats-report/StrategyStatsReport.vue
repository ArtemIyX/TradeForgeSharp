<style scoped src="./StrategyStatsReport.css"></style>

<template>
  <div class="report-wrapper">
    <div class="section-container">
      <table class="custom-table">

        <!-- Strategy Stats -->
        <tbody>
        <tr class="title-row">
          <td colspan="6">Strategy</td>
        </tr>
        <tr>
          <td class="label">Wins / Losses Ratio</td>
          <td class="value">{{ fmt(stats.winLossRatio) }}</td>
          <td class="label">Payout Ratio (Avg Win/Loss)</td>
          <td class="value">{{ fmt(stats.payoutRatio) }}</td>
          <td class="label">Average # of Bars in Trade</td>
          <td class="value">{{ fmt(stats.avgBarsInTrade) }}</td>
        </tr>
        <tr>
          <td class="label">AHPR</td>
          <td class="value">{{ fmt(stats.ahpr) }}</td>
          <td class="label">Z-Score</td>
          <td class="value">{{ fmt(stats.zScore) }}</td>
          <td class="label">Z-Probability</td>
          <td class="value">{{ fmtPct(stats.zProbability) }}</td>
        </tr>
        <tr>
          <td class="label">Expectancy</td>
          <td class="value">{{ fmt(stats.expectancy) }}</td>
          <td class="label">Deviation</td>
          <td class="value">{{ fmtMoney(stats.deviation) }}</td>
          <td class="label">Exposure</td>
          <td class="value">{{ fmtPct(stats.exposure) }}</td>
        </tr>
        <tr>
          <td class="label">Stagnation in Days</td>
          <td class="value">{{ stats.stagnationInDays }}</td>
          <td class="label">Stagnation in %</td>
          <td class="value">{{ fmtPct(stats.stagnationInPercent) }}</td>
          <td></td>
          <td></td>
        </tr>
        </tbody>

        <!-- Divider -->
        <tbody>
        <tr class="divider-row">
          <td colspan="6"></td>
        </tr>
        </tbody>

        <!-- Risk-Adjusted Ratios -->
        <tbody>
        <tr class="title-row">
          <td colspan="6">Risk-Adjusted Ratios</td>
        </tr>
        <tr v-for="(chunk, i) in ratioChunks" :key="i">
          <template v-for="item in chunk" :key="item.key">
            <td class="label-ratio">
              <div class="label-with-tooltip">
                <span>{{ item.label }}</span>
                <v-tooltip :text="item.tooltip" location="top">
                  <template #activator="{ props: tip }">
                    <v-icon v-bind="tip" class="tooltip-icon" size="0.875rem">mdi-information-outline</v-icon>
                  </template>
                </v-tooltip>
              </div>
            </td>
            <td class="value">{{ fmt(ratio[item.key]) }}</td>
          </template>
          <!-- fill remaining columns if chunk has < 3 items -->
          <template v-if="chunk.length < 3">
            <td v-for="n in (3 - chunk.length) * 2" :key="'empty-' + n"></td>
          </template>
        </tr>
        </tbody>

      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type {
  StrategyRatioReport,
  StrategyStatsReport
} from '@/types/strategy/StrategyStatsReport.interface';

interface Props {
  ratio: StrategyRatioReport;
  stats: StrategyStatsReport
}

defineProps<Props>();

// ── Formatters ────────────────────────────────────────────────────────────────
function fmt(v: number): string {
  return +v.toFixed(2) + '';
}
function fmtPct(v: number): string {
  return `${+v.toFixed(2)} %`;
}
function fmtMoney(v: number): string {
  return `$ ${+Math.abs(v).toFixed(2)}`;
}

// ── Ratio definitions with tooltips ──────────────────────────────────────────
const ratioItems: { key: keyof StrategyRatioReport; label: string; tooltip: string }[] = [
  {
    key: 'sharpeRatio',
    label: 'Sharpe Ratio',
    tooltip: 'Measures excess return per unit of total risk (std deviation). Higher is better. Above 1.0 is considered acceptable, above 2.0 is strong.',
  },
  {
    key: 'sortinoRatio',
    label: 'Sortino Ratio',
    tooltip: 'Like Sharpe, but only penalizes downside volatility. Better suited for asymmetric return distributions. Higher is better.',
  },
  {
    key: 'calmarRatio',
    label: 'Calmar Ratio',
    tooltip: 'Annualized return divided by maximum drawdown. Measures return per unit of drawdown risk. Higher is better.',
  },
  {
    key: 'sterlingRatio',
    label: 'Sterling Ratio',
    tooltip: 'Similar to Calmar but uses average drawdown instead of max. Less sensitive to a single extreme drawdown event.',
  },
  {
    key: 'omegaRatio',
    label: 'Omega Ratio',
    tooltip: 'Ratio of probability-weighted gains to probability-weighted losses above/below a threshold. Above 1.0 means gains outweigh losses.',
  },
  {
    key: 'marRatio',
    label: 'MAR Ratio',
    tooltip: 'Managed Account Ratio — compounded annual growth rate divided by max drawdown. A simple measure of return efficiency relative to risk.',
  },
];

// Chunk ratios into rows of 3 (to match the 6-column layout: label+value × 3)
const ratioChunks = computed(() => {
  const size = 3;
  const result = [];
  for (let i = 0; i < ratioItems.length; i += size) {
    result.push(ratioItems.slice(i, i + size));
  }
  return result;
});
</script>
