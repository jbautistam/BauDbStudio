using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.DbStudio.Controllers.EventArguments;

/// <summary>
///		Argumentos de los eventos de abrir ventana
/// </summary>
public class OpenDocumentEventArgs : EventArgs
{
	public OpenDocumentEventArgs(System.Windows.Controls.UserControl userControl, IDetailViewModel viewModel)
	{
		UserControl = userControl;
		ViewModel = viewModel;
	}

	/// <summary>
	///		Control de usuario
	/// </summary>
	public System.Windows.Controls.UserControl UserControl { get; }

	/// <summary>
	///		ViewModel
	/// </summary>
	public IDetailViewModel ViewModel { get; }
}
