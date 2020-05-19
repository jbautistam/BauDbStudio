using System;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences
{
	/// <summary>
	///		Sentencia para copiar archivos o directorios
	/// </summary>
	internal class CopySentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate(Core.Models.JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(Source))
				return $"Source path or file is undefined";
			else if (string.IsNullOrWhiteSpace(Target))
				return $"Target path or file is undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Archivo / directorio origen
		/// </summary>
		internal string Source { get; set; }

		/// <summary>
		///		Archivo / directorio destino
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Máscara de archivos
		/// </summary>
		internal string Mask { get; set; }
	}
}
