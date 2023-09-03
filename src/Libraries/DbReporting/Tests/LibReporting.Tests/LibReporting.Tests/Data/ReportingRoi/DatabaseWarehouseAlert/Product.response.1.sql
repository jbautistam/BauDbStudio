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
						   SUM(DataBaseWarehouses.NegativeStock) AS NegativeStock, 
 SUM(DataBaseWarehouses.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBaseWarehouses
						
		            	 INNER JOIN  ProductsCte
															ON 
 [DataBaseWarehouses].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[RootProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[ProductCode], [RootProductCode], [ProductDescription], [UrlImage], 
							NegativeStock, 
 NegativePendingReceptions,
 COUNT(*) OVER () AS TotalCount, 
 SUM(NegativeStock) OVER() AS TotalNegativeStock, 
 SUM(NegativePendingReceptions) OVER() AS TotalNegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY