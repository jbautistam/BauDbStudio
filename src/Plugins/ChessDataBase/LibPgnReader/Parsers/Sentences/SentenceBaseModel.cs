using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Clase base para lo movimientos
	/// </summary>
	internal abstract class SentenceBaseModel
	{
		internal SentenceBaseModel(string content)
		{
			Content = content;
		}

		/// <summary>
		///		Contenido de la etiqueta
		/// </summary>
		internal string Content { get; }
	}
}
