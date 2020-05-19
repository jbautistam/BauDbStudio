using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de una comprobación de datos
	/// </summary>
	internal class SentenceIfExists : SentenceBase
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
		///		Sentencias a ejecutar si existe el registro
		/// </summary>
		internal SentenceCollection SentencesThen { get; } = new SentenceCollection();

		/// <summary>
		///		Sentencias a ejecutar si no existe el registro
		/// </summary>
		internal SentenceCollection SentencesElse { get; } = new SentenceCollection();
	}
}
