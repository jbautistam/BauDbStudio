using Microsoft.Extensions.Logging;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ListView;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;
using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Connections;

/// <summary>
///		ViewModel con los datos de ejecución de conexiones
/// </summary>
public class ConnectionExecutionViewModel : BaseObservableObject
{
	// Enumerados privados
	/// <summary>
	///		Modo de ejecución
	/// </summary>
	private enum ExecutionMode
	{
		/// <summary>Desconocido</summary>
		Unknown,
		/// <summary>Script de SQL</summary>
		ScriptSql,
		/// <summary>Script de SQL extendido (interpretado)</summary>
		ScriptSqlExtended,
		/// <summary>Lote de scripts SQL</summary>
		BatchSql
	}

	// Variables privadas
	private ComboViewModel _connections = default!;
	private string _connectionParametersFileName = string.Empty, _connectionShortFileName = string.Empty;
	private string _etlParametersFileName = string.Empty, _etlShortFileName = string.Empty;
	private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Black;
	private string _executionTime = string.Empty;
	private bool _isExecuting;
	private CancellationTokenSource _tokenSource = default!;
	private CancellationToken _cancellationToken = CancellationToken.None;
	private System.Timers.Timer _timer = default!;
	private System.Diagnostics.Stopwatch _stopwatch = default!;
	private ControlGenericListViewModel<LastParameterFileViewModel> _lastParametersFileViewModel = default!;

	public ConnectionExecutionViewModel(DbStudioViewModel solutionViewModel)
	{
		// Asigna la solución y el viewmodel de parámetros de ejecución
		SolutionViewModel = solutionViewModel;
		// Asigna los comandos
		ExecuteFileCommand = new BaseCommand(ExecuteFolderFile);
		ExecuteScriptCommand = new BaseCommand(async _ => await ExecuteScriptAsync(), _ => !IsExecuting)
												.AddListener(this, nameof(IsExecuting));
		CancelScriptExecutionCommand = new BaseCommand(_ => CancelScriptExecution(), _ => IsExecuting)
												.AddListener(this, nameof(IsExecuting));
		UpdateParametersFileCommand = new BaseCommand(_ => UpdateParametersFile());
		OpenParametersFileCommand = new BaseCommand(_ => OpenParametersFile(), _ => !string.IsNullOrWhiteSpace(ConnectionParametersFileName))
												.AddListener(this, nameof(ConnectionParametersFileName));
	}

