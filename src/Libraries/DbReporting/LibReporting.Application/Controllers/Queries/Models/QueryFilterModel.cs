using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Datos de un filtro para la consulta
/// </summary>
internal class QueryFilterModel
{
	internal QueryFilterModel(FilterRequestModel.ConditionType condition, List<object?> values)
	{
		Condition = condition;
		Values = values;
	}

	/// <summary>
	///		Obtiene el SQL de la condición
	/// </summary>
	internal string GetSql(string tableAlias, string field)
	{
		return $"[{tableAlias}].[{field}] {GetCondition()} {GetValues()}";
	}

	/// <summary>
	///		Obtiene el SQL de la condición
	/// </summary>
	internal string GetSql(string aggregatedField) => $"{aggregatedField} {GetCondition()} {GetValues()}";

	/// <summary>
	///		Obtiene la cadena SQL de la condición
	/// </summary>
	private string GetCondition()
	{
		switch (Condition)
		{
			case FilterRequestModel.ConditionType.Equals:
				return "=";
			case FilterRequestModel.ConditionType.Less:
				return "<";
			case FilterRequestModel.ConditionType.Greater:
				return ">";
			case FilterRequestModel.ConditionType.LessOrEqual:
				return "<=";
			case FilterRequestModel.ConditionType.GreaterOrEqual:
				return ">=";
			case FilterRequestModel.ConditionType.Contains:
				return "LIKE";
			case FilterRequestModel.ConditionType.In:
				return "IN";
			case FilterRequestModel.ConditionType.Between:
				return "BETWEEN";
			default:
				throw new LibReporting.Models.Exceptions.ReportingException($"Condition unknown: {Condition.ToString()}");
		}
	}

	/// <summary>
	///		Obtiene la cadena SQL de los valores
	/// </summary>
	private string GetValues()
	{
		if (Values.Count < 1)
			throw new LibReporting.Models.Exceptions.ReportingException("Not defined values for filter");
		else
			switch (Condition)
			{
				case FilterRequestModel.ConditionType.Between:
						return GetValuesBetween();
				case FilterRequestModel.ConditionType.In:
					return GetValuesIn();
				case FilterRequestModel.ConditionType.Contains:
					return GetValueLike();
				default:
					return GetValue(Values[0]);
			}
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición BETWEEN
	/// </summary>
	private string GetValuesBetween()
	{
		if (Values.Count < 2)
			throw new LibReporting.Models.Exceptions.ReportingException("Not enough values defined for filter BETWEEN");
		else
			return $"{GetValue(Values[0])} AND {GetValue(Values[1])}";
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición IN
	/// </summary>
	private string GetValuesIn()
	{
		string sql = string.Empty;

			// Concatena los valores
			foreach (object value in Values)
				sql = sql.AddWithSeparator(GetValue(value), ",");
			// Devuelve la cadena
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición LIKE
	/// </summary>
	private string GetValueLike() => $"'%{GetValue(Values[0], false)}%'";

	/// <summary>
	///		Obtiene la cadena SQL para un valor
	/// </summary>
	private string GetValue(object value, bool withApostrophe = true)
	{
		switch (value)
		{
			case null:
				return "NULL";
			case int valueInteger:
				return ConvertIntToSql(valueInteger);
			case short valueInteger:
				return ConvertIntToSql(valueInteger);
			case long valueInteger:
				return ConvertIntToSql(valueInteger);
			case double valueDecimal:
				return ConvertDecimalToSql(valueDecimal);
			case float valueDecimal:
				return ConvertDecimalToSql(valueDecimal);
			case decimal valueDecimal:
				return ConvertDecimalToSql((double) valueDecimal);
			case string valueString:
				return ConvertStringToSql(valueString, withApostrophe);
			case DateTime valueDate:
				return ConvertDateToSql(valueDate);
			case bool valueBool:
				return ConvertBooleanToSql(valueBool);
			default:
				return ConvertStringToSql(value?.ToString() ?? string.Empty, withApostrophe);
		}
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
	///		Condición
	/// </summary>
	internal FilterRequestModel.ConditionType Condition { get; }

	/// <summary>
	///		Valores del filtro
	/// </summary>
	internal List<object?> Values { get; }
}