WITH
GroupedCte AS 
(
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
		            	
						 GROUP BY CompanyDemandForecast.[Week]
)
SELECT [Week], 
							
							TotalStock, 
 Stock, 
 AccumulatedReceptions, 
 Receptions, 
 Sales, 
 Breakage, 
 SimulatedReceptions, 
 PointOfSaleDeliveries, 
 Orders, 
 OrdersAdjustedToMinimumAndMultiple, 
 OrdersAdjustedToCapacity, 
 OrdersAdjustedToCapacityAdvance, 
 Capacity, 
 AccumulatedSales, 
 AccumulatedPointOfSalesDeliveries, 
 AccumulatedOrders, 
 AccumulatedOrdersAdjustedToMinimunAndMultiple, 
 AccumulatedOrdersAdjustedToCapacity, 
 AccumulatedOrdersAdjustedToCapacityAdvance
						FROM GroupedCte
						ORDER BY [Week]
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY