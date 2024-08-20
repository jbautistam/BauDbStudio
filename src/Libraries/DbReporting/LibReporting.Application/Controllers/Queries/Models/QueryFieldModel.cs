﻿using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Campo de una consulta
/// </summary>
internal class QueryFieldModel
{
	// Variables privadas
	private string _alias = string.Empty;

	internal QueryFieldModel(QueryModel query, bool primaryKey, string table, string field, string alias, 
							 BaseColumnRequestModel.SortOrder orderBy, 
							 ExpressionColumnRequestModel.AggregationType aggregation, bool visible)
	{
		Query = query;
		IsPrimaryKey = primaryKey;
		Table = table;
		Field = field;
		Alias = alias;
		Aggregation = aggregation;
		Visible = visible;
		if (Visible)
			OrderBy = orderBy;
	}

	/// <summary>
	///		Obtiene la agregación necesaria del campo
	/// </summary>
	internal string GetAggregation(string table)
	{
		string computed = Query.Generator.SqlTools.GetFieldName(table, Field);

			return Aggregation switch
						{
							ExpressionColumnRequestModel.AggregationType.Average => $"AVG({computed})",
							ExpressionColumnRequestModel.AggregationType.Max => $"MAX({computed})",
							ExpressionColumnRequestModel.AggregationType.Min => $"MIN({computed})",
							ExpressionColumnRequestModel.AggregationType.StandardDeviation => $"STDDEV({computed})",
							ExpressionColumnRequestModel.AggregationType.Sum => $"SUM({computed})",
							_ => computed
						};
	}

	/// <summary>
	///		Cosulta a la que se asocia el campo
	/// </summary>
	internal QueryModel Query { get; }

	/// <summary>
	///		Indica si es una clave primaria
	/// </summary>
	internal bool IsPrimaryKey { get; }

	/// <summary>
	///		Nombre o alias de la tabla
	/// </summary>
	internal string Table { get; }

	/// <summary>
	///		Nombre del campo
	/// </summary>
	internal string Field { get; }

	/// <summary>
	///		Alias
	/// </summary>
	internal string Alias 
	{ 
		get
		{
			string alias = _alias;

				// Si no se ha definido el alias, se calcula
				if (string.IsNullOrWhiteSpace(_alias))
				{
					// Genera el alias inicial
					alias = $"{Table}_{Field}";
					// Añade la agregación si es necesario
					switch (Aggregation)
					{
						case ExpressionColumnRequestModel.AggregationType.Average:
								alias += "_AVG";
							break;
						case ExpressionColumnRequestModel.AggregationType.Max:
								alias += "_MAX";
							break;
						case ExpressionColumnRequestModel.AggregationType.Min:
								alias += "_MIN";
							break;
						case ExpressionColumnRequestModel.AggregationType.StandardDeviation:
								alias += "_STD";
							break;
						case ExpressionColumnRequestModel.AggregationType.Sum:
								alias += "_SUM";
							break;
					}
				}
				// Devuelve el alias
				return alias;
		}
		set { _alias = value; }
	}

	/// <summary>
	///		Agregación
	/// </summary>
	internal ExpressionColumnRequestModel.AggregationType Aggregation { get; }

	/// <summary>
	///		Ordenación
	/// </summary>
	internal BaseColumnRequestModel.SortOrder OrderBy { get; }

	/// <summary>
	///		Indica si la columna es visible en la consulta
	/// </summary>
	internal bool Visible { get; }

	/// <summary>
	///		Filtros de la cláusula WHERE
	/// </summary>
	internal List<QueryFilterModel> FiltersWhere { get; } = new();

	/// <summary>
	///		Filtros de la cláusula HAVING
	/// </summary>
	internal List<QueryFilterModel> FilterHaving { get; } = new();
}
