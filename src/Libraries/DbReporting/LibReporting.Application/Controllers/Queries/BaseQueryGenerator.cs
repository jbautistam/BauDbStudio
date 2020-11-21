using System;
using System.Collections.Generic;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries
{
	/// <summary>
	///		Clase baase para los generadores de consultas
	/// </summary>
	internal abstract class BaseQueryGenerator
	{
		protected BaseQueryGenerator(ReportQueryGenerator generator, List<DimensionModel> dimensionsRequested)
		{
			Generator = generator;
			DimensionsRequested = dimensionsRequested;
		}

		/// <summary>
		///		Obtiene una cadena concatenada con campos
		/// </summary>
		protected string GetSqlFields(List<string> fields)
		{
			string sql = string.Empty;

				// Crea la cadena SQL con los campos de dimensión
				foreach (string field in fields)
					sql = sql.AddWithSeparator($"[{field}]", ",");
				// Devuelve la cadena
				return sql;
		}

		/// <summary>
		///		Obtiene una cadena concatenada con campos
		/// </summary>
		protected string GetSqlFields(NormalizedDictionary<string> keyFields)
		{
			string sql = string.Empty;

				// Crea la cadena SQL con los campos de dimensión
				foreach ((string key, string field) in keyFields.Enumerate())
					sql = sql.AddWithSeparator($"[{field}]", ",");
				// Devuelve la cadena
				return sql;
		}

		/// <summary>
		///		Busca un origen de datos
		/// </summary>
		protected BaseDataSourceModel SearchDataSource(string dataSourceId)
		{
			// Busca el origen de datos
			foreach (DataWarehouseModel dataWarehouse in Generator.Schema.DataWarehouses)
			{
				BaseDataSourceModel dataSource = dataWarehouse.DataSources.Search(dataSourceId);

					if (dataSource != null)
						return dataSource;
			}
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Añade un campo de clave primaria a la consulta
		/// </summary>
		protected void AddPrimaryKey(QueryModel query, string column)
		{
			query.Fields.Add(new QueryFieldModel(true, column, string.Empty, ExpressionRequestModel.AggregationType.NoAggregated, false));
		}

		/// <summary>
		///		Añade un campo a la consulta
		/// </summary>
		protected void AddColumn(QueryModel query, string columnId, string alias, BaseColumnRequestModel requestColumn)
		{
			AddColumn(query, columnId, alias, ExpressionRequestModel.AggregationType.NoAggregated, requestColumn);
		}

		/// <summary>
		///		Añade un campo a la consulta
		/// </summary>
		protected void AddColumn(QueryModel query, string columnId, string alias, ExpressionRequestModel.AggregationType aggregatedBy, BaseColumnRequestModel requestColumn)
		{
			QueryFieldModel field = new QueryFieldModel(false, columnId, alias, aggregatedBy, requestColumn.Visible);

				// Añade los filtros
				field.FiltersWhere.AddRange(GetFilters(requestColumn.FiltersWhere));
				field.FilterHaving.AddRange(GetFilters(requestColumn.FiltersHaving));
				// Añade la columna a la consulta
				query.Fields.Add(field);
		}

		/// <summary>
		///		Convierte los filtros
		/// </summary>
		private List<QueryFilterModel> GetFilters(List<FilterRequestModel> filters)
		{
			List<QueryFilterModel> converted = new List<QueryFilterModel>();

				// Convierte los filtros
				foreach (FilterRequestModel filter in filters)
					converted.Add(new QueryFilterModel(filter.Condition, filter.Values));
				// Devuelve los filtros convertidos
				return converted;
		}

		/// <summary>
		///		Generador del informe
		/// </summary>
		protected ReportQueryGenerator Generator { get; }

		/// <summary>
		///		Dimensiones solicitadas
		/// </summary>
		protected List<DimensionModel> DimensionsRequested { get; }
	}
}
