using Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel para <see cref="TimeModel"/>
/// </summary>
public class TimeViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _project = default!, _task = default!, _elapsed = default!;
	private string? _remarks;
	private DateTime _date;
	private TimeOnly _timeStart, _timeEnd;

	public TimeViewModel(TimeScheduleViewModel timeScheduleViewModel, TimeModel? time)
	{
		TimeScheduleViewModel = timeScheduleViewModel;
		InitViewModel(time);
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel(TimeModel? time)
	{
		// Crea el objeto si no existía
		if (time is null)
		{
			// Crea un nuevo objeto
			time = new TimeModel(new TaskModel(new ProjectModel()));
			// Asigna las propiedades
			time.Task.Name = TimeScheduleViewModel.ActualTimeViewModel.Task;
			time.Task.Project.Name = TimeScheduleViewModel.ActualTimeViewModel.Project;
			time.Start = GetEditDate(TimeScheduleViewModel.TimeListViewModel.Date, -20);
			time.End = GetEditDate(TimeScheduleViewModel.TimeListViewModel.Date, 0);
		}
		// Guarda los datos de la fecha
		Time = time;
		// Asigna los datos al viewModel
		Project = Time.Task.Project.Name;
		Task = Time.Task.Name;
		Remarks = time.Remarks;
		Date = time.Start;
		TimeStart = TimeOnly.FromDateTime(time.Start);
		TimeEnd = TimeOnly.FromDateTime(time.End);
		UpdateElapsed();

		// Obtiene la fecha de edición
		DateTime GetEditDate(DateTime start, int minutes)
		{
			DateTime date = new(start.Year, start.Month, start.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

				// Añade los minutos especificados a la fecha
				return date.AddMinutes(minutes);
		}
	}

	/// <summary>
	///		Modifica el tiempo
	/// </summary>
	private void UpdateElapsed()
	{
		(DateTime start, DateTime end) = GetDates();

			// Texto con el tiempo pasado
			Elapsed = TimeScheduleViewModel.FormatElapsed(start, end);
	}

	/// <summary>
	///		Obtiene las fechas a partir de las horas
	/// </summary>
	private (DateTime start, DateTime end) GetDates()
	{
		DateTime start = CreateDate(Time.Start, TimeStart);
		DateTime end;

			// Obtiene la fecha final
			if (TimeEnd < TimeStart)
				end = CreateDate(start.AddDays(1), TimeEnd);
			else
				end = CreateDate(start, TimeEnd);
			// Devuelve las fechas
			return (start, end);

		// Crea una fecha con los datos de fecha y hora
		DateTime CreateDate(DateTime date, TimeOnly time) => new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Project))
				TimeScheduleViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Enter the project name");
			else if (string.IsNullOrWhiteSpace(Task))
				TimeScheduleViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Enter the task name");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{
			// Asigna los datos
			Time.Task.Project.Name = Project;
			Time.Task.Name = Task;
			Time.Remarks = Remarks;
			(Time.Start, Time.End) = GetDates();
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		Clona el ViewModel
	/// </summary>
	internal TimeViewModel Clone() => new TimeViewModel(TimeScheduleViewModel, Time);

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public TimeScheduleViewModel TimeScheduleViewModel { get; }

	/// <summary>
	///		Datos de la hora
	/// </summary>
	public TimeModel Time { get; private set; } = default!;

	/// <summary>
	///		Proyecto
	/// </summary>
	public string Project
	{
		get { return _project; }
		set { CheckProperty(ref _project, value); }
	}

	/// <summary>
	///		Tarea
	/// </summary>
	public string Task
	{
		get { return _task; }
		set { CheckProperty(ref _task, value); }
	}

	/// <summary>
	///		Fecha de la tarea
	/// </summary>
	public DateTime Date
	{
		get { return _date; }
		set { CheckProperty(ref _date, value); }
	}

	/// <summary>
	///		Hora de inicio
	/// </summary>
	public TimeOnly TimeStart
	{
		get { return _timeStart; }
		set 
		{ 
			if (CheckProperty(ref _timeStart, value))
				UpdateElapsed();
		}
	}

	/// <summary>
	///		Hora de fin
	/// </summary>
	public TimeOnly TimeEnd
	{
		get { return _timeEnd; }
		set 
		{ 
			if (CheckProperty(ref _timeEnd, value))
				UpdateElapsed();
		}
	}

	/// <summary>
	///		Tiempo
	/// </summary>
	public string Elapsed
	{
		get { return _elapsed; }
		set { CheckProperty(ref _elapsed, value); }
	}

	/// <summary>
	///		Comentarios
	/// </summary>
	public string? Remarks
	{
		get { return _remarks; }
		set { CheckProperty(ref _remarks, value); }
	}
}