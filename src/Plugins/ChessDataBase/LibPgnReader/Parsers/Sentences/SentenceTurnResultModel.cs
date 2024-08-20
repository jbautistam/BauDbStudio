using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Movimiento con los datos de resultado de un juego
	/// </summary>
	internal class SentenceTurnResultModel : SentenceBaseModel
	{
		internal SentenceTurnResultModel(string result): base(result) {}
	}
}
