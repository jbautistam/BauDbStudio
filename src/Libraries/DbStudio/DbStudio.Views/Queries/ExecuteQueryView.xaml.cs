using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Queries;

namespace Bau.Libraries.DbStudio.Views.Queries;

/// <summary>
///		Ventana para ejecutar una consulta
/// </summary>
public partial class ExecuteQueryView : UserControl
{
	public ExecuteQueryView(ExecuteQueryViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		ViewModel.StartSearchRequired += (sender, args) => udtQuery.OpenSearch();
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	private void InitForm()
	{
		udtQuery.LoadControl(ViewModel.QueryViewModel);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ExecuteQueryViewModel ViewModel { get; }

	private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
	{
		InitForm();
	}
}