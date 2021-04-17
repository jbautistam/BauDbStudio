using System;

namespace Bau.Libraries.DbStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de DbStudio
	/// </summary>
	public class DbStudioController : ViewModels.Controllers.IDbStudioController
	{
		public DbStudioController(PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
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
		///		Controlador de plugin
		/// </summary>
		public PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }
	}
}
