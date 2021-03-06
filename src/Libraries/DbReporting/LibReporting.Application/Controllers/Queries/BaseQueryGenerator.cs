using System;
using System.Collections.Generic;
using System.Linq;

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
	///		Clase base para los generadores de consultas
	/// </summary>
	internal abstract class BaseQueryGenerator
	{
		protected BaseQueryGenerator(ReportQueryGenerator generator)
		{
			Generator = generator;
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
			foreach (DataWarehouseModel dataWarehouse in Generator.Schema.DataWarehouses.EnumerateValues())
			{
				BaseDataSourceModel dataSource = dataWarehouse.DataSources.Search(dataSourceId);

					if (dataSource != null)
						return dataSource;
			}
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Obtiene la dimensión
		/// </summary>
		protected DimensionModel GetDimension(DimensionRequestModel dimensionRequest)
		{
			DimensionModel dimension = Generator.Report.DataWarehouse.Dimensions.Search(dimensionRequest.DimensionId);

				// Devuelve la dimensión localizada o lanza una excepción
				if (dimension == null)
					throw new LibReporting.Models.Exceptions.ReportingException($"Cant find the dimension {dimensionRequest.DimensionId}");
				else
					return dimension;
		}

		/// <summary>
		///		Añade un campo de clave primaria a la consulta
		/// </summary>
		protected void AddPrimaryKey(QueryModel query, string column)
		{
			query.Fields.Add(new QueryFieldModel(true, column, string.Empty, BaseColumnRequestModel.SortOrder.Undefined, 
												 ExpressionColumnRequestModel.AggregationType.NoAggregated, false));
		}

		/// <summary>
		///		Añade un campo a la consulta
		/// </summary>
		protected void AddColumn(QueryModel query, string columnId, string alias, BaseColumnRequestModel requestColumn)
		{
			AddColumn(query, columnId, alias, ExpressionColumnRequestModel.AggregationType.NoAggregated, requestColumn);
		}

		/// <summary>
		///		Añade un campo a la consulta
		/// </summary>
		protected void AddColumn(QueryModel query, string columnId, string alias, 
								 ExpressionColumnRequestModel.AggregationType aggregatedBy, BaseColumnRequestModel requestColumn)
		{
			QueryFieldModel field = GetQueryField(query, columnId, alias, aggregatedBy, requestColumn);

				// Añade los filtros
				field.FiltersWhere.AddRange(GetFilters(requestColumn.FiltersWhere));
				field.FilterHaving.AddRange(GetFilters(requestColumn.FiltersHaving));
				// Añade la columna a la consulta
				query.Fields.Add(field);
		}

		/// <summary>
		///		Obtiene el campo de la consulta
		/// </summary>
		private QueryFieldModel GetQueryField(QueryModel query, string columnId, string alias,
											  ExpressionColumnRequestModel.AggregationType aggregatedBy, BaseColumnRequestModel requestColumn)
		{
			QueryFieldModel field = query.Fields.FirstOrDefault(item => item.CompareWith(columnId, alias));

				// Si no existía, lo añade
				if (field == null)
					field = new QueryFieldModel(false, columnId, alias, requestColumn.OrderBy, aggregatedBy, requestColumn.Visible);
				// Devuelve el campo
				return field;
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
	}
}
