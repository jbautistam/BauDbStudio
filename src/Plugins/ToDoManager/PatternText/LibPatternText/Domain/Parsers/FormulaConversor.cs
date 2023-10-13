using Bau.Libraries.LibPatternText.Domain.Parsers.Source;
using Bau.Libraries.LibPatternText.Models;

namespace Bau.Libraries.LibPatternText.Domain.Parsers;

/// <summary>
///		Conversor de fórmula
/// </summary>
internal class FormulaConversor
{
	// Variables privadas
	private Formulas.IdentifierParser _identifierParser = new();

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
			// Quita los saltos de línea del final
			if (!string.IsNullOrWhiteSpace(result))
				result = result.TrimEnd();
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
				result += Join(bottoms);
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

			if (tokens.Count == 0)
				throw new Exceptions.PatternTextException("Can't find any token at formula");
			else
			{
				System.Text.StringBuilder builder = new();
				int row = 1;

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
											builder.Append(_identifierParser.Convert(reader, row, token.Content));
										break;
								}
							// Añade un salto de línea
							builder.AppendLine();
							// Incrementa el número de línea
							row++;
						}
				}
				// Devuelve los datos generados
				return builder.ToString();
			}
	}

	/// <summary>
	///		Patrón
	/// </summary>
	internal PatternModel Pattern { get; }
}