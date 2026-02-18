import type {
  StrategyPerformanceReportData
} from "@/types/strategy/StrategyPerformanceReport.interface.ts";
import type {StrategyReport} from "@/types/strategy/StrategyStatsReport.interface.ts";
import type {StrategyTradesReport} from "@/types/strategy/StrategyTradesReport.interface.ts";
import type {
  MonthlyReportMap
} from "@/types/strategy/MonthlyReport.interface.ts";

export interface StrategyReportData {
  performance: StrategyPerformanceReportData | null;
  strategy: StrategyReport | null;
  trades: StrategyTradesReport | null;
  months: MonthlyReportMap | null;
}
