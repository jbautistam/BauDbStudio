using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.Libraries.PluginsStudio.ViewModels.TasksQueue;

/// <summary>
///		Elemento de la ejecución de archivos
/// </summary>
public class TasksQueueItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Enumerados públicos 
	/// <summary>
	///		Estado de ejecución del archivo
	/// </summary>
	public enum Status
	{
		/// <summary>Encolado</summary>
		Enqueued,
		/// <summary>Cancelado</summary>
		Canceled,
		/// <summary>Iniciado</summary>
		Processing,
		/// <summary>Finalizado</summary>
		End,
		/// <summary>Error</summary>
		Error
	}
	// Variables privadas
	private string _statusText = string.Empty, _executionTime = string.Empty, _message = string.Empty;
	private long _actual, _total;
	private bool _isExecuting;
	private System.Timers.Timer _timer;
	private DateTime? _start;
	private Status _status;

	public TasksQueueItemViewModel(TasksQueueListViewModel viewModel, ProcessModel process, bool isBold = false, MvvmColor? foreground = null) 
										: base(process.Name, null, isBold, foreground)
	{
		// Asigna los objetos
		Process = process;
		// Asigna las propiedades
		ViewModel = viewModel;
		Text = $"{process.Group} - {process.Name}";
		State = Status.Enqueued;
		Actual = 0;
		Total = 200_000; // ... para que muestre la barra de progreso vacía
		// Inicializa los comandos
		CancelCommand = new BauMvvm.ViewModels.BaseCommand(_ => Cancel(), _ => IsExecuting)
									.AddListener(this, nameof(IsExecuting));
		DeleteCommand = new BauMvvm.ViewModels.BaseCommand(_ => Delete(), _ => !IsExecuting)
									.AddListener(this, nameof(IsExecuting));
		// Inicializa el temporizador
		_timer = new System.Timers.Timer(TimeSpan.FromSeconds(5));
		_timer.Elapsed += (sender, args) => Update();
		// Actualiza las propiedades
		Update();
	}

	/// <summary>
	///		Actualiza el estado
	/// </summary>
	internal void Update()
	{
		// Cambia el estado
		Foreground = MvvmColor.Black;
		switch (State)
		{	
			case Status.Canceled:
					StatusText = "Canceled";
					Foreground = MvvmColor.Gray;
					IsBold = false;
				break;
			case Status.Enqueued:
					StatusText = "Pending";
					IsBold = false;
				break;
			case Status.Processing:
					StatusText = "In progress";
					IsBold = true;
					_timer.Start();
				break;
			case Status.End:
					StatusText = "Success";
					Foreground = MvvmColor.Navy;
					IsBold = false;
				break;
			case Status.Error:
					StatusText = "Error";
					Foreground = MvvmColor.Red;
					IsBold = false;
				break;
		}
		// Cambia el tiempo de ejecución y el mensaje
		if (_start is null)
		{
			_start = DateTime.Now;
			ExecutionTime = string.Empty;
		}
		// Cambia el valor que indica si se está ejecutando
		if (State == Status.Processing)
			IsExecuting = true;
		else if (State == Status.End || State == Status.Canceled)
			IsExecuting = false;
		// Cierra el temporizador
		if (State == Status.End || State == Status.Error || State == Status.Canceled)
		{
			_timer.Stop();
			_timer.Dispose();
		}
		// Muestra el tiempo de ejecución
		ExecutionTime = $"{(DateTime.Now - _start).ToString()}";
	}

	/// <summary>
	///		Ejecuta el proceso
	/// </summary>
	internal async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		// Asigna los manejadores de eventos
		Process.Progress += (sender, args) => UpdateProgress(args);
		Process.Log += (sender, args) => UpdateLog(args);
		// Indica que el proceso está en ejecución
		State = Status.Processing;
		_start = DateTime.Now;
		// Ejecuta el proceso en un hilo
		await Task.Run(async () => await Process.ExecuteAsync(CancellationTokenSource.Token), cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	///		Modifica el log
	/// </summary>
	private void UpdateLog(LogEventArgs args)
	{
		// Cambia los estados
		switch (args.State)
		{
			case LogEventArgs.Status.Error:
					State = Status.Error;
					Message = args.Message;
				break;
			case LogEventArgs.Status.Success:
					if (State != Status.Canceled && State !=  Status.Error)
					{
						State = Status.End;
						Message = args.Message;
					}
				break;
			case LogEventArgs.Status.Warning:
			case LogEventArgs.Status.Info:
					State = Status.Processing;
					Message = args.Message;
				break;
		}
		// Actualiza la pantalla
		Update();
	}

	/// <summary>
	///		Modifica el progreso
	/// </summary>
	private void UpdateProgress(ProgressEventArgs args)
	{
		Actual = args.Actual;
		Total = args.Total;
		if (!string.IsNullOrWhiteSpace(args.Message))
			Message = args.Message;
	}

	/// <summary>
	///		Cancela la ejecución del proceso
	/// </summary>
	private void Cancel()
	{
		// Cancela el proceso
		CancellationTokenSource.Cancel();
		// Marca el estado
		State = Status.Canceled;
	}

	/// <summary>
	///		Borra del log el elemento
	/// </summary>
	private void Delete()
	{
		ViewModel.Delete(this);
	}

	/// <summary>
	///		Proceso
	/// </summary>
	public ProcessModel Process { get; }

	/// <summary>
	///		Token de cancelación
	/// </summary>
	public CancellationTokenSource CancellationTokenSource { get; } = new();

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public TasksQueueListViewModel ViewModel { get; }

	/// <summary>
	///		Indica si se está ejecutando
	/// </summary>
	public bool IsExecuting
	{
		get { return _isExecuting; }
		set { CheckProperty(ref _isExecuting, value); }
	}

	/// <summary>
	///		Estado del elemento
	/// </summary>
	public Status State
	{
		get { return _status; }
		set 
		{ 
			CheckProperty(ref _status, value); 
			Update();
		}
	}

	/// <summary>
	///		Texto del estado
	/// </summary>
	public string StatusText
	{
		get { return _statusText; }
		set { CheckProperty(ref _statusText, value); }
	}

	/// <summary>
	///		Tiempo de ejecución
	/// </summary>
	public string ExecutionTime
	{
		get { return _executionTime; }
		set { CheckProperty(ref _executionTime, value); }
	}

	/// <summary>
	///		Mensaje
	/// </summary>
	public string Message
	{
		get { return _message; }
		set { CheckProperty(ref _message, value); }
	}

	/// <summary>
	///		Progreso actual
	/// </summary>
	public long Actual
	{
		get { return _actual; }
		set { CheckProperty(ref _actual, value); }
	}

	/// <summary>
	///		Progreso total
	/// </summary>
	public long Total
	{
		get { return _total; }
		set { CheckProperty(ref _total, value); }
	}

	/// <summary>
	///		Comando para cancelar la ejecución
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand CancelCommand { get; }

	/// <summary>
	///		Comando para eliminar el elemento de la lista
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand DeleteCommand { get; }
}
