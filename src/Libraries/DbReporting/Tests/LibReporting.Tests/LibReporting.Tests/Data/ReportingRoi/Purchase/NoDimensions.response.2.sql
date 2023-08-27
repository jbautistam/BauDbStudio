WITH
GroupedCte AS 
(
SELECT 			   
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
		            	
						
)
SELECT 
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
 ReductionDueToCapacity
						FROM GroupedCte
						
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY