WITH
PointsOfSaleSourceCte AS 
(
SELECT [PointOfSaleSourcePointsOfSale].[Id] AS [PointOfSaleSourcePointOfSaleId], [PointOfSaleSourcePointsOfSale].[Name] AS [PointOfSaleSourcePointOfSale], 
		[PointOfSaleSourcePointsOfSale].[ErpCode] AS [PointOfSaleSourceErpCode], [PointOfSaleSourcePointsOfSale].[ImageUrl] AS [PointOfSaleSourceImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointOfSaleSourcePointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSale], [PointsOfSaleSourceCte].[PointOfSaleSourceErpCode], [PointsOfSaleSourceCte].[PointOfSaleSourceImageUrl], 
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
						
		            	
						
						 GROUP BY [PointsOfSaleSourceCte].[PointOfSaleSourcePointOfSale], [PointsOfSaleSourceCte].[PointOfSaleSourceErpCode], [PointsOfSaleSourceCte].[PointOfSaleSourceImageUrl]
						
)
SELECT [PointOfSaleSourcePointOfSale], [PointOfSaleSourceErpCode], [PointOfSaleSourceImageUrl], 
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