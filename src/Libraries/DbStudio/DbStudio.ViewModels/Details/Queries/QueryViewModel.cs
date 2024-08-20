using System.Data;
using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbStudio.ViewModels.Controllers.Exporter;
using Bau.Libraries.DbStudio.ViewModels.Details.Connections;
using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.DbScripts.Manager.Builders;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Queries;

/// <summary>
///		ViewModel común para ejecución de consultas
/// </summary>
public class QueryViewModel : BaseObservableObject
{
	// Constantes
	private const string DefaultFileName = "New file.csv";
	private const string MaskExportFiles = "Archivos CSV (*.csv)|*.csv|Archivos parquet (*.parquet)|*.parquet|Archivo SQL - INSERT (*.sql)|*.sql";
	private const string DefaultExtension = ".csv";
	// Eventos públicos
	public event EventHandler? ExecutionRequested;
	public event EventHandler<PluginsStudio.ViewModels.Base.Controllers.EventArguments.EditorSelectedTextRequiredEventArgs>? SelectedTextRequired;
	// Variables privadas
	private string _fileName = string.Empty, _query = string.Empty, _lastQuery = string.Empty, _executionTime = string.Empty, _executionPlanText = string.Empty;
	private ArgumentListModel _arguments = default!;
	private DataTable? _dataResults;
	private ComboConnectionsViewModel _comboConnectionsViewModel = default!;
	private int _actualPage, _pageSize;
	private bool _paginateQuery;
	private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Black;
	private bool _isExecuting;
	private CancellationTokenSource _tokenSource = default!;
	private CancellationToken _cancellationToken = CancellationToken.None;
	private System.Timers.Timer _timer = default!;
	private System.Diagnostics.Stopwatch _stopwatch = default!;

	public QueryViewModel(DbStudioViewModel solutionViewModel, string? selectedConnection, string query, bool executeQueryByParts, bool raisePrepareExecution) : base(false)
	{
		// Asigna los viewModel
		SolutionViewModel = solutionViewModel;
		// Asigna las propiedades
		Query = query;
		ExecuteQueryByParts = executeQueryByParts;
		Arguments = new ArgumentListModel();
		PaginateQuery = false;
		ActualPage = 1;
		PageSize = 10_000;
		// Carga el combo de conexiones
		ComboConnections = new ComboConnectionsViewModel(solutionViewModel, selectedConnection);
		// Asigna los comandos
		if (raisePrepareExecution)
			ProcessCommand = new BaseCommand(_ => RaiseEventPrepareExecution(), _ => !IsExecuting)
									.AddListener(this, nameof(IsExecuting));
		else
			ProcessCommand = new BaseCommand(async _ => await ExecuteQueryAsync(), _ => !IsExecuting)
									.AddListener(this, nameof(IsExecuting));
		CancelQueryCommand = new BaseCommand(_ => CancelQuery(), _ => IsExecuting)
												.AddListener(this, nameof(IsExecuting));
		ShowExecutionPlanCommand = new BaseCommand(async _ => await ShowExecutionPlanAsync(), _ => !IsExecuting)
										.AddListener(this, nameof(IsExecuting));
		ExportCommand = new BaseCommand(async _ => await ExportAsync(CancellationToken.None), _ => !IsExecuting)
								.AddListener(this, nameof(IsExecuting));
		FirstPageCommand = new BaseCommand(async _ => await GoPageAsync(1), _ => PaginateQuery && !IsExecuting)
								.AddListener(this, nameof(IsExecuting))
								.AddListener(this, nameof(PaginateQuery));
		PreviousPageCommand = new BaseCommand(async _ => await GoPageAsync(ActualPage - 1), _ => PaginateQuery && ActualPage > 1 && !IsExecuting)
								.AddListener(this, nameof(IsExecuting))
								.AddListener(this, nameof(PaginateQuery));
		NextPageCommand = new BaseCommand(async _ => await GoPageAsync(ActualPage + 1), _ => PaginateQuery && !IsExecuting)
								.AddListener(this, nameof(IsExecuting))
								.AddListener(this, nameof(PaginateQuery));
		LastPageCommand = new BaseCommand(async _ => await GoPageAsync(ActualPage + 1), _ => PaginateQuery && !IsExecuting)
								.AddListener(this, nameof(IsExecuting))
								.AddListener(this, nameof(PaginateQuery));
	}

