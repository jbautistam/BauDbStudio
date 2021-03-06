using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.DataSources;

namespace Bau.DbStudio.Views.Reporting.Details.DataSources
{
	/// <summary>
	///		Ventana para mantenimiento de <see cref="DataSourceSqlViewModel"/>
	/// </summary>
	public partial class DataSourceSqlView : UserControl
	{
		public DataSourceSqlView(DataSourceSqlViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			// Carga las columnas y los parámetros
			lstColumns.Load(ViewModel.ColumnsViewModel);
			lstParameters.Load(ViewModel.ParametersViewModel);
			// Asigna la configuración al editor
			udtEditor.EditorFontName = MainWindow.MainController.ConfigurationController.EditorFontName;
			udtEditor.EditorFontSize = MainWindow.MainController.ConfigurationController.EditorFontSize;
			udtEditor.ShowLinesNumber = MainWindow.MainController.ConfigurationController.EditorShowLinesNumber;
			// Carga los datos del viewModel
			if (ViewModel != null)
			{
				// Asigna el nombre de archivo
				udtEditor.Text = ViewModel.Sql;
				udtEditor.ChangeHighLightByExtension(".sql");
				// Indica que no ha habido modificaciones
				ViewModel.IsUpdated = false;
			}
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public DataSourceSqlViewModel ViewModel { get; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}

		private void udtEditor_TextChanged(object sender, EventArgs e)
		{
			ViewModel.Sql = udtEditor.Text;
		}
	}
}