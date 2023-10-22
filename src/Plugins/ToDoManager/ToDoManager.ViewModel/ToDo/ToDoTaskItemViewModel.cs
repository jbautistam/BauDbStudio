using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.ToDoManager.Application.ToDo.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.ToDo;

/// <summary>
///		ViewModel de un <see cref="TaskModel"/>
/// </summary>
public class ToDoTaskItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Variables privadas
	private string _name = default!, _description = default!;
	private bool _canMoveNext, _canMovePrevious;
	private TaskModel.PriorityType _priority = TaskModel.PriorityType.Normal;
	private TaskModel.MoskowType _moskow = TaskModel.MoskowType.CouldHave;

	public ToDoTaskItemViewModel(ToDoFileViewModel toDoFileViewModel, TaskModel task, 
								 bool isBold = false, MvvmColor? foreground = null) : base(task.Name, task, isBold, foreground)
	{
		// Guarda los objetos
		ToDoFileViewModel = toDoFileViewModel;
		Task = task;
		// Inicializa las propiedades
		InitProperties();
		// Asigna los comandos
		EditTaskCommand = new BaseCommand(_ => OpenUpdateTask());
		DeleteTaskCommand = new BaseCommand(_ => DeleteTask());
		MoveTaskNextCommand = new BaseCommand(_ => MoveTask(true), _ => Task.Status != TaskModel.StatusType.Done);
		MoveTaskPreviousCommand = new BaseCommand(_ => MoveTask(false), _ => Task.Status != TaskModel.StatusType.Planned);
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		Name = Task.Name;
		Description = Task.Description;
		Priority = Task.Priority;
		Moskow = Task.Moskow;
		CanMoveNext = Task.Status != TaskModel.StatusType.Done;
		CanMovePrevious = Task.Status != TaskModel.StatusType.Planned;
	}

	/// <summary>
	///		Abre la modificación de una tarea
	/// </summary>
	private void OpenUpdateTask()
	{
		ToDoFileViewModel.OpenUpdateTask(this);
	}

	/// <summary>
	///		Borra la tarea
	/// </summary>
	private void DeleteTask()
	{
		if (ToDoFileViewModel.MainViewModel.ViewsController.SystemController.ShowQuestionCancel($"Do you want to delete the task '{Task.Name}'") == 
				BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			ToDoFileViewModel.DeleteTask(this);
	}

	/// <summary>
	///		Mueve la tarea
	/// </summary>
	private void MoveTask(bool moveNext)
	{
		ToDoFileViewModel.MoveTask(this, moveNext);
	}

	/// <summary>
	///		ViewModel del archivo
	/// </summary>
	public ToDoFileViewModel ToDoFileViewModel { get; }

	/// <summary>
	///		Tarea
	/// </summary>
	public TaskModel Task { get; }

	/// <summary>
	///		Nombre de la tarea
	/// </summary>
	public string Name 
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción de la tarea
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}

	/// <summary>
	///		Prioridad de la tarea
	/// </summary>
	public TaskModel.PriorityType Priority
	{
		get { return _priority;}
		set { CheckProperty(ref _priority, value); }
	}

	/// <summary>
	///		Identificador de la metodología MoSKoW de la tarea
	/// </summary>
	public TaskModel.MoskowType Moskow
	{
		get { return _moskow;}
		set { CheckProperty(ref _moskow, value); }
	}

	/// <summary>
	///		Indica si se puede mover la tarea al siguiente Pool
	/// </summary>
	public bool CanMoveNext
	{
		get { return _canMoveNext;}
		set { CheckProperty(ref _canMoveNext, value); }
	}
	
	/// <summary>
	///		Indica si se puede mover la tarea al anterior Pool
	/// </summary>
	public bool CanMovePrevious
	{
		get { return _canMovePrevious;}
		set { CheckProperty(ref _canMovePrevious, value); }
	}

	/// <summary>
	///		Comando para editar una tarea
	/// </summary>
	public BaseCommand EditTaskCommand { get; }

	/// <summary>
	///		Comando para mover una tarea al siguiente pool
	/// </summary>
	public BaseCommand MoveTaskNextCommand { get; }

	/// <summary>
	///		Comando para mover una tarea al pool anterior
	/// </summary>
	public BaseCommand MoveTaskPreviousCommand { get; }

	/// <summary>
	///		Comando para borrar una tarea
	/// </summary>
	public BaseCommand DeleteTaskCommand { get; }
}
