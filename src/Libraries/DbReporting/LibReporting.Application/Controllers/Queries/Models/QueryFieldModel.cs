using System;
using System.Collections.Generic;

using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models
{
	/// <summary>
	///		Campo de una consulta
	/// </summary>
	internal class QueryFieldModel
	{
		internal QueryFieldModel(bool primaryKey, string field, string alias, ExpressionColumnRequestModel.AggregationType aggregation, bool visible)
		{
			IsPrimaryKey = primaryKey;
			Field = field;
			if (string.IsNullOrWhiteSpace(alias))
				Alias = GetAliasFromAggregation(aggregation);
			else
				Alias = alias;
			Aggregation = aggregation;
			Visible = visible;
		}

		/// <summary>
		///		Obtiene un alias dependiendo de la agregación
		/// </summary>
		private string GetAliasFromAggregation(ExpressionColumnRequestModel.AggregationType aggregation)
		{ 
			switch (aggregation)
			{
				case ExpressionColumnRequestModel.AggregationType.Average:
					return $"Average {Field}";
				case ExpressionColumnRequestModel.AggregationType.Max:
					return $"Max {Field}";
				case ExpressionColumnRequestModel.AggregationType.Min:
					return $"Min {Field}";
				case ExpressionColumnRequestModel.AggregationType.StandardDeviation:
					return $"Standard deviation {Field}";
				case ExpressionColumnRequestModel.AggregationType.Sum:
					return $"Sum {Field}";
				default:
					return Field;
			}
		}

		/// <summary>
		///		Obtiene el alias de un campo para una consulta
		/// </summary>
		internal string GetTableFieldAlias(string table)
		{
			return $"[{table}].[{Alias}]";
		}

		/// <summary>
		///		Obtiene la agregación necesaria del campo
		/// </summary>
		internal string GetAggregation(string table)
		{
			string computed = $"[{table}].[{Field}]";

				switch (Aggregation)
				{
					case ExpressionColumnRequestModel.AggregationType.Average:
						return $"AVG({computed})";
					case ExpressionColumnRequestModel.AggregationType.Max:
						return $"MAX({computed})";
					case ExpressionColumnRequestModel.AggregationType.Min:
						return $"MIN({computed})";
					case ExpressionColumnRequestModel.AggregationType.StandardDeviation:
						return $"STDDEV({computed})";
					case ExpressionColumnRequestModel.AggregationType.Sum:
						return $"SUM({computed})";
					default:
						return computed;
				}
		}

		/// <summary>
		///		Indica si es una clave primaria
		/// </summary>
		internal bool IsPrimaryKey { get; }

		/// <summary>
		///		Nombre del campo
		/// </summary>
		internal string Field { get; }

		/// <summary>
		///		Alias
		/// </summary>
		internal string Alias { get; }

		/// <summary>
		///		Agregación
		/// </summary>
		internal ExpressionColumnRequestModel.AggregationType Aggregation { get; }

		/// <summary>
		///		Indica si la columna es visible en la consulta
		/// </summary>
		internal bool Visible { get; }

		/// <summary>
		///		Filtros de la cláusula WHERE
		/// </summary>
		internal List<QueryFilterModel> FiltersWhere { get; } = new List<QueryFilterModel>();

		/// <summary>
		///		Filtros de la cláusula HAVING
		/// </summary>
		internal List<QueryFilterModel> FilterHaving { get; } = new List<QueryFilterModel>();
	}
}
