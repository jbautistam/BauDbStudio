using System;

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
		ViewsController = mainController;
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		// no hace nada, simplemente implementa la interface
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
		if (!string.IsNullOrWhiteSpace(fileName) && fileName.EndsWith(ToDoFileExtension, StringComparison.CurrentCultureIgnoreCase))
		{
			Reader.ToDoFileViewModel viewModel = new(this, fileName);

				// Carga el archivo con la contraseña pasada como parámetro y si puede hacerlo, abre el documento
				if (viewModel.LoadFile())
					ViewsController.OpenWindow(viewModel);
				// e indica que ha podido abrir el archivo (para que no se abra ningún documento)
				return true;
		}
		else
			return false;
	}

	/// <summary>
	///		Muestra las notas
	/// </summary>
	public void ShowNotes()
	{
		if (NotesVisible)
			ViewsController.HideNotes();
		else
			foreach (Application.Models.Notes.NoteModel note in ToDoManager.NotesManager.Notes.Notes)
				ViewsController.OpenDialog(new Notes.NoteViewModel(this, note, false));
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
	///		Manager de la aplicación ToDo
	/// </summary>
	public Application.ToDoManager ToDoManager { get; } = new();

	/// <summary>
	///		Indica si las notas están visibles
	/// </summary>
	public bool NotesVisible { get; private set; }
}