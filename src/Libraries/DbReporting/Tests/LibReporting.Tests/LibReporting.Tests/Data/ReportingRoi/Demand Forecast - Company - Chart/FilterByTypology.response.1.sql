WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId]
	FROM [Dim].[Products] AS [Products]
	INNER JOIN [Dim].[Typologies] AS [Typologies]
	ON [Products].[TypologyId] = [Typologies].[Id]
	WHERE [Typologies].[Name] = 'Nuñez de Arenas'


)
SELECT CompanyDemandForecast.[Week] AS [Week],   
						   SUM(CompanyDemandForecast.AccumulatedReceptions + CompanyDemandForecast.Receptions + CompanyDemandForecast.SimulatedReceptions + CompanyDemandForecast.Stock) AS TotalStock, 
 SUM(CompanyDemandForecast.Stock) AS Stock, 
 SUM(CompanyDemandForecast.AccumulatedReceptions) AS AccumulatedReceptions, 
 SUM(CompanyDemandForecast.Receptions) AS Receptions, 
 SUM(CompanyDemandForecast.Sales) AS Sales, 
 SUM(CompanyDemandForecast.Breakage) AS Breakage, 
 SUM(CompanyDemandForecast.SimulatedReceptions) AS SimulatedReceptions, 
 SUM(CompanyDemandForecast.PointOfSaleDeliveries) AS PointOfSaleDeliveries, 
 SUM(CompanyDemandForecast.Orders) AS Orders, 
 SUM(CompanyDemandForecast.OrdersAdjustedToMinimumAndMultiple) AS OrdersAdjustedToMinimumAndMultiple, 
 SUM(CompanyDemandForecast.OrdersAdjustedToCapacity) AS OrdersAdjustedToCapacity, 
 SUM(CompanyDemandForecast.OrdersAdjustedToCapacityAdvance) AS OrdersAdjustedToCapacityAdvance, 
 SUM(CompanyDemandForecast.Capacity) AS Capacity, 
 SUM(CompanyDemandForecast.AccumulatedSales) AS AccumulatedSales, 
 SUM(CompanyDemandForecast.AccumulatedPointOfSalesDeliveries) AS AccumulatedPointOfSalesDeliveries, 
 SUM(CompanyDemandForecast.AccumulatedOrders) AS AccumulatedOrders, 
 SUM(CompanyDemandForecast.AccumulatedOrdersAdjustedToMinimunAndMultiple) AS AccumulatedOrdersAdjustedToMinimunAndMultiple, 
 SUM(CompanyDemandForecast.AccumulatedOrdersAdjustedToCapacity) AS AccumulatedOrdersAdjustedToCapacity, 
 SUM(CompanyDemandForecast.AccumulatedOrdersAdjustedToCapacityAdvance) AS AccumulatedOrdersAdjustedToCapacityAdvance						   
						FROM Fact.CompanyDemandForecast
		            	 INNER JOIN  ProductsCte
															ON 
 [CompanyDemandForecast].[ProductId] = [ProductsCte].[ProductId]
		            	GROUP BY CompanyDemandForecast.[Week]
		            	ORDER BY CompanyDemandForecast.[Week]