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
						   SUM(DataBasePointOfSales.NegativeStock) AS NegativeStock, 
 SUM(DataBasePointOfSales.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBasePointOfSales
						 INNER JOIN  PointsOfSaleCte
																		ON 
 [DataBasePointOfSales].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	 INNER JOIN  ProductsCte
																		ON 
 [DataBasePointOfSales].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[PointOfSale], [ErpCode], [ImageUrl], [ProductCode], [ProductDescription], [UrlImage], 
							NegativeStock, 
 NegativePendingReceptions,
 COUNT(*) OVER () AS TotalCount, 
 SUM(NegativeStock) OVER() AS TotalNegativeStock, 
 SUM(NegativePendingReceptions) OVER() AS TotalNegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY