using System;

namespace Bau.Libraries.DbStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de DbStudio
	/// </summary>
	public class DbStudioController : ViewModels.Controllers.IDbStudioController
	{
		public DbStudioController(DbStudioViewManager dbStudioViewManager, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			DbStudioViewManager = dbStudioViewManager;
			AppController = new AppController(this);
			PluginController = pluginController;
		}

		/// <summary>
		///		Obtiene el nombre del archivo de la consola de ejecución de proyectos ETL
		/// </summary>
		public string GetEtlConsoleFileName()
		{
			return string.Empty;
		}

		/// <summary>
		///		ViewManager
		/// </summary>
		public DbStudioViewManager DbStudioViewManager { get; }

		/// <summary>
		///		Controlador de vistas de la aplicación
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IAppController AppController { get; }

		/// <summary>
		///		Controlador de plugin
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
	}
}
