﻿using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Tools;

/// <summary>
///		Generador de SQL para filtros
/// </summary>
internal class SqlFilterGenerator
{
	internal SqlFilterGenerator(SqlTools sqlTools)
	{
		SqlTools = sqlTools;
	}

	/// <summary>
	///		Obtiene la SQL de los filtros para un <see cref="List{FilterRequestModel}"/>
	/// </summary>
	internal string GetSql(string table, string column, List<FilterRequestModel> filters) => GetSql(table, column, string.Empty, filters);

	/// <summary>
	///		Obtiene la SQL de los filtros para un <see cref="List{FilterRequestModel}"/>
	/// </summary>
	internal string GetSql(string table, string column, string aggregation, List<FilterRequestModel> filters)
	{
		string sql = string.Empty;

			// Genera la SQL de los filtros
			foreach (FilterRequestModel filter in filters)
				sql = sql.AddWithSeparator(GetSql(table, column, aggregation, filter), "AND");
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para un <see cref="FilterRequestModel"/>
	/// </summary>
	internal string GetSql(string table, string column, FilterRequestModel filter)  => GetSql(table, column, string.Empty, filter);

	/// <summary>
	///		Obtiene la cadena SQL de un <see cref="FilterRequestModel"/>
	/// </summary>
	internal string GetSql(string table, string column, string aggregation, FilterRequestModel filter)
	{
		return $"{GetFieldNameWithAggregation(table, column, aggregation)} {GetCondition(filter.Condition)} {GetValues(filter.Condition, filter.Values)}";
	}

	/// <summary>
	///		Obtiene el nombre de tabla / campo incluyendo la función de agregación si es necesario
	/// </summary>
	private string GetFieldNameWithAggregation(string table, string column, string aggregation)
	{
		string field = SqlTools.GetFieldName(table, column);

			// Añade la función de agregación si es necesario
			if (!string.IsNullOrWhiteSpace(aggregation))
				field = $"{aggregation}({field})";
			// Devuelve el nombre del campo
			return field;
	}

	/// <summary>
	///		Obtiene la cadena SQL de la condición
	/// </summary>
	private string GetCondition(FilterRequestModel.ConditionType condition)
	{
		return condition switch
					{
						FilterRequestModel.ConditionType.Equals => "=",
						FilterRequestModel.ConditionType.Less => "<",
						FilterRequestModel.ConditionType.Greater => ">",
						FilterRequestModel.ConditionType.LessOrEqual => "<=",
						FilterRequestModel.ConditionType.GreaterOrEqual => ">=",
						FilterRequestModel.ConditionType.Contains => "LIKE",
						FilterRequestModel.ConditionType.In => "IN",
						FilterRequestModel.ConditionType.Between => "BETWEEN",
						_ => throw new LibReporting.Models.Exceptions.ReportingException($"Condition unknown: {condition.ToString()}")
					};
	}

	/// <summary>
	///		Obtiene la cadena SQL de los valores
	/// </summary>
	private string GetValues(FilterRequestModel.ConditionType condition, List<object?> values)
	{
		if (values.Count < 1)
			throw new LibReporting.Models.Exceptions.ReportingException("Not defined values for filter");
		else
			return condition switch
						{
							FilterRequestModel.ConditionType.Between => GetValuesBetween(values),
							FilterRequestModel.ConditionType.In => GetValuesIn(values),
							FilterRequestModel.ConditionType.Contains => GetValueLike(values[0]),
							_ => GetValue(values[0])
						};
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición BETWEEN
	/// </summary>
	private string GetValuesBetween(List<object?> values)
	{
		if (values.Count < 2)
			throw new LibReporting.Models.Exceptions.ReportingException("Not enough values defined for filter BETWEEN");
		else
			return $"{GetValue(values[0])} AND {GetValue(values[1])}";
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición IN
	/// </summary>
	private string GetValuesIn(List<object?> values)
	{
		string sql = string.Empty;

			// Concatena los valores
			foreach (object? value in values)
				sql = sql.AddWithSeparator(GetValue(value), ",");
			// Devuelve la cadena
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición LIKE
	/// </summary>
	private string GetValueLike(object? value) => $"'%{GetValue(value, false)}%'";

	/// <summary>
	///		Obtiene la cadena SQL para un valor
	/// </summary>
	private string GetValue(object? value, bool withApostrophe = true)
	{
		return value switch
				{
					null => "NULL",
					byte valueInteger => ConvertIntToSql(valueInteger),
					int valueInteger => ConvertIntToSql(valueInteger),
					short valueInteger => ConvertIntToSql(valueInteger),
					long valueInteger => ConvertIntToSql(valueInteger),
					double valueDecimal => ConvertDecimalToSql(valueDecimal),
					float valueDecimal => ConvertDecimalToSql(valueDecimal),
					decimal valueDecimal => ConvertDecimalToSql((double) valueDecimal),
					string valueString => ConvertStringToSql(valueString, withApostrophe),
					DateTime valueDate => ConvertDateToSql(valueDate),
					bool valueBool => ConvertBooleanToSql(valueBool),
					_ => ConvertStringToSql(value?.ToString() ?? string.Empty, withApostrophe)
				};
	}

	/// <summary>
	///		Convierte un valor lógico a SQL
	/// </summary>
	private string ConvertBooleanToSql(bool value)
	{
		if (value)
			return "1";
		else
			return "0";
	}

	/// <summary>
	///		Convierte una fecha a SQL
	/// </summary>
	private string ConvertDateToSql(DateTime valueDate) => $"'{valueDate:yyyy-MM-dd}'";

	/// <summary>
	///		Convierte un valor decimal a Sql
	/// </summary>
	private string ConvertDecimalToSql(double value) => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

	/// <summary>
	///		Convierte un entero en una cadena
	/// </summary>
	private string ConvertIntToSql(long value) => value.ToString();

	/// <summary>
	///		Convierte una cadena a SQL
	/// </summary>
	private string ConvertStringToSql(string value, bool withApostrophe)
	{
		if (withApostrophe)
			return "'" + value.Replace("'", "''") + "'";
		else
			return value.Replace("'", "''");
	}

	/// <summary>
	///		Herramientas de generación de SQL
	/// </summary>
	internal SqlTools SqlTools { get; }
}