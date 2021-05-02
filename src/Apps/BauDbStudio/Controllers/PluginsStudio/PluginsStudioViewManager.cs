using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.DbStudio.Controllers.PluginsStudio
{
	/// <summary>
	///		Manager de las vistas de PluginStudio
	/// </summary>
	public class PluginsStudioViewManager
	{
		public PluginsStudioViewManager(Libraries.PluginsStudio.Views.Base.Interfaces.IAppViewsController appController, 
										Libraries.PluginsStudio.ViewModels.Base.Controllers.IMainWindowController mainController,
										Libraries.PluginsStudio.ViewModels.Base.Controllers.IConfigurationController configurationController)
		{
			AppViewController = appController;
			PluginsManager = new Plugins.PluginsManager(this);
			PluginStudioController = new Controllers.PluginsStudioController(this, mainController, configurationController);
			PluginsStudioViewModel = new Libraries.PluginsStudio.ViewModels.PluginsStudioViewModel(PluginStudioController);
		}

		/// <summary>
		///		Añade un plugin a la colección
		/// </summary>
		public void AddPlugin(Libraries.PluginsStudio.Views.Base.Interfaces.IPlugin plugin)
		{
			PluginsManager.Add(plugin);
		}

		/// <summary>
		///		Inicializa los plugins
		/// </summary>
		public void InitializePlugins()
		{
			PluginsManager.Initialize();
		}

		/// <summary>
		///		Carga los datos
		/// </summary>
		public void Load(string path, string workspace)
		{
			// Carga los espacios de trabajo
			PluginsStudioViewModel.Load(path);
			// Selecciona el espacio de trabajo
			SelectWorkspace(workspace);
		}

		/// <summary>
		///		Abre un archivo
		/// </summary>
		public void OpenFile(string fileName)
		{
			if (!PluginsManager.OpenFile(fileName))
				PluginsStudioViewModel.OpenFile(fileName);
		}

		/// <summary>
		///		Selecciona un espacio de trabajo
		/// </summary>
		public void SelectWorkspace(string workspace)
		{
			// Selecciona el espacio de trabajo
			PluginsStudioViewModel.SelectWorkspace(workspace);
			// Llama a los plugins para cargar los datos del directorio correspondiente
			if (PluginsStudioViewModel.WorkspacesViewModel.SelectedItem != null)
				PluginsManager.SelectWorkspace(PluginsStudioViewModel.WorkspacesViewModel.SelectedItem.Path);
		}

		/// <summary>
		///		Obtiene los paneles de la ventana principal y de los plugins
		/// </summary>
		public List<PaneModel> GetPanes()
		{
			List<PaneModel> panes = new List<PaneModel>();

				// Añade los paneles de la aplicación principal
				panes.Add(new PaneModel
								{
									Id = "LogView",
									Title = "Log",
									Position = PaneModel.PositionType.Bottom,
									View = new Views.Tools.Log.LogView(PluginsStudioViewModel.LogViewModel)
								}
						 );
				panes.Add(new PaneModel
								{
									Id = "SearchView",
									Title = "Search",
									Position = PaneModel.PositionType.Right,
									View = new Views.Tools.Search.SearchView(PluginsStudioViewModel.SearchFilesViewModel)
								}
						 );
				panes.Add(new PaneModel
								{
									Id = "FilesExplorerView",
									Title = "Files explorer",
									Position = PaneModel.PositionType.Left,
									View = new Views.Explorers.TreeFilesExplorer(PluginsStudioViewModel.TreeFoldersViewModel)
								}
						 );
				// Añade los paneles de los plugins
				panes.AddRange(PluginsManager.GetPanes());
				// Devuelve la colección de paneles
				return panes;
		}

		/// <summary>
		///		Obtiene las barras de herramientas del plugin
		/// </summary>
		public List<ToolBarModel> GetToolBars()
		{
			return PluginsManager.GetToolbars();
		}

		/// <summary>
		///		Obtiene los menús de los plugins
		/// </summary>
		public List<MenuListModel> GetMenus()
		{
			return PluginsManager.GetMenus();
		}

		/// <summary>
		///		Controlador de vistas principal
		/// </summary>
		internal Libraries.PluginsStudio.Views.Base.Interfaces.IAppViewsController AppViewController { get; }

		/// <summary>
		///		Controlador de PluginsStudio
		/// </summary>
		internal Controllers.PluginsStudioController PluginStudioController { get; }

		/// <summary>
		///		Manager de plugins
		/// </summary>
		internal Plugins.PluginsManager PluginsManager { get; }

		/// <summary>
		///		ViewModel
		/// </summary>
		public Bau.Libraries.PluginsStudio.ViewModels.PluginsStudioViewModel PluginsStudioViewModel { get; }
	}
}
