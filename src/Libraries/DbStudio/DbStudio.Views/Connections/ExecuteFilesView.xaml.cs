using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Connections;

namespace Bau.Libraries.DbStudio.Views.Connections;

/// <summary>
///		Ventana para ejecutar una serie de archivos
/// </summary>
public partial class ExecuteFilesView : UserControl
{
	public ExecuteFilesView(ExecuteFilesViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ExecuteFilesViewModel ViewModel { get; }
}
