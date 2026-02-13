export interface Ticker {
  id: string;
  symbol: string;
  instrument: string;
  dateFrom: Date;
  dateTo: Date;
  totalRecords: number;
  timeFrame: TimeFrame;
  category: string;
}

export enum TimeFrame {
  m1 = '1m',
  m5 = '5m',
  m15 = '15m',
  m30 = '30m',
  h1 = '1h',
  h4 = '4h',
  daily = '1D',
  week = '1W',
  month = '1M'
}
