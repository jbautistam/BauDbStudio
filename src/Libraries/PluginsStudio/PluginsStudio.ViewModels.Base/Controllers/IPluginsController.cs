namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

/// <summary>
///		Interface para los controladores de solución
/// </summary>
public interface IPluginsController
{
	/// <summary>
	///		Controlador de plugins
	/// </summary>
	IMainWindowController MainWindowController { get; }

	/// <summary>
	///		Controlador del host de plugins
	/// </summary>
	IHostPluginsController HostPluginsController { get; }

	/// <summary>
	///		Controlador de la configuración
	/// </summary>
	IConfigurationController ConfigurationController { get; }
}
