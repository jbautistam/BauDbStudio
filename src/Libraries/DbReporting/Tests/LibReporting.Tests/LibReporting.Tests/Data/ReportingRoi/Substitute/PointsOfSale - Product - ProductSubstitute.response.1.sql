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
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage], 
						   MIN(SubstituteReport.Type) AS Type, 
 MIN(SubstituteReport.Quantity) AS Quantity
                    FROM Fact.SubstituteReport
						 INNER JOIN  PointsOfSaleCte
																		ON 
 [SubstituteReport].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	 INNER JOIN  ProductsCte
																		ON 
 [SubstituteReport].[ProductId] = [ProductsCte].[ProductId]
						 INNER JOIN  ProductsSubstituteCte
																		ON 
 [SubstituteReport].[RelatedProductId] = [ProductsSubstituteCte].[ProductSubstituteProductId]
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsSubstituteCte].[ProductSubstituteProductCode], [ProductsSubstituteCte].[ProductSubstituteProductDescription], [ProductsSubstituteCte].[ProductSubstituteUrlImage]
)
SELECT [PointOfSale], [ErpCode], [ImageUrl], [ProductCode], [ProductDescription], [UrlImage], [ProductSubstituteProductCode], [ProductSubstituteProductDescription], [ProductSubstituteUrlImage], 
						   Type, 
 Quantity,
 COUNT(*) OVER () AS TotalCount, 
 MIN(Type) OVER () AS Type, 
 MIN(Quantity) OVER () AS Quantity
                    FROM GroupedCte
					
					OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY