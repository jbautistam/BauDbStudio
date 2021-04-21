using System;
using System.Collections.Generic;

namespace Bau.Libraries.PluginsStudio.Views
{
	/// <summary>
	///		Manager de las vistas de PluginStudio
	/// </summary>
	public class PluginsStudioViewManager
	{
		public PluginsStudioViewManager(Base.Interfaces.IAppViewsController appController, ViewModels.Base.Controllers.IMainWindowController mainController,
										ViewModels.Base.Controllers.IConfigurationController configurationController)
		{
			AppViewController = appController;
			PluginsManager = new Plugins.PluginsManager(this);
			PluginStudioController = new Controllers.PluginsStudioController(this, mainController, configurationController);
			PluginsStudioViewModel = new ViewModels.PluginsStudioViewModel(PluginStudioController);
		}

		/// <summary>
		///		Añade un plugin a la colección
		/// </summary>
		public void AddPlugin(Base.Interfaces.IPlugin plugin)
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
		public List<Base.Models.PaneModel> GetPanes()
		{
			List<Base.Models.PaneModel> panes = new List<Base.Models.PaneModel>();

				// Añade los paneles de la aplicación principal
				panes.Add(new Base.Models.PaneModel
											{
												Id = "LogView",
												Title = "Log",
												Position = Base.Models.PaneModel.PositionType.Bottom,
												View = new Tools.Log.LogView(PluginsStudioViewModel.LogViewModel)
											}
						 );
				panes.Add(new Base.Models.PaneModel
											{
												Id = "SearchView",
												Title = "Search",
												Position = Base.Models.PaneModel.PositionType.Right,
												View = new Tools.Search.SearchView(PluginsStudioViewModel.SearchFilesViewModel)
											}
						 );
				panes.Add(new Base.Models.PaneModel
											{
												Id = "FilesExplorerView",
												Title = "Files explorer",
												Position = Base.Models.PaneModel.PositionType.Left,
												View = new Explorers.TreeFilesExplorer(PluginsStudioViewModel.TreeFoldersViewModel)
											}
						 );
				// Añade los paneles de los plugins
				panes.AddRange(PluginsManager.GetPanes());
				// Devuelve la colección de paneles
				return panes;
		}

		/// <summary>
		///		Controlador de vistas principal
		/// </summary>
		internal Base.Interfaces.IAppViewsController AppViewController { get; }

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
		public ViewModels.PluginsStudioViewModel PluginsStudioViewModel { get; }
	}
}
