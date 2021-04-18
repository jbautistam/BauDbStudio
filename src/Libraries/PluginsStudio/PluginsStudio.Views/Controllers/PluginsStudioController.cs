using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

namespace Bau.Libraries.PluginsStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de plugins principal
	/// </summary>
	public class PluginsStudioController : ViewModels.Controllers.IPluginsStudioController
	{
		public PluginsStudioController(PluginsStudioViewManager pluginsStudioViewManager, IMainWindowController mainPluginController, IConfigurationController configurationController)
		{
			PluginsStudioViewManager = pluginsStudioViewManager;
			AppController = new AppController(pluginsStudioViewManager);
			PluginsController = new PluginsController(mainPluginController, new HostPluginsController(pluginsStudioViewManager, this), configurationController);
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		public PluginsStudioViewManager PluginsStudioViewManager { get; }

		/// <summary>
		///		Controlador de la aplicación
		/// </summary>
		public IAppController AppController { get; }

		/// <summary>
		///		Controlador de plugins
		/// </summary>
		public IPluginsController PluginsController { get; }
	}
}
