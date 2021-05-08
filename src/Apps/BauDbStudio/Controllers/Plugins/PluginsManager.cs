using System;
using System.Collections.Generic;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.DbStudio.Controllers.Plugins
{
	/// <summary>
	///		Manager de plugins
	/// </summary>
	internal class PluginsManager
	{
		internal PluginsManager(DbStudioViewsManager dbStudioViewManager)
		{
			DbStudioViewManager = dbStudioViewManager;
		}

		/// <summary>
		///		Añade un plugin
		/// </summary>
		internal void Add(IPlugin plugin)
		{
			Plugins.Add(plugin);
		}

		/// <summary>
		///		Inicializa los plugins
		/// </summary>
		internal void Initialize()
		{
			// Iniclaliza los plugins
			foreach (IPlugin plugin in Plugins)
				plugin.Initialize(DbStudioViewManager.AppViewController, DbStudioViewManager.PluginStudioController.PluginsController);
			// Obtiene las opciones de los archivos
			foreach (IPlugin plugin in Plugins)
				DbStudioViewManager.PluginsStudioViewModel.TreeFoldersViewModel.AddPluginOptions(plugin.GetFilesOptions());
		}

		/// <summary>
		///		Selecciona el directorio de un espacio de trabajo
		/// </summary>
		internal void SelectWorkspace(string path)
		{
			foreach (IPlugin plugin in Plugins)
				plugin.Load(path);
		}

		/// <summary>
		///		Intenta abrir un archivo en los plugins
		/// </summary>
		internal bool OpenFile(string fileName)
		{
			bool opened = false;

				// Intenta abrir el archivo en los plugins
				foreach (IPlugin plugin in Plugins)
					if (!opened)
						opened = plugin.OpenFile(fileName);
				// Devuelve el valor que indica si se ha podido abrir el archivo
				return opened;
		}

		/// <summary>
		///		Obtiene los paneles de los plugins
		/// </summary>
		internal List<PaneModel> GetPanes()
		{
			List<PaneModel> panes = new List<PaneModel>();

				// Añade los paneles de los diferentes plugins
				foreach (IPlugin plugin in Plugins)
					panes.AddRange(plugin.GetPanes());
				// Devuelve la colección de paneles
				return panes;
		}

		/// <summary>
		///		Obtiene las barras de herramientas de los plugins
		/// </summary>
		internal List<ToolBarModel> GetToolbars()
		{
			List<ToolBarModel> toolbars = new();

				// Añade las barras de herramientas
				foreach (IPlugin plugin in Plugins)
					toolbars.AddRange(plugin.GetToolBars());
				// Devuelve la lista
				return toolbars;
		}

		/// <summary>
		///		Obtiene los menús de los plugins
		/// </summary>
		internal List<MenuListModel> GetMenus()
		{
			List<MenuListModel> menus = new();

				// Añade los menús
				foreach (IPlugin plugin in Plugins)
					menus.AddRange(plugin.GetMenus());
				// Devuelve la lista
				return menus;
		}

		/// <summary>
		///		Obtiene las vistas de configuración de los plugins
		/// </summary>
		internal List<IPluginConfigurationView> GetConfigurationViews()
		{
			List<IPluginConfigurationView> views = new();

				// Añade las configuraciones
				foreach (IPlugin plugin in Plugins)
				{
					IPluginConfigurationView configurationView = plugin.GetConfigurationView();

						/// Añade la configuración a la lista
						if (configurationView != null)
							views.Add(configurationView);
				}
				// Devuelve la lista
				return views;
		}

		/// <summary>
		///		Actualiza los exploradores y ventanas
		/// </summary>
		internal void Refresh()
		{
			foreach (IPlugin plugin in Plugins)
				plugin.Refresh();
		}

		/// <summary>
		///		Ejecuta un comando sobre un plugin
		/// </summary>
		internal void ExecutePluginCommand(string plugin, string viewModel, string command)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Comprueba si se puede ejecutar un comando sobre un plugin
		/// </summary>
		internal bool CheckCanExecutePluginCommand(string plugin, string viewModel, string command)
		{
			return true;
		}

		/// <summary>
		///		Manager de plugins
		/// </summary>
		internal DbStudioViewsManager DbStudioViewManager { get; }

		/// <summary>
		///		Plugins
		/// </summary>
		private List<IPlugin> Plugins { get; } = new();
	}
}
