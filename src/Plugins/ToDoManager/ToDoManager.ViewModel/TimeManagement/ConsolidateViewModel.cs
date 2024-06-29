namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel para los parámetros de conslidación
/// </summary>
public class ConsolidateViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private int _taskMinimumMinutes, _gapBetweenTasksMinutes, _gapBetweenTasksConsolidateMinutes;

	public ConsolidateViewModel(TimeScheduleViewModel viewModel)
	{
		TimeScheduleViewModel = viewModel;
	}

	/// <summary>
	///		Comprueba si los datos introducidos son correctos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (TaskMinimumMinutes < 0)
				TimeScheduleViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("The minutes value must be greater or equal than zero");
			else if (GapBetweenTasksMinutes < 1 || GapBetweenTasksConsolidateMinutes < 1)
				TimeScheduleViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("The minutes value must be greater than zero");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Acepta los datos del filtro
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
			RaiseEventClose(true);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public TimeScheduleViewModel TimeScheduleViewModel { get; }

	/// <summary>
	///		Mínimo de minutos para una tarea
	/// </summary>
	public int TaskMinimumMinutes
	{
		get { return _taskMinimumMinutes; }
		set { CheckProperty(ref _taskMinimumMinutes, value); }
	}

	/// <summary>
	///		Mínimo de minutos entre tareas para eliminar la separación
	/// </summary>
	public int GapBetweenTasksMinutes
	{
		get { return _gapBetweenTasksMinutes; }
		set { CheckProperty(ref _gapBetweenTasksMinutes, value); }
	}

	/// <summary>
	///		Mínimo de minutos entre tareas para consolidar
	/// </summary>
	public int GapBetweenTasksConsolidateMinutes
	{
		get { return _gapBetweenTasksConsolidateMinutes; }
		set { CheckProperty(ref _gapBetweenTasksConsolidateMinutes, value); }
	}
}
