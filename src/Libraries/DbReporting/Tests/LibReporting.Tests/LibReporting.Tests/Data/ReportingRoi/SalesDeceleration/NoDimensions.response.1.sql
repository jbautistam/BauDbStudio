WITH
GroupedCte AS 
(
SELECT 			   
						   MAX(SalesDecelerations.SellingFrequency) AS SellingFrequency, 
 MAX(SalesDecelerations.Limit) AS Limit, 
 SUM(SalesDecelerations.ActualStock) AS ActualStock, 
 MAX(SalesDecelerations.LastSaleDays) AS LastSaleDays, 
 SUM(SalesDecelerations.SellThrough) AS SellThrough, 
 SUM(SalesDecelerations.PotentiallyLostSales) AS PotentiallyLostSales						   
						FROM Fact.SalesDecelerations
						
		            	
						
)
SELECT 
							
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
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY