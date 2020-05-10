using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects;

namespace Bau.DbStudio.Views.EtlProjects
{
	/// <summary>
	///		Vista para mostrar los datos de una conexión
	/// </summary>
	public partial class CreateTestXmlView : Window
	{
		public CreateTestXmlView(CreateTestXmlViewModel viewModel)
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
		public CreateTestXmlViewModel ViewModel { get; }
	}
}
