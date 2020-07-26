using System;
using System.Windows.Controls;

using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Queries;

namespace Bau.DbStudio.Views.Queries
{
	/// <summary>
	///		Ventana para ejecutar una consulta
	/// </summary>
	public partial class ExecuteQueryView : UserControl
	{
		// Variables privadas
		private DragDropTreeExplorerController _dragDropController = new DragDropTreeExplorerController();

		public ExecuteQueryView(ExecuteQueryViewModel viewModel)
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
			udtEditor.EditorFontName = MainWindow.MainController.ConfigurationController.EditorFontName;
			udtEditor.EditorFontSize = MainWindow.MainController.ConfigurationController.EditorFontSize;
			udtEditor.ShowLinesNumber = MainWindow.MainController.ConfigurationController.EditorShowLinesNumber;
			// Carga los datos del viewModel
			if (ViewModel != null)
			{
				// Asigna el nombre de archivo
				udtEditor.Text = ViewModel.Query;
				udtEditor.ChangeHighLightByExtension(".sql");
				// Asigna el manejador de eventos
				ViewModel.SelectedTextRequired += (sender, args) => args.SelectedText = udtEditor.GetSelectedText();
				// Indica que no ha habido modificaciones
				ViewModel.IsUpdated = false;
			}
			// Pasa el foco al control de edición
			udtEditor.Focus();
			// Asigna la configuración al editor del plan de ejecución
			udtExecutionPlan.EditorFontName = MainWindow.MainController.ConfigurationController.EditorFontName;
			udtExecutionPlan.EditorFontSize = MainWindow.MainController.ConfigurationController.EditorFontSize;
			udtExecutionPlan.ShowLinesNumber = MainWindow.MainController.ConfigurationController.EditorShowLinesNumber;
			ViewModel.PropertyChanged += (sender, args) => {
																if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																		args.PropertyName.Equals(nameof(ExecuteQueryViewModel.ExecutionPlanText), StringComparison.CurrentCultureIgnoreCase))
																	udtExecutionPlan.Text = ViewModel.ExecutionPlanText;
															};
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ExecuteQueryViewModel ViewModel { get; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}

		private void udtEditor_TextChanged(object sender, EventArgs e)
		{
			ViewModel.Query = udtEditor.Text;
		}

		private void udtEditor_DragEnter(object sender, System.Windows.DragEventArgs e)
		{
			e.Effects = System.Windows.DragDropEffects.All;
		}

		private void udtEditor_Drop(object sender, System.Windows.DragEventArgs e)
		{
			if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections.NodeTableViewModel tableNodeViewModel)
				udtEditor.InsertText(tableNodeViewModel.GetSqlSelect(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey), 
									 e.GetPosition(udtEditor));
			else if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections.NodeTableFieldViewModel fieldNodeViewModel)
				udtEditor.InsertText(fieldNodeViewModel.GetSqlSelect(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey), 
									 e.GetPosition(udtEditor));
			else if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.DbStudio.ViewModels.Solutions.Explorers.Files.NodeFileViewModel fileNodeViewModel)
				udtEditor.InsertText(fileNodeViewModel.GetSqlSelect(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey), 
									 e.GetPosition(udtEditor));
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