using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files.Structured;

namespace Bau.DbStudio.Views.Files
{
	/// <summary>
	///		Vista para mostrar las propiedades de un archivo parquet
	/// </summary>
	public partial class ParquetFilePropertiesView : Window
	{
		public ParquetFilePropertiesView(ParquetFilePropertiesViewModel viewModel)
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
		public ParquetFilePropertiesViewModel ViewModel { get; }
	}
}
