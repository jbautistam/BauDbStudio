WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[Description] AS [ProductDescription], 
		[Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
GroupedCte AS 
(
SELECT [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], 			   
						   MAX(SalesAccelerations.ProductSellThrough) AS ProductSellThrough, 
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
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[ProductCode], [ProductDescription], [UrlImage], 
							ProductSellThrough, 
 RootProductSellThrough, 
 DaysInStore, 
 InitialStock, 
 ActualStock, 
 PendingReceptions, 
 Sales, 
 WarehouseStock
						FROM GroupedCte
						
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY