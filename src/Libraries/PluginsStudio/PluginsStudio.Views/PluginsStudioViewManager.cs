using System;
using System.Collections.Generic;

namespace Bau.Libraries.PluginsStudio.Views
{
	/// <summary>
	///		Manager de las vistas de PluginStudio
	/// </summary>
	public class PluginsStudioViewManager
	{
		public PluginsStudioViewManager(ViewModels.Base.Controllers.IMainWindowController mainController)
		{
			// Inicializa el controlador
			PluginStudioController = new Controllers.PluginsStudioController(this, mainController);
			// Inicializa el ViewModel
			PluginsStudioViewModel = new ViewModels.PluginsStudioViewModel(PluginStudioController);
		}

		/// <summary>
		///		Inicializa los plugins
		/// </summary>
		public void InitializePlugins()
		{
		}

		/// <summary>
		///		Carga los datos
		/// </summary>
		public void Load(string path, string workspace)
		{
			PluginsStudioViewModel.Load(path, workspace);
		}

		/// <summary>
		///		Selecciona un espacio de trabajo
		/// </summary>
		public void SelectWorkspace(string workspace)
		{
			PluginsStudioViewModel.SelectWorkspace(workspace);
		}

		/// <summary>
		///		Obtiene los paneles de la ventana principal y de los plugins
		/// </summary>
		public List<Base.Models.PaneModel> GetPanes()
		{
			List<Base.Models.PaneModel> panes = new List<Base.Models.PaneModel>();

				// Añade los paneles de la aplicación principal
				panes.Add(new Base.Models.PaneModel
											{
												Id = "LogView",
												Title = "Log",
												Position = Base.Models.PaneModel.PositionType.Bottom,
												View = new Tools.Log.LogView(new ViewModels.Tools.Log.LogListViewModel(PluginsStudioViewModel))
											}
						 );
				panes.Add(new Base.Models.PaneModel
											{
												Id = "SearchView",
												Title = "Search",
												Position = Base.Models.PaneModel.PositionType.Right,
												View = new Tools.Search.SearchView(new ViewModels.Tools.Search.SearchFilesViewModel(PluginsStudioViewModel))
											}
						 );
				// Devuelve la colección de paneles
				return panes;
		}

		/// <summary>
		///		Controlador de PluginsStudio
		/// </summary>
		internal Controllers.PluginsStudioController PluginStudioController { get; }

		/// <summary>
		///		Manager de plugins
		/// </summary>
		internal Plugins.PluginsManager PluginsManager { get; } = new();

		/// <summary>
		///		ViewModel
		/// </summary>
		public ViewModels.PluginsStudioViewModel PluginsStudioViewModel { get; }
	}
}
