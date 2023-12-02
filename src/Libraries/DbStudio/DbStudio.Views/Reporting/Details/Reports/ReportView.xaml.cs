using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Reports;

namespace Bau.Libraries.DbStudio.Views.Reporting.Details.Reports;

/// <summary>
///		Ventana para mantenimiento de <see cref="ReportViewModel"/>
/// </summary>
public partial class ReportView : UserControl
{
	public ReportView(ReportViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ReportViewModel ViewModel { get; }
}