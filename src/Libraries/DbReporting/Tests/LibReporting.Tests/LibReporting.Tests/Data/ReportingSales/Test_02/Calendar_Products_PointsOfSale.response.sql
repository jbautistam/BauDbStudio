DECLARE @StartInterval date = DateFromParts(YEAR(@StartDate), MONTH(@StartDate), 1);
DECLARE @EndInterval date = EoMonth(@EndDate);

WITH ProductsCte AS 
		(SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], 
				[Products].[RootProductCode] AS [RootProductCode], [Products].[Description] AS [Description], 
				[Products].[UrlImage] AS [UrlImage], 
				[ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue1] AS [ClassificationLevelValue1], 
				[ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue2] AS [ClassificationLevelValue2], 
				[Typologies].[Name] AS [Typology] 
			FROM [Dim].[Products] AS [Products] 
			INNER JOIN [Dim].[ProductClassificationLevelValuesReadingModel] AS [ProductClassificationLevelValuesReadingModel] 
				ON [Products].[ProductClassificationLevelValuesReadingModelId] = [ProductClassificationLevelValuesReadingModel].[Id] 
			INNER JOIN [Dim].[Typologies] AS [Typologies] ON [Products].[TypologyId] = [Typologies].[Id]
		), 
	PointsOfSaleCte AS 
		(SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[ErpCode] AS [ErpCode], [PointsOfSale].[Name] AS [PointOfSale], 
				[PointsOfSale].[ImageUrl] AS [ImageUrl], [Channels].[Name] AS [ChannelName], [PointOfSaleTypes].[Name] AS [PointOfSaleType] 
			FROM [Dim].[PointsOfSale] AS [PointsOfSale] 
			INNER JOIN [Dim].[Channels] AS [Channels] 
				ON [PointsOfSale].[ChannelId] = [Channels].[Id] 
			INNER JOIN [Dim].[PointOfSaleTypes] AS [PointOfSaleTypes] 
				ON [PointsOfSale].[PointOfSaleTypeId] = [PointOfSaleTypes].[Id]
		), 
	CalendarCte AS 
		(SELECT [CalendarIso].[Date] AS [Date] 
			FROM [Dim].[CalendarIso] AS [CalendarIso] 
			WHERE [Date] BETWEEN @StartInterval AND @EndInterval
		) 
	SELECT [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ImageUrl], 
			[PointsOfSaleCte].[ChannelName], [PointsOfSaleCte].[PointOfSaleType], [ProductsCte].[ProductCode], 
			[ProductsCte].[RootProductCode], [ProductsCte].[Description], [ProductsCte].[UrlImage], 
			[ProductsCte].[ClassificationLevelValue1], [ProductsCte].[ClassificationLevelValue2], 
			[ProductsCte].[Typology], SalesAnalysis.SalesQuantity, SalesAnalysis.SalesAmountTaxesIncluded, 
			SalesAnalysis.SalesAmountTaxesExcluded, SalesAnalysis.Refund 
		FROM Fact.SalesAnalysis INNER JOIN PointsOfSaleCte 
			ON [SalesAnalysis].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		INNER JOIN ProductsCte 
			ON [SalesAnalysis].[ProductId] = [ProductsCte].[ProductId]
		INNER JOIN CalendarCte 
			ON [SalesAnalysis].[Date] = [CalendarCte].[Date]
