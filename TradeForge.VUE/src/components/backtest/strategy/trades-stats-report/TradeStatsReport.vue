<style scoped src="./TradeStatsReport.css">

</style>

<template>
  <div class="trades-container">
    <table class="custom-table">
      <tbody>

      <!-- Title -->
      <tr class="title-row">
        <td colspan="8">Trades</td>
      </tr>

      <!-- Row 1: wins / losses / cancelled -->
      <tr>
        <td colspan="2"></td>
        <td class="label"># of Wins</td>
        <td class="value">{{ data.wins }}</td>
        <td class="label"># of Losses</td>
        <td class="value">{{ data.losses }}</td>
        <td class="label"># of Cancelled/Expired</td>
        <td class="value">{{ data.canceledOrExpired }}</td>
      </tr>

      <!-- Row 2: gross profit/loss, avg win/loss -->
      <tr>
        <td class="label">Gross Profit</td>
        <td class="value">{{ formatMoney(data.grossProfit) }}</td>
        <td class="label">Gross Loss</td>
        <td :class="['value', data.grossLoss < 0 ? 'value-negative' : '']">
          {{ formatMoney(data.grossLoss) }}
        </td>
        <td class="label">Average Win</td>
        <td class="value">{{ formatMoney(data.avgWin) }}</td>
        <td class="label">Average Loss</td>
        <td :class="['value', data.avgLoss < 0 ? 'value-negative' : '']">
          {{ formatMoney(data.avgLoss) }}
        </td>
      </tr>

      <!-- Row 3: largest win/loss, max consec -->
      <tr>
        <td class="label">Largest Win</td>
        <td class="value">{{ formatMoney(data.largestWin) }}</td>
        <td class="label">Largest Loss</td>
        <td :class="['value', data.largestLoss < 0 ? 'value-negative' : '']">
          {{ formatMoney(data.largestLoss) }}
        </td>
        <td class="label">Max Consec Wins</td>
        <td class="value">{{ data.maxConsWins }}</td>
        <td class="label">Max Consec Losses</td>
        <td class="value">{{ data.maxConsLosses }}</td>
      </tr>

      <!-- Row 4: avg consec, avg bars -->
      <tr>
        <td class="label">Avg Consec Wins</td>
        <td class="value">{{ data.avgConsWins }}</td>
        <td class="label">Avg Consec Loss</td>
        <td class="value">{{ data.avgConsLosses }}</td>
        <td class="label">Avg # of Bars in Wins</td>
        <td class="value">{{ data.avgBarsInWins }}</td>
        <td class="label">Avg # of Bars in Losses</td>
        <td class="value">{{ data.avgBarsInLosses }}</td>
      </tr>

      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import type {TradesReport} from '@/types/strategy/TradesReport.interface';

interface Props {
  data: TradesReport;
}

defineProps<Props>();

function formatMoney(value: number): string {
  const abs = Math.abs(value);
  const formatted = `$ ${+abs.toFixed(2)}`;
  return value < 0 ? `- ${formatted}` : formatted;
}
</script>
