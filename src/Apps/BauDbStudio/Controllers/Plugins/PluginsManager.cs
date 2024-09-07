using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.DbStudio.Controllers.Plugins;

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
		List<PaneModel> panes = [];

			// Añade los paneles de los diferentes plugins
			foreach (IPlugin plugin in Plugins)
			{
				List<PaneModel>? pluginPanels = plugin.GetPanes();

					if (pluginPanels is not null)
						panes.AddRange(pluginPanels);
			}
			// Devuelve la colección de paneles
			return panes;
	}

	/// <summary>
	///		Obtiene las barras de herramientas de los plugins
	/// </summary>
	internal List<ToolBarModel> GetToolbars()
	{
		List<ToolBarModel> toolbars = [];

			// Añade las barras de herramientas
			foreach (IPlugin plugin in Plugins)
			{
				List<ToolBarModel>? pluginToolBars = plugin.GetToolBars();

					if (pluginToolBars is not null)
						toolbars.AddRange(pluginToolBars);
			}
			// Devuelve la lista
			return toolbars;
	}

	/// <summary>
	///		Obtiene los menús de los plugins
	/// </summary>
	internal List<MenuListModel> GetMenus()
	{
		List<MenuListModel> menus = [];

			// Añade los menús
			foreach (IPlugin plugin in Plugins)
			{
				List<MenuListModel>? pluginMenus = plugin.GetMenus();

					if (pluginMenus is not null)
						menus.AddRange(pluginMenus);
			}
			// Devuelve la lista
			return menus;
	}

	/// <summary>
	///		Obtiene las vistas de configuración de los plugins
	/// </summary>
	internal List<IPluginConfigurationView> GetConfigurationViews()
	{
		List<IPluginConfigurationView> views = [];

			// Añade las configuraciones
			foreach (IPlugin plugin in Plugins)
			{
				IPluginConfigurationView? configurationView = plugin.GetConfigurationView();

					/// Añade la configuración a la lista
					if (configurationView is not null)
						views.Add(configurationView);
			}
			// Devuelve la lista
			return views;
	}

	/// <summary>
	///		Obtiene las extensiones de archivo asignadas
	/// </summary>
	internal List<FileAssignedModel> GetFilesAssigned()
	{
		List<FileAssignedModel> files = new();

			// Añade los archivos asignados
			foreach (IPlugin plugin in Plugins)
			{
				List<FileAssignedModel>? pluginFiles = plugin.GetFilesAssigned();

					if (pluginFiles is not null)
						files.AddRange(pluginFiles);
			}
			// Devuelve la lista
			return files;
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
		if (!string.IsNullOrWhiteSpace(plugin))
		{
			IPlugin? pluginDefinition = Plugins.FirstOrDefault(item => item.GetKey().Equals(plugin, StringComparison.CurrentCultureIgnoreCase));

				if (pluginDefinition is not null)
					System.Diagnostics.Debug.WriteLine("Debería ejecutarse algo");
		}
	}

	/// <summary>
	///		Indica a los plugins que se está cerrando la aplicación
	/// </summary>
	internal bool CloseApp()
	{
		bool canClose = true;

			// Recorre los plugins marcando si se puede cerrar
			foreach (IPlugin plugin in Plugins)
				if (!plugin.ClosePlugin())
					canClose = false;
			// Devuelve el valor que indica si se puede cerrar
			return canClose;
	}

	/// <summary>
	///		Comprueba si se puede ejecutar un comando sobre un plugin
	/// </summary>
	internal bool CheckCanExecutePluginCommand(string plugin, string viewModel, string command) => true;

	/// <summary>
	///		Manager de plugins
	/// </summary>
	internal DbStudioViewsManager DbStudioViewManager { get; }

	/// <summary>
	///		Plugins
	/// </summary>
	private List<IPlugin> Plugins { get; } = new();
}
