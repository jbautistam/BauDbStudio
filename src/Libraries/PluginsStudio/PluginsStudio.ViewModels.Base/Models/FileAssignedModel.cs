using System;
using System.Collections.Generic;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Models
{
	/// <summary>
	///		Extensiones de archivo asignadas a un plugin
	/// </summary>
	public class FileAssignedModel
	{
		/// <summary>
		///		Id principal
		/// </summary>
		public string Id { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		///		Extensiones de archivo a las que se aplica la opción
		/// </summary>
		public string FileExtension { get; set; }

		/// <summary>
		///		Icono
		/// </summary>
		public string Icon { get; set; }
	}
}