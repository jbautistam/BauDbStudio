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
			Parquet,
			/// <summary>Archivo Excel</summary>
			Excel
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
		///		Obtiene la extensión de archivo asociada a un tipo de archivo
		/// </summary>
		private string GetExtension(FileType type)
		{
			switch (type)
			{
				case FileType.Csv:
					return ".csv";
				case FileType.Parquet:
					return ".parquet";
				case FileType.Excel:
					return ".xlsx";
				default:
					return string.Empty;
			}
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
		///		Extensión de los archivos origen
		/// </summary>
		internal string SourceExtension 
		{
			get { return GetExtension(Source); }
		}

		/// <summary>
		///		Tipo del archivo destino
		/// </summary>
		internal FileType Target { get; set; }

		/// <summary>
		///		Extensión de los archivos destino
		/// </summary>
		internal string TargetExtension 
		{
			get { return GetExtension(Target); }
		}

		/// <summary>
		///		Indice de la hoja de los archivos Excel
		/// </summary>
		internal int ExcelSheetIndex { get; set; } = 1;

		/// <summary>
		///		Indica si los archivos Excel tienen cabecera
		/// </summary>
		internal bool ExcelWithHeader { get; set; } = true;
	}
}
