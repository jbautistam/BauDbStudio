using System;
using System.Collections.Generic;
using System.Linq;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries
{
	/// <summary>
	///		Generador de consultas SQL para un informe
	/// </summary>
	internal class ReportQueryGenerator
	{
		internal ReportQueryGenerator(ReportingSchemaModel schema, ReportModel report, ReportRequestModel request)
		{
			Schema = schema;
			Report = report;
			Request = request;
		}

		/// <summary>
		///		Obtiene la cadena SQL de un informe para responder a una solicitud
		/// </summary>
		/// <remarks>
		///		La cadena SQL va a ser del tipo
		///		WITH tmpDimension1 AS
		///			(SELECT Id, Field1, Field2
		///				FROM Dimension1
		///				WHERE [Filtros]
		///			),
		///			tmpExpression AS
		///			(SELECT DimensionId1, AGGREGATE(Expression1) AS Aggregate_Expresion1, ...
		///				FROM DataSource
		///				WHERE [Filtros]
		///				GROUP BY DimensionId1
		///				HAVING [Filtros]
		///			)
		///			SELECT Fields
		///				FROM tmpExpressions INNER JOIN tmpDimension
		///					ON Conditions
		///				ORDER BY Fields
		/// </remarks>
		internal string GetSql()
		{
			List<DimensionModel> dimensionsRequested = GetDimensionsRequested(Report, Request);
			List<QueryModel> dimensionQueries = GetDimensionQueries(dimensionsRequested);
			QueryModel expressionQuery = new QueryExpressionsGenerator(this, dimensionsRequested).GetQuery();
			string sql = string.Empty;

				// Obtiene las dimensiones de las consultas
				sql = GetSqlWithBlocks(dimensionQueries, expressionQuery);
				// Obtiene la consulta de unión de los bloques WITH
				sql += Environment.NewLine + GetSqlJoinBlocks(dimensionQueries, expressionQuery);
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene el bloque WITH de la SQL
		/// </summary>
		private string GetSqlWithBlocks(List<QueryModel> dimensionQueries, QueryModel expressionQuery)
		{
			string sql = string.Empty;

				// Obtiene los bloques WITH de las dimensiones
				foreach (QueryModel dimensionQuery in dimensionQueries)
					sql = sql.AddWithSeparator(GetSqlWithBlock(dimensionQuery), "," + Environment.NewLine);
				// Obtiene el bloque WITH de las expresiones
				sql = sql.AddWithSeparator(GetSqlWithBlock(expressionQuery), "," + Environment.NewLine);
				// Devuelve la cadena SQL
				return $"WITH {sql}";
		}

		/// <summary>
		///		Obtiene el bloque WITH de una consulta
		/// </summary>
		private string GetSqlWithBlock(QueryModel query)
		{
			return $"{query.Alias} AS {Environment.NewLine}({query.Build()})";
		}

		/// <summary>
		///		Obtiene la cadena SQL que une los diferentes WITH
		/// </summary>
		private string GetSqlJoinBlocks(List<QueryModel> dimensionQueries, QueryModel expressionQuery)
		{
			string sql = "SELECT " + GetSqlFinalFields(dimensionQueries, expressionQuery);

				// Añade la tabla de expresiones
				sql += Environment.NewLine + $"FROM [{expressionQuery.Alias}]";
				// Añade los joins con las dimensiones
				foreach (QueryModel dimensionQuery in dimensionQueries)
				{
					string sqlOn = string.Empty;

						// Añade el INNER JOIN
						sql += Environment.NewLine + $"INNER JOIN [{dimensionQuery.Alias}]";
						// Añade las cláusulas ON
						sql += Environment.NewLine + "ON ";
						foreach (QueryForeignKeyFieldModel foreignKey in expressionQuery.ForeignKeys)
							if (foreignKey.Dimension.GlobalId.Equals(dimensionQuery.SourceId, StringComparison.CurrentCultureIgnoreCase))
							{
								// Añade la cláusula AND si es necesario
								if (!string.IsNullOrWhiteSpace(sqlOn))
									sqlOn += "AND ";
								// Añade la relación de tablas
								sqlOn += $"[{expressionQuery.Alias}].[{foreignKey.ColumnRelated}] = [{dimensionQuery.Alias}].[{foreignKey.ColumnDimension}]";
							}
						// Añade la cláusula ON a la cadena SQL
						sql += sqlOn;
				}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL con los campos de salida: aquellos que no son clave en las diferentes consultas
		/// </summary>
		private string GetSqlFinalFields(List<QueryModel> dimensionQueries, QueryModel expressionQuery)
		{
			string sqlFields = string.Empty;

				// Obtiene los campos de las dimensiones
				foreach (QueryModel query in dimensionQueries)
				{
					// Añade los campos de la consulta
					sqlFields = sqlFields.AddWithSeparator(GetSqlVisibleFields(query), ",");
					// Añade los campos de las consultas hija
					foreach (QueryJoinModel join in query.Joins)
						sqlFields = sqlFields.AddWithSeparator(GetSqlVisibleFields(join.Query), ",");
				}
				// Obtiene los campos de las expresiones
				foreach (QueryFieldModel field in expressionQuery.Fields)
					if (!field.IsPrimaryKey && field.Visible)
						sqlFields = sqlFields.AddWithSeparator($"[{expressionQuery.Alias}].[{field.Alias}]", ",");
				// Devuelve la cadena SQL
				return sqlFields;
		}

		/// <summary>
		///		Obtiene una cadena con los campos visibles de una consulta
		/// </summary>
		private string GetSqlVisibleFields(QueryModel query)
		{
			string sqlFields = string.Empty;

				// Añade los campos visualizables: los que no son clave primaria y están marcados como visibles (porque algunos
				// estarán en la consulta únicamente por los filtros)
				foreach (QueryFieldModel field in query.Fields)
					if (!field.IsPrimaryKey && field.Visible)
						sqlFields = sqlFields.AddWithSeparator($"[{query.Alias}].[{field.Alias}]", ",");
				// Devuelve la cadena con los campos
				return sqlFields;
		}

		/// <summary>
		///		Obtiene las consultas de dimensión solicitadas
		/// </summary>
		private List<QueryModel> GetDimensionQueries(List<DimensionModel> dimensions)
		{
			List<QueryModel> queries = new List<QueryModel>();

				// Obtiene las consutlas de dimensiones
				foreach (DimensionModel dimension in dimensions)
					queries.Add(new QueryDimensionGenerator(this, dimensions).GetQuery(dimension));
				// Devuelve las consultas
				return queries;
		}

		/// <summary>
		///		Obtiene las dimensiones solicitadas
		/// </summary>
		private List<DimensionModel> GetDimensionsRequested(ReportModel report, ReportRequestModel request)
		{
			List<DimensionModel> requestedDimensions = new List<DimensionModel>();

				// Recorre las dimensiones solicitadas
				foreach (ReportDataSourceModel dataSource in report.Expressions)
					foreach (DimensionRelationModel relation in dataSource.Relations)
						if (IsRequested(relation.Dimension, request) && requestedDimensions.FirstOrDefault(item => item.GlobalId == relation.Dimension.GlobalId) == null)
							requestedDimensions.Add(relation.Dimension);
				// Devuelve las dimensiones solicitadas
				return requestedDimensions;
		}

		/// <summary>
		///		Comprueba si se ha solicitado una dimensión
		/// </summary>
		private bool IsRequested(DimensionModel dimension, ReportRequestModel request)
		{
			// Comprueba si se ha solicitado alguna columna de esta dimensión
			foreach (BaseColumnRequestModel baseColumnRequest in request.Columns)
				if (baseColumnRequest is DimensionRequestModel requestColumn && dimension.HasColumn(requestColumn.DimensionId, requestColumn.ColumnId))
					return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Esquema para las consultas
		/// </summary>
		internal ReportingSchemaModel Schema { get; }

		/// <summary>
		///		Informe solicitado
		/// </summary>
		internal ReportModel Report { get; }

		/// <summary>
		///		Datos de la solicitud
		/// </summary>
		internal ReportRequestModel Request { get; }
	}
}
