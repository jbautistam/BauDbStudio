WITH
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


)
SELECT Capacities.ClassificationLevels,
							[PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], 			   
						   SUM(Capacities.MinimumStock) AS MinimumStock, 
 SUM(Capacities.MaximumStock) AS MaximumStock, 
 SUM(Capacities.MinimumVolumetry) AS MinimumVolumetry, 
 SUM(Capacities.MaximumVolumetry) AS MaximumVolumetry, 
 SUM(Capacities.IdealStock) AS IdealStock, 
 SUM(Capacities.ExpectedStock) AS ExpectedStock, 
 MAX(Capacities.AlertMessageId) AS AlertMessageId						   
						FROM Fact.Capacities
						 INNER JOIN  PointsOfSaleCte
															ON 
 [Capacities].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], Capacities.ClassificationLevels
						ORDER BY Capacities.ClassificationLevels
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY