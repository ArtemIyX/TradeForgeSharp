import type { BarChartDataPoint } from '@/components/shared/charts/GBarChart.vue'
import type { GroupedBarDataPoint } from '@/components/shared/charts/GGroupBarChart.vue'
import type { ScatterChartDataPoint } from '@/components/shared/charts/GScatterChart.vue'

// ─── Reusable primitives ────────────────────────────────────────────────────

export interface PieSlice {
  name: string
  value: number
  color?: string
}

export interface WinLossGroupedData {
  /** x labels (e.g. 0-31 for days, 0-23 for hours, month names, weekday names) */
  categories: (string | number)[]
  wins: number[]
  losses: number[]
}

export interface WinLossProfitGroupedData {
  categories: (string | number)[]
  /** profit values for winning trades (positive) */
  winProfits: number[]
  /** loss values for losing trades (negative) */
  lossProfits: number[]
}

// ─── Main interface ──────────────────────────────────────────────────────────

export interface TradeAnalyticsData {
  /** 1. P&L by year — bar chart */
  pnlByYear: BarChartDataPoint[] | null

  /** 2. Long vs Short P&L — bar chart (two bars: Long P/L, Short P/L) */
  longShortPnl: { longPnl: number; shortPnl: number } | null

  /** 3. Long vs Short trades — pie chart */
  longShortTrades: { longCount: number; shortCount: number } | null

  /** 4. Overall Profit/Loss — pie chart */
  profitLossPie: { totalProfit: number; totalLoss: number } | null

  /** 5. P&L growth by duration — scatter (x: duration in seconds, y: profit/loss) */
  pnlByDuration: ScatterChartDataPoint[] | null

  /** 6. Long Profit/Loss — pie chart */
  longProfitLossPie: { profit: number; loss: number } | null

  /** 7. Short Profit/Loss — pie chart */
  shortProfitLossPie: { profit: number; loss: number } | null

  /** 8. P&L by day (0-31) */
  pnlByDay: BarChartDataPoint[] | null

  /** 9. P&L by hour (0-23) */
  pnlByHour: BarChartDataPoint[] | null

  /** 10. P&L by month (Jan-Dec) */
  pnlByMonth: BarChartDataPoint[] | null

  /** 11. P&L by weekday (Sun-Sat) */
  pnlByWeekday: BarChartDataPoint[] | null

  /** 12. Trades by day (0-31) */
  tradesByDay: BarChartDataPoint[] | null

  /** 13. Trades by month (Jan-Dec) */
  tradesByMonth: BarChartDataPoint[] | null

  /** 14. Trades by weekday */
  tradesByWeekday: BarChartDataPoint[] | null

  /** 15. Trades by year */
  tradesByYear: BarChartDataPoint[] | null

  /** 16. Win/Losses count by day */
  winLossByDay: WinLossGroupedData | null

  /** 17. Win/Losses count by hour */
  winLossByHour: WinLossGroupedData | null

  /** 18. Win/Losses count by month */
  winLossByMonth: WinLossGroupedData | null

  /** 19. Win/Losses count by weekday */
  winLossByWeekday: WinLossGroupedData | null

  /** 20. Win/Losses profit by day */
  winLossProfitByDay: WinLossProfitGroupedData | null

  /** 21. Win/Losses profit by hour */
  winLossProfitByHour: WinLossProfitGroupedData | null

  /** 22. Win/Losses profit by month */
  winLossProfitByMonth: WinLossProfitGroupedData | null

  /** 23. Win/Losses profit by weekday */
  winLossProfitByWeekday: WinLossProfitGroupedData | null
}
