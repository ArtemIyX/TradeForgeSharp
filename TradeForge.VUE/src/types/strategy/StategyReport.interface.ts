import type {
  StrategyPerformanceReportData
} from "@/types/strategy/StrategyPerformanceReport.interface.ts";
import type {StrategyTradesReportData} from "@/types/strategy/StrategyTradesReport.interface.ts";
import type {
  MonthlyReportMap
} from "@/types/strategy/MonthlyReport.interface.ts";
import type {
  StrategyRatioReportData,
  StrategyStatsReportData
} from "@/types/strategy/StrategyStatsReport.interface.ts";

export interface StrategyReportData {
  performance: StrategyPerformanceReportData | null;
  ratio: StrategyRatioReportData | null;
  stats: StrategyStatsReportData | null;
  trades: StrategyTradesReportData | null;
  months: MonthlyReportMap | null;
}
