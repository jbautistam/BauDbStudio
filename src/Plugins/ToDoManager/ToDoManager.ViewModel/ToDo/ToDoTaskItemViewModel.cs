using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.ToDoManager.Application.ToDo.Models;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;

namespace Bau.Libraries.ToDoManager.ViewModel.ToDo;

/// <summary>
///		ViewModel de un <see cref="TaskModel"/>
/// </summary>
public class ToDoTaskItemViewModel : ControlItemViewModel
{
	// Variables privadas
	private string _name = default!, _description = default!, _descriptionShort = default!;
	private int _order, _maxOrder;
	private bool _canMoveNext, _canMovePrevious, _canMoveUp, _canMoveDown;
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
		MoveTaskFirstCommand = new BaseCommand(_ => UpdateOrder(ToDoFileViewModel.MovementType.First), _ => CanUpdateOrder(true))
									.AddListener(this, nameof(CanMoveDown))
									.AddListener(this, nameof(CanMoveUp));
		MoveTaskUpCommand = new BaseCommand(_ => UpdateOrder(ToDoFileViewModel.MovementType.Up), _ => CanUpdateOrder(true))
									.AddListener(this, nameof(CanMoveDown))
									.AddListener(this, nameof(CanMoveUp));
		MoveTaskDownCommand = new BaseCommand(_ => UpdateOrder(ToDoFileViewModel.MovementType.Down), _ => CanUpdateOrder(false))
									.AddListener(this, nameof(CanMoveDown))
									.AddListener(this, nameof(CanMoveUp));
		MoveTaskLastCommand = new BaseCommand(_ => UpdateOrder(ToDoFileViewModel.MovementType.Last), _ => CanUpdateOrder(false))
									.AddListener(this, nameof(CanMoveDown))
									.AddListener(this, nameof(CanMoveUp));
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
		Order = Task.Order;
		CanMoveNext = Task.Status != TaskModel.StatusType.Done;
		CanMovePrevious = Task.Status != TaskModel.StatusType.Planned;
		CanMoveUp = CanUpdateOrder(true);
		CanMoveDown = CanUpdateOrder(false);
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
	///		Modifica el orden
	/// </summary>
	private void UpdateOrder(ToDoFileViewModel.MovementType type)
	{
		ToDoFileViewModel.UpdateOrder(this, type);
	}
	
	/// <summary>
	///		Indica si puede modificar el orden
	/// </summary>
	private bool CanUpdateOrder(bool moveUp)
	{
		if (moveUp)
			return Task.Order > 0;
		else
			return Task.Order < MaxOrder;
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
		set 
		{ 
			if (CheckProperty(ref _description, value))
				DescriptionShort = value;
		}
	}

	/// <summary>
	///		Descripción corta de la tarea
	/// </summary>
	public string DescriptionShort
	{
		get { return _descriptionShort; }
		set { CheckProperty(ref _descriptionShort, value.Left(750)); }
	}

	/// <summary>
	///		Orden de la tarea
	/// </summary>
	public int Order
	{
		get { return _order; }
		set 
		{ 
			// Cambia el valor de la propiedad
			CheckProperty(ref _order, value);
			// Cambia el valor que indica si se puede mover arriba o abajo
			//? Aunque no haya cambiado el orden porque se puede haber añadido otro después
			CanMoveUp = CanUpdateOrder(true);
			CanMoveDown = CanUpdateOrder(false);
		}
	}

	/// <summary>
	///		Orden máximo de la lista
	/// </summary>
	public int MaxOrder
	{
		get { return _maxOrder; }
		set 
		{ 
			// Cambia el valor de la propiedad
			CheckProperty(ref _maxOrder, value);
			// Cambia el valor que indica si se puede mover arriba o abajo
			//? Aunque no haya cambiado el orden porque se puede haber añadido otro después
			CanMoveUp = CanUpdateOrder(true);
			CanMoveDown = CanUpdateOrder(false);
		}
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
	///		Indica si se puede mover la tarea hacia arriba
	/// </summary>
	public bool CanMoveUp
	{
		get { return _canMoveUp;}
		set { CheckProperty(ref _canMoveUp, value); }
	}
	
	/// <summary>
	///		Indica si se puede mover la tarea hacia abajo
	/// </summary>
	public bool CanMoveDown
	{
		get { return _canMoveDown;}
		set { CheckProperty(ref _canMoveDown, value); }
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
	///		Comando para mover una tarea a la primera posición
	/// </summary>
	public BaseCommand MoveTaskFirstCommand { get; }

	/// <summary>
	///		Comando para mover arriba una tarea
	/// </summary>
	public BaseCommand MoveTaskUpCommand { get; }

	/// <summary>
	///		Comando para mover abajo una tarea
	/// </summary>
	public BaseCommand MoveTaskDownCommand { get; }

	/// <summary>
	///		Comando para mover una tarea a la primera posición
	/// </summary>
	public BaseCommand MoveTaskLastCommand { get; }

	/// <summary>
	///		Comando para borrar una tarea
	/// </summary>
	public BaseCommand DeleteTaskCommand { get; }
}
