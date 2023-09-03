
				DECLARE @StartInterval date = DateFromParts(YEAR(@StartDate), MONTH(@StartDate), 1);
				DECLARE @EndInterval date = EoMonth(@EndDate);
			WITH
CalendarCte AS 
(
SELECT [CalendarIso].[Date] AS [Date]
	FROM [Dim].[CalendarIso] AS [CalendarIso]


 WHERE [Date] BETWEEN @StartInterval AND @EndInterval

)
SELECT 
	                		SalesAnalysis.SalesQuantity, SalesAnalysis.SalesAmountTaxesIncluded, SalesAnalysis.SalesAmountTaxesExcluded,
	                		SalesAnalysis.Refund
	                	FROM Fact.SalesAnalysis
	                	
	                	
	                	 INNER JOIN  CalendarCte
															ON 
 [SalesAnalysis].[Date] = [CalendarCte].[Date]