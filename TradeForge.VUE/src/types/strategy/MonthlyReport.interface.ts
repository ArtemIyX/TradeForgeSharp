export interface MonthlyReportMap {
  data: Map<number, MonthlyReportItem>; // year -> profits
}

export interface MonthlyReportItem {
  profits: number[]; // For each of 12 month (0 - jan, 11 - dec)
}
