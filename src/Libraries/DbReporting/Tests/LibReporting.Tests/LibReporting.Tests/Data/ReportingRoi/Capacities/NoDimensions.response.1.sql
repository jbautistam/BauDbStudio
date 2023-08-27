SELECT Capacities.ClassificationLevels,
										   
						   SUM(Capacities.MinimumStock) AS MinimumStock, 
 SUM(Capacities.MaximumStock) AS MaximumStock, 
 SUM(Capacities.MinimumVolumetry) AS MinimumVolumetry, 
 SUM(Capacities.MaximumVolumetry) AS MaximumVolumetry, 
 SUM(Capacities.IdealStock) AS IdealStock, 
 SUM(Capacities.ExpectedStock) AS ExpectedStock, 
 MAX(Capacities.AlertMessageId) AS AlertMessageId,
 COUNT(*) OVER () AS TotalCount						   
						FROM Fact.Capacities
						
		            	
						 GROUP BY Capacities.ClassificationLevels
						ORDER BY Capacities.ClassificationLevels
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY