using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects;

namespace Bau.DbStudio.Views.EtlProjects
{
	/// <summary>
	///		Vista para crear proyectos de prueba de base de datos
	/// </summary>
	public partial class CreateImportFilesScriptView : Window
	{
		public CreateImportFilesScriptView(CreateImportFilesScriptViewModel viewModel)
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
		public CreateImportFilesScriptViewModel ViewModel { get; }
	}
}
