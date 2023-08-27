WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[Description] AS [ProductDescription], 
		[Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], 			   
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
		            	 INNER JOIN  ProductsCte
																		ON 
 [SalesAccelerations].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[PointOfSale], [ErpCode], [ImageUrl], [ProductCode], [ProductDescription], [UrlImage], 
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
						
						