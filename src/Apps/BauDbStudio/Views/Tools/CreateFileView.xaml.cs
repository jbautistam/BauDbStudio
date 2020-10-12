using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Tools;

namespace Bau.DbStudio.Views.Tools
{
	/// <summary>
	///		Vista para seleccionar un nombre de archivo
	/// </summary>
	public partial class CreateFileView : Window
	{
		public CreateFileView(CreateFileViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
			ViewModel.SelectEncoding(MainWindow.MainController.ConfigurationController.LastEncodingIndex);
			ViewModel.Close += (sender, eventArgs) => 
									{
										// Guarda la codificación
										if (eventArgs.IsAccepted)
										{
											MainWindow.MainController.ConfigurationController.LastEncodingIndex = (int) ViewModel.GetSelectedEncoding();
											MainWindow.MainController.ConfigurationController.Save();
										}
										DialogResult = eventArgs.IsAccepted; 
										Close();
									};
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public CreateFileViewModel ViewModel { get; }
	}
}
