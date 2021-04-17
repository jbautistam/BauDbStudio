using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Controllers
{
	/// <summary>
	///		Interface para los controladores de solución
	/// </summary>
	public interface IPluginsStudioController
	{
		/// <summary>
		///		Controlador de plugins
		/// </summary>
		Base.Controllers.IMainWindowController MainWindowController { get; }

		/// <summary>
		///		Controlador del host de plugins
		/// </summary>
		Base.Controllers.IHostPluginsController HostPluginsController { get; }
	}
}