	/// <summary>
	///		Inicializa el viewModel (cuando ya se ha cargado la solución)
	/// </summary>
	public void Load()
	{
		ConnectionModel? selectedConnection = GetSelectedConnection();

			// Obtiene la última conexión seleccionada
			if (selectedConnection is null && !string.IsNullOrWhiteSpace(SolutionViewModel.Solution.LastConnectionSelectedGlobalId))
				selectedConnection = SolutionViewModel.Solution.Connections.Search(SolutionViewModel.Solution.LastConnectionSelectedGlobalId);
			// Carga el nombre del archivo de parámetros de la solución
			LoadConnectionFileNames();
			// Inicializa el combo
			Connections = new ComboViewModel(this);
			// Carga las conexiones
			if (SolutionViewModel.Solution.Connections.Count == 0)
				Connections.AddItem(null, "<Seleccione una conexión>", null);
			else
			{
				// Ordena las conexiones por nombre
				SolutionViewModel.Solution.Connections.SortByName();
				// Añade las conexiones al combo
				foreach (ConnectionModel connection in SolutionViewModel.Solution.Connections)
					Connections.AddItem(null, connection.Name, connection);
			}
			// Selecciona la conexión anterior
			if (selectedConnection is not null)
				foreach (ControlItemViewModel item in Connections.Items)
					if ((item.Tag as ConnectionModel)?.GlobalId.Equals(selectedConnection?.GlobalId, StringComparison.CurrentCultureIgnoreCase) ?? false)
						Connections.SelectedItem = item;
			// Si no se ha seleccionado nada, selecciona el primer elemento
			if (Connections.SelectedItem == null)
				Connections.SelectedItem = Connections.Items[0];
			// Ajusta el manejador de eventos para grabar la última conexión seleccionada
			Connections.PropertyChanged += (sender, args) => {
																if (!string.IsNullOrWhiteSpace(args.PropertyName) && 
																	args.PropertyName.Equals(nameof(Connections.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
																{
																	ConnectionModel? connection = GetSelectedConnection();

																		if (connection is not null && 
																			!connection.GlobalId.Equals(SolutionViewModel.Solution.LastConnectionSelectedGlobalId, 
																										StringComparison.CurrentCultureIgnoreCase))
																		{
																			SolutionViewModel.Solution.LastConnectionSelectedGlobalId = connection.GlobalId;
																			SolutionViewModel.SaveSolution();
																		}
																}
															 };
			// Inicializa la lista de parámetros
			LastParameterFiles = new ControlGenericListViewModel<LastParameterFileViewModel>();
			LoadListParameterFiles();
			LastParameterFiles.PropertyChanged += (sender, args) =>
														{
															if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																	args.PropertyName.Equals(nameof(LastParameterFiles.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
																UpdateParametersFile(LastParameterFiles.SelectedItem?.Tag as string);
														};
	}

	/// <summary>
	///		Carga la lista de parámetros
	/// </summary>
	private void LoadListParameterFiles()
	{
		List<string> files = new();

			// Limpia la lista
			LastParameterFiles.Items.Clear();
			// Añade los elementos a la lista intermedia
			foreach (string fileName in SolutionViewModel.Solution.QueueConnectionParameters.Enumerate())
				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
					files.Add(fileName);
			// Ordena la lista
			files.Sort((first, second) => System.IO.Path.GetFileName(first).CompareTo(System.IO.Path.GetFileName(second)));
			// Añade los nombres de archivo al listview
			foreach (string fileName in files)
				LastParameterFiles.Items.Add(new LastParameterFileViewModel(fileName));
	}

	/// <summary>
	///		Carga los nombres de archivos de la solución
	/// </summary>
	private void LoadConnectionFileNames()
	{
		// Carga el nombre del archivo de parámetros de conexión de la solución
		if (File.Exists(SolutionViewModel.Solution.LastConnectionParametersFileName))
			ConnectionParametersFileName = SolutionViewModel.Solution.LastConnectionParametersFileName;
		else
			ConnectionParametersFileName = string.Empty;
		// Carga el nombre del archivo de parámetros de conexión de los proyectos ETl de la solución
		if (File.Exists(SolutionViewModel.Solution.LastEtlParametersFileName))
			EtlParametersFileName = SolutionViewModel.Solution.LastEtlParametersFileName;
		else
			EtlParametersFileName = string.Empty;
	}

	/// <summary>
	///		Obtiene la conexión seleccionada en el combo
	/// </summary>
	public ConnectionModel? GetSelectedConnection()
	{
		if (Connections?.SelectedItem?.Tag is ConnectionModel connection)
			return connection;
		else
			return null;
	}

	/// <summary>
	///		Ejecuta un script
	/// </summary>
	internal async Task ExecuteScriptAsync()
	{
		if (IsExecuting)
			SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Ya se está ejecutando una consulta");
		else
		{
			IDetailViewModel? selectedViewModel = SolutionViewModel.MainController.MainWindowController.GetActiveDetails();

				if (selectedViewModel is not null)
					switch (GetExecutionMode(selectedViewModel))
					{
						case ExecutionMode.Unknown:
								SolutionViewModel.MainController.HostController.SystemController.ShowMessage("No se puede ejecutar este archivo");
							break;
						case ExecutionMode.BatchSql:
						case ExecutionMode.ScriptSql:
						case ExecutionMode.ScriptSqlExtended:
								await PrepareExecuteScriptSqlAsync(selectedViewModel);
							break;
					}
		}
	}

	/// <summary>
	///		Exporta las tablas de una base de datos a un directorio
	/// </summary>
	public async Task ExportDataBaseAsync(ConnectionModel? connection, ConnectionTableModel? table, CancellationToken cancellationToken)
	{
		if (IsExecuting)
			SolutionViewModel.MainController.SystemController.ShowMessage("No se pueden exportar los datos en este momento. Espere que finalice la ejecución de los scripts");
		else
		{
			ExportDatabaseViewModel viewModel = new(connection, table, SolutionViewModel);

				if (SolutionViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				{
					// Obtiene la conexión seleccionada
					connection = viewModel.ComboConnections.GetSelectedConnection();
					// Si realmente tenemos algo seleccionado
					if (connection is null)
						SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la conexión");
					else
					{
						Controllers.Exporter.ExportTablesProcessor processor = new(SolutionViewModel, connection, viewModel.TreeConnection.GetSelectedTables(),
																					viewModel.OutputPath, viewModel.FormatType, viewModel.BlockSize,
																					viewModel.FileCsvViewModel.GetCsvParameters());

							// Encola el proceso
							await SolutionViewModel.MainController.MainWindowController.EnqueueProcessAsync(processor, cancellationToken);
							// Log
							SolutionViewModel.MainController.Logger.LogInformation($"Exportando tablas de {connection.Name}");
					}
				}
		}
	}

	/// <summary>
	///		Importa un archivo sobre una tabla
	/// </summary>
	public async Task ImportDataBaseAsync(ConnectionModel connection, ConnectionTableModel? table, CancellationToken cancellationToken)
	{
		if (IsExecuting)
			SolutionViewModel.MainController.SystemController.ShowMessage("No se pueden importar los datos en este momento. Espere que finalice la ejecución de los scripts");
		else if (connection is null)
			SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una conexión");
		else
		{
			ImportDatabaseViewModel viewModel = new(connection, SolutionViewModel);

				// Carga los datos
				await viewModel.InitializeAsync(table, cancellationToken);
				// Muestra la ventana
				if (SolutionViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				{
					// Si realmente tenemos algo seleccionado
					if (connection is null)
						SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la conexión");
					else if (string.IsNullOrWhiteSpace(viewModel.FileName))
						SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el nombre de archivo");
					else
					{
						ConnectionTableModel? selectedTable = viewModel.GetSelectedTable();

							if (selectedTable is not null)
							{
								Controllers.Exporter.ImportFilesProcessor processor = new(SolutionViewModel, connection,
																						 viewModel.FileName, selectedTable,
																						 viewModel.GetColumnsImported(), 
																						 viewModel.BlockSize,
																						 viewModel.FileCsvViewModel.GetCsvParameters());

									// Encola el proceso
									await SolutionViewModel.MainController.MainWindowController.EnqueueProcessAsync(processor, cancellationToken);
									// Log
									SolutionViewModel.MainController.Logger.LogInformation($"Importando tablas de {connection.Name}");
							}
					}
				}
		}
	}

	/// <summary>
	///		Arranca los procesos para ejecución
	/// </summary>
	private void StartExecution()
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
	private void StopExecuting()
	{
		// Detiene los temporizadores
		_timer.Stop();
		_stopwatch.Stop();
		// Indica que ya no es está ejecutando
		ExecutionTime = _stopwatch.Elapsed.ToString();
		ExecutionTimeColor = BauMvvm.ViewModels.Media.MvvmColor.Black;
		IsExecuting = false;
		// Vacía el token de cancelación
		_cancellationToken = CancellationToken.None;
	}

	/// <summary>
	///		Obtiene el modo de ejecución
	/// </summary>
	private ExecutionMode GetExecutionMode(IDetailViewModel viewModel)
	{
		switch (viewModel)
		{
			case Files.ScriptFileViewModel fileViewModel:
				if (fileViewModel.FileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
					return ExecutionMode.ScriptSqlExtended;
				else if (fileViewModel.FileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
					return ExecutionMode.ScriptSql;
				else
					return ExecutionMode.Unknown;
			case ExecuteFilesViewModel _:
				return ExecutionMode.BatchSql;
			default:
				return ExecutionMode.Unknown;
		}
	}

	/// <summary>
	///		Prepara la ejecución de un script SQL o un lote de scripts SQL
	/// </summary>
	private async Task PrepareExecuteScriptSqlAsync(IDetailViewModel viewModel)
	{
		ConnectionModel? connection = GetSelectedConnection();

			if (connection is null)
				SolutionViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
			else
			{
				(ArgumentListModel arguments, string error) = GetParameters();

					// Si ha podido cargar el archivo de parámetros, ejecuta el script
					if (!string.IsNullOrWhiteSpace(error))
						SolutionViewModel.MainController.HostController.SystemController.ShowMessage(error);
					else
					{
						// Arranca la ejecución
						StartExecution();
						// Ejecuta la tarea
						try
						{
							// Ejecuta el script
							await ExecuteScriptSqlAsync(viewModel, connection, arguments, _cancellationToken);
							// Mensaje al usuario
							SolutionViewModel.MainController.MainWindowController
									.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
													  "Ejecución de script SQL",
													  "Ha terminado correctamente la ejecución del script SQL");
						}
						catch (Exception exception)
						{
							SolutionViewModel.MainController.Logger.LogError(exception, $"Error al ejecutar la consulta. {exception.Message}");
						}
						// Indica que ha finalizado la tarea y detiene el temporizador
						StopExecuting();
					}
			}
	}

	/// <summary>
	///		Ejecuta el script
	/// </summary>
	private async Task ExecuteScriptSqlAsync(IDetailViewModel viewModel, ConnectionModel connection, ArgumentListModel arguments, CancellationToken cancellationToken)
	{
		bool isExecuting = false;

			// Ejecuta sobre el ViewModel activo
			switch (viewModel)
			{
				case Files.ScriptFileViewModel fileViewModel:
						await fileViewModel.ExecuteSqlScriptAsync(connection, arguments, cancellationToken);
						isExecuting = true;
					break;
				case ExecuteFilesViewModel fileViewModel:
						await fileViewModel.ExecuteScriptAsync(connection, arguments, cancellationToken);
						isExecuting = true;
					break;
			}
			// Si no se está ejecutando nada, muestra un mensaje al usuario
			if (!isExecuting)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione la ventana de ejecución");
	}

	/// <summary>
	///		Cancela la ejecución del script
	/// </summary>
	private void CancelScriptExecution()
	{
		if (IsExecuting && _cancellationToken != CancellationToken.None && _cancellationToken.CanBeCanceled)
		{
			// Cancela las tareas
			_tokenSource.Cancel();
			// Log
			SolutionViewModel.MainController.Logger.LogInformation("Consulta cancelada");
			// Indica que ya no está en ejecución
			StopExecuting();
		}
	}

	/// <summary>
	///		Obtiene los parámetros de un archivo
	/// </summary>
	internal (ArgumentListModel arguments, string error) GetParameters()
	{
		ArgumentListModel arguments = new ArgumentListModel();
		string error = string.Empty;

			// Carga los parámetros si es necesario
			if (!string.IsNullOrWhiteSpace(ConnectionParametersFileName))
			{
				if (!File.Exists(ConnectionParametersFileName))
					error = "No se encuentra el archivo de parámetros";
				else 
					try
					{
						System.Data.DataTable table = new LibJsonConversor.JsonToDataTableConversor()
																.ConvertToDataTable(LibHelper.Files.HelperFiles.LoadTextFile(ConnectionParametersFileName));

							// Crea la colección de parámetros a partir de la tabla
							if (table.Rows.Count == 0)
								error = "No se ha encontrado ningún parámetro en el archivo";
							else
								foreach (System.Data.DataColumn column in table.Columns)
								{
									if (column.ColumnName.StartsWith("Constant.", StringComparison.CurrentCultureIgnoreCase))
										arguments.Constants.Add(column.ColumnName.Substring("Constant.".Length), table.Rows[0][column.Ordinal]);
									else
										arguments.Parameters.Add(column.ColumnName, table.Rows[0][column.Ordinal]);
								}
					}
					catch (Exception exception)
					{
						error = $"Error cuando se cargaba el archivo de parámetros. {exception.Message}";
					}
			}
			// Devuelve el resultado
			return (arguments, error);
	}

	/// <summary>
	///		Modifica el archivo de parámetros seleccionado
	/// </summary>
	private void UpdateParametersFile()
	{
		UpdateParametersFile(SolutionViewModel.MainController.DialogsController.OpenDialogLoad
									(string.Empty,
									 "Archivos de parámetros (*.json)|*.json|Todos los archivos (*.*)|*.*"));
	}

	/// <summary>
	///		Modifica el archivo de parámetros seleccionado
	/// </summary>
	private void UpdateParametersFile(string? fileName)
	{
		if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
		{
			// Guarda el nombre de archivo en la solución
			SolutionViewModel.Solution.LastConnectionParametersFileName = fileName;
			SolutionViewModel.SaveSolution();
			// Carga los nombres de archivos en el viewModel
			LoadConnectionFileNames();
			// y actualiza la lista de archivos de parámetros seleccionado
			LoadListParameterFiles();
		}
	}

	/// <summary>
	///		Abre el archivo de parámetros
	/// </summary>
	private void OpenParametersFile()
	{
		if (!string.IsNullOrWhiteSpace(ConnectionParametersFileName))
			SolutionViewModel.MainController.OpenWindow(new Files.ScriptFileViewModel(SolutionViewModel, ConnectionParametersFileName));
	}

	/// <summary>
	///		Ejecuta el script de un archivo o una carpeta
	/// </summary>
	private void ExecuteFolderFile(object? parameter)
	{
		if (parameter != null && parameter is string fileName && !string.IsNullOrWhiteSpace(fileName))
		{
			List<string> files = GetFilesFromPath(fileName, ".sql");

				// Ejecuta los archivos (si ha encontrado alguno)
				if (files.Count == 0)
					SolutionViewModel.MainController.SystemController.ShowMessage("No se encuentra ningún archivo SQL para ejecutar");
				else
					SolutionViewModel.MainController.OpenWindow(new ExecuteFilesViewModel(SolutionViewModel, files));
		}
	}

	/// <summary>
	///		Obtiene los archivos SQL de un directorio (o el archivo seleccionado)
	/// </summary>
	private List<string> GetFilesFromPath(string selectedFileName, string extension)
	{
		List<string> files = new();

			// Obtiene el archivo seleccionado o los archivos de un directorio
			if (Directory.Exists(selectedFileName))
			{
				// Obtiene la lista de todos los archivos
				files = LibHelper.Files.HelperFiles.ListRecursive(selectedFileName, $"*{extension}");
				// Quita los archivos que no coincidan con la máscara
				for (int index = files.Count - 1; index >= 0; index--)
					if (!files[index].EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
						files.RemoveAt(index);
				// Ordena los archivos
				files.Sort((first, second) => first.CompareIgnoreNullTo(second));
			}
			else if (selectedFileName.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
				files.Add(selectedFileName);
			// Devuelve la colección de archivos
			return files;
	}

	/// <summary>
	///		ViewModel de la solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }

	/// <summary>
	///		Combo de conexiones
	/// </summary>
	public ComboViewModel Connections
	{
		get { return _connections; }
		set { CheckObject(ref _connections, value); }
	}

	/// <summary>
	///		Nombre del archivo con los parámetros de ejecución sobre una conexión
	/// </summary>
	public string ConnectionParametersFileName
	{ 
		get { return _connectionParametersFileName; }
		set 
		{ 
			if (CheckProperty(ref _connectionParametersFileName, value))
			{
				if (string.IsNullOrWhiteSpace(value))
					ConnectionShortFileName = string.Empty;
				else
					ConnectionShortFileName = Path.GetFileName(value);
			}
		}
	}

	/// <summary>
	///		Nombre corto del archivo con parámetros de ejecución sobre una conexión
	/// </summary>
	public string ConnectionShortFileName
	{ 
		get { return _connectionShortFileName; }
		set { CheckProperty(ref _connectionShortFileName, value); }
	}

	/// <summary>
	///		Nombre del archivo con los parámetros de ejecución para los proyectos de ETL
	/// </summary>
	public string EtlParametersFileName
	{ 
		get { return _etlParametersFileName; }
		set 
		{
			if (CheckProperty(ref _etlParametersFileName, value))
			{
				if (string.IsNullOrWhiteSpace(value))
					EtlShortFileName = string.Empty;
				else
					EtlShortFileName = Path.GetFileName(value);
			}
		}
	}

	/// <summary>
	///		Nombre corto del archivo de parámetros de ejecución de los proyectos de ETL
	/// </summary>
	public string EtlShortFileName
	{ 
		get { return _etlShortFileName; }
		set { CheckProperty(ref _etlShortFileName, value); }
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
	///		Ultimos archivos de parámetros
	/// </summary>
	public ControlGenericListViewModel<LastParameterFileViewModel> LastParameterFiles
	{
		get { return _lastParametersFileViewModel; }
		set { CheckObject(ref _lastParametersFileViewModel, value); }
	}

	/// <summary>
	///		Comando de ejecución de un script
	/// </summary>
	public BaseCommand ExecuteScriptCommand { get; }

	/// <summary>
	///		Comando para cancelar la ejecución de un script
	/// </summary>
	public BaseCommand CancelScriptExecutionCommand { get; }

	/// <summary>
	///		Comando para abrir el archivo de parámetros
	/// </summary>
	public BaseCommand OpenParametersFileCommand { get; }

	/// <summary>
	///		Comando para modificar un archivo de parámetros
	/// </summary>
	public BaseCommand UpdateParametersFileCommand { get; }

	/// <summary>
	///		Ejecuta un archivo / directorio
	/// </summary>
	public BaseCommand ExecuteFileCommand { get; }
}
