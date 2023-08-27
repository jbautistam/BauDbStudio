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
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[ErpCode] AS [ErpCode], [PointsOfSale].[Name] AS [PointOfSale], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
GroupedCte AS 
(
SELECT PointOfSaleDemandForecast.[Week] AS [Week],
						   [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue10], [ProductsCte].[Typology], 			   
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
						 INNER JOIN  PointsOfSaleCte
																		ON 
 [PointOfSaleDemandForecast].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	 INNER JOIN  ProductsCte
																		ON 
 [PointOfSaleDemandForecast].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue10], [ProductsCte].[Typology], PointOfSaleDemandForecast.[Week]
)
SELECT [Week], 
							[ErpCode], [PointOfSale], [ImageUrl], [EanCode], [ProductCode], [ProductDescription], [UrlImage], [ClassificationLevelValue1], [ClassificationLevelValue10], [Typology], 
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
 AccumulatedPurchaseOrderMatchedToMinimunAndMultiple
						FROM GroupedCte
						ORDER BY [Week]
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY