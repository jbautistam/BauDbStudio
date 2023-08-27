WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId]
	FROM [Dim].[Products] AS [Products]


),
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]


),
CalendarCte AS 
(
SELECT [CalendarIso].[Date] AS [Date]
	FROM [Dim].[CalendarIso] AS [CalendarIso]


 WHERE [Date] BETWEEN @StartDate AND @EndDate

),
DailyCte AS 
(
SELECT Date, SUM(Stock) AS Stock, SUM(Sales) AS Sales, SUM(StockOut) AS StockOut
	                    FROM (SELECT CalendarCte.Date, SUM(Stocks.Stock) AS Stock, 0 AS Sales, 0 AS StockOut
			                    FROM CalendarCte 
			                    INNER JOIN (SELECT StockIntervals.StartDate, StockIntervals.EndDate, 
			                    					SUM(StockIntervals.Quantity) AS Stock
						                      FROM Fact.DailyStocksIntervals StockIntervals
						                      INNER JOIN PointsOfSaleCte
							                    ON StockIntervals.PointOfSaleId = PointsOfSaleCte.PointOfSaleId
						                      INNER JOIN ProductsCTE
							                    ON StockIntervals.ProductId = ProductsCTE.ProductId
						                      WHERE StockIntervals.StartDate BETWEEN @StartDate AND @EndDate
							                    OR StockIntervals.EndDate BETWEEN @StartDate AND @EndDate
							                    OR @StartDate BETWEEN StockIntervals.StartDate AND StockIntervals.EndDate
							                    OR @EndDate BETWEEN StockIntervals.StartDate AND StockIntervals.EndDate
							                  GROUP BY StockIntervals.StartDate, StockIntervals.EndDate
						                    ) AS Stocks
				                    ON CalendarCte.Date BETWEEN Stocks.StartDate AND Stocks.EndDate
			                    GROUP BY CalendarCte.Date
		                     UNION ALL
		                     SELECT Sales.[Date], 0 AS Stock, SUM(Sales.SalesQuantity) AS Sales, 0 AS StockOut
		                       FROM Fact.Sales3 Sales INNER JOIN PointsOfSaleCte
				                    ON Sales.PointOfSaleId = PointsOfSaleCte.PointOfSaleId
		                       INNER JOIN ProductsCTE
				                    ON Sales.ProductId = ProductsCTE.ProductId
			                    WHERE Sales.[Date] BETWEEN @StartDate AND @EndDate
			                    GROUP BY Sales.[Date]
		                    UNION ALL
		                    SELECT LostSales.[Date], 0 AS Stock, 0 AS Sales, SUM(LostSales.Units) AS Stockout
		                       FROM Fact.LostSales
		                       INNER JOIN PointsOfSaleCte
				                    ON LostSales.PointOfSaleId = PointsOfSaleCte.PointOfSaleId
		                       INNER JOIN ProductsCTE
				                    ON LostSales.ProductId = ProductsCTE.ProductId
		                       WHERE LostSales.[Date] BETWEEN @StartDate AND @EndDate
		                       GROUP BY LostSales.[Date]
		                    ) AS tmpTotals
	                    GROUP BY Date
),
GroupedCte AS 
(
SELECT CalendarCte.[Date], COALESCE(DailyCte.Stock, 0) AS Stock,
	                       COALESCE(DailyCte.Sales, 0) + COALESCE(DailyCte.Stockout, 0) AS PotentialSales,
	                       COALESCE(DailyCte.Sales, 0) AS Sales, COALESCE(DailyCte.Stockout, 0) AS Stockout
	                    FROM CalendarCte LEFT JOIN DailyCte 
	                    	ON CalendarCte.[Date] = DailyCte.[Date]
)
SELECT [Date], 
						COUNT(*) OVER () AS TotalCount, 
						   [Stock], [PotentialSales], [Sales], [Stockout]
						FROM GroupedCte
						ORDER BY [Date] ASC
						