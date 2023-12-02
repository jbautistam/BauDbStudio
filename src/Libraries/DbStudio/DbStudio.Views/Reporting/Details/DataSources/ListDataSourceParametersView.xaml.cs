using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.DataSources;

namespace Bau.Libraries.DbStudio.Views.Reporting.Details.DataSources;

/// <summary>
///		Ventana para mantenimiento de <see cref="ListDataSourceColumnsViewModel"/>
/// </summary>
public partial class ListDataSourceParametersView : UserControl
{
	public ListDataSourceParametersView()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Carga las columnas
	/// </summary>
	public void Load(ListDataSourceParametersViewModel viewModel)
	{
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ListDataSourceParametersViewModel ViewModel { get; private set; } = default!;
}