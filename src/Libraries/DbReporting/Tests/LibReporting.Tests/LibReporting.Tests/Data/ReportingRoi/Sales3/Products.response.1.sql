WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[Description] AS [ProductDescription], [Products].[EanCode] AS [EanCode], 
		[Products].[ProductCode] AS [ProductCode], [Products].[UrlImage] AS [UrlImage], [ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue1] AS [ClassificationLevelValue1], 
		[ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue3] AS [ClassificationLevelValue3], 
		[ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue5] AS [ClassificationLevelValue5], 
		[Typologies].[Name] AS [Typology]
	FROM [Dim].[Products] AS [Products]
	INNER JOIN [Dim].[ProductClassificationLevelValuesReadingModel] AS [ProductClassificationLevelValuesReadingModel]
	ON [Products].[ProductClassificationLevelValuesReadingModelId] = [ProductClassificationLevelValuesReadingModel].[Id]
	INNER JOIN [Dim].[Typologies] AS [Typologies]
	ON [Products].[TypologyId] = [Typologies].[Id]


),
CalendarCte AS 
(
SELECT [CalendarIso].[Date] AS [Date]
	FROM [Dim].[CalendarIso] AS [CalendarIso]


 WHERE [Date] BETWEEN @StartDate AND @EndDate

)
SELECT [ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], 
	                		Sales3.SalesQuantity, Sales3.AmountTaxesExcluded, Sales3.SalesTurnover, Sales3.Refund
	                	FROM Fact.Sales3
	                	
	                	 INNER JOIN  ProductsCte
															ON 
 [Sales3].[ProductId] = [ProductsCte].[ProductId]
	                	 INNER JOIN  CalendarCte
															ON 
 [Sales3].[Date] = [CalendarCte].[Date]
						ORDER BY 1
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY