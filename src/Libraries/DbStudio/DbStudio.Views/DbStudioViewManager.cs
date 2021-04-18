using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.DbStudio.Views
{
	/// <summary>
	///		Manager de vistas de DbStudio
	/// </summary>
	public class DbStudioViewManager : PluginsStudio.Views.Base.Interfaces.IPlugin
	{
		/// <summary>
		///		Inicializa el manager de vistas de DbStudio
		/// </summary>
		public void Initialize(PluginsStudio.Views.Base.Interfaces.IAppViewsController appViewsController, 
							   PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new ViewModels.MainViewModel("DbStudio", new Controllers.DbStudioController(this, pluginController));
		}

		/// <summary>
		///		Carga los datos del directorio
		/// </summary>
		public void Load(string path)
		{
			MainViewModel.Load(path);
		}

		/// <summary>
		///		Intenta abrir un archivo en un plugin
		/// </summary>
		public bool OpenFile(string fileName)
		{
			return MainViewModel.SolutionViewModel.OpenFile(fileName);
		}

		/// <summary>
		///		Obtiene los paneles del plugin
		/// </summary>
		public List<PaneModel> GetPanes()
		{
			List<PaneModel> panes = new List<PaneModel>();

				// Añade los paneles de la aplicación principal
				panes.Add(new PaneModel
								{
									Id = "TreeConnectionsExplorer",
									Title = "Connections",
									Position = PaneModel.PositionType.Left,
									View = new Explorers.TreeConnectionsExplorer(MainViewModel.SolutionViewModel.TreeConnectionsViewModel)
								}
						 );
				panes.Add(new PaneModel
								{
									Id = "TreeStorageExplorer",
									Title = "Storage",
									Position = PaneModel.PositionType.Right,
									View = new Explorers.TreeStoragesExplorer(MainViewModel.SolutionViewModel.TreeStoragesViewModel)
								}
						 );
			//dckManager.AddPane("", "Conexiones", new Views.TreeConnectionsExplorer(ViewModel.SolutionViewModel.TreeConnectionsViewModel), 
			//				   null, Controls.DockLayout.DockLayoutManager.DockPosition.Left);
			//dckManager.AddPane("TreeReportingExplorer", "Informes", 
			//				   new Views.Reporting.Explorers.TreeReportingExplorer(ViewModel.SolutionViewModel.ReportingSolutionViewModel.TreeReportingViewModel), 
			//				   null, Controls.DockLayout.DockLayoutManager.DockPosition.Left);
			//dckManager.AddPane("", "Storage", new Views.TreeStoragesExplorer(ViewModel.SolutionViewModel.TreeStoragesViewModel), 
			//				   null, Controls.DockLayout.DockLayoutManager.DockPosition.Right);
				// Devuelve la lista de paneles
				return panes;
		}

		/// <summary>
		///		Controlador de aplicación
		/// </summary>
		internal PluginsStudio.Views.Base.Interfaces.IAppViewsController AppViewsController { get; private set; }

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public ViewModels.MainViewModel MainViewModel { get; private set; }
	}
}
