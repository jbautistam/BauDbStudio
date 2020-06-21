using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects;

namespace Bau.DbStudio.Views.EtlProjects
{
	/// <summary>
	///		Vista para crear proyectos de prueba de base de datos
	/// </summary>
	public partial class ExportDatabaseView : Window
	{
		public ExportDatabaseView(ExportDatabaseViewModel viewModel)
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
		public ExportDatabaseViewModel ViewModel { get; }
	}
}
