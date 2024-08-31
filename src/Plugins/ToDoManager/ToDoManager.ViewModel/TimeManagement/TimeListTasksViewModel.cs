using System.Collections.ObjectModel;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ToDoManager.Application.TimeManagement.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel de la lista de tareas
/// </summary>
public class TimeListTasksViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private DateTime _start, _end;
	private ObservableCollection<TimeViewModel> _items = default!;
	private TimeViewModel? _selectedItem;

	public TimeListTasksViewModel(TimeScheduleViewModel mainViewModel) : base(false)
	{
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		Items = new ObservableCollection<TimeViewModel>();
		// Inicializa el viewModel
		InitViewModel();
		// Asigna los comandos
		SearchCommand = new BaseCommand(_ => Load());
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Inicializa las propiedades
		Start = DateTime.Now.AddDays(-15);
		End = DateTime.Now;
		// Carga los datos
		Load();
	}

	/// <summary>
	///		Carga los datos
	/// </summary>
	private void Load()
	{
		// Se asegura que las fechas vayan de inicio a fin
		EnsureDates();
		// Carga las tareas
		LoadTaskTimes(DateOnly.FromDateTime(Start), DateOnly.FromDateTime(End));
		LoadListItems();

		// Se asegura que las fechas sean correctas
		void EnsureDates()
		{
			if (End < Start)
				(Start, End) = (End, Start);
		}
	}

	/// <summary>
	///		Carga las fechas
	/// </summary>
	private void LoadTaskTimes(DateOnly start, DateOnly end)
	{
		// Limpia el array de tareas
		TaskTimes.Clear();
		// Carga las tareas
		do
		{
			// Carga la tarea de esa fecha
			TaskTimes.Add(MainViewModel.LoadDate(start));
			// Incrementa la fecha
			start = start.AddDays(1);
		}
		while (start <= end);
		// Ordena las tareas
		TaskTimes.Sort((first, second) => first.Date.CompareTo(second.Date));
	}

	/// <summary>
	///		Carga los datos en la lista de presentación
	/// </summary>
	private void LoadListItems()
	{
		// Limpia la colección
		Items.Clear();
		// Carga los datos
		foreach (TimeControlModel timeControl in TaskTimes)
			foreach (TimeModel time in timeControl.GetTimes())
				Items.Add(new TimeViewModel(MainViewModel, time));
	}

	/// <summary>
	///		Obtiene el mensaje de grabación y cierre de la ventana (sólo immplementa la interface)
	/// </summary>
	public string GetSaveAndCloseMessage() => string.Empty;

	/// <summary>
	///		Graba los datos (sólo implementa la interface)
	/// </summary>
	public void SaveDetails(bool newName)
	{
	}

	/// <summary>
	///		Cierra la ventana (sólo implementa la interface)
	/// </summary>
	public void Close()
	{
	}

	/// <summary>
	///		ViewModel de la ventana principal
	/// </summary>
	public TimeScheduleViewModel MainViewModel { get; }

	/// <summary>
	///		Cabecera de la ventana
	/// </summary>
	public string Header => "Tasks list";

	/// <summary>
	///		Id del documento
	/// </summary>
	public string TabId => nameof(TimeListTasksViewModel);

	/// <summary>
	///		Datos de las tareas
	/// </summary>
	public List<TimeControlModel> TaskTimes { get; } = [];

	/// <summary>
	///		Fecha inicial
	/// </summary>
	public DateTime Start
	{
		get { return _start; }
		set { CheckProperty(ref _start, value); }
	}

	/// <summary>
	///		Fecha final
	/// </summary>
	public DateTime End
	{
		get { return _end; }
		set { CheckProperty(ref _end, value); }
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
	///		Comando para filtrar los datos
	/// </summary>
	public BaseCommand SearchCommand { get; }
}
