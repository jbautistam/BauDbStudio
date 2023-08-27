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
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[City] AS [City], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ImageUrl] AS [ImageUrl], [Channels].[Name] AS [ChannelName], 
		[PointOfSaleClassificationLevelValuesReadingModel].[ClassificationLevelValue1] AS [PointOfSaleClassificationLevel1], 
		[PointOfSaleClassificationLevelValuesReadingModel].[ClassificationLevelValue2] AS [PointOfSaleClassificationLevel2]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]
	INNER JOIN [Dim].[Channels] AS [Channels]
	ON [PointsOfSale].[ChannelId] = [Channels].[Id]
	INNER JOIN [Dim].[PointOfSaleClassificationLevelValuesReadingModel] AS [PointOfSaleClassificationLevelValuesReadingModel]
	ON [PointsOfSale].[PointOfSaleClassificationLevelValuesReadingModelId] = [PointOfSaleClassificationLevelValuesReadingModel].[Id]


),
CalendarCte AS 
(
SELECT [CalendarIso].[Date] AS [Date]
	FROM [Dim].[CalendarIso] AS [CalendarIso]


 WHERE [Date] BETWEEN @StartDate AND @EndDate

)
SELECT [PointsOfSaleCte].[City], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleClassificationLevel1], [PointsOfSaleCte].[PointOfSaleClassificationLevel2], [ProductsCte].[ProductDescription], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue3], [ProductsCte].[ClassificationLevelValue5], [ProductsCte].[Typology], [CalendarCte].[Date], 
	                		Sales3.SalesQuantity, Sales3.AmountTaxesExcluded, Sales3.SalesTurnover, Sales3.Refund
	                	FROM Fact.Sales3
	                	 INNER JOIN  PointsOfSaleCte
																		ON 
 [Sales3].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
	                	 INNER JOIN  ProductsCte
																		ON 
 [Sales3].[ProductId] = [ProductsCte].[ProductId]
	                	 INNER JOIN  CalendarCte
																		ON 
 [Sales3].[Date] = [CalendarCte].[Date]
						ORDER BY 1
						