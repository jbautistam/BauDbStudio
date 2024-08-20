namespace Bau.Libraries.ToDoManager.ViewModel;

/// <summary>
///		ViewModel principal del manager de listas de tareas
/// </summary>
public class ToDoManagerViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Constantes públicas
	public const string ToDoFileExtension = ".bau.todo";

	public ToDoManagerViewModel(Controllers.IToDoManagerController mainController)
	{
		// Inicializa los controladores
		ViewsController = mainController;
		// Inicializa los ViewModel hijos
		ConfigurationViewModel = new Configuration.ConfigurationViewModel(this);
		CalendarViewModel = new Appointments.CalendarViewModel(this);
		TimeScheduleViewModel = new TimeManagement.TimeScheduleViewModel(this);
		// Inicializa los comandos
		CreateNewNoteCommand = new BauMvvm.ViewModels.BaseCommand(_ => CreateNewNote());
		ShowNotesCommand = new BauMvvm.ViewModels.BaseCommand(_ => ShowNotes());
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		ConfigurationViewModel.Initialize();
	}

	/// <summary>
	///		Carga el directorio
	/// </summary>
	public void Load(string path)
	{
		ToDoManager.Load(path);
		CalendarViewModel.Load();
		TimeScheduleViewModel.Load();
	}

	/// <summary>
	///		Abre un archivo de entradas
	/// </summary>
	public bool OpenFile(string fileName)
	{
		bool open = false;

			// Abre el archivo
			if (!string.IsNullOrWhiteSpace(fileName)) 
			{
				if (fileName.EndsWith(ToDoFileExtension, StringComparison.CurrentCultureIgnoreCase))
				{
					ToDo.ToDoFileViewModel viewModel = new(this, fileName);

						// Carga el archivo
						if (viewModel.LoadFile())
							ViewsController.OpenWindow(viewModel);
						// e indica que ha podido abrir el archivo (para que no se abra ningún documento)
						open = true;
				}
			}
			// Devuelve el valor que indica si se ha abierto
			return open;
	}

	/// <summary>
	///		Muestra las notas
	/// </summary>
	public void ShowNotes()
	{
		// Muestra / oculta las notas
		if (NotesVisible)
			ViewsController.HideNotes();
		else
			foreach (Application.Notes.Models.NoteModel note in ToDoManager.NotesManager.Notes.Notes)
				ViewsController.OpenDialog(new Notes.NoteViewModel(this, note, false));
		// Cambia el valor que indica si las notas están visibles
		NotesVisible = !NotesVisible;
	}

	/// <summary>
	///		Crea una nueva nota
	/// </summary>
	public void CreateNewNote()
	{
		ViewsController.OpenDialog(new Notes.NoteViewModel(this, new Application.Notes.Models.NoteModel(), true));
	}

	/// <summary>
	///		Cierra el ViewModel cuando se cierra la aplicación. Graba los archivos que tenga pendientes
	/// </summary>
	public bool Close()
	{
		// Cierra la tarea que esté activa y graba
		TimeScheduleViewModel.Close();
		// Devuelve el valor que indica que se ha cerrado correctamente
		return true;
	}

	/// <summary>
	///		Controlador de vistas de aplicación
	/// </summary>
	public Controllers.IToDoManagerController ViewsController { get; }
	
	/// <summary>
	///		ViewModel de la configuración
	/// </summary>
	public Configuration.ConfigurationViewModel ConfigurationViewModel { get; }

	/// <summary>
	///		ViewModel del calendario
	/// </summary>
	public Appointments.CalendarViewModel CalendarViewModel { get; }

	/// <summary>
	///		ViewModel del control de tiempo
	/// </summary>
	public TimeManagement.TimeScheduleViewModel TimeScheduleViewModel { get; }

	/// <summary>
	///		Manager de la aplicación ToDo
	/// </summary>
	public Application.ToDoManager ToDoManager { get; } = new();

	/// <summary>
	///		Indica si las notas están visibles
	/// </summary>
	public bool NotesVisible { get; private set; }

	/// <summary>
	///		Comando para crear nueva nota
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand CreateNewNoteCommand { get; }

	/// <summary>
	///		Comando para mostrar las notas
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand ShowNotesCommand { get; }
}