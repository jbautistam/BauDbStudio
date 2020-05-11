using System;
using System.Collections.Generic;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences
{
	/// <summary>
	///		Sentencia de conversión de archivos
	/// </summary>
	internal class ConvertFileSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(FileNameSource))
				return $"{nameof(FileNameSource)} is undefined";
			else if (string.IsNullOrWhiteSpace(FileNameTarget))
				return $"{nameof(FileNameTarget)} is undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Nombre del archivo origen
		/// </summary>
		internal string FileNameSource { get; set; }

		/// <summary>
		///		Nombre del archivo destino
		/// </summary>
		internal string FileNameTarget { get; set; }

		/// <summary>
		///		Columnas del archivo
		/// </summary>
		internal List<FileColumnModel> Columns { get; } = new List<FileColumnModel>();
	}
}
