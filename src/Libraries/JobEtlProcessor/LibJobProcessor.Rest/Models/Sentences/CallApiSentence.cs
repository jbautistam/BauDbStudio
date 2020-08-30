using System;
using System.Collections.Generic;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences
{
	/// <summary>
	///		Sentencia de definición de API
	/// </summary>
	internal class CallApiSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos de la sentencia
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (string.IsNullOrWhiteSpace(Url))
				return $"{nameof(Url)} is undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Dirección de la API
		/// </summary>
		internal string Url { get; set; }

		/// <summary>
		///		Usuario
		/// </summary>
		internal string User { get; set; }

		/// <summary>
		///		Contraseña
		/// </summary>
		internal string Password { get; set; }

		/// <summary>
		///		Métodos a los que se debe llamar
		/// </summary>
		internal List<CallApiMethodSentence> Methods { get; } = new List<CallApiMethodSentence>();
	}
}
