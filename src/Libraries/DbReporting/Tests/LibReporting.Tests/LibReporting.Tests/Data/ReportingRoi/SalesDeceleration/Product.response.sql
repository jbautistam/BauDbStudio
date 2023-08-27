WITH
ProductsCte AS 
(
SELECT [Products].[Id] AS [ProductId], [Products].[ProductCode] AS [ProductCode], [Products].[Description] AS [ProductDescription], 
		[Products].[UrlImage] AS [UrlImage]
	FROM [Dim].[Products] AS [Products]


),
GroupedCte AS 
(
SELECT [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage], 			   
						   MAX(SalesDecelerations.SellingFrequency) AS SellingFrequency, 
 MAX(SalesDecelerations.Limit) AS Limit, 
 SUM(SalesDecelerations.ActualStock) AS ActualStock, 
 MAX(SalesDecelerations.LastSaleDays) AS LastSaleDays, 
 SUM(SalesDecelerations.SellThrough) AS SellThrough, 
 SUM(SalesDecelerations.PotentiallyLostSales) AS PotentiallyLostSales						   
						FROM Fact.SalesDecelerations
						
		            	 INNER JOIN  ProductsCte
																		ON 
 [SalesDecelerations].[ProductId] = [ProductsCte].[ProductId]
						 GROUP BY [ProductsCte].[ProductCode], [ProductsCte].[ProductDescription], [ProductsCte].[UrlImage]
)
SELECT 
							[ProductCode], [ProductDescription], [UrlImage], 
							SellingFrequency, 
 Limit, 
 ActualStock, 
 LastSaleDays, 
 SellThrough, 
 PotentiallyLostSales,
 COUNT(*) OVER () AS TotalCount, 
 MAX(SellingFrequency) OVER() AS TotalSellingFrequency, 
 MAX(Limit) OVER() AS TotalLimit, 
 SUM(ActualStock) OVER() AS TotalActualStock, 
 MAX(LastSaleDays) OVER() AS TotalLastSaleDays, 
 SUM(SellThrough) OVER() AS TotalSellThrough, 
 SUM(PotentiallyLostSales) OVER() AS TotalPotentiallyLostSales
						FROM GroupedCte
						
						