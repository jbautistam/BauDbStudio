SELECT Curves.[Week] AS [Week],
						   CAST(AVG(Curves.[Value]) AS decimal(10,2)) AS [Percentage]                            
                     FROM Fact.CurvesByPointOfSaleAndProduct INNER JOIN Fact.Curves
	                    ON CurvesByPointOfSaleAndProduct.CurveId = Curves.CurveId                                   
						
		            	
		            GROUP BY Curves.[Week]
		            ORDER BY Curves.[Week] ASC