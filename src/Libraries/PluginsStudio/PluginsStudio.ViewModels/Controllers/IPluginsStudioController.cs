using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Controllers
{
	/// <summary>
	///		Interface para los controladores de solución
	/// </summary>
	public interface IPluginsStudioController
	{
		/// <summary>
		///		Controlador de la aplicación
		/// </summary>
		Base.Controllers.IAppController AppController { get; }

		/// <summary>
		///		Controlador de plugins
		/// </summary>
		Base.Controllers.IPluginsController PluginsController { get; }

		/// <summary>
		///		Controlador de la ventana principal
		/// </summary>
		Base.Controllers.IMainWindowController MainWindowController => PluginsController.MainWindowController;

		/// <summary>
		///		Controlador de host
		/// </summary>
		Base.Controllers.IHostPluginsController HostPluginsController => PluginsController.HostPluginsController;
	}
}
