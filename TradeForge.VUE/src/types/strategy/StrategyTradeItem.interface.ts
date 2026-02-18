export interface StrategyTradeItem {
  magic: number; // strategy magic number
  ticket: number; // trade id to find in chart
  symbol: string; // symbol name

  openTime: Date;
  openPrice: number;

  size: number; // lots

  closeTime: Date;
  closePrice: number;

  profitOrLoss: number;
  balanceAfter: number;
  closeType: string; // SL, TP, Trailling Stop, End of friday etc

  mae: number;
  mfe: number;
  timeInTrade: string;

  comment: string;
}
