using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.DbStudio.Controllers;

/// <summary>
///		Manager de las vistas de PluginStudio
/// </summary>
public class DbStudioViewsManager
{
	public DbStudioViewsManager(string appName, string appPath, MainWindow mainWindow)
	{
		// Asigna los controladores
		MainWindow = mainWindow;
		AppName = appName;
		AppPath = appPath;
		AppViewController = new AppViewsController(this);
		ConfigurationController = new ConfigurationController(this);
		MainWindowsController = new MainWindowController(this);
		PluginsManager = new Plugins.PluginsManager(this);
		PluginStudioController = new PluginsStudioController(this);
		PluginsStudioViewModel = new Libraries.PluginsStudio.ViewModels.PluginsStudioViewModel(PluginStudioController);
		// Crea el directorio de aplicación
		Libraries.LibHelper.Files.HelperFiles.MakePath(appPath);
	}

	/// <summary>
	///		Añade un plugin a la colección
	/// </summary>
	public void AddPlugin(Libraries.PluginsStudio.Views.Base.Interfaces.IPlugin plugin)
	{
		PluginsManager.Add(plugin);
	}

	/// <summary>
	///		Inicializa la ventana principal y los plugins
	/// </summary>
	public void Initialize()
	{
		// Inicializa los controladores
		ConfigurationController.Load(AppPath);
		// Inicializa los plugins
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
			PluginsStudioViewModel.OpenFile(fileName, string.Empty);
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
		List<PaneModel> panes = [
									new PaneModel
											{
												Id = "LogView",
												Title = "Log",
												Position = PaneModel.PositionType.Bottom,
												View = new Views.Tools.Log.LogView(PluginsStudioViewModel.LogViewModel),
											},
									new PaneModel
											{
												Id = "FilesExplorerView",
												Title = "Files explorer",
												Position = PaneModel.PositionType.Left,
												View = new Views.Explorers.TreeFilesExplorer(PluginsStudioViewModel.TreeFoldersViewModel)
											},
									new PaneModel
											{
												Id = "TasksQueueView",
												Title = "Processes queue",
												Position = PaneModel.PositionType.Bottom,
												View = new Views.TasksQueue.TasksQueueView(PluginsStudioViewModel.TasksQueueListViewModel)
											}
								];

			// Añade los paneles de los plugins
			panes.AddRange(PluginsManager.GetPanes());
			// Devuelve la colección de paneles
			return panes;
	}

	/// <summary>
	///		Obtiene un panel
	/// </summary>
	internal PaneModel? GetPane(string tabId) => GetPanes().FirstOrDefault(item => item.Id.Equals(tabId, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Obtiene las barras de herramientas del plugin
	/// </summary>
	public List<ToolBarModel> GetToolBars() => PluginsManager.GetToolbars();

	/// <summary>
	///		Obtiene los menús de los plugins
	/// </summary>
	public List<MenuListModel> GetMenus() => PluginsManager.GetMenus();

	/// <summary>
	///		Obtiene los controles de configuración de los plugins
	/// </summary>
	public List<Libraries.PluginsStudio.Views.Base.Interfaces.IPluginConfigurationView> GetConfigurationViews() => PluginsManager.GetConfigurationViews();

	/// <summary>
	///		Nombre de la aplicación
	/// </summary>
	public string AppName { get; }

	/// <summary>
	///		Directorio de aplicación
	/// </summary>
	public string AppPath { get; }

	/// <summary>
	///		Ventana principal
	/// </summary>
	internal MainWindow MainWindow { get; }

	/// <summary>
	///		Controlador de vistas principal
	/// </summary>
	internal Libraries.PluginsStudio.Views.Base.Interfaces.IAppViewsController AppViewController { get; }

	/// <summary>
	///		Controlador de PluginsStudio
	/// </summary>
	internal PluginsStudioController PluginStudioController { get; }

	/// <summary>
	///		Manager de plugins
	/// </summary>
	internal Plugins.PluginsManager PluginsManager { get; }

	/// <summary>
	///		Controlador de ventanas principal
	/// </summary>
	internal MainWindowController MainWindowsController { get; }

	/// <summary>
	///		ViewModel
	/// </summary>
	public Libraries.PluginsStudio.ViewModels.PluginsStudioViewModel PluginsStudioViewModel { get; }

	/// <summary>
	///		Controlador de configuración de la aplicación
	/// </summary>
	public ConfigurationController ConfigurationController { get; }
}
