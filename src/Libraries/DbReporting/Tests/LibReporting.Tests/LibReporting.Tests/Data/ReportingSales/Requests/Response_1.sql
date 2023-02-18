
				DECLARE @StartInterval date = DateFromParts(YEAR(@StartDate), MONTH(@StartDate), 1);
				DECLARE @EndInterval date = EoMonth(@EndDate);
			WITH
CalendarCte AS 
(
SELECT [CalendarIso].[Date] AS [Date], [CalendarIso].[YearMonth] AS [YearMonth]
	FROM [Dim].[CalendarIso] AS [CalendarIso]


 WHERE Date BETWEEN @StartInterval AND @EndInterval

)
SELECT CalendarCte.YearMonth, 
	                		SalesAnalysis.SalesQuantity, SalesAnalysis.SalesAmountTaxesIncluded, SalesAnalysis.SalesAmountTaxesExcluded,
	                		SalesAnalysis.Refund, Accumulated.Transactions
	                	FROM Fact.SalesAnalysis
	                	
	                	
	                	
	                	
	                	 INNER JOIN  CalendarCte
																		ON 
 SalesAnalysis.Date = CalendarCte.Date
 AND  SalesAnalysis.Date = CalendarCte.Date