using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ToDoManager.Application.ToDo.Models;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;

namespace Bau.Libraries.ToDoManager.ViewModel.ToDo;

/// <summary>
///		ViewModel para ver el contenido de un archivo de tareas
/// </summary>
public class ToDoFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Enumerados
	/// <summary>
	///		Tipo de movimiento
	/// </summary>
	internal enum MovementType
	{
		/// <summary>Mover al principio</summary>
		First,
		/// <summary>Mover al final</summary>
		Last,
		/// <summary>Mover hacia arriba</summary>
		Up,
		/// <summary>Mover hacia abajo</summary>
		Down
	}
	// Variables privadas
	private string _fileName = string.Empty;
	private ControlItemCollectionViewModel<ToDoTaskItemViewModel> _tasksPlanned = default!;
	private ControlItemCollectionViewModel<ToDoTaskItemViewModel> _tasksDoing = default!;
	private ControlItemCollectionViewModel<ToDoTaskItemViewModel> _tasksDone = default!;

	public ToDoFileViewModel(ToDoManagerViewModel mainViewModel, string fileName) : base(false)
	{ 
		// Inicializa los objetos
		MainViewModel = mainViewModel;
		FileName = fileName;
		ToDoFile = new ToDoFileModel();
		Header = Path.GetFileNameWithoutExtension(fileName);
		TasksPlanned = new ControlItemCollectionViewModel<ToDoTaskItemViewModel>();
		TasksDoing = new ControlItemCollectionViewModel<ToDoTaskItemViewModel>();
		TasksDone = new ControlItemCollectionViewModel<ToDoTaskItemViewModel>();
		// Inicializa los comandos
		NewTaskCommand = new BaseCommand(_ => OpenUpdateTask(null));
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	internal bool LoadFile()
	{
		bool loaded = false;

			// Intenta cargar el archivo
			try
			{
				// Carga el archivo
				ToDoFile = ToDoManager.Load(FileName);
				// Carga las listas
				TasksPlanned = LoadListTasks(ToDoFile, TaskModel.StatusType.Planned);
				TasksDoing = LoadListTasks(ToDoFile, TaskModel.StatusType.Doing);
				TasksDone = LoadListTasks(ToDoFile, TaskModel.StatusType.Done);
				// Ordena las listas
				SetOrders();
				// Indica que se ha cargado el archivo
				loaded = true;
			}
			catch (Exception exception)
			{
				MainViewModel.ViewsController.Logger.LogError(exception, $"Error when load {FileName}. {exception.Message}");
				MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load {FileName}. {exception.Message}");
			}
			// Devuelve el valor que indica si ha podido cargar el archivo
			return loaded;
	}

	/// <summary>
	///		Carga la lista de tareas
	/// </summary>
	private ControlItemCollectionViewModel<ToDoTaskItemViewModel> LoadListTasks(ToDoFileModel toDoFile, TaskModel.StatusType status)
	{
		ControlItemCollectionViewModel<ToDoTaskItemViewModel> tasksViewModel = new();

			// Ordena las tareas por orden
			toDoFile.Tasks.SortByOrder();
			// Añade las tareas del tipo solicitado
			foreach (TaskModel task in toDoFile.Tasks)
				if (task.Status == status)
					tasksViewModel.Add(new ToDoTaskItemViewModel(this, task));
			// Devuelve el control
			return tasksViewModel;
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "Do you want to save the file before continuing?";
		else
			return $"¿Desea grabar el archivo '{Path.GetFileName(FileName)}' antes de continuar?";
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// Graba el archivo
		if (string.IsNullOrWhiteSpace(FileName) || newName)
		{
			string? newFileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave
									(string.Empty, 
									 $"Archivo tareas (*{ToDoManagerViewModel.ToDoFileExtension})|*{ToDoManagerViewModel.ToDoFileExtension}|Todos los archivos (*.*)|*.*",
									 FileName, ToDoManagerViewModel.ToDoFileExtension);

				// Cambia el nombre de archivo si es necesario
				if (!string.IsNullOrWhiteSpace(newFileName))
					FileName = newFileName;
		}
		// Graba el archivo
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			// Graba el archivo
			ToDoManager.Save(FileName, GetTasksFile());
			// Actualiza el árbol
			MainViewModel.ViewsController.HostPluginsController.RefreshFiles();
			// Añade el archivo a los últimos archivos abiertos
			MainViewModel.ViewsController.HostPluginsController.AddFileUsed(FileName);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Obtiene el archivo de tareas
	/// </summary>
	private ToDoFileModel GetTasksFile()
	{
		ToDoFileModel todoFile = new();

			// Añade las tareas
			todoFile.Tasks.AddRange(GetTasks(GetTasksList(TaskModel.StatusType.Planned)));
			todoFile.Tasks.AddRange(GetTasks(GetTasksList(TaskModel.StatusType.Doing)));
			todoFile.Tasks.AddRange(GetTasks(GetTasksList(TaskModel.StatusType.Done)));
			// Devuelve el archivo
			return todoFile;
	}

	/// <summary>
	///		Obtiene las tareas de una lista
	/// </summary>
	private IEnumerable<TaskModel> GetTasks(ControlItemCollectionViewModel<ToDoTaskItemViewModel> listViewModel)
	{
		foreach (ToDoTaskItemViewModel itemViewModel in listViewModel)
			yield return itemViewModel.Task;
	}

	/// <summary>
	///		Cierra la ventana de detalles
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Abre la ventana para modificar una tarea
	/// </summary>
	internal void OpenUpdateTask(ToDoTaskItemViewModel? toDoTaskItemViewModel)
	{
		ToDoTaskViewModel taskViewModel = new ToDoTaskViewModel(this, toDoTaskItemViewModel?.Task ?? new(), toDoTaskItemViewModel is null);
		TaskModel.StatusType oldStatus = taskViewModel.Task.Status;
			
			// Abre la ventana de modificación de la tarea
			if (MainViewModel.ViewsController.OpenDialog(taskViewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Elimina la tarea anterior de la lista
				if (toDoTaskItemViewModel is not null)
					GetTasksList(oldStatus).Remove(toDoTaskItemViewModel);
				// Asigna el orden
				if (toDoTaskItemViewModel is null || oldStatus != taskViewModel.Task.Status)
					taskViewModel.Task.Order = GetTasksList(taskViewModel.Task.Status).Count;
				// Añade la nueva tarea
				GetTasksList(taskViewModel.Task.Status).Insert(taskViewModel.Task.Order, new ToDoTaskItemViewModel(this, taskViewModel.Task));
				SetOrders();
				// Indica que se ha modificado
				IsUpdated = true;
			}
	}

	/// <summary>
	///		Borra una tarea
	/// </summary>
	internal void DeleteTask(ToDoTaskItemViewModel taskViewModel)
	{
		// Borra el elemento de la lista apropiada
		GetTasksList(taskViewModel.Task.Status).Remove(taskViewModel);
		// Indica que ha habido modificaciones
		IsUpdated = true;
	}

	/// <summary>
	///		Mueve una tarea entre estados
	/// </summary>
	internal void MoveTask(ToDoTaskItemViewModel taskViewModel, bool moveNext)
	{
		TaskModel.StatusType newStatus = GetNextStatus(taskViewModel.Task.Status, moveNext);
		TaskModel task = taskViewModel.Task;

			// Quita la tarea de la lista inicial
			GetTasksList(task.Status).Remove(taskViewModel);
			// Añade la tarea a la nueva lista
			task.Status = newStatus;
			GetTasksList(newStatus).Add(new ToDoTaskItemViewModel(this, task));
			// Reordena las tareas
			SetOrders();
			// Indica que ha habido modificaciones
			IsUpdated = true;
	}

	/// <summary>
	///		Reordena las tareas
	/// </summary>
	private void SetOrders()
	{
		SetOrder(TaskModel.StatusType.Planned);
		SetOrder(TaskModel.StatusType.Doing);
		SetOrder(TaskModel.StatusType.Done);
	}

	/// <summary>
	///		Cambia el orden de las tareas de una lista
	/// </summary>
	private void SetOrder(TaskModel.StatusType status)
	{
		SetOrder(GetTasksList(status));
	}

	/// <summary>
	///		Cambia el orden de las tareas de una lista
	/// </summary>
	private void SetOrder(ControlItemCollectionViewModel<ToDoTaskItemViewModel> list)
	{
		int order = 0;

			// Cambia el orden de los elementos
			foreach (ToDoTaskItemViewModel item in list)
			{
				item.Order = item.Task.Order = order++;
				item.MaxOrder = list.Count - 1;
			}
	}

	/// <summary>
	///		Obtiene la lista de tareas correspondiente a un estado
	/// </summary>
	private ControlItemCollectionViewModel<ToDoTaskItemViewModel> GetTasksList(TaskModel.StatusType status)
	{
		return status switch
				{
					TaskModel.StatusType.Planned => TasksPlanned,
					TaskModel.StatusType.Doing => TasksDoing,
					_ => TasksDone
				};
	}

	/// <summary>
	///		Obtiene el estado siguiente / anterior de una tarea
	/// </summary>
	private TaskModel.StatusType GetNextStatus(TaskModel.StatusType status, bool moveNext)
	{
		switch (status)
		{
			case TaskModel.StatusType.Planned:
				if (moveNext)
					return TaskModel.StatusType.Doing;
				else
					return TaskModel.StatusType.Done;
			case TaskModel.StatusType.Doing:
				if (moveNext)
					return TaskModel.StatusType.Done;
				else
					return TaskModel.StatusType.Planned;
			default:
				if (moveNext)
					return TaskModel.StatusType.Planned;
				else
					return TaskModel.StatusType.Doing;
		}
	}

	/// <summary>
	///		Abre el elemento seleccionado de una lista (posiblemente desde un doble click)
	/// </summary>
	public void OpenSelected(TaskModel.StatusType type)
	{
		ControlItemCollectionViewModel<ToDoTaskItemViewModel> viewModel = GetTasksList(type);

			if (viewModel.SelectedItem is not null)
				OpenUpdateTask(viewModel.SelectedItem);
	}

	/// <summary>
	///		Modifica el orden
	/// </summary>
	internal void UpdateOrder(ToDoTaskItemViewModel toDoTaskItemViewModel, MovementType type)
	{
		ControlItemCollectionViewModel<ToDoTaskItemViewModel> list = Clone(GetTasksList(toDoTaskItemViewModel.Task.Status));
		ToDoTaskItemViewModel itemNew = list[GetTasksList(toDoTaskItemViewModel.Task.Status).IndexOf(toDoTaskItemViewModel)];

			// Mueve el elemento
			switch (type)
			{
				case MovementType.Up:
						list.Move(itemNew, true);
					break;
				case MovementType.Down:
						list.Move(itemNew, false);
					break;
				case MovementType.First:
						list.MoveToFirst(itemNew);
					break;
				case MovementType.Last:
						list.MoveToLast(itemNew);
					break;
			}
			// Recarga la lista
			switch (toDoTaskItemViewModel.Task.Status)
			{
				case TaskModel.StatusType.Planned:
						TasksPlanned = Clone(list);
					break;
				case TaskModel.StatusType.Doing:
						TasksDoing = Clone(list);
					break;
				case TaskModel.StatusType.Done:
						TasksDone = Clone(list);
					break;
			}
			// Cambia el orden
			SetOrder(toDoTaskItemViewModel.Task.Status);
			// Indica que se ha modificado
			IsUpdated = true;
	}

	/// <summary>
	///		Clona una lista
	/// </summary>
	private ControlItemCollectionViewModel<ToDoTaskItemViewModel> Clone(ControlItemCollectionViewModel<ToDoTaskItemViewModel> source)
	{
		ControlItemCollectionViewModel<ToDoTaskItemViewModel> list = new();

			// Clona los objetos
			foreach (ToDoTaskItemViewModel item in source)
				list.Add(new ToDoTaskItemViewModel(this, item.Task));
			// Devuelve la lista
			return list;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Manager de la aplicación de lista de tareas
	/// </summary>
	public Application.ToDo.ToDoFileManager ToDoManager { get; } = new();

	/// <summary>
	///		Archivo de tareas
	/// </summary>
	public ToDoFileModel ToDoFile { get; private set; }	
	
	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id de la ficha en pantalla
	/// </summary>
	public string TabId  => GetType().ToString() + "_" + FileName;

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				if (string.IsNullOrWhiteSpace(_fileName))
					Header = "New filename";
				else
					Header = Path.GetFileName(_fileName);
			}
		}
	}

	/// <summary>
	///		Tares planificadas
	/// </summary>
	public ControlItemCollectionViewModel<ToDoTaskItemViewModel> TasksPlanned
	{
		get { return _tasksPlanned; }
		set { CheckObject(ref _tasksPlanned, value); }
	}

	/// <summary>
	///		Tares en curso
	/// </summary>
	public ControlItemCollectionViewModel<ToDoTaskItemViewModel> TasksDoing
	{
		get { return _tasksDoing; }
		set { CheckObject(ref _tasksDoing, value); }
	}

	/// <summary>
	///		Tares finalizadas
	/// </summary>
	public ControlItemCollectionViewModel<ToDoTaskItemViewModel> TasksDone
	{
		get { return _tasksDone; }
		set { CheckObject(ref _tasksDone, value); }
	}

	/// <summary>
	///		Comando de nueva tarea
	/// </summary>
	public BaseCommand NewTaskCommand { get; }
}
