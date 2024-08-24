using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel principal para la administración de horas de trabajo
/// </summary>
public class TimeScheduleViewModel : BaseObservableObject
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
		ConvertFilesCommand = new BaseCommand(_ => ConvertFiles());
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
	internal void Close()
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
	///		Convierte los archivos al nuevo sistema de directorios
	/// </summary>
	[Obsolete]
	private void ConvertFiles()
	{
		foreach (string file in Directory.GetFiles(MainViewModel.ConfigurationViewModel.TimeManagementFolder))
			if (file.EndsWith(".time.xml", StringComparison.CurrentCultureIgnoreCase))
			{
				string dateMask = Path.GetFileNameWithoutExtension(file);
				string[] parts;

					// Quita la segunda extensión
					dateMask = Path.GetFileNameWithoutExtension(dateMask);
					// Separa el nombre de la fecha
					parts = dateMask.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
					// Copia el archivo
					if (parts.Length == 2)
					{
						DateTime? date = parts[1].GetDateTime();

							// Copia el archivo a la carpeta
							if (date is not null)
							{
								string folder = Path.Combine(MainViewModel.ConfigurationViewModel.TimeManagementFolder, $"{date.Value.Year:0000}", $"{date.Value.Month:00}");

									LibHelper.Files.HelperFiles.CopyFile(file, Path.Combine(folder, Path.GetFileName(file)));
							}
					}
			}
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
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Control de tiempo actual
	/// </summary>
	public TimeControlModel ActualTimeControl { get; private set; } = default!;

	/// <summary>
	///		ViewModel para la lista de horas
	/// </summary>
	public TimeListViewModel TimeListViewModel
	{
		get { return _timeListViewModel; }
		set { CheckObject(ref _timeListViewModel, value); }
	}

	/// <summary>
	///		ViewModel de la hora que se está actualizando
	/// </summary>
	public TimeEditableViewModel ActualTimeViewModel
	{ 
		get { return _actualTimeViewModel; }
		set { CheckObject(ref _actualTimeViewModel, value); }
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

	/// <summary>
	///		Comando para convertir los archivos
	/// </summary>
	[Obsolete("Eliminar cuando se hayan convertido los archivos")]
	public BaseCommand ConvertFilesCommand { get; }
}
