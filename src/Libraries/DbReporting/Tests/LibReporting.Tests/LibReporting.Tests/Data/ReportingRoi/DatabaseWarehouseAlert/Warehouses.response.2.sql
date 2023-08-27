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
 NegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY