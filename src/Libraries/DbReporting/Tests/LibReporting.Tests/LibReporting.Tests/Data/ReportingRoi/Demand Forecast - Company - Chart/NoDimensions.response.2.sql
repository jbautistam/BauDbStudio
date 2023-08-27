SELECT CompanyDemandForecast.[Week] AS [Week],   
						   SUM(CompanyDemandForecast.AccumulatedReceptions + CompanyDemandForecast.Receptions + CompanyDemandForecast.SimulatedReceptions + CompanyDemandForecast.Stock) AS TotalStock, 
 SUM(CompanyDemandForecast.Stock) AS Stock, 
 SUM(CompanyDemandForecast.AccumulatedReceptions) AS AccumulatedReceptions, 
 SUM(CompanyDemandForecast.Receptions) AS Receptions, 
 SUM(CompanyDemandForecast.Sales) AS Sales, 
 SUM(CompanyDemandForecast.Breakage) AS Breakage, 
 SUM(CompanyDemandForecast.AccumulatedSales) AS AccumulatedSales, 
 SUM(CompanyDemandForecast.AccumulatedOrders) AS AccumulatedOrders						   
						FROM Fact.CompanyDemandForecast
		            	
		            	GROUP BY CompanyDemandForecast.[Week]
		            	ORDER BY CompanyDemandForecast.[Week]