using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Dimension;

namespace Bau.Libraries.DbStudio.Views.Reporting.Details.Dimensions;

/// <summary>
///		Vista para crear el esquema de informes a partir de un esquema de base de datos
/// </summary>
public partial class DimensionChildView : Window
{
	public DimensionChildView(DimensionChildViewModel viewModel)
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
	public DimensionChildViewModel ViewModel { get; }
}
