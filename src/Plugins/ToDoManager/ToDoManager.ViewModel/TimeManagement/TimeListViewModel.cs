using System.Collections.ObjectModel;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		Lista de controles de tiempo
/// </summary>
public class TimeListViewModel : BaseObservableObject
{
	// Variables privadas
	private DateTime _date;
	private string _elapsed = default!;
	private TimeSpan _dayTime;
	private ObservableCollection<TimeViewModel> _items = default!;
	private TimeViewModel? _selectedItem;

	public TimeListViewModel(TimeScheduleViewModel timeScheduleViewModel)
	{
		// Inicializa las propiedades
		TimeScheduleViewModel = timeScheduleViewModel;
		Items = new ObservableCollection<TimeViewModel>();
		// Inicializa los comandos
		NewCommand = new BaseCommand(_ => UpdateItem(null));
		UpdateCommand = new BaseCommand(_ => UpdateItem(SelectedItem), _ => SelectedItem is not null)
							.AddListener(this, nameof(SelectedItem));
		DeleteCommand = new BaseCommand(_ => DeleteItem(SelectedItem), _ => SelectedItem is not null)
							.AddListener(this, nameof(SelectedItem));
		StartCommand = new BaseCommand(_ => StartItem(SelectedItem), _ => SelectedItem is not null)
							.AddListener(this, nameof(SelectedItem));
		PreviousCommand = new BaseCommand(_ => Date = Date.AddDays(-1));
		NextCommand = new BaseCommand(_ => Date = Date.AddDays(1));
		CollapseCommand = new BaseCommand(_ => Collapse());
	}

	/// <summary>
	///		Carga los datos de la fecha
	/// </summary>
	public void Load(DateOnly date)
	{
		List<TimeViewModel> times = [];

			// Guarda la fecha e inicaliza el tiempo total del día
			Date = new DateTime(date, TimeOnly.MinValue);
			_dayTime = TimeSpan.FromSeconds(0);
			// Guarda el control de los elementos
			TimeControl = TimeScheduleViewModel.LoadDate(date);
			// Asigna los datos
			foreach (ProjectModel project in TimeControl.Projects)
				foreach (TaskModel task in project.Tasks)
					foreach (TimeModel time in task.Times)
					{
						// Añade el viewModel a la lista
						times.Add(new TimeViewModel(TimeScheduleViewModel, time));
						// Añade el tiempo al total del día
						_dayTime = _dayTime.Add(time.End - time.Start);
					}
			// Ordena por fecha de inicio descendente
			times.Sort((first, second) => -1 * first.Time.Start.CompareTo(second.Time.Start));
			// Añade los elementos a la lista de salida
			Items.Clear();
			foreach (TimeViewModel time in times)
				Items.Add(time);
			// Muestra el tiempo del día
			Elapsed = TimeScheduleViewModel.FormatElapsed(_dayTime);
	}

	/// <summary>
	///		Añade el total de la tarea que se está ejecutando en este momento
	/// </summary>
	internal void UpdateTotalElapsed(TimeEditableViewModel actualTimeViewModel)
	{
		if (actualTimeViewModel.StartDate.Date == Date.Date)
			Elapsed = TimeScheduleViewModel.FormatElapsed(_dayTime + (actualTimeViewModel.EndDate - actualTimeViewModel.StartDate));
	}

