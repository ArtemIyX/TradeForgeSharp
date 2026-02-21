<style scoped src="./TradesTable.css"></style>

<template>
  <div class="trades-table-container">
    <div class="table-title">Trade History</div>

    <div class="table-wrapper">
      <table class="custom-table">
        <thead class="table-head">
        <tr>
          <th>Ticket</th>
          <th>Symbol</th>
          <th>Open Time</th>
          <th>Open Price</th>
          <th>Close Time</th>
          <th>Close Price</th>
          <th>Size</th>
          <th>Close Type</th>
          <th class="th-right">P / L</th>
          <th class="th-right">Balance After</th>
          <th class="th-right">MAE</th>
          <th class="th-right">MFE</th>
          <th>Time in Trade</th>
          <th>Comment</th>
        </tr>
        </thead>
        <tbody>
        <tr v-if="!items.length">
          <td colspan="14" class="empty-state">No trades available</td>
        </tr>
        <tr
          v-for="trade in items"
          :key="trade.ticket"
          class="data-row"
          :class="trade.profitOrLoss >= 0 ? 'row-win' : 'row-loss'"
        >
          <td class="td-ticket">{{ trade.ticket }}</td>
          <td class="td-symbol">{{ trade.symbol }}</td>
          <td>{{ formatDateTime(trade.openTime) }}</td>
          <td class="td-price">{{ formatPrice(trade.openPrice) }}</td>
          <td>{{ formatDateTime(trade.closeTime) }}</td>
          <td class="td-price">{{ formatPrice(trade.closePrice) }}</td>
          <td>{{ trade.size }}</td>
          <td>
            <span class="close-type-badge">{{ trade.closeType }}</span>
          </td>
          <td :class="['td-right', plClass(trade.profitOrLoss)]">
            {{ formatMoney(trade.profitOrLoss) }}
          </td>
          <td class="td-right">{{ formatMoney(trade.balanceAfter) }}</td>
          <td :class="['td-right', 'profit-negative']">{{ formatMoney(trade.mae) }}</td>
          <td :class="['td-right', 'profit-positive']">{{ formatMoney(trade.mfe) }}</td>
          <td class="td-time">{{ trade.timeInTrade }}</td>
          <td class="td-comment">{{ trade.comment }}</td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { StrategyTradeItem } from '@/types/strategy/StrategyTradeItem.interface';

interface Props {
  items: StrategyTradeItem[];
}

defineProps<Props>();

function formatDateTime(date: string): string {
  const d = new Date(date);
  return d.toLocaleString('en-US', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false,
  });
}

function formatPrice(value: number): string {
  return value.toFixed(5);
}

function plClass(value: number): string {
  if (value > 0) return 'profit-positive';
  if (value < 0) return 'profit-negative';
  return '';
}

function formatMoney(value: number): string {
  const abs = Math.abs(value);
  const formatted = `$ ${abs.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  return value < 0 ? `- ${formatted}` : formatted;
}
</script>
