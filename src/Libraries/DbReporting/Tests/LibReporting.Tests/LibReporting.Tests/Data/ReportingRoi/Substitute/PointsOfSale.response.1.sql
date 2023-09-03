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
						   MIN(SubstituteReport.Type) AS Type, 
 MIN(SubstituteReport.Quantity) AS Quantity
                    FROM Fact.SubstituteReport
						 INNER JOIN  PointsOfSaleCte
															ON 
 [SubstituteReport].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
						
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl]
)
SELECT [PointOfSale], [ErpCode], [ImageUrl], 
						   Type, 
 Quantity,
 COUNT(*) OVER () AS TotalCount, 
 MIN(Type) OVER () AS Type, 
 MIN(Quantity) OVER () AS Quantity
                    FROM GroupedCte
					
					OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY