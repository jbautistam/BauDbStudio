using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Tools;

namespace Bau.DbStudio.Views.Reporting.Tools
{
	/// <summary>
	///		Vista para crear los scripts SQL de base de datos de reporting a partir del XML de reporting
	/// </summary>
	public partial class CreateReportingSqlView : Window
	{
		public CreateReportingSqlView(CreateScriptsSqlReportingViewModel viewModel)
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
		public CreateScriptsSqlReportingViewModel ViewModel { get; }
	}
}
