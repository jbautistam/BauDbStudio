using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase con los datos para una relación entre tablas
/// </summary>
internal class QueryFieldsGenerator
{
	internal QueryFieldsGenerator(ReportQueryGenerator manager, ParserFieldsSectionModel section)
	{
		Manager = manager;
		Section = section;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal string GetSql() 
	{
		string sql = GetSqlFieldsForDimensions(Section.ParserDimensions);

			// Añade una coma si es obligatoria
			if (!string.IsNullOrWhiteSpace(sql) && Section.WithComma)
				sql += ", ";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
	/// </summary>
	private string GetSqlFieldsForDimensions(List<ParserFieldsDimensionSectionModel> dimensions)
	{
		string sql = string.Empty;

			// Obtiene los campos
			foreach (ParserFieldsDimensionSectionModel fieldDimension in dimensions)
				if (Manager.RequestController.CheckIfDimensionRequest(fieldDimension.DimensionKey))
				{
					QueryTableModelNew? table = Manager.RequestController.GetRequestedTable(fieldDimension.Table, fieldDimension.Table, fieldDimension.DimensionKey,
																							fieldDimension.WithPrimaryKeys, fieldDimension.WithRequestedFields);

						if (table is not null)
							sql = sql.AddWithSeparator(GetSqlFields(fieldDimension, table), ",");
				}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de los campos
	/// </summary>
	private string GetSqlFields(ParserFieldsDimensionSectionModel fielDimension, QueryTableModelNew table)
	{
		string sql = string.Empty;

			// Genera la SQL para las columnas de la tabla
			foreach (QueryTableColumnModelNew column in table.Columns)
				if (string.IsNullOrEmpty(fielDimension.AdditionalTable))
					sql = sql.AddWithSeparator(column.GetFieldName(), ",");
				else
					sql = sql.AddWithSeparator($"IsNull({column.GetFieldName(fielDimension.AdditionalTable)}, {column.GetFieldName()}) AS {column.NameParts.Alias}", ",");
			// Devuelve la SQL
			return sql;
	}

	/// <summary>
	///		Manager
	/// </summary>
	internal ReportQueryGenerator Manager { get; }
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserFieldsSectionModel Section { get; }
}
