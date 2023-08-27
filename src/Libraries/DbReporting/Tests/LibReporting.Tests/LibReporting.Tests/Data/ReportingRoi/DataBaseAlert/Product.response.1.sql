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
						   SUM(DataBasePointOfSales.NegativeStock) AS NegativeStock, 
 SUM(DataBasePointOfSales.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBasePointOfSales
						
		            	 INNER JOIN  ProductsCte
																		ON 
 [DataBasePointOfSales].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[ProductCode], [ProductDescription], [UrlImage], 
							NegativeStock, 
 NegativePendingReceptions,
 COUNT(*) OVER () AS TotalCount, 
 SUM(NegativeStock) OVER() AS TotalNegativeStock, 
 SUM(NegativePendingReceptions) OVER() AS TotalNegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY