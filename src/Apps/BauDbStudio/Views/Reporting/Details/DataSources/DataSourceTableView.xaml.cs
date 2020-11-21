using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.DataSources;

namespace Bau.DbStudio.Views.Reporting.Details.DataSources
{
	/// <summary>
	///		Ventana para mantenimiento de <see cref="DataSourceTableViewModel"/>
	/// </summary>
	public partial class DataSourceTableView : UserControl
	{
		public DataSourceTableView(DataSourceTableViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			lstColumns.Load(ViewModel.ColumnsViewModel);
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public DataSourceTableViewModel ViewModel { get; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}
	}
}