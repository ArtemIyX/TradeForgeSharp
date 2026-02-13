import type {Ticker} from "@/types/Ticker.ts";

export interface DataManagerContextMenuItem {
  key: string;
  label: string;
  icon?: string;
  action: (ticker: Ticker) => void;
}
