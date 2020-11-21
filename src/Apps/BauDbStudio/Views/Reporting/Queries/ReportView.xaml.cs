using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries;

namespace Bau.DbStudio.Views.Reporting.Queries
{
	/// <summary>
	///		Ventana para ejecutar una consulta
	/// </summary>
	public partial class ReportView : UserControl
	{
		public ReportView(ReportViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			// Asigna la configuración al editor
			udtSql.EditorFontName = MainWindow.MainController.ConfigurationController.EditorFontName;
			udtSql.EditorFontSize = MainWindow.MainController.ConfigurationController.EditorFontSize;
			udtSql.ShowLinesNumber = MainWindow.MainController.ConfigurationController.EditorShowLinesNumber;
			udtSql.ChangeHighLightByExtension("sql");
			// Inicializa el árbol
			trvFields.LoadControl(ViewModel.TreeColumns);
			// Asigna los manejadores de eventos
			ViewModel.PropertyChanged += (sender, args) => {
																if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																		args.PropertyName.Equals(nameof(ReportViewModel.SqlQuery), StringComparison.CurrentCultureIgnoreCase))
																	udtSql.Text = ViewModel.SqlQuery;
															};
			// Actualiza el viewModel
			ViewModel.Load();
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ReportViewModel ViewModel { get; }

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