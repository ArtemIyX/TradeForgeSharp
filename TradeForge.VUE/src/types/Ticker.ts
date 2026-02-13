export interface Ticker {
  id: string;
  symbol: string;
  instrument: string;
  dateFrom: Date;
  dateTo: Date;
  totalRecords: number;
  timeFrame: '1m' | '5m' | '15m' | '30m' | '1h' | '4h' | '1D' | '1W' | '1M';
  category: string;
}
