using System;

namespace Bau.Libraries.RestStudio.ViewModels.Controllers
{
	/// <summary>
	///		Controlador de RestStudio
	/// </summary>
	public interface IRestStudioController
	{
		/// <summary>
		///		Controlador plugin
		/// </summary>
		PluginsStudio.ViewModels.Base.Controllers.IMainWindowController PluginController { get; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		BauMvvm.ViewModels.Controllers.IHostController HostController => PluginController.HostController;
	}
}
