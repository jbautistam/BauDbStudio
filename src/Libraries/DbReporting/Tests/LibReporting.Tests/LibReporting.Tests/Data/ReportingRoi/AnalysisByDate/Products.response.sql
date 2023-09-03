WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[Description] AS [ProductDescription], [Products].[EanCode] AS [EanCode], 
		[Products].[ProductCode] AS [ProductCode], [Products].[UrlImage] AS [UrlImage], [ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue1] AS [ClassificationLevelValue1], 
		[ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue3] AS [ClassificationLevelValue3], 
		[ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue5] AS [ClassificationLevelValue5], 
		[Typologies].[Name] AS [Typology]
	FROM [Dim].[Products] AS [Products]
	INNER JOIN [Dim].[ProductClassificationLevelValuesReadingModel] AS [ProductClassificationLevelValuesReadingModel]
	ON [Products].[ProductClassificationLevelValuesReadingModelId] = [ProductClassificationLevelValuesReadingModel].[Id]
	INNER JOIN [Dim].[Typologies] AS [Typologies]
	ON [Products].[TypologyId] = [Typologies].[Id]


),
StockIntervalsCte AS 
(
SELECT 
						   [ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], 
						   DailyStocksIntervals.ProductId, DailyStocksIntervals.PointOfSaleId,
						   DailyStocksIntervals.StartDate, DailyStocksIntervals.EndDate, 
						   DailyStocksIntervals.Quantity
						FROM Fact.DailyStocksIntervals
						
		            	 INNER JOIN  ProductsCte
															ON 
 [DailyStocksIntervals].[ProductId] = [ProductsCte].[ProductId]
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
						   [ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], 
						   SUM(StockIntervalsCte.Quantity) AS Stock,
						   SUM(StockIntervalsCte.Quantity * ProductCostAndPriceByPointOfSaleIntervals.Cost) AS StockCost
						FROM StockIntervalsCte
						
		            	 INNER JOIN  ProductsCte
															ON 
 [StockIntervalsCte].[ProductId] = [ProductsCte].[ProductId]
						INNER JOIN Dim.CalendarIso
							ON CalendarIso.[Date] BETWEEN StockIntervalsCte.StartDate AND StockIntervalsCte.EndDate
								AND CalendarIso.Date BETWEEN @StartDate AND @EndDate
						INNER JOIN Fact.ProductCostAndPriceByPointOfSaleIntervals
							ON StockIntervalsCte.ProductId = ProductCostAndPriceByPointOfSaleIntervals.ProductId
								AND StockIntervalsCte.PointOfSaleId = ProductCostAndPriceByPointOfSaleIntervals.PointOfSaleId
								AND CalendarIso.[Date] BETWEEN ProductCostAndPriceByPointOfSaleIntervals.[StartDate] 
														AND ProductCostAndPriceByPointOfSaleIntervals.[EndDate]
						 GROUP BY [ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], CalendarIso.[Date]
),
StockDvCte AS 
(
SELECT CalendarIso.Date,
								[ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], 
							   COUNT(DISTINCT Products.RootProductCode) AS DesignVariationsWithStock				
						  FROM StockIntervalsCte INNER JOIN Dim.Products
							ON StockIntervalsCte.ProductId = Products.Id
						  INNER JOIN Dim.CalendarIso
							ON CalendarIso.[Date] BETWEEN StockIntervalsCte.StartDate AND StockIntervalsCte.EndDate
								AND CalendarIso.Date BETWEEN @StartDate AND @EndDate
						 INNER JOIN  ProductsCte
															ON 
 [StockIntervalsCte].[ProductId] = [ProductsCte].[ProductId]
						
						 GROUP BY [ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], CalendarIso.Date
),
AnalysisFilteredCte AS 
(
SELECT [ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], 
						   Analysis.ProductId, Analysis.PointOfSaleId,
						   Products.RootProductCode AS AnalysisRootProductCode,
						   Analysis.Date, Analysis.StockBreakage, Analysis.StockBreakageAmount,
						   Analysis.Sales, Analysis.SalesTaxesIncluded, Analysis.SalesTaxesExcluded,
						   Analysis.SalesOutlier, Analysis.SalesOutlierTaxesIncluded,
						   Analysis.SalesOutlierTaxesExcluded
						FROM Fact.Analysis
						
		            	 INNER JOIN  ProductsCte
															ON 
 [Analysis].[ProductId] = [ProductsCte].[ProductId]
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
SELECT [ProductDescription], [EanCode], [ProductCode], [UrlImage], [ClassificationLevelValue1], [ClassificationLevelValue3], [ClassificationLevelValue5], [Typology], 
								   Date, COUNT(1) AS DesignVariationsWithSales
								FROM AnalysisFilteredCte
								 GROUP BY [ProductDescription], [EanCode], [ProductCode], [UrlImage], [ClassificationLevelValue1], [ClassificationLevelValue3], [ClassificationLevelValue5], [Typology], AnalysisRootProductCode, Date
								HAVING SUM(Sales) > 0
),
AnalysisCte AS 
(
SELECT AnalysisFilteredCte.Date,
						   [AnalysisFilteredCte].[ProductDescription], [AnalysisFilteredCte].[EanCode], [AnalysisFilteredCte].[ProductCode], [AnalysisFilteredCte].[UrlImage], [AnalysisFilteredCte].[ClassificationLevelValue1], [AnalysisFilteredCte].[ClassificationLevelValue3], [AnalysisFilteredCte].[ClassificationLevelValue5], [AnalysisFilteredCte].[Typology], 
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
 IsNull([AnalysisFilteredCte].[ProductDescription], '') = IsNull([AnalysisDvSalesCte].[ProductDescription], '')
 AND  IsNull([AnalysisFilteredCte].[EanCode], '') = IsNull([AnalysisDvSalesCte].[EanCode], '')
 AND  IsNull([AnalysisFilteredCte].[ProductCode], '') = IsNull([AnalysisDvSalesCte].[ProductCode], '')
 AND  IsNull([AnalysisFilteredCte].[UrlImage], '') = IsNull([AnalysisDvSalesCte].[UrlImage], '')
 AND  IsNull([AnalysisFilteredCte].[ClassificationLevelValue1], '') = IsNull([AnalysisDvSalesCte].[ClassificationLevelValue1], '')
 AND  IsNull([AnalysisFilteredCte].[ClassificationLevelValue3], '') = IsNull([AnalysisDvSalesCte].[ClassificationLevelValue3], '')
 AND  IsNull([AnalysisFilteredCte].[ClassificationLevelValue5], '') = IsNull([AnalysisDvSalesCte].[ClassificationLevelValue5], '')
 AND  IsNull([AnalysisFilteredCte].[Typology], '') = IsNull([AnalysisDvSalesCte].[Typology], '') AND  AnalysisFilteredCte.Date = AnalysisDvSalesCte.Date
					 GROUP BY [AnalysisFilteredCte].[ProductDescription], [AnalysisFilteredCte].[EanCode], [AnalysisFilteredCte].[ProductCode], [AnalysisFilteredCte].[UrlImage], [AnalysisFilteredCte].[ClassificationLevelValue1], [AnalysisFilteredCte].[ClassificationLevelValue3], [AnalysisFilteredCte].[ClassificationLevelValue5], [AnalysisFilteredCte].[Typology], AnalysisFilteredCte.Date
),
ReportCte AS 
(
SELECT IsNull([StockCte].[ProductDescription], [AnalysisCte].[ProductDescription]) AS ProductDescription, IsNull([StockCte].[EanCode], [AnalysisCte].[EanCode]) AS EanCode, IsNull([StockCte].[ProductCode], [AnalysisCte].[ProductCode]) AS ProductCode, IsNull([StockCte].[UrlImage], [AnalysisCte].[UrlImage]) AS UrlImage, IsNull([StockCte].[ClassificationLevelValue1], [AnalysisCte].[ClassificationLevelValue1]) AS ClassificationLevelValue1, IsNull([StockCte].[ClassificationLevelValue3], [AnalysisCte].[ClassificationLevelValue3]) AS ClassificationLevelValue3, IsNull([StockCte].[ClassificationLevelValue5], [AnalysisCte].[ClassificationLevelValue5]) AS ClassificationLevelValue5, IsNull([StockCte].[Typology], [AnalysisCte].[Typology]) AS Typology, 
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
 IsNull([StockCte].[ProductDescription], '') = IsNull([StockDvCte].[ProductDescription], '')
 AND  IsNull([StockCte].[EanCode], '') = IsNull([StockDvCte].[EanCode], '')
 AND  IsNull([StockCte].[ProductCode], '') = IsNull([StockDvCte].[ProductCode], '')
 AND  IsNull([StockCte].[UrlImage], '') = IsNull([StockDvCte].[UrlImage], '')
 AND  IsNull([StockCte].[ClassificationLevelValue1], '') = IsNull([StockDvCte].[ClassificationLevelValue1], '')
 AND  IsNull([StockCte].[ClassificationLevelValue3], '') = IsNull([StockDvCte].[ClassificationLevelValue3], '')
 AND  IsNull([StockCte].[ClassificationLevelValue5], '') = IsNull([StockDvCte].[ClassificationLevelValue5], '')
 AND  IsNull([StockCte].[Typology], '') = IsNull([StockDvCte].[Typology], '') AND  StockCte.[Date] = StockDvCte.[Date]
						 FULL OUTER JOIN  AnalysisCte
															ON 
 IsNull([StockCte].[ProductDescription], '') = IsNull([AnalysisCte].[ProductDescription], '')
 AND  IsNull([StockCte].[EanCode], '') = IsNull([AnalysisCte].[EanCode], '')
 AND  IsNull([StockCte].[ProductCode], '') = IsNull([AnalysisCte].[ProductCode], '')
 AND  IsNull([StockCte].[UrlImage], '') = IsNull([AnalysisCte].[UrlImage], '')
 AND  IsNull([StockCte].[ClassificationLevelValue1], '') = IsNull([AnalysisCte].[ClassificationLevelValue1], '')
 AND  IsNull([StockCte].[ClassificationLevelValue3], '') = IsNull([AnalysisCte].[ClassificationLevelValue3], '')
 AND  IsNull([StockCte].[ClassificationLevelValue5], '') = IsNull([AnalysisCte].[ClassificationLevelValue5], '')
 AND  IsNull([StockCte].[Typology], '') = IsNull([AnalysisCte].[Typology], '') AND  StockCte.[Date] = AnalysisCte.[Date]
)
SELECT [ReportCte].[ProductDescription], [ReportCte].[EanCode], [ReportCte].[ProductCode], [ReportCte].[UrlImage], [ReportCte].[ClassificationLevelValue1], [ReportCte].[ClassificationLevelValue3], [ReportCte].[ClassificationLevelValue5], [ReportCte].[Typology], 
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
					