	/// <summary>
	///		Lanza el evento para preparar la ejecución
	/// </summary>
	private void RaiseEventPrepareExecution()
	{
		ExecutionRequested?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Ejecuta la consulta
	/// </summary>
	public async Task ExecuteQueryAsync()
	{
		string querySelected = GetEditorSelectedText();

			if (string.IsNullOrWhiteSpace(querySelected))
				SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
			else
			{
				ConnectionModel? connection = ComboConnections.GetSelectedConnection();

					if (connection is null)
						SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
					else
					{
						(QueryModel? query, string error) = GetQuery(connection, querySelected, true);

							if (!string.IsNullOrWhiteSpace(error))
								SolutionViewModel.MainController.HostController.SystemController.ShowMessage(error);
							else if (query is null)
								SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Enter the query");
							else
							{
								// Limpia los datos
								DataResults = null;
								// Arranca la consulta
								StartQuery();
								// Ejecuta la consulta
								try
								{
									// Actualiza la página actual si es una consulta nueva
									if (string.IsNullOrWhiteSpace(_lastQuery) || !querySelected.Equals(_lastQuery, StringComparison.CurrentCultureIgnoreCase))
										ActualPage = 1;
									// Carga la consulta
									DataResults = await SolutionViewModel.Manager.GetDatatableAsync(query, _cancellationToken);
									// Guarda la consulta que se acaba de lanzar
									_lastQuery = querySelected;
								}
								catch (Exception exception)
								{
									SolutionViewModel.Manager.Logger.LogError(exception, $"Error when execute the query. {exception.Message}");
								}
								// Detiene la ejecucion
								StopQuery();
							}
					}
			}
	}

	/// <summary>
	///		Obtiene los argumentos
	/// </summary>
	private (ArgumentListModel arguments, string error) GetArguments()
	{
		(ArgumentListModel arguments, string error) = SolutionViewModel.ConnectionExecutionViewModel.GetParameters();

			// Añade los argumentos predefinidos
			if (string.IsNullOrWhiteSpace(error))
			{
				arguments.Constants.AddRange(Arguments.Constants);
				arguments.Parameters.AddRange(Arguments.Parameters);
			}
			// Devuelve los resultados
			return (arguments, error);
	}

	/// <summary>
	///		Ejecuta una consulta
	/// </summary>
	private (QueryModel? query, string error) GetQuery(ConnectionModel connection, string sql, bool includePagination)
	{
		(ArgumentListModel arguments, string error) = GetArguments();
		QueryModel? query = null;

			// Obtiene la query
			if (string.IsNullOrWhiteSpace(error))
			{
				QueryBuilder builder = new(connection);

					// Asigna las propiedades
					builder.WithSql(sql, ExecuteQueryByParts);
					builder.WithTimeout(connection.TimeoutExecuteScript);
					builder.WithArguments(arguments);
					// Asigna la paginación
					if (includePagination && PaginateQuery)
						builder.WithPagination(ActualPage, PageSize);
					// Obtiene los resultados
					query = builder.Build();
			}
			// Devuelve los resultados
			return (query, error);
	}

	/// <summary>
	///		Muestra el plan de ejecución
	/// </summary>
	private async Task ShowExecutionPlanAsync()
	{
		string querySelected = GetEditorSelectedText();

			if (string.IsNullOrWhiteSpace(querySelected))
				SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta");
			else
			{
				ConnectionModel? connection = ComboConnections.GetSelectedConnection();

					if (connection is null)
						SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
					else
					{
						(QueryModel? query, string error) = GetQuery(connection, querySelected, false);

							if (!string.IsNullOrWhiteSpace(error))
								SolutionViewModel.MainController.HostController.SystemController.ShowMessage(error);
							else if (query is null)
								SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Enter the query");
							else
								try
								{
									DataTable? table = await SolutionViewModel.Manager.GetExecutionPlanAsync(query, _cancellationToken);
									string plan = string.Empty;

										// Obtiene el plan de ejecución
										if (table != null)
											foreach (DataRow row in table.Rows)
											{
												// Añade el contenido de las columnas
												for (int column = 0; column < table.Columns.Count; column++)
													if (!(row[column] is DBNull) && row[column] != null)
														plan += row[column].ToString();
												// Añade un salto de línea
												plan += Environment.NewLine;
											}
										// Asigna el texto del plan de ejecución
										ExecutionPlanText = plan;
								}
								catch (Exception exception)
								{
									SolutionViewModel.MainController.Logger.LogError(exception, $"Error al obtener el plan de ejecución. {exception.Message}");
								}
					}
			}
	}

	/// <summary>
	///		Lanza un evento para solicitar el texto seleccionado al editor
	/// </summary>
	private string GetEditorSelectedText()
	{
		PluginsStudio.ViewModels.Base.Controllers.EventArguments.EditorSelectedTextRequiredEventArgs eventArgs = new();

			// Lanza el evento
			SelectedTextRequired?.Invoke(this, eventArgs);
			// Recupera el texto
			if (string.IsNullOrEmpty(eventArgs.SelectedText))
				return Query;
			else
				return eventArgs.SelectedText;
	}

	/// <summary>
	///		Prepara los datos para arrancar la ejecución de la consulta
	/// </summary>
	private void StartQuery()
	{
		// Inicializa el temporizador
		_timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(500).TotalMilliseconds);
		_stopwatch = new System.Diagnostics.Stopwatch();
		// Indica que se está ejecutando una tarea y arranca el temporizador
		IsExecuting = true;
		_timer.Elapsed += (sender, args) => ExecutionTime = _stopwatch.Elapsed.ToString();
		_timer.Start();
		_stopwatch.Start();
		ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Red;
		// Obtiene el token de cancelación
		_tokenSource = new CancellationTokenSource();
		_cancellationToken = _tokenSource.Token;
	}

