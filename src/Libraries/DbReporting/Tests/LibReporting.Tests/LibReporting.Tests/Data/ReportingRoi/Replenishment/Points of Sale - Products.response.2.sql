WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[EanCode] AS [EanCode], [Products].[ProductCode] AS [ProductCode], 
		[Products].[Description] AS [ProductDescription], [Products].[UrlImage] AS [UrlImage], [ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue4] AS [ClassificationLevelValue4]
	FROM [Dim].[Products] AS [Products]
	INNER JOIN [Dim].[ProductClassificationLevelValuesReadingModel] AS [ProductClassificationLevelValuesReadingModel]
	ON [Products].[ProductClassificationLevelValuesReadingModelId] = [ProductClassificationLevelValuesReadingModel].[Id]


),
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[Name] AS [PointOfSale], [PointsOfSale].[ErpCode] AS [ErpCode], 
		[PointsOfSale].[ImageUrl] AS [ImageUrl], [Channels].[Name] AS [ChannelName]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]
	INNER JOIN [Dim].[Channels] AS [Channels]
	ON [PointsOfSale].[ChannelId] = [Channels].[Id]


),
GroupedCte AS 
(
SELECT [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue4], 
						   MIN(ReplenishmentReport.MinimumMultiple) AS MinimumMultiple, 
 SUM(ReplenishmentReport.OriginalOrder) AS OriginalOrder, 
 SUM(ReplenishmentReport.OriginalOrderCost) AS OriginalOrderCost, 
 SUM(ReplenishmentReport.Orders) AS Orders, 
 SUM(ReplenishmentReport.OrderCost) AS OrderCost, 
 SUM(ReplenishmentReport.ImminentRupture) AS ImminentRupture, 
 SUM(ReplenishmentReport.SalesTwoMonths) AS SalesTwoMonths, 
 SUM(ReplenishmentReport.SalesSixMonths) AS SalesSixMonths, 
 SUM(ReplenishmentReport.SalesProjection) AS SalesProjection, 
 SUM(ReplenishmentReport.ActualStock) AS ActualStock, 
 /* SUM(CASE WHEN ROW_NUMBER() OVER (PARTITION BY ReplenishmentReport.ProductId ORDER BY ReplenishmentReport.ProductId) = 1 			THEN ReplenishmentReport.WarehouseStock 			ELSE 0 		END) AS WarehouseStock */ 0 AS WarehouseStock, 
 SUM(ReplenishmentReport.PendingReceptions) AS PendingReceptions, 
 SUM(ReplenishmentReport.IdealWindow) AS IdealWindow, 
 SUM(ReplenishmentReport.RealWindow) AS RealWindow, 
 SUM(ReplenishmentReport.AvailableStock) AS AvailableStock, 
 SUM(ReplenishmentReport.StockCost)  AS StockCost, 
 SUM(ReplenishmentReport.Overstock) AS Overstock, 
 SUM(ReplenishmentReport.MissingStock) AS MissingStock, 
 SUM(ReplenishmentReport.TransferIn) AS TransferIn, 
 SUM(ReplenishmentReport.TransferOut) AS TransferOut, 
 SUM(ReplenishmentReport.Returns) AS Returns, 
 MIN(ReplenishmentReport.PromotionValue) AS PromotionValue, 
 SUM(CAST(ReplenishmentReport.ForcedWindows AS bigint)) AS ForcedWindows, 
 SUM(ReplenishmentReport.ForcedProductive) AS ForcedProductive, 
 MAX(ReplenishmentReport.LastDayOfSale) AS LastDayOfSale, 
 SUM(ReplenishmentReport.LastMonthSales) AS LastMonthSales, 
 SUM(ReplenishmentReport.SalesTwoMonthsOutlier) AS SalesTwoMonthsOutlier, 
 SUM(ReplenishmentReport.SalesSixMonthsOutlier) AS SalesSixMonthsOutlier, 
 SUM(ReplenishmentReport.RuptureTwoMonths) AS RuptureTwoMonths, 
 SUM(ReplenishmentReport.RuptureSixMonths) AS RuptureSixMonths, 
 MAX(CASE WHEN ReplenishmentReport.GlobalBlocks = 1 THEN 1 ELSE 0 END) AS GlobalBlocks, 
 SUM(ReplenishmentReport.SalesIndirect2Months) AS SalesIndirect2Months, 
 SUM(ReplenishmentReport.SalesIndirect6Months) AS SalesIndirect6Months, 
 SUM(ReplenishmentReport.OmnichannelIdealStock) AS OmnichannelIdealStock
                    FROM Fact.ReplenishmentReport
						 INNER JOIN  PointsOfSaleCte
															ON 
 [ReplenishmentReport].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	 INNER JOIN  ProductsCte
															ON 
 [ReplenishmentReport].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [PointsOfSaleCte].[PointOfSale], [PointsOfSaleCte].[ErpCode], [PointsOfSaleCte].[ImageUrl], [PointsOfSaleCte].[ChannelName], [ProductsCte].[EanCode], [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue4]
)
SELECT [PointOfSale], [ErpCode], [ImageUrl], [ChannelName], [EanCode], [ProductCode], [ProductDescription], [UrlImage], [ClassificationLevelValue4], 
						   MinimumMultiple, 
 OriginalOrder, 
 OriginalOrderCost, 
 Orders, 
 OrderCost, 
 ImminentRupture, 
 SalesTwoMonths, 
 SalesSixMonths, 
 SalesProjection, 
 ActualStock, 
 WarehouseStock, 
 PendingReceptions, 
 IdealWindow, 
 RealWindow, 
 AvailableStock, 
 StockCost, 
 Overstock, 
 MissingStock, 
 TransferIn, 
 TransferOut, 
 Returns, 
 PromotionValue, 
 ForcedWindows, 
 ForcedProductive, 
 LastDayOfSale, 
 LastMonthSales, 
 SalesTwoMonthsOutlier, 
 SalesSixMonthsOutlier, 
 RuptureTwoMonths, 
 RuptureSixMonths, 
 GlobalBlocks, 
 SalesIndirect2Months, 
 SalesIndirect6Months, 
 OmnichannelIdealStock
                    FROM GroupedCte
					
					OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY