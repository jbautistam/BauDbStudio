using System;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de prueba de una consulta que devuelve un resultado numérico
	/// </summary>
	internal class SentenceAssertScalar : SentenceAssertBase
	{
		/// <summary>
		///		Resultado esperado de la consulta
		/// </summary>
		internal long Result { get; set; }
	}
}
