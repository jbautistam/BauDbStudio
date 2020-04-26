using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de prueba de ejecución de un comando
	/// </summary>
	internal class SentenceAssertExecute : SentenceAssertBase
	{
		/// <summary>
		///		Indica si se espera que la ejecución de la consulta devuelva un error
		/// </summary>
		internal bool WithError { get; set; }

		/// <summary>
		///		Número de registros afectados por la consulta si no ha habido error
		/// </summary>
		internal int Records { get; set; }
	}
}
