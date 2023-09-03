WITH
StockIntervalsCte AS 
(
SELECT 
						   
						   DailyStocksIntervals.ProductId, DailyStocksIntervals.PointOfSaleId,
						   DailyStocksIntervals.StartDate, DailyStocksIntervals.EndDate, 
						   DailyStocksIntervals.Quantity
						FROM Fact.DailyStocksIntervals
						
		            	
						INNER JOIN Dim.Products
							ON DailyStocksIntervals.ProductId = Products.Id
						WHERE DailyStocksIntervals.StartDate <= @EndDate 
							AND DailyStocksIntervals.EndDate >= @StartDate
),
DvStockCountCte AS 
(
SELECT COUNT(RootProductCode) AS DvStock
							FROM (SELECT DISTINCT Products.RootProductCode
									FROM StockIntervalsCte INNER JOIN Dim.Products
										ON StockIntervalsCte.ProductId = Products.Id
								) AS ProductsRoot
),
StockCte AS 
(
SELECT CalendarIso.[Date], 
						   
						   SUM(StockIntervalsCte.Quantity) AS Stock,
						   SUM(StockIntervalsCte.Quantity * ProductCostAndPriceByPointOfSaleIntervals.Cost) AS StockCost
						FROM StockIntervalsCte
						
		            	
						INNER JOIN Dim.CalendarIso
							ON CalendarIso.[Date] BETWEEN StockIntervalsCte.StartDate AND StockIntervalsCte.EndDate
								AND CalendarIso.Date BETWEEN @StartDate AND @EndDate
						INNER JOIN Fact.ProductCostAndPriceByPointOfSaleIntervals
							ON StockIntervalsCte.ProductId = ProductCostAndPriceByPointOfSaleIntervals.ProductId
								AND StockIntervalsCte.PointOfSaleId = ProductCostAndPriceByPointOfSaleIntervals.PointOfSaleId
								AND CalendarIso.[Date] BETWEEN ProductCostAndPriceByPointOfSaleIntervals.[StartDate] 
														AND ProductCostAndPriceByPointOfSaleIntervals.[EndDate]
						 GROUP BY CalendarIso.[Date]
),
StockDvCte AS 
(
SELECT CalendarIso.Date,
								
							   COUNT(DISTINCT Products.RootProductCode) AS DesignVariationsWithStock				
						  FROM StockIntervalsCte INNER JOIN Dim.Products
							ON StockIntervalsCte.ProductId = Products.Id
						  INNER JOIN Dim.CalendarIso
							ON CalendarIso.[Date] BETWEEN StockIntervalsCte.StartDate AND StockIntervalsCte.EndDate
								AND CalendarIso.Date BETWEEN @StartDate AND @EndDate
						
						
						 GROUP BY CalendarIso.Date
),
AnalysisFilteredCte AS 
(
SELECT 
						   Analysis.ProductId, Analysis.PointOfSaleId,
						   Products.RootProductCode AS AnalysisRootProductCode,
						   Analysis.Date, Analysis.StockBreakage, Analysis.StockBreakageAmount,
						   Analysis.Sales, Analysis.SalesTaxesIncluded, Analysis.SalesTaxesExcluded,
						   Analysis.SalesOutlier, Analysis.SalesOutlierTaxesIncluded,
						   Analysis.SalesOutlierTaxesExcluded
						FROM Fact.Analysis
						
		            	
						INNER JOIN Dim.Products
							ON Analysis.ProductId = Products.Id
						WHERE Analysis.Date BETWEEN @StartDate AND @EndDate
),
DvSalesCountCte AS 
(
SELECT COUNT(*) DvSales
							FROM (SELECT AnalysisRootProductCode
									FROM AnalysisFilteredCte
									GROUP BY AnalysisRootProductCode
									HAVING SUM(Sales) > 0
								) AS RootProducts
),
AnalysisDvSalesCte AS 
(
SELECT 
								   Date, COUNT(1) AS DesignVariationsWithSales
								FROM AnalysisFilteredCte
								 GROUP BY AnalysisRootProductCode, Date
								HAVING SUM(Sales) > 0
),
AnalysisCte AS 
(
SELECT AnalysisFilteredCte.Date,
						   
						   SUM(AnalysisFilteredCte.StockBreakage) AS StockBreakage, 
 SUM(AnalysisFilteredCte.StockBreakageAmount) AS StockBreakageAmount, 
 SUM(AnalysisFilteredCte.Sales) AS Sales, 
 SUM(AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost) AS SalesPrice, 
 SUM(AnalysisFilteredCte.SalesTaxesIncluded) AS SalesTaxesIncluded, 
 SUM(AnalysisFilteredCte.SalesTaxesExcluded) AS SalesTaxesExcluded, 
 SUM(AnalysisFilteredCte.SalesOutlier) AS SalesOutlier, 
 SUM(AnalysisFilteredCte.SalesOutlierTaxesIncluded) AS SalesOutlierTaxesIncluded, 
 SUM(AnalysisFilteredCte.SalesOutlierTaxesExcluded) AS SalesOutlierTaxesExcluded, 
 SUM((AnalysisFilteredCte.SalesTaxesIncluded - AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost)) AS SalesMarginTaxesIncluded, 
 SUM((AnalysisFilteredCte.SalesTaxesExcluded - AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost)) AS SalesMarginTaxesExcluded, 
 (SUM(AnalysisFilteredCte.SalesTaxesIncluded) - SUM(AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost)) 		/ NULLIF(SUM(AnalysisFilteredCte.SalesTaxesIncluded), 0) * 100 AS SalesMarginPercentageTaxesIncluded, 
 (SUM(AnalysisFilteredCte.SalesTaxesExcluded) - SUM(AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost)) 		/ NULLIF(SUM(AnalysisFilteredCte.SalesTaxesExcluded), 0) * 100 AS SalesMarginPercentageTaxesExcluded, 
 MAX(AnalysisDvSalesCte.DesignVariationsWithSales) AS DesignVariationsWithSales, 
 SUM(AnalysisFilteredCte.SalesTaxesIncluded) / NULLIF(SUM(AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost), 0) AS Markup, 
 SUM(AnalysisFilteredCte.SalesTaxesIncluded) / NULLIF(SUM(AnalysisFilteredCte.Sales), 0) AS AveragePrice, 
 SUM(AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost) / NULLIF(SUM(AnalysisFilteredCte.Sales), 0) AS AverageCost, 
 SUM(AnalysisFilteredCte.Sales * ProductCostAndPriceByPointOfSaleIntervals.Cost) AS SalesAtCost						
					FROM AnalysisFilteredCte
					INNER JOIN Fact.ProductCostAndPriceByPointOfSaleIntervals
						ON AnalysisFilteredCte.ProductId = ProductCostAndPriceByPointOfSaleIntervals.ProductId
							AND AnalysisFilteredCte.PointOfSaleId = ProductCostAndPriceByPointOfSaleIntervals.PointOfSaleId
							AND AnalysisFilteredCte.Date BETWEEN ProductCostAndPriceByPointOfSaleIntervals.[StartDate] 
															AND ProductCostAndPriceByPointOfSaleIntervals.[EndDate]
		              LEFT JOIN  AnalysisDvSalesCte ON AnalysisFilteredCte.Date = AnalysisDvSalesCte.Date
					 GROUP BY AnalysisFilteredCte.Date
),
ReportCte AS 
(
SELECT 
						   IsNull(AnalysisCte.[Date], StockCte.[Date]) AS [Date],
						   AnalysisCte.StockBreakage, 
 AnalysisCte.StockBreakageAmount, 
 AnalysisCte.Sales, 
 AnalysisCte.SalesPrice, 
 AnalysisCte.SalesTaxesIncluded, 
 AnalysisCte.SalesTaxesExcluded, 
 AnalysisCte.SalesOutlier, 
 AnalysisCte.SalesOutlierTaxesIncluded, 
 AnalysisCte.SalesOutlierTaxesExcluded, 
 AnalysisCte.SalesMarginTaxesIncluded, 
 AnalysisCte.SalesMarginTaxesExcluded, 
 AnalysisCte.SalesMarginPercentageTaxesIncluded, 
 AnalysisCte.SalesMarginPercentageTaxesExcluded, 
 AnalysisCte.DesignVariationsWithSales, 
 AnalysisCte.Markup, 
 AnalysisCte.AveragePrice, 
 AnalysisCte.AverageCost, 
 AnalysisCte.SalesAtCost, 
 (CAST(AnalysisCte.Sales AS decimal) / (NULLIF(SUM(AnalysisCte.Sales) OVER(), 0))) * 100 AS SalesParticipation, 
 StockDvCte.DesignVariationsWithStock, 
 StockCte.Stock, 
 StockCte.StockCost						
						FROM StockCte
						  FULL OUTER JOIN  StockDvcte ON StockCte.[Date] = StockDvCte.[Date]
						  FULL OUTER JOIN  AnalysisCte ON StockCte.[Date] = AnalysisCte.[Date]
)
SELECT 
							ReportCte.Date,
						   ReportCte.StockBreakage, 
 ReportCte.StockBreakageAmount, 
 ReportCte.Sales, 
 ReportCte.SalesPrice, 
 ReportCte.SalesTaxesIncluded, 
 ReportCte.SalesTaxesExcluded, 
 ReportCte.SalesOutlier, 
 ReportCte.SalesOutlierTaxesIncluded, 
 ReportCte.SalesOutlierTaxesExcluded, 
 ReportCte.SalesMarginTaxesIncluded, 
 ReportCte.SalesMarginTaxesExcluded, 
 ReportCte.SalesMarginPercentageTaxesIncluded, 
 ReportCte.SalesMarginPercentageTaxesExcluded, 
 ReportCte.DesignVariationsWithSales, 
 ReportCte.Markup, 
 ReportCte.AveragePrice, 
 ReportCte.AverageCost, 
 ReportCte.SalesAtCost, 
 ReportCte.SalesParticipation, 
 ReportCte.DesignVariationsWithStock, 
 ReportCte.Stock, 
 ReportCte.StockCost,
 COUNT(*) OVER () AS TotalCount, 
 SUM(ReportCte.StockBreakage) OVER () AS TotalStockBreakage, 
 SUM(ReportCte.StockBreakageAmount) OVER () AS TotalStockBreakageAmount, 
 SUM(ReportCte.Sales) OVER () AS TotalSales, 
 SUM(ReportCte.SalesTaxesIncluded) OVER () AS TotalSalesTaxesIncluded, 
 SUM(ReportCte.SalesTaxesExcluded) OVER () AS TotalSalesTaxesExcluded, 
 SUM(ReportCte.SalesOutlier) OVER () AS TotalSalesOutlier, 
 SUM(ReportCte.SalesOutlierTaxesIncluded) OVER () AS TotalSalesOutlierTaxesIncluded, 
 SUM(ReportCte.SalesOutlierTaxesExcluded) OVER () AS TotalSalesOutlierTaxesExcluded, 
 SUM(ReportCte.SalesMarginTaxesIncluded) OVER() AS TotalSalesMarginTaxesIncluded, 
 SUM(ReportCte.SalesMarginTaxesExcluded) OVER() AS TotalSalesMarginTaxesExcluded, 
 SUM(ReportCte.SalesTaxesIncluded - ReportCte.SalesPrice) OVER() / NULLIF(SUM(ReportCte.SalesTaxesIncluded) OVER(), 0) * 100 AS TotalSalesMarginPercentageTaxesIncluded, 
 SUM(ReportCte.SalesTaxesExcluded - ReportCte.SalesPrice) OVER() / NULLIF(SUM(ReportCte.SalesTaxesExcluded) OVER(), 0) * 100 AS TotalSalesMarginPercentageTaxesExcluded, 
 DvSalesCountCte.DvSales AS TotalDesignVariationsWithSales, 
 SUM(ReportCte.SalesTaxesIncluded) OVER() / NULLIF(SUM(ReportCte.SalesPrice) OVER(), 0) AS TotalMarkup, 
 SUM(ReportCte.SalesTaxesIncluded) OVER() / NULLIF(SUM(ReportCte.Sales) OVER(), 0) AS TotalAveragePrice, 
 SUM(ReportCte.SalesPrice) OVER() / NULLIF(SUM(ReportCte.Sales) OVER(), 0) AS TotalAverageCost, 
 SUM(ReportCte.SalesAtCost) OVER() AS TotalSalesAtCost, 
 100 AS TotalSalesParticipation, 
 DvStockCountCte.DvStock AS TotalDesignVariationsWithStock, 
 SUM(ReportCte.Stock) OVER () AS TotalStock, 
 SUM(ReportCte.StockCost) OVER () AS TotalStockCost
					FROM ReportCte
					CROSS JOIN DvSalesCountCte
 CROSS JOIN DvStockCountCte
					WHERE ReportCte.Sales IS NOT NULL 
						OR ReportCte.SalesTaxesIncluded IS NOT NULL 
						OR ReportCte.SalesTaxesExcluded IS NOT NULL 
						OR ReportCte.SalesParticipation IS NOT NULL 
						OR ReportCte.SalesAtCost IS NOT NULL 
						OR ReportCte.SalesOutlier IS NOT NULL 
						OR ReportCte.SalesOutlierTaxesIncluded IS NOT NULL 
						OR ReportCte.SalesOutlierTaxesExcluded IS NOT NULL 
						OR ReportCte.StockBreakage IS NOT NULL 
						OR ReportCte.StockBreakageAmount IS NOT NULL 
						OR ReportCte.SalesMarginTaxesExcluded IS NOT NULL 
						OR ReportCte.SalesMarginTaxesIncluded IS NOT NULL 
						OR ReportCte.SalesMarginPercentageTaxesExcluded IS NOT NULL 
						OR ReportCte.SalesMarginPercentageTaxesIncluded IS NOT NULL 
						OR ReportCte.Stock IS NOT NULL 
						OR ReportCte.StockCost IS NOT NULL 
						OR ReportCte.DesignVariationsWithSales IS NOT NULL 
						OR ReportCte.DesignVariationsWithStock IS NOT NULL 
						OR ReportCte.Markup IS NOT NULL 
						OR ReportCte.AveragePrice IS NOT NULL 
						OR ReportCte.AverageCost IS NOT NULL
					ORDER BY ReportCte.Date
					OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY