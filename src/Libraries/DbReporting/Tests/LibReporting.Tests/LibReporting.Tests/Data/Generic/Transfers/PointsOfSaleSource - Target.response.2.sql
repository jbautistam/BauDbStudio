WITH
PointsOfSaleSourceCte AS 
(
SELECT [PointOfSaleSourcePointsOfSale].[Id] AS [PointOfSaleSourcePointOfSaleId], [PointOfSaleSourcePointsOfSale].[Name] AS [PointOfSaleSourcePointOfSale], 
		[PointOfSaleSourcePointsOfSale].[ErpCode] AS [PointOfSaleSourceErpCode], [PointOfSaleSourcePointsOfSale].[ImageUrl] AS [PointOfSaleSourceImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointOfSaleSourcePointsOfSale]


),
PointsOfSaleTargetCte AS 
(
SELECT [PointOfSaleTargetPointsOfSale].[Id] AS [PointOfSaleTargetPointOfSaleId], [PointOfSaleTargetPointsOfSale].[Name] AS [PointOfSaleTargetPointOfSale], 
		[PointOfSaleTargetPointsOfSale].[ErpCode] AS [PointOfSaleTargetErpCode], [PointOfSaleTargetPointsOfSale].[ImageUrl] AS [PointOfSaleTargetImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointOfSaleTargetPointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSale], [PointsOfSaleSourceCte].[PointOfSaleSourceErpCode], [PointsOfSaleSourceCte].[PointOfSaleSourceImageUrl], [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSale], [PointsOfSaleTargetCte].[PointOfSaleTargetErpCode], [PointsOfSaleTargetCte].[PointOfSaleTargetImageUrl], 
						   MIN(TransferReports.TransferType) AS TransferType, 
 SUM(TransferReports.ActualStockOriginStore) AS ActualStockOriginStore, 
 SUM(TransferReports.IdealStockOriginStore) AS IdealStockOriginStore, 
 SUM(TransferReports.ActualStockDestinationStore) AS ActualStockDestinationStore, 
 SUM(TransferReports.IdealStockDestinationStore) AS IdealStockDestinationStore, 
 SUM(TransferReports.CostPVM) AS CostPVM, 
 SUM(TransferReports.TotalTransfersOut) AS TotalTransfersOut, 
 SUM(TransferReports.DaysInStore) AS DaysInStore, 
 SUM(TransferReports.Sales) AS Sales, 
 SUM(TransferReports.Margin) AS Margin, 
 SUM(TransferReports.MarginPercentage) AS MarginPercentage
                    FROM Fact.TransferReports
						 INNER JOIN  PointsOfSaleSourceCte
																		ON 
 [TransferReports].[OriginPointOfSaleId] = [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSaleId]
						 INNER JOIN  PointsOfSaleTargetCte
																		ON 
 [TransferReports].[DestinationPointOfSaleId] = [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSaleId]
		            	
						
						 GROUP BY [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSale], [PointsOfSaleSourceCte].[PointOfSaleSourceErpCode], [PointsOfSaleSourceCte].[PointOfSaleSourceImageUrl], [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSale], [PointsOfSaleTargetCte].[PointOfSaleTargetErpCode], [PointsOfSaleTargetCte].[PointOfSaleTargetImageUrl]
						
)
SELECT [PointOfSaleSourcePointOfSale], [PointOfSaleSourceErpCode], [PointOfSaleSourceImageUrl], [PointOfSaleTargetPointOfSale], [PointOfSaleTargetErpCode], [PointOfSaleTargetImageUrl], 
						   TransferType, 
 ActualStockOriginStore, 
 IdealStockOriginStore, 
 ActualStockDestinationStore, 
 IdealStockDestinationStore, 
 CostPVM, 
 TotalTransfersOut, 
 DaysInStore, 
 Sales, 
 Margin, 
 MarginPercentage
                    FROM GroupedCte
					ORDER BY 1
					OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY