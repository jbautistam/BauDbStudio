WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[Description] AS [ProductDescription], 
		[Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
ProductsSubstituteCte AS 
(
SELECT [ProductSubstituteProducts].[Id] AS [ProductSubstituteProductId], [ProductSubstituteProducts].[ProductCode] AS [ProductSubstituteProductCode], 
		[ProductSubstituteProducts].[Description] AS [ProductSubstituteProductDescription], [ProductSubstituteProducts].[UrlImage] AS [ProductSubstituteUrlImage]
	FROM [Dim].[Products] AS [ProductSubstituteProducts]


),
GroupedCte AS 
(
SELECT [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage], 
						   MIN(SubstituteReport.Type) AS Type, 
 MIN(SubstituteReport.Quantity) AS Quantity
                    FROM Fact.SubstituteReport
						
		            	 INNER JOIN  ProductsCte
																		ON 
 [SubstituteReport].[ProductId] = [ProductsCte].[ProductId]
						 INNER JOIN  ProductsSubstituteCte
																		ON 
 [SubstituteReport].[RelatedProductId] = [ProductsSubstituteCte].[ProductSubstituteProductId]
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage]
)
SELECT [ProductCode], [ProductDescription], [UrlImage], [ProductSubstituteProductCode], [ProductSubstituteProductDescription], [ProductSubstituteUrlImage], 
						   Type, 
 Quantity,
 COUNT(*) OVER () AS TotalCount, 
 MIN(Type) OVER () AS Type, 
 MIN(Quantity) OVER () AS Quantity
                    FROM GroupedCte
					
					