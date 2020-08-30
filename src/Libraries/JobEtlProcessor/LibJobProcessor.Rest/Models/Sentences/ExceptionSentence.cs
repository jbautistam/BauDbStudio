using System;
using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences
{
	/// <summary>
	///		Sentencia con una excepción
	/// </summary>
	internal class ExceptionSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(Message))
				return $"{nameof(Message)} undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Mensaje de la excepción
		/// </summary>
		public string Message { get; set; }
	}
}
