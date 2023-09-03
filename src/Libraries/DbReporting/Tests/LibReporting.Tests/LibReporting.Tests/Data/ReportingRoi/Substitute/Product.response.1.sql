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
						   MIN(SubstituteReport.Type) AS Type, 
 MIN(SubstituteReport.Quantity) AS Quantity
                    FROM Fact.SubstituteReport
						
		            	 INNER JOIN  ProductsCte
															ON 
 [SubstituteReport].[ProductId] = [ProductsCte].[ProductId]
						
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT [ProductCode], [ProductDescription], [UrlImage], 
						   Type, 
 Quantity,
 COUNT(*) OVER () AS TotalCount, 
 MIN(Type) OVER () AS Type, 
 MIN(Quantity) OVER () AS Quantity
                    FROM GroupedCte
					
					OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY