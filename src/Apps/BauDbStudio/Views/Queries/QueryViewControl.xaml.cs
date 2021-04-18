using System;
using System.Windows.Controls;

using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Queries;

namespace Bau.DbStudio.Views.Queries
{
	/// <summary>
	///		Ventana para ejecutar una consulta
	/// </summary>
	public partial class QueryViewControl : UserControl
	{
		// Variables privadas
		private DragDropTreeExplorerController _dragDropController = new DragDropTreeExplorerController();

		public QueryViewControl()
		{
			InitializeComponent();
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		public void LoadControl(QueryViewModel viewModel)
		{
			// Asigna el contexto
			DataContext = ViewModel = viewModel;
			// Inicializa el gráfico
			udtChart.InitControl(ViewModel.ChartViewModel);
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
			// Asigna los manejadores de eventos
			ViewModel.PropertyChanged += (sender, args) => {
																if (!string.IsNullOrWhiteSpace(args.PropertyName))
																	switch (args.PropertyName)
																	{
																		case nameof(QueryViewModel.ExecutionPlanText):
																				udtExecutionPlan.Text = ViewModel.ExecutionPlanText;
																			break;
																		case nameof(QueryViewModel.Query):
																				udtEditor.Text = ViewModel.Query;
																			break;
																	}
															};
			ViewModel.SelectedTextRequired += (sender, args) => args.SelectedText = udtEditor.GetSelectedText();
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public QueryViewModel ViewModel { get; private set; }

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
			else if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.PluginsStudio.ViewModels.Explorers.Files.NodeFileViewModel fileNodeViewModel)
				udtEditor.InsertText(ViewModel.GetAdvancedDroppedNodeFile(fileNodeViewModel.FileName, e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey), 
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