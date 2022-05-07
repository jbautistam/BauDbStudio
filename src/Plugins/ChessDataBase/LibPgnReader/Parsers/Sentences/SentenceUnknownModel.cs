using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Movimiento desconocido: normalmente un error de lectura
	/// </summary>
	internal class SentenceUnknownModel : SentenceBaseModel
	{
		internal SentenceUnknownModel(Tokens.TokenModel token) : base($"{token.Type.ToString()} - {token.Content}") {}
	}
}
