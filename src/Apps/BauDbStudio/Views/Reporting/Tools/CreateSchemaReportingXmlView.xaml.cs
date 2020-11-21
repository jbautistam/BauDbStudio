using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Tools;

namespace Bau.DbStudio.Views.Reporting.Tools
{
	/// <summary>
	///		Vista para crear el esquema de informes a partir de un esquema de base de datos
	/// </summary>
	public partial class CreateSchemaReportingXmlView : Window
	{
		public CreateSchemaReportingXmlView(CreateSchemaReportingXmlViewModel viewModel)
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
		public CreateSchemaReportingXmlViewModel ViewModel { get; }
	}
}
