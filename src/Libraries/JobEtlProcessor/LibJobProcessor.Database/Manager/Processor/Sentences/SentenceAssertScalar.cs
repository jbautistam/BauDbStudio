using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences
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
