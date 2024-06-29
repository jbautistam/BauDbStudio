using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.ToDoManager.ViewModel.Configuration;

/// <summary>
///		ViewModel de la configuración
/// </summary>
public class ConfigurationViewModel : BaseObservableObject
{
	// Variables privadas
	private string _timeManagementFolder = default!;
	private bool _startTasksAutomatically, _hookGlobal;

	public ConfigurationViewModel(ToDoManagerViewModel mainViewModel)
	{
		MainViewModel =	mainViewModel;
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		HookGlobal = MainViewModel.ViewsController.PluginController.ConfigurationController
									.GetConfiguration(nameof(ToDoManagerViewModel), nameof(HookGlobal)).GetBool();
		TimeManagementFolder = MainViewModel.ViewsController.PluginController.ConfigurationController
									.GetConfiguration(nameof(ToDoManagerViewModel), nameof(TimeManagementFolder));
		StartTasksAutomatically = MainViewModel.ViewsController.PluginController.ConfigurationController
										.GetConfiguration(nameof(ToDoManagerViewModel), nameof(StartTasksAutomatically)).GetBool();
		ConsolidationTaskMinimumMinutes = MainViewModel.ViewsController.PluginController.ConfigurationController
												.GetConfiguration(nameof(ToDoManagerViewModel), nameof(ConsolidationTaskMinimumMinutes)).GetInt(1);
		ConsolidationGapBetweenTasksMinutes = MainViewModel.ViewsController.PluginController.ConfigurationController
												.GetConfiguration(nameof(ToDoManagerViewModel), nameof(ConsolidationGapBetweenTasksMinutes)).GetInt(5);
		ConsolidationGapBetweenTasksConsolidateMinutes = MainViewModel.ViewsController.PluginController.ConfigurationController
															.GetConfiguration(nameof(ToDoManagerViewModel), nameof(ConsolidationGapBetweenTasksConsolidateMinutes)).GetInt(5);
	}

	/// <summary>
	///		Graba los datos de configuración
	/// </summary>
	public void Save()
	{
		MainViewModel.ViewsController.PluginController.ConfigurationController
				.SetConfiguration(nameof(ToDoManagerViewModel), nameof(HookGlobal), HookGlobal.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController
				.SetConfiguration(nameof(ToDoManagerViewModel), nameof(TimeManagementFolder), TimeManagementFolder);
		MainViewModel.ViewsController.PluginController.ConfigurationController
				.SetConfiguration(nameof(ToDoManagerViewModel), nameof(StartTasksAutomatically), StartTasksAutomatically.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController
			.SetConfiguration(nameof(ToDoManagerViewModel), nameof(ConsolidationTaskMinimumMinutes), ConsolidationTaskMinimumMinutes.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController
			.SetConfiguration(nameof(ToDoManagerViewModel), nameof(ConsolidationGapBetweenTasksMinutes), ConsolidationGapBetweenTasksMinutes.ToString());
		MainViewModel.ViewsController.PluginController.ConfigurationController
			.SetConfiguration(nameof(ToDoManagerViewModel), nameof(ConsolidationGapBetweenTasksConsolidateMinutes), ConsolidationGapBetweenTasksConsolidateMinutes.ToString());
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Indica si se debe asociar al hook del teclado
	/// </summary>
	public bool HookGlobal
	{
		get { return _hookGlobal; }
		set { CheckProperty(ref _hookGlobal, value); }
	}

	/// <summary>
	///		Indica si se deben arrancar las tareas automáticamente
	/// </summary>
	public bool StartTasksAutomatically
	{
		get { return _startTasksAutomatically; }
		set { CheckProperty(ref _startTasksAutomatically, value); }
	}

	/// <summary>
	///		Carpeta donde se guardan los archivos del control de tiempos
	/// </summary>
	public string TimeManagementFolder
	{
		get 
		{ 
			// Inicializa el directorio con la carpeta predeterminada
			if (string.IsNullOrWhiteSpace(_timeManagementFolder))
				_timeManagementFolder = MainViewModel.ToDoManager.Folder;
			// Devuelve el directorio
			return _timeManagementFolder; 
		}
		set { CheckProperty(ref _timeManagementFolder, value); }
	}

	/// <summary>
	///		Mínimo de minutos para una tarea en la consolidación
	/// </summary>
	public int ConsolidationTaskMinimumMinutes { get; set; }

	/// <summary>
	///		Mínimo de minutos entre tareas para eliminar la separación en la consolidación
	/// </summary>
	public int ConsolidationGapBetweenTasksMinutes { get; set; }

	/// <summary>
	///		Mínimo de minutos entre tareas para consolidar
	/// </summary>
	public int ConsolidationGapBetweenTasksConsolidateMinutes { get; set; }
}
