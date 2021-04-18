using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files.Structured;

namespace Bau.Libraries.DbStudio.Views.Files
{
	/// <summary>
	///		Ventana para mostrar el contenido de un archivo parquet
	/// </summary>
	public partial class DataTableFileView : UserControl
	{
		public DataTableFileView(BaseFileViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			ViewModel.LoadFile();
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public BaseFileViewModel ViewModel { get; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}

		private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			e.Row.Header = (e.Row.GetIndex() + 1).ToString(); 
		}

		/// <summary>
		///		Evita que desaparezcan los caracteres "_" de las cabeceras
		/// </summary>
		private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(e.Column?.Header?.ToString()))
				e.Column.Header = e.Column.Header.ToString().Replace("_", "__");
		}
	}
}
