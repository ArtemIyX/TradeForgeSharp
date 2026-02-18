export interface StrategyPerformanceReportData {
  totaProfit: number; // $
  profitInPips: number; // ticks
  yearlyAvgProfit: number; // $
  yearlyAvgProfitPercent: number; // %
  cagr: number; // %

  numberOfTrades: number;
  profitFactor: number;
  returnDdRatio: number;
  winningPercentage: number;

  drawdown: number; // $
  drawdownPercent: number; // %
  dailyAvgProfit: number;
  monthlyAvgProfit: number;
  avgTradeProfit: number;
}
