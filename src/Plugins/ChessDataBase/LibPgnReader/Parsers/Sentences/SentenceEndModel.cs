using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Movimiento que indica el final del archivo
	/// </summary>
	internal class SentenceEndModel : SentenceBaseModel
	{
		internal SentenceEndModel() : base("End") {}
	}
}
