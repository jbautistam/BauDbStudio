using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia base para pruebas
	/// </summary>
	internal class SentenceAssertBase : SentenceBase
	{
		/// <summary>
		///		Proveedor sobre el que se ejecuta la sentencia
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Comando a ejecutar
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; set; }
	}
}
