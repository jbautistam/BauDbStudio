WITH
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], 
						   SUM(DataBasePointOfSales.NegativeStock) AS NegativeStock, 
 SUM(DataBasePointOfSales.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBasePointOfSales
						 INNER JOIN  PointsOfSaleCte
															ON 
 [DataBasePointOfSales].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl]
)
SELECT 
							[PointOfSale], [ErpCode], [ImageUrl], 
							NegativeStock, 
 NegativePendingReceptions,
 COUNT(*) OVER () AS TotalCount, 
 SUM(NegativeStock) OVER() AS TotalNegativeStock, 
 SUM(NegativePendingReceptions) OVER() AS TotalNegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY