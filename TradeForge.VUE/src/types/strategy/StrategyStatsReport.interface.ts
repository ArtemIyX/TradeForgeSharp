export interface StrategyRatioReport {
  sharpeRatio: number;
  sortinoRatio: number;
  calmarRatio: number;
  sterlingRatio: number;
  omegaRatio: number;
  marRatio: number;
}

export interface StrategyStatsReport {
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


export interface StrategyReport {
  stats: StrategyStatsReport
  ratio: StrategyRatioReport
}

