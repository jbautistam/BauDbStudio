WITH
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[City] AS [City], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ImageUrl] AS [ImageUrl], [Channels].[Name] AS [ChannelName], 
		[PointOfSaleClassificationLevelValuesReadingModel].[ClassificationLevelValue1] AS [PointOfSaleClassificationLevel1], 
		[PointOfSaleClassificationLevelValuesReadingModel].[ClassificationLevelValue2] AS [PointOfSaleClassificationLevel2]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]
	INNER JOIN [Dim].[Channels] AS [Channels]
	ON [PointsOfSale].[ChannelId] = [Channels].[Id]
	INNER JOIN [Dim].[PointOfSaleClassificationLevelValuesReadingModel] AS [PointOfSaleClassificationLevelValuesReadingModel]
	ON [PointsOfSale].[PointOfSaleClassificationLevelValuesReadingModelId] = [PointOfSaleClassificationLevelValuesReadingModel].[Id]


),
StockIntervalsCte AS 
(
SELECT 
						   [PointsOfSaleCte].[City], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleClassificationLevel1], [PointsOfSaleCte].[PointOfSaleClassificationLevel2], 
						   DailyStocksIntervals.ProductId, DailyStocksIntervals.PointOfSaleId,
						   DailyStocksIntervals.StartDate, DailyStocksIntervals.EndDate, 
						   DailyStocksIntervals.Quantity
						FROM Fact.DailyStocksIntervals
						 INNER JOIN  PointsOfSaleCte
															ON 
 [DailyStocksIntervals].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
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
						   [PointsOfSaleCte].[City], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleClassificationLevel1], [PointsOfSaleCte].[PointOfSaleClassificationLevel2], 
						   SUM(StockIntervalsCte.Quantity) AS Stock,
						   SUM(StockIntervalsCte.Quantity * ProductCostAndPriceByPointOfSaleIntervals.Cost) AS StockCost
						FROM StockIntervalsCte
						 INNER JOIN  PointsOfSaleCte
															ON 
 [StockIntervalsCte].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
						INNER JOIN Dim.CalendarIso
							ON CalendarIso.[Date] BETWEEN StockIntervalsCte.StartDate AND StockIntervalsCte.EndDate
								AND CalendarIso.Date BETWEEN @StartDate AND @EndDate
						INNER JOIN Fact.ProductCostAndPriceByPointOfSaleIntervals
							ON StockIntervalsCte.ProductId = ProductCostAndPriceByPointOfSaleIntervals.ProductId
								AND StockIntervalsCte.PointOfSaleId = ProductCostAndPriceByPointOfSaleIntervals.PointOfSaleId
								AND CalendarIso.[Date] BETWEEN ProductCostAndPriceByPointOfSaleIntervals.[StartDate] 
														AND ProductCostAndPriceByPointOfSaleIntervals.[EndDate]
						 GROUP BY [PointsOfSaleCte].[City], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleClassificationLevel1], [PointsOfSaleCte].[PointOfSaleClassificationLevel2], CalendarIso.[Date]
),
StockDvCte AS 
(
SELECT CalendarIso.Date,
								[PointsOfSaleCte].[City], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleClassificationLevel1], [PointsOfSaleCte].[PointOfSaleClassificationLevel2], 
							   COUNT(DISTINCT Products.RootProductCode) AS DesignVariationsWithStock				
						  FROM StockIntervalsCte INNER JOIN Dim.Products
							ON StockIntervalsCte.ProductId = Products.Id
						  INNER JOIN Dim.CalendarIso
							ON CalendarIso.[Date] BETWEEN StockIntervalsCte.StartDate AND StockIntervalsCte.EndDate
								AND CalendarIso.Date BETWEEN @StartDate AND @EndDate
						
						 INNER JOIN  PointsOfSaleCte
															ON 
 [StockIntervalsCte].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
						 GROUP BY [PointsOfSaleCte].[City], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleClassificationLevel1], [PointsOfSaleCte].[PointOfSaleClassificationLevel2], CalendarIso.Date
),
AnalysisFilteredCte AS 
(
SELECT [PointsOfSaleCte].[City], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleClassificationLevel1], [PointsOfSaleCte].[PointOfSaleClassificationLevel2], 
						   Analysis.ProductId, Analysis.PointOfSaleId,
						   Products.RootProductCode AS AnalysisRootProductCode,
						   Analysis.Date, Analysis.StockBreakage, Analysis.StockBreakageAmount,
						   Analysis.Sales, Analysis.SalesTaxesIncluded, Analysis.SalesTaxesExcluded,
						   Analysis.SalesOutlier, Analysis.SalesOutlierTaxesIncluded,
						   Analysis.SalesOutlierTaxesExcluded
						FROM Fact.Analysis
						 INNER JOIN  PointsOfSaleCte
															ON 
 [Analysis].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
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
SELECT [City], [ErpCode], [PointOfSale], [ImageUrl], [ChannelName], [PointOfSaleClassificationLevel1], [PointOfSaleClassificationLevel2], 
								   Date, COUNT(1) AS DesignVariationsWithSales
								FROM AnalysisFilteredCte
								 GROUP BY [City], [ErpCode], [PointOfSale], [ImageUrl], [ChannelName], [PointOfSaleClassificationLevel1], [PointOfSaleClassificationLevel2], AnalysisRootProductCode, Date
								HAVING SUM(Sales) > 0
),
AnalysisCte AS 
(
SELECT AnalysisFilteredCte.Date,
						   [AnalysisFilteredCte].[City], [AnalysisFilteredCte].[ErpCode], [AnalysisFilteredCte].[PointOfSale], [AnalysisFilteredCte].[ImageUrl], [AnalysisFilteredCte].[ChannelName], [AnalysisFilteredCte].[PointOfSaleClassificationLevel1], [AnalysisFilteredCte].[PointOfSaleClassificationLevel2], 
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
		             LEFT JOIN  AnalysisDvSalesCte
															ON 
 IsNull([AnalysisFilteredCte].[City], '') = IsNull([AnalysisDvSalesCte].[City], '')
 AND  IsNull([AnalysisFilteredCte].[ErpCode], '') = IsNull([AnalysisDvSalesCte].[ErpCode], '')
 AND  IsNull([AnalysisFilteredCte].[PointOfSale], '') = IsNull([AnalysisDvSalesCte].[PointOfSale], '')
 AND  IsNull([AnalysisFilteredCte].[ImageUrl], '') = IsNull([AnalysisDvSalesCte].[ImageUrl], '')
 AND  IsNull([AnalysisFilteredCte].[ChannelName], '') = IsNull([AnalysisDvSalesCte].[ChannelName], '')
 AND  IsNull([AnalysisFilteredCte].[PointOfSaleClassificationLevel1], '') = IsNull([AnalysisDvSalesCte].[PointOfSaleClassificationLevel1], '')
 AND  IsNull([AnalysisFilteredCte].[PointOfSaleClassificationLevel2], '') = IsNull([AnalysisDvSalesCte].[PointOfSaleClassificationLevel2], '') AND  AnalysisFilteredCte.Date = AnalysisDvSalesCte.Date
					 GROUP BY [AnalysisFilteredCte].[City], [AnalysisFilteredCte].[ErpCode], [AnalysisFilteredCte].[PointOfSale], [AnalysisFilteredCte].[ImageUrl], [AnalysisFilteredCte].[ChannelName], [AnalysisFilteredCte].[PointOfSaleClassificationLevel1], [AnalysisFilteredCte].[PointOfSaleClassificationLevel2], AnalysisFilteredCte.Date
),
ReportCte AS 
(
SELECT IsNull([StockCte].[City], [AnalysisCte].[City]) AS City, IsNull([StockCte].[ErpCode], [AnalysisCte].[ErpCode]) AS ErpCode, IsNull([StockCte].[PointOfSale], [AnalysisCte].[PointOfSale]) AS PointOfSale, IsNull([StockCte].[ImageUrl], [AnalysisCte].[ImageUrl]) AS ImageUrl, IsNull([StockCte].[ChannelName], [AnalysisCte].[ChannelName]) AS ChannelName, IsNull([StockCte].[PointOfSaleClassificationLevel1], [AnalysisCte].[PointOfSaleClassificationLevel1]) AS PointOfSaleClassificationLevel1, IsNull([StockCte].[PointOfSaleClassificationLevel2], [AnalysisCte].[PointOfSaleClassificationLevel2]) AS PointOfSaleClassificationLevel2, 
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
						 FULL OUTER JOIN  StockDvCte
															ON 
 IsNull([StockCte].[City], '') = IsNull([StockDvCte].[City], '')
 AND  IsNull([StockCte].[ErpCode], '') = IsNull([StockDvCte].[ErpCode], '')
 AND  IsNull([StockCte].[PointOfSale], '') = IsNull([StockDvCte].[PointOfSale], '')
 AND  IsNull([StockCte].[ImageUrl], '') = IsNull([StockDvCte].[ImageUrl], '')
 AND  IsNull([StockCte].[ChannelName], '') = IsNull([StockDvCte].[ChannelName], '')
 AND  IsNull([StockCte].[PointOfSaleClassificationLevel1], '') = IsNull([StockDvCte].[PointOfSaleClassificationLevel1], '')
 AND  IsNull([StockCte].[PointOfSaleClassificationLevel2], '') = IsNull([StockDvCte].[PointOfSaleClassificationLevel2], '') AND  StockCte.[Date] = StockDvCte.[Date]
						 FULL OUTER JOIN  AnalysisCte
															ON 
 IsNull([StockCte].[City], '') = IsNull([AnalysisCte].[City], '')
 AND  IsNull([StockCte].[ErpCode], '') = IsNull([AnalysisCte].[ErpCode], '')
 AND  IsNull([StockCte].[PointOfSale], '') = IsNull([AnalysisCte].[PointOfSale], '')
 AND  IsNull([StockCte].[ImageUrl], '') = IsNull([AnalysisCte].[ImageUrl], '')
 AND  IsNull([StockCte].[ChannelName], '') = IsNull([AnalysisCte].[ChannelName], '')
 AND  IsNull([StockCte].[PointOfSaleClassificationLevel1], '') = IsNull([AnalysisCte].[PointOfSaleClassificationLevel1], '')
 AND  IsNull([StockCte].[PointOfSaleClassificationLevel2], '') = IsNull([AnalysisCte].[PointOfSaleClassificationLevel2], '') AND  StockCte.[Date] = AnalysisCte.[Date]
)
SELECT [ReportCte].[City], [ReportCte].[ErpCode], [ReportCte].[PointOfSale], [ReportCte].[ImageUrl], [ReportCte].[ChannelName], [ReportCte].[PointOfSaleClassificationLevel1], [ReportCte].[PointOfSaleClassificationLevel2], 
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
 ReportCte.StockCost
					FROM ReportCte
					
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
					OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY