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
 AccumulatedOrdersAdjustedToCapacityAdvance,
 COUNT(*) OVER () AS TotalCount, 
 SUM(TotalStock) OVER() AS TotalTotalStock, 
 SUM(Stock) OVER() AS TotalStocks, 
 SUM(AccumulatedReceptions) OVER() AS TotalAccumulatedReceptions, 
 SUM(Receptions) OVER() AS TotalReceptions, 
 SUM(Sales) OVER() AS TotalSales, 
 SUM(Breakage) OVER() AS TotalBreakage, 
 SUM(SimulatedReceptions) OVER() AS TotalSimulatedReceptions, 
 SUM(PointOfSaleDeliveries) OVER() AS TotalPointOfSaleDeliveries, 
 SUM(Orders) OVER() AS TotalOrders, 
 SUM(OrdersAdjustedToMinimumAndMultiple) OVER() AS TotalOrdersAdjustedToMinimumAndMultiple, 
 SUM(OrdersAdjustedToCapacity) OVER() AS TotalOrdersAdjustedToCapacity, 
 SUM(OrdersAdjustedToCapacityAdvance) OVER() AS TotalOrdersAdjustedToCapacityAdvance, 
 SUM(Capacity) OVER() AS TotalCapacity, 
 SUM(AccumulatedSales) OVER() AS TotalAccumulatedSales, 
 SUM(AccumulatedPointOfSalesDeliveries) OVER() AS TotalAccumulatedPointOfSalesDeliveries, 
 SUM(AccumulatedOrders) OVER() AS TotalAccumulatedOrders, 
 SUM(AccumulatedOrdersAdjustedToMinimunAndMultiple) OVER() AS TotalAccumulatedOrdersAdjustedToMinimunAndMultiple, 
 SUM(AccumulatedOrdersAdjustedToCapacity) OVER() AS TotalAccumulatedOrdersAdjustedToCapacity, 
 SUM(AccumulatedOrdersAdjustedToCapacityAdvance) OVER() AS TotalAccumulatedOrdersAdjustedToCapacityAdvance
						FROM GroupedCte
						ORDER BY [Week]
						