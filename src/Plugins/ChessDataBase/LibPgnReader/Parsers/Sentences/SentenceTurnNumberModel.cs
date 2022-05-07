using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Movimiento con los datos de un turno
	/// </summary>
	internal class SentenceTurnNumberModel : SentenceBaseModel
	{
		internal SentenceTurnNumberModel(string number): base(number) {}
	}
}
