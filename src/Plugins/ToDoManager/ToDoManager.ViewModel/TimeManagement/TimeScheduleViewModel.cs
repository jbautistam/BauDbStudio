using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;
using Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel principal para la administración de horas de trabajo
/// </summary>
public class TimeScheduleViewModel : BaseObservableObject, IPaneViewModel
{
	// Variables privadas
	private TimeListViewModel _timeListViewModel = default!;
	private TimeEditableViewModel _actualTimeViewModel = default!;

	public TimeScheduleViewModel(ToDoManagerViewModel mainViewModel)
	{
		// Inicializa los objetos
		MainViewModel = mainViewModel;
		TimeListViewModel = new TimeListViewModel(this);
		ActualTimeViewModel = new TimeEditableViewModel(this);
		// Inicializa los comandos
		ExportCommand = new BaseCommand(_ => ExportCsv());
		ListTasksCommand = new BaseCommand(_ => OpenWindowListTasks());
	}

	/// <summary>
	///		Carga los datos
	/// </summary>
	public void Load()
	{
		// Carga las horas
		ActualTimeControl = LoadDate(DateOnly.FromDateTime(DateTime.Now));
		// Muestra los datos en el control
		TimeListViewModel.Load(ActualTimeControl.Date);
	}

	/// <summary>
	///		Graba los datos en un archivo
	/// </summary>
	internal void Save(TimeControlModel timeControl)
	{
		MainViewModel.ToDoManager.TimeManagementManager.Save(timeControl, MainViewModel.ConfigurationViewModel.TimeManagementFolder);
	}

	/// <summary>
	///		Carga las horas trabajadas de una fecha
	/// </summary>
	internal TimeControlModel LoadDate(DateOnly date)
	{
		return MainViewModel.ToDoManager.TimeManagementManager.Load(MainViewModel.ConfigurationViewModel.TimeManagementFolder, date);
	}

	/// <summary>
	///		Cierra las tareas que tenga pendientes por cerrar y graba el archivo
	/// </summary>
	public void Close()
	{
		ActualTimeViewModel.StopCommand.Execute(null);
	}

	/// <summary>
	///		Exporta los proyectos y tareas a CSV
	/// </summary>
	private void ExportCsv()
	{
		CsvFilterViewModel filterViewModel = new(this);

			if (MainViewModel.ViewsController.OpenDialog(filterViewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				MainViewModel.ToDoManager.TimeManagementManager.ExportToCsv(MainViewModel.ConfigurationViewModel.TimeManagementFolder, 
																			filterViewModel.Start.Date, filterViewModel.End.Date, 
																			filterViewModel.FileName);
	}

	/// <summary>
	///		Abre la ventana de la lista de tareas
	/// </summary>
	private void OpenWindowListTasks()
	{
		MainViewModel.ViewsController.OpenWindow(new TimeListTasksViewModel(this));
	}

	/// <summary>
	///		Formatea con horas la diferencia entre dos fechas
	/// </summary>
	internal string FormatElapsed(DateTime start, DateTime? end) => FormatElapsed((end ?? DateTime.Now) - start);

	/// <summary>
	///		Formatea una cadena con horas
	/// </summary>
	internal string FormatElapsed(TimeSpan value) => $"{value.Hours:00}:{value.Minutes:00}:{value.Seconds:00}";

	/// <summary>
	///		Ejecuta un comando externo: sólo implementa el interface
	/// </summary>
	public void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		// No hace nada sólo implementa el interface
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Control de tiempo actual
	/// </summary>
	public TimeControlModel ActualTimeControl { get; private set; } = default!;

	/// <summary>
	///		Cabecera del panel
	/// </summary>
	public string Header => "Time management";

	/// <summary>
	///		Id del panel
	/// </summary>
	public string TabId => GetType().ToString();

	/// <summary>
	///		ViewModel para la lista de horas
	/// </summary>
	public TimeListViewModel TimeListViewModel
	{
		get { return _timeListViewModel; }
		set { CheckObject(ref _timeListViewModel!, value); }
	}

	/// <summary>
	///		ViewModel de la hora que se está actualizando
	/// </summary>
	public TimeEditableViewModel ActualTimeViewModel
	{ 
		get { return _actualTimeViewModel; }
		set { CheckObject(ref _actualTimeViewModel!, value); }
	}

	/// <summary>
	///		Carpeta en la que se está grabando
	/// </summary>
	public string Folder { get; set; } = default!;

	/// <summary>
	///		Comando para exportar
	/// </summary>
	public BaseCommand ExportCommand { get; }

	/// <summary>
	///		Comando para listar tareas
	/// </summary>
	public BaseCommand ListTasksCommand { get; }
}
