using Bau.Libraries.LibPatternText.Domain.Parsers.Source;

namespace Bau.Libraries.LibPatternText.Domain.Parsers.Formulas;

/// <summary>
///		Evaluador de identificadores
/// </summary>
internal class IdentifierParser
{
	/// <summary>
	///		Tipo de texto
	/// </summary>
	private enum TextType
	{
		/// <summary>Cadena</summary>
		String,
		/// <summary>Entero</summary>
		Int,
		/// <summary>Decimal</summary>
		Decimal,
		/// <summary>Fecha</summary>
		Date,
		/// <summary>Valor lógico</summary>
		Bool
	}

	/// <summary>
	///		Convierte el contenido de un campo con el valor leido
	/// </summary>
	internal string Convert(SourceTextReader reader, int row, string content)
	{
		(bool header, int? field, string format) = SplitIdentifierParts(content);

			if (field is null)
				throw new Exceptions.PatternTextException($"Can't parse field index at {content}. Row: {(row + 1).ToString()}");
			else if (field > reader.RecordValues.Count)
				throw new Exceptions.PatternTextException($"Can't find the value '{content}'. Row: {(row + 1).ToString()}");
			else if (field < 0 && reader.Columns.Count - (field ?? 0) < 0)
				throw new Exceptions.PatternTextException($"Can't find a value with inndex '{content}'. Row: {(row + 1).ToString()}");
			else
			{
				if (header)
				{
					if (field >= 0)
						return ApplyFormat(reader.Columns[(field ?? 0) - 1], format);
					else
						return ApplyFormat(reader.Columns[reader.Columns.Count + (field ?? 0)], format);
				}
				else 
				{
					if (field == 0)
						return ApplyFormat(row.ToString(), format);
					else if (field >= 0)
						return ApplyFormat(reader.RecordValues[(field ?? 0) - 1], format);
					else
						return ApplyFormat(reader.RecordValues[reader.Columns.Count + (field ?? 0)], format);
				}
			}
	}

	/// <summary>
	///		Aplica un formato a un valor
	/// </summary>
	private string ApplyFormat(string value, string identifierFormat)
	{
		// Aplica el formato
		if (!string.IsNullOrWhiteSpace(identifierFormat))
		{
			(TextType type, string format) = ExtractFormat(identifierFormat);

				// Formatea el valor
				value = Format(ConvertValue(value, type), format);
		}
		// Devuelve el valor formateado
		return value;
	}

	/// <summary>
	///		Formatea un valor
	/// </summary>
	private string Format(object? value, string format)
	{
		if (format.Equals("sql", StringComparison.CurrentCultureIgnoreCase))
			return FormatSql(value);
		else if (value is null)
			return "null";
		else
			return string.Format("{0:" + format + "}", value);
	}

	/// <summary>
	///		Formatea como SQL
	/// </summary>
	private string FormatSql(object? value)
	{
		return value switch
				{
					null => "NULL",
					int result => result.ToString(System.Globalization.CultureInfo.InvariantCulture),
					double result => result.ToString(System.Globalization.CultureInfo.InvariantCulture),
					DateTime result => $"{result:yyyy-MM-dd HH:mm:ss}",
					bool result => result ? "1" : "0",
					_ => (value?.ToString() ?? string.Empty).Replace("'", "''")
				};
	}

	/// <summary>
	///		Convierte el valor al tipo
	/// </summary>
	private object? ConvertValue(string value, TextType type)
	{
		if (string.IsNullOrWhiteSpace(value))
			return null;
		else
			return type switch
					{
						TextType.Bool => GetBool(value),
						TextType.Int => GetInt(value),
						TextType.Decimal => GetDecimal(value),
						TextType.Date => GetDateTime(value),
						_ => value
					};
	}

	/// <summary>
	///		Obtiene un valor lógico
	/// </summary>
	private bool GetBool(string value) => !string.IsNullOrWhiteSpace(value) && (value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("1"));

	/// <summary>
	///		Obtiene un valor entero
	/// </summary>
	private int? GetInt(string value)
	{
		if (int.TryParse(value, out int result))
			return result;
		else
			return null;
	}

	/// <summary>
	///		Obtiene un valor decimal
	/// </summary>
	private double? GetDecimal(string value)
	{
		if (double.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out double result))
			return result;
		else
			return null;
	}

	/// <summary>
	///		Obtiene un valor decimal
	/// </summary>
	private DateTime? GetDateTime(string value)
	{
		if (DateTime.TryParse(value, out DateTime result))
			return result;
		else
			return null;
	}

	/// <summary>
	///		Extrae el formato del identificador
	/// </summary>
	private (TextType type, string format) ExtractFormat(string identifierFormat)
	{
		string[] parts = identifierFormat.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		TextType type = TextType.String;
		string format;

			// Obtiene el tipo y formato
			if (parts.Length == 1)
				format = parts[0];
			else
			{
				type = GetType(parts[0]);
				format = parts[1];
			}
			// Devuelve las partes
			return (type, format);
	}

	/// <summary>
	///		Obtiene el tipo
	/// </summary>
	private TextType GetType(string typeText)
	{
		if (!Enum.TryParse(typeText, true, out TextType type))
			return TextType.String;
		else
			return type;
	}

	/// <summary>
	///		Extrae las secciones del identificador
	/// </summary>
	private (bool header, int? index, string format) SplitIdentifierParts(string value)
	{
		bool header = false;
		int? index = null;
		string format = string.Empty;

			// Obtiene el índice
			if (!string.IsNullOrWhiteSpace(value))
			{
				// Quita los espacios
				value = value.Trim();
				// Quita el separador
				if (value.StartsWith('$'))
					value = value.Substring(1);
				// Quita el formato
				if (value.Contains('['))
				{
					int start = value.IndexOf('[');

						// Obtiene el formato
						format = value.Substring(start + 1);
						if (format.EndsWith(']'))
							format = format.Substring(0, format.Length - 1);
						// Quita el formato del valor
						value = value.Substring(0, start);
				}
				// Quita el valor que indica si es cabecera
				if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("H", StringComparison.CurrentCultureIgnoreCase))
				{
					header = true;
					value = value.Substring(1);
				}
				// Obtiene el valor numérico
				if (int.TryParse(value, out int result))
					index = result;
			}
			// Si ha llegado hasta aquí es porque no ha podido convertir el índice
			return (header, index, format);
	}
}
