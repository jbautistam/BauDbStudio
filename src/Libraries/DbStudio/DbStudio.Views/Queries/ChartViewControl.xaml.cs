using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Queries;

namespace Bau.Libraries.DbStudio.Views.Queries;

/// <summary>
///		Control para visualización de un gráfico
/// </summary>
public partial class ChartViewControl : UserControl
{
	public ChartViewControl()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	public void InitControl(ChartViewModel viewModel)
	{
		DataContext = ViewModel = viewModel;
		viewModel.ChartPrepared += (sender, args) => DrawChart();
	}

	/// <summary>
	///		Dibuja el gráfico establecido en el viewModel
	/// </summary>
	private void DrawChart()
	{
		wpfPlot.DrawChart(ViewModel.Chart);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ChartViewModel ViewModel { get; private set; } = default!;
}