	/// <summary>
	///		Crea / modifica un elemento
	/// </summary>
	private void UpdateItem(TimeViewModel? timeViewModel)
	{
		TimeViewModel cloned = (timeViewModel ?? new TimeViewModel(TimeScheduleViewModel, null)).Clone();

			// Modifica los datos
			if (TimeScheduleViewModel.MainViewModel.ViewsController.OpenDialog(cloned) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Agrega el valor si es nuevo o modifica sus datos
				if (timeViewModel is null)
					Items.Add(cloned);
				else
				{
					timeViewModel.Project = cloned.Project;
					timeViewModel.Task = cloned.Task;
					timeViewModel.TimeStart = cloned.TimeStart;
					timeViewModel.TimeEnd = cloned.TimeEnd;
					timeViewModel.Time.Remarks = cloned.Time.Remarks;
				}
				// Graba el archivo
				Save();
			}
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	private void DeleteItem(TimeViewModel? timeViewModel)
	{
		if (timeViewModel is not null &&
			TimeControl is not null &&
			TimeScheduleViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowQuestion("Do you really want to delete this item?"))
		{
			// Borra el elemento
			Items.Remove(timeViewModel);
			// Graba el archivo
			Save();
		}
	}

	/// <summary>
	///		Arranca un elemento
	/// </summary>
	private void StartItem(TimeViewModel? timeViewModel)
	{
		if (timeViewModel is not null)
			TimeScheduleViewModel.ActualTimeViewModel.StartTask(timeViewModel.Time.Task.Project.Name, timeViewModel.Time.Task.Name);
	}

	/// <summary>
	///		Actualiza los datos 
	/// </summary>
	private void Save()
	{
		// Limpia los datos
		TimeControl.Projects.Clear();
		// Agrega los datos de la lista
		foreach (TimeViewModel timeViewModel in Items)
			TimeControl.Add(timeViewModel.Project, timeViewModel.Task, timeViewModel.Time.Start, timeViewModel.Time.End);
		// Graba los datos
		TimeScheduleViewModel.Save(TimeControl);
	}

	/// <summary>
	///		Colapsa el proyecto
	/// </summary>
	private void Collapse()
	{
		ConsolidateViewModel filterViewModel = new(TimeScheduleViewModel);

			// Asigna los datos iniciales
			filterViewModel.TaskMinimumMinutes = TimeScheduleViewModel.MainViewModel.ConfigurationViewModel.ConsolidationTaskMinimumMinutes;
			filterViewModel.GapBetweenTasksMinutes = TimeScheduleViewModel.MainViewModel.ConfigurationViewModel.ConsolidationGapBetweenTasksMinutes;
			filterViewModel.GapBetweenTasksConsolidateMinutes = TimeScheduleViewModel.MainViewModel.ConfigurationViewModel.ConsolidationGapBetweenTasksConsolidateMinutes;
			// Abre el cuadro de diálogo
			if (TimeScheduleViewModel.MainViewModel.ViewsController.OpenDialog(filterViewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{

				// Colapsa el proyecto
				TimeControl.Collapse(TimeSpan.FromMinutes(filterViewModel.TaskMinimumMinutes), 
									 TimeSpan.FromMinutes(filterViewModel.GapBetweenTasksMinutes), 
									 TimeSpan.FromMinutes(filterViewModel.GapBetweenTasksConsolidateMinutes));
				// Graba los datos
				TimeScheduleViewModel.Save(TimeControl);
				// Recarga los datos
				Load(DateOnly.FromDateTime(Date));
				// Guarda los nuevos datos en la configuración
				TimeScheduleViewModel.MainViewModel.ConfigurationViewModel.ConsolidationTaskMinimumMinutes = filterViewModel.TaskMinimumMinutes;
				TimeScheduleViewModel.MainViewModel.ConfigurationViewModel.ConsolidationGapBetweenTasksMinutes = filterViewModel.GapBetweenTasksMinutes;
				TimeScheduleViewModel.MainViewModel.ConfigurationViewModel.ConsolidationGapBetweenTasksConsolidateMinutes = filterViewModel.GapBetweenTasksConsolidateMinutes;
				// Graba la configuración
				TimeScheduleViewModel.MainViewModel.ConfigurationViewModel.Save();
				// Mensaje al usuario
				TimeScheduleViewModel.MainViewModel.ViewsController.MainWindowController.SystemController.ShowMessage("Task consolidation completed");
			}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public TimeScheduleViewModel TimeScheduleViewModel { get; }

	/// <summary>
	///		Tiempo
	/// </summary>
	public TimeControlModel TimeControl { get; private set; } = default!;

	/// <summary>
	///		Fecha
	/// </summary>
	public DateTime Date
	{
		get { return _date; }
		set 
		{ 
			if (CheckProperty(ref _date, value))
				Load(DateOnly.FromDateTime(value));
		}
	}

	/// <summary>
	///		Elementos de la lista
	/// </summary>
	public ObservableCollection<TimeViewModel> Items
	{
		get { return _items; }
		set { CheckObject(ref _items, value); }
	}

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public TimeViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Tiempo pasado en todo el día
	/// </summary>
	public string Elapsed
	{
		get { return _elapsed; }
		set { CheckProperty(ref _elapsed, value); }
	}

	/// <summary>
	///		Nuevo elemento
	/// </summary>
	public BaseCommand NewCommand { get; }

	/// <summary>
	///		Modificar elemento
	/// </summary>
	public BaseCommand UpdateCommand { get; }

	/// <summary>
	///		Borrar elemento
	/// </summary>
	public BaseCommand DeleteCommand { get; }

	/// <summary>
	///		Iniciar elemento
	/// </summary>
	public BaseCommand StartCommand { get; }

	/// <summary>
	///		Pasar a la fecha anterior
	/// </summary>
	public BaseCommand PreviousCommand { get; }
	
	/// <summary>
	///		Pasar a la fecha siguiente
	/// </summary>
	public BaseCommand NextCommand { get; }

	/// <summary>
	///		Comando para consolidar
	/// </summary>
	public BaseCommand CollapseCommand { get; }
}
