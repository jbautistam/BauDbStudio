using System;
using System.Collections.Generic;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models
{
	/// <summary>
	///		Opciones asociadas a un archivo
	/// </summary>
	public class FileOptionsModel
	{
		/// <summary>
		///		Id principal
		/// </summary>
		public string Id { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		///		Indica si es una opción para carpetas
		/// </summary>
		public bool ForFolder { get; set; }

		/// <summary>
		///		Extensiones de archivo a las que se aplica la opción
		/// </summary>
		public List<string> FileExtension { get; } = new();

		/// <summary>
		///		Opción de menú
		/// </summary>
		public MenuModel Menu { get; set; }
	}
}