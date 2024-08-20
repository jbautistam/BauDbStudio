using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Dimension;

namespace Bau.Libraries.DbStudio.Views.Reporting.Details.Dimensions;

/// <summary>
///		Ventana para mantenimiento de <see cref="DimensionViewModel"/>
/// </summary>
public partial class DimensionView : UserControl
{
	public DimensionView(DimensionViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public DimensionViewModel ViewModel { get; }
}