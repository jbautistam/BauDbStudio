WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[Description] AS [ProductDescription], 
		[Products].[UrlImage] AS [UrlImage], [ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue1] AS [ClassificationLevelValue1], 
		[Typologies].[Name] AS [Typology]
	FROM [Dim].[Products] AS [Products]
	INNER JOIN [Dim].[ProductClassificationLevelValuesReadingModel] AS [ProductClassificationLevelValuesReadingModel]
	ON [Products].[ProductClassificationLevelValuesReadingModelId] = [ProductClassificationLevelValuesReadingModel].[Id]
	INNER JOIN [Dim].[Typologies] AS [Typologies]
	ON [Products].[TypologyId] = [Typologies].[Id]


),
GroupedCte AS 
(
SELECT [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[Typology], 			   
						   SUM(Overstock.ProjectionVariationPercentage) AS ProjectionVariationPercentage, 
 SUM(Overstock.ActualStock) AS ActualStock, 
 SUM(Overstock.WarehouseStock) AS WarehouseStock, 
 SUM(Overstock.Overstock12) AS Overstock12, 
 SUM(Overstock.OverstockCost12) AS OverstockCost12, 
 SUM(Overstock.Overstock28) AS Overstock28, 
 SUM(Overstock.OverstockCost28) AS OverstockCost28, 
 SUM(Overstock.Overstock53) AS Overstock53, 
 SUM(Overstock.OverstockCost53) AS OverstockCost53, 
 SUM(Overstock.PointOfSaleImminentPendingReceptions) AS PointOfSaleImminentPendingReceptions, 
 SUM(Overstock.PointOfSalePendingReceptions) AS PointOfSalePendingReceptions, 
 SUM(Overstock.WarehouseImminentPendingReceptions) AS WarehouseImminentPendingReceptions, 
 SUM(Overstock.WarehousePendingReceptions) AS WarehousePendingReceptions, 
 SUM(Overstock.PointsOfSaleWithStock) AS PointsOfSaleWithStock, 
 SUM(Overstock.PointsOfSaleInAssortment) AS PointsOfSaleInAssortment, 
 SUM(Overstock.PointsOfSaleBlocked) AS PointsOfSaleBlocked, 
 SUM(Overstock.OriginalOrder) AS OriginalOrder, 
 SUM(Overstock.Sales4WeeksAgo) AS Sales4WeeksAgo, 
 SUM(Overstock.Sales3WeeksAgo) AS Sales3WeeksAgo, 
 SUM(Overstock.Sales2WeeksAgo) AS Sales2WeeksAgo, 
 SUM(Overstock.Sales1WeekAgo) AS Sales1WeekAgo, 
 SUM(Overstock.Stock4WeeksAgo) AS Stock4WeeksAgo, 
 SUM(Overstock.Stock3WeeksAgo) AS Stock3WeeksAgo, 
 SUM(Overstock.Stock2WeeksAgo) AS Stock2WeeksAgo, 
 SUM(Overstock.Stock1WeekAgo) AS Stock1WeekAgo						   
						FROM Fact.Overstock
		            	 INNER JOIN  ProductsCte
																		ON 
 [Overstock].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], [ProductsCte].[ClassificationLevelValue1], [ProductsCte].[Typology]
)
SELECT [ProductCode], [ProductDescription], [UrlImage], [ClassificationLevelValue1], [Typology], 
							ProjectionVariationPercentage, 
 ActualStock, 
 WarehouseStock, 
 Overstock12, 
 OverstockCost12, 
 Overstock28, 
 OverstockCost28, 
 Overstock53, 
 OverstockCost53, 
 PointOfSaleImminentPendingReceptions, 
 PointOfSalePendingReceptions, 
 WarehouseImminentPendingReceptions, 
 WarehousePendingReceptions, 
 PointsOfSaleWithStock, 
 PointsOfSaleInAssortment, 
 PointsOfSaleBlocked, 
 OriginalOrder, 
 Sales4WeeksAgo, 
 Sales3WeeksAgo, 
 Sales2WeeksAgo, 
 Sales1WeekAgo, 
 Stock4WeeksAgo, 
 Stock3WeeksAgo, 
 Stock2WeeksAgo, 
 Stock1WeekAgo
						FROM GroupedCte
						
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY