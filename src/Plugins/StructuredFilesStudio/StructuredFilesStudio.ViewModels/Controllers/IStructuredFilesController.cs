using Microsoft.Extensions.Logging;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Controllers;

/// <summary>
///		Interface para los controladores de solución de los archivos estructurados
/// </summary>
public interface IStructuredFilesStudioController
{
	/// <summary>
	///		Abre una ventana de detalles
	/// </summary>
	BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType OpenWindow(PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailsViewModel);

	/// <summary>
	///		Abre un cuadro de diálogo
	/// </summary>
	BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel);

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
	ILogger Logger => MainWindowController.Logger;
}
