using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.ToDoManager.Application.ToDo.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.ToDo;

/// <summary>
///		ViewModel de un <see cref="TaskModel"/>
/// </summary>
public class ToDoTaskViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _name = default!, _description = default!, _notes = default!;
	private DateTime _createdAt = default!;
	private DateTime? _dueAt, _finishedAt;
	private ComboViewModel _comboStatus = default!, _comboPriorities = default!, _comboMoskow = default!;

	public ToDoTaskViewModel(ToDoFileViewModel toDoFileViewModel, TaskModel task, bool isNew)
	{
		ToDoFileViewModel = toDoFileViewModel;
		Task = task;
		IsNew = isNew;
		InitProperties();
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		// Carga los combos
		LoadComboStatus();
		LoadComboPriorities();
		LoadComboMoskow();
		// Asigna las propiedades
		Name = Task.Name;
		Description = Task.Description;
		Notes = Task.Notes;
		ComboStatus.SelectedId = (int) Task.Status;
		ComboPriorities.SelectedId = (int) Task.Priority;
		ComboMoskow.SelectedId = (int) Task.Moskow;
		CreatedAt = Task.CreatedAt;
		DueAt = Task.DueAt;
		FinishedAt = Task.FinishedAt;
	}

	/// <summary>
	///		Carga el combo de estados
	/// </summary>
	private void LoadComboStatus()
	{
		// Crea el combo
		ComboStatus = new ComboViewModel(this);
		// Añade los elementos
		ComboStatus.AddItem((int) TaskModel.StatusType.Planned, "Planned");
		ComboStatus.AddItem((int) TaskModel.StatusType.Doing, "Doing");
		ComboStatus.AddItem((int) TaskModel.StatusType.Done, "Done");
		// Selecciona el primer elemento
		ComboStatus.SelectedItem = ComboStatus.Items[0];
	}

	/// <summary>
	///		Carga el combo de prioridades
	/// </summary>
	private void LoadComboPriorities()
	{
		// Crea el combo
		ComboPriorities = new ComboViewModel(this);
		// Añade los elementos
		ComboPriorities.AddItem((int) TaskModel.PriorityType.Low, "Low");
		ComboPriorities.AddItem((int) TaskModel.PriorityType.Normal, "Normal");
		ComboPriorities.AddItem((int) TaskModel.PriorityType.High, "High");
		// Selecciona el primer elemento
		ComboPriorities.SelectedItem = ComboPriorities.Items[1];
	}

	/// <summary>
	///		Carga el combo de la metodología MoSCoW
	/// </summary>
	private void LoadComboMoskow()
	{
		// Crea el combo
		ComboMoskow = new ComboViewModel(this);
		// Añade los elementos
		ComboMoskow.AddItem((int) TaskModel.MoskowType.MustHave, "Must have");
		ComboMoskow.AddItem((int) TaskModel.MoskowType.ShouldHave, "Should have");
		ComboMoskow.AddItem((int) TaskModel.MoskowType.CouldHave, "Could have");
		ComboMoskow.AddItem((int) TaskModel.MoskowType.WontHave, "Won't have");
		// Selecciona el primer elemento
		ComboMoskow.SelectedItem = ComboMoskow.Items[2];
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Name))
				ToDoFileViewModel.MainViewModel.ViewsController.SystemController.ShowMessage("Enter the task name");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{
			// Asigna los datos a la tarea
			Task.Name = Name;
			Task.Description = Description;
			Task.Notes = Notes;
			Task.Status = (TaskModel.StatusType) (ComboStatus.SelectedId ?? 1);
			Task.Priority = (TaskModel.PriorityType) (ComboPriorities.SelectedId ?? 1);
			Task.Moskow = (TaskModel.MoskowType) (ComboMoskow.SelectedId ?? 1);
			Task.DueAt = DueAt;
			if (Task.Status == TaskModel.StatusType.Done)
				Task.FinishedAt = DateTime.Now;
			else
				Task.FinishedAt = null;
			// Indica que ya no es nuevo y está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
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
	///		Indica si es nuevo
	/// </summary>
	public bool IsNew { get; }

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
	///		Notas de la tarea
	/// </summary>
	public string Notes
	{
		get { return _notes; }
		set { CheckProperty(ref _notes, value); }
	}

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt
	{
		get { return _createdAt; }
		set { CheckProperty(ref _createdAt, value); }
	}

	/// <summary>
	///		Fecha planificada
	/// </summary>
	public DateTime? DueAt
	{
		get { return _dueAt; }
		set { CheckProperty(ref _dueAt, value); }
	}

	/// <summary>
	///		Fecha de finalización
	/// </summary>
	public DateTime? FinishedAt
	{
		get { return _finishedAt; }
		set { CheckProperty(ref _finishedAt, value); }
	}

	/// <summary>
	///		Combo de estados de la tarea
	/// </summary>
	public ComboViewModel ComboStatus
	{
		get { return _comboStatus; }
		set { CheckObject(ref _comboStatus, value); }
	}

	/// <summary>
	///		Combo de prioridades de la tarea
	/// </summary>
	public ComboViewModel ComboPriorities
	{
		get { return _comboPriorities; }
		set { CheckObject(ref _comboPriorities, value); }
	}

	/// <summary>
	///		Combo de tipos MoSCoW de la tarea
	/// </summary>
	public ComboViewModel ComboMoskow
	{
		get { return _comboMoskow; }
		set { CheckObject(ref _comboMoskow, value); }
	}
}
