using System;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.Powershell.Models
{
	/// <summary>
	///		Ejecuta un script
	/// </summary>
	internal class ExecuteScriptSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos de la sentencia
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(FileName) && string.IsNullOrWhiteSpace(Content))
				return $"{nameof(FileName)} and {nameof(Content)} are empty";
			else if (!string.IsNullOrWhiteSpace(FileName) && !System.IO.File.Exists(project.GetFullFileName(FileName)))
				return $"Cant find the file {FileName}";
			else
				return string.Empty;
		}

		/// <summary>
		///		Nombre de archivo donde se encuentra el script
		/// </summary>
		internal string FileName { get; set; }

		/// <summary>
		///		Contenido del script
		/// </summary>
		internal string Content { get; set; }

		/// <summary>
		///		Mapeos
		/// </summary>
		internal NormalizedDictionary<string> Mappings { get; } = new NormalizedDictionary<string>();

		/// <summary>
		///		Mapeos de directorios sobre parámetros
		/// </summary>
		internal NormalizedDictionary<string> Paths { get; } = new NormalizedDictionary<string>();
	}
}
