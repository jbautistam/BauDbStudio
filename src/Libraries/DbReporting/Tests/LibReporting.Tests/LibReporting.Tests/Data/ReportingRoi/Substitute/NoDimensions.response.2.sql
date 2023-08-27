WITH
GroupedCte AS 
(
SELECT 
						   MIN(SubstituteReport.Type) AS Type, 
 MIN(SubstituteReport.Quantity) AS Quantity
                    FROM Fact.SubstituteReport
						
		            	
						
						
)
SELECT 
						   Type, 
 Quantity
                    FROM GroupedCte
					
					OFFSET 100 ROWS FETCH FIRST 100 ROWS ONLY