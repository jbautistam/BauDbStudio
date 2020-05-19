using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de copia de datos
	/// </summary>
	internal class SentenceCopy : SentenceBase
	{
		/// <summary>
		///		Proveedor del que se leen los datos
		/// </summary>
		internal string Source { get; set; }

		/// <summary>
		///		Proveedor sobre el que se ejecuta la sentencia
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Comando de carga
		/// </summary>
		internal Parameters.ProviderSentenceModel LoadCommand { get; set; }

		/// <summary>
		///		Comando de grabación
		/// </summary>
		internal Parameters.ProviderSentenceModel SaveCommand { get; set; }
	}
}
