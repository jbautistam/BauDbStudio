using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de copia masiva
	/// </summary>
	internal class SentenceBulkCopy : SentenceBase
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
		///		Tabla sobre la que se realiza la inserción
		/// </summary>
		internal string Table { get; set; }

		/// <summary>
		///		Mapeo de columnas. Clave: nombre de columna origen, Valor: nombre de columna destino
		/// </summary>
		internal System.Collections.Generic.Dictionary<string, string> Mappings { get; } = new System.Collections.Generic.Dictionary<string, string>();

		/// <summary>
		///		Comando a ejecutar
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; set; }

		/// <summary>
		///		Tamaño del lote de escritura
		/// </summary>
		internal int BatchSize { get; set; } = 30_000;
	}
}
