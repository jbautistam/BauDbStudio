WITH
TypologiesCte AS 
(
SELECT [Typologies].[Id] AS [TypologyId], [Typologies].[Name] AS [Typology]
	FROM [Dim].[Typologies] AS [Typologies]


)
SELECT Capacities.ClassificationLevels,
							[TypologiesCte].[Typology], 			   
						   SUM(Capacities.MinimumStock) AS MinimumStock, 
 SUM(Capacities.MaximumStock) AS MaximumStock, 
 SUM(Capacities.MinimumVolumetry) AS MinimumVolumetry, 
 SUM(Capacities.MaximumVolumetry) AS MaximumVolumetry, 
 SUM(Capacities.IdealStock) AS IdealStock, 
 SUM(Capacities.ExpectedStock) AS ExpectedStock, 
 MAX(Capacities.AlertMessageId) AS AlertMessageId,
 COUNT(*) OVER () AS TotalCount						   
						FROM Fact.Capacities
						
		            	 INNER JOIN  TypologiesCte
															ON 
 [Capacities].[TypologyId] = [TypologiesCte].[TypologyId]
						
						 GROUP BY [TypologiesCte].[Typology], Capacities.ClassificationLevels
						ORDER BY Capacities.ClassificationLevels
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY