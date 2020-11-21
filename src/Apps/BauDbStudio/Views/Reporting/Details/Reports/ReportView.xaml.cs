using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Reports;

namespace Bau.DbStudio.Views.Reporting.Details.Reports
{
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
}