WITH
GroupedCte AS 
(
SELECT 			   
						   MAX(SalesAccelerations.RootProductSellThrough) AS RootProductSellThrough, 
 MAX(SalesAccelerations.DaysInStore) AS DaysInStore, 
 SUM(SalesAccelerations.InitialStock) AS InitialStock, 
 SUM(SalesAccelerations.ActualStock) AS ActualStock, 
 SUM(SalesAccelerations.PendingReceptions) AS PendingReceptions, 
 SUM(SalesAccelerations.Sales) AS Sales, 
 SUM(SalesAccelerations.WarehouseStock) AS WarehouseStock						   
						FROM Fact.SalesAccelerations
						
		            	
						
)
SELECT 
							
							RootProductSellThrough, 
 DaysInStore, 
 InitialStock, 
 ActualStock, 
 PendingReceptions, 
 Sales, 
 WarehouseStock
						FROM GroupedCte
						
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY