namespace Bau.Libraries.LibPatternText.Domain.Parsers.Formulas;

/// <summary>
///		Parser de fórmulas
/// </summary>
internal class FormulaTextParser
{
	// Variables privadas
	private int _actualChar, _bracketsOpened;

	internal FormulaTextParser(string formula)
	{
		Formula = formula;
	}

	/// <summary>
	///		Interpreta la fórmula
	/// </summary>
	internal IEnumerable<TokenModel> Parse()
	{
		TokenModel? token = ReadToken();

			while (token is not null)
			{
				// Devuelve el token leido
				yield return token;
				// Obtiene el siguiente token
				token = ReadToken();
			}
	}

	/// <summary>
	///		Lee el token
	/// </summary>
	private TokenModel? ReadToken()
	{
		if (string.IsNullOrWhiteSpace(Formula) || _actualChar >= Formula.Length)
			return null;
		else
		{
			TokenModel.TokenType type = GetTokenType();
		
				return type switch
						{
							TokenModel.TokenType.Field => new TokenModel(type, GetIdentifier()),
							TokenModel.TokenType.Separator => new TokenModel(type, GetSeparator()),
							TokenModel.TokenType.Dolar => new TokenModel(type, GetContentScape()),
							_ => new TokenModel(type, GetContent())
						};
		}
	}

	/// <summary>
	///		Obtiene el tipo de token
	/// </summary>
	private TokenModel.TokenType GetTokenType()
	{
		switch (GetChar())
		{
			case '$':
				if (GetNextChar() == '$')
					return TokenModel.TokenType.Dolar;
				else
					return TokenModel.TokenType.Field;
			case ' ':
			case '\t':
			case '\r':
			case '\n':
				return TokenModel.TokenType.Separator;
			default:
				return TokenModel.TokenType.Word;
		}
	}

	/// <summary>
	///		Obtiene un identificador
	/// </summary>
	private string GetIdentifier()
	{
		string content = string.Empty;
		bool mustContinue = true;

			// Mientras que quede algo por leer del identificador
			while (_actualChar < Formula.Length && mustContinue)
			{
				char character = GetChar();

					// Sólo puede haber un carácter H o -, el resto deben ser dígitos
					if (character.Equals('H') || character.Equals('h') || character.Equals('-') || character.Equals('$'))
					{
						if (!content.Contains(character, StringComparison.CurrentCultureIgnoreCase))
							content += character;
						else
							mustContinue = false;
					}
					else if (character.Equals('['))
					{
						// Obtiene el texto hasta el corchete de cierre
						content += GetContentTo(IsBracketEnd);
						// Añade el corchete de cierre
						content += ']';
						_actualChar++;
						// Indica que ya no debe continuar
						mustContinue = false;
					}
					else if (char.IsDigit(character))
						content += character;
					else
						mustContinue = false;
					// Pasa al siguiente carácter
					if (mustContinue)
						_actualChar++;
			}
			// Devuelve el contenido
			return content;
	}

	/// <summary>
	///		Obtiene un contenido
	/// </summary>
	private string GetContent() => GetContentTo(IsContentSeparator);

	/// <summary>
	///		Obtiene la cadena de separadores
	/// </summary>
	private string GetSeparator() => GetContentTo(IsLegible);

	/// <summary>
	///		Obtiene un contenido
	/// </summary>
	private string GetContentTo(Func<bool> separatorFunction)
	{
		string content = string.Empty;

			// Lee el identificador
			while (_actualChar < Formula.Length && !separatorFunction())
			{
				// Añade el carácter al identificador
				content += GetChar();
				// Incrementa el índice del carácter
				_actualChar++;
			}
			// Devuelve el identificador
			return content;
	}

	/// <summary>
	///		Obtiene el contenido quitando el dólar escapado
	/// </summary>
	private string GetContentScape()
	{
		// Incrementa el contador
		_actualChar += 2;
		// Devuelve el signo dólar
		return "$";
	}

	/// <summary>
	///		Comprueba si el carácter es un separador de contenido
	/// </summary>
	private bool IsContentSeparator() => IsSpace() || GetChar() == '$';

	/// <summary>
	///		Comprueba si el carácter es un espacio
	/// </summary>
	private bool IsSpace() => CharIn(' ', '\t', '\r', '\n');

	/// <summary>
	///		Comprueba si el carácter es un carácter interpretable (no es un espacio)
	/// </summary>
	private bool IsLegible() => !IsSpace();

	/// <summary>
	///		Comprueba si se ha llegado al corchete de cierre
	/// </summary>
	private bool IsBracketEnd()
	{
		// Ajusta el número de corchetes
		if (CharIn('['))
			_bracketsOpened++;
		else if (CharIn(']'))
			_bracketsOpened--;
		// Comprueba si se ha llegado al final
		return _bracketsOpened == 0;
	}

	/// <summary>
	///		Obtiene el carácter
	/// </summary>
	private char GetChar() => Formula[_actualChar];

	/// <summary>
	///		Obtiene el siguiente carácter
	/// </summary>
	private char GetNextChar()
	{
		if (_actualChar + 1 < Formula.Length)
			return Formula[_actualChar + 1];
		else
			return ' ';
	}

	/// <summary>
	///		Comprueba si un carácter está en una lista
	/// </summary>
	private bool CharIn(params char[] characters)
	{
		char actual = GetChar();

			// Comprueba si el carácter actual está entre los pasados como parámetro
			foreach (char character in characters)
				if (actual == character)
					return true;
			// Si ha llegado hasta aquí es porque no está en la lista
			return false;
	}

	/// <summary>
	///		Fórmula
	/// </summary>
	internal string Formula { get; }
}