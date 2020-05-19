using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un bucle por cada registro leido del proveedor
	/// </summary>
	internal class SentenceForEach : SentenceBase
	{
		/// <summary>
		///		Proveedor del que se leen los datos
		/// </summary>
		internal string Source { get; set; }

		/// <summary>
		///		Comando a ejecutar
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; set; }

		/// <summary>
		///		Instrucciones que se deben ejecutar cuando se encuentra algún dato
		/// </summary>
		internal SentenceCollection SentencesWithData { get; } = new SentenceCollection();

		/// <summary>
		///		Instrucciones que se deben ejecutar cuando no se encuentra ningún dato
		/// </summary>
		internal SentenceCollection SentencesEmptyData { get; } = new SentenceCollection();
	}
}
