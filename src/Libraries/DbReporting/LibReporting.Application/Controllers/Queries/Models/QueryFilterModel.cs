using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Datos de un filtro para la consulta
/// </summary>
internal class QueryFilterModel
{
	internal QueryFilterModel(QueryModel query, FilterRequestModel.ConditionType condition, List<object?> values)
	{
		Query = query;
		Condition = condition;
		Values = values;
	}

	/// <summary>
	///		Obtiene el SQL de la condición
	/// </summary>
	internal string GetSql(string tableAlias, string field) => $"{Query.Generator.GetFieldName(tableAlias,field)} {GetCondition()} {GetValues()}";

	/// <summary>
	///		Obtiene el SQL de la condición
	/// </summary>
	internal string GetSql(string aggregatedField) => $"{aggregatedField} {GetCondition()} {GetValues()}";

	/// <summary>
	///		Obtiene la cadena SQL de la condición
	/// </summary>
	private string GetCondition()
	{
		return Condition switch
					{
						FilterRequestModel.ConditionType.Equals => "=",
						FilterRequestModel.ConditionType.Less => "<",
						FilterRequestModel.ConditionType.Greater => ">",
						FilterRequestModel.ConditionType.LessOrEqual => "<=",
						FilterRequestModel.ConditionType.GreaterOrEqual => ">=",
						FilterRequestModel.ConditionType.Contains => "LIKE",
						FilterRequestModel.ConditionType.In => "IN",
						FilterRequestModel.ConditionType.Between => "BETWEEN",
						_ => throw new LibReporting.Models.Exceptions.ReportingException($"Condition unknown: {Condition.ToString()}")
					};
	}

	/// <summary>
	///		Obtiene la cadena SQL de los valores
	/// </summary>
	private string GetValues()
	{
		if (Values.Count < 1)
			throw new LibReporting.Models.Exceptions.ReportingException("Not defined values for filter");
		else
			return Condition switch
						{
							FilterRequestModel.ConditionType.Between => GetValuesBetween(),
							FilterRequestModel.ConditionType.In => GetValuesIn(),
							FilterRequestModel.ConditionType.Contains => GetValueLike(),
							_ => GetValue(Values[0])
						};
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
			foreach (object? value in Values)
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
	private string GetValue(object? value, bool withApostrophe = true)
	{
		return value switch
				{
					null => "NULL",
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
	///		Consulta
	/// </summary>
	internal QueryModel Query { get; }

	/// <summary>
	///		Condición
	/// </summary>
	internal FilterRequestModel.ConditionType Condition { get; }

	/// <summary>
	///		Valores del filtro
	/// </summary>
	internal List<object?> Values { get; }
}