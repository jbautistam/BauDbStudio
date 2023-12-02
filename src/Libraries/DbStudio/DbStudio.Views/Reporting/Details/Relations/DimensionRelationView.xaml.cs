using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Relations;

namespace Bau.Libraries.DbStudio.Views.Reporting.Details.Relations;

/// <summary>
///		Vista para crear el esquema de informes a partir de un esquema de base de datos
/// </summary>
public partial class DimensionRelationView : Window
{
	public DimensionRelationView(DimensionRelationViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		ViewModel.Close += (sender, eventArgs) => 
								{
									DialogResult = eventArgs.IsAccepted; 
									Close();
								};
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public DimensionRelationViewModel ViewModel { get; }
}
