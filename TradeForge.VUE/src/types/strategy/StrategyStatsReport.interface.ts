export interface StrategyRatioReportData {
  sharpeRatio: number;
  sortinoRatio: number;
  calmarRatio: number;
  sterlingRatio: number;
  omegaRatio: number;
  marRatio: number;
}

export interface StrategyStatsReportData {
  winLossRatio: number;
  payoutRatio: number;
  avgBarsInTrade: number;

  ahpr: number;
  zScore: number;
  zProbability: number; // percent

  expectancy: number;
  deviation: number;
  exposure: number; // percent

  stagnationInDays: number;
  stagnationInPercent: number; // percent
}



