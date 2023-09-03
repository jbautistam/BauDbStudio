WITH
ProductsSubstituteCte AS 
(
SELECT [ProductSubstituteProducts].[Id] AS [ProductSubstituteProductId], [ProductSubstituteProducts].[ProductCode] AS [ProductSubstituteProductCode], 
		[ProductSubstituteProducts].[Description] AS [ProductSubstituteProductDescription], [ProductSubstituteProducts].[UrlImage] AS [ProductSubstituteUrlImage]
	FROM [Dim].[Products] AS [ProductSubstituteProducts]


),
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage], 
						   MIN(SubstituteReport.Type) AS Type, 
 MIN(SubstituteReport.Quantity) AS Quantity
                    FROM Fact.SubstituteReport
						 INNER JOIN  PointsOfSaleCte
															ON 
 [SubstituteReport].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
						 INNER JOIN  ProductsSubstituteCte
															ON 
 [SubstituteReport].[RelatedProductId] = [ProductsSubstituteCte].[ProductSubstituteProductId]
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage]
)
SELECT [PointOfSale], [ErpCode], [ImageUrl], [ProductSubstituteProductCode], [ProductSubstituteProductDescription], [ProductSubstituteUrlImage], 
						   Type, 
 Quantity,
 COUNT(*) OVER () AS TotalCount, 
 MIN(Type) OVER () AS Type, 
 MIN(Quantity) OVER () AS Quantity
                    FROM GroupedCte
					
					OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY