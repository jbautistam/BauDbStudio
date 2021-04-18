using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files.Structured;

namespace Bau.Libraries.DbStudio.Views.Files
{
	/// <summary>
	///		Vista para mostrar las propiedades de un archivo
	/// </summary>
	public partial class CsvFilePropertiesView : Window
	{
		public CsvFilePropertiesView(CsvFilePropertiesViewModel viewModel)
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
		///		ViewModel del archivo
		/// </summary>
		public CsvFilePropertiesViewModel ViewModel { get; }
	}
}
