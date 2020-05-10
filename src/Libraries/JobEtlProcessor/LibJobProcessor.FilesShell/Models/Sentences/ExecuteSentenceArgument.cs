using System;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences
{
	/// <summary>
	///		Argumento de una sentencia de ejecución
	/// </summary>
	internal class ExecuteSentenceArgument
	{
		/// <summary>
		///		Clave / prefijo del argumento
		/// </summary>
		internal string Key { get; set; }

		/// <summary>
		///		Valor del argumento
		/// </summary>
		internal string Value { get; set; }

		/// <summary>
		///		Indica si el argumento se debe traducir como un nombre de archivo
		/// </summary>
		internal bool TransformFileName { get; set; }
	}
}
