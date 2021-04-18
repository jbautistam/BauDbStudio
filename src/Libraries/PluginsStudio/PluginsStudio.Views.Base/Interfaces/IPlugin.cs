using System;
using System.Collections.Generic;

namespace Bau.Libraries.PluginsStudio.Views.Base.Interfaces
{
	/// <summary>
	///		Interface para los plugin
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		///		Inicializa el plugin
		/// </summary>
		void Initialize(IAppViewsController appController, ViewModels.Base.Controllers.IPluginsController pluginsController);

		/// <summary>
		///		Carga los datos de un plugin
		/// </summary>
		void Load(string path);

		/// <summary>
		///		Intenta abrir un archivo en un plugin
		/// </summary>
		bool OpenFile(string fileName);

		/// <summary>
		///		Obtiene los paneles del plugin
		/// </summary>
		List<Models.PaneModel> GetPanes();
	}
}
