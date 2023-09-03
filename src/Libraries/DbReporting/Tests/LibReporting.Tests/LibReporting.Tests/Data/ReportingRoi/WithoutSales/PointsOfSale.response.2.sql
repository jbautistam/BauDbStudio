WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId]
	FROM [Dim].[Products] AS [Products]


),
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
DailyStocksIntervalsCte AS 
(
SELECT DailyStocksIntervals.ProductId, DailyStocksIntervals.PointOfSaleId, 
						   DailyStocksIntervals.Quantity, Products.RootProductCode
						FROM [Fact].DailyStocksIntervals
						INNER JOIN Dim.Products
							ON DailyStocksIntervals.ProductId = Products.Id
						 INNER JOIN  PointsOfSaleCte
															ON 
 [DailyStocksIntervals].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
					  WHERE @EndPeriod BETWEEN DailyStocksIntervals.StartDate AND DailyStocksIntervals.EndDate
),
SalesIntervalsCte AS 
(
SELECT SalesIntervals.PointOfSaleId, SalesIntervals.ProductId, 
							Products.RootProductCode
						FROM [Fact].[SalesIntervals]
						INNER JOIN Dim.Products
							ON SalesIntervals.ProductId = Products.Id
						 INNER JOIN  PointsOfSaleCte
															ON 
 [SalesIntervals].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	
						WHERE SalesIntervals.StartDate <= @EndPeriod AND SalesIntervals.Enddate >= @StartPeriod
),
StockWithoutSalesCte AS 
(
SELECT DISTINCT DailyStocksIntervalsCte.ProductId, DailyStocksIntervalsCte.PointOfSaleId, 
							COALESCE(DailyStocksIntervalsCte.Quantity, 0) AS StockQuantity
						FROM DailyStocksIntervalsCte LEFT JOIN SalesIntervalsCte
							ON DailyStocksIntervalsCte.RootProductCode = SalesIntervalsCte.RootProductCode
								AND DailyStocksIntervalsCte.PointOfSaleId = SalesIntervalsCte.PointOfSaleId
						WHERE SalesIntervalsCte.RootProductCode IS NULL
),
WarehouseStocksCte AS 
(
SELECT ProductId, SUM(Isnull(WarehouseStock,0)) AS WarehouseStock
						FROM Fact.WarehouseStocks
						GROUP BY ProductId
),
TotalStocksCte AS 
(
SELECT PointOfSaleId, SUM(Quantity) AS LevelStockTotal,
							SUM(SUM(Quantity)) OVER() AS TotalOfLevelStockTotal
						FROM DailyStocksIntervalsCte
						GROUP BY PointOfSaleId
),
StockWithoutSalesGroupedCte AS 
(
SELECT 
						   [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], 
						   SUM(StockWithoutSalesCte.StockQuantity) AS Stock, 
 SUM(WarehouseStocksCte.WarehouseStock) AS WarehouseStock, 
 MAX(DaysInPointsOfSale.DaysInPointOfSale) AS DaysInPointOfSale, 
 SUM(TotalStocksCte.LevelStockTotal) AS TotalStockLevels, 
 CAST(100.0 * SUM(StockWithoutSalesCte.StockQuantity) / 		NULLIF(SUM(TotalStocksCte.LevelStockTotal), 0) AS decimal(10, 2) 	) AS ParticipationPercentage
					FROM StockWithoutSalesCte
		            
		             INNER JOIN  PointsOfSaleCte
															ON 
 [StockWithoutSalesCte].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
					LEFT JOIN Fact.DaysInPointsOfSale 
						ON StockWithoutSalesCte.ProductId = DaysInPointsOfSale.ProductId 
							AND StockWithoutSalesCte.PointOfSaleId = DaysInPointsOfSale.PointOfSaleId
					LEFT JOIN WarehouseStocksCte 
						ON StockWithoutSalesCte.ProductId = WarehouseStocksCte.ProductId
					LEFT JOIN TotalStocksCte
						ON StockWithoutSalesCte.PointOfSaleId = TotalStocksCte.PointOfSaleId
					 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl]
)
SELECT 
							[PointOfSale], [ErpCode], [ImageUrl], 
							Stock, 
 WarehouseStock, 
 DaysInPointOfSale, 
 TotalStockLevels, 
 ParticipationPercentage
						FROM StockWithoutSalesGroupedCte
						ORDER BY 1
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY