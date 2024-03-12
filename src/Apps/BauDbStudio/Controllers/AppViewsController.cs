using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.DbStudio.Controllers;

/// <summary>
///		Controlador de aplicación
/// </summary>
public class AppViewsController : Libraries.PluginsStudio.Views.Base.Interfaces.IAppViewsController
{
	// Eventos públicos
	public event EventHandler<EventArguments.OpenDocumentEventArgs>? OpenDocumentRequired;

	public AppViewsController(DbStudioViewsManager dbStudioViewsManager)
	{
		DbStudioViewsManager = dbStudioViewsManager;
	}

	/// <summary>
	///		Abre un cuadro de diálogo
	/// </summary>
	public SystemControllerEnums.ResultType OpenDialog(System.Windows.Window window)
	{
		return DbStudioViewsManager.MainWindowsController.HostHelperController.ShowDialog(DbStudioViewsManager.MainWindow, window);
	}

	/// <summary>
	///		Abre una ventana no modal
	/// </summary>
	public void OpenNoModalDialog(System.Windows.Window window, bool withoutParent)
	{
		DbStudioViewsManager.MainWindowsController.HostHelperController.ShowNoModal(DbStudioViewsManager.MainWindow, window, withoutParent);
	}

	/// <summary>
	///		Abre un documento
	/// </summary>
	public void OpenDocument(System.Windows.Controls.UserControl userControl, IDetailViewModel detailViewModel)
	{
		OpenDocumentRequired?.Invoke(this, new EventArguments.OpenDocumentEventArgs(userControl, detailViewModel));
	}

	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	internal DbStudioViewsManager DbStudioViewsManager { get; }
}
