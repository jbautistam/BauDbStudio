using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects;

namespace Bau.DbStudio.Views.EtlProjects
{
	/// <summary>
	///		Vista para ejecutar un proyecto de ETL
	/// </summary>
	public partial class ExecuteEtlConsoleView : Window
	{
		public ExecuteEtlConsoleView(ExecuteEtlConsoleViewModel viewModel)
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
		public ExecuteEtlConsoleViewModel ViewModel { get; }
	}
}
