using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.RestStudio.ViewModels.Controllers
{
	/// <summary>
	///		Controlador de RestStudio
	/// </summary>
	public interface IRestStudioController
	{
		/// <summary>
		///		Abre una ventana de detalles
		/// </summary>
		SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel);

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel);

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
		IHostController HostController => MainWindowController.HostController;

		/// <summary>
		///		Controlador de diálogos del sistema
		/// </summary>
		IHostDialogsController DialogsController => MainWindowController.HostController.DialogsController;

		/// <summary>
		///		Controlador con ventanas de sistema
		/// </summary>
		IHostSystemController SystemController => MainWindowController.HostController.SystemController;

		/// <summary>
		///		Controlador de log
		/// </summary>
		LibLogger.Core.LogManager Logger => MainWindowController.Logger;
	}
}
