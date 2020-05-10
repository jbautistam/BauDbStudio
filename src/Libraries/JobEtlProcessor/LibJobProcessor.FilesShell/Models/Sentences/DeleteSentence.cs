using System;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences
{
	/// <summary>
	///		Sentencia para borrar archivos o directorios
	/// </summary>
	internal class DeleteSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate(Core.Models.JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(Path))
				return $"Path or file is undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Archivo / directorio
		/// </summary>
		internal string Path { get; set; }

		/// <summary>
		///		Máscara de archivos
		/// </summary>
		internal string Mask { get; set; }
	}
}
