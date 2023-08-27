WITH
CalendarCte AS 
(
SELECT [CalendarIso].[Date] AS [Date]
	FROM [Dim].[CalendarIso] AS [CalendarIso]


 WHERE [Date] BETWEEN @StartDate AND @EndDate

)
SELECT [CalendarCte].[Date], 
	                		Sales3.SalesQuantity, Sales3.AmountTaxesExcluded, Sales3.SalesTurnover, Sales3.Refund
	                	FROM Fact.Sales3
	                	
	                	
	                	 INNER JOIN  CalendarCte
																		ON 
 [Sales3].[Date] = [CalendarCte].[Date]
						ORDER BY 1
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY