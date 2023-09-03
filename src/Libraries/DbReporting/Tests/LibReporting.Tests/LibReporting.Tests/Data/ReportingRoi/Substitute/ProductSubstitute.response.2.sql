WITH
ProductsSubstituteCte AS 
(
SELECT [ProductSubstituteProducts].[Id] AS [ProductSubstituteProductId], [ProductSubstituteProducts].[ProductCode] AS [ProductSubstituteProductCode], 
		[ProductSubstituteProducts].[Description] AS [ProductSubstituteProductDescription], [ProductSubstituteProducts].[UrlImage] AS [ProductSubstituteUrlImage]
	FROM [Dim].[Products] AS [ProductSubstituteProducts]


),
GroupedCte AS 
(
SELECT [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage], 
						   MIN(SubstituteReport.Type) AS Type, 
 MIN(SubstituteReport.Quantity) AS Quantity
                    FROM Fact.SubstituteReport
						
		            	
						 INNER JOIN  ProductsSubstituteCte
															ON 
 [SubstituteReport].[RelatedProductId] = [ProductsSubstituteCte].[ProductSubstituteProductId]
						 GROUP BY [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage]
)
SELECT [ProductSubstituteProductCode], [ProductSubstituteProductDescription], [ProductSubstituteUrlImage], 
						   Type, 
 Quantity
                    FROM GroupedCte
					
					OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY