WITH
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], 			   
						   MAX(SalesAccelerations.ProductSellThrough) AS ProductSellThrough, 
 MAX(SalesAccelerations.RootProductSellThrough) AS RootProductSellThrough, 
 MAX(SalesAccelerations.DaysInStore) AS DaysInStore, 
 SUM(SalesAccelerations.InitialStock) AS InitialStock, 
 SUM(SalesAccelerations.ActualStock) AS ActualStock, 
 SUM(SalesAccelerations.PendingReceptions) AS PendingReceptions, 
 SUM(SalesAccelerations.Sales) AS Sales, 
 SUM(SalesAccelerations.WarehouseStock) AS WarehouseStock						   
						FROM Fact.SalesAccelerations
						 INNER JOIN  PointsOfSaleCte
															ON 
 [SalesAccelerations].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl]
)
SELECT 
							[PointOfSale], [ErpCode], [ImageUrl], 
							ProductSellThrough, 
 RootProductSellThrough, 
 DaysInStore, 
 InitialStock, 
 ActualStock, 
 PendingReceptions, 
 Sales, 
 WarehouseStock,
 COUNT(*) OVER () AS TotalCount, 
 MAX(ProductSellThrough) OVER() AS TotalProductSellThrough, 
 MAX(RootProductSellThrough) OVER() AS TotalRootProductSellThrough, 
 MAX(DaysInStore) OVER() AS TotalDaysInStore, 
 SUM(InitialStock) OVER() AS TotalInitialStock, 
 SUM(ActualStock) OVER() AS TotalActualStock, 
 SUM(PendingReceptions) OVER() AS TotalPendingReceptions, 
 SUM(Sales) OVER() AS TotalSales, 
 SUM(WarehouseStock) OVER() AS TotalWarehouseStock
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY