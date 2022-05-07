using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Sentencia que indica una línea vacía
	/// </summary>
	internal class SentenceEmptyLine : SentenceBaseModel
	{
		internal SentenceEmptyLine() : base("EmptyLine") {}
	}
}
