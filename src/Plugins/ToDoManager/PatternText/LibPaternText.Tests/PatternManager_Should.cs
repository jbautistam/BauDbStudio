using Bau.Libraries.LibPatternText.Models;

namespace Bau.Libraries.LibPatternText.Tests;

/// <summary>
///		Pruebas de <see cref="PatternManager"/>
/// </summary>
public class PatternManager_Should
{
	/// <summary>
	///		Comprueba la conversión de textos
	/// </summary>
	[Theory]
	[InlineData(@"Header1, Header2, Header3
				  Row1-1, Row1-2, Row1-3
				  Row2-1, Row2-2, Row2-3
				  Row3-1, Row3-2, Row3-3",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ('$1', '$2');",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row1-1', 'Row1-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row2-1', 'Row2-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row3-1', 'Row3-2');")]
	public void parse_text(string source, string formula, string result)
	{
		string converted = new PatternManager().Convert(new PatternSourceModel(source), new PatternFormulaModel(formula));

			// Comprueba los resultados
			Normalize(converted).Should().Be(Normalize(result));
	}

	/// <summary>
	///		Normaliza una cadena quitándole saltos de línea, tabuladores y espacios
	/// </summary>
	private string Normalize(string value)
	{
		// Normaliza la cadena
		if (!string.IsNullOrEmpty(value))
		{
			// Quita los espacios
			value = value.Trim();
			// Quita los saltos de línea y tabuladores
			value = value.Replace('\t', ' ');
			value = value.Replace('\r', ' ');
			value = value.Replace('\n', ' ');
			// Quita los espacios dobles
			while (value.IndexOf("  ") >= 0)
				value = value.Replace("  ", " ");
		}
		// Devuelve el valor
		return value;
	}
}