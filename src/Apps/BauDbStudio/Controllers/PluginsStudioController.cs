using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador de plugins principal
	/// </summary>
	public class PluginsStudioController : Libraries.PluginsStudio.ViewModels.Controllers.IPluginsStudioController
	{
		public PluginsStudioController(DbStudioViewsManager dbStudioViewManager)
		{
			DbStudioViewManager = dbStudioViewManager;
			PluginsController = new PluginsController(dbStudioViewManager);
		}

		/// <summary>
		///		Llama a los diferentes plugins para que actualicen exploradores y ventanas
		/// </summary>
		public void Refresh()
		{
			DbStudioViewManager.PluginsManager.Refresh();
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		public DbStudioViewsManager DbStudioViewManager { get; }

		/// <summary>
		///		Controlador de la aplicación
		/// </summary>
		public IAppController AppController => DbStudioViewManager.AppController;

		/// <summary>
		///		Controlador de plugins
		/// </summary>
		public IPluginsController PluginsController { get; }
	}
}
