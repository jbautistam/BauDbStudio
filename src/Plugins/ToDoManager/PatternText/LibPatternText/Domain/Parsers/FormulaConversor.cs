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
		List<Formulas.TokenModel> tokens = new Formulas.FormulaTextParser(Pattern.Formula).Parse().ToList();
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
		int? field = GetFieldIndex(content);

			if (field is null)
				throw new Exceptions.PatternTextException($"Can't parse field index at {content}");
			else if (field >= 0)
				return reader.RecordValues[(field ?? 0) - 1];
			else
				return reader.RecordValues[reader.Columns.Count + (field ?? 0)];
	}

	/// <summary>
	///		Obtiene el índice del campo
	/// </summary>
	private int? GetFieldIndex(string value)
	{
		// Obtiene el índice
		if (!string.IsNullOrWhiteSpace(value))
		{
			// Quita los espacios
			value = value.Trim();
			// Quita el separador
			if (value.StartsWith('$'))
				value = value.Substring(1);
			// Obtiene el valor numérico
			if (int.TryParse(value, out int index))
				return index;
		}
		// Si ha llegado hasta aquí es porque no ha podido convertir el índice
		return null;
	}

	/// <summary>
	///		Patrón
	/// </summary>
	internal PatternModel Pattern { get; }
}