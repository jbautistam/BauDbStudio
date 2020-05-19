using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences
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
		///		Mensaje que acompaña al resultado de la comparación
		/// </summary>
		internal string Message { get; set; }

		/// <summary>
		///		Comando a ejecutar
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; set; }
	}
}
