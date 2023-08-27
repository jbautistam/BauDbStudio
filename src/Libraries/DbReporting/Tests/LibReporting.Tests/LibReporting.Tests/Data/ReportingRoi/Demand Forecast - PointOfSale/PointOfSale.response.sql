WITH
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[ErpCode] AS [ErpCode], [PointsOfSale].[Name] AS [PointOfSale], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
GroupedCte AS 
(
SELECT PointOfSaleDemandForecast.[Week] AS [Week],
						   [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], 			   
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
		            	
						 GROUP BY [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], PointOfSaleDemandForecast.[Week]
)
SELECT [Week], 
							[ErpCode], [PointOfSale], [ImageUrl], 
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
						