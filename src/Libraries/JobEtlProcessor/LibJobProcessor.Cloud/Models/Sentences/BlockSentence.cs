using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences
{
	/// <summary>
	///		Bloque de sentencias
	/// </summary>
	internal class BlockSentence : BaseSentence
	{
		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected override string Validate(Core.Models.JobProjectModel project)
		{
			string error = string.Empty;

				// Comprueba las sentencias
				foreach (BaseSentence sentence in Sentences)
					if (!sentence.ValidateData(project, out string errorSentence))
						error += Environment.NewLine + errorSentence;
				// Devuelve el error
				return error;
		}

		/// <summary>
		///		Obtiene el mensaje o un valor predeterminado
		/// </summary>
		internal string GetMessage(string defaultMessage)
		{
			if (string.IsNullOrWhiteSpace(Message))
				return defaultMessage;
			else
				return Message;
		}

		/// <summary>
		///		Mensaje asociado al bloque
		/// </summary>
		internal string Message { get; set; }

		/// <summary>
		///		Instrucciones del bloque
		/// </summary>
		internal List<BaseSentence> Sentences { get; } = new List<BaseSentence>();
	}
}
