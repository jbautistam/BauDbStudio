using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

namespace Bau.Libraries.PluginsStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de plugins
	/// </summary>
	public class PluginsController : IPluginsController
	{
		public PluginsController(IMainWindowController mainWindowController, IHostPluginsController hostPluginsController, IConfigurationController configurationController)
		{
			MainWindowController = mainWindowController;
			HostPluginsController = hostPluginsController;
			ConfigurationController = configurationController;
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
}
