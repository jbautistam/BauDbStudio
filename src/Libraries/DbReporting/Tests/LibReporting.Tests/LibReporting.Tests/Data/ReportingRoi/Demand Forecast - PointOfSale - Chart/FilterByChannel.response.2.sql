WITH
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]
	INNER JOIN [Dim].[Channels] AS [Channels]
	ON [PointsOfSale].[ChannelId] = [Channels].[Id]
	WHERE [Channels].[Name] = 'Tiendas Propias'


)
SELECT PointOfSaleDemandForecast.[Week] AS [Week],
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
		            	
		            	GROUP BY PointOfSaleDemandForecast.[Week]
		            	ORDER BY PointOfSaleDemandForecast.[Week]