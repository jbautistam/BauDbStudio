WITH
GroupedCte AS 
(
SELECT 			   
						   SUM(DataBaseWarehouses.NegativeStock) AS NegativeStock, 
 SUM(DataBaseWarehouses.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBaseWarehouses
						
		            	
						
)
SELECT 
							
							NegativeStock, 
 NegativePendingReceptions,
 COUNT(*) OVER () AS TotalCount, 
 SUM(NegativeStock) OVER() AS TotalNegativeStock, 
 SUM(NegativePendingReceptions) OVER() AS TotalNegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 0 ROWS FETCH FIRST 100 ROWS ONLY