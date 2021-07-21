using System;

using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files
{
	/// <summary>
	///		Base para las sentencias de tratamientos de archivos
	/// </summary>
	internal class SentenceFileBase : SentenceBase
	{
		/// <summary>
		///		Tipo de archivo
		/// </summary>
		internal enum FileType
		{
			/// <summary>Csv</summary>
			Csv = 1,
			/// <summary>Parquet</summary>
			Parquet,
			/// <summary>Json</summary>
			Json
		}

		/// <summary>
		///		Obtiene la extensión del archivo
		/// </summary>
		internal string GetExtension()
		{
			switch (Type)
			{
				case FileType.Csv:
					return ".csv";
				case FileType.Parquet:
					return ".parquet";
				case FileType.Json:
					return ".json";
				default:
					throw new NotImplementedException($"Unknown file type: {Type.ToString()}");
			}
		}

		/// <summary>
		///		Tipo de archivo
		/// </summary>
		internal FileType Type { get; set; }

		/// <summary>
		///		Conexión origen
		/// </summary>
		internal string Source { get; set; }

		/// <summary>
		///		Clave del storage destino
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Contenedor
		/// </summary>
		internal string Container { get; set; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		internal string FileName { get; set; }

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
