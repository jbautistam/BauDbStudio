using Bau.Libraries.LibPatternText.Domain.Parsers.Source;
using Bau.Libraries.LibPatternText.Models;

namespace Bau.Libraries.LibPatternText.Domain.Parsers;

/// <summary>
///		Conversor de fórmula
/// </summary>
internal class FormulaConversor
{
	internal FormulaConversor(PatternModel pattern)
	{
		Pattern = pattern;
	}

	/// <summary>
	///		Convierte los datos
	/// </summary>
	internal string Convert()
	{
		return Join(Extract(Pattern.Formula, "+top"), 
					ConvertFormula(ExtractPrefixLines(Pattern.Formula)), 
					Extract(Pattern.Formula, "+bottom"));
	}

	/// <summary>
	///		Quita las líneas que tengan prefijos de la fórmula
	/// </summary>
	private string ExtractPrefixLines(string formula)
	{
		string result = string.Empty;

			// Convierte la fórmula
			if (!string.IsNullOrWhiteSpace(formula))
				foreach (string part in formula.Split('\r', '\n'))
					if (!string.IsNullOrWhiteSpace(part) && 
							!part.Trim().StartsWith("+top", StringComparison.CurrentCultureIgnoreCase) &&
							!part.Trim().StartsWith("+bottom", StringComparison.CurrentCultureIgnoreCase))
						result += part + Environment.NewLine;
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Une las cadenas resultantes
	/// </summary>
	private string Join(List<string> tops, string convertedFormula, List<string> bottoms)
	{
		string result = string.Empty;

			// Añade las cadenas superiores
			if (tops.Count > 0)
				result += Join(tops);
			// Añade la fórmula convertida
			if (!string.IsNullOrWhiteSpace(result))
				result += Environment.NewLine;
			result += convertedFormula;
			// Añade las cadenas inferiores
			if (bottoms.Count > 0)
			{
				// Añade un salto de línea
				if (!string.IsNullOrWhiteSpace(result))
					result += Environment.NewLine;
				// Añade las cadenas inferiores
				result += Join(bottoms);
			}
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Une una lista de cadenas
	/// </summary>
	private string Join(List<string> values)
	{
		string result = string.Empty;

			// Asigna las cadenas
			foreach (string value in values)
				result += value + Environment.NewLine;
			// Devuelve la cadena resultante
			return result;
	}

	/// <summary>
	///		Extrae las líneas que comienzan por un prefijo
	/// </summary>
	private List<string> Extract(string formula, string prefix)
	{
		List<string> parts = new();

			// Obtiene las líneas que comienzan por el prefijo
			foreach (string part in formula.Split('\r', '\n'))
				if (!string.IsNullOrWhiteSpace(part))
				{
					string value = part.Trim();

						// Si la cadena comienza con el prefijo
						if (value.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
							parts.Add(value.Substring(prefix.Length).Trim());
				}
			// Devuelve la lista de partes
			return parts;
	}

	/// <summary>
	///		Convierte una fórmula
	/// </summary>
	private string ConvertFormula(string formula)
	{
		List<Formulas.TokenModel> tokens = new Formulas.FormulaTextParser(formula).Parse().ToList();
		System.Text.StringBuilder builder = new();

			if (tokens.Count == 0)
				throw new Exceptions.PatternTextException("Can't find any token at formula");
			else
			{
				// Lee los datos y los convierte
				using (SourceTextReader reader = new(Pattern))
				{
					// Lee la cabecera
					reader.ReadHeader();
					// Interpreta el contenido
					while (reader.Read())
					{
						// Interpreta los tokens
						foreach (Formulas.TokenModel token in tokens)
							switch (token.Type)
							{
								case Formulas.TokenModel.TokenType.Separator:
								case Formulas.TokenModel.TokenType.Word:
								case Formulas.TokenModel.TokenType.Dolar:
										builder.Append(token.Content);
									break;
								case Formulas.TokenModel.TokenType.Field:
										builder.Append(ConvertField(reader, token.Content));
									break;
							}
						// Añade un salto de línea
						builder.AppendLine();
					}
				}
				// Devuelve los datos generados
				return builder.ToString();
			}
	}

	/// <summary>
	///		Convierte el contenido de un campo con el valor leido
	/// </summary>
	private string ConvertField(SourceTextReader reader, string content)
	{
		(bool header, int? field) = GetFieldIndex(content);

			if (field is null)
				throw new Exceptions.PatternTextException($"Can't parse field index at {content}");
			else if (field > reader.RecordValues.Count)
				throw new Exceptions.PatternTextException($"Can't find the value '{content}'");
			else if (field < 0 && reader.Columns.Count - (field ?? 0) < 0)
				throw new Exceptions.PatternTextException($"Can't find a value with inndex '{content}'");
			{
				if (header)
				{
					if (field >= 0)
						return reader.Columns[(field ?? 0) - 1];
					else
						return reader.Columns[reader.Columns.Count + (field ?? 0)];
				}
				else 
				{
					if (field >= 0)
						return reader.RecordValues[(field ?? 0) - 1];
					else
						return reader.RecordValues[reader.Columns.Count + (field ?? 0)];
				}
			}
	}

	/// <summary>
	///		Obtiene el índice del campo
	/// </summary>
	private (bool header, int? index) GetFieldIndex(string value)
	{
		bool header = false;
		int? index = null;

			// Obtiene el índice
			if (!string.IsNullOrWhiteSpace(value))
			{
				// Quita los espacios
				value = value.Trim();
				// Quita el separador
				if (value.StartsWith('$'))
					value = value.Substring(1);
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
			return (header, index);
	}

	/// <summary>
	///		Patrón
	/// </summary>
	internal PatternModel Pattern { get; }
}