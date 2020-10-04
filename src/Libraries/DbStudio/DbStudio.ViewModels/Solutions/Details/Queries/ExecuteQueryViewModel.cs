using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbStudio.ViewModels.Controllers.Exporter;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Connections;
using Bau.Libraries.DbScripts.Manager.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Queries
{
	/// <summary>
	///		ViewModel para ejecución de una consulta
	/// </summary>
	public class ExecuteQueryViewModel : BaseObservableObject, IDetailViewModel
	{
		// Eventos públicos
		public event EventHandler<Controllers.EventArguments.EditorSelectedTextRequiredEventArgs> SelectedTextRequired;
		// Variables privadas
		private string _header, _fileName, _query, _lastQuery, _executionTime, _executionPlanText;
		private DataTable _dataResults;
		private ComboConnectionsViewModel _comboConnectionsViewModel;
		private int _actualPage, _pageSize;
		private bool _paginateQuery;
		private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor;
		private bool _isExecuting;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _cancellationToken = CancellationToken.None;
		private System.Timers.Timer _timer;
		private System.Diagnostics.Stopwatch _stopwatch;

		public ExecuteQueryViewModel(SolutionViewModel solutionViewModel, string selectedConnection, string query) : base(false)
		{
			// Asigna los viewModel
			SolutionViewModel = solutionViewModel;
			ChartViewModel = new ChartViewModel(this);
			// Asigna las propiedades
			Query = query;
			Header = "Consulta";
			PaginateQuery = false;
			ActualPage = 1;
			PageSize = 10_000;
			// Carga el combo de conexiones
			ComboConnections = new ComboConnectionsViewModel(solutionViewModel, selectedConnection);
			// Asigna los comandos
			ProcessCommand = new BaseCommand(async _ => await ExecuteQueryAsync(), _ => !IsExecuting)
									.AddListener(this, nameof(IsExecuting));
			CancelQueryCommand = new BaseCommand(_ => CancelQuery(), _ => IsExecuting)
													.AddListener(this, nameof(IsExecuting));
			ShowExecutionPlanCommand = new BaseCommand(async _ => await ShowExecutionPlanAsync(), _ => !IsExecuting)
											.AddListener(this, nameof(IsExecuting));
			ExportCommand = new BaseCommand(async _ => await ExportAsync(), _ => !IsExecuting)
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
		///		Ejecuta la consulta
		/// </summary>
		private async Task ExecuteQueryAsync()
		{
			string querySelected = GetEditorSelectedText();

				if (string.IsNullOrWhiteSpace(querySelected))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
				else
				{
					ConnectionModel connection = ComboConnections.GetSelectedConnection();

						if (connection == null)
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
						else
						{
							(ArgumentListModel arguments, string error) = SolutionViewModel.ConnectionExecutionViewModel.GetParameters();

								if (!string.IsNullOrWhiteSpace(error))
									SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
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
										if (PaginateQuery)
											DataResults = await SolutionViewModel.MainViewModel.Manager.GetDatatableQueryAsync(connection, querySelected, arguments, 
																															   ActualPage, PageSize, 
																															   connection.TimeoutExecuteScript, 
																															   _cancellationToken);
										else
											DataResults = await SolutionViewModel.MainViewModel.Manager.GetDatatableQueryAsync(connection, querySelected, arguments, 0, 0,
																															   connection.TimeoutExecuteScript, 
																															   _cancellationToken);
										// Guarda la consulta que se acaba de lanzar
										_lastQuery = querySelected;
									}
									catch (Exception exception)
									{
										SolutionViewModel.MainViewModel.Manager.Logger.Default.LogItems.Error($"Error al ejecutar la consulta. {exception.Message}");
									}
									// Detiene la ejecucion
									StopQuery();
								}
						}
				}
		}

		/// <summary>
		///		Muestra el plan de ejecución
		/// </summary>
		private async Task ShowExecutionPlanAsync()
		{
			string query = GetEditorSelectedText();

				if (string.IsNullOrWhiteSpace(query))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta");
				else
				{
					ConnectionModel connection = ComboConnections.GetSelectedConnection();

						if (connection == null)
							SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
						else
						{
							(ArgumentListModel arguments, string error) = SolutionViewModel.ConnectionExecutionViewModel.GetParameters();

								if (!string.IsNullOrWhiteSpace(error))
									SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
								else
									try
									{
										DataTable table = await SolutionViewModel.MainViewModel.Manager.GetExecutionPlanAsync(connection, query, arguments, 
																															  connection.TimeoutExecuteScript, _cancellationToken);
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
										SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al obtener el plan de ejecución. {exception.Message}");
									}
						}
				}
		}

		/// <summary>
		///		Lanza un evento para solicitar el texto seleccionado al editor
		/// </summary>
		private string GetEditorSelectedText()
		{
			Controllers.EventArguments.EditorSelectedTextRequiredEventArgs eventArgs = new Controllers.EventArguments.EditorSelectedTextRequiredEventArgs();

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
			// Prepara el gráfico
			PrepareDraw();
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
			// Log
			SolutionViewModel.MainViewModel.Manager.Logger.Flush();
			// Prepara el gráfico
			PrepareDraw();
		}

		/// <summary>
		///		Cancela la ejecución de la consulta
		/// </summary>
		private void CancelQuery()
		{
			if (IsExecuting && _cancellationToken != null && _cancellationToken != CancellationToken.None)
			{
				if (_cancellationToken.CanBeCanceled)
				{
					// Cancela las tareas
					_tokenSource.Cancel();
					// Log
					SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Info("Consulta cancelada");
					// Indica que ya no está en ejecución
					StopQuery();
				}
			}
		}

		/// <summary>
		///		Prepara el dibujo
		/// </summary>
		private void PrepareDraw()
		{
			if (IsExecuting || DataResults == null || DataResults.Rows?.Count == 0)
				ChartViewModel.CanDraw = false;
			else 
				ChartViewModel.PrepareSeries();
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
				string newFileName = SolutionViewModel.MainViewModel.OpenDialogSave("New query.sql", "Script SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*", ".sql");

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
				SolutionViewModel.TreeFoldersViewModel.Load();
				// Indica que no ha habido modificaciones
				IsUpdated = false;
			}
		}

		/// <summary>
		///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return "¿Desea grabar la consulta antes de continuar?";
		}

		/// <summary>
		///		Exporta la tabla de datos
		/// </summary>
		private async Task ExportAsync()
		{
			if (string.IsNullOrWhiteSpace(Query))
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
			else
			{
				ConnectionModel connection = ComboConnections.GetSelectedConnection();

					if (connection == null)
						SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
					else
					{
						(ArgumentListModel arguments, string error) = SolutionViewModel.ConnectionExecutionViewModel.GetParameters();

							if (!string.IsNullOrWhiteSpace(error))
								SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
							else
							{
								string fileName = SolutionViewModel.MainViewModel.OpenDialogSave(ExportDataController.DefaultFileName, 
																								 ExportDataController.MaskExportFiles, 
																								 ExportDataController.DefaultExtension);

									if (!string.IsNullOrEmpty(fileName))
									{
										// Arranca los temporizadores e inicializa el interface de usuario con el inicio de la consulta
										StartQuery();
										// Ejecuta la exportación
										try
										{
											(bool exported, string exportError) = await ExportAsync(connection, arguments, fileName, _cancellationToken);

												if (!exported)
													SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage(error);
												else
													SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("El archivo se ha grabado correctamente");
										}
										catch (Exception exception)
										{
											SolutionViewModel.MainViewModel.MainController.Logger.Default.LogItems.Error($"Error al grabar el archivo '{fileName}'", exception);
											SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"Error al grabar el archivo '{fileName}'");
										}
										// Detiene la consulta
										StopQuery();
									}
							}
					}
			}
		}

		/// <summary>
		///		Exporta el resultado de una consulta a un archivo 
		/// </summary>
		private async Task<(bool exported, string error)> ExportAsync(ConnectionModel connection, ArgumentListModel arguments, 
																	  string fileName, CancellationToken cancellationToken)
		{
			using (System.Data.Common.DbDataReader reader = await SolutionViewModel.MainViewModel.Manager.ExecuteReaderAsync(connection, Query, arguments,
																															 connection.TimeoutExecuteScript,
																															 cancellationToken))
			{
				return await new ExportDataController().ExportAsync(SolutionViewModel.MainViewModel.MainController.Logger, fileName, reader, cancellationToken);
			}
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		ViewModel para el gráfico
		/// </summary>
		public ChartViewModel ChartViewModel { get; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header 
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + Guid.NewGuid().ToString(); } 
		}

		/// <summary>
		///		Consulta a ejecutar
		/// </summary>
		public string Query
		{
			get { return _query; }
			set { CheckProperty(ref _query, value); }
		}

		/// <summary>
		///		Resultados de la ejecución de la consulta
		/// </summary>
		public DataTable DataResults
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
			set 
			{ 
				if (CheckProperty(ref _fileName, value))
				{
					if (!string.IsNullOrWhiteSpace(value))
						Header = System.IO.Path.GetFileName(value);
					else
						Header = "Consulta";
				}
			}
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
}