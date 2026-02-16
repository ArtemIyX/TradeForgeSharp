// OhlcData.ts - Add these interfaces and function

export interface OhlcData {
  timestamp: number | Date;
  open: number;
  high: number;
  low: number;
  close: number;
  volume?: number;
}

export interface OhlcResponseModel {
  timestamp: string;
  open: number;
  high: number;
  low: number;
  close: number;
  volume?: number;
}

export interface ChartDrawing {
  type: 'line' | 'horizontal' | 'vertical' | 'rect' | 'text';
  data: any;
  style?: {
    color?: string;
    width?: number;
    lineStyle?: 'solid' | 'dashed' | 'dotted';
  };
}

export function convertToOhlc(response: OhlcResponseModel): OhlcData {
  return {
    timestamp: new Date(response.timestamp),
    open: response.open,
    high: response.high,
    low: response.low,
    close: response.close,
    volume: response.volume,
  };
}

export function convertToOhlcArray(responses: OhlcResponseModel[]): OhlcData[] {
  return responses.map(convertToOhlc);
}
