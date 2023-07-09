namespace Bau.Libraries.PluginsStudio.ViewModels.Controllers;

/// <summary>
///		Interface para los controladores de solución
/// </summary>
public interface IPluginsStudioController
{
	/// <summary>
	///		Llama a lo plugins para actualizar los exploradores y ventanas
	/// </summary>
	void Refresh();

	/// <summary>
	///		Abre una ventana de detalles
	/// </summary>
	BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType OpenWindow(Base.Interfaces.IDetailViewModel detailsViewModel);

	/// <summary>
	///		Abre un cuadro de diálogo
	/// </summary>
	BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel);

	/// <summary>
	///		Controlador de plugins
	/// </summary>
	Base.Controllers.IPluginsController PluginsController { get; }

	/// <summary>
	///		Controlador de la ventana principal
	/// </summary>
	Base.Controllers.IMainWindowController MainWindowController => PluginsController.MainWindowController;

	/// <summary>
	///		Controlador de host
	/// </summary>
	Base.Controllers.IHostPluginsController HostPluginsController => PluginsController.HostPluginsController;
}
