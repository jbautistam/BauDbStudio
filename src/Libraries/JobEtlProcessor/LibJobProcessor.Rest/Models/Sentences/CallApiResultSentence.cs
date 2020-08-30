using System;
using System.Collections.Generic;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences
{
	/// <summary>
	///		Sentencia a ejecutar cuando se da un resultado
	/// </summary>
	internal class CallApiResultSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos de la sentencia
		/// </summary>
		protected override string Validate(JobProjectModel project)
		{
			if (Sentences.Count == 0)
				return $"There's no {nameof(Sentences)} defined";
			else
				return string.Empty;
		}

		/// <summary>
		///		Resultado del método: valor inicial
		/// </summary>
		public int ResultFrom { get; set; }

		/// <summary>
		///		Resultado del método: valor final
		/// </summary>
		public int ResultTo { get; set; }

		/// <summary>
		///		Sentencias a ejecutar si el resultado está entre estos valores
		/// </summary>
		internal List<BaseSentence> Sentences { get; } = new List<BaseSentence>();
	}
}