	/// <summary>
	///		Detiene la ejecución
	/// </summary>
	private void StopQuery()
	{
		// Detiene los temporizadores
		_timer.Stop();
		_stopwatch.Stop();
		// Inicializa el token de cancelación
		_cancellationToken = CancellationToken.None;
		// Indica que ya no se está ejecutando
		ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Black;
		IsExecuting = false;
	}

	/// <summary>
	///		Cancela la ejecución de la consulta
	/// </summary>
	private void CancelQuery()
	{
		if (IsExecuting && _cancellationToken != CancellationToken.None && _cancellationToken.CanBeCanceled)
		{
			// Cancela las tareas
			_tokenSource.Cancel();
			// Log
			SolutionViewModel.MainController.Logger.LogInformation("Consulta cancelada");
			// Indica que ya no está en ejecución
			StopQuery();
		}
	}

	/// <summary>
	///		Salta a una página de la consulta
	/// </summary>
	private async Task GoPageAsync(int nextPage)
	{
		ActualPage = nextPage;
		await ExecuteQueryAsync();
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// Graba el archivo
		if (string.IsNullOrWhiteSpace(FileName) || newName)
		{
			string? newFileName = SolutionViewModel.MainController.DialogsController
										.OpenDialogSave(string.Empty, "Script SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*", 
														"New query.sql", ".sql");

				// Cambia el nombre de archivo si es necesario
				if (!string.IsNullOrWhiteSpace(newFileName))
					FileName = newFileName;
		}
		// Graba el archivo
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			// Graba el archivo
			LibHelper.Files.HelperFiles.SaveTextFile(FileName, Query, System.Text.Encoding.UTF8);
			// Actualiza el árbol
			SolutionViewModel.MainController.HostPluginsController.RefreshFiles();
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
	/// </summary>
	public string GetSaveAndCloseMessage() => "¿Desea grabar la consulta antes de continuar?";

	/// <summary>
	///		Trata el contenido que se inserta cuando se suelta un nodo del explorador sobre el editor
	/// </summary>
	public async Task<string> TreatTextDroppedAsync(string content, bool shiftPressed, CancellationToken cancellationToken)
	{
		return await new Controllers.DropItems.NodeTextDropHelper(true).TreatTextDroppedAsync(content, shiftPressed, cancellationToken);
	}

	/// <summary>
	///		Exporta la tabla de datos
	/// </summary>
	private async Task ExportAsync(CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(Query))
			SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
		else
		{
			ConnectionModel? connection = ComboConnections.GetSelectedConnection();

				if (connection is null)
					SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
				else
				{
					(QueryModel? query, string error) = GetQuery(connection, Query, false);

						if (!string.IsNullOrWhiteSpace(error))
							SolutionViewModel.MainController.HostController.SystemController.ShowMessage(error);
						else if (query is null)
							SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Enter the query");
						else
						{
							string? fileName = SolutionViewModel.MainController.DialogsController
														.OpenDialogSave(string.Empty, MaskExportFiles, DefaultFileName, DefaultExtension);

								if (!string.IsNullOrEmpty(fileName))
								{
									ExportQueryProcessor processor = new(SolutionViewModel, query, fileName, GetFormatType(fileName), 200_000);

										// Encola el proceso
										await SolutionViewModel.MainController.MainWindowController.EnqueueProcessAsync(processor, cancellationToken);
								}
						}
				}
		}
	}

