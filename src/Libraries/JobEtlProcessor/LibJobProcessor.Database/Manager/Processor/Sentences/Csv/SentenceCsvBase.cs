using System;

using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv
{
	/// <summary>
	///		Base para las sentencias CSV
	/// </summary>
	internal class SentenceCsvBase : SentenceBase
	{
		/// <summary>
		///		Número de registros por bloque
		/// </summary>
		internal int BatchSize { get; set; } = 200_000;

		/// <summary>
		///		Parámetros del archivo
		/// </summary>
		internal FileModel Definition { get; set; } = new FileModel();

		/// <summary>
		///		Tiempo de espera para la ejecución de la sentencia
		/// </summary>
		internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);
	}
}
