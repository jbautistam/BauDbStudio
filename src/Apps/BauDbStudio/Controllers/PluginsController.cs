using Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

namespace Bau.DbStudio.Controllers;

/// <summary>
///		Controlador de plugins
/// </summary>
public class PluginsController : IPluginsController
{
	public PluginsController(DbStudioViewsManager dbStudioViewsManager)
	{
		MainWindowController = dbStudioViewsManager.MainWindowsController;
		HostPluginsController = new HostPluginsController(dbStudioViewsManager);
		ConfigurationController = dbStudioViewsManager.ConfigurationController;
	}

	/// <summary>
	///		Controlador de la ventana principal
	/// </summary>
	public IMainWindowController MainWindowController { get; }

	/// <summary>
	///		Controlador del host de plugins
	/// </summary>
	public IHostPluginsController HostPluginsController { get; }

	/// <summary>
	///		Controlador de configuración
	/// </summary>
	public IConfigurationController ConfigurationController { get; }
}
