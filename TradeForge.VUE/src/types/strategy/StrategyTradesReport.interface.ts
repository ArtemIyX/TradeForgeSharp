export interface StrategyTradesReport {
  wins: number;
  losses: number;
  canceledOrExpired: number;
  grossProfit: number;
  grossLoss: number;
  avgWin: number;
  avgLoss: number;
  largestWin: number;
  largestLoss: number;
  maxConsWins: number;
  maxConsLosses: number;
  avgConsWins: number;
  avgConsLosses: number;
  avgBarsInWins: number;
  avgBarsInLosses: number;
}
