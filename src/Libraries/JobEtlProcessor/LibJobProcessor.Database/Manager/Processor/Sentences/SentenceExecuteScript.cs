using System;
using System.Collections.Generic;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un script de SQL
	/// </summary>
	internal class SentenceExecuteScript : SentenceBase
	{
		/// <summary>
		///		Proveedor sobre el que se ejecuta la sentencia
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Nombre de archivo (relativo al directorio del proyecto)
		/// </summary>
		internal string FileName { get; set; }

		/// <summary>
		///		Lista de mapeo entre nombres de variables / parámetros en ejecución con los nombres de variables en el script
		/// </summary>
		internal List<(string variable, string to)> Mapping { get; } = new List<(string variable, string to)>();
	}
}
