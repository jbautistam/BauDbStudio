using System;
using System.Data;
using System.Threading.Tasks;
using System.Threading;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Connections;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries
{
	/// <summary>
	///		ViewModel para mostrar un informe
	/// </summary>
	public class ReportViewModel : BaseObservableObject, IDetailViewModel
	{
		// Variables privadas
		private string _header, _sqlQuery, _executionTime;
		private DataTable _dataResults;
		private ComboConnectionsViewModel _comboConnectionsViewModel;
		private TreeQueryReportViewModel _treeColumns;
		private BauMvvm.ViewModels.Media.MvvmColor _executionTimeColor;
		private bool _isExecuting;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _cancellationToken = CancellationToken.None;
		private System.Timers.Timer _timer;
		private System.Diagnostics.Stopwatch _stopwatch;

		public ReportViewModel(ReportingSolutionViewModel reportingSolutionViewModel, ReportModel report) : base(false)
		{
			// Asigna las propiedades
			ReportingSolutionViewModel = reportingSolutionViewModel;
			Report = report;
			Header = report.Name;
			// Carga el combo de conexiones
			ComboConnections = new ComboConnectionsViewModel(ReportingSolutionViewModel.SolutionViewModel, string.Empty);
			// Inicializa el árbol de campos
			TreeColumns = new TreeQueryReportViewModel(this);
			// Inicializa los comandos
			ProcessCommand = new BaseCommand(async _ => await ExecuteQueryAsync());
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
			ConnectionModel connection = ComboConnections.GetSelectedConnection();

				if (connection == null)
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
				else
				{
					// Limpia los datos
					DataResults = null;
					// Arranca la consulta
					StartQuery();
					// Ejecuta la consulta
					try
					{
						// Guarda la consulta para el editor (en bonito)
						SqlQuery = GetQueryRequested();
						// Si hay alguna consulta la ejecuta (la original, no la modificada para que aparezca en el editor, por si acaso el embellecedor modifica la cadena, que no debería)
						if (string.IsNullOrWhiteSpace(SqlQuery))
							ReportingSolutionViewModel.SolutionViewModel.MainViewModel.Manager.Logger.Default.LogItems.Error("No se ha podido crear una consulta para el informe");
						else
							DataResults = await ReportingSolutionViewModel.SolutionViewModel.MainViewModel.Manager
													.GetDatatableQueryAsync(connection, SqlQuery, null, 0, 0,
																			connection.TimeoutExecuteScript, 
																			_cancellationToken);
					}
					catch (Exception exception)
					{
						ReportingSolutionViewModel.SolutionViewModel.MainViewModel.Manager.Logger.Default.LogItems.Error($"Error al ejecutar la consulta. {exception.Message}");
					}
					// Detiene la ejecucion
					StopQuery();
				}
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
			// Log
			ReportingSolutionViewModel.SolutionViewModel.MainViewModel.Manager.Logger.Flush();
		}

		/// <summary>
		///		Obtiene la consulta solicitada para este informe
		/// </summary>
		private string GetQueryRequested()
		{
			return ReportingSolutionViewModel.ReportingManager.GetSqlResponse(TreeColumns.GetReportRequest());
		}

		/// <summary>
		///		Obtiene el mensaje para grabar y cerrar
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return string.Empty;
		}

		/// <summary>
		///		Graba el contenido
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// No hace nada, sólo implementa la interface
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

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
		public string TabId
		{
			get { return $"{GetType().ToString()}_{Report.GlobalId}"; }
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
		///		Arbol de columnas de la consulta
		/// </summary>
		public TreeQueryReportViewModel TreeColumns
		{
			get { return _treeColumns; }
			set { CheckObject(ref _treeColumns, value); }
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
		///		Consulta SQL generada para el informe
		/// </summary>
		public string SqlQuery
		{
			get { return _sqlQuery; }
			set { CheckProperty(ref _sqlQuery, value); }
		}

		/// <summary>
		///		Comando para ejecutar la consulta
		/// </summary>
		public BaseCommand ProcessCommand { get; }
	}
}