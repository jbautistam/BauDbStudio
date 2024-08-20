using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Connections;

namespace Bau.Libraries.DbStudio.Views.Controls;

/// <summary>
///		Barra de herramientas de ejecución
/// </summary>
public partial class ExecutionToolBar : ToolBar
{
	public ExecutionToolBar(ConnectionExecutionViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ConnectionExecutionViewModel ViewModel { get; }
}
