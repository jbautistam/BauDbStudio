using System;
using System.Collections.Generic;
using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un proceso
	/// </summary>
	internal class ExecuteSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos de la sentencia
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(Process))
				return $"Process is undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Proceso que se va a ejecutar
		/// </summary>
		internal string Process { get; set; }

		/// <summary>
		///		Argumentos del proceso
		/// </summary>
		internal List<ExecuteSentenceArgument> Arguments = new List<ExecuteSentenceArgument>();
	}
}
