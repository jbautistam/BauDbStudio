WITH
WarehousesCte AS 
(
SELECT [Warehouses].[Id] AS [WarehouseId], [Warehouses].[ErpCode] AS [ErpCode], [Warehouses].[Name] AS [WarehouseName]
	FROM [Dim].[Warehouses] AS [Warehouses]


),
GroupedCte AS 
(
SELECT [WarehousesCte].[ErpCode], [WarehousesCte].[WarehouseName], 			   
						   SUM(DataBaseWarehouses.NegativeStock) AS NegativeStock, 
 SUM(DataBaseWarehouses.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBaseWarehouses
						 INNER JOIN  WarehousesCte
															ON 
 [DataBaseWarehouses].[WarehouseId] = [WarehousesCte].[WarehouseId]
		            	
						 GROUP BY [WarehousesCte].[ErpCode], [WarehousesCte].[WarehouseName]
)
SELECT 
							[ErpCode], [WarehouseName], 
							NegativeStock, 
 NegativePendingReceptions,
 COUNT(*) OVER () AS TotalCount, 
 SUM(NegativeStock) OVER() AS TotalNegativeStock, 
 SUM(NegativePendingReceptions) OVER() AS TotalNegativePendingReceptions
						FROM GroupedCte
						
						