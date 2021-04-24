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

		/// <summary>
		///		Obtiene las barras de herramientas del plugin
		/// </summary>
		List<Models.ToolBarModel> GetToolBars();

		/// <summary>
		///		Obtiene los menús del plugin
		/// </summary>
		List<ViewModels.Base.Models.MenuListModel> GetMenus();

		/// <summary>
		///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
		/// </summary>
		List<ViewModels.Base.Models.FileOptionsModel> GetFilesOptions();
	}
}
