namespace Bau.Libraries.ToDoManager.ViewModel;

/// <summary>
///		ViewModel principal del manager de listas de tareas
/// </summary>
public class ToDoManagerViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Constantes públicas
	public const string ToDoFileExtension = ".bau.todo";
	public const string PatternFileExtension = ".pattern";

	public ToDoManagerViewModel(Controllers.IToDoManagerController mainController)
	{
		// Inicializa los controladores
		ViewsController = mainController;
		// Inicializa los ViewModel hijos
		ConfigurationViewModel = new Configuration.ConfigurationViewModel(this);
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
		ToDoManager.NotesManager.Load(path);
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
					Reader.ToDoFileViewModel viewModel = new(this, fileName);

						// Carga el archivo
						if (viewModel.LoadFile())
							ViewsController.OpenWindow(viewModel);
						// e indica que ha podido abrir el archivo (para que no se abra ningún documento)
						open = true;
				}
				else if (fileName.EndsWith(PatternFileExtension, StringComparison.CurrentCultureIgnoreCase))
				{
					// Abre el archivo
					ViewsController.OpenWindow(new PatternsFile.PatternFileViewModel(this, fileName));
					// indica que ha podido abrir el archivo
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
			foreach (Application.Models.Notes.NoteModel note in ToDoManager.NotesManager.Notes.Notes)
				ViewsController.OpenDialog(new Notes.NoteViewModel(this, note, false));
		// Cambia el valor que indica si las notas están visibles
		NotesVisible = !NotesVisible;
	}

	/// <summary>
	///		Crea una nueva nota
	/// </summary>
	public void CreateNewNote()
	{
		ViewsController.OpenDialog(new Notes.NoteViewModel(this, new Application.Models.Notes.NoteModel(), true));
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