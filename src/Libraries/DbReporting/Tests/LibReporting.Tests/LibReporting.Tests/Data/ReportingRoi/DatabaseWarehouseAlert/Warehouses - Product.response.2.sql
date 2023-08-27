WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[RootProductCode] AS [RootProductCode], 
		[Products].[Description] AS [ProductDescription], [Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
WarehousesCte AS 
(
SELECT [Warehouses].[Id] AS [WarehouseId], [Warehouses].[ErpCode] AS [ErpCode], [Warehouses].[Name] AS [WarehouseName]
	FROM [Dim].[Warehouses] AS [Warehouses]


),
GroupedCte AS 
(
SELECT [WarehousesCte].[ErpCode], [WarehousesCte].[WarehouseName], [ProductsCte].[ProductCode], [ProductsCte].[RootProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], 			   
						   SUM(DataBaseWarehouses.NegativeStock) AS NegativeStock, 
 SUM(DataBaseWarehouses.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBaseWarehouses
						 INNER JOIN  WarehousesCte
																		ON 
 [DataBaseWarehouses].[WarehouseId] = [WarehousesCte].[WarehouseId]
		            	 INNER JOIN  ProductsCte
																		ON 
 [DataBaseWarehouses].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [WarehousesCte].[ErpCode], [WarehousesCte].[WarehouseName], [ProductsCte].[ProductCode], [ProductsCte].[RootProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[ErpCode], [WarehouseName], [ProductCode], [RootProductCode], [ProductDescription], [UrlImage], 
							NegativeStock, 
 NegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY