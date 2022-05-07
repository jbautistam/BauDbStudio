using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Movimiento que indica un error
	/// </summary>
	internal class SentenceErrorModel : SentenceBaseModel
	{
		internal SentenceErrorModel(string error) : base(error) {}
	}
}
