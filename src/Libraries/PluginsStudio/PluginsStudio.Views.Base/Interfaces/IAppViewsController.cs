using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.PluginsStudio.Views.Base.Interfaces;

/// <summary>
///		Controlador de la interface principal
/// </summary>
public interface IAppViewsController
{
	/// <summary>
	///		Abre un cuadro de diálogo
	/// </summary>
	SystemControllerEnums.ResultType OpenDialog(System.Windows.Window window);

	/// <summary>
	///		Abre una ventana no modal
	/// </summary>
	void OpenNoModalDialog(System.Windows.Window window, bool withoutParent);

	/// <summary>
	///		Abre un documento
	/// </summary>
	void OpenDocument(System.Windows.Controls.UserControl userControl, IDetailViewModel detailViewModel);
}