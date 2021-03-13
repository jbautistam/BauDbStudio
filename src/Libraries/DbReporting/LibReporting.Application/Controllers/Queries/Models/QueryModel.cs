using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models
{
	/// <summary>
	///		Clase con los datos de una consulta
	/// </summary>
	internal class QueryModel
	{
		/// <summary>
		///		Tipo de consulta
		/// </summary>
		internal enum QueryType
		{
			/// <summary>Consulta de datos de una dimensión</summary>
			Dimension,
			/// <summary>Consulta de expresiones</summary>
			Expressions
		}

		internal QueryModel(string sourceId, QueryType type, string alias)
		{
			SourceId = sourceId;
			Type = type;
			Alias = alias;
		}

		/// <summary>
		///		Obtiene el nombre de tabla (o la claúsula FROM) y las columnas de un origen de datos
		/// </summary>
		internal void Prepare(BaseDataSourceModel baseDataSource)
		{
			switch (baseDataSource)
			{
				case DataSourceTableModel dataSource:
						FromTable = dataSource.FullName;
						FromAlias = dataSource.Table;
					break;
				case DataSourceSqlModel dataSource:
						FromTable = $"({dataSource.Sql})"; 
						FromAlias = dataSource.Id;
					break;
			}
		}

		/// <summary>
		///		Prepara la consulta únicamente con un nombre de tabla / alias (lo utiliza el generador de expresiones para los INNER JOIN con las consultas de dimensión)
		/// </summary>
		internal void Prepare(string table, string alias)
		{
			FromTable = table;
			FromAlias = alias;
		}

		/// <summary>
		///		Genera la consulta SQL de esta clase
		/// </summary>
		internal string Build()
		{
			Prettifier.StringPrettifier prettifier = new Prettifier.StringPrettifier();

				// Añade la cláusula SELECT con los campos (los de ésta y los de mis JOIN hija)
				prettifier.Append("SELECT " + GetSqlFields(), 100, ",");
				// Añade la cláusula FROM
				prettifier.NewLine();
				prettifier.Indent();
				prettifier.Append($"FROM {FromTable} AS [{FromAlias}]");
				// Añade los JOIN
				if (Joins.Count > 0)
				{
					prettifier.NewLine();
					prettifier.Append(GetSqlJoins());
				}
				prettifier.Unindent();
				prettifier.NewLine();
				// Añade los WHERE
				prettifier.Indent();
				prettifier.Append(GetSqlClauseFilters(true));
				prettifier.Unindent();
				// Añade los GROUP BY
				prettifier.Indent();
				prettifier.Append(GetSqlGroupBy(), 100, ",");
				prettifier.Unindent();
				// Añade los HAVING
				prettifier.Indent();
				prettifier.Append(GetSqlClauseFilters(false));
				prettifier.Unindent();
				// Devuelve la consulta SQL
				return prettifier.ToString();
		}

		/// <summary>
		///		Obtiene los campos de la consulta (los campos clave de esta tabla, los campos de salida de esta tabla y 
		///	los campos con las tabla con las que se haga JOIN por debajo)
		/// </summary>
		private string GetSqlFields()
		{
			string sqlFields = string.Empty;

				// Añade las claves
				foreach (QueryFieldModel field in Fields)
					if (field.IsPrimaryKey)
						sqlFields = sqlFields.AddWithSeparator($"[{FromAlias}].[{field.Field}]", ",");
				// Añade los campos
				foreach (QueryFieldModel field in Fields)
					if (!field.IsPrimaryKey && field.Visible && field.Aggregation == ExpressionColumnRequestModel.AggregationType.NoAggregated)
						sqlFields = sqlFields.AddWithSeparator(GetSqlField(FromAlias, field), ",");
				// Añade los campos (no clave) de los JOIN hijo (dimensiones hija) que no estén agregados
				foreach (QueryJoinModel join in Joins)
					foreach (QueryFieldModel field in join.Query.Fields)
						if (!field.IsPrimaryKey && field.Visible && field.Aggregation == ExpressionColumnRequestModel.AggregationType.NoAggregated)
							sqlFields = sqlFields.AddWithSeparator(GetSqlField(join.Query.FromAlias, field), ",");
				// Añade los campos agrupados
				foreach (QueryFieldModel field in Fields)
					if (field.Aggregation != ExpressionColumnRequestModel.AggregationType.NoAggregated)
						sqlFields = sqlFields.AddWithSeparator($"{field.GetAggregation(FromAlias)} AS [{field.Alias}]", ",");
				// Devuelve los campos
				return sqlFields;
		}

		/// <summary>
		///		Obtiene el nombre de un campo con el AS
		/// </summary>
		private string GetSqlField(string table, QueryFieldModel field)
		{
			string fieldName = $"[{table}].[{field.Field}]";

				// Añade el alias
				if (!string.IsNullOrWhiteSpace(field.Alias))
					fieldName += $" AS [{field.Alias}]";
				// Devuelve el nombre del campo
				return fieldName;
		}

		/// <summary>
		///		Obtiene la cadena de filtros incluyendo la cláusula WHERE / HAVING adecuada
		/// </summary>
		private string GetSqlClauseFilters(bool whereClause)
		{
			string sql = GetSqlFilters(whereClause);

				// Asigna la cláusula
				if (!string.IsNullOrWhiteSpace(sql))
				{
					if (whereClause)
						sql = $"WHERE {sql}" + Environment.NewLine;
					else
						sql = $"HAVING {sql}" + Environment.NewLine;
				}
				// Devuelve la cadena SQL del filtro
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena SQL de los filtros y de sus hijos
		/// </summary>
		private string GetSqlFilters(bool where)
		{
			string sql = string.Empty;

				// Añade los filtros de esta consulta
				foreach (QueryFieldModel field in Fields)
					foreach (QueryFilterModel filter in where ? field.FiltersWhere : field.FilterHaving)
						if (where)
							sql = sql.AddWithSeparator(filter.GetSql(FromAlias, field.Field), Environment.NewLine + "AND");
						else // ... si estamos en un having, la condición es por el agregado
							sql = sql.AddWithSeparator(filter.GetSql(field.GetAggregation(FromAlias)), Environment.NewLine + "AND");
				// Añade los filtros de las consulta con JOIN
				foreach (QueryJoinModel join in Joins)
				{
					string sqlFilter = join.Query.GetSqlFilters(where);

						if (!string.IsNullOrWhiteSpace(sqlFilter))
							sql = sql.AddWithSeparator(sqlFilter, Environment.NewLine + "AND");
				}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la SQL de consulta de los JOIN
		/// </summary>
		private string GetSqlJoins()
		{
			string sql = string.Empty;

				// Añade las consultas para los JOIN
				foreach (QueryJoinModel join in Joins)
				{
					bool needAnd = false;

						// Añade el tipo de JOIN
						sql = sql.AddWithSeparator(GetJoin(join.Type), Environment.NewLine);
						// Añade el nombre de tabla y el alias
						sql += $" {join.Query.FromTable} AS [{join.Query.FromAlias}]" + Environment.NewLine;
						// Añade las relaciones
						sql += " ON ";
						foreach (QueryRelationModel relation in join.Relations)
						{
							// Añade el operador AND si es necesario
							if (needAnd)
								sql += " AND ";
							else
								needAnd = true;
							// Añade la condición entre campos
							sql += $"[{FromAlias}].[{relation.Column}] = [{relation.RelatedTable}].[{relation.RelatedColumn}]" + Environment.NewLine;
						}
				}
				// Devuelve la consulta
				return sql;
		}

		/// <summary>
		///		Obtiene la cláusula adecuada para el JOIN
		/// </summary>
		private string GetJoin(QueryJoinModel.JoinType type)
		{
			switch (type)
			{
				case QueryJoinModel.JoinType.Left:
					return "LEFT JOIN";
				case QueryJoinModel.JoinType.Right:
					return "RIGHT JOIN";
				case QueryJoinModel.JoinType.Full:
					return "FULL OUTER JOIN";
				default:
					return "INNER JOIN";
			}
		}

		/// <summary>
		///		Obtiene la cadena de agrupación
		/// </summary>
		private string GetSqlGroupBy()
		{
			string sqlFields = string.Empty;

				// Sólo tiene GROUP BY cuando hay algún campo agregado
				if (NeedGroupBy())
				{
					// Añade los campos de agrupación
					foreach (QueryFieldModel field in Fields)
						if (field.IsPrimaryKey || (field.Aggregation == ExpressionColumnRequestModel.AggregationType.NoAggregated && 
												   field.Visible))
							sqlFields = sqlFields.AddWithSeparator($"[{FromAlias}].[{field.Field}]", ",");
					// Si hay algún campo de agrupación, le añade la cláusula
					if (!string.IsNullOrWhiteSpace(sqlFields))
						sqlFields = $"GROUP BY {sqlFields}" + Environment.NewLine;
				}
				// Devuelve la cadena de agrupación
				return sqlFields;
		}

		/// <summary>
		///		Comprueba si necesita una cláusula GROUP BY: si hay algún agregado
		/// </summary>
		private bool NeedGroupBy()
		{
			// Recorre los campos buscando si hay algún agregado
			foreach (QueryFieldModel field in Fields)
				if (field.Aggregation != ExpressionColumnRequestModel.AggregationType.NoAggregated)
					return true;
			// Si ha llegado hasta aquí es porque no hay ningún campo agregado
			return false;
		}

		/// <summary>
		///		Obtiene la cadena de ordenación
		/// </summary>
		internal string GetOrderByFields(string tableAliasAtWith = null)
		{
			string sql = string.Empty;

				// Normaliza el nombre del alias de la tabla que
				if (string.IsNullOrWhiteSpace(tableAliasAtWith))
					tableAliasAtWith = Alias;
				// Añade los campos a ordenar
				foreach (QueryFieldModel field in Fields)
					if (field.Visible && field.OrderBy != BaseColumnRequestModel.SortOrder.Undefined)
						sql = sql.AddWithSeparator($"[{tableAliasAtWith}].[{field.Field}] {GetSortClause(field.OrderBy)}", ",");
				// Añade los campos a ordenar de las consultas hijo
				foreach (QueryJoinModel join in Joins)
					sql = sql.AddWithSeparator(join.Query.GetOrderByFields(tableAliasAtWith), ",");
				// Devuelve la cadena
				return sql;
		}

		/// <summary>
		///		Obtiene la cláusula de ordenación
		/// </summary>
		private string GetSortClause(BaseColumnRequestModel.SortOrder orderBy)
		{
			if (orderBy == BaseColumnRequestModel.SortOrder.Ascending)
				return "ASC";
			else
				return "DESC";
		}

		/// <summary>
		///		Id del origen de la consulta (código de expresión, código de informe)
		/// </summary>
		internal string SourceId { get; }

		/// <summary>
		///		Tipo de consulta
		/// </summary>
		internal QueryType Type { get; }

		/// <summary>
		///		Tabla de la consulta
		/// </summary>
		internal string FromTable { get; private set; }

		/// <summary>
		///		Alias de la tabla
		/// </summary>
		internal string FromAlias { get; private set; }

		/// <summary>
		///		Alias de la consulta
		/// </summary>
		internal string Alias { get; }

		/// <summary>
		///		Campos de la consulta
		/// </summary>
		internal List<QueryFieldModel> Fields { get; } = new List<QueryFieldModel>();

		/// <summary>
		///		Subconsultas combinadas
		/// </summary>
		internal List<QueryJoinModel> Joins { get; } = new List<QueryJoinModel>();

		/// <summary>
		///		Claves foráneas de esta consulta
		/// </summary>
		internal List<QueryForeignKeyFieldModel> ForeignKeys { get; } = new List<QueryForeignKeyFieldModel>();
	}
}
