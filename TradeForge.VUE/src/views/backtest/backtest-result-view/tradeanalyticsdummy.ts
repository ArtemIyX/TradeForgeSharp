import type {TradeAnalyticsData} from '@/types/strategy/TradeAnalyticsReport.interface'

const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
const WEEKDAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
const HOURS = Array.from({length: 24}, (_, i) => i)
const DAYS = Array.from({length: 31}, (_, i) => i + 1)

export const tradeAnalyticsDummy: TradeAnalyticsData = {

  // 1. P&L by year
  pnlByYear: [
    {x: 2019, y: -320.5},
    {x: 2020, y: 1240.0},
    {x: 2021, y: 880.75},
    {x: 2022, y: -540.2},
    {x: 2023, y: 2150.3},
    {x: 2024, y: 1870.0},
  ],

  // 2. Long vs Short P&L
  longShortPnl: {
    longPnl: 6213.5,
    shortPnl: -890.2,
  },

  // 3. Long vs Short trades (count)
  longShortTrades: {
    longCount: 420,
    shortCount: 242,
  },

  // 4. Overall Profit / Loss pie
  profitLossPie: {
    totalProfit: 23316.1,
    totalLoss: -17102.6,
  },

  // 5. P&L growth by duration (x: seconds, y: profit/loss)
  pnlByDuration: [
    // Wins — green
    {x: 300, y: 45.2},
    {x: 900, y: 120.0},
    {x: 3600, y: 175.0},
    {x: 7200, y: 88.5},
    {x: 14400, y: 160.3},
    {x: 21600, y: 95.0},
    {x: 43200, y: 142.7},
    {x: 86400, y: 175.0},
    {x: 172800, y: 130.5},
    {x: 259200, y: 165.2},
    {x: 345600, y: 110.0},
    {x: 432000, y: 155.8},
    // Losses — red
    {x: 600, y: -30.1},
    {x: 1800, y: -46.0},
    {x: 5400, y: -41.5},
    {x: 10800, y: -38.2},
    {x: 18000, y: -44.0},
    {x: 36000, y: -29.7},
    {x: 72000, y: -46.0},
    {x: 144000, y: -33.8},
    {x: 216000, y: -40.1},
    {x: 302400, y: -25.5},
  ],

  // 6. Long Profit/Loss pie
  longProfitLossPie: {
    profit: 18500.4,
    loss: -12287.9,
  },

  // 7. Short Profit/Loss pie
  shortProfitLossPie: {
    profit: 4815.7,
    loss: -4814.7,
  },

  // 8. P&L by day (1-31)
  pnlByDay: DAYS.map(d => ({
    x: d,
    y: Math.round((Math.random() * 600 - 200) * 10) / 10,
  })),

  // 9. P&L by hour (0-23)
  pnlByHour: HOURS.map(h => ({
    x: h,
    y: Math.round((Math.random() * 400 - 150) * 10) / 10,
  })),

  // 10. P&L by month
  pnlByMonth: [
    {x: 'Jan', y: 253.3},
    {x: 'Feb', y: 79.6},
    {x: 'Mar', y: -184.0},
    {x: 'Apr', y: -92.0},
    {x: 'May', y: 61.8},
    {x: 'Jun', y: -80.4},
    {x: 'Jul', y: 3.6},
    {x: 'Aug', y: 35.5},
    {x: 'Sep', y: -73.5},
    {x: 'Oct', y: -9.0},
    {x: 'Nov', y: 14.0},
    {x: 'Dec', y: 206.2},
  ],

  // 11. P&L by weekday
  pnlByWeekday: [
    {x: 'Sun', y: -120.5},
    {x: 'Mon', y: 430.2},
    {x: 'Tue', y: 1080.7},
    {x: 'Wed', y: 2250.3},
    {x: 'Thu', y: 2180.1},
    {x: 'Fri', y: -310.4},
    {x: 'Sat', y: 0},
  ],

  // 12. Trades by day (1-31)
  tradesByDay: DAYS.map(d => ({
    x: d,
    y: Math.floor(Math.random() * 25 + 1),
  })),

  // 13. Trades by month
  tradesByMonth: MONTHS.map(m => ({
    x: m,
    y: Math.floor(Math.random() * 80 + 20),
  })),

  // 14. Trades by weekday
  tradesByWeekday: [
    {x: 'Sun', y: 8},
    {x: 'Mon', y: 145},
    {x: 'Tue', y: 152},
    {x: 'Wed', y: 148},
    {x: 'Thu', y: 130},
    {x: 'Fri', y: 148},
    {x: 'Sat', y: 0},
  ],

  // 15. Trades by year
  tradesByYear: [
    {x: 2019, y: 88},
    {x: 2020, y: 134},
    {x: 2021, y: 178},
    {x: 2022, y: 210},
    {x: 2023, y: 195},
    {x: 2024, y: 162},
  ],

  // 16. Win/Loss count by day
  winLossByDay: {
    categories: DAYS,
    wins: DAYS.map(() => Math.floor(Math.random() * 12)),
    losses: DAYS.map(() => Math.floor(Math.random() * 12)),
  },

  // 17. Win/Loss count by hour
  winLossByHour: {
    categories: HOURS,
    wins: [0, 0, 0, 0, 0, 1, 3, 8, 14, 22, 30, 28, 25, 20, 18, 15, 10, 8, 5, 3, 2, 1, 0, 0],
    losses: [0, 0, 0, 0, 0, 1, 2, 5, 10, 18, 22, 20, 18, 14, 12, 10, 7, 5, 3, 2, 1, 1, 0, 0],
  },

  // 18. Win/Loss count by month
  winLossByMonth: {
    categories: MONTHS,
    wins: [18, 14, 9, 12, 22, 10, 8, 15, 11, 9, 13, 20],
    losses: [12, 10, 18, 15, 12, 16, 14, 10, 17, 13, 11, 9],
  },

  // 19. Win/Loss count by weekday
  winLossByWeekday: {
    categories: WEEKDAYS,
    wins: [3, 55, 62, 58, 52, 60, 0],
    losses: [5, 90, 90, 90, 78, 88, 0],
  },

  // 20. Win/Loss profit by day
  winLossProfitByDay: {
    categories: DAYS,
    winProfits: DAYS.map(() => Math.round(Math.random() * 800 * 10) / 10),
    lossProfits: DAYS.map(() => -Math.round(Math.random() * 500 * 10) / 10),
  },

  // 21. Win/Loss profit by hour
  winLossProfitByHour: {
    categories: HOURS,
    winProfits: [0, 0, 0, 0, 0, 45, 120, 380, 820, 1400, 1800, 1650, 1500, 1100, 950, 780, 420, 310, 180, 90, 60, 30, 0, 0],
    lossProfits: [0, 0, 0, 0, 0, -20, -60, -180, -420, -780, -960, -880, -820, -640, -520, -380, -200, -140, -80, -40, -20, -10, 0, 0],
  },

  // 22. Win/Loss profit by month
  winLossProfitByMonth: {
    categories: MONTHS,
    winProfits: [2100, 2200, 1050, 1200, 2300, 1100, 900, 1600, 1150, 950, 1350, 2100],
    lossProfits: [-1100, -1200, -1400, -1350, -1050, -1300, -1200, -900, -1350, -1100, -1000, -800],
  },

  // 23. Win/Loss profit by weekday
  winLossProfitByWeekday: {
    categories: WEEKDAYS,
    winProfits: [180, 2800, 3100, 3800, 3600, 2900, 0],
    lossProfits: [-300, -1800, -1900, -1400, -1300, -2100, 0],
  },
}