	/// <summary>
	///		Obtiene el tipo de formato a partir del nombre de archivo
	/// </summary>
	private Application.SolutionManager.FormatType GetFormatType(string fileName)
	{
		if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
			return Application.SolutionManager.FormatType.Parquet;
		else if (fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
			return Application.SolutionManager.FormatType.Sql;
		else
			return Application.SolutionManager.FormatType.Csv;
	}

	/// <summary>
	///		Solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Indica si se debe ejecutar la consulta completa o particionarla como scripts
	/// </summary>
	public bool ExecuteQueryByParts { get; }

	/// <summary>
	///		Consulta a ejecutar
	/// </summary>
	public string Query
	{
		get { return _query; }
		set { CheckProperty(ref _query, value); }
	}

	/// <summary>
	///		Argumentos de la consulta
	/// </summary>
	public ArgumentListModel Arguments
	{
		get { return _arguments; }
		set { CheckProperty(ref _arguments, value); }
	}

	/// <summary>
	///		Resultados de la ejecución de la consulta
	/// </summary>
	public DataTable? DataResults
	{ 
		get { return _dataResults; }
		set { CheckObject(ref _dataResults, value); }
	}

	/// <summary>
	///		Indica si se está ejecutando una tarea
	/// </summary>
	public bool IsExecuting
	{
		get { return _isExecuting; }
		set { CheckProperty(ref _isExecuting, value); }
	}

	/// <summary>
	///		Tiempo de ejecución de la última consulta
	/// </summary>
	public string ExecutionTime
	{
		get { return _executionTime; }
		set { CheckProperty(ref _executionTime, value); }
	}

	/// <summary>
	///		Color del texto del tiempo de ejecución
	/// </summary>
	public BauMvvm.ViewModels.Media.MvvmColor ExecutionTimeColor
	{
		get { return _executionTimeColor; }
		set { CheckObject(ref _executionTimeColor, value); }
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		ViewModel del combo de conexiones
	/// </summary>
	public ComboConnectionsViewModel ComboConnections
	{
		get { return _comboConnectionsViewModel; }
		set { CheckObject(ref _comboConnectionsViewModel, value); }
	}

	/// <summary>
	///		Indica si se debe paginar la consulta
	/// </summary>
	public bool PaginateQuery
	{
		get { return _paginateQuery; }
		set { CheckProperty(ref _paginateQuery, value); }
	}

	/// <summary>
	///		Página actual
	/// </summary>
	public int ActualPage
	{
		get { return _actualPage; }
		set { CheckProperty(ref _actualPage, value); }
	}

	/// <summary>
	///		Tamaño de página
	/// </summary>
	public int PageSize
	{
		get { return _pageSize; }
		set { CheckProperty(ref _pageSize, value); }
	}

	/// <summary>
	///		Texto del plan de ejecución
	/// </summary>
	public string ExecutionPlanText 
	{ 
		get { return _executionPlanText; }
		set { CheckProperty(ref _executionPlanText, value); }
	}

	/// <summary>
	///		Comando para ejecutar la consulta
	/// </summary>
	public BaseCommand ProcessCommand { get; }

	/// <summary>
	///		Comando para cancelar la ejecución de un script
	/// </summary>
	public BaseCommand CancelQueryCommand { get; }

	/// <summary>
	///		Comando para mostrar el plan de ejecución
	/// </summary>
	public BaseCommand ShowExecutionPlanCommand { get; }

	/// <summary>
	///		Comando para grabar el resultado de una consulta
	/// </summary>
	public BaseCommand ExportCommand { get; }

	/// <summary>
	///		Comando para ver la primera página
	/// </summary>
	public BaseCommand FirstPageCommand { get; }

	/// <summary>
	///		Comando para ver la primera página
	/// </summary>
	public BaseCommand PreviousPageCommand { get; }

	/// <summary>
	///		Comando para ver la primera página
	/// </summary>
	public BaseCommand NextPageCommand { get; }

	/// <summary>
	///		Comando para ver la primera página
	/// </summary>
	public BaseCommand LastPageCommand { get; }
}