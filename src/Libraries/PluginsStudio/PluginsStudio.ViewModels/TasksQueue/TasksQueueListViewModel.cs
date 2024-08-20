using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.Libraries.PluginsStudio.ViewModels.TasksQueue;

/// <summary>
///		ViewModel para mostrar procesos en ejecución
/// </summary>
public class TasksQueueListViewModel : BaseObservableObject, Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _header = string.Empty;
	private BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<TasksQueueItemViewModel> _tasks = default!;

	public TasksQueueListViewModel(PluginsStudioViewModel mainViewModel) : base(false)
	{
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		Header = "Processes queue";
		Tasks = new BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<TasksQueueItemViewModel>(); 
		// Crea el temporizador que mantiene el estado de los elementos de la lista
		new Timer(UpdateStatus, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15));
	}

	/// <summary>
	///		Encola un proceso
	/// </summary>
	public async Task EnqueueProcessAsync(ProcessModel process, CancellationToken cancellationToken)
	{
		TasksQueueItemViewModel viewModel = new(this, process);

			// Añade el proceso a la lista
			Tasks.Add(viewModel);
			// Ejecuta el proceso
			await viewModel.ExecuteAsync(cancellationToken);
	}

	/// <summary>
	///		Actualiza el estado (se llama desde el temporizador)
	/// </summary>
	private void UpdateStatus(object? state)
	{
		foreach (TasksQueueItemViewModel viewModel in Tasks)
			viewModel.Update();
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// No hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
	/// </summary>
	public string GetSaveAndCloseMessage() => string.Empty;

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Elimina el elemento de la lista
	/// </summary>
	internal void Delete(TasksQueueItemViewModel itemViewModel)
	{
		Tasks.Remove(itemViewModel);
	}

	/// <summary>
	///		Solución
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header 
	{
		get { return _header; }
		set { CheckProperty(ref _header, value); }
	}

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId => GetType().ToString() + "_" + Guid.NewGuid().ToString();

	/// <summary>
	///		Lista de archivos a ejecutar
	/// </summary>
	public BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<TasksQueueItemViewModel> Tasks
	{
		get { return _tasks; }
		set { CheckObject(ref _tasks, value); }
	}
}