using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.ViewModels.Details.Queries;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries;

/// <summary>
///		ViewModel para mostrar un informe
/// </summary>
public class ReportQueryViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Constantes privadas
	private const string MaskRequest = "Requests (*.request.xml)|*.request.xml|All files (*.*)|*.*";
	// Eventos públicos
	public EventHandler? StartSearchRequired;
	// Variables privadas
	private string _header = string.Empty, _sqlFileName = string.Empty;
	private TreeQueryReportViewModel _treeColumns = default!;

	public ReportQueryViewModel(ReportingSolutionViewModel viewModel, ReportModel report) : base(false)
	{
		// Asigna las propiedades
		ViewModel = viewModel;
		QueryViewModel = new QueryViewModel(ViewModel.SolutionViewModel, string.Empty, string.Empty, false, true);
		Report = report;
		Header = report.Id;
		// Inicializa el árbol de campos
		TreeColumns = new TreeQueryReportViewModel(this);
		// Apunta al manejador de eventos de la consulta
		QueryViewModel.ExecutionRequested += async (sender, args) => await ExecuteQueryAsync();
		// Inicializa los comandos
		OpenRequestCommand = new BaseCommand(_ => OpenRequest());
		SaveRequestCommand = new BaseCommand(_ => SaveRequest());
	}

	/// <summary>
	///		Carga los datos
	/// </summary>
	public void Load()
	{
		TreeColumns.Load();
	}

	/// <summary>
	///		Ejecuta la consulta
	/// </summary>
	private async Task ExecuteQueryAsync()
	{
		ReportRequestModel reportRequest = TreeColumns.GetReportRequest();

			// Añade la paginación
			if (QueryViewModel.PaginateQuery)
			{
				reportRequest.Pagination.MustPaginate = true;
				reportRequest.Pagination.Page = QueryViewModel.ActualPage;
				reportRequest.Pagination.RecordsPerPage = QueryViewModel.PageSize;
			}
			// Actualiza el informe recargando el archivo
			ViewModel.ReportingSolutionManager.RefreshAdvancedReport(Report.DataWarehouse, Report.FileName);
			// Obtiene la consulta
			QueryViewModel.Query = ViewModel.ReportingSolutionManager.GetSqlResponse(reportRequest);
			// Añade los parámetros
			foreach (ParameterRequestModel parameter in reportRequest.Parameters)
				QueryViewModel.Arguments.Parameters.Add(parameter.Key, parameter.Value);
			// y la ejecuta
			await QueryViewModel.ExecuteQueryAsync();
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage() => string.Empty;

	/// <summary>
	///		Inicia una búsqueda (por ejemplo, abre la ventana de búsqueda)
	/// </summary>
	public void StartSearch()
	{
		StartSearchRequired?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Graba la consulta
	/// </summary>
	public void SaveDetails(bool newName)
	{
		bool mustSave = !newName;

			// Obtiene el nuevo nombre de archivo si es necesario
			if (string.IsNullOrWhiteSpace(_sqlFileName) || newName)
			{
				string? newFileName = ViewModel.SolutionViewModel.MainController.DialogsController
											.OpenDialogSave(ViewModel.SolutionViewModel.MainController.DialogsController.LastPathSelected,
															"Archivos Sql (*.sql)|*.sql|Todos los archivos (*.*)|*.*",
															"Query.sql", ".sql");

					if (!string.IsNullOrWhiteSpace(newFileName))
					{
						_sqlFileName = newFileName;
						mustSave = true;
					}

			}
			// Si se debe grabar el archivo
			if (mustSave && !string.IsNullOrWhiteSpace(_sqlFileName))
				LibHelper.Files.HelperFiles.SaveTextFile(_sqlFileName, QueryViewModel.Query);
	}

	/// <summary>
	///		Carga una solicitud
	/// </summary>
	private void OpenRequest()
	{
		string? fileName = ViewModel.SolutionViewModel.MainController.DialogsController
									.OpenDialogLoad(ViewModel.SolutionViewModel.MainController.DialogsController.LastPathSelected,
													MaskRequest,
													"Request.request.xml", ".request.xml");

			if (!string.IsNullOrWhiteSpace(fileName))
				TreeColumns.LoadRequest(ViewModel.ReportingSolutionManager.LoadRequest(fileName));
	}

	/// <summary>
	///		Graba una solicitud
	/// </summary>
	private void SaveRequest()
	{
		string? newFileName = ViewModel.SolutionViewModel.MainController.DialogsController
									.OpenDialogSave(ViewModel.SolutionViewModel.MainController.DialogsController.LastPathSelected,
													MaskRequest,
													"Request.request.xml", ".request.xml");

			if (!string.IsNullOrWhiteSpace(newFileName))
				ViewModel.ReportingSolutionManager.SaveRequest(TreeColumns.GetReportRequest(), newFileName);
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public ReportingSolutionViewModel ViewModel { get; }

	/// <summary>
	///		ViewModel de ejecución de la consulta
	/// </summary>
	public QueryViewModel QueryViewModel { get; }

	/// <summary>
	///		Informe
	/// </summary>
	public ReportModel Report { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Id de la pestaña
	/// </summary>
	public string TabId => $"{GetType().ToString()}_{Report.Id}";

	/// <summary>
	///		Arbol de columnas de la consulta
	/// </summary>
	public TreeQueryReportViewModel TreeColumns
	{
		get { return _treeColumns; }
		set { CheckObject(ref _treeColumns, value); }
	}

	/// <summary>
	///		Comando para abrir una solicitud
	/// </summary>
	public BaseCommand OpenRequestCommand { get; }

	/// <summary>
	///		Comando para guardar una solicitud
	/// </summary>
	public BaseCommand SaveRequestCommand { get; }
}