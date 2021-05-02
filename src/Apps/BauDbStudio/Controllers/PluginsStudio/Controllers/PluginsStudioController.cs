using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

namespace Bau.DbStudio.Controllers.PluginsStudio.Controllers
{
	/// <summary>
	///		Controlador de plugins principal
	/// </summary>
	public class PluginsStudioController : Libraries.PluginsStudio.ViewModels.Controllers.IPluginsStudioController
	{
		public PluginsStudioController(PluginsStudioViewManager pluginsStudioViewManager, IMainWindowController mainPluginController, IConfigurationController configurationController)
		{
			PluginsStudioViewManager = pluginsStudioViewManager;
			AppController = new AppController(pluginsStudioViewManager);
			PluginsController = new PluginsController(mainPluginController, new HostPluginsController(pluginsStudioViewManager, this), configurationController);
		}

		/// <summary>
		///		Llama a los diferentes plugins para que actualicen exploradores y ventanas
		/// </summary>
		public void Refresh()
		{
			PluginsStudioViewManager.PluginsManager.Refresh();
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
