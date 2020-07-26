using System;
using System.Collections.Generic;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences
{
	/// <summary>
	///		Sentencia que comprueba si existe un directorio o archivo
	/// </summary>
	internal class IfExistsSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			string error = string.Empty;

				// Comprueba el contenido
				if (string.IsNullOrWhiteSpace(Path))
					error = "Path can't be empty";
				// Comprueba el bloque if
				if (IfSentences.Count > 0)
					foreach (BaseSentence sentence in IfSentences)
						if (!sentence.ValidateData(project, out string errorSentence))
							error += Environment.NewLine + errorSentence;
				// Comprueba el bloque else
				if (ElseSentences.Count > 0)
					foreach (BaseSentence sentence in ElseSentences)
						if (!sentence.ValidateData(project, out string errorSentence))
							error += Environment.NewLine + errorSentence;
				// Devuelve el error
				return error;
		}
		/// <summary>
		///		Directorio / archivo
		/// </summary>
		internal string Path { get; set; }

		/// <summary>
		///		Sentencias que se ejecutan si existe el archivo
		/// </summary>
		internal List<BaseSentence> IfSentences { get; } = new List<BaseSentence>();

		/// <summary>
		///		Sentencias que se ejecutan si no existe el archivo
		/// </summary>
		internal List<BaseSentence> ElseSentences { get; } = new List<BaseSentence>();
	}
}
