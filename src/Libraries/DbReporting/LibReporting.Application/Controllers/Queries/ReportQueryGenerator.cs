using System;
using System.Collections.Generic;

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
			List<QueryModel> dimensionQueries = GetDimensionQueries(Request);
			QueryModel expressionQuery = new QueryExpressionsGenerator(this).GetQuery();
			string sql = string.Empty;

				// Obtiene las dimensiones de las consultas
				sql = GetSqlWithBlocks(dimensionQueries, expressionQuery);
				// Obtiene la consulta de unión de los bloques WITH
				sql += Environment.NewLine + GetSqlJoinBlocks(dimensionQueries, expressionQuery);
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Pone una cadena en bonito
		/// </summary>
		private string Prettify(string sql)
		{
			Prettifier.StringPrettifier prettifier = new Prettifier.StringPrettifier();

				// Asigna la indentación
				prettifier.Indent();
				// Añade la cadena
				prettifier.Append(sql, 100, ",");
				// Devuevle la cadena
				return prettifier.ToString();
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
			Prettifier.StringPrettifier prettifier = new Prettifier.StringPrettifier();

				// Añade una indentación
				prettifier.Indent();
				// Añade la SELECT con los campos
				prettifier.Append("SELECT " + GetSqlFinalFields(dimensionQueries, expressionQuery), 100, ",");
				prettifier.NewLine();
				// Indenta para FROM y JOINS
				prettifier.Indent();
				// Añade el from
				prettifier.Append($"FROM [{expressionQuery.Alias}]");
				// Añade los joins con las dimensiones
				foreach (QueryModel dimensionQuery in dimensionQueries)
				{
					// Añade el INNER JOIN
					prettifier.NewLine();
					prettifier.Append($"INNER JOIN [{dimensionQuery.Alias}]");
					// Añade la cláusula ON a la cadena SQL
					prettifier.NewLine();
					prettifier.Indent();
					prettifier.Append($"ON {GetSqlOnConditions(dimensionQuery, expressionQuery)}", 100, Environment.NewLine);
					prettifier.Unindent();
				}
				// Añade el ORDER BY
				prettifier.NewLine();
				prettifier.Append(GetSqlOrderBy(dimensionQueries, expressionQuery), 100, ",");
				prettifier.NewLine();
				// Quita la indentación de FROM y JOINS y la general
				prettifier.Unindent();
				prettifier.Unindent();
				// Devuelve la cadena SQL
				return prettifier.ToString();
		}

		/// <summary>
		///		Obtiene los datos de la condición ON
		/// </summary>
		private string GetSqlOnConditions(QueryModel dimensionQuery, QueryModel expressionQuery)
		{
			string sql = string.Empty;

				// Añade la comparación de las claves foráneas
				foreach (QueryForeignKeyFieldModel foreignKey in expressionQuery.ForeignKeys)
					if (foreignKey.Dimension.Id.Equals(dimensionQuery.SourceId, StringComparison.CurrentCultureIgnoreCase))
					{
						// Añade la cláusula AND si es necesario
						if (!string.IsNullOrWhiteSpace(sql))
							sql += "AND ";
						// Añade la relación de tablas
						sql += $"[{expressionQuery.Alias}].[{foreignKey.ColumnRelated}] = [{dimensionQuery.Alias}].[{foreignKey.ColumnDimension}]";
					}
				// Devuelve la cadena
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
					sqlFields = sqlFields.AddWithSeparator(GetSqlVisibleFields(query, query.Alias), ",");
					// Añade los campos de las consultas hija
					foreach (QueryJoinModel join in query.Joins)
						sqlFields = sqlFields.AddWithSeparator(GetSqlVisibleFields(join.Query, query.Alias), ",");
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
		private string GetSqlVisibleFields(QueryModel query, string tableAliasAtWith)
		{
			string sqlFields = string.Empty;

				// Añade los campos visualizables: los que no son clave primaria y están marcados como visibles (porque algunos
				// estarán en la consulta únicamente por los filtros)
				foreach (QueryFieldModel field in query.Fields)
					if (!field.IsPrimaryKey && field.Visible)
						sqlFields = sqlFields.AddWithSeparator($"[{tableAliasAtWith}].[{field.Alias}]", ",");
				// Devuelve la cadena con los campos
				return sqlFields;
		}

		/// <summary>
		///		Obtiene las consultas de dimensión solicitadas
		/// </summary>
		private List<QueryModel> GetDimensionQueries(ReportRequestModel request)
		{
			List<QueryModel> queries = new List<QueryModel>();

				// Obtiene las consultas de las dimensiones
				foreach (DimensionRequestModel dimensionRequested in request.Dimensions)
					queries.Add(new QueryDimensionGenerator(this).GetQuery(dimensionRequested));
				// Devuelve las consultas
				return queries;
		}

		/// <summary>
		///		Obtiene la cadena SQL ORDER BY
		/// </summary>
		private string GetSqlOrderBy(List<QueryModel> dimensionQueries, QueryModel expressionQuery)
		{
			string sqlFields = string.Empty;

				// Obtiene los campos por los que se ordenan las dimensiones
				foreach (QueryModel dimensionQuery in dimensionQueries)
					sqlFields = sqlFields.AddWithSeparator(dimensionQuery.GetOrderByFields(), ",");
				// Obtiene los campos por los que se ordenan las expresiones
				sqlFields = sqlFields.AddWithSeparator(expressionQuery.GetOrderByFields(), ",");
				// Añade la cláusula ORDER BY si es necesario
				if (!string.IsNullOrWhiteSpace(sqlFields))
					sqlFields = $"ORDER BY {sqlFields}";
				// Devuelve la cadena SQL
				return sqlFields;
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
