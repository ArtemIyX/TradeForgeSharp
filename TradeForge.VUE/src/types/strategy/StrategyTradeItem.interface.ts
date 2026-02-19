export interface StrategyTradeItem {
  magic: number; // strategy magic number
  ticket: number; // trade id to find in chart
  symbol: string; // symbol name

  openTime: string;
  openPrice: number;

  size: number; // lots

  closeTime: string;
  closePrice: number;

  profitOrLoss: number;
  balanceAfter: number;
  closeType: string; // SL, TP, Trailling Stop, End of friday etc

  mae: number;
  mfe: number;
  timeInTrade: string;

  comment: string;
}


