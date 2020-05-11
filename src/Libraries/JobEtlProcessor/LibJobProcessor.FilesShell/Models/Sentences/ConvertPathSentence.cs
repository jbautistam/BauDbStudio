using System;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences
{
	/// <summary>
	///		Sentencia de conversión de archivos de un directorio
	/// </summary>
	internal class ConvertPathSentence : BaseSentence
	{
		/// <summary>
		///		Tipo de conversión de archivos
		/// </summary>
		internal enum FileType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Archivo CSV</summary>
			Csv,
			/// <summary>Archivo parquet</summary>
			Parquet
		}

		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (string.IsNullOrEmpty(Path))
				return $"{nameof(Path)} is undefined";
			else if (Source == FileType.Unknown)
				return $"{nameof(Source)} is undefined";
			else if (Target == FileType.Unknown)
				return $"{nameof(Target)} is undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Directorio
		/// </summary>
		internal string Path { get; set; }

		/// <summary>
		///		Tipo del archivo origen
		/// </summary>
		internal FileType Source { get; set; }

		/// <summary>
		///		Tipo del archivo destino
		/// </summary>
		internal FileType Target { get; set; }
	}
}
