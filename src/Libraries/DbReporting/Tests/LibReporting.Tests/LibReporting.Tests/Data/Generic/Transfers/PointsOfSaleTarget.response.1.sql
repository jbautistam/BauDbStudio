WITH
PointsOfSaleTargetCte AS 
(
SELECT [PointOfSaleTargetPointsOfSale].[Id] AS [PointOfSaleTargetPointOfSaleId], [PointOfSaleTargetPointsOfSale].[Name] AS [PointOfSaleTargetPointOfSale], 
		[PointOfSaleTargetPointsOfSale].[ErpCode] AS [PointOfSaleTargetErpCode], [PointOfSaleTargetPointsOfSale].[ImageUrl] AS [PointOfSaleTargetImageUrl]
	FROM [Dim].[PointsOfSale] AS [PointOfSaleTargetPointsOfSale]


),
GroupedCte AS 
(
SELECT [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSale], [PointsOfSaleTargetCte].[PointOfSaleTargetErpCode], [PointsOfSaleTargetCte].[PointOfSaleTargetImageUrl], 
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
						
						 INNER JOIN  PointsOfSaleTargetCte
																		ON 
 [TransferReports].[DestinationPointOfSaleId] = [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSaleId]
		            	
						
						 GROUP BY [PointsOfSaleTargetCte].[PointOfSaleTargetPointOfSale], [PointsOfSaleTargetCte].[PointOfSaleTargetErpCode], [PointsOfSaleTargetCte].[PointOfSaleTargetImageUrl]
						
)
SELECT [PointOfSaleTargetPointOfSale], [PointOfSaleTargetErpCode], [PointOfSaleTargetImageUrl], 
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
 MarginPercentage,
 COUNT(*) OVER () AS TotalCount, 
 MIN(TransferType) OVER() AS TotalTransferType, 
 SUM(ActualStockOriginStore) OVER() AS TotalActualStockOriginStore, 
 SUM(IdealStockOriginStore) OVER() AS TotalIdealStockOriginStore, 
 SUM(ActualStockDestinationStore) OVER() AS TotalActualStockDestinationStore, 
 SUM(IdealStockDestinationStore) OVER() AS TotalIdealStockDestinationStore, 
 SUM(CostPVM) OVER() AS TotalCostPVM, 
 SUM(TotalTransfersOut) OTransferReportsTransfersOut, 
 SUM(DaysInStore) OVER() AS TotalDaysInStore, 
 SUM(Sales) OVER() AS TotalSales, 
 SUM(Margin) OVER() AS TotalMargin, 
 SUM(MarginPercentage) OVER() AS TotalMarginPercentage
                    FROM GroupedCte
					ORDER BY 1
					OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY