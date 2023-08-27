WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[RootProductCode] AS [RootProductCode], 
		[Products].[Description] AS [ProductDescription], [Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
GroupedCte AS 
(
SELECT [ProductsCte].[ProductCode], [ProductsCte].[RootProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], 			   
						   MAX(SalesAccelerations.RootProductSellThrough) AS RootProductSellThrough, 
 MAX(SalesAccelerations.DaysInStore) AS DaysInStore, 
 SUM(SalesAccelerations.InitialStock) AS InitialStock, 
 SUM(SalesAccelerations.ActualStock) AS ActualStock, 
 SUM(SalesAccelerations.PendingReceptions) AS PendingReceptions, 
 SUM(SalesAccelerations.Sales) AS Sales, 
 SUM(SalesAccelerations.WarehouseStock) AS WarehouseStock						   
						FROM Fact.SalesAccelerations
						
		            	 INNER JOIN  ProductsCte
																		ON 
 [SalesAccelerations].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[RootProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[ProductCode], [RootProductCode], [ProductDescription], [UrlImage], 
							RootProductSellThrough, 
 DaysInStore, 
 InitialStock, 
 ActualStock, 
 PendingReceptions, 
 Sales, 
 WarehouseStock,
 COUNT(*) OVER () AS TotalCount, 
 MAX(RootProductSellThrough) OVER() AS TotalRootProductSellThrough, 
 MAX(DaysInStore) OVER() AS TotalDaysInStore, 
 SUM(InitialStock) OVER() AS TotalInitialStock, 
 SUM(ActualStock) OVER() AS TotalActualStock, 
 SUM(PendingReceptions) OVER() AS TotalPendingReceptions, 
 SUM(Sales) OVER() AS TotalSales, 
 SUM(WarehouseStock) OVER() AS TotalWarehouseStock
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY