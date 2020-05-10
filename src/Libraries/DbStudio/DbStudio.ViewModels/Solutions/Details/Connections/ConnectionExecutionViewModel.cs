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
		private string _fileNameParameters, _shortFileName, _executionTime;
		private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor;
		private bool _isExecuting;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _cancellationToken = CancellationToken.None;
		private System.Timers.Timer _timer;
		private System.Diagnostics.Stopwatch _stopwatch;

		public ConnectionExecutionViewModel(SolutionViewModel solutionViewModel)
		{
			// Asigna la solución
			SolutionViewModel = solutionViewModel;
			// Asigna los comandos
			ExecuteScripCommand = new BaseCommand(async _ => await ExecuteScriptAsync(), _ => !IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			CancelScriptExecutionCommand = new BaseCommand(_ => CancelScriptExecution(), _ => IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			OpenParametersFileCommand = new BaseCommand(_ => OpenParametersFile(), _ => !string.IsNullOrWhiteSpace(FileNameParameters))
												.AddListener(this, nameof(FileNameParameters));
			UpdateParametersFileCommand = new BaseCommand(_ => UpdateParametersFile());
			RemoveParametersFileCommand = new BaseCommand(_ => FileNameParameters = string.Empty);
		}

		/// <summary>
		///		Inicializa el viewModel (cuando ya se ha cargado la solución)
		/// </summary>
		public void Initialize()
		{
			// Carga el nombre del archivo de parámetros de la solución
			if (System.IO.File.Exists(SolutionViewModel.Solution.LastParametersFileName))
				FileNameParameters = SolutionViewModel.Solution.LastParametersFileName;
			else
				FileNameParameters = string.Empty;
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
				else 
				{
					string contextFileName = SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogLoad(SolutionViewModel.MainViewModel.LastPathSelected,
																																			"Archivos de contexto (*.xml)|*.xml|Todos los archivos|*.*",
																																			"context.xml", "*.xml");

						if (string.IsNullOrWhiteSpace(contextFileName) || !System.IO.File.Exists(contextFileName))
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un archivo de contexto");
						else
						{
							// Cambia el último directorio seleccionado
							SolutionViewModel.MainViewModel.LastPathSelected = System.IO.Path.GetDirectoryName(contextFileName);
							// Arranca la ejecución
							StartExecution();
							// Ejecuta la tarea
							try
							{
								// Ejecuta el script XML
								await fileViewModel.ExecuteXmlScriptAsync(contextFileName, _cancellationToken);
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
				if (!string.IsNullOrWhiteSpace(FileNameParameters))
				{
					if (!System.IO.File.Exists(FileNameParameters))
						error = "No se encuentra el archivo de parámetros";
					else 
						try
						{
							System.Data.DataTable table = new LibJsonConversor.JsonToDataTableConversor().ConvertToDataTable(LibHelper.Files.HelperFiles.LoadTextFile(FileNameParameters));

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
		///		Abre el archivo de parámetros
		/// </summary>
		private void OpenParametersFile()
		{
			if (!string.IsNullOrWhiteSpace(FileNameParameters))
				SolutionViewModel.MainViewModel.MainController.OpenWindow(new Files.FileViewModel(SolutionViewModel, FileNameParameters));
		}

		/// <summary>
		///		Modifica el archivo de parámetros seleccionado
		/// </summary>
		private void UpdateParametersFile()
		{
			string fileName = SolutionViewModel.MainViewModel.MainController.HostController.DialogsController.OpenDialogLoad
									(SolutionViewModel.MainViewModel.LastPathSelected, "Archivo json (*.json)|*.json|Todos los archivos (*.*)|*.*", null, "*.json");

				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
				{
					// Guarda el nombre de archivo en los parámetros
					FileNameParameters = fileName;
					// Guarda el nombre de archivo en la solución
					SolutionViewModel.Solution.LastParametersFileName = fileName;
					SolutionViewModel.MainViewModel.SaveSolution();
					// Cambia el último directorio seleccionado
					SolutionViewModel.MainViewModel.LastPathSelected = System.IO.Path.GetDirectoryName(FileNameParameters);
				}
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
		///		Nombre del archivo de parámetros
		/// </summary>
		public string FileNameParameters
		{
			get { return _fileNameParameters; }
			set 
			{ 
				if (CheckProperty(ref _fileNameParameters, value))
				{
					if (string.IsNullOrWhiteSpace(value))
						ShortFileName = string.Empty;
					else
						ShortFileName = System.IO.Path.GetFileName(value);
				}
			}
		}

		/// <summary>
		///		Nombre corto del archivo de parámetros
		/// </summary>
		public string ShortFileName
		{
			get { return _shortFileName; }
			set { CheckProperty(ref _shortFileName, value); }
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
		///		Comando para abrir un archivo de parámetros
		/// </summary>
		public BaseCommand OpenParametersFileCommand { get; }

		/// <summary>
		///		Comando para modificar un archivo de parámetros
		/// </summary>
		public BaseCommand UpdateParametersFileCommand { get; }

		/// <summary>
		///		Comando para quitar el archivo de parámetros
		/// </summary>
		public BaseCommand RemoveParametersFileCommand { get; }
	}
}
