using System;
using System.Collections.Generic;

namespace Bau.Libraries.PluginsStudio.Views.Plugins
{
	/// <summary>
	///		Manager de plugins
	/// </summary>
	internal class PluginsManager
	{
		internal PluginsManager(PluginsStudioViewManager pluginsStudioViewManager)
		{
			PluginsStudioViewManager = pluginsStudioViewManager;
		}

		/// <summary>
		///		Añade un plugin
		/// </summary>
		internal void Add(Base.Interfaces.IPlugin plugin)
		{
			Plugins.Add(plugin);
		}

		/// <summary>
		///		Inicializa los plugins
		/// </summary>
		internal void Initialize()
		{
			foreach (Base.Interfaces.IPlugin plugin in Plugins)
				plugin.Initialize(PluginsStudioViewManager.AppViewController, PluginsStudioViewManager.PluginStudioController.PluginsController);
		}

		/// <summary>
		///		Selecciona el directorio de un espacio de trabajo
		/// </summary>
		internal void SelectWorkspace(string path)
		{
			foreach (Base.Interfaces.IPlugin plugin in Plugins)
				plugin.Load(path);
		}

		/// <summary>
		///		Intenta abrir un archivo en los plugins
		/// </summary>
		internal bool OpenFile(string fileName)
		{
			bool opened = false;

				// Intenta abrir el archivo en los plugins
				foreach (Base.Interfaces.IPlugin plugin in Plugins)
					if (!opened)
						opened = plugin.OpenFile(fileName);
				// Devuelve el valor que indica si se ha podido abrir el archivo
				return opened;
		}

		/// <summary>
		///		Obtiene los paneles de los plugins
		/// </summary>
		internal List<Base.Models.PaneModel> GetPanes()
		{
			List<Base.Models.PaneModel> panes = new List<Base.Models.PaneModel>();

				// Añade los paneles de los diferentes plugins
				foreach (Base.Interfaces.IPlugin plugin in Plugins)
					panes.AddRange(plugin.GetPanes());
				// Devuelve la colección de paneles
				return panes;
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
		internal PluginsStudioViewManager PluginsStudioViewManager { get; }

		/// <summary>
		///		Plugins
		/// </summary>
		private List<Base.Interfaces.IPlugin> Plugins { get; } = new();
	}
}
