using System;
using System.Windows;

using Bau.Libraries.PluginsStudio.ViewModels.Tools;

namespace Bau.DbStudio.Views.Files
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
			ViewModel.SelectEncoding(ViewModel.MainViewModel.PluginsStudioController.PluginsController.ConfigurationController.LastEncodingIndex);
			ViewModel.Close += (sender, eventArgs) => 
									{
										// Guarda la codificación
										if (eventArgs.IsAccepted)
										{
											ViewModel.MainViewModel.PluginsStudioController.PluginsController.ConfigurationController.LastEncodingIndex = (int) ViewModel.GetSelectedEncoding();
											ViewModel.MainViewModel.PluginsStudioController.PluginsController.ConfigurationController.Save();
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
