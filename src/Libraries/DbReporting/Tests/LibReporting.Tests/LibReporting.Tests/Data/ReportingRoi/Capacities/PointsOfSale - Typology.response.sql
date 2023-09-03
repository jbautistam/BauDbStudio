WITH
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
TypologiesCte AS 
(
SELECT [Typologies].[Id] AS [TypologyId], [Typologies].[Name] AS [Typology]
	FROM [Dim].[Typologies] AS [Typologies]


)
SELECT Capacities.ClassificationLevels,
							[PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [TypologiesCte].[Typology], 			   
						   SUM(Capacities.MinimumStock) AS MinimumStock, 
 SUM(Capacities.MaximumStock) AS MaximumStock, 
 SUM(Capacities.MinimumVolumetry) AS MinimumVolumetry, 
 SUM(Capacities.MaximumVolumetry) AS MaximumVolumetry, 
 SUM(Capacities.IdealStock) AS IdealStock, 
 SUM(Capacities.ExpectedStock) AS ExpectedStock, 
 MAX(Capacities.AlertMessageId) AS AlertMessageId,
 COUNT(*) OVER () AS TotalCount						   
						FROM Fact.Capacities
						 INNER JOIN  PointsOfSaleCte
															ON 
 [Capacities].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	 INNER JOIN  TypologiesCte
															ON 
 [Capacities].[TypologyId] = [TypologiesCte].[TypologyId]
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [TypologiesCte].[Typology], Capacities.ClassificationLevels
						ORDER BY Capacities.ClassificationLevels
						