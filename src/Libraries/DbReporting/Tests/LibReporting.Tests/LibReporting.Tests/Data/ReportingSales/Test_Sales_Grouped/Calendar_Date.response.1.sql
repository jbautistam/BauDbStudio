
				DECLARE @StartInterval date = DateFromParts(YEAR(@StartDate), MONTH(@StartDate), 1);
				DECLARE @EndInterval date = EoMonth(@EndDate);
			WITH
CalendarCte AS 
(
SELECT [CalendarIso].[Date] AS [Date]
	FROM [Dim].[CalendarIso] AS [CalendarIso]


 WHERE [Date] BETWEEN @StartInterval AND @EndInterval

),
SalesCte AS 
(
SELECT [CalendarCte].[Date], 
	                		[SalesAnalysis].[SalesQuantity], [SalesAnalysis].[SalesAmountTaxesIncluded], [SalesAnalysis].[SalesAmountTaxesExcluded]
	                	FROM [Fact].[SalesAnalysis]
	                	
	                	
	                	 INNER JOIN  CalendarCte
																		ON 
 [SalesAnalysis].[Date] = [CalendarCte].[Date]
	                	WHERE [SalesAnalysis].[Refund] = 0
)
SELECT [Date], 
	                		SUM(SalesQuantity) AS Quantity, SUM(SalesAmountTaxesIncluded) AS AmountTaxesIncluded, 
	                		SUM(SalesAmountTaxesExcluded) AS AmountTaxesExcluded
	                	FROM SalesCte
	                	 GROUP BY [Date]