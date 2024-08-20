namespace Bau.Libraries.LibPatternText.Domain.Parsers.Formulas;

/// <summary>
///		Clase con los datos de un token
/// </summary>
internal class TokenModel
{
	/// <summary>
	///		Tipos de token
	/// </summary>
	internal enum TokenType
	{
		Word,
		Field,
		Separator,
		Dolar
	}

	internal TokenModel(TokenType type, string content)
	{
		Type = type;
		Content = content;
	}

	/// <summary>
	///		Tipo de token
	/// </summary>
	internal TokenType Type { get; }

	/// <summary>
	///		Contenido
	/// </summary>
	internal string Content { get; }
}
