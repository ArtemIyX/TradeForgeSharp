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

export interface TickerResponseModel {
  id: string;
  symbol: string;
  instrument: string;
  dateFrom: string;
  dateTo: string;
  totalRecords: number;
  timeFrame: string;
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

export function convertToTicker(response: TickerResponseModel): Ticker {
  return {
    id: response.id,
    symbol: response.symbol,
    instrument: response.instrument,
    dateFrom: new Date(response.dateFrom),
    dateTo: new Date(response.dateTo),
    totalRecords: response.totalRecords,
    timeFrame: response.timeFrame as TimeFrame,
    category: response.category,
  };
}

export function convertToTickers(responses: TickerResponseModel[]): Ticker[] {
  return responses.map(convertToTicker);
}
