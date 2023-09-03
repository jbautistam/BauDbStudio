WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[EanCode] AS [EanCode], [Products].[ProductCode] AS [ProductCode], 
		[Products].[Description] AS [ProductDescription], [Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
GroupedCte AS 
(
SELECT [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], 			   
						   MIN(PurchaseReports.MinimumMultiple) AS MinimumMultiple, 
 MIN(PurchaseReports.SupplierLeadTime) AS SupplierLeadTime, 
 MIN(PurchaseReports.Periodicity) AS Periodicity, 
 MIN(PurchaseReports.WarehouseCoverage) AS WarehouseCoverage, 
 SUM(PurchaseReports.ExpectedReplenishment) AS ExpectedReplenishment, 
 SUM(PurchaseReports.ActualPlannedWarehouse) AS ActualPlannedWarehouse, 
 SUM(PurchaseReports.Orders) AS Orders, 
 SUM(PurchaseReports.OrdersAdjustedToMinimumAndMultiple) AS OrdersAdjustedToMinimumAndMultiple, 
 SUM(PurchaseReports.AdjustedOrderCost) AS AdjustedOrderCost, 
 SUM(PurchaseReports.RupturesWarehouse) AS RupturesWarehouse, 
 SUM(PurchaseReports.RupturesStores) AS RupturesStores, 
 SUM(PurchaseReports.PlannedSalesToReception) AS PlannedSalesToReception, 
 SUM(PurchaseReports.IdealWindow) AS IdealWindow, 
 SUM(PurchaseReports.PendingReceptionsWarehouse) AS PendingReceptionsWarehouse, 
 SUM(PurchaseReports.PendingReceptionsStores) AS PendingReceptionsStores, 
 SUM(PurchaseReports.ActualStockWarehouse) AS ActualStockWarehouse, 
 SUM(PurchaseReports.ActualStockStores) AS ActualStockStores, 
 SUM(PurchaseReports.ShippingToStoresToReception) AS ShippingToStoresToReception, 
 SUM(PurchaseReports.ReceptionWarehouseToReception) AS ReceptionWarehouseToReception, 
 SUM(PurchaseReports.ReceptionStoreToReception) AS ReceptionStoreToReception, 
 SUM(PurchaseReports.SalesTwoMonths) AS SalesTwoMonths, 
 SUM(PurchaseReports.SalesSixMonths) AS SalesSixMonths, 
 SUM(PurchaseReports.AdjustedOrderCapacity) AS AdjustedOrderCapacity, 
 SUM(PurchaseReports.AdjustedOrderCapacityAdvanced) AS AdjustedOrderCapacityAdvanced, 
 SUM(PurchaseReports.AdvancedOrder) AS AdvancedOrder, 
 SUM(PurchaseReports.ReductionDueToCapacity) AS ReductionDueToCapacity						   
						FROM Fact.PurchaseReports
		            	 INNER JOIN  ProductsCte
															ON 
 [PurchaseReports].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT [EanCode], [ProductCode], [ProductDescription], [UrlImage], 
							MinimumMultiple, 
 SupplierLeadTime, 
 Periodicity, 
 WarehouseCoverage, 
 ExpectedReplenishment, 
 ActualPlannedWarehouse, 
 Orders, 
 OrdersAdjustedToMinimumAndMultiple, 
 AdjustedOrderCost, 
 RupturesWarehouse, 
 RupturesStores, 
 PlannedSalesToReception, 
 IdealWindow, 
 PendingReceptionsWarehouse, 
 PendingReceptionsStores, 
 ActualStockWarehouse, 
 ActualStockStores, 
 ShippingToStoresToReception, 
 ReceptionWarehouseToReception, 
 ReceptionStoreToReception, 
 SalesTwoMonths, 
 SalesSixMonths, 
 AdjustedOrderCapacity, 
 AdjustedOrderCapacityAdvanced, 
 AdvancedOrder, 
 ReductionDueToCapacity,
 COUNT(*) OVER () AS TotalCount, 
 MIN(MinimumMultiple) OVER() AS TotalMinimumMultiple, 
 MIN(SupplierLeadTime) OVER() AS TotalSupplierLeadTime, 
 MIN(Periodicity) OVER() AS TotalPeriodicity, 
 MIN(WarehouseCoverage) OVER() AS TotalWarehouseCoverage, 
 SUM(ExpectedReplenishment) OVER() AS TotalExpectedReplenishment, 
 SUM(ActualPlannedWarehouse) OVER() AS TotalActualPlannedWarehouse, 
 SUM(Orders) OVER() AS TotalOrders, 
 SUM(OrdersAdjustedToMinimumAndMultiple) OVER() AS TotalOrdersAdjustedToMinimumAndMultiple, 
 SUM(AdjustedOrderCost) OVER() AS TotalAdjustedOrderCost, 
 SUM(RupturesWarehouse) OVER() AS TotalRupturesWarehouse, 
 SUM(RupturesStores) OVER() AS TotalRupturesStores, 
 SUM(PlannedSalesToReception) OVER() AS TotalPlannedSalesToReception, 
 SUM(IdealWindow) OVER() AS TotalIdealWindow, 
 SUM(PendingReceptionsWarehouse) OVER() AS TotalPendingReceptionsWarehouse, 
 SUM(PendingReceptionsStores) OVER() AS TotalPendingReceptionsStores, 
 SUM(ActualStockWarehouse) OVER() AS TotalActualStockWarehouse, 
 SUM(ActualStockStores) OVER() AS TotalActualStockStores, 
 SUM(ShippingToStoresToReception) OVER() AS TotalShippingToStoresToReception, 
 SUM(ReceptionWarehouseToReception) OVER() AS TotalReceptionWarehouseToReception, 
 SUM(ReceptionStoreToReception) OVER() AS TotalReceptionStoreToReception, 
 SUM(SalesTwoMonths) OVER() AS TotalSalesTwoMonths, 
 SUM(SalesSixMonths) OVER() AS TotalSalesSixMonths, 
 SUM(AdjustedOrderCapacity) OVER() AS TotalAdjustedOrderCapacity, 
 SUM(AdjustedOrderCapacityAdvanced) OVER() AS TotalAdjustedOrderCapacityAdvanced, 
 SUM(AdvancedOrder) OVER() AS TotalAdvancedOrder, 
 SUM(ReductionDueToCapacity) OVER() AS TotalReductionDueToCapacity
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY