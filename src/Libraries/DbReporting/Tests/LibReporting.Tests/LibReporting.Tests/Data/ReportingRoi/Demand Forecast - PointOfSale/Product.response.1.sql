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
SELECT PointOfSaleDemandForecast.[Week] AS [Week],
						   [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue10], [ProductsCte].[Typology], 			   
						   SUM(PointOfSaleDemandForecast.AccumulatedReceptions + PointOfSaleDemandForecast.Receptions + PointOfSaleDemandForecast.SimulatedReceptions + PointOfSaleDemandForecast.Stock) AS TotalStock, 
 SUM(PointOfSaleDemandForecast.Stock) AS Stock, 
 SUM(PointOfSaleDemandForecast.AccumulatedReceptions) AS AccumulatedReceptions, 
 SUM(PointOfSaleDemandForecast.Receptions) AS Receptions, 
 SUM(PointOfSaleDemandForecast.Sales) AS Sales, 
 SUM(PointOfSaleDemandForecast.Breakage) AS Breakage, 
 SUM(PointOfSaleDemandForecast.PurchaseOrder) AS PurchaseOrder, 
 SUM(PointOfSaleDemandForecast.OrdersMatchedToMinimunAndMultiple) AS OrdersMatchedToMinimunAndMultiple, 
 SUM(PointOfSaleDemandForecast.AccumulatedSales) AS AccumulatedSales, 
 SUM(PointOfSaleDemandForecast.AccumulatedOrders) AS AccumulatedOrders, 
 SUM(PointOfSaleDemandForecast.PurchaseOrderMatchedToMinimunAndMultiple) AS PurchaseOrderMatchedToMinimunAndMultiple, 
 SUM(PointOfSaleDemandForecast.AccumulatedPurchaseOrder) AS AccumulatedPurchaseOrder, 
 SUM(PointOfSaleDemandForecast.AccumulatedPurchaseOrderMatchedToMinimunAndMultiple) AS AccumulatedPurchaseOrderMatchedToMinimunAndMultiple						   
						FROM Fact.PointOfSaleDemandForecast
						
		            	 INNER JOIN  ProductsCte
																		ON 
 [PointOfSaleDemandForecast].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue10], [ProductsCte].[Typology], PointOfSaleDemandForecast.[Week]
)
SELECT [Week], 
							[EanCode], [ProductCode], [ProductDescription], [UrlImage], [ClassificationLevelValue1], [ClassificationLevelValue10], [Typology], 
							TotalStock, 
 Stock, 
 AccumulatedReceptions, 
 Receptions, 
 Sales, 
 Breakage, 
 PurchaseOrder, 
 OrdersMatchedToMinimunAndMultiple, 
 AccumulatedSales, 
 AccumulatedOrders, 
 PurchaseOrderMatchedToMinimunAndMultiple, 
 AccumulatedPurchaseOrder, 
 AccumulatedPurchaseOrderMatchedToMinimunAndMultiple,
 COUNT(*) OVER () AS TotalCount, 
 SUM(TotalStock) OVER() AS TotalTotalStock, 
 SUM(Stock) OVER() AS TotalStocks, 
 SUM(AccumulatedReceptions) OVER() AS TotalAccumulatedReceptions, 
 SUM(Receptions) OVER() AS TotalReceptions, 
 SUM(Sales) OVER() AS TotalSales, 
 SUM(Breakage) OVER() AS TotalBreakage, 
 SUM(PurchaseOrder) OVER() AS TotalPurchaseOrder, 
 SUM(OrdersMatchedToMinimunAndMultiple) OVER() AS TotalOrdersMatchedToMinimunAndMultiple, 
 SUM(AccumulatedSales) OVER() AS TotalAccumulatedSales, 
 SUM(AccumulatedOrders) OVER() AS TotalAccumulatedOrders, 
 SUM(PurchaseOrderMatchedToMinimunAndMultiple) OVER() AS TotalPurchaseOrderMatchedToMinimunAndMultiple, 
 SUM(AccumulatedPurchaseOrder) OVER() AS TotalAccumulatedPurchaseOrder, 
 SUM(AccumulatedPurchaseOrderMatchedToMinimunAndMultiple) OVER() AS TotalAccumulatedPurchaseOrderMatchedToMinimunAndMultiple
						FROM GroupedCte
						ORDER BY [Week]
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY