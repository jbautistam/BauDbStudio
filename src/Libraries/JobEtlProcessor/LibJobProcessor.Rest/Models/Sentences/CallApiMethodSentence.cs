using System;
using System.Collections.Generic;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences
{
	/// <summary>
	///		Sentencia de llamada a una API
	/// </summary>
	internal class CallApiMethodSentence : BaseSentence
	{
		/// <summary>
		///		Método de llamada
		/// </summary>
		internal enum MethodType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unkwnown,
			/// <summary>Método GET</summary>
			Get,
			/// <summary>Método POST</summary>
			Post,
			/// <summary>Método PUT</summary>
			Put,
			/// <summary>Método DELETE</summary>
			Delete
		}

		/// <summary>
		///		Comprueba los datos de la sentencia
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (Method == MethodType.Unkwnown)
				return $"{nameof(Method)} is undefined";
			else if (string.IsNullOrWhiteSpace(EndPoint))
				return $"{nameof(EndPoint)} is undefined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Punto de entrada de la API
		/// </summary>
		internal string EndPoint { get; set; }

		/// <summary>
		///		Método de la API que se va a llamar
		/// </summary>
		internal MethodType Method { get; set; }

		/// <summary>
		///		Cuerpo de la llamada a la API
		/// </summary>
		internal string Body { get; set; }

		/// <summary>
		///		Comprobación de resultados
		/// </summary>
		internal List<CallApiResultSentence> Results { get; } = new List<CallApiResultSentence>();
	}
}