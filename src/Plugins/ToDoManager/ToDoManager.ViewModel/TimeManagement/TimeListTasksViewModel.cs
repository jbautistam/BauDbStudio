using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel de la lista de tareas
/// </summary>
public class TimeListTasksViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	public TimeListTasksViewModel(TimeScheduleViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
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
}
