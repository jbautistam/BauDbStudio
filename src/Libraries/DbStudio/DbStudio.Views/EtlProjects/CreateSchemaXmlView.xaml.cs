using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

namespace Bau.Libraries.DbStudio.Views.EtlProjects
{
	/// <summary>
	///		Vista para crear proyectos de prueba de base de datos
	/// </summary>
	public partial class CreateSchemaXmlView : Window
	{
		public CreateSchemaXmlView(CreateSchemaXmlViewModel viewModel)
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
		public CreateSchemaXmlViewModel ViewModel { get; }
	}
}
