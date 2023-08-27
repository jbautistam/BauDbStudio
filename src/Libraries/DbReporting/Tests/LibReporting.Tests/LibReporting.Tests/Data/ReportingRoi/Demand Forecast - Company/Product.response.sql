WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[EanCode] AS [EanCode], [Products].[ProductCode] AS [ProductCode], 
		[Products].[Description] AS [ProductDescription], [Products].[UrlImage] AS [UrlImage], [ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue1] AS [ClassificationLevelValue1], 
		[ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue10] AS [ClassificationLevelValue10], 
		[Typologies].[Name] AS [Typology]
	FROM [Dim].[Products] AS [Products]
	INNER JOIN [Dim].[ProductClassificationLevelValuesReadingModel] AS [ProductClassificationLevelValuesReadingModel]
	ON [Products].[ProductClassificationLevelValuesReadingModelId] = [ProductClassificationLevelValuesReadingModel].[Id]
	INNER JOIN [Dim].[Typologies] AS [Typologies]
	ON [Products].[TypologyId] = [Typologies].[Id]


),
GroupedCte AS 
(
SELECT CompanyDemandForecast.[Week] AS [Week],
						   [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue10], [ProductsCte].[Typology], 			   
						   SUM(CompanyDemandForecast.AccumulatedReceptions + CompanyDemandForecast.Receptions + CompanyDemandForecast.SimulatedReceptions + CompanyDemandForecast.Stock) AS TotalStock, 
 SUM(CompanyDemandForecast.Stock) AS Stock, 
 SUM(CompanyDemandForecast.AccumulatedReceptions) AS AccumulatedReceptions, 
 SUM(CompanyDemandForecast.Receptions) AS Receptions, 
 SUM(CompanyDemandForecast.Sales) AS Sales, 
 SUM(CompanyDemandForecast.Breakage) AS Breakage, 
 SUM(CompanyDemandForecast.AccumulatedSales) AS AccumulatedSales, 
 SUM(CompanyDemandForecast.AccumulatedOrders) AS AccumulatedOrders						   
						FROM Fact.CompanyDemandForecast
		            	 INNER JOIN  ProductsCte
																		ON 
 [CompanyDemandForecast].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue10], [ProductsCte].[Typology], CompanyDemandForecast.[Week]
)
SELECT [Week], 
							[EanCode], [ProductCode], [ProductDescription], [UrlImage], [ClassificationLevelValue1], [ClassificationLevelValue10], [Typology], 
							TotalStock, 
 Stock, 
 AccumulatedReceptions, 
 Receptions, 
 Sales, 
 Breakage, 
 AccumulatedSales, 
 AccumulatedOrders,
 COUNT(*) OVER () AS TotalCount, 
 SUM(TotalStock) OVER() AS TotalTotalStock, 
 SUM(Stock) OVER() AS TotalStocks, 
 SUM(AccumulatedReceptions) OVER() AS TotalAccumulatedReceptions, 
 SUM(Receptions) OVER() AS TotalReceptions, 
 SUM(Sales) OVER() AS TotalSales, 
 SUM(Breakage) OVER() AS TotalBreakage, 
 SUM(AccumulatedSales) OVER() AS TotalAccumulatedSales, 
 SUM(AccumulatedOrders) OVER() AS TotalAccumulatedOrders
						FROM GroupedCte
						ORDER BY [Week]
						