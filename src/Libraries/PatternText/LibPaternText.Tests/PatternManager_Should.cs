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
	[InlineData(@"Id, CountryId, Cluster
				  1, 18, A
				  2, 17, B
				  3, 19, C",
				@"UPDATE Companies
					SET $h3 = '$3',
						$h2 = $2,
					WHERE $h1 = $1;",
				@"UPDATE Companies
					SET Cluster = 'A',
						CountryId = 18,
					WHERE Id = 1;
				  UPDATE Companies
					SET Cluster = 'B',
						CountryId = 17,
					WHERE Id = 2;
				  UPDATE Companies
					SET Cluster = 'C',
						CountryId = 19,
					WHERE Id = 3;
				")]
	[InlineData(@"Id, CountryId, Cluster
				  1, 18, A
				  2, 17, B
				  3, 19, C",
				@"UPDATE Companies
					SET $h-1 = '$-1',
						$h-2 = $-2,
					WHERE $h1 = $1;",
				@"UPDATE Companies
					SET Cluster = 'A',
						CountryId = 18,
					WHERE Id = 1;
				  UPDATE Companies
					SET Cluster = 'B',
						CountryId = 17,
					WHERE Id = 2;
				  UPDATE Companies
					SET Cluster = 'C',
						CountryId = 19,
					WHERE Id = 3;
				")]
	[InlineData(@"Header1, Header2, Header3
				  Row1-1, Row1-2, Row1-3
				  Row2-1, Row2-2, Row2-3
				  Row3-1, Row3-2, Row3-3",
				@"+top -- Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$1', '$2');",
				@"-- Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row1-1', 'Row1-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row2-1', 'Row2-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row3-1', 'Row3-2');")]
	[InlineData(@"Header1, Header2, Header3
				  Row1-1, Row1-2, Row1-3
				  Row2-1, Row2-2, Row2-3
				  Row3-1, Row3-2, Row3-3",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ('$1', '$2');
				  +bottom -- Esta es la línea inferior",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row1-1', 'Row1-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row2-1', 'Row2-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row3-1', 'Row3-2');
				  -- Esta es la línea inferior")]
	[InlineData(@"Header1, Header2, Header3
				  Row1-1, Row1-2, Row1-3
				  Row2-1, Row2-2, Row2-3
				  Row3-1, Row3-2, Row3-3",
				@"+top --Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$1', '$2');
				  +bottom -- Esta es la línea inferior",
				@"--Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row1-1', 'Row1-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row2-1', 'Row2-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Row3-1', 'Row3-2');
				  -- Esta es la línea inferior")]
	[InlineData(@"Header1, Header2, Header3
				  Row1-1, Row1-2, Row1-3",
				@"+top --Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$$', '$2');
				  +bottom -- Esta es la línea inferior",
				@"--Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$', 'Row1-2');
				  -- Esta es la línea inferior")]
	[InlineData(@"Header1, Header2, Header3
				  Row1-1, Row1-2, Row1-3
				  Row2-1, Row2-2, Row2-3",
				@"+top --Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$$$3', '$2');
				  +bottom -- Esta es la línea inferior",
				@"--Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$Row1-3', 'Row1-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$Row2-3', 'Row2-2');
				  -- Esta es la línea inferior")]
	[InlineData(@"Header1, Header2, Header3
				  Row1-1, Row1-2, Row1-3
				  Row2-1, Row2-2, Row2-3",
				@"+top --Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$$$3$$$1', '$2');
				  +bottom -- Esta es la línea inferior",
				@"--Esta es la línea superior
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$Row1-3$Row1-1', 'Row1-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('$Row2-3$Row2-1', 'Row2-2');
				  -- Esta es la línea inferior")]
	[InlineData(@"FirstName, LastName
				  Ana, Rodríguez
				  Pablo, Fernández",
				@"INSERT INTO Customer ($h1, $h2)
					VALUES ('$1', '$2');",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ('Ana', 'Rodríguez');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ('Pablo', 'Fernández');
				 ")]
	[InlineData(@"FirstName, LastName
				  Ana, Rodríguez
				  Pablo, Fernández",
				@"INSERT INTO Customer (FullName)
					VALUES ('$2, $1');",
				@"INSERT INTO Customer (FullName)
					VALUES ('Rodríguez, Ana');
				  INSERT INTO Customer (FullName)
					VALUES ('Fernández, Pablo');
				 ")]
	[InlineData(@"FirstName, LastName
				  Ana, Rodríguez
				  Pablo, Fernández",
				@"INSERT INTO Customer (Id, FullName)
					VALUES ($0, '$2, $1');",
				@"INSERT INTO Customer (Id, FullName)
					VALUES (1, 'Rodríguez, Ana');
				  INSERT INTO Customer (Id, FullName)
					VALUES (2, 'Fernández, Pablo');
				 ")]
	public void parse_text(string source, string formula, string result)
	{
		string converted = new PatternManager().Convert(GetPatternModel(source, formula));

			// Comprueba los resultados
			Normalize(converted).Should().Be(Normalize(result));
	}

	/// <summary>
	///		Comprueba la conversión de textos
	/// </summary>
	[Theory]
	[InlineData(@"Header1, Header2, Header3
				  1, O'Donnel, Row1-3
				  2, Row2-2, Row2-3
				  3, Row3-2, Row3-3",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ($1[int,00], '$2');",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES (01, 'O'Donnel');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES (02, 'Row2-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES (03, 'Row3-2');")]
	[InlineData(@"Header1, Header2, Header3
				  1, O'Donnel, Row1-3
				  2, Row2-2, Row2-3
				  3, Row3-2, Row3-3",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ($1[int,[00]], '$2');",
				@"INSERT INTO Customer (FirstName, LastName)
					VALUES ([01], 'O'Donnel');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ([02], 'Row2-2');
				  INSERT INTO Customer (FirstName, LastName)
					VALUES ([03], 'Row3-2');")]
	[InlineData(@"FirstName, LastName, Other
				  1, O'Donnel, 1.8
				  2, Row2-2, 2.9
				  3, Row3-2, 3.4",
				@"INSERT INTO Customer ($h1, $h2, $h3)
					VALUES ($1[int,sql], '$2[string,sql]', $3[decimal,sql]);",
				@"INSERT INTO Customer (FirstName, LastName, Other)
					VALUES (1, 'O''Donnel', 1.8);
				  INSERT INTO Customer (FirstName, LastName, Other)
					VALUES (2, 'Row2-2', 2.9);
				  INSERT INTO Customer (FirstName, LastName, Other)
					VALUES (3, 'Row3-2', 3.4);")]
	[InlineData(@"FirstName, LastName, Decimal, Date
				  O'Donnel, Skip last, , 2023-02-10 18:30:20
				  Row2-2, Skip last, 2.9, 2023-02-11 19:30:20
				  Row3-2, Skip last, 3.4, 2023-02-12 20:30:20",
				@"INSERT INTO Customer (Id, $h1, $h3, $h4)
					VALUES ($0[int,sql], '$1[string,sql]', $3[decimal,sql], '$4[date,sql]');",
				@"INSERT INTO Customer (Id, FirstName, Decimal, Date)
					VALUES (1, 'O''Donnel', NULL, '2023-02-10 18:30:20');
				  INSERT INTO Customer (Id, FirstName, Decimal, Date)
					VALUES (2, 'Row2-2', 2.9, '2023-02-11 19:30:20');
				  INSERT INTO Customer (Id, FirstName, Decimal, Date)
					VALUES (3, 'Row3-2', 3.4, '2023-02-12 20:30:20');")]
	public void parse_text_with_formats(string source, string formula, string result)
	{
		string converted = new PatternManager().Convert(GetPatternModel(source, formula));

			// Comprueba los resultados
			Normalize(converted).Should().Be(Normalize(result));
	}

	/// <summary>
	///		Obtiene un patrón
	/// </summary>
	private PatternModel GetPatternModel(string source, string formula, bool withHeader = true, string separator = ",", string quoteChar = "\"")
	{
		return new PatternModel()
						{
							Source = source,
							Formula = formula,
							WithHeader = withHeader,
							Separator = separator,
							QuoteChar = quoteChar
						};
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