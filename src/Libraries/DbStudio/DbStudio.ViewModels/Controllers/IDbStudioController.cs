using System;

namespace Bau.Libraries.DbStudio.ViewModels.Controllers
{
	/// <summary>
	///		Interface para los controladores de solución de DbStudio
	/// </summary>
	public interface IDbStudioController
	{
		/// <summary>
		///		Obtiene el nombre del archivo de la consola de ejecución de proyectos ETL
		/// </summary>
		string GetEtlConsoleFileName();

		/// <summary>
		///		Controlador de la aplicación
		/// </summary>
		PluginsStudio.ViewModels.Base.Controllers.IAppController AppController { get; }

		/// <summary>
		///		Controlador plugin
		/// </summary>
		PluginsStudio.ViewModels.Base.Controllers.IPluginsController PluginController { get; }

		/// <summary>
		///		Controlador de la ventana principal
		/// </summary>
		PluginsStudio.ViewModels.Base.Controllers.IMainWindowController MainWindowController => PluginController.MainWindowController;

		/// <summary>
		///		Controlador del host de plugins
		/// </summary>
		PluginsStudio.ViewModels.Base.Controllers.IHostPluginsController HostPluginsController => PluginController.HostPluginsController;

		/// <summary>
		///		Controlador principal
		/// </summary>
		BauMvvm.ViewModels.Controllers.IHostController HostController => MainWindowController.HostController;

		/// <summary>
		///		Controlador de diálogos del sistema
		/// </summary>
		BauMvvm.ViewModels.Controllers.IHostDialogsController DialogsController => MainWindowController.HostController.DialogsController;

		/// <summary>
		///		Controlador con ventanas de sistema
		/// </summary>
		BauMvvm.ViewModels.Controllers.IHostSystemController SystemController => MainWindowController.HostController.SystemController;

		/// <summary>
		///		Controlador de log
		/// </summary>
		LibLogger.Core.LogManager Logger => MainWindowController.Logger;
	}
}
