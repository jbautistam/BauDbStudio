using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Bloque de sentencias
	/// </summary>
	internal class SentenceBlock : SentenceBase
	{
		/// <summary>
		///		Nombre del bloque
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Sentencias del bloque
		/// </summary>
		internal SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
