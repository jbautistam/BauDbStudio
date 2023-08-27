WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId]
	FROM [Dim].[Products] AS [Products]
	INNER JOIN [Dim].[ProductClassificationLevelValuesReadingModel] AS [ProductClassificationLevelValuesReadingModel]
	ON [Products].[ProductClassificationLevelValuesReadingModelId] = [ProductClassificationLevelValuesReadingModel].[Id]
	WHERE [ProductClassificationLevelValuesReadingModel].[ClassificationLevelValue1] = '09-TRAJE'


),
PointsOfSaleCte AS 
(
SELECT [PointsOfSale].[Id] AS [PointOfSaleId], [PointsOfSale].[ErpCode] AS [ErpCode], [PointsOfSale].[ImageUrl] AS [ImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointsOfSale]
	WHERE [PointsOfSale].[Name] = 'ALBACENTER'


)
SELECT Curves.[Week] AS [Week],
						   CAST(AVG(Curves.[Value]) AS decimal(10,2)) AS [Percentage]                            
                     FROM Fact.CurvesByPointOfSaleAndProduct INNER JOIN Fact.Curves
	                    ON CurvesByPointOfSaleAndProduct.CurveId = Curves.CurveId                                   
						 INNER JOIN  PointsOfSaleCte
																		ON 
 [CurvesByPointOfSaleAndProduct].[PointOfSaleId] = [PointsOfSaleCte].[PointOfSaleId]
		            	 INNER JOIN  ProductsCte
																		ON 
 [CurvesByPointOfSaleAndProduct].[ProductId] = [ProductsCte].[ProductId]
		            GROUP BY Curves.[Week]
		            ORDER BY Curves.[Week] ASC