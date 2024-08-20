using System;

namespace Bau.Libraries.LibPgnReader.Models
{
	/// <summary>
	///		Datos de cabecera
	/// </summary>
	public class HeaderPgnModel
	{
		public HeaderPgnModel(string tag, string content)
		{
			Tag = tag;
			Content = content;
		}

		/// <summary>
		///		Etiqueta
		/// </summary>
		public string Tag { get; }

		/// <summary>
		///		Contenido
		/// </summary>
		public string Content { get; }
	}
}
