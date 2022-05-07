using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Sentences
{
	/// <summary>
	///		Movimiento con los datos de una etiqueta
	/// </summary>
	internal class SentenceTagModel : SentenceBaseModel
	{
		internal SentenceTagModel(string tag, string content) : base(content)
		{
			Tag = tag;
		}

		/// <summary>
		///		Nombre de la etiqueta
		/// </summary>
		internal string Tag { get; }
	}
}
