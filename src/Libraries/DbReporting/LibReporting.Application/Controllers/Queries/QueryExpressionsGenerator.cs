using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries;

/// <summary>
///		Generador de consultas para las expresiones
/// </summary>
internal class QueryExpressionsGenerator : QueryBaseGenerator
{
	internal QueryExpressionsGenerator(ReportQueryGenerator generator) : base(generator) {}

	/// <summary>
	///		Obtiene la consulta de las expresiones
	/// </summary>
	internal QueryModel GetQuery(List<QueryModel> dimensionQueries)
	{
		if (Generator.Report is ReportModel report)
			return GetQuery(report, dimensionQueries);
		else
			throw new NotImplementedException($"Can't get expressions report {Generator.Report.Id}. Type: {Generator.Report.GetType().ToString()}");
	}

	/// <summary>
	///		Obtiene la consulta de las expresiones
	/// </summary>
	internal QueryModel GetQuery(ReportModel report, List<QueryModel> dimensionQueries)
	{
		//TODO En realidad, un informe puede tener varios orígenes de datos, cuál de los orígenes de datos se escoge, depende de ciertas condiciones como las dimensiones
		//TODO escogidas en la solicitud, para las primeras pruebas, cogemos directamente la primera solicitud
		ReportDataSourceModel reportDataSource = report.ReportDataSources[0];
		BaseDataSourceModel baseDataSource = reportDataSource.DataSource;
		QueryModel query = new QueryModel(Generator, baseDataSource.Id, QueryModel.QueryType.Expressions, GetDataSourceAlias(baseDataSource));

			// Prepara la consulta
			query.Prepare(baseDataSource);
			// Añade los campos que vienen de otras dimensiones
			ComputeDimensionFields(query, dimensionQueries);
			// Obtiene los campos por los que vamos a agrupar y los añade a la colección de claves de dimensión de la consulta
			ComputeFieldsGroupBy(query, reportDataSource, dimensionQueries);
			// Obtiene los campos agrupados
			ComputeFieldsGrouped(query, Generator.Request, baseDataSource);
			// Devuelve la consulta
			return query;
	}

	/// <summary>
	///		Obtiene los campos que se deben añadir de las consultas de dimensión
	/// </summary>
	private void ComputeDimensionFields(QueryModel query, List<QueryModel> dimensionQueries)
	{
		foreach (QueryModel dimensionQuery in dimensionQueries)
			ComputeDimensionFields(query, dimensionQuery, dimensionQuery.Alias);
	}

	/// <summary>
	///		Obtiene los campos que se deben añadir de una consulta de dimensión
	/// </summary>
	private void ComputeDimensionFields(QueryModel query, QueryModel dimensionQuery, string dimensionAlias)
	{
		// Agrega los campos de la dimensión
		foreach (QueryFieldModel field in dimensionQuery.Fields)
			if (field.Visible)
				query.Fields.Add(new QueryFieldModel(query, field.IsPrimaryKey, dimensionAlias, field.Alias, field.Alias, 
													 field.OrderBy, field.Aggregation, field.Visible));
		// Añade los campos de las dimensiones hija
		foreach (QueryJoinModel queryJoin in dimensionQuery.Joins)
			ComputeDimensionFields(query, queryJoin.Query, dimensionAlias);
	}

	/// <summary>
	///		Obtiene el alias del origen de datos
	/// </summary>
	private string GetDataSourceAlias(BaseDataSourceModel baseDataSource)
	{
		switch (baseDataSource)
		{
			case DataSourceTableModel dataSource:
				return dataSource.Table;
			case DataSourceSqlModel dataSource:
				return dataSource.Id;
			default:
				return baseDataSource.Id;
		}
	}

	/// <summary>
	///		Obtiene los campos de dimensión por los que se va a agrupar (no se recojen directamente de las dimensiones solicitadas porque el
	///	orden de solicitud importa): estos campos de dimensión son los campos clave
	/// </summary>
	private void ComputeFieldsGroupBy(QueryModel query, ReportDataSourceModel dataSource, List<QueryModel> dimensionQueries)
	{
		foreach (DimensionRequestModel dimensionRequest in Generator.Request.Dimensions)
		{
			DimensionModel dimension = GetDimension(dimensionRequest);

				// Añade las relaciones
				foreach (DimensionRelationModel relation in dataSource.Relations)
					if (relation.Dimension is not null && relation.Dimension.Id.Equals(dimension.Id, StringComparison.CurrentCultureIgnoreCase))
						query.Joins.Add(CreateJoin(dimension, relation, dimensionQueries));
		}
	}

	/// <summary>
	///		Crea un JOIN con una dimensión
	/// </summary>
	private QueryJoinModel CreateJoin(DimensionModel dimension, DimensionRelationModel relation, List<QueryModel> dimensionQueries)
	{
		QueryModel query = new QueryModel(Generator, dimension.Id, QueryModel.QueryType.Dimension, dimension.Id);
		QueryJoinModel join = new QueryJoinModel(QueryJoinModel.JoinType.Inner, query, dimension.Id);
		QueryModel? relatedQuery = dimensionQueries.FirstOrDefault(item => item.SourceId.Equals(dimension.Id, StringComparison.CurrentCultureIgnoreCase));

			// Asigna los datos apropiados a la consulta (sólo necesitamos los nombres de tabla)
			query.Prepare(dimension.Id, dimension.Id);
			// Añade las relaciones
			if (relatedQuery is not null)
				foreach (RelationForeignKey column in relation.ForeignKeys)
				{
					QueryFieldModel? relatedField = relatedQuery.Fields.FirstOrDefault(item => item.Field.Equals(column.TargetColumnId, StringComparison.CurrentCultureIgnoreCase));

						if (relatedField is not null)
							join.Relations.Add(new QueryRelationModel(column.ColumnId, relatedQuery.Alias, relatedField.Alias));
				}
			// Devuelve el join
			return join;
	}

	/// <summary>
	///		Obtiene los campos de expresiones agrupados
	/// </summary>
	private void ComputeFieldsGrouped(QueryModel query, ReportRequestModel request, BaseDataSourceModel dataSource)
	{
		foreach (ExpressionRequestModel expression in request.Expressions)
			if (expression.ReportDataSourceId == dataSource.Id)
				foreach (ExpressionColumnRequestModel requestColumn in expression.Columns)
					foreach (DataSourceColumnModel column in dataSource.Columns.EnumerateValues())
						if (requestColumn.ColumnId.Equals(column.Id, StringComparison.CurrentCultureIgnoreCase))
							query.AddColumn(column.Id, column.Alias, requestColumn.AggregatedBy, requestColumn);
	}
}