namespace Bau.Libraries.LibPatternText.Domain.Parsers.Formulas;

/// <summary>
///		Parser de fórmulas
/// </summary>
internal class FormulaTextParser
{
	// Variables privadas
	private int _actualChar;

	internal FormulaTextParser(Models.PatternFormulaModel formula)
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
		if (string.IsNullOrWhiteSpace(Formula.Text) || _actualChar >= Formula.Text.Length)
			return null;
		else
		{
			TokenModel.TokenType type = GetTokenType();
		
				return type switch
					{
						TokenModel.TokenType.Field => new TokenModel(type, GetIdentifier()),
						TokenModel.TokenType.Separator => new TokenModel(type, GetSeparator()),
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
					return TokenModel.TokenType.Word;
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
	private string GetIdentifier() => GetContentTo(IsIdentifierSeparator);

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
			while (_actualChar < Formula.Text.Length && !separatorFunction())
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
	///		Comprueba si el carácter es un separador de un identificador
	/// </summary>
	private bool IsIdentifierSeparator() => IsSpace() || CharIn(',', '.', '(', ')', '\'', '"');

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
	///		Obtiene el carácter
	/// </summary>
	private char GetChar() => Formula.Text[_actualChar];

	/// <summary>
	///		Obtiene el siguiente carácter
	/// </summary>
	private char GetNextChar()
	{
		if (_actualChar + 1 < Formula.Text.Length)
			return Formula.Text[_actualChar + 1];
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
	internal Models.PatternFormulaModel Formula { get; }
}
