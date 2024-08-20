using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel para <see cref="TimeModel"/>
/// </summary>
public class TimeEditableViewModel : BaseObservableObject
{
	// Variables privadas
	private string _timeText = default!, _project = default!, _task = default!, _startText = default!;
	private string? _endText;
	private DateTime _start, _end;
	private bool _isStarted;
	private System.Timers.Timer _timer;

	public TimeEditableViewModel(TimeScheduleViewModel timeScheduleViewModel)
	{
		// Asigna las propiedades
		TimeScheduleViewModel = timeScheduleViewModel;
		// Inicializa el viewModel
		InitViewModel();
		// Crea el temporizador
		_timer = new System.Timers.Timer(TimeSpan.FromSeconds(1));
		_timer.Elapsed += (sender, args) => UpdateTime();
		// Asigna los comandos
		StartCommand = new BaseCommand(_ => Start(), _ => !IsStarted)
								.AddListener(this, nameof(IsStarted));
		StopCommand = new BaseCommand(_ => Stop(), _ => IsStarted)
								.AddListener(this, nameof(IsStarted));
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		Project = "No project";
		Task = "No task";
	}

	/// <summary>
	///		Actualiza el tiempo
	/// </summary>
	private void UpdateTime()
	{
		if (IsStarted)
		{
			// Asigna los datos
			EndDate = DateTime.Now;
			EndText = $"{EndDate:HH:mm:ss}";
			Elapsed = TimeScheduleViewModel.FormatElapsed(StartDate, EndDate);
			// Lanza el evento de tarea actualizada
			TimeScheduleViewModel.TimeListViewModel.UpdateTotalElapsed(this);
		}
	}

	/// <summary>
	///		Arranca el temporizador
	/// </summary>
	private void Start()
	{
		// Asigna la fecha de inicio
		StartDate = DateTime.Now;
		EndDate = DateTime.Now;
		StartText = $"{StartDate:HH:mm:ss}";
		EndText = string.Empty;
		// Arranca el temporizador
		_timer.Start();
		// Indica que se ha arrancado
		IsStarted = true;
	}

	/// <summary>
	///		Detiene el temporizador
	/// </summary>
	private void Stop()
	{
		if (IsStarted)
		{
			// Guarda la fecha de fin
			EndDate = DateTime.Now;
			// Detiene el temporizador
			_timer.Stop();
			// Añade la tarea al control de tiempo y graba el archivo
			Save(Project, Task, StartDate, EndDate);
			// Vacía los datos
			StartText = string.Empty;
			EndText = string.Empty;
			Elapsed = string.Empty;
			// Indica que se ha detenido
			IsStarted = false;
		}
	}

	/// <summary>
	///		Arranca una nueva tarea
	/// </summary>
	internal void StartTask(string project, string task)
	{
		// Detiene el temporizador de la tarea actual (y lo añade a la lista)
		Stop();
		// Inicia esta tarea
		Start();
		// Ajusta los datos de la tarea actual
		Project = project;
		Task = task;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	private void Save(string project, string task, DateTime start, DateTime end)
	{
		TimeControlModel timeControl = TimeScheduleViewModel.LoadDate(DateOnly.FromDateTime(DateTime.Now));

			// Añade los datos de la tarea
			timeControl.Add(project, task, start, end);
			// Graba los datos
			TimeScheduleViewModel.Save(timeControl);
			// Actualiza la lista
			if (DateOnly.FromDateTime(TimeScheduleViewModel.TimeListViewModel.Date) == timeControl.Date)
				TimeScheduleViewModel.TimeListViewModel.Load(timeControl.Date);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public TimeScheduleViewModel TimeScheduleViewModel { get; }

	/// <summary>
	///		Nombre de proyecto
	/// </summary>
	public string Project 
	{ 
		get { return _project; }
		set { CheckProperty(ref _project, value); }
	}

	/// <summary>
	///		Nombre de la tarea
	/// </summary>
	public string Task
	{
		get { return _task; }
		set { CheckProperty(ref _task, value); }
	}

	/// <summary>
	///		Indica si está iniciado
	/// </summary>
	public bool IsStarted
	{
		get { return _isStarted; }
		set { CheckProperty(ref _isStarted, value); }
	}

	/// <summary>
	///		Fecha / hora de inicio
	/// </summary>
	public DateTime StartDate
	{
		get { return _start; }
		set { CheckProperty(ref _start, value); }
	}

	/// <summary>
	///		Fecha / hora de fine
	/// </summary>
	public DateTime EndDate
	{
		get { return _end; }
		set { CheckProperty(ref _end, value); }
	}

	/// <summary>
	///		Texto con el momento inicial
	/// </summary>
	public string StartText
	{
		get { return _startText; }
		set { CheckProperty(ref _startText, value); }
	}

	/// <summary>
	///		Texto con el momento de fin
	/// </summary>
	public string? EndText
	{
		get { return _endText; }
		set { CheckProperty(ref _endText, value); }
	}

	/// <summary>
	///		Texto con el tiempo pasado
	/// </summary>
	public string Elapsed
	{
		get { return _timeText; }
		set { CheckProperty(ref _timeText, value); }
	}

	/// <summary>
	///		Comando de inicio
	/// </summary>
	public BaseCommand StartCommand { get; }

	/// <summary>
	///		Comando de fin
	/// </summary>
	public BaseCommand StopCommand { get; }
}