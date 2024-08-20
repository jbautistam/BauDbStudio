using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Movimiento con los datos de una jugada en un turno
	/// </summary>
	internal class SentenceTurnPlayModel : SentenceBaseModel
	{
		internal SentenceTurnPlayModel(string Sentence): base(Sentence) {}
	}
}
