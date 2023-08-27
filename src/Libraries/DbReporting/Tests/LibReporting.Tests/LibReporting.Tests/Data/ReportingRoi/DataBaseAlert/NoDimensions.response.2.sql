WITH
GroupedCte AS 
(
SELECT 			   
						   SUM(DataBasePointOfSales.NegativeStock) AS NegativeStock, 
 SUM(DataBasePointOfSales.NegativePendingReceptions) AS NegativePendingReceptions						   
						FROM Fact.DataBasePointOfSales
						
		            	
						
)
SELECT 
							
							NegativeStock, 
 NegativePendingReceptions
						FROM GroupedCte
						
						OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY