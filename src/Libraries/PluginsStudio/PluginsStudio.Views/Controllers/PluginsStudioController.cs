using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

namespace Bau.Libraries.PluginsStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de plugins principal
	/// </summary>
	public class PluginsStudioController : ViewModels.Controllers.IPluginsStudioController
	{
		public PluginsStudioController(PluginsStudioViewManager pluginsStudioViewManager, IMainWindowController mainPluginController)
		{
			PluginsStudioViewManager = pluginsStudioViewManager;
			MainWindowController = mainPluginController;
			HostPluginsController = new HostPluginsController(this);
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		public PluginsStudioViewManager PluginsStudioViewManager { get; }

		/// <summary>
		///		Controlador de plugins
		/// </summary>
		public IMainWindowController MainWindowController { get; }

		/// <summary>
		///		Controlador del host de plugins
		/// </summary>
		public IHostPluginsController HostPluginsController { get; }
	}
}
