export interface Ticker {
  id: string;
  symbol: string;
  instrument: string;
  dateFrom: Date | null;
  dateTo: Date | null;
  totalRecords: number;
  timeFrame: TimeFrame | null;
  category: string;
}

export interface TickerResponseModel {
  id: string;
  symbol: string;
  instrument: string;
  dateFrom: string | null;
  dateTo: string | null;
  totalRecords: number;
  timeFrame: string | null;
  category: string;
}

export interface TickerDetails {
  id: string;
  symbol: string;
  instrument: string;
  category: string;
  contractSize: number;
  units: string;
  minVolume: number;
  maxVolume: number;
  volumeStep: number;
  minTick: number;
  leverage: number;
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
    dateFrom: response.dateFrom ? new Date(response.dateFrom) : null,
    dateTo: response.dateTo ? new Date(response.dateTo) : null,
    totalRecords: response.totalRecords,
    timeFrame: response.timeFrame as TimeFrame | null,
    category: response.category,
  };
}

export function convertToTickers(responses: TickerResponseModel[]): Ticker[] {
  return responses.map(convertToTicker);
}
