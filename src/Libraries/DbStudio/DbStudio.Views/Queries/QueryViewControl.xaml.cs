using System.Windows.Controls;

using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.DbStudio.ViewModels.Details.Queries;

namespace Bau.Libraries.DbStudio.Views.Queries;

/// <summary>
///		Ventana para ejecutar una consulta
/// </summary>
public partial class QueryViewControl : UserControl
{
	// Variables privadas
	private DragDropTreeController _dragDropController;

	public QueryViewControl()
	{
		InitializeComponent();
		_dragDropController = new DragDropTreeController(this, "BaseNodeViewModel");
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	public void LoadControl(QueryViewModel viewModel)
	{
		// Asigna el contexto
		DataContext = ViewModel = viewModel;
		// Asigna la configuración al editor
		udtEditor.EditorFontName = ViewModel.SolutionViewModel.MainController.PluginController.ConfigurationController.EditorFontName;
		udtEditor.EditorFontSize = ViewModel.SolutionViewModel.MainController.PluginController.ConfigurationController.EditorFontSize;
		udtEditor.ShowLinesNumber = ViewModel.SolutionViewModel.MainController.PluginController.ConfigurationController.EditorShowLinesNumber;
		// Asigna el nombre de archivo
		udtEditor.Text = ViewModel.Query;
		udtEditor.ChangeHighLightByExtension(".sql");
		// Asigna el manejador de eventos
		ViewModel.SelectedTextRequired += (sender, args) => args.SelectedText = udtEditor.GetSelectedText();
		// Indica que no ha habido modificaciones
		ViewModel.IsUpdated = false;
		// Asigna la configuración al editor del plan de ejecución
		udtExecutionPlan.EditorFontName = ViewModel.SolutionViewModel.MainController.PluginController.ConfigurationController.EditorFontName;
		udtExecutionPlan.EditorFontSize = ViewModel.SolutionViewModel.MainController.PluginController.ConfigurationController.EditorFontSize;
		udtExecutionPlan.ShowLinesNumber = ViewModel.SolutionViewModel.MainController.PluginController.ConfigurationController.EditorShowLinesNumber;
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
		// Pasa el foco al control de edición
		udtEditor.Focus();
	}

	/// <summary>
	///		Abre la ventana de búsqueda en el editor
	/// </summary>
	internal void OpenSearch()
	{
		udtEditor.ShowSearch(true);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public QueryViewModel? ViewModel { get; private set; }

	private void udtEditor_TextChanged(object sender, EventArgs e)
	{
		if (ViewModel is not null)
			ViewModel.Query = udtEditor.Text;
	}

	private void udtEditor_DragEnter(object sender, System.Windows.DragEventArgs e)
	{
		e.Effects = System.Windows.DragDropEffects.All;
	}

	private async void udtEditor_Drop(object sender, System.Windows.DragEventArgs e)
	{
		if (ViewModel is not null)
		{
			object? data = _dragDropController.GetDragDropFileNode(e.Data);

				if (data is PluginsStudio.ViewModels.Base.Explorers.PluginNodeViewModel baseNodeViewModel)
				{
					string content = baseNodeViewModel.GetTextForEditor(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey);

						if (!string.IsNullOrWhiteSpace(content))
							udtEditor.InsertText(await ViewModel.TreatTextDroppedAsync(content, e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey,
																					   CancellationToken.None), 
												 e.GetPosition(udtEditor));
				}
		}
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
		string? header = e.Column?.Header?.ToString();

			if (!string.IsNullOrWhiteSpace(header) && e.Column is not null)
				e.Column.Header = header;
	}
}