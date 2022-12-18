<?xml version='1.0' encoding='utf-8'?>
<ReportRequest Id = "Sales analysis date with transactions" >
	<Parameter Key = "StartDate"  Type = "String"  Value = "" />
	<Parameter Key = "EndDate"  Type = "String"  Value = "" />
	<Dimension Id = "Calendar" >
		<Column   Id = "NaturalYear"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		<Column   Id = "WeekDay"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
	</Dimension>
	<Dimension Id = "PointsOfSale" >
		<Column   Id = "Email"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" >
			<Where Condition = "Contains" >
				<Parameter Key = "Value"  Type = "String"  Value = "@" />
			</Where>
			<Having Condition = "Contains" >
				<Parameter Key = "Value"  Type = "String"  Value = "@" />
			</Having>
		</Column>
		<Column   Id = "ErpCode"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		<Dimension Id = "Channels" >
			<Column   Id = "Description"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" >
				<Where Condition = "Contains" >
					<Parameter Key = "Value"  Type = "String"  Value = "a" />
				</Where>
				<Having Condition = "Contains" >
					<Parameter Key = "Value"  Type = "String"  Value = "a" />
				</Having>
			</Column>
		</Dimension>
		<Dimension Id = "PointOfSaleTypes" >
			<Column   Id = "Name"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		</Dimension>
	</Dimension>
	<Dimension Id = "Products" >
		<Column   Id = "DiscontinuedProduct"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		<Dimension Id = "ProductClassifications" >
			<Column   Id = "ClassificationLevelValue2"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
			<Column   Id = "ClassificationLevelValue3"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		</Dimension>
		<Dimension Id = "Typologies" >
			<Column   Id = "Name"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		</Dimension>
	</Dimension>
	<Dimension Id = "Users" >
		<Column   Id = "FirstName"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		<Column   Id = "ImageUrl"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		<Column   Id = "LastName"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" >
			<Where Condition = "Contains" >
				<Parameter Key = "Value"  Type = "String"  Value = "b" />
			</Where>
			<Where Condition = "Contains" >
				<Parameter Key = "Value"  Type = "String"  Value = "c" />
			</Where>
			<Having Condition = "Contains" >
				<Parameter Key = "Value"  Type = "String"  Value = "b" />
			</Having>
			<Having Condition = "Contains" >
				<Parameter Key = "Value"  Type = "String"  Value = "c" />
			</Having>
		</Column>
		<Dimension Id = "UserSalesRoles" >
			<Column   Id = "Label"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		</Dimension>
		<Dimension Id = "UserSalesRolLevels" >
			<Column   Id = "Label"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
		</Dimension>
	</Dimension>
	<Dimension Id = "UsersPointOfSale" >
		<Column   Id = "UserLevel"  Visible = "yes"  OrderIndex = "-1"  OrderBy = "Undefined" />
	</Dimension>
	<Expression  >
		<Column Id = "SalesTaxesIncluded"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "SalesTaxesIncludedAccumulated"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "SalesTaxesExcluded"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "Sales"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "SalesTargetTaxesIncluded"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "SalesTargetTaxesIncludedAccumulated"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "SalesAchievementPercentage"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "SalesAchievementPercentageAccumulated"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "MonthlyTargetAchievement"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "MonthlySalesProjection"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "SalesPerCalendarHour"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "WeeklyEvolutionPercentage"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "Transactions"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "TransactionsPerCalendarHour"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "AverageBasket"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "AverageBasketAccumulated"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "AveragePrice"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "AveragePriceAccumulated"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "UPT"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "UptAccumulated"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "ExternalCustomerFlow"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "InternalCustomerFlow"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "ExternalConversionRate"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
	<Expression  >
		<Column Id = "InternalConversionRate"  AggregatedBy = "NoAggregated"  Visible = "yes"  OrderIndex = "0"  OrderBy = "Undefined" />
	</Expression>
</ReportRequest>
