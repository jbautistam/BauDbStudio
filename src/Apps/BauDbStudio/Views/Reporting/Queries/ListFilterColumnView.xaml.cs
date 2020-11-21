using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries;

namespace Bau.DbStudio.Views.Reporting.Queries
{
	/// <summary>
	///		Vista para mostrar los datos de un <see cref="ListReportColumnFilterViewModel"/>
	/// </summary>
	public partial class ListFilterColumnView : Window
	{
		public ListFilterColumnView(ListReportColumnFilterViewModel viewModel)
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
		public ListReportColumnFilterViewModel ViewModel { get; }
	}
}
