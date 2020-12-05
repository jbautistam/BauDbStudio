using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries
{
	/// <summary>
	///		Generador de consultas para las expresiones
	/// </summary>
	internal class QueryExpressionsGenerator : BaseQueryGenerator
	{
		internal QueryExpressionsGenerator(ReportQueryGenerator generator) : base(generator) {}

		/// <summary>
		///		Obtiene la consulta de las expresiones
		/// </summary>
		internal QueryModel GetQuery()
		{
			//TODO En realidad, un informe puede tener varios orígenes de datos, cuál de los orígenes de datos se escoge, depende de ciertas condiciones como las dimensiones
			//TODO escogidas en la solicitud, para las primeras pruebas, cogemos directamente la primera solicitud
			ReportDataSourceModel reportDataSource = Generator.Report.ReportDataSources[0];
			BaseDataSourceModel baseDataSource = reportDataSource.DataSource;
			QueryModel query = new QueryModel(baseDataSource.GlobalId, QueryModel.QueryType.Expressions, "tmpExpressions");

				// Prepara la consulta
				query.Prepare(baseDataSource);
				// Obtiene los campos por los que vamos a agrupar y los añade a la colección de claves de dimensión de la consulta
				ComputeFieldsGroupBy(query, reportDataSource);
				// Obtiene los campos agrupados
				ComputeFieldsGrouped(query, Generator.Request, baseDataSource);
				// Devuelve la consulta
				return query;
		}

		/// <summary>
		///		Obtiene los campos de dimensión por los que se va a agrupar (no se recojen directamente de las dimensiones solicitadas porque el
		///	orden de solicitud importa): estos campos de dimensión son los campos clave
		/// </summary>
		private void ComputeFieldsGroupBy(QueryModel query, ReportDataSourceModel dataSource)
		{
			// 
			foreach (DimensionRequestModel dimensionRequest in Generator.Request.Dimensions)
			{
				DimensionModel dimension = GetDimension(dimensionRequest);

					// Añade las relaciones
					foreach (DimensionRelationModel relation in dataSource.Relations)
						if (relation.Dimension.GlobalId.Equals(dimension.GlobalId, StringComparison.CurrentCultureIgnoreCase))
						{
							// Añade la dimensión al JOIN
							query.Joins.Add(CreateJoin(dimension, relation));
							// Añade las columnas de clave para el GROUP BY y las claves foráneas
							foreach (RelationForeignKey column in relation.ForeignKeys)
							{
								// Añade el campo destino del origen de datos del informe a la lista de campos clave
								AddPrimaryKey(query, column.ColumnId);
								// Añade el campo a la lista de claves foráneas
								query.ForeignKeys.Add(new QueryForeignKeyFieldModel(dimension, column.TargetColumnId, column.ColumnId));
							}
						}
			}
		}

		/// <summary>
		///		Crea un JOIN con una dimensión
		/// </summary>
		private QueryJoinModel CreateJoin(DimensionModel dimension, DimensionRelationModel relation)
		{
			QueryModel query = new QueryModel(dimension.GlobalId, QueryModel.QueryType.Dimension, dimension.GlobalId);
			QueryJoinModel join = new QueryJoinModel(QueryJoinModel.JoinType.Inner, query, dimension.GlobalId);

				// Asigna los datos apropiados a la consulta (sólo necesitamos los nombres de tabla
				query.Prepare(dimension.GlobalId, dimension.GlobalId);
				// Añade las relaciones
				foreach (RelationForeignKey column in relation.ForeignKeys)
					join.Relations.Add(new QueryRelationModel(column.ColumnId, dimension.GlobalId, column.TargetColumnId));
				// Devuelve el join
				return join;
		}

		/// <summary>
		///		Obtiene los campos de expresiones agrupados
		/// </summary>
		private void ComputeFieldsGrouped(QueryModel query, ReportRequestModel request, BaseDataSourceModel dataSource)
		{
			foreach (ExpressionRequestModel expression in request.Expressions)
				if (expression.ReportDataSourceId == dataSource.GlobalId)
					foreach (ExpressionColumnRequestModel requestColumn in expression.Columns)
						foreach (DataSourceColumnModel column in dataSource.Columns)
							if (requestColumn.ColumnId.Equals(column.ColumnId, StringComparison.CurrentCultureIgnoreCase))
								AddColumn(query, column.ColumnId, string.Empty, requestColumn.AggregatedBy, requestColumn);
		}
	}
}