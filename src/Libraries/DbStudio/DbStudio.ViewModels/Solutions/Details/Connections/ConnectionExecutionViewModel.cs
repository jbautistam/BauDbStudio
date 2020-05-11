using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Application.Connections.Models;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Connections
{
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
			/// <summary>Xml de procesos</summary>
			JobsXml,
			/// <summary>Lote de scripts SQL</summary>
			BatchSql
		}

		// Variables privadas
		private ComboViewModel _connections;
		private string _connectionParametersFileName, _connectionShortFileName;
		private string _etlParametersFileName, _etlShortFileName;
		private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor;
		private string _executionTime;
		private bool _isExecuting;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _cancellationToken = CancellationToken.None;
		private System.Timers.Timer _timer;
		private System.Diagnostics.Stopwatch _stopwatch;

		public ConnectionExecutionViewModel(SolutionViewModel solutionViewModel)
		{
			// Asigna la solución y el viewmodel de parámetros de ejecución
			SolutionViewModel = solutionViewModel;
			// Asigna los comandos
			ExecuteScripCommand = new BaseCommand(async _ => await ExecuteScriptAsync(), _ => !IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			CancelScriptExecutionCommand = new BaseCommand(_ => CancelScriptExecution(), _ => IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			UpdateParametersFileCommand = new BaseCommand(_ => UpdateParametersFile(), _ => !string.IsNullOrWhiteSpace(ConnectionParametersFileName));
			OpenParametersFileCommand = new BaseCommand(_ => OpenParametersFile());
		}

		/// <summary>
		///		Inicializa el viewModel (cuando ya se ha cargado la solución)
		/// </summary>
		public void Initialize()
		{
			// Carga el nombre del archivo de parámetros de la solución
			LoadConnectionFileNames();
			// Inicializa el combo
			Connections = new ComboViewModel(this);
			// Carga las conexiones
			if (SolutionViewModel.Solution.Connections.Count == 0)
				Connections.AddItem(null, "<Seleccione una conexión>", null);
			else
				foreach (ConnectionModel connection in SolutionViewModel.Solution.Connections)
					Connections.AddItem(null, connection.Name, connection);
			// Si no se ha seleccionado nada, selecciona el primer elemento
			if (Connections.SelectedItem == null)
				Connections.SelectedItem = Connections.Items[0];
		}

		/// <summary>
		///		Carga los nombres de archivos de la solución
		/// </summary>
		private void LoadConnectionFileNames()
		{
			// Carga el nombre del archivo de parámetros de conexión de la solución
			if (System.IO.File.Exists(SolutionViewModel.Solution.LastConnectionParametersFileName))
				ConnectionParametersFileName = SolutionViewModel.Solution.LastConnectionParametersFileName;
			else
				ConnectionParametersFileName = string.Empty;
			// Carga el nombre del archivo de parámetros de conexión de los proyectos ETl de la solución
			if (System.IO.File.Exists(SolutionViewModel.Solution.LastEtlParametersFileName))
				EtlParametersFileName = SolutionViewModel.Solution.LastEtlParametersFileName;
			else
				EtlParametersFileName = string.Empty;

		}

		/// <summary>
		///		Obtiene la conexión seleccionada en el combo
		/// </summary>
		public ConnectionModel GetSelectedConnection()
		{
			if (Connections.SelectedItem?.Tag is ConnectionModel connection)
				return connection;
			else
				return null;
		}

		/// <summary>
		///		Ejecuta un script
		/// </summary>
		private async Task ExecuteScriptAsync()
		{
			if (IsExecuting)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Ya se está ejecutando una consulta");
			else
			{
				IDetailViewModel selectedViewModel = SolutionViewModel.MainViewModel.MainController.GetActiveDetails();

					switch (GetExecutionMode(selectedViewModel))
					{
						case ExecutionMode.Unknown:
								SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se puede ejecutar este archivo");
							break;
						case ExecutionMode.BatchSql:
						case ExecutionMode.ScriptSql:
								await PrepareExecuteScriptSqlAsync(selectedViewModel);
							break;
						case ExecutionMode.JobsXml:
								await PrepareExecuteScriptXmlAsync(selectedViewModel);
							break;
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
			// Log
			SolutionViewModel.MainViewModel.MainController.Logger.Flush();
		}

		/// <summary>
		///		Obtiene el modo de ejecución
		/// </summary>
		private ExecutionMode GetExecutionMode(IDetailViewModel viewModel)
		{
			switch (viewModel)
			{
				case Files.FileViewModel fileViewModel:
					if (fileViewModel.FileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
						return ExecutionMode.ScriptSql;
					else if (fileViewModel.FileName.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
						return ExecutionMode.JobsXml;
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
			ConnectionModel connection = GetSelectedConnection();

				if (connection == null)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
				else
				{
					(ArgumentListModel arguments, string error) = GetParameters();

						// Si ha podido cargar el archivo de parámetros, ejecuta el script
						if (!string.IsNullOrWhiteSpace(error))
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
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
								SolutionViewModel.MainViewModel.MainController.HostController.SystemController
										.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
														  "Ejecución de script SQL",
														  "Ha terminado correctamente la ejecución del script SQL", 
														  TimeSpan.FromSeconds(10));
							}
							catch (Exception exception)
							{
								SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al ejecutar la consulta. {exception.Message}");
							}
							// Indica que ha finalizado la tarea y detiene el temporizador
							StopExecuting();
						}
				}
		}

		/// <summary>
		///		Prepara la ejecución de un script XML
		/// </summary>
		private async Task PrepareExecuteScriptXmlAsync(IDetailViewModel viewModel)
		{
			if (viewModel is Files.FileViewModel fileViewModel)
			{
				if (!fileViewModel.FileName.EndsWith(".xml"))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se pueden ejecutar scripts de este tipo de archivo");
				else if (fileViewModel.IsUpdated)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Grabe el contenido del archivo antes de ejecutar");
				else if (string.IsNullOrWhiteSpace(fileViewModel.Content))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el proyecto para ejecutar");
				else if (string.IsNullOrWhiteSpace(EtlParametersFileName) || !System.IO.File.Exists(EtlParametersFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un archivo de contexto");
				else
				{
					// Cambia el último directorio seleccionado
					SolutionViewModel.MainViewModel.LastPathSelected = System.IO.Path.GetDirectoryName(EtlParametersFileName);
					// Arranca la ejecución
					StartExecution();
					// Ejecuta la tarea
					try
					{
						// Ejecuta el script XML
						await fileViewModel.ExecuteXmlScriptAsync(EtlParametersFileName, _cancellationToken);
						// Muestra el mensaje al usuario
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController
								.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Information,
													"Ejecución de script XML",
													"Ha terminado correctamente la ejecución del script", TimeSpan.FromSeconds(10));
					}
					catch (Exception exception)
					{
						SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al ejecutar la consulta. {exception.Message}");
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
					case Files.FileViewModel fileViewModel:
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
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione la ventana de ejecución");
		}

		/// <summary>
		///		Cancela la ejecución del script
		/// </summary>
		private void CancelScriptExecution()
		{
			if (IsExecuting && _cancellationToken != null && _cancellationToken != CancellationToken.None)
			{
				if (_cancellationToken.CanBeCanceled)
				{
					// Cancela las tareas
					_tokenSource.Cancel();
					// Log
					SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error("Consulta cancelada");
					// Indica que ya no está en ejecución
					StopExecuting();
				}
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
					if (!System.IO.File.Exists(ConnectionParametersFileName))
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
			ConnectionParametersExecutionViewModel parametersViewModel = new ConnectionParametersExecutionViewModel(this);

				if (SolutionViewModel.MainViewModel.MainController.OpenDialog(parametersViewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				{
					// Guarda los nombres de archivos en la solución
					SolutionViewModel.Solution.LastConnectionParametersFileName = parametersViewModel.ConnectionParametersFileName;
					SolutionViewModel.Solution.LastEtlParametersFileName = parametersViewModel.EtlParametersFileName;
					SolutionViewModel.MainViewModel.SaveSolution();
					// Carga los nombres de archivos en el viewModel
					LoadConnectionFileNames();
				}
		}

		/// <summary>
		///		Abre el archivo de parámetros
		/// </summary>
		private void OpenParametersFile()
		{
			if (!string.IsNullOrWhiteSpace(ConnectionParametersFileName))
				SolutionViewModel.MainViewModel.MainController.OpenWindow(new Files.FileViewModel(SolutionViewModel, ConnectionParametersFileName));
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

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
						ConnectionShortFileName = System.IO.Path.GetFileName(value);
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
						EtlShortFileName = System.IO.Path.GetFileName(value);
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
		///		Comando de ejecución de un script
		/// </summary>
		public BaseCommand ExecuteScripCommand { get; }

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
	}
}